using System.Drawing;
using System.Drawing.Imaging;

namespace UoFiddler.Plugin.ResourceAccess;

/// <summary>Reads one glyph from disk without initializing the global font tables.</summary>
internal static class FontReader
{
    public static object Read(string path, ResourceRequest request)
    {
        int codePoint = request.CodePoint ?? throw new ResourceException("invalid_request", "codePoint is required.");
        if (request.Font < 0 || request.Font > 19 || codePoint < 0 || codePoint > 0xFFFF)
        {
            throw new ResourceException("invalid_request", "Invalid font index or code point.");
        }
        using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        using var reader = new BinaryReader(stream);
        return request.Unicode ? ReadUnicode(reader, path, codePoint, request) : ReadAscii(reader, path, codePoint, request);
    }

    private static object ReadAscii(BinaryReader reader, string path, int codePoint, ResourceRequest request)
    {
        if (codePoint < 0x20 || codePoint > 0xFF)
        {
            throw new ResourceException("invalid_request", "ASCII font codePoint must be between 32 and 255.");
        }
        Stream stream = reader.BaseStream;
        for (int font = 0; font <= request.Font; font++)
        {
            RequireRemaining(stream, 1);
            byte header = reader.ReadByte();
            for (int character = 0x20; character <= 0xFF; character++)
            {
                RequireRemaining(stream, 3);
                long offset = stream.Position;
                int width = reader.ReadByte();
                int height = reader.ReadByte();
                byte unknown = reader.ReadByte();
                int length = width * height * 2;
                RequireRemaining(stream, length);
                if (font == request.Font && character == codePoint)
                {
                    using var image = width > 0 && height > 0 && request.IncludeImage ? new Bitmap(width, height) : null;
                    if (image != null)
                    {
                        for (int y = 0; y < height; y++)
                        for (int x = 0; x < width; x++)
                        {
                            ushort pixel = reader.ReadUInt16();
                            if (pixel != 0)
                            {
                                image.SetPixel(x, y, Color.FromArgb(255, ((pixel >> 10) & 31) * 255 / 31,
                                    ((pixel >> 5) & 31) * 255 / 31, (pixel & 31) * 255 / 31));
                            }
                        }
                    }
                    return new { source = path, view = "disk", request.Font, codePoint, header, width, height,
                        unknown, offset, length = length + 3, image = Encode(image) };
                }
                stream.Seek(length, SeekOrigin.Current);
            }
        }
        throw new ResourceException("not_found", "The requested font was not found.");
    }

    private static object ReadUnicode(BinaryReader reader, string path, int codePoint, ResourceRequest request)
    {
        Stream stream = reader.BaseStream;
        const int tableLength = 0x10000 * 4;
        RequireRemaining(stream, tableLength);
        stream.Position = codePoint * 4L;
        uint offset = reader.ReadUInt32();
        if (offset == 0 || offset == uint.MaxValue)
        {
            throw new ResourceException("not_found", "The Unicode glyph is absent.");
        }
        if (offset < tableLength || offset > stream.Length - 4)
        {
            throw new InvalidDataException("The Unicode glyph offset is outside the data section.");
        }
        stream.Position = offset;
        sbyte xOffset = reader.ReadSByte();
        sbyte yOffset = reader.ReadSByte();
        int width = reader.ReadByte();
        int height = reader.ReadByte();
        int rowBytes = (width + 7) / 8;
        int length = height * rowBytes;
        RequireRemaining(stream, length);
        byte[] pixels = reader.ReadBytes(length);
        using var image = width > 0 && height > 0 && request.IncludeImage ? new Bitmap(width, height) : null;
        if (image != null)
        {
            for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
            {
                if ((pixels[y * rowBytes + x / 8] & (1 << (7 - x % 8))) != 0)
                {
                    image.SetPixel(x, y, Color.Black);
                }
            }
        }
        return new { source = path, view = "disk", request.Font, codePoint, xOffset, yOffset, width, height,
            offset, length = length + 4, image = Encode(image) };
    }

    internal static void RequireRemaining(Stream stream, long length)
    {
        if (length < 0 || length > stream.Length - stream.Position)
        {
            throw new EndOfStreamException();
        }
    }

    internal static object? Encode(Bitmap? bitmap)
    {
        if (bitmap == null)
        {
            return null;
        }
        if ((long)bitmap.Width * bitmap.Height > 4 * 1024 * 1024)
        {
            throw new ResourceException("resource_too_large", "The image exceeds four million pixels.");
        }
        using var png = new MemoryStream();
        bitmap.Save(png, ImageFormat.Png);
        return new { mimeType = "image/png", width = bitmap.Width, height = bitmap.Height, base64 = Convert.ToBase64String(png.ToArray()) };
    }
}

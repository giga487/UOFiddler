using System.Globalization;
using System.Text.Json;
using Ultima;
using UoFiddler.Controls.Classes;

namespace UoFiddler.Plugin.ResourceAccess;

/// <summary>Called on Fiddler's UI thread so SDK caches and pending edits remain consistent.</summary>
internal sealed class ResourceService
{
    private StringList? _compressedCliloc;
    private string? _clilocPath;
    private long _clilocLength;
    private DateTime _clilocTime;
    private readonly Func<string?>? _questSource;

    public ResourceService(Func<string?>? questSource = null)
    {
        _questSource = questSource;
    }

    public static readonly string[] Commands = ["status", "files", "read", "item", "land", "gump", "font", "cliloc", "quest"];

    public object Execute(ResourceRequest request)
    {
        return request.Command switch
        {
            "status" => new { protocolVersion = 1, processId = Environment.ProcessId,
                profile = Options.ProfileName, root = Files.RootDir, commands = Commands,
                readOnly = true, maxReadBytes = Protocol.MaxReadBytes },
            "files" => FileCatalog().OrderBy(pair => pair.Key).Select(pair =>
            {
                var file = new FileInfo(pair.Value);
                return new { name = pair.Key, path = pair.Value, exists = file.Exists,
                    length = file.Exists ? (long?)file.Length : null,
                    lastWriteUtc = file.Exists ? (DateTime?)file.LastWriteTimeUtc : null };
            }).ToArray(),
            "read" => ReadRange(request),
            "item" or "land" or "gump" => ReadImage(request),
            "font" => FontReader.Read(Resolve(request.Unicode
                ? (request.Font == 0 ? "unifont.mul" : $"unifont{request.Font}.mul") : "fonts.mul"), request),
            "cliloc" => ReadCliloc(request),
            "quest" => ReadQuest(request),
            _ => throw new ResourceException("invalid_request", "Unknown command. Use status to list supported commands.")
        };
    }

    private Dictionary<string, string> FileCatalog()
    {
        var files = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        if (Files.MulPath != null)
        {
            foreach (var pair in Files.MulPath)
            {
                if (string.IsNullOrWhiteSpace(pair.Value))
                {
                    continue;
                }
                // Match Files.GetFilePath: a bare name is relative to RootDir;
                // a value containing a directory is resolved as supplied by the profile.
                string mapped = string.IsNullOrEmpty(Path.GetDirectoryName(pair.Value))
                    ? Path.Combine(Files.RootDir ?? AppContext.BaseDirectory, pair.Value) : pair.Value;
                files[pair.Key] = Path.GetFullPath(mapped);
            }
        }
        // SeaHats quest files are not part of the standard SDK file map.
        if (!string.IsNullOrEmpty(Files.RootDir))
        {
            string quest = Path.Combine(Files.RootDir, "Quests.Json");
            if (File.Exists(quest))
            {
                files.TryAdd("Quests.Json", Path.GetFullPath(quest));
            }
        }
        if (_questSource?.Invoke() is string selected)
        {
            files["Quests.Json"] = Path.GetFullPath(selected);
        }
        return files;
    }

    private string Resolve(string? name)
    {
        if (string.IsNullOrWhiteSpace(name) || !FileCatalog().TryGetValue(name, out string? path))
        {
            throw new ResourceException("not_found", "The file is not mapped by the active profile. Use files to list available names.");
        }
        if (!File.Exists(path))
        {
            throw new ResourceException("not_found", $"The mapped file '{name}' does not exist.");
        }
        return path;
    }

    private object ReadRange(ResourceRequest request)
    {
        if (request.Offset < 0 || request.Length < 1 || request.Length > Protocol.MaxReadBytes)
        {
            throw new ResourceException("invalid_request", "offset must be nonnegative and length must be between 1 and 1048576.");
        }
        string path = Resolve(request.File);
        using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        if (request.Offset > stream.Length || request.Length > stream.Length - request.Offset)
        {
            throw new ResourceException("invalid_request", "The requested byte range is outside the file.");
        }
        stream.Position = request.Offset;
        byte[] bytes = new byte[request.Length];
        stream.ReadExactly(bytes);
        return new { source = path, view = "disk", request.Offset, request.Length,
            hex = Convert.ToHexString(bytes), base64 = Convert.ToBase64String(bytes) };
    }

    private static int RequiredId(ResourceRequest request, int max)
    {
        if (request.Id is not int id || id < 0 || id > max)
        {
            throw new ResourceException("invalid_request", $"id is required and must be between 0 and {max}.");
        }
        return id;
    }

    private object ReadImage(ResourceRequest request)
    {
        // Validate before calling Art.GetStatic, which otherwise silently wraps/clamps IDs.
        int id = RequiredId(request, request.Command == "land" ? 0x3FFF : 0xFFFF);
        bool patched;
        bool modified;
        object? metadata = null;
        System.Drawing.Bitmap? bitmap;
        string? source;
        if (request.Command == "gump")
        {
            source = FindBackingFile("gumpartlegacymul.uop", "gumpart.mul");
            if (source == null || id >= Gumps.GetCount())
            {
                throw new ResourceException("not_found", "The gump is outside the active client's index.");
            }
            bitmap = Gumps.GetGump(id, out patched);
            modified = Gumps.IsModified(id);
        }
        else
        {
            source = FindBackingFile("artlegacymul.uop", "art.mul");
            if (source == null)
            {
                throw new ResourceException("not_found", "No art file is mapped by the active profile.");
            }
            if (request.Command == "item")
            {
                if (id > Art.GetMaxItemId())
                {
                    throw new ResourceException("not_found", "The item is outside the active client's index.");
                }
                bitmap = Art.GetStatic(id, out patched, false);
                modified = Art.IsStaticModified(id);
                if (TileData.ItemTable != null && id < TileData.ItemTable.Length)
                {
                    ItemData tile = TileData.ItemTable[id];
                    metadata = new { tile.Name, tile.Height, tile.Weight, tile.Animation, tile.Hue,
                        tile.Quality, tile.Quantity, flags = tile.Flags.ToString(), flagsValue = (ulong)tile.Flags };
                }
            }
            else
            {
                bitmap = Art.GetLand(id, out patched);
                modified = Art.IsLandModified(id);
                if (TileData.LandTable != null && id < TileData.LandTable.Length)
                {
                    LandData tile = TileData.LandTable[id];
                    metadata = new { tile.Name, tile.TextureId, flags = tile.Flags.ToString(), flagsValue = (ulong)tile.Flags };
                }
            }
        }
        if (bitmap == null)
        {
            throw new ResourceException("not_found", "The resource has no image or was removed in the current session.");
        }
        if (request.IncludeImage && (long)bitmap.Width * bitmap.Height > 4 * 1024 * 1024)
        {
            throw new ResourceException("resource_too_large", "The image exceeds four million pixels.");
        }
        // SDK bitmaps may belong to its edit/cache tables; they must never be disposed here.
        using var copy = request.IncludeImage ? new System.Drawing.Bitmap(bitmap) : null;
        return new { id, hexId = $"0x{id:X4}", kind = request.Command, source, view = "session",
            patched, modified, width = bitmap.Width, height = bitmap.Height, metadata,
            image = FontReader.Encode(copy) };
    }

    private string? FindBackingFile(string uop, string mul)
    {
        Dictionary<string, string> files = FileCatalog();
        if (files.TryGetValue(uop, out string? packed) && File.Exists(packed))
        {
            return packed;
        }
        return files.TryGetValue(mul, out string? legacy) && File.Exists(legacy) ? legacy : null;
    }

    private object ReadCliloc(ResourceRequest request)
    {
        int id = RequiredId(request, int.MaxValue);
        string language = request.Language;
        if (string.IsNullOrEmpty(language) || language.Length > 16 || language.Any(c => !char.IsAsciiLetterOrDigit(c)))
        {
            throw new ResourceException("invalid_request", "language must be a cliloc language name, such as enu or custom1.");
        }
        string path = Resolve($"cliloc.{language}");
        using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        if (stream.Length > Protocol.MaxDocumentBytes)
        {
            throw new ResourceException("resource_too_large", "Cliloc files over 64 MiB require byte-range reads.");
        }
        FontReader.RequireRemaining(stream, 6);
        using var reader = new BinaryReader(stream);
        byte[] header = reader.ReadBytes(6);
        // A Mythic length is XOR-obfuscated with 0x8E2C9A3D; its high bit
        // distinguishes it from the conventional uncompressed cliloc header.
        if ((header[3] & 0x80) != 0)
        {
            // Compression has no random-access index: decompress once, invalidate on file changes.
            var file = new FileInfo(path);
            if (_compressedCliloc == null || path != _clilocPath || file.Length != _clilocLength || file.LastWriteTimeUtc != _clilocTime)
            {
                uint expandedLength = Ultima.Helpers.MythicDecompress.PeekDecompressedLength(header);
                if (expandedLength > Protocol.MaxDocumentBytes)
                {
                    throw new ResourceException("resource_too_large", "The decompressed cliloc exceeds 64 MiB.");
                }
                _compressedCliloc = new StringList(language, path, true);
                _clilocPath = path;
                _clilocLength = file.Length;
                _clilocTime = file.LastWriteTimeUtc;
            }
            StringEntry? entry = _compressedCliloc.Entries.FirstOrDefault(entry => entry.Number == id);
            if (entry == null)
            {
                throw new ResourceException("not_found", "The cliloc ID is absent.");
            }
            return new { source = path, view = "disk", id, language, entry.Text, entry.Flag,
                compressed = true, warning = _compressedCliloc.LoadWarning };
        }
        while (stream.Position < stream.Length)
        {
            FontReader.RequireRemaining(stream, 7);
            long offset = stream.Position;
            int number = reader.ReadInt32();
            byte flag = reader.ReadByte();
            int length = reader.ReadUInt16();
            FontReader.RequireRemaining(stream, length);
            if (number == id)
            {
                string text = System.Text.Encoding.UTF8.GetString(reader.ReadBytes(length));
                return new { source = path, view = "disk", id, language, text, flag, offset,
                    length = length + 7, compressed = false };
            }
            stream.Seek(length, SeekOrigin.Current);
        }
        throw new ResourceException("not_found", "The cliloc ID is absent.");
    }

    private object ReadQuest(ResourceRequest request)
    {
        int id = RequiredId(request, ushort.MaxValue);
        string path = Resolve(request.File ?? "Quests.Json");
        if (!path.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
        {
            throw new ResourceException("invalid_request", "quest requires a JSON source file.");
        }
        using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        if (stream.Length > Protocol.MaxDocumentBytes)
        {
            throw new ResourceException("resource_too_large", "Quest files over 64 MiB require byte-range reads.");
        }
        // Parse JSON as data: never instantiate the $type values stored by the SeaHats editor.
        using JsonDocument document = ParseQuestDocument(stream);
        if (document.RootElement.ValueKind != JsonValueKind.Object ||
            !document.RootElement.TryGetProperty("Quests", out JsonElement quests) || quests.ValueKind != JsonValueKind.Object)
        {
            throw new InvalidDataException("A quest file must contain a Quests object.");
        }
        if (!quests.TryGetProperty(id.ToString(CultureInfo.InvariantCulture), out JsonElement quest) || quest.ValueKind != JsonValueKind.Object)
        {
            throw new ResourceException("not_found", "The quest ID is absent.");
        }
        // Return only the selected quest, not the root SecretKey.
        return new { source = path, view = "disk", id, quest = quest.Clone() };
    }

    private static JsonDocument ParseQuestDocument(Stream stream)
    {
        try
        {
            return JsonDocument.Parse(stream);
        }
        catch (JsonException ex)
        {
            throw new InvalidDataException("The quest JSON file is malformed.", ex);
        }
    }
}

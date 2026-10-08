using System.Buffers.Binary;
using System.Drawing;
using Ultima;
using Ultima.Uop;
using UoFiddler.Controls.Classes;

namespace UoFiddler.Plugin.ResourceAccess;

internal sealed partial class ResourceService
{
    private static string ImageKind(ResourceRequest request)
    {
        if (request.Kind is not ("item" or "land" or "gump"))
            throw new ResourceException("invalid_request", "kind must be item, land or gump.");
        return request.Kind;
    }

    private object ImportImage(ResourceRequest request)
    {
        string kind = ImageKind(request);
        int id = RequiredId(request, kind == "land" ? 0x3FFF : 0xFFFF);
        // Replacement only: validate the existing slot before decoding or mutating anything.
        _ = ReadImage(request with { Command = kind, IncludeImage = false });
        if (string.IsNullOrWhiteSpace(request.ImagePath) || !Path.IsPathFullyQualified(request.ImagePath))
            throw new ResourceException("invalid_request", "imagePath must be an absolute PNG path.");
        using var stream = new FileStream(request.ImagePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        if (stream.Length > 32 * 1024 * 1024)
            throw new ResourceException("resource_too_large", "PNG files are limited to 32 MiB.");
        Span<byte> header = stackalloc byte[24];
        stream.ReadExactly(header);
        if (!header[..8].SequenceEqual(new byte[] { 137, 80, 78, 71, 13, 10, 26, 10 }) ||
            BinaryPrimitives.ReadUInt32BigEndian(header[8..]) != 13 || !header.Slice(12, 4).SequenceEqual("IHDR"u8))
            throw new ResourceException("invalid_file", "A PNG image is required.");
        uint width = BinaryPrimitives.ReadUInt32BigEndian(header[16..]);
        uint height = BinaryPrimitives.ReadUInt32BigEndian(header[20..]);
        if (width == 0 || height == 0 || width > 32767 || height > 32767 || (ulong)width * height > 4 * 1024 * 1024)
            throw new ResourceException("resource_too_large", "Invalid dimensions or image exceeds four million pixels.");
        if (kind == "land" && (width != 44 || height != 44))
            throw new ResourceException("invalid_request", "Land images must be exactly 44 x 44 pixels.");
        stream.Position = 0;
        using var decoded = new Bitmap(stream);
        Bitmap converted = Utils.ToUoBitmap(decoded);
        try
        {
            if (kind == "item" && !Art.ValidateStaticSize(converted, out _))
                throw new ResourceException("resource_too_large", "Image exceeds the static art encoding limit.");
            switch (kind)
            {
                case "item": Art.ReplaceStatic(id, converted); break;
                case "land": Art.ReplaceLand(id, converted); break;
                case "gump": Gumps.ReplaceGump(id, converted); break;
            }
        }
        catch
        {
            converted.Dispose();
            throw;
        }
        // The SDK owns the converted bitmap from here on.
        Options.ChangedUltimaClass[kind == "gump" ? "Gumps" : "Art"] = true;
        switch (kind)
        {
            case "item": ControlEvents.FireItemChangeEvent(this, id); break;
            case "land": ControlEvents.FireLandTileChangeEvent(this, id); break;
            case "gump": ControlEvents.FireGumpChangeEvent(this, id); break;
        }
        return ReadImage(request with { Command = kind });
    }

    private object SaveImages(ResourceRequest request)
    {
        string kind = ImageKind(request);
        if (request.Id != null)
            throw new ResourceException("invalid_request", "save writes the whole art or gump collection; omit id.");
        ContainerFormat format = request.Format switch
        {
            "mul" => ContainerFormat.Mul,
            "uop" => ContainerFormat.Uop,
            _ => throw new ResourceException("invalid_request", "format must be mul or uop.")
        };
        string? requestedOutput = request.OutputDirectory ?? Options.OutputPath;
        if (string.IsNullOrWhiteSpace(requestedOutput) || !Path.IsPathFullyQualified(requestedOutput))
            throw new ResourceException("invalid_request", "Set an absolute outputDirectory or Fiddler output folder.");
        string output = Path.GetFullPath(requestedOutput);
        FileType type = kind == "gump" ? FileType.GumpartLegacyMul : FileType.ArtLegacyMul;
        var names = UopFileNames.For(type);
        if (FindBackingFile(names.Uop, names.Mul) == null)
            throw new ResourceException("not_found", "The active profile has no source for this image collection.");
        string[] fileNames = format == ContainerFormat.Mul ? [names.Mul, names.Idx] : [names.Uop];
        // Never overwrite a profile source while its indexed readers are open.
        var sources = FileCatalog().Values.ToHashSet(StringComparer.OrdinalIgnoreCase);
        foreach (string name in fileNames)
        {
            string destination = Path.Combine(output, name);
            if (sources.Contains(destination) || (!string.IsNullOrEmpty(Files.RootDir) &&
                string.Equals(Path.TrimEndingDirectorySeparator(output), Path.TrimEndingDirectorySeparator(Path.GetFullPath(Files.RootDir)), StringComparison.OrdinalIgnoreCase)))
                throw new ResourceException("invalid_request", "Save to a separate output folder, not the active client files.");
            if (Directory.Exists(destination))
                throw new ResourceException("invalid_request", "An output file name is occupied by a directory.");
        }
        // Reject junctions/symlinks so an output alias cannot overwrite a loaded source.
        for (DirectoryInfo? dir = new(output); dir != null; dir = dir.Parent)
            if (dir.Exists && (dir.Attributes & FileAttributes.ReparsePoint) != 0)
                throw new ResourceException("invalid_request", "Output folders must not traverse symbolic links or junctions.");
        foreach (string name in fileNames)
            if (File.Exists(Path.Combine(output, name)) && (File.GetAttributes(Path.Combine(output, name)) & FileAttributes.ReparsePoint) != 0)
                throw new ResourceException("invalid_request", "Output files must not be symbolic links.");

        Directory.CreateDirectory(output);
        string stage = Path.Combine(output, ".resource-save-" + Guid.NewGuid().ToString("N"));
        string? backup = null;
        var installed = new List<string>();
        var originals = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        try
        {
            ClientFileSaveResult result = ClientFileSaver.Save(type, stage, format,
                path => { if (kind == "gump") Gumps.Save(path, false); else Art.Save(path, false); });
            foreach (string name in fileNames)
            {
                if (!File.Exists(Path.Combine(output, name))) continue;
                backup ??= Path.Combine(output, "ResourceAccess-backups", DateTime.UtcNow.ToString("yyyyMMdd-HHmmss") + "-" + Guid.NewGuid().ToString("N"));
                Directory.CreateDirectory(backup);
                File.Copy(Path.Combine(output, name), Path.Combine(backup, name));
                originals.Add(name);
            }
            try
            {
                foreach (string name in fileNames)
                {
                    File.Move(Path.Combine(stage, name), Path.Combine(output, name), true);
                    installed.Add(name);
                }
            }
            catch
            {
                foreach (string name in installed)
                {
                    if (originals.Contains(name)) File.Copy(Path.Combine(backup!, name), Path.Combine(output, name), true);
                    else File.Delete(Path.Combine(output, name));
                }
                throw;
            }
            if (kind == "gump") Gumps.ClearModified();
            else { Art.ClearModified(); Art.Modified = false; }
            Options.ChangedUltimaClass[kind == "gump" ? "Gumps" : "Art"] = false;
            return new { outputDirectory = output, files = fileNames, backupDirectory = backup,
                format = request.Format, scope = kind == "gump" ? "all gumps" : "all items and land",
                warnings = result.Warnings };
        }
        catch (Exception ex) when (ex is not OutOfMemoryException)
        {
            throw new ResourceException("save_failed", $"Save failed; session edits are retained. Backup: {backup ?? "none"}. {ex.Message}");
        }
        finally
        {
            try { if (Directory.Exists(stage)) Directory.Delete(stage, true); }
            catch (IOException) { }
            catch (UnauthorizedAccessException) { }
        }
    }
}

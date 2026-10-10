using System.Security.Cryptography;
using System.Text.Json;
using System.Text.RegularExpressions;
using Ultima.Uop;

internal static class Program
{
    private sealed record FileRecord(string Path, long Bytes, string Sha256);
    private sealed record Conversion(string Source, string Mul, string? Idx, FileType Type, int MapIndex);

    [STAThread]
    private static int Main(string[] args)
    {
        if (args.Length != 4)
        {
            Console.Error.WriteLine("Usage: PrepareClientData <originals> <client-output> <server-output> <report-directory>");
            return 2;
        }

        try
        {
            Prepare(Path.GetFullPath(args[0]), Path.GetFullPath(args[1]), Path.GetFullPath(args[2]), Path.GetFullPath(args[3]));
            return 0;
        }
        catch (Exception exception)
        {
            Console.Error.WriteLine(exception);
            Console.Error.WriteLine("Preparation failed. Any partial output is retained; source files were opened only for reading.");
            return 1;
        }
    }

    private static void Prepare(string source, string client, string server, string report)
    {
        if (!Directory.Exists(source))
        {
            throw new DirectoryNotFoundException(source);
        }

        ValidateDestination(source, client);
        ValidateDestination(source, server);
        if (Contains(client, server) || Contains(server, client))
        {
            throw new ArgumentException("Client and server destinations must be separate folders.");
        }

        var conversions = new List<Conversion>();
        foreach (var type in new[] { FileType.ArtLegacyMul, FileType.GumpartLegacyMul, FileType.SoundLegacyMul, FileType.MultiCollection })
        {
            var names = UopFileNames.For(type);
            if (!File.Exists(Path.Combine(source, names.Uop)))
            {
                throw new FileNotFoundException("Original UOP required for conversion.", names.Uop);
            }
            conversions.Add(new Conversion(names.Uop, names.Mul, names.Idx, type, 0));
        }
        foreach (var file in Directory.GetFiles(source, "*.uop").Order(StringComparer.OrdinalIgnoreCase))
        {
            var match = Regex.Match(Path.GetFileName(file), @"^map(\d+)(x?)LegacyMUL\.uop$", RegexOptions.IgnoreCase);
            if (match.Success)
            {
                conversions.Add(new Conversion(Path.GetFileName(file), $"map{match.Groups[1].Value}{match.Groups[2].Value}.mul",
                    null, FileType.MapLegacyMul, int.Parse(match.Groups[1].Value)));
            }
        }
        foreach (var map in Enumerable.Range(0, 6))
        {
            if (!conversions.Any(entry => entry.Mul == $"map{map}.mul"))
            {
                throw new FileNotFoundException($"Original map{map}LegacyMUL.uop is missing.");
            }
        }

        var converted = conversions.Select(entry => entry.Source).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var replaced = conversions.SelectMany(entry => entry.Idx == null ? new[] { entry.Mul } : new[] { entry.Mul, entry.Idx })
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        var extensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            ".mul", ".idx", ".uop", ".def", ".txt", ".cfg", ".rle", ".bin"
        };
        var originals = Directory.GetFiles(source)
            .Where(file => extensions.Contains(Path.GetExtension(file)) || Path.GetFileName(file).StartsWith("cliloc.", StringComparison.OrdinalIgnoreCase)
                || Path.GetFileName(file).Equals("client.exe", StringComparison.OrdinalIgnoreCase))
            .Concat(Directory.Exists(Path.Combine(source, "Music")) ? Directory.GetFiles(Path.Combine(source, "Music"), "*", SearchOption.AllDirectories) : [])
            .Order(StringComparer.OrdinalIgnoreCase).ToArray();

        Console.WriteLine($"Hashing {originals.Length} original resource files.");
        var sourceRecords = originals.Select(file => Record(source, file)).ToArray();
        Directory.CreateDirectory(client);
        Directory.CreateDirectory(server);
        foreach (var file in originals)
        {
            var name = Path.GetFileName(file);
            if (converted.Contains(name) || replaced.Contains(name))
            {
                continue;
            }
            Copy(file, Path.Combine(client, Path.GetRelativePath(source, file)));
        }

        var converter = new LegacyMulFileConverter();
        foreach (var conversion in conversions)
        {
            Console.WriteLine($"UOFiddler UOP Packer: {conversion.Source} -> {conversion.Mul}");
            converter.FromUop(Path.Combine(source, conversion.Source), Path.Combine(client, conversion.Mul),
                conversion.Idx == null ? null : Path.Combine(client, conversion.Idx), conversion.Type, conversion.MapIndex,
                conversion.Type == FileType.MultiCollection ? Path.Combine(client, "housing.bin") : "");
            if (conversion.Idx != null)
            {
                ValidateIndex(Path.Combine(client, conversion.Mul), Path.Combine(client, conversion.Idx));
            }
            else
            {
                var length = new FileInfo(Path.Combine(client, conversion.Mul)).Length;
                long[] expected = [89915392, 89915392, 11289600, 16056320, 6421156, 16056320];
                if (length == 0 || length % 196 != 0 || conversion.MapIndex < expected.Length && length != expected[conversion.MapIndex])
                {
                    throw new InvalidDataException($"Unexpected terrain size for {conversion.Mul}: {length}.");
                }
            }
        }

        foreach (var map in Enumerable.Range(0, 6))
        {
            ValidateIndex(Path.Combine(client, $"statics{map}.mul"), Path.Combine(client, $"staidx{map}.mul"));
        }
        foreach (var file in Directory.GetFiles(client, "*", SearchOption.AllDirectories))
        {
            Copy(file, Path.Combine(server, Path.GetRelativePath(client, file)));
        }

        Console.WriteLine("Verifying both sets and checking that original resources are unchanged.");
        var clientRecords = Directory.GetFiles(client, "*", SearchOption.AllDirectories).Select(file => Record(client, file))
            .OrderBy(file => file.Path, StringComparer.OrdinalIgnoreCase).ToArray();
        var serverRecords = Directory.GetFiles(server, "*", SearchOption.AllDirectories).Select(file => Record(server, file))
            .OrderBy(file => file.Path, StringComparer.OrdinalIgnoreCase).ToArray();
        if (!clientRecords.SequenceEqual(serverRecords))
        {
            throw new InvalidDataException("Client and server sets differ.");
        }
        foreach (var original in sourceRecords)
        {
            if (original != Record(source, Path.Combine(source, original.Path)))
            {
                throw new InvalidDataException($"Original source changed during preparation: {original.Path}.");
            }
        }

        Directory.CreateDirectory(report);
        var manifest = new
        {
            createdUtc = DateTime.UtcNow, source, client, server,
            converter = "UOFiddler Ultima.Uop.LegacyMulFileConverter (UOP Packer)",
            fileCountPerSet = clientRecords.Length, bytesPerSet = clientRecords.Sum(file => file.Bytes),
            identicalSets = true, originalResourcesUnchanged = true,
            conversions, originals = sourceRecords, outputs = clientRecords,
            pendingCustomResources = new[] { "Quests.MUL", "SeaHats facet 6: map6.mul, staidx6.mul, statics6.mul", "SeaHats custom fonts and graphics" }
        };
        File.WriteAllText(Path.Combine(report, "manifest.json"), JsonSerializer.Serialize(manifest, new JsonSerializerOptions { WriteIndented = true }));
        File.WriteAllText(Path.Combine(report, "README.md"), $"""
            # Base originale SeaHats

            Sorgente: `{source}`.
            Client: `{client}`.
            Server: `{server}`.

            Creati {clientRecords.Length} file per set, byte per set: {clientRecords.Sum(file => file.Bytes)}.
            Verifica SHA-256: set identici e tutte le risorse originali utilizzate invariate.
            I MUL di art, gump, suoni, multi e mappe sono ottenuti con il convertitore nativo UOP Packer di UOFiddler.
            Gli UOP convertiti non sono copiati nei set, affinché non prevalgano sui MUL modificati.
            Le animazioni moderne AnimationFrame/AnimationSequence e MainMisc restano in UOP; le coppie anim*.mul/idx sono conservate.
            Le risorse comuni, gli statici, i font, le localizzazioni e la musica sono copiati dagli originali.
            housing.bin e multi-components.txt conservano le informazioni necessarie per riconvertire i multi in UOP.

            Aprire in UOFiddler una delle due cartelle con Path Settings, poi Reload Files.
            Quando si modificano risorse condivise (mappe, statici, tiledata, multi), mantenere le due copie sincronizzate.
            I profili e le configurazioni di esecuzione non sono stati modificati.
            Questi sono set di risorse, non pacchetti completi degli eseguibili del server o del client.
            Quest cifrate SeaHats, facet speciale 6 e personalizzazioni non fanno parte degli originali e vanno aggiunte separatamente.
            Non è stato collaudato l'avvio del gioco con questi set.
            """);
        Console.WriteLine($"READY: {clientRecords.Length} files per set, {clientRecords.Sum(file => file.Bytes)} bytes per set; SHA256 identical; originals unchanged.");
        Console.WriteLine($"Manifest: {Path.Combine(report, "manifest.json")}");
    }

    private static void ValidateDestination(string source, string destination)
    {
        if (Contains(source, destination) || Contains(destination, source))
        {
            throw new ArgumentException("Output must be separate from the original installation.");
        }
        if (File.Exists(destination) || Directory.Exists(destination))
        {
            throw new IOException($"Destination already exists; refusing to overwrite: {destination}");
        }
        for (var parent = Directory.GetParent(destination); parent != null; parent = parent.Parent)
        {
            if (parent.Exists && parent.Attributes.HasFlag(FileAttributes.ReparsePoint))
            {
                throw new IOException("Output cannot be inside a symbolic link or junction.");
            }
        }
    }

    private static bool Contains(string parent, string child) => parent.Equals(child, StringComparison.OrdinalIgnoreCase)
        || child.StartsWith(Path.TrimEndingDirectorySeparator(parent) + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase);

    private static FileRecord Record(string root, string file)
    {
        using var stream = File.OpenRead(file);
        return new FileRecord(Path.GetRelativePath(root, file), stream.Length, Convert.ToHexString(SHA256.HashData(stream)));
    }

    private static void Copy(string source, string output)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(output)!);
        File.Copy(source, output, false);
    }

    private static void ValidateIndex(string mul, string idx)
    {
        var length = new FileInfo(mul).Length;
        using var reader = new BinaryReader(File.OpenRead(idx));
        if (reader.BaseStream.Length == 0 || reader.BaseStream.Length % 12 != 0)
        {
            throw new InvalidDataException($"Invalid index length: {idx}");
        }
        while (reader.BaseStream.Position < reader.BaseStream.Length)
        {
            var offset = reader.ReadInt32();
            var size = reader.ReadInt32();
            reader.ReadInt32();
            if (offset < 0)
            {
                continue;
            }
            if (size < 0 || (long)offset + size > length)
            {
                throw new InvalidDataException($"Index points outside its MUL: {idx}");
            }
        }
    }
}

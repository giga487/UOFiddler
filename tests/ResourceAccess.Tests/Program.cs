using System.Diagnostics;
using System.Drawing;
using System.IO.Pipes;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Ultima;
using UoFiddler.Controls.Classes;
using UoFiddler.Plugin.ResourceAccess;

internal static class Program
{
    private static int _assertions;

    [STAThread]
    private static int Main(string[] args)
    {
        string fixture = Path.Combine(Path.GetTempPath(), "fiddler-resources-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(fixture);
        try
        {
            CreateFixture(fixture);
            Files.RootDir = fixture;
            Files.MulPath = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (string path in Directory.GetFiles(fixture))
            {
                Files.MulPath[Path.GetFileName(path)] = path;
            }
            Files.MulPath["verdata.mul"] = "";
            Files.MulPath["artlegacymul.uop"] = "";
            Files.MulPath["gumpartlegacymul.uop"] = "";
            Options.ProfileName = "test-profile";
            Dictionary<string, string> hashes = Directory.GetFiles(fixture).ToDictionary(path => path, Hash);
            var service = new ResourceService();
            TestService(service, fixture);
            TestUop(service, fixture);
            Console.WriteLine("Resource reader checks passed.");
            SynchronizationContext.SetSynchronizationContext(null);
            TestPipeAsync(service).GetAwaiter().GetResult();
            Console.WriteLine("Pipe checks passed.");
            if (args.Length > 0)
            {
                TestPowerShellClientAsync(service, args[0], fixture).GetAwaiter().GetResult();
            }
            TestPluginLifecycle();
            TestUiBridge();
            foreach (var pair in hashes)
            {
                Check(Hash(pair.Key) == pair.Value, "source file unchanged: " + Path.GetFileName(pair.Key));
            }
            Console.WriteLine($"PASS: {_assertions} assertions. Fixture: {fixture}");
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex);
            Console.Error.WriteLine("Fixture retained: " + fixture);
            return 1;
        }
    }

    private static string Hash(string path) => Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(path)));

    private static JsonElement Json(object data) => JsonSerializer.SerializeToElement(data, Protocol.JsonOptions);

    private static void Check(bool condition, string description)
    {
        if (!condition)
        {
            throw new Exception("FAIL: " + description);
        }
        _assertions++;
        if (description.Contains("pipe", StringComparison.OrdinalIgnoreCase) || description.Contains("recovery") || description.Contains("property") || description.Contains("bounded"))
        {
            Console.WriteLine("PASS: " + description);
        }
    }

    private static void Error(Action action, string code)
    {
        try
        {
            action();
            throw new Exception("Expected error: " + code);
        }
        catch (Exception ex) when (ex is ResourceException or InvalidDataException or EndOfStreamException or JsonException)
        {
            Check(Protocol.Failure(ex).Error?.Code == code, code);
        }
    }

    private static void TestService(ResourceService service, string fixture)
    {
        Check(Json(service.Execute(new())).GetProperty("profile").GetString() == "test-profile", "active profile");
        Check(Json(service.Execute(new() { Command = "files" })).GetArrayLength() >= 8, "file catalog");
        var range = Json(service.Execute(new() { Command = "read", File = "ARTIDX.MUL", Length = 12 }));
        Check(range.GetProperty("hex").GetString() == "FFFFFFFFFFFFFFFF00000000", "exact case-insensitive byte range");
        Files.MulPath["relative-index"] = Path.GetRelativePath(Environment.CurrentDirectory, Path.Combine(fixture, "artidx.mul"));
        Check(Json(service.Execute(new() { Command = "read", File = "relative-index", Length = 12 })).GetProperty("hex").GetString() == "FFFFFFFFFFFFFFFF00000000", "relative mapped directory matches SDK resolution");
        Files.MulPath.Remove("relative-index");
        Error(() => service.Execute(new() { Command = "read", File = "../artidx.mul" }), "not_found");
        Error(() => service.Execute(new() { Command = "read", File = "artidx.mul", Offset = -1 }), "invalid_request");
        Error(() => service.Execute(new() { Command = "read", File = "artidx.mul", Offset = long.MaxValue }), "invalid_request");
        Error(() => service.Execute(new() { Command = "read", File = "artidx.mul", Length = 1048577 }), "invalid_request");
        Error(() => service.Execute(new() { Command = "write" }), "invalid_request");

        var item = Json(service.Execute(new() { Command = "item", Id = 1 }));
        Check(item.GetProperty("width").GetInt32() == 2, "static art dimensions");
        Check(item.GetProperty("metadata").GetProperty("name").GetString() == "test item", "item tile data");
        Check(Pixel(item, 0, 0).R > 240, "static art PNG pixel");
        var land = Json(service.Execute(new() { Command = "land", Id = 2 }));
        Check(land.GetProperty("width").GetInt32() == 44 && Pixel(land, 21, 0).B > 240, "land diamond PNG");
        var gump = Json(service.Execute(new() { Command = "gump", Id = 4 }));
        Check(gump.GetProperty("width").GetInt32() == 2 && Pixel(gump, 0, 0).R > 240, "gump PNG");
        Error(() => service.Execute(new() { Command = "item", Id = -1 }), "invalid_request");
        Error(() => service.Execute(new() { Command = "item", Id = 0x10000 }), "invalid_request");
        Error(() => service.Execute(new() { Command = "item", Id = 5 }), "not_found");
        var withoutImage = Json(service.Execute(new() { Command = "item", Id = 1, IncludeImage = false }));
        Check(!withoutImage.TryGetProperty("image", out _), "image can be omitted");

        var replacement = new Bitmap(3, 1);
        replacement.SetPixel(0, 0, Color.Green);
        Art.ReplaceStatic(1, replacement);
        var edited = Json(service.Execute(new() { Command = "item", Id = 1 }));
        Check(edited.GetProperty("modified").GetBoolean() && edited.GetProperty("width").GetInt32() == 3, "session edits are visible");

        var glyph = Json(service.Execute(new() { Command = "font", Font = 1, CodePoint = 65 }));
        Check(glyph.GetProperty("width").GetInt32() == 2 && glyph.GetProperty("offset").GetInt64() > 673, "seek glyph in second font");
        Check(Pixel(glyph, 0, 0).R == 255 && Pixel(glyph, 1, 0).A == 0, "ASCII glyph pixels and transparency");
        var unicode = Json(service.Execute(new() { Command = "font", Unicode = true, CodePoint = 65 }));
        Check(unicode.GetProperty("xOffset").GetInt32() == -1 && unicode.GetProperty("width").GetInt32() == 9, "Unicode offsets and width");
        Check(Pixel(unicode, 8, 0).A == 255 && Pixel(unicode, 1, 0).A == 0, "Unicode row bit alignment");
        Error(() => service.Execute(new() { Command = "font", CodePoint = 0 }), "invalid_request");
        Error(() => FontReader.Read(Path.Combine(fixture, "truncated.mul"), new() { CodePoint = 65 }), "invalid_file");
        Error(() => service.Execute(new() { Command = "font", Unicode = true, CodePoint = 66 }), "not_found");

        var cliloc = Json(service.Execute(new() { Command = "cliloc", Id = 100002 }));
        Check(cliloc.GetProperty("text").GetString() == "Città ✓", "UTF-8 cliloc lookup");
        var compressed = Json(service.Execute(new() { Command = "cliloc", Language = "deu", Id = 100002 }));
        Check(compressed.GetProperty("text").GetString() == "Città ✓" && compressed.GetProperty("compressed").GetBoolean(), "compressed cliloc lookup");
        Check(Json(service.Execute(new() { Command = "cliloc", Language = "deu", Id = 100001 })).GetProperty("text").GetString() == "First", "cached compressed cliloc");
        Error(() => service.Execute(new() { Command = "cliloc", Language = "custom1", Id = 100001 }), "resource_too_large");
        Error(() => service.Execute(new() { Command = "cliloc", Id = 12 }), "not_found");
        Error(() => service.Execute(new() { Command = "cliloc", Id = 1, Language = "../enu" }), "invalid_request");
        var quest = Json(service.Execute(new() { Command = "quest", Id = 42 }));
        Check(quest.GetProperty("quest").GetProperty("QuestName").GetString() == "Fixture quest", "single quest lookup");
        Check(!quest.GetRawText().Contains("do-not-export"), "root secret is excluded");
        Check(quest.GetProperty("quest").GetProperty("$type").GetString() == "Unknown.Type, Unknown.Assembly", "JSON type tags remain data");
        Error(() => service.Execute(new() { Command = "quest", Id = 43 }), "not_found");
        Error(() => service.Execute(new() { Command = "quest", Id = 42, File = "invalid.json" }), "invalid_file");
        Error(() => service.Execute(new() { Command = "quest", Id = 42, File = "malformed.json" }), "invalid_file");
        var customQuest = new ResourceService(() => Path.Combine(fixture, "selected.json"));
        Check(Json(customQuest.Execute(new() { Command = "quest", Id = 7 })).GetProperty("id").GetInt32() == 7, "selected quest source");
    }

    private static Color Pixel(JsonElement data, int x, int y)
    {
        byte[] bytes = Convert.FromBase64String(data.GetProperty("image").GetProperty("base64").GetString()!);
        using var stream = new MemoryStream(bytes);
        using var bitmap = new Bitmap(stream);
        return bitmap.GetPixel(x, y);
    }

    private static void TestPluginLifecycle()
    {
        var plugin = new ResourceAccessPlugin();
        plugin.Initialize();
        using var tabs = new TabControl();
        using var tools = new ToolStripDropDownButton();
        plugin.ModifyTabPages(tabs);
        plugin.ModifyPluginToolStrip(tools);
        Check(tabs.TabPages.Count == 1 && tools.DropDownItems.Count == 1, "native plugin tab and menu");
        plugin.Unload();
        plugin.Unload();
        Check(tabs.TabPages.Count == 0, "plugin unload releases the tab");
    }

    private static void TestUiBridge()
    {
        using var control = new ResourceAccessControl();
        using var context = new ApplicationContext();
        control.StartConnection();
        Task driver = Task.Run(async () =>
        {
            try
            {
                string pipe = $"UOFiddler.ResourceAccess.{Environment.ProcessId}";
                JsonElement status = await QueryAsync(pipe, "{\"command\":\"status\"}");
                Check(status.GetProperty("data").GetProperty("profile").GetString() == "test-profile", "requests marshalled to Fiddler UI thread");
                Check((await QueryAsync(pipe, "{\"command\":\"item\",\"id\":1}")).GetProperty("ok").GetBoolean(), "image request through native control");
            }
            finally
            {
                control.BeginInvoke(new Action(() =>
                {
                    control.StopConnection();
                    context.ExitThread();
                }));
            }
        });
        // Exercise native UI dispatch without displaying a window.
        Application.Run(context);
        driver.GetAwaiter().GetResult();
    }

    private static void TestUop(ResourceService service, string fixture)
    {
        Files.MulPath["artlegacymul.uop"] = Path.Combine(fixture, "artlegacymul.uop");
        Files.MulPath["gumpartlegacymul.uop"] = Path.Combine(fixture, "gumpartlegacymul.uop");
        Art.Reload();
        Gumps.Reload();
        var item = Json(service.Execute(new() { Command = "item", Id = 1 }));
        Check(item.GetProperty("source").GetString()!.EndsWith(".uop") && Pixel(item, 0, 0).R > 240, "UOP static art");
        var gump = Json(service.Execute(new() { Command = "gump", Id = 4 }));
        Check(gump.GetProperty("source").GetString()!.EndsWith(".uop") && Pixel(gump, 0, 0).R > 240, "UOP gump");
    }

    private static async Task<JsonElement> QueryAsync(string pipeName, string request)
    {
        using var cancellation = new CancellationTokenSource(TimeSpan.FromSeconds(10));
        using var pipe = new NamedPipeClientStream(".", pipeName, PipeDirection.InOut, PipeOptions.Asynchronous);
        await pipe.ConnectAsync(cancellation.Token);
        using var writer = new StreamWriter(pipe, new UTF8Encoding(false), leaveOpen: true) { AutoFlush = true };
        using var reader = new StreamReader(pipe, Encoding.UTF8, leaveOpen: true);
        await writer.WriteLineAsync(request.AsMemory(), cancellation.Token);
        string response = await reader.ReadLineAsync(cancellation.Token) ?? throw new Exception("Missing pipe response.");
        return JsonDocument.Parse(response).RootElement.Clone();
    }

    private static async Task TestPipeAsync(ResourceService service)
    {
        string name = "Fiddler.Test." + Guid.NewGuid().ToString("N");
        using var server = new ResourcePipeServer(name, (request, _) => Task.FromResult(service.Execute(request)));
        server.Start();
        Check((await QueryAsync(name, "{\"command\":\"status\"}")).GetProperty("ok").GetBoolean(), "pipe round trip");
        Console.WriteLine("Pipe round trip passed.");
        Check((await QueryAsync(name, "not-json")).GetProperty("error").GetProperty("code").GetString() == "invalid_request", "malformed JSON recovery");
        Check((await QueryAsync(name, "{\"command\":\"status\",\"unsupported\":true}")).GetProperty("error").GetProperty("code").GetString() == "invalid_request", "unknown property rejected");
        Check((await QueryAsync(name, new string('x', Protocol.MaxRequestBytes + 1))).GetProperty("error").GetProperty("code").GetString() == "request_too_large", "bounded request");
        Check((await QueryAsync(name, "{\"command\":\"item\",\"id\":-1}")).GetProperty("error").GetProperty("code").GetString() == "invalid_request", "service errors over pipe");
        Check((await QueryAsync(name, "{\"command\":\"font\",\"font\":1,\"codePoint\":65}")).GetProperty("data").GetProperty("image").GetProperty("mimeType").GetString() == "image/png", "PNG over pipe after errors");
        using (var disconnected = new NamedPipeClientStream(".", name, PipeDirection.InOut, PipeOptions.Asynchronous))
        {
            using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(10));
            await disconnected.ConnectAsync(timeout.Token);
        }
        Check((await QueryAsync(name, "{\"command\":\"status\"}")).GetProperty("ok").GetBoolean(), "disconnected client recovery");
        server.Dispose();
        await server.Completion.WaitAsync(TimeSpan.FromSeconds(5));
        Check(server.Completion.IsCompletedSuccessfully, "shutdown while waiting for a client");
        using var restarted = new ResourcePipeServer(name, (request, _) => Task.FromResult(service.Execute(request)));
        restarted.Start();
        Check((await QueryAsync(name, "{}")).GetProperty("ok").GetBoolean(), "restart on same pipe");
        using var idle = new NamedPipeClientStream(".", name, PipeDirection.InOut, PipeOptions.Asynchronous);
        using var idleTimeout = new CancellationTokenSource(TimeSpan.FromSeconds(10));
        await idle.ConnectAsync(idleTimeout.Token);
        restarted.Dispose();
        await restarted.Completion.WaitAsync(TimeSpan.FromSeconds(5));
        Check(restarted.Completion.IsCompletedSuccessfully, "shutdown with idle client");
    }

    private static async Task TestPowerShellClientAsync(ResourceService service, string script, string fixture)
    {
        string name = "Fiddler.Test." + Guid.NewGuid().ToString("N");
        using var server = new ResourcePipeServer(name, (request, _) => Task.FromResult(service.Execute(request)));
        server.Start();
        string image = Path.Combine(fixture, "export.png");
        string bytes = Path.Combine(fixture, "export.bin");
        foreach (string command in new[] { "font", "read" })
        {
            var start = new ProcessStartInfo("pwsh") { UseShellExecute = false, CreateNoWindow = true, RedirectStandardOutput = true, RedirectStandardError = true };
            foreach (string argument in new[] { "-NoProfile", "-File", Path.GetFullPath(script), "-PipeName", name, "-Command", command })
            {
                start.ArgumentList.Add(argument);
            }
            string[] parameters = command == "font" ? ["-Font", "1", "-CodePoint", "65", "-ImagePath", image]
                : ["-File", "artidx.mul", "-Length", "12", "-BytesPath", bytes];
            foreach (string parameter in parameters) start.ArgumentList.Add(parameter);
            using Process process = Process.Start(start)!;
            Task<string> output = process.StandardOutput.ReadToEndAsync();
            Task<string> error = process.StandardError.ReadToEndAsync();
            await process.WaitForExitAsync().WaitAsync(TimeSpan.FromSeconds(45));
            Check(process.ExitCode == 0, "PowerShell " + command + ": " + await error + await output);
        }
        using var exported = new Bitmap(image);
        Check(exported.Width == 2 && exported.GetPixel(0, 0).R == 255, "PowerShell PNG export");
        Check(File.ReadAllBytes(bytes).SequenceEqual(File.ReadAllBytes(Path.Combine(fixture, "artidx.mul")).Take(12)), "PowerShell binary export");
    }

    private static void CreateFixture(string root)
    {
        using (var writer = new BinaryWriter(File.Create(Path.Combine(root, "fonts.mul"))))
        {
            for (int font = 0; font < 2; font++)
            {
                writer.Write((byte)(font + 1));
                for (int character = 32; character <= 255; character++)
                {
                    bool selected = font == 1 && character == 65;
                    writer.Write((byte)(selected ? 2 : 0)); writer.Write((byte)(selected ? 1 : 0)); writer.Write((byte)0);
                    if (selected) { writer.Write((ushort)0x7C00); writer.Write((ushort)0); }
                }
            }
        }
        using (var writer = new BinaryWriter(File.Create(Path.Combine(root, "unifont.mul"))))
        {
            writer.BaseStream.SetLength(0x10000 * 4);
            writer.BaseStream.Position = 65 * 4;
            writer.Write(0x10000 * 4);
            writer.BaseStream.Position = writer.BaseStream.Length;
            writer.Write((sbyte)-1); writer.Write((sbyte)2); writer.Write((byte)9); writer.Write((byte)1);
            writer.Write((byte)0x80); writer.Write((byte)0x80);
        }
        using (var writer = new BinaryWriter(File.Create(Path.Combine(root, "cliloc.enu"))))
        {
            writer.Write(0); writer.Write((short)0);
            foreach ((int id, string text) in new[] { (100001, "First"), (100002, "Città ✓") })
            {
                byte[] body = Encoding.UTF8.GetBytes(text);
                writer.Write(id); writer.Write((byte)0); writer.Write((ushort)body.Length); writer.Write(body);
            }
        }
        File.WriteAllText(Path.Combine(root, "Quests.Json"), "{\"SecretKey\":\"do-not-export\",\"Quests\":{\"42\":{\"ID\":42,\"QuestName\":\"Fixture quest\",\"$type\":\"Unknown.Type, Unknown.Assembly\"}}}");
        File.WriteAllText(Path.Combine(root, "selected.json"), "{\"Quests\":{\"7\":{\"ID\":7}}}");
        File.WriteAllText(Path.Combine(root, "invalid.json"), "null");
        File.WriteAllText(Path.Combine(root, "malformed.json"), "{invalid");
        File.WriteAllBytes(Path.Combine(root, "truncated.mul"), [1, 2]);
        byte[] cliloc = File.ReadAllBytes(Path.Combine(root, "cliloc.enu"));
        using (var writer = new BinaryWriter(File.Create(Path.Combine(root, "cliloc.deu"))))
        {
            writer.Write((uint)cliloc.Length ^ 0x8E2C9A3D);
            writer.Write(Ultima.Helpers.MythicDecompress.Transform(cliloc));
        }
        using (var writer = new BinaryWriter(File.Create(Path.Combine(root, "cliloc.custom1"))))
        {
            writer.Write((uint)(Protocol.MaxDocumentBytes + 1) ^ 0x8E2C9A3D);
            writer.Write(0);
        }
        using (var writer = new BinaryWriter(File.Create(Path.Combine(root, "art.mul"))))
        {
            for (int i = 0; i < 1012; i++) writer.Write((ushort)0x001F);
            writer.Write(0); writer.Write((ushort)2); writer.Write((ushort)1); writer.Write((ushort)0);
            writer.Write((ushort)0); writer.Write((ushort)2); writer.Write((ushort)0x7C00); writer.Write((ushort)0x03E0);
            writer.Write((ushort)0); writer.Write((ushort)0);
        }
        CreateIndex(Path.Combine(root, "artidx.mul"), 0x8000, [(2, 0, 2024, 0), (0x4001, 2024, 22, 0)]);
        using (var writer = new BinaryWriter(File.Create(Path.Combine(root, "gumpart.mul"))))
        {
            writer.Write(1); writer.Write((ushort)0x7C00); writer.Write((ushort)2);
        }
        CreateIndex(Path.Combine(root, "gumpidx.mul"), 256, [(4, 0, 8, 0x00020001)]);
        byte[] art = File.ReadAllBytes(Path.Combine(root, "art.mul"));
        using (var output = File.Create(Path.Combine(root, "artlegacymul.uop")))
        using (var writer = new Ultima.Uop.UopContainerWriter(output, Ultima.Uop.UopLayout.Version5, 2))
        {
            writer.WriteEntry(Ultima.Helpers.UopUtils.HashFileName("build/artlegacymul/00000002.tga"), art.AsSpan(0, 2024), 2024, 0);
            writer.WriteEntry(Ultima.Helpers.UopUtils.HashFileName("build/artlegacymul/00016385.tga"), art.AsSpan(2024), 22, 0);
            writer.Complete();
        }
        using (var output = File.Create(Path.Combine(root, "gumpartlegacymul.uop")))
        using (var writer = new Ultima.Uop.UopContainerWriter(output, Ultima.Uop.UopLayout.Version5, 1))
        {
            byte[] payload = new byte[16];
            BitConverter.GetBytes(2).CopyTo(payload, 0); BitConverter.GetBytes(1).CopyTo(payload, 4);
            File.ReadAllBytes(Path.Combine(root, "gumpart.mul")).CopyTo(payload, 8);
            writer.WriteEntry(Ultima.Helpers.UopUtils.HashFileName("build/gumpartlegacymul/00000004.tga"), payload, payload.Length, 0);
            writer.Complete();
        }
        const int landBytes = 512 * (4 + 32 * 26);
        byte[] tiledata = new byte[landBytes + 512 * (4 + 32 * 37)];
        Encoding.ASCII.GetBytes("test item").CopyTo(tiledata, landBytes + 4 + 37 + 17);
        File.WriteAllBytes(Path.Combine(root, "tiledata.mul"), tiledata);
    }

    private static void CreateIndex(string path, int count, (int Id, int Offset, int Length, int Extra)[] entries)
    {
        using var writer = new BinaryWriter(File.Create(path));
        for (int i = 0; i < count; i++) { writer.Write(-1); writer.Write(-1); writer.Write(0); }
        foreach (var entry in entries)
        {
            writer.BaseStream.Position = entry.Id * 12L;
            writer.Write(entry.Offset); writer.Write(entry.Length); writer.Write(entry.Extra);
        }
    }
}

# Resource Access

A native UOFiddler plugin for reading resources from the active profile on demand.
It returns structured JSON and optional PNG images through a Windows named pipe.
The accompanying PowerShell client queries a running Fiddler session from a terminal.

## Build and enable

Requires .NET 10 SDK to build, .NET 10 Desktop Runtime to run Fiddler, and
PowerShell 7.4 or later for the client. From the repository root:

```powershell
dotnet build UoFiddler.sln -c Release
```

The build puts `ResourceAccess.dll`, `Invoke-FiddlerResource.ps1`, and
`ResourceAccess.README.md` in `UoFiddler/bin/Release/plugins/`. The plugin uses
the host's `Ultima.dll` and `Controls.dll`; use the aligned 4.24.0 fork.

1. Open UOFiddler and select the desired client profile.
2. Enable **Resource Access** in the plugin manager, then restart UOFiddler.
3. Open the **Resource Access** tab and click **Start connection**.
4. Use **Copy status command** or run the examples below from the repository root.

The connection is stopped initially and closes on unload/application exit.
Its name is `UOFiddler.ResourceAccess.<process-id>`. Windows pipe permissions
allow only the same Windows user. It opens no TCP port.

## Query examples

```powershell
# Profile, commands and limits
./tools/Invoke-FiddlerResource.ps1 -Command status

# Catalog names, resolved paths, sizes and modification times
./tools/Invoke-FiddlerResource.ps1 -Command files

# Single images and tile properties
./tools/Invoke-FiddlerResource.ps1 -Command item -Id 0x1234 -ImagePath ./item.png
./tools/Invoke-FiddlerResource.ps1 -Command land -Id 2 -ImagePath ./land.png
./tools/Invoke-FiddlerResource.ps1 -Command gump -Id 4 -ImagePath ./gump.png
./tools/Invoke-FiddlerResource.ps1 -Command item -Id 0x1234 -NoImage

# ASCII A in font 3; Unicode A in unifont.mul
./tools/Invoke-FiddlerResource.ps1 -Command font -Font 3 -CodePoint 65 -ImagePath ./ascii-A.png
./tools/Invoke-FiddlerResource.ps1 -Command font -Font 0 -CodePoint 65 -Unicode -ImagePath ./unicode-A.png

# One localization entry or saved SeaHats quest
./tools/Invoke-FiddlerResource.ps1 -Command cliloc -Language enu -Id 100002
./tools/Invoke-FiddlerResource.ps1 -Command quest -Id 42 | ConvertTo-Json -Depth 64

# Exact bytes from a mapped file
./tools/Invoke-FiddlerResource.ps1 -Command read -File artidx.mul -Offset 0 -Length 12
./tools/Invoke-FiddlerResource.ps1 -Command read -File artidx.mul -Offset 0 -Length 12 -BytesPath ./entry.bin
```

For multiple Fiddler instances, specify `-FiddlerProcessId 1234` or the exact
`-PipeName` in the tab. The script returns PowerShell objects. Without an export
path, PNG data is returned as `image.base64` and byte ranges as `hex` and `base64`.
Export paths refer to the terminal's current directory and replace existing output
files. The plugin never saves the source client files.

Quest lookup defaults to `Quests.Json` in the client profile's root. Use
**Choose quest JSON** to select a saved file elsewhere; it then appears as
`Quests.Json` in the catalog. This version supports saved JSON quest sources.
Encrypted `Quests.MUL` and unsaved quest-editor changes are outside its quest reader.

## Read behavior

| Command | View | Access behavior |
| --- | --- | --- |
| `status`, `files` | Active profile | Configuration and file metadata; no bulk decoding. |
| `read` | Disk | Seeks directly to the offset; maximum 1 MiB per request. |
| `font` | Disk | ASCII: skips preceding records and decodes one glyph. Unicode: seeks through the offset table. No global font initialization. |
| `cliloc` | Disk | Uncompressed: scans headers and reads the selected string. Compressed: decompresses once and caches until its path, size or modification time changes. |
| `quest` | Disk | Parses one JSON document and returns the selected quest. JSON type tags remain data; root `SecretKey` is excluded. |
| `item`, `land`, `gump` | Fiddler session | Uses indexed MUL/UOP readers and SDK caches. Returns current edits with `modified` and `patched` indicators. Tile properties may initialize the SDK's tile-data table. |

Resource responses report `source` and `view`. Images include MIME type,
dimensions and PNG data. IDs are validated before SDK calls to prevent silent
wrapping. Returned images are copies; host cached/edited bitmaps are not disposed.
SDK calls execute on Fiddler's UI thread to serialize them with profile changes
and editor actions. Large decodes can briefly occupy that thread.

JSON/cliloc documents and decompressed cliloc tables are limited to 64 MiB;
images to four million pixels. Requests are capped at 16 KiB, with a 30-second
connection timeout. A request waiting for the UI can be cancelled before it
begins; an SDK decode already executing on the UI thread completes normally.

## Wire protocol

Send one UTF-8 JSON object followed by a newline per connection:

```json
{"command":"item","id":4660,"includeImage":true}
```

Success: `{"ok":true,"data":{...}}`.
Failure: `{"ok":false,"error":{"code":"not_found","message":"..."}}`.
The response is one JSON line. Unknown request properties are rejected.
Error codes include `invalid_request`, `not_found`, `invalid_file`,
`resource_too_large`, `request_too_large`, `cancelled`, and `read_failed`.

`read.file` is a catalog name rather than an arbitrary path. Request properties
are `command`, `id`, `file`, `offset`, `length`, `font`, `codePoint`, `unicode`,
`language`, and `includeImage`, as applicable to each command.

## Verification

```powershell
dotnet run --project tests/ResourceAccess.Tests/ResourceAccess.Tests.csproj -c Release -- tools/Invoke-FiddlerResource.ps1
```

The dependency-free test executable creates temporary MUL/UOP/font/cliloc/quest
fixtures, checks pixels and metadata, verifies session edits, exercises native UI
dispatch without displaying a window, tests malformed requests and disconnect,
restart and shutdown behavior, and runs the real PowerShell client to export PNG
and binary files. It compares source-file hashes before and after. Fixtures are
retained in the printed temporary directory for inspection.

## Upstream alignment

On 7 October 2026, SeaHats was merged with `polserver/UOFiddler` master at
`610c930833dac8d158f08a0af36da43dd90d08b4` (4.24.0). The merge preserves SeaHats
quest and font extensions, adapts the SeaHats plugin to .NET 10, and uses the
upstream loader for compressed/uncompressed cliloc data. `SeaHatsExternal` stays
on .NET 8 for compatibility with the client/server consumers.

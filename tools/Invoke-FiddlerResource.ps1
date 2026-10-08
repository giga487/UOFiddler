#Requires -Version 7.4
<#
.SYNOPSIS
Queries the Resource Access plugin in an open UOFiddler session.
.EXAMPLE
./Invoke-FiddlerResource.ps1 -Command item -Id 0x1234 -ImagePath ./item.png
.EXAMPLE
./Invoke-FiddlerResource.ps1 -Command font -Font 3 -CodePoint 65 -ImagePath ./A.png
.EXAMPLE
./Invoke-FiddlerResource.ps1 -Command read -File artidx.mul -Offset 0 -Length 12
#>
[CmdletBinding()]
param(
    [ValidateSet('status', 'files', 'read', 'item', 'land', 'gump', 'font', 'cliloc', 'quest', 'import', 'save')]
    [string] $Command = 'status',
    [string] $PipeName,
    [int] $FiddlerProcessId,
    [int] $Id,
    [string] $File,
    [long] $Offset = 0,
    [ValidateRange(1, 1048576)] [int] $Length = 256,
    [ValidateRange(0, 19)] [int] $Font = 0,
    [int] $CodePoint,
    [switch] $Unicode,
    [string] $Language = 'enu',
    [switch] $NoImage,
    [string] $ImagePath,
    [string] $BytesPath,
    [ValidateSet('item', 'land', 'gump')] [string] $Kind,
    [string] $InputImagePath,
    [string] $OutputDirectory,
    [ValidateSet('mul', 'uop')] [string] $Format = 'mul',
    [ValidateRange(1, 3600)] [int] $TimeoutSeconds = 35
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
if ($Command -eq 'save' -and -not $PSBoundParameters.ContainsKey('TimeoutSeconds')) { $TimeoutSeconds = 600 }
if (-not $PipeName) {
    if (-not $FiddlerProcessId) {
        $fiddlerProcesses = @(Get-Process -Name UoFiddler -ErrorAction SilentlyContinue)
        if ($fiddlerProcesses.Count -ne 1) {
            throw 'Specify -PipeName or -FiddlerProcessId when there is not exactly one running UOFiddler process.'
        }
        $FiddlerProcessId = $fiddlerProcesses[0].Id
    }
    $PipeName = "UOFiddler.ResourceAccess.$FiddlerProcessId"
}

$request = @{ command = $Command; includeImage = (-not $NoImage.IsPresent) }
if ($PSBoundParameters.ContainsKey('Id')) { $request.id = $Id }
if ($File) { $request.file = $File }
if ($Command -in @('import', 'save')) {
    if (-not $Kind) { throw '-Kind is required for import and save.' }
    $request.kind = $Kind
}
switch ($Command) {
    'import' {
        if (-not $InputImagePath -or -not $PSBoundParameters.ContainsKey('Id')) { throw 'import requires -Id and -InputImagePath.' }
        $request.imagePath = (Resolve-Path -LiteralPath $InputImagePath -ErrorAction Stop).ProviderPath
    }
    'save' {
        if ($PSBoundParameters.ContainsKey('Id')) { throw 'save writes the whole collection; omit -Id.' }
        $request.format = $Format
        if ($OutputDirectory) { $request.outputDirectory = $ExecutionContext.SessionState.Path.GetUnresolvedProviderPathFromPSPath($OutputDirectory) }
    }
    'read' { $request.offset = $Offset; $request.length = $Length }
    'font' {
        if (-not $PSBoundParameters.ContainsKey('CodePoint')) { throw '-CodePoint is required for font.' }
        $request.font = $Font; $request.codePoint = $CodePoint; $request.unicode = $Unicode.IsPresent
    }
    'cliloc' { $request.language = $Language }
}
if ($ImagePath -and $NoImage) { throw '-ImagePath cannot be combined with -NoImage.' }
if ($ImagePath -and $Command -notin @('item', 'land', 'gump', 'font', 'import')) { throw '-ImagePath requires an image command.' }
if ($BytesPath -and $Command -ne 'read') { throw '-BytesPath requires the read command.' }

$pipe = [IO.Pipes.NamedPipeClientStream]::new('.', $PipeName, [IO.Pipes.PipeDirection]::InOut, [IO.Pipes.PipeOptions]::Asynchronous)
$reader = $null
$writer = $null
$cancellation = [Threading.CancellationTokenSource]::new()
try {
    $pipe.Connect($TimeoutSeconds * 1000)
    $encoding = [Text.UTF8Encoding]::new($false)
    $reader = [IO.StreamReader]::new($pipe, $encoding, $false, 4096, $true)
    $writer = [IO.StreamWriter]::new($pipe, $encoding, 4096, $true)
    $writer.AutoFlush = $true
    $writer.WriteLine(($request | ConvertTo-Json -Compress))
    $cancellation.CancelAfter($TimeoutSeconds * 1000)
    $responseLine = $reader.ReadLineAsync($cancellation.Token).AsTask().GetAwaiter().GetResult()
    if (-not $responseLine) { throw 'Fiddler closed the connection without a response.' }
    $response = $responseLine | ConvertFrom-Json
    if (-not $response.ok) { throw "$($response.error.code): $($response.error.message)" }
    if ($ImagePath) {
        if (-not ($response.data.PSObject.Properties.Name -contains 'image') -or -not $response.data.image) {
            throw 'The resource has no image to export.'
        }
        [IO.File]::WriteAllBytes($ExecutionContext.SessionState.Path.GetUnresolvedProviderPathFromPSPath($ImagePath), [Convert]::FromBase64String($response.data.image.base64))
        $response.data.image.PSObject.Properties.Remove('base64')
        $response.data.image | Add-Member -NotePropertyName path -NotePropertyValue (Get-Item -LiteralPath $ImagePath).FullName
    }
    if ($BytesPath) {
        [IO.File]::WriteAllBytes($ExecutionContext.SessionState.Path.GetUnresolvedProviderPathFromPSPath($BytesPath), [Convert]::FromBase64String($response.data.base64))
        $response.data.PSObject.Properties.Remove('base64')
        $response.data.PSObject.Properties.Remove('hex')
        $response.data | Add-Member -NotePropertyName path -NotePropertyValue (Get-Item -LiteralPath $BytesPath).FullName
    }
    $response.data
}
finally {
    if ($writer) { $writer.Dispose() }
    if ($reader) { $reader.Dispose() }
    $pipe.Dispose()
    $cancellation.Dispose()
}

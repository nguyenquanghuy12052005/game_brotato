param(
    [Parameter(Mandatory)] [string]$CaptureDirectory,
    [Parameter(Mandatory)] [string]$OutputPath,
    [ValidateRange(1, 2)] [int]$DelayScale = 1
)

$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.Drawing
$frames = Get-ChildItem -LiteralPath $CaptureDirectory -Filter '*.png' | Sort-Object Name
if ($frames.Count -lt 2) { throw 'Missing Unity camera frames.' }

$writer = [IO.BinaryWriter]::new([IO.File]::Create($OutputPath))
try {
    for ($i = 0; $i -lt $frames.Count; $i++) {
        $bitmap = [Drawing.Bitmap]::new($frames[$i].FullName)
        $stream = [IO.MemoryStream]::new()
        try {
            $bitmap.Save($stream, [Drawing.Imaging.ImageFormat]::Gif)
            $bytes = $stream.ToArray()
        }
        finally {
            $bitmap.Dispose()
            $stream.Dispose()
        }

        $paletteExponent = $bytes[10] -band 7
        $paletteSize = 3 * (2 -shl $paletteExponent)
        $imageStart = 13 + $paletteSize
        while ($bytes[$imageStart] -eq 33) {
            $imageStart += 2
            while ($bytes[$imageStart] -ne 0) { $imageStart += 1 + $bytes[$imageStart] }
            $imageStart++
        }
        if ($bytes[$imageStart] -ne 44) { throw "Unexpected GIF image structure in $($frames[$i].Name)" }
        if ($i -eq 0) {
            $writer.Write([Text.Encoding]::ASCII.GetBytes('GIF89a'))
            $writer.Write($bytes, 6, 7 + $paletteSize)
            $writer.Write([byte[]]@(33,255,11))
            $writer.Write([Text.Encoding]::ASCII.GetBytes('NETSCAPE2.0'))
            $writer.Write([byte[]]@(3,1,0,0,0))
        }

        $delay = if ($i -eq $frames.Count - 1) { 45 } elseif ($i % 2 -eq 0) { 7 } else { 8 }
        $delay *= $DelayScale
        $writer.Write([byte[]]@(33,249,4,4,$delay,0,0,0))
        $writer.Write($bytes, $imageStart, 9)
        $writer.Write([byte](128 -bor $paletteExponent -bor ($bytes[$imageStart + 9] -band 64)))
        $writer.Write($bytes, 13, $paletteSize)
        $writer.Write($bytes, $imageStart + 10, $bytes.Length - $imageStart - 11)
    }
    $writer.Write([byte]59)
}
finally { $writer.Dispose() }

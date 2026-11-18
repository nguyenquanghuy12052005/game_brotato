param(
    [Parameter(Mandatory)]
    [ValidateSet('Q1', 'Basic', 'BasicDiagonals', 'EastDiagonals', 'Slam', 'Diagonals', 'Hero', 'HeroWest', 'SignatureQ', 'SignatureE', 'SignatureF', 'IdleLoops', 'Turn', 'Continuous', 'Directional', 'SpinAtlas')]
    [string]$Action,
    [ValidateSet('Swordsman', 'Assassin', 'DaoWarrior', 'QiSwordsman')]
    [string]$Form = 'Swordsman',
    [string]$SourcePath,
    [string]$ClipName,
    [int]$SourceGridRows,
    [int]$FrameOffset = 0,
    [ValidateSet('East', 'West', 'South', 'North', 'NE', 'SE', 'SW', 'NW')]
    [string]$Facing = 'East',
    [string[]]$DirectionRows = @('NE', 'SE', 'SW', 'NW'),
    [string[]]$IncludeDirections = @('NE', 'SE', 'SW', 'NW'),
    [int[]]$IncludePoseRows = @(),
    [switch]$SinglePose,
    [int]$PoseHeight = 160,
    [string]$OutputDirectory
)

$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.Drawing
if (-not ('HexcoreSilhouette' -as [type])) {
    Add-Type -TypeDefinition @'
public static class HexcoreSilhouette {
    public static void RetainMain(byte[] pixels, int width, int height) {
        int count = width * height;
        var groups = new int[count];
        var queue = new int[count];
        int id = 0, largestId = 0, largestSize = 0;
        for (int start = 0; start < count; start++) {
            if (groups[start] != 0 || pixels[start * 4 + 3] < 192) continue;
            int read = 0, written = 1;
            queue[0] = start;
            groups[start] = ++id;
            while (read < written) {
                int index = queue[read++], x = index % width, y = index / width;
                for (int yy = System.Math.Max(0, y - 1); yy <= System.Math.Min(y + 1, height - 1); yy++)
                for (int xx = System.Math.Max(0, x - 1); xx <= System.Math.Min(x + 1, width - 1); xx++) {
                    int neighbor = yy * width + xx;
                    if (groups[neighbor] != 0 || pixels[neighbor * 4 + 3] < 192) continue;
                    groups[neighbor] = id;
                    queue[written++] = neighbor;
                }
            }
            if (written <= largestSize) continue;
            largestSize = written;
            largestId = id;
        }
        for (int i = 0; i < count; i++) pixels[i * 4 + 3] = groups[i] == largestId && largestId != 0 ? (byte)255 : (byte)0;
    }
}
'@
}
$repo = (Resolve-Path "$PSScriptRoot/../../..").Path
$basicDiagonal = $Action -eq 'BasicDiagonals'
$directional = $Action -eq 'Directional'
$diagonal = $Action.EndsWith('Diagonals') -and !$basicDiagonal
$export = "$repo/Assets/Prefabs/Player/$Form/HexcorePreview/$(if ($diagonal) {'Locomotion'} elseif ($basicDiagonal) {'Basic'} else {$Action})"
if ($OutputDirectory) { $export = $OutputDirectory }
$source = if ($diagonal) { $Action } else { "${Action}_East" }
if (!$SourcePath) { $SourcePath = "$PSScriptRoot/${source}_Source.png" }
$sheet = [Drawing.Bitmap]::new((Resolve-Path -LiteralPath $SourcePath).Path)
$format = [Drawing.Imaging.ImageFormat]::Png
$windows = [Drawing.GraphicsUnit]::Pixel
$columns = if ($basicDiagonal -or $directional -or $Action -eq 'SpinAtlas') { 8 } elseif ($diagonal) { 5 } else { 4 }
$rows = if ($diagonal -or $basicDiagonal -or $directional) { $DirectionRows.Count } elseif ($Action -eq 'HeroWest') { 6 } elseif ($Action -in 'Hero','IdleLoops','Turn') { 4 } else { 3 }
if ($SourceGridRows -gt 0) { $rows = $SourceGridRows }
if ($SinglePose) {
    if (!$directional -or $DirectionRows.Count -ne 1 -or !$ClipName) {
        throw 'SinglePose needs a Directional clip name and one heading.'
    }
    $columns = $rows = 1
}
$exportRows = if ($Action -eq 'EastDiagonals') { 2 } else { $rows }
$directions = $DirectionRows
$size = if ($Form -in 'DaoWarrior','QiSwordsman') { 320 } else { 256 }
$pivot = $size / 2
$cellWidth = $sheet.Width / [double]$columns
$cellHeight = $sheet.Height / [double]$rows

try {
    New-Item -ItemType Directory -Path $export -Force | Out-Null
    $scale = 0.0
    for ($i = 0; $i -lt $columns * $exportRows; $i++) {
        $column = $i % $columns
        $row = [int][Math]::Floor($i / [double]$columns)
        if ($IncludePoseRows.Count -gt 0 -and $row -notin $IncludePoseRows) { continue }
        if (($diagonal -or $basicDiagonal -or $directional) -and $directions[$row] -notin $IncludeDirections) { continue }
        $x = [int][Math]::Round($column * $cellWidth)
        $y = [int][Math]::Round($row * $cellHeight)
        $w = [int][Math]::Round(($column + 1) * $cellWidth) - $x
        $h = [int][Math]::Round(($row + 1) * $cellHeight) - $y
        $cellRect = [Drawing.Rectangle]::new($x, $y, $w, $h)
        $cell = $sheet.Clone($cellRect, [Drawing.Imaging.PixelFormat]::Format32bppArgb)
        $localRect = [Drawing.Rectangle]::new(0, 0, $w, $h)
        $lock = $cell.LockBits($localRect, [Drawing.Imaging.ImageLockMode]::ReadWrite, [Drawing.Imaging.PixelFormat]::Format32bppArgb)
        $pixels = [byte[]]::new($w * $h * 4)
        [Runtime.InteropServices.Marshal]::Copy($lock.Scan0, $pixels, 0, $pixels.Length)
        [HexcoreSilhouette]::RetainMain($pixels, $w, $h)
        [Runtime.InteropServices.Marshal]::Copy($pixels, 0, $lock.Scan0, $pixels.Length)
        $cell.UnlockBits($lock)
        $top = $h
        $bottom = -1
        $solid = 0
        for ($dy = 0; $dy -lt $h; $dy++) {
            for ($dx = 0; $dx -lt $w; $dx++) {
                if ($pixels[($dy * $w + $dx) * 4 + 3] -eq 0) { continue }
                $top = [Math]::Min($top, $dy)
                $bottom = $dy
                $solid++
            }
        }
        if ($solid -lt 1000) { throw "Cell $i has no usable pose." }
        if ($scale -eq 0) {
            $scale = [Math]::Min($PoseHeight / ($bottom - $top + 1.0), ($size - 8) / $cellWidth)
        }
        $ground = if ($Action -eq 'Q1' -and $i -eq 5) { $pivot + 67 } else { $pivot + 80 }
        $frame = [Drawing.Bitmap]::new($size, $size)
        $graphics = [Drawing.Graphics]::FromImage($frame)
        try {
            $graphics.InterpolationMode = 'NearestNeighbor'
            $graphics.PixelOffsetMode = 'Half'
            $destination = [Drawing.Rectangle]::new(
                [int][Math]::Round($pivot - .5 * $w * $scale),
                [int][Math]::Round($ground - $bottom * $scale),
                [int][Math]::Round($w * $scale),
                [int][Math]::Round($h * $scale))
            $graphics.DrawImage($cell, $destination, $localRect, $windows)
            $stem = if ($ClipName) { $ClipName } else { $Action }
            $name = if ($diagonal) { "$($directions[$row])_$column" } elseif ($basicDiagonal) { "Basic_$($directions[$row])_0$column" } elseif ($directional) { "${stem}_$($directions[$row])_$(($column + $FrameOffset).ToString('00'))" } elseif ($Action -eq 'IdleLoops') { "Idle_$($directions[$row])_0$column" } else { "${stem}_${Facing}_$(($i + $FrameOffset).ToString('00'))" }
            $frame.Save("$export/$name.png", $format)
            Write-Output "$Action $i cell ($x,$y) to ($($destination.X),$($destination.Y)), scale $scale"
        }
        finally {
            $graphics.Dispose()
            $frame.Dispose()
            $cell.Dispose()
        }
    }
}
finally {
    $sheet.Dispose()
}

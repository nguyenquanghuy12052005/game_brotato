param(
    [Parameter(Mandatory)] [string]$CaptureRoot,
    [Parameter(Mandatory)] [string]$OutputRoot
)

$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.Drawing
$CaptureRoot = (Resolve-Path -LiteralPath $CaptureRoot).Path
New-Item -ItemType Directory -Force $OutputRoot | Out-Null
$OutputRoot = (Resolve-Path -LiteralPath $OutputRoot).Path
$gifBuilder = Join-Path $PSScriptRoot '../HexcoreForm01/Build-ReviewGif.ps1'
$scratch = Join-Path (Get-Location) 'Temp/HexcoreReviewComposite'
New-Item -ItemType Directory -Force $scratch | Out-Null
$headings = 'N','NE','E','SE','S','SW','W','NW'
$index = [Collections.Generic.List[string]]::new()
$index.Add('# HEXCORE · Bộ GIF review')
$index.Add('')
$index.Add('Ghi hình bằng camera Unity. Clip hướng thiếu artwork mang nhãn Missing art. Blockout dùng để kiểm tra chuyển động và timing.')
$sheet = Join-Path $OutputRoot 'UnityPoseChecks.png'
if (Test-Path -LiteralPath $sheet) {
    $index.Add('')
    $index.Add('[Bảng khung hình kiểm tra pose, điểm chạm và phân thân](UnityPoseChecks.png)')
}
$index.Add('')
$index.Add('## Locomotion, Turn và Basic theo tám hướng')
$index.Add('')
$framesByMode = @{ Idle=8; Run=8; Turn=8; Basic=12 }
$font = [Drawing.Font]::new('Arial', 12)
try {
    foreach ($form in 1..4) {
        foreach ($action in 'Idle','Run','Turn','Basic') {
            $base = "$action-$form"
            $folder = Join-Path $scratch $base
            New-Item -ItemType Directory -Force $folder | Out-Null
            for ($frame = 0; $frame -lt $framesByMode[$action]; $frame++) {
                $bitmap = [Drawing.Bitmap]::new(1024, 332)
                $g = [Drawing.Graphics]::FromImage($bitmap)
                try {
                    $g.Clear([Drawing.Color]::FromArgb(32, 42, 38))
                    for ($d = 0; $d -lt $headings.Length; $d++) {
                        $x = ($d % 4) * 256
                        $y = [int][Math]::Floor($d / 4.0) * 166
                        $path = Join-Path $CaptureRoot ("$base-$($headings[$d])/{0:000}.png" -f $frame)
                        if (Test-Path -LiteralPath $path) {
                            $shot = [Drawing.Bitmap]::new($path)
                            try {
                                $g.DrawImage($shot, [Drawing.Rectangle]::new($x+2,$y+2,252,142))
                            } finally { $shot.Dispose() }
                            $label = if ($action -eq 'Turn') { "$($headings[$d]) → $($headings[($d+1)%8])" } else { $headings[$d] }
                        } else {
                            $label = "$($headings[$d]) · Missing art"
                        }
                        $g.DrawString($label, $font, [Drawing.Brushes]::White, $x+8, $y+145)
                    }
                    $bitmap.Save((Join-Path $folder ('{0:000}.png' -f $frame)), [Drawing.Imaging.ImageFormat]::Png)
                } finally {
                    $g.Dispose()
                    $bitmap.Dispose()
                }
            }
            & $gifBuilder -CaptureDirectory $folder -OutputPath (Join-Path $OutputRoot "$base.gif")
            $index.Add("- [Form $form · $action]($base.gif)")
        }
    }
} finally { $font.Dispose() }

$index.Add('')
$index.Add('## Kỹ năng theo form tương thích')
$index.Add('')
$index.Add('Tên tệp dùng thứ tự 01–24 trong catalog. Cột form và hướng nằm sau số kỹ năng. Clean tắt VFX.')
$index.Add('')
$catalog = @(Get-Content -Encoding utf8 'Assets/Scripts/UI/HexcorePreviewCatalog.cs' | ForEach-Object {
    if ($_ -match 'Entry\("([^"]+)"') { $Matches[1] }
})
foreach ($clip in Get-ChildItem -LiteralPath $CaptureRoot -Directory | Where-Object Name -match '^(Skill|Clean)-' | Sort-Object Name) {
    $name = $clip.Name + '.gif'
    & $gifBuilder -CaptureDirectory $clip.FullName -OutputPath (Join-Path $OutputRoot $name)
    $fields = $clip.Name.Split('-')
    $item = $catalog[[int]$fields[1] - 1]
    $index.Add("- [$($fields[0]) · $item · Form $($fields[2]) · $($fields[3])]($name)")
    $slowReview = ($fields[2] -eq '1' -and $fields[3] -eq 'E' -and [int]$fields[1] -in 13,14,15) -or
        ($fields[2] -eq '1' -and $fields[3] -eq 'NW' -and [int]$fields[1] -in 1,2,3) -or
        ($fields[3] -eq 'NE' -and [int]$fields[1] -in 13,14,15)
    if ($fields[0] -eq 'Skill' -and $slowReview) {
        $slow = $clip.Name + '-Slow.gif'
        & $gifBuilder -CaptureDirectory $clip.FullName -OutputPath (Join-Path $OutputRoot $slow) -DelayScale 2
        $index.Add("- [$item · Form $($fields[2]) · $($fields[3]) · 0,5×]($slow)")
    }
}
Set-Content -LiteralPath (Join-Path $OutputRoot 'README.md') -Value $index -Encoding utf8

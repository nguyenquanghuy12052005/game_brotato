param([string[]]$Names)
$ErrorActionPreference='Stop'
Add-Type -AssemblyName System.Drawing
function Write-GifFrame($writer,$bitmap,$delay,$first) {
 $memory=[IO.MemoryStream]::new();$bitmap.Save($memory,[Drawing.Imaging.ImageFormat]::Gif);$b=$memory.ToArray()
 $n=$b[10] -band 7;$pal=3*(2 -shl $n);$pos=13+$pal
 while($b[$pos] -eq 33){$pos+=2;while($b[$pos] -ne 0){$pos+=1+$b[$pos]};$pos++}
 if($b[$pos] -ne 44){throw 'Invalid GIF'}
 if($first){$writer.Write([Text.Encoding]::ASCII.GetBytes('GIF89a'));$writer.Write($b,6,7+$pal);$writer.Write([byte[]]@(33,255,11));$writer.Write([Text.Encoding]::ASCII.GetBytes('NETSCAPE2.0'));$writer.Write([byte[]]@(3,1,0,0,0))}
 $writer.Write([byte[]]@(33,249,4,4,$delay,0,0,0));$writer.Write($b,$pos,9);$writer.Write([byte](128 -bor $n -bor ($b[$pos+9] -band 64)));$writer.Write($b,13,$pal);$writer.Write($b,$pos+10,$b.Length-$pos-11)
 $memory.Dispose()
}

$root=Join-Path $PWD 'Assets/Prefabs/Player/Melee/Shared/VFX/MeleeSkills'
$sourceRoot=Join-Path $PWD 'docs/Melee/Source/VFX/MeleeSkills'
$preview=Join-Path $PWD 'docs/Melee/MeleeSkills'
foreach($entry in $Names) {
 $name=Split-Path $entry -Leaf
 $folder="$root/$entry"
 if (!(Test-Path -LiteralPath "$sourceRoot/$entry/Sheet.png" -PathType Leaf)) { throw "No active VFX master for $entry" }
 $source=[Drawing.Bitmap]::new("$sourceRoot/$entry/Sheet.png")
 New-Item "$folder/Frames" -ItemType Directory -Force | Out-Null
 $writer=[IO.BinaryWriter]::new([IO.File]::Create("$preview/$name.gif"))
 for($i=0;$i -lt 8;$i++) {
  $frame=[Drawing.Bitmap]::new(256,256)
  $g=[Drawing.Graphics]::FromImage($frame)
  $g.InterpolationMode='NearestNeighbor'
  $g.PixelOffsetMode='Half'
  $rect=[Drawing.Rectangle]::new(($i%4)*($source.Width/4),[Math]::Floor($i/4)*($source.Height/2),$source.Width/4,$source.Height/2)
  $g.DrawImage($source,[Drawing.Rectangle]::new(0,0,256,256),$rect,[Drawing.GraphicsUnit]::Pixel)
  $g.Dispose()
  for($y=0;$y -lt 256;$y++){for($x=0;$x -lt 256;$x++){
   $c=$frame.GetPixel($x,$y)
   $edge=[Math]::Min([Math]::Min($x,255-$x),[Math]::Min($y,255-$y))
   $alpha=[int]($c.A*[Math]::Min(1.0,$edge/10.0))
   $spread=[Math]::Max($c.R,[Math]::Max($c.G,$c.B))-[Math]::Min($c.R,[Math]::Min($c.G,$c.B))
   if(($spread -lt 24 -and $c.A -lt 220) -or $edge -lt 8){$alpha=0}
   if($name -eq 'DiaLietTram' -and $spread -lt 45 -and $c.R -gt 140){$alpha=0}
   if($alpha -lt 20){$alpha=0}
   $frame.SetPixel($x,$y,[Drawing.Color]::FromArgb($alpha,$c.R,$c.G,$c.B))
  }}
  $frame.Save("$folder/Frames/${name}_$($i.ToString('00')).png",[Drawing.Imaging.ImageFormat]::Png)
  $display=[Drawing.Bitmap]::new(256,256)
  $g=[Drawing.Graphics]::FromImage($display)
  $g.Clear([Drawing.Color]::FromArgb(28,39,35))
  $g.DrawImageUnscaled($frame,0,0)
  $g.Dispose()
  Write-GifFrame $writer $display 8 ($i -eq 0)
  $display.Dispose();$frame.Dispose()
 }
 $writer.Write([byte]59);$writer.Dispose();$source.Dispose()
}


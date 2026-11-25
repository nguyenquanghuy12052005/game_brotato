$ErrorActionPreference='Stop'
Add-Type -AssemblyName System.Drawing
$root=Join-Path $PWD 'Assets/Prefabs/Player/QiSwordsman'
$previewRoot=Join-Path $PWD 'docs/player-art/QiSwordsmanFull'
New-Item "$root/WaveFrames" -ItemType Directory -Force | Out-Null
$source=[Drawing.Bitmap]::new("$root/Sheets/Wave.png")
$bounds=@();$maxHeight=1
for($i=0;$i -lt 4;$i++) {
 $left=[int][Math]::Round($i*$source.Width/4.0);$right=[int][Math]::Round(($i+1)*$source.Width/4.0)
 $x0=$right;$x1=$left;$y0=$source.Height;$y1=0
 for($y=0;$y -lt $source.Height;$y++){for($x=$left;$x -lt $right;$x++){
  if($source.GetPixel($x,$y).A -gt 32){$x0=[Math]::Min($x0,$x);$x1=[Math]::Max($x1,$x);$y0=[Math]::Min($y0,$y);$y1=[Math]::Max($y1,$y)}
 }}
 if($x0 -gt $x1){throw 'Empty wave cell'}
 $padding=[int][Math]::Ceiling(($y1-$y0+1)*.20)
 $x0=[Math]::Max($left,$x0-$padding);$x1=[Math]::Min($right-1,$x1+$padding)
 $y0=[Math]::Max(0,$y0-$padding);$y1=[Math]::Min($source.Height-1,$y1+$padding)
 $bounds+=[Drawing.Rectangle]::new($x0,$y0,$x1-$x0+1,$y1-$y0+1)
 $maxHeight=[Math]::Max($maxHeight,$y1-$y0+1)
}
$scale=160.0/$maxHeight
for($i=0;$i -lt 4;$i++) {
 $boundsNow=$bounds[$i]
 $frame=[Drawing.Bitmap]::new(256,256);$g=[Drawing.Graphics]::FromImage($frame)
 $g.InterpolationMode='NearestNeighbor';$g.PixelOffsetMode='Half'
 $width=[int][Math]::Round($boundsNow.Width*$scale);$height=[int][Math]::Round($boundsNow.Height*$scale)
 $g.DrawImage($source,[Drawing.Rectangle]::new([int](128-$width/2),[int](128-$height/2),$width,$height),$boundsNow,[Drawing.GraphicsUnit]::Pixel)
 $g.Dispose()
 $alphaFactor=@(.75,1.0,.9,.3)[$i]
 for($y=0;$y -lt 256;$y++){for($x=0;$x -lt 256;$x++){
  $color=$frame.GetPixel($x,$y)
  $frame.SetPixel($x,$y,[Drawing.Color]::FromArgb([int][Math]::Round($color.A*$alphaFactor),$color.R,$color.G,$color.B))
 }}
 $frame.Save("$root/WaveFrames/QiWave_$($i.ToString('00')).png",[Drawing.Imaging.ImageFormat]::Png);$frame.Dispose()
}
$source.Dispose()

function Write-GifFrame($writer,$bitmap,$delay,$first) {
 $memory=[IO.MemoryStream]::new();$bitmap.Save($memory,[Drawing.Imaging.ImageFormat]::Gif);$b=$memory.ToArray()
 $n=$b[10] -band 7;$pal=3*(2 -shl $n);$pos=13+$pal
 while($b[$pos] -eq 33){$pos+=2;while($b[$pos] -ne 0){$pos+=1+$b[$pos]};$pos++}
 if($b[$pos] -ne 44){throw 'Invalid GIF'}
 if($first){$writer.Write([Text.Encoding]::ASCII.GetBytes('GIF89a'));$writer.Write($b,6,7+$pal);$writer.Write([byte[]]@(33,255,11));$writer.Write([Text.Encoding]::ASCII.GetBytes('NETSCAPE2.0'));$writer.Write([byte[]]@(3,1,0,0,0))}
 $writer.Write([byte[]]@(33,249,4,4,$delay,0,0,0));$writer.Write($b,$pos,9);$writer.Write([byte](128 -bor $n -bor ($b[$pos+9] -band 64)));$writer.Write($b,13,$pal);$writer.Write($b,$pos+10,$b.Length-$pos-11)
 $memory.Dispose()
}
$forest=[Drawing.Bitmap]::new((Resolve-Path 'Assets/public/map/roboden-tileset/roboden-tileset/forest/tiles.png').Path)
$writer=[IO.BinaryWriter]::new([IO.File]::Create("$previewRoot/QiWave.gif"))
$delays=@(6,8,10,12)
for($i=0;$i -lt 4;$i++) {
 $frame=[Drawing.Bitmap]::new(256,256);$g=[Drawing.Graphics]::FromImage($frame)
 for($y=0;$y -lt 256;$y+=32){$g.DrawImage($forest,[Drawing.Rectangle]::new(0,$y,256,32),[Drawing.Rectangle]::new(0,0,256,32),[Drawing.GraphicsUnit]::Pixel)}
 $wave=[Drawing.Bitmap]::new("$root/WaveFrames/QiWave_$($i.ToString('00')).png")
 $g.DrawImageUnscaled($wave,0,0);$wave.Dispose();$g.Dispose()
 Write-GifFrame $writer $frame $delays[$i] ($i -eq 0);$frame.Dispose()
}
$writer.Write([byte]59);$writer.Dispose()

$writer=[IO.BinaryWriter]::new([IO.File]::Create("$previewRoot/ReleaseDemo.gif"))
$starts=@(0,.12,.24,.31,.37,.45,.57)
for($i=0;$i -lt 27;$i++) {
 $t=$i*.04;$index=0
 for($j=0;$j -lt 7;$j++){if($t -ge $starts[$j]){$index=$j}}
 $frame=[Drawing.Bitmap]::new(640,320);$g=[Drawing.Graphics]::FromImage($frame)
 for($y=0;$y -lt 320;$y+=32){$g.DrawImage($forest,[Drawing.Rectangle]::new(0,$y,640,32),[Drawing.Rectangle]::new(0,0,640,32),[Drawing.GraphicsUnit]::Pixel)}
 $character=[Drawing.Bitmap]::new("$root/Frames/Attack/Right/Attack_Right_$($index.ToString('00')).png")
 $g.DrawImageUnscaled($character,0,0);$character.Dispose()
 if($t -ge .31 -and $t -lt .67) {
  $age=$t-.31;$waveIndex=if($age -lt .06){0}elseif($age -lt .14){1}elseif($age -lt .24){2}else{3}
  $wave=[Drawing.Bitmap]::new("$root/WaveFrames/QiWave_$($waveIndex.ToString('00')).png")
  $g.DrawImageUnscaled($wave,[int](285+$age/.36*190-128),46);$wave.Dispose()
 }
 $g.Dispose();Write-GifFrame $writer $frame 4 ($i -eq 0);$frame.Dispose()
}
$writer.Write([byte]59);$writer.Dispose();$forest.Dispose()
'Exported four wave PNGs and two preview GIFs.'

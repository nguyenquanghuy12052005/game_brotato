param([string[]]$Actions=@('Idle','Run','Attack','Death'))
$ErrorActionPreference='Stop'
Add-Type -AssemblyName System.Drawing
Add-Type -TypeDefinition @'
public static class AssassinPixels {
 public static void Clean(byte[] p) {
  bool[] seen=new bool[65536];int[] queue=new int[65536];
  for(int start=0;start<65536;start++) {
   if(seen[start]||p[start*4+3]<64)continue;
   int head=0,tail=1;queue[0]=start;seen[start]=true;
   while(head<tail) {
    int k=queue[head++],x=k%256,y=k/256;
    for(int dy=-1;dy<=1;dy++)for(int dx=-1;dx<=1;dx++){
     int nx=x+dx,ny=y+dy;if(nx<0||ny<0||nx>=256||ny>=256)continue;
     int n=ny*256+nx;if(seen[n]||p[n*4+3]<64)continue;
     seen[n]=true;queue[tail++]=n;
    }
   }
   if(tail<100)for(int i=0;i<tail;i++)p[queue[i]*4+3]=0;
  }
 }
}
'@
$root=Join-Path $PWD 'Assets/Prefabs/Player/Assassin'
$previewRoot=Join-Path $PWD 'docs/player-art/AssassinFull'
$directions=@('Down','Left','Right','Up')
$counts=@{Idle=4;Run=5;Attack=5;Death=4}
$delays=@{Idle=@(16,17,17,17);Run=@(10,10,10,10,10);Attack=@(10,8,5,7,18);Death=@(12,12,14,60)}
$forest=[Drawing.Bitmap]::new((Resolve-Path 'Assets/public/map/roboden-tileset/roboden-tileset/forest/tiles.png').Path)
foreach($action in $Actions) {
 $source=[Drawing.Bitmap]::new("$root/Sheets/$action.png")
 for($y=0;$y -lt $source.Height;$y++){for($x=0;$x -lt $source.Width;$x++){
  if($source.GetPixel($x,$y).A -lt 64){$source.SetPixel($x,$y,[Drawing.Color]::Transparent)}
 }}
 $columns=$counts[$action];$cellW=$source.Width/[double]$columns;$cellH=$source.Height/4.0
 $top=[int]$cellH;$bottom=0
 for($y=0;$y -lt [int]$cellH;$y++){for($x=0;$x -lt [int]$cellW;$x++){
  if($source.GetPixel($x,$y).A -gt 192){$top=[Math]::Min($top,$y);$bottom=[Math]::Max($bottom,$y)}
 }}
 $scale=160.0/($bottom-$top+1)
 for($row=0;$row -lt 4;$row++) {
  $direction=$directions[$row];$folder="$root/Frames/$action/$direction"
  New-Item $folder -ItemType Directory -Force | Out-Null
  $writer=[IO.BinaryWriter]::new([IO.File]::Create("$previewRoot/${action}_${direction}.gif"))
  for($i=0;$i -lt $columns;$i++) {
   $sx=[int][Math]::Round($i*$cellW);$sy=[int][Math]::Round($row*$cellH)
   $sw=[int][Math]::Round(($i+1)*$cellW)-$sx;$sh=[int][Math]::Round(($row+1)*$cellH)-$sy
   $bottom=0
   for($y=$sy;$y -lt $sy+$sh;$y++){for($x=$sx;$x -lt $sx+$sw;$x++){
    if($source.GetPixel($x,$y).A -gt 192){$bottom=[Math]::Max($bottom,$y-$sy)}
   }}
   $anchor=$sw/2.0
   if($action -eq 'Attack') {
    $padding=[int][Math]::Ceiling($cellW*.12)
    $left=[Math]::Max(0,$sx-$padding)
    $right=[Math]::Min($source.Width,$sx+$sw+$padding)
    $anchor+=($sx-$left);$sx=$left;$sw=$right-$left
   }
   $frame=[Drawing.Bitmap]::new(256,256);$g=[Drawing.Graphics]::FromImage($frame)
   $g.InterpolationMode='NearestNeighbor';$g.PixelOffsetMode='Half'
   $w=[int][Math]::Round($sw*$scale);$h=[int][Math]::Round($sh*$scale)
   $g.DrawImage($source,[Drawing.Rectangle]::new([int](128-$anchor*$scale),[int](208-$bottom*$scale),$w,$h),[Drawing.Rectangle]::new($sx,$sy,$sw,$sh),[Drawing.GraphicsUnit]::Pixel)
   $g.Dispose()
   $lock=$frame.LockBits([Drawing.Rectangle]::new(0,0,256,256),[Drawing.Imaging.ImageLockMode]::ReadWrite,[Drawing.Imaging.PixelFormat]::Format32bppArgb)
   $pixels=[byte[]]::new(262144);[Runtime.InteropServices.Marshal]::Copy($lock.Scan0,$pixels,0,$pixels.Length)
   [AssassinPixels]::Clean($pixels);[Runtime.InteropServices.Marshal]::Copy($pixels,0,$lock.Scan0,$pixels.Length);$frame.UnlockBits($lock)
   $frame.Save("$folder/${action}_${direction}_$($i.ToString('00')).png",[Drawing.Imaging.ImageFormat]::Png)
   $preview=[Drawing.Bitmap]::new(256,256);$g=[Drawing.Graphics]::FromImage($preview)
   for($y=0;$y -lt 256;$y+=32){$g.DrawImage($forest,[Drawing.Rectangle]::new(0,$y,256,32),[Drawing.Rectangle]::new(0,0,256,32),[Drawing.GraphicsUnit]::Pixel)}
   $g.DrawImageUnscaled($frame,0,0);$g.Dispose()
   $memory=[IO.MemoryStream]::new();$preview.Save($memory,[Drawing.Imaging.ImageFormat]::Gif);$b=$memory.ToArray()
   $n=$b[10] -band 7;$pal=3*(2 -shl $n);$pos=13+$pal
   while($b[$pos] -eq 33){$pos+=2;while($b[$pos] -ne 0){$pos+=1+$b[$pos]};$pos++}
   if($b[$pos] -ne 44){throw 'Invalid GIF frame'}
   if($i -eq 0){$writer.Write([Text.Encoding]::ASCII.GetBytes('GIF89a'));$writer.Write($b,6,7+$pal);$writer.Write([byte[]]@(33,255,11));$writer.Write([Text.Encoding]::ASCII.GetBytes('NETSCAPE2.0'));$writer.Write([byte[]]@(3,1,0,0,0))}
   $writer.Write([byte[]]@(33,249,4,4,$delays[$action][$i],0,0,0));$writer.Write($b,$pos,9);$writer.Write([byte](128 -bor $n -bor ($b[$pos+9] -band 64)));$writer.Write($b,13,$pal);$writer.Write($b,$pos+10,$b.Length-$pos-11)
   $frame.Dispose();$preview.Dispose();$memory.Dispose()
  }
  $writer.Write([byte]59);$writer.Dispose()
 }
 $source.Dispose()
}
$forest.Dispose()
'Export completed.'

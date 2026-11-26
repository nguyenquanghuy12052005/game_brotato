param([string]$Video='C:\Users\ADMIN\Videos\Screen Recordings\Screen Recording 2026-11-13 165813.mp4',[string]$Prefix='VideoRef')
Add-Type -AssemblyName PresentationCore,WindowsBase
$media=New-Object Windows.Media.MediaPlayer
$media.Open([Uri]$Video)
$media.Volume=0
$media.Play()
function Pump([int]$milliseconds){
 $timer=New-Object Windows.Threading.DispatcherTimer
 $timer.Interval=[TimeSpan]::FromMilliseconds($milliseconds)
 $loop=New-Object Windows.Threading.DispatcherFrame
 $timer.Tag=$loop
 $timer.Add_Tick({param($sender,$args) $sender.Stop();$sender.Tag.Continue=$false})
 $timer.Start()
 [Windows.Threading.Dispatcher]::PushFrame($loop)
}
Pump 1500
$media.Pause()
$duration=$media.NaturalDuration.TimeSpan.TotalSeconds
Write-Output "Duration $duration size $($media.NaturalVideoWidth)x$($media.NaturalVideoHeight)"
for($i=0;$i -lt 12;$i++){
 $media.Position=[TimeSpan]::FromSeconds($i*$duration/12)
 $media.Play();Pump 180;$media.Pause()
 $visual=New-Object Windows.Media.DrawingVisual
 $context=$visual.RenderOpen()
 $context.DrawVideo($media,(New-Object Windows.Rect(0,0,640,360)))
 $context.Close()
 $bitmap=New-Object Windows.Media.Imaging.RenderTargetBitmap(640,360,96,96,[Windows.Media.PixelFormats]::Pbgra32)
 $bitmap.Render($visual)
 $encoder=New-Object Windows.Media.Imaging.PngBitmapEncoder
 $encoder.Frames.Add([Windows.Media.Imaging.BitmapFrame]::Create($bitmap))
 $stream=[IO.File]::Create((Join-Path $PWD "docs/Melee/MeleeUltimates/${Prefix}_$i.png"))
 $encoder.Save($stream);$stream.Dispose()
}
$media.Close()

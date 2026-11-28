using System;
using System.IO;
using UnityEngine;
using UnityEngine.Video;

internal sealed class AudioPreviewSourceTake
{
    private readonly VideoPlayer player;
    private bool failed;
    private bool started;

    public bool HasTake { get; private set; }
    public bool Preparing => HasTake && !failed && !player.isPrepared;
    public bool Ready => HasTake && !failed && player.isPrepared;
    public Texture Texture => Ready && player.frame >= 0 ? player.texture : null;
    public static bool Exists(int id) => (id is >=45 and <=48 or >=105 and <=107) &&
                                        File.Exists(AudioPreviewCatalog.Resolve($"reference/{id}.mp4"));

    public AudioPreviewSourceTake(GameObject owner)
    {
        player=owner.AddComponent<VideoPlayer>();
        player.playOnAwake=false;
        player.audioOutputMode=VideoAudioOutputMode.None;
        player.source=VideoSource.Url;
        player.renderMode=VideoRenderMode.APIOnly;
        player.timeReference=VideoTimeReference.ExternalTime;
        player.waitForFirstFrame=true;
        player.skipOnDrop=true;
        player.errorReceived+=(_,_)=>failed=true;
    }

    public void Set(int id, bool hit)
    {
        string path=hit ? AudioPreviewCatalog.Resolve($"reference/{id}.mp4") : null;
        bool requested=path!=null && File.Exists(path);
        if(!requested)
        {
            if(HasTake) player.Stop();
            HasTake=started=false;
            return;
        }

        string url=new Uri(path).AbsoluteUri;
        if(HasTake && !failed && player.url==url && (player.isPrepared || Preparing))
        {
            if(player.isPlaying) player.Pause();
            if(player.isPrepared) player.time=0;
            started=false;
            return;
        }

        player.Stop();
        HasTake=true;
        started=failed=false;
        player.url=url;
        player.externalReferenceTime=0;
        player.Prepare();
    }

    public void Disable() { failed=true; if(player!=null) player.Stop(); }

    public void Sample(float seconds, float speed, bool playing)
    {
        if(!Ready) return;
        double end=Math.Max(0,player.length-.045);
        double clock=Math.Min(end,seconds);
        player.externalReferenceTime=clock;
        if(player.canSetPlaybackSpeed) player.playbackSpeed=speed;
        if(!playing || seconds>=end)
        {
            if(player.isPlaying) player.Pause();
            return;
        }

        if(!started || !player.isPlaying)
        {
            player.time=clock;
            player.Play();
            started=true;
        }
    }
}

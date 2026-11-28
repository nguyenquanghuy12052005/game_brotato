using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public sealed partial class AudioPreviewController : MonoBehaviour
{
    [SerializeField] private AudioPreviewCharacterDriver stage;
    [SerializeField] private AudioPreviewAudioDriver sound;
    private AudioPreviewEntry[] entries;
    private AudioPreviewEntry current;
    private readonly HashSet<int> available = new();
    private int category, version, mode;
    private float master=.68f, music=.75f, sfx=.82f, pitch, zoom=1;
    private bool half, animate=true, vfx=true, dummy=true, muteMusic, muteSfx, automatic;
    private bool reachedEnd;
    private bool pendingSource;
    private float sourceDeadline;
    private float replayAt;
    private string query="";
    private string branchFilter="";
    private string reviewNote="";
    private Vector2 listScroll, infoScroll;
    private int hoverButton=-1;
    private static readonly string[] Categories = {"Nhân vật","Hệ Súng","Hệ Cận chiến","Hệ Pháp sư","Quái vật và Boss",
        "HEXCORE và tiến trình","Sự kiện gameplay","Menu và UI","Môi trường","Nhạc nền"};
    private static readonly string[] CategoryLabels = {"Nhân vật","Hệ Súng","Hệ Cận chiến","Hệ Pháp sư","Quái vật / Boss",
        "HEXCORE & tiến trình","Gameplay & sự kiện","UI","Môi trường","Nhạc nền"};
    private AudioPreviewVariant Variant => current.variants[version];
    private bool Track => current.category=="Nhạc nền";
    private bool Miss => mode==1;
    private bool ReferenceAudio => mode is 2 or -1;
    private bool LoopLayer => mode>=3 && Variant.layers[mode-3].loop;
    private bool AnimatedTake => !ReferenceAudio && !LoopLayer;
    private bool Loop => LoopLayer || Variant.loop && !ReferenceAudio;
    private string ReviewKey => KeyFor(current,version);
    private string CommentKey => ReviewKey+".comment";
    private static string KeyFor(AudioPreviewEntry entry,int index) =>
        $"Brotato.AudioStudio.{entry.id}.{index}"+(string.IsNullOrEmpty(entry.revision)?"":"."+entry.revision);
    private static string BranchName(AudioPreviewEntry entry) => entry.branch.Split('[')[0].TrimEnd();
    private string Status => sound.Problem ?? (pendingSource ? "Đang mở take nguồn" : sound.Loading ? "Đang đọc WAV" : sound.Playing ? (Loop ? "Đang lặp" : "Đang phát") : reachedEnd ? "Đã phát hết" : "Sẵn sàng");

    private IEnumerator Start()
    {
        entries=AudioPreviewCatalog.Load()?.entries;
        if (entries == null || entries.Length==0) yield break;
        foreach (var item in entries)
            if (item.variants?.Length>0 && File.Exists(AudioPreviewCatalog.Resolve(item.variants[0].mix)))
                available.Add(item.id);
        yield return new WaitUntil(() => stage.Ready);
        Select(Array.Find(entries, row=>row.id==74) ?? entries[0], false);
    }

    private void Update()
    {
        if (current == null) return;
        sound.SetMix(master,music,sfx,muteMusic,muteSfx,half ? .5f : 1,pitch);
        stage.SetPlaySpeed(Mathf.Clamp((half?.5f:1f)*Mathf.Pow(2,pitch/12),.25f,2f));
        if(pendingSource && (!stage.WaitingForSource || Time.unscaledTime>=sourceDeadline))
        {
            if(stage.WaitingForSource) stage.SkipSourceWait();
            StartAudio();
        }
        sound.ReadClock();
        bool playing = sound.Playing;
        if (sound.Finished && !reachedEnd)
        {
            reachedEnd=true;
            replayAt=Time.unscaledTime+.38f;
        }
        if (automatic && reachedEnd && Time.unscaledTime>=replayAt) { PlayCue(); return; }
        stage.Sample(AnimatedTake && (playing || reachedEnd) ? sound.Position : 0,
            animate, vfx, dummy, zoom, AnimatedTake && playing);
    }

    private void Select(AudioPreviewEntry item, bool audition)
    {
        sound.StopClip();
        pendingSource=false;
        current=item;
        if (item==null) return;
        int nextCategory=Mathf.Max(0,Array.IndexOf(Categories,item.category));
        if(nextCategory!=category) branchFilter="";
        category=nextCategory;
        version=mode=0;
        reachedEnd=false;
        infoScroll=Vector2.zero;
        reviewNote=PlayerPrefs.GetString(CommentKey);
        PrepareStage();
        if(audition) PlayCue();
    }

    private void SwitchCategory(int index)
    {
        category=index;
        query="";
        branchFilter="";
        listScroll=Vector2.zero;
        Select(Array.Find(entries, item=>item.category==Categories[index]),false);
    }

    private void PrepareStage(bool audition=false) =>
        stage.Prepare(current, Variant, Variant.seconds, vfx, dummy, Miss,
            audition && AnimatedTake && animate && vfx && dummy);

    private string FileReference()
    {
        if (mode==-1) return Variant.prior;
        if (mode==1) return Variant.miss;
        if (mode==2) return current.old;
        if (mode>=3) return Variant.layers[mode-3].file;
        return Variant.mix;
    }

    private void PlayCue()
    {
        reachedEnd=false;
        sound.StopClip();
        PrepareStage(true);
        pendingSource=AnimatedTake && animate && vfx && dummy && stage.WaitingForSource;
        sourceDeadline=Time.unscaledTime+3;
        if(!pendingSource) StartAudio();
    }

    private void StartAudio()
    {
        pendingSource=false;
        sound.StartClip(AudioPreviewCatalog.Resolve(FileReference()),Track,Loop);
    }

    private void StopCue()
    {
        sound.StopClip();
        pendingSource=false;
        reachedEnd=false;
        PrepareStage();
    }

    private void SwitchMode(int next)
    {
        mode=next;
        PlayCue();
    }

    private void SaveReviewNote()
    {
        PlayerPrefs.SetString(CommentKey,reviewNote);
        PlayerPrefs.Save();
    }

    private void StepVariant(int delta)
    {
        version=(version+delta+current.variants.Length)%current.variants.Length;
        reviewNote=PlayerPrefs.GetString(CommentKey);
        mode=0;
        PlayCue();
    }

    private void SetDummy()
    {
        dummy=!dummy;
        // The melee sandbox places its training dummies at the start of each take.
        if (!stage.IsMelee) return;
        if (sound.Playing || sound.Loading || pendingSource) PlayCue();
        else PrepareStage();
    }

    private string ReviewStatus(AudioPreviewEntry item)
    {
        if (!available.Contains(item.id)) return "Thiếu audio";
        int index = item==current ? version : 0;
        return PlayerPrefs.GetInt(KeyFor(item,index)) switch
        {
            1 => "Đạt",
            2 => "Cần chỉnh sửa",
            _ => "Có audio · Cần duyệt",
        };
    }

#if UNITY_EDITOR
    internal bool StudioAvailable => entries?.Length==146 && current!=null;
    internal void StudioSelect(int id, bool audition) => Select(Array.Find(entries,row=>row.id==id), audition);
    internal bool StudioPlaying => sound.Playing;
    internal bool StudioFinished => reachedEnd;
    internal bool StudioMelee => stage.IsMelee;
    internal bool StudioEffect => stage.EffectVisible;
    internal void StudioVariant(int delta) => StepVariant(delta);
    internal void StudioMode(int next) => SwitchMode(next);
    internal void StudioHalf() => half=!half;
    internal void StudioStop() => StopCue();
#endif
}

using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class AudioPreviewCharacterDriver : MonoBehaviour
{
    [SerializeField] private Camera cameraView;
    [SerializeField] private MeleeController melee;
    [SerializeField] private AnimationClip idle, run, death;
    [SerializeField] private Texture2D[] gunSheets, gunPixelSheets;
    [SerializeField] private Material gunMaterial;
    [SerializeField] private Sprite pistol, bullet, dummySprite, playerSprite, mageSprite, staff;
    [SerializeField] private Sprite[] bossEffects;
    [SerializeField] private MageEffectLibrary mageEffects;
    private GunNewSkillArt gunEffects;
    private RenderTexture viewport;
    private AudioPreviewSourceTake sourceTake;
    private SpriteRenderer actor, target, prop;
    private readonly List<SpriteRenderer> effects = new();
    private readonly Texture2D[] biome = new Texture2D[4];
    private readonly Dictionary<Texture2D, Sprite> portraits = new();
    private readonly Vector3 demoCenter = new(65, 0, -10);
    private Vector3 meleeCenter;
    private AudioPreviewEntry entry;
    private AudioPreviewVariant variant;
    private float previous, cueLength, visualLength;
    private bool meleeMode, isCast, isMusic, showDummy, showVfx, missed, usePlayerClip, passiveFired;
    private (float audio,float visual)[] anchors = Array.Empty<(float,float)>();
    private static readonly int[] SourceSkills = {0,-2,-3,3,4,15,5,16,17,6,7,18,8,19,20,9,10,21,11,22,23,12,13,14};

    public RenderTexture Image => viewport;
    public Texture SourceImage { get; private set; }
    public bool WaitingForSource => sourceTake.Preparing;
    public void SkipSourceWait() => sourceTake.Disable();
    public void SetPlaySpeed(float speed) => previewSpeed=speed;
    private float previewSpeed=1;
    public Texture2D Background { get; private set; }
    public bool IsMelee => meleeMode;
    public string Phase => meleeMode ? melee.Phase : "Mốc dựng từ WAV";
    public string VisualNote { get; private set; }
    public bool Ready => melee != null && melee.StudioReady;
    public bool EffectVisible => meleeMode ? melee.HasEffect : effects.Exists(fx => fx.enabled);

    private void Awake()
    {
        sourceTake=new AudioPreviewSourceTake(gameObject);
        melee.AudioStudio = true;
        var authoredCamera = cameraView.transform.position;
        meleeCenter = new Vector3(authoredCamera.x, authoredCamera.y, -10);
        viewport = new RenderTexture(960,540,24, RenderTextureFormat.ARGB32) {name = "Audio Studio View", filterMode = FilterMode.Point};
        viewport.Create();
        cameraView.targetTexture = viewport;
        cameraView.rect = new Rect(0,0,1,1);
        actor = MakeSprite("Diễn viên minh họa", 4);
        target = MakeSprite("Quái mục tiêu", 3);
        prop = MakeSprite("Vũ khí minh họa", 5);
        for (int i = 0; i < 12; i++) effects.Add(MakeSprite("VFX Studio " + i, 8+i));
        gunEffects = gameObject.AddComponent<GunNewSkillArt>();
        gunEffects.Setup(gunSheets, gunPixelSheets, gunMaterial);
        string[] maps = {"forest","moon","inferno","tundra"};
        for (int i=0; i < maps.Length; i++) biome[i] = Resources.Load<Texture2D>("BrotatoMenu/Maps/"+maps[i]);
    }

    private SpriteRenderer MakeSprite(string label, int order)
    {
        var child = new GameObject(label);
        child.transform.SetParent(transform, false);
        var renderer = child.AddComponent<SpriteRenderer>();
        renderer.sortingOrder = order;
        renderer.enabled = false;
        return renderer;
    }

    public void Prepare(AudioPreviewEntry selected, AudioPreviewVariant version, float duration, bool vfx, bool dummy, bool miss,
        bool audition = false)
    {
        entry = selected;
        variant = version;
        cueLength = Mathf.Max(.12f,duration);
        previous = 0;
        passiveFired = false;
        missed = miss;
        showDummy = dummy;
        showVfx = vfx;
        Background = null;
        int id = entry.id;
        bool matchingTake = !miss && (id!=107 || version==selected.variants[0]) &&
                            AudioPreviewSourceTake.Exists(id);
        sourceTake.Set(id,audition && matchingTake);
        SourceImage=null;
        meleeMode = id is >=49 and <= 76 && Ready;
        isMusic = entry.category == "Nhạc nền" || entry.category == "Môi trường";
        isCast = IsGun(id) || id is >=77 and <=107 or 109 or 112 or >=117 and <=125;
        anchors = Array.Empty<(float,float)>();
        if (meleeMode)
        {
            int form = id<=52 ? id-49 : id is >=56 and <=61 or 76 ? 1 :
                id is >=62 and <=67 or 74 ? 2 : id is >=68 and <=73 or 75 ? 3 : 0;
            int skill = id<=52 ? -1 : SourceSkills[id-53];
            if (skill is -2 or -3) melee.StudioPassive(skill == -2 ? 1 : 2,dummy && !miss,vfx);
            else melee.StudioPrepare(form, skill, dummy && !miss, vfx);
            visualLength = skill is -2 or -3 ? cueLength : melee.StudioDuration;
            anchors = Anchors(id);
            VisualNote = id switch
            {
                53 => "Pose Kiếm Lướt gốc · cue mới là Phong Ảnh Trảm. Dùng để so nhịp, hình thức proc khác bản lướt.",
                54 or 55 => "VFX proc Rev2 thật · kích hoạt bằng fixture Studio. Pose đánh thường dùng để minh họa.",
                56 or 62 or 68 or >=74 => "Pose, VFX và vùng trúng từ sân Cận chiến · có neo chính theo mốc WAV.",
                _ => "Pose/VFX của sân Cận chiến. Scale theo thời lượng WAV khi chưa có neo riêng."
            };
            actor.enabled = target.enabled = prop.enabled = false;
            return;
        }

        if (id is >=134 and <=137) Background = biome[id-134];
        else if (id==145) Background = biome[Mathf.Max(0, Array.FindIndex(entry.variants, item=>item==variant))%4];
        else if (id==138) Background = Resources.Load<Texture2D>("BrotatoMenu/Home/Battlefield");
        string portrait = id switch
        {
            >=77 and <=107 => "mage",
            >=112 and <=113 => "bow-snow",
            114 or 115 or 116 => "elite-red",
            >=117 and <=119 => "skeleton",
            >=120 and <=122 => "golem",
            >=123 and <=125 => "yeti",
            >=108 and <=111 => "normal-brown",
            _ => "player"
        };
        actor.sprite = portrait=="player" ? playerSprite : portrait=="mage" ? mageSprite : Portrait(portrait);
        usePlayerClip = portrait=="player";
        actor.enabled = !isMusic && entry.category!="Menu và UI" && actor.sprite!=null;
        prop.sprite = IsGun(id) ? pistol : id is >=77 and <=107 ? staff : null;
        prop.enabled = actor.enabled && prop.sprite!=null;
        target.sprite = id is 109 or 112 or >=117 and <=125 ? playerSprite : dummySprite;
        VisualNote = id switch
        {
            1 or 7 or 142 => "Animation Run/Death/Idle thật của nhân vật.",
            >=117 and <=125 => "Sprite và hình hiệu ứng Boss từ dự án. Chuyển động dựng theo cue, chờ AI/animation tích hợp.",
            >=77 and <=107 => "Form Pháp sư và atlas VFX thật. Động tác, đường bay và va chạm dựng theo mốc WAV.",
            _ when IsGun(id) => "Sprite Súng và atlas sẵn có. Loạt bắn và điểm trúng dựng theo mốc WAV.",
            _ when isMusic => "Ảnh nền theo tài nguyên của dự án, dùng trong lúc nghe loop.",
            _ when entry.category=="Menu và UI" => "Điều khiển UI mẫu để thử di chuột, nhấn và xác nhận.",
            _ => "Sprite thật. Dùng clip gần nhất hoặc sprite đứng yên khi hành động chưa có animation."
        };
        if(matchingTake)
            VisualNote="Take từ scene nguồn · hình/timing của fixture đã ghi. Tắt một lớp hình để xem riêng sân minh họa.";
        else if(id is >=45 and <=48 or >=105 and <=107)
            VisualNote="Bản âm thử. Take nguồn của biến thể hoặc trạng thái này còn thiếu.";
    }

    private Sprite Portrait(string name)
    {
        Texture2D image = Resources.Load<Texture2D>("BrotatoMenu/Portraits/"+name);
        if (image==null) return playerSprite;
        if (!portraits.TryGetValue(image, out Sprite sprite))
        {
            sprite = Sprite.Create(image, new Rect(0,0,image.width,image.height),new Vector2(.5f,.5f),Mathf.Max(image.width,image.height)/1.35f);
            portraits.Add(image,sprite);
        }
        return sprite;
    }

    private (float,float)[] Anchors(int id)
    {
        // Skill catalog inserts Prepare before the authored action list.
        float B(int action, float u = 0) => melee.StudioTimeOf(action+1,u);
        return id switch
        {
            49 or 50 or 51 or 52 => Array.Empty<(float,float)>(),
            53 => new[] {(.225f,.225f),(.30f,.30f),(.45f,.45f),(.525f,.525f),(.675f,.675f)},
            55 => new[] {(.14f,.08f),(.36f,.22f),(.48f,.275f)},
            56 => new[] {(.17f,B(1)),(.28f,B(2)),(.36f,B(3)),(.50f,B(4)),(.59f,B(5))},
            62 => new[] {(.24f,B(1)),(.41f,B(2))},
            68 => new[] {(.13f,B(1)),(.32f,B(2))},
            74 => new[] {(.40f,B(1)),(.53f,B(2)),(.77f,B(3))},
            75 => new[] {(.38f,B(2)),(.97f,B(4)),(1.81f,B(7))},
            76 => new[] {(.29f,B(1)),(.50f,B(1,.43f)),(.76f,B(2)),(.97f,B(2,.43f)),(1.14f,B(3)),(1.30f,B(4)),(1.42f,B(5))},
            _ => Array.Empty<(float,float)>()
        };
    }

    private float MapTime(float time)
    {
        float a=0, b=0;
        foreach(var (audio, visual) in anchors)
        {
            if(time <= audio) return Mathf.Lerp(b,visual,Mathf.InverseLerp(a,audio,time));
            a=audio; b=visual;
        }
        return Mathf.Lerp(b,visualLength,Mathf.InverseLerp(a,Mathf.Max(a+.05f,cueLength),time));
    }

    public void Sample(float time, bool animate, bool vfx, bool dummy, float zoom, bool playing)
    {
        if (entry == null) return;
        sourceTake.Sample(time,previewSpeed,playing);
        SourceImage=animate && vfx && dummy ? sourceTake.Texture : null;
        showDummy = dummy;
        showVfx = vfx;
        foreach (var fx in effects) fx.enabled = false;
        if (meleeMode)
        {
            float visual = MapTime(time);
            if (playing && !passiveFired && (entry.id==54 || entry.id==55 && time>=.14f))
            {
                passiveFired=true;
                melee.StudioFirePassive();
            }
            melee.StudioSample(visual, Mathf.Max(0,visual-previous), showVfx, showDummy && !missed,
                !animate || entry.id is 54 or 55);
            previous = visual;
            cameraView.transform.position = meleeCenter + new Vector3(
                entry.id is 75 ? 2.4f : entry.id is 74 or 76 ? 1.6f : 1.05f,
                entry.id==74 ? -.68f : 0,0);
            cameraView.orthographicSize = (entry.id==74 ? 4.55f : entry.id>=75 ? 4f : 2.8f) / zoom;
            return;
        }
        cameraView.transform.position = demoCenter;
        cameraView.orthographicSize = 2.7f / zoom;
        if (isMusic || entry.category=="Menu và UI") return;
        target.enabled = showDummy && isCast && !missed;
        target.color = Color.white;
        Vector3 origin = demoCenter + new Vector3(-1.38f,-.35f,10);
        Vector3 hit = demoCenter + new Vector3(1.62f,-.32f,10);
        float sample = animate ? time : 0;
        if (usePlayerClip)
        {
            AnimationClip clip = entry.id==7 && playing && animate ? death :
                entry.id is 1 or 142 && playing && animate ? run : idle;
            if(clip!=null) clip.SampleAnimation(actor.gameObject, clip==death ? Mathf.Min(sample,clip.length) : Mathf.Repeat(sample,Mathf.Max(.01f,clip.length)));
        }
        float recoil = playing && animate ? RecentImpulse(time) : 0;
        float travel = entry.id is 1 or 2 or 142 ? Mathf.PingPong(sample*1.15f,1.0f) : 0;
        float jump = entry.id==3 ? Mathf.Sin(Mathf.PI*Mathf.Clamp01((sample-.1f)/.37f))*.76f : 0;
        actor.transform.position = origin+new Vector3(travel-recoil*.29f,jump,0);
        prop.transform.position = actor.transform.position+new Vector3(.44f,.02f,0);
        prop.transform.localScale = Vector3.one*.8f;
        target.transform.position = hit;
        if (!playing || SourceImage!=null || entry.id is >=45 and <=48 or >=105 and <=107) return;
        RenderEvents(time,origin,hit);
    }

    private float RecentImpulse(float time)
    {
        float latest = -1;
        foreach (var layer in variant.layers ?? Array.Empty<AudioPreviewLayer>())
            if (layer.label.StartsWith("Thi triển"))
                foreach (var mark in layer.marks ?? Array.Empty<float>())
                    if (mark<=time) latest=Mathf.Max(mark,latest);
        return latest<0 ? 0 : 1-Mathf.Clamp01((time-latest)/.18f);
    }

    private void RenderEvents(float time, Vector3 origin, Vector3 hit)
    {
        int slot=0;
        int id=entry.id;
        foreach(var layer in variant.layers ?? Array.Empty<AudioPreviewLayer>())
        {
            bool contact = layer.label.StartsWith("Va chạm");
            bool travel = layer.label.StartsWith("Di chuyển");
            bool cast = layer.label.StartsWith("Thi triển");
            if (!contact && !travel && !cast || missed && contact) continue;
            foreach(float mark in layer.marks ?? Array.Empty<float>())
            {
                float age=time-mark;
                if(age < 0 || age > (travel ? .52f : .40f)) continue;
                if(contact && target.enabled && age<.085f) target.color = new Color(1,.49f,.34f);
                if(!showVfx || slot>=effects.Count) continue;
                Sprite sprite;
                if(IsGun(id))
                    sprite = travel && id<30 ? bullet : gunEffects.Frame(GunClip(id,contact),Mathf.FloorToInt(age*36));
                else if(id is >=77 and <=107 && mageEffects != null)
                    sprite = mageEffects.GetFrame(MageClip(id,contact,travel),Mathf.FloorToInt(age*40));
                else if(id is >=117 and <=125 && bossEffects?.Length==9)
                    sprite = bossEffects[id-117];
                else continue;
                if(sprite==null) continue;
                var effect=effects[slot++];
                effect.enabled = true;
                effect.sprite = sprite;
                effect.color = new Color(1,1,1,1-Mathf.Clamp01((age-.19f)/.34f));
                effect.transform.position = contact ? hit : travel ? Vector3.Lerp(origin+Vector3.right*.55f,hit,age/.46f) : origin+Vector3.right*.57f;
                effect.transform.localScale = Vector3.one*(1.15f/Mathf.Max(.1f,sprite.bounds.size.x));
            }
        }
    }

    private static bool IsGun(int id) => id is >=20 and <=48 || id==115 || id==144;

    private static GunNewSkillArt.Clip GunClip(int id, bool hit) => id switch
    {
        23 or 29 or 33 or 46 => hit ? GunNewSkillArt.Clip.PlasmaExplosion : GunNewSkillArt.Clip.Rocket,
        30 => GunNewSkillArt.Clip.Sweep,
        31 => GunNewSkillArt.Clip.Fireball,
        32 => GunNewSkillArt.Clip.Breath,
        34 => GunNewSkillArt.Clip.ChainLightning,
        35 => GunNewSkillArt.Clip.HealRing,
        36 => GunNewSkillArt.Clip.Quake,
        37 => GunNewSkillArt.Clip.BallLightning,
        38 => GunNewSkillArt.Clip.SkyCircle,
        39 => GunNewSkillArt.Clip.Shield,
        40 => GunNewSkillArt.Clip.Saw,
        41 => GunNewSkillArt.Clip.HotMuzzle,
        42 => GunNewSkillArt.Clip.DecayHit,
        43 => GunNewSkillArt.Clip.Portal,
        44 => GunNewSkillArt.Clip.AnchorSnap,
        45 => GunNewSkillArt.Clip.BlizzardVortex,
        47 => GunNewSkillArt.Clip.EnergyCore,
        48 => hit ? GunNewSkillArt.Clip.CageCollapse : GunNewSkillArt.Clip.CagePulse,
        _ => hit ? GunNewSkillArt.Clip.Impact : GunNewSkillArt.Clip.HotMuzzle
    };

    private static MageEffectId MageClip(int id, bool hit, bool travel) => id switch
    {
        78 => MageEffectId.Heal,
        79 => MageEffectId.Shockwave,
        80 => MageEffectId.Gravity,
        >=81 and <=86 => hit ? MageEffectId.Explosion : MageEffectId.Fireball,
        >=87 and <=92 => hit ? MageEffectId.Freeze : MageEffectId.IceBreath,
        >=93 and <=98 => hit ? MageEffectId.Smite : MageEffectId.LightPillar,
        >=99 and <=104 => hit ? MageEffectId.Curse : MageEffectId.DarkMissile,
        105 => hit ? MageEffectId.Implosion : MageEffectId.TimeStop,
        106 => MageEffectId.StarFall,
        107 => travel ? MageEffectId.ArcaneMissiles : MageEffectId.ArcaneOrb,
        _ => hit ? MageEffectId.Smite : MageEffectId.ArcaneMissiles
    };

    private void OnDestroy()
    {
        sourceTake?.Disable();
        foreach (var sprite in portraits.Values) Destroy(sprite);
        if (viewport!=null) {viewport.Release(); Destroy(viewport);}
    }
}

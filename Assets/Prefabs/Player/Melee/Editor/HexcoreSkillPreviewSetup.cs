using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[InitializeOnLoad]
public static partial class HexcoreSkillPreviewSetup
{
    private const string PlayerRoot = "Assets/Prefabs/Player/Melee/";
    private const string SharedVfx = PlayerRoot + "Shared/VFX/";
    private const string ScenePath = "Assets/Scenes/HexcoreSkillPreview.unity";
    private const string Verifying = "HexcoreSkillPreview.Verifying";
    private static readonly HashSet<string> Phases = new();
    private static int verificationStep;
    private static int directionCheck;
    private static int captureIndex;
    private static float nextCapture;
    private static bool sawImpactCue;
    private static int timedIndex = -1;
    private static double timedStarted, fastDuration;
    private static bool timedCamera, timedCopies;
    private static readonly float[] MixBlock = new float[512];
    private static readonly List<string> MixResults = new();
    private static float mixPeak;
    private static int mixAudibleReads;
    private static bool timedComplete;
    private static bool timelineStarted;
    private static readonly (int form, int skill, int facing)[] TimedVariants =
    {
        (0, 2, 2), (3, 12, 2), (2, 13, 6), (1, 14, 2),
        (0, 2, 1), (2, 12, 4), (1, 13, 0), (3, 14, 0)
    };
    private static double deadline;

    static HexcoreSkillPreviewSetup()
    {
        EditorApplication.update += VerifyPlayback;
    }

    [MenuItem("Tools/Brotato/Create HEXCORE Skill Preview")]
    public static void Build()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode) return;
        var form = Load<GameObject>(PlayerRoot + "Swordsman/SwordsmanVisual.prefab");
        var enemy = Load<GameObject>("Assets/Prefabs/Enemy/Normal/Brown/Enemy_Normal_Brown.prefab").GetComponentInChildren<SpriteRenderer>();
        var lineMaterial = Load<Material>("Assets/Prefabs/Player/PlayerUnlit.mat");
        var basic = LoadFrames("Basic/Basic_East_", 12);
        var q1 = LoadFrames("Q1/Q1_East_", 12);
        if (!Application.isBatchMode && !EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;

        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        var camera = new GameObject("Preview Camera", typeof(Camera)).GetComponent<Camera>();
        camera.transform.position = new Vector3(-.95f, .6f, -10);
        camera.orthographic = true;
        camera.orthographicSize = 2.4f;
        camera.backgroundColor = new Color(.12f, .16f, .15f);
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.gameObject.AddComponent<AudioListener>();

        var actor = ((GameObject)PrefabUtility.InstantiatePrefab(form, scene)).GetComponent<Animator>();
        var dummy = new GameObject("Target Dummy [placeholder]").AddComponent<SpriteRenderer>();
        dummy.sprite = enemy.sprite;
        dummy.sharedMaterial = enemy.sharedMaterial;
        dummy.transform.position = new Vector3(.16f, .575f, 0);
        dummy.transform.localScale = Vector3.one * .65f;
        dummy.sortingOrder = -1;

        var impact = CreateCue("Impact cue", lineMaterial, new Color(1, .82f, .45f));
        var swordTrail = CreateCue("Sword trail", lineMaterial, new Color(.72f, .91f, 1, .78f));
        var dust = CreateCue("Foot dust", lineMaterial, new Color(.82f, .68f, .49f, .64f));

        var preview = new GameObject("HEXCORE Skill Preview").AddComponent<MeleeController>();
        var data = new SerializedObject(preview);
        data.FindProperty("swordsman").objectReferenceValue = actor;
        data.FindProperty("dummy").objectReferenceValue = dummy;
        data.FindProperty("previewCamera").objectReferenceValue = camera;
        data.FindProperty("impact").objectReferenceValue = impact;
        data.FindProperty("swordTrail").objectReferenceValue = swordTrail;
        data.FindProperty("dust").objectReferenceValue = dust;
        var sounds = data.FindProperty("cues");
        sounds.arraySize = MeleeController.AudioCueCount;
        for (int i = 0; i < sounds.arraySize; i++)
        {
            string file = MeleeController.AudioCueName(i);
            var clip = Load<AudioClip>($"{PlayerRoot}Shared/Audio/{file}.wav");
            if (clip.samples == 0 || clip.channels != 1)
                throw new InvalidOperationException("Invalid audio cue: " + file);
            sounds.GetArrayElementAtIndex(i).objectReferenceValue = clip;
        }
        data.FindProperty("qiWavePrefab").objectReferenceValue = Load<GameObject>(SharedVfx + "QiWave/QiWaveVisual.prefab");
        data.FindProperty("cleanupMaterial").objectReferenceValue = Load<Material>(SharedVfx + "MeleeSkills/CleanVfx.mat");
        data.FindProperty("goldMaterial").objectReferenceValue = Load<Material>(SharedVfx + "MeleeUltimates/VanAnh/HexcoreGoldVfx.mat");
        AssignFrames(data.FindProperty("basicEast"), basic);
        AssignFrames(data.FindProperty("q1East"), q1);
        AssignOptional(data.FindProperty("q1West"), FrameRoot + "Q1/Q1_West_");
        AssignOptional(data.FindProperty("q1South"), FrameRoot + "Q1/Q1_South_");
        var q1Additional = new Sprite[8 * 12];
        string[] dashDirections = { "North", "NE", "", "SE", "", "SW", "", "NW" };
        for (int direction = 0; direction < dashDirections.Length; direction++)
        {
            if (dashDirections[direction].Length == 0) continue;
            string prefix = FrameRoot + "Q1/Q1_" + dashDirections[direction] + "_";
            if (AssetDatabase.LoadAssetAtPath<Sprite>(prefix + "00.png") == null) continue;
            var frames = new Sprite[12];
            for (int frame = 0; frame < frames.Length; frame++) frames[frame] = Load<Sprite>(prefix + $"{frame:00}.png");
            Array.Copy(frames, 0, q1Additional, direction * 12, 12);
            SaveSpriteClip(prefix.TrimEnd('_') + ".anim", frames, .075f);
        }
        AssignFrames(data.FindProperty("q1Additional"), q1Additional);
        SaveSpriteClip(FrameRoot + "Q1/Q1_East.anim", q1, .075f);
        AssignRoster(data);
        data.ApplyModifiedPropertiesWithoutUndo();
        AssetDatabase.SaveAssets();
        if (!EditorSceneManager.SaveScene(scene, ScenePath))
            throw new InvalidOperationException("Could not save HEXCORE preview scene.");
    }

    private const string FrameRoot = PlayerRoot + "Swordsman/HexcorePreview/";

    private static void AssignRoster(SerializedObject data)
    {
        string[] names = { "Swordsman", "Assassin", "DaoWarrior", "QiSwordsman" };
        string[] directions = { "NE", "SE", "SW", "NW" };
        var forms = data.FindProperty("forms");
        forms.arraySize = names.Length;
        for (int form = 0; form < names.Length; form++)
        {
            var entry = forms.GetArrayElementAtIndex(form);
            string root = PlayerRoot + names[form] + "/";
            var prefab = Load<GameObject>(root + names[form] + "Visual.prefab");
            var animator = prefab.GetComponent<Animator>();
            if (animator == null || animator.runtimeAnimatorController == null || prefab.GetComponent<SpriteRenderer>() == null)
                throw new InvalidOperationException("Incomplete form: " + names[form]);
            entry.FindPropertyRelative("prefab").objectReferenceValue = prefab;
            var frames = new Sprite[20];
            for (int i = 0; i < frames.Length; i++)
                frames[i] = AssetDatabase.LoadAssetAtPath<Sprite>(
                    root + "HexcorePreview/Locomotion/" + directions[i / 5] + "_" + i % 5 + ".png");
            for (int i = 0; i < frames.Length; i++)
                if ((frames[i] != null) != (frames[i / 5 * 5] != null))
                    throw new InvalidOperationException("Incomplete direction: " + names[form] + " " + directions[i / 5]);
            for (int i = 0; i < frames.Length; i += 5)
            {
                if (frames[i] == null) continue;
                var run = new Sprite[4];
                Array.Copy(frames, i + 1, run, 0, run.Length);
                string clips = root + "HexcorePreview/Locomotion/";
                SaveSpriteClip(clips + "Idle_" + directions[i / 5] + ".anim", new[] { frames[i] }, .67f, true);
                SaveSpriteClip(clips + "Run_" + directions[i / 5] + ".anim", run, .125f, true);
            }
            AssignFrames(entry.FindPropertyRelative("locomotion"), frames);
            var idle = new Sprite[16];
            for (int direction = 0; direction < directions.Length; direction++)
            {
                var sequence = new Sprite[4];
                string prefix = root + "HexcorePreview/IdleLoops/Idle_" + directions[direction] + "_";
                if (AssetDatabase.LoadAssetAtPath<Sprite>(prefix + "00.png") == null) continue;
                for (int frame = 0; frame < sequence.Length; frame++) sequence[frame] = Load<Sprite>(prefix + $"{frame:00}.png");
                Array.Copy(sequence, 0, idle, direction * 4, 4);
                SaveSpriteClip(prefix.TrimEnd('_') + ".anim", sequence, .17f, true);
            }
            AssignFrames(entry.FindPropertyRelative("diagonalIdle"), idle);
            AssignOptional(entry.FindPropertyRelative("turns"), root + "HexcorePreview/Turn/Turn_East_", 16);
            AssignOptional(entry.FindPropertyRelative("basic"), root + "HexcorePreview/Basic/Basic_East_");
            AssignOptional(entry.FindPropertyRelative("basicWest"), root + "HexcorePreview/Basic/Basic_West_");
            AssignOptional(entry.FindPropertyRelative("basicNorth"), root + "HexcorePreview/Basic/Basic_North_");
            AssignOptional(entry.FindPropertyRelative("basicSouth"), root + "HexcorePreview/Basic/Basic_South_");
            var diagonalCombos = new Sprite[32];
            for (int direction = 0; direction < directions.Length; direction++)
            {
                string prefix = root + "HexcorePreview/Basic/Basic_" + directions[direction] + "_";
                if (AssetDatabase.LoadAssetAtPath<Sprite>(prefix + "00.png") == null) continue;
                var sequence = new Sprite[8];
                for (int frame = 0; frame < sequence.Length; frame++) sequence[frame] = Load<Sprite>(prefix + $"{frame:00}.png");
                Array.Copy(sequence, 0, diagonalCombos, direction * 8, 8);
                SaveSpriteClip(prefix.TrimEnd('_') + ".anim", sequence, .9f / 8);
            }
            AssignFrames(entry.FindPropertyRelative("basicDiagonal"), diagonalCombos);
            AssignOptional(entry.FindPropertyRelative("slam"), root + "HexcorePreview/Slam/Slam_East_");
            AssignOptional(entry.FindPropertyRelative("hero"), root + "HexcorePreview/Hero/Hero_East_", 16);
            string west = root + "HexcorePreview/HeroWest/";
            if (AssetDatabase.LoadAssetAtPath<Sprite>(west + "HeroWest_West_00.png") != null)
            {
                var all = new Sprite[24];
                for (int frame = 0; frame < all.Length; frame++) all[frame] = Load<Sprite>(west + $"HeroWest_West_{frame:00}.png");
                AssignFrames(entry.FindPropertyRelative("heroWest"), all);
                for (int core = 0; core < 3; core++)
                {
                    var eight = new Sprite[8];
                    Array.Copy(all, core * 8, eight, 0, 8);
                    SaveSpriteClip(west + $"R{core+1}_West.anim", eight, .11f);
                }
            }
        }

        var effects = data.FindProperty("effects");
        effects.arraySize = MeleeController.SkillCount;
        var signatures = new Sprite[MeleeController.SkillCount * 4];
        var continuousEast = new Sprite[MeleeController.SkillCount * 12];
        for (int i = 0; i < effects.arraySize; i++)
        {
            string label = MeleeController.SkillName(i);
            if (i != 0 && label[0] != 'R')
            {
                int form = MeleeController.PrimaryForm(i);
                string art = PlayerRoot + names[form] + "/HexcorePreview/Signature" + label[0] + "/";
                int row = label[1] - (label[0] == 'Q' ? '2' : '1');
                var keys = new Sprite[4];
                for (int key = 0; key < keys.Length; key++)
                    keys[key] = AssetDatabase.LoadAssetAtPath<Sprite>(art + $"Signature{label[0]}_East_{row * 4 + key:00}.png");
                if (keys[0] != null)
                {
                    Array.Copy(keys, 0, signatures, i * 4, keys.Length);
                    SaveSpriteClip(art + label[..2] + "_East_KeyPoses.anim", keys, .15f);
                }
                string prefix = PlayerRoot + names[form] + "/HexcorePreview/Continuous/" + label[..2] + "_East";
                if (AssetDatabase.LoadAssetAtPath<Sprite>(prefix + "_00.png") != null)
                {
                    var sequence = new Sprite[12];
                    for (int frame = 0; frame < 12; frame++) sequence[frame] = Load<Sprite>(prefix + $"_{frame:00}.png");
                    Array.Copy(sequence, 0, continuousEast, i * 12, 12);
                    SaveSpriteClip(prefix + ".anim", sequence, .075f);
                }
            }
            string effect = MeleeController.EffectPath(i);
            if (string.IsNullOrEmpty(effect)) continue;
            string name = Path.GetFileName(effect);
            string path = effect == "QiSwordsman/QiWave"
                ? SharedVfx + "QiWave/QiWaveVisual.prefab"
                : SharedVfx + effect + "/" + name + "Visual.prefab";
            effects.GetArrayElementAtIndex(i).objectReferenceValue = Load<GameObject>(path);
        }
        AssignFrames(data.FindProperty("signatures"), signatures);
        AssignFrames(data.FindProperty("continuousEast"), continuousEast);
        AssignDirectionalTakes(data.FindProperty("directionalTakes"), names);
    }

    private static void AssignDirectionalTakes(SerializedProperty property, string[] formNames)
    {
        string[] headings = { "N", "NE", "E", "SE", "S", "SW", "W", "NW" };
        property.arraySize = 0;
        AddSpinTakes(property, headings);
        for (int form = 0; form < formNames.Length; form++)
        {
            string root = PlayerRoot + formNames[form] + "/HexcorePreview/Directional/";
            for (int skill = 1; skill < MeleeController.SkillCount; skill++)
            {
                if (!MeleeController.Compatible(skill, form)) continue;
                for (int direction = 0; direction < headings.Length; direction++)
                {
                    string prefix = root + MeleeController.SkillName(skill)[..2] + "_" + headings[direction];
                    if (AssetDatabase.LoadAssetAtPath<Sprite>(prefix + "_00.png") == null) continue;
                    var frames = new Sprite[8];
                    for (int frame = 0; frame < frames.Length; frame++)
                        frames[frame] = Load<Sprite>(prefix + $"_{frame:00}.png");
                    AddDirectionalTake(property, form, skill, direction, prefix, frames);
                }
            }
        }
    }

    private static void AddSpinTakes(SerializedProperty property, string[] headings)
    {
        const string root = FrameRoot + "SpinAtlas/";
        if (AssetDatabase.LoadAssetAtPath<Sprite>(root + "SpinAtlas_East_00.png") == null) return;
        var atlas = LoadFrames("SpinAtlas/SpinAtlas_East_", 24);
        string[] cardinal = { "Up", "Right", "Down", "Left" };
        for (int direction = 0; direction < 8; direction++)
        {
            if (direction == 2) continue;
            int start = (direction + 6) % 8;
            string heading = headings[direction];
            string idle = direction % 2 == 0
                ? PlayerRoot + $"Swordsman/Frames/Idle/{cardinal[direction / 2]}/Idle_{cardinal[direction / 2]}_00.png"
                : FrameRoot + $"IdleLoops/Idle_{heading}_00.png";
            var frames = new Sprite[12];
            frames[0] = Load<Sprite>(idle);
            frames[1] = atlas[start];
            for (int turn = 0; turn < 8; turn++) frames[turn + 2] = atlas[8 + (start + turn) % 8];
            frames[10] = atlas[8 + start];
            frames[11] = atlas[16 + start];
            AddDirectionalTake(property, 0, 1, direction, root + "Q2_" + heading, frames);
        }
    }

    private static void AddDirectionalTake(SerializedProperty property, int form, int skill, int direction,
        string prefix, Sprite[] frames)
    {
        int index = property.arraySize;
        property.arraySize++;
        var entry = property.GetArrayElementAtIndex(index);
        entry.FindPropertyRelative("form").intValue = form;
        entry.FindPropertyRelative("skill").intValue = skill;
        entry.FindPropertyRelative("direction").intValue = direction;
        AssignFrames(entry.FindPropertyRelative("frames"), frames);
        var anchors = AssetDatabase.LoadAssetAtPath<TextAsset>(prefix + "_anchors.json");
        if (anchors != null && !MeleeController.ValidateAnchors(anchors, frames))
            throw new InvalidOperationException("Invalid measured anchors: " + prefix);
        entry.FindPropertyRelative("anchors").objectReferenceValue = anchors;
        SaveSpriteClip(prefix + ".anim", frames, .1f);
    }

    private static void AssignOptional(SerializedProperty field, string prefix, int count = 12)
    {
        if (AssetDatabase.LoadAssetAtPath<Sprite>(prefix + "00.png") == null) return;
        var frames = new Sprite[count];
        for (int i = 0; i < frames.Length; i++)
            frames[i] = Load<Sprite>(prefix + i.ToString("00") + ".png");
        AssignFrames(field, frames);
        SaveSpriteClip(prefix.TrimEnd('_') + ".anim", frames, .075f);
    }

    private static void SaveSpriteClip(string path, Sprite[] sprites, float step, bool loop = false)
    {
        var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(path);
        if (clip == null)
        {
            clip = new AnimationClip();
            AssetDatabase.CreateAsset(clip, path);
        }
        clip.frameRate = 40;
        var keys = new ObjectReferenceKeyframe[sprites.Length + 1];
        for (int i = 0; i < keys.Length; i++)
        {
            var sprite = sprites[i == sprites.Length ? (loop ? 0 : i - 1) : i];
            if (sprite == null || sprite.pixelsPerUnit != 120 || sprite.pivot != sprite.rect.size * .5f)
                throw new InvalidOperationException("Invalid sprite import in " + path);
            keys[i] = new ObjectReferenceKeyframe { time = i * step, value = sprite };
        }
        var binding = EditorCurveBinding.PPtrCurve("", typeof(SpriteRenderer), "m_Sprite");
        AnimationUtility.SetObjectReferenceCurve(clip, binding, keys);
        var settings = AnimationUtility.GetAnimationClipSettings(clip);
        settings.loopTime = loop;
        AnimationUtility.SetAnimationClipSettings(clip, settings);
        EditorUtility.SetDirty(clip);
    }

    private static Sprite[] LoadFrames(string prefix, int count)
    {
        var frames = new Sprite[count];
        for (int i = 0; i < count; i++) frames[i] = Load<Sprite>(FrameRoot + prefix + i.ToString("00") + ".png");
        return frames;
    }

    private static void AssignFrames(SerializedProperty property, Sprite[] frames)
    {
        property.arraySize = frames.Length;
        for (int i = 0; i < frames.Length; i++) property.GetArrayElementAtIndex(i).objectReferenceValue = frames[i];
    }

    private static LineRenderer CreateCue(string label, Material material, Color color)
    {
        var cue = new GameObject(label).AddComponent<LineRenderer>();
        cue.sharedMaterial = material;
        cue.sortingOrder = 10;
        cue.startColor = cue.endColor = color;
        cue.widthCurve = AnimationCurve.Linear(0, 1, 1, .2f);
        cue.enabled = false;
        return cue;
    }

    // Batch entry point. Omit -quit so the Player Loop can run.
    public static void BuildAndVerify()
    {
        Build();
        EnterVerification();
    }

    public static void VerifyExisting()
    {
        EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        EnterVerification();
    }

    public static void VerifyV2()
    {
        EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        SessionState.SetBool(VerifyingV2, true);
        EnterVerification();
    }

    public static void VerifyV21Samples()
    {
        EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        SessionState.SetBool(VerifyingV21, true);
        EnterVerification();
    }

    private static void EnterVerification()
    {
        SessionState.SetBool(Verifying, true);
        EditorApplication.EnterPlaymode();
    }

    private static void VerifyPlayback()
    {
        if (!EditorApplication.isPlaying || !SessionState.GetBool(Verifying, false)) return;
        try
        {
            var preview = UnityEngine.Object.FindFirstObjectByType<MeleeController>();
            var actor = UnityEngine.Object.FindFirstObjectByType<Animator>();
            var spark = GameObject.Find("Impact cue").GetComponent<LineRenderer>();
            if (deadline == 0) deadline = EditorApplication.timeSinceStartup + 220;
            if (EditorApplication.timeSinceStartup > deadline)
                throw new InvalidOperationException($"Preview timed out at step {verificationStep}, phase {preview.Phase}.");
            if (preview == null || actor == null || preview.CurrentSprite == null)
                throw new InvalidOperationException("Preview actor or initial sprite missing.");
            if (SessionState.GetBool(VerifyingV21, false))
            {
                VerifyPolishSamples(preview);
                Finish(0);
                return;
            }
            if (SessionState.GetBool(VerifyingV2, false))
            {
                VerifyV2Catalog(preview);
                Finish(0);
                return;
            }
            if (SessionState.GetBool(VerifyingInteraction, false))
            {
                if (!AdvanceInteraction(preview)) return;
                VerifyCatalog(preview);
                Debug.Log("HEXCORE INTERACTION AND CATALOG PASSED");
                Finish(0);
                return;
            }
            if (!timelineStarted)
            {
                timelineStarted = true;
                preview.Select(MeleeController.Preview.Chain);
            }
            if (verificationStep < 2)
            {
                ReadMix();
                Phases.Add(preview.Phase);
                if (verificationStep == 0 && preview.Elapsed >= nextCapture &&
                    Array.IndexOf(Environment.GetCommandLineArgs(), "-hexcoreCapture") >= 0)
                {
                    CaptureFrame(UnityEngine.Object.FindFirstObjectByType<Camera>());
                    nextCapture += .075f;
                }
                if (spark.enabled && preview.CurrentSprite.name != "Q1_East_07")
                    throw new InvalidOperationException("Impact was outside the authored contact frame.");
                sawImpactCue |= spark.enabled;
                if (preview.Phase != "Idle · E") return;
                if (!sawImpactCue) throw new InvalidOperationException("Impact cue never appeared.");
                if (!preview.PlayedDashCues) throw new InvalidOperationException("Missing foot, dash, swing or contact audio events.");
                RecordMix(0, 0, 2, verificationStep == 1);
                foreach (string phase in new[] { "Run · E", "Basic · Chém ngang", "Basic · Chém ngược",
                    "Anticipation", "Foot Push", "Dash", "Sword Swing", "Impact", "Braking", "Recovery" })
                    if (!Phases.Contains(phase)) throw new InvalidOperationException("Missing phase: " + phase);
                if (Mathf.Abs(actor.transform.position.x - .9f) > .001f)
                    throw new InvalidOperationException("Run after Q1 did not continue from the dash endpoint.");
                preview.Replay();
                if (spark.enabled || actor.transform.position.x > -2.8f)
                    throw new InvalidOperationException("Replay did not reset VFX and position.");
                Phases.Clear();
                sawImpactCue = false;
                verificationStep++;
                if (verificationStep == 1) preview.Select(MeleeController.Preview.Chain, slow: true);
                return;
            }
            if (directionCheck < 8)
            {
                bool available = preview.Select(MeleeController.Preview.Idle, directionCheck);
                if (available)
                {
                    var idle = preview.CurrentSprite;
                    if (!preview.Select(MeleeController.Preview.Run, directionCheck) ||
                        preview.CurrentSprite == null || preview.CurrentSprite == idle)
                        throw new InvalidOperationException("Missing run sprite at direction " + directionCheck);
                }
                Debug.Log($"HEXCORE LOCOMOTION {directionCheck}: {(available ? "OK" : "art missing")}");
                directionCheck++;
                return;
            }
            if (!timedComplete)
            {
                if (!AdvanceTimedCases(preview)) return;
                timedComplete = true;
            }
            if (Array.IndexOf(Environment.GetCommandLineArgs(), "-hexcoreVisualReview") >= 0 && !RecordCoreMovies(preview)) return;
            VerifyCatalog(preview);
            MixResults.Insert(0, "form,skill_index,direction,speed,listener_peak,reads_with_signal");
            File.WriteAllLines("Library/HexcoreAudioReview.csv", MixResults);
            Debug.Log("HEXCORE PREVIEW PLAY MODE PASSED: Q1, Q3 and all three hero cores at 1x and 0.5x; replay and full catalog.");
            Finish(0);
        }
        catch (Exception exception)
        {
            Debug.LogException(exception);
            Finish(1);
        }
    }

    private static bool AdvanceTimedCases(MeleeController preview)
    {
        if (timedIndex >= 0)
        {
            ReadMix();
            timedCamera |= preview.CameraMoving;
            timedCopies |= preview.CopiesActive;
            Phases.Add(preview.Phase);
            if (preview.Phase != "Idle · E") return false;
            int skill = TimedVariants[timedIndex / 2].skill;
            bool impulse = Phases.Contains("Hit") || Phases.Contains("Land") || Phases.Contains("Burst");
            if (!Phases.Contains("Prepare") || !Phases.Contains("Recover") || !impulse ||
                !timedCamera || !preview.PlayedSkillAudio || skill == 14 && !timedCopies)
                throw new InvalidOperationException("Timed phases or feedback missing for " + MeleeController.SkillName(skill));
            double duration = EditorApplication.timeSinceStartup - timedStarted;
            if (timedIndex % 2 == 0) fastDuration = duration;
            else if (duration <= fastDuration * 1.35)
                throw new InvalidOperationException("0.5x unexpectedly replayed as fast as 1x.");
            preview.Replay();
            if (preview.HasEffect || preview.CopiesActive || preview.AudioActive || preview.CameraMoving)
                throw new InvalidOperationException("Replay did not clear timed result.");
            var tested = TimedVariants[timedIndex / 2];
            RecordMix(tested.form, tested.skill, tested.facing, timedIndex % 2 == 1);
            Debug.Log($"HEXCORE TIMED QA: form {tested.form + 1}, direction {tested.facing}, {MeleeController.SkillName(skill)} {(timedIndex % 2 == 0 ? "1x" : "0.5x")} in {duration:F2}s");
        }
        timedIndex++;
        if (timedIndex >= TimedVariants.Length * 2) return true;
        var variant = TimedVariants[timedIndex / 2];
        if (!preview.SelectSkill(variant.form, variant.skill, 2, variant.facing, true, timedIndex % 2 == 1))
            throw new InvalidOperationException("Missing timed test selection.");
        Phases.Clear();
        timedCamera = false;
        timedCopies = false;
        timedStarted = EditorApplication.timeSinceStartup;
        return false;
    }

    private static void ReadMix()
    {
        AudioListener.GetOutputData(MixBlock, 0);
        float peak = 0;
        foreach (float sample in MixBlock) peak = Mathf.Max(peak, Mathf.Abs(sample));
        mixPeak = Mathf.Max(mixPeak, peak);
        if (peak > .0001f) mixAudibleReads++;
    }

    private static void RecordMix(int form, int skill, int direction, bool slow)
    {
        MixResults.Add(FormattableString.Invariant($"{form + 1},{skill + 1},{direction},{(slow ? .5f : 1f)},{mixPeak:F6},{mixAudibleReads}"));
        mixPeak = 0;
        mixAudibleReads = 0;
    }

    private static void VerifyCatalog(MeleeController preview)
    {
        if (MeleeController.SkillCount != 24) throw new InvalidOperationException("Expected 24 skills.");
        bool capture = Array.IndexOf(Environment.GetCommandLineArgs(), "-hexcoreCapture") >= 0;
        bool captureAll = Array.IndexOf(Environment.GetCommandLineArgs(), "-hexcoreCaptureAll") >= 0;
        var camera = UnityEngine.Object.FindFirstObjectByType<Camera>();
        int combinations = 0;
        int cases = 0;
        int missing = 0;
        string[] headings = { "N", "NE", "E", "SE", "S", "SW", "W", "NW" };
        string[] dashNames = { "North", "NE", "East", "SE", "South", "SW", "West", "NW" };
        var results = new List<string> { "form,skill,heading,art_status,different_sprites,vfx_samples,reset,measured_frames,chain" };
        var formResults = new List<string> { "form,heading,idle_run,basic_combo,basic_different_sprites,turn_to_next" };
        bool captureMatrix = Array.IndexOf(Environment.GetCommandLineArgs(), "-hexcoreMatrixCapture") >= 0;
        bool visualReview = Array.IndexOf(Environment.GetCommandLineArgs(), "-hexcoreVisualReview") >= 0;
        int captureSkill = -1;
        var arguments = Environment.GetCommandLineArgs();
        int scope = Array.IndexOf(arguments, "-hexcoreVisualSkill");
        if (scope >= 0)
        {
            if (scope + 1 >= arguments.Length || !int.TryParse(arguments[scope + 1], out int catalogIndex) ||
                catalogIndex < 1 || catalogIndex > MeleeController.SkillCount)
                throw new ArgumentException("Specify a skill number between 1 and 24.");
            captureSkill = catalogIndex - 1;
        }
        for (int form = 0; form < 4; form++)
        {
            int directions = 0;
            for (int direction = 0; direction < 8; direction++)
            {
                bool locomotion = preview.SelectLocomotion(form, direction, false);
                if (locomotion)
                {
                    var idle = preview.CurrentSprite;
                    if (!preview.SelectLocomotion(form, direction, true) || preview.CurrentSprite == null ||
                        preview.CurrentSprite == idle) throw new InvalidOperationException("Missing locomotion sprite.");
                    directions++;
                }
                bool combo = preview.SelectBasic(form, direction);
                var comboSprites = new HashSet<Sprite>();
                if (combo)
                {
                    int expected = MeleeController.BasicFrameCount(direction);
                    for (int frame = 0; frame < expected; frame++)
                    {
                        preview.ReviewAt((frame + .5f) * .9f / expected, true);
                        comboSprites.Add(preview.CurrentSprite);
                    }
                    if (comboSprites.Count != expected) throw new InvalidOperationException("Incomplete Basic combo.");
                    if (captureMatrix)
                        for (int frame = 0; frame < 12; frame++)
                        {
                            preview.ReviewAt((frame + .5f) * .075f, true);
                            CaptureFrame(camera, $"Library/HexcoreMatrix/Basic-{form+1}-{headings[direction]}", $"{frame:000}", 320, 180);
                        }
                }
                bool turn = preview.SelectTurn(form, direction, (direction + 1) % 8);
                if (turn)
                {
                    var turnSprites = new HashSet<Sprite>();
                    for (int frame = 0; frame < 8; frame++)
                    {
                        preview.ReviewAt(frame * .075f, false);
                        turnSprites.Add(preview.CurrentSprite);
                        if (captureMatrix)
                            CaptureFrame(camera, $"Library/HexcoreMatrix/Turn-{form+1}-{headings[direction]}", $"{frame:000}", 256, 144);
                    }
                    if (turnSprites.Count < 2) throw new InvalidOperationException("Turn shows a single pose.");
                }
                formResults.Add($"{form + 1},{headings[direction]},{(locomotion ? "Available" : "MissingArt")},{(combo ? "Authored" : "MissingArt")},{comboSprites.Count},{(turn ? "Authored" : "SourcePose")}");
                if (locomotion && captureMatrix)
                {
                    foreach (bool run in new[] { false, true })
                    {
                        preview.SelectLocomotion(form, direction, run);
                        for (int frame = 0; frame < 8; frame++)
                        {
                            preview.ReviewAt(frame * .075f, false);
                            CaptureFrame(camera, $"Library/HexcoreMatrix/{(run ? "Run" : "Idle")}-{form+1}-{headings[direction]}", $"{frame:000}", 256, 144);
                        }
                    }
                }
            }
            Debug.Log($"HEXCORE FORM {form + 1}: {directions}/8 locomotion selectors");
            for (int skill = 0; skill < MeleeController.SkillCount; skill++)
            {
                if (!MeleeController.Compatible(skill, form)) continue;
                bool captureVariant = visualReview && (captureSkill < 0 || captureSkill == skill);
                for (int facing = 0; facing < 8; facing++)
                {
                    bool selected = preview.SelectSkill(form, skill, 2, facing, false, false);
                    if (!selected) throw new InvalidOperationException("Missing compatible selection.");
                    cases++;
                    string coverage = preview.DirectionCoverage(skill, facing);
                    string label = MeleeController.SkillName(skill);
                    if (coverage == "MissingArt")
                    {
                        missing++;
                        preview.ReviewAt(.55f, true);
                        if (preview.HasEffect || !preview.Phase.Contains("thiếu", StringComparison.OrdinalIgnoreCase))
                            throw new InvalidOperationException("Missing art is not reported: " + label);
                        preview.Replay();
                        results.Add($"{form + 1},\"{label}\",{headings[facing]},{coverage},0,0,OK,0,NotAvailable");
                        continue;
                    }
                    if (facing == 2 && skill >= 12 && skill <= 14)
                    {
                        preview.ReviewAt(.82f, true);
                        string prefix = skill == 12 ? "Slam_East_" : "Hero_East_";
                        if (!preview.CurrentSprite.name.StartsWith(prefix, StringComparison.Ordinal))
                            throw new InvalidOperationException("Missing dedicated hero sprite on form " + (form + 1));
                    }
                    else if (facing == 6 && skill >= 12 && skill <= 14 && coverage == "AuthoredBlockout")
                    {
                        preview.ReviewAt(.82f, false);
                        if (!preview.CurrentSprite.name.StartsWith("HeroWest_West_", StringComparison.Ordinal))
                            throw new InvalidOperationException("Missing west-facing hero art.");
                    }
                    else if (facing == 2 && skill != 0)
                    {
                        preview.ReviewAt(.08f, false);
                        if (!preview.HasContinuousSkill(skill) && !preview.CurrentSprite.name.StartsWith("Signature", StringComparison.Ordinal))
                            throw new InvalidOperationException("Missing east-facing key pose: " + MeleeController.SkillName(skill));
                    }
                    if (skill == 0)
                    {
                        preview.ReviewAt(.55f, true);
                        string direction = dashNames[facing];
                        if (preview.CurrentSprite.name != $"Q1_{direction}_07")
                            throw new InvalidOperationException("Incorrect Q1 impact pose: " + direction);
                    }
                    float duration = MeleeController.SkillDuration(skill) + .25f;
                    var sprites = new HashSet<Sprite>();
                    int effectsSeen = 0;
                    for (int frame = 0; frame * .075f < duration; frame++)
                    {
                        float seconds = frame * .075f + .003f;
                        for (int style = 0; style < 2; style++)
                        {
                            preview.ReviewAt(seconds, style == 1);
                            if (string.IsNullOrEmpty(preview.Phase) || preview.CurrentSprite == null)
                                throw new InvalidOperationException("Empty preview: " + MeleeController.SkillName(skill));
                            if (skill == 14 && preview.Phase == "Clone" && !preview.CopiesUseAuthoredStrikes)
                                throw new InvalidOperationException("R3 clone fell back to a source attack.");
                            if (skill == 5 && facing is 0 or 4 && !preview.ActorFitsVertically)
                                throw new InvalidOperationException("Tàng Ảnh leaves the review frame.");
                            if (style == 0 && preview.HasEffect) throw new InvalidOperationException("VFX toggle did not clear feedback.");
                            if (style == 1 && preview.HasEffect) effectsSeen++;
                            bool cleanReview = skill <= 2 && facing is 1 or 2 or 7 ||
                                skill >= 12 && skill <= 14 && facing is 0 or 1 or 2 or 6;
                            if (style == 0 && captureMatrix && cleanReview)
                                CaptureFrame(camera, $"Library/HexcoreMatrix/Clean-{skill+1:00}-{form+1}-{headings[facing]}", $"{frame:000}", 320, 180);
                            if (captureVariant)
                            {
                                string layer = style == 0 ? "Clean" : "Skill";
                                CaptureFrame(camera, $"Library/HexcoreVisualReview/{layer}-{skill+1:00}-{form+1}-{headings[facing]}", $"{frame:000}", 320, 180);
                            }
                        }
                        sprites.Add(preview.CurrentSprite);
                        if (captureMatrix)
                            CaptureFrame(camera, $"Library/HexcoreMatrix/Skill-{skill+1:00}-{form+1}-{headings[facing]}", $"{frame:000}", 320, 180);
                    }
                    if (capture && facing == 2)
                    {
                        float seconds = skill == 0 ? .57f : MeleeController.SkillDuration(skill) - .38f;
                        preview.ReviewAt(seconds, true);
                        CaptureFrame(camera, "Library/HexcoreCatalog", $"{skill:00}-{form + 1}");
                    }
                    preview.Replay();
                    if (preview.HasEffect || sprites.Count < 2)
                        throw new InvalidOperationException("Replay leaked feedback or timeline did not animate: " + label);
                    if (effectsSeen == 0) throw new InvalidOperationException("Timeline has no visible effect: " + label);
                    if (facing == 2 && preview.HasContinuousSkill(skill) && sprites.Count < 7)
                        throw new InvalidOperationException("Continuous clip did not sample enough poses: " + label);
                    string chain = "NotRun";
                    if (visualReview)
                    {
                        int move = (facing + 2) % 8;
                        preview.SelectSkill(form, skill, move, facing, true, false);
                        bool basicIn = false, basicOut = false, runOut = false, turn = false;
                        float skillEnd = 1.3f + MeleeController.SkillDuration(skill);
                        for (int frame = 0; frame * .075f < duration + 2.65f; frame++)
                        {
                            float seconds = frame * .075f + .003f;
                            preview.ReviewAt(seconds, true);
                            string phase = preview.Phase;
                            bool basic = phase.StartsWith("Basic", StringComparison.Ordinal);
                            basicIn |= seconds < 1.3f && basic;
                            basicOut |= seconds >= skillEnd && basic;
                            runOut |= seconds > skillEnd && phase.StartsWith("Run", StringComparison.Ordinal);
                            turn |= phase.StartsWith("Turn", StringComparison.Ordinal);
                            if (captureVariant)
                                CaptureFrame(camera, $"Library/HexcoreVisualReview/Chain-{skill+1:00}-{form+1}-{headings[facing]}", $"{frame:000}", 320, 180);
                        }
                        if (!basicIn || !basicOut || !runOut || !turn)
                            throw new InvalidOperationException($"Chain or direction turn missing: {label}, form {form + 1}, {headings[facing]}.");
                        chain = "OK";
                    }
                    results.Add($"{form + 1},\"{label}\",{headings[facing]},{coverage},{sprites.Count},{effectsSeen},OK,{preview.MeasuredFrameCount(skill, facing)},{chain}");
                }
                if (capture && form == 0 && skill >= 12 && skill <= 14)
                {
                    preview.SelectSkill(form, skill, 2, 2, false, false);
                    float length = MeleeController.SkillDuration(skill) + .24f;
                    for (int frame = 0; frame * .075f < length; frame++)
                    {
                        preview.ReviewAt(frame * .075f, true);
                        CaptureFrame(camera, $"Library/HexcoreHero{skill}", $"{frame:000}");
                    }
                }
                if (captureAll && MeleeController.PrimaryForm(skill) == form)
                {
                    preview.SelectSkill(form, skill, 2, 2, false, false);
                    float length = MeleeController.SkillDuration(skill) + .24f;
                    for (int frame = 0; frame * .075f < length; frame++)
                    {
                        preview.ReviewAt(frame * .075f, true);
                        CaptureFrame(camera, $"Library/HexcoreSkills/{skill + 1:00}", $"{frame:000}", 320, 180);
                    }
                }
                combinations++;
            }
        }
        if (cases != 264) throw new InvalidOperationException($"Expected 264 checks, found {cases}.");
        var utf8 = new System.Text.UTF8Encoding(true);
        File.WriteAllLines("Library/HexcoreSkillMatrix.csv", results, utf8);
        File.WriteAllLines("Library/HexcoreFormMatrix.csv", formResults, utf8);
        Debug.Log($"HEXCORE MATRIX QA: {combinations} compatible form-skill pairs, {cases} directions ({cases - missing} playable, {missing} missing artwork).");
    }

    private static void CaptureFrame(Camera camera, string folder = "Library/HexcoreFrames", string file = null,
        int width = 480, int height = 270, bool rejectPink = false)
    {
        Directory.CreateDirectory(folder);
        Rect previousRect = camera.rect;
        RenderTexture previousTarget = camera.targetTexture;
        RenderTexture previousActive = RenderTexture.active;
        var frame = RenderTexture.GetTemporary(width, height, 24);
        var image = new Texture2D(width, height, TextureFormat.RGB24, false);
        try
        {
            camera.rect = new Rect(0, 0, 1, 1);
            camera.targetTexture = frame;
            camera.Render();
            RenderTexture.active = frame;
            image.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            if (rejectPink)
            {
                foreach (Color32 pixel in image.GetPixels32())
                    if (pixel.r == 255 && pixel.g == 0 && pixel.b == 255)
                        throw new InvalidOperationException("Missing shader produced pink pixels in " + file);
            }
            File.WriteAllBytes($"{folder}/{file ?? $"{captureIndex++:000}"}.png", image.EncodeToPNG());
        }
        finally
        {
            camera.targetTexture = previousTarget;
            camera.rect = previousRect;
            RenderTexture.active = previousActive;
            RenderTexture.ReleaseTemporary(frame);
            UnityEngine.Object.DestroyImmediate(image);
        }
    }

    private static void Finish(int exitCode)
    {
        bool closeEditor = Application.isBatchMode || SessionState.GetBool(VerifyingInteraction, false) && CaptureInteractionUi;
        SessionState.SetBool(Verifying, false);
        SessionState.SetBool(VerifyingV2, false);
        SessionState.SetBool(VerifyingV21, false);
        Phases.Clear();
        verificationStep = 0;
        directionCheck = 0;
        timedIndex = -1;
        timedStarted = 0;
        fastDuration = 0;
        timedCamera = false;
        timedCopies = false;
        MixResults.Clear();
        timedComplete = false;
        timelineStarted = false;
        ResetInteractionVerification();
        ResetMovieCapture();
        mixPeak = 0;
        mixAudibleReads = 0;
        captureIndex = 0;
        nextCapture = 0;
        sawImpactCue = false;
        deadline = 0;
        if (closeEditor && !Application.isBatchMode)
        {
            EditorApplication.Exit(exitCode);
            return;
        }
        EditorApplication.ExitPlaymode();
        if (closeEditor) EditorApplication.delayCall += () => EditorApplication.Exit(exitCode);
    }

    private static T Load<T>(string path) where T : UnityEngine.Object
    {
        return AssetDatabase.LoadAssetAtPath<T>(path)
            ?? throw new InvalidOperationException("Missing preview asset: " + path);
    }
}

using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[InitializeOnLoad]
public static class MeleeDemoVerification
{
    private const string Running = "MeleeDemoVerification.Running";
    private static int index;
    private static int phase;
    private static double nextCheck;
    private static Sprite firstSprite;
    private static Vector3 firstPosition;
    private static Quaternion firstRotation;

    static MeleeDemoVerification()
    {
        EditorApplication.update += Update;
    }

    public static void Run()
    {
        index = 0;
        phase = 0;
        nextCheck = 0;
        firstSprite = null;
        EditorSceneManager.OpenScene("Assets/Scenes/MeleeDemo.unity");
        SessionState.SetBool(Running, true);
        EditorApplication.EnterPlaymode();
    }

    private static void Update()
    {
        if (!SessionState.GetBool(Running, false) || !EditorApplication.isPlaying) return;
        if (EditorApplication.timeSinceStartup < nextCheck) return;
        try
        {
            var demo = Object.FindFirstObjectByType<MeleeSkillDemo>();
            if (demo == null || demo.SkillCount != 15) throw new System.Exception("Expected 15 demo skills.");
            if (phase == 0)
            {
                demo.SelectSkill(index);
                var encounter = demo.GetComponent<MeleeDemoCombat>();
                encounter.Paused = true;
                encounter.Seek(index == 13 ? 2f : 2.5f);
                phase = 1;
                nextCheck = EditorApplication.timeSinceStartup + .12;
                return;
            }
            var combat = demo.GetComponent<MeleeDemoCombat>();
            var renderer = combat.EffectRenderer;
            if (renderer == null || renderer.sprite == null || demo.SelectedIndex != index)
                throw new System.Exception("Missing selected effect at " + index);
            if (phase == 1)
            {
                firstSprite = renderer.sprite;
                firstPosition = renderer.transform.localPosition;
                firstRotation = renderer.transform.localRotation;
                combat.Seek(index == 13 ? 2.25f : 2.75f);
                phase = 2;
                nextCheck = EditorApplication.timeSinceStartup + .16;
                return;
            }
            if (index != 0 && renderer.sprite == firstSprite && renderer.transform.localPosition == firstPosition && renderer.transform.localRotation == firstRotation)
                throw new System.Exception("Animation did not advance at " + index);
            if (index == 0)
            {
                if (renderer.sharedMaterial.shader.name != "Brotato/Barrier Shield")
                    throw new System.Exception("Missing surrounding barrier material.");
                Vector3 before = combat.ActorPosition;
                combat.Paused = false;
                combat.Move(Vector2.one, .1f);
                combat.Paused = true;
                combat.Seek(2.75f);
                if (Mathf.Abs(Vector3.Distance(before, combat.ActorPosition) - .28f) > .01f)
                    throw new System.Exception("Demo diagonal movement was not normalized.");
                if (Vector3.Distance(renderer.transform.localPosition, combat.ActorPosition) > .001f)
                    throw new System.Exception("Barrier did not follow character.");
            }
            if (combat.EnemyCount != 4 || demo.GetComponentsInChildren<SpriteRenderer>().Length != 6)
                throw new System.Exception("Expected one character, one effect and four enemies.");
            int expectedCharacter = index < 3 ? 0 : index < 12 ? index / 3 : 1;
            if (combat.CharacterIndex != expectedCharacter)
                throw new System.Exception("Incorrect branch character.");
            if (System.Array.IndexOf(System.Environment.GetCommandLineArgs(), "-demoCapture") >= 0 ||
                 System.IO.File.Exists("Library/MeleeDemoCapture.request"))
            {
                System.IO.Directory.CreateDirectory("docs/player-art/MeleeDemo");
                var camera = Camera.main;
                var target = new RenderTexture(1280, 720, 24);
                var previousTarget = camera.targetTexture;
                var previousActive = RenderTexture.active;
                try
                {
                    camera.targetTexture = target;
                    camera.Render();
                    RenderTexture.active = target;
                    var image = new Texture2D(1280, 720, TextureFormat.RGB24, false);
                    image.ReadPixels(new Rect(0, 0, 1280, 720), 0, 0);
                    image.Apply();
                    System.IO.File.WriteAllBytes("docs/player-art/MeleeDemo/CombatPreview" + index + ".png", image.EncodeToPNG());
                    Object.DestroyImmediate(image);
                }
                finally
                {
                    camera.targetTexture = previousTarget;
                    RenderTexture.active = previousActive;
                    Object.DestroyImmediate(target);
                }
                nextCheck = EditorApplication.timeSinceStartup + .4;
            }
            index++;
            phase = 0;
            if (index < 15) return;
            SessionState.SetBool(Running, false);
            Debug.Log("MELEE DEMO PLAY MODE PASSED: 15 choreographies, correct branch actors, four enemies, animated VFX and clean selection replacement.");
            System.IO.File.WriteAllText("Temp/MeleeDemoCombatVerification.txt", "PASS: 15 skills, correct characters, four enemies, animated VFX, no stale encounter.");
            EditorApplication.ExitPlaymode();
            if (Application.isBatchMode) EditorApplication.delayCall += () => EditorApplication.Exit(0);
        }
        catch (System.Exception exception)
        {
            SessionState.SetBool(Running, false);
            Debug.LogException(exception);
            System.IO.File.WriteAllText("Temp/MeleeDemoCombatVerification.txt", exception.ToString());
            if (Application.isBatchMode) EditorApplication.Exit(1);
            else EditorApplication.ExitPlaymode();
        }
    }
}

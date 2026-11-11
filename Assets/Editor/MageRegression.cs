using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

// Runs against Magical.unity in real Play Mode. Test edits are not saved.
[InitializeOnLoad]
public static class MageRegression
{
    private const string Pending = "Brotato.MageRegression";
    private static IEnumerator routine;
    private static readonly List<string> results = new List<string>();
    private static double deadline;
    private static bool runtimeError;

    static MageRegression()
    {
        EditorApplication.playModeStateChanged += state =>
        {
            if (state != PlayModeStateChange.EnteredPlayMode || !SessionState.GetBool(Pending, false)) return;
            SessionState.SetBool(Pending, false);
            results.Clear();
            runtimeError = false;
            Application.logMessageReceived += TrackErrors;
            deadline = EditorApplication.timeSinceStartup + 45;
            routine = Run();
            EditorApplication.update += Tick;
        };
    }

    [MenuItem("Tools/Brotato/Run Mage Regression")]
    public static void Start()
    {
        if (EditorApplication.isPlaying) throw new InvalidOperationException("Stop Play Mode first.");
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "Magical")
            throw new InvalidOperationException("Open Magical.unity before running Mage regression.");
        SessionState.SetBool(Pending, true);
        EditorApplication.isPlaying = true;
    }

    private static void TrackErrors(string message, string trace, LogType type)
    {
        if (type != LogType.Error && type != LogType.Exception && type != LogType.Assert) return;
        runtimeError = true;
        results.Add("RUNTIME ERROR: " + message);
    }

    private static void Tick()
    {
        try
        {
            if (!EditorApplication.isPlaying) throw new Exception("Play Mode stopped unexpectedly.");
            if (EditorApplication.timeSinceStartup > deadline) throw new Exception("Test timed out.");
            if (!routine.MoveNext()) Finish(null);
        }
        catch (Exception error) { Finish(error); }
    }

    private static void Finish(Exception error)
    {
        EditorApplication.update -= Tick;
        Application.logMessageReceived -= TrackErrors;
        if (error != null) results.Add("FAIL: " + error);
        else if (runtimeError) results.Add("FAIL: runtime error appeared");
        else results.Add("ALL CHECKS PASSED");
        System.IO.Directory.CreateDirectory("Temp");
        System.IO.File.WriteAllLines("Temp/MageRegression.txt", results);
        if (error == null && !runtimeError) Debug.Log(string.Join("\n", results));
        else Debug.LogError(string.Join("\n", results));
        EditorApplication.isPlaying = false;
    }

    private static void Check(bool condition, string message)
    {
        if (!condition) throw new Exception(message);
        results.Add("PASS: " + message);
    }

    private static void Set(object target, string field, object value)
    {
        FieldInfo info = target.GetType().GetField(field, BindingFlags.Instance | BindingFlags.NonPublic);
        if (info == null) throw new Exception("Missing field: " + field);
        info.SetValue(target, value);
    }

    private static object Call(object target, string method, params object[] args)
    {
        MethodInfo info = target.GetType().GetMethod(method, BindingFlags.Instance | BindingFlags.NonPublic);
        if (info == null) throw new Exception("Missing method: " + method);
        try { return info.Invoke(target, args); }
        catch (TargetInvocationException error) { throw error.InnerException ?? error; }
    }

    private static IEnumerator Wait(float seconds)
    {
        double end = EditorApplication.timeSinceStartup + seconds;
        while (EditorApplication.timeSinceStartup < end) yield return null;
    }

    private static IEnumerator Run()
    {
        IEnumerator wait = Wait(0.3f);
        while (wait.MoveNext()) yield return null;

        MageCombat combat = Object.FindFirstObjectByType<MageCombat>();
        MageUltimate ultimate = Object.FindFirstObjectByType<MageUltimate>();
        MageAim aim = Object.FindFirstObjectByType<MageAim>();
        MageLoadout loadout = Object.FindFirstObjectByType<MageLoadout>();
        PlayerHealth player = Object.FindFirstObjectByType<PlayerHealth>();
        MageEffectLibrary effects = Object.FindFirstObjectByType<MageEffectLibrary>();
        Check(combat && ultimate && aim && loadout && player && effects, "Mage components are wired");
        Set(combat, "nextAttack", Time.time + 1000f);
        player.SetDamageEnabled(false);
        player.GetComponent<PlayerController>().enabled = false;
        aim.enabled = false;
        Set(aim, "<Direction>k__BackingField", Vector2.right);
        Rigidbody2D body = player.GetComponent<Rigidbody2D>();
        Camera camera = Camera.main;
        Check(camera != null && camera.GetComponent<CameraFollow>() != null &&
            Mathf.Approximately(camera.orthographicSize, 4.5f), "Mage camera follows with closer zoom");
        body.position = new Vector2(8f, 1f);
        wait = Wait(0.2f); while (wait.MoveNext()) yield return null;
        Check(Vector2.Distance(camera.transform.position, body.position) < 0.05f,
            "Camera follows moved Mage in Play Mode");
        body.position = Vector2.zero;
        wait = Wait(0.2f); while (wait.MoveNext()) yield return null;
        Check(player.transform.Find("Visual").localScale.x >= 1.5f, "Mage visual is enlarged");

        loadout.SelectElement(MageElement.Light);
        loadout.SetTier(0, 2);
        loadout.SetCore(1, true);
        Check(loadout.GetTier(0) == 2 && loadout.GetCore(1), "Preview panel can change tiers and cores");
        loadout.SelectElement(MageElement.Fire);
        foreach (EnemyHealth existing in Object.FindObjectsByType<EnemyHealth>(FindObjectsSortMode.None))
            existing.gameObject.SetActive(false);

        for (int id = 0; id < 18; id++)
            Check(effects.GetFrame((MageEffectId)id, 0) != null, "Effect atlas " + (MageEffectId)id);

        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Enemy/Enemy.prefab");
        Check(prefab != null, "Preview enemy prefab exists");
        GameObject targetObject = Object.Instantiate(prefab, new Vector3(1.4f, 0f), Quaternion.identity);
        EnemyHealth enemy = targetObject.GetComponent<EnemyHealth>();
        enemy.InitializeHealth(1000);
        targetObject.GetComponent<EnemyController>().enabled = false;
        targetObject.GetComponent<EnemyContactDamage>().enabled = false;
        MageEnemyStatus status = combat.Status(enemy);

        player.SetDamageEnabled(true);
        player.TakeDamage(30);
        player.SetDamageEnabled(false);
        int beforeHeal = player.CurrentHealth;
        Set(loadout, "starterSkill", MageStarterSkill.Heal);
        Call(combat, "UseStarter");
        Check(player.CurrentHealth == Mathf.Min(player.MaxHealth, beforeHeal + 25), "Q Heal restores 25 HP");

        Set(loadout, "starterSkill", MageStarterSkill.Shockwave);
        int beforeShock = enemy.CurrentHealth;
        Call(combat, "UseStarter");
        Check(enemy.CurrentHealth == beforeShock - 15, "Q Shockwave deals 15 once");
        targetObject.transform.position = new Vector3(1.4f, 0f);

        int beforeGravity = enemy.CurrentHealth;
        combat.StartCoroutine((IEnumerator)Call(combat, "Gravity", new Vector2(1.4f, 0f)));
        wait = Wait(1.2f); while (wait.MoveNext()) yield return null;
        Check(enemy.CurrentHealth <= beforeGravity - 2 && status.MoveMultiplier <= 0.65f,
            "Q Gravity damages and slows in area");

        int beforeFire = enemy.CurrentHealth;
        Call(combat, "LaunchFire", Vector2.right, 1f);
        wait = Wait(0.5f); while (wait.MoveNext()) yield return null;
        Check(enemy.CurrentHealth <= beforeFire - 20 && status.IsBurning, "Fire explosion and burn apply");

        for (int i = 0; i < 6; i++) Call(combat, "Ice");
        Check(status.IsFrozen && status.MoveMultiplier == 0f, "Ice freezes after six accepted casts");
        wait = Wait(2.2f); while (wait.MoveNext()) yield return null;

        Set(loadout, "judgement", true);
        int beforeLight = enemy.CurrentHealth;
        for (int i = 0; i < 3; i++) Call(combat, "LightPulse", new Vector2(1.4f, 0f), 2f, 1f);
        Check(enemy.CurrentHealth <= beforeLight - 42, "Light pulses trigger Judgement core");

        Call(combat, "Dark");
        wait = Wait(0.5f); while (wait.MoveNext()) yield return null;
        Check(status.IsCursed, "Dark missile applies curse");

        int beforeStop = enemy.CurrentHealth;
        ultimate.StartCoroutine((IEnumerator)Call(ultimate, "TimeStop"));
        Check(status.MoveMultiplier == 0f, "TimeStop halts normal enemy");
        combat.Damage(enemy, 10);
        Check(enemy.CurrentHealth == beforeStop, "TimeStop queues spell damage");
        wait = Wait(3.2f); while (wait.MoveNext()) yield return null;
        Check(enemy.CurrentHealth <= beforeStop - 10, "Queued damage resolves after TimeStop");

        int beforeOrb = enemy.CurrentHealth;
        Call(ultimate, "StartOrb");
        for (int i = 0; i < 6; i++) ultimate.NotifyHit();
        Check(ultimate.OrbCharge == 6, "Orb charge caps at six");
        Call(ultimate, "ReleaseOrb");
        wait = Wait(0.8f); while (wait.MoveNext()) yield return null;
        Check(enemy.CurrentHealth < beforeOrb, "Charged orb missiles hit enemy");

        int beforeStars = enemy.CurrentHealth;
        ultimate.StartCoroutine((IEnumerator)Call(ultimate, "StarFallAt", new Vector2(1.4f, 0f)));
        wait = Wait(2.2f); while (wait.MoveNext()) yield return null;
        Check(enemy.CurrentHealth <= beforeStars - 60, "StarFall lands five 12-damage strikes");

        GameObject bossObject = Object.Instantiate(prefab, new Vector3(-1.4f, 0f), Quaternion.identity);
        EnemyHealth bossHealth = bossObject.GetComponent<EnemyHealth>();
        bossHealth.InitializeHealth(100);
        bossObject.GetComponent<EnemyController>().enabled = false;
        bossObject.AddComponent<MageBossMarker>();
        MageEnemyStatus boss = combat.Status(bossHealth);
        boss.StopTime(3f);
        boss.ApplyIceHit(0.3f, 2.5f, 2f, false);
        Check(boss.MoveMultiplier == 1f, "Boss ignores Mage control effects");
        combat.Damage(bossHealth, 10);
        Check(bossHealth.CurrentHealth == 90, "Boss still receives Mage damage");
        Check(!runtimeError, "No runtime errors");
    }
}

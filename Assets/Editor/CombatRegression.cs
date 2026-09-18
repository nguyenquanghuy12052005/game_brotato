using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

// Editor-only smoke tests: transient Play Mode changes never save into the scene.
[InitializeOnLoad]
public static class CombatRegression
{
    private const string Pending = "Brotato.CombatRegression";
    private static IEnumerator routine;
    private static readonly List<string> results = new List<string>();
    private static double deadline;
    private static bool runtimeError;

    static CombatRegression()
    {
        EditorApplication.playModeStateChanged += state =>
        {
            if (state == PlayModeStateChange.EnteredPlayMode && SessionState.GetBool(Pending, false))
            {
                SessionState.SetBool(Pending, false);
                results.Clear();
                runtimeError = false;
                Application.logMessageReceived += TrackErrors;
                deadline = EditorApplication.timeSinceStartup + 60;
                routine = Run();
                EditorApplication.update += Tick;
            }
        };
    }

    [MenuItem("Tools/Brotato/Run Combat Regression")]
    public static void Start()
    {
        if (EditorApplication.isPlaying) throw new InvalidOperationException("Stop Play Mode first.");
        SessionState.SetBool(Pending, true);
        EditorApplication.isPlaying = true;
    }

    private static void TrackErrors(string message, string trace, LogType type)
    {
        if (type == LogType.Error || type == LogType.Exception || type == LogType.Assert)
        {
            runtimeError = true;
            results.Add("RUNTIME ERROR: " + message);
        }
    }

    private static void Tick()
    {
        try
        {
            if (!EditorApplication.isPlaying) throw new Exception("Play Mode stopped before completion.");
            if (EditorApplication.timeSinceStartup > deadline) throw new Exception("Test timeout.");
            if (!routine.MoveNext()) Finish(null);
        }
        catch (Exception error) { Finish(error); }
    }

    private static void Finish(Exception error)
    {
        EditorApplication.update -= Tick;
        Application.logMessageReceived -= TrackErrors;
        if (error != null) results.Add("FAIL: " + error);
        else results.Add("ALL CHECKS PASSED");
        System.IO.File.WriteAllLines("Temp/CombatRegression.txt", results);
        if (error == null) Debug.Log(string.Join("\n", results));
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
        target.GetType().GetField(field, BindingFlags.Instance | BindingFlags.NonPublic).SetValue(target, value);
    }

    private static void Call(object target, string method, params object[] args)
    {
        target.GetType().GetMethod(method, BindingFlags.Instance | BindingFlags.NonPublic).Invoke(target, args);
    }

    private static IEnumerator Wait(float seconds)
    {
        double end = EditorApplication.timeSinceStartup + seconds;
        while (EditorApplication.timeSinceStartup < end) yield return null;
    }

    private static IEnumerator Run()
    {
        var wait = Wait(0.5f);
        while (wait.MoveNext()) yield return null;
        var game = Object.FindFirstObjectByType<GameManager>();
        var waves = Object.FindFirstObjectByType<WaveManager>();
        var spawner = Object.FindFirstObjectByType<EnemySpawner>();
        var player = Object.FindFirstObjectByType<PlayerHealth>();
        Check(game && waves && spawner && player && Camera.main.GetComponent<CameraFollow>(),
            "Saved scene has managers, player and camera follow");
        Check(new SerializedObject(waves).FindProperty("enemySpawner").objectReferenceValue == spawner,
            "WaveManager explicitly references EnemySpawner");
        foreach (var weapon in player.GetComponentsInChildren<WeaponAim>()) weapon.enabled = false;
        player.GetComponent<PlayerController>().enabled = false;
        player.SetDamageEnabled(false);
        spawner.ClearEnemies();
        foreach (var bullet in Object.FindObjectsByType<Bullet>(FindObjectsSortMode.None))
        {
            bullet.gameObject.SetActive(false);
            Object.Destroy(bullet.gameObject);
        }
        yield return null;

        var body = player.GetComponent<Rigidbody2D>();
        float cameraZ = Camera.main.transform.position.z;
        body.position += new Vector2(12f, -7f);
        wait = Wait(0.2f);
        while (wait.MoveNext()) yield return null;
        Check(Vector2.Distance(Camera.main.transform.position, player.transform.position) < 0.01f &&
            Mathf.Approximately(Camera.main.transform.position.z, cameraZ), "Camera follows Player XY and preserves Z");

        string bulletPath = AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets("Bullet t:Prefab", new[] { "Assets/Prefabs" })[0]);
        var bulletPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(bulletPath);
        Check(bulletPrefab.GetComponent<CircleCollider2D>().enabled &&
            !bulletPrefab.GetComponent<BoxCollider2D>().enabled, "Only circle collider enabled on saved Bullet prefab");
        string enemyPath = AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets("Enemy t:Prefab", new[] { "Assets/Prefabs" })[0]);
        var enemyPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(enemyPath);
        var target = Object.Instantiate(enemyPrefab, new Vector3(1000, 1000, 0), Quaternion.identity);
        target.GetComponent<EnemyController>().enabled = false;
        var health = target.GetComponent<EnemyHealth>();
        health.InitializeHealth(30);
        var shot = Object.Instantiate(bulletPrefab, target.transform.position, Quaternion.identity);
        // Re-enable duplicate collider to prove the guard even with the old prefab configuration.
        shot.GetComponent<BoxCollider2D>().enabled = true;
        wait = Wait(0.2f);
        while (wait.MoveNext()) yield return null;
        Check(health.CurrentHealth == 20 && !shot, "Real Physics2D overlap with two bullet colliders deals exactly 10 damage");
        var secondShot = Object.Instantiate(bulletPrefab, new Vector3(1100, 1100, 0), Quaternion.identity).GetComponent<Bullet>();
        Call(secondShot, "OnTriggerEnter2D", target.GetComponent<Collider2D>());
        Call(secondShot, "OnTriggerEnter2D", target.GetComponent<Collider2D>());
        Check(health.CurrentHealth == 10, "Repeated trigger callback consumes only one bullet hit");
        health.TakeDamage(999);
        Check(health.CurrentHealth == 0, "Enemy HP clamps at zero");
        yield return null;

        player.SetDamageEnabled(true);
        player.TakeDamage(10);
        player.TakeDamage(10);
        Check(player.CurrentHealth == 90, "Player damage cooldown rejects immediate second hit");
        player.SetDamageEnabled(false);
        int retainedHP = player.CurrentHealth;

        for (int index = 0; index < 3; index++)
        {
            player.SetDamageEnabled(false);
            int[] caps = { 10, 15, 30 };
            int[] hp = { 10, 20, 30 };
            float[] speed = { 2, 2.5f, 3 };
            float[] intervals = { 1.5f, 1, 0.6f };
            float[] durations = { 30, 30, 45 };
            Check(waves.CurrentWave == index + 1 && waves.CurrentSettings.duration == durations[index] &&
                waves.CurrentSettings.spawnInterval == intervals[index], "Wave " + (index + 1) + " duration/interval");
            // Exercise the actual Update cap guard, forcing the scheduling clock only.
            for (int attempt = 0; attempt < caps[index] + 3; attempt++)
            {
                Set(spawner, "nextSpawnTime", -1f);
                Call(spawner, "Update");
            }
            Check(spawner.AliveCount == caps[index], "Wave " + (index + 1) + " exact alive cap");
            foreach (var enemy in Object.FindObjectsByType<EnemyHealth>(FindObjectsSortMode.None))
            {
                Check(enemy.CurrentHealth == hp[index] && enemy.GetComponent<EnemyController>().MoveSpeed == speed[index],
                    "Wave " + (index + 1) + " spawned HP/speed");
                Vector3 viewport = Camera.main.WorldToViewportPoint(enemy.transform.position);
                Check(viewport.x < 0 || viewport.x > 1 || viewport.y < 0 || viewport.y > 1,
                    "Spawn outside camera");
                enemy.GetComponent<EnemyController>().enabled = false;
            }
            var victim = Object.FindFirstObjectByType<EnemyHealth>();
            victim.TakeDamage(999);
            yield return null;
            Set(spawner, "nextSpawnTime", -1f);
            Call(spawner, "Update");
            Check(spawner.AliveCount == caps[index], "Replenish dead enemy without exceeding cap");
            Object.Instantiate(bulletPrefab, player.transform.position + Vector3.up * 2, Quaternion.identity);
            Set(waves, "<RemainingTime>k__BackingField", 0.001f);
            wait = Wait(0.2f);
            while (wait.MoveNext()) yield return null;
            Check(!game.IsPlaying && Time.timeScale == 0 && spawner.AliveCount == 0 &&
                Object.FindObjectsByType<Bullet>(FindObjectsSortMode.None).Length == 0, "Timer ends wave, freezes and clears combat");
            player.TakeDamage(10);
            Check(player.CurrentHealth == retainedHP, "Player HP preserved and protected between waves");
            if (index < 2)
            {
                Check(game.State == GameManager.GameState.WaveComplete && waves.CurrentWave == index + 1 &&
                    waves.IntermissionRemaining > 1f, "Visible two-second intermission");
                wait = Wait(2.1f);
                while (wait.MoveNext()) yield return null;
                Check(game.IsPlaying && Time.timeScale == 1 && spawner.enabled, "Next wave resumes combat");
                spawner.ClearEnemies();
                yield return null;
            }
        }
        Check(game.State == GameManager.GameState.Victory, "Wave 3 ends in Victory, no Wave 4");
        Call(game, "RestartRound");
        wait = Wait(0.5f);
        while (wait.MoveNext()) yield return null;
        game = Object.FindFirstObjectByType<GameManager>();
        waves = Object.FindFirstObjectByType<WaveManager>();
        player = Object.FindFirstObjectByType<PlayerHealth>();
        Check(game.IsPlaying && waves.CurrentWave == 1 && waves.RemainingTime > 28 &&
            player.CurrentHealth == 100 && Time.timeScale == 1 &&
            player.GetComponent<PlayerController>().enabled &&
            player.GetComponentInChildren<WeaponAim>().enabled, "Restart after Victory resets full round");
        Set(player, "nextDamageTime", -1f);
        player.TakeDamage(999);
        Check(player.CurrentHealth == 0 && game.State == GameManager.GameState.GameOver && Time.timeScale == 0,
            "Lethal damage clamps Player HP and enters Game Over");
        Call(game, "RestartRound");
        wait = Wait(0.5f);
        while (wait.MoveNext()) yield return null;
        game = Object.FindFirstObjectByType<GameManager>();
        waves = Object.FindFirstObjectByType<WaveManager>();
        player = Object.FindFirstObjectByType<PlayerHealth>();
        Check(game.IsPlaying && waves.CurrentWave == 1 && player.CurrentHealth == 100 &&
            Time.timeScale == 1 && Object.FindFirstObjectByType<EnemySpawner>().enabled, "Restart after Game Over resets round");
        Check(!runtimeError, "No runtime errors during regression");
    }
}

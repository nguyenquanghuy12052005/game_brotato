using System;
using UnityEngine;

[Serializable]
public class WaveSettings
{
    [Min(1f)] public float duration = 30f;
    [Min(1)] public int maxEnemiesAlive = 10;
    [Min(1)] public int enemyHealth = 10;
    [Min(0.05f)] public float spawnInterval = 1.5f;
    [Min(0.1f)] public float enemySpeed = 2f;

    public WaveSettings(float seconds, int count, int health, float interval, float speed)
    {
        duration = seconds;
        maxEnemiesAlive = count;
        enemyHealth = health;
        spawnInterval = interval;
        enemySpeed = speed;
    }
}

public class WaveManager : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private EnemySpawner enemySpawner;
    [SerializeField] private MapManager mapManager;
    [SerializeField, Min(0.1f)] private float intermissionDuration = 2f;

    // Mỗi wave = một loại quái MỚI xuất hiện thêm. Wave sau giữ loại của wave trước.
    // Hiện mới có lính thường nên mỗi map chỉ 1 wave. Có cung thủ thì thêm wave thứ 2,
    // có tinh nhuệ thì thêm wave thứ 3, có trùm thì thêm wave cuối.
    [SerializeField] private WaveSettings[] waves =
    {
        new WaveSettings(30f, 10, 10, 1.5f, 2f)
    };

    public float RemainingTime { get; private set; }
    public float IntermissionRemaining { get; private set; }
    public int CurrentWave { get; private set; } = 1;
    public int TotalWaves => waves.Length;
    public bool IsLastWave => CurrentWave >= TotalWaves;
    public bool IsLastMap => mapManager == null || mapManager.IsLastMap;
    public int CurrentMapNumber => mapManager != null ? mapManager.CurrentMap + 1 : 1;
    public int TotalMaps => mapManager != null ? mapManager.TotalMaps : 1;
    public WaveSettings CurrentSettings => waves[CurrentWave - 1];

    private void Awake()
    {
        RemainingTime = Mathf.Max(1f, CurrentSettings.duration);
    }

    private void Start()
    {
        if (gameManager == null || enemySpawner == null)
        {
            Debug.LogError("WaveManager: hãy gắn Game Manager và Enemy Spawner trong Inspector.", this);
            enabled = false;
            return;
        }

        if (mapManager != null) mapManager.ApplyCurrentMap();
        enemySpawner.ConfigureForWave(CurrentSettings);
    }

    private void Update()
    {
        if (gameManager == null) return;

        // Phím tắt để thử nhanh, không ảnh hưởng lúc chơi bình thường.
        // N = bỏ qua wave hiện tại, M = sang map kế ngay.
        if (gameManager.IsPlaying)
        {
            var keyboard = UnityEngine.InputSystem.Keyboard.current;
            if (keyboard != null)
            {
                if (keyboard.nKey.wasPressedThisFrame)
                {
                    RemainingTime = 0f;
                }
                if (keyboard.mKey.wasPressedThisFrame && mapManager != null && !mapManager.IsLastMap)
                {
                    mapManager.AdvanceToNextMap();
                    CurrentWave = 1;
                    RemainingTime = Mathf.Max(1f, CurrentSettings.duration);
                    enemySpawner.ConfigureForWave(CurrentSettings);
                    Debug.Log($"[Map] Nhảy sang map {CurrentMapNumber}/{TotalMaps}.");
                }
            }
        }

        if (gameManager.State == GameManager.GameState.WaveComplete)
        {
            // Gameplay đã dừng; thời gian nghỉ vẫn chạy bằng unscaledDeltaTime.
            IntermissionRemaining = Mathf.Max(0f, IntermissionRemaining - Time.unscaledDeltaTime);
            if (IntermissionRemaining <= 0f) AdvanceWave();
            return;
        }

        if (!gameManager.IsPlaying) return;
        RemainingTime = Mathf.Max(0f, RemainingTime - Time.deltaTime);
        if (RemainingTime <= 0f) gameManager.CompleteWave();
    }

    public void BeginIntermission()
    {
        IntermissionRemaining = Mathf.Max(0.1f, intermissionDuration);
    }

    private void AdvanceWave()
    {
        if (gameManager.State != GameManager.GameState.WaveComplete) return;

        if (IsLastWave)
        {
            // Hết wave cuối của map. Còn map thì sang map kế và quay lại wave 1.
            if (mapManager == null || mapManager.IsLastMap)
            {
                gameManager.CompleteLastMap();
                return;
            }

            mapManager.AdvanceToNextMap();
            CurrentWave = 1;
        }
        else
        {
            CurrentWave++;
        }

        RemainingTime = Mathf.Max(1f, CurrentSettings.duration);
        enemySpawner.ConfigureForWave(CurrentSettings);
        gameManager.BeginNextWave();
        Debug.Log($"Map {CurrentMapNumber}/{TotalMaps} — Wave {CurrentWave}/{TotalWaves} bắt đầu!");
    }
}

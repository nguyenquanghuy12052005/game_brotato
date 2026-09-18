using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public enum GameState
    {
        Playing,
        WaveComplete,
        GameOver,
        Victory
    }

    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private EnemySpawner enemySpawner;
    [SerializeField] private WaveManager waveManager;

    public GameState State { get; private set; } = GameState.Playing;
    public bool IsPlaying => State == GameState.Playing;
    private PlayerController movement;
    private bool movementWasEnabled;
    private WeaponAim[] weapons;
    private bool[] weaponsWereEnabled;

    private void Awake()
    {
        Time.timeScale = 1f;
    }

    private void OnEnable()
    {
        if (playerHealth != null)
        {
            playerHealth.Died += GameOver;
        }
    }

    private void Start()
    {
        if (playerHealth == null || enemySpawner == null || waveManager == null)
        {
            Debug.LogError("GameManager: hãy gắn Player Health, Enemy Spawner và Wave Manager trong Inspector.", this);
        }
    }

    private void OnDisable()
    {
        if (playerHealth != null)
        {
            playerHealth.Died -= GameOver;
        }
    }

    private void OnDestroy()
    {
        Time.timeScale = 1f;
    }

    public void CompleteWave()
    {
        if (!IsPlaying || waveManager == null) return;
        // Nếu Player đã chết thì không chuyển kết quả sang Wave Complete.
        if (playerHealth != null && playerHealth.CurrentHealth <= 0)
        {
            GameOver();
            return;
        }

        if (waveManager.IsLastWave)
        {
            EndRound(GameState.Victory);
            return;
        }

        EndRound(GameState.WaveComplete);
        waveManager.BeginIntermission();
    }

    public void BeginNextWave()
    {
        if (State != GameState.WaveComplete) return;
        State = GameState.Playing;
        if (movement != null) movement.enabled = movementWasEnabled;
        if (weapons != null)
            for (int i = 0; i < weapons.Length; i++)
                if (weapons[i] != null) weapons[i].enabled = weaponsWereEnabled[i];
        if (playerHealth != null) playerHealth.SetDamageEnabled(true);
        if (enemySpawner != null) enemySpawner.enabled = true;
        Time.timeScale = 1f;
    }

    private void GameOver()
    {
        EndRound(GameState.GameOver);
    }

    private void EndRound(GameState result)
    {
        if (!IsPlaying)
        {
            return;
        }

        State = result;

        if (enemySpawner != null)
        {
            enemySpawner.enabled = false;
            enemySpawner.ClearEnemies();
        }

        foreach (Bullet bullet in FindObjectsByType<Bullet>(FindObjectsSortMode.None))
        {
            if (bullet.gameObject.scene != gameObject.scene) continue;
            bullet.gameObject.SetActive(false);
            Destroy(bullet.gameObject);
        }

        if (playerHealth != null)
        {
            playerHealth.SetDamageEnabled(false);
            movement = playerHealth.GetComponent<PlayerController>();
            if (movement != null)
            {
                movementWasEnabled = movement.enabled;
                movement.enabled = false;
            }

            weapons = playerHealth.GetComponentsInChildren<WeaponAim>();
            weaponsWereEnabled = new bool[weapons.Length];
            for (int i = 0; i < weapons.Length; i++)
            {
                weaponsWereEnabled[i] = weapons[i].enabled;
                weapons[i].enabled = false;
            }
        }

        // Dừng physics, đạn, quái và các bộ đếm dùng thời gian gameplay.
        Time.timeScale = 0f;
        Debug.Log(result == GameState.GameOver ? "GAME OVER" : $"WAVE {waveManager.CurrentWave} COMPLETE");
    }

    private void RestartRound()
    {
        string scenePath = gameObject.scene.path;
        if (!Application.CanStreamedLevelBeLoaded(scenePath))
        {
            Debug.LogError("Hãy thêm GameScene vào Scene List của Build Profiles để dùng nút chơi lại.", this);
            return;
        }

        Time.timeScale = 1f;
        SceneManager.LoadScene(scenePath);
    }

    private void OnGUI()
    {
        if (playerHealth == null || waveManager == null)
        {
            return;
        }

        // UI tối giản cho prototype; sẽ thay bằng Canvas khi hoàn thiện UI.
        GUI.Box(new Rect(15f, 15f, 210f, 85f),
            $"Wave {waveManager.CurrentWave}/{waveManager.TotalWaves}\nHP: {playerHealth.CurrentHealth}/{playerHealth.MaxHealth}\nTime: {Mathf.CeilToInt(waveManager.RemainingTime)}s");

        if (IsPlaying)
        {
            return;
        }

        float x = (Screen.width - 300f) / 2f;
        float y = (Screen.height - 140f) / 2f;
        if (State == GameState.WaveComplete)
        {
            GUI.Box(new Rect(x, y, 300f, 100f),
                $"WAVE {waveManager.CurrentWave} COMPLETE\nNext Wave in {Mathf.CeilToInt(waveManager.IntermissionRemaining)}s");
            return;
        }

        string title = State == GameState.GameOver ? "GAME OVER" : "ALL WAVES COMPLETE";
        string detail = State == GameState.GameOver
            ? $"Wave Reached: {waveManager.CurrentWave}"
            : $"Completed {waveManager.TotalWaves} Waves";

        GUI.Box(new Rect(x, y, 300f, 140f), $"{title}\n{detail}");
        if (GUI.Button(new Rect(x + 60f, y + 80f, 180f, 35f), "PLAY AGAIN"))
        {
            RestartRound();
        }
    }
}

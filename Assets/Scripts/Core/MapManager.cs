using System;
using UnityEngine;

/// <summary>
/// Một map trong scene. Bốn biome đã dựng sẵn nằm cạnh nhau trên trục X,
/// cách nhau 35 đơn vị: Forest 0, Moon 35, Inferno 70, Tundra 105.
/// </summary>
[Serializable]
public class MapSettings
{
    public string biomeName = "Forest";
    public float centerX = 0f;
    [Tooltip("Quái được phép ra ở map này. Màu quái nên khớp màu biome.")]
    public EnemySpawnEntry[] enemies;
}

/// <summary>
/// Quản lý 4 map nằm chung một scene. Chuyển map = dời Player sang vùng kế,
/// xoá quái cũ, đổi bảng màu quái, hồi đầy máu.
/// </summary>
public class MapManager : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private EnemySpawner enemySpawner;
    [SerializeField] private MapSettings[] maps;

    [Header("Kích thước vùng chơi")]
    [Tooltip("Nửa bề rộng map, tính bằng unit. Map 30 ô thì 15.")]
    [SerializeField, Min(1f)] private float mapHalfWidth = 14f;
    [SerializeField, Min(1f)] private float mapHalfHeight = 8f;

    public int CurrentMap { get; private set; } = 0;
    public int TotalMaps => maps != null ? maps.Length : 0;
    public bool IsLastMap => CurrentMap >= TotalMaps - 1;
    public MapSettings CurrentSettings =>
        maps != null && maps.Length > 0 ? maps[Mathf.Clamp(CurrentMap, 0, maps.Length - 1)] : null;

    // Đưa bảng màu quái + biên map của map hiện tại vào.
    public void ApplyCurrentMap()
    {
        if (CurrentSettings == null)
        {
            return;
        }

        if (enemySpawner != null)
        {
            enemySpawner.SetEnemyTypes(CurrentSettings.enemies);
        }

        if (playerController != null)
        {
            playerController.SetMapBounds(CurrentSettings.centerX, mapHalfWidth, mapHalfHeight);
        }
    }

    // Sang map kế: đổi bảng quái, dời Player về giữa vùng mới, hồi đầy máu.
    public void AdvanceToNextMap()
    {
        if (IsLastMap)
        {
            return;
        }

        CurrentMap++;
        ApplyCurrentMap();

        if (enemySpawner != null)
        {
            enemySpawner.ClearEnemies();
        }

        if (player != null)
        {
            player.position = new Vector3(CurrentSettings.centerX, 0f, player.position.z);
        }

        if (playerHealth != null)
        {
            playerHealth.RestoreFullHealth();
        }

        Debug.Log($"[Map] Sang map {CurrentMap + 1}/{TotalMaps}: {CurrentSettings.biomeName} " +
                  $"(tâm X = {CurrentSettings.centerX}).");
    }
}

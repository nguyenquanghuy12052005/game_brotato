using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

/// <summary>
/// Sinh 4 map Roboden (Forest / Moon / Inferno / Tundra) trong scene.
///
/// Cách dùng: menu Tools > Brotato > Generate Roboden Maps.
/// Chạy trong Editor, tạo Tilemap + Tile assets, không đụng code runtime.
///
/// Nguyên tắc sinh map:
///   1. Lấy danh sách ô từ file .json của tileset. File này cho biết tỉ lệ
///      mỗi ô được dùng nhiều nhất - đó là ý đồ bố trí của tác giả tileset.
///   2. Chia map thành các vùng. Mỗi vùng chọn 1 ô nền, ô đó lặp lại khắp vùng.
///      Nhờ vậy nền đồng nhất từng mảng, giống ảnh preview.
///   3. Rải cụm vật thể (đá, cây, tuyết, vết nứt) theo tỉ lệ của file .json.
///      Vật thể đứng thành cụm chứ không rải đều, nên map không bị lấm tấm.
///
/// Mật độ từng loại landmark được điều chỉnh bằng HowMany trong bảng biome.
/// Chạy lại tool cho ra map giống hệt nhau, vì random có seed cố định.
/// </summary>
public static class RobodenMapBuilder
{
    private const string TilesetRoot = "Assets/Tilesets/Roboden";
    private const string TileAssetRoot = "Assets/Tilesets/Roboden/Tiles";
    // Ô vật thể trang trí được tách thành PNG thật để Unity import.
    private const string DecorSpriteRoot = "Assets/Tilesets/Roboden/Sprites";
    private const string MapRootName = "RobodenMaps";

    // Kích thước map: 30 x 18 ô. Camera nhìn thấy 10 unit, mỗi ô = 1 unit,
    // nên map rộng gấp 3 lần và cao gấp 1.8 lần màn hình.
    private const int Width = 30;
    private const int Height = 18;

    // Sorting Order âm để map vẽ sau Player (Player đang để mặc định 0).
    private const int SortingOrder = -10;

    // Khoảng cách giữa các map, tính bằng đơn vị (ô).
    // 30 ô rộng + 5 ô hở. Nếu đặt cả 4 ở (0,0) chúng sẽ chồng lên nhau
    // và chỉ thấy map vẽ sau cùng, nên mỗi map phải nằm ở một vị trí khác nhau.
    private const float MapSpacing = 35f;

    /// <summary>
    /// Một loại vật thể trang trí: file PNG riêng, cắt thành nhiều ô.
    /// Vì các file này KHÔNG phải tileset lưới vuông, ta tự cắt thành sprite
    /// rồi tạo Tile từ sprite đó.
    /// </summary>
    private enum DecorPlacement
    {
        Solid,
        Cluster,
        Scatter
    }

    private class Decor
    {
        public string FileName;    // tên file trong folder biome
        public int CellWidth;      // bề rộng 1 ô vật thể (tính bằng pixel)
        public int CellHeight;     // chiều cao 1 ô vật thể
        public int Count;          // số ô vật thể có trong file
        public int HowMany;        // số lần rải vào map
        public DecorPlacement Placement;
        public string ClusterGroup;

        public Decor(string file, int w, int h, int count, int howMany,
                     DecorPlacement placement, string clusterGroup = "")
        {
            FileName = file; CellWidth = w; CellHeight = h;
            Count = count; HowMany = howMany;
            Placement = placement; ClusterGroup = clusterGroup;
        }
    }

    private class Biome
    {
        public string Name;
        public string JsonFile;
        public Decor[] Decorations;

        // Số lượng chòm núi và núi đứng lẻ.
        public int RidgeCount;
        public int LoneCount;

        public Biome(string name, string json, Decor[] decorations,
                     int ridges, int lone)
        {
            Name = name; JsonFile = json;
            Decorations = decorations;
            RidgeCount = ridges; LoneCount = lone;
        }
    }

    private static readonly Decor[] ForestDecor =
    {
        new Decor("trees.png", 32, 32, 6, 52, DecorPlacement.Cluster, "trees"),
        new Decor("mountain_big.png", 64, 64, 2, 0, DecorPlacement.Solid),
        new Decor("mountain_medium.png", 48, 48, 2, 0, DecorPlacement.Solid),
        new Decor("mountain_small.png", 32, 32, 2, 0, DecorPlacement.Solid),
        new Decor("mountain_tall.png", 48, 64, 1, 0, DecorPlacement.Solid),
        new Decor("mountain_wide.png", 64, 48, 1, 0, DecorPlacement.Solid),
    };

    private static readonly Decor[] MoonDecor =
    {
        new Decor("landcrack.png", 32, 32, 16, 6, DecorPlacement.Cluster, "cracks"),
        new Decor("landcrack2.png", 32, 32, 16, 6, DecorPlacement.Cluster, "cracks"),
        new Decor("landcrack3.png", 32, 32, 16, 6, DecorPlacement.Cluster, "cracks"),
        new Decor("landcrack4.png", 32, 32, 16, 6, DecorPlacement.Cluster, "cracks"),
        new Decor("oil.png", 32, 32, 8, 7, DecorPlacement.Scatter),
        new Decor("mountain_big.png", 64, 64, 2, 0, DecorPlacement.Solid),
        new Decor("mountain_medium.png", 48, 48, 2, 0, DecorPlacement.Solid),
        new Decor("mountain_small.png", 32, 32, 2, 0, DecorPlacement.Solid),
        new Decor("mountain_tall.png", 48, 64, 1, 0, DecorPlacement.Solid),
        new Decor("mountain_wide.png", 64, 48, 1, 0, DecorPlacement.Solid),
    };

    private static readonly Decor[] InfernoDecor =
    {
        new Decor("lava.png", 32, 32, 9, 5, DecorPlacement.Cluster, "lava"),
        new Decor("lava2.png", 32, 32, 9, 5, DecorPlacement.Cluster, "lava"),
        new Decor("lava3.png", 32, 32, 9, 5, DecorPlacement.Cluster, "lava"),
        new Decor("lava4.png", 32, 32, 9, 5, DecorPlacement.Cluster, "lava"),
        new Decor("lava5.png", 32, 32, 9, 5, DecorPlacement.Cluster, "lava"),
        new Decor("geyser.png", 24, 24, 1, 4, DecorPlacement.Scatter),
        new Decor("mountain_big.png", 64, 64, 3, 0, DecorPlacement.Solid),
        new Decor("mountain_medium.png", 48, 48, 3, 0, DecorPlacement.Solid),
        new Decor("mountain_small.png", 32, 32, 4, 0, DecorPlacement.Solid),
        new Decor("mountain_tall.png", 48, 64, 1, 0, DecorPlacement.Solid),
        new Decor("mountain_wide.png", 64, 48, 1, 0, DecorPlacement.Solid),
    };

    private static readonly Decor[] TundraDecor =
    {
        new Decor("trees.png", 32, 32, 12, 50, DecorPlacement.Cluster, "trees"),
        new Decor("snowpile.png", 32, 32, 9, 4, DecorPlacement.Cluster, "snow"),
        new Decor("snowpile2.png", 32, 32, 9, 4, DecorPlacement.Cluster, "snow"),
        new Decor("snowpile3.png", 32, 32, 9, 4, DecorPlacement.Cluster, "snow"),
        new Decor("snowpile4.png", 32, 32, 9, 4, DecorPlacement.Cluster, "snow"),
        new Decor("snowpile5.png", 32, 32, 9, 4, DecorPlacement.Cluster, "snow"),
        new Decor("landcrack.png", 32, 32, 16, 2, DecorPlacement.Cluster, "cracks"),
        new Decor("landcrack2.png", 32, 32, 16, 2, DecorPlacement.Cluster, "cracks"),
        new Decor("landcrack3.png", 32, 32, 16, 2, DecorPlacement.Cluster, "cracks"),
        new Decor("landcrack4.png", 32, 32, 16, 2, DecorPlacement.Cluster, "cracks"),
        new Decor("mountain_big.png", 64, 64, 2, 0, DecorPlacement.Solid),
        new Decor("mountain_medium.png", 48, 48, 2, 0, DecorPlacement.Solid),
        new Decor("mountain_small.png", 32, 32, 2, 0, DecorPlacement.Solid),
        new Decor("mountain_tall.png", 48, 64, 1, 0, DecorPlacement.Solid),
        new Decor("mountain_wide.png", 64, 48, 1, 0, DecorPlacement.Solid),
    };

    // ridge = số chòm núi san sát, lone = số núi đứng lẻ, small = cây/tuyết.
    // Giữ thấp: map phải còn lối chạy cho player né quái, không nhốt người chơi.
    private static readonly Biome[] Biomes =
    {
        new Biome("Forest", "forest_tiles.json", ForestDecor, 4, 4),
        new Biome("Moon", "moon_tiles.json", MoonDecor, 3, 4),
        new Biome("Inferno", "inferno_tiles.json", InfernoDecor, 3, 4),
        new Biome("Tundra", "snow_tiles.json", TundraDecor, 3, 4),
    };

    private struct ForestPlacement
    {
        public string FileName;
        public int Width;
        public int Height;
        public int Variant;
        public int PixelX;
        public int PixelY;
        public bool BlocksMovement;

        public ForestPlacement(string file, int width, int height, int variant,
                               int x, int y, bool blocks = false)
        {
            FileName = file;
            Width = width;
            Height = height;
            Variant = variant;
            PixelX = x;
            PixelY = y;
            BlocksMovement = blocks;
        }
    }

    // Tọa độ được đối chiếu trực tiếp từ ô Forest 448x288 trong preview gốc.
    private static readonly ForestPlacement[] ForestPlacements =
    {
        new ForestPlacement("mountain_big.png", 64, 64, 0, 109, 26, true),
        new ForestPlacement("mountain_big.png", 64, 64, 0, 81, 3, true),
        new ForestPlacement("mountain_big.png", 64, 64, 1, 78, 57, true),
        new ForestPlacement("mountain_big.png", 64, 64, 1, 333, 155, true),
        new ForestPlacement("mountain_medium.png", 48, 48, 0, 328, 183, true),
        new ForestPlacement("mountain_medium.png", 48, 48, 0, 160, 50, true),
        new ForestPlacement("mountain_medium.png", 48, 48, 1, 358, 195, true),
        new ForestPlacement("mountain_medium.png", 48, 48, 1, 142, 57, true),
        new ForestPlacement("mountain_medium.png", 48, 48, 1, 62, 86, true),
        new ForestPlacement("mountain_medium.png", 48, 48, 1, 201, 14, true),
        new ForestPlacement("mountain_small.png", 32, 32, 0, 175, 83, true),
        new ForestPlacement("mountain_small.png", 32, 32, 0, 5, 229, true),
        new ForestPlacement("mountain_small.png", 32, 32, 0, 49, 7, true),
        new ForestPlacement("mountain_small.png", 32, 32, 0, 226, 43, true),
        new ForestPlacement("mountain_small.png", 32, 32, 1, 203, 43, true),
        new ForestPlacement("mountain_small.png", 32, 32, 1, 324, 217, true),

        // 10 đá vệ tinh rải trên phần ngoài của toàn map 30x18. Tọa độ vượt
        // khung 448x288 là có chủ ý: khung ảnh mẫu chỉ chiếm một phần map lớn.
        new ForestPlacement("mountain_small.png", 32, 32, 1, 0, -144, true),
        new ForestPlacement("mountain_small.png", 32, 32, 0, 176, -128, true),
        new ForestPlacement("mountain_small.png", 32, 32, 1, 368, -160, true),
        new ForestPlacement("mountain_small.png", 32, 32, 0, 560, -112, true),
        new ForestPlacement("mountain_small.png", 32, 32, 1, 784, -144, true),
        new ForestPlacement("mountain_small.png", 32, 32, 0, 560, 48, true),
        new ForestPlacement("mountain_small.png", 32, 32, 1, 752, 80, true),
        new ForestPlacement("mountain_small.png", 32, 32, 0, 816, 224, true),
        new ForestPlacement("mountain_small.png", 32, 32, 1, 592, 304, true),
        new ForestPlacement("mountain_small.png", 32, 32, 0, 272, 320, true),

        // Bốn núi lớn mới làm tâm cho các vùng Forest ngoài khung ảnh mẫu.
        new ForestPlacement("mountain_big.png", 64, 64, 1, 32, -128, true),
        new ForestPlacement("mountain_big.png", 64, 64, 0, 352, -128, true),
        new ForestPlacement("mountain_big.png", 64, 64, 1, 576, 32, true),
        new ForestPlacement("mountain_big.png", 64, 64, 0, 704, 240, true),

        new ForestPlacement("mountain_tall.png", 48, 64, 0, 10, 186, true),
        new ForestPlacement("mountain_wide.png", 64, 48, 0, 261, 15, true),
        new ForestPlacement("mountain_wide.png", 64, 48, 0, 22, 99, true),

        new ForestPlacement("trees.png", 32, 32, 0, 240, 114),
        new ForestPlacement("trees.png", 32, 32, 1, 374, 224),
        new ForestPlacement("trees.png", 32, 32, 1, 279, 239),
        new ForestPlacement("trees.png", 32, 32, 1, 334, 86),
        new ForestPlacement("trees.png", 32, 32, 1, 366, 1),
        new ForestPlacement("trees.png", 32, 32, 1, 297, 185),
        new ForestPlacement("trees.png", 32, 32, 2, 80, 138),
        new ForestPlacement("trees.png", 32, 32, 2, 96, 157),
        new ForestPlacement("trees.png", 32, 32, 2, 401, 11),
        new ForestPlacement("trees.png", 32, 32, 2, 408, 37),
        new ForestPlacement("trees.png", 32, 32, 2, 402, 133),
        new ForestPlacement("trees.png", 32, 32, 2, 61, 239),
        new ForestPlacement("trees.png", 32, 32, 3, 345, 244),
        new ForestPlacement("trees.png", 32, 32, 3, 396, 246),
        new ForestPlacement("trees.png", 32, 32, 3, 76, 173),
        new ForestPlacement("trees.png", 32, 32, 3, 160, 106),
        new ForestPlacement("trees.png", 32, 32, 3, 339, 34),
        new ForestPlacement("trees.png", 32, 32, 3, 385, 76),
        new ForestPlacement("trees.png", 32, 32, 3, 338, 122),
        new ForestPlacement("trees.png", 32, 32, 3, 253, 151),
        new ForestPlacement("trees.png", 32, 32, 4, 395, 178),
        new ForestPlacement("trees.png", 32, 32, 4, 260, 92),
        new ForestPlacement("trees.png", 32, 32, 4, 304, 136),
        new ForestPlacement("trees.png", 32, 32, 4, 240, 204),
        new ForestPlacement("trees.png", 32, 32, 4, 135, 197),
        new ForestPlacement("trees.png", 32, 32, 4, 212, 78),
        new ForestPlacement("trees.png", 32, 32, 4, 409, 214),
        new ForestPlacement("trees.png", 32, 32, 4, 171, 159),
        new ForestPlacement("trees.png", 32, 32, 5, 372, 254),
        new ForestPlacement("trees.png", 32, 32, 5, 291, 65),
        new ForestPlacement("trees.png", 32, 32, 5, 205, 153),
        new ForestPlacement("trees.png", 32, 32, 5, 10, 126),
        new ForestPlacement("trees.png", 32, 32, 5, 12, 27),
        new ForestPlacement("trees.png", 32, 32, 5, 241, 17),
        new ForestPlacement("trees.png", 32, 32, 5, 56, 29),
        new ForestPlacement("trees.png", 32, 32, 5, 164, 222),
        new ForestPlacement("trees.png", 32, 32, 5, 29, 40),
        new ForestPlacement("trees.png", 32, 32, 5, 265, 172),
        new ForestPlacement("trees.png", 32, 32, 5, 27, 12),
    };

    private static void AddOuterForestTrees(List<ForestPlacement> placements)
    {
        Vector2Int[] centers =
        {
            new Vector2Int(130, -95),
            new Vector2Int(600, -95),
            new Vector2Int(650, 75),
            new Vector2Int(680, 235),
            new Vector2Int(350, 325),
        };
        Vector2Int[] radii =
        {
            new Vector2Int(180, 75),
            new Vector2Int(220, 75),
            new Vector2Int(180, 100),
            new Vector2Int(170, 100),
            new Vector2Int(260, 35),
        };

        System.Random random = new System.Random(GetStableSeed("ForestOuterTrees"));
        HashSet<Vector2Int> used = new HashSet<Vector2Int>();

        for (int cluster = 0; cluster < centers.Length; cluster++)
        {
            int placed = 0;
            for (int attempt = 0; attempt < 400 && placed < 16; attempt++)
            {
                int offsetX = random.Next(-radii[cluster].x, radii[cluster].x + 1);
                int offsetY = random.Next(-radii[cluster].y, radii[cluster].y + 1);
                float ellipseX = offsetX / (float)radii[cluster].x;
                float ellipseY = offsetY / (float)radii[cluster].y;
                if (ellipseX * ellipseX + ellipseY * ellipseY > 1f) continue;

                Vector2Int position = new Vector2Int(
                    Mathf.Clamp(centers[cluster].x + offsetX, -64, 864),
                    Mathf.Clamp(centers[cluster].y + offsetY, -192, 352));
                if (!used.Add(position)) continue;

                placements.Add(new ForestPlacement(
                    "trees.png", 32, 32, random.Next(6),
                    position.x, position.y));
                placed++;
            }
        }
    }

    // Bố cục 448x288 được đối chiếu từ ô Moon trong preview gốc.
    private static readonly ForestPlacement[] MoonPlacements =
    {
        new ForestPlacement("mountain_big.png", 64, 64, 0, 127, 190, true),
        new ForestPlacement("mountain_medium.png", 48, 48, 0, 198, 4, true),
        new ForestPlacement("mountain_medium.png", 48, 48, 0, 193, 218, true),
        new ForestPlacement("mountain_medium.png", 48, 48, 1, 73, 106, true),
        new ForestPlacement("mountain_medium.png", 48, 48, 1, 416, 120, true),
        new ForestPlacement("mountain_small.png", 32, 32, 0, 75, 41, true),
        new ForestPlacement("mountain_small.png", 32, 32, 0, 350, 81, true),
        new ForestPlacement("mountain_small.png", 32, 32, 0, 11, 227, true),
        new ForestPlacement("mountain_small.png", 32, 32, 1, 246, 40, true),
        new ForestPlacement("mountain_small.png", 32, 32, 1, 90, 138, true),
        new ForestPlacement("mountain_small.png", 32, 32, 1, 380, 220, true),
        new ForestPlacement("mountain_tall.png", 48, 64, 0, 161, 215, true),
        new ForestPlacement("mountain_wide.png", 64, 48, 0, 168, 30, true),
        new ForestPlacement("mountain_wide.png", 64, 48, 0, 239, 238, true),

        new ForestPlacement("landcrack4.png", 32, 32, 3, 16, 20),
        new ForestPlacement("landcrack4.png", 32, 32, 4, 48, 20),
        new ForestPlacement("landcrack4.png", 32, 32, 8, 12, 52),
        new ForestPlacement("landcrack2.png", 32, 32, 6, 114, 105),
        new ForestPlacement("landcrack.png", 32, 32, 8, 115, 137),
        new ForestPlacement("landcrack.png", 32, 32, 0, 387, 58),
        new ForestPlacement("landcrack2.png", 32, 32, 0, 15, 173),
        new ForestPlacement("landcrack4.png", 32, 32, 2, 318, 150),
        new ForestPlacement("landcrack.png", 32, 32, 2, 354, 150),
        new ForestPlacement("landcrack3.png", 32, 32, 11, 321, 182),
        new ForestPlacement("landcrack3.png", 32, 32, 12, 353, 182),
        new ForestPlacement("landcrack3.png", 32, 32, 8, 317, 214),

        new ForestPlacement("oil.png", 32, 32, 0, 275, 70),
        new ForestPlacement("oil.png", 32, 32, 4, 308, 92),
    };

    private static void AddOuterMoonDecorations(List<ForestPlacement> placements)
    {
        placements.Add(new ForestPlacement("mountain_big.png", 64, 64, 1, 32, -128, true));
        placements.Add(new ForestPlacement("mountain_medium.png", 48, 48, 0, 320, -132, true));
        placements.Add(new ForestPlacement("mountain_wide.png", 64, 48, 0, 600, -140, true));
        placements.Add(new ForestPlacement("mountain_small.png", 32, 32, 1, 820, -120, true));
        placements.Add(new ForestPlacement("mountain_big.png", 64, 64, 0, 560, 20, true));
        placements.Add(new ForestPlacement("mountain_tall.png", 48, 64, 0, 760, 80, true));
        placements.Add(new ForestPlacement("mountain_medium.png", 48, 48, 1, 520, 170, true));
        placements.Add(new ForestPlacement("mountain_small.png", 32, 32, 0, 830, 190, true));
        placements.Add(new ForestPlacement("mountain_big.png", 64, 64, 1, 650, 275, true));
        placements.Add(new ForestPlacement("mountain_wide.png", 64, 48, 0, 300, 310, true));
        placements.Add(new ForestPlacement("mountain_medium.png", 48, 48, 0, 60, 305, true));

        Vector2Int[] crackOrigins =
        {
            new Vector2Int(60, -120),
            new Vector2Int(500, -100),
            new Vector2Int(650, 120),
            new Vector2Int(430, 300),
        };
        foreach (Vector2Int origin in crackOrigins)
        {
            placements.Add(new ForestPlacement("landcrack4.png", 32, 32, 2,
                                               origin.x, origin.y));
            placements.Add(new ForestPlacement("landcrack.png", 32, 32, 2,
                                               origin.x + 32, origin.y));
            placements.Add(new ForestPlacement("landcrack3.png", 32, 32, 11,
                                               origin.x, origin.y + 32));
            placements.Add(new ForestPlacement("landcrack3.png", 32, 32, 12,
                                               origin.x + 32, origin.y + 32));
        }

        Vector2Int[] oilPositions =
        {
            new Vector2Int(20, -20), new Vector2Int(270, -120),
            new Vector2Int(470, 40), new Vector2Int(800, 20),
            new Vector2Int(530, 230), new Vector2Int(790, 300),
            new Vector2Int(200, 320), new Vector2Int(20, 300),
        };
        for (int i = 0; i < oilPositions.Length; i++)
        {
            placements.Add(new ForestPlacement(
                "oil.png", 32, 32, i % 8,
                oilPositions[i].x, oilPositions[i].y));
        }
    }

    // Bố cục 448x288 được đối chiếu từ ô Inferno trong preview gốc.
    private static readonly ForestPlacement[] InfernoPlacements =
    {
        new ForestPlacement("mountain_big.png", 64, 64, 0, 292, 21, true),
        new ForestPlacement("mountain_big.png", 64, 64, 0, 375, 4, true),
        new ForestPlacement("mountain_medium.png", 48, 48, 0, 276, 60, true),
        new ForestPlacement("mountain_medium.png", 48, 48, 0, 386, 146, true),
        new ForestPlacement("mountain_medium.png", 48, 48, 1, 14, 70, true),
        new ForestPlacement("mountain_medium.png", 48, 48, 1, 391, 61, true),
        new ForestPlacement("mountain_medium.png", 48, 48, 2, 50, 101, true),
        new ForestPlacement("mountain_medium.png", 48, 48, 2, 321, 85, true),
        new ForestPlacement("mountain_small.png", 32, 32, 0, 118, 20, true),
        new ForestPlacement("mountain_small.png", 32, 32, 0, 394, 96, true),
        new ForestPlacement("mountain_small.png", 32, 32, 1, 19, 21, true),
        new ForestPlacement("mountain_small.png", 32, 32, 1, 338, 54, true),
        new ForestPlacement("mountain_small.png", 32, 32, 1, 290, 93, true),
        new ForestPlacement("mountain_small.png", 32, 32, 1, 74, 141, true),
        new ForestPlacement("mountain_small.png", 32, 32, 1, 412, 177, true),
        new ForestPlacement("mountain_small.png", 32, 32, 2, 366, 74, true),
        new ForestPlacement("mountain_small.png", 32, 32, 2, 322, 116, true),
        new ForestPlacement("mountain_small.png", 32, 32, 2, 398, 188, true),
        new ForestPlacement("mountain_small.png", 32, 32, 3, 48, 132, true),
        new ForestPlacement("mountain_small.png", 32, 32, 3, 293, 134, true),

        // Hồ dung nham lớn 3x3.
        new ForestPlacement("lava.png", 32, 32, 0, 167, 36),
        new ForestPlacement("lava.png", 32, 32, 1, 199, 36),
        new ForestPlacement("lava.png", 32, 32, 2, 231, 36),
        new ForestPlacement("lava2.png", 32, 32, 3, 166, 68),
        new ForestPlacement("lava2.png", 32, 32, 4, 198, 68),
        new ForestPlacement("lava2.png", 32, 32, 5, 230, 68),
        new ForestPlacement("lava2.png", 32, 32, 6, 167, 100),
        new ForestPlacement("lava2.png", 32, 32, 7, 199, 100),
        new ForestPlacement("lava2.png", 32, 32, 8, 231, 100),

        // Hồ dung nham nhỏ 2x2.
        new ForestPlacement("lava.png", 32, 32, 0, 299, 177),
        new ForestPlacement("lava4.png", 32, 32, 2, 333, 177),
        new ForestPlacement("lava3.png", 32, 32, 6, 298, 209),
        new ForestPlacement("lava3.png", 32, 32, 8, 330, 209),

        new ForestPlacement("geyser.png", 24, 24, 0, 48, 231),
    };

    private static void AddOuterInfernoDecorations(List<ForestPlacement> placements)
    {
        placements.Add(new ForestPlacement("mountain_big.png", 64, 64, 1, 20, -128, true));
        placements.Add(new ForestPlacement("mountain_medium.png", 48, 48, 2, 300, -120, true));
        placements.Add(new ForestPlacement("mountain_wide.png", 64, 48, 0, 600, -140, true));
        placements.Add(new ForestPlacement("mountain_big.png", 64, 64, 2, 800, -130, true));
        placements.Add(new ForestPlacement("mountain_small.png", 32, 32, 3, 540, 20, true));
        placements.Add(new ForestPlacement("mountain_big.png", 64, 64, 0, 730, 60, true));
        placements.Add(new ForestPlacement("mountain_medium.png", 48, 48, 1, 540, 170, true));
        placements.Add(new ForestPlacement("mountain_small.png", 32, 32, 2, 820, 180, true));
        placements.Add(new ForestPlacement("mountain_big.png", 64, 64, 2, 650, 280, true));
        placements.Add(new ForestPlacement("mountain_wide.png", 64, 48, 0, 300, 310, true));
        placements.Add(new ForestPlacement("mountain_medium.png", 48, 48, 0, 50, 305, true));

        // Một hồ lớn ở dải trên ngoài khung ảnh mẫu.
        int largeLavaX = 480;
        int largeLavaY = -120;
        for (int column = 0; column < 3; column++)
        {
            placements.Add(new ForestPlacement(
                "lava.png", 32, 32, column,
                largeLavaX + column * 32, largeLavaY));
            placements.Add(new ForestPlacement(
                "lava2.png", 32, 32, column + 3,
                largeLavaX + column * 32, largeLavaY + 32));
            placements.Add(new ForestPlacement(
                "lava2.png", 32, 32, column + 6,
                largeLavaX + column * 32, largeLavaY + 64));
        }

        Vector2Int[] smallLavaOrigins =
        {
            new Vector2Int(720, -40),
            new Vector2Int(550, 150),
            new Vector2Int(760, 260),
            new Vector2Int(150, 310),
        };
        foreach (Vector2Int origin in smallLavaOrigins)
        {
            placements.Add(new ForestPlacement("lava.png", 32, 32, 0,
                                               origin.x, origin.y));
            placements.Add(new ForestPlacement("lava4.png", 32, 32, 2,
                                               origin.x + 34, origin.y));
            placements.Add(new ForestPlacement("lava3.png", 32, 32, 6,
                                               origin.x - 1, origin.y + 32));
            placements.Add(new ForestPlacement("lava3.png", 32, 32, 8,
                                               origin.x + 31, origin.y + 32));
        }

        Vector2Int[] geyserPositions =
        {
            new Vector2Int(80, -30), new Vector2Int(330, -140),
            new Vector2Int(510, 50), new Vector2Int(820, 100),
            new Vector2Int(520, 280), new Vector2Int(250, 320),
        };
        foreach (Vector2Int position in geyserPositions)
        {
            placements.Add(new ForestPlacement(
                "geyser.png", 24, 24, 0, position.x, position.y));
        }
    }

    // Bố cục 448x288 được đối chiếu từ ô Tundra trong preview gốc.
    private static readonly ForestPlacement[] TundraPlacements =
    {
        new ForestPlacement("mountain_big.png", 64, 64, 0, 0, 242, true),
        new ForestPlacement("mountain_big.png", 64, 64, 0, 295, 202, true),
        new ForestPlacement("mountain_big.png", 64, 64, 1, 385, 236, true),
        new ForestPlacement("mountain_medium.png", 48, 48, 0, 82, 240, true),
        new ForestPlacement("mountain_medium.png", 48, 48, 0, 320, 24, true),
        new ForestPlacement("mountain_medium.png", 48, 48, 0, 333, 227, true),
        new ForestPlacement("mountain_medium.png", 48, 48, 1, 232, 195, true),
        new ForestPlacement("mountain_medium.png", 48, 48, 1, 416, 164, true),
        new ForestPlacement("mountain_small.png", 32, 32, 0, 339, 56, true),
        new ForestPlacement("mountain_small.png", 32, 32, 0, 243, 228, true),
        new ForestPlacement("mountain_small.png", 32, 32, 0, 133, 235, true),
        new ForestPlacement("mountain_small.png", 32, 32, 1, 364, 31, true),
        new ForestPlacement("mountain_small.png", 32, 32, 1, 104, 199, true),
        new ForestPlacement("mountain_tall.png", 48, 64, 0, 303, 203, true),

        // Hai mảng tuyết 3x3 trong ảnh mẫu.
        new ForestPlacement("snowpile3.png", 32, 32, 0, 73, 182),
        new ForestPlacement("snowpile3.png", 32, 32, 1, 105, 182),
        new ForestPlacement("snowpile3.png", 32, 32, 2, 137, 182),
        new ForestPlacement("snowpile3.png", 32, 32, 3, 73, 214),
        new ForestPlacement("snowpile3.png", 32, 32, 4, 105, 214),
        new ForestPlacement("snowpile3.png", 32, 32, 5, 137, 214),
        new ForestPlacement("snowpile3.png", 32, 32, 6, 73, 246),
        new ForestPlacement("snowpile3.png", 32, 32, 7, 105, 246),
        new ForestPlacement("snowpile3.png", 32, 32, 8, 137, 246),
        new ForestPlacement("snowpile3.png", 32, 32, 0, 288, 84),
        new ForestPlacement("snowpile3.png", 32, 32, 1, 320, 84),
        new ForestPlacement("snowpile3.png", 32, 32, 2, 352, 84),
        new ForestPlacement("snowpile3.png", 32, 32, 3, 288, 116),
        new ForestPlacement("snowpile3.png", 32, 32, 4, 320, 116),
        new ForestPlacement("snowpile3.png", 32, 32, 5, 352, 116),
        new ForestPlacement("snowpile3.png", 32, 32, 6, 288, 148),
        new ForestPlacement("snowpile3.png", 32, 32, 7, 320, 148),
        new ForestPlacement("snowpile3.png", 32, 32, 8, 352, 148),

        new ForestPlacement("landcrack.png", 32, 32, 1, 3, 5),
        new ForestPlacement("landcrack3.png", 32, 32, 5, 35, 6),
        new ForestPlacement("landcrack.png", 32, 32, 0, 397, 81),

        new ForestPlacement("trees.png", 32, 32, 0, 24, 74),
        new ForestPlacement("trees.png", 32, 32, 1, 397, 208),
        new ForestPlacement("trees.png", 32, 32, 1, 331, 97),
        new ForestPlacement("trees.png", 32, 32, 2, 212, 26),
        new ForestPlacement("trees.png", 32, 32, 2, 51, 33),
        new ForestPlacement("trees.png", 32, 32, 2, 42, 109),
        new ForestPlacement("trees.png", 32, 32, 3, 138, 174),
        new ForestPlacement("trees.png", 32, 32, 3, 192, 231),
        new ForestPlacement("trees.png", 32, 32, 4, 292, 68),
        new ForestPlacement("trees.png", 32, 32, 4, 343, 120),
        new ForestPlacement("trees.png", 32, 32, 4, 201, 142),
        new ForestPlacement("trees.png", 32, 32, 4, 223, 158),
        new ForestPlacement("trees.png", 32, 32, 4, 320, 159),
        new ForestPlacement("trees.png", 32, 32, 4, 270, 166),
        new ForestPlacement("trees.png", 32, 32, 4, 178, 31),
        new ForestPlacement("trees.png", 32, 32, 5, 138, 36),
        new ForestPlacement("trees.png", 32, 32, 5, 162, 46),
        new ForestPlacement("trees.png", 32, 32, 6, 362, 132),
        new ForestPlacement("trees.png", 32, 32, 6, 5, 160),
        new ForestPlacement("trees.png", 32, 32, 6, 40, 194),
        new ForestPlacement("trees.png", 32, 32, 6, 58, 218),
        new ForestPlacement("trees.png", 32, 32, 7, 350, 153),
        new ForestPlacement("trees.png", 32, 32, 8, 382, 153),
        new ForestPlacement("trees.png", 32, 32, 9, 175, 99),
        new ForestPlacement("trees.png", 32, 32, 9, 72, 112),
        new ForestPlacement("trees.png", 32, 32, 9, 150, 117),
        new ForestPlacement("trees.png", 32, 32, 9, 105, 144),
        new ForestPlacement("trees.png", 32, 32, 9, 155, 187),
        new ForestPlacement("trees.png", 32, 32, 11, 127, 68),
        new ForestPlacement("trees.png", 32, 32, 11, 199, 68),
        new ForestPlacement("trees.png", 32, 32, 11, 4, 93),
        new ForestPlacement("trees.png", 32, 32, 11, 268, 99),
        new ForestPlacement("trees.png", 32, 32, 11, 65, 154),
        new ForestPlacement("trees.png", 32, 32, 11, 248, 159),
        new ForestPlacement("trees.png", 32, 32, 11, 199, 198),
    };

    private static void AddOuterTundraDecorations(List<ForestPlacement> placements)
    {
        placements.Add(new ForestPlacement("mountain_big.png", 64, 64, 1, 20, -128, true));
        placements.Add(new ForestPlacement("mountain_medium.png", 48, 48, 0, 300, -125, true));
        placements.Add(new ForestPlacement("mountain_wide.png", 64, 48, 0, 600, -140, true));
        placements.Add(new ForestPlacement("mountain_small.png", 32, 32, 1, 820, -120, true));
        placements.Add(new ForestPlacement("mountain_big.png", 64, 64, 0, 570, 25, true));
        placements.Add(new ForestPlacement("mountain_tall.png", 48, 64, 0, 760, 85, true));
        placements.Add(new ForestPlacement("mountain_medium.png", 48, 48, 1, 530, 185, true));
        placements.Add(new ForestPlacement("mountain_small.png", 32, 32, 0, 830, 190, true));
        placements.Add(new ForestPlacement("mountain_big.png", 64, 64, 1, 660, 280, true));
        placements.Add(new ForestPlacement("mountain_medium.png", 48, 48, 0, 40, 305, true));

        Vector2Int[] snowOrigins =
        {
            new Vector2Int(80, -130),
            new Vector2Int(500, -115),
            new Vector2Int(650, 145),
            new Vector2Int(300, 300),
        };
        foreach (Vector2Int origin in snowOrigins)
        {
            for (int variant = 0; variant < 9; variant++)
            {
                placements.Add(new ForestPlacement(
                    "snowpile3.png", 32, 32, variant,
                    origin.x + variant % 3 * 32,
                    origin.y + variant / 3 * 32));
            }
        }

        Vector2Int[] crackPositions =
        {
            new Vector2Int(220, -100), new Vector2Int(760, -20),
            new Vector2Int(500, 80), new Vector2Int(820, 260),
            new Vector2Int(510, 310), new Vector2Int(20, 300),
        };
        foreach (Vector2Int position in crackPositions)
        {
            placements.Add(new ForestPlacement(
                "landcrack.png", 32, 32, 0, position.x, position.y));
        }

        Vector2Int[] treeCenters =
        {
            new Vector2Int(130, -95), new Vector2Int(600, -95),
            new Vector2Int(650, 75), new Vector2Int(680, 235),
            new Vector2Int(350, 325),
        };
        Vector2Int[] treeRadii =
        {
            new Vector2Int(180, 75), new Vector2Int(220, 75),
            new Vector2Int(180, 100), new Vector2Int(170, 100),
            new Vector2Int(260, 35),
        };
        System.Random random = new System.Random(GetStableSeed("TundraOuterTrees"));
        HashSet<Vector2Int> used = new HashSet<Vector2Int>();

        for (int cluster = 0; cluster < treeCenters.Length; cluster++)
        {
            int placed = 0;
            for (int attempt = 0; attempt < 400 && placed < 14; attempt++)
            {
                int offsetX = random.Next(-treeRadii[cluster].x, treeRadii[cluster].x + 1);
                int offsetY = random.Next(-treeRadii[cluster].y, treeRadii[cluster].y + 1);
                float ellipseX = offsetX / (float)treeRadii[cluster].x;
                float ellipseY = offsetY / (float)treeRadii[cluster].y;
                if (ellipseX * ellipseX + ellipseY * ellipseY > 1f) continue;

                Vector2Int position = new Vector2Int(
                    Mathf.Clamp(treeCenters[cluster].x + offsetX, -64, 864),
                    Mathf.Clamp(treeCenters[cluster].y + offsetY, -192, 352));
                if (!used.Add(position)) continue;

                placements.Add(new ForestPlacement(
                    "trees.png", 32, 32, random.Next(12),
                    position.x, position.y));
                placed++;
            }
        }
    }

    private static void CreateReferenceDecorations(
        Transform parent, Biome biome, List<ForestPlacement> placements)
    {
        const int referenceHeight = 288;
        const float pixelsPerUnit = 32f;
        Vector2 referenceOrigin = new Vector2(2f, 3f);
        Dictionary<string, Sprite> loadedSprites = new Dictionary<string, Sprite>();

        string biomeFolder = DecorSpriteRoot + "/" + biome.Name;
        string wholeSpriteFolder = biomeFolder + "/Whole";
        EnsureFolder(DecorSpriteRoot);
        EnsureFolder(biomeFolder);
        EnsureFolder(wholeSpriteFolder);

        for (int i = 0; i < placements.Count; i++)
        {
            ForestPlacement placement = placements[i];
            string spriteKey = placement.FileName + "_" + placement.Variant;
            Sprite sprite;

            if (!loadedSprites.TryGetValue(spriteKey, out sprite))
            {
                string assetName = Path.GetFileNameWithoutExtension(placement.FileName);
                string outputPath = wholeSpriteFolder + "/" + assetName + "_" +
                                    placement.Variant + ".png";
                string sourcePath = TilesetRoot + "/" + biome.Name + "/" +
                                    placement.FileName;

                Texture2D source = new Texture2D(2, 2, TextureFormat.RGBA32, false);
                source.LoadImage(File.ReadAllBytes(sourcePath));
                Color[] pixels = source.GetPixels(
                    placement.Variant * placement.Width,
                    0,
                    placement.Width,
                    placement.Height);

                Texture2D extracted = new Texture2D(
                    placement.Width, placement.Height, TextureFormat.RGBA32, false);
                extracted.SetPixels(pixels);
                extracted.Apply();
                File.WriteAllBytes(outputPath, extracted.EncodeToPNG());
                Object.DestroyImmediate(source);
                Object.DestroyImmediate(extracted);

                AssetDatabase.ImportAsset(outputPath, ImportAssetOptions.ForceUpdate);
                TextureImporter importer = AssetImporter.GetAtPath(outputPath) as TextureImporter;
                importer.textureType = TextureImporterType.Sprite;
                importer.spriteImportMode = SpriteImportMode.Single;
                importer.spritePixelsPerUnit = pixelsPerUnit;
                importer.filterMode = FilterMode.Point;
                importer.mipmapEnabled = false;
                importer.alphaIsTransparency = true;
                importer.textureCompression = TextureImporterCompression.Uncompressed;
                importer.SaveAndReimport();

                sprite = AssetDatabase.LoadAssetAtPath<Sprite>(outputPath);
                loadedSprites.Add(spriteKey, sprite);
            }

            GameObject decoration = new GameObject(
                biome.Name + "_" + Path.GetFileNameWithoutExtension(placement.FileName) +
                "_" + i.ToString("00"));
            decoration.transform.SetParent(parent, false);
            decoration.transform.localPosition = new Vector3(
                referenceOrigin.x + (placement.PixelX + placement.Width * 0.5f) /
                                    pixelsPerUnit,
                referenceOrigin.y + (referenceHeight - placement.PixelY -
                                    placement.Height * 0.5f) / pixelsPerUnit,
                0f);

            SpriteRenderer renderer = decoration.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            int depth = Mathf.Clamp((placement.PixelY + 192) / 64, 0, 8);
            renderer.sortingOrder = SortingOrder + 1 + depth;

            if (placement.BlocksMovement)
            {
                float spriteWidth = placement.Width / pixelsPerUnit;
                float spriteHeight = placement.Height / pixelsPerUnit;
                float colliderHeight = Mathf.Min(0.55f, spriteHeight * 0.28f);
                BoxCollider2D mountainCollider = decoration.AddComponent<BoxCollider2D>();
                mountainCollider.size = new Vector2(spriteWidth * 0.72f, colliderHeight);
                mountainCollider.offset = new Vector2(
                    0f,
                    -spriteHeight * 0.5f + colliderHeight * 0.5f + 0.05f);
            }
        }
    }

    [MenuItem("Tools/Brotato/Generate Roboden Maps")]
    public static void GenerateMaps()
    {
        // Không được chạy khi đang Play: MarkSceneDirty/SaveScene sẽ ném
        // InvalidOperationException và scene không được lưu, nên sau đó vẫn
        // thấy map của lần chạy trước.
        if (EditorApplication.isPlayingOrWillChangePlaymode)
        {
            EditorUtility.DisplayDialog("Roboden Maps",
                "Dang o PLAY MODE.\n\nHay thoat Play (Ctrl+P) truoc khi tao map.",
                "OK");
            return;
        }

        if (!EditorUtility.DisplayDialog(
                "Generate Roboden Maps",
                "Tao 4 Tilemap moi trong scene hien tai.\n\n" +
                "Cac Tilemap cu cung ten se bi xoa de tao lai.\n" +
                "Scene se duoc luu lai.\n\nTien tuc?",
                "Tao", "Huy"))
            return;

        // Xoá các Tile asset cũ: bản trước tạo sprite lúc runtime nên
        // m_Sprite bị null, để lại sẽ hiện ô vuông xanh (màu mặc định của
        // TilemapRenderer) thay vì hình vật thể.
        if (AssetDatabase.IsValidFolder(TileAssetRoot))
            AssetDatabase.DeleteAsset(TileAssetRoot);
        if (AssetDatabase.IsValidFolder(DecorSpriteRoot))
            AssetDatabase.DeleteAsset(DecorSpriteRoot);
        AssetDatabase.Refresh();

        GameObject existing = GameObject.Find(MapRootName);
        if (existing != null) Undo.DestroyObjectImmediate(existing);

        GameObject root = new GameObject(MapRootName);
        Undo.RegisterCreatedObjectUndo(root, "Generate Roboden Maps");
        root.transform.position = Vector3.zero;

        Grid grid = root.AddComponent<Grid>();
        grid.cellSize = new Vector3(1f, 1f, 0f);

        // Bọc từng biome riêng: nếu một biome lỗi thì các biome còn lại vẫn
        // được tạo, và scene luôn được lưu. Trước đây một IndexOutOfRange
        // làm cả tool chết trước khi lưu, scene giữ nguyên bản cũ nên trông
        // như "không có gì thay đổi".
        for (int i = 0; i < Biomes.Length; i++)
        {
            try
            {
                GenerateBiome(grid.transform, Biomes[i], i);
            }
            catch (System.Exception ex)
            {
                Debug.LogError("[RobodenMapBuilder] Biome " + Biomes[i].Name +
                               " that bai: " + ex.Message + "\n" + ex.StackTrace);
            }
        }

        // Phai luu AssetDatabase: CreateAsset chi ghi file ngay luc tao (khi do
        // m_Sprite chua gan), cac thay doi sau do chi nam trong RAM.
        AssetDatabase.SaveAssets();
        Scene activeScene = SceneManager.GetActiveScene();
        EditorSceneManager.MarkSceneDirty(activeScene);
        EditorSceneManager.SaveScene(activeScene);

        // Dem tile thieu sprite bang cach DOC FILE TREN DIA, khong dung RAM.
        // AssetDatabase.LoadAssetAtPath tra ve doi tuong da gan sprite trong
        // bo nho nen luon bao 0 loi, con file tren dia thi chua - dung lai
        // Unity lai se hien o vuong mau nen cua Camera.
        int broken = 0, total = 0;
        string[] files = Directory.GetFiles(TileAssetRoot, "*.asset",
                                             SearchOption.AllDirectories);
        for (int i = 0; i < files.Length; i++)
        {
            total++;
            string text = File.ReadAllText(files[i]);
            if (text.Contains("m_Sprite: {fileID: 0}")) broken++;
        }
        // Chi cannh bao khi con tile hong. Neu khong thi in Log binh thuong,
        // tranh bi coi nham la loi.
        string tileReport = "[RobodenMapBuilder] Tile thieu sprite tren dia: " +
                            broken + " / " + total;
        if (broken > 0) Debug.LogWarning(tileReport);
        else Debug.Log(tileReport);

        Selection.activeGameObject = root;

        // In ra toạ độ. Mỗi map gồm 2 lớp Tilemap chồng nhau (nền + vật thể)
        // nên có 8 Tilemap cho 4 map. Gom theo vị trí để đọc dễ.
        System.Text.StringBuilder report = new System.Text.StringBuilder();
        Tilemap[] maps = root.GetComponentsInChildren<Tilemap>();
        System.Collections.Generic.Dictionary<Vector3, string> byPos =
            new System.Collections.Generic.Dictionary<Vector3, string>();
        for (int i = 0; i < maps.Length; i++)
        {
            Vector3 p = maps[i].transform.localPosition;
            string label;
            if (!byPos.TryGetValue(p, out label))
            {
                label = maps[i].name.Replace("Tilemap_", "").Replace("_Ground", "");
                byPos[p] = label;
            }
            Bounds b = maps[i].localBounds;
            report.AppendLine("  " + label + ": pos=" + p +
                              ", size=" + (int)b.size.x + "x" + (int)b.size.y +
                              " (" + maps[i].name + ")");
        }
        Debug.Log("[RobodenMapBuilder] Da tao " + byPos.Count + " map trong '" +
                  MapRootName + "', gồm " + maps.Length + " lớp Tilemap:\n" + report);
    }

    private static void GenerateBiome(Transform gridRoot, Biome biome, int mapIndex)
    {
        string texturePath = TilesetRoot + "/" + biome.Name + "/tiles.png";
        string jsonPath = TilesetRoot + "/" + biome.Name + "/" + biome.JsonFile;

        Sprite[] sprites = LoadSprites(texturePath);
        if (sprites.Length == 0)
        {
            Debug.LogError("[RobodenMapBuilder] Khong tim thay sprite: " + texturePath);
            return;
        }

        List<float> weights = LoadWeights(jsonPath, sprites.Length);
        if (weights == null)
        {
            Debug.LogError("[RobodenMapBuilder] Khong doc duoc file trong so: " + jsonPath);
            return;
        }

        Tilemap tilemap = CreateTilemap(gridRoot, biome.Name + "_Ground", SortingOrder);

        // Tilemap thứ hai chứa núi/cây. Nếu vẽ núi trực tiếp lên Tilemap nền
        // thì ô trong suốt của núi sẽ XOA mất ô cỏ bên dưới, lộ ra màu nền
        // của Camera (xanh dương). Tách riêng thì cỏ vẫn hiện xuyên qua.
        Tilemap decor = CreateTilemap(gridRoot, biome.Name + "_Decor", SortingOrder + 1);

        // Căn map quanh trục Y=0 để Player/camera bắt đầu bên trong Forest.
        // Các biome tiếp theo vẫn nằm ngang, cách nhau một khoảng nhỏ.
        Vector3 mapOffset = new Vector3(
            mapIndex * MapSpacing - Width * 0.5f,
            -Height * 0.5f,
            0f);
        tilemap.transform.localPosition = mapOffset;
        decor.transform.localPosition = mapOffset;

        // Random có seed cố định theo tên biome: chạy lại vẫn ra cùng map,
        // nên khi sửa code thì so sánh được với kết quả cũ.
        System.Random random = new System.Random(GetStableSeed(biome.Name));

        PaintWeightedGround(tilemap, weights, random, sprites);

        // Tilemap thứ ba chứa Ô VA CHẠM của vật cản. Nó cố tình ẩn (không vẽ gì),
        // chỉ dùng để sinh hình học cho CompositeCollider2D. Nếu gộp va chạm
        // vào Decor thì cả cây nhỏ/lông tuyết cũng chặn người chơi.
        Tilemap block = CreateTilemap(gridRoot, biome.Name + "_Block", SortingOrder + 2);
        block.GetComponent<TilemapRenderer>().enabled = false;
        block.transform.localPosition = mapOffset;

        // Rigidbody2D kiểu Static là điều kiện bắt buộc để CompositeCollider2D
        // sinh hình học tĩnh cho nhân vật dùng Rigidbody2D động va chạm vào.
        Rigidbody2D body = block.gameObject.AddComponent<Rigidbody2D>();
        body.bodyType = RigidbodyType2D.Static;

        // TilemapCollider2D là nguồn sinh hình học; CompositeCollider2D gộp
        // các cạnh chạm nhau thành đa giác liền mạch, giảm số collider và
        // tránh người chơi kẹt vào khe giữa hai ô vuông.
        TilemapCollider2D tileCollider = block.gameObject.AddComponent<TilemapCollider2D>();
        tileCollider.isTrigger = false;
        tileCollider.compositeOperation = Collider2D.CompositeOperation.Merge;
        CompositeCollider2D collider = block.gameObject.AddComponent<CompositeCollider2D>();
        collider.geometryType = CompositeCollider2D.GeometryType.Polygons;
        collider.generationType = CompositeCollider2D.GenerationType.Synchronous;

        if (biome.Name == "Forest")
        {
            List<ForestPlacement> forestPlacements =
                new List<ForestPlacement>(ForestPlacements);
            AddOuterForestTrees(forestPlacements);
            CreateReferenceDecorations(decor.transform, biome, forestPlacements);
        }
        else if (biome.Name == "Moon")
        {
            List<ForestPlacement> moonPlacements =
                new List<ForestPlacement>(MoonPlacements);
            AddOuterMoonDecorations(moonPlacements);
            CreateReferenceDecorations(decor.transform, biome, moonPlacements);
        }
        else if (biome.Name == "Inferno")
        {
            List<ForestPlacement> infernoPlacements =
                new List<ForestPlacement>(InfernoPlacements);
            AddOuterInfernoDecorations(infernoPlacements);
            CreateReferenceDecorations(decor.transform, biome, infernoPlacements);
        }
        else if (biome.Name == "Tundra")
        {
            List<ForestPlacement> tundraPlacements =
                new List<ForestPlacement>(TundraPlacements);
            AddOuterTundraDecorations(tundraPlacements);
            CreateReferenceDecorations(decor.transform, biome, tundraPlacements);
        }
        else
            ScatterDecorations(decor, biome, random, block);

        Debug.Log("[RobodenMapBuilder] " + biome.Name + ": " + sprites.Length +
                  " loai tile nen co trong so, " + biome.Decorations.Length +
                  " bo trang tri.");
    }

    private static Tilemap CreateTilemap(Transform parent, string name, int order)
    {
        GameObject go = new GameObject("Tilemap_" + name);
        go.transform.SetParent(parent, false);
        go.transform.localPosition = Vector3.zero;

        Tilemap tilemap = go.AddComponent<Tilemap>();
        TilemapRenderer renderer = go.AddComponent<TilemapRenderer>();
        renderer.sortingOrder = order;
        return tilemap;
    }

    private static int GetStableSeed(string value)
    {
        unchecked
        {
            int hash = 17;
            foreach (char character in value) hash = hash * 31 + character;
            return hash;
        }
    }

    private static Sprite[] LoadSprites(string texturePath)
    {
        Object[] assets = AssetDatabase.LoadAllAssetsAtPath(texturePath);
        List<Sprite> sprites = new List<Sprite>();
        foreach (Object asset in assets)
        {
            Sprite sprite = asset as Sprite;
            if (sprite != null) sprites.Add(sprite);
        }
        // Sắp xếp theo số thứ tự trong tên (tiles_0, tiles_1, ...)
        // để thứ tự ô luôn giống nhau giữa các lần chạy.
        sprites.Sort(delegate(Sprite a, Sprite b)
        {
            return ParseTileIndex(a.name).CompareTo(ParseTileIndex(b.name));
        });
        return sprites.ToArray();
    }

    private static int ParseTileIndex(string spriteName)
    {
        int underscore = spriteName.LastIndexOf('_');
        int value;
        if (underscore >= 0 &&
            int.TryParse(spriteName.Substring(underscore + 1), out value))
            return value;
        return 0;
    }

    private static void PaintWeightedGround(Tilemap tilemap, List<float> weights,
                                            System.Random random, Sprite[] sprites)
    {
        float totalWeight = 0f;
        int count = Mathf.Min(weights.Count, sprites.Length);
        for (int i = 0; i < count; i++)
            totalWeight += Mathf.Max(0f, weights[i]);

        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                float roll = (float)random.NextDouble() * totalWeight;
                int index = 0;
                for (; index < count - 1; index++)
                {
                    roll -= Mathf.Max(0f, weights[index]);
                    if (roll <= 0f) break;
                }

                tilemap.SetTile(new Vector3Int(x, y, 0),
                                GetOrCreateTile(sprites[index], tilemap));
            }
        }
    }

    /// <summary>
    /// Dựng ba lớp bố cục giống ảnh mẫu: núi thành dãy, landmark thành cụm
    /// và chi tiết nhỏ rải thưa. Các ô đã dùng không chồng lên nhau để vẫn
    /// giữ được hành lang trống cho gameplay.
    /// </summary>
    private static void ScatterDecorations(Tilemap tilemap, Biome biome,
                                           System.Random random, Tilemap colliderMap)
    {
        bool[,] occupied = new bool[Height, Width];
        List<DecorBlock> blocks = LoadDecorBlocks(biome);

        if (blocks.Count == 0)
        {
            Debug.LogWarning("[RobodenMapBuilder] Khong tim thay khoi trang tri nao cho " + biome.Name);
            return;
        }

        List<DecorBlock> mountains = new List<DecorBlock>();
        List<DecorBlock> scattered = new List<DecorBlock>();
        Dictionary<string, List<DecorBlock>> clusterGroups =
            new Dictionary<string, List<DecorBlock>>();
        foreach (DecorBlock block in blocks)
        {
            if (block.Placement == DecorPlacement.Solid)
            {
                mountains.Add(block);
            }
            else if (block.Placement == DecorPlacement.Scatter)
            {
                scattered.Add(block);
            }
            else
            {
                List<DecorBlock> group;
                if (!clusterGroups.TryGetValue(block.ClusterGroup, out group))
                {
                    group = new List<DecorBlock>();
                    clusterGroups.Add(block.ClusterGroup, group);
                }
                group.Add(block);
            }
        }

        // 1. DÃY NÚI: đi theo một hướng, các núi san sát nhau.
        for (int c = 0; c < biome.RidgeCount && mountains.Count > 0; c++)
        {
            DecorBlock block = mountains[random.Next(mountains.Count)];
            if (Width - block.CellsWide < 2 || Height - block.CellsHigh < 2) continue;

            int x = random.Next(1, Width - block.CellsWide);
            int y = random.Next(1, Height - block.CellsHigh);
            float angle = (float)random.NextDouble() * Mathf.PI * 2f;
            // Chòm chỉ 2-4 núi, không phải cả dãy dài.
            int length = random.Next(3, 5);

            // KHÔNG cho phép núi trong cùng chòm chồng lên nhau. Bước đi
            // đơn trục ở dưới đã bảo đảm các núi liền nhau mà không chồng,
            // nên kiểm tra chỗ trống vẫn giữ được chòm núi liền mạch.
            for (int i = 0; i < length; i++)
            {
                if (!IsAreaFree(occupied, x, y, block.CellsWide, block.CellsHigh))
                    break;
                if (!StampBlock(tilemap, block, x, y, random, colliderMap)) break;
                MarkArea(occupied, x, y, block.CellsWide, block.CellsHigh);

                // Bước tới theo hướng hiện tại. Bước bằng đúng bề rội khối để
                // núi kế tiếp nằm sát núi trước.
                //
                // PHẢI dịch theo đúng MỘT trục. Nếu dịch cả X và Y (vd (1,1))
                // thì khối 2x2 mới sẽ chồng đúng một ô vào khối cũ, ra nui
                // khuyết mất mảng. Vì vậy chỉ giữ lại trục nào dịch nhiều hơn.
                float step = block.CellsWide;
                angle += ((float)random.NextDouble() - 0.5f) * 0.7f;
                int dx = Mathf.RoundToInt(Mathf.Cos(angle) * step);
                int dy = Mathf.RoundToInt(Mathf.Sin(angle) * step);
                if (dx != 0 && dy != 0)
                {
                    if (Mathf.Abs(dx) >= Mathf.Abs(dy)) dy = 0; else dx = 0;
                }
                x = Mathf.Clamp(x + dx, 1, Width - block.CellsWide - 1);
                y = Mathf.Clamp(y + dy, 1, Height - block.CellsHigh - 1);
            }
        }

        // 2. NÚI ĐỨNG LẺ ở chỗ còn trống.
        for (int c = 0; c < biome.LoneCount && mountains.Count > 0; c++)
        {
            DecorBlock block = mountains[random.Next(mountains.Count)];
            if (Width - block.CellsWide < 2 || Height - block.CellsHigh < 2) continue;

            for (int attempt = 0; attempt < 20; attempt++)
            {
                int x = random.Next(1, Width - block.CellsWide);
                int y = random.Next(1, Height - block.CellsHigh);
                if (!IsAreaFree(occupied, x, y, block.CellsWide, block.CellsHigh))
                    continue;
                if (StampBlock(tilemap, block, x, y, random, colliderMap))
                    MarkArea(occupied, x, y, block.CellsWide, block.CellsHigh);
                break;
            }
        }

        // 3. CÂY / LAVA / KHE NỨT / TUYẾT: đi thành các mảng hữu cơ.
        foreach (List<DecorBlock> group in clusterGroups.Values)
            PlaceClusters(tilemap, group, occupied, random);

        // 4. Dầu, geyser và chi tiết nhỏ: rải thưa ở phần còn trống.
        foreach (DecorBlock block in scattered)
            PlaceScattered(tilemap, block, occupied, random);
    }

    private static void PlaceClusters(Tilemap tilemap, List<DecorBlock> blocks,
                                      bool[,] occupied, System.Random random)
    {
        int remaining = 0;
        foreach (DecorBlock block in blocks) remaining += block.HowMany;
        int clusterCount = Mathf.Max(1, Mathf.CeilToInt(remaining / 10f));

        for (int cluster = 0; cluster < clusterCount && remaining > 0; cluster++)
        {
            int target = Mathf.CeilToInt((float)remaining / (clusterCount - cluster));
            int x = random.Next(1, Width - 1);
            int y = random.Next(1, Height - 1);
            int placed = 0;

            for (int attempt = 0; attempt < target * 12 && placed < target; attempt++)
            {
                DecorBlock block = blocks[random.Next(blocks.Count)];
                if (IsAreaFree(occupied, x, y, block.CellsWide, block.CellsHigh) &&
                    StampBlock(tilemap, block, x, y, random, null))
                {
                    MarkArea(occupied, x, y, block.CellsWide, block.CellsHigh);
                    placed++;
                }

                // Random walk tạo mảng liền nhưng không thành hình vuông đều.
                x = Mathf.Clamp(x + random.Next(-1, 2), 1, Width - 2);
                y = Mathf.Clamp(y + random.Next(-1, 2), 1, Height - 2);
            }
            remaining -= placed;
        }
    }

    private static void PlaceScattered(Tilemap tilemap, DecorBlock block,
                                       bool[,] occupied, System.Random random)
    {
        for (int placed = 0; placed < block.HowMany; placed++)
        {
            for (int attempt = 0; attempt < 20; attempt++)
            {
                int x = random.Next(1, Width - block.CellsWide);
                int y = random.Next(1, Height - block.CellsHigh);
                if (!IsAreaFree(occupied, x, y, block.CellsWide, block.CellsHigh))
                    continue;
                if (StampBlock(tilemap, block, x, y, random, null))
                    MarkArea(occupied, x, y, block.CellsWide, block.CellsHigh);
                break;
            }
        }
    }

    /// <summary>Một loại vật thể đã cắt sẵn thành khối ô 32x32.</summary>
    private class DecorBlock
    {
        public string BiomePrefix;
        public string AssetName;
        public int SourceWidth;
        public int SourceHeight;
        public int VariantCount;
        public int CellsWide;
        public int CellsHigh;
        public int HowMany;
        public DecorPlacement Placement;
        public string ClusterGroup;
        public DecorTile[][][] Variants;
        public Texture2D Texture;
    }

    /// <summary>Variants[biến thể][cột][hàng] là các ô 32x32.</summary>
    private class DecorTile
    {
        public Tile Tile;
        public bool HasPixels;   // ô có nội dung nhìn thấy hay không
        public DecorTile(Tile tile, bool hasPixels)
        {
            Tile = tile; HasPixels = hasPixels;
        }
    }

    private static List<DecorBlock> LoadDecorBlocks(Biome biome)
    {
        List<DecorBlock> result = new List<DecorBlock>();

        foreach (Decor decor in biome.Decorations)
        {
            string path = TilesetRoot + "/" + biome.Name + "/" + decor.FileName;
            if (!File.Exists(path)) continue;

            byte[] bytes;
            try { bytes = File.ReadAllBytes(path); }
            catch { continue; }

            Texture2D texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
            if (!texture.LoadImage(bytes)) continue;

            DecorBlock block = new DecorBlock();
            block.BiomePrefix = biome.Name;
            block.AssetName = Path.GetFileNameWithoutExtension(path);
            block.Texture = texture;
            block.SourceWidth = decor.CellWidth;
            block.SourceHeight = decor.CellHeight;
            block.VariantCount = decor.Count;
            block.CellsWide = Mathf.Max(1, Mathf.CeilToInt(decor.CellWidth / 32f));
            block.CellsHigh = Mathf.Max(1, Mathf.CeilToInt(decor.CellHeight / 32f));
            block.HowMany = decor.HowMany;
            block.Placement = decor.Placement;
            block.ClusterGroup = decor.ClusterGroup;
            block.Variants = BuildVariants(block);
            UnityEngine.Object.DestroyImmediate(texture);
            block.Texture = null;
            if (block.Variants.Length > 0) result.Add(block);
        }

        return result;
    }

    private static DecorTile[][][] BuildVariants(DecorBlock block)
    {
        int availableVariants = block.Texture.width / Mathf.Max(1, block.SourceWidth);
        int maxVariants = Mathf.Min(block.VariantCount, availableVariants);
        List<DecorTile[][]> variants = new List<DecorTile[][]>();

        for (int v = 0; v < maxVariants; v++)
        {
            DecorTile[][] cells = new DecorTile[block.CellsWide][];
            for (int col = 0; col < block.CellsWide; col++)
                cells[col] = new DecorTile[block.CellsHigh];

            for (int col = 0; col < block.CellsWide; col++)
            {
                for (int row = 0; row < block.CellsHigh; row++)
                {
                    int sourceX = col * 32;
                    int sourceY = row * 32;
                    int copyWidth = Mathf.Min(32, block.SourceWidth - sourceX);
                    int copyHeight = Mathf.Min(32, block.SourceHeight - sourceY);
                    if (copyWidth <= 0 || copyHeight <= 0) continue;

                    int sx = v * block.SourceWidth + sourceX;
                    int sy = sourceY;
                    bool hasPixels = GetOrCreateDecorTile(
                        block, v, col, row, sx, sy, copyWidth, copyHeight) != null;
                    cells[col][row] = new DecorTile(
                        hasPixels ? LoadDecorTile(block, v, col, row) : null,
                        hasPixels);
                }
            }

            // Bỏ biến thể không có ô nào hợp lệ (ảnh nhỏ hơn khối, ô ngoài
            // ranh giới ảnh, hoặc import lỗi) - tránh đặt vật thể rỗng.
            int valid = 0;
            for (int col = 0; col < cells.Length; col++)
                for (int row = 0; row < cells[col].Length; row++)
                    if (cells[col][row] != null && cells[col][row].Tile != null &&
                        cells[col][row].Tile.sprite != null) valid++;
            if (valid > 0) variants.Add(cells);
        }

        return variants.ToArray();
    }

    /// <summary>
    /// Trả về Tile của ô 32x32, hoặc null nếu ô đó trong suốt hoàn toàn.
    /// Dùng chung cho cả lớp hình và lớp va chạm.
    /// </summary>
    private static Tile LoadDecorTile(DecorBlock block, int variant, int col, int row)
    {
        string id = block.AssetName + "_" + variant + "_" + col + "_" + row;
        return AssetDatabase.LoadAssetAtPath<Tile>(
            TileAssetRoot + "/" + block.BiomePrefix + "/" + id + ".asset");
    }

    private static Tile GetOrCreateDecorTile(DecorBlock block, int variant,
                                             int col, int row, int sx, int sy,
                                             int copyWidth, int copyHeight)
    {
        string id = block.AssetName + "_" + variant + "_" + col + "_" + row;
        string spriteFolder = DecorSpriteRoot + "/" + block.BiomePrefix;
        string spritePath = spriteFolder + "/" + id + ".png";
        string tileFolder = TileAssetRoot + "/" + block.BiomePrefix;
        string tilePath = tileFolder + "/" + id + ".asset";

        EnsureFolder(DecorSpriteRoot);
        EnsureFolder(spriteFolder);
        EnsureFolder(TileAssetRoot);
        EnsureFolder(tileFolder);        // Sprite tạo bằng Sprite.Create lúc runtime KHÔNG lưu được vào asset
        // (m_Sprite sẽ thành fileID 0), nên phải tách ô ra file PNG thật rồi
        // để Unity import. Cách này giữ được alpha trong suốt.
        {
            // GetPixels32(x, y, w, h) không có trong bản Unity này nên tự cắt.
            Color32[] all = block.Texture.GetPixels32();
            int texWidth = block.Texture.width;
            Color32[] pixels = new Color32[32 * 32];
            bool anyVisible = false;
            for (int py = 0; py < copyHeight; py++)
            {
                for (int px = 0; px < copyWidth; px++)
                {
                    if (sx + px >= block.Texture.width ||
                        sy + py >= block.Texture.height) continue;
                    Color32 c = all[(sy + py) * texWidth + sx + px];
                    pixels[py * 32 + px] = c;
                    if (c.a > 8) anyVisible = true;
                }
            }

            // Ô hoàn toàn trong suốt: KHÔNG đặt tile, để ô nền cỏ bên dưới
            // hiện ra. Nếu đặt tile trong suốt, ô đó sẽ che nền và lộ ra
            // màu nền mặc định của Camera (màu xanh) quanh núi.
            if (!anyVisible)
            {
                if (File.Exists(spritePath)) File.Delete(spritePath);
                if (AssetImporter.GetAtPath(spritePath) != null)
                    AssetDatabase.DeleteAsset(spritePath);
                return null;
            }

            Texture2D cell = new Texture2D(32, 32, TextureFormat.RGBA32, false);
            cell.SetPixels32(pixels);
            cell.Apply();
            File.WriteAllBytes(spritePath, cell.EncodeToPNG());
            UnityEngine.Object.DestroyImmediate(cell);
            AssetDatabase.ImportAsset(spritePath, ImportAssetOptions.ForceUpdate);
        }

        TextureImporter importer = AssetImporter.GetAtPath(spritePath) as TextureImporter;
        if (importer != null)
        {
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.spritePixelsPerUnit = 32f;
            importer.filterMode = FilterMode.Point;
            importer.mipmapEnabled = false;
            importer.alphaIsTransparency = true;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.SaveAndReimport();
        }

        Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);
        if (sprite == null)
        {
            Debug.LogError("[RobodenMapBuilder] Khong import duoc sprite: " + spritePath);
            return null;
        }

        Tile tile = AssetDatabase.LoadAssetAtPath<Tile>(tilePath);
        if (tile == null)
        {
            tile = ScriptableObject.CreateInstance<Tile>();
            AssetDatabase.CreateAsset(tile, tilePath);
        }
        tile.sprite = sprite;
        tile.colliderType = Tile.ColliderType.None;

        // Bat buoc SetDirty truoc: CreateAsset ghi ban mac dinh (m_Sprite = 0)
        // roi danh sach thay doi. SaveAssetIfDirty chi ghi khi object dirty,
        // nen chi goi SaveAssets() o cuoi se KHONG luu gi, va file tren dia
        // van giu m_Sprite = 0 => o trong suot che nen, lop mau nen Camera.
        EditorUtility.SetDirty(tile);
        AssetDatabase.SaveAssetIfDirty(tile);
        return tile;
    }

    private static bool StampBlock(Tilemap tilemap, DecorBlock block,
                                   int x, int y, System.Random random,
                                   Tilemap colliderMap)
    {
        if (block.Variants.Length == 0) return false;
        if (x < 0 || y < 0) return false;
        if (x + block.CellsWide > Width) return false;
        if (y + block.CellsHigh > Height) return false;

        DecorTile[][] cells = block.Variants[random.Next(block.Variants.Length)];

        bool blocks = colliderMap != null &&
                      block.Placement == DecorPlacement.Solid;

        for (int col = 0; col < block.CellsWide; col++)
        {
            for (int row = 0; row < block.CellsHigh; row++)
            {
                if (col >= cells.Length || row >= cells[col].Length) continue;
                DecorTile part = cells[col][row];
                if (part == null || part.Tile == null || part.Tile.sprite == null) continue;

                int cellY = y + row;
                Vector3Int cell = new Vector3Int(x + col, cellY, 0);
                tilemap.SetTile(cell, part.Tile);

                // Chi o co that su co hinh moi chiem va cham. O trong suot
                // o giua khoi 2x2 khong tao hinh, dat vao se tao o vuong
                // vo trong khong mong muon.
                if (blocks && part.HasPixels)
                {
                    part.Tile.colliderType = Tile.ColliderType.Grid;
                    EditorUtility.SetDirty(part.Tile);
                    colliderMap.SetTile(cell, part.Tile);
                }
            }
        }

        return true;
    }

    /// <summary>
    /// Ô trống không? <paramref name="ignore"/> là các ô do chính chòm núi
    /// vừa đặt chiếm - chúng được bỏ qua để các núi trong một chòm nằm sát nhau.
    /// </summary>
    private static bool IsAreaFree(bool[,] occupied, int x, int y, int wide, int high,
                                   List<Vector2Int> ignore)
    {
        for (int row = y; row < y + high; row++)
        {
            for (int col = x; col < x + wide; col++)
            {
                if (col < 0 || col >= Width || row < 0 || row >= Height) return false;
                if (ignore != null && ignore.Contains(new Vector2Int(col, row))) continue;
                if (occupied[row, col]) return false;
            }
        }
        return true;
    }

    private static bool IsAreaFree(bool[,] occupied, int x, int y, int wide, int high)
    {
        return IsAreaFree(occupied, x, y, wide, high, null);
    }

    private static void MarkArea(bool[,] occupied, int x, int y, int wide, int high)
    {
        for (int row = y; row < y + high; row++)
        {
            for (int col = x; col < x + wide; col++)
            {
                if (col >= 0 && col < Width && row >= 0 && row < Height)
                    occupied[row, col] = true;
            }
        }
    }

    private static List<float> LoadWeights(string jsonPath, int spriteCount)
    {
        if (!File.Exists(jsonPath)) return null;

        string json = File.ReadAllText(jsonPath);
        List<float> weights = new List<float>();
        int searchFrom = 0;

        // Mỗi phần tử trong mảng "tiles" có {"id": N, "probability": X}.
        // Đọc bằng cách tìm chuỗi để không phải viết lớp Serialization cho file dữ liệu.
        while (weights.Count < spriteCount)
        {
            int start = json.IndexOf("\"id\"", searchFrom, System.StringComparison.Ordinal);
            if (start < 0) break;

            weights.Add(ReadProbability(json, start));
            searchFrom = start + 4;
        }

        if (weights.Count == 0) return null;
        while (weights.Count < spriteCount) weights.Add(1f);

        bool allZero = true;
        foreach (float w in weights)
        {
            if (w > 0f) { allZero = false; break; }
        }
        if (allZero)
        {
            for (int i = 0; i < weights.Count; i++) weights[i] = 1f;
        }

        return weights;
    }

    private static float ReadProbability(string json, int fromIndex)
    {
        int prob = json.IndexOf("\"probability\"", fromIndex, System.StringComparison.Ordinal);
        if (prob < 0) return 1f;

        int open = json.IndexOf(':', prob);
        if (open < 0) return 1f;

        int cursor = open + 1;
        while (cursor < json.Length && (json[cursor] == ' ' || json[cursor] == '\t')) cursor++;

        int numberEnd = cursor;
        while (numberEnd < json.Length &&
               (char.IsDigit(json[numberEnd]) || json[numberEnd] == '.' ||
                json[numberEnd] == '-' || json[numberEnd] == 'e')) numberEnd++;

        if (numberEnd <= cursor) return 1f;

        float parsed;
        bool ok = float.TryParse(json.Substring(cursor, numberEnd - cursor),
                                 System.Globalization.NumberStyles.Float,
                                 System.Globalization.CultureInfo.InvariantCulture,
                                 out parsed);
        return ok ? parsed : 1f;
    }

    private static Tile GetOrCreateTile(Sprite sprite, Tilemap tilemap)
    {
        // Tên asset có tiền tố tên biome, vì cả 4 tileset đều có ô trùng tên
        // (tiles_0 tồn tại ở cả Forest, Moon, Inferno, Tundra).
        string biomePrefix = tilemap.name.Replace("Tilemap_", "");
        string assetPath = TileAssetRoot + "/" + biomePrefix + "/" + biomePrefix + "_" +
                           sprite.name + ".asset";

        Tile existing = AssetDatabase.LoadAssetAtPath<Tile>(assetPath);
        if (existing != null) return existing;

        EnsureFolder(TileAssetRoot);
        EnsureFolder(TileAssetRoot + "/" + biomePrefix);

        Tile tile = ScriptableObject.CreateInstance<Tile>();
        tile.sprite = sprite;
        tile.colliderType = Tile.ColliderType.None;
        AssetDatabase.CreateAsset(tile, assetPath);
        return tile;
    }

    private static void EnsureFolder(string path)
    {
        if (AssetDatabase.IsValidFolder(path)) return;

        string parent = path.Substring(0, path.LastIndexOf('/'));
        string name = path.Substring(path.LastIndexOf('/') + 1);
        AssetDatabase.CreateFolder(parent, name);
    }
}

# Kế hoạch quái (bot) — 4 map

Nhánh `feature/bot-play`. Đọc kèm: [game_bot.txt](game_bot.txt), [CHARACTER_PREFAB_PLAN](CHARACTER_PREFAB_PLAN.md).
`[?]` = chưa có thông tin, đang chặn.

## 1. Mục tiêu

Bốn map, mỗi map một scene. Quái đổi màu theo màu map. Mỗi map đủ 4 tầng quái, ra **theo thứ tự thời gian**, và mỗi con **tự mạnh lên khi còn sống**.

## 2. Quy tắc màu

- Map màu nào thì lính thường, cung thủ, tinh nhuệ màu đó.
- Map 1 (nâu) dùng tinh nhuệ **xanh** — kho không có file nâu.
- Map 4: cung thủ dùng chung ảnh với lính thường trắng (`bow_enemy/snow.png` là bản sao của `normal_enemy/snow.png`). Phân biệt bằng hành vi.

## 3. Bốn tầng quái

| Tầng | Quái | Art | Đánh bằng | Số lượng cùng lúc |
|---|---|---|---|---|
| 1 | Lính thường — goblin cầm dao đá | `normal_enemy` | **Chém** (có lấy đà) | 10–20 |
| 2 | Cung thủ | `bow_enemy` | Bắn xa | 3–5 |
| 3 | Tinh nhuệ | `elite_boss` | Chém (chung động tác lính thường) | **3** |
| 4 | Trùm map | Skeleton / LavaGolem / Yeti | 3 chiêu riêng | 1 |

## 4. Phân bổ theo map

| Map | Scene | Lính thường | Cung thủ | Tinh nhuệ | Trùm |
|---|---|---|---|---|---|
| 1 — Nâu | `Map1_Forest` | `normal_enemy/brown` | `bow_enemy/brown` | `elite_boss/green` ×2 | **không có** |
| 2 — Xanh | `Map2_Moon` | `normal_enemy/green` | `bow_enemy/green` | `elite_boss/green` ×3 | Xương khô cầm kiếm |
| 3 — Đỏ | `Map3_Inferno` | `normal_enemy/red` | `bow_enemy/red` | `elite_boss/red` ×3 | Golem dung nham |
| 4 — Trắng | `Map4_Tundra` | `normal_enemy/snow` | `bow_enemy/snow` | `elite_boss/snow` ×3 | Yeti |

## 5. Thứ tự xuất hiện

Đợt **không tính theo giây**. Đợt kết thúc khi **giết hết quái của đợt đó**; Player chết thì thua luôn.

| Đợt | Quái của đợt (tổng số phải giết) | Mở khoá |
|---|---|---|
| 1 | lính thường ×12 | chỉ lính thường |
| 2 | lính thường ×12 + cung thủ ×4 | + cung thủ |
| 3 | lính thường ×12 + cung thủ ×4 + tinh nhuệ ×2 | + tinh nhuệ |
| 4 | trùm (map 2/3/4) + lính thường ×6 | + trùm |

- Đợt sau giữ loại của đợt trước, chỉ thêm loại mới. Map 1 dừng ở đợt 3.
- Số phải giết nhân theo map: map 2 ×1.3, map 3 ×1.6, map 4 ×2 (làm tròn).
- Cap sống cùng lúc: lính 10, cung thủ 3, tinh nhuệ 3 (map 1: 2). Sinh dần theo nhịp, chết bao nhiêu thì sinh bù bấy nhiêu cho tới hết tổng số.
- Trùm ra sau khi giết hết đợt 3 — tức cuối map.

Đợt 1–3 dùng đúng cơ chế wave của `WaveManager`, chỉ đổi điều kiện kết thúc từ "hết giờ" sang "giết hết quái". HUD bỏ đồng hồ, thay bằng số quái còn lại.

## 6. Quái tự mạnh lên khi còn sống

Không phụ thuộc lúc ra trận: con sống càng lâu càng mạnh.

| Chỉ số | Tăng | Dừng ở | Tối đa |
|---|---|---|---|
| Máu tối đa | +2%/giây | 60 giây | 220% |
| Sát thương chém | +1%/giây | 60 giây | 160% |
| Tốc độ chạy | — | — | 100% |

`chỉ số = gốc × (1 + min(giây_sống, 60) × %mỗi_giây)`. Máu tối đa tăng thì cộng phần chênh vào máu hiện tại, không hồi đầy.

## 7. Lính thường và tinh nhuệ — đánh gần

**Chém mới mất máu.**

| Bước | Thời gian | Việc xảy ra |
|---|---|---|
| 1 | — | Đuổi Player |
| 2 | — | Tới tầm đánh ~1,2 unit thì dừng |
| 3 | 0,35 giây | Giơ dao lấy đà (frame 1–2 của animation chặt) |
| 4 | 1 khung hình | Trừ máu nếu Player còn trong tầm (frame 3) |
| 5 | 1,5 giây | Hồi chiêu |

Animation 16 ô của lính thường là **4 động tác × 4 frame**, 1 hướng nhìn:

| Hàng | Động tác |
|---|---|
| 1 | Đứng (lặp) |
| 2 | Đi (lặp) |
| 3 | Chặt: giơ dao → chém → thu tay |
| 4 | Trúng đòn (frame 1) + chết (frame 2–4) |

Chỉ 1 hướng → đi sang trái thì lật `flipX`. Đừng dùng lại cách của `PlayerSpriteAnimator`.

Tinh nhuệ: máu ×3, sát thương ×1,5, tốc độ 80%, tầm đánh 1,4 unit.

## 8. Cung thủ

- Giữ khoảng cách **5–6 unit** (trong tầm này mới bắn được), lùi khi Player < 3,5 unit, không lao vào.
- Bắn **1 giây/phát**, đạn bay 8 unit/giây, sát thương 8.
- Máu 80% lính thường cùng đợt.
- Bắn ra **mũi tên bay trên màn hình** — thấy được, né được, trúng mới mất máu.

## 9. Trùm map — 4 mạng

| Map | Trùm | Tổng máu | Mỗi mạng |
|---|---|---|---|
| 2 | Xương khô cầm kiếm | 1200 | 300 |
| 3 | Golem dung nham | 1800 | 450 |
| 4 | Yeti | 2400 | 600 |

Mất 1 mạng thì **hồi đầy máu** cho mạng sau.

**Chọn chiêu:** mỗi trùm có 1 **chiêu mặc định** dùng bất cứ lúc nào. Các chiêu còn lại chỉ dùng khi đã hồi chiêu xong; nếu nhiều chiêu cùng sẵn sàng thì **chọn ngẫu nhiên**.

### Xương khô (map 2)

| Chiêu | Mô tả | Animation | VFX | Hồi chiêu |
|---|---|---|---|---|
| 1 | Lướt tới áp sát | `Skill1_Dash` | `DashTrail` | 8s |
| 2 | Xoay kiếm vòng tròn | `Skill2_SpinSlash` | `SpinSlash` | 10s |
| 3 | Nhảy tới rồi gầm | `Skill3_Jump` + `Skill3_Roar` | `RoarShockwave` | 14s |

### Golem (map 3)

| Chiêu | Mô tả | Animation | VFX | Hồi chiêu |
|---|---|---|---|---|
| 1 | Phun lửa hình nón | `Skill1_ConeFire` | `ConeFire` | 9s |
| 2 | Hồi máu (khi máu < 60%) | `Skill2_Heal` | `HealAura` | 15s |
| 3 | Ném cầu lửa → tách cầu nhỏ + hố dung nham | `Skill3_Throw` | `Fireball`, `LavaPool` | 12s |

Cầu lửa nhỏ dùng lại `Fireball.png` thu nhỏ.

### Yeti (map 4)

| Chiêu | Mô tả | Animation | VFX | Hồi chiêu |
|---|---|---|---|---|
| 1 | Ném 1 viên tuyết | `Skill1_Throw` | `Snowball` | 6s |
| 2 | Vòng tuyết quanh Player, không né trong 0,5–1s thì đông cứng | `Skill2_FreezeCast` | `FreezeWarning`, `IcePrison` | 12s |
| 3 | Gồng → tăng công/thủ → bão tuyết làm chậm Player | `Skill3_BlizzardCast` | `Snowflake` | 20s |

Animation và VFX của cả 3 trùm đã có sẵn, đã cắt 256×256.
Hồi chiêu dùng số ở cột trên. Yeti chiêu 3 `[?]` chọn tăng công hay tăng thủ khi nào.

## 10. Hiệu ứng lên Player

| Hiệu ứng | Nguồn | Số liệu |
|---|---|---|
| Đông cứng | Yeti chiêu 2 | đứng yên 1,5s, không bắn |
| Làm chậm | Yeti chiêu 3 | −40% tốc chạy, 3s |
| Hố dung nham | Golem chiêu 3 | 5 sát thương mỗi 0,5s, tồn tại 4s |

## 11. Art

Đã có:

| Art | File | Kích thước | Bố cục | Tình trạng |
|---|---|---|---|---|
| Lính thường nâu/xanh/đỏ | `public/normal_enemy/*.png` | 1448×1086 | 4 động tác × 4 frame | RGB, **không alpha** |
| Lính thường trắng | `public/normal_enemy/snow.png` | 2046×768 | 8×4 = 32 pose, khác 3 màu kia | RGB, không alpha |
| Cung thủ nâu/xanh/đỏ | `public/bow_enemy/*.png` | 1448×1086 | 4×4 | RGB, không alpha |
| Cung thủ trắng | `public/bow_enemy/snow.png` | 2046×768 | trùng file lính trắng | RGB, không alpha |
| Tinh nhuệ xanh/đỏ/trắng | `public/elite_boss/*.png` | 1254×1254 | 4×4, pose dính nhau | RGB, không alpha |
| Animation 3 trùm | `Assets/Prefabs/Enemy/*/Animations/` | 1024×2048, 1024×1792 | 8/7 hàng × 4 frame | đã cắt 256×256 |
| VFX 3 trùm | `Assets/Prefabs/Enemy/*/SkillAssets/` | PNG rời | — | dùng được |

Phải làm:

1. Tách nền caro cho 11 file quái thường/cung thủ/tinh nhuệ.
2. Cắt theo connected-component, không cắt lưới đều — tinh nhuệ có tay với sang ô bên.
3. File trắng (8×4) xử lý riêng tham số.
4. **PPU sao cho quái cao bằng Player** (Player cao 1,34 unit) → lính thường/cung thủ ~PPU 110, tinh nhuệ phóng to ×2.
5. Cung thủ và tinh nhuệ giả định cùng bố cục 4 động tác như lính thường; GĐ 1 cắt xong nhìn là biết, sai thì sửa tham số.

Con nhện cũ (`spider.png`) bỏ, chỉ giữ trong scene tạm tới khi GĐ 2 xong.

## 12. Code

Script mới:

| File | Việc |
|---|---|
| `Enemy/EnemyGrowth.cs` | Mạnh lên theo thời gian sống; nâng máu + sát thương đòn chém |
| `Enemy/EnemyMeleeAttack.cs` | Dừng ở tầm, lấy đà 0,35s, trúng thì trừ máu, hồi chiêu 1,5s |
| `Enemy/EnemyRangedMovement.cs` | Giữ khoảng cách |
| `Enemy/EnemyProjectile.cs` | Đạn của quái |
| `Enemy/BossLives.cs` | 4 mạng, hết máu → trừ mạng → hồi máu |
| `Enemy/SkeletonBoss.cs` / `LavaGolemBoss.cs` / `YetiBoss.cs` | Chiêu từng trùm |
| `Enemy/HazardZone.cs` | Vùng sát thương dưới đất |
| `Player/PlayerStatusEffects.cs` | Đông cứng, làm chậm |
| `Editor/EnemySpritePreparation.cs` | Tách nền + cắt frame + clip + controller |
| `Editor/BiomeSceneSplitter.cs` | Tách 4 biome ra 4 scene |

Script sửa:

| File | Sửa |
|---|---|
| `Enemy/EnemyHealth.cs` | Thêm `GrowHealth`, event khi chết |
| `Enemy/EnemySpawner.cs` | Nhiều loại quái, trọng số, cap riêng, sinh trùm |
| `Wave/WaveManager.cs` | Đợt kết thúc khi **giết hết quái** thay vì hết giờ; `WaveSettings` thêm danh sách loại quái, tổng số phải giết, prefab trùm |
| `Core/GameManager.cs` | HUD bỏ đồng hồ, hiện số quái còn lại |
| `Player/PlayerController.cs` | Hệ số tốc độ, khoá di chuyển khi đông cứng |
| `Editor/CombatRegression.cs` | Test mới |

Dữ liệu mỗi đợt:

```
WaveSettings   (= một đợt)
  maxEnemiesAlive, spawnInterval, enemyHealth, enemySpeed
  enemies[]      → prefab, weight, maxAlive, healthMultiplier, totalToKill
  bossPrefab, bossLives, bossHealthPerLife   (chỉ điền ở đợt có trùm)
```

Mỗi đợt khai báo đủ danh sách loại được ra (đợt 2 khai lính + cung thủ…). Không dùng ScriptableObject.

## 13. Giai đoạn

| GĐ | Việc | Xong khi | Ước lượng |
|---|---|---|---|
| 1 | `EnemySpritePreparation` trên `normal_enemy/brown.png` | 16 PNG sạch nền, 4 clip đúng | 1 buổi |
| 2 | Prefab goblin: Animator + `EnemyGrowth` + `EnemyMeleeAttack` | idle/đi/chặt chạy đúng, né được, máu tăng theo thời gian | 1,5 buổi |
| 3 | Spawner nhiều loại + 4 màu + thứ tự đợt | đợt 1 chỉ lính, đợt 2 mới có cung thủ, đợt 3 mới có tinh nhuệ; regression PASS | 1 buổi |
| 4 | Cung thủ | giữ khoảng cách, đạn bay thấy được | 1 buổi |
| 5 | Tinh nhuệ | đạt 3 con, không lấn cap lính thường | 0,5 buổi |
| 6 | Khung boss + Xương khô | đủ 4 mạng, 3 chiêu chạy | 1,5 buổi |
| 7 | Golem + Yeti + hiệu ứng Player | 3 chiêu mỗi con, đông cứng/làm chậm/hố dung nham hoạt động | 2 buổi |
| 8 | Tách 4 scene + cân bằng | chơi liền map 1 → 4 | 1,5 buổi |

Tách scene: mỗi biome gồm `Tilemap_<Biome>_{Ground,Decor,Block}`, trang trí là con của Decor, dời `localPosition` về `(-15, -9)`.

## 14. Kiểm thử

`CombatRegression` hiện kiểm tra "wave 1 mọi con đều 10 HP" — sẽ sai sau khi thêm `EnemyGrowth`. Sửa và thêm:

1. Quái tăng đúng +2%/giây máu, dừng sau 60s.
2. Máu tối đa tăng thì máu hiện tại tăng theo phần chênh, không hồi đầy.
3. Spawner tôn trọng cap riêng từng loại và dừng sinh khi đủ tổng số.
4. **Đợt kết thúc khi giết hết quái**, không phải khi hết giờ.
5. Đạn cung thủ gây sát thương một lần.
6. Đòn chém: lấy đà rồi mới trừ máu; né ra thì không mất máu.
7. Trùm trừ đúng 1 mạng, hồi đầy máu, chết sau mạng thứ 4.
8. Đông cứng 1,5s; làm chậm −40% trong 3s.

Test tay: chơi trọn 1 đợt; đứng yên 30s xem quái mạnh lên; đánh trùm đủ 4 mạng.

## 15. Rủi ro

| Rủi ro | Cách né |
|---|---|
| Trùm 4 mạng × máu đầy quá dài | Chia đều máu mỗi mạng (mục 9) |
| File trắng 8×4 khác 3 màu kia | Xử lý riêng, không dùng chung tham số |
| Tinh nhuệ pose dính nhau | Cắt connected-component, xem từng frame |
| `GameScene` 2,1 MB tách tay dễ sai | Viết tool tách |
| Sửa `EnemySpawner` phá hệ wave đã test | Chạy lại `CombatRegression` |
| `Debug.Log` mỗi lần trừ máu spam Console | Gỡ khỏi `EnemyHealth.TakeDamage` |

## 16. Ngoài phạm vi

Không sửa hệ súng / `GunSkills` / `GunSkillTest`. Không đổi art Player, map, súng. Không làm HUD, menu, pause, item.

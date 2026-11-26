# Kế hoạch quái (bot) — 4 map

Nhánh `feature/bot-play`. Đọc kèm: [game_bot.txt](game_bot.txt), [CHARACTER_PREFAB_PLAN](CHARACTER_PREFAB_PLAN.md).
`[?]` = chưa có thông tin, đang chặn.

## 1. Mục tiêu

Bốn map nằm trong **một scene** (`GameScene.unity` đã dựng sẵn 4 biome cạnh nhau), chuyển map bằng cách dời Player sang vùng kế. Quái đổi màu theo màu map. Mỗi map đủ 4 tầng quái, ra **theo thứ tự**, và mỗi con **tự mạnh lên khi còn sống**.

**Làm dọc từng loại quái:** xong 1 loại là chia vào 4 map và chơi thử được ngay, rồi mới làm loại kế tiếp.

| Vòng | Làm gì | Xong thì chơi được |
|---|---|---|
| 1 | Lính thường 4 màu | 4 map, mỗi map 1 màu goblin |
| 2 | Cung thủ 4 màu + AI bắn | 4 map có goblin + cung thủ |
| 3 | Tinh nhuệ 3 màu | 4 map có thêm tinh nhuệ |
| 4 | 3 trùm | map 2/3/4 có trùm riêng |

## 2. Quy tắc màu

- Map màu nào thì lính thường, cung thủ, tinh nhuệ màu đó.
- Map 1 (nâu) dùng tinh nhuệ **xanh** — kho không có file nâu.
- Map 4: cung thủ trắng **đã có sẵn** trong `snow.png` (nửa phải của bảng 8×4), không cần vẽ thêm.

## 3. Bốn tầng quái

| Tầng | Quái | Art | Đánh bằng | Số lượng cùng lúc |
|---|---|---|---|---|
| 1 | Lính thường — goblin cầm dao đá | `normal_enemy` | **Chém** (có lấy đà) | 10–20 |
| 2 | Cung thủ | `bow_enemy` | Bắn xa | 3–5 |
| 3 | Tinh nhuệ | `elite_boss` | Chém (chung động tác lính thường) | **3** |
| 4 | Trùm map | Skeleton / LavaGolem / Yeti | 3 chiêu riêng | 1 |

## 4. Phân bổ theo map

| Map | Vùng trong scene | Tâm X | Lính thường | Cung thủ | Tinh nhuệ | Trùm |
|---|---|---|---|---|---|---|
| 1 — Nâu | Forest | 0 | `brown` | `brown` | `green` ×2 | **không có** |
| 2 — Xanh | Moon | 35 | `green` | `green` | `green` ×3 | Xương khô |
| 3 — Đỏ | Inferno | 70 | `red` | `red` | `red` ×3 | Golem dung nham |
| 4 — Trắng | Tundra | 105 | `snow` | `snow` | `snow` ×3 | Yeti |

### Cách chuyển map: 1 scene, 4 vùng

Scene hiện tại **đã có sẵn cả 4 biome** nằm cạnh nhau trên trục X, cách nhau 35 unit:

| Biome | Object | localPosition X | Tâm vùng |
|---|---|---|---|
| Forest | `Tilemap_Forest_{Ground,Decor,Block}` | −15,0 | **0** |
| Moon | `Tilemap_Moon_*` | 20 | **35** |
| Inferno | `Tilemap_Inferno_*` | 55 | **70** |
| Tundra | `Tilemap_Tundra_*` | 90 | **105** |

Mỗi map rộng 30 ô (tâm ±15). Chuyển map = **dời Player và camera sang tâm vùng kế**, cộng 35 vào X:

```
Player.position.x += 35
camera tự bám theo CameraFollow
```

Không cần scene riêng, không nhân bản Player/Camera/Manager. Đổi lại: cả 4 map nằm chung file `GameScene.unity` (2 MB).

Khi sang map mới: xoá hết quái cũ, đổi bảng màu quái sang màu của map đó, reset Player về tâm vùng mới, hồi đầy máu.

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

**Đã dời vào `Assets/Art/Enemy/{Normal,Bow,Elite}/` và xoá `Assets/public/{normal,bow,elite_boss}/`.** Ảnh gốc sao lưu ở `Temp/enemy_backup/`.

| Art | File | Tình trạng |
|---|---|---|
| Lính thường nâu/xanh/đỏ | `Art/Enemy/Normal/{Brown,Green,Red}.png` | ✅ đã cắt 16 frame 256×256, PPU 110 |
| Lính thường trắng | `Art/Enemy/Normal/Snow.png` | ✅ đã cắt 16 frame, PPU 110, clip + prefab đã nối |
| Cung thủ nâu | `Art/Enemy/Bow/Brown.png` | ✅ đã cắt 16 frame, PPU 110 |
| Cung thủ xanh/đỏ | `Art/Enemy/Bow/{Green,Red}.png` | ⬜ mới import, **chưa cắt** (PPU 100, 1 sprite) |
| Cung thủ trắng | `Art/Enemy/Bow/Snow.png` | ✅ đã cắt 16 frame, PPU 110, clip + prefab đã nối (chưa có AI bắn) |
| Tinh nhuệ xanh/đỏ/trắng | `Art/Enemy/Elite/{Green,Red,Snow}.png` | ⬜ mới import, **chưa cắt** (PPU 100, 1 sprite) |
| Animation 3 trùm | `Prefabs/Enemy/Boss/*/Animations/` | ✅ đã cắt 256×256 |
| VFX 3 trùm | `Prefabs/Enemy/Boss/*/SkillAssets/` | ✅ dùng được |

Còn phải làm:

1. Cắt frame cho `Bow/{Green,Red}` và `Elite/{Green,Red,Snow}` — dùng lại quy trình connected-component đã dùng cho các file khác.
2. Tinh nhuệ phóng to ×2 so với lính thường; xem từng frame vì pose dính nhau sang ô bên.
3. Tạo clip + controller + prefab cho cung thủ 3 màu và tinh nhuệ 3 màu, đặt dưới `Prefabs/Enemy/<Loại>/<Màu>/`.

**Bài học từ snow:** khi tách ảnh gộp, phải sửa cả `internalID` và `nameFileIdTable` trong `.meta`; nếu chỉ sửa rect thì clip sẽ trỏ sai frame. PPU phải đồng nhất 110 cho mọi màu, nếu không quái lệch chiều cao.

Con nhện cũ (`spider.png`) đã xoá.

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
| `Core/MapManager.cs` | Quản lý 4 map trong 1 scene: đổi map, dời Player +35 X, xoá quái cũ, đổi bảng màu quái |

Script sửa:

| File | Sửa |
|---|---|
| `Enemy/EnemyHealth.cs` | Thêm `GrowHealth`, event khi chết |
| `Enemy/EnemySpawner.cs` | Nhiều loại quái, trọng số, cap riêng, sinh trùm |
| `Wave/WaveManager.cs` | Đợt kết thúc khi **giết hết quái** thay vì hết giờ; `WaveSettings` thêm danh sách loại quái, tổng số phải giết, prefab trùm |
| `Core/GameManager.cs` | HUD bỏ đồng hồ, hiện số quái còn lại |
| `Player/PlayerController.cs` | Hệ số tốc độ, khoá di chuyển khi đông cứng |
| `Editor/CombatRegression.cs` | Test mới |

Dữ liệu mỗi map:

```
WaveSettings   (= một đợt trong một map)
  maxEnemiesAlive, spawnInterval, enemyHealth, enemySpeed
  enemies[]      → prefab, weight, maxAlive, healthMultiplier, totalToKill
  bossPrefab, bossLives, bossHealthPerLife   (chỉ điền ở đợt có trùm)

MapSettings    (= một map)
  mapIndex, biomeName, centerX (0 / 35 / 70 / 105)
  waves[]        → WaveSettings
```

Mỗi đợt khai báo đủ danh sách loại được ra (đợt 2 khai lính + cung thủ…). Không dùng ScriptableObject.

## 13. Giai đoạn — làm dọc theo từng loại quái

| GĐ | Việc | Xong khi | Ước lượng |
|---|---|---|---|
| ~~1~~ | ~~Cắt art + prefab goblin nâu~~ | ✅ xong | — |
| ~~2~~ | ~~4 màu goblin + spawner nhiều loại~~ | ✅ xong | — |
| **3** | **Phân map cho lính thường**: `MapSettings` + dời Player 35 đơn vị khi xong map; 4 map chỉ ra goblin màu của map | đánh hết map 1 tự sang map 2, mỗi map đúng 1 màu | 1 buổi |
| ~~4~~ | ~~Cung thủ: `EnemyRangedMovement` + `EnemyRangedAttack` + `EnemyProjectile` + 4 màu~~ | ✅ xong 2026-11-26 | — |
| ~~5~~ | ~~Phân map cho cung thủ~~ | ✅ xong 2026-11-26 — 4 map đều có lính + cung thủ cùng màu | — |
| 6 | Tinh nhuệ 3 màu + phân map | 4 map có thêm tinh nhuệ | 1 buổi |
| 7 | Khung boss 4 mạng + Xương khô | đủ 4 mạng, 3 chiêu | 1,5 buổi |
| 8 | Golem + Yeti + hiệu ứng Player | đông cứng/làm chậm/hố dung nham chạy | 2 buổi |
| 9 | Phân map cho trùm + cân bằng | map 2/3/4 có trùm riêng, chơi liền 4 map | 1 buổi |

Chuyển map: cộng **35** vào X của Player (`0 → 35 → 70 → 105`), xoá quái cũ, đổi bảng màu quái, hồi đầy máu.

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

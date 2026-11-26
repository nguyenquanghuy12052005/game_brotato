# Current project state

## GĐ 5 — Sửa luật đợt: giết hết quái, không tính giờ (2026-11-26)

Nhánh `feature/bot-play`, chưa commit. **User chỉ ra mình làm sai luật** ở [KE_HOACH §5](KE_HOACH_BOT_4_MAP.md#L65): đợt *không tính theo giây*, đợt 1 chỉ lính thường, đợt 2 mới thêm cung thủ, và **phải giết hết quái mới được sang map khác**. Trước đó mình cho cung thủ ra ngay từ đầu và chuyển map theo đồng hồ 30 giây.

**Cấu trúc lại dữ liệu:** trước đây `WaveSettings[]` nằm phẳng trên `WaveManager`, và `MapSettings` lại chứa `EnemySpawnEntry[]`. Hai thứ mâu thuẫn vì đợt phải thuộc về từng map. Nay đúng theo [KE_HOACH §12](KE_HOACH_BOT_4_MAP.md#L228):

```
MapManager.maps[]              (4 map)
  └─ MapSettings
       ├─ biomeName, centerX
       └─ waves[]              (2 đợt của map này)
            └─ WaveSettings
                 ├─ maxEnemiesAlive, enemyHealth, spawnInterval, enemySpeed
                 └─ enemies[]  (WaveEnemyEntry)
                      ├─ prefab
                      ├─ totalToKill   tổng số phải sinh & giết hết
                      ├─ maxAlive      số sống cùng lúc của loại này
                      └─ weight
```

`WaveManager.waves` giờ để **rỗng** (`[]`); nó tự nạp từ `MapManager.CurrentSettings.waves` mỗi khi vào map (`LoadWavesForCurrentMap`).

**Code mới/sửa:**

| File | Việc |
|---|---|
| `Wave/WaveSettings.cs` (mới) | Tách khỏi `WaveManager`. Bỏ `duration`; thêm `WaveEnemyEntry` và `TotalToKill`. |
| `Wave/WaveManager.cs` | Bỏ hẳn đồng hồ (`RemainingTime`, `duration`). `Update` kiểm `enemySpawner.AllSpawned && AliveCount == 0` → hết đợt. Thêm `RemainingToKill` để HUD hiện số quái còn lại. |
| `Enemy/EnemySpawner.cs` | Thêm `spawnQuota[]` / `alivePerType[]`; chọn loại theo trọng số nhưng tôn trọng `totalToKill` **và** `maxAlive` riêng từng loại. Thêm `RemainingSpawns`, `AllSpawned`, `NotifyEnemyRemoved`. |
| `Enemy/SpawnedEnemyType.cs` (mới) | Nhãn gắn lúc sinh, để `EnemyHealth.Die` trả lại suất sống cho đúng loại. |
| `Enemy/EnemyHealth.cs` | Khi chết: gọi `EnemySpawner.NotifyEnemyRemoved(TypeIndex)`. |
| `Core/MapManager.cs` | `MapSettings.enemies` → `waves[]`. `ApplyCurrentMap` chỉ set biên map. Thêm `GetMapSettings(int)`. |
| `Core/GameManager.cs` | HUD bỏ đồng hồ, hiện `Còn lại: N quái`; chữ Việt hoá. |
| `Editor/MapSetupTool.cs` | Nạp 2 đợt cho mỗi map, mỗi map 2 prefab (lính + cung thủ cùng màu). |
| `Editor/WaveSetupTool.cs` | Không ghi đè nữa; chỉ kiểm tra và hướng dẫn sang `MapSetupTool`. |
| `Editor/CombatSetup.cs` | Bỏ phần ghi `duration` đã xoá. |
| `Editor/CombatRegression.cs` | Viết lại vòng test theo luật mới (sinh đủ chỉ tiêu → giết hết → hết đợt), bỏ mọi phụ thuộc `RemainingTime`/`duration`. |

**Dữ liệu đã nạp cho cả 4 map** (mỗi map 2 đợt, cấp màu theo biome):

| Đợt | Loại quái | Tổng phải giết | Cap sống |
|---|---|---|---|
| 1 | lính thường | 12 | 10 |
| 2 | lính thường + cung thủ | 12 + 4 = 16 | 10 + 3 = 13 |

**Kiểm tra đã chạy thật:** Roslyn compile `Assets/Scripts` → **exit 0**; `Assets/Editor` → **exit 0**. Đọc lại scene kiểm cấu trúc: `WaveManager.waves` rỗng, 4 map × 2 đợt, đợt 1 đúng 1 loại (12 lính), đợt 2 đúng 2 loại (12 lính + 4 cung thủ) → **PASS**.

**Chưa kiểm được:** Play Mode. Unity batchmode vẫn bị chặn bởi licensing (`attempt to write a readonly database`), nên chưa chạy được `CombatRegression` mới.

## GĐ 4 — Cung thủ bắn được (2026-11-23)

Nhánh `feature/bot-play`, chưa commit. GĐ 3 (phân map cho lính thường) đã có code từ commit `37f1df1`; GĐ 4 thêm AI bắn.

**Script mới** (đều ở `Assets/Scripts/Enemy/`):

| File | Việc |
|---|---|
| `EnemyRangedMovement.cs` | Giữ khoảng cách 5–6 unit: xa hơn 6 thì tiến, gần hơn 3,5 thì lùi, trong khoảng giữa đứng bắn. Tự `enabled = false` con `EnemyController` vì prefab dùng chung với lính thường. |
| `EnemyRangedAttack.cs` | Giơ cung 0,3s rồi mới bắn, hồi chiêu 1s, tầm bắn 6 unit. Chỉ bắn khi Player còn trong tầm lúc cung buông. |
| `EnemyProjectile.cs` | Mũi tên bay 8 u/s, sát thương 8, tự huỷ sau 4s. Chỉ trừ máu `PlayerHealth`; chạm thứ khác thì bay tiếp. Nhận hướng qua `SetDirection`, không dùng `transform.right`. |

**Art mới:** `Assets/Art/Enemy/Bow/Arrow.png` (91×31, PPU 110) — cắt nguyên mũi tên từ frame Attack cuối của `Bow/Green.png`, đúng art style, không phải vẽ lại.

**Prefab mới:** `Assets/Prefabs/Enemy/Bow/EnemyArrow.prefab` — SpriteRenderer + Rigidbody2D (gravity 0) + CircleCollider2D trigger r=0,15 + `EnemyProjectile`.

**Prefab sửa:** `Prefabs/Enemy/Bow/Snow/Enemy_Bow_Snow.prefab` — bỏ `EnemyMeleeAttack`, thêm `EnemyRangedMovement` + `EnemyRangedAttack` (đã nối sẵn prefab đạn).

**Code sửa:**

- `EnemyHealth.cs`: thêm tham chiếu `EnemyRangedMovement`/`EnemyRangedAttack` để tắt cả hai khi chết.
- `EnemySpawner.cs`: `ConfigureEnemy` gán target + tốc độ cho cung thủ, và đặt máu **80%** lính thường cùng đợt.

**Đã thả vào game (2026-11-26):** cả **4 map** giờ có lính thường + cung thủ **cùng màu**, mỗi map 2 loại (lính `weight: 3`, cung thủ `weight: 1`):

| Map | Lính thường | Cung thủ |
|---|---|---|
| Forest (X=0) | `Enemy_Normal_Brown` | `Enemy_Bow_Brown` |
| Moon (X=35) | `Enemy_Normal_Green` | `Enemy_Bow_Green` |
| Inferno (X=70) | `Enemy_Normal_Red` | `Enemy_Bow_Red` |
| Tundra (X=105) | `Enemy_Normal_Snow` | `Enemy_Bow_Snow` |

Trước 2026-11-26 cung thủ có code và prefab nhưng **0 lần** xuất hiện trong scene, nên không ai thấy nó chạy.

**3 màu cung thủ còn lại (2026-11-26):**

- Cắt `Bow/Green.png` và `Bow/Red.png` (1448×1086) thành 16 pose 256×256 theo connected-component, chân đặt y=231, tâm x=127,5 — cùng chuẩn với `Bow/Brown.png` và `Bow/Snow.png`. Kiểm hình học cả 16 ô: foot=231, |cx−127,5| ≤ 1,5 → PASS.
- Sinh 4 clip + controller + prefab cho Brown/Green/Red, đặt ở `Prefabs/Enemy/Bow/<Màu>/`. Mỗi prefab đủ 5 component (`EnemyController`, `EnemyHealth`, `EnemyGrowth`, `EnemyRangedMovement`, `EnemyRangedAttack`), **không có** `EnemyMeleeAttack`, `projectilePrefab` nối tới `EnemyArrow`.
- PPU `Bow/{Green,Red}` sửa 100 → **110** cho đồng nhất 4 màu.

**Lỗi đã gặp và sửa (2026-11-26):** cung thủ Brown/Green/Red **đứng ghim tại chỗ**, không di chuyển. Nguyên nhân: prefab do mình sinh tay đặt `Rigidbody2D.m_Constraints: 3` = **Freeze Position X + Y** (đúng phải là `4` = Freeze Rotation). Vì prefab chuẩn chỉ khoá xoay, việc ghim vị trí làm `MovePosition` không có tác dụng. Lỗi phụ cùng nguồn: `CapsuleCollider2D.m_Direction: 0` (dọc) thay vì `1` (ngang).

Cách sửa: **sinh lại 3 prefab từ chính `Enemy_Bow_Snow.prefab`** (bản Unity đã import và re-save), chỉ thay tên / sprite / controller. Nhờ vậy mọi `serializedVersion` và field vật lý theo đúng chuẩn Unity 6, không phải đoán. Diff sau khi sửa chỉ còn 3 khác biệt có chủ ý (tên, sprite, controller); 5 thông số vật lý (`m_Constraints`, `m_BodyType`, `m_GravityScale`, `m_Interpolate`, `m_Direction`) giống hệt prefab lính thường.

**Bài học:** khi sinh `.prefab` bằng tay, phải lấy khuôn từ prefab **Unity đã import lại**, không tự viết từ đầu — giá trị enum như `m_Constraints` rất dễ sai mà file vẫn hợp lệ nên Unity không báo lỗi.

**Lỗi 3 — không bắn được + nhìn sai hướng (2026-11-26):** sau khi sửa lỗi ghim vị trí, cung thủ vẫn **không bắn ra mũi tên** và **không quay mặt về Player**. Nguyên nhân:

1. **`approachDistance` = `fireRange` = 6.** Cung thủ tiến tới đúng 6 unit thì dừng, mà điều kiện bắn là `distance > fireRange → đi bộ, không bắn`. Ở ranh giới `6.0001 > 6` là true nên nó **chạy animation Walk mãi, không bao giờ vào nhánh bắn**. Sửa: `approachDistance` 6 → **5,5**, `fireRange` 6 → **7** (tách ra 1,5 unit để vùng đứng yên nằm gọn trong tầm bắn).
2. **Đạn sinh ra bên trong thân quái.** Collider rộng 2,21875 → nửa rộng **1,109**, nhưng `spawnOffset` chỉ **0,6**. Sửa: 0,6 → **1,3** (> nửa collider).
3. **Hai script giành ghi `view.flipX`.** `EnemyRangedMovement.FaceTarget` chạy trong `FixedUpdate`, `EnemyRangedAttack.SpawnProjectile` chạy trong `Update` — hai nhịp khác nhau nên hình giật/nhìn sai hướng. Sửa: **chỉ `EnemyRangedMovement` được ghi `flipX`** (hàm đổi thành `public`), bỏ hẳn dòng ghi trong `EnemyRangedAttack` cùng biến `view` không còn dùng.

Đã mô phỏng lại logic ở 15 mốc khoảng cách 1→8 unit: mọi khoảng "đứng yên" (3,5–5,5) đều nằm trong tầm bắn (≤7), và đạn luôn sinh ngoài collider. Compile Roslyn **exit 0**; 4 prefab đều có `approach < fireRange` và `spawnOffset > nửa collider`.

**Lỗi 5 — mũi tên vô hình dù vẫn gây sát thương (2026-11-26):** sau khi sửa material, mũi tên **vẫn không hiện**, nhưng log chứng minh nó vẫn trúng: máu Player tụt `68 → 76 → 84 → 92`, mỗi lần đúng **8** = sát thương mũi tên. Vậy object tồn tại, collider và script chạy — chỉ `SpriteRenderer` không có sprite.

**Nguyên nhân gốc:** `m_Sprite` trong `EnemyArrow.prefab` trỏ `fileID: 6239289893221253262` = **`internalID` của sub-sprite**. Nhưng `Arrow.png` là **`spriteMode: 1` (Single)** — với sprite Single, sprite chính có fileID cố định là **`21300000`**, còn `internalID` chỉ dùng cho sub-sprite của `spriteMode: 2` (Multiple).

Kiểm chứng quy luật này trên các asset đã chạy tốt:

| Asset | spriteMode | fileID prefab dùng |
|---|---|---|
| `Player`, 3 trùm, `Idle_Down_00.png` | 1 | **21300000** |
| `Bow/Snow.png`, 8 file quái 16 frame | 2 | internalID của frame |

Sửa: `EnemyArrow.m_Sprite` → `fileID: 21300000`. Đã quét **toàn bộ** `Assets/**/*.prefab` + `Scenes/*.unity`: không còn chỗ nào trỏ sai kiểu này.

**Đây là lỗi mình tạo ra khi viết `Arrow.png.meta` bằng tay** — mình đặt `spriteMode: 1` nhưng lại tham chiếu sprite qua `internalID`, trong khi code kiểm cũ của mình chỉ so `fileID` với danh sách `internalID` nên báo PASS sai.

**Cần kiểm lại trong Play Mode** — cả 3 lỗi trên mới chỉ sửa và kiểm tĩnh, chưa chạy thật.

**Lỗi 4 — mũi tên trúng nhưng không nhìn thấy (2026-11-26):** log Unity cho thấy `Player HP: 92/100`, tức mũi tên **có bay và gây đúng 8 sát thương** — lỗi là **hiển thị**, không phải logic. Nguyên nhân: `EnemyArrow.prefab` dùng `m_Materials` là **material mặc định của Unity** (`a97c1056…`), trong khi **mọi prefab khác trong dự án** (quái, Player, 3 trùm) dùng `Assets/Prefabs/Player/PlayerUnlit.mat` (`b672de82…`). Project là **URP 2D**, material mặc định không render sprite đúng. Sửa: đổi `EnemyArrow` sang `PlayerUnlit.mat`.

Lỗi phụ cùng lúc: `SetDirection` chỉ lưu vector mà **không xoay sprite**, nên mũi tên luôn nằm ngang dù bay chéo. Sửa: xoay `transform.rotation` theo `Atan2(direction.y, direction.x)`, và đổi `Rigidbody2D.m_Constraints` của đạn `0` → **`4`** (Freeze Rotation) để physics không kéo rotation về 0.

**Giới hạn art — cung thủ chỉ có 1 hướng nhìn (không phải lỗi code):** user hỏi vì sao cung thủ chỉ lật trái/phải mà không quay chéo/dọc. Kiểm tra cả 8 file art (`Art/Enemy/{Normal,Bow}/{Brown,Green,Red,Snow}.png`): mỗi file là **4 hàng × 4 frame = 16 pose của 4 ĐỘNG TÁC** (Idle/Walk/Attack/Death), **không phải 4 hướng**. Cả 16 pose đều vẽ nhân vật quay mặt sang **một bên**, nên code chỉ có thể `flipX` để quay trái/phải. So sánh: Player có 4 thư mục hướng riêng (`Down/Left/Right/Up`) vì được vẽ riêng.

Muốn quay chéo/dọc phải **vẽ thêm art** (8 hướng × 4 động tác × 4 frame cho mỗi màu), không giải quyết được bằng code. Cách rẻ hơn: giữ 2 hướng như hiện tại, hoặc xoay nhẹ sprite theo hướng di chuyển nếu chấp nhận hình bị nghiêng.

**Chưa kiểm được:** Play Mode — cần xem mũi tên bay, né được, trúng thì mất 8 máu; và cung thủ có giữ đúng khoảng cách 3,5–6 unit không. Unity batchmode vẫn không chạy được trên máy này (licensing `attempt to write a readonly database`), nên mọi asset sinh bằng file + kiểm tĩnh, chưa qua Unity import.

## GĐ 2 — Tách snow + dọn public (2026-11-23)

Nhánh `feature/bot-play`, chưa commit. Việc này hoàn tất phần dọn art của GĐ 2.

**Phát hiện: tiền đề trong báo cáo trước SAI.** Hai file `Art/Enemy/{Normal,Bow}/Snow.png` (1024×1024) **không phải bản cũ** — chúng khớp pixel 100% với nửa trái/nửa phải của `public/normal_enemy/snow.png` (2048×1024). Đã kiểm bằng SHA-256 vùng ảnh: `255ad04b25d19a5f` (nửa trái = Normal) và `59ab2ca2dbec3a9d` (nửa phải = Bow). Không cần tách lại.

**Vấn đề thật:** `public/normal_enemy/snow.png` (guid `605d32c7…`) đang được 10 file trỏ vào (8 clip + 2 prefab trắng), còn 2 file `Art/` có 0 reference. Xoá `public` ngay sẽ làm hỏng toàn bộ quái trắng.

**Đã làm:**

1. Sao lưu 4 file `public/*snow*` sang `Temp/enemy_backup/` (không vào git).
2. Trỏ 8 clip trắng + 2 prefab trắng sang texture `Art/` đúng nửa. Mỗi clip lấy đúng 4 frame theo hàng: Idle `frame_0–3`, Walk `4–7`, Attack `8–11`, Death `12–15`.
3. Sửa PPU 2 meta trắng từ **83 → 110** cho cao bằng 3 màu kia (trước đó goblin trắng cao hơn ~32%).
4. Dựng lại `nameFileIdTable` trong 2 meta trắng — bảng cũ lệch 16/16 dòng so với `internalID` của sprite (còn sót `frame_16`–`frame_31` của ảnh gộp).
5. Xoá `public/{normal,bow,elite_boss}/` + 3 meta thư mục rỗng.
6. Sửa 3 chỗ gãy do đổi tên prefab ở phiên trước:
   - `GunSkillTestArena.cs`: bỏ tham chiếu `EnemyContactDamage` (đã xoá) → chuyển sang `Destroy(EnemyMeleeAttack)`. **Đây là lỗi CS0246 chặn compile toàn project.**
   - `EnemySpawnerSetup.cs` + `MapSetupTool.cs`: cập nhật đường dẫn prefab sang `Enemy/{Normal,Bow,Elite}/<Màu>/`.

**Kiểm tra đã chạy thật:**

- Roslyn compile `Assets/Scripts` → **exit 0**; `Assets/Editor` → **exit 0** (tham chiếu netstandard 2.1 + shims + UnityEngine modules + `Library/ScriptAssemblies`).
- Render từng frame mà 8 clip trắng lấy ra từ texture `Art/` và xem trực tiếp: đúng pose (dao cho Normal, cung cho Bow; Attack có frame giơ cung + mũi tên bay).
- Quét toàn `Assets/`: **0 tham chiếu** còn trỏ tới guid `public/normal_enemy/snow.png` hay `spider.png`; 0 đường dẫn prefab gãy trong `*.cs`.

**Chưa kiểm được:**

- **Play Mode**: không chạy được. `Temp/UnityLockfile` bị kernel giữ từ lần Unity crash 21/11 (`relay_win.exe` orphan đã kill nhưng lock vẫn còn). Unity batchmode báo "another Unity instance is running" rồi exit 1. Cần **khởi động lại máy** (hoặc đóng tiến trình giữ lock) trước khi mở Unity.
- Chưa xem hình ảnh goblin trắng trong Game View sau khi đổi texture.
- Unity chưa import lại 2 texture trắng. Khi mở Unity lần đầu, nếu sprite bị `Missing` hoặc sai frame thì **kiểm tra `nameFileIdTable` trong `.meta` trước** rồi mới nghĩ tới clip.

**Quan sát nhỏ (chưa sửa, ngoài phạm vi):** `textureCompression` không đồng nhất giữa 11 file art — `Normal/Brown.png` là `0` (Uncompressed), 10 file còn lại là `1` (Compressed). Với pixel art nên là `0`; nếu chữ/sprite bị nhòe thì sửa hàng loạt.

**Còn lại của GĐ 2:** `Art/Enemy/Bow/{Brown,Green,Red}.png` và `Art/Enemy/Elite/{Green,Red,Snow}.png` mới import, **chưa cắt frame** (PPU 100, 1 sprite). `Prefabs/Enemy/Bow/{Brown,Green,Red}/` và `Elite/*/` đang rỗng.

## Sân thử hệ Súng - 2026-11-10

- Pháo đài cơ động dùng atlas tách chân đế, nòng, vòng dưới đất và chớp đầu nòng. Play Mode trong bản sao xác nhận chân đế giữ góc đứng thẳng khi nòng ngắm cả bốn hướng, chớp bám hướng nòng và giới hạn hai trụ vẫn hoạt động.

- Phím R thay nút Tung chiêu trong GunSkillTest. Bảng vẫn hiển thị hồi chiêu và số trụ. Play Mode trong bản sao xác nhận phím R mô phỏng kích hoạt cả năm chiêu qua Input System và vẫn giữ giới hạn hai trụ.

- Năm chiêu cuối Hexcore chạy trong GunSkillTest, chọn và tung từ bảng bên phải. 14 texture đã nối vào scene. Pháo đài cơ động giới hạn hai trụ. Play Mode headless xác nhận cả năm chiêu phát sprite và gây sát thương, hai loạt bom có năm điểm nổ, tia xuyên hai bia, Lồng đạn bắn đủ sáu cặp, và trụ thứ ba bị chặn cho đến khi trụ cũ kết thúc. Đặt lại sân dọn hiệu ứng và giữ lựa chọn. Cần xác nhận trực tiếp thao tác nút và hình ảnh trong Game View.

- Chuẩn bị 14 sprite sheet cho năm chiêu cuối cấp 12 tại `public/gun/hexcore-ultimate/`. Ngũ bom có 5 quả, Pháo đài cơ động thiết kế cho tối đa 2 trụ cùng tồn tại. README ghi FPS, pivot và cách ghép hiệu ứng. Đã kiểm tra kích thước, thứ tự frame, mép ô, bản xem trước và SHA-256. Cần xác minh khi nhập và nối vào Unity.

- GunSkillTest dùng bảng nút cuộn cố định bên phải để chọn nhánh, chỉnh bậc, bật hai lõi và đổi mục tiêu. Camera theo người chơi trong phần sân còn lại. Play Mode headless xác nhận vùng camera, chọn lại bia boss và tự thử lại hai lần khi hết máu. Cần xác nhận trực tiếp thao tác bấm và cuộn trong Game View.

- Sửa sân thử boss lõi hai dừng khi người chơi về 0 HP. Sân đếm ngược 1,5 giây rồi hồi 100 HP, tạo lại boss 2.500 HP và tiếp tục với các lựa chọn lõi hiện tại. Play Mode headless xác nhận hai lần chết do đòn boss liên tiếp, cả hai bên tiếp tục tấn công sau khi đặt lại.

- Lõi hai trong GunSkillTest có 6 kỹ năng. Ctrl+1/2 bật độc lập theo nhánh và có thể bật cùng lõi đầu. Shift+4 bật boss 2.500 HP với đạn toả và lao có báo trước. Play Mode headless trong bản sao Unity 6000.3.22f1 xác nhận 11 texture được nhập, cả sáu kỹ năng có hiệu ứng và gây sát thương, Giáp chặn đạn, cổng đổi hướng, neo đứt hoặc hết hạn, và boss bắn rồi lao. Ba nhánh đã được thử với kỹ năng lõi đầu bật cùng lõi hai. Cần xác nhận phím và hình ảnh trực tiếp trong Game View.

- 2026-11-11. Hoàn thành 11 sprite sheet PNG nền trong suốt cho sáu kỹ năng lõi hai tại `public/gun/core-2/`. Mỗi sheet có 32 frame 256 × 256. Đã kiểm tra thứ tự frame, mép ô và SHA-256. README ghi FPS, pivot và cách ghép các phần. Cần xác minh trong Unity khi nối gameplay.

- Cấp 11 chạy trong GunSkillTest với chín kỹ năng và sprite lấy từ bộ art đã duyệt. Shift+1/2/3 chọn kỹ năng theo nhánh, Shift+7 bật một bia boss 1.500 HP miễn đẩy lùi. Play Mode headless trên bản sao Unity 6000.3.22f1 đã xác nhận đủ chín animation đổi frame và tạo sát thương trên bia, gồm hai lượt của Xuyên kích hồi quy. Chưa xác nhận phím vật lý và hình ảnh trong Game View.

- Art cấp 11 đã chuẩn bị trong `public/gun/level-11/`. Có 9 sprite sheet kỹ năng và 2 sheet lặp riêng cho vòng quỹ đạo và đạn vệ tinh. Mỗi sheet có 32 ô RGBA 256 × 256. Đã kiểm tra kích thước, mép ô, bản xem trước và SHA-256 sau khi chép. Cần kiểm tra trong Unity khi nhập asset và nối gameplay.

- Dư chấn dùng sprite xanh ngọc nhập từ public/gun/piercing-shockwave.png. Sóng lan và mờ dần trong 0,38 giây. Play Mode tự động trong bản sao Unity xác nhận sprite được gắn đúng, xuất hiện ở mục tiêu thứ hai, nở ra, giảm alpha và được dọn sau khi phát. Chưa xem Game View.

- Giảm tốc độ bắn ba nhánh hai lần, mỗi lần còn một nửa. Đạn tỏa cách 2,60 giây mỗi phát, Liên thanh cách 0,32 giây mỗi viên và nghỉ 1,20 giây sau loạt, Xuyên thấu cách 0,80 giây mỗi phát.

- Thêm chín nâng cấp bậc 0–3 theo ba nhánh. Phím 7–9 tăng bậc, Shift+8 đổi tự bắn, Shift+9 đổi máu và Shift+0 bật bia di chuyển. Play Mode trên bản sao Unity 6000.3.22f1 xác nhận số đạn, góc, đẩy lùi, số lần nảy và mảnh, sát thương xuyên, giảm tốc và hồi tốc, bán kính và giới hạn một Dư chấn. Bài hồi quy va chạm cho các hiệu ứng cũ cũng PASS. Cần xem Game View để đánh giá độ rõ của vòng Dư chấn.

- Súng mặc định tự bắn ngay khi vào Play. Sau khi hạ bia cuối, cả dàn bia hồi sinh sau 0,75 giây mà không khởi động lại nhịp bắn. Kiểm tra Play Mode trong bản sao Unity 6000.3.22f1 đã xác nhận tự bắn, không sinh sớm khi còn bia và hồi sinh đủ 8 bia trong hai lượt liên tiếp.

- Bảng sân thử dùng phím số 0–9 để chọn kỹ năng, chọn nhánh, bật nâng cấp, tự bắn, đổi máu bia và đặt lại sân. C# biên dịch qua Roslyn. Phím số chưa được kiểm tra trực tiếp trong Play Mode.

- Thêm `Assets/Scenes/GunSkillTest.unity` và ba script dưới `Assets/Scripts/Weapon/GunSkills/`. Scene thử dùng lại prefab Player và Enemy, dựng bia đứng yên, cho chọn kỹ năng cấp 4 và nhánh riêng với công tắc nâng cấp. Xem `docs/HUONG_DAN_TEST_SUNG.md`.
- Nhập sáu PNG hiện có từ `public/gun` vào `Assets/Art/GunSkills/`. Đạn xuyên dùng alpha có sẵn và hiệu ứng Ngắm điểm yếu phát bốn frame lửa ở 12 FPS.
- C# biên dịch bằng Roslyn với Unity 6000.3.22f1 và thư viện hiện có của dự án. Đã kiểm tra các GUID mới trong scene có đúng một asset tương ứng, và sáu PNG khớp SHA-256 với public. Play Mode tự động trong bản sao Unity 6000.3.22f1 đã PASS: nảy sang bia sống gần nhất, sinh 3 đạn phụ khi hạ bia, frame lửa chạy, Đạn tỏa 3/4, Liên thanh 5/7 và Xuyên thấu nâng cấp gây 10/13/16 qua ba bia. Chưa xem hình ảnh trong Game View.

## Player artwork installed - 2026-11-09

- User follow-up authorized completing Unity setup directly. Applied and saved the existing Player prefab: Idle_Down_00 sprite, PlayerCharacter Animator controller, PlayerSpriteAnimator on PlayerVisual, centered/unit visual transform, root motion off, normal update mode/always animate, old PlayerController.playerVisual rotation reference cleared. GameScene inherits the updated prefab; existing scene Player/tag/camera/weapon/manager references remain intact.
- Added Tools > Brotato > Apply Player Artwork as a separate explicit setup command in PlayerSpritePreparation. Prepare Player Sprites still only generates artwork assets. Unity's prefab save also dropped obsolete serialized respawnForTesting/respawnDelay fields, which are absent from current PlayerHealth code; no HP/respawn behavior was changed.
- Checks actually executed in Unity 6000.3.25f1: script compiled; setup command completed; entered real GameScene Play Mode and observed the new character, Idle_Down_02 frame progression, independently aimed gun and spawned bullets. Natural lethal enemy contact reached Game Over with Death_Down_03 and Animator Unscaled Time, confirming the death clip reached its final frame during combat freeze. Clicked the actual PLAY AGAIN button and observed a fresh standing character and firing. Stopped Play Mode afterward; prefab/setup remained saved.
- Needs verification: sustained directional Run/Idle playback and intermission animation freeze were not conclusively observed in this short manual run; no full-duration balance test, new automated regression or Windows build. Existing unrelated changes preserved; no commit/push.

## Player sprite preparation - 2026-11-09

- Request: cut balanced frames for each action and provide step-by-step Unity setup. Completed asset preparation; prefab/scene installation is deliberately manual for the user's learning workflow.
- Added `Assets/Prefabs/Player/CharacterArt/`: transparent extraction sheet, 52 individual PNG frames (Idle 4 / Run 5 / Death 4 per direction Down/Left/Right/Up), 12 AnimationClips and PlayerCharacter.controller. All Unity-generated .meta files accompany the assets.
- Used built-in imagegen for background extraction; prompt saved in `docs/player-art/extraction-prompt.txt`. Original source/imported source remain intact. AI extraction is not guaranteed pixel-identical to the original artwork.
- Editor preparation identifies 52 separate strong-alpha components, retains each component and its nearby soft outline instead of copying neighboring poses inside overlapping bounding rectangles, then centers each pose horizontally on a transparent 192x224 canvas. No per-frame resizing. PPU120, Center pivot, Point/no compression; strong-alpha ground is y34 in all 52 outputs, with no opaque pixel touching a canvas edge.
- Added `Assets/Editor/PlayerSpritePreparation.cs`, menu Tools > Brotato > Prepare Player Sprites. Generates/validates frames, clips, controller, bounds CSV and contact sheet; never edits Player prefab/GameScene. Rerunning overwrites this generated set's imagery/clip curves, preserving their GUIDs; do not rerun over later hand edits without intent.
- Added optional `Assets/Scripts/Player/PlayerSpriteAnimator.cs` for PlayerVisual. Reads current WASD for visual direction, plays named Idle/Run/Death states, subscribes to parent PlayerHealth.Died, freezes normal clips with scaled time and uses unscaled animation only for terminal Death. Existing PlayerController/PlayerHealth/GameManager/WeaponAim were not modified.
- User wiring: assign Idle_Down_00 + controller + Animator + PlayerSpriteAnimator to existing PlayerVisual; clear PlayerController.playerVisual to None so the old Atan2 rotation is skipped; keep Player root physics and scene-added WeaponHolder. Detailed Vietnamese guide: `docs/HUONG_DAN_PLAYER_ANIMATION.md`.
- Checks actually executed: Unity 6000.3.25f1 imported/compiled both scripts; ran preparation in the existing Editor via computer-use; Unity logged PLAYER SPRITE ASSET VALIDATION PASSED and 52 frames / 12 clips / 1 controller. Validator checked all clip sprite references and 12 states. Console showed 0 errors and 2 editor automation/account API warnings, not gameplay errors. Preview inspected and neighbor-bleed defect corrected before final output. Read-only pixel checks confirmed 52 PNGs at 192x224, common strong-alpha ground y34 and zero opaque edge pixels; 52 corresponding PNG .meta files verified.
- Needs verification: user wiring, actual Idle/Run/Death playback in Play Mode, intermission freeze/death-at-timeScale-zero, aim/fire and retry after wiring. No new Play Mode regression or build was executed. Generated assets alone do not change current scene visuals.
- Preserved pre-existing EditorBuildSettings and Assets/public changes. Unity's incidental ShaderGraphSettings line-ending rewrite was restored to original checkout formatting with identical content. No commit/push.

## Asset guidance checkpoint - 2026-11-09

- Request: explain how to replace the Player artwork using `public/player` and see it in GameScene. This checkpoint provides instructions only; no runtime code, imports, prefab or scene changes were applied.
- Inspected current code, Player prefab, GameScene prefab-instance overrides and texture import metadata. Scene Player inherits PlayerVisual's sprite; WeaponHolder is still a scene-added child of Player. Preserve that instance and its references when replacing artwork.
- Source `public/player/ChatGPT Image 19_25_43 25 thg 9, 2026.png` and the already-imported `Assets/public/player` copy have identical SHA256 hashes. Image is 1448x1086, RGB, every pixel alpha 255: the purple checkerboard is painted background, not transparency. Background removal requires a separately prepared transparent PNG; importer transparency settings alone cannot remove it.
- Imported sheet uses Multiple mode but has only one full-sheet rectangle, PPU 100 and Bilinear filtering. Guide recommends manually defining one upright frame first, Point filtering, no compression, and tuning PPU for roughly 1.3-1.6 world-unit height before any animation work.
- PlayerController.UpdateFacingDirection currently rotates PlayerVisual with Atan2. Guide proposes replacing that visual rotation with horizontal SpriteRenderer flipping for a right-facing upright frame; no code change or animation implementation was performed.
- Saved camera orthographic size is 5; PlayerVisual sorting order is 0 and GunVisual order is 1. Player root scale is 1; keep root physics/weapon scale intact while tuning artwork.
- Current ProjectVersion.txt specifies Unity 6000.3.25f1, superseding the older 6000.3.22f1 notes below for this checkout. No editor/package upgrade was performed.
- Checks: read-only source/serialized inspection, image preview, pixel alpha inspection and source/imported-copy hash comparison. Needs verification: compilation after any user-applied code change, actual sprite import, sizing, facing and shooting in Play Mode. No new compilation, Play Mode run or build was performed.
- Preserved pre-existing `ProjectSettings/EditorBuildSettings.asset` status and untracked `Assets/public/` plus `Assets/public.meta`; no commit/push.

Session checkpoint: 2026-09-18. Git root: D:/lap_trinh_game/cuoi_ky/game. Current HEAD: 60c4f25 (làm wawe quái và máu nhân vật); working tree was clean immediately before this documentation update. Refresh status every session.

## Goal and phase

Stabilize the university project's 3-wave MVP before adding Week 4/5 features. The reported difficulty, missing spawner reference, instant transitions, duplicate-hit risk and missing camera-follow implementation have now been addressed. Unity Play Mode regression passed; full-duration balance playtest and Windows build remain unverified.

## Completed today

- Fixed the requested combat/difficulty issues and saved GameScene/Bullet prefab; exact gameplay settings are recorded below.
- Added and ran reusable Editor-only combat regression in real Unity Play Mode.
- Updated original plan section 24 and project memory to reflect the fixes, tests and remaining verification.
- Reviewed progress with the user: Week 3 features are implemented; most Week 4 wave behavior exists, but WaveData ScriptableObjects do not.
- Explained WaveData in Vietnamese: separate per-wave configuration assets read by WaveManager; existing inline WaveSettings already contain those values, so migration changes data organization rather than adding new gameplay.
- Explained PLAY AGAIN: drawn by GameManager.OnGUI in Game View after Game Over or final Victory; no Button object exists in Hierarchy/Canvas. Intermediate Wave Complete has no retry button.
- This checkpoint edits documentation only; no implementation, scene or prefab changes.

## Currently in progress

- MVP validation/handoff: accelerated regression has passed, but full-duration playtest and Windows-build verification remain pending.
- User is learning the next Week 4 task and the current prototype UI. No WaveData implementation is underway or authorized by the explanation/checkpoint requests.
- No additional feature is being implemented. Do not treat discussion of Week 5 as authorization to add upgrades.

## Implemented and saved

- GameScene remains the gameplay/build scene. Existing normalized WASD, mouse aim and continuous auto-fire are preserved.
- WaveManager owns a serialized WaveSettings array, editable directly in Inspector (not ScriptableObjects yet).
- Wave 1: 30 seconds, cap 10 living enemies, 10 HP each, spawn interval 1.5 seconds, speed 2 units/s.
- Wave 2: 30 seconds, cap 15, 20 HP, interval 1 second, speed 2.5 units/s.
- Wave 3: 45 seconds, cap 30, 30 HP, interval 0.6 seconds, speed 3 units/s.
- Counts are simultaneous living caps, NOT total enemies for a wave. Death frees a slot; spawning continues until time expires.
- Explicit WaveManager -> EnemySpawner and other manager references saved in GameScene.
- Wave 1/2 completion freezes combat and shows WAVE COMPLETE for 2 real seconds; then the next wave starts automatically.
- Old enemies and bullets are deactivated/destroyed at transitions and terminal outcomes. Player HP and position persist across waves; no free healing.
- Final completion uses Victory state / ALL WAVES COMPLETE. Death uses Game Over. Both allow PLAY AGAIN.
- Enemy initialization sets absolute HP/speed, including the placed scene enemy; no multiplier switch or compounded scaling.
- Bullet guards its first accepted hit before damage/Destroy. Circle trigger remains enabled; redundant Box trigger disabled in saved prefab, retained for recovery.
- CameraFollow on Main Camera tracks Player XY in LateUpdate, preserving camera Z. Spawning uses outside-camera positions, margin 1 and minimum Player distance 3.
- Player movement is intentionally unbounded for now: camera follows, no invented wall/map rectangle.

## Checks actually executed

- Runtime scripts compiled with Roslyn against project Unity assemblies: exit 0; Inspector-field CS0649 warnings only.
- Unity Editor imported/compiled changes and saved scene/prefab via Tools > Brotato > Apply Combat Setup.
- Ran Tools > Brotato > Run Combat Regression in actual Unity Play Mode: ALL CHECKS PASSED.
- Confirmed serialized references, all 3 durations/intervals, living caps 10/15/30, HP/speed on spawned enemies, spawn positions outside camera and replacement after a kill.
- Real Physics2D overlap with BOTH bullet colliders temporarily enabled did exactly 10 damage; explicitly repeating callback also did one hit only. Enemy HP clamps to zero.
- Camera followed a displaced Player and retained Z; Player hit cooldown blocked a consecutive hit.
- Timer expiry -> 2-second intermission -> next wave; cleanup/freeze, preserved/protected HP, final Victory, lethal HP zero -> Game Over.
- Restart method used by PLAY AGAIN successfully reloaded scene after both results, resetting wave/timer/HP/timeScale and controls.
- No gameplay runtime errors during the regression; final Console showed zero errors/warnings.
- Reusable Editor-only test: Assets/Editor/CombatRegression.cs. Local detailed result: Temp/CombatRegression.txt (generated/ignored, not source).

## Verification limits / next steps

1. Play an ordinary full-duration 30/30/45-second round to assess difficulty, moving/aiming feel and camera-follow feel. Automated tests shorten timers and force spawn scheduling to exercise caps; they are not a human balance playtest.
2. Build/test Windows executable; not done in this session.
3. Arena boundaries are still unspecified. Current choice is free movement + following camera; don't invent map limits without design.
4. When requested, migrate inline WaveSettings to reusable WaveData ScriptableObjects. Keep current values/transition policy unless user changes them.
5. Only then proceed to Kill Count / gun upgrades and further content at user's request.

Concrete continuation for the next session:

1. Open GameScene in Unity 6000.3.22f1; play normal 30/30/45-second waves while moving and aiming. Assess spawn pressure, damage cooldown and camera behavior; record actual observations.
2. Exercise the visible PLAY AGAIN button after Game Over and final completion. Earlier regression invoked its restart method; it did not click the rendered button.
3. Build a Windows player with GameScene enabled and verify launch, gameplay and retry outside the Editor.
4. When the user asks to begin WaveData, explain and implement a small ScriptableObject with duration, maxEnemiesAlive, enemyHealth, spawnInterval and enemySpeed. Create Wave1/Wave2/Wave3 assets using the current values; wire the list into WaveManager and pass the selected data to EnemySpawner. Preserve the two-second cleanup transition and one-life HP policy. Regression must still pass.
5. Once Week 4 is validated, request-directed Week 5 work is Kill Count, WeaponLevelData, WeaponUpgradeManager and weapon-level HUD; keep 3 waves until explicitly expanded.

## Known bugs/issues and uncertainty

- Previously confirmed multiplier-case bug is fixed; the earlier instant-transition/missing-reference/duplicate-hit concerns are addressed. No additional gameplay bug was confirmed by this checkpoint.
- WaveManager assumes a nonempty waves array with valid entries; clearing the Inspector list or null entries can cause initialization errors. Current saved scene has three valid entries. Validation hardening is not implemented.
- Full-duration balance, moving/aiming feel and Windows build are still Needs verification. Automated spawn/timer acceleration does not establish human-play difficulty.
- Arena dimensions/walls remain unspecified; camera follows free Player movement. This is the current prototype policy, not a confirmed bug.
- Prototype UI is OnGUI, not Canvas: PLAY AGAIN exists only after terminal outcomes and is not discoverable as a Hierarchy Button.
- Applying CombatSetup again resets wave tuning to this request's defaults; use deliberately.
- Original plan section 24 contains an explicitly marked historical pre-fix log. Its old bug list and all-30-second values are superseded by the later update.

## Important paths and setup

- Assets/Scenes/GameScene.unity: main scene, saved wave values and camera/spawner wiring.
- Assets/Scripts/Core/{GameManager,CameraFollow}.cs: game state/UI/retry and camera.
- Assets/Scripts/Wave/WaveManager.cs: timer, inline settings, unscaled intermission.
- Assets/Scripts/Enemy/{EnemySpawner,EnemyController,EnemyHealth,EnemyContactDamage}.cs: spawning, absolute stats, pursuit, contact.
- Assets/Scripts/Player/{PlayerController,PlayerHealth}.cs and Weapon/{WeaponAim,Bullet}.cs: controls/combat.
- Assets/Editor/CombatSetup.cs: one-time setup helper. Reapplying it resets wave tuning to this request's defaults; not needed on every launch.
- Player weapon hierarchy remains scene-added, not fully inside Player prefab.
- OnGUI is prototype UI; no finished Canvas/main menu/pause/items/upgrades yet.

## Important files changed earlier today

- Assets/Scripts/Enemy/EnemySpawner.cs: per-wave absolute stats, active-living cap, spawn distance and cleanup.
- Assets/Scripts/Enemy/EnemyController.cs and EnemyHealth.cs: absolute speed/HP initialization and exposed test/HUD properties.
- Assets/Scripts/Wave/WaveManager.cs: inline WaveSettings, 30/30/45 timers and unscaled intermission.
- Assets/Scripts/Core/GameManager.cs: intermediate/final states, cleanup, freeze/resume and result/retry UI.
- Assets/Scripts/Core/CameraFollow.cs (+ .meta): new camera target tracking.
- Assets/Scripts/Player/PlayerHealth.cs: damage protection while combat is stopped.
- Assets/Scripts/Weapon/Bullet.cs and Assets/Prefabs/Weapon/Bullet.prefab: one-hit guard and disabled redundant Box trigger.
- Assets/Scenes/GameScene.unity: explicit references, camera component/target and saved wave tuning.
- Assets/Editor/CombatSetup.cs and CombatRegression.cs (+ generated .meta): scene/prefab setup and reusable Play Mode checks.
- AGENTS.md, docs/STATE.md, DECISIONS.md, ARCHITECTURE.md, GAME_DESIGN.md and ke_hoach_do_an_game_2d_brotato_like.md: updated rules, handoff and dated progress. Older prefab/build-setting/weapon changes were preserved rather than attributed to this checkpoint.

Checkpoint checks: reread project guidance/memory, inspected current WaveManager and retry UI code, refreshed HEAD/status, and confirmed the existing Temp/CombatRegression.txt ends with ALL CHECKS PASSED. No new compile, Play Mode run or Windows build was performed for this documentation-only checkpoint. Other memory files already reflect current decisions/architecture/design and need no further edits.

## Git handoff

Earlier implementation preserved pre-existing changes and did not itself commit/push. At checkpoint, git status was clean and HEAD had advanced to 60c4f25; earlier work is now represented in the local repository. This checkpoint modifies only docs/STATE.md and does not commit/push. Remote push status was not checked. Include source/.meta/docs when sharing; exclude Library, Temp and Logs.

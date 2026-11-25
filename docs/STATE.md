# Current project state

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

**Kiểm tra đã chạy thật:** Roslyn compile `Assets/Scripts` (25 file) → **exit 0**; `Assets/Editor` → **exit 0**. Kiểm prefab: 10 component, không fileID nào trỏ vào block không tồn tại; không còn tham chiếu `EnemyMeleeAttack` trong prefab cung thủ. Kiểm `.meta`: guid sprite mũi tên trong prefab khớp `Arrow.png.meta`.

**Chưa kiểm được:** Play Mode — cần xem mũi tên bay, né được, trúng thì mất 8 máu; và cung thủ có giữ đúng khoảng cách 3,5–6 unit không.

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

- Thay tám animation theo public/new_asset bằng GunNewSkillArt. Quake tạo vệt lửa; rocket và cầu lửa có đoạn bay/nổ, cầu lửa gây rạn giáp; Holy Smite thay thương; vệ tinh đỏ dùng life-steal với cơ chế tấn công; khiên vàng chặn mọi hướng và giữ đạn phản riêng; portal dùng cặp cổng đứng; Phun lửa Hexcore bám hướng ngắm trong 5 giây, hồi thử 9 giây từ lúc bắt đầu. Play Mode trong bản sao xác nhận đủ tám sheet đúng kích thước, frame đổi, sát thương, rạn giáp, khiên bốn hướng, đạn phản cũ, cổng, phím R và luồng lửa ngừng gây sát thương sau 5 giây. Cần xem trực tiếp Game View.

- Mưa thiên thạch thay Trực thăng oanh kích trong GunSkillTest. Dùng sheet public/new_asset/meteor.png, 12 cột với ô 272 × 232. Một lần R gọi 5 điểm rơi trong vùng quanh điểm ngắm, cách nhau 0,22 giây, mỗi điểm gây 47 sát thương diện rộng. Play Mode trong bản sao xác nhận phím R gọi đủ 5 thiên thạch, tất cả dùng sheet mới và rơi trong vùng đã chọn, mỗi quả gây sát thương đúng một lần, mục tiêu ngoài vùng an toàn, hồi chiêu 5 giây và tự dọn hiệu ứng. Chưa xem trực tiếp trong Game View.

- Lựu đạn mới dùng nguyên sheet public/new_asset/grenade.png, cắt 10 cột với ô 300 × 200. Ném một quả mỗi lần R, tối đa 3 lượt dự trữ, hồi tuần tự mỗi 2,4 giây. Play Mode trong bản sao Unity xác nhận texture giữ đúng kích thước, animation ném và nổ dùng sheet mới, ba lượt ném gây ba lần sát thương, hết bom bị chặn, hồi lần lượt và không quá ba quả. Chuyển chiêu không làm đầy kho bom, đặt lại sân phục hồi kho. Chưa xem trực tiếp trong Game View.

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

## HEXCORE · Game View và điều khiển tự do

- Scene `HexcoreSkillPreview` mở ở chế độ Tự do. WASD hoặc mũi tên di chuyển tám hướng, Space thi triển theo hướng hiện tại, R về điểm đầu. Phát lại trong chế độ này bắt đầu ở vị trí hiện tại. Chế độ Trình diễn giữ lựa chọn pose, chuỗi, tốc độ và bước khung.
- Tên tiếng Việt đầy đủ theo năm nhóm, vùng HUD/danh sách nằm ngoài viewport camera. Sửa index vòng chạy chéo từ tám về bốn frame của đúng hướng. Vạn Ảnh dùng shader/material riêng của preview để lọc nền xám gần vệt vàng. [Kiểm thử và ảnh đối chiếu](HEXCORE_ANIMATION_QA.md#đợt-giao-diện-và-điều-khiển).

## HEXCORE · Visual polish và gallery

- [Gallery trước/sau](player-art/HexcoreVisual/index.html) bao phủ 264 tổ hợp, có clip tắt VFX, chuỗi Basic và video ba lõi có audio từ Unity. [CSV hiện tại](player-art/HexcoreVisual/HexcoreVisualReview.csv) ghi 263 AuthoredBlockout, 1 In Review và 0 mục Ready for Review mới.
- Thay 320 frame chéo của ba lõi, bổ sung 16 pose cho Cuồng Phong Đao và 64 điểm đo. Unity 6000.3.25f1 xác minh chuỗi, đổi hướng và reset bằng scene đang có. [Báo cáo polish](HEXCORE_VISUAL_POLISH_REPORT.md) và [QA](HEXCORE_ANIMATION_QA.md) ghi các lỗi còn lại và cách chạy lại review.

## HEXCORE · Hoàn thiện phạm vi hướng

- `HexcoreSkillPreview` có dữ liệu 24 kỹ năng, 33 cặp form–skill và 264 hàng có hướng. Q1 Tây Bắc được bổ sung. Q2 quay bằng pose riêng, các hàng Q3/E/F/R được dựng bằng chuỗi theo form và hướng. Bốn form vẫn có Idle/Run/Turn/Basic tám hướng.
- 2.816 PNG, 364 clip pose, 13 WAV. Dữ liệu neo theo frame được thêm cho Q3 ở bảy hướng và R1/R2 N/S trên bốn form. Các kỹ năng mới giữ Blockout trong khi chờ polish và duyệt hình. Q1 E tiếp tục In Review.
- Xem [bàn giao](HEXCORE_ASSET_PRODUCTION_REPORT.md), [kết quả kiểm thử](HEXCORE_ANIMATION_QA.md), [checkpoint](HEXCORE_PRODUCTION_PROGRESS.md) và [bộ GIF](player-art/HexcoreFinal/Review/README.md).

## HEXCORE · Bổ sung hướng và animation · 2026-11-18

- Cả bốn form có Idle/Run, pose Turn và Basic tám hướng. Có 20 chuỗi Q2/Q3/E/F riêng hướng E, Q1 ở 7 hướng và sprite riêng R1–R3 E/W trên bốn form. Hai phương án Q1 NW chưa qua kiểm tra tay kiếm/giáp vai.
- Thêm 13 cue WAV sinh từ nguồn tổng hợp có thể tái tạo. Đo điểm mũi kiếm khi phát QiWave và R2, điểm chạm đất R1 và neo tác động Q1.
- Unity 6000.3.25f1 đã chạy Play Mode trên bản sao. Ma trận có 264 hàng, gồm 135 trình diễn (84 dùng pose nguồn) và 129 thiếu pose. Có 1.140 PNG và 151 clip trong các thư mục HexcorePreview. Các hướng chính chỉ có pose nguồn giữ trạng thái Blockout. Xem [bàn giao và giới hạn](HEXCORE_ASSET_PRODUCTION_REPORT.md).

## HEXCORE · Preview toàn danh mục · 2026-11-18

- Scene `HexcoreSkillPreview` có 4 form, 24 kỹ năng và 33 tổ hợp tương thích. Có 399 sprite và 69 clip bổ sung trong các thư mục `HexcorePreview`, tính cả 34 sprite của Đợt 1. Repo giữ các prefab và Animator gốc cho hướng chính.
- Có Basic E/W cho bốn form, Q1 E/S/W, pose riêng E cho ba lõi trên bốn form, bốn key pose E cho mỗi Q2/Q3/E/F. 8/7/8/8 hướng locomotion xem được, với Idle chéo giữ pose. Sát thủ SW chưa đạt phối cảnh. Các combo/hướng chưa có pose, Turn chuyên biệt, in-between và polish SFX vẫn cần bổ sung.
- Unity 6000.3.25f1 Play Mode trên bản sao đã qua kiểm tra chuỗi Q1 1×/0,5×, 33 tổ hợp và mốc VFX. Chụp 33 ảnh camera và xuất 24 GIF hướng Đông. Q1 E In Review, các kỹ năng còn lại Blockout. [Báo cáo bàn giao](HEXCORE_ASSET_PRODUCTION_REPORT.md)

## HEXCORE Đợt 1 · Form 01 · 2026-11-18

- Thêm 34 sprite từ ảnh ImageGen dùng tham chiếu Kiếm sĩ gốc. Scene có Basic hai nhát và bộ Q1 riêng hướng Đông, 4 hướng Idle/Run từ Animator gốc, hai hướng NE/SE với Run mới và Idle giữ một pose.
- Vệt kiếm, bụi chân và tia va chạm đi theo các điểm neo/mốc trên timeline preview. Chuỗi mặc định kết thúc bằng Run sau nhát Basic.
- Unity 6000.3.25f1 Play Mode trên bản sao đã qua bài kiểm tra 1× và 0,5×, mốc chạm, chạy ra, replay và sáu hướng có dữ liệu. Ghi GIF từ 48 ảnh camera Unity.
- Needs verification: designer duyệt chuyển tiếp và chân trụ. Thiếu SW/NW, Idle NE/SE nhiều frame, Basic/Q1 ở các hướng ngoài E và SFX. [Báo cáo Đợt 1](HEXCORE_FORM01_STAGE1.md)

## HEXCORE Đợt 0 và Q1 blockout · 2026-11-18

- Lưu bản đặc tả đầy đủ và [báo cáo kiểm kê](HEXCORE_ASSET_AUDIT.md), gồm 4 form, ma trận tương thích ban đầu và yêu cầu sản xuất pose. Kiểm tra tĩnh đã đối chiếu 312 PNG với thông số import, GUID của 64 clip và 24 tiêu đề kỹ năng trong đặc tả.
- `HexcoreSkillPreview` dùng prefab Kiếm sĩ nguồn, dummy nhện Kenney CC0 và tia va chạm dựng bằng LineRenderer. Preview Idle/Run N/E/S/W và chuỗi Run → Basic → Q1 → Basic hướng E. Dùng pose tạm cho Dash/Recovery, một nhát Basic nguồn, chưa có SFX. Kỹ năng ở trạng thái Blockout.
- Unity 6000.3.25f1 đã biên dịch và chạy Play Mode trên bản sao vì project gốc đang có khóa Editor. Bài kiểm tra thấy tia va chạm trong đúng sprite swing, nhân vật về Idle tại điểm cuối đường lướt và Phát lại đưa nhân vật về vị trí đầu, xóa tia. Log `Temp/HexcoreRetest.log` có ngoại lệ từ Unity Search khi Editor lập chỉ mục và dòng PASS của bài kiểm tra demo.
- Needs verification: độ mượt và bố cục Game View khi người dùng duyệt, pose hướng chéo, nhát Basic thứ hai, các frame chuyển tiếp và pose Q1 riêng.

## All-skill background cleanup audit — 2026-11-13

- Added CleanVfx shader/material to Demo effects except the dedicated surrounding barrier and gold-only VanAnh materials. Suppresses low-saturation background haze, keeps colored cuts/particles and nearby white highlights, and retains dark opaque steel/stone details. Uses existing PNGs without rewriting source artwork.
- MeleeDemoSetup wires the shared cleanup material. Verification capture now supports all fifteen selections. Graphics-enabled Unity batch BuildAndVerify passed all fifteen checks, including character movement/barrier follow checks, with no reported shader/C# errors.
- Rendered all fifteen skills and inspected labelled docs/player-art/MeleeDemo/CleanupAudit.png. No grey backing panels visible in these representative renders; original shield outline, blade trails and fire/ground fragments remain visible. Material filtering applies throughout each clip. Subtle translucent glow is deliberately retained around actual cuts.

## Surrounding barrier and WASD demo control — 2026-11-13

- Opening shield Demo now renders a transparent turquoise/gold elliptical barrier around the whole character using BarrierShield shader/material. Original sprite texture is reused on a runtime FullRect sprite so the old tight shield mesh cannot clip the barrier. Barrier stays visible, pulses on the paused demo clock and follows actor center.
- WASD uses existing Input System Keyboard input with normalized diagonal movement at 2.8 units/s. First input takes control from scripted movement, position stays under player control through demo cycles, and arena bounds keep the actor visible. Run direction follows movement. Pause freezes control; replay resets position. Body-attached barrier/orbit effects follow the actor.
- MeleeDemoSetup wires the barrier material and includes Rebuild and Verify Melee Demo (Ctrl+Alt+F8). Footer explains Game View focus/WASD. Demo retains scripted skill playback and illustrative enemy reactions.
- Unity Play Mode passed all 15 skill checks plus normalized diagonal displacement and barrier-to-actor centering. Rendered and visually inspected docs/player-art/MeleeDemo/CombatPreview0.png with a complete surrounding barrier and visible character. This remains a controllable visual demonstration with scripted skill timing/reactions.

## Demo corrections from screenshots — 2026-11-13

- Fixed UI trails by clearing header/sidebar/footer outside the camera viewport on every repaint. UI and camera reserve space using the same responsive scale, including small Game View sizes. Inspected live Unity Game View with clean text and visible actor/enemies.
- Reworked existing KhienChan sheet into a recognizable pointed shield with metal rim using built-in imagegen. Re-exported original eight frame paths/GIF; prompt and prior source retained under docs/player-art/MeleeSkills. Updated shield description.
- Removed assassin retreat discontinuity at 1.4s and makes retreat face left. HoanhTao uses continuous rotation of original sweep frame; HoiNguyet uses one original crescent on a smooth out-and-back trajectory with return flip. PhanQuang uses a stable original fan frame moving from the weapon. Heavy impact waits for the actual heavy attack release at 2.41s; qi ultimate release and weapon offsets aligned. KiemAn stays centered on its target.
- Added GoldVfxOnly shader/material for VanAnh demo. Filters neutral/teal field while retaining gold cuts; rendered CombatPreview14.png inspected with grey rectangle removed. Reaction region remains invisible and checks target positions against its elliptical footprint, including an outside enemy.
- Interactive Unity Play Mode verification passed all 15 scenes, accepting sprite or transform animation for continuously moving effects. Repeatable verifier now resets internal state between runs. Captured and inspected actual rendered effects; live Game View text was reviewed. Detailed subjective timing/pose fit still benefits from user review.

## Character/enemy melee demonstrations — 2026-11-13

- MeleeDemo now instantiates existing Swordsman/Assassin/DaoWarrior/QiSwordsman visual prefabs and four small enemies using the actual Enemy prefab sprite/material. Stage1 uses Swordsman; each passive uses its branch actor; ultimate toolbar selects any of three branches.
- Added MeleeDemoCombat, a deterministic 5.2-second demonstration timeline. Samples original Idle/Run/Attack clips and VFX on one clock. Includes preliminary strikes for passives, stab/retreat, circling weakpoint, heavy windup, approach to break qi marks, dash and landing arc. Enemies flash red/recoil; landing also lifts them briefly. Dash demonstration avoids damage. Replay resets whole encounter and pause freezes actors, effects and enemies together.
- MeleeDemoSetup serializes four character prefab refs, existing enemy sprite/material and combat presenter into saved MeleeDemo scene. No new character or effect artwork. Demo timing and reactions are illustrative choreography; these changes do not implement full combat rules in GameScene.
- Actual Unity Play Mode passed all 15 selections with correct branch actors, four enemies, advancing VFX and six active renderers per encounter. Runtime validation report Temp/MeleeDemoCombatVerification.txt. Visual review of full OnGUI layout remains Needs verification.
- Final graphics-enabled batch Play Mode passed after timing fixes. Rendered and inspected camera-only combat frame docs/player-art/MeleeDemo/CombatPreview.png, showing DaoWarrior, four original spider sprites and circular slash. This render excludes OnGUI.

## Melee demo scene — 2026-11-13

- Added Assets/Scenes/MeleeDemo.unity, runtime MeleeSkillDemo and Editor MeleeDemoSetup. Uses all 15 existing skill visual prefabs (3 opening, 9 branch passives, 3 ultimates), original sprites/controllers/materials. Left scrollable grouped sidebar switches one active prefab; replay/pause/repeat and uniform zoom controls in main area.
- Uses existing OnGUI approach, dark forest-color background and original Point-filtered VFX. Camera viewport reserves header/footer and fits both dimensions. Scene is independent and can be opened directly in Unity and played.
- New artwork was not generated. C# compiled and scene build verified with MELEE DEMO READY for 15 wired prefabs. MeleeDemoVerification added to verify selection replacement and sprite animation in Play Mode.
- Actual headless Unity Play Mode verification passed all 15 selections: each has a non-null sprite, advances animation and leaves exactly one active effect. Log Temp/MeleeDemoPlayMode.log contains MELEE DEMO PLAY MODE PASSED. Headless verification does not assess rendered UI layout.
- Needs verification: manual visual assessment of sidebar/control layout in Unity Game view at the user's display size. Demo displays supplied animation clips; combat conditions and damage remain outside the visual gallery.

## Stage 1 and 2 melee VFX — 2026-11-13

- Added Assets/Prefabs/Player/MeleeSkills with three Stage1 effects (KhienChan, AnhBo, LuoiDaoHoThe) and nine branch passives. Assassin: TamLienThu, AnhHoiKich, AnTuHuyet. DaoWarrior: DiaLietTram, HoanhTao, TrongDao. QiSwordsman: HoiNguyet, PhanQuang, KiemAn.
- Each uses eight transparent 256px frames, PPU48, Point filter and a 0.64-second visual clip. Twelve controllers and visual prefabs generated by MeleeSkillAssetBuilder. Orbiting blades loop; other clips clear sprite at end. Timing chosen for visual preview and requires alignment with combat later.
- Sources generated through built-in imagegen; prompts, extraction script, twelve GIFs and labelled Overview.png in docs/player-art/MeleeSkills. Export cleans neutral haze and cell edges; overview visually inspected and ground-crack haze corrected.
- Verified 96 PNG dimensions, all twelve eight-frame GIFs, 96 frame metas, twelve clips/prefabs. Final Unity 6000.3.25f1 batch compile/import reports twelve sets built successfully and successful batchmode exit.
- Needs verification: gameplay-scale visual review, attachment offsets and real Play Mode. Passive conditions, damage, movement, shield blocking and duration controls require gameplay integration. No gameplay logic added.

## Ultimate video reference revisions — 2026-11-13

- Viewed sampled frames from user videos 165813 (gold-white crossing slashes over teal arena) and 170126 (three long pointed sword lanes in a fan). Updated VanAnh to warm gold ghost/slash traces with teal arena and five-point path. Updated TamThien to pale blue charging lanes transitioning to straight gold-white narrow blades released in a fan. Both retain 16-frame visual clips.
- Built-in imagegen used video frame references. Prompts VanAnh-v2/TamThien-v3 and reference sampling script are in docs/player-art/MeleeUltimates; earlier sheets retained there. Export removes neutral sheet residue for TamThien. GIFs updated. Inspected dark-background composites.
- Needs verification: gameplay-scale human review. Five-ghost physical motion and hold-R variable charge remain preview artwork awaiting gameplay integration. No Play Mode combat test performed for these revisions.
- Unity 6000.3.25f1 compiled/imported with MELEE ULTIMATES READY and return code0 after both revisions.

## TamThien art revision — 2026-11-13

- Replaced TamThien sheet/16 frame PNGs from user-provided gold-white sword fan reference. New blades have pointed ivory edges, gold rims and charcoal trails; transparent fan wash reveals targets. Inspected composited frame on dark background. Updated TamThien.gif and saved v2 prompt/preview in docs/player-art/MeleeUltimates; previous source backed up there.
- Export accepts Names to target one set. Builder has BuildTamThien menu/entry point for isolated import. Character skill triggering and variable charge remain pending gameplay integration.
- Verified Unity 6000.3.25f1 compilation/import completed with MELEE ULTIMATES READY for one set and successful batchmode exit. No gameplay Play Mode test performed.

## Melee ultimate VFX assets — 2026-11-13

- Added Assets/Prefabs/Player/MeleeUltimates with ThienVan (descending fire/impact/circular sweep), TamThien (charge then three-blade filled cone facing +X), VanAnh (five apparitions and progressively drawn pentagram/filled center). Each contains 16 transparent 256px frames. Shared preview timing is 1.28 seconds, PPU48, Point filtering, final sprite cleared.
- Built-in imagegen source sheets and 48 extracted frames retained. Prompts, extraction script and three dark-background GIFs are in docs/player-art/MeleeUltimates. Extraction preserves cell placement with edge feathering to soften source-sheet halo seams.
- MeleeUltimateAssetBuilder creates three one-shot clips/controllers/visual prefabs through Tools > Brotato > Build Melee Ultimate Assets.
- Verified Unity 6000.3.25f1 C# compilation and asset import; log reports MELEE ULTIMATES READY and Exiting batchmode successfully. All three prefabs generated. No gameplay Play Mode test performed.
- Needs verification: visual polish at gameplay scale, character jump/landing and physical five-ghost movement. Current sheet animates star formation with apparitions at vertices. Hold-R charge/release, aiming, boss selection, hit areas, damage, cooldown and invulnerability still require gameplay integration. These are visual asset previews.

## Full qi swordsman visual set — 2026-11-12

- Added Assets/Prefabs/Player/QiSwordsman with 80 transparent 320x320 character PNGs, four 256x256 wave PNGs, 17 clips, two controllers and two visual prefabs. PPU120; character ground y240, center pivot. Per direction Idle4 / Run5 / Attack7 / Death4. Attack lasts 0.78 seconds with suggested wave release at frame03/time0.31.
- Separate right-facing +X QiWave preserves translucent cyan glow and fades over four frames/0.36 seconds; final curve key clears Sprite. Directional rotation and projectile movement/damage are future combat integration. ReleaseDemo.gif is a composited visual illustration of the proposed release/flight timing.
- Built-in imagegen sources follow the approved long-black-hair/teal-robe swordsman. Export normalizes scale/ground, cleans character alpha haze and detached neighbor fragments, and pads wave bounds to preserve soft glow. Prompts, reproducible export scripts and 18 preview GIFs are in docs/player-art/QiSwordsmanFull.
- Unity 6000.3.25f1 batch compiled/imported the builder with QI SWORDSMAN READY and exit0. Verified 80 character/4 wave PNG dimensions, 18 GIF frame counts, no strongly opaque canvas edge pixels, 17 clip assets and both prefabs. Character clip references and first four wave sprite references are checked during construction; final null wave key is intentional. Composite preview inspected and stray neighbor glint removed.
- Needs verification: human assessment of all directional cycles and actual GameScene/Play Mode combat integration. AI-generated poses can retain small drawing variations. Existing gameplay remains at its prior implementation.

## Full heavy curved-saber asset set — 2026-11-12

- Added Assets/Prefabs/Player/DaoWarrior with 84 transparent 320x320 PNGs at PPU120, 16 clips, DaoWarrior.controller and DaoWarriorVisual.prefab. Per direction Down/Left/Right/Up: Idle4 / Run5 / Attack8 / Death4. Body height is normalized near 160 pixels, ground y240, center pivot. Wider canvas accommodates the large curved blade.
- Attack is a 1.10-second weighted windup/diagonal chop/recovery cycle; Run loops at 0.60 seconds. Left Attack frames are mirrored to correct generated source orientation. Alpha haze and small detached specks are cleaned. Attack source rectangles include horizontal padding to preserve the full blade.
- Four imagegen source sheets follow the approved topknot/bronze-armor/red-sash character. Prompts, export script and 16 looping forest GIF previews are in docs/player-art/DaoWarriorFull.
- Checks completed: 84 PNG dimensions, zero strongly opaque pixels on output canvas edges, all 16 GIF frame counts, 84 sprite meta files, 16 clip sprite references and preview prefab. Unity 6000.3.25f1 batch compiled/imported with DAO WARRIOR READY and exit 0.
- Needs verification: human review of complete animation cycles and gameplay combat integration in Play Mode. This delivery provides visual assets and an Animator preview prefab. AI-generated poses can retain small drawing differences.

## Full dagger assassin asset set — 2026-11-12

- Added Assets/Prefabs/Player/Assassin with 72 transparent 256x256 PNG frames at PPU120, 16 clips, Assassin.controller and AssassinVisual.prefab. Per direction Down/Left/Right/Up: Idle4 / Run5 / Attack5 / Death4. Attack is a fast 0.48-second stab/retract cycle. Idle/Run loop; Attack/Death play once.
- Built-in imagegen used the approved silver-haired/purple-scarf assassin reference. Export centers frames, aligns ground, normalizes action scale, removes faint alpha haze and small disconnected specks. Expanded Attack source rectangles preserve the dagger tip crossing nominal cell boundaries. Verified no strongly opaque pixels touch any output canvas edge.
- AssassinAssetBuilder completed in Unity 6000.3.25f1 batch mode with ASSASSIN READY and exit 0. Checked 72 PNG dimensions, 16 GIF frame counts and clip sprite references during construction. Source prompts, reproducible export script and 16 forest GIF previews are under docs/player-art/AssassinFull.
- Visual asset package is ready for preview. AI artwork can retain small proportion/pose differences. Gameplay stab triggering/damage and Play Mode combat integration remain Needs verification. Existing Player/GameScene gameplay is not changed by this visual package.

## Full swordsman asset set — 2026-11-12

- Added Assets/Prefabs/Player/Swordsman with 76 transparent 256x256 PNG frames, PPU120, 16 clips, Swordsman.controller and SwordsmanVisual.prefab. Four directions each have Idle4 / Run5 / Attack6 / Death4. Idle/Run loop, Attack/Death play once. Attack uses reviewed 0.74-second timing. The wider common canvas accommodates the sword arc.
- Built-in imagegen produced four action sheets from approved reference. Export aligns ground and normalizes action scale; left Attack/Death are mirrored to correct wrong-facing generated rows. Alpha haze and small disconnected specks are removed. AI-generated artwork can retain small pose/proportion differences.
- Unity 6000.3.25f1 batch build compiled SwordsmanAssetBuilder and completed with SWORDSMAN READY and exit 0. Verified 76 frame metas, 16 clips and visual prefab. Sixteen looping forest GIFs and generation prompts are under docs/player-art/SwordsmanFull. Play Mode combat integration and human assessment of all animation cycles remain Needs verification.

Swordsman motion review: `docs/player-art/Swordsman/` contains the six right-facing attack poses, aligned PNG frames and a looping forest GIF. One cycle is 0.74 seconds including the recovery hold. GIF decoded with six frames. Timing and silhouette await user review before directional production or Unity integration.

## Melee character art review — 2026-11-12

`docs/player-art/MeleeLineup-v1.png` contains the first eight-pose forest concept sheet. Columns show swordsman, assassin, heavy saber fighter and qi swordsman. Rows show right-facing idle and peak attack. User review of silhouettes and weapons precedes the four Attack_Right motion samples. In-game scale and timing still need Unity verification.

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


## 2026-11-18 · Approved pixel VFX integrated in gun test scene

Replaced the displayed artwork for Lưỡi đạn càn quét, Kíp nổ liên hoàn, Xuyên kích hồi quy, Lưỡi cưa bám đuổi, Hoả lực leo thang, Đạn phân rã, Mỏ neo xuyên tâm and Lồng đạn Hexcore. Sixteen separated runtime atlases are under Assets/Art/GunSkills/PixelSkills; approved previews and source-layer copies are under public/new_asset/pixel-skills-v2. GunNewSkillArt caches the additional layers and GunSkillTest.unity wires them through pixelSkillSheets. Original gameplay damage, triggers, timers, cooldowns and upgrades are preserved; Echo remains on its existing art.

Unity 6000.3.22f1 batch compilation and actual automated Play Mode passed on an isolated copy. GunLabPixelSmoke exercised nine cases: sweep 32, fuse 35 with no damage before the original fuse delay, recall 15+27 with new outbound/return art, saw six ticks totalling 54, overcharge muzzle and heavy-round atlas, decay 8, anchor expiry 21, anchor snap 40, and R-cast cage twelve node shots / six damage pairs totalling 162 (ranks 2 where applicable). Reset disabled remaining visuals; new atlases are referenced and Point filtered. Log: pixel-skills-smoke.log in the session visualization folder. The isolated Editor Search startup exception was excluded by its specific UnityEditor.Search stack; no gun runtime errors occurred. No Windows build or human visual Play Mode review was performed. Main GameScene, packages and project version were not changed by this task.


## 2026-11-18 · Remove rectangular glow from portal and fire breath

Added NoGlowSprite.shader/material under Assets/Art/GunSkills/UpdatedSkills and wired skillNoGlowMaterial in GunSkillTest. Only the two portal SpriteRenderers and Hexcore fire-breath renderer use it. The shader discards source alpha below 0.25 before applying sprite tint/fade, removing the broad low-opacity rectangular glow while retaining higher-opacity fire, portal and particle detail. Original PNGs, gameplay timing, damage and cooldowns are unchanged.

Unity 6000.3.22f1 shader import/compilation and automated Play Mode passed on an isolated copy: both effect types used the filtered material and the existing eight imported-art regressions passed (including five-second tracked fire breath and functioning gates). Log: no-glow-smoke.log in the session visualization folder. GPU-rendered visual confirmation remains for the user in GunSkillTest; the automated run used -nographics.

## 2026-11-25 · Pistol artwork in GunSkillTest

Basic shots and standard Burst rounds use the muzzle flash and aligned moving tracer from public/gun_new/pistol-shots.png. Spread, Piercing and the separate heavy round retain their current art. The isolated Unity 6000.3.22f1 batch Play Mode check passed all four branch selections, including the new muzzle animation in Basic and Burst. Rendering was not visually checked in Game view.

## Shotgun and SMG artwork in GunSkillTest

The branch buttons now say Shotgun and SMG. Each uses its own muzzle sheet and both share the identical bullet-tracer.png from public/gun_new. Shotgun plays one muzzle clip per volley. SMG plays one short non-looping clip per emitted round. Heavy rounds keep their own projectile sprite. Isolated Unity 6000.3.22f1 Play Mode confirmed all four branches, the expected shot and muzzle counts, and no SMG flash remaining during its idle interval. Rendering was not visually checked in Game view.

## Rocket artwork in GunSkillTest

The Piercing branch now appears as Rocket. It uses cannonball muzzle flash and dissipating smoke, with the old cannonball excluded from frames 4–14, and plays only flight frames 25–33 from the existing identical rocket-launcher atlas. Piercing gameplay is unchanged. All four branch art checks passed in isolated Unity batch Play Mode. The 11 cropped sprite frames were also checked against the source PNG. Game view rendering was not visually checked.

## Basic gun skills in GunSkillTest

Replaced Weak Point with Piercing. Penetration reuses the existing projectile hit rule independently of projectile artwork. Rocket retains its native penetration and its own branch upgrades. Ricochet retains its original steering and projectile artwork. Death Scatter keeps its lethal-hit trigger, count and damage, and fragments use the basic pistol tracer animation. Unity batch Play Mode passed normal firing checks for four branches and all 12 branch/skill combinations using controlled hit callbacks.

Rocket penetration now requires the selected Piercing skill. Without it, normal Rocket rounds stop on the first target, and Ricochet and Death Scatter still trigger as selected. Isolated Play Mode checks passed all four Rocket skill selections.

## Rocket branch upgrades in GunSkillTest

Rocket now offers blast, knockback and extra rounds. Blast uses the existing rocket-launcher impact frames with 0.8/1.1/1.4 splash radius and 5/8/11 splash damage to nearby targets, separate from the direct hit. Knockback reuses Shotgun's 0.25 units per rank. Extra rounds fire 1–4 rockets in a single volley with 5 degrees between rounds and one muzzle animation. Isolated Unity Play Mode checked all four ranks for projectile count, target displacement, splash radius/damage and the explosion atlas, plus the existing branch and basic-skill cases.

## SMG branch upgrades in GunSkillTest

The SMG rank slots are Ice Round, Grenade and Extra Ammo. Ice uses cropped ice-lance and frost-breath sprites and slows movement by 15/30/45% for 1.5 seconds. Grenades reuse the identical existing grenade atlas from the ultimate and launch 1–3 rounds only after the final bullet, landing at the current aim point within throw range and doing 18 damage in radius 1.15. Ammo ranks produce 5/7/9/11 rounds. The ammo-linked core modifiers follow the new ammo slot, and modifiers formerly linked to SMG scatter follow the grenade slot. Isolated Unity Play Mode passed four SMG ranks, including grenade timing and damage, ice art, slow/recovery and ammo counts, with existing Rocket and basic skill cases also passing. Game view rendering was not visually reviewed.

Shotgun's first-core Pressure Blast now charges once and fires three fireballs at about 0.16-second intervals. The existing impact damage and armor debuff apply to each fireball. Automated Play Mode verified all three hits and the armor debuff.

Shotgun first-core Sweep uses sword-slash.png (136x104 cells). The opening flash is held until the existing hit window starts. Isolated Play Mode verified sprite playback and the existing 32 damage at rank 2, with no change to the hit logic. Game view visuals were not manually inspected.

Sweep now centers the bright slash on the aiming line, skips the opening glow-only frame and uses the existing alpha-cutoff material with a per-renderer cutoff. Isolated Play Mode verified the sprite pivot, renderer direction, material settings, frame changes and rank-2 damage.

Shotgun first-core slot three is now Fire Breath. Its source fire-breath.png matches the existing imported atlas. It triggers once after a hit on every other volley, using a widened forward cone with 5.4-unit reach and four damage ticks across 0.8 seconds. Isolated Play Mode verified animation, a hit in the added side/range area, no hit outside the cone, and other imported art regressions. Game view was not manually inspected.

The Shotgun Sweep art is counter-rotated to center the authored slash on the firing axis. Its reach is 5.2 and its base half-width is 3.4. Shotgun Fire Breath lasts 1.6 seconds in a 30-degree half-angle cone, spreading existing total damage across eight ticks. Isolated Play Mode verified centered art and in/out targets for both expanded effects. No manual Game view inspection was performed.

Shotgun pursuit saw uses energy-disc.png and makes three revolutions around the target across its 2.4-second lifetime. Shotgun Fire Breath blends three straightened streams into a centered cone while retaining the 30-degree half-angle damage check. Isolated Play Mode confirmed disc animation, orbit count and damage, symmetric flame axes and cone hit boundaries. Game view was not manually inspected.

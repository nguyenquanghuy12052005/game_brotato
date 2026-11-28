# Current project state

## Vòng phối Foley và kiểm tra nguồn (2026-11-28)

- Đã chạy sáu scene nguồn trong bản sao, ghi mẫu 88 cấu hình chiến đấu ở hai tốc độ, và làm lại 20 cue, gồm 10 tuyệt kỹ. Các mục còn lại giữ bản phối thử và ghi tình trạng trong ma trận 146 mục. Chờ nghe duyệt bằng tai. [Bảng đối chiếu](HEXCORE_AUDIO_SPOTTING.md) · [Báo cáo](HEXCORE_AUDIO_QUALITY_REPORT.md)
- Studio đọc bảy take thật cho tuyệt kỹ Gun/Mage, giữ rig Cận chiến thật, thêm lọc nhánh, bản phối trước, loop rời và lưu nhận xét theo phiên bản/biến thể. Gameplay, VFX, animation và scene nguồn được giữ nguyên. [Mở Studio](../Assets/Scenes/AudioPreviewScene.unity)

## Audio Preview Studio trong Unity (2026-11-28)

- Mở [AudioPreviewScene](../Assets/Scenes/AudioPreviewScene.unity) rồi nhấn Play. Có 146 mục trong 10 danh mục, 1.557 WAV tham chiếu từ demo web, lựa chọn trúng/hụt, cue cũ, stem và ba bản nhạc Concept B. Duyệt lưu theo biến thể trên máy. [Cách sử dụng](HEXCORE_UNITY_AUDIO_PREVIEW.md)
- Rig Cận chiến dùng pose và VFX thật, lấy thời gian theo sample WAV. Súng, Pháp sư và Boss dựng minh họa từ sprite/atlas gốc. Các scene hiện có và Build Settings không đổi trong đợt này.
- Unity 6000.3.22f1 biên dịch, lưu scene và chạy Play Mode Direct3D 11 trong bản sao. Đã kiểm tra gọi sân minh họa cho 146 mục, đường dẫn đủ 1.557 WAV, và phát đại diện Cận chiến, Súng, Pháp sư, UI, ambience và nhạc. Timing thủ công từng layer và phối âm trong trận là phần còn cần xác minh.

## Audio Preview HEXCORE (2026-11-28)

- Có demo web cho 146 mục, 311 bản phối, 1.054 stem, 192 bản hụt và 34 mục so với WAV cũ. Nhạc Main Menu Concept B có ba phương án. [Mở demo](../demos/brotato-audio/index.html) · [Kiểm kê](../demos/brotato-audio/INVENTORY.md)
- Bản xuất PCM WAV ngoài Unity, có Foley Kenney CC0 và lớp tái dùng từ WAV dự án. Đã kiểm tra WAV cùng nguồn theo SHA-256, và phát mix/hụt/cue cũ/stem trong Chrome ở demo file cục bộ. Cần xác minh nghe duyệt và timing với gameplay khi tích hợp. [Giấy phép](../demos/brotato-audio/LICENSES.md)

## Đề xuất âm thanh HEXCORE (2026-11-28)

- Kiểm kê 15 WAV Unity, phân biệt cue của GameScene, MainMenu, các sân thử và Boss có tư liệu art. Bản kế hoạch gồm 146 mục cue cùng prompt tiếng Anh, phương án đồng bộ animation/gameplay, mix và tiêu chí nghiệm thu. [Kế hoạch](HEXCORE_AUDIO_SYSTEM_PLAN.md)
- Mẫu PCM nguyên bản dài 16 giây có bảy phác thảo, đặt ngoài Unity Assets. Đã đo thông số WAV và kiểm tra tĩnh. Cần xác minh nghe trong Game View, seam loop và hiệu năng khi tích hợp sau duyệt. [Mẫu nghe](HEXCORE_AUDIO_PREVIEW.wav)

## MainMenu Unity · Đại chiến sinh tồn

- Mở `Assets/Scenes/MainMenu.unity` rồi nhấn Play. Scene dựng giao diện native gồm sáu nút, Chọn màn, Cài đặt và Guide bốn tab. [Scene](../Assets/Scenes/MainMenu.unity)
- Bắt đầu tải `GameScene` vào Forest. Chơi tiếp đang khóa. Tiến độ nhánh và quái trong Guide là dữ liệu minh họa. Tạm dừng/Tiếp tục trong Guide mô phỏng trạng thái tại chỗ. Cấu hình khởi động dự án vẫn vào GameScene.
- Unity 6000.3.22f1 biên dịch thành công và chạy Play Mode với Direct3D 11. Kiểm thử sáu nút, điều hướng bỏ qua Chơi tiếp, bốn tab, cuộn bản đồ, Tạm dừng/Tiếp tục, bộ lọc Boss, trả focus và vào GameScene tại Forest. Đã xem ảnh kết xuất 1920×1080 và 1280×720. Ảnh tại `Temp/MainMenuQA`.

## Concept B · Hướng dẫn và bộ sưu tập demo (2026-11-28)

- Chốt Đại chiến sinh tồn để tiếp tục duyệt bản web. Bản B có luồng vào Forest mẫu, bốn tab Hướng dẫn, 10 nhánh tiến hóa và 14 mục quái. [Mở Concept B](../demos/brotato-main-menu/concept-b.html) · [Mở bản kiểm thử](../demos/brotato-main-menu/concept-b-test.html)
- Mở khóa bằng thao tác mô phỏng chọn nhánh/gặp quái, lưu cục bộ. Công cụ thử và reset chỉ mở ở chế độ kiểm thử. Tổng quan dùng ảnh toàn cảnh bốn vùng dựng từ dữ liệu scene. Điều khiển có nút Tạm dừng/Tiếp tục bấm thử trong HTML.
- Tổng quan dùng lại banner và bố cục bốn thẻ giới thiệu của mẫu trước. Cuộn xuống để xem các thẻ bản đồ đã duyệt.
- Kiểm tra Chrome 1920×1080 và 1280×720, tab và bộ lọc, chi tiết khóa, mở khóa, tải lại, reset, bàn phím và smoke test A/C. [Ảnh và hướng dẫn](../demos/brotato-main-menu/README.md)

## Demo Main Menu BROTATO 2D

- Ba mẫu HTML/CSS/JS cục bộ có tranh riêng, chuyển động chủ cảnh, nhạc và tương tác minh họa cho sáu chức năng. [Mở trang chọn concept](../demos/brotato-main-menu/index.html) · [Hướng dẫn, nhận xét và nguồn tài nguyên](../demos/brotato-main-menu/README.md)
- Kiểm tra Chrome 1920×1080 và 1280×720, trạng thái có hoặc thiếu điểm lưu mẫu, điều hướng chuột/bàn phím và tùy chọn nhạc/chuyển động. Chờ chọn concept trước giai đoạn MainMenu Unity.

## Nâng quy mô ba lõi cuối Cận chiến (2026-11-27)

- Thiên Vẫn Trảm có vụ nổ, nứt đất tỏa nhiều hướng và hai vòng chấn động quanh điểm đáp. Tam Thiên Kiếm Quyết có ba sóng tăng dần chiều dài và bề rộng. Vạn Ảnh Tuyệt Sát có hai lượt bốn dư ảnh cắt chéo lệch nhịp, thêm vệt bóng và vùng kết liễu. Nhân vật dùng pose và tỷ lệ gốc.
- Hai sân thử dùng cùng vùng tác động và quy tắc đếm hit. [Kích thước và sát thương thử](GAME_DESIGN.md#ba-lõi-cuối-cận-chiến-trong-sân-thử).
- Unity 6000.3.22f1 biên dịch và chạy Play Mode trên bản sao vì thư mục chính đang giữ `Temp/UnityLockfile`. Ba chiêu qua 96 tổ hợp form/hướng và ca trúng tâm, rìa, ngoài vùng ở 20/120 fps. Đã xem bốn khung kết xuất Direct3D 11. Ma trận toàn bộ đạt 312/336. Cùng 24 ca ngoài ba chiêu lỗi khi chạy lại mã gốc tại `1ca7933` (ID 02, 06, 21).
- Play Mode trong `CharacterProgressionTest` xác nhận luồng mở lõi và gọi ba chiêu qua bộ điều khiển sân thử. Quái thật trong vùng nhận lần lượt 65/75/83 sát thương, quái ngoài vùng nhận 0. Kiểm tra dùng chênh lệch HP hiện tại và HP tối đa để tính cả cơ chế tăng HP theo thời gian sống.

## Mage merge vào main (2026-11-27)

- Giữ bộ icon lên cấp của Mage và GUID gốc ở `Assets/Resources/Magical/`. Các ghi chú Cận chiến và Mage đã được gộp.
- Roslyn biên dịch qua 42 file runtime và 19 file Editor với các thư viện Unity 6000.3.22f1 có sẵn. Chưa chạy Play Mode bằng Unity 6000.3.25f1 của dự án.

## HEXCORE Cận chiến v2.1 · Sáu chỉnh sửa sau Rev2

- Basic Kiếm khí phóng hai sóng ngắn, Nguyệt Nha đẩy quái thường, Phi Kiếm chạm tối đa năm bia. Ám Bộ quay về quái phía sau và chém hai nhát. Song Ảnh thêm dao găm làn giữa. Hoành Thiên tính trúng theo dải sóng đang bay.
- Roslyn và 152 ca hình học offline PASS. Play Mode, SFX và hồi quy R NOT TESTED vì project chính bị khóa. [So sánh, fixture và kết quả](Melee/HEXCORE_VISUAL_POLISH_REPORT.md)

## HEXCORE Cận chiến v2.1 Rev2

- Rev2 thay đặc tả tại cùng đường dẫn, giữ 24 ID nguồn. Đao F1 dùng 09 cho Bá Vương Đao Vực. Kiếm khí E3 đưa Kiếm Vũ Thiên Hà từ 22 lên, F1 dùng 12 cho Thiên Kiếm Xuyên Tâm. Demo có fixture bia đầu kháng kéo.
- Roslyn runtime/Editor và ma trận tĩnh 336 kỹ năng + 32 Basic PASS. Bộ 50 GIF so sánh hướng Đông là render offline. Play Mode, SFX và hồi quy R NOT TESTED vì project chính bị khóa. [Báo cáo Rev2](Melee/HEXCORE_VISUAL_POLISH_REPORT.md)

## HEXCORE Cận chiến v2.1 · Mẫu VFX (2026-11-26)

- Atlas 12 clip phối từ 10 nguồn PixelFX cho Basic bốn form, P2, Huyết Ấn, Chấn Địa Trảm và Nguyệt Nha Trảm. Gallery có 16 GIF trước–sau hướng Đông offline ở 1× và 0,5×. Debug vùng tác động mặc định tắt.
- Roslyn runtime/Editor và 42 phép thử hình học offline PASS. Đã làm sạch mảng xám tách khỏi thân trên chín PNG Run của Kiếm sĩ. Play Mode, Game View, âm thanh và hồi quy ba lõi NOT TESTED do khóa project. Editor khớp `6000.3.25f1` có sẵn. Chờ duyệt tám mẫu trước khi nhân rộng. [Báo cáo, contact sheet và GIF](Melee/HEXCORE_VISUAL_POLISH_REPORT.md)

## HEXCORE Cận chiến v2.0 (2026-11-26)

- Scene mở tại [Assets/Scenes/HexcoreSkillPreview.unity](../Assets/Scenes/HexcoreSkillPreview.unity).
- Demo hiện đọc slot P/E/R/F từ mapping v2, giữ nguyên 24 ID nguồn. Có đòn thường trong Tự do, sự kiện nhận đòn, bia mô phỏng, vùng tác động E/F và thử nền sáng.
- Roslyn qua runtime/Editor. Ma trận dữ liệu có 336 tổ hợp hợp lệ. Play Mode, ghi hình, hồi quy R bằng camera và lỗi viền xám vẫn chờ Editor tương thích và project rảnh. [Báo cáo v2 và hướng dẫn thử](Melee/HEXCORE_SKILL_REDESIGN_V2_IMPLEMENTATION.md)

## Refactor Cận chiến HEXCORE (2026-11-26)

- Runtime và bốn form chuyển vào Assets/Prefabs/Player/Melee/. Hai file runtime giữ dữ liệu 24 kỹ năng và chức năng trình diễn. Bia đọc sprite Brown hợp lệ. Nguồn, QA và bộ review chuyển về docs/Melee/.
- Xóa demo cũ, VFX đời cũ không còn tham chiếu và clone QA Library/HexcoreValidation. [Danh sách di chuyển/dọn và bảng tra kỹ năng](Melee/HEXCORE_MELEE_CURRENT_STATE.md)
- Roslyn trước và sau đều biên dịch qua. So tĩnh 3.377 GUID, 33 cặp form–skill và 264 hàng hướng. Play Mode trên project chính chưa chạy được vì khóa project và thiếu Editor 6000.3.22f1.

## Mage visual adjustments (2026-11-26)

- The freeze atlas has two vertically stacked ice forms inside each 176×192 cell, which made a single SpriteRenderer look like two columns. `MageEffectLibrary` now crops Freeze frames to the lower 96 px shell. Ice buildup uses one renderer through frames 30–34, grows with each hit, and holds frame 34 during the freeze; no second full-sheet animation is spawned.
- Shockwave now plays only the first 14 frames of its sheet at 45% speed, keeping the single central burst and outward wave while dropping the long particle/ring tail. Gameplay damage and knockback remain one pass per activation.
- Increased Arcane Orb charge/burst scale, slowed its animation to 55%, extended the visible burst, and enlarged/slowed its missiles (scale 1.25, speed 5 units/s). Damage, charge duration and cooldown are unchanged.
- Set the Light Pillar sprite pivot near the bottom of each frame, so the mouse target aligns with the visible base. Damage and radius still use the mouse point.
- Verification: `dotnet build Assembly-CSharp.csproj --no-restore --verbosity:quiet` passed with 0 errors and 27 existing CS0649 warnings in Hexcore preview data. `git diff --check` passed for the seven touched source/documentation files. Needs verification: visual appearance and timing in `Magical.unity` Play Mode; Play Mode was not run.

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

- Branch check before push: the combined `magical` worktree compiled successfully in Unity 6000.3.22f1 batch mode after resolving its declared dependencies. The branch's Unity 6000.3.25f1 version and package lock were preserved. Needs verification: Play Mode in this combined worktree on 6000.3.25f1; the Mage Play Mode regression below ran in the source worktree on 6000.3.22f1.

## 2026-11-11 Mage shots originate at the staff tip

- In `Magical.unity`, `MageAim.CastPoint` now transforms a serialized local X offset of 0.64 on the Staff sprite, matching its visible crystal tip at the current 1.45 scale. MageAim updates before combat. Fireball, Dark missile and Arcane Orb missiles create at this point and face their launch direction on the first frame; their sprite pivots place the visible rear of each shot near the tip. Ice Breath's left edge starts at the tip and keeps following aim during playback.
- `MageProjectile` checks for an enemy at its spawn position before its first movement step and consumes the hit once, covering targets already touching the staff tip. `Tools > Brotato > Apply Mage Preview Scale` saved the new `staffTipOffset` on the Mage prefab; no GameScene scene object was changed.
- Verification: Unity 6000.3.22f1 compiled without C# errors. `Run Mage Regression` passed in real Play Mode, including a measured cast point near the visible staff tip, Fire explosion/burn, Dark curse, Ice freeze, Orb missiles and no runtime errors. Needs verification: visual alignment at every aim angle and every projectile animation frame by human playtest.

## 2026-11-11 Magical training room follow-up

- `Magical.unity` now wires the existing Enemy prefab into `MageDemoHUD` for a configurable training room. The panel can rebuild 1-30 stationary dummies with 1-10000 HP and a 1-6 m spawn radius; default is 7 dummies, 100 HP, 2.2 m. Dummies do not deal contact damage. The panel also has `−25 HP` and `Dùng Q` so Heal can be previewed while test invulnerability is on. Heal still requires missing HP. The radius parser accepts comma or dot decimal separators.
- Reassigned the MageStaff sprite explicitly after importer refresh and enlarged its prefab display scale to 1.45; it is visible in Play Mode. Ice Breath follows the Mage's current cast point and mouse direction while its animation plays. Shockwave grows outward from the Mage. Arcane Orb is small during charge and shows a larger burst on release. Gravity adds inward movement to affected ordinary enemies, including stationary training dummies. One Ice Mark overlay advances through the freeze sheet's formation frames as hits accumulate; the sixth hit plays the full freeze sheet.
- Verification: Unity 6000.3.22f1 recompiled without C# errors. `Run Mage Regression` passed in real Play Mode, including staff sprite/scale, Heal, Shockwave, Gravity damage/slow and measured inward pull of a training dummy, six-hit freeze, ultimate checks and no runtime errors. Manually observed staff, default dummies at 2.2 m and panel `−25 HP` then `Dùng Q` restoring 75/100 to 100/100 with cooldown. Needs verification: physical Q/E key presses, visual timing of each effect while moving/aiming, custom dummy counts at extremes and other screen sizes. Synthetic Q sent to Unity was not observed, so no physical-key pass is claimed.

## 2026-11-11 Mage character and skill preview

- Branch handoff: integrated Mage files onto the existing `magical` branch without replacing its GunSkillTest work. Unity 6000.3.22f1 batch-mode compilation of this combined worktree exited successfully with no C# errors; the earlier Mage Play Mode regression was run on the source project, not repeated in this worktree. The branch retains its declared Unity 6000.3.25f1 project version and package lock. Needs verification: a visual/Play Mode pass in the combined branch on the team's declared Unity version.
- Follow-up visual adjustment: all Mage sheet effects now use a 2.2× display multiplier (previously 1.4×), and the separate Ice Mark overlay uses 0.75 scale (previously 0.5). The staff sprite pivots at its grip (0.38, 0.5); `MageAim` places that grip near the visible hand at local (±0.28, 0.33), rotates it toward the mouse. The earlier 0.43-unit cast offset is superseded by the 0.64 local sprite-tip offset above. Applied with `Tools > Brotato > Apply Mage Preview Scale` in the existing `Magical.unity` scene. This changes presentation and projectile origin, while skill damage, range and hit radii retain their configured values.
- Verification: Unity 6000.3.22f1 compiled after correcting the TextureImporterSettings API; the menu saved the prefab, scene and sprite pivot. Observed Mage and enlarged Fire effect in real Play Mode with aiming on both sides. `Run Mage Regression` passed again in real Play Mode at 21:19 on 2026-11-11, including all skill/ultimate/status checks and no runtime errors. Needs verification: visual alignment for every animation frame/aim angle and other resolutions.
- Earlier preview presentation: Mage visual was set to 1.5×, held staff to 0.65 scale (was 0.45), and Mage VFX to a 1.4× display multiplier (now superseded by 2.2×); collider radius is 0.45. Magical camera already had `CameraFollow` with the Mage as target; orthographic size changed from 5 to 4.5 and the target reference was re-saved. `Tools > Brotato > Apply Mage Preview Scale` applied these values to the existing prefab/scene without rebuilding the scene or changing the saved loadout.
- `MageDemoHUD` now has an in-game **Bảng test skill** panel: select Q, element, E, current branch's three tier values and four cores during Play Mode. Changing Q/E/element clears the relevant preview cooldown. A preview-only invulnerability toggle defaults on so the team can inspect effects without dying; disabling it restores normal one-life damage. **Đặt lại cảnh** reloads the Mage preview and restores its saved Inspector loadout. This panel is for testing, not EXP/level-choice UI.
- Verification: `Run Mage Regression` passed again in real Play Mode, including moving Mage and checking camera follow, 4.5 camera size, enlarged visual, loadout tier/core setters, all earlier skill/status/ultimate checks, and no runtime errors. Manually opened the panel, switched Fire to Ice, increased a tier, checked the opaque panel layout and used **Đặt lại cảnh**; the panel closed and the Mage/enemies reappeared. Needs verification: visual scale on other display resolutions and physical Q/E key presses.

- Added a separate `Assets/Scenes/Magical.unity` preview with `Assets/Prefabs/Player/Mage.prefab`, seven 100-HP spider targets, camera follow and a simple HUD. `GameScene.unity` and its three-wave loop remain unchanged. Mage has four-way movement animation, mouse-aimed staff, one-life HP, Q starter skills (Heal/Shockwave/Gravity), four automatic elemental attacks (Fire/Ice/Light/Dark), all branch tiers/cores as Inspector settings, and E ultimates (TimeStop/StarFall/ArcaneOrb). No EXP, leveling, stage progression or progression skill-selection UI was added. The in-game test panel described above supplements the Mage instance's `MageLoadout` Inspector.
- Copied the 20 current PNGs from `public/player` and `public/skill/Magical` into `Assets/Art/Mage` and added the PixelFX MIT license there. Unity generated `.meta` files. `MagePreviewSetup` configures point-filtered uncompressed imports and builds scene/prefab through `Tools > Brotato > Build Mage Preview`; rerunning it resets the preview instance and its Inspector choices. Three boss prefabs now carry `MageBossMarker`; control effects skip bosses while damage still applies.
- Checks: Unity 6000.3.22f1 compiled the new scripts with no `error CS` in Editor.log. Ran the build menu and actual Play Mode in Magical: Mage appeared, Fire projectiles animated, enemies lost HP/died, contact reduced Mage HP, and the HUD showed health/loadout. `Tools > Brotato > Run Mage Regression` then passed in real Play Mode: all 18 atlas frames loaded; Heal/Shockwave/Gravity, Fire explosion/burn, six-hit Ice freeze, Light/Judgement, Dark curse, TimeStop queued damage, full-charge Orb missiles, five StarFall strikes and boss control immunity/damage were checked. No runtime errors occurred. Detailed local result: ignored `Temp/MageRegression.txt`. Validated all 18 PNG atlas dimensions/frame capacities and confirmed 20 imported PNGs plus license. `git diff --check` reports only the Unity serializer's trailing spaces on three new boss-marker `m_Name:` lines; no source-code whitespace issue. Needs verification: Q/E key activation from physical keyboard, every optional tier/core combination, sprite scale/pivots over different enemies, full balance and Windows build. Synthetic UI key presses were not observed to trigger Q, so no physical-key Play Mode pass is claimed.

## 2026-11-11 asset handoff

- Corrected the planned 20-level Mage stage boundaries on user clarification: stage 1 = levels 1-4, stage 2 = 5-8, stage 3 = 9-14, stage 4 = 15-20. Updated the four labels and their vertical placement in `magicstree.drawio`, plus GAME_DESIGN and M15. Parsed draw.io XML and checked the four ranges; no Unity gameplay or assets changed. Needs verification: visual layout in diagrams.net.

- Updated planned Mage design on user request: `magicstree.drawio` now names 20 levels across four stages, describes the three level-5 Q skills and three level-15 E ultimates with initial values, and changes the Ice branch to breath/6 accepted hits. `docs/GAME_DESIGN.md` records the same rules and specifies an Ice mark taken from formation frames of the existing `freeze-v2` sheet; the earlier shard/3-hit concept is superseded in `docs/DECISIONS.md` M15. Levels 13-14 choose two distinct cores, and levels 19-20 choose the two remaining cores. This is design only; no sprite cropping, Unity import, C# behavior or Play Mode verification was performed. Needs verification: visual fit of longer draw.io labels, exact freeze-sheet frame selection, asset timing/pivots and balance of all new healing/damage/cooldown values.

- Updated `D:/HOC_2026_2027/LTGAME/magicstree.drawio` so both Ice-core nodes (levels 13-14 and 19-20) use the same user-entered `Tụ Hàn` rule instead of the remaining `Băng Thương` entry. The first node had already been edited by the user and was preserved. Parsed the result as valid draw.io XML and confirmed both entries match; no other diagram content was changed by this edit. Recorded the approved rule in GAME_DESIGN/M13. Needs verification: visual text fit in diagrams.net and reconciliation of the tree's Ice breath/six-hit design with GAME_DESIGN's shard/three-hit design. No Unity gameplay or assets changed.

- Follow-up Mage asset audit: `public/skill/Magical/light/smite.png` and `dark/implosion.png` are now present, raising the current folder to 19 PNGs. Both are RGBA with transparency and were visually inspected: `smite` is a descending gold light column/impact candidate for Phán Quyết, while `implosion` is a purple contracting burst candidate for Dày Vò. No matching JSON metadata, Unity import, animation clips, scene wiring or Play Mode check exists yet. The earlier audit gap for these two visual candidates is resolved; the Ice-core replacement is recorded in the later tree update above.

- Read-only Mage skill-tree audit against `D:/HOC_2026_2027/LTGAME/magicstree.drawio` and the current `public/skill/Magical/` folder: 17 PNG candidate assets cover three level-5 starter visuals, Fire/Dark base effects, an Ice breath/freezing candidate, a Light pillar candidate, and all three ultimate concepts. No animation JSON or MIT notice is present in that current folder; the files are outside Unity `Assets/`. `Assets/Scenes/Magical.unity` contains only Main Camera, and no Mage skill, EXP/level-choice or status system is implemented. Gaps for the full diagram include the Ice piercing spike, Light judgement lightning, Dark curse detonation and possibly separate stage visuals for Ice hit accumulation. The diagram labels itself 12 levels but reaches level 20; its level-9 breath/six-hit Ice rule conflicts with the approved level-6 shard/three-hit rule in GAME_DESIGN, and its 13-14 cores repeat at 19-20. Needs verification/decision before implementation: canonical level map and Ice rule, reuse versus separate VFX for upgrade procs, precise sprite-sheet frame metadata and Play Mode appearance. No gameplay files or drawio content changed in this audit.

- Exported a separate PixelFX Burning variant from `D:/PixelFX/pixelfx/src/fx/status/burning-effect-only.js` to `D:/PixelFX/pixelfx/records/status/burning-effect-only/` on user request. The original `burning.js` stays unchanged. The variant keeps four flame hot spots, sparks, smoke and floor glow while removing stock-character body flashes and scorch pixels; render used `--sheet --no-actor --bg none --all`. Visually checked the contact sheet and verified transparent RGBA atlas 504 x 480, 36 nonempty 72 x 80 frames at 30 FPS. Needs verification: alignment over actual enemies in Unity; no Unity import or Play Mode test was performed.

- Exported a separate PixelFX Cursed variant from `D:/PixelFX/pixelfx/src/fx/status/cursed-effect-only.js` to `D:/PixelFX/pixelfx/records/status/cursed-effect-only/` on user request. The variant removes the stock character body, eyes and body-shaped stain while keeping the original smoke trails, ground stain and glitching skull; the original `cursed.js` stays unchanged. Rendered with `--sheet --no-actor --bg none --all`. Verified the contact sheet visually and checked transparent RGBA atlas 640 x 672, 72 nonempty 64 x 84 frames at 30 FPS. Needs verification: alignment and appearance over the actual enemy in Unity; no Unity import or Play Mode test was performed.

- Downloaded original PixelFX Arcane PNG sprite sheets and matching animation JSON for the three selected shared Mage ultimate concepts: `time-stop` (Ngưng Đọng Thời Gian), `star-fall` plus `magic-circle-cast` (Thiên Tinh Giáng Lâm), and `arcane-orb` plus `arcane-missiles` (Thiên Cầu Ma Lực). All files are directly in `public/skill/Magical/`; `time-stop.png` already existed and matched the upstream download byte-for-byte, so it was preserved. Added the upstream MIT text as `PixelFX-LICENSE.txt`. Verified all five PNGs are RGBA, all JSON frame rectangles fit their atlases, and the 109/76/118/72/54 frame counts match the downloads. Some start/end frames are intentionally fully transparent. These are upstream assets, not re-styled to the teammate art, resized to the project's 256-square/30-frame format, imported into Unity, or tested in Play Mode. Needs verification: visual fit, animation timing and Unity import setup.
- Created a static neutral Mage staff export at `public/skill/Magical/MageStaff.png` using Codex host `image_gen` (`art_source=host_image`) with the current dark-robed Mage sprite and teammate VFX art as references. One horizontal right-facing short staff, graphite-brown grip, antique silver details and pale white-violet crystal, usable across Fire/Ice/Light/Dark without painting a fixed element into the weapon. Final export is 128 x 128 RGBA with visible bbox (16,52)-(112,75), transparent outer 8-pixel margin and no animation frames. Reviewed a 6x nearest-neighbor dark-background preview in ignored `Temp/MageStaff_preview.png`. No player/weapon prefab, scene, aim, attack code or gameplay behavior was changed. Needs verification: held scale and pivot alignment with the Mage in Unity Play Mode.
- Rendered fresh GIF previews directly from all 11 current `public/skill/Magical/*_30frames.png` exports at 20 fps on a dark background, saved only as ignored `Temp/MagicalPreview_*.gif`. Also rendered `Temp/MagicalPreview_IceGrowth_3Hits.gif` from the three static growth stages followed by the current freeze loop. Verified each sheet's expected 2560 x 768 grid and each animated GIF's 30 playback frames; the staged growth GIF stores 33 optimized frames with held stage durations. No exported PNG or gameplay file was changed for this preview request.
- Generated `public/skill/Magical/CurseWisp_30frames.png` from the accepted Dark attack visual proposal, replacing and removing the obsolete `DarkRay_30frames.png`. Each frame contains one compact right-facing black-violet smoke seed with luminous amethyst cracks; the same art can be reused for every code-directed target or short transfer hop without painting a fixed number of targets. Export: RGBA 2560 x 768, 30 frames in a 10 x 3 grid of 256-square cells, true alpha. Visual review passed; automated checks: 30 distinct nonempty frames, zero alpha in outer 8-pixel cell borders, and bright head X within 183-185 px across the animation. Temporary dark-background preview: `Temp/CurseWisp_preview.gif`. Recorded the user's three Dark breakthrough options in GAME_DESIGN/M13 with omitted numeric increments left open. Needs verification in Unity Play Mode: apparent scale, flight loop, target-to-target routing and overlay readability. No gameplay code, prefab or scene changed.
- User clarified that future Mage breakthroughs can increase the Dark curse target cap beyond the current three. Recorded the scalable cap in GAME_DESIGN/M13. The Hắc Tiễn projectile is a visual proposal only; no replacement for `DarkRay_30frames.png`, no new asset and no gameplay implementation were requested or made in this clarification. Future art should represent one reusable attack unit without a fixed count of three.
- Rebuilt `public/skill/Magical/CurseDebuff_30frames.png` on user approval of the crack concept. The image-generated loop now consists of thin branching black-violet fissures and moving amethyst glints, with no circular emblem or solid purple patch. Export is RGBA 2560 x 768 (30 frames, 10 x 3 cells of 256 square); only the final PNG is in the Mage export folder. Automated checks: all 30 frames distinct and nonempty, zero alpha on the outer 8-pixel cell borders, and 5.7%-6.9% occupancy within each frame's central 150-square region. Visually reviewed the sheet; temporary dark-background animation preview is `Temp/CurseDebuff_preview.gif`. Needs verification: appearance over actual enemy sprites and loop feel in Unity Play Mode. No gameplay code/scene/prefab changed.
- Rebuilt `public/skill/Magical/BurnDebuff_30frames.png` after user rejected its circular emblem. New image-generated 30-frame loop shows three separate orange flame tongues at the lower/sides of an invisible enemy footprint, small rising embers and an open transparent center; no ring/rune. Export remains RGBA 2560 x 768 (10 x 3 cells, each 256 square) with only the final PNG in the Mage export folder. Visual review passed. Automated checks: 30 distinct nonempty frames, zero alpha at all outer 8-pixel cell borders, stable flame base Y within 215-218 px and outer X bounds varying by at most 8 px. Temporary dark-background preview: `Temp/BurnDebuff_preview.gif`. Needs verification in Unity Play Mode: overlay scale, readability over actual enemy sprites and loop feel. No combat code/scene/prefab changed.
- Replaced the level 6 Ice beam with `public/skill/Magical/IceShardProjectile_30frames.png` and removed the obsolete `IceRay_30frames.png`. The new image-generated source shows one small right-facing faceted shard per frame; the 30-frame export is RGBA 2560 x 768 (10 x 3, 256-square cells) with a stable pivot for code-driven flight. Visual review confirmed one projectile in each frame. Packaging QA: 30 distinct frames, zero alpha on outer 8-pixel cell borders, weighted visible-center range 1.46 px X and 1.89 px Y. The earlier three ice-growth overlays and completed freeze block remain; no code or Unity scene was changed. Needs verification: projectile scale and cadence in Play Mode.
- Revised Ice branch on user's feedback: replaced the circular frozen overlay with a chunky faceted ice block and removed `FrostMark_1/2/3.png`. New `IceGrowth_1/2/3.png` are registered 256 x 256 RGBA overlays for successive hits on the same enemy; stage 3 matches the completed ice silhouette. Regenerated `IceRay_30frames.png` as a straight crystalline lance without a circular emitter and `FreezeDebuff_30frames.png` as a subtle 30-frame completed block loop. All final files are in `public/skill/Magical/`; temporary preview is `Temp/ice_three_hit_preview.gif`. Automated check: both animated sheets are 2560 x 768 with 30 distinct nonempty frames and zero alpha on all outer 8-pixel cell borders; the three static stages are nonempty 256 x 256 RGBA, with visible ice increasing 9037 -> 17964 -> 27959 pixels. Reviewed all sheets and stages visually. Needs verification in Unity Play Mode: overlay scale/visibility on actual enemy sprites and loop feel. No gameplay code, prefab or scene was changed; hit counts and freezing are design only.
- Generated the level 6 Mage VFX requested for Fire, Ice, Light and Dark using the `generate2dsprite` workflow with Codex host `image_gen` (`art_source=host_image`) and the approved Mage/teammate VFX as visual references. Final PNGs only in `public/skill/Magical/`: `FireProjectile`, `FireExplosion`, `BurnDebuff`, `IceRay`, `FreezeDebuff`, `LightPillar`, `DarkRay`, `CurseDebuff` (each `_30frames.png`), plus static `FrostMark_1/2/3.png`. Every animated export is RGBA 2560 x 768, 10 x 3 cells of 256 x 256, read left-to-right/top-to-bottom. Static marks are separate RGBA 96 x 96 icons. The Light pillar sheet has four bright beam pulses separated by dim sigil frames. No source/guide/processed/spill/frames_qa folders were added to the export directory.
- Visual review covered all eight sheets and three frost icons. Automated packaging QA: all eight sheets contain 30 distinct, nonempty frames; maximum alpha in the outer 8-pixel border of every cell is zero. All three static marks have nonempty true alpha. The generated source images and build script remain outside the final export area (`.codex/generated_images` and ignored `Temp/`). No gameplay code, Unity scene, prefab, damage or status logic was changed. Needs verification: animated appearance, scale and beam aiming/extensibility in Unity Play Mode; cost/model metadata was not supplied by the host image tool.
- User supplied level 6 Mage branch mechanics (Fire/Ice/Light/Dark) and requested semantic descriptions plus asset-generation prompts. Recorded exact values and open rules in GAME_DESIGN/M13. The interrupted magic-weapon/projectile generation produced no exported weapon or projectile asset; only a temporary generated weapon draft exists outside the source assets. No level 6 gameplay or scene behavior was implemented or tested.
- User rejected the first three 30-frame mage previews as unattractive. Regenerated and replaced all three `public/skill/Magical/*_30frames.png` sheets from distinct image-generated multi-frame sources with actual `origin/huy` VFX sheets attached as style references. Removed broad generated haze in post-processing while retaining detailed rune/arc frames and soft alpha. Export layout remains 10 x 3 cells of 256 x 256; no persistent source/QA folders were created. Temporary refreshed GIF previews are `Temp/{HealingEffect,ShockwaveEffect,GravityFieldEffect}_v2_preview.gif`.
- Current checks: each export has exactly 30 nonempty frames, 0 pixels above alpha 12 in the 4-pixel border of any frame, and 233/256/256 alpha levels respectively. Shockwave visible widths progress from 18 to 196 px without shrinking. Gravity frame 30-to-7 mean RGB difference on a dark background is 2.954, close to frame 29-to-30 at 2.897. GIFs have 30/30/54 playback frames and durations 1.5/1.5/2.7 seconds at 20 fps. Visually reviewed contacts and selected full-size frames. Needs verification: subjective appearance and scale in Unity Play Mode; no gameplay logic or scene wiring was changed.
- Rendered temporary GIF previews of the three 30-frame mage sheets at 20 frames/s on a dark background for visual review (`Temp/*_animation_preview.gif`, ignored/generated). Healing and Shockwave play once per GIF cycle (1.5 s); Gravity previews activation then one additional stable loop segment (2.75 s). This is an animation preview, not a Unity Play Mode test; export PNGs remain unchanged.
- Restyled all three mage FX on the user's request and expanded each to 30 frames: `public/skill/Magical/{HealingEffect,ShockwaveEffect,GravityFieldEffect}_30frames.png`. Each sheet is 2560 x 768, laid out 10 columns x 3 rows of 256 x 256 RGBA cells. Generated one master still per effect with actual `origin/huy` art attached as style reference, then used image transforms to animate the healing pulse, expanding shockwave and stationary gravity rune with rotating inner trails. Removed the three superseded 8-frame PNGs from the main export folder. No persistent intermediate folders were created.
- Checks: all three sheets have 30 nonempty frames and zero alpha pixels above 8 within four pixels of any cell edge. Soft alpha is preserved (207-252 distinct levels). Gravity's frame 30-to-6 loop seam measured mean RGB difference 0.298 on a dark background, within adjacent loop-step range 0.298-0.304. Visually reviewed samples on dark and light backgrounds. Needs verification: animation feel and scale in Unity Play Mode; no scene or gameplay logic was changed.
- Inspected the user's `huy/Assets/Art` reference through `origin/huy` at commit `60f25635861ebb0d77eed5473f13bad2af424f9e` because the GitHub tree page was blocked by the web reader. Reviewed all 17 PNGs and several frames at full size. Recorded the future mage VFX style in `docs/ART_STYLE.md` and decision M12. Existing mage sheets were not changed; visual consistency of those sheets with the teammate's art remains Needs verification/revision if requested.
- Created `public/skill/Magical/GravityFieldEffect_8frames.png`: one 8-frame indigo/purple gravity rune sheet, 128 x 128 per frame, binary alpha. Frames 1-2 activate; frames 3-8 keep a stable ring for looping. Generated with Codex `image_gen` (`host_image`); only the final PNG was saved for this effect. Manual image checks: all frames nonempty, 0 opaque pixels within four pixels of left/right cell edges, stable-frame outer bboxes about 105 x 106 px, frame 8-to-3 mean RGB difference 6.6 versus adjacent loop-step differences 4.03-11.13. Visual alignment still needs verification in Unity Play Mode; no gameplay logic or scene wiring was added.
- Created `public/skill/Magical/ShockwaveEffect_8frames.png`: one horizontal 8-frame violet-blue shockwave sheet, 128 x 128 per frame, centered expanding ring, binary alpha. Generated with Codex `image_gen` (`host_image`), then aligned/scaled to prevent frame bleed. Checked all 8 frames are nonempty, cell edges clear, and visual bbox centers stay within 0.5 px of the cell center. No Play Mode check or gameplay logic was added. Only the final PNG was saved for this new effect; no intermediate asset folders were created.
- Created a proposed mage healing VFX sprite sheet, now available directly at `public/skill/Magical/HealingEffect_8frames.png` (8 frames, 128 x 128 each). The image was generated with Codex `image_gen`, then cropped, registered, reduced to a shared limited palette, and given binary transparency. No skill gameplay code, scene, prefab, or package was changed.
- Asset checks: raw sheet had 0 crossed cell components and 0 empty cells; processed sheet passed strict geometry QC. Raw sheet warnings were `safe_frame` and `faint_specks`; character anatomy/loop QA is inapplicable to this one-shot FX. See the asset README for values and provenance.
- Needs verification: visual scale and pivot in actual Unity Play Mode. No Unity animation clip or Inspector wiring has been performed.

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
- Tên tiếng Việt đầy đủ theo năm nhóm, vùng HUD/danh sách nằm ngoài viewport camera. Sửa index vòng chạy chéo từ tám về bốn frame của đúng hướng. Vạn Ảnh dùng shader/material riêng của preview để lọc nền xám gần vệt vàng. [Kiểm thử và ảnh đối chiếu](Melee/HEXCORE_ANIMATION_QA.md#đợt-giao-diện-và-điều-khiển).

## HEXCORE · Visual polish và gallery

- [Gallery trước/sau](Melee/HexcoreVisual/index.html) bao phủ 264 tổ hợp, có clip tắt VFX, chuỗi Basic và video ba lõi có audio từ Unity. [CSV hiện tại](Melee/HexcoreVisual/HexcoreVisualReview.csv) ghi 263 AuthoredBlockout, 1 In Review và 0 mục Ready for Review mới.
- Thay 320 frame chéo của ba lõi, bổ sung 16 pose cho Cuồng Phong Đao và 64 điểm đo. Unity 6000.3.25f1 xác minh chuỗi, đổi hướng và reset bằng scene đang có. [Báo cáo polish](Melee/HEXCORE_VISUAL_POLISH_REPORT.md) và [QA](Melee/HEXCORE_ANIMATION_QA.md) ghi các lỗi còn lại và cách chạy lại review.

## HEXCORE · Hoàn thiện phạm vi hướng

- `HexcoreSkillPreview` có dữ liệu 24 kỹ năng, 33 cặp form–skill và 264 hàng có hướng. Q1 Tây Bắc được bổ sung. Q2 quay bằng pose riêng, các hàng Q3/E/F/R được dựng bằng chuỗi theo form và hướng. Bốn form vẫn có Idle/Run/Turn/Basic tám hướng.
- 2.816 PNG, 364 clip pose, 13 WAV. Dữ liệu neo theo frame được thêm cho Q3 ở bảy hướng và R1/R2 N/S trên bốn form. Các kỹ năng mới giữ Blockout trong khi chờ polish và duyệt hình. Q1 E tiếp tục In Review.
- Xem [bàn giao](Melee/HEXCORE_ASSET_PRODUCTION_REPORT.md), [kết quả kiểm thử](Melee/HEXCORE_ANIMATION_QA.md), [checkpoint](Melee/HEXCORE_PRODUCTION_PROGRESS.md) và [bộ GIF](Melee/HexcoreFinal/Review/README.md).

## HEXCORE · Bổ sung hướng và animation · 2026-11-18

- Cả bốn form có Idle/Run, pose Turn và Basic tám hướng. Có 20 chuỗi Q2/Q3/E/F riêng hướng E, Q1 ở 7 hướng và sprite riêng R1–R3 E/W trên bốn form. Hai phương án Q1 NW chưa qua kiểm tra tay kiếm/giáp vai.
- Thêm 13 cue WAV sinh từ nguồn tổng hợp có thể tái tạo. Đo điểm mũi kiếm khi phát QiWave và R2, điểm chạm đất R1 và neo tác động Q1.
- Unity 6000.3.25f1 đã chạy Play Mode trên bản sao. Ma trận có 264 hàng, gồm 135 trình diễn (84 dùng pose nguồn) và 129 thiếu pose. Có 1.140 PNG và 151 clip trong các thư mục HexcorePreview. Các hướng chính chỉ có pose nguồn giữ trạng thái Blockout. Xem [bàn giao và giới hạn](Melee/HEXCORE_ASSET_PRODUCTION_REPORT.md).

## HEXCORE · Preview toàn danh mục · 2026-11-18

- Scene `HexcoreSkillPreview` có 4 form, 24 kỹ năng và 33 tổ hợp tương thích. Có 399 sprite và 69 clip bổ sung trong các thư mục `HexcorePreview`, tính cả 34 sprite của Đợt 1. Repo giữ các prefab và Animator gốc cho hướng chính.
- Có Basic E/W cho bốn form, Q1 E/S/W, pose riêng E cho ba lõi trên bốn form, bốn key pose E cho mỗi Q2/Q3/E/F. 8/7/8/8 hướng locomotion xem được, với Idle chéo giữ pose. Sát thủ SW chưa đạt phối cảnh. Các combo/hướng chưa có pose, Turn chuyên biệt, in-between và polish SFX vẫn cần bổ sung.
- Unity 6000.3.25f1 Play Mode trên bản sao đã qua kiểm tra chuỗi Q1 1×/0,5×, 33 tổ hợp và mốc VFX. Chụp 33 ảnh camera và xuất 24 GIF hướng Đông. Q1 E In Review, các kỹ năng còn lại Blockout. [Báo cáo bàn giao](Melee/HEXCORE_ASSET_PRODUCTION_REPORT.md)

## HEXCORE Đợt 1 · Form 01 · 2026-11-18

- Thêm 34 sprite từ ảnh ImageGen dùng tham chiếu Kiếm sĩ gốc. Scene có Basic hai nhát và bộ Q1 riêng hướng Đông, 4 hướng Idle/Run từ Animator gốc, hai hướng NE/SE với Run mới và Idle giữ một pose.
- Vệt kiếm, bụi chân và tia va chạm đi theo các điểm neo/mốc trên timeline preview. Chuỗi mặc định kết thúc bằng Run sau nhát Basic.
- Unity 6000.3.25f1 Play Mode trên bản sao đã qua bài kiểm tra 1× và 0,5×, mốc chạm, chạy ra, replay và sáu hướng có dữ liệu. Ghi GIF từ 48 ảnh camera Unity.
- Needs verification: designer duyệt chuyển tiếp và chân trụ. Thiếu SW/NW, Idle NE/SE nhiều frame, Basic/Q1 ở các hướng ngoài E và SFX. [Báo cáo Đợt 1](Melee/HEXCORE_FORM01_STAGE1.md)

## HEXCORE Đợt 0 và Q1 blockout · 2026-11-18

- Lưu bản đặc tả đầy đủ và [báo cáo kiểm kê](Melee/HEXCORE_ASSET_AUDIT.md), gồm 4 form, ma trận tương thích ban đầu và yêu cầu sản xuất pose. Kiểm tra tĩnh đã đối chiếu 312 PNG với thông số import, GUID của 64 clip và 24 tiêu đề kỹ năng trong đặc tả.
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

`docs/Melee/Source/MeleeLineup-v1.png` contains the first eight-pose forest concept sheet. Columns show swordsman, assassin, heavy saber fighter and qi swordsman. Rows show right-facing idle and peak attack. User review of silhouettes and weapons precedes the four Attack_Right motion samples. In-game scale and timing still need Unity verification.

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

## Merge of enemy and Mage branches

The enemy AI keeps its attack stopping distance and death animation alongside Mage movement effects, death notifications and player healing. Mage training dummies use the current enemy prefab and have their attack components removed when spawned. Runtime and Editor scripts compiled separately with Roslyn using the installed Unity 6000.3.22f1 references. Play Mode under the project version 6000.3.25f1 still needs verification.

SMG first-core slot three is now Hồi máu. Eight hits on one target summon a slime for five seconds and heal the practice player four HP each second, capped at 100. The ground ring, body hearts and follower use separate cropped atlas ranges with a shared yellow-green/ivory shader palette, coordinated pulse and fade. Isolated Unity Play Mode checked activation threshold, atlas/material bindings, follow after movement, five ticks, expiry and health cap. Game view colors and composition were not visually reviewed.

SMG Hoả lực leo thang now swaps all SMG rounds to plasma for 5 seconds. Plasma inflicts a refreshable 4-second poison that deals 3 damage per half-second and ramps movement slow from 10% to 45%. It combines with ice using the stronger slow and restores speed when poison expires. Only the ground ring slices of poisoned.png are drawn, excluding the baked actor. Unity Play Mode verified the shot window, atlas slices, damage ticks, slow ramp, refresh and cleanup.

Rocket's Portal core links two 125%-scale portals with arcane-beam.png. Portals are placed outside the marked target along the incoming shot direction and last 2.5 seconds. The beam deals 8 damage per 0.25-second tick to enemies intersecting its segment, once per target even with multiple colliders. Piercing projectile transport is preserved. Isolated Play Mode verified portal position and scale, animated beam, repeated line hits, no damage after leaving the beam, transport and expiry.

Rocket's Mỏ neo xuyên tâm uses tesla-coil.png and a cropped chain-lightning.png strip that excludes the three ground markers. Crossing the 2.65-unit tether range triggers thunderclap.png at the coil and stuns the linked enemy for 1.25 seconds, with stunned.png tracking above its head. Remaining in range until expiry still triggers area damage. The final C# sources compiled and isolated Play Mode verified the tether, range break, blast placement, stun, stars and speed recovery. Game view was not manually reviewed.

Rocket Xuyên kích hồi quy now spawns one ball-lightning.png orb on the first hit of each fourth volley. It follows the target for three orbits in 2.4 seconds and applies eight damage ticks of 5 + Rocket rank X at 0.3-second intervals. Ball lightning and Tesla anchor share the cropped chain-lightning strip and link positioning. C# compilation and isolated Play Mode verified the hit gate, moving orb and tether, all damage ticks, and cleanup on expiry and selection change. Game view was not manually reviewed.

Rocket Xuyên kích hồi quy now drifts slowly between random angles and distances near the marked target. Each of the existing eight damage ticks produces a brief jagged strike with three fading branches and a cropped chain-lightning impact accent. The main bolt and branches disappear between ticks. C# compiled, and isolated Unity Play Mode confirmed slow movement, changing distance, branching and gaps, tracking a moving target, damage and cleanup. A rendered frame was visually inspected.

Pháo đài cơ động in GunSkillTest places a stationary Energy Core at the ground aim point. Three can coexist, each lasting 15 seconds with a plasma pulse on placement and every 0.5 seconds afterward. Each pulse deals 11 damage once to each target within radius 4. The 2.4-second deployment cooldown is unchanged. C# compiled and isolated Play Mode verified atlas animation, placement, radius, cadence, all 30 pulses, the deployment cap, expiry and reset. Game View was not manually inspected.

Lốc lửa replaces Ngũ bom liên hoàn in GunSkillTest. R charges still cap at three and recharge one per 2.4 seconds. A fire-tornado.png column advances on its initial heading at 1.1 units per second for seven seconds. It pulls enemies within 1.8 units toward its foot, with reduced pull on bosses, and deals six damage each half second. C# compiled and isolated Play Mode verified the atlas, direction, travel, pull, damage, expiry, recharge, and continued availability of the SMG grenade sheet. Game View was not manually inspected.

Lốc lửa now uses the shared low-alpha filter to remove the baked halo. Compared two atlas frames with the material cutoff and verified the binding and existing tornado behavior in isolated Play Mode. The column is rendered at double width and height around its ground pivot.

GunSkillTest now offers four Hexcore ultimates after removing Phun lửa Hexcore. The Shotgun Phun lửa core remains available. Lốc lửa keeps its control name, mechanics, double scale and glow filter, and now uses blizzard-vortex.png with a ground pivot. C# compilation and isolated Play Mode verified the art binding, remaining menu options, movement, pull, damage, expiry and recharge. Game View was not manually inspected.

The blizzard vortex runtime sprites crop the bottom 15 pixels of each cell to remove the horizontal ground streak. The source atlas stays intact. Checked the strip across all 60 used frames and verified sprite cropping and the ground pivot in isolated Play Mode.

## Mage level-up implementation (2026-11-26)

Implemented the 20-level Mage progression in `Magical.unity` only. XP thresholds are 10 + 2 per prior level; excess XP carries over, and level 2 grants the Mage's 30% XP bonus. A normal enemy awards `ceil(max HP × 0.25)` XP (3/5/8 for HP 10/20/30); training dummies award none. Choice modal pauses gameplay and resumes after the pending choices are resolved. Default offers show three distinct choices; level 9 offers four elemental choices. No duplicate options are inserted when a pool has fewer choices. The existing Mage icon atlas is used. Editor quick-test controls are available. C# build passed with 0 errors. Play Mode visually confirmed level 2 XP bonus and the three-choice modal/atlas crop before the final icon-size adjustment; final sizing and full four-choice level-9 flow still need Play Mode verification. Stat increments are provisional because `magicstree.drawio` was absent from this checkout.

## Mage level-up UI polish (2026-11-26)

- The source atlas has overlapping artwork between cells, so per-cell UV cropping could not guarantee clean edges. Added `Assets/Resources/Magical/MageLevelUpIconsClean.png`: a 4×8 atlas of individually extracted, centered icons on transparent 160×160 cells. The preview keeps the original atlas as source and loads only the clean atlas for level-up cards. Its importer uses point filtering and no texture compression.
- Rebalanced the choice modal to a shorter centered panel with consistent margins, a clearer header divider, more compact cards, a stronger cyan action button and a darker background overlay so the preview HUD does not compete with the choices. Play Mode verified the clean atlas at 3-card stat and 4-card element layouts. Icons are centered, distinct and free of neighboring fragments; choosing a stat and the Light element applied successfully and resumed gameplay. Needs verification: layout at a narrow Game View size.

Sân thử ba hệ CharacterProgressionTest có nút Thử lên cấp. Bắt đầu bằng súng cơ bản; cấp 2 chọn hệ và chuyên nhánh, rồi lần lượt bị động, Q, E và R tới cấp 6. Kiểm thử ngoài Unity xác nhận thứ tự, khóa phím, thẻ Pháp sư đủ bốn nguyên tố và luồng của cả 10 chuyên nhánh. Unity runtime và Editor biên dịch qua Roslyn. Chưa chạy Play Mode vì dự án đang mở trong Unity.

CharacterProgressionTest hiện chỉ lên cấp qua nút Thử lên cấp. Menu Chọn quái tập dùng prefab, cấu hình HP và AI boss từ GunSkillTest cho cả ba hệ. Cận chiến giữ clip đã duyệt, lấy hướng và điểm ngắm từ chuột, kiểm tra vùng đánh trên EnemyHealth đang hoạt động và để quái hiển thị qua các nhịp tung chiêu. Roslyn runtime/Editor và kiểm thử ngoài Unity cho 10 chuyên nhánh đã qua. Chưa chạy Play Mode vì project đang bị khóa bởi Unity.

Trong CharacterProgressionTest, camera Cận chiến bám vị trí thật của nhân vật ở orthographic size 5, không dùng góc quay và bố cục dựng sẵn của clip. Cận chiến tự đánh mục tiêu còn sống gần nhất trong tầm, giữ điều khiển di chuyển trong đòn thường. Q/E/R có thể ngắt đòn thường. Roslyn runtime và Editor biên dịch qua. Kiểm thử ngoài Unity xác nhận 400 khung mô phỏng camera khi quay và dùng chiêu, giới hạn tầm đánh và chọn mục tiêu. Chưa chạy Play Mode vì dự án đang mở trong Unity.

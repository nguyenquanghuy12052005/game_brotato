# HEXCORE Cận chiến v2.0 · Triển khai và nghiệm thu

Đối chiếu [đặc tả v2 đã duyệt](HEXCORE_SKILL_REDESIGN_V2_APPROVED.md) với [bản v1](HEXCORE_SKILL_ASSET_DEMO_PLAN.md). Văn bản v1 trong repo trùng nội dung với tệp đính kèm sau khi chuẩn hóa xuống dòng. Áp dụng v2 cho cơ chế, slot và phạm vi; giữ yêu cầu về tạo hình, animation, SFX, nguồn và QA. Đặc tả được duyệt, bản triển khai demo vẫn chờ kiểm tra Play Mode và duyệt hình.

## Mốc trước triển khai

- [Hiện trạng refactor](HEXCORE_MELEE_CURRENT_STATE.md) và [gallery Unity v1](HexcoreVisual/index.html) là mốc so sánh. V1 có 24 ID nguồn, 33 cặp form–skill và 264 tổ hợp hướng.
- Bốn form thật trong `Assets/Prefabs/Player/Melee/` có Animator nguồn, Idle/Run/Turn/Basic tám hướng và phong cách Basic riêng. Có 2.816 PNG và 364 clip trong các thư mục pose preview. Bốn kỹ năng thuộc hai cặp đổi slot tiếp tục tra clip và sprite theo ID nguồn và form. P dùng Basic hoặc overlay theo sự kiện nên mở thêm 72 tổ hợp hướng trên ba chuyên nhánh mà không nhân bản clip Q.
- Cấu trúc trường serialize của [scene](../../Assets/Scenes/HexcoreSkillPreview.unity), thứ tự catalog, tên asset và GUID giữ theo mốc refactor. Điểm vào scene gắn controller sẵn, nên giao diện v2 được cập nhật qua script hiện có.

## Bảng nguồn → v2.0

Đơn vị `u` là world unit mô phỏng. Chiều cao alpha ≥ 128 của pose Idle hướng Nam là 160–161 px tại 120 PPU, khoảng 1,33–1,34 u. Các con số trong bảng là fixture để đọc phạm vi và thử trên bia; chưa chốt cân bằng, sát thương, cooldown hoặc cấp mở khóa cho game. Quạt ghi góc mở toàn phần; hình tròn ghi bán kính, đường kính bằng hai lần bán kính.

| ID nguồn | Skill nguồn v1 | Slot và tên v2 | Nhánh | Cơ chế đang cài đặt trong preview |
|---|---|---|---|---|
| 01 | Q1 · Kiếm Lướt / Phong Ảnh Trảm | P1 · Phong Ảnh Trảm | Dùng chung | Phóng đường kiếm sau 5 đòn thường trúng. Hành lang dài 2,65 u, nửa rộng 0,58 u. |
| 02 | Q2 · Hoành Nguyệt Trảm | P2 · Hoành Nguyệt Trảm | Dùng chung | Vòng 360° sau 4 đòn thường trúng. Bán kính 1,30 u. |
| 03 | Q3 · Phản Kiếm Quyết | P3 · Phản Kiếm Quyết | Dùng chung | Nhận đòn → giảm HP mất và phản kiếm; hồi nội bộ mô phỏng 2 s, hành lang dài 2,65 u. |
| 04 | E1 · Ám Bộ Trảm | E1 · Ám Bộ Trảm | Sát thủ | Lướt 3,15 u; hai mốc trúng trên hành lang 3,85 × 1,86 u. |
| 05 | E2 · Huyết Ấn | E2 · Huyết Ấn | Sát thủ | Chọn tối đa 5 bia trong bán kính 1,70 u quanh điểm ngắm; nổ bán kính 1,12 u mỗi ấn, gộp trúng chồng một lần. |
| 06 | E3 · Tàng Ảnh | F1 · Tàng Ảnh | Sát thủ | Khói phủ đoạn tái định vị 2,40 u; một quạt phục kích tầm 2,90 u. |
| 07 | E1 · Chấn Địa Trảm | E1 · Chấn Địa Trảm | Đao sĩ | Bổ tại chỗ vào điểm chạm đất; bán kính 1,85 u và choáng mô phỏng 0,45 s. |
| 08 | E2 · Hoành Thiên Đao | E2 · Hoành Thiên Đao | Đao sĩ | Chém quạt 114°, tầm 3,90 u; đẩy bia trúng 0,48 u. |
| 09 | E3 · Bất Động Chiến Thế | F1 · Bất Động Chiến Thế | Đao sĩ | Vòng xung kích bán kính 2,65 u, aura phòng thủ 5 s mô phỏng. |
| 10 | E1 · Nguyệt Nha Trảm | E1 · Nguyệt Nha Trảm | Kiếm khí | Sóng đơn xuyên qua nhiều bia, quãng bay 4,10 u; bán kính va chạm lấy mẫu 0,86 u. |
| 11 | E2 · Lưỡng Nghi Kiếm | E2 · Lưỡng Nghi Kiếm | Kiếm khí | Hai vệt chém giao phía trước, nổ tại tâm ngắm bán kính 1,52 u. |
| 12 | E3 · Kiếm Giới | E3 · Kiếm Giới | Kiếm khí | Kiếm trận cố định tại điểm ngắm, bán kính 2 u, hai nhịp sát thương mô phỏng. |
| 13 | R1 · Thiên Vẫn Trảm | R1 · Thiên Vẫn Trảm | Dùng chung | Lõi R1 dùng tập jump/dive/land, điểm neo và VFX hiện có. |
| 14 | R2 · Tam Thiên Kiếm Quyết | R2 · Tam Thiên Kiếm Quyết | Dùng chung | Lõi R2 dùng tụ lực và ba nhát, các wave và hit-stop hiện có. |
| 15 | R3 · Vạn Ảnh Tuyệt Sát | R3 · Vạn Ảnh Tuyệt Sát | Dùng chung | Lõi R3 dùng chuỗi biến mất/phân thân/tái xuất hiện có. |
| 16 | F1 · Dạ Vũ Liên Trảm | E3 · Dạ Vũ Liên Trảm | Sát thủ | Ba đoạn lao của nhân vật, hành lang riêng rộng 1,96 u; giữ ba cặp nhịp swing/hit nguồn. |
| 17 | F2 · Đoạt Mệnh | F2 · Đoạt Mệnh | Sát thủ | Một đòn xuyên hành lang 5,70 × 1,72 u; hệ số thưởng cho bia còn tối đa 35 HP mô phỏng. |
| 18 | F3 · Song Ảnh Truy Sát | F3 · Song Ảnh Truy Sát | Sát thủ | Hai làn phân thân dài khoảng 5,4 u, lệch hai bên khoảng 1 u; nhân vật chính vẫn hiện diện. |
| 19 | F1 · Cuồng Phong Đao | E3 · Cuồng Phong Đao | Đao sĩ | Hai vòng xoay dùng atlas Cuồng Phong; bán kính mỗi nhịp 2,20 u. |
| 20 | F2 · Đoạn Sơn Trảm | F2 · Đoạn Sơn Trảm | Đao sĩ | Bổ dọc, nứt dọc hành lang 6,30 × 1,76 u; điểm nhấn tại pha Land. |
| 21 | F3 · Bất Diệt Chiến Ý | F3 · Bất Diệt Chiến Ý | Đao sĩ | Cường hóa mô phỏng 5 s; mỗi đòn thường tạo xung kích theo đường 5,30 × 2 u. |
| 22 | F1 · Kiếm Vũ Thiên Hà | F1 · Kiếm Vũ Thiên Hà | Kiếm khí | Hai đợt kiếm rơi ở tâm ngắm; bán kính 2,75 u. |
| 23 | F2 · Phi Kiếm Truy Hồn | F2 · Phi Kiếm Truy Hồn | Kiếm khí | Phi kiếm uốn tới bia trước mặt, tối đa 7 u để tìm đích; giới hạn một va chạm. |
| 24 | F3 · Hồi Nguyệt Kiếm | F3 · Hồi Nguyệt Kiếm | Kiếm khí | Sóng đi 6,10 u rồi quay về kiếm; mỗi bia tối đa một lần mỗi lượt. |

## Asset và thay đổi triển khai

| Thành phần | Xử lý |
|---|---|
| 4 prefab form, Animator, clip, sprite, target renderer | Dùng lại. Đã đối chiếu blob Git của 3.244 PNG, 439 clip, 15 controller, 15 prefab từ bảng di chuyển. Không đổi silhouette, pivot, file PNG hoặc metadata tương ứng. |
| Q1/Q2/Q3 nguồn | Giữ nguyên clip, mốc và đường dẫn để dựng v1. V2 dùng overlay kiếm/vòng/phản đòn, cue WAV hiện có và sự kiện Basic/nhận đòn. Không khởi chạy animation Q nguồn làm bị động. |
| E/F | Pose, mốc pha và anchor dùng nguồn tương ứng. Điều chỉnh riêng hướng/travel nhịp lướt, bán kính, điểm đặt ấn, quỹ đạo, diễn hình vùng đất/đao/sóng, vị trí phân thân và thời điểm hit. Dùng lại Hoành Tảo, Ấn Tử Huyết, Trọng Đao, Địa Liệt, Tam Liên, Kiếm Ấn, Phân Quang, Hồi Nguyệt và QiWave ở những chiêu đã gán. |
| R1–R3 | Dùng nguyên catalog, timing, prefab VFX, clip theo form/hướng và vật liệu gốc. Trình diễn R tiếp tục dùng một bia hiệu ứng gốc. Các vùng mô phỏng mới dành cho Basic, P/E/F. |
| SFX | 13 WAV khớp blob Git. Những pha cũ dùng cue cũ; P dùng cue Wave/Parry, đòn Basic cường hóa dùng Wave. |
| Bổ sung | Bia mô phỏng lấy sprite/material của bia Brown, bộ vẽ vùng và VFX đường dùng material LineRenderer sẵn có. Không thêm texture, prefab VFX hoặc file âm thanh ngoài. Không có asset PixelFX được nhập trong đợt này. |
| Code | [MeleeController.cs](../../Assets/Prefabs/Player/Melee/Scripts/MeleeController.cs) quản lý nút, queue Basic ↔ Skill, camera và chuyển động. [MeleeSkillController.cs](../../Assets/Prefabs/Player/Melee/Scripts/MeleeSkillController.cs) giữ catalog nguồn và triển khai va chạm/phạm vi, bia, các điều kiện bị động và mốc pha v2. [Công cụ Editor v2](../../Assets/Prefabs/Player/Melee/Editor/HexcoreSkillPreviewSetup.V2.cs) dùng lại camera capture hiện có. |

## Viền xám và chuyển tiếp

Đã kiểm tra pixel alpha của **88 PNG Kiếm sĩ** thuộc Idle/Run bốn hướng chính, Locomotion/IdleLoops chéo và Turn. Các meta mẫu dùng Point filter, alpha transparency, không nén ở DefaultTexturePlatform; override Standalone của mẫu tắt. Phép dò pixel gần trung tính bán trong suốt cách vùng alpha ≥ 220 trên 2 px tìm ứng viên ở 19/20 frame Run chính hướng, tối đa 15 pixel/frame, không tìm thấy ở các nhóm Idle, Locomotion, IdleLoops và Turn. Mẫu pixel nằm liền cạnh nét bán trong suốt của sprite, chưa đủ căn cứ phân biệt viền lỗi với chi tiết hợp lệ. Chưa tái hiện Game View nên chưa xác nhận nguyên nhân và chưa sửa texture/shader. Nút **Nền sáng kiểm tra viền** đổi nền camera khi thử tám hướng và đổi hướng để so với nền tối.

Luồng preview tách hướng chạy và hướng đánh khi bỏ chọn **Ngắm theo hướng chạy**. Basic có pha nối combo từ input J. Space trong lúc Basic xếp lượt dùng skill sau combo; J trong lúc dùng chiêu chủ động xếp Basic ở điểm kết thúc. Khởi động lại tiến độ Idle/Run khi vận tốc đi qua ngưỡng để giảm đổi pose giữa chu kỳ. Mức độ trượt chân, độ liền mạch, pivot và tác động camera vẫn cần kiểm tra trực quan ở 1× và 0,5×.

## Mở scene và kiểm thử

1. Mở [HexcoreSkillPreview.unity](../../Assets/Scenes/HexcoreSkillPreview.unity). Chọn tạo hình, chuyên nhánh và một slot trong từng nhóm P/E/R/F. Khi mở một chiêu E/F, demo dùng form có animation tương thích. P/R thử được trên cả bốn form.
2. **Tự do:** WASD/mũi tên đi tám hướng, J đánh thường, Space thử kỹ năng đang chọn, H mô phỏng nhận đòn, R reset. Với nhóm P, Space dùng đòn thường để tích điều kiện; P3 thử bằng H. Chọn 0/1/5/9 bia và dùng **Đặt lại bia theo hướng ngắm**. Thanh điểm ngắm điều khiển tâm các chiêu đất khi phát lần kế tiếp. **Ẩn bia gần nhất** dùng để thử mục tiêu mất trong khi thi triển. Bia màu khác biểu thị HP thấp, flash biểu thị trúng và màu xanh nhạt biểu thị choáng.
3. **Trình diễn:** thử Đứng/Chạy/Quay/Đánh thường/Kỹ năng/Chuỗi đòn; phát lại, 1×/0,5×, tua, vùng tác động, VFX/SFX, rung và dừng va chạm. Phát lại trong Trình diễn đặt lại bia của lượt mới. Bộ đếm bị động giữ giữa các lượt; **Reset mô phỏng** xóa bộ đếm và hiệu ứng đang có.

Khi project chính rảnh và có Editor tương thích, chạy `-executeMethod HexcoreSkillPreviewSetup.VerifyV2` để lấy mẫu timeline và kiểm tra điều kiện bị động, va chạm mô phỏng, cleanup và đối chiếu pose/displacement/phân thân R. Cờ `-hexcoreV2Capture` ghi PNG camera 16 fps cho 24 cặp đại diện ở `Library/HexcoreV2/{Before,After}/<ID nguồn>`. `Before` lấy lại hành vi v1 từ cùng asset nguồn. Dùng chuỗi PNG ở 16 fps hoặc 8 fps để xuất GIF/MP4 bằng công cụ encode sẵn có. Đây là capture camera theo timeline; cần chạy tương tác riêng để nghe mix thật, xác minh keyboard, slow playback, pause/replay và đánh giá thẩm mỹ. `HexcoreSkillPreviewSetup.VerifyExisting` vẫn là kiểm thử hồi quy v1.

## Checklist nghiệm thu

| Kiểm tra | Kết quả | Bằng chứng |
|---|---|---|
| 24 mapping, 4 form, 8 hướng và 336 tổ hợp v2 (96 P, 72 E, 96 R, 72 F) | PASS | [Ma trận dữ liệu](HEXCORE_V2_TEST_MATRIX.csv) khớp mapping và các fixture pose/vùng hiện có; cột Play Mode/visual/capture được ghi riêng. |
| C# runtime và Editor | PASS | Roslyn với bộ assembly Unity hiện có biên dịch 40 file runtime và 19 file Editor, exit 0. |
| Nguồn animation/VFX/R và cấu hình project | PASS | 24 entry và bảng timing khớp Git. Đối chiếu blob của các asset nhập và SHA-256 29 file ProjectSettings/Packages trước–sau. |
| Điều kiện P, đánh nhiều bia, tối đa 5 ấn, chống chồng lấn, hai lượt Hồi Nguyệt, kiểm tra bia biến mất | NOT TESTED | Mã mô phỏng và runner đã có. Chưa chạy Unity. |
| Chuyển tiếp, 8 hướng, chân trụ, khả năng đọc phạm vi, HUD, camera, VFX/SFX, giá trị thủ/cường hóa | NOT TESTED | Chưa có Game View hoặc âm thanh phát trực tiếp từ bản v2. |
| Hồi quy 96 tổ hợp R trong Unity | NOT TESTED | Có phép so sánh baseline ở runner. Mốc hình v1 còn trong gallery. |
| Sửa viền xám Kiếm sĩ | NOT TESTED | Kiểm tra PNG hoàn tất. Chưa có nguyên nhân tái hiện trong Unity; cần so nền sáng/tối và ảnh frame lỗi trước khi sửa asset. |
| GIF/MP4 từ Unity và hình trước/sau của v2 | NOT TESTED | Chưa có capture mới. Bộ review v1 lưu tại gallery và không gắn nhãn là v2. |

### Giới hạn môi trường

Project chính yêu cầu Unity **6000.3.22f1**. Môi trường ghi nhận **6000.3.25f1** và **6000.6.0f1**. Kiểm tra chỉ đọc độc quyền báo khóa `Temp/UnityLockfile` đang được tiến trình khác giữ. Dừng bước Play Mode và capture theo quy định của dự án. Không đổi Editor của project, package, cấu hình hay khóa. Không tạo project sao. Các kết quả PASS ở trên chỉ áp dụng cho kiểm tra dữ liệu và biên dịch đã thực hiện.

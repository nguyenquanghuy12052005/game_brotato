# HEXCORE — ĐẶC TẢ COMBAT & VISUAL POLISH v2.1 · REV 2

> **Trạng thái thiết kế: APPROVED + cập nhật phản hồi ngày 26/11/2026.** 14 mục gốc đã duyệt; phương án Đao sĩ mới đã được chủ dự án chốt: thay Bất Động Chiến Thế bằng Bá Vương Đao Vực, giữ Bất Diệt Chiến Ý.
> Bản REV 2 thay thế đặc tả v2.1 trước đó; không phải xác nhận chất lượng VFX hoặc Play Mode đã đạt. Các thông số kỹ thuật đề xuất vẫn là fixture demo.
> Đây là đặc tả được duyệt để triển khai, không phải xác nhận sản phẩm đã hoàn thành hoặc đạt QA.
> Phạm vi: hệ Cận chiến HEXCORE, Skill Preview Scene; Unity 2D pixel art top-down. Không tích hợp gameplay production trong đợt này.

## 1. Cơ sở và thứ tự ưu tiên

Đọc `Assets/Prefabs/Player/Melee/AGENTS.md`, AGENTS.md ở Git root và kiểm tra repository hiện tại trước khi sửa. Tài liệu tham chiếu trong `docs/Melee/`:

1. `HEXCORE_SKILL_VISUAL_POLISH_V2_1.md` (REV 2, tài liệu này): ưu tiên cao nhất đối với thay đổi thiết kế v2.1; thay nội dung bản cũ tại cùng đường dẫn.
2. `HEXCORE_SKILL_REDESIGN_V2_APPROVED.md`: giữ các quy tắc v2.0 không bị thay thế.
3. `HEXCORE_SKILL_REDESIGN_V2_IMPLEMENTATION.md`, `HEXCORE_V2_TEST_MATRIX.csv`: baseline triển khai và kết quả kiểm tra v2.0.
4. `HEXCORE_MELEE_CURRENT_STATE.md`, `HEXCORE_SKILL_ASSET_DEMO_PLAN.md`, `HEXCORE_VISUAL_POLISH_REPORT.md` mới nhất, báo cáo QA và gallery hiện có: tra cứu nguồn, đường dẫn, hoạt động và lịch sử. Phản hồi trực tiếp mới nhất của chủ dự án được tích hợp tại các mục 6–10.

Nếu đường dẫn trong báo cáo cũ khác checkout hiện tại, dùng đường dẫn xác minh từ repo. Không tự tạo một hệ mới song song với hệ hiện có.

### 1.1. Mục tiêu

Cơ chế và phân bổ v2.0 cơ bản đúng ý, nhưng hiệu ứng còn đơn giản, chuyển động chưa mượt, phạm vi sát thương và hình ảnh chưa khớp, một số chiêu giống nhau. Nâng cấp toàn bộ đánh thường và 24 skill, đặc biệt P/E/F; giữ thiết kế 3 lõi R. Chất lượng quan trọng hơn số lượng sprite. Tái sử dụng animation, VFX, SFX và code phù hợp; phát triển asset mới từ PixelFX khi cần để đạt chất lượng thực sự, không chỉ phóng to/đổi màu hiệu ứng cũ.

### 1.2. Baseline và giới hạn

Báo cáo v2.0 ghi nhận 24 ID nguồn, 336 tổ hợp form–skill–hướng (96 P + 72 E + 96 R + 72 F), C# runtime/Editor compile thành công. Báo cáo vòng tinh chỉnh 8 mẫu v2.1 ghi nhận 10 hiệu ứng PixelFX chọn lọc, 16 GIF trước–sau offline hướng Đông, 42 phép kiểm tra hình học offline và Roslyn PASS; Play Mode, SFX, điểm neo động, tám hướng và 96 tổ hợp R vẫn NOT TESTED. Đã xử lý một nguồn viền xám: 17 mảng rời / 9 PNG Run / 11.165 pixel alpha, chưa xác minh sau import.

Báo cáo gần nhất ghi project và Editor 6000.3.25f1, khác mốc 6000.3.22f1 ban đầu. Đọc `ProjectSettings/ProjectVersion.txt`, Git diff và Editor thực tế để xác minh, không tự đổi phiên bản hoặc revert thay đổi của người dùng. Khóa project được báo cáo ở mốc trước, không giả định hiện vẫn khóa.

Phản hồi mới của chủ dự án cho thấy nhiều E/F vẫn chưa đúng hoặc chưa triển khai đủ; kiểm tra runtime hiện tại thay vì suy đoán từ tên skill hay gallery cũ. Không lấy PASS tĩnh hoặc hình offline làm PASS trực quan. Các ảnh tĩnh đã gửi cho thấy cần chú ý điểm neo Basic, phối cảnh vòng P2/Chấn Địa và dấu ấn chồng lấn; chưa có GIF thực tế trong cuộc trao đổi để đánh giá chuyển động.

## 2. Quyết định đã duyệt

| ID | Nội dung |
|---|---|
| D01 | Phong cách VFX riêng cho 4 tạo hình |
| D02 | Mặc định ẩn vùng debug; đồng bộ hitbox với hiệu ứng |
| D03 | Nâng cấp toàn bộ đánh thường và 3 bị động |
| D04 | Huyết Ấn gắn trên từng quái rồi đồng loạt nổ |
| D05 | Tăng tầm Tàng Ảnh, làm lại hiệu ứng Đoạt Mệnh |
| D06 | Song Ảnh Truy Sát cho hai phân thân lao chém hai bên, đồng nhịp với dao găm xuyên ở giữa của bản thể |
| D07 (thay thế) | F1 Đao sĩ mới Bá Vương Đao Vực (gom quái rồi quét đao); giữ F3 Bất Diệt Chiến Ý. Bất Động Chiến Thế không còn là lựa chọn độc lập, bảo toàn nguồn. |
| D08 | Lưỡng Nghi Kiếm chỉ còn hai đường kiếm hội tụ |
| D09 | Kiếm Vũ Thiên Hà lên E3, kế thừa asset Kiếm Giới |
| D10 | F1 mới Thiên Kiếm Xuyên Tâm chuyên đánh đơn/boss |
| D11 | Phi Kiếm Truy Hồn chạm tối đa 5 mục tiêu khác nhau, gồm mục tiêu đầu và 4 lần nảy |
| D12 | Hồi Nguyệt Kiếm lớn hơn, trúng cả lượt đi và về |
| D13 | Giữ nguyên thiết kế 3 lõi R |
| D14 (quy trình cập nhật) | Làm theo giai đoạn nội bộ; theo yêu cầu bàn giao một lượt, hoàn thiện các hạng mục phản hồi và phần v2.1 còn thiếu trong cùng nhiệm vụ. Duyệt sản phẩm ở cuối, không mặc định các mẫu cũ đã đạt. |

Phản hồi bổ sung: thống nhất VFX mỗi nhát Basic Đao sĩ/Kiếm khí, sửa phần cuối Ám Bộ/Dạ Vũ/Đoạt Mệnh, nâng cấp Song Ảnh, sửa hình quạt Hoành Thiên Đao, hoàn thiện E2/E3/F1/F2/F3 Kiếm khí đúng thiết kế.

Các số đo, màu và timing chi tiết dưới đây là mục tiêu thử nghiệm demo, có thể tinh chỉnh theo hình ảnh và phản hồi; không phải cân bằng gameplay chính thức. Không tự thay đổi thiết kế đã duyệt.

## 3. Bảng kỹ năng v2.1 chính thức

| Nhóm | Sát thủ | Đao sĩ | Kiếm khí |
|---|---|---|---|
| P1 | Phong Ảnh Trảm | Phong Ảnh Trảm | Phong Ảnh Trảm |
| P2 | Hoành Nguyệt Trảm | Hoành Nguyệt Trảm | Hoành Nguyệt Trảm |
| P3 | Phản Kiếm Quyết | Phản Kiếm Quyết | Phản Kiếm Quyết |
| E1 | Ám Bộ Trảm | Chấn Địa Trảm | Nguyệt Nha Trảm |
| E2 | Huyết Ấn | Hoành Thiên Đao | Lưỡng Nghi Kiếm |
| E3 | Dạ Vũ Liên Trảm | Cuồng Phong Đao | **Kiếm Vũ Thiên Hà** |
| R1 | Thiên Vẫn Trảm | Thiên Vẫn Trảm | Thiên Vẫn Trảm |
| R2 | Tam Thiên Kiếm Quyết | Tam Thiên Kiếm Quyết | Tam Thiên Kiếm Quyết |
| R3 | Vạn Ảnh Tuyệt Sát | Vạn Ảnh Tuyệt Sát | Vạn Ảnh Tuyệt Sát |
| F1 | Tàng Ảnh | **Bá Vương Đao Vực** | **Thiên Kiếm Xuyên Tâm** |
| F2 | Đoạt Mệnh | Đoạn Sơn Trảm | Phi Kiếm Truy Hồn |
| F3 | Song Ảnh Truy Sát | Bất Diệt Chiến Ý | Hồi Nguyệt Kiếm |

Tổng vẫn 24 kỹ năng khác nhau: 3 P dùng chung + 9 E AOE + 3 R dùng chung + 9 F. Chọn 1/3 mỗi nhóm, giữ lựa chọn khi chuyển nhánh; P/R thử được trên cả 4 form trong demo, E/F đúng chuyên nhánh. Không tự đặt cấp mở khóa, công thức damage/cooldown hay thay phím gameplay.

### 3.1. Mapping ID nguồn — đặc biệt lưu ý Đao sĩ và Kiếm khí

| ID nguồn v2.0 | v2.0 | v2.1 |
|---|---|---|
| 09 | Dao F1 Bất Động Chiến Thế | Không còn là kỹ năng chọn độc lập; giữ pose thủ, aura và asset nguồn cần thiết để kế thừa/truy vết |
| ID logic mới hoặc mapping tương thích (Dao) | — | Dao F1 Bá Vương Đao Vực; dùng pose nguồn 09 và các pose bổ/quét Đao sĩ phù hợp, ánh xạ rõ nguồn |
| 12 | Qi E3 Kiếm Giới | Không còn là kỹ năng chọn độc lập; giữ asset/pose/ấn trận làm nguồn kế thừa |
| 22 | Qi F1 Kiếm Vũ Thiên Hà | Chuyển thành Qi E3 Kiếm Vũ Thiên Hà; tra đúng animation nguồn 22 |
| ID logic mới hoặc mapping tương thích | — | Qi F1 Thiên Kiếm Xuyên Tâm; kế thừa pose/ấn từ nguồn 12 và kiếm năng lượng phù hợp |

Không âm thầm ghi đè định danh nguồn 09/12 hoặc tráo animation 12/22 theo index. Giữ 24 ID nguồn và GUID để truy vết; ưu tiên tách định danh lựa chọn v2.1 khỏi ID pose/asset nguồn, hoặc mapping tương thích minh bạch nếu kiến trúc hiện có buộc dùng index. Catalog lựa chọn chỉ có 24 mục (không thêm Bất Động Chiến Thế hoặc Kiếm Giới thành mục thứ 25/26); hai skill mới có ID logic riêng hoặc mapping tương thích không xung đột. Các ID nguồn khác và slot giữ theo v2.0. Cập nhật builder, dữ liệu neo, UI, matrix và gallery theo mapping; bảo toàn dữ liệu review cũ. Hai ngoại lệ đã duyệt đối với quy tắc v2.0 không bỏ kỹ năng cũ là Bất Động Chiến Thế và Kiếm Giới: không còn slot riêng nhưng giữ asset nguồn để kế thừa, không xóa tùy tiện.

## 4. Tiêu chuẩn nghệ thuật và asset PixelFX

### 4.1. Nguồn tham khảo chính thức

- Thư viện: https://pixelfx.io/effects/
- Hướng dẫn AI coding: https://pixelfx.io/guides/ai-coding-tools/
- Developer docs: https://pixelfx.io/docs/
- Upstream: https://github.com/patch-gg/pixelfx
- Giấy phép/attribution: https://pixelfx.io/licenses/

Thư viện miễn phí có sprite sheet PNG trong suốt và JSON frame/timing/pivot. Upstream cho phép chỉnh palette, tham số và viết effect JS; dùng quy trình draft → render → inspect → refine → export. Chỉ dùng hiệu ứng miễn phí/MIT đã kiểm tra nguồn, giữ copyright, license và upstream notices cùng bản phân phối; không mặc định font hoặc gói Vault/AI trả phí cũng có giấy phép này. Không mua dịch vụ, không tải toàn bộ thư viện vào Assets. Kiểm tra điều kiện sử dụng từng nguồn. Nếu không truy cập được nguồn, báo rõ và dùng tài nguyên hiện có hoặc asset tự tạo, không bịa kết quả tải/render.

| Nhóm tham khảo | Tên effect có trong thư viện | Ứng dụng dự kiến |
|---|---|---|
| Melee & slashes | Sword slash, Double slash, Dagger flurry, Spin slash, Heavy cleave, Thrust | Basic, P1/P2, Ám Bộ, Dạ Vũ, Đoạt Mệnh, Hoành Thiên, Cuồng Phong |
| Hit & impact | Hit spark, Hit flash, Critical hit, Parry, Shield block, Ground slam | Phản hồi trúng, P3, đòn đao nặng, phòng thủ |
| Shadow / Explosions | Hex sigil, Curse, Dark explosion, Smoke bomb | Huyết Ấn, Tàng Ảnh, dấu phi tiêu và tàn ảnh Sát thủ |
| Arcane / Projectiles | Rune circle, Spell charge, Magic missile, Energy disc | Sóng kiếm, trận mưa kiếm, đại kiếm, phi kiếm và hồi kiếm |
| Complete moves | Dash strike, Sword combo, Ground pound | Tham khảo timing, trọng tâm và chuyển pha; không thay sprite nhân vật bằng nhân vật mẫu |

Tên trên là nguồn ứng viên, không phải danh sách asset đã tải hay bắt buộc import. Kiếm rơi/đại kiếm/phi tiêu phải có silhouette riêng; không dùng Rain nước, projectile hình tên lửa hay đường LineRenderer thô làm VFX thành phẩm. Có thể dùng procedural motion để vận chuyển sprite, debug và particle phụ trợ.

### 4.2. Bản sắc hình ảnh

| Form | Màu đề xuất | Hình dạng/chuyển động |
|---|---|---|
| Kiếm sĩ | Trắng bạc, xanh nhạt (#E2E8F0, #94A3B8, #60A5FA, #DBEAFE) | Vệt chém sắc, gọn, ít nhiễu |
| Sát thủ | Tím đen, đỏ tím (#261638, #6D28D9, #C026D3, #FB7185) | Vệt cắt mảnh, tàn ảnh, khói, dấu ấn, tốc độ cao |
| Đao sĩ | Cam vàng (#92400E, #EA580C, #F59E0B, #FDE68A) | Cung đao dày, đá vỡ, nứt đất, trọng lượng |
| Kiếm khí | Xanh ngọc, trắng sáng (#0E7490, #22D3EE, #67E8F9, #ECFEFF) | Kiếm năng lượng, hạt sáng, quỹ đạo cong, bay xa |

Không đổi trang phục, tỷ lệ, silhouette hay ngoại hình 4 form. P dùng chung có thể phối màu nhẹ theo form nhưng cùng cơ chế; R bảo toàn màu/asset nguồn theo D13. Phân biệt chiêu bằng hình dáng, nhịp, đường đi và impact, không chỉ recolor.

### 4.3. Chất lượng animation/VFX

- Thiết kế theo 5 lớp/pha: chuẩn bị/tích lực → ra đòn (vệt kiếm chính) → va chạm (hit flash/spark) → dư chấn/particle → tan biến/thu thế. Không bắt buộc 5 lớp lớn cho mọi đòn; Basic/P ngắn gọn, E/F nổi bật, R giữ vị thế mạnh nhất.
- Neo đúng tay, chuôi, mũi kiếm, chân, điểm chạm đất hoặc vị trí mục tiêu theo frame/hướng; cập nhật JSON neo cùng pose nếu sửa. Không để hiệu ứng dính sát thân vô lý hoặc che mất silhouette.
- Chuyển pha lấy đà, chuyển trọng tâm, chân trụ, tăng/giảm tốc, nhát chém và recovery tự nhiên; không lặp pose tĩnh, giật về Idle hoặc trượt chân. Chỉ bổ sung frame nếu cần, không nhân bản 8 hướng giả tạo. Ưu tiên sửa mảng sprite runtime và wiring, không chỉ file .anim.
- Dùng timing JSON khi import; giữ pixel art sắc nét, alpha sạch, pivot/PPU nhất quán với project, tránh halo/viền xám, clipping, nhảy scale và texture filtering mờ. Kiểm tra nền sáng/tối. SFX phát đúng mốc chạm, hit-stop/rung ngắn có chủ đích, không kéo dài khóa điều khiển vô lý.
- Phối cảnh top-down: vòng kiếm ngang và sóng đất không trông như khiên dựng đứng; phân lớp trước/sau nhân vật, neo mặt đất ở chân/điểm đáp, đá bay cao tách khỏi mặt đất. Phối cảnh render không được tự làm méo hitbox world space.
- Ưu tiên phát triển lại 10 hiệu ứng PixelFX đã nhập; mở rộng có chọn lọc khi thực sự cần. Giữ pha đầu đang tốt, thay pha kết thúc kém; tránh vệt sáng thô, hình mũi tên, hình học debug dùng như VFX.
- Render sprite sheet, contact sheet và preview GIF trước khi nhập. Đánh giá ở kích thước Game View thực, 1x và 0,5x. Không coi ảnh VFX tĩnh hoặc render ngoài Unity là bằng chứng đã đồng bộ trong game.

## 5. Đồng bộ vùng đánh, hitbox và phản hồi sát thương

Áp dụng cho Basic của 4 form và tất cả P/E/R/F. R không đổi cơ chế/phạm vi nguồn; nếu không có hitbox chiến đấu thì chỉ kiểm tra hình ảnh hiện hữu, không tự thêm damage vào R.

- Mặc định tắt vùng debug; toggle riêng để kiểm tra. Không vẽ vòng/hành lang hình học liên tục ở chế độ bình thường. Vòng rune/telegraph có chủ đích của skill được giữ nhưng phải là VFX thẩm mỹ, không phải debug.
- Phân biệt hình dạng vùng đánh, tầm tìm mục tiêu, quãng dịch chuyển, độ rộng projectile, bán kính/đường kính và vùng gây sát thương. Vùng debug phản ánh dữ liệu va chạm đang dùng, không phải một vùng minh họa tùy ý.
- Một nguồn dữ liệu cho tâm/hướng/độ rộng/timing/quỹ đạo dùng khi dựng VFX và tính trúng trong demo. Hitbox chỉ hoạt động ở pha có sát thương; projectile phải chạm mục tiêu mới gây damage mô phỏng. Sóng nhanh cần kiểm tra đường quét giữa các frame để không lọt mục tiêu. Vùng nổ kiểm tra tại thời điểm nổ, không gây trúng trước khi VFX xuất hiện.
- Quái trúng có hit flash, impact và phản hồi riêng; quái ngoài vùng không mất HP, không nhận dấu trúng. Phần hạt trang trí có thể vượt mép vùng nhưng phần lõi có tính gây sát thương phải dễ đọc. Không tính trùng do nhiều renderer/particle, dùng ID mục tiêu và danh sách trúng theo đòn/lượt.
- Kiểm tra bia ở trong, sát mép và ngoài vùng, mục tiêu di chuyển/biến mất, 0/1/5/9 bia, cụm chồng, 8 hướng, reset/replay, chạy trước và sau khi cast. Tất cả sát thương ở đây là mô phỏng, không chốt chỉ số production.

## 6. Đánh thường (Basic) — bốn form

Giữ số nhát và combo nguồn; nâng cấp vệt chém, nhát trúng, chuyển động cơ thể, điểm neo, thời điểm và hitbox từng nhát. Không đếm các nhát hoặc hiệu ứng phụ sai quy tắc bị động v2.

| Form | Combo và hình ảnh |
|---|---|
| Kiếm sĩ | Hai nhát bạc nối tiếp, vệt mỏng sắc, impact gọn |
| Sát thủ | Chém nhanh, chém chéo, đâm; vệt tím mảnh, nhát đâm cuối có tia va chạm mạnh |
| Đao sĩ | Giữ chém ngang nặng rồi bổ; cả hai dùng cùng bộ cung đao cam vàng dày, silhouette nhất quán. Chỉ đổi góc, vị trí neo, hướng và timing theo vũ khí; bụi/impact phụ không thay cung chính. Không để nhát hai thành một VFX khác hẳn nhát đầu. |
| Kiếm khí | Hai nhát thuận–nghịch nối theo pose cơ thể, mỗi nhát phóng một sóng bán nguyệt xanh ngọc từ mũi kiếm theo hướng ngắm. Dùng cùng sprite, cỡ và thời gian bay cho cả hai nhát, đảo chiều vệt theo nhát nghịch. Mỗi sóng bay ngắn khoảng 2,15u, nhỏ hơn Nguyệt Nha. Mỗi bia nhận tối đa một hit mỗi sóng. Mỗi sóng trúng cộng một lần tích bị động, kể cả xuyên nhiều bia. |

Basic Kiếm sĩ: tăng độ rõ cung kiếm, neo sát lưỡi; Basic Sát thủ: sửa vệt đâm thấp dưới vũ khí, neo theo trục đâm và không giống viên đạn. Đao sĩ giảm bụi che chân, Kiếm khí giữ phong cách bán nguyệt. Đây là nhận xét hình ảnh bổ sung từ contact sheet, cần xác minh trong runtime.

## 7. Bị động P dùng chung

| ID / Skill | Cơ chế và yêu cầu nâng cấp | Phạm vi thử (world unit) |
|---|---|---|
| 01 P1 Phong Ảnh Trảm | Sau 5 đòn Basic hợp lệ trúng, phóng một vệt kiếm bay thật theo hướng đánh, lõi sáng và đuôi mờ, xuyên bia với hit spark tại điểm chạm. Không ép nhân vật lướt. Hitbox đi cùng sóng; không gây damage cả hành lang ngay lúc phát. | Quãng bay từ 2,65u lên khoảng 3,5u; chiều rộng khớp sprite |
| 02 P2 Hoành Nguyệt Trảm | Sau 4 đòn Basic hợp lệ trúng, vòng kiếm khí 360° mở rộng từ thân, hai lớp cung kiếm xoáy ngang mặt phẳng chiến đấu, phân lớp trước/sau, có mép cắt và đuôi tan; không tạo hai vòng đều giống bong bóng/khiên dựng đứng. Damage khi vòng đi qua bia, mỗi bia một lần cho một kích hoạt; không nổ tức thì toàn đĩa. | Bán kính từ 1,30u lên khoảng 2,2u |
| 03 P3 Phản Kiếm Quyết | Nhận đòn hợp lệ → giảm một phần sát thương → flash đỡ kiếm, tia va chạm → vệt phản kích về phía nguồn đánh. Không khóa di chuyển. Giữ hồi nội bộ demo, dùng H giả lập và hướng tấn công mô phỏng khi cần. | Hành lang phản kiếm khoảng 3,2u (cũ 2,65u), khớp VFX |

Giữ ngưỡng 5/4 theo v2.0; mỗi lần Basic hợp lệ chỉ tăng một lần dù trúng nhiều bia, không tích từ skill, phân thân, VFX phụ hoặc chính bị động. P3 tối đa một kích hoạt mỗi sự kiện nhận đòn nếu sẵn sàng; hồi nội bộ 2s của v2.0 là fixture hiện hành, không phải cooldown chính thức. P dùng overlay không ngắt Idle/Run/Basic/cast đang diễn ra.

## 8. Sát thủ — 6 kỹ năng

### E1 · Ám Bộ Trảm (nguồn 04)
- Giữ pha lướt đầu. Khi tái hiện, quay về phía quái phía sau điểm đáp, rồi chém thuận, chém nghịch và thu kiếm. Dùng pose Turn và pose hai nhát Sát thủ theo hướng đã quay. Nếu thiếu quái phía sau, hướng thu chiêu đối diện hướng lướt.
- Hai vùng chém ngắn hướng về phía quái, tính tại mỗi nhát quanh vị trí đáp. Dùng hai cung cùng bộ chém nhưng ngược quỹ đạo, neo theo mũi dao lúc chạm. Thu kiếm bằng pose hồi, kết thúc cả hai vệt chém trước khi trở lại Idle.

### E2 · Huyết Ấn (nguồn 05)
- Chọn tối đa 5 mục tiêu hợp lệ trong vùng ngắm theo cơ chế v2.0. Mỗi quái có ấn đỏ tím riêng trên thân, đi theo Transform của quái khi di chuyển.
- Ấn phát sáng, nhấp nháy/co lại rồi đồng loạt nổ tại vị trí hiện tại của từng quái. Nổ có flash, lõi nổ, hạt và SFX phù hợp; thu gọn dấu ấn, tách rõ pha gắn/tích/nổ và giảm vòng sáng chồng lấn che quái. Giữ quy tắc chống trùng sát thương chồng lấn của v2.0 trong một lần nổ.
- Mục tiêu bị xóa/ẩn trước khi nổ: hủy ấn gắn trên mục tiêu đó an toàn; không để ấn mồ côi, không đánh mục tiêu vô hiệu. Giữ nguyên quy tắc cho các ấn hợp lệ còn lại.

### E3 · Dạ Vũ Liên Trảm (nguồn 16)
- Ba lần lao chém của nhân vật thật theo ba đường khác nhau; mỗi lần có tàn ảnh, vệt cắt và impact. Nhát cuối nổi bật hơn bằng hình và nhịp, không mặc định thêm hệ số damage production.
- Giữ hiệu ứng đầu đang ổn; làm lại các vệt cắt, đặc biệt nhát cuối dài/sáng hơn theo quỹ đạo, có impact và pha tan sạch; không dùng chung nguyên vệt kết của Ám Bộ hoặc Đoạt Mệnh.
- Hitbox theo từng đường quét, không gộp thành một hình lớn không liên quan; giữ ba cặp swing/hit. Không thêm phân thân, không biến thành R3.

### F1 · Tàng Ảnh (nguồn 06)
- Tan vào khói tím, tái định vị xa hơn, xuất hiện rồi chém phục kích. Tăng dịch chuyển từ 2,40u lên khoảng 4,5–5u để tạo khác biệt dễ thấy. Khói đi theo điểm biến mất/tái xuất, không phủ kín nhân vật vô cớ.
- Nhát phục kích gây trúng tại vị trí xuất hiện theo cung chém thật. Không gây damage suốt đường lướt nếu không có vệt tấn công; không biến thành Đoạt Mệnh hoặc Dạ Vũ. Giữ cơ chế ẩn thân mô phỏng.

### F2 · Đoạt Mệnh (nguồn 17)
- Tụ lực ngắn rồi lao đâm xuyên theo một đường thẳng dài khoảng 6–7u (quãng lao thử), lưỡi đâm trắng tím sắc, đường xuyên và impact kết liễu rõ. Điều chỉnh hành lang sát thương theo cú xuyên và mũi kiếm, không dùng lại nổ huyết ấn/khói Tàng Ảnh.
- Giữ phần đầu đang ổn, thay vệt chém kết bằng một đường xuyên trắng tím hẹp và sắc theo trục kiếm, có điểm sáng tập trung tại bia trúng, không nổ huyết ấn và không biến thành ba nhát.
- Giữ thưởng sát thương với mục tiêu thấp máu theo v2.0; không bảo đảm one-shot elite/boss. Nhấn mạnh một cú xuyên mạnh thay vì nhiều lần đánh/ẩn thân.

### F3 · Song Ảnh Truy Sát (nguồn 18)
- Nâng cấp mạnh VFX: triệu hồi hai ảo ảnh có silhouette Sát thủ rõ từ khói/bóng tím; hai làn có tàn ảnh, vệt chém dài sắc, impact đồng thời, phân thân tan thành khói. Không chỉ dùng hai vệt sáng.
- Hai ảo ảnh đúng form xuất hiện hai bên. Cùng lúc hai ảo ảnh lao chém, bản thể phóng một dao găm xuyên ở làn giữa. Ba đầu đòn cùng tiến theo thời gian, xác nhận trúng khi quét tới bia.
- Cả ba làn dùng chung danh sách bia đã trúng trong một lượt. Bia thực sự trúng nhận tia va chạm và dấu phi tiêu tím bám thân 0,50 giây. Dấu không gây sát thương bổ sung. Dọn phân thân và dấu khi hết hạn hoặc reset.

## 9. Đao sĩ — 6 kỹ năng

Cơ chế v2.0 cơ bản ổn; ưu tiên VFX, độ nặng và đồng bộ, không tự tăng mọi thông số.

| ID / Skill | Nâng cấp |
|---|---|
| 07 E1 Chấn Địa Trảm | Bổ tại chỗ, vết nứt gãy khúc bất đối xứng lan từ điểm lưỡi đao chạm đất, đá văng lên, bụi và sóng xung kích sát sàn; không trông như pháp trận dựng đứng. Hit vào pha Land; giữ bán kính và choáng mô phỏng. Không nhảy như R1. |
| 08 E2 Hoành Thiên Đao | Phóng sóng từ lưỡi đao ở mốc nhát chém. Mép cung bay ra và mở rộng dần trong góc đối xứng ±40°, tầm 5u. Vùng va chạm là dải sóng vừa quét qua, tính một hit mỗi bia rồi đẩy theo hướng chém. Debug vẽ dải đang quét và mặc định tắt. |
| 19 E3 Cuồng Phong Đao | Hai vòng lốc đao liên tiếp, bụi/đá xoay theo, đúng hai nhịp va chạm. Giữ pose xoay, không xoay sprite tĩnh; vòng damage theo tâm nhân vật. |
| F1 Bá Vương Đao Vực (mới; kế thừa nguồn 09) | Cắm đao xuống đất → tạo vùng trọng lực gom quái thường → rút đao, quét một nhát mạnh kết thúc. Không còn buff thủ/aura của Bất Động Chiến Thế. Chi tiết bên dưới. |
| 20 F2 Đoạn Sơn Trảm | Tụ lực rồi bổ dọc, mặt đất tách thành hành lang dài, năng lượng bùng từ khe nứt; impact theo đường bổ, không dùng nổ tròn giống E1. |
| 21 F3 Bất Diệt Chiến Ý | Năng lượng bốc lên từ thân và đao, hạt sáng bay lên; trong thời gian cường hóa, mỗi Basic hợp lệ tạo sóng đao lớn đồng bộ mốc chém. Không thêm khiên phòng thủ. |

### F1 · Bá Vương Đao Vực — thiết kế mới đã chốt

- Vai trò: khống chế/gom cụm quái để phối hợp AOE của Đao sĩ; không phải buff phòng thủ, không lặp cơ chế cường hóa của F3. Chủ dự án chọn bỏ slot Bất Động Chiến Thế, đồng nghĩa mất lựa chọn buff thủ riêng này.
- Pha 1 — Cắm đao: trụ chân, cắm đao chạm đất, bụi và khe nứt mở từ điểm tiếp xúc. Vùng trọng lực có các vệt năng lượng cam vàng hướng vào tâm, không dùng khiên bong bóng hay aura lửa của F3.
- Pha 2 — Gom quái: bán kính hút thử 3,5–4u tính từ tâm khóa tại điểm thi triển. Quái thường hợp lệ trong vùng được kéo dần về gần tâm, không teleport, không chồng vào thân; có khoảng dừng an toàn. Boss không bị kéo; quy tắc elite/kháng khống chế production chưa chốt, demo có thể dùng bia kháng kéo để thử. Không gây sát thương trong pha hút; không kéo quái ngoài vùng. Mục tiêu biến mất phải được bỏ khỏi danh sách.
- Pha 3 — Quét đao: rút đao và quét một nhát mạnh quanh vùng gom (đề xuất quét vòng ngang để dễ trúng cụm). Phạm vi quét và thời lượng hút dùng fixture có ghi chú, chọn sau khi thử hình ảnh; không mặc định cả vùng hút đều bị đánh. Chỉ hit mục tiêu nằm trong vùng quét tại pha va chạm, tối đa một lần/lượt, kể cả boss trong vùng. VFX quét dày, sắc, có tia va chạm; không thêm nhiều nhát hoặc sát thương kéo theo thời gian. Các chi tiết kỹ thuật này là đề xuất demo, không phải balance đã chốt.
- Kế thừa pose thủ/chuẩn bị nguồn 09, pose bổ/quét từ Đao sĩ, đá nứt/impact từ Chấn Địa/Đoạn Sơn, và cung đao phù hợp. Bổ sung pose nối nếu cần để động tác cắm/rút đao rõ ràng, không giả làm chỉ bằng xoay sprite. Giữ nguồn 09/TrongDao và metadata nếu còn dùng; không sửa nhầm F3.
- Kiểm thử 0/1/5/9 bia, bia trong/ngoài/sát mép, bia kháng kéo/boss mô phỏng, mục tiêu biến mất, một hit cuối, không kéo/hit sai, reset giữa pha hút, tám hướng. Dọn mọi trạng thái kéo và VFX khi kết thúc/reset/đổi form.

### F3 · Bất Diệt Chiến Ý — giữ lại

Giữ cường hóa tấn công 5s mô phỏng làm baseline; Basic hợp lệ tạo sóng đao lớn tại mốc chém, không dùng hiệu ứng kéo của F1. Có thể dùng chung material/pool/spark nhưng F1 và F3 phải khác rõ cả cơ chế, silhouette và nhịp. Không giữ Bất Động Chiến Thế như lựa chọn thứ tư của nhóm F.

## 10. Kiếm khí — 6 kỹ năng

### E1 · Nguyệt Nha Trảm (nguồn 10)
- Một bán nguyệt lớn với lõi sáng nhiều lớp, đuôi năng lượng; xuyên nhiều bia. Quãng bay khoảng 5u (cũ 4,10u), bề rộng va chạm toàn phần thử 2,6–3u (cũ khoảng 1,72u).
- Vệt bay xuyên bia, gây tối đa một hit mỗi bia. Bia thường bị đẩy theo hướng sóng tới và có tia xung lực tại điểm chạm. Boss mô phỏng vẫn nhận hit, đứng nguyên vị trí. Giữ một sóng lớn để phân biệt Basic và R2.

### E2 · Lưỡng Nghi Kiếm (nguồn 11)
- Đúng hai đường kiếm đối xứng từ hai nhát chém; bay/hội tụ ở phía trước thành chữ X và một vụ nổ ở điểm giao. Không có ba đường kiếm ban đầu, không sinh thêm ba đường ở điểm giao.
- Hai nhát có timing và quỹ đạo rõ, cùng điểm giao hợp lý theo hướng ngắm. Bia trên đường chịu hit đường kiếm khi chạm; bia ở tâm chịu thêm một hit nổ. Theo dõi từng đường và nổ riêng để tránh tính trùng ngoài ý đồ; chưa chốt hệ số.
- Dùng lại pose hai nhát và burst/PhanQuang thích hợp, sửa hình và quỹ đạo thay vì chồng thêm VFX. Kiểm tra trực tiếp đúng 2 đường bay và 1 điểm nổ: không chỉ đổi tên/hiệu ứng bề mặt trong khi renderer cũ vẫn phát thêm đường kiếm.

### E3 · Kiếm Vũ Thiên Hà (chuyển từ nguồn 22, F1 v2.0)
- Vung kiếm lên trời, triệu hồi 12–18 thanh kiếm năng lượng chia 3 đợt thử nghiệm rơi trong bán kính khoảng 3–3,3u. Kiếm có chuôi, lưỡi, mũi và bóng/điểm đáp đọc được, không dùng vạch mưa nước.
- Kế thừa ấn trận Kiếm Giới làm telegraph ở vị trí ngắm; khi kiếm chạm đất xuất hiện tia/hạt/impact. Bố trí điểm rơi phủ đều vùng bằng các điểm lệch tâm tái lập được ở nhiều phía, không gom toàn bộ vào tâm. Mỗi kiếm có quỹ đạo từ trên cao xuống đúng điểm đáp và impact riêng; không gây damage trước pha rơi. Kiểm tra cả bia ở rìa vùng, không chỉ bia trung tâm.
- Damage mô phỏng theo vùng va chạm từng đợt, gộp trùng trên một bia trong cùng đợt nếu nhiều kiếm chồng; có thể trúng lại ở đợt kế. Không biến toàn bộ hình tròn telegraph thành hit liên tục. Dùng fixture có thể tái lập khi review.
- Phạm vi là bán kính, không phải đường kính. Giữ chức năng dọn cụm quái; không còn Kiếm Giới độc lập trong catalog.

### F1 · Thiên Kiếm Xuyên Tâm (mới)
- Vai trò: đánh đơn mạnh, chuyên elite/boss, khác mưa kiếm và sóng AOE. Khóa một mục tiêu hợp lệ theo hướng ngắm; ưu tiên elite/boss nếu có trong vùng chọn. Demo có thể dùng một bia mạnh được gắn nhãn mô phỏng, không cần thêm hệ boss thật.
- Ba pha rõ ràng: dấu khóa trên mục tiêu → một thanh đại kiếm năng lượng xuất hiện trên đầu → đâm thẳng xuống mục tiêu, flash và impact kết liễu. Gây một đòn damage lớn cho đúng mục tiêu khi chạm, hiệu ứng va chạm lan chỉ là trang trí, không gây AOE ngoài cơ chế.
- Mục tiêu di chuyển: dấu khóa/đại kiếm theo mục tiêu trong pha chuẩn bị. Mục tiêu mất trước khi chạm: hủy hoặc tan hiệu ứng, không hit mục tiêu không hợp lệ. Không tự đổi mục tiêu giữa cú đâm. Tầm khóa và damage dùng thông số fixture có ghi chú, chưa duyệt số chính thức.
- Tái sử dụng vòng ấn/pose nguồn Kiếm Giới (12), sprite kiếm của mưa kiếm nếu phù hợp, cue tụ lực và impact. Đại kiếm phải khác mưa kiếm về kích thước, silhouette, nhịp và mục đích. Không dùng nhiều kiếm nhỏ thay cho một đại kiếm.

### F2 · Phi Kiếm Truy Hồn (nguồn 23)
- Một phi kiếm có chuôi, lưỡi và đuôi sáng tự kiếm đường cong qua tối đa 5 mục tiêu khác nhau, gồm mục tiêu đầu và tối đa 4 lần nảy. Mỗi bia chỉ nhận một hit. Hết bia hợp lệ thì kết thúc sớm.
- Tìm bia đầu theo hướng ngắm; sau mỗi hit tìm bia hợp lệ gần điểm chạm trong tầm nảy hữu hạn. Tầm tìm đầu giữ baseline tối đa 7u, tầm nảy thử điều chỉnh có ghi chú. Không nảy lại bia đã trúng, mỗi bia một hit, không teleport tức thời giữa mục tiêu.
- Nếu bia mất trước lúc chạm, tìm bia hợp lệ chưa trúng trong tầm hoặc tan; nếu hết bia thì kết thúc. Không tính lần nảy/hit cho bia không trúng. Mũi kiếm theo tiếp tuyến bay, impact riêng mỗi lần.

### F3 · Hồi Nguyệt Kiếm (nguồn 24)
- Sóng kiếm lớn bay đi 7–8u (cũ 6,10u), bề rộng thử khoảng 3u; sau đó quay về vị trí hiện tại của nhân vật, kể cả nhân vật di chuyển. Vệt sáng và quỹ đạo quay về rõ, có pha đón kiếm phù hợp. Asset chỉ là bán nguyệt năng lượng thuần túy, bỏ dấu `>`/mũi tên/chevron đè trên hình bán nguyệt ở mọi pha. Dùng cùng motif khi đi và về; xoay/flip theo hướng vận tốc thực, chuyển hướng mượt.
- Mỗi bia tối đa một hit khi đi + một hit khi về, hai danh sách trúng riêng. Hit đúng lúc sóng đi qua bia; không đánh cả hành lang trước. Tăng sức nặng sát thương mô phỏng so với bản hiện tại; có thể thử lượt về mạnh hơn, chưa chốt hệ số. Không đánh thêm lần do đổi pha hoặc overlap nhiều frame.
- Nếu nhân vật đổi vị trí, điểm đến cập nhật theo thời gian; có giới hạn lifetime và cleanup khi reset/đổi form để không tồn đọng projectile.

## 11. Ba lõi HEXCORE R — bảo toàn

| ID | Skill | Yêu cầu |
|---|---|---|
| 13 | R1 Thiên Vẫn Trảm | Giữ jump/dive/land, phạm vi, pose, VFX, SFX và timing nguồn |
| 14 | R2 Tam Thiên Kiếm Quyết | Giữ 3 nhát tăng dần, sóng kiếm và timing nguồn |
| 15 | R3 Vạn Ảnh Tuyệt Sát | Giữ biến mất/phân thân/tái xuất và VFX nguồn |

Không đổi thiết kế, quy mô, logic hoặc asset R trong đợt nâng cấp P/E/F. Quy định ẩn debug và sửa lỗi dùng chung áp dụng nếu không làm đổi hành vi R. Kiểm tra 96 tổ hợp R, đặc biệt pose, alpha, điểm neo, quỹ đạo, impact, âm thanh, reset và camera. Nếu phát hiện lỗi riêng R cần sửa vượt phạm vi bảo toàn, ghi báo cáo để duyệt riêng. Không dùng R làm mẫu để nhân bản nguyên chiêu cho E/F.

## 12. Cấu trúc làm việc và quản lý asset

Workspace chính: `D:\LT game\CKGame\game_brotato`. Giữ cấu trúc refactor; scene dự kiến tại `Assets/Scenes/HexcoreSkillPreview.unity` theo yêu cầu di chuyển gần nhất, kiểm tra repo để xác nhận và cập nhật các đường dẫn nếu chưa đồng bộ. Không tạo scene trùng.

```text
Assets/
  Scenes/HexcoreSkillPreview.unity
  Prefabs/Player/Melee/
    Swordsman/  Assassin/  DaoWarrior/  QiSwordsman/
    Shared/
      VFX/PixelFX/      # chỉ asset chọn lọc đã phát triển và dùng thực tế
      Audio/
    Scripts/MeleeController.cs
    Scripts/MeleeSkillController.cs
    Resources/
    Editor/
    Demo/Capture/      # nếu còn dùng
    AGENTS.md
docs/Melee/
  Source/PixelFX/      # source cần thiết, recipe, license/notices, thông tin upstream
  HexcoreVisual/      # review/gallery hiện có, phân biệt v1/v2/v2.1
  HEXCORE_SKILL_VISUAL_POLISH_V2_1.md
  ...                 # cập nhật tài liệu hiện có, không nhân bản bộ báo cáo
```

- Runtime giữ 2 file chính; Editor tool/capture riêng nếu cần. Không tạo 24 controller riêng hoặc hệ combat trùng lặp. Ưu tiên dùng pool và dọn hiệu ứng, giữ asset dùng chung không nhân bản giữa các form.
- Lưu nguồn PixelFX theo effect ID, URL, phiên bản/commit nếu có, recipe, tham số, PNG/JSON xuất tương ứng và giấy phép. Giữ bản gốc cần tái lập; không import master/ảnh review vào Unity. Dùng thư mục review hiện có và chỉ lưu artifact hữu ích; không tạo clone Unity.
- Bảo toàn .meta, GUID, fileID, serialized reference, Resources.Load key, builder, ID nguồn và nguồn sản xuất. Không tự xóa asset Bất Động Chiến Thế, Kiếm Giới hoặc Q cũ. Chỉ dọn bản mới sinh dư đã xác minh không còn dùng. Không chỉnh gameplay, hệ Súng/Pháp sư, package, Build Settings hoặc project version.
- Không tự nâng/hạ phiên bản Unity, sửa ProjectVersion.txt, xóa khóa đang hoạt động hoặc ép tắt tiến trình. Nếu thiếu Editor phù hợp/project khóa, tiếp tục công việc nguồn/VFX có thể kiểm tra độc lập và báo NOT TESTED cho phần cần Unity. Không dùng bản sao project làm đường vòng.

## 13. Demo và lỗi viền xám

Giữ demo hiện có: WASD/mũi tên di chuyển, J Basic, Space thử skill đang chọn, H nhận đòn mô phỏng, R reset; chọn form/nhánh/skill bằng tên Việt có dấu. Giữ Tự do/Trình diễn, 0/1/5/9 bia, bia yếu, ẩn bia, hướng ngắm, tua, 1x/0,5x, VFX/SFX/hit-stop/camera. Thêm fixture bia di chuyển, bia mạnh và bia sát mép vùng nếu cần để chứng minh cơ chế mới. Debug range mặc định OFF, vẫn có toggle. Không làm UI che nhân vật/skill.

Viền xám: vòng gần nhất đã xử lý một nguồn trong 9 PNG Run Kiếm sĩ: 17 mảng alpha rời, 11.165 pixel; giữ khối nhân vật và vũ khí theo báo cáo. Bảo toàn kết quả, xác minh builder không tái sinh lỗi từ sheet gốc. Tái hiện nền sáng/tối cả 8 hướng sau import, đứng/chạy/turn và chuyển trạng thái; nếu còn lỗi, tách sprite, shadow/trail, material/shader, alpha/import và atlas padding để xác định nguyên nhân. Chỉ sửa nguồn gây lỗi có bằng chứng. Capture sau import khi chạy được Unity; nếu chưa chạy, ghi NOT TESTED, không đánh đồng xử lý PNG với hết lỗi Game View.

## 14. Lộ trình triển khai và cổng duyệt

| Giai đoạn | Công việc | Điều kiện bàn giao |
|---|---|---|
| 1. Audit & VFX foundation | Đọc repo, mapping ID, baseline, chọn effect PixelFX, tạo recipe, import thử có kiểm soát | Mapping + nguồn/license + contact sheet |
| 2. Vertical slice | Mẫu Basic 4 form (ưu tiên kiểm tra cả 4), P2, Huyết Ấn, Chấn Địa Trảm, Nguyệt Nha Trảm | GIF mẫu, so v2 nếu có bằng chứng; kiểm tra hình dáng, nhịp, phạm vi, palette |
| Rà lại mẫu | Dùng bộ 8 mẫu hiện có làm baseline, xử lý phản hồi cụ thể và tinh chỉnh | Không tự ghi Approved; không coi mẫu offline là nghiệm thu. |
| 3. Nền tảng | Đồng bộ hitbox/VFX, debug OFF, điểm neo, locomotion/viền xám, phản hồi hit và Basic | Kiểm tra trong/ngoài vùng, 8 hướng, chuyển pha |
| 4. P | Hoàn thiện 3 bị động trên cả 4 form | Đúng trigger, không ngắt điều khiển, không trùng hit |
| 5. Sát thủ | 6 E/F | Dấu từng bia, nổ, dash, phân thân đồng thời |
| 6. Đao sĩ | 6 E/F; thay F1 bằng Bá Vương Đao Vực | Quạt đối xứng, Basic nhất quán, gom quái rồi quét một nhát; F3 cường hóa giữ nguyên cơ chế |
| 7. Kiếm khí | 6 E/F, mapping Kiếm Giới/Thiên Hà/Xuyên Tâm | Chữ X đúng 2 đường, mưa kiếm thật, đánh đơn, nảy và hồi kiếm |
| 8. Hồi quy & QA | R, 4 form, 24 skill, Basic, 8 hướng, UI và cleanup | Ma trận + bằng chứng thực tế, không ảnh hưởng gameplay |

Codex thực hiện theo các bước nội bộ, kiểm tra hiện trạng trước và chỉ sửa phần còn thiếu/chưa đạt. Trong nhiệm vụ tổng hợp này, triển khai các phản hồi và phần v2.1 còn thiếu trong cùng lượt, không dừng để duyệt từng chiêu hay hỏi quyết định kỹ thuật nhỏ. Đây là điều chỉnh quy trình theo yêu cầu bàn giao một lượt, không phải xác nhận 8 mẫu cũ đã đạt. Cổng nghiệm thu cuối vẫn thuộc chủ dự án; nếu phát sinh thay đổi thiết kế ngoài phạm vi đã chốt thì giữ nguyên phần đó và ghi đề xuất. Khi Play Mode bị chặn, hoàn thành phần offline khả thi, ghi NOT TESTED, không tuyên bố duyệt runtime. Không tạo project sao.

## 15. Checklist nghiệm thu bắt buộc

| Hạng mục | Tiêu chí |
|---|---|
| Dữ liệu và compile | 24 mục, mapping đúng, 4 form, 336 tổ hợp P/E/R/F; Basic riêng 4×8=32 tổ hợp hướng; Unity compile nếu chạy được, phân biệt Roslyn với Unity |
| Animation | 8 hướng đúng form, không khựng/chân trượt/lệch pivot, chuyển Idle/Run/Turn/Basic/Skill tự nhiên; kiểm tra 1x và 0,5x |
| VFX | Sprite hiệu ứng có hình riêng, đồng bộ lưỡi kiếm/tâm va chạm, không chỉ recolor/scale/line thô, rõ trên nền sáng/tối, không che nhân vật quá mức |
| Phạm vi | Debug mặc định tắt, khi bật khớp hitbox thực; bia trong/ngoài vùng có kết quả hợp lý; không trúng trước khi projectile chạm |
| P | 5/4 đòn đúng quy tắc; phản đòn đúng sự kiện và hồi nội bộ; không ngắt điều khiển |
| Sát thủ | Ám Bộ quay lại chém đúng hai nhát rồi thu kiếm; Song Ảnh chạy đồng nhịp hai làn phân thân và một dao giữa, chống trùng hit; dấu phi tiêu chỉ trên bia trúng; các chiêu khác theo đặc tả trên |
| Đao sĩ | Hoành Thiên phát sóng từ lưỡi đao, quét ±40°/5u, bia xa chỉ trúng khi mép sóng tới; Bá Vương kéo bia thường, không kéo boss; Bất Diệt giữ cường hóa |
| Kiếm khí | Basic phóng hai sóng ngắn và cộng tối đa một điểm bị động mỗi sóng trúng; E1 đẩy bia thường, miễn đẩy cho boss; F2 tối đa 5 bia khác nhau; E2, E3, F1 và F3 theo đặc tả trên |
| R | 96 tổ hợp hồi quy không đổi hành vi nguồn; so camera/pose/quỹ đạo/timing/SFX với baseline khi có |
| Độ ổn định | Không có lỗi Console, VFX/mark/buff/projectile/clone mồ côi, reset/đổi form/replay sạch; không tác động GameScene/Player chính |
| Asset | License/notice/recipe đầy đủ, không asset trùng vô ích, không phá GUID, không tạo clone |
| Review | Video/GIF từ camera Unity cho Basic và 24 kỹ năng đại diện, thêm hướng/trường hợp lỗi; trước/sau nếu có baseline thật, visual QA toàn ma trận hoặc ghi phần chưa xem |

Tự render → quan sát → chỉnh nhiều vòng; chấm 1–5 cho hình, mượt, lực, bản sắc, khớp nhân vật, khớp vùng, khả năng quan sát và WOW, mục tiêu từ 4/5 cho từng tiêu chí kiểm chứng được; không chấm điểm thiếu bằng chứng. Bàn giao một gallery dễ xem, so trước/sau 1×/0,5× cho các mục sửa, ưu tiên đoạn cuối của Sát thủ và các cơ chế mới; các GIF offline phải có nhãn và không thay capture Unity.

Báo cáo PASS / FAIL / NOT TESTED riêng theo compile, static data, Play Mode, visual, audio và capture. Không đánh dấu PASS hình ảnh vì chỉ compile hoặc vì có PNG. Không đánh dấu Approved khi chủ dự án chưa duyệt. Dùng gallery cũ làm tham chiếu lịch sử có nhãn, không gắn nhãn thành capture v2.1.

## 16. Bàn giao

Cập nhật báo cáo hiện trạng/QA hiện có trong `docs/Melee/`; thêm ma trận v2.1 hoặc cột/version rõ để không ghi đè bằng chứng lịch sử. Bàn giao danh sách skill đã sửa, mapping ID, danh sách asset nguồn → asset dùng, license, recipe, preview mẫu và nhận xét trước/sau; báo cáo lỗi viền xám, giới hạn môi trường, kết quả test và việc còn tồn tại. Báo cáo rõ file chỉnh sửa, scene và đường dẫn review. Không sinh quá nhiều file báo cáo trùng nội dung.

**Chưa chốt:** công thức sát thương, hồi chiêu, cấp mở khóa, thông số balance/AI/boss chính thức, thời lượng hút và tầm quét Bá Vương Đao Vực, hệ số kháng kéo elite, hiệu suất khi tích hợp gameplay thật. Góc 80°/tầm 5u, bán kính hút 3,5–4u và dấu phi tiêu 0,4–0,6s là fixture đề xuất, không phải chỉ số production. Những quy tắc xử lý mục tiêu mất, chống trùng và ngưỡng thử chi tiết nêu ở trên là đề xuất kỹ thuật an toàn cho demo, không phải quyết định cân bằng production. Nếu cần thay đổi cơ chế đã duyệt, báo để chủ dự án quyết định.

**Mục tiêu cuối:** bộ Cận chiến đẹp, mượt, có lực, phạm vi rõ và mỗi chiêu có bản sắc; không hy sinh cấu trúc gọn, tài nguyên đã làm hoặc tính đúng của gameplay mô phỏng để chạy theo số lượng hiệu ứng.

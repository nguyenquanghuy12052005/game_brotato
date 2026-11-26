# HEXCORE — ĐẶC TẢ THIẾT KẾ LẠI KỸ NĂNG v2.0

> Trạng thái: APPROVED — Chủ dự án đã duyệt 8/8 quyết định thiết kế ngày 26/11/2026; không có yêu cầu bổ sung.  
> Phiên bản: v2.0 chính thức về thiết kế và phạm vi demo; chưa phải báo cáo hoàn thành triển khai.  
> Dự án: Brotato-Game / HEXCORE · Unity 2D · Pixel Art · Top-down Action Roguelite.  
> Cơ sở: `HEXCORE_SKILL_ASSET_DEMO_PLAN(2).md` (v1.0), mục 1–9 và 13–14, cùng các yêu cầu chỉnh sửa của chủ dự án.  
> Phạm vi: thiết kế lại bộ kỹ năng và Skill Preview Scene, chưa tích hợp gameplay production.

## 1. Yêu cầu của chủ dự án

| ID | Yêu cầu |
|---|---|
| REQ-01 | Ba kỹ năng đầu của Kiếm sĩ chuyển thành bị động dùng chung, kế thừa sau khi chọn chuyên nhánh. |
| REQ-02 | Mỗi chuyên nhánh Sát thủ, Đao sĩ, Kiếm khí có 3 kỹ năng chủ động gây sát thương AOE, chọn 1 để dọn quái nhanh; tổng 9. |
| REQ-03 | Giữ nguyên 3 lõi HEXCORE đã build: Thiên Vẫn Trảm, Tam Thiên Kiếm Quyết, Vạn Ảnh Tuyệt Sát; dùng chung cả ba nhánh. |
| REQ-04 | Ba kỹ năng cuối của từng nhánh lấy từ những kỹ năng cũ còn lại của chính nhánh đó; không trùng nhóm AOE, không bỏ phí asset. |
| REQ-05 | Mở rộng phạm vi kỹ năng, nhất là nhóm cuối; hình ảnh rõ, không dính sát nhân vật. |
| REQ-06 | Sửa viền xám quanh Kiếm sĩ khi di chuyển. |
| REQ-07 | Kế thừa tối đa sprite, animation, VFX, SFX, prefab và logic hiện có nếu tương thích. |
| REQ-08 | Theo các yêu cầu review trước: chuyển động mượt, kỹ năng có bản sắc; demo hiện tên tiếng Việt có dấu và có điều khiển di chuyển. |

## 2. Quy tắc và phạm vi đã duyệt

| Giai đoạn | Nhóm | Số kỹ năng toàn hệ | Lựa chọn của người chơi |
|---|---|---:|---|
| 1. Kiếm sĩ | P (Q cũ) | 3 | Chọn 1, tự kích hoạt, giữ xuyên suốt |
| 2. Chuyên nhánh | E | 9 (3 × 3) | Chọn 1 AOE của nhánh đã chọn |
| 3. HEXCORE | R | 3 | Chọn 1 lõi chung |
| 4. Bổ trợ chuyên nhánh | F | 9 (3 × 3) | Chọn 1 trong 3 skill cũ còn lại của nhánh |
| Tổng | | 24 | 1 bị động + 3 chủ động |

- Quy tắc chọn 1/3 mỗi giai đoạn được giữ từ v1.0; các kỹ năng đã chọn không mất khi mở nhóm mới. Không khóa R theo nhánh.
- Có 243 tổ hợp lý thuyết nếu các lựa chọn độc lập: 3 nhánh × 3 P × 3 E × 3 R × 3 F. Không phải số lượng bộ asset phải làm.
- P/E/R/F là nhãn nhóm thiết kế; phím điều khiển thực tế và cấp mở khóa chưa chốt. Không đổi ID lưu trữ, tên file, GUID hoặc tham chiếu prefab một cách tùy tiện.
- Cấp 4/6/12 là mốc lịch sử của v1.0, không tự áp dụng sang v2.0. F mở sau R theo thứ tự giai đoạn người dùng nêu, nhưng chưa có cấp cụ thể.
- Demo mô phỏng điều kiện kích hoạt, phạm vi, mục tiêu và va chạm. Chưa chính thức hóa công thức sát thương, cooldown, AI, cấp độ, loot, boss hay hiệu ứng cấp 10–11.

## 3. Ba bị động dùng chung (P) — đã duyệt

| ID thiết kế | Skill nguồn | Cơ chế demo | Kế thừa |
|---|---|---|---|
| P1 | Q1 – Kiếm Lướt / Phong Ảnh Trảm | Mỗi 5 đòn đánh thường trúng địch, phóng một đường kiếm theo hướng đánh; không tự lướt. | Slash, trail, dư ảnh phù hợp |
| P2 | Q2 – Hoành Nguyệt Trảm | Mỗi 4 đòn đánh thường trúng địch, phát vòng kiếm khí 360°. | VFX vòng chém |
| P3 | Q3 – Phản Kiếm Quyết | Khi nhận đòn, giảm một phần sát thương và phản kiếm về phía địch; có hồi nội bộ. | Guard flash, phản kiếm, hit spark |

- P1/P2 đếm theo lần đánh thường hợp lệ, không đếm theo số quái bị cùng một nhát đánh trúng. Sóng kiếm của basic cùng lần đánh không được đếm lần nữa. Không tự tích từ P/E/R/F, phân thân hay VFX phụ.
- P3 kích hoạt tối đa một lần cho sự kiện nhận đòn khi đã sẵn sàng; demo có nút giả lập bị đánh. Không cần bắt nhân vật đứng thủ thế lâu.
- Không tự thay đổi vị trí, facing hoặc ngắt animation đang chạy. Dùng lớp VFX hoặc overlay tương thích, không ép phát toàn bộ clip Q cũ nếu gây giật. Giữ các clip nguồn để tái sử dụng.
- Các ngưỡng 5/4 dùng làm giá trị khởi đầu cho preview; không phải thông số balance gameplay đã duyệt. Hồi nội bộ và mức giảm sát thương P3 chỉ là tham số demo, chưa chốt giá trị chính thức.

## 4. Phân bổ 18 kỹ năng chuyên nhánh (E/F)

**Phương án đã duyệt — ưu tiên AOE vốn có của asset cũ:** so với phương án trước (giữ nguyên mọi slot E/F), đổi chỗ 2 cặp: Tàng Ảnh ↔ Dạ Vũ Liên Trảm; Bất Động Chiến Thế ↔ Cuồng Phong Đao. Lý do: Dạ Vũ và Cuồng Phong đã có chuyển động tấn công diện rộng/đa nhịp phù hợp hơn cho nhóm dọn quái, còn hai chiêu thiên về phòng thủ/ẩn thân phù hợp nhóm bổ trợ. Chủ dự án đã duyệt cả hai hoán đổi và xác nhận giữ nguyên phân bổ Kiếm khí.

### 4.1. Sát thủ

| Slot mới | Skill nguồn v1 | Cơ chế demo | Phạm vi/VFX và khác biệt |
|---|---|---|---|
| E1 Ám Bộ Trảm | E1 | Lướt xuyên nhóm quái; hai nhát nhanh phủ một hành lang sát thương. | Một đường lướt, kéo dài trail và vùng quét; không bắt buộc có mục tiêu đơn nếu có hướng ngắm. |
| E2 Huyết Ấn | E2 | Đặt ấn tối đa 5 địch trong vùng, kích nổ sau khoảng ngắn, gây AOE từng ấn. | Dùng rune/burst hiện có; giới hạn sát thương chồng lấn trên một địch. Tên hiển thị chính thức trong bản đặc tả là Huyết Ấn; không tự đổi tên. |
| E3 Dạ Vũ Liên Trảm | F1 | Ba nhịp lướt chém theo những đường khác nhau, quét cụm quái rộng. | Nhân vật thật di chuyển qua 3 đường; khác R3 là phân thân liên trảm. Không thêm cơ chế phân thân. |
| F1 Tàng Ảnh | E3 | Tạo màn khói, ẩn thân ngắn để tái định vị, tái xuất với nhát phục kích. | Vùng khói rộng; nhát phục kích rõ. Không biến thành một chuỗi 3 lần lướt như E3. |
| F2 Đoạt Mệnh | F2 | Một nhát xuyên kích mạnh, thưởng sát thương với địch thấp máu. | Đường kiếm dài, rộng vừa phải; nhấn mạnh một cú kết liễu, không bảo đảm giết boss. |
| F3 Song Ảnh Truy Sát | F3 | Hai phân thân tách hai bên, tiếp cận và phối hợp đánh các nhóm địch. | Quỹ đạo ra xa nhân vật; bản sao đúng form; khác R3 là nhân vật thật vẫn hiện diện và phân thân hỗ trợ. |

### 4.2. Đao sĩ

| Slot mới | Skill nguồn v1 | Cơ chế demo | Phạm vi/VFX và khác biệt |
|---|---|---|---|
| E1 Chấn Địa Trảm | E1 | Bổ đao xuống phía trước tạo nổ diện rộng, khống chế ngắn quái thường. | Nổ tròn tại điểm va đất; không nhảy như R1. |
| E2 Hoành Thiên Đao | E2 | Một nhát chém ngang hình quạt rộng, có đẩy lùi. | Vệt đao mở rộng từ vũ khí; không xoay liên tục. |
| E3 Cuồng Phong Đao | F1 | Xoay chém liên tục trong khoảng ngắn, có thể di chuyển nếu clip hỗ trợ. | Lốc đao 360° di động, đồng bộ thân/chân; không xoay sprite tĩnh. |
| F1 Bất Động Chiến Thế | E3 | Vào thế thủ, giảm sát thương nhận; tạo xung kích phòng thủ khi kích hoạt. | Vòng xung kích lan rộng, aura nhẹ bám thân có chủ đích; không làm cường hóa tấn công như F3. |
| F2 Đoạn Sơn Trảm | F2 | Tụ lực bổ dọc, đường nứt dài và va chạm dọc hướng chém. | Dạng hành lang dài, không nổ tròn như E1, không nhảy như R1. |
| F3 Bất Diệt Chiến Ý | F3 | Cường hóa tấn công trong thời gian ngắn, đòn thường phát xung kích phụ. | VFX tấn công lan xa; aura bám thân nhỏ để nhận diện, không thiên giảm sát thương như F1. |

### 4.3. Kiếm khí

| Slot mới | Skill nguồn v1 | Cơ chế demo | Phạm vi/VFX và khác biệt |
|---|---|---|---|
| E1 Nguyệt Nha Trảm | E1 | Phóng một sóng kiếm bán nguyệt rộng, xuyên nhóm quái. | Một sóng rộng bay xa; giữ bản sắc khác R2 có ba nhát tăng dần. |
| E2 Lưỡng Nghi Kiếm | E2 | Hai nhát kiếm giao nhau ở phía trước, gây nổ tại điểm giao. | Đường chữ X và vùng giao nằm xa thân; giữ hai pose chém nối nhau. |
| E3 Kiếm Giới | E3 | Đặt kiếm trận ở vị trí ngắm, gây sát thương theo nhịp trong vùng. | Trận cố định trên sàn, không luôn bám chân nhân vật. |
| F1 Kiếm Vũ Thiên Hà | F1 | Mưa kiếm năng lượng rơi xuống vùng chọn từ xa. | Vùng lớn, nhịp rơi theo đợt; khác Kiếm Giới là mưa kiếm từ trên cao. |
| F2 Phi Kiếm Truy Hồn | F2 | Phóng phi kiếm tự truy mục tiêu; có thể nhân bản prefab nếu cần nhiều kiếm. | Quỹ đạo cong, mũi kiếm theo hướng bay, giới hạn số lần va chạm. |
| F3 Hồi Nguyệt Kiếm | F3 | Phóng sóng kiếm lớn bay đi rồi quay về, gây sát thương hai lượt. | Tầm bay lớn; có pose đón kiếm; giới hạn một lần trúng mỗi lượt trên mỗi địch. |

**Kiểm kê:** mỗi nhánh có đúng 6 skill nguồn khác nhau, phân 3 E + 3 F. Không loại bỏ chiêu nào. Thay đổi slot không đồng nghĩa phải đổi file, vẽ lại sprite hoặc làm lại toàn bộ animation. Có thể tái sử dụng asset giữa các kỹ năng, nhưng không lặp nguyên một kỹ năng ở hai slot.

## 5. Ba lõi HEXCORE dùng chung (R) — giữ nguyên

| Slot | Tên | Thi triển giữ nguyên theo v1 |
|---|---|---|
| R1 | Thiên Vẫn Trảm | Lấy đà → bật nhảy → lao xuống → va đất, nứt đất và xung kích → phục hồi. |
| R2 | Tam Thiên Kiếm Quyết | Tụ lực → ba nhát kiếm khí nối nhau tăng dần → nhát cuối cao trào → phục hồi. |
| R3 | Vạn Ảnh Tuyệt Sát | Biến mất → bóng ảnh đúng form liên trảm từ nhiều hướng → tái xuất/kết thúc. |

- Không đổi bản sắc, trình tự, cấu trúc VFX hay animation đã duyệt. Chỉ sửa lỗi chung (nếu có) sau khi ghi nhận và được phép; không tự tăng scale R hàng loạt.
- R là nhóm có cao trào hình ảnh mạnh nhất, dù thứ tự mở F dự kiến sau R. F là bổ trợ/tuyệt kỹ chuyên nhánh, không mặc định phải lớn hơn R.
- Dùng chung logic và VFX; animation cơ thể vẫn phải đúng form và vũ khí tương thích. Không tạo chín phiên bản lõi riêng.

## 6. Phạm vi và cảm giác hình ảnh — nguyên tắc đã duyệt

- Ưu tiên hình rõ và gameplay đọc được thay vì chỉ làm VFX sáng/lớn. Gọi H là chiều cao sprite nhân vật trong world units ở scale gameplay. Các mốc thử: basic khoảng 1–1,5H; P khoảng 1,5–2H; E khoảng 2,5–4H; F khoảng 4–6H cho kích thước chủ đạo vùng chém/đường tác động. R giữ kích thước đã duyệt; có thể ghi đo hiện trạng để đối chiếu. Không coi đây là bán kính chung hay hệ số scale prefab bắt buộc.
- Đường thẳng: đo chiều dài và bề rộng riêng; hình tròn: ghi rõ đường kính và bán kính; hình quạt: ghi góc và tầm. Projectile: đo bề rộng và quãng bay riêng. Audit trước khi đặt số cuối cùng.
- Skill định hướng sinh từ weapon anchor đúng frame, sau đó bay/lan ra; ground AOE đặt tại điểm va chạm/điểm ngắm. Aura phòng thủ và chém vòng có thể bám/tâm tại nhân vật theo chủ đích, nhưng không che silhouette.
- Chỉnh riêng spawn offset, vùng tác động, kích thước sprite/particle, trail, thời lượng và tốc độ. Preview hitbox hoặc gizmo phải khớp VFX; không phóng toàn bộ prefab một cách mù quáng.
- Hiệu ứng phụ không tự kích hoạt lặp chính nó. Đòn đa nhịp/nhiều projectile có giới hạn trúng mỗi mục tiêu phù hợp cơ chế; chưa khóa số balance.

## 7. Animation, di chuyển và sửa viền xám

Kế thừa tiêu chuẩn v1.0: 4 form theo inventory thật (không mặc định 1 form = 1 nhánh); Idle/Run/Turn/Basic 8 hướng; skill định hướng đúng hướng hoặc aim phù hợp; các combo đánh thường riêng Kiếm sĩ, Sát thủ, Đao sĩ, Kiếm khí. Không nhân vô điều kiện thành 96 sprite sheet riêng.

- Chuỗi hành động: chuẩn bị → thi triển → va chạm → phục hồi; bridge Run/Basic/Skill, đồng bộ weapon anchor, pivot chân, sorting layer, mốc release/impact/recover, SFX và camera. Kiểm tra cả tốc độ thường và 0,5x. Không dùng VFX để che chuyển động khựng.
- Lỗi xám: tái hiện trên Kiếm sĩ chạy cả 8 hướng và đổi hướng; phân biệt pixel nguồn, alpha/matte, texture filtering/compression, atlas bleeding/padding, material/shader outline, trail/afterimage, renderer phụ. Chỉ sửa nguyên nhân xác nhận. Không xóa màu xám hợp lệ trên trang phục. Test lại trên nền sáng/tối, không còn halo, không mất chi tiết, không ảnh hưởng form khác.
- Chưa kiểm tra project Unity thực tế, vì vậy nguyên nhân viền xám và mức độ hoàn thiện asset chưa được xác nhận trong tài liệu này.

## 8. Skill Preview Scene

- Dùng scene và pipeline hiện tại; audit repo trước, không dựng scene/hệ thống trùng khi đã có.
- Chọn form thực có, chuyên nhánh, skill theo nhóm; hiển thị đầy đủ tên tiếng Việt có dấu, kèm P/E/R/F và mô tả ngắn. Có điều khiển di chuyển (đề xuất WASD/phím mũi tên), hướng/aim và dummy, không chỉ nút phát hoạt ảnh tại chỗ.
- Bị động: test đánh thường tích đòn hoặc nút mô phỏng sự kiện nhận đòn; có hiển thị số tích lũy/điều kiện và reset demo. Đây là mô phỏng preview, không viết hệ thống combat production.
- E/R/F: thử từng chiêu, replay, 1x/0,5x, bật/tắt VFX/SFX/shake/hit-stop, hiển thị vùng tác động và phase; test Basic → Skill → Run/Basic.
- Cho phép nhiều dummy để quan sát AOE, một dummy để phân biệt single target, kiểm tra mục tiêu giả mất đi, đổi hướng và chạy lại liên tục. Không còn đối tượng, VFX, coroutine hoặc trạng thái tồn đọng.

## 9. Kế thừa asset và nghiệm thu

| Nhóm | Số lượng | Xử lý |
|---|---:|---|
| Q cũ → P | 3 | Chuyển trigger và VFX; giữ file nguồn Q |
| E | 9 | Chọn từ 6 skill mỗi nhánh; chỉnh AOE và bản sắc |
| R | 3 | Giữ nguyên thiết kế và asset đã có |
| F | 9 | Dùng 3 skill còn lại của mỗi nhánh; mở rộng hợp lý |
| Tổng | 24 | Không loại bỏ kỹ năng nguồn |

Trình tự đề xuất: audit → snapshot/review hiện trạng → sửa viền xám/locomotion → lập mapping P/E/R/F → chỉnh P → chỉnh E → chỉnh F → regression R → polish demo và QA. Không ghi đè hoặc xóa asset cũ không cần thiết; lưu mapping ID, prefab, clip, nguồn/license và các sửa đổi.

Checklist nghiệm thu:
- [ ] Đủ 24 skill thiết kế, không trùng slot; 3 R vẫn nguyên bản.
- [ ] 3 P tự kích hoạt và demo được trên mọi chuyên nhánh tương thích, không giành điều khiển.
- [ ] 9 E đều thực sự có vùng sát thương để dọn nhiều dummy; mỗi chiêu đúng bản sắc.
- [ ] 9 F sử dụng các skill nguồn còn lại; khác cơ chế E/R, vùng tác động đọc được.
- [ ] Animation nhân vật/vũ khí/VFX đồng bộ, không giật pivot, kẹt pose, trượt chân, sai hướng hoặc che thân vô lý.
- [ ] Kiếm sĩ di chuyển/chuyển hướng 8 hướng không còn viền xám lỗi.
- [ ] UI tên có dấu, di chuyển/test dummy/replay/tốc độ chậm hoạt động.
- [ ] Ma trận form × skill × hướng phù hợp; báo cáo PASS/FAIL/NOT TESTED, không coi placeholder là hoàn thành.
- [ ] Có GIF/video trước–sau hoặc bằng chứng preview thực tế; danh mục asset tái sử dụng, chỉnh sửa, thiếu và lý do nếu cần asset mới.
- [ ] Không tích hợp gameplay production hoặc tự thay đổi progression/balance chưa được duyệt.

## 10. Biên bản duyệt thiết kế

Ngày duyệt: 26/11/2026. Kết quả: **8/8 quyết định đã chốt**, không có yêu cầu bổ sung.

| ID | Quyết định được chủ dự án xác nhận | Trạng thái |
|---|---|---|
| D1 | Ba bị động dùng chung: Phong Ảnh Trảm, Hoành Nguyệt Trảm, Phản Kiếm Quyết | Đã duyệt |
| D2 | Sát thủ: đổi vị trí Tàng Ảnh và Dạ Vũ Liên Trảm | Đã duyệt |
| D3 | Đao sĩ: đổi vị trí Bất Động Chiến Thế và Cuồng Phong Đao | Đã duyệt |
| D4 | Kiếm khí: giữ phân bổ cũ, cải thiện phạm vi và hình ảnh | Đã duyệt |
| D5 | Ba lõi HEXCORE giữ nguyên thiết kế đã build, dùng chung cả ba nhánh | Đã duyệt |
| D6 | Chín kỹ năng cuối lấy từ skill cũ còn lại của từng nhánh, không trùng nhóm AOE | Đã duyệt |
| D7 | Tăng phạm vi VFX theo loại kỹ năng, không phóng to prefab hàng loạt | Đã duyệt |
| D8 | Hoàn thiện demo, chưa tích hợp gameplay chính thức | Đã duyệt |

### 10.1. Nội dung chưa chốt (không thuộc phạm vi triển khai gameplay)

- Sát thương, hồi chiêu, thời gian hiệu ứng và các giới hạn cân bằng chính thức.
- Cấp mở khóa cụ thể của P/E/R/F, phím điều khiển gameplay chính thức, tiến trình 12 cấp và tương tác hiệu ứng cấp 10–11.
- Giá trị kích thước, khoảng cách và thời lượng VFX cuối cùng: xác định qua audit, preview và review; các tỷ lệ H trong mục 6 chỉ là mốc thử nghiệm.
- Nguyên nhân thực tế của viền xám, ma trận tương thích form × skill × hướng và mức hoàn thiện của asset: cần kiểm chứng trong Unity/repository.

Không tự suy diễn những nội dung trên thành quy tắc gameplay chính thức. Có thể dùng tham số mô phỏng có ghi nhãn trong demo để kiểm tra hành vi và hình ảnh.

## 11. Bảng phân bổ khóa thiết kế (tham chiếu nhanh)

| Nhóm | Slot | Sát thủ | Đao sĩ | Kiếm khí |
|---|---|---|---|---|
| Bị động chung | P1 | Phong Ảnh Trảm | Phong Ảnh Trảm | Phong Ảnh Trảm |
| Bị động chung | P2 | Hoành Nguyệt Trảm | Hoành Nguyệt Trảm | Hoành Nguyệt Trảm |
| Bị động chung | P3 | Phản Kiếm Quyết | Phản Kiếm Quyết | Phản Kiếm Quyết |
| Chủ động AOE | E1 | Ám Bộ Trảm | Chấn Địa Trảm | Nguyệt Nha Trảm |
| Chủ động AOE | E2 | Huyết Ấn | Hoành Thiên Đao | Lưỡng Nghi Kiếm |
| Chủ động AOE | E3 | Dạ Vũ Liên Trảm | Cuồng Phong Đao | Kiếm Giới |
| Lõi chung | R1 | Thiên Vẫn Trảm | Thiên Vẫn Trảm | Thiên Vẫn Trảm |
| Lõi chung | R2 | Tam Thiên Kiếm Quyết | Tam Thiên Kiếm Quyết | Tam Thiên Kiếm Quyết |
| Lõi chung | R3 | Vạn Ảnh Tuyệt Sát | Vạn Ảnh Tuyệt Sát | Vạn Ảnh Tuyệt Sát |
| Bổ trợ | F1 | Tàng Ảnh | Bất Động Chiến Thế | Kiếm Vũ Thiên Hà |
| Bổ trợ | F2 | Đoạt Mệnh | Đoạn Sơn Trảm | Phi Kiếm Truy Hồn |
| Bổ trợ | F3 | Song Ảnh Truy Sát | Bất Diệt Chiến Ý | Hồi Nguyệt Kiếm |

P và R xuất hiện ở cả ba cột để thể hiện quyền sử dụng chung, không phải ba phiên bản asset riêng. Tổng cộng 24 kỹ năng duy nhất: 3 P + 9 E + 3 R + 9 F. Mỗi chuyên nhánh có 6 kỹ năng riêng biệt, mỗi build chọn 1 P, 1 E, 1 R, 1 F.

## 12. Quan hệ tài liệu và bàn giao cho Codex

- Bản v2.0 đã duyệt này thay thế bản `HEXCORE_SKILL_REDESIGN_V2_DRAFT.md` và có ưu tiên đối với nội dung mâu thuẫn trong `HEXCORE_SKILL_ASSET_DEMO_PLAN(2).md` (v1.0).
- Các yêu cầu v1.0 không bị thay thế (4 form, 8 hướng, combo Basic, đồng bộ animation/VFX, quản lý nguồn/license và QA) vẫn giữ nguyên.
- Đính kèm bản v2.0 này và tài liệu v1.0 khi giao việc cho Codex. Prompt triển khai sẽ tách riêng, yêu cầu audit repository, tuân thủ mapping, bảo toàn asset, thực hiện theo từng đợt và báo cáo bằng chứng nghiệm thu.
- Không dùng bản draft làm chỉ thị triển khai song song. Không tự đổi danh sách 24 skill, tạo thêm biến thể R theo nhánh, xóa asset cũ hoặc mở rộng phạm vi sang gameplay production.
- Trạng thái APPROVED xác nhận **thiết kế và phạm vi demo**, không xác nhận animation, asset, lỗi viền xám hoặc Unity scene đã hoàn thành. Checklist mục 9 vẫn chưa được đánh dấu cho đến khi có bằng chứng kiểm thử thực tế.

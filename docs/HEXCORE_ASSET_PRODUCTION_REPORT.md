# HEXCORE · Bàn giao sản xuất asset demo

## Đợt visual polish

[Visual Review Gallery](player-art/HexcoreVisual/index.html) dùng bản ghi trước và sau từ camera Unity cho từng form–skill–hướng. Bản mới thay 320 frame từ 10 atlas chéo của các lõi R, bổ sung 16 pose Cuồng Phong Đao và 64 điểm đo lưỡi/đáp. Bản hiện tại vẫn có những biến thể cần sửa in-between, footwork và neo. Xem [bảng điểm và nhận xét](player-art/HexcoreVisual/VisualScores.csv) và [báo cáo visual polish](HEXCORE_VISUAL_POLISH_REPORT.md).

## Đợt hoàn thiện hướng

Mở [scene Skill Preview](../Assets/Scenes/HexcoreSkillPreview.unity) với Unity 6000.3.25f1. Chọn form, kỹ năng, hướng chạy và hướng đánh. Các tổ hợp trong ma trận có chuỗi pose riêng. Trạng thái Blockout vẫn áp dụng khi cần sửa chuyển động hoặc điểm neo.

| Trạng thái form–skill–hướng | Ban đầu | Hiện tại |
|---|---:|---:|
| In Review / Ready for Review | 1 | 1 |
| AuthoredBlockout | 50 | 263 |
| SourcePoseBlockout | 84 | 0 |
| MissingArt | 129 | 0 |

| Form | PNG trong HexcorePreview | Clip trong HexcorePreview | Idle/Run/Turn/Basic |
|---|---:|---:|---|
| Kiếm sĩ | 536 | 70 | 8/8 |
| Sát thủ | 760 | 98 | 8/8 |
| Đao sĩ | 760 | 98 | 8/8 |
| Kiếm khí | 760 | 98 | 8/8 |
| Tổng | 2.816 | 364 | 32 hàng form–hướng |

- Thêm 1.676 PNG và 213 clip dưới `HexcorePreview`. Q1 Tây Bắc có bộ 12 pose riêng. Q2 lấy thứ tự tám tư thế xoay theo hướng xuất phát. Q3 có guard, phản đòn và phục hồi theo hướng. E/F và ba lõi R có pose riêng ở các hướng trước đây thiếu hoặc dùng pose nguồn.
- Đo điểm tay/kiếm cho Q3, ba mốc phát kiếm khí của R2 N/S và điểm đáp của R1 N/S trên bốn form. Tổng 61 frame đo thuộc 23 biến thể mới. Dùng lại VFX nội bộ và 13 cue WAV. R3 dùng bản sao đúng form và phát sprite theo pha chém. Tách làn phân thân và giảm độ che của VFX kết liễu sau khi xem ảnh Unity.
- Giữ Q1 E ở In Review. Các bộ ảnh mới cần duyệt chậm và chỉnh in-between, hướng lưỡi, chân trụ, cầu nối hoặc điểm neo trước khi đạt Ready for Review.

Xem [kết quả QA](HEXCORE_ANIMATION_QA.md), [checkpoint](HEXCORE_PRODUCTION_PROGRESS.md), [nguồn artwork](HEXCORE_ASSET_SOURCE_LICENSES.md) và [bộ GIF hiện tại](player-art/HexcoreFinal/Review/README.md). Các phần bên dưới là lịch sử sản xuất.

## Bản bổ sung hướng và chuyển động

- Bổ sung Idle/Run SW cho Sát thủ. Cả bốn form có bộ Idle chéo bốn frame, bộ Turn 16 pose và combo Basic theo tám hướng. Basic có 12 frame cho mỗi hướng chính và 8 frame cho mỗi hướng chéo. Vòng Idle chéo và pose Turn có art riêng. Cảnh dùng các pose Turn ở đoạn trước/sau combat khi hướng chạy khác hướng ngắm. Bridge chuyên biệt giữa tất cả cặp động tác vẫn cần duyệt và bổ sung.
- Q1 có 7 hướng. Hai bản art NW đổi tay kiếm hoặc chuyển giáp vai giữa các pose, nên hướng này mang trạng thái thiếu art. Mũi/chuôi kiếm và chân trụ được đo thủ công ở các frame phát VFX của hướng mới.
- 20 kỹ năng Q2/Q3/E/F có chuỗi 12 sprite riêng hướng E và phục hồi. Chuỗi thay cho các key pose giữ trước đây. Ba lõi R có bộ động tác E/W riêng trên bốn form. R1 có bóng dưới chân và đo vị trí lưỡi khi đáp, R2 xuất ba sóng từ vị trí lưỡi đo theo nhát, R3 dùng phân thân mang Animator của đúng form.
- Thay hai âm tổng hợp tại runtime bằng 13 file WAV được thiết kế theo vai trò. Cue mốc đạp, lướt, chạm, gạt, tụ, phóng, biến mất/tái xuất, đáp và nổ/cắt cuối đi theo timeline. Đây là âm nguồn của dự án, vẫn cần nghe duyệt trong Game View.
- Những hướng sử dụng pose nguồn hoặc thiếu động tác riêng vẫn mang nhãn Blockout/Missing art. Không có tổ hợp mới được nâng lên Ready for Review hay Approved chỉ vì bài kiểm thử chạy được. Q1 E giữ trạng thái In Review từ đợt trước.

Xem [82 GIF bổ sung](player-art/HexcoreRemaining/Review/README.md), [bảng 33 ảnh Unity](player-art/HexcoreRemaining/SkillContactSheet.png), [CSV 264 trường hợp](player-art/HexcoreRemaining/HexcoreSkillMatrix.csv) và [CSV hướng locomotion/Basic/Turn](player-art/HexcoreRemaining/HexcoreFormMatrix.csv). Bộ GIF gồm các bảng tám hướng Idle/Run/Turn/Basic, 33 tổ hợp hướng E, bảy hướng Q1, 12 tổ hợp lõi hướng W, các đoạn lõi tắt VFX và ba clip phát chậm. Các mục dưới đây ghi lại mốc bàn giao trước.

| Form | PNG đang có trong HexcorePreview | Clip trong HexcorePreview |
|---|---:|---:|
| Kiếm sĩ | 300 | 37 |
| Sát thủ | 280 | 38 |
| Đao sĩ | 280 | 38 |
| Kiếm khí | 280 | 38 |
| Tổng | 1.140 | 151 |

Tăng 741 PNG và 82 clip so với mốc trước. Bài QA 264 trường hợp ghi nhận 1 In Review, 50 AuthoredBlockout, 84 SourcePoseBlockout và 129 MissingArt. Phần thiếu ưu tiên là các kỹ năng/hướng chéo, chuỗi riêng cho những hướng dùng pose nguồn, cầu nối theo từng cặp động tác và điểm neo frame-by-frame cho các hiệu ứng còn dựng nháp.

## Bản Unity

Mở [HexcoreSkillPreview.unity](../Assets/Scenes/HexcoreSkillPreview.unity) trong Unity 6000.3.25f1. Chọn form, chọn Basic hoặc kỹ năng tương thích, rồi chọn riêng hướng di chuyển và hướng thi triển. Dấu * báo thiếu pose cho hướng đó trong chế độ đang chọn. Bản dựng chạy theo một timeline preview, với tạm dừng, phát lại, bước frame, 1×/0,5×, VFX, SFX, rung camera và hit-stop.

- [Ma trận đủ 24 kỹ năng và bốn form](HEXCORE_FORM_SKILL_MATRIX.md)
- [Kiểm thử và review hình ảnh](HEXCORE_ANIMATION_QA.md)
- [Nguồn và giấy phép](HEXCORE_ASSET_SOURCE_LICENSES.md)
- [24 GIF quay từ camera Unity](player-art/HexcoreProduction/README.md)
- [Bảng 33 ảnh form × kỹ năng hướng Đông](player-art/HexcoreProduction/UnityCatalog_East.png)

## Tiến độ sản xuất

1. **Audit và locomotion.** Kiểm kê lại bốn prefab, 312 sprite và 64 clip gốc. Dùng đúng phiên bản ghi trong ProjectVersion. Thêm các hướng chéo có nguồn phù hợp. Dữ liệu mỗi hướng chéo hiện gồm một pose Idle giữ và bốn frame Run. Form nào thiếu hướng được đánh dấu trong scene.
2. **Basic và chuyển tiếp.** Dựng combo riêng E/W của cả bốn form. Basic Kiếm khí kích hoạt QiWave theo nhát hai. Khi hướng chạy khác hướng đánh, scene cho dừng ngắn và lấy các pose hướng kề có sẵn để xem chuyển hướng. Cần in-between riêng để duyệt foot planting.
3. **24 kỹ năng và ba lõi.** Đủ 24 mục có storyboard theo pha và bộ chọn tương thích. Q1 có chuỗi riêng E, S, W. Ba lõi R trên bốn form dùng sprite hướng E mới. R1 có pose bay và đáp, R2 có ba swing và sóng ra theo nhịp, R3 có animator phân thân lấy từ form hiện tại. Q2/Q3/E/F đã có bốn key pose E mỗi kỹ năng và đang ở mức Blockout.
4. **Hiệu ứng.** Tận dụng VFX dự án cho những nhịp phù hợp. Tia chạm, vệt lướt và bụi dùng điểm neo trong timeline. Hai cue PCM tổng hợp minh họa tiếng vung và chạm. Bộ âm riêng cho từng kỹ năng vẫn cần sản xuất và duyệt.
5. **QA và review.** Chạy Play Mode trên bản sao project. Bảng ảnh và GIF lấy trực tiếp từ camera Unity. Q1 E tiếp tục In Review. Không có mục Approved.

| Form | Sprite / clip gốc | Sprite / clip dưới `HexcorePreview` |
|---|---:|---:|
| Kiếm sĩ | 76 / 16 | 116 / 17 |
| Sát thủ | 72 / 16 | 91 / 16 |
| Đao sĩ | 84 / 16 | 96 / 18 |
| Kiếm khí | 80 / 16 | 96 / 18 |
| Tổng | 312 / 64 | 399 / 69 |

Thư mục `HexcorePreview` bao gồm 34 sprite xuất từ đợt trước và 365 sprite thêm trong lần này. Bảng đếm loại trừ clip VFX và atlas PNG nguồn.

## Hạng mục còn lại

Ưu tiên pose Turn và chuyển tiếp thật, loop Idle các hướng chéo, combo và kỹ năng ngoài các hướng đã sản xuất, in-between của các key pose Blockout, điểm neo vũ khí cho mỗi hướng, VFX chuyên biệt, SFX và duyệt Game View. Các nhịp đánh dùng clip nguồn ở hướng chưa có combo vẫn mang nhãn thiếu combo. Các bản chưa qua kiểm tra trang phục, tay kiếm hoặc phối cảnh không được dùng để lấp chỗ trống.

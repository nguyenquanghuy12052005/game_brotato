# HEXCORE · Ma trận tạo hình và kỹ năng

[CSV visual mới nhất](player-art/HexcoreVisual/HexcoreVisualReview.csv) có mốc trước, trạng thái hiện tại, kết quả chuỗi, chú thích và đường dẫn GIF theo 264 tổ hợp. [Gallery](player-art/HexcoreVisual/index.html) dùng cùng dữ liệu. Phần bảng bên dưới mô tả phạm vi tạo hình và kỹ năng.

| Form | Idle/Run xem được | Basic | Turn |
|---|---|---|---|
| 01 · Kiếm sĩ | N, NE, E, SE, S, SW, W, NW | Combo 2 nhát có art cả 8 hướng. E In Review, các hướng còn lại Blockout | 16 pose xoay |
| 02 · Sát thủ | N, NE, E, SE, S, SW, W, NW | Combo 3 nhịp có art cả 8 hướng, Blockout | 16 pose xoay |
| 03 · Đao sĩ | N, NE, E, SE, S, SW, W, NW | Combo 2 nhịp có art cả 8 hướng, Blockout | 16 pose xoay |
| 04 · Kiếm khí | N, NE, E, SE, S, SW, W, NW | Combo 2 nhịp có art cả 8 hướng với QiWave, Blockout | 16 pose xoay |

Idle chéo có vòng 4 frame. Q2 dùng pose xoay riêng khi vào từ hướng ngoài E. Q3, E/F và ba lõi có biến thể xuất riêng theo form và hướng. Xem [CSV 264 trường hợp](player-art/HexcoreRemaining/HexcoreSkillMatrix.csv) và [bộ GIF](player-art/HexcoreFinal/Review/README.md) để kiểm tra từng biến thể. `AuthoredBlockout` ghi nhận có sprite riêng và vẫn cần duyệt chất lượng, chuyển tiếp và điểm neo.

| Nhóm | Kỹ năng | Form | Trạng thái | Việc còn thiếu để duyệt hình |
|---|---|---|---|---|
| Q1 | Kiếm Lướt / Phong Ảnh Trảm | 01 | E In Review, 7 hướng khác AuthoredBlockout | Tây Bắc có art mới. Rà chân trụ, chuyển trọng tâm và bridge khi xem chậm |
| Q2 | Hoành Nguyệt Trảm | 01 | Blockout | Pose xoay thân/chân liên tục 360°, vệt kiếm theo quỹ đạo thật |
| Q3 | Phản Kiếm Quyết | 01 | Blockout | Guard pose, điểm gạt đòn và chuyển trọng tâm sang phản kiếm |
| E1 | Ám Bộ Trảm | 02 | Blockout | Hai nhát riêng sau tái xuất, điểm kết thúc sau dummy |
| E2 | Huyết Ấn | 02 | Blockout | Khung khắc ấn và VFX ấn máu giữ nhịp trước nổ |
| E3 | Tàng Ảnh | 02 | Blockout | Pose biến mất/tái xuất, động tác phục kích và khói riêng |
| E1 | Chấn Địa Trảm | 03 | Blockout | Chém đất riêng đúng trục, khóa neo vũ khí xuống đất |
| E2 | Hoành Thiên Đao | 03 | Blockout | Pose xoay vai hông, lưỡi đao quét ngang và thu đà |
| E3 | Bất Động Chiến Thế | 03 | Blockout | Dậm chân, guard và aura xuyên suốt loạt đao |
| E1 | Nguyệt Nha Trảm | 04 | Blockout | Key release và neo mũi kiếm theo từng hướng |
| E2 | Lưỡng Nghi Kiếm | 04 | Blockout | Hai cung chéo đối hướng, điểm giao và pose chuyển thân |
| E3 | Kiếm Giới | 04 | Blockout | Pose cắm kiếm, pháp trận riêng và các đợt cắt có khoảng nghỉ |
| R1 | Thiên Vẫn Trảm | 01–04 | AuthoredBlockout | Duyệt dáng bật, độ cao, bàn chân và kiếm khi đáp. Đo neo đáp ở các hướng chéo |
| R2 | Tam Thiên Kiếm Quyết | 01–04 | AuthoredBlockout | Duyệt ba lần đổi thế, hướng kiếm và nguồn phát sóng khi chuyển động |
| R3 | Vạn Ảnh Tuyệt Sát | 01–04 | AuthoredBlockout | Duyệt quỹ đạo, tay kiếm của phân thân, tái xuất và đòn kết |
| F1 | Dạ Vũ Liên Trảm | 02 | Blockout | Ba pose lướt/chém chuyên biệt, nhịp đổi hướng và đặt chân |
| F2 | Đoạt Mệnh | 02 | Blockout | Pose xuyên kích qua mục tiêu và khung kết liễu |
| F3 | Song Ảnh Truy Sát | 02 | Blockout | Pose triệu hồi, đồng bộ chuyển trọng tâm của ba nhân vật |
| F1 | Cuồng Phong Đao | 03 | Blockout | Pose quay liên tục sau mỗi nhịp và lốc không che thân |
| F2 | Đoạn Sơn Trảm | 03 | Blockout | Pose bổ cực nặng, phá giáp và thu đao |
| F3 | Bất Diệt Chiến Ý | 03 | Blockout | Chuỗi cường hóa, aura xuyên nhịp và xung kích theo đao |
| F1 | Kiếm Vũ Thiên Hà | 04 | Blockout | Pose triệu hồi và sprite lưỡi kiếm rơi nhiều vị trí |
| F2 | Phi Kiếm Truy Hồn | 04 | Blockout | Pose phóng, quay mũi kiếm theo tiếp tuyến và va chạm |
| F3 | Hồi Nguyệt Kiếm | 04 | Blockout | Pose đón/thu và điểm bắt sóng tại tay kiếm |

# HEXCORE · Visual polish

## Xem trong Unity và trình duyệt

Mở [Visual Review Gallery](player-art/HexcoreVisual/index.html) bằng trình duyệt. Chọn tạo hình, kỹ năng và hướng. Các chế độ gồm so sánh hai mốc, bản hiện tại, tắt VFX và chuỗi Basic → Skill → Basic → Run. Dùng Phát lại, hai mũi tên hoặc Duyệt tự động để đi theo danh sách. Bản trước dùng đúng đoạn ghi hình của mốc trước. Nếu mốc trước chỉ có 1×, nhãn của từng khung sẽ ghi tốc độ tương ứng.

Trong Unity 6000.3.25f1, mở [HexcoreSkillPreview](../Assets/Scenes/HexcoreSkillPreview.unity) và vào Play. Chọn form, kỹ năng, hướng ngắm và hướng di chuyển. Chọn `Basic → Skill → Basic` để thấy cả bước vào và thoát chiêu. Dùng `0.5x`, `VFX`, `SFX`, `Hit-stop` và các nút bước khung để đối chiếu pose, điểm chạm và âm.

## Các lỗi và phần đã sửa

| Quan sát | Sửa trong preview |
|---|---|
| Thời lượng các beat bằng nhau làm các chiêu cùng nhịp. Giữ frame quá lâu ở cú chém nhẹ | Dùng thời lượng riêng theo hành động cho 23 kỹ năng. Q1 giữ timing riêng theo frame. Dãn đoạn lấy lực và thu thế, rút nhịp va chạm ở các đòn nhẹ |
| Hit-stop giống nhau ở mọi nhịp chạm, làm đòn nhanh có cảm giác ngắt liên tục | Dùng pause ngắn theo loại va chạm. Các nhát giữa của đòn liên hoàn bỏ pause, nhát đáp và finisher giữ nhịp nhấn |
| Ám Bộ dừng ở phía trước mục tiêu. Dạ Vũ lặp hướng và đường đi | Ám Bộ vượt mục tiêu rồi dùng pose theo hướng ngược lại khi phản thân. Dạ Vũ chọn ba điểm lướt cắt vùng mục tiêu và lấy pose tương ứng với hướng mỗi đoạn |
| Thiên Vẫn bật và đáp gần cùng điểm, hiệu ứng chạm dịch theo frame thu đao | Dịch trên sàn theo hướng thi triển bên dưới cung độ cao, thêm marker đáp. Khóa tâm nứt, vòng xung kích và bụi vào frame chạm đầu tiên |
| Tam Thiên dùng ba lượt phóng cùng hình. Một số pose chéo quay sai hướng hoặc mất lưỡi | Thay pose chéo, tách nhịp chuyển thế và lần giữ lực cuối. Ba nét phát có hình, khẩu độ và vận tốc riêng. Đo điểm đầu tại lưỡi của từng lần release ở bốn hướng chéo |
| Vạn Ảnh tái xuất quá xa mục tiêu. Hai bóng cùng trượt ngang với nhịp giống nhau | Nối đường biến mất, dựng lại người thật và nhát kết. Đổi góc tiếp cận giữa hai lượt bóng, dùng bộ chém theo form và hướng, kéo vệt theo lượt chém |
| Cuồng Phong Đao dễ lẫn với Hoành Nguyệt | 16 pose thật cho hai vòng. Vòng đầu tấn rộng, vòng sau bước chéo. Cả hai dùng vệt xoắn thấp và hãm từng nhịp |
| Lưỡng Nghi thiếu dấu giao hai đường kiếm | Hai nét chéo gặp nhau ở điểm bùng. Nguyệt Nha giữ một sóng, Tam Thiên phóng ba hình riêng |

## Visual identity của 24 kỹ năng

| Kỹ năng | Silhouette và nhịp | Quỹ đạo | Hình VFX và dấu va chạm | Điểm phân biệt |
|---|---|---|---|---|
| Q1 Kiếm Lướt | Ép thấp rồi duỗi cả thân, một nhát | Lao thẳng, dừng dài | Vệt nối lưỡi và bụi đạp | Một lần lao, một nhát |
| Q2 Hoành Nguyệt Trảm | Lấy thế xoay, một vòng đủ rồi hãm | Vòng quanh chân trụ | Một vòng ngang | Một chu kỳ, vòng phẳng |
| Q3 Phản Kiếm Quyết | Khép thế thủ, gạt rồi nảy về phản kiếm | Trụ tại chỗ, đẩy lưỡi ra trước | Tia gạt nhỏ rồi tia phản | Khoảng thủ rõ, có tình huống thu kiếm khi trượt |
| E1 Ám Bộ Trảm | Cuộn, tan, quay người và chém đôi | Vượt mục tiêu rồi đánh ngược | Bụi tắt/mở và hai nhịp chạm | Đổi phía trước hai nhát |
| E2 Huyết Ấn | Vươn đặt ấn, ngắt nhịp rồi nổ | Điểm neo trên dummy | Ấn thoi nén rồi bung | Trì hoãn nổ trên một mục tiêu |
| E3 Tàng Ảnh | Hạ người, lui vào bóng, tái xuất và cắt | Hai đoạn dịch khi mờ | Tàn ảnh khói và một nhát | Phục kích một nhịp |
| E1 Chấn Địa Trảm | Nâng đao, bổ nặng, khuỵu | Đứng, chém dọc mặt sàn | Nứt thành đường, chạm trầm | Bổ từ đất, một điểm |
| E2 Hoành Thiên Đao | Mở vai rộng, chém ngang, giữ đà | Cung ngang kéo từ vai | Cung đao nặng | Quét một phía từ thân xoay |
| E3 Bất Động Chiến Thế | Vào trụ, thủ rồi thử hai đòn | Trụ tại tâm | Aura quanh thân và nhịp thử đao | Vào thế phòng thủ trước khi đánh |
| E1 Nguyệt Nha Trảm | Chuyển thế một lần, thu nhanh | Kiếm khí đi thẳng | Một bán nguyệt | Một lần phóng |
| E2 Lưỡng Nghi Kiếm | Hai chém chéo đối hướng | Giao nhau tại mục tiêu | Tia chữ X ở điểm gặp | Nhát thứ hai hoàn tất hình cắt |
| E3 Kiếm Giới | Hướng kiếm xuống, giữ phép rồi thu | Trận cố định trên sàn | Pháp trận và hai đợt điểm nổ | Phạm vi cố định, nhiều nhịp |
| R1 Thiên Vẫn Trảm | Ép thấp, bật, treo đỉnh, cắm vũ khí, khuỵu | Cung cao kết hợp dịch trên sàn | Marker, nứt, vòng xung kích, vật vụn | Cú đáp diện rộng có trọng lượng |
| R2 Tam Thiên Kiếm Quyết | Tụ lực, giật nhanh, đổi thế, nhát hai, giữ, kết | Ba đường phóng từ mũi kiếm | Vệt gấp, cung rộng, mũi xung hai tầng | Ba nhát tăng khẩu độ, tốc độ và lực |
| R3 Vạn Ảnh Tuyệt Sát | Chuẩn bị, tan, bóng cắt chéo, hiện và kết | Các làn cắt gặp nhau | Vệt giao, ánh kết phía sau | Nhân vật thật biến mất giữa lượt bóng |
| F1 Dạ Vũ Liên Trảm | Ba nhịp chân ngắn, mỗi nhịp một slash | Ba đoạn lướt tam giác | Bụi từng cú và va chạm cuối | Ba đường đi khác nhau |
| F2 Đoạt Mệnh | Nén người, lướt rồi xuyên một nhát | Đường xuyên mục tiêu | Tia tím tập trung | Một cú kết gọn, không liên hoàn |
| F3 Song Ảnh Truy Sát | Nhân vật thật giữ nhịp và hai bóng đánh nối | Hai làn song song | Bóng theo hai bên | Người thật vẫn ở trong cảnh |
| F1 Cuồng Phong Đao | Hai vòng giữ trụ, dập từng nhịp | Hai sweep quanh thân | Xoắn nâng theo chiều cao | Chuỗi quay, vòng có độ nâng |
| F2 Đoạn Sơn Trảm | Hai nhịp tích lực rồi dồn vào bổ | Bổ theo trục hẹp | Nứt và nhấn phá ở cuối | Tụ hai lần và va chạm kép |
| F3 Bất Diệt Chiến Ý | Vào aura, đòn nối và duy trì nhịp lâu | Trụ và xoay tại chỗ | Aura đi cùng các tia chém | Chủ động đánh trong aura lâu hơn E3 |
| F1 Kiếm Vũ Thiên Hà | Gọi kiếm trên cao, nhả theo cụm | Nhiều đoạn rơi lệch tâm | Mũi kiếm rơi lần lượt | Phân bố từ trên xuống |
| F2 Phi Kiếm Truy Hồn | Vươn tay đẩy kiếm và giữ hướng | Đường cong bám mục tiêu | Kiếm khí đổi tiếp tuyến giữa đường | Một đường cong có điểm đến |
| F3 Hồi Nguyệt Kiếm | Phóng ra, giữ tay đón, thu kiếm | Ra rồi trở lại | Vệt quay đầu và tia bắt | Nhân vật đón tại cuối chiêu |

## Chấm theo clip đại diện

[VisualScores.csv](player-art/HexcoreVisual/VisualScores.csv) ghi tám tiêu chí, điểm 1–5 và dẫn chứng cho mỗi kỹ năng. Điểm là nhận xét sơ bộ theo clip đại diện. Cần xem đúng tổ hợp trong gallery trước khi dùng điểm đó để đề nghị nâng trạng thái. Gate Ready for Review cần cả tám điểm từ 4 trở lên, hình thể ổn định, điểm neo đúng và bằng chứng chuyển tiếp cùng hướng.

## Kết quả và phần còn thiếu

| Trạng thái | Trước | Sau |
|---|---:|---:|
| MissingArt | 0 | 0 |
| SourcePoseBlockout | 0 | 0 |
| AuthoredBlockout | 263 | 263 |
| Ready for Review | 0 | 0 |
| In Review | 1 | 1 |
| Approved | 0 | 0 |

[CSV trạng thái](player-art/HexcoreVisual/HexcoreVisualReview.csv) có đủ 264 hàng. Lượt này xem ảnh Unity của từng tổ hợp, đối chiếu chuỗi ảnh theo pha và chấm một clip đại diện cho mỗi kỹ năng. Các tổ hợp còn lại có bằng chứng camera riêng trong gallery và cần duyệt chuyển động từng pha trước khi nâng trạng thái.

- Sản xuất 320 frame thay thế từ 10 atlas hướng chéo của ba lõi. Thêm 16 pose cho hai vòng Cuồng Phong Đao. Các sprite sửa dùng lại GUID của frame demo. Prefab và clip nguồn vẫn phục vụ locomotion. SFX dùng cue WAV nội bộ.
- Thêm 64 điểm đo cho R1 và R2. Tổng 125 frame có dữ liệu đo, thuộc 55 tổ hợp. Điểm neo còn lại cần đo riêng, đặc biệt là trail của từng bóng R3 và các hướng chéo của Q/E/F.
- Lượt tới cần thêm khung dồn gối/thu đao khi R1 đáp, bước đổi chân giữa nhát 1–2 của R2, cầu bắt đà lúc R3 tái xuất, dáng trụ ở Bất Động/Bất Diệt và các cầu từ Basic vào tư thế đặc biệt. Cần duyệt trực quan nhịp stop và chất lượng hòa âm trên thiết bị.
- Kết quả kỹ thuật gồm 264/264 lượt lấy mẫu VFX bật/tắt, 264/264 chuỗi Basic vào và ra với hướng chạy khác hướng đánh, 264/264 Replay/reset. Kiểm tra theo đồng hồ ở hai tốc độ cho Q1, Q3 và các lõi mẫu.

## Video ba lõi

| Kỹ năng | Bản 1× | Bản 0,5× |
|---|---|---|
| Thiên Vẫn Trảm · Đao sĩ E | [MP4](player-art/HexcoreVisual/Videos/R1-Thien-Van-Tram-1x.mp4) | [MP4](player-art/HexcoreVisual/Videos/R1-Thien-Van-Tram-05x.mp4) |
| Tam Thiên Kiếm Quyết · Kiếm khí NE | [MP4](player-art/HexcoreVisual/Videos/R2-Tam-Thien-Kiem-Quyet-1x.mp4) | [MP4](player-art/HexcoreVisual/Videos/R2-Tam-Thien-Kiem-Quyet-05x.mp4) |
| Vạn Ảnh Tuyệt Sát · Sát thủ SE | [MP4](player-art/HexcoreVisual/Videos/R3-Van-Anh-Tuyet-Sat-1x.mp4) | [MP4](player-art/HexcoreVisual/Videos/R3-Van-Anh-Tuyet-Sat-05x.mp4) |

[Bảng khung hình ba lõi](player-art/HexcoreVisual/HeroMotionContact.png) và [mức PCM trong video](player-art/HexcoreVisual/HexcoreMovieAudio.csv).

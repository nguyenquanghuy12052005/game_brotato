# Thử kỹ năng hệ Súng

Mở `Assets/Scenes/GunSkillTest.unity` rồi nhấn Play. Súng tự bắn. Dùng WASD để di chuyển và chuột để ngắm. Cả dàn bia hồi sinh sau 0,75 giây kể từ khi bia cuối cùng bị hạ.

Bảng nút cố định bên phải có thể cuộn. Camera bám người chơi trong phần sân bên trái. Khi rê chuột qua bảng, súng giữ hướng ngắm cuối trong sân.

- Chọn một nhánh trong **Nhánh súng**. Chọn **Cơ bản** để về súng thường. Bấm lại một kỹ năng cơ bản đang bật để tắt nó.
- Bấm mỗi nút **Nâng cấp nhánh** để chuyển bậc theo vòng 0 → 1 → 2 → 3 → 0. Bậc được lưu riêng cho mỗi nhánh.
- Trong **Mục tiêu và sân thử**, chọn bia hoặc boss, bật/tắt tự bắn và điều khiển bia. **Bia 100 HP** áp dụng cho lượt sinh bia thường tiếp theo. **Đặt lại sân** đưa người chơi và mục tiêu về trạng thái ban đầu.

## Đạn tỏa

- **Thêm viên.** Bắn 3 viên ở bậc 0 và thêm 1 viên mỗi bậc, tối đa 6. Mỗi phát cách nhau 2,60 giây.
- **Đạn hẹp.** Góc tỏa giảm 15° mỗi bậc, từ 60° xuống 15°.
- **Đẩy lùi.** Mỗi viên đẩy bia còn sống lùi 0,25 đơn vị mỗi bậc theo hướng đạn. Biên sân giữ bia trong tầm bắn.

## Liên thanh

- **Thêm viên.** Bắn 5 viên ở bậc 0 và thêm 2 viên mỗi bậc, tối đa 11. Các viên cách nhau 0,32 giây, nghỉ 1,20 giây sau viên cuối trước khi bắn loạt tiếp theo.
- **Đạn nảy.** Mỗi bậc cho thêm một lần chuyển hướng sang địch sống gần nhất mà viên đạn chưa trúng. Kỹ năng Đạn nảy bảo đảm tối thiểu một lần nảy.
- **Bung tỏa.** Khi đạn hạ địch, bậc 1/2/3 sinh 4/5/6 viên phụ theo các hướng ngẫu nhiên. Kỹ năng Đạn bung toả ở bậc 0 sinh 3 viên phụ. Mỗi viên phụ gây 5 sát thương.

## Xuyên thấu

- **Đạn xuyên.** Mỗi phát cách nhau 0,80 giây. Mục tiêu đầu nhận 10 sát thương. Sau mỗi địch xuyên qua, viên đạn cộng thêm 3 sát thương mỗi bậc.
- **Dấu xuyên.** Địch sống chậm 10/20/30% trong 1,5 giây và đổi sang màu xanh. Bật bia di chuyển để thấy rõ thay đổi tốc độ.
- **Dư chấn.** Tại mục tiêu thứ hai, đạn tạo một sóng gây 5 sát thương cho địch bên cạnh trong bán kính 0,8/1,1/1,4 đơn vị. Mỗi viên tạo một sóng. Sprite xanh ngọc lan ra từ điểm trúng và tan dần trong 0,38 giây.

**Ngắm điểm yếu** phát bốn khung lửa ở đầu đạn. Các hiệu ứng có thể thử cùng nâng cấp của nhánh đang chọn.

## Kỹ năng cấp 11

Chọn một kỹ năng trong nhóm **Lõi đầu**. Bấm lại kỹ năng đang bật để tắt. Sân thử nhớ lựa chọn riêng cho từng nhánh.

| Nhánh | Kỹ năng thứ nhất | Kỹ năng thứ hai | Kỹ năng thứ ba |
|---|---|---|---|
| Đạn toả | **Lưỡi đạn càn quét.** Mỗi phát thứ hai thêm lưỡi năng lượng tầm gần. Tiến sát bia để thử. | **Áp lực công phá.** Hai hoặc ba loạt trúng boss liên tiếp làm vỡ giáp. Bậc đẩy lùi tích áp lực nhanh hơn. | **Phát bắn dồn lực.** Mỗi phát thứ hai gắn dấu. Càng nhiều viên trúng, vụ nổ càng mạnh. |
| Liên thanh | **Kíp nổ liên hoàn.** Đủ 5 lượt đạn trúng cùng bia thì kíp phát nổ. Bậc bung toả tăng sát thương nổ. | **Băng đạn cộng hưởng.** Trúng ít nhất 60% số viên của một loạt vào cùng bia sẽ gọi súng ảo. | **Bão đạn quỹ đạo.** Đủ 8 lượt trúng cùng bia thì các vệ tinh phóng lần lượt về bia. |
| Xuyên thấu | **Vết rách không gian.** Mỗi phát thứ tư để lại vệt gây sát thương theo nhịp. | **Xuyên kích hồi quy.** Mỗi phát thứ tư phóng thương. Thương có thể đánh trúng cùng bia một lần ở mỗi lượt đi và về. | **Thiên thương.** Sau 3 lần trúng cùng bia, thương giáng vào vị trí đã đánh dấu. Boss có thể rời vùng va chạm. |

Thông số là giá trị thử. Hiệu ứng cấp 11 có thể dùng cùng kỹ năng cấp 4 và các bậc nâng cấp đang có. Sát thương phụ từ cấp 11 không nạp lại kỹ năng cấp 11.

## Lõi thứ hai và boss có đòn tấn công

- Chọn **Boss tấn công · 2.500 HP** để thử boss lõi hai. Boss báo hướng trước khi bắn chùm đạn hoặc lao tới. Người chơi có 100 HP. Khi hết máu, sân tự đặt lại sau 1,5 giây. Bấm **Đặt lại sân** để đặt lại ngay hoặc chọn **Bia boss · 1.500 HP** để thử trên mục tiêu dễ hơn.
- Bật hoặc tắt từng kỹ năng trong nhóm **Lõi hai**. Cả hai có thể bật cùng lựa chọn trong **Lõi đầu**.

| Nhánh | Kỹ năng thứ nhất | Kỹ năng thứ hai |
|---|---|---|
| Đạn toả | **Giáp phản pháo.** Hai viên trúng boss trong cự ly gần tạo giáp trước mặt. Ngắm về phía đòn tới để chặn và bắn trả. Thử với boss lõi hai. | **Lưỡi cưa bám đuổi.** Hai viên của mỗi phát thứ hai phải trúng cùng boss. Ở gần boss để cưa tiếp tục gây sát thương. |
| Liên thanh | **Hoả lực leo thang.** Ba loạt có ít nhất ba viên trúng sẽ cường hoá các viên cuối loạt trong 5,5 giây. | **Đạn phân rã.** Tám viên trúng kích hoạt trạng thái phân rã. Tiếp tục bắn để gây thêm sát thương theo nhịp. |
| Xuyên thấu | **Cổng xuyên kích.** Mỗi phát thứ tư trúng boss sẽ mở cặp cổng. Đứng ngoài cặp cổng, bắn qua cổng gần để thấy đạn thoát ở phía bên kia và bay ngược lại. | **Mỏ neo xuyên tâm.** Mỗi phát thứ tư trúng boss đặt neo. Boss đi xa sẽ làm dây đứt. Nếu boss ở lại, neo gây sát thương vùng khi hết hạn. |

Các ngưỡng, thời gian và sát thương là thông số thử. Đạn phản pháo và sát thương phụ từ cưa, phân rã hoặc neo không tự nạp kỹ năng. Đạn xuyên đi qua cổng tối đa một lần.

## Lõi Hexcore · Chiêu cuối cấp 12

Ở đầu bảng bên phải, chọn một chiêu trong **Lõi Hexcore**. Đưa chuột tới vị trí muốn nhắm trong sân rồi nhấn **R** để tung chiêu. Dấu ngắm xanh giữ vị trí khi chuột đi qua bảng. Chiêu cuối có thể dùng cùng các lõi trước. Chọn **Bia boss · 1.500 HP** hoặc **Boss tấn công · 2.500 HP** để dễ thấy sát thương. Có thể tắt **Tự bắn** khi muốn quan sát riêng chiêu cuối.

| Chiêu cuối | Cách thử | Hồi chiêu thử |
|---|---|---:|
| Ngũ bom liên hoàn | Nhắm gần boss để chồng nhiều vùng nổ. Năm quả đáp và nổ lệch nhịp. | 4 giây |
| Trực thăng oanh kích | Nhắm dọc hướng boss di chuyển. Dấu báo xuất hiện trước khi trực thăng bay qua và thả năm quả. | 5 giây |
| Pháo đài cơ động | Nhấn **R** lần hai sau khi hồi chiêu để đặt trụ thứ hai. Trụ ưu tiên boss trong tầm, hoạt động 8 giây sau khi triển khai. Bảng báo đầy nếu cả hai trụ vẫn còn. | 2,4 giây |
| Pháo ray Hexcore | Xếp nhiều mục tiêu trên đường bắn. Pháo tụ lực 0,78 giây rồi bắn xuyên theo hướng đã chọn khi bấm. | 4 giây |
| Lồng đạn Hexcore | Nhắm vào nơi boss sắp đứng. Sáu đỉnh bắn từng cặp vào tâm trong hai vòng. | 5 giây |

Hồi chiêu dùng chung giữa các chiêu trong sân thử. **Đặt lại sân** dọn chiêu đang chạy, dọn trụ và xoá hồi chiêu, đồng thời giữ lựa chọn Hexcore.

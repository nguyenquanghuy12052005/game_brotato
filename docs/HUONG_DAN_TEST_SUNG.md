# Thử kỹ năng hệ Súng

Mở `Assets/Scenes/GunSkillTest.unity` rồi nhấn Play. Súng tự bắn. Dùng WASD để di chuyển và chuột để ngắm. Cả dàn bia hồi sinh sau 0,75 giây kể từ khi bia cuối cùng bị hạ.

- **1 / 2 / 3** chọn Đạn nảy, Ngắm điểm yếu hoặc Đạn bung tỏa. Nhấn lại phím đang chọn để bỏ kỹ năng.
- **4 / 5 / 6** chọn Đạn tỏa, Liên thanh hoặc Xuyên thấu. Nhấn lại phím đang chọn để về súng cơ bản.
- **7 / 8 / 9** lần lượt tăng bậc của ba nâng cấp thuộc nhánh hiện tại. Mỗi nâng cấp chạy qua 0, 1, 2, 3 rồi quay về 0. Cấp đã chọn được giữ riêng cho từng nhánh.
- **Shift+8** bật hoặc tắt tự bắn. **Shift+9** chọn 100 HP hoặc máu mặc định cho lần sinh bia tiếp theo. **Shift+0** bật hoặc tắt bia di chuyển. **0** hoặc **R** đặt lại nhân vật và bia.

## Đạn tỏa

- **7 · Thêm viên.** Bắn 3 viên ở bậc 0 và thêm 1 viên mỗi bậc, tối đa 6. Mỗi phát cách nhau 2,60 giây.
- **8 · Đạn hẹp.** Góc tỏa giảm 15° mỗi bậc, từ 60° xuống 15°.
- **9 · Đẩy lùi.** Mỗi viên đẩy bia còn sống lùi 0,25 đơn vị mỗi bậc theo hướng đạn. Biên sân giữ bia trong tầm bắn.

## Liên thanh

- **7 · Thêm viên.** Bắn 5 viên ở bậc 0 và thêm 2 viên mỗi bậc, tối đa 11. Các viên cách nhau 0,32 giây, nghỉ 1,20 giây sau viên cuối trước khi bắn loạt tiếp theo.
- **8 · Đạn nảy.** Mỗi bậc cho thêm một lần chuyển hướng sang địch sống gần nhất mà viên đạn chưa trúng. Kỹ năng phím 1 bảo đảm tối thiểu một lần nảy.
- **9 · Bung tỏa.** Khi đạn hạ địch, bậc 1/2/3 sinh 4/5/6 viên phụ theo các hướng ngẫu nhiên. Kỹ năng phím 3 ở bậc 0 sinh 3 viên phụ. Mỗi viên phụ gây 5 sát thương.

## Xuyên thấu

- **7 · Đạn xuyên.** Mỗi phát cách nhau 0,80 giây. Mục tiêu đầu nhận 10 sát thương. Sau mỗi địch xuyên qua, viên đạn cộng thêm 3 sát thương mỗi bậc.
- **8 · Dấu xuyên.** Địch sống chậm 10/20/30% trong 1,5 giây và đổi sang màu xanh. Bật bia di chuyển để thấy rõ thay đổi tốc độ.
- **9 · Dư chấn.** Tại mục tiêu thứ hai, đạn tạo một sóng gây 5 sát thương cho địch bên cạnh trong bán kính 0,8/1,1/1,4 đơn vị. Mỗi viên tạo một sóng. Sprite xanh ngọc lan ra từ điểm trúng và tan dần trong 0,38 giây.

Ngắm điểm yếu ở phím **2** phát bốn khung lửa ở đầu đạn. Các hiệu ứng có thể thử cùng nâng cấp của nhánh đang chọn.

## Kỹ năng cấp 11

Bấm **Shift+7** để bật bia boss 1.500 HP, miễn đẩy lùi. Bấm **4 / 5 / 6** để chọn nhánh, sau đó **Shift+1 / Shift+2 / Shift+3** để chọn một kỹ năng. Bấm lại tổ hợp đang chọn để tắt. Game nhớ lựa chọn riêng cho mỗi nhánh. **Shift+0** cho boss di chuyển, **0** hoặc **R** đặt lại vị trí, máu và tiến độ tích hiệu ứng.

| Nhánh | Shift+1 | Shift+2 | Shift+3 |
|---|---|---|---|
| Đạn toả (4) | **Lưỡi đạn càn quét.** Mỗi phát thứ hai thêm lưỡi năng lượng tầm gần. Tiến sát bia để thử. | **Áp lực công phá.** Hai hoặc ba loạt trúng boss liên tiếp làm vỡ giáp. Bậc đẩy lùi tích áp lực nhanh hơn. | **Phát bắn dồn lực.** Mỗi phát thứ hai gắn dấu. Càng nhiều viên trúng, vụ nổ càng mạnh. |
| Liên thanh (5) | **Kíp nổ liên hoàn.** Đủ 5 lượt đạn trúng cùng bia thì kíp phát nổ. Bậc bung toả tăng sát thương nổ. | **Băng đạn cộng hưởng.** Trúng ít nhất 60% số viên của một loạt vào cùng bia sẽ gọi súng ảo. | **Bão đạn quỹ đạo.** Đủ 8 lượt trúng cùng bia thì các vệ tinh phóng lần lượt về bia. |
| Xuyên thấu (6) | **Vết rách không gian.** Mỗi phát thứ tư để lại vệt gây sát thương theo nhịp. | **Xuyên kích hồi quy.** Mỗi phát thứ tư phóng thương. Thương có thể đánh trúng cùng bia một lần ở mỗi lượt đi và về. | **Thiên thương.** Sau 3 lần trúng cùng bia, thương giáng vào vị trí đã đánh dấu. Boss có thể rời vùng va chạm. |

Thông số là giá trị thử. Hiệu ứng cấp 11 có thể dùng cùng kỹ năng cấp 4 và các bậc nâng cấp đang có. Sát thương phụ từ cấp 11 không nạp lại kỹ năng cấp 11.

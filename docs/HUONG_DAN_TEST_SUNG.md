# Thử kỹ năng hệ Súng

Mở `Assets/Scenes/GunSkillTest.unity` rồi nhấn Play. Súng tự bắn. Dùng WASD để di chuyển và chuột để ngắm. Cả dàn bia hồi sinh sau 0,75 giây kể từ khi bia cuối cùng bị hạ.

Bảng nút cố định bên phải có thể cuộn. Camera bám người chơi trong phần sân bên trái. Khi rê chuột qua bảng, súng giữ hướng ngắm cuối trong sân.

- Chọn một nhánh trong **Nhánh súng**. Chọn **Cơ bản** để về súng thường. Bấm lại một kỹ năng cơ bản đang bật để tắt nó.
- Bấm mỗi nút **Nâng cấp nhánh** để chuyển bậc theo vòng 0 → 1 → 2 → 3 → 0. Bậc được lưu riêng cho mỗi nhánh.
- Trong **Mục tiêu và sân thử**, chọn bia hoặc boss, bật/tắt tự bắn và điều khiển bia. **Bia 100 HP** áp dụng cho lượt sinh bia thường tiếp theo. **Đặt lại sân** đưa người chơi và mục tiêu về trạng thái ban đầu.

## Shotgun

- **Thêm viên.** Bắn 3 viên ở bậc 0 và thêm 1 viên mỗi bậc, tối đa 6. Mỗi phát cách nhau 2,60 giây.
- **Đạn hẹp.** Góc tỏa giảm 15° mỗi bậc, từ 60° xuống 15°.
- **Đẩy lùi.** Mỗi viên đẩy bia còn sống lùi 0,25 đơn vị mỗi bậc theo hướng đạn. Biên sân giữ bia trong tầm bắn.

## SMG

Đầu nòng chớp theo từng viên và tắt giữa các phát.

- **Đạn băng.** Bậc 1/2/3 làm chậm 15/30/45% trong 1,5 giây trên mục tiêu trúng đạn. Các phát trúng tiếp theo làm mới thời gian chậm. Đầu đạn có quầng sáng xanh từ ice-lance.png. Đầu nòng dùng chớp ngắn cắt từ frost-breath.png.
- **Ném lựu đạn.** Sau viên cuối của loạt, ném 1/2/3 quả theo bậc 1/2/3 về điểm ngắm trong tầm 2–8 đơn vị. Mỗi quả gây 18 sát thương trong bán kính 1,15 đơn vị khi chạm đất.
- **Thêm đạn.** Bắn 5/7/9/11 viên theo bậc 0/1/2/3. Các viên cách nhau 0,32 giây, nghỉ 1,20 giây sau viên cuối trước khi bắn loạt tiếp theo.

## Rocket

Chớp đầu nòng và khói dùng cannonball.png. Đạn bay dùng rocket-launcher.png và chỉ xuyên qua mục tiêu khi bật kỹ năng Xuyên thấu.

- **Bùng nổ.** Mỗi viên Rocket nổ tại mỗi mục tiêu trúng. Bán kính bậc 1/2/3 là 0,8/1,1/1,4 đơn vị, gây 5/8/11 sát thương lên các mục tiêu xung quanh. Mục tiêu trúng trực tiếp nhận 10 sát thương từ viên đạn. Hiệu ứng nổ dùng rocket-launcher.png.
- **Đẩy lùi.** Mỗi viên đẩy mục tiêu còn sống 0,25 đơn vị mỗi bậc theo hướng đạn. Giới hạn sân và miễn đẩy của boss giống Shotgun.
- **Thêm đạn.** Bắn đồng thời 1/2/3/4 viên ở bậc 0/1/2/3. Các viên cách nhau 5° và mỗi lượt bắn cách nhau 0,80 giây.


Ở nhóm kỹ năng cơ bản, chọn **Đạn nảy**, **Xuyên thấu** hoặc **Đạn bung toả**. Đạn nảy và xuyên giữ hình theo nhánh đang bắn. Xuyên thấu cho đạn đi qua nhiều mục tiêu và chỉ đánh mỗi mục tiêu một lần. Đạn bung toả tạo đạn con dùng hoạt ảnh đạn cơ bản khi hạ mục tiêu.

## Kỹ năng cấp 11

Chọn một kỹ năng trong nhóm **Lõi đầu**. Bấm lại kỹ năng đang bật để tắt. Sân thử nhớ lựa chọn riêng cho từng nhánh.

| Nhánh | Kỹ năng thứ nhất | Kỹ năng thứ hai | Kỹ năng thứ ba |
|---|---|---|---|
| Shotgun | **Lưỡi đạn càn quét.** Mỗi phát thứ hai tạo vệt kiếm xanh chém ngang theo hướng bắn. Tầm chém 5,2 đơn vị và nửa bề rộng tối đa 3,4 đơn vị trước khi tính bậc Đạn hẹp. | **Áp lực công phá.** Tích áp lực từ các loạt trúng rồi tụ một lần và phóng liên tiếp 3 cầu lửa, cách nhau khoảng 0,16 giây. Mỗi cầu lửa nổ và gây rạn giáp trong 5 giây. Bậc đẩy lùi tích áp lực nhanh hơn. | **Phun lửa.** Mỗi lượt Shotgun thứ hai có đạn trúng sẽ phun lửa trong 1,6 giây theo hướng bắn. Luồng giữa phun thẳng, hai luồng bên tạo hình nón. Vùng lửa vươn 5,4 đơn vị, mở 30° mỗi bên và chia tổng sát thương thành tám nhịp. Trúng nhiều viên vào mục tiêu kích hoạt sẽ tăng tổng sát thương. |
| SMG | **Kíp nổ liên hoàn.** Đủ 5 lượt đạn trúng cùng bia thì tên lửa phép từ arcane-missiles.png bay từ nhân vật rồi hội tụ và nổ tại bia. Hiệu ứng hiển thị ở tỷ lệ 200%. Bậc Ném lựu đạn tăng sát thương kíp nổ. | **Băng đạn cộng hưởng.** Trúng ít nhất 60% số viên của một loạt vào cùng bia sẽ gọi hai súng phụ dùng weapon_chaser_02.png ở hai bên nhân vật trong 3 giây. Cả hai cùng bắn laser, và bậc Thêm đạn tăng số nhịp bắn phụ. | **Hồi máu.** Đủ 8 viên trúng cùng bia sẽ gọi slime theo nhân vật trong 5 giây. Vòng xanh hiện dưới chân, tim hiện trên người, và hồi 4 HP mỗi giây (tối đa 20 HP mỗi lần gọi). |
| Rocket | **Vết rách không gian.** Mỗi phát thứ tư tạo một vệt lửa Quake dọc đường bắn, gây sát thương liên tục theo nhịp. | **Xuyên kích hồi quy.** Loạt thứ tư trúng mục tiêu tạo một quả cầu sét trôi chậm quanh mục tiêu trong 2,4 giây, đổi hướng ngẫu nhiên. Mỗi 0,3 giây quả cầu giật một tia sét phân nhánh vào mục tiêu. | **Thiên thương.** Sau 3 lần trúng cùng bia, vòng phép từ magic-circle-cast.png xuất hiện và phóng cột sáng tại vị trí đã đánh dấu. Boss có thể rời vùng va chạm. |

Thông số là giá trị thử. Hiệu ứng cấp 11 có thể dùng cùng kỹ năng cấp 4 và các bậc nâng cấp đang có. Sát thương phụ từ cấp 11 không nạp lại kỹ năng cấp 11.

## Lõi thứ hai và boss có đòn tấn công

- Chọn **Boss tấn công · 2.500 HP** để thử boss lõi hai. Boss báo hướng trước khi bắn chùm đạn hoặc lao tới. Người chơi có 100 HP. Khi hết máu, sân tự đặt lại sau 1,5 giây. Bấm **Đặt lại sân** để đặt lại ngay hoặc chọn **Bia boss · 1.500 HP** để thử trên mục tiêu dễ hơn.
- Bật hoặc tắt từng kỹ năng trong nhóm **Lõi hai**. Cả hai có thể bật cùng lựa chọn trong **Lõi đầu**.

| Nhánh | Kỹ năng thứ nhất | Kỹ năng thứ hai |
|---|---|---|
| Shotgun | **Giáp phản pháo.** Hai viên trúng boss trong cự ly gần tạo khiên vàng bao quanh nhân vật. Khiên chặn đòn từ mọi hướng rồi bắn trả theo hướng ngắm. Animation khiên tách riêng khỏi đạn phản pháo. | **Lưỡi cưa bám đuổi.** Hai viên của mỗi phát thứ hai phải trúng cùng boss. Đĩa năng lượng từ energy-disc.png quay quanh boss ba vòng trong 2,4 giây. Ở gần boss để đĩa tiếp tục gây sát thương. |
| SMG | **Hoả lực leo thang.** Ba loạt có ít nhất ba viên trúng sẽ đổi toàn bộ đạn sang plasma trong 5 giây. Plasma trúng quái gây độc trong 4 giây, mất 3 HP mỗi 0,5 giây và chậm dần từ 10% đến 45%. Trúng lại gia hạn độc. Vòng độc chỉ dùng phần hiệu ứng dưới chân từ poisoned.png. | **Đạn phân rã.** Tám viên trúng kích hoạt trạng thái phân rã. Tiếp tục bắn để gây thêm sát thương theo nhịp. |
| Rocket | **Cổng xuyên kích.** Mỗi phát thứ tư trúng boss mở hai cổng trước và sau boss trong 2,5 giây. Tia tím nối hai cổng, gây 8 sát thương mỗi 0,25 giây lên quái chạm tia. Đạn có Xuyên thấu chạm cổng gần vẫn thoát ở cổng bên kia và bay ngược lại. | **Mỏ neo xuyên tâm.** Mỗi phát thứ tư trúng boss đặt cuộn Tesla và nối dây sét. Boss đi xa hơn 2,65 đơn vị làm dây đứt, neo nổ và boss choáng 1,25 giây với sao trên đầu. Nếu boss ở lại, neo gây sát thương vùng khi hết hạn. |

Các ngưỡng, thời gian và sát thương là thông số thử. Đạn phản pháo và sát thương phụ từ cưa, phân rã hoặc neo không tự nạp kỹ năng. Đạn xuyên đi qua cổng tối đa một lần.

## Lõi Hexcore · Chiêu cuối cấp 12

Ở đầu bảng bên phải, chọn một chiêu trong **Lõi Hexcore**. Đưa chuột tới vị trí muốn nhắm trong sân rồi nhấn **R** để tung chiêu. Dấu ngắm xanh giữ vị trí khi chuột đi qua bảng. Chiêu cuối có thể dùng cùng các lõi trước. Chọn **Bia boss · 1.500 HP** hoặc **Boss tấn công · 2.500 HP** để dễ thấy sát thương. Có thể tắt **Tự bắn** khi muốn quan sát riêng chiêu cuối.

| Chiêu cuối | Cách thử | Hồi chiêu thử |
|---|---|---:|
| Lốc lửa | Nhấn **R** để tạo lốc băng đi thẳng theo hướng ngắm trong 7 giây. Lốc hút quái trong bán kính 1,8 đơn vị và gây 6 sát thương mỗi 0,5 giây. | Tích tối đa 3 lượt, hồi 1 lượt mỗi 2,4 giây |
| Mưa thiên thạch | Một lần R gọi 5 thiên thạch liên tiếp trong vùng quanh điểm ngắm, cách nhau 0,22 giây. Mỗi quả gây sát thương diện rộng khi chạm đất. | 5 giây |
| Pháo đài cơ động | Ngắm xuống sàn rồi nhấn **R** để đặt trụ trong tầm ngắm. Tối đa 3 trụ, mỗi trụ tồn tại 15 giây và phát sóng plasma gây 11 sát thương trong bán kính 4 đơn vị mỗi 0,5 giây. | 2,4 giây |
| Lồng đạn Hexcore | Nhắm vào nơi boss sắp đứng. Sáu đỉnh bắn từng cặp vào tâm trong hai vòng. | 5 giây |

Bom hồi tuần tự từng lượt, tối đa 3 quả, và vẫn hồi khi chọn chiêu khác. Các chiêu khác dùng hồi chiêu chung của sân thử. **Đặt lại sân** dọn chiêu đang chạy, dọn trụ và xoá hồi chiêu, đồng thời giữ lựa chọn Hexcore.


## Asset pixel đã duyệt — 18/11/2026

Mở Assets/Scenes/GunSkillTest.unity rồi Play. Chọn nhánh trên bảng bên phải; bật lần lượt Lưỡi đạn càn quét, Kíp nổ liên hoàn hoặc Xuyên kích hồi quy ở lõi thứ nhất. Ở lõi thứ hai, thử Lưỡi cưa bám đuổi, Hoả lực leo thang, Đạn phân rã và Mỏ neo xuyên tâm trên boss; chúng vẫn cùng tồn tại với lõi thứ nhất. Chọn Lồng đạn Hexcore rồi nhấn R, ngắm về phía boss. Cơ chế và hồi chiêu giữ như trước.

Quan sát lửa/khói pixel mới, đạn hồi quy quay đầu mà không lật hai lần, lửa đầu nòng tách khỏi đạn cường hoá, vụn phân rã chỉ bật theo hit và neo đứng thẳng khi dây pixel kéo căng. Thử chuyển nhánh/reset khi đang có hiệu ứng để kiểm tra dọn sạch. Bản duyệt và manifest atlas ở public/new_asset/pixel-skills-v2; runtime art nằm Assets/Art/GunSkills/PixelSkills, đã nối scene nên không cần kéo asset bằng tay.

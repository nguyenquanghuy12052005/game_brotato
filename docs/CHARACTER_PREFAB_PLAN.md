# Kế hoạch ảnh animation cho boss Map 2–4

Trạng thái cập nhật: **ba ảnh đứng, ba prefab và ba sprite sheet bản nháp đã có; VFX, Unity slicing và animation runtime chưa làm**.  
Hạn mục tiêu: hoàn tất phần ảnh trước **01/11/2026**.

## 1. Mục tiêu hiện tại

Tạo ảnh animation cho ba boss, dùng các ảnh trong `public/{normal_enemy,bow_enemy,elite_boss}` làm mẫu cách trình bày động tác:

1. Map 2 — **Skeleton Swordsman**.
2. Map 3 — **Lava Golem** thiết kế nguyên bản, không sao chép nhân vật Genshin Impact.
3. Map 4 — **Yeti**.

Đây là task **ảnh animation**, không phải task gameplay. Ảnh đứng hiện tại được dùng làm thiết kế gốc để giữ nhân vật nhất quán giữa các frame.

## 2. Phạm vi được làm

- Tạo các tư thế Idle, di chuyển, tung chiêu, trúng đòn và chết.
- Tạo sprite sheet PNG có nền trong suốt thật.
- Giữ màu sắc, trang phục, tỷ lệ và vũ khí nhất quán giữa các frame.
- Căn chân trên cùng một baseline để animation không bị giật vị trí.
- Kiểm tra sheet chia được thành lưới đều trong Unity.
- Tạo các ảnh VFX tối thiểu sau khi ba sheet nhân vật đã được duyệt.
- Có thể import/slice thử trong Unity để kiểm tra, nhưng chưa tạo gameplay logic.

## 3. Ngoài phạm vi

- AI truy đuổi, né hoặc chọn mục tiêu.
- C# điều khiển dash, nhảy, xoay kiếm hoặc tung chiêu.
- Sát thương, hồi máu, đóng băng, làm chậm, buff và cooldown.
- Cơ chế 2 mạng/3 mạng, đổi phase hoặc reset HP.
- Random vị trí hố dung nham và thời gian tồn tại hiệu ứng.
- Animator Controller, file `.anim` và state machine, trừ khi được yêu cầu riêng sau khi ảnh hoàn tất.
- Đặt boss vào GameScene hoặc thay đổi hệ thống wave.

## 4. Chuẩn file bắt buộc

- Mỗi frame nằm trong ô **256 × 256 px**.
- Mỗi động tác dùng **4 frame**, đọc từ trái sang phải như mẫu trong `public`.
- Sheet rộng cố định **4 cột = 1024 px**.
- Số hàng bằng số động tác của từng nhân vật.
- PNG RGBA, nền trong suốt thật; không dùng nền caro giả.
- Nhân vật nằm giữa ô, đủ đầu và chân, không chạm mép.
- Hướng nhìn mặc định sang phải; khi cần nhìn trái, Unity có thể dùng `Flip X`.
- Không nhét nhiều kích thước ô khác nhau vào cùng một sheet.

Các ảnh trong `public` chỉ là mẫu bố cục. Chúng có nền caro bị dính cứng và kích thước không chia đều hoàn hảo, nên không dùng trực tiếp làm sheet cuối.

## 5. Danh sách animation

### Skeleton Swordsman — 4 cột × 8 hàng = 1024 × 2048

| Hàng | Tên | Nội dung 4 frame |
|---:|---|---|
| 0 | Idle | Đứng thở/chuyển trọng tâm nhẹ |
| 1 | Run | Chạy tiếp cận Player |
| 2 | Skill1_Dash | Thu người → lao tới → chém/lướt qua → hãm lại |
| 3 | Skill2_SpinSlash | Lấy đà → xoay kiếm → vòng chém lớn → kết thúc |
| 4 | Skill3_Jump | Chùng chân → bật nhảy → trên không → chuẩn bị đáp |
| 5 | Skill3_Roar | Tiếp đất → gồng → gầm → hồi về tư thế đứng |
| 6 | Hurt | Bị đánh và hồi tư thế |
| 7 | Death | Khựng lại → ngã → nằm xuống → bất động |

Ảnh VFX làm sau: vòng chém tròn và sóng gầm.

### Lava Golem — 4 cột × 7 hàng = 1024 × 1792

| Hàng | Tên | Nội dung 4 frame |
|---:|---|---|
| 0 | Idle | Đá/dung nham chuyển động nhẹ |
| 1 | Walk | Bước đi nặng nề |
| 2 | Skill1_ConeFire | Hít/lấy đà → cúi người → phun lửa → dừng |
| 3 | Skill2_Heal | Gồng người → lõi sáng → hồi phục → giảm sáng |
| 4 | Skill3_Throw | Tạo cầu lửa → giơ lên → ném → thu tay |
| 5 | Hurt | Bị đánh, khe dung nham chớp sáng |
| 6 | Death | Nứt vỡ → quỳ → sụp xuống → bất động |

Ảnh VFX làm sau: lửa hình nón, cầu lửa lớn, cầu lửa nhỏ, hố dung nham và vòng hồi máu.

### Yeti — 4 cột × 7 hàng = 1024 × 1792

| Hàng | Tên | Nội dung 4 frame |
|---:|---|---|
| 0 | Idle | Đứng thở/chuyển động lông nhẹ |
| 1 | Walk | Bước đi nặng |
| 2 | Skill1_Throw | Tạo/cầm tuyết → lấy đà → ném → thu tay |
| 3 | Skill2_FreezeCast | Giơ tay → tụ băng → niệm → kết thúc |
| 4 | Skill3_BlizzardCast | Gồng → tăng sức mạnh → gọi bão → giữ tư thế |
| 5 | Hurt | Bị đánh và lùi người |
| 6 | Death | Mất thăng bằng → quỳ → ngã → bất động |

Ảnh VFX làm sau: viên tuyết, vòng cảnh báo, vòng đóng băng quanh Player và bão tuyết toàn màn hình.

## 6. Cấu trúc đầu ra

    Assets/Prefabs/Enemy/
    ├── Skeleton/
    │   ├── skeleton.png
    │   ├── SkeletonBoss.prefab
    │   └── Animations/Skeleton_Animations.png
    ├── LavaGolem/
    │   ├── lava_golem.png
    │   ├── LavaGolemBoss.prefab
    │   └── Animations/LavaGolem_Animations.png
    └── Yeti/
        ├── yeti.png
        ├── YetiBoss.prefab
        └── Animations/Yeti_Animations.png

VFX chỉ thêm vào thư mục `VFX/` của từng nhân vật sau khi sheet nhân vật được duyệt. Unity tự tạo `.meta`; phải commit `.meta` cùng asset.

## 7. Trình tự thực hiện

### Giai đoạn A — Skeleton trước

1. Dùng `skeleton.png` hiện tại làm ảnh nhận diện gốc.
2. Làm từng hàng theo thứ tự: Idle → Run → Skill1 → Skill2 → Skill3 Jump → Skill3 Roar → Hurt → Death.
3. Sau mỗi hàng, kiểm tra bốn frame có cùng khuôn mặt, trang phục, kiếm, tỷ lệ và baseline.
4. Ghép thành `Skeleton_Animations.png` đúng 1024 × 2048.
5. Kiểm tra nền trong suốt và Unity slice Grid by Cell Size 256 × 256 được đúng 32 frame.
6. Dừng để người dùng duyệt trước khi làm Lava Golem.

### Giai đoạn B — Lava Golem

Lặp lại quy trình với 7 hàng/28 frame; ưu tiên độ nhất quán của lõi dung nham, khe nứt và tỷ lệ thân lớn. Dừng để duyệt.

### Giai đoạn C — Yeti

Lặp lại quy trình với 7 hàng/28 frame; giữ lông trắng/xanh nhưng phải đọc rõ trên nền Tundra. Dừng để duyệt.

### Giai đoạn D — VFX tối thiểu

Chỉ bắt đầu sau khi ba sheet nhân vật được duyệt. Mỗi VFX dùng strip 4 frame nền trong suốt. Không làm logic kích hoạt hoặc gây sát thương.

## 8. Lịch mục tiêu

- **29/10:** cập nhật chuẩn sheet và hoàn thành/duyệt Skeleton.
- **30/10:** hoàn thành/duyệt Lava Golem.
- **31/10:** hoàn thành/duyệt Yeti; làm VFX quan trọng nhất nếu còn thời gian.

Nếu không đủ thời gian, ưu tiên theo thứ tự: ba sheet nhân vật → VFX projectile/vòng cảnh báo → hiệu ứng toàn màn hình. Không viết logic gameplay trong task này.

## 9. Điều kiện hoàn thành

- Có đủ ba sheet đúng kích thước và đúng số frame.
- Nền alpha trong suốt thật, không có nền caro dính vào ảnh.
- Không frame nào bị cắt đầu, chân, vũ khí hoặc chạm mép ô.
- Nhân vật không đổi màu, mặt, trang phục, tỷ lệ hoặc hướng nhìn giữa các frame.
- Chân nằm trên baseline ổn định; chạy thử tuần tự không bị nhảy vị trí.
- Các động tác skill nhìn vào là phân biệt được dù chưa có logic.
- Unity slice theo ô 256 × 256 đúng, không dư pixel và không lệch lưới.
- Không thay đổi GameScene, C# gameplay hoặc package.

## 10. Bước tiếp theo

1. Người dùng duyệt trực quan cả ba master sheet.
2. Sau khi được duyệt, import/slice thử trong Unity theo Grid by Cell Size 256 × 256 và kiểm tra thứ tự frame.
3. Chỉ khi được yêu cầu tiếp mới tạo VFX riêng cho projectile, vòng cảnh báo, vòng chém, sóng gầm, hố dung nham và bão tuyết.
4. Animator, `.anim` và logic gameplay vẫn ngoài phạm vi hiện tại.

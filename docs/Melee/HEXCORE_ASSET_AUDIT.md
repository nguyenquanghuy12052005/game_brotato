# HEXCORE Skill Asset Demo · Đợt 0

Đặc tả đọc từ bản đính kèm và lưu nguyên văn tại [HEXCORE_SKILL_ASSET_DEMO_PLAN.md](HEXCORE_SKILL_ASSET_DEMO_PLAN.md). Phạm vi dài hạn là Q (3), E (9), R (3), F (9). Ba R dùng chung cho cả ba nhánh. Audit này chỉ mở một bài thử pipeline Q1 · Kiếm Lướt / Phong Ảnh Trảm.

## Kiểm kê và ma trận tương thích

Các mã form bên dưới là ánh xạ theo bốn prefab đã tìm thấy. Cột phong cách chỉ mô tả vũ khí và động tác nguồn thực tế. Tổ hợp form × phong cách khác và các lõi R dùng chung cần kiểm tra animation riêng.

| Form | Prefab nguồn | Phong cách có dữ liệu | Idle / Run / Attack / Death mỗi hướng | Canvas, mốc chân từ trên | Tình trạng combo |
|---|---|---|---|---|---|
| 01 | `SwordsmanVisual` | Kiếm sĩ, kiếm thép một tay | 4 / 5 / 6 / 4 | 256×256, y208 | Có một cú quét, thiếu nhát ngược và pose nối |
| 02 | `AssassinVisual` | Sát thủ, dao găm | 4 / 5 / 5 / 4 | 256×256, y208 | Có cú đâm, thiếu hai nhát mở và pose nối |
| 03 | `DaoWarriorVisual` | Đao sĩ, đại đao cong | 4 / 5 / 8 / 4 | 320×320, y240 | Có cú bổ chéo nặng, thiếu nhát ngang và nối sang bổ dọc |
| 04 | `QiSwordsmanVisual` | Kiếm khí, kiếm mảnh | 4 / 5 / 7 / 4 | 320×320, y240 | Có cú chém và `QiWave` riêng, thiếu nhát mở và pose nối |

- Mỗi prefab nối SpriteRenderer với Animator của chính form. Có 312 PNG nhân vật, 64 clip theo hướng N/E/S/W và một clip `QiWave` riêng. Tất cả dùng PPU 120, pivot tâm canvas. Frame chuẩn có chân cách tâm 80 px, tương đương 0,667 Unity unit. Bốn hướng chéo, Turn, Hurt và các bộ pose chuyên biệt chưa xuất hiện trong các gói form này.
- Controller nguồn dùng state theo tên và clip thay sprite. `Attack_Right` của Kiếm sĩ chỉ có đường cong sprite, không có root motion, điểm gắn vũ khí hay animation event. Tay, kiếm và cung chém ở các frame 02–03 được vẽ chung. Tắt VFX rời không thể ẩn cung chém đó. README ghi hàng Attack/Death trái của Kiếm sĩ được lật từ nguồn, nên cần duyệt tay kiếm và giáp lệch vai ở W trước khi dùng cho bản chính thức.
- `MeleeDemo` có 15 trình diễn hiện có, bốn prefab form và dummy lấy từ Enemy. Có thể dùng mô hình một đồng hồ preview, lấy mẫu Animator theo tên và dùng cùng prefab. Danh mục, choreography và VFX cũ cần so theo từng chiêu của đặc tả mới. Ví dụ Tam Thiên hiện là tụ rồi phóng quạt, đặc tả mới cần ba nhát nối tiếp có pose riêng.
- `ProjectSettings/ProjectVersion.txt` và Unity cài trên máy cùng ghi **6000.3.25f1**. Kiểm tra này lấy phiên bản từ project thực tế.

## Nguồn dùng cho bài thử

| Nguồn | Dấu vết | Cách dùng |
|---|---|---|
| Sprite, clip và prefab Kiếm sĩ | `Assets/Prefabs/Player/Melee/Swordsman`, prompt ImageGen trong `docs/Melee/SwordsmanFull/prompts.json` | Đọc trực tiếp asset nội bộ của repo |
| Sprite/material dummy | `Enemy.prefab` → `spider.png`. SHA-256 trùng ảnh gốc trong gói Kenney Platformer Art (more enemies and animations) của Kenney Vleugels | CC0 theo `kenney_platformer-art-extended-enemies/license.txt`. Ghi công tùy chọn. Bài thử chỉ đổi kích thước hiển thị và màu phản hồi |
| Tia va chạm | LineRenderer dựng trực tiếp trong cảnh bằng material nội bộ | Placeholder để kiểm tra timing. Chưa có SFX Q1 |

PixelFX là nguồn tham khảo trong đặc tả. Bài thử không nhập PixelFX hoặc gói âm thanh. Với mỗi đợt nhập sau, ghi URL trang asset, tác giả, ngày lấy, giấy phép và ghi công trước khi đưa vào Unity.

## Yêu cầu sản xuất artwork

Mọi tọa độ pixel tính từ góc trên trái. Giữ tay kiếm, giáp lệch và phụ kiện theo form gốc. Vẽ điểm neo tay và đầu lưỡi theo từng khung, ghi điểm chân trụ để khóa vị trí đất. Khung hướng chéo cần góc nhìn riêng trong những pose làm đảo tay hoặc đổi phối cảnh.

| Hạng mục | Hướng / số frame đề xuất mỗi hướng | Key pose và điểm gắn |
|---|---|---|
| Idle / Run / Turn | NE/SE/SW/NW: 4 Idle, 5 Run. Turn 2–3 cho mỗi cặp hướng kề nhau cần nối | Giữ canvas và mốc chân theo form. Run có đặt chân, bước qua, đổi chân. Turn bắt đầu từ chân trụ và xoay thân/kiếm |
| Basic Kiếm sĩ | 8 hướng. 6–8 frame nhát ngược, 2–3 frame nối mỗi đoạn Run → Basic, Basic → Run/Q1 và Q1 → Basic | Khung chém ngang kết thúc ở bên đối diện thế đầu nhát ngược. Nối vai/đổi chân rồi quét ngược. Tách cung chém khỏi sprite thân. Neo nắm kiếm, mũi kiếm và chân trụ |
| Q1 Kiếm Lướt | E trước, sau đó 7 hướng còn lại. 10–12 frame, 256×256, PPU 120, pivot tâm, mốc chân x128/y208 | 2 hạ trọng tâm → 2 đạp và rướn → 3 vung, chạm, theo đà → 3–5 hãm và thu kiếm. Neo bàn chân đạp, bàn chân hãm, nắm kiếm, đầu lưỡi và điểm chạm mục tiêu |
| Q1 VFX/SFX | VFX vệt kiếm 5–7 frame, bụi chân và tia chạm gồm 4–6 frame mỗi loại. SFX ngắn tại đạp, vung và chạm | Bụi tại điểm chân đạp, vệt giữa nắm và đầu lưỡi, tia ở dummy khi lưỡi chạm. Ưu tiên asset trên nền trong suốt, xem ở 1x và 0,5x |

Storyboard Q1 hướng Đông trong blockout: giữ pose lấy đà 0–0,14 s, tiến về trước từ 0,14 s, thay sang pose vung trước va chạm ở 0,34 s, hãm vị trí đến 0,56 s, thu tới 0,86 s. Các mốc dành cho review pipeline và sẽ căn lại theo frame sản xuất. Sprite chạy thế cho đạp/lướt và đòn nguồn thế cho động tác vung/thu. Nhát Basic trước và sau là cùng clip nguồn.

Mở `Assets/Scenes/HexcoreSkillPreview.unity` và nhấn Play để xem chuỗi mặc định. Dùng các nút trên cùng để xem từng phần, chuyển 0,5x, tạm dừng, phát lại hoặc tắt phản hồi va chạm. Unity 6000.3.25f1 đã chạy Play Mode trên bản sao của project và xác nhận nhịp impact đúng sprite swing, Idle ở điểm dừng, Phát lại reset vị trí và hiệu ứng. Bố cục và chất lượng chuyển động trong Game View cần duyệt trực quan.

## Thứ tự triển khai tiếp

1. Chốt pose Q1 hướng E và nhát chém ngược trên Form 01 ở kích thước chơi. Kiểm tra với VFX rời tắt.
2. Duyệt Idle/Run/Turn và Basic ở đủ tám hướng trước khi chốt tương thích Q1. Thêm metadata chân/kiếm, sau đó thử đổi hướng di chuyển và hướng ngắm độc lập.
3. Dựng storyboard đúng tên cho 24 chiêu rồi ưu tiên ba lõi R. R1 cần bật/đáp, R2 cần ba swing tăng lực, R3 cần sprite động của form nguồn cho từng dư ảnh. Duyệt lại ma trận theo form, hướng và kỹ năng trước khi polish Q/E/F.

# HEXCORE · Nguồn asset và quyền sử dụng

Đợt visual polish dùng 11 atlas ImageGen dựa trên form gốc của dự án. Atlas chéo của ba lõi và atlas hai vòng Cuồng Phong Đao nằm ở [Source](player-art/HexcoreVisual/Source). [Prompt và mô tả nguồn](player-art/HexcoreVisual/Source/prompts.json) ghi công cụ và yêu cầu từng atlas. VFX vệt, cung, xoắn và nứt sử dụng LineRenderer với material preview hiện có. MP4 ghi âm các cue WAV nội bộ khi Unity phát thật.

| Asset | Nguồn | Quyền / ghi công |
|---|---|---|
| Bốn form và clip N/E/S/W | Các prefab và frame dưới `Assets/Prefabs/Player`. Prompt và atlas nội bộ trong `docs/player-art` | Artwork có sẵn của dự án. Dùng làm tham chiếu và tái sử dụng trực tiếp trong demo |
| Sprite chéo, Basic, Q1 và ba lõi R bổ sung | ImageGen tích hợp dựa trên các frame gốc của từng form. [Atlas, GIF và prompt](player-art/HexcoreProduction) | Asset mới tạo cho dự án, không nhập sprite nhân vật của bên thứ ba. Sheet có nền/tay hoặc tư thế sai hướng bị loại trước khi dựng clip |
| Q1 Tây Bắc, bộ xoay Q2, Q3 ngoài hướng Đông, các biến thể E/F và lõi R ở hướng khác E/W | ImageGen với ảnh từng form trong repo. [Atlas nguồn](player-art/HexcoreFinal) và [prompt](player-art/HexcoreFinal/prompts.json) | Asset mới của dự án. Sprite tách nền, chuẩn hóa theo form bằng công cụ xuất PNG nội bộ. Bản R1 chéo Sát thủ có nền xám đã thay bằng bản nền trong. Bản R1 N frame nén của Kiếm sĩ có bản sửa riêng |
| VFX MeleeSkills và MeleeUltimates | Asset nội bộ đang dùng trong `MeleeDemo` | Dùng trên timeline mới. Thiên Vẫn lấy đoạn đáp đất, Vạn Ảnh lấy đoạn kết liễu; Tam Thiên dùng `QiWave` để phóng từng nhát |
| Vệt lưỡi, bụi, tia va chạm, kiếm rơi và vòng tụ lực | LineRenderer và `PlayerUnlit.mat` từ repo | Hiệu ứng code trong scene review |
| 13 cue WAV | Thiết kế bằng tổng hợp PCM có seed, bộ lọc và bao âm tại [render_sfx.py](player-art/HexcoreRemaining/render_sfx.py). Tệp ở `Assets/Audio/HexcorePreview` | Âm mới cho repo, không có sample ngoài. Cue áp dụng theo frame hoặc pha; cần xác nhận âm lượng bằng Game View |
| Dummy nhện | Kenney Vleugels, gói Platformer Art (more enemies and animations) | CC0, ghi công tùy chọn. SHA-256 đã đối chiếu với `kenney_platformer-art-extended-enemies`, xem [audit](HEXCORE_ASSET_AUDIT.md) |

[PixelFX](https://pixelfx.io/effects/) là tài liệu tham khảo. Repo dùng lại VFX và 13 cue WAV có sẵn cho đợt này.

Các nguồn Sprite bổ sung nằm trong [HexcoreRemaining](player-art/HexcoreRemaining) và [bộ prompt](player-art/HexcoreRemaining/prompts.json). Sử dụng ImageGen tích hợp với prefab gốc làm tham chiếu. Hai atlas Q1 NW không vượt kiểm tra nhất quán tay kiếm/giáp vai. Dao sĩ R2 W được tạo lại sau khi phát hiện lẹm đầu đao. F2 Kiếm khí được tạo lại để thanh kiếm luôn ở tay, còn projectile là lớp riêng.

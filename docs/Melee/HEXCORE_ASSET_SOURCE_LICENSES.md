# HEXCORE · Nguồn asset và quyền sử dụng

Đợt tinh chỉnh sau Rev2 dùng lại Dagger Flurry cho hai cung Ám Bộ, Thrust cho luồng sáng sau dao giữa của Song Ảnh và Heavy Cleave cho lớp hạt sau mép Hoành Thiên. Cả ba dùng commit PixelFX đã lưu bên dưới. Hai sprite clip bổ sung nằm trong atlas hiện có. [Recipe](Source/PixelFX/render_polish_samples.py)

Rev2 tiếp tục dùng 10 nguồn đã lưu ở mục bên dưới để phát triển thêm lớp texture cho đòn kết Sát thủ, quạt Đao, ấn, khí xoáy và sóng Kiếm khí. Sprite kiếm có chuôi, vết rift và đường phản được phát triển trong cùng recipe. Không có lượt nhập nguồn hoặc giấy phép mới. Atlas dùng chung giữ nguyên đường dẫn và GUID. [Recipe Rev2](Source/PixelFX/render_polish_samples.py)

## v2.1 · Nguồn PixelFX được chọn cho tám mẫu

[Nguồn lưu trong repo](Source/PixelFX/Upstream) theo commit `5294518ce7c72e48b6100a744596e0acf57f2826`. Chín cặp PNG/JSON được dùng là Sword Slash, Dagger Flurry, Thrust, Heavy Cleave, Spin Slash, Ground Slam, Energy Disc, Hex Sigil và Dark Explosion. Hit Spark từ đợt tích hợp đầu tiếp tục được dùng. Có 12 tệp JS tham khảo, gồm mã của mười effect trên, Dash Strike và Smoke Bomb. [Mã nguồn tại commit đã pin](https://github.com/patch-gg/pixelfx/tree/5294518ce7c72e48b6100a744596e0acf57f2826)

[Recipe](Source/PixelFX/render_polish_samples.py) phối các lớp chọn lọc vào một atlas demo với 12 clip. Chỉ atlas và metadata timing được Unity import. Khi phân phối atlas, kèm [MIT](Source/PixelFX/Upstream/LICENSE), [notices upstream](Source/PixelFX/Upstream/NOTICE.md) và [notice Hit Spark](Source/PixelFX/NOTICE.txt). Bộ nhập không chứa font Patch Pixel. [Điều khoản PixelFX](https://pixelfx.io/licenses/)

### v2.1 · Lần nhập Hit Spark đầu tiên

Mẫu v2.1 phát một asset dẫn xuất từ PixelFX `impacts/hit-spark`. Giữ [PNG và JSON export v1](Source/PixelFX/HitSpark), [giấy phép MIT](Source/PixelFX/LICENSE.txt) và [notices](Source/PixelFX/NOTICE.txt) kèm khi phân phối bản dựng có atlas. [Trang effect](https://pixelfx.io/effects/impacts/hit-spark/) · [Điều khoản ghi công](https://pixelfx.io/licenses/)

- SHA-256 PNG nguồn là `450d586632894bfd17af704d0c90b553f9620832cad9d30b0090ca94051c9368`.
- SHA-256 JSON nguồn là `a09b3924b0b7feaf215b70eb2966b8c499a7fc88844b87915bc33b5ab5978b6a`. Metadata ghi 16 frame 136×80 px, pivot 44×64 px từ góc trên trái và 33 ms/frame.
- Recipe ở lần nhập đầu lấy cả 16 frame, giảm cỡ nearest-neighbor và căn pivot về cùng ô. Pose và WAV lấy từ dự án.

## Các đợt trước v2.1

Đợt visual polish dùng 11 atlas ImageGen dựa trên form gốc của dự án. Atlas chéo của ba lõi và atlas hai vòng Cuồng Phong Đao nằm ở [Source](HexcoreVisual/Source). [Prompt và mô tả nguồn](HexcoreVisual/Source/prompts.json) ghi công cụ và yêu cầu từng atlas. VFX vệt, cung, xoắn và nứt sử dụng LineRenderer với material preview hiện có. MP4 ghi âm các cue WAV nội bộ khi Unity phát thật.

| Asset | Nguồn | Quyền / ghi công |
|---|---|---|
| Bốn form và clip N/E/S/W | Các prefab và frame dưới `Assets/Prefabs/Player`. Prompt và atlas nội bộ trong `docs/player-art` | Artwork có sẵn của dự án. Dùng làm tham chiếu và tái sử dụng trực tiếp trong demo |
| Sprite chéo, Basic, Q1 và ba lõi R bổ sung | ImageGen tích hợp dựa trên các frame gốc của từng form. [Atlas, GIF và prompt](HexcoreProduction) | Asset mới tạo cho dự án, không nhập sprite nhân vật của bên thứ ba. Sheet có nền/tay hoặc tư thế sai hướng bị loại trước khi dựng clip |
| Q1 Tây Bắc, bộ xoay Q2, Q3 ngoài hướng Đông, các biến thể E/F và lõi R ở hướng khác E/W | ImageGen với ảnh từng form trong repo. [Atlas nguồn](HexcoreFinal) và [prompt](HexcoreFinal/prompts.json) | Asset mới của dự án. Sprite tách nền, chuẩn hóa theo form bằng công cụ xuất PNG nội bộ. Bản R1 chéo Sát thủ có nền xám đã thay bằng bản nền trong. Bản R1 N frame nén của Kiếm sĩ có bản sửa riêng |
| VFX MeleeSkills và MeleeUltimates | Asset nội bộ đang dùng trong `MeleeDemo` | Dùng trên timeline mới. Thiên Vẫn lấy đoạn đáp đất, Vạn Ảnh lấy đoạn kết liễu; Tam Thiên dùng `QiWave` để phóng từng nhát |
| Vệt lưỡi, bụi, tia va chạm, kiếm rơi và vòng tụ lực | LineRenderer và `PlayerUnlit.mat` từ repo | Hiệu ứng code trong scene review |
| 13 cue WAV | Thiết kế bằng tổng hợp PCM có seed, bộ lọc và bao âm tại [render_sfx.py](HexcoreRemaining/render_sfx.py). Tệp ở `Assets/Prefabs/Player/Melee/Shared/Audio` | Âm mới cho repo, không có sample ngoài. Cue áp dụng theo frame hoặc pha; cần xác nhận âm lượng bằng Game View |
| Dummy nhện | Kenney Vleugels, gói Platformer Art (more enemies and animations) | CC0, ghi công tùy chọn. SHA-256 đã đối chiếu với `kenney_platformer-art-extended-enemies`, xem [audit](HEXCORE_ASSET_AUDIT.md) |

[PixelFX](https://pixelfx.io/effects/) là tài liệu tham khảo của đợt cũ. Đợt đó dùng lại VFX và 13 cue WAV có sẵn.

Các nguồn Sprite bổ sung nằm trong [HexcoreRemaining](HexcoreRemaining) và [bộ prompt](HexcoreRemaining/prompts.json). Sử dụng ImageGen tích hợp với prefab gốc làm tham chiếu. Hai atlas Q1 NW không vượt kiểm tra nhất quán tay kiếm/giáp vai. Dao sĩ R2 W được tạo lại sau khi phát hiện lẹm đầu đao. F2 Kiếm khí được tạo lại để thanh kiếm luôn ở tay, còn projectile là lớp riêng.

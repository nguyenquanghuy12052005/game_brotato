# Hồ sơ giấy phép âm thanh

## Foley và UI

Vòng Foley bổ sung lấy **Swishes Sound Pack** của artisticdude, CC0 1.0. Tác giả mô tả dùng móc áo và que gỗ thật. Archive SHA-256 `7980215241b739a787dc26f660ce510bb237900968789395a86911619795693`. Tệp dưới `foley/swishes-original` giữ 24-bit stereo 44,1 kHz nguyên gốc. Bản làm việc mono 32 kHz trong `foley`. Các lớp mới trim đầu, lọc, dịch vị trí pan và phối với Kenney và tổng hợp. [Trang tác giả, CC0 và cách thu](https://lpc.opengameart.org/content/swishes-sound-pack) · [Tệp lấy cho dự án](https://opengameart.org/sites/default/files/swishes.zip)

Kenney Vleugels công bố hai gói dưới CC0 1.0. Bản dùng cho demo tải từ bài đăng của Kenney trên OpenGameArt ngày 28/11/2026. Tệp giấy phép gốc đi kèm trong thư mục `foley`. Dùng SHA-256 để đối chiếu đúng archive. [Văn bản CC0 1.0](https://creativecommons.org/publicdomain/zero/1.0/)

| Gói | Bài đăng và tệp tải | SHA-256 archive | Vai trò |
| --- | --- | --- | --- |
| RPG Audio | [Kenney](https://kenney.nl/assets/rpg-audio) · [Bài đăng OpenGameArt](https://opengameart.org/content/50-rpg-sound-effects) · [ZIP](https://opengameart.org/sites/default/files/RPGsounds_Kenney.zip) · [Giấy phép gốc](foley/LICENSE-RPG.txt) | `3ae398ad63e293f9c450bda22d5d81c3af69c74df66fc1400f33c012c0bbc231` | Bước chân, vải, da, gỗ, giấy, kim loại và chuyển động lưỡi |
| Interface Sounds | [Kenney](https://kenney.nl/assets/interface-sounds) · [Bài đăng OpenGameArt](https://opengameart.org/content/interface-sounds) · [ZIP](https://opengameart.org/sites/default/files/kenney_interfaceSounds.zip) · [Giấy phép gốc](foley/LICENSE-INTERFACE.txt) | `f2193d072726d6758a5f7871b2dcc54dcce0d5c35c6f0a62f92549b327c81232` | Click, thủy tinh và các lớp ngắn cho UI/phép |

Tệp `foley/*.ogg` giữ byte gốc. Tệp `foley/*.wav` là bản giải mã mono 32 kHz để tái dựng. Bản phối cắt khoảng im đầu, chỉnh trọng lượng tần số, thời gian, vị trí và kết hợp vật liệu theo cue. [Danh sách tệp gốc thực sự dùng và SHA-256](used-foley.json) · [Nguồn cụ thể theo mục](INVENTORY.md)

[Impact Sounds](https://kenney.nl/assets/impact-sounds) đã đối chiếu trang CC0. Đường tải trực tiếp trả HTTP 403 trong môi trường làm việc nên gói này không có trong bản phối. Các trang tài nguyên trong kế hoạch cũ là gợi ý tìm thêm. Ledger này xác định chính xác nguồn đã dùng.

## Âm thanh và nhạc mới

- `review-audio/` giữ vòng phối Foley, còn `audio/` giữ WAV đợt trước để nghe A/B. `reference/*.mp4` là các take hình không tiếng được ghi từ scene nguồn và dùng asset hiện có của dự án.

- `audio/` chứa bản phối thử do dự án tạo. Trình dựng dùng bộ tạo nhiễu có seed, bộ lọc, mô phỏng dây gảy và cộng hưởng, lập trình nhịp, Foley ở trên cùng các motif và hòa âm mới. Có thể tái tạo bằng `render.py`, `materials.txt` và `cues.txt`. Âm vocal/hiệu ứng môi trường được dựng từ vật liệu đã nêu và tổng hợp, chưa dùng thu tiếng diễn viên hoặc thu môi trường tại hiện trường.
- Ba bản menu mới, nhạc bốn map, nhạc Boss và nhạc kết quả dùng cấu trúc và motif do demo tạo. Không lấy âm thanh hoặc melody của các game tham khảo. Không gọi dịch vụ AI tạo âm thanh bên ngoài.
- Thẻ **Cue cũ** tham chiếu WAV Unity của dự án tại đường dẫn đã có, giữ nguyên file nguồn. Một số bản phối tái dùng Foot, Dash, Swing, Wave, Parry, Slam và Confirm ở mức trợ lực, có ghi theo hàng trong kiểm kê. [Hồ sơ Cận chiến](../../docs/Melee/HEXCORE_ASSET_SOURCE_LICENSES.md)
- Hình Concept B, sprite và GIF lấy từ tài nguyên dự án. GIF dùng làm tham chiếu hình động. [Hồ sơ Concept B](../brotato-main-menu/README.md) · [Nguồn hình Cận chiến](../../docs/Melee/HEXCORE_ASSET_SOURCE_LICENSES.md)

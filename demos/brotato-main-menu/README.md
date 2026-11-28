# BROTATO 2D · Ba demo Main Menu

## Concept B · Đại chiến sinh tồn

Mở [Concept B](concept-b.html) trong Chrome hoặc Edge. Bấm **Hướng dẫn** để xem bốn tab **Tổng quan**, **Điều khiển**, **Nhân vật** và **Quái vật**. Nút **Bắt đầu** xác nhận vào Forest trong bản xem thử. Dùng [điểm lưu mẫu](menu.html?concept=b&save=1) để kiểm tra luồng **Chơi tiếp** và hộp thoại bắt đầu mới.

[Mở bản kiểm thử](concept-b-test.html) để thử mở khóa. Trong **Nhân vật**, chọn một nhánh khóa và bấm **Thử chọn nhánh**. Trong **Quái vật**, chọn một bóng đen và bấm **Thử gặp quái**. Hai thao tác ghi khám phá ngay vào `localStorage`, mô phỏng đúng hai mốc chọn tiến hóa và gặp quái. **Reset bộ sưu tập** xóa dữ liệu khám phá của demo sau khi xác nhận. Lần mở đầu tiên nạp sẵn 3/10 nhánh và 3/14 quái làm dữ liệu minh họa. Nhân vật khởi đầu luôn mở và nằm ngoài bộ đếm nhánh.

| Màn hình | 1920×1080 | 1280×720 |
| --- | --- | --- |
| Main Menu | [Ảnh](screenshots/concept-b.png) | [Ảnh](screenshots/concept-b-720p.png) |
| Tổng quan | [Ảnh](screenshots/guide-b-overview.png) | [Ảnh](screenshots/guide-b-overview-720p.png) |
| Bốn bản đồ trong Tổng quan | [Ảnh](screenshots/guide-b-overview-maps.png) | [Ảnh](screenshots/guide-b-overview-maps-720p.png) |
| Điều khiển | [Ảnh](screenshots/guide-b-controls.png) | [Ảnh](screenshots/guide-b-controls-720p.png) |
| Nhân vật | [Ảnh](screenshots/guide-b-characters.png) | [Ảnh](screenshots/guide-b-characters-720p.png) |
| Quái vật | [Ảnh](screenshots/guide-b-bestiary.png) | [Ảnh](screenshots/guide-b-bestiary-720p.png) |

Guide dùng bản sao bốn frame Idle của Player, Sát thủ, Đao sĩ, Kiếm khí, Pháp sư và các atlas gốc Normal, Bow, Elite, Skeleton, Lava Golem, Yeti. Shotgun, SMG và Rocket dùng chung hình thái người bắn. Hỏa, Băng, Quang và Ám dùng chung tạo hình Pháp sư. Các mục quái chỉ có art hoặc prefab được ghi rõ trạng thái khi mở khóa, với chỉ số và hành vi chỉ hiển thị theo cấu hình đã xác minh.

Cuộn qua banner và các thẻ giới thiệu để xem bốn bản đồ. Ảnh Forest, Moon, Inferno và Tundra được dựng từ tọa độ tilemap và sprite trang trí đã lưu trong `Assets/Scenes/GameScene.unity`, dùng tài nguyên Roboden của dự án. Mỗi ảnh hiển thị toàn bộ vùng 30 × 18 ô.

Tab Điều khiển nhóm di chuyển, kỹ năng, tương tác và tạm dừng. Bố cục Q/E/R lấy từ sân thử tiến trình. Hai nút Tạm dừng và Tiếp tục chuyển trạng thái trong khung mẫu HTML. Điều khiển chiến đấu chính thức được xác nhận ở vòng Unity sau.

## Mở và thử

Mở [index.html](index.html) bằng Chrome hoặc Edge rồi chọn một concept. Các tệp HTML, hình và âm thanh chạy cục bộ. Giữ nguyên cấu trúc thư mục nếu sao chép demo sang máy khác. Nhạc bắt đầu sau khi bấm một nút khả dụng.

- Mẫu A và C giữ luồng thử ban đầu. **Bắt đầu** mở khung xem đội hình và **Hướng dẫn** cho xem các động tác mẫu. **Chọn màn** có bốn biome với trạng thái mở và khóa minh họa.
- **Chơi tiếp** mặc định tắt. Mẫu A và C có thể thêm `&save=1` sau tên concept trong thanh địa chỉ để bật điểm lưu mẫu.
- **Cài đặt** cho thử tắt nhạc, chỉnh âm lượng và giảm chuyển động. **Thoát** có xác nhận và màn kết thúc lượt xem.
- Dùng chuột hoặc `Tab`, các phím mũi tên, `Enter`, `Space` và `ESC`. `ESC` trong hộp thoại quay về nút đã mở. Tại menu chính, `ESC` mở xác nhận thoát.

## So sánh

| Concept | Điểm mạnh | Đánh đổi | Ảnh chụp từ trình duyệt |
| --- | --- | --- | --- |
| **A · Hội tụ chiến binh** | Kiếm sĩ dẫn mắt vào đội hình, màu rừng và ánh nắng tạo cảm giác phiêu lưu. Cụm nút bên trái dễ đọc và nổi bật. | Súng và Cận chiến có gương mặt gần nhau theo tạo hình gốc. Pistol và vệt kiếm là hai dấu nhận biết quan trọng khi xem nhỏ. | [1920×1080](screenshots/concept-a.png) · [1280×720](screenshots/concept-a-720p.png) |
| **B · Đại chiến sinh tồn** | Súng, phép và quái gặp nhau giữa khung hình, truyền tải hành động tức thì. Dải lệnh dưới nhường chỗ cho màn đối đầu. | Dải nút phụ thấp hơn A và C. Các nút cần đủ chiều ngang để tên tiếng Việt dễ đọc. | [1920×1080](screenshots/concept-b.png) · [1280×720](screenshots/concept-b-720p.png) |
| **C · Đấu trường HEXCORE** | Góc nhìn gần gameplay top-down hơn, ba hệ đứng ở ba bục riêng. Lõi HEXCORE có vai trò thị giác rõ nhất. | Lõi sáng dễ kéo mắt khỏi nút Bắt đầu. Nút xanh chanh có vùng tương phản riêng để giữ phân cấp. | [1920×1080](screenshots/concept-c.png) · [1280×720](screenshots/concept-c-720p.png) |

[Ảnh trang so sánh vòng đầu](screenshots/comparison.png). Concept B là mẫu đã chốt để thử bốn tab Hướng dẫn.

## Tạo hình và nguồn asset

| Thành phần | Nguồn và cách dùng |
| --- | --- |
| Hệ Súng | `Assets/Prefabs/Player/CharacterArt/Frames/Idle/Right` và `Assets/Prefabs/Weapon/weapon_pistol_01.png`, đối chiếu pistol được gán trong `GunSkillTest.unity`. Dùng làm tham chiếu minh họa và bản sao để minh họa Idle. |
| Cận chiến | `Assets/Prefabs/Player/Melee/Swordsman/Frames` và Idle chéo trong `HexcorePreview/IdleLoops`. Dùng tóc nâu, khăn đỏ, giáp vai và kiếm gốc, sao chép frame Idle và Attack vào demo. Nguồn tạo hình có trong [kiểm kê Cận chiến](../../docs/Melee/HEXCORE_ASSET_SOURCE_LICENSES.md). |
| Pháp sư | Sprite `Assets/Art/Mage/Mage_Neutral_FullSheet_52frames.png` và `Assets/Art/Mage/MageStaff.png`. Cắt frame theo vùng import Unity, giữ mũ chóp, áo tối và trượng đầu tinh thể. |
| Quái | `Assets/Art/Enemy/Normal/Green.png` và `Assets/Art/Enemy/Bow/Snow.png` làm tham chiếu cho goblin cầm dao đá và cung thủ màu tuyết. Bảng đội hình chạy frame của goblin xanh. |
| Biome | Bản sao tileset Forest, Moon, Inferno, Tundra trong `Assets/Tilesets/Roboden`. [Giấy phép CC0 kèm demo](assets/Roboden-LICENSE.txt). |
| Hiệu ứng | Hiệu ứng đạn, kiếm khí và pháp thuật của bản demo được vẽ bằng Canvas theo ngôn ngữ màu và hình đã kiểm tra trong `Assets/Art/GunSkills`, VFX Cận chiến và `Assets/Art/Mage`. Hiệu ứng lõi trong tranh là diễn giải minh họa cho menu. |
| Key art, logo và âm thanh | Ba tranh mới từ ImageGen, bám theo sprite dự án. Logo dựng bằng lettering nhiều lớp CSS. Nhạc và SFX do Web Audio tổng hợp tại chỗ. [Prompt tạo tranh](PROMPTS.md). |

## Kiểm tra và giới hạn

Đã chạy Chrome ở 1920×1080 và 1280×720, kiểm tra hiển thị sáu nút, điều hướng bàn phím, phục hồi focus, xác nhận thoát, chuyển trạng thái nhạc và chuyển động, màn khóa và dữ liệu lưu mẫu. Ảnh lấy từ bản demo đang chạy. Đã đo được tín hiệu Web Audio ở cả ba mẫu sau lượt bấm. Cân bằng âm lượng trên loa hoặc tai nghe vẫn cần nghe thử.

Màn chính dùng chuyển động từng vùng của tranh và lớp VFX, nên Idle và phản lực là chuyển động 2D nhẹ. Các frame sprite gốc chạy trong bảng đội hình. Dữ liệu tiến trình và mở khóa trong bản demo là fixture. Chọn màn dùng texture biome thật để minh họa, chưa có ảnh toàn cảnh trong Unity. Tranh mới, file sao chép và mã web nằm trong thư mục demo. Nguồn cấp quyền phân phối ngoài repo của các sprite dự án cần được đối chiếu trước khi phát hành.

# Hiệu ứng giai đoạn 1 và 2

Ghi chú lịch sử. Các hiệu ứng Stage1 và Anh Hồi Kích đã rời Unity. Sheet gốc nằm trong Source/RetiredVfx. Builder hiện chỉ xử lý tám hiệu ứng đang dùng.

Giai đoạn 1 chọn một trong Khiên Chắn / Ảnh Bộ / Lưỡi Đao Hộ Thể. Chuyển nhánh ở giai đoạn 2 mở lựa chọn một trong ba nội tại của nhánh; nội tại tự kích hoạt khi đủ điều kiện.

| Thư mục | Hiệu ứng |
|---|---|
| Stage1 | KhienChan, AnhBo, LuoiDaoHoThe |
| Assassin | TamLienThu, AnhHoiKich, AnTuHuyet |
| DaoWarrior | DiaLietTram, HoanhTao, TrongDao |
| QiSwordsman | HoiNguyet, PhanQuang, KiemAn |

Mỗi bộ có tám khung PNG trong suốt 256×256, một clip, controller và prefab Visual trong Assets/Prefabs/Player/Melee/Shared/VFX/MeleeSkills. PPU48 cho VFX, Point filtering. Clip xem thử dài 0,64 giây. Lưỡi Đao Hộ Thể lặp vòng; hiệu ứng khác tự xóa sprite cuối clip. Thời gian này phục vụ xem chuyển động, cần chỉnh theo nhịp combat khi ghép game.

Hướng tấn công mặc định +X, tâm canvas dùng làm pivot. Khi ghép vào vũ khí/mục tiêu cần căn offset theo hình. Kéo prefab vào scene thử rồi Play để xem một chu kỳ; có thể chọn clip trong Animation để tua lại. Menu dựng lại là Tools > Brotato > Build Melee Stage Skills.

Gói cung cấp artwork VFX. Hoạt ảnh cơ thể vẫn dùng các bộ nhân vật đã có. Tích tầng, nhận biết trúng đòn, vị trí vòng sau lưng, sát thương, hướng lướt và thời gian tồn tại cần được ghép ở bước gameplay. GIF chỉ minh họa hình ảnh hiệu ứng.

Nguồn tạo bằng imagegen tích hợp. Prompts và script xuất nằm cùng thư mục tài liệu này. AI có thể có sai khác nhỏ giữa các khung.

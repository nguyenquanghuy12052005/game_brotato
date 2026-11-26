# VFX lõi của demo cận chiến đời đầu

TamThien đã rời Unity. Bản HEXCORE hiện hành dựng ba sóng thủ tục. Sheet gốc đời cũ lưu trong Source/RetiredVfx. Chỉ Thiên Vẫn và Vạn Ảnh còn prefab VFX trong Shared.

Mỗi bộ có 16 PNG trong suốt, một clip 1,28 giây, controller và prefab Visual. Clip tự xóa sprite ở cuối. Mở prefab và xem clip trong Animation, hoặc kéo vào scene thử rồi chạy Play để xem một lần. Menu dựng lại là Tools > Brotato > Build Melee Ultimate Assets.

- ThienVan là lửa đáp đất rồi quạt vòng quanh tâm.
- TamThien là tụ kiếm hướng phải (+X) rồi phóng ba kiếm bao phủ hình nón. Xoay prefab quanh Z để đổi hướng.
- VanAnh là năm ảnh và các nét chém hình sao, sau đó sáng toàn bộ vùng trong sao.

PPU48 được chọn cho VFX để bộ 256px có bề ngang khoảng 5,33 Unity unit. Nhân vật hiện có vẫn dùng PPU120. Có thể chỉnh scale prefab khi ghép vùng đánh. Pivot của các clip là tâm canvas; điểm gốc hình nón ở phía trái canvas, cần bù vị trí khi ghép vào vũ khí.

Đây là asset hiệu ứng và clip xem chuyển động. Hoạt ảnh nhảy/đáp của nhân vật, giữ R tụ lực, chọn boss, đường đi năm ảnh, vùng sát thương và hất tung cần được ghép ở bước gameplay. Clip tụ/phóng hiện là một đoạn minh họa cố định; khi triển khai giữ R cần tách nhịp tụ và nhịp phóng.

Nguồn dùng imagegen tích hợp. Prompt và script xuất ở docs/Melee/MeleeUltimates. Preview GIF là hiệu ứng trên nền tối để duyệt hình. AI có thể có sai khác nhỏ giữa các frame.

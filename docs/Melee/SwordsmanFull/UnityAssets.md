# Kiếm sĩ

Bộ hình tóc nâu, khăn đỏ, kiếm thép một tay. Có 76 PNG riêng và 16 animation theo bốn hướng Down, Left, Right, Up.

| Hành động | Khung mỗi hướng | Thời lượng | Lặp trong Unity |
|---|---:|---:|---|
| Idle | 4 | 0,67 giây | Có |
| Run | 5 | 0,50 giây | Có |
| Attack | 6 | 0,74 giây | Một lần |
| Death | 4 | 0,58 giây | Một lần |

PNG dùng canvas 256×256, nền trong suốt, PPU 120, Pivot Center. Canvas rộng dành chỗ cho cung kiếm. Mốc đáy chân ở y208 tính từ trên xuống. Tỉ lệ được chuẩn hóa theo khung đầu mỗi hành động, giữ một tỉ lệ chung trong toàn hành động. Một số chi tiết và tư thế có thể dao động nhẹ do ảnh tạo bằng AI.

`SwordsmanVisual.prefab` chứa SpriteRenderer và Animator với `Swordsman.controller`. Có thể kéo vào Scene để xem hình, chọn clip trong thư mục Clips để xem animation. Tên state theo dạng `Attack_Right`. Controller mặc định phát Idle_Down. Hệ thống combat cần chủ động gọi state tấn công khi triển khai gameplay cận chiến.

Các GIF phát lặp để xem ở `docs/Melee/SwordsmanFull`. GIF Death giữ khung cuối lâu hơn để dễ quan sát trước khi lặp lại. Nguồn tạo hình nằm tại docs/Melee/Source/Swordsman/Sheets. Hướng trái của Attack và Death được lật ngang từ hàng nguồn để sửa hướng nhìn.

Menu `Tools > Brotato > Build Swordsman Assets` nhập lại 76 PNG, tạo 16 clip, controller và prefab hiển thị. Lệnh cập nhật bộ asset kiếm sĩ này. Các clip được kiểm tra liên kết sprite khi tạo.

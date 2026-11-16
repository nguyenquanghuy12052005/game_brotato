# Sát thủ dao găm

Nhân vật tóc bạc, khăn tím, giáp da tối và một dao găm. Có 72 PNG riêng theo bốn hướng Down, Left, Right, Up. Mỗi hướng có Idle 4 khung, Run 5 khung, Attack 5 khung, Death 4 khung.

Canvas 256×256, nền trong suốt, PPU 120, Pivot Center. Mốc chân ở y208 tính từ trên xuống. Giữ một tỉ lệ chung trong mỗi hành động để hạn chế co giãn giữa các khung.

Idle lặp trong 0,67 giây. Run lặp trong 0,50 giây. Attack phát một lần trong 0,48 giây với thời gian từng khung lần lượt 0,10 / 0,08 / 0,05 / 0,07 / 0,18 giây. Death phát một lần trong 0,58 giây rồi giữ khung cuối.

`AssassinVisual.prefab` có SpriteRenderer và Animator dùng `Assassin.controller`. Có thể kéo vào Scene để xem hình hoặc chọn từng clip trong Clips để xem chuyển động. Controller mặc định phát Idle_Down. Quy ước tên state gồm `Run_Right`, `Attack_Left` và các hướng tương ứng. Đòn đâm dao găm cần được hệ thống combat gọi khi triển khai gameplay sát thủ.

Các GIF phát lặp trên nền rừng ở `docs/player-art/AssassinFull`. GIF Death giữ khung cuối lâu hơn để dễ xem trước khi lặp. Các ảnh nguồn do imagegen tạo nằm trong Sheets. Ảnh có thể có sai khác nhỏ về đường nét và tư thế giữa các khung.

Menu `Tools > Brotato > Build Assassin Assets` nhập PNG và tạo clip/controller/prefab. Lệnh cập nhật bộ asset này và kiểm tra đủ 72 sprite.

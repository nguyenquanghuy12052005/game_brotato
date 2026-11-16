# Đao khách đại đao

Nhân vật vai rộng, tóc búi cao, giáp đồng và đai đỏ. Vũ khí là đại đao cong bản rộng. Bộ gồm 84 PNG, 16 clip, DaoWarrior.controller và DaoWarriorVisual.prefab.

Mỗi hướng Down/Left/Right/Up có Idle 4, Run 5, Attack 8 và Death 4 khung. Canvas 320×320 dành chỗ cho lưỡi đao. PPU 120, Pivot Center, mốc chân y240 từ trên xuống. Thân người cao khoảng 160 pixel theo tỉ lệ chung trong từng hành động.

Idle lặp trong 0,67 giây. Run lặp trong 0,60 giây. Đòn bổ chéo phát một lần trong 1,10 giây. Các khung lấy đà dài hơn, lưỡi đao đi xuống nhanh, hồi thế chậm để thể hiện sức nặng. Death phát một lần trong 0,58 giây và giữ khung cuối.

Kéo DaoWarriorVisual.prefab vào Scene để xem hình. Có thể chọn từng clip trong Clips để xem animation. Controller mặc định là Idle_Down. State dùng quy ước `Attack_Right`, `Run_Left` và các hướng tương ứng. Hệ thống gameplay cần gọi animation đòn đánh khi tích hợp combat.

Nguồn ảnh imagegen nằm trong Sheets. GIF phát lặp và prompt nằm ở docs/player-art/DaoWarriorFull. Một số nét vẽ và tư thế có thể dao động nhẹ giữa các khung do ảnh được tạo bằng AI.

Hàng Attack_Left đã được lật ngang khi xuất để sửa hướng nhìn của ảnh nguồn. Vùng lấy ảnh đòn đánh được mở rộng ngang để giữ trọn mũi đao. Các GIF Death giữ khung nằm cuối lâu hơn trước khi lặp lại để dễ xem.

Menu Tools > Brotato > Build DaoWarrior Assets nhập lại PNG, tạo clip, controller và prefab của bộ đại đao.

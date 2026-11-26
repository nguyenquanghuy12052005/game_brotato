# Kiếm khách hệ khí

Tạo hình tóc đen dài, áo xanh ngọc viền vàng nhạt và kiếm thép mảnh. Bộ nhân vật có 80 PNG, 16 clip, QiSwordsman.controller và QiSwordsmanVisual.prefab. Mỗi hướng Down/Left/Right/Up có Idle4 / Run5 / Attack7 / Death4.

Canvas nhân vật 320×320, PPU120, Pivot Center. Thân người chuẩn hóa khoảng 160 pixel, mốc chân y240 từ trên xuống. Idle lặp 0,67 giây; Run lặp 0,50 giây; Attack phát một lần 0,78 giây; Death phát một lần 0,58 giây và giữ khung cuối.

Hiệu ứng riêng có 4 PNG trong WaveFrames, clip QiWave.anim, controller và QiWaveVisual.prefab. Sóng khí hướng mặc định sang phải (+X), tâm sprite ở giữa, canvas 256×256, PPU120. Có thể xoay Z của prefab theo hướng phóng. Clip hình thành, sáng lên, ổn định và tan trong 0,36 giây, sau đó ẩn Sprite. Giữ alpha mờ ở viền sóng để hiệu ứng dịu trên nền rừng.

Mốc phóng sóng đề xuất là khung Attack_03 tại giây 0,31, khi tay cầm kiếm duỗi ra. Đây là thông tin để tích hợp gameplay. Prefab sóng cung cấp hình và Animator; chuyển động bay, va chạm và sát thương cần được hệ thống combat điều khiển.

Các clip nhân vật dùng tên `Attack_Right`, `Run_Left` và các hướng tương ứng. Prefab nhân vật mặc định phát Idle_Down. Có thể kéo prefab vào Scene để xem hình hoặc chọn clip trong Clips để xem animation.

Ảnh nguồn do imagegen tạo nằm tại docs/Melee/Source/QiSwordsman/Sheets. Prompts, script xuất và GIF trên nền rừng nằm ở docs/Melee/QiSwordsmanFull. Một số đường nét và tư thế có thể dao động nhẹ giữa các khung.

ReleaseDemo.gif ghép animation nhân vật với sóng di chuyển từ mốc 0,31 giây để minh họa hình ảnh dự kiến khi tích hợp combat. Đường bay trong GIF được dựng để xem thử, chưa phải chức năng projectile trong GameScene.

Menu Tools > Brotato > Build QiSwordsman Assets nhập PNG, tạo clip/controller và hai prefab hiển thị.

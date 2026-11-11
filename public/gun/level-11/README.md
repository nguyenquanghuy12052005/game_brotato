# Sprite sheet súng cấp 11

Mỗi PNG nền trong suốt RGBA là một sheet 2048 × 1024 px gồm 32 frame, 8 cột × 4 hàng. Mỗi frame 256 × 256 px, đọc từ trái sang phải rồi xuống hàng. Pivot giữa ô (0.5, 0.5).

| Tệp PNG | Hiệu ứng | FPS gợi ý | Điểm đặt pivot |
|---|---|---:|---|
| `spread-sweeping-blade.png` | Lưỡi đạn càn quét | 32 | Nòng súng · hướng +X |
| `spread-sunder-pressure.png` | Áp lực công phá | 32 | Tâm mục tiêu |
| `spread-charge-shot.png` | Phát bắn dồn lực | 24 | Tâm mục tiêu |
| `burst-chain-detonator.png` | Kíp nổ liên hoàn | 32 | Tâm mục tiêu |
| `burst-echo-magazine.png` | Băng đạn cộng hưởng | 32 | Cạnh nhân vật · hướng +X |
| `burst-orbital-storm.png` | Bão đạn quỹ đạo | 24 | Tâm nhân vật · phóng về +X |
| `pierce-spatial-rift.png` | Vết rách không gian | 24 | Tâm vệt đạn · trục X |
| `pierce-returning-lance.png` | Xuyên kích hồi quy | 30 | Điểm khai hoả · hướng +X |
| `pierce-sky-lance.png` | Thiên thương | 30 | Tâm va chạm · rơi từ trên |
| `burst-orbit-loop.png` | Quỹ đạo chờ | 32 | Tâm nhân vật · lặp |
| `burst-orbit-bolt.png` | Đạn vệ tinh | 32 | Tâm viên đạn · hướng +X · lặp |

Trong Unity, dùng Sprite (2D and UI), Sprite Mode Multiple, Grid by Cell Size 256 × 256, offset/padding 0, pivot Center. Bật Keep Empty Rects nếu muốn giữ đúng cả 32 frame kể cả ô trong suốt. Giữ Max Size 2048, Filter Mode Bilinear, Alpha Is Transparency bật, Compression None khi xem thử.

Chín sheet mang tên kỹ năng phát một lượt. Với Bão đạn quỹ đạo bám vị trí boss, dùng vòng chờ và viên vệ tinh riêng để điều khiển đường bay trong game. Bản dựng đầy đủ minh hoạ trình tự xoay và phóng theo trục +X.

Xoay asset theo góc bắn với các hiệu ứng hướng +X. Thiên thương có hướng rơi cố định theo màn hình, còn điểm chạm nằm tại tâm ô.

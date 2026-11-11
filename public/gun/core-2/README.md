# Sprite sheet lõi súng thứ hai

11 PNG RGBA nền trong suốt cho 6 kỹ năng. Mỗi sheet 2048 × 1024 px, gồm 32 frame 256 × 256 px, 8 cột × 4 hàng. Đọc từ trái sang phải, từ trên xuống dưới. Pivot ở tâm ô (0.5, 0.5).

| Tệp PNG | FPS gợi ý | Phát | Điểm đặt |
|---|---:|---|---|
| spread-counter-shield-loop.png | 24 | Lặp | Tâm người chơi, mặt giáp +X |
| spread-counter-shield-break.png | 36 | Một lượt | Tâm người chơi, phản pháo +X |
| spread-pursuit-saw-loop.png | 32 | Lặp | Tâm boss, đĩa cưa xoay quanh tâm |
| burst-escalation-muzzle-loop.png | 24 | Lặp | Tâm nòng súng, hướng +X |
| burst-escalation-heavy-round-loop.png | 32 | Lặp | Tâm viên đạn, hướng +X |
| burst-disintegration-aura-loop.png | 24 | Lặp | Tâm boss |
| burst-disintegration-hit.png | 40 | Một lượt | Điểm trúng, mảnh văng theo +X |
| pierce-crossfire-portal-loop.png | 24 | Lặp | Tâm một cổng, nhân hai trong game |
| pierce-crossfire-portal-pass.png | 40 | Một lượt | Tâm cổng, vệt chạy theo +X |
| pierce-heart-anchor-loop.png | 24 | Lặp | Tâm neo trên mặt đất |
| pierce-heart-anchor-snap.png | 36 | Một lượt | Điểm dây đứt, xoay theo trục dây |

- Giáp phản pháo giữ sheet giáp khi còn hiệu lực, rồi phát sheet vỡ khi chặn đòn. Mảnh sáng trong sheet minh hoạ cú phản pháo. Đường bay của đạn thật tính riêng.
- Hoả lực leo thang dùng sheet vòng nòng khi quá tải và sheet đạn hạng nặng cho những viên cuối loạt.
- Đạn phân rã gắn hào quang lên boss, phát sheet va chạm ở mỗi nhịp sát thương bổ sung.
- Cổng xuyên kích đặt hai bản của cùng sprite cổng và phát chớp tại cổng khi có đạn đi qua.
- Mỏ neo xuyên tâm vẽ dây theo hai đầu neo và boss trong game. Sheet đứt dây đặt tại vị trí đứt và xoay theo trục dây.

Trong Unity, đặt Max Size 2048, Filter Bilinear, tắt Mip Maps và bật Alpha Is Transparency. Khi cắt sprite thủ công, dùng Multiple, Grid by Cell Size 256 × 256 và pivot Center.

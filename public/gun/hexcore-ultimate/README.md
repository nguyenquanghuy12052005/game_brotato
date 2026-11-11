# Animation chiêu cuối cấp 12 · Lõi Hexcore

14 PNG RGBA nền trong suốt cho 5 chiêu cuối. Mỗi sheet 2048 × 1024 px, có 32 frame 256 × 256 px, 8 cột × 4 hàng. Đọc từ trái sang phải, từ trên xuống dưới. Pivot giữa ô (0.5, 0.5).

| Tệp | Chiêu cuối | FPS | Phát | Điểm đặt |
|---|---|---:|---|---|
| grenade-armed-loop.png | Ngũ bom liên hoàn | 24 | Lặp | Tâm bom. Quỹ đạo của 5 quả tính riêng. |
| grenade-chain-blast.png | Ngũ bom liên hoàn | 36 | Một lượt | Tâm từng quả khi nổ. |
| airstrike-helicopter-loop.png | Trực thăng oanh kích | 32 | Lặp | Tâm thân, mũi hướng +X. Dịch chuyển bằng gameplay. |
| airstrike-ground-mark-loop.png | Trực thăng oanh kích | 24 | Lặp | Tâm vùng cảnh báo, trục X theo đường oanh kích. |
| airstrike-bomb-impact.png | Trực thăng oanh kích | 36 | Một lượt | Tâm từng điểm trúng. Màu cam với viền xanh. |
| turret-deploy.png | Pháo đài cơ động | 32 | Một lượt | Tâm từng trụ, nòng hướng +X. |
| turret-idle-loop.png | Pháo đài cơ động | 24 | Lặp | Tâm từng trụ, nòng hướng +X. |
| turret-fire.png | Pháo đài cơ động | 48 | Một lượt | Tâm từng trụ, nòng hướng +X. Vệt lửa ở nòng là VFX. |
| turret-retire.png | Pháo đài cơ động | 32 | Một lượt | Tâm từng trụ khi hết thời gian. |
| rail-charge.png | Pháo ray Hexcore | 32 | Một lượt | Tâm nòng súng. |
| rail-beam.png | Pháo ray Hexcore | 48 | Một lượt | Tâm đường bắn, xoay theo +X. Tia chiếm khoảng 224/256 chiều rộng. |
| cage-field-loop.png | Lồng đạn Hexcore | 32 | Lặp | Tâm vùng lồng, 6 điểm đều cách tâm. |
| cage-node-fire.png | Lồng đạn Hexcore | 48 | Một lượt | Tâm điểm phát, xoay +X hướng vào tâm lồng. |
| cage-collapse.png | Lồng đạn Hexcore | 36 | Một lượt | Tâm lồng khi kết thúc. |

- Ngũ bom liên hoàn dùng chung animation bom cho 5 quả. Đặt mỗi vụ nổ tại tâm va chạm và lệch thời gian để thấy nhịp nổ nối tiếp.
- Trực thăng oanh kích báo trước bằng dấu dưới đất. Di chuyển thân trực thăng dọc trục đã chọn và phát mỗi vụ nổ tại điểm thả tương ứng.
- Pháo đài cơ động cho tối đa **2 trụ cùng tồn tại**. Trong Unity, mỗi trụ dùng atlas turret-layered.png (512 × 512 px), gồm chân đế, nòng, vòng dưới đất và chớp đầu nòng. Chân đế đứng thẳng trong cả bốn trạng thái. Chỉ nòng và chớp đầu nòng xoay theo mục tiêu. Bốn sprite sheet trụ nguyên khối là bản xem trước. Dùng hiệu ứng khai hoả tại trụ và tạo đường đạn riêng khi nối gameplay.
- Pháo ray Hexcore gắn tụ lực ở nòng, sau đó đặt tia vào giữa đường bắn. Kéo chiều ngang tia theo độ dài đường bắn, giữ chiều dày riêng.
- Lồng đạn Hexcore đặt 6 hiệu ứng khai hoả tại 6 đỉnh và hướng vào tâm. Có thể bắn theo từng cặp đối diện. Kết thúc bằng hiệu ứng sập lồng.

Trong Unity, giữ Max Size 2048, tắt Mip Maps, bật Alpha Is Transparency và dùng Bilinear. Khi cắt sprite thủ công, dùng Multiple, Grid by Cell Size 256 × 256, pivot Center. Giữ các ô cuối trong suốt để phát đúng độ dài clip.

Hình bom, thân trực thăng và trụ súng được dựng bằng ImageGen. Phần cánh quạt, vệt đạn và VFX được dựng theo từng frame.

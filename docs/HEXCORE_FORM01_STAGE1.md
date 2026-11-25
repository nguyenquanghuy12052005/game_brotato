# HEXCORE · Đợt 1 · Form 01 Kiếm sĩ

**Trạng thái: In Review cho hướng Đông.** Mở `Assets/Scenes/HexcoreSkillPreview.unity` và nhấn Play. Chuỗi mặc định là Run → Basic hai nhát → Q1 → Basic hai nhát → Run. Chọn Basic hoặc Q1 để xem từng động tác, chọn 0,5× để xem từng điểm chuyển tư thế. [GIF camera hướng Đông](player-art/HexcoreForm01/Q1_East_Review.gif)

## Asset tham gia

| Hạng mục | Có thể xem | Nguồn |
|---|---|---|
| Idle/Run N, E, S, W | Clip Animator gốc | `SwordsmanVisual.prefab` |
| Idle/Run NE, SE | Một pose Idle giữ và bốn pose Run mới mỗi hướng | 10 PNG mới |
| Basic E | 12 frame mới. Sáu frame chém ngang nối trực tiếp sáu frame chém ngược | `HexcorePreview/Basic` |
| Q1 E | 12 frame riêng. Anticipation → Foot Push → Dash → Sword Swing → Impact → Braking → Recovery | `HexcorePreview/Q1` |
| VFX | Vệt kiếm từ chuôi tới mũi, bụi chân tại điểm đạp và hãm, tia va chạm tại dummy, rung camera | LineRenderer trong scene, dùng material nội bộ |
| Dummy | Nhện đã xác minh CC0 ở Đợt 0 | Sprite/material từ Enemy prefab |

34 PNG mới có nền trong suốt, ô 256×256, PPU 120 và pivot tâm. Atlas nguồn, prompt từ ImageGen tích hợp và script xuất ở [HexcoreForm01](player-art/HexcoreForm01). Export giữ silhouette chính và mốc chân y208. Pose bay Q1 frame 05 cao hơn mốc 13 px. SFX chưa có.

## Asset cần bổ sung

| Hạng mục | Yêu cầu sản xuất |
|---|---|
| Idle/Run SW, NW | 4 frame Idle và 4–5 frame Run mỗi hướng, chân trụ y208. SW nhìn thấy mặt/ngực hướng dưới trái. NW thấy lưng hướng trên trái. Tay kiếm và giáp phải giữ liên tục giữa pose tiếp đất và bước qua. Các sheet thử có đổi tay cầm hoặc đổi giáp giữa khung nên hai nút này đang tắt. |
| Idle NE, SE | Thêm 3 frame thở cùng chân trụ để có vòng lặp. |
| Basic và Q1 ngoài E | 12 frame Basic và 12 frame Q1 đề xuất cho mỗi hướng còn lại. Không đổi tay giữa hai nhát. Q1 giữ chân đạp và chân hãm làm mốc, ghi tọa độ chuôi và mũi kiếm ở các frame vung/chạm. |
| Pose nối | Duyệt chênh vai, mũi kiếm và chân giữa Run → Basic, Basic → Q1 và Basic → Run ở 0,5×. Bổ sung 2–3 frame cho từng điểm nối bị giật. |

## Kiểm chứng

- Unity 6000.3.25f1 biên dịch và chạy Play Mode trên bản sao. Xác nhận đủ pha của Q1 ở 1× và 0,5×, đúng frame va chạm, nối hai nhát Basic, chạy ra từ điểm dừng, reset bằng Replay. Kiểm tra Sprite Idle và Run ở sáu hướng có dữ liệu, xác nhận SW/NW đang thiếu. Log `Temp/HexcoreStage1Final.log`.
- GIF có 48 ảnh từ camera Unity. Đã xem các ảnh đại diện ở bắt đầu, trước chạm, chạm, hãm và chạy ra. Cần designer duyệt vòng chạy NE/SE, cảm giác hai nhát Basic, vị trí chân qua điểm nối và hướng nhìn ở kích thước Game View. Scene đang dùng timeline trình diễn hướng E cho combat.

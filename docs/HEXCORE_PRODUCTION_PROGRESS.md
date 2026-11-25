# HEXCORE · Checkpoint sản xuất

## Mốc visual polish

- Đã chỉnh nhịp và hit-stop, thay frame lõi chéo, thêm hai vòng Cuồng Phong Đao và ghi lại gallery trước/sau. Play Mode đã kiểm tra đủ 264 chuỗi có đổi hướng. [Báo cáo](HEXCORE_VISUAL_POLISH_REPORT.md) và [gallery](player-art/HexcoreVisual/index.html).
- Còn 263 AuthoredBlockout và 1 In Review. Ưu tiên lượt sản xuất tiếp theo theo nhận xét của [24 clip đại diện](player-art/HexcoreVisual/VisualScores.csv), đặc biệt landing/recovery của R1, đổi chân R2, chuyển tiếp tái xuất R3, điểm neo từng bóng và các chiêu aura dễ trùng.

## Mốc ban đầu đã đối chiếu

Unity 6000.3.25f1, bốn form và 32 hàng locomotion/Basic/Turn. Ma trận 24 kỹ năng gồm 33 cặp form–skill và 264 trường hợp có hướng. Mốc đầu có 1.140 PNG, 151 clip, 13 WAV và 82 GIF Unity.

| Trạng thái | Ban đầu |
|---|---:|
| In Review | 1 |
| AuthoredBlockout | 50 |
| SourcePoseBlockout | 84 |
| MissingArt | 129 |

## Đã bàn giao

- 264/264 tổ hợp phát trong scene bằng art đúng form và hướng có thể xem. Unity ghi 1 In Review, 263 AuthoredBlockout, 0 SourcePoseBlockout và 0 MissingArt. Xem [ma trận](player-art/HexcoreRemaining/HexcoreSkillMatrix.csv).
- Thêm 1.676 PNG, 213 clip kiểm tra pose và 23 cấu hình neo có 61 frame đo. 65 nguồn PNG được lưu cùng [prompt](player-art/HexcoreFinal/prompts.json). Sửa Q1 Tây Bắc, R1 Bắc Kiếm sĩ và thay nguồn R1 chéo Sát thủ trong quá trình xem lại.
- Ghi camera Unity cho 264 biến thể, 16 GIF locomotion/Basic/Turn dạng bảng tám hướng, 57 GIF tắt VFX và 18 GIF phát chậm. Bộ review có [mục lục](player-art/HexcoreFinal/Review/README.md).
- Lượt cuối chỉnh camera bám xuyên suốt Tàng Ảnh N/S và xác minh hai phân thân R3 dùng pose chém riêng trên cả bốn form, tám hướng. [Bảng khung hình](player-art/HexcoreFinal/Review/UnityPoseChecks.png).

## Còn cần sản xuất và duyệt

1. R1: kiểm tra in-between bay/đáp hướng chéo, chân trụ, bóng và điểm tiếp đất riêng. R2: khóa tiếp tuyến nhát chém và ba điểm phát ở các hướng chéo. R3: rà hướng chém và vị trí xuất hiện lại của phân thân.
2. Chỉnh pose chuyển trọng tâm của Q/E/F và các bridge Basic → Skill → Basic/Run. Các dải tám frame giải quyết phạm vi xem, một số đòn nhiều pha còn cần in-between và độ dài giữ pose theo nhịp.
3. Đo neo các hướng chưa đo, ưu tiên tip, foot, aura/projectile. Các hiệu ứng ở các frame đó đang dùng tọa độ dựng nháp. Xem và nghe Game View trên máy người dùng trước khi nâng trạng thái.

## Quy trình và phạm vi

- Artwork xuất riêng theo form và hướng. Import dùng Point, 120 PPU, pivot của form và canvas 256 hoặc 320 px theo nguồn. Q2 dùng atlas các góc cơ thể riêng. Scene nối pose và metadata vào timeline hiện có.
- Hàng có art mới vẫn ở AuthoredBlockout trong lúc cần sửa in-between, chân trụ, cầu nối hoặc điểm neo. CSV do Unity xuất là nguồn đối chiếu trạng thái từng hàng.
- Vòng kiểm tra tiếp theo ưu tiên kiểm tra mắt ở 0,5×, thay các frame có phụ kiện/tay đổi bên, rồi bổ sung điểm neo vũ khí, projectile, aura và cầu nối ở từng pha.

## Tái chạy kiểm thử

1. Dùng bản sao dự án nếu workspace đang mở trong Editor. Bản kiểm thử cục bộ ở `Library/HexcoreValidation`.
2. Chạy Unity 6000.3.25f1 với `-batchmode -projectPath "<bản sao>" -executeMethod HexcoreSkillPreviewSetup.BuildAndVerify -hexcoreMatrixCapture -hexcoreCapture -logFile "<đường dẫn log>"`. Unity thoát theo mã từ bài kiểm thử.
3. CSV và ảnh camera nằm dưới `<bản sao>/Library/Hexcore*`. Dùng [Export-MatrixReview.ps1](player-art/HexcoreRemaining/Export-MatrixReview.ps1) với `-CaptureRoot "<bản sao>/Library/HexcoreMatrix" -OutputRoot "docs/player-art/HexcoreFinal/Review"` để tạo GIF.
4. So sánh các pose tắt VFX, clip chậm, mốc đáp đất, điểm phóng và chuỗi chuyển tiếp ở Game View trước khi đổi trạng thái.

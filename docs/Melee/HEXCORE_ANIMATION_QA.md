# HEXCORE · Kiểm thử animation

## v2.1 · Kiểm thử sáu chỉnh sửa

Roslyn runtime và Editor **PASS**. 152 phép thử hình học offline **PASS** trên tám hướng. Các fixture mới trong `VerifyV2` bao phủ sáu kỹ năng × tám hướng × hai bước thời gian. Chúng kiểm đếm Basic, miễn đẩy boss, bia thứ sáu, chém sau điểm đáp, chống trùng làn Song Ảnh và thời điểm bia xa gặp sóng Đao. Fixture, Game View, SFX và hồi quy R đều **NOT TESTED** do project đang bị khóa. [Kết quả và GIF](HEXCORE_VISUAL_POLISH_REPORT.md)

## v2.1 Rev2

PASS Roslyn runtime/Editor, đối chiếu catalog tĩnh và 78 kiểm tra hình học vùng tác động offline. Ma trận Rev2 gồm 336 hàng kỹ năng và 32 Basic. Play Mode, Game View, âm thanh và hồi quy R là NOT TESTED vì project chính đang bị khóa trên máy có Unity 6000.3.25f1. Runner v2 được cập nhật cho hai slot kế thừa, số kiếm rơi, chuỗi truy hồn, một bia Xuyên Tâm, hai tốc độ Basic và Bá Vương với 0/1/5/9 bia đủ tám hướng. [Ma trận](HEXCORE_V21_REV2_TEST_MATRIX.csv) · [Báo cáo](HEXCORE_VISUAL_POLISH_REPORT.md)

## v2.1 · Mẫu giai đoạn 1–2

Roslyn runtime/Editor, 42 phép thử hình học vùng tác động offline và đối chiếu pixel thân trước–sau trên chín PNG Run Kiếm sĩ đều PASS. Runner `HexcoreSkillPreviewSetup.VerifyV21Samples` có 64 ca mẫu và cờ `-hexcoreV21Capture`. Play Mode, Game View, đồng bộ va chạm, âm thanh, kiểm tra viền sau import và hồi quy R đều NOT TESTED do khóa project, với Editor khớp `6000.3.25f1` đã có sẵn. [Bộ hình, GIF và bảng kết quả](HEXCORE_VISUAL_POLISH_REPORT.md)

## Refactor thư mục và runtime · 2026-11-26

Roslyn biên dịch qua trước và sau trên project chính. Đối chiếu 3.377 GUID của scene, 24 định nghĩa vẫn giữ thứ tự và 264 hàng hướng. Scene có đúng hai thay đổi serialize, tên lớp MeleeController và reference bia sprite Brown. Kết quả Play Mode và camera bên dưới thuộc các phiên trước. Lần chạy mới trước/sau bị khóa project; chưa có Editor 6000.3.22f1 trên máy. [Chi tiết bàn giao](HEXCORE_MELEE_CURRENT_STATE.md)

## Đợt giao diện và điều khiển

- Mở `Assets/Scenes/HexcoreSkillPreview.unity` rồi bấm Play. Trong **Tự do**, WASD hoặc mũi tên đi tám hướng, Space thi triển kỹ năng đã chọn và R về điểm đầu. Khi chiêu sử dụng quỹ đạo riêng, giữ phím di chuyển để nhân vật chạy tiếp sau khi chiêu kết thúc. **Thử tại chỗ** bắt đầu lần thi triển mới từ vị trí hiện tại. **Trình diễn** dùng để chọn hướng riêng và xem lại pose, chuỗi, tốc độ hoặc bước khung.
- Unity 6000.3.25f1 chạy `HexcoreSkillPreviewSetup.VerifyInteraction` trong Game View trên bản sao. Thiết bị Keyboard của Input System được cấp trạng thái phím cho 32 trường hợp form–hướng. Kiểm tra hướng dịch, chuyển Run → Idle và frame chạy chéo đúng dải. Kiểm tra Space, dịch chuyển 2,05 đơn vị của Kiếm Lướt, đường bay và điểm đáp của Thiên Vẫn, phát lại tại chỗ và R. Bài ma trận xác minh 264 tổ hợp của 24 kỹ năng.
- Vạn Ảnh dùng riêng `HexcoreGoldVfx.mat`. Đối chiếu ảnh camera Sát thủ hướng Đông Nam theo cùng thời gian cho thấy hai frame thân trước hiệu ứng trùng pixel. Ở hai frame kết liễu, lần lượt có 778 và 612 pixel trở về nền scene, với nét vàng vẫn nhìn thấy. Bài test xác minh shader được hỗ trợ và kiểm tra pixel hồng do lỗi material.
- Quét alpha của 3.128 frame nhân vật từ bốn thư mục tạo hình. Kiểm tra bốn góc canvas và các cạnh của khung alpha. Có 0 frame đạt ngưỡng nhận diện ô xám. Duyệt hình theo ảnh Game View và các khung camera lấy mẫu.
- Chạy hồi quy `VerifyExisting` cho chế độ Trình diễn. Q1, Q3 và ba lõi mẫu đều chạy ở 1× và 0,5×. Ma trận 264 tổ hợp và Replay cũng qua kiểm tra. Log cục bộ lưu ở `Temp/HexcoreDockVerified.log` và `Temp/HexcoreDockRegression.log`.
- [Game View trước](HexcoreInteraction/GameViewBefore.png) · [Game View sau](HexcoreInteraction/GameViewAfter.png) · [Vạn Ảnh trước/sau](HexcoreInteraction/VanAnhBeforeAfter.png). Ảnh sau ghi ở lúc Đao sĩ đang ở trên không trong lần thi triển bằng bàn phím.

Để ghi lại, đóng Editor đang mở project chính rồi chạy Unity với `-executeMethod HexcoreSkillPreviewSetup.VerifyInteraction -hexcoreInteractionUi`. Game View lưu ảnh vào `Library/HexcoreInteraction`. Có thể chạy cùng phương thức ở `-batchmode` để kiểm tra tương tác và camera.

## Đợt visual polish

- `VerifyExisting` vào Play Mode bằng scene đã lưu trên bản sao Unity 6000.3.25f1. Các lượt `Temp/HexcoreVisualFinal.log` và `Temp/HexcoreVisualR2Final.log` dùng scene này. [Ma trận runtime](HexcoreVisual/HexcoreRuntimeReview.csv) ghi 264 hàng phát được, 264 Replay/reset và 264 chuỗi kiểm tra Basic ở hai phía, Turn và Run với hướng di chuyển khác hướng đánh.
- 18 lượt chạy theo đồng hồ ở 1× và 0,5× đo tín hiệu có thật ở AudioListener. Sáu MP4 lõi lấy ảnh và PCM tại Play Mode, stereo 48 kHz. Các mức peak được lưu trong [CSV listener](HexcoreVisual/HexcoreAudioReview.csv) và [CSV video](HexcoreVisual/HexcoreMovieAudio.csv). Việc nghe cân bằng âm trên thiết bị phát cần người duyệt xác nhận.
- [Gallery](HexcoreVisual/index.html) có 2.112 GIF đã giải mã được. Mỗi tổ hợp có clip trước, clip sau, bản tắt VFX và clip chuỗi ở 1× và 0,5×. GIF 0,5× dùng cùng ảnh với thời lượng khung nhân đôi. Edge qua kiểm tra bộ lọc, chuyển hướng, phát lại, đọc MP4 và chiều rộng mobile 390 px. [Bảng điểm](HexcoreVisual/VisualScores.csv) dựa trên các ảnh và clip đại diện của 24 kỹ năng. [CSV visual](HexcoreVisual/HexcoreVisualReview.csv) giữ trạng thái từng tổ hợp.
- Để ghi lại, chạy Unity với `-batchmode -projectPath "<project chính>" -executeMethod HexcoreSkillPreviewSetup.VerifyExisting -hexcoreVisualReview -logFile "<log>"`. Dùng `Export-VisualReview.py "<project chính>/Library/HexcoreVisualReview"` bằng Python có Pillow. Dựng lại GIF mốc trước từ thư mục camera cũ với `--baseline`. Chép các CSV kiểm thử và video, rồi chạy `Build-VisualReview.py` để tạo manifest và CSV visual.

## Đợt hoàn thiện hướng

- Bài test Play Mode lấy mẫu từng timeline với VFX bật/tắt, kiểm tra sprite, pha, VFX, Replay và các cảnh báo thiếu art. CSV thêm `measured_frames` để phân biệt frame có điểm neo được đo với các frame dùng tọa độ mặc định.
- Bài chạy theo đồng hồ kiểm tra chuỗi Basic → Skill → Basic → Run, pha chuẩn bị, va chạm, phục hồi, camera, phát cue, phân thân R3 và reset sau Replay. Q1 E có bài test riêng. Q3 và ba lõi R có mỗi loại hai form/hướng hoặc hai hướng, chạy ở 1× và 0,5×. GIF 0,5× được tạo bằng kéo thời lượng mỗi khung từ ảnh camera.
- Bài test ma trận kiểm tra hai phân thân của R3 tại pha Clone trên 32 tổ hợp form–hướng. Clone chọn frame chém cùng hướng từ chuỗi R3, gồm cả hai bộ Hero hướng E/W.
- Kiểm tra tĩnh 13 WAV mono PCM 16-bit / 24 kHz: đầu và cuối đều bằng 0, peak nguồn lớn nhất 0,800. Cue chồng được giảm gain còn 0,7; AudioSource dùng volume 0,38. Cần nghe duyệt âm sắc, độ lớn và nhịp bằng thiết bị trong Game View.
- Đối chiếu atlas trước khi xuất. Loại nguồn Sát thủ R1 có nền xám và dùng atlas nền trong mới. Thay frame nén R1 N Kiếm sĩ để giữ giáp vai và kiếm cùng bên với frame đáp. Duyệt thủ công riêng các frame neo của Q3, R1 N/S và R2 N/S.

Unity 6000.3.25f1 trên bản sao kiểm tra 264/264 timeline, 264/264 Replay reset, 32 hàng Idle/Run, Basic và Turn. Có 23 hàng mới với tổng 61 frame đo neo. Lượt cuối ghi PASS trong `Temp/HexcoreFinalFraming.log`. Log có ngoại lệ lập chỉ mục từ `UnityEditor.Search` lúc Editor tải. Visual QA đã đối chiếu các cảnh đại diện trong bảng ảnh, giảm độ che của VFX kết liễu R3 và sửa camera để Tàng Ảnh hướng Bắc/Nam luôn nằm trong khung. Đã xem lại ảnh sau khi sửa. Cần xem động từng biến thể ở kích thước chơi, kiểm tra chân trụ và chuyển phase trước khi nâng trạng thái.

Mở [355 GIF Unity](HexcoreFinal/Review/README.md), [bảng khung hình](HexcoreFinal/Review/UnityPoseChecks.png), [CSV kỹ năng](HexcoreRemaining/HexcoreSkillMatrix.csv) và [CSV bốn form](HexcoreRemaining/HexcoreFormMatrix.csv). Giải mã được cả 355 tệp GIF. Khung đầu của GIF R3 Kiếm khí NE có thời lượng 70 ms ở bản thường và 140 ms ở bản chậm. Các con số ở phần dưới là kết quả của các đợt trước.

## Kiểm thử ma trận mở rộng

- `BuildAndVerify -hexcoreMatrixCapture` giữ bài test chạy theo đồng hồ thật ở 1×/0,5× cho chuỗi Q1 E. Kiểm tra các cue đạp chân, lao, vung và chạm đã đi qua lại sau mỗi Replay.
- Bài test ma trận đi qua 33 cặp form–skill × 8 hướng. Với hướng có thể phát, lấy mẫu từ chuẩn bị qua phục hồi theo bước thời gian, đếm sprite khác nhau, kiểm tra bật/tắt VFX và Replay. Với hướng thiếu art, xác minh thông báo và không để VFX chạy. `SourcePoseBlockout` ghi animation thử từ pose nguồn cùng hướng. `AuthoredBlockout` ghi bộ pose riêng đang cần duyệt. `MissingArt` ghi hướng thiếu pose. Các trạng thái này không phải kết luận đồ họa đã đạt.
- Thử Idle, Run, Basic và Turn sang hướng kề trên bốn form và tám hướng. Basic kiểm tra toàn bộ 12 hoặc 8 frame theo hướng, không chỉ frame đầu. Đầu file WAV đã về 0, mono PCM 24 kHz, peak dưới mức clip.
- Visual QA dùng [bộ GIF](HexcoreRemaining/Review/README.md). Hai bản Q1 NW có lỗi đổi tay hoặc giáp vai nên đã rút khỏi runtime. Bản đầu Dao sĩ R2 W bị lẹm đầu đao, đã thay nguồn riêng. F2 Kiếm khí đã thay atlas để lưỡi kiếm luôn gắn vào tay và projectile nằm ở lớp hiệu ứng.

[Kết quả skill theo tám hướng](HexcoreRemaining/HexcoreSkillMatrix.csv) · [Kết quả locomotion, combo và Turn](HexcoreRemaining/HexcoreFormMatrix.csv)

Unity 6000.3.25f1 chạy Play Mode trên bản sao và ghi PASS trong `Temp/HexcoreRemainingVerified.log`. 32 hàng form–hướng có Idle/Run, Basic và pose Turn. Ma trận kỹ năng có 135 hàng phát được và 129 hàng thiếu art; nhóm phát được gồm 84 hàng dựng nháp bằng pose nguồn. Đã đối chiếu ảnh theo các pha của Q1, ba lõi và một số combo/Turn. Trace lập chỉ mục của Unity Search xuất hiện lúc Editor khởi động. Âm thanh thiết bị và chất lượng toàn bộ in-between cần duyệt trực quan; bài test theo đồng hồ ở hai tốc độ tập trung vào Q1 E.

Phần bên dưới ghi lại bài kiểm tra của mốc trước.

## Kịch bản tự động

`HexcoreSkillPreviewSetup.BuildAndVerify` dựng scene bằng Unity đã cài, import sprite, tạo clip, kiểm tra PPU/pivot và vào Play Mode. Truyền `-hexcoreCapture` để ghi ảnh camera kỹ năng, lấy mẫu GIF và contact sheet. Khi chủ động dựng lại, chạy trên project chính sau khi đóng Editor đang mở và kiểm tra Git.

- Chuỗi Q1 E chạy theo đồng hồ runtime 1× và 0,5×. Kiểm tra cả hai nhát Basic, bảy pha Q1, frame chạm, chạy khỏi điểm dừng và Replay.
- Kiểm tra bốn prefab có Animator và SpriteRenderer, sprite của các hướng mới có đủ bộ 5; bộ xếp riêng cho mỗi form giữ thiết lập nhập nhất quán.
- Đi qua 24 ID, tổng cộng 33 lựa chọn form × skill, lấy mẫu các beat ở N/E/S/W với VFX bật/tắt trong Play Mode. Xác nhận pose riêng E cho 20 kỹ năng Q2/Q3/E/F, đúng frame chạm cho Q1 E/S/W và pose riêng E cho cả ba lõi trên bốn form. Q1 N báo thiếu pose.
- Chụp 33 khung hướng Đông trong Unity rồi duyệt trực quan. Qua lần xem đầu, giảm kích thước VFX che thân và bỏ texture có dáng nhân vật dựng sẵn khỏi nhóm lướt Sát thủ.

Lần chạy bàn giao xuất 24 GIF. Sau khi chuyển VFX kết liễu của Vạn Ảnh ra sau nhân vật, chạy lại và đối chiếu ảnh camera. Lần cuối ghi PASS trong `Temp/HexcoreProductionVisual.log` và scene được sao chép nguyên vẹn từ project đã kiểm chứng. Kết quả locomotion 8/7/8/8 theo thứ tự form. Log có ngoại lệ Unity Search khi Editor lập chỉ mục; vị trí ngoại lệ thuộc UnityEditor.Search.

## Cần duyệt bằng Game View

- Đối chiếu cả bốn form với ảnh gốc khi đổi hướng. Xác nhận giải phẫu tay kiếm tay đao, độ rộng khăn/giáp, chân trụ và vòng Idle/Run ở các đường chéo. Mỗi Idle chéo hiện là một pose giữ.
- Kiểm tra Basic E riêng và chạy nối Basic → Skill → Basic. Duyệt xem từng nhát được đọc đúng thứ tự, có bước đổi chân và đủ pose thu. Hướng ngoài E cần vòng kiểm tra riêng sau khi có asset.
- Thiên Vẫn: kiểm tra chân và vũ khí lúc đáp, tâm nứt đất và VFX sau khi tắt/bật. Tam Thiên: kiểm tra mỗi nhát phát sóng từ mũi kiếm, nhát ba mạnh nhất, nhịp tụ lực rõ. Vạn Ảnh: kiểm tra đúng tạo hình của từng phân thân, hai hướng giao nhau, khoảng reappear và đòn kết.
- Thử nhiều lần với âm thanh và hit-stop. Hai âm tổng hợp hiện là cue dựng nháp. Bố cục IMGUI ở độ phân giải thực tế và âm thanh phát qua thiết bị vẫn cần xác minh.

Xem [contact sheet Unity](HexcoreProduction/UnityCatalog_East.png) và [24 GIF hướng Đông](HexcoreProduction/README.md).

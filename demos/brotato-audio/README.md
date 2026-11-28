# BROTATO 2D · Audio Preview

Vòng Foley mới tái phối 20 mục, có **Bản phối trước** và **Loop rời** ở các cue phù hợp. Unity Studio có thêm lọc nhánh, lưu nhận xét và take nguồn cho bảy tuyệt kỹ. [Kiểm tra phiên này](../../docs/HEXCORE_AUDIO_QUALITY_REPORT.md)

Mở [index.html](index.html) bằng Chrome hoặc Edge trong thư mục dự án. Nếu mở theo đường dẫn `file://` không phát được WAV, chạy `python -m http.server 8062` từ thư mục gốc của dự án rồi truy cập [Audio Preview](http://localhost:8062/demos/brotato-audio/). Giữ cấu trúc repo khi chuyển thư mục để link ảnh, GIF và cue cũ vẫn trỏ đúng.

## Nghe và duyệt

1. Chọn một trong 10 danh mục hoặc gõ tên kỹ năng trong ô tìm. Tìm kiếm nhận cả tiếng Việt có dấu và không dấu.
2. Chọn bản nghe thử, bấm **Phát** hoặc **Dừng**. Chọn **Bản phối · Hụt** ở cue có hit mục tiêu để nghe các pha còn lại. Các điểm chạm địa hình vẫn thuộc bản phối hụt khi kỹ năng luôn tạo va chạm đất.
3. Bấm vào tên một lớp để nghe solo. Mỗi stem của hiệu ứng giữ trục thời gian chung của bản phối. Các con số là mốc bắt đầu sự kiện trong stem. **Cue cũ** mở file nguồn dùng để đối chiếu. **Tải WAV** tải đúng bản đang chọn.
4. Chỉnh âm lượng chung. Đánh dấu **Chờ duyệt**, **Cần chỉnh** hoặc **Đã chọn** theo biến thể. Trạng thái lưu tại trình duyệt và địa chỉ đang mở. GIF chỉ hỗ trợ quan sát động tác, đồng bộ khung hình chính xác là công việc của đợt Unity.

## Ba phương án nhạc Concept B

Vào **Nhạc nền → Main Menu · Đại chiến sinh tồn**, hoặc mở [mục Main Menu](index.html#cue=138). Mỗi bản có loop 16 ô nhịp và stem trống, bass/hòa âm, giai điệu.

| Bản | BPM | Màu âm và tiết tấu |
| --- | ---: | --- |
| **01 · Hẻm nổ** | 142 | Dây gảy staccato, bass bật ngược phách và bộ gõ chắc. Mở menu có nhịp vui và rõ |
| **02 · Đường băng** | 126 | Brass mềm hơn, nhịp đẩy lơi và điểm gõ giấy. Hợp phiên nghe lâu khi duyệt các panel |
| **03 · Lõi vượt tốc** | 158 | Mallet ngắn, trống đảo phách và nhịp dồn. Năng lượng cao và nhiều cạnh hơn |

## Tệp và phạm vi bản duyệt

- [Bảng kiểm kê theo 146 mục](INVENTORY.md) có 311 bản phối, 1.054 stem, 192 bản hụt, 34 mục so được với WAV cũ và nguồn Foley theo hàng. [Hồ sơ giấy phép](LICENSES.md) có tác giả, bản tải và mã băm.
- `audio/` chứa bản phối, các stem và bản hụt dưới dạng PCM WAV 16-bit/32 kHz. SFX thường là mono, hiệu ứng không gian và nhạc là stereo. Đây là mức export để nghe duyệt. Pitch, độ rộng stereo, giới hạn voice, peak khi phối chung và thời điểm phát vẫn cần cân lại trong game.
- Tài nguyên chưa có để làm bản chính thức gồm diễn viên thu tiếng người/quái, ghi âm hiện trường riêng cho bốn môi trường, và cue thực chiến cho Boss bản đồ, pickup, pause/resume hay trạng thái mới đang chờ cơ chế. Các mục tương ứng có thiết kế nghe thử bằng Foley và tổng hợp. Số bản biến thể, vòng ambience 14 giây và nhạc loop hiện là bộ duyệt. Độ dài 30–48 giây hoặc 48–72 giây và số lượng biến thể đích trong kế hoạch thuộc bước hoàn thiện master sau duyệt.
- Bản preview đặt ngoài `Assets/`. Phiên này cập nhật kế hoạch và hồ sơ nguồn. Các thao tác trong demo không chạy gameplay Unity.

## Dựng lại bản xuất

Chạy `python -X utf8 demos/brotato-audio/render.py` từ gốc repo. Bộ dựng đọc bảng kế hoạch, vật liệu, mốc phối và WAV Foley đã giải mã để xuất lại thư viện theo seed. Có thể thêm các STT cần render vào cuối lệnh khi catalog đã tồn tại. Chạy `python -X utf8 demos/brotato-audio/verify-preview.py` để kiểm tra tệp và cập nhật kiểm kê.

Nếu cần giải mã lại OGG, chạy `node demos/brotato-audio/decode-foley.mjs "<chrome.exe>" demos/brotato-audio/foley` bằng Node có WebSocket toàn cục. Chrome chỉ giải mã các tệp cục bộ.

## Kiểm tra

Đã đối chiếu 1.557 WAV với catalog, kiểm tra format, độ dài, mẫu clipping, hash của 50 nguồn Foley, 3 track menu khác nhau và bản hụt của Thiên Vẫn còn điểm đáp. Chrome đã mở demo trực tiếp từ file và phát bản phối, bản hụt, cue cũ, stem và nhạc menu. Kiểm tra lưu duyệt, âm lượng, tìm kiếm không dấu và bố cục ở 1440 px và 390 px. Cần nghe duyệt chất âm trên thiết bị mục tiêu và xác minh cue trong Unity khi chuyển sang tích hợp.

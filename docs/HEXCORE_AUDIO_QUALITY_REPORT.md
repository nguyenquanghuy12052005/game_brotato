# BROTATO 2D · Kiểm tra nguồn và vòng phối Foley

## Phạm vi đã thực hiện

Đã chạy sáu scene nguồn bằng Unity 6000.3.22f1 Direct3D 11 trên bản sao. Sân Cận chiến có 28 cấu hình, sân Súng có 30 (gồm Boss sân thử), Magical có 30. Mỗi cấu hình được chạy và lấy mẫu ở 1×, 0,5×. GameScene, MainMenu và CharacterProgressionTest có smoke test mở và kết xuất. Bảng 146 mục ghi rõ trường hợp mới quan sát được cấu hình và trường hợp đã có take nguồn. Không dùng ảnh tĩnh hoặc thay đổi HP của cả sân để suy ra mọi proc đều đã nổ đúng. [Bảng 146 mục](HEXCORE_AUDIO_SPOTTING.md) · [Nhật ký chạy nguồn](../demos/brotato-audio/reference/source-observations.csv)

Ảnh trích đặt ba khung 1× bên trái và ba khung 0,5× bên phải. Mỗi hàng dùng ID của kế hoạch. [Súng](../demos/brotato-audio/reference/gun-frames.png) · [Cận chiến](../demos/brotato-audio/reference/melee-frames.png) · [Pháp sư](../demos/brotato-audio/reference/mage-frames.png) · [Tuyệt kỹ](../demos/brotato-audio/reference/hexcore-spots.png)

Vòng này làm lại 20 cue có lựa chọn A/B. Gồm 10 tuyệt kỹ, 6 cue Cận chiến tiêu biểu và 4 âm sắc nguyên tố. 130 mục còn lại giữ phối thử trước. Mỗi bản mới có WAV đầy đủ, stem và phiên bản hụt khi có lớp va mục tiêu. Waveform được đo để so mốc, năng lượng và headroom. Môi trường tự động không có đường nghe kiểm âm đã hiệu chuẩn. **Chưa xác nhận độ hay, độ tự nhiên, cảm giác lực, độ mệt tai hoặc chất lượng phối khi nghe lặp bằng tai.** Mọi cue mới giữ trạng thái Chờ duyệt. [Kiểm kê WAV](../demos/brotato-audio/INVENTORY.md) · [Giấy phép Foley](../demos/brotato-audio/LICENSES.md)

## Những điểm đã chỉnh theo nguồn

| ID | Điều quan sát hoặc đối chiếu runtime | Thay đổi trong bản phối thử |
| --- | --- | --- |
| 45 | Tên hiển thị **Lốc lửa**, hình nguồn là xoáy băng. Cột tồn tại 7 giây, tiếp xúc tùy đường chạy | Gió lạnh duy trì, hạt kính theo vòng và tiếng rút. Take hình dùng lượt 0,5× quy đổi về thời gian game. Mốc hit chỉ đại diện fixture |
| 46 | Năm thiên thạch cách nhau 0,22 s. Code đặt va tại frame 19/40 fps. Bia mất HP lần đầu được lấy mẫu gần 0,458 s | Năm tiếng rơi và năm va đất ở 0,475 / 0,695 / 0,915 / 1,135 / 1,355 s. Bỏ hit mục tiêu vẫn giữ đá chạm nền |
| 47 | Trụ phát sóng ngay lúc đặt, sau đó mỗi 1 s, tồn tại 15 s | Tách chân đế, dòng plasma, 15 xung và nhả khóa. Bản hụt duy trì toàn bộ xung |
| 48 | Vùng mở rồi bắn hai tia vào tâm ở mỗi nhịp. Sáu nhịp bắt đầu 0,44 s và cách 0,28 s | Sáu cặp xung thiên về tiếng xé gió và chốt cơ khí. Hạ độ dày giữa nhịp, đặt sập lồng ở 2,14 s |
| 53–56 | Kiếm Lướt có đạp chân/chuốt kiếm; Hoành Nguyệt và Phản Kiếm là proc; Ám Bộ biến mất rồi có hai nhát đảo hướng | Foley vải và gỗ tách lớp, thép đối đáp và hai âm cạnh riêng. Cue 53 đối chiếu pose Kiếm Lướt gốc theo fixture; Phong Ảnh Trảm trong Rev2 là biến thể proc |
| 62, 68 | Một đao đáp đất đối lập với thế phóng sóng bán nguyệt | Chấn Địa dùng lực đập ngắn và đá nhỏ. Nguyệt Nha dùng cạnh kim loại, vệt gió sáng rồi va mỏng |
| 74 | Lấy đà, lao xuống, một điểm chạm và các vòng lan | Vải bật, dải lao xuống, va đất chính 0,77 s trên đồng hồ preview. Hai lớp dư chấn đi sau tại 0,96 và 1,20 s. Tiếng trúng bia nằm riêng |
| 75 | Ba đợt kiếm khí mở rộng. Mỗi đợt giữ gốc phóng và vùng riêng | Dùng ba chất liệu cắt, xòe và rách không gian với độ dài khác nhau, mốc preview 0,38 / 0,97 / 1,81 s |
| 76 | Hai lượt bốn bóng và một nhát kết, không phải tám tiếng nổ | Hai cụm lát mỏng lấy từ swish ngắn, có khoảng trống tách cụm, kết chặt và đuôi hụt thấp |
| 81, 87, 93, 99 | Hỏa nổ lúc cầu chạm, Băng kiểm hình nón ngay cast, Quang có bốn xung mỗi 0,25 s, Ám bám đích rồi mới đặt nguyền | Tách bụp và di chuyển Hỏa, khí lạnh và tiếng chạm tinh thể, bốn nhịp cộng hưởng Quang, hút tối và khóa mềm Ám. Mốc bay đại diện khoảng cách fixture |
| 105 | Vòng hiện khi bắt đầu. Khóa thời gian 3 s, sát thương đang treo chỉ giải khi có tích lũy | Chốt tam âm sát đầu, lớp không khí giữ nhỏ, giãn vòng tại 3 s; tiếng trả sát thương ở stem mục tiêu |
| 106 | Năm lần áp sát thương theo mục tiêu, vòng đặt từ đầu và chu kỳ 0,4 s | Năm cụm chuông/thủy tinh. Căn sự kiện ở 0 / 0,4 / 0,8 / 1,2 / 1,6 s. Không rải trời giả giữa các xung |
| 107 | Cầu giữ tối đa 8 s, có thể nhả sớm. Số phi đạn bằng 2 cộng lượng tụ | Dòng tụ liên tục, giải phóng và vệt đạn tại 8 s trong fixture timeout. Hai bản cho 8 và 2 phi đạn. Take hình chỉ gắn bản 8 phi đạn |

## Việc còn chờ xác minh

- Cần bắt riêng setup hit đối với đạn nảy, xuyên thấu, bung tỏa, các proc ngưỡng Súng và Mage theo nhiều mục tiêu, các nhánh kit bắt buộc lựa chọn riêng, và starter Mage khi fixture chưa ghi rõ VFX. WAV hiện là lượt minh họa fixed-distance. Không khóa cue với đạn thật trước khi lấy event spawn/contact.
- Sân Súng có `IndexOutOfRangeException` trong `GunSkillWeapon.PlayMuzzleFlash` khi chạy vài cấu hình tự động. Đây là lỗi phát hiện trong scene nguồn. Đợt âm thanh giữ nguyên gameplay và VFX gốc. [Vị trí code](../Assets/Scripts/Weapon/GunSkills/GunSkillWeapon.cs#L154)
- Boss bản đồ Xương khô, Golem, Yeti còn ở tài nguyên/đề xuất phần AI chiêu phù hợp. Cần fixture riêng cho đòn và chuyển phase. Nhặt EXP rơi/vật phẩm thật, nhảy tự do, Chơi tiếp với save thật, Pause/Resume gameplay và một số visual cảnh báo vẫn chờ cơ chế hoặc đường nối. Giữ cue tham chiếu đã có và không tự tạo hệ thống mới.
- Các trạng thái rà thêm từ source gồm hết tầm đạn, đổi nhịp tích cooldown, cleanup chuyển wave/map, vô địch, phục hồi do chuyển map, đứng yên, đóng menu bằng ESC, hết thời gian cố định và nút debug N/M. Dùng im lặng với cleanup, đạn đi hết tầm, tick mỗi frame, đứng yên và thao tác debug. Trạng thái hữu ích nghe bằng tai đã có dòng tương ứng trong ma trận.
- Hình và âm được đối chiếu bằng chạy nguồn, contact sheet và sample clock. Chưa có kiểm tra nhìn/nghe đồng thời từng frame của 146 mục. Cần nghe trên loa/tai nghe và trong mix nhiều đòn trước khi chốt. Năm cue duy trì có loop rời đã crossfade tại điểm nối và hai phiên bản mỗi cue. Cần kiểm tra lại vòng vào/ra theo trạng thái khi tích hợp gameplay.

## Kiểm tra kỹ thuật bản xuất

- Catalog hiện tham chiếu 311 bản phối, 1.046 lớp và 192 bản hụt. 20 cue phối lại giữ đường dẫn tới WAV của lần trước. Kiểm tra cả 1.549 WAV đang được tham chiếu, gồm 10 loop rời. Peak mẫu lớn nhất khoảng −1,5 dBFS. Kiểm tra này xác nhận số mẫu PCM, thời lượng, các bản A/B khác nội dung, độ gián đoạn biên đối với loop và hash của 61 nguồn. Nó không đánh giá loudness cảm nhận hoặc true peak của phối trong trận.
- Play Mode bản Studio cuối trên bản sao đã gọi sân minh họa cho 146 mục và kiểm tra các luồng đại diện. Bài test giải mã take Mưa thiên thạch, phát WAV trước khi phối lại, lưu và đọc lại nhận xét tiếng Việt, nghe độc lập loop Pháo đài qua điểm nối, cùng kiểm tra 0,5× và ba toggle trên rig Cận chiến. Việc play qua điểm nối xác nhận đồng hồ tiếp tục chạy; độ mượt cần nghe lại.
- Studio đọc hai nguồn clock audio và video bằng sample/audio và external reference time của Unity. Video không cung cấp các lớp tách riêng. Bản camera source có thể sai khác sắc độ nhẹ qua Windows Media Foundation. [Unity VideoPlayer externalReferenceTime](https://docs.unity3d.com/6000.0/Documentation/ScriptReference/Video.VideoPlayer-externalReferenceTime.html)

[Mở Studio và xem cách duyệt](HEXCORE_UNITY_AUDIO_PREVIEW.md)

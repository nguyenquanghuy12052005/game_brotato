# HEXCORE · Visual polish

## v2.1 · Tinh chỉnh theo phản hồi mới

| Mẫu | Thay đổi và cơ chế demo | So sánh offline |
|---|---|---|
| Basic Kiếm khí | Hai pose thuận–nghịch có điểm phóng riêng trên cả tám hướng. Cùng bán nguyệt xanh ngọc, bay 2,15u trong 0,21 giây. Mỗi sóng có danh sách trúng riêng và chỉ cộng một lần tích bị động nếu chạm ít nhất một bia. | [1×](HexcoreVisual/V2_1/Rev2/Feedback/Basic-Kiem-khi-1x.gif) · [0,5×](HexcoreVisual/V2_1/Rev2/Feedback/Basic-Kiem-khi-05x.gif) |
| Nguyệt Nha Trảm | Bán nguyệt lớn bay 5u, đẩy bia thường 0,63u theo hướng bay trong 0,19 giây, phát xung tại điểm chạm. Boss mô phỏng nhận damage nhưng giữ vị trí. | [1×](HexcoreVisual/V2_1/Rev2/Feedback/Nguyet-Nha-1x.gif) · [0,5×](HexcoreVisual/V2_1/Rev2/Feedback/Nguyet-Nha-05x.gif) |
| Phi Kiếm Truy Hồn | Tối đa 5 bia khác nhau. Ở mỗi chặng, mũi kiếm quay theo hướng chuyển động cong và từng điểm chạm phát tia và cue riêng. Tầm tìm đầu 7u, giữa các bia 3,6u. | [1×](HexcoreVisual/V2_1/Rev2/Feedback/Phi-Kiem-1x.gif) · [0,5×](HexcoreVisual/V2_1/Rev2/Feedback/Phi-Kiem-05x.gif) |
| Ám Bộ Trảm | Giữ dịch chuyển đầu. Sau lướt, quay theo bia phía sau điểm đáp, dùng hai pose chém hiện có với điểm neo riêng, rồi thu kiếm. Vùng trúng hướng về phía điểm chém sau lướt. | [1×](HexcoreVisual/V2_1/Rev2/Feedback/Am-Bo-1x.gif) · [0,5×](HexcoreVisual/V2_1/Rev2/Feedback/Am-Bo-05x.gif) |
| Song Ảnh Truy Sát | Hai phân thân chém ở hai bên, bản thể phóng dao xuyên làn giữa cùng nhịp. Ba đường dùng một mặt nạ chống trùng hit. Chỉ bia trúng mang dấu phi tiêu 0,50 giây. | [1×](HexcoreVisual/V2_1/Rev2/Feedback/Song-Anh-1x.gif) · [0,5×](HexcoreVisual/V2_1/Rev2/Feedback/Song-Anh-05x.gif) |
| Hoành Thiên Đao | Sóng đao phát từ mũi lưỡi, mở dần trong 0,47 giây tới biên ±40°/5u. Mỗi nhịp chỉ tính dải sóng vừa đi qua. Bia trúng được đẩy 0,48u và phát phản hồi chạm. | [1×](HexcoreVisual/V2_1/Rev2/Feedback/Hoanh-Thien-1x.gif) · [0,5×](HexcoreVisual/V2_1/Rev2/Feedback/Hoanh-Thien-05x.gif) |

[Contact sheet](HexcoreVisual/V2_1/Rev2/Feedback/ContactSheet.png) · [Atlas 31 clip](../../Assets/Prefabs/Player/Melee/Shared/VFX/PixelFX/Resources/HexcoreV21/StyleSheet.png). Hai cột GIF là bản phối Rev2 trước phản hồi và bản phối hiện tại, cùng dựng offline hướng Đông. Vòng render thứ hai giảm độ sáng của lớp dư sau sóng Đao, làm rõ hai mốc chém Ám Bộ và thêm điểm chạm cho boss minh họa. Dùng [recipe `--feedback`](Source/PixelFX/render_polish_samples.py) để render lại.

- **PASS** Roslyn runtime và Editor. 152 phép thử offline chạy trên hình học lấy trực tiếp từ C#, kiểm qua tám hướng các cạnh quạt, tầm xa, bia chưa tới, lưỡi sóng ngắn, làn giữa và hướng chém sau lướt.
- **PASS** Kiểm tra atlas, link và thời lượng của cả 12 GIF. Debug hitbox mặc định ẩn.
- **NOT TESTED** 96 ca fixture Unity mới, Game View, SFX thực tế và hồi quy ba lõi R. Unity 6000.3.25f1 có sẵn, nhưng project đang có khóa hoạt động. Lấy kết quả mới từ `HexcoreSkillPreviewSetup.VerifyV2` khi chạy được. Trong scene, bật **Bia đầu là boss mô phỏng** để thử Nguyệt Nha.

## v2.1 Rev2 · Bản triển khai demo

[Đặc tả Rev2](HEXCORE_SKILL_VISUAL_POLISH_V2_1.md) thay bản cũ tại cùng đường dẫn. [Gallery 25 bản so sánh](HexcoreVisual/V2_1/Rev2/index.html) có 50 GIF ở 1× và 0,5×. Bên phải là bố cục render offline hướng Đông, dùng pose thật và atlas đang tích hợp. Bên trái là GIF cũ, với nhãn phân biệt mẫu offline và capture lịch sử cùng ID nguồn. [Contact sheet](HexcoreVisual/V2_1/Rev2/ContactSheet.png) · [Atlas 29 clip](../../Assets/Prefabs/Player/Melee/Shared/VFX/PixelFX/Resources/HexcoreV21/StyleSheet.png)

| Nhóm | Thay đổi triển khai |
|---|---|
| Basic | Kiếm sĩ và Sát thủ giữ số nhát, đưa tâm vệt về phía neo vũ khí. Đao sĩ dùng chung cung đao cam cho hai nhát, đổi góc nhát bổ. Kiếm khí dùng cùng silhouette bán nguyệt xanh ngọc ở nhát tại chỗ và sóng bay. |
| P | P1 đi 3,5u, P2 mở vành tới 2,2u, P3 đi 3,2u và có tia đỡ riêng. Giữ ngưỡng 5/4 và hồi nội bộ P3 2 giây. P phát overlay. |
| Sát thủ | Ám Bộ có hai mép cắt ngược, Dạ Vũ có nhát cuối dài riêng và Đoạt Mệnh dùng lưỡi đâm hẹp theo quét 6,5u. Tàng Ảnh đi 4,75u và đánh tại điểm tái xuất. Huyết Ấn tiếp tục bám tối đa năm bia và nổ cùng nhịp. Song Ảnh có hai làn hoạt động cùng lúc, cờ trúng dùng chung và dấu phi tiêu theo bia 0,50 giây. |
| Đao sĩ | Chấn Địa giảm vành tròn sáng để đọc khe nứt. Hoành Thiên dùng tâm thi triển, quạt ±40° và mép xa 5u. Cuồng Phong có hai nhịp lốc. Bá Vương kế thừa pose 09 và lấy pose cắm/quét từ 07/08 khi có take. Bia thường trong bán kính 3,8u bị kéo về cách tâm ít nhất 0,78u trong pha hút 0,67 giây. Pha hút không gây hit; vòng quét cuối 2,45u. Bia đầu có toggle kháng kéo. Đoạn Sơn dùng khe nứt kéo dài. Bất Diệt giữ buff 5 giây và phát sóng theo nhát Basic trúng. |
| Kiếm khí | Nguyệt Nha giữ bán nguyệt xuyên qua bia. Lưỡng Nghi quét hai đường, duy trì dư ảnh đường đầu khi đường thứ hai đến, phát một vụ nổ ở tâm giao. Thiên Hà dùng 15 kiếm trong ba đợt, mỗi thanh có vị trí rơi và vùng tác động. Xuyên Tâm khóa một bia hợp lệ, ưu tiên bia kháng kéo trong tầm ngắm nếu có. Truy Hồn có tối đa ba hit lên bia phân biệt trong tầm nảy 3,6u. Hồi Nguyệt dùng bán nguyệt thuần, đi 7,6u và tìm điểm về theo vũ khí của nhân vật khi di chuyển; mặt nạ trúng tách theo lượt. |
| R và nguồn | Ba lõi tiếp tục lấy catalog/timeline/pose/VFX/cue nguồn. Giữ 24 chỉ số nguồn, 4 form, scene và 9 PNG đã sửa viền. Giá trị trên là fixture demo. |

### Mapping các trường hợp thay thế

| Slot Rev2 | ID pose/asset đã giữ | Nguồn cũ |
|---|---:|---|
| Đao sĩ F1 · Bá Vương Đao Vực | 09 | Bất Động Chiến Thế, vẫn giữ asset/metadata |
| Kiếm khí E3 · Kiếm Vũ Thiên Hà | 22 | Kiếm Vũ Thiên Hà ở F1 |
| Kiếm khí F1 · Thiên Kiếm Xuyên Tâm | 12 | Kiếm Giới, vẫn giữ pose và asset ấn |

### Tự đánh giá hình ảnh

Điểm sơ bộ bên dưới dựa trên hai vòng quan sát contact sheet và bố cục offline. Thứ tự là **hình / lực nhìn thấy / bản sắc / khả năng quan sát / WOW**. Độ mượt pose, khớp vũ khí và khớp vùng tác động trong Unity đều **NOT TESTED**. Các chỉ số hiệu ứng ở bên phải GIF là bố cục minh họa; dùng Game View để đo phạm vi.

| Mẫu | Điểm sơ bộ | Điều cần xem trong Game View |
|---|---|---|
| Basic Kiếm sĩ | 4/3/4/5/3 | Lực nhát hai và mũi lưỡi |
| Basic Sát thủ | 4/3/4/5/3 | Cao độ nhát đâm |
| Basic Đao sĩ | 4/4/4/4/4 | Góc cung nhát bổ và chân trụ |
| Basic Kiếm khí | 4/4/4/4/4 | Chênh thời điểm của sóng với nhát hai |
| P1 Phong Ảnh Trảm | 4/3/4/5/3 | Bia gần và kết thúc đường bay |
| P2 Hoành Nguyệt Trảm | 4/4/4/4/4 | Độ đọc vành phía sau nhân vật |
| P3 Phản Kiếm Quyết | 3/3/4/4/3 | Flash đỡ và độ tương phản của đường phản |
| Ám Bộ Trảm | 4/4/4/4/4 | Thời điểm vệt ngược ở nhát cuối |
| Huyết Ấn | 4/4/4/3/4 | Chồng sáng khi năm bia sát nhau |
| Dạ Vũ Liên Trảm | 4/4/4/4/4 | Quỹ đạo từng nhát và nhịp tan |
| Tàng Ảnh | 3/3/4/4/3 | Đọc rõ điểm tái xuất khi dash dài |
| Đoạt Mệnh | 4/4/4/4/4 | Đầu mũi đâm tại lúc chạm bia |
| Song Ảnh Truy Sát | 4/4/4/3/4 | Hai phân thân và dấu phi tiêu cùng khung |
| Chấn Địa Trảm | 4/4/4/4/4 | Nứt đất, chân và điểm chạm |
| Hoành Thiên Đao | 4/4/4/4/4 | Mép ±40° ở tám hướng |
| Cuồng Phong Đao | 4/4/4/4/4 | Hai nhịp va chạm theo từng vòng |
| Bá Vương Đao Vực | 4/4/4/4/4 | Liền pose cắm, rút và quét; bia boss |
| Đoạn Sơn Trảm | 4/4/4/4/4 | Phạm vi khe nứt và tia trúng |
| Bất Diệt Chiến Ý | 4/4/4/4/4 | Buff và sóng Basic trong cùng nhịp |
| Nguyệt Nha Trảm | 4/4/4/5/4 | Biên bán nguyệt xa và bên |
| Lưỡng Nghi Kiếm | 4/4/4/4/4 | Hai hit đường và một hit nổ |
| Kiếm Vũ Thiên Hà | 4/4/4/4/4 | Rìa vùng, phân bố và nhịp ba đợt |
| Thiên Kiếm Xuyên Tâm | 4/4/4/4/4 | Điểm chạm khi bia di chuyển hoặc mất |
| Phi Kiếm Truy Hồn | 3/3/4/4/3 | Chuyển hướng và impact rõ cả ba bia |
| Hồi Nguyệt Kiếm | 4/4/4/4/4 | Lượt về khi người chơi giữ phím chạy |
| R1, R2, R3 | NOT TESTED | [Capture lịch sử để đối chiếu](HexcoreVisual/index.html) |

### Kiểm thử Rev2

| Hạng mục | Kết quả |
|---|---|
| Roslyn runtime và Editor | PASS |
| Catalog và PNG Basic | PASS tĩnh. 24 ID duy nhất, 336 tổ hợp P/E/R/F (gồm 96 R) và 32 Basic có PNG kèm meta. [Ma trận Rev2](HEXCORE_V21_REV2_TEST_MATRIX.csv) |
| Hình học vùng tác động | PASS offline. 78 kiểm tra trên các hàm lấy trực tiếp từ C# runtime, gồm biên góc ±40°, tầm 5u ở tám hướng, mép vùng kéo/quét, lưỡi sóng và vành P2. |
| Atlas và 50 GIF | PASS offline. Đã sửa bản atlas đầu vượt kích thước import 2.048 px, kiểm lại 29 clip và thời lượng 2/4 giây. Đã quan sát hai contact sheet và giảm vành sáng liên tục của P2/Chấn Địa, tách motif P3 và chỉnh lại bia minh họa. |
| Unity Play Mode, SFX, camera, đồng bộ tám hướng, viền xám sau import và hồi quy R | NOT TESTED. Editor 6000.3.25f1 tương thích có sẵn, project chính đang bị tiến trình giữ khóa. |

Mở [HexcoreSkillPreview](../../Assets/Scenes/HexcoreSkillPreview.unity) và chọn **Tự do** để thử WASD, **J**, **Space**, **H**, **R**. Toggle **Bia đầu kháng kéo** giúp thử Bá Vương và Xuyên Tâm. Thử 0/1/5/9 bia và bật vùng tác động nếu cần đối chiếu mép. Runner `VerifyV2` chuẩn bị bảng Skill/Basic, hồi quy R và fixture kéo bia. Các điểm dưới 4 ở trên và các tiêu chí chưa có Game View vẫn chờ polish và nghiệm thu.

## v2.1 · Vòng tinh chỉnh 8 mẫu · Chờ duyệt

Mở [gallery so sánh](HexcoreVisual/V2_1/Compare/index.html) để xem **16 GIF trước–sau**, 8 mẫu ở 1× và 0,5×. Bên trái là bản dựng offline đã bàn giao ở vòng trước. Bên phải dùng pose nguồn hướng Đông và atlas PixelFX phát triển ở vòng này. [Contact sheet mới](HexcoreVisual/V2_1/Compare/ContactSheet.png) · [Sprite sheet dùng trong demo](../../Assets/Prefabs/Player/Melee/Shared/VFX/PixelFX/Resources/HexcoreV21/StyleSheet.png) · [Kiếm sĩ trên nền sáng và tối](HexcoreVisual/V2_1/Compare/GrayEdgeCheck.png)

### Nghiên cứu và tinh chỉnh

Đã đọc 12 effect JS ở commit `5294518ce7c72e48b6100a744596e0acf57f2826`. Vòng nghiên cứu đầu dùng contact sheet nguồn để chọn cửa sổ animation và phân lớp. Vòng phối đầu dùng các nhát, vệt xoay, bụi, đá, ấn và chớp đen từ sheet thật trong atlas. Sau khi xem, đã sửa bán nguyệt sáng bệt thành bốn dải màu có sống sáng mảnh, đổi khung chọn Basic Kiếm sĩ sang mốc tiếp xúc và dời điểm tia va chạm của bản dựng đến các bia trong vùng mô phỏng. GIF bản cuối phân bố thời gian theo đơn vị 10 ms để tốc độ 0,5× đúng gấp đôi tổng thời gian 1×. [Nguồn đã pin](https://github.com/patch-gg/pixelfx/tree/5294518ce7c72e48b6100a744596e0acf57f2826) · [Nguồn lưu trong repo](Source/PixelFX/Upstream)

| Mẫu | Điều lấy từ source và thay đổi nhìn thấy |
|---|---|
| Basic Kiếm sĩ | Sword Slash có mép ellipse sáng, cổ tay tăng tốc và đuôi dither. Phối hai nhịp bạc từ cửa sổ chém nguồn vào pose có sẵn, giữ lớp tia nhỏ để thấy vai và tay. |
| Basic Sát thủ | Dagger Flurry đổi tay nhanh và tách đuôi tím theo nhịp. Thrust kéo dài vệt theo trục đâm. Cắt cửa sổ hiệu ứng ngắn để giữ đúng ba nhát của form, dùng silhouette chéo cho hai nhát đầu và mũi đâm hẹp ở nhát cuối. |
| Basic Đao sĩ | Heavy Cleave tăng tốc qua đầu rồi dồn mảnh và bụi vào sàn. Lấy cửa sổ cung nặng và vùng va sàn riêng cho hai nhát. Nhát trúng mô phỏng phát cue Impact cùng dừng va chạm ngắn đang dùng. |
| Basic Kiếm khí | Sword Slash cho nhát mở mảnh, Energy Disc cho hạt và đường tốc độ ở đuôi nhát thứ hai. Bán nguyệt phóng từ điểm lưỡi kiếm lưu sẵn của từng hướng. |
| P2 Hoành Nguyệt | Spin Slash có vòng con đi trễ và dải tối phía sau. Ghép vòng echo nguồn vào vành ngoài đang mở rộng, giữ hit một lần mỗi bia khi vành đi qua. |
| Huyết Ấn | Hex Sigil vẽ nét theo thời gian, Dark Explosion có sao tối và sóng dẹp sát sàn. Crop ấn bằng cùng một pivot, nối progress pha Mark sang Charge, phối cụm nổ ở từng bia tại Burst. Huyết Ấn giữ ngân sách tối đa 5 dấu và mask cùng lượt. |
| Chấn Địa | Ground Slam cho cột bụi, đá và hai mép sóng. Phối cửa sổ Land cùng lớp nứt và mảnh nóng; lõi sàn dùng chung tâm và bán kính 1,85 u với vùng hit. |
| Nguyệt Nha | Giữ bán nguyệt tự thiết kế, dùng phần đuôi Energy Disc đã cắt bỏ tâm đĩa để tạo wake, giữ sáng mảnh ở đầu. Vùng quét hai đầu thụt theo cùng đường cong của sprite thay cho hình chữ nhật; dây debug vẽ cùng đường cong. |

Dash Strike và Smoke Bomb được đọc để đối chiếu nhịp lấy đà, dừng chân, bụi sát đất và đuôi tan. Hai effect này chỉ lưu JS tham khảo, vì tám mẫu không có chiêu lướt/khói riêng. Hit Spark tiếp tục dùng dấu chớp thực tế trên bia trúng. Chín bộ PNG/JSON chọn lần này và Hit Spark từ vòng trước được giữ làm nguồn. Một atlas 12 clip được import dưới thư mục VFX đang dùng. [Recipe](Source/PixelFX/render_polish_samples.py) · [Danh sách bản quyền và cách phân phối](HEXCORE_ASSET_SOURCE_LICENSES.md)

### Đồng bộ và viền xám

- Bán nguyệt Basic Kiếm khí và Nguyệt Nha dùng sweep giữa hai vị trí mẫu với biên cong và mask trúng từng bia. Tia và cue impact lấy lần va chạm. P2 lấy dải quét giữa hai bán kính liên tiếp. Huyết Ấn, Chấn Địa tính hit theo snapshot tâm nổ/đáp; màu ngoài biên được dùng cho hạt trang trí. Debug vùng sát thương tiếp tục mặc định tắt. Trong các mẫu, lệnh đánh khi đang đổi hướng có thể chờ hết nhịp Turn 0,10 s thay vì cắt trực tiếp sang pose ra đòn.
- **Đã tìm thấy và sửa nguồn viền xám.** Khi xem sheet pose Idle–Run–Basic–Recover của 4 form × 8 hướng, xuất hiện mảng sáng dưới chân trong `Run_Up_02.png`. Kiểm tra theo **component alpha 8-connected**, tách riêng khối nhân vật lớn nhất thay vì coi mọi pixel đặc là thân. Trong 88 PNG locomotion Kiếm sĩ có 17 mảng trung tính tách rời trên 9 frame Run chính hướng. Đã đặt alpha về 0 riêng 11.165 pixel ở các mảng này. Khối thân, vũ khí, kích thước và `.meta` giữ nguyên. [Bằng chứng trước–sau trên nền sáng/tối](HexcoreVisual/V2_1/Compare/GrayEdgeCheck.png)
- Chín PNG đã sửa là `Run_Down_04`, `Run_Left_01/03/04`, `Run_Right_04`, `Run_Up_01/02/03/04`. Bước xem lại sau import Unity, kiểm tra Turn thật ở 8 hướng và phát hiện viền khác do material/hiệu ứng vẫn NOT TESTED.

### Tự đánh giá tám mẫu

Điểm 1–5 bên dưới dành cho hình và bố cục nhìn được trong **bản dựng offline** sau hai vòng phối, không thay cho đánh giá Game View. Cột Lực là dấu hiệu thị giác trong GIF. Cột Quan sát là độ đọc silhouette và bia trong bố cục mô phỏng. NT là NOT TESTED, chưa chấm khi thiếu bằng chứng Unity.

| Mẫu | Hình | Mượt animation | Lực | Bản sắc | Khớp nhân vật | Khớp vùng | Quan sát | WOW |
|---|---:|---|---:|---:|---|---|---:|---:|
| Basic Kiếm sĩ | 4 | NT | 4 | 4 | NT | NT | 5 | 4 |
| Basic Sát thủ | 4 | NT | 4 | 5 | NT | NT | 4 | 4 |
| Basic Đao sĩ | 4 | NT | 5 | 5 | NT | NT | 4 | 4 |
| Basic Kiếm khí | 4 | NT | 4 | 4 | NT | NT | 4 | 4 |
| P2 Hoành Nguyệt | 4 | NT | 4 | 4 | NT | NT | 4 | 4 |
| Huyết Ấn | 5 | NT | 5 | 5 | NT | NT | 4 | 5 |
| Chấn Địa | 5 | NT | 5 | 5 | NT | NT | 4 | 5 |
| Nguyệt Nha | 4 | NT | 4 | 5 | NT | NT | 5 | 4 |

### Kết quả kiểm thử vòng này

| Hạng mục | Kết quả | Bằng chứng |
|---|---|---|
| Render, gallery và tốc độ | PASS | 16 GIF giải mã được. Mỗi cặp có đúng 1.750/3.500 ms. Đã xem hai contact sheet phối và trước–sau alpha |
| Vùng hit scalar | PASS offline | 42 kiểm tra C# từ công thức đang dùng. Có bia trong, ngoài, sát mép, tám phép đổi hướng và nhiều bia đi qua vành P2 |
| Frame nguồn và alpha | PASS offline | Quan sát bảng pose 4×8 và kiểm tra 88 PNG locomotion Kiếm sĩ. Xóa đúng 17 mảng ngoài thân; đối chiếu pixel thân trước–sau |
| Compile | PASS | Roslyn biên dịch runtime và Editor. Kiểm tra 13 WAV có dữ liệu tín hiệu |
| Unity Play Mode, Game View, điểm neo động, SFX thực tế, 96 tổ hợp R | NOT TESTED | Project công bố **6000.3.25f1**, Editor cùng phiên bản có sẵn. `Temp/UnityLockfile` vẫn bị tiến trình khác giữ; chưa mở Editor thứ hai |
| Duyệt mẫu | Chờ chủ dự án | [Gallery so sánh 1× và 0,5×](HexcoreVisual/V2_1/Compare/index.html) |

Khi project rảnh, dùng [scene hiện có](../../Assets/Scenes/HexcoreSkillPreview.unity) và `HexcoreSkillPreviewSetup.VerifyV21Samples`. Bật **Hiện vùng tác động** để đối chiếu bia sát mép, chạy **Tự do → J → Space → chạy** để kiểm tra nối pose, so nền sáng/tối ở các hướng Run, rồi chạy hồi quy ba lõi R. Bằng chứng bên trên giữ riêng với bộ capture lịch sử.

## v2.1 · Giai đoạn 1–2 · Chờ duyệt mẫu

Đã lưu [đặc tả v2.1 đã duyệt](HEXCORE_SKILL_VISUAL_POLISH_V2_1.md). Mốc trước thay đổi là [v2.0](HEXCORE_SKILL_REDESIGN_V2_IMPLEMENTATION.md), 24 ID nguồn và 336 tổ hợp. Gallery bên dưới lưu capture lịch sử, còn bộ hình v2.1 trong mục này là **bản dựng offline hướng Đông từ pose thật và VFX mới**. Các bia trong GIF là bố cục xem nhịp/hình, chưa đo va chạm từ Unity.

[Contact sheet bản cuối](HexcoreVisual/V2_1/ContactSheet.png)

| Mẫu | Hình ảnh và điểm đối chiếu với v2.0 | GIF |
|---|---|---|
| Basic Kiếm sĩ | Hai vệt bạc mỏng và ít hạt theo nhịp hai nhát nguồn | [Xem](HexcoreVisual/V2_1/01-Basic-Kiem-si.gif) |
| Basic Sát thủ | Hai vệt tím chéo kiểu dao và silhouette đâm riêng ở nhát cuối | [Xem](HexcoreVisual/V2_1/02-Basic-Sat-thu.gif) |
| Basic Đao sĩ | Cung cam dày, đòn bổ có silhouette riêng, thêm dừng 33 ms khi đòn Basic trúng bia | [Xem](HexcoreVisual/V2_1/03-Basic-Dao-si.gif) |
| Basic Kiếm khí | Cung xanh ngọc rồi bán nguyệt phóng từ mốc lưỡi kiếm nguồn; hit khi lưỡi sóng đi qua bia, tính Basic hợp lệ một lần | [Xem](HexcoreVisual/V2_1/04-Basic-Kiem-khi.gif) |
| P2 Hoành Nguyệt Trảm · nguồn 02 | Hai lớp vệt xoay. Bỏ damage tức thời toàn đĩa ở mẫu v2.1; quét vành tới khoảng 2,2 u, mỗi bia tối đa một hit/kích hoạt. Giữ ngưỡng 4 Basic hợp lệ | [Xem](HexcoreVisual/V2_1/05-P2-Hoanh-Nguyet.gif) |
| Huyết Ấn · nguồn 05 | Tối đa năm ấn trên renderer từng bia, đồng nổ ở vị trí hiện tại; bia bị ẩn trước pha Burst sẽ rơi khỏi danh sách. Vùng mỗi nổ bán kính 1,12 u, chống trùng overlap cùng lượt | [Xem](HexcoreVisual/V2_1/06-E2-Huyet-An.gif) |
| Chấn Địa Trảm · nguồn 07 | Đất nứt phát sáng, mảnh đá, vệt dư chấn tại mốc Land. Atlas và hit cùng dùng tâm đáp và bán kính 1,85 u | [Xem](HexcoreVisual/V2_1/07-E1-Chan-Dia.gif) |
| Nguyệt Nha Trảm · nguồn 10 | Một mép bán nguyệt có lõi sáng và đuôi tách lớp. Đi 5 u, ngang 2,8 u; đường quét giữa các mẫu đánh mỗi bia một lần, cue impact phát ở hit đầu | [Xem](HexcoreVisual/V2_1/08-E1-Nguyet-Nha.gif) |

Đã xem contact sheet nháp và bản cuối. Tăng khoảng trống dưới vòng P2 và Chấn Địa để giữ toàn bộ silhouette trong khung review. Tách cung dao tím thành đường chéo đứt và dùng silhouette mũi đâm riêng để khác kiếm bạc. Art giữ phối màu gốc của nhân vật. Các mẫu dùng pose nguồn, mốc Basic/pha skill và renderer mới. Chưa chốt độ mượt, điểm neo nhìn thực tế hay khả năng đọc vùng ở 1×/0,5× trong Game View.

### Mapping và asset

- Giai đoạn 1–2 giữ các ID nguồn 01–24 và slot runtime v2.0 để các fixture/hồ sơ cũ còn đọc đúng. Trong thiết kế đích v2.1, nguồn **22** chuyển sang **E3 Kiếm Vũ Thiên Hà**, nguồn **12** giữ pose/ấn Kiếm Giới làm tài nguyên, **F1 Thiên Kiếm Xuyên Tâm** cần định danh lựa chọn riêng ánh xạ pose nguồn 12. Ba thay đổi Kiếm khí này thuộc giai đoạn 7. Các ID còn lại theo [bảng v2.0](HEXCORE_SKILL_REDESIGN_V2_IMPLEMENTATION.md).
- Chọn đúng [Hit Spark](https://pixelfx.io/effects/impacts/hit-spark/) ID `impacts/hit-spark`. Đã lưu [PNG/JSON upstream](Source/PixelFX/HitSpark) cùng [MIT](Source/PixelFX/LICENSE.txt) và [notices](Source/PixelFX/NOTICE.txt). JSON export bản 1, 16 frame, pivot 44×64 từ góc trên trái; lấy duration từng frame khi đóng atlas. Hạ cỡ raster nearest-neighbor 60%, căn pivot về ô chuẩn, phát phiên hit trong khoảng 0,25 s. Ground Slam của PixelFX chỉ dùng làm tham khảo cho cấu trúc nứt, đá và dư chấn. [Nguồn tham khảo Ground Slam](https://pixelfx.io/effects/impacts/ground-slam/)
- [Recipe tái lập](Source/PixelFX/render_polish_samples.py) dựng 11 clip hình riêng cộng clip Hit Spark đã căn, đóng **một atlas RGBA 1120×1680, 148 frame, 12 clip** và JSON timing. Chạy với Python và Pillow từ bất kỳ cwd, các đường dẫn tính từ file recipe. Bản gốc/sheet review nằm ngoài Assets; runtime chỉ tải atlas + timing dưới `Assets/Prefabs/Player/Melee/Shared/VFX/PixelFX/Resources/HexcoreV21`.
- Renderer dùng pool trong hai file runtime hiện có. Sử dụng lại sprite nhân vật, pose, cue, material và công cụ camera. Vùng debug mặc định tắt, bật lại bằng **Hiện vùng tác động**. Ba lõi R tiếp tục dùng asset nguồn. SHA-256 scene và thiết lập project đối chiếu khớp baseline trước chỉnh sửa.

### Kiểm chứng và điểm cần duyệt

| Kiểm tra | Trạng thái | Bằng chứng |
|---|---|---|
| Nguồn chọn lọc và định dạng | PASS | Đã tải 1 PNG, 1 JSON, MIT và notices từ PixelFX; PNG nguồn và metadata khớp kích thước |
| Render offline | PASS | Đã render/xem hai mốc contact sheet và giải mã đủ 8 GIF bản cuối; không phải capture camera Unity |
| Roslyn và kiểm tra dữ liệu atlas | PASS | Runtime 40 file và Editor 19 file biên dịch exit 0; tất cả ô atlas hợp lệ, timing dương |
| Play Mode và va chạm trong/ngoài/sát mép | NOT TESTED | Runner `HexcoreSkillPreviewSetup.VerifyV21Samples` đã nối với scene chính, 64 ca mẫu, chưa chạy |
| Game View 1×/0,5×, độ mượt, SFX, hồi quy 96 tổ hợp R | NOT TESTED | Chưa tái hiện hoặc nghe trực tiếp trong Unity |
| Viền xám Kiếm sĩ | NOT TESTED | Chưa tái hiện nền sáng/tối trong Game View; giữ ảnh nguồn và đường chẩn đoán v2.0 |
| Capture Unity/GIF gameplay v2.1 | NOT TESTED | Khi project chính chạy được, dùng `-executeMethod HexcoreSkillPreviewSetup.VerifyV21Samples -hexcoreV21Capture`; ảnh hướng Đông và CSV ghi trong `Library/HexcoreV21` |

Project khai báo Unity **6000.3.22f1**. Máy có **6000.3.25f1** và **6000.6.0f1**; đọc kiểm tra thấy `Temp/UnityLockfile` đang được sử dụng. Chờ duyệt mẫu về bề dày của bốn loại vệt Basic, màu đỏ tím Huyết Ấn, độ mạnh Chấn Địa và độ lớn của vòng P2/bán nguyệt. Triển khai hàng loạt thuộc giai đoạn sau cổng duyệt.

## Xem trong Unity và trình duyệt

Mở [Visual Review Gallery](HexcoreVisual/index.html) bằng trình duyệt. Chọn tạo hình, kỹ năng và hướng. Các chế độ gồm so sánh hai mốc, bản hiện tại, tắt VFX và chuỗi Basic → Skill → Basic → Run. Dùng Phát lại, hai mũi tên hoặc Duyệt tự động để đi theo danh sách. Bản trước dùng đúng đoạn ghi hình của mốc trước. Nếu mốc trước chỉ có 1×, nhãn của từng khung sẽ ghi tốc độ tương ứng.

Trong Unity 6000.3.25f1, mở [HexcoreSkillPreview](../../Assets/Scenes/HexcoreSkillPreview.unity) và vào Play. Chọn form, kỹ năng, hướng ngắm và hướng di chuyển. Chọn `Basic → Skill → Basic` để thấy cả bước vào và thoát chiêu. Dùng `0.5x`, `VFX`, `SFX`, `Hit-stop` và các nút bước khung để đối chiếu pose, điểm chạm và âm.

## Các lỗi và phần đã sửa

| Quan sát | Sửa trong preview |
|---|---|
| Thời lượng các beat bằng nhau làm các chiêu cùng nhịp. Giữ frame quá lâu ở cú chém nhẹ | Dùng thời lượng riêng theo hành động cho 23 kỹ năng. Q1 giữ timing riêng theo frame. Dãn đoạn lấy lực và thu thế, rút nhịp va chạm ở các đòn nhẹ |
| Hit-stop giống nhau ở mọi nhịp chạm, làm đòn nhanh có cảm giác ngắt liên tục | Dùng pause ngắn theo loại va chạm. Các nhát giữa của đòn liên hoàn bỏ pause, nhát đáp và finisher giữ nhịp nhấn |
| Ám Bộ dừng ở phía trước mục tiêu. Dạ Vũ lặp hướng và đường đi | Ám Bộ vượt mục tiêu rồi dùng pose theo hướng ngược lại khi phản thân. Dạ Vũ chọn ba điểm lướt cắt vùng mục tiêu và lấy pose tương ứng với hướng mỗi đoạn |
| Thiên Vẫn bật và đáp gần cùng điểm, hiệu ứng chạm dịch theo frame thu đao | Dịch trên sàn theo hướng thi triển bên dưới cung độ cao, thêm marker đáp. Khóa tâm nứt, vòng xung kích và bụi vào frame chạm đầu tiên |
| Tam Thiên dùng ba lượt phóng cùng hình. Một số pose chéo quay sai hướng hoặc mất lưỡi | Thay pose chéo, tách nhịp chuyển thế và lần giữ lực cuối. Ba nét phát có hình, khẩu độ và vận tốc riêng. Đo điểm đầu tại lưỡi của từng lần release ở bốn hướng chéo |
| Vạn Ảnh tái xuất quá xa mục tiêu. Hai bóng cùng trượt ngang với nhịp giống nhau | Nối đường biến mất, dựng lại người thật và nhát kết. Đổi góc tiếp cận giữa hai lượt bóng, dùng bộ chém theo form và hướng, kéo vệt theo lượt chém |
| Cuồng Phong Đao dễ lẫn với Hoành Nguyệt | 16 pose thật cho hai vòng. Vòng đầu tấn rộng, vòng sau bước chéo. Cả hai dùng vệt xoắn thấp và hãm từng nhịp |
| Lưỡng Nghi thiếu dấu giao hai đường kiếm | Hai nét chéo gặp nhau ở điểm bùng. Nguyệt Nha giữ một sóng, Tam Thiên phóng ba hình riêng |

## Visual identity của 24 kỹ năng

| Kỹ năng | Silhouette và nhịp | Quỹ đạo | Hình VFX và dấu va chạm | Điểm phân biệt |
|---|---|---|---|---|
| Q1 Kiếm Lướt | Ép thấp rồi duỗi cả thân, một nhát | Lao thẳng, dừng dài | Vệt nối lưỡi và bụi đạp | Một lần lao, một nhát |
| Q2 Hoành Nguyệt Trảm | Lấy thế xoay, một vòng đủ rồi hãm | Vòng quanh chân trụ | Một vòng ngang | Một chu kỳ, vòng phẳng |
| Q3 Phản Kiếm Quyết | Khép thế thủ, gạt rồi nảy về phản kiếm | Trụ tại chỗ, đẩy lưỡi ra trước | Tia gạt nhỏ rồi tia phản | Khoảng thủ rõ, có tình huống thu kiếm khi trượt |
| E1 Ám Bộ Trảm | Cuộn, tan, quay người và chém đôi | Vượt mục tiêu rồi đánh ngược | Bụi tắt/mở và hai nhịp chạm | Đổi phía trước hai nhát |
| E2 Huyết Ấn | Vươn đặt ấn, ngắt nhịp rồi nổ | Điểm neo trên dummy | Ấn thoi nén rồi bung | Trì hoãn nổ trên một mục tiêu |
| E3 Tàng Ảnh | Hạ người, lui vào bóng, tái xuất và cắt | Hai đoạn dịch khi mờ | Tàn ảnh khói và một nhát | Phục kích một nhịp |
| E1 Chấn Địa Trảm | Nâng đao, bổ nặng, khuỵu | Đứng, chém dọc mặt sàn | Nứt thành đường, chạm trầm | Bổ từ đất, một điểm |
| E2 Hoành Thiên Đao | Mở vai rộng, chém ngang, giữ đà | Cung ngang kéo từ vai | Cung đao nặng | Quét một phía từ thân xoay |
| E3 Bất Động Chiến Thế | Vào trụ, thủ rồi thử hai đòn | Trụ tại tâm | Aura quanh thân và nhịp thử đao | Vào thế phòng thủ trước khi đánh |
| E1 Nguyệt Nha Trảm | Chuyển thế một lần, thu nhanh | Kiếm khí đi thẳng | Một bán nguyệt | Một lần phóng |
| E2 Lưỡng Nghi Kiếm | Hai chém chéo đối hướng | Giao nhau tại mục tiêu | Tia chữ X ở điểm gặp | Nhát thứ hai hoàn tất hình cắt |
| E3 Kiếm Giới | Hướng kiếm xuống, giữ phép rồi thu | Trận cố định trên sàn | Pháp trận và hai đợt điểm nổ | Phạm vi cố định, nhiều nhịp |
| R1 Thiên Vẫn Trảm | Ép thấp, bật, treo đỉnh, cắm vũ khí, khuỵu | Cung cao kết hợp dịch trên sàn | Marker, nứt, vòng xung kích, vật vụn | Cú đáp diện rộng có trọng lượng |
| R2 Tam Thiên Kiếm Quyết | Tụ lực, giật nhanh, đổi thế, nhát hai, giữ, kết | Ba đường phóng từ mũi kiếm | Vệt gấp, cung rộng, mũi xung hai tầng | Ba nhát tăng khẩu độ, tốc độ và lực |
| R3 Vạn Ảnh Tuyệt Sát | Chuẩn bị, tan, bóng cắt chéo, hiện và kết | Các làn cắt gặp nhau | Vệt giao, ánh kết phía sau | Nhân vật thật biến mất giữa lượt bóng |
| F1 Dạ Vũ Liên Trảm | Ba nhịp chân ngắn, mỗi nhịp một slash | Ba đoạn lướt tam giác | Bụi từng cú và va chạm cuối | Ba đường đi khác nhau |
| F2 Đoạt Mệnh | Nén người, lướt rồi xuyên một nhát | Đường xuyên mục tiêu | Tia tím tập trung | Một cú kết gọn, không liên hoàn |
| F3 Song Ảnh Truy Sát | Nhân vật thật giữ nhịp và hai bóng đánh nối | Hai làn song song | Bóng theo hai bên | Người thật vẫn ở trong cảnh |
| F1 Cuồng Phong Đao | Hai vòng giữ trụ, dập từng nhịp | Hai sweep quanh thân | Xoắn nâng theo chiều cao | Chuỗi quay, vòng có độ nâng |
| F2 Đoạn Sơn Trảm | Hai nhịp tích lực rồi dồn vào bổ | Bổ theo trục hẹp | Nứt và nhấn phá ở cuối | Tụ hai lần và va chạm kép |
| F3 Bất Diệt Chiến Ý | Vào aura, đòn nối và duy trì nhịp lâu | Trụ và xoay tại chỗ | Aura đi cùng các tia chém | Chủ động đánh trong aura lâu hơn E3 |
| F1 Kiếm Vũ Thiên Hà | Gọi kiếm trên cao, nhả theo cụm | Nhiều đoạn rơi lệch tâm | Mũi kiếm rơi lần lượt | Phân bố từ trên xuống |
| F2 Phi Kiếm Truy Hồn | Vươn tay đẩy kiếm và giữ hướng | Đường cong bám mục tiêu | Kiếm khí đổi tiếp tuyến giữa đường | Một đường cong có điểm đến |
| F3 Hồi Nguyệt Kiếm | Phóng ra, giữ tay đón, thu kiếm | Ra rồi trở lại | Vệt quay đầu và tia bắt | Nhân vật đón tại cuối chiêu |

## Chấm theo clip đại diện

[VisualScores.csv](HexcoreVisual/VisualScores.csv) ghi tám tiêu chí, điểm 1–5 và dẫn chứng cho mỗi kỹ năng. Điểm là nhận xét sơ bộ theo clip đại diện. Cần xem đúng tổ hợp trong gallery trước khi dùng điểm đó để đề nghị nâng trạng thái. Gate Ready for Review cần cả tám điểm từ 4 trở lên, hình thể ổn định, điểm neo đúng và bằng chứng chuyển tiếp cùng hướng.

## Kết quả và phần còn thiếu

| Trạng thái | Trước | Sau |
|---|---:|---:|
| MissingArt | 0 | 0 |
| SourcePoseBlockout | 0 | 0 |
| AuthoredBlockout | 263 | 263 |
| Ready for Review | 0 | 0 |
| In Review | 1 | 1 |
| Approved | 0 | 0 |

[CSV trạng thái](HexcoreVisual/HexcoreVisualReview.csv) có đủ 264 hàng. Lượt này xem ảnh Unity của từng tổ hợp, đối chiếu chuỗi ảnh theo pha và chấm một clip đại diện cho mỗi kỹ năng. Các tổ hợp còn lại có bằng chứng camera riêng trong gallery và cần duyệt chuyển động từng pha trước khi nâng trạng thái.

- Sản xuất 320 frame thay thế từ 10 atlas hướng chéo của ba lõi. Thêm 16 pose cho hai vòng Cuồng Phong Đao. Các sprite sửa dùng lại GUID của frame demo. Prefab và clip nguồn vẫn phục vụ locomotion. SFX dùng cue WAV nội bộ.
- Thêm 64 điểm đo cho R1 và R2. Tổng 125 frame có dữ liệu đo, thuộc 55 tổ hợp. Điểm neo còn lại cần đo riêng, đặc biệt là trail của từng bóng R3 và các hướng chéo của Q/E/F.
- Lượt tới cần thêm khung dồn gối/thu đao khi R1 đáp, bước đổi chân giữa nhát 1–2 của R2, cầu bắt đà lúc R3 tái xuất, dáng trụ ở Bất Động/Bất Diệt và các cầu từ Basic vào tư thế đặc biệt. Cần duyệt trực quan nhịp stop và chất lượng hòa âm trên thiết bị.
- Kết quả kỹ thuật gồm 264/264 lượt lấy mẫu VFX bật/tắt, 264/264 chuỗi Basic vào và ra với hướng chạy khác hướng đánh, 264/264 Replay/reset. Kiểm tra theo đồng hồ ở hai tốc độ cho Q1, Q3 và các lõi mẫu.

## Video ba lõi

| Kỹ năng | Bản 1× | Bản 0,5× |
|---|---|---|
| Thiên Vẫn Trảm · Đao sĩ E | [MP4](HexcoreVisual/Videos/R1-Thien-Van-Tram-1x.mp4) | [MP4](HexcoreVisual/Videos/R1-Thien-Van-Tram-05x.mp4) |
| Tam Thiên Kiếm Quyết · Kiếm khí NE | [MP4](HexcoreVisual/Videos/R2-Tam-Thien-Kiem-Quyet-1x.mp4) | [MP4](HexcoreVisual/Videos/R2-Tam-Thien-Kiem-Quyet-05x.mp4) |
| Vạn Ảnh Tuyệt Sát · Sát thủ SE | [MP4](HexcoreVisual/Videos/R3-Van-Anh-Tuyet-Sat-1x.mp4) | [MP4](HexcoreVisual/Videos/R3-Van-Anh-Tuyet-Sat-05x.mp4) |

[Bảng khung hình ba lõi](HexcoreVisual/HeroMotionContact.png) và [mức PCM trong video](HexcoreVisual/HexcoreMovieAudio.csv).

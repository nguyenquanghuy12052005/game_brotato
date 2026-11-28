# Audio Preview Studio trong Unity

Mở [AudioPreviewScene](../Assets/Scenes/AudioPreviewScene.unity) trong Unity **6000.3.22f1**, chọn tab **Game** và nhấn Play. Nên dùng Game View 16:9 để thấy cả sân minh họa và bảng thông tin. Giữ thư mục `demos/brotato-audio` trong checkout để Studio lấy được các WAV và bản đối chiếu.

## Nghe và duyệt

1. Chọn một trong 10 nhóm ở trái hoặc tìm tên có dấu hay không dấu. Bấm mục trong danh sách để nghe và xem. Mặc định khi mở scene chọn Thiên Vẫn Trảm và chờ bạn bấm phát.
2. Dùng **Phát**, **Dừng**, **Phát lại** và hai nút **Biến thể**. Phím Space phát/dừng, ←/→ đổi biến thể khi con trỏ rời ô tìm kiếm. Tốc độ **1× / 0,5×** và **Pitch test** tác động lên đồng hồ audio. Sân lấy thời gian từ cùng đồng hồ.
3. Dùng **Bản phối**, **Hụt**, **Cue cũ** hoặc bấm lớp riêng trong bảng phải khi mục có dữ liệu tương ứng. Bản hụt giảm hiệu ứng hit mục tiêu, vẫn giữ VFX chạm đất đối với chiêu luôn có cú giáng. Cue cũ phát độc lập để tiện so âm lượng và âm sắc.
4. Tắt VFX, Animation hoặc Dummy để soi từng thành phần. Với Cận chiến, đổi Dummy khi đang phát sẽ chạy lại lượt để tạo đội hình. **Tự lặp** phản hồi sau khi cue một lượt phát xong. Nhạc và ambience có cờ loop trong catalog. Các nhóm Master, Nhạc và SFX được áp bằng AudioSource riêng của Studio.
5. Chọn **Duyệt** hoặc **Cần chỉnh**. Kết quả lưu theo ID và biến thể trong PlayerPrefs của Studio trên máy hiện tại. Nhạc Concept B có ba lựa chọn ở nhóm Nhạc nền, mục 138.

### Vòng phối Foley

- Bấm **Tất cả nhánh** để lọc nhánh trong hệ đang mở. Dùng **Bản phối trước** để đối chiếu 20 cue đã phối lại. Kết quả **Đạt / Cần chỉnh / Chờ duyệt** và **Lưu nhận xét** tách theo phiên bản phối và biến thể.
- Bảy tuyệt kỹ Súng và Pháp sư có take hình ghi từ scene nguồn. Studio mở take đúng phiên bản khi bật Animation, VFX và Dummy. Take không tiếng và chạy theo đồng hồ audio. Khi tắt một trong ba lớp hình, sân trở về ảnh/sprite để tránh giả định có thể tách layer trong video. **Hụt** hoặc bản Thiên Cầu 2 đạn chỉ có WAV tương ứng khi không có take nguồn đúng biến thể. Với Cận chiến, ba toggle vẫn tách trực tiếp trên rig.
- Năm cue duy trì có lớp **Loop rời** để tự lặp độc lập. Chọn bản phối chung hoặc lớp thi triển/kết thúc để nghe đầu và đuôi. [Báo cáo vòng phối](HEXCORE_AUDIO_QUALITY_REPORT.md) · [Mốc 146 mục](HEXCORE_AUDIO_SPOTTING.md)

## Asset và độ khớp hình ảnh

| Mục | Nguồn minh họa | Cách đọc timing |
| --- | --- | --- |
| 49–76 Cận chiến | Form Kiếm sĩ, Sát thủ, Đao sĩ, Kiếm khí, pose và VFX từ rig của `HexcoreSkillPreview`. Mục 53 dùng pose Kiếm Lướt gốc để so cue Phong Ảnh Trảm. 54/55 thử đúng hiệu ứng proc, với fixture và pose Basic | Studio lấy mẫu timeline theo đồng hồ WAV. Ám Bộ, Chấn Địa, Nguyệt Nha và ba lõi có neo cụ thể. Các mục khác dùng timeline gốc tỷ lệ theo độ dài bản phối |
| 20–48 Súng và 115/144 | Sprite vũ khí/đạn và atlas chia frame của sân GunSkillTest | Dựng chuyển động, bắn, bay và va theo layer/mốc WAV. Đây là bản minh họa, bộ điều khiển bắn của lab không được chạy |
| 77–107 Pháp sư | Sprite form/staff từ Mage prefab và atlas phép từ MageEffectLibrary | Dựng đường bay theo mốc WAV. Một số biến thể dùng VFX gần nhất trong cùng nguyên tố, theo ghi chú trên scene |
| 1, 7, 142 và các hành động nhân vật | Clip Run, Death, Idle và sprite nhân vật từ CharacterArt | Đi, tử trận và đổi trạng thái có clip thật. Nhảy và một số phản ứng dùng dịch chuyển minh họa hoặc Idle, vì vòng chơi chính chưa có clip/hành động tương ứng |
| Quái thường, cung thủ, tinh nhuệ, Boss | Sprite thật từ dự án. Các mục 117–125 có hình VFX liên quan trong `SkillAssets` | Động tác và va là dựng thử theo mốc cue. AI Boss bản đồ chưa có timeline gameplay để so |
| UI, môi trường, nhạc | Nút mẫu, tranh Concept B và ảnh bốn biome hiện có | UI phát khi chọn hoặc thử nút. Ambience và nhạc chạy loop theo bản đã chọn |

## Cấu hình và giới hạn

- [Kế hoạch âm thanh](HEXCORE_AUDIO_SYSTEM_PLAN.md) và [catalog web](../demos/brotato-audio/catalog.js) là nguồn dữ liệu. Unity lấy bản sao catalog tại `Assets/Resources/AudioPreview`. Menu **Tools → Brotato → Audio Preview → Xây lại scene và catalog** tạo lại scene và nhập lại danh mục từ web. Scene tái dùng tham chiếu trong các asset gốc.
- Studio đọc WAV theo yêu cầu từ checkout bằng UnityWebRequest và giữ clip đang dùng. Bản danh mục 146 mục dùng chung 311 bản phối, các stem, mix hụt và đường dẫn cue cũ. Hai AudioSource là nhóm âm lượng tách riêng trong preview. AudioMixer chính thức và tích hợp gameplay thuộc bước sau khi chọn mẫu.
- Scene là công cụ review trong Unity Editor. Bản build trò chơi không kèm thư viện WAV ngoài Assets qua cấu hình này. Các clip nghe thử chưa được đánh dấu phê duyệt theo mặc định. Khi nghe lớp riêng, stem giữ khoảng lặng trước mốc để so với timeline hình ảnh.
- Nguồn âm thanh và phạm vi cue được ghi trong [kiểm kê](../demos/brotato-audio/INVENTORY.md) và [hồ sơ giấy phép](../demos/brotato-audio/LICENSES.md).

## Kiểm tra

- Đã đối chiếu 146 mục, 10 nhóm và 1.557 đường dẫn WAV với thư viện. Các scene nguồn chính trùng byte với bản sao trước khi dựng. Build Settings không gọi Studio.
- Unity 6000.3.22f1 Direct3D 11 đã chạy Play Mode trên bản sao. Đã chọn và lấy mẫu sân minh họa cho cả 146 mục. Test phát WAV đại diện xác nhận điểm đáp Thiên Vẫn, đợt kiếm Tam Thiên ở 0,5×, đổi biến thể, phát hụt, tắt VFX/Animation/Dummy trên Vạn Ảnh, cùng luồng Súng, Mage, Forest, đổi nhạc và UI. Đã xem ảnh render sân Thiên Vẫn tại mốc đáp.
- Cần xác minh thủ công các mốc hình/tiếng chưa có neo riêng, cân âm bằng tai và timing sau khi đưa cue vào gameplay.

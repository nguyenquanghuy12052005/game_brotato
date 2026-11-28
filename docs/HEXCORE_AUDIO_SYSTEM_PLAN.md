# BROTATO 2D · Kế hoạch thiết kế âm thanh HEXCORE

Bản đề xuất duyệt ngày 28/11/2026. Phạm vi gồm bản chơi chính, MainMenu, bốn sân thử và tài nguyên Boss đã có. Các thông số sản xuất, mix và tích hợp bên dưới là mục tiêu thử nghiệm.

## Bản sản xuất nghe thử

Phạm vi và cấu trúc đã được duyệt để sản xuất mẫu. Toàn bộ 146 mục hiện có cue nghe thử trong [Audio Preview](../demos/brotato-audio/index.html). Demo có bản phối, lớp riêng, biến thể, cue trúng/hụt khi áp dụng và tham chiếu WAV cũ. Nhạc Concept B có ba bản độc lập ở 142, 126 và 158 BPM. [Hướng dẫn và giới hạn](../demos/brotato-audio/README.md) · [Kiểm kê bản xuất](../demos/brotato-audio/INVENTORY.md) · [Hồ sơ giấy phép](../demos/brotato-audio/LICENSES.md)

Các mốc, độ dài và số biến thể trong bảng bên dưới là đích sản xuất. Bảng kiểm kê của demo ghi sản phẩm thực tế và các quyết định còn chờ duyệt. Bản WAV 16 giây ở mục 4 được giữ làm phác thảo trước đợt sản xuất.

Có thể duyệt trong Unity bằng Audio Preview Studio. Scene lấy cùng catalog và WAV, với rig Cận chiến thật và minh họa asset gốc cho các nhóm còn lại. [Cách mở, nguồn asset và phạm vi timing](HEXCORE_UNITY_AUDIO_PREVIEW.md)

Vòng Foley đã chạy lại sáu scene nguồn và làm lại 20 cue ở chế độ chờ duyệt, trong đó có đủ 10 tuyệt kỹ. Studio cung cấp bản phối trước, stem, trạng thái trúng/hụt và take hình thật cho bảy tuyệt kỹ Súng/Pháp sư. [Bảng đối chiếu 146 mục](HEXCORE_AUDIO_SPOTTING.md) · [Báo cáo kiểm thử và giới hạn nghe](HEXCORE_AUDIO_QUALITY_REPORT.md)

## 1. Định hướng âm thanh

Chất âm là **cartoon action hiện đại**. Mỗi hành động có đầu âm gọn, thân vừa đủ lực và đuôi nhường chỗ cho lần bắn hoặc chém kế tiếp. Menu Concept B dùng bộ gõ nhanh và phản hồi vui tai. Khi vào trận, người chơi có thể phân biệt bằng tai ba việc quan trọng nhất: mình vừa gây sát thương, mình sắp bị đánh và một kỹ năng lớn vừa đổi nhịp trận.

- **Súng** dùng khóa cơ khí, áp lực khí và đuôi ngắn. Shotgun là một khối, SMG là nhịp, Rocket có hành trình và điểm nổ riêng.
- **Sát thủ** dùng khoảng lặng, cắt khô và đổi hướng. **Đao sĩ** dựa vào lực đẩy không khí, trọng lượng và vật liệu đất. **Kiếm khí** dùng cao độ và chuyển động của vệt sáng.
- **Pháp sư** chia âm sắc Hỏa/khí nóng, Băng/tinh thể, Quang/chuông và Ám/lọc rỗng. Dấu trạng thái có đầu âm riêng tại lúc thực sự áp.
- **HEXCORE** dùng motif ba nốt chung khi thức tỉnh, rồi chất liệu độc lập cho từng lõi. Ba lõi Cận chiến nhấn ba cấu trúc khác nhau: một lần giáng, ba đợt kiếm khí, hai cụm phân thân và một nhát kết.
- **UI** ưu tiên gõ, búng và lướt có thể nghe ở âm lượng thấp. **Môi trường** làm nền cho từng vùng và tách khỏi cảnh báo gameplay. Nhạc tham khảo về nhịp và độ rõ từ dòng game đã nêu trong brief. Motif, melody, Foley và thiết kế âm đều được làm riêng.

## 2. Phạm vi gameplay và bằng chứng

| Điểm vào hoặc tài nguyên | Kiểm kê hiện trạng | Ý nghĩa với thiết kế |
| --- | --- | --- |
| [GameScene](../Assets/Scenes/GameScene.unity), [Build Settings](../ProjectSettings/EditorBuildSettings.asset) | Scene bật trong build. WASD, ngắm chuột và bắn tự động. Forest → Moon → Inferno → Tundra, một wave thử mỗi vùng, cùng cấu hình 30 s/cap 10 theo hiện trạng. Chuyển vùng có intermission 2 s chạy thời gian thực. HP về 0 dẫn đến Game Over | P0 đặt ở sự kiện thật. Kế hoạch quái/wave trong thiết kế còn là roadmap. Hồi HP lúc reset map dùng cue chuyển map, không thêm cue hồi máu rời |
| [Gameplay](../Assets/Scenes/Gameplay.unity) | Scene cũ, độc lập với cấu hình đang bật | Chỉ đưa vào smoke test nếu team còn mở scene này |
| [MainMenu](../Assets/Scenes/MainMenu.unity), [MainMenuController](../Assets/Scripts/UI/MainMenuController.cs), [MainMenuGuide](../Assets/Scripts/UI/MainMenuGuide.cs) | Menu native Concept B có sáu mục, Chơi tiếp vô hiệu, Bắt đầu tải GameScene. Guide có nút Pause/Resume trình diễn. AudioSource chung phát Music loop và Confirm one-shot, cùng volume/mute. Scene chưa được thêm vào danh sách startup | Thiết kế mix UI/Music độc lập. Dữ liệu chiến đấu, mở khóa và điểm lưu ở Guide không tạo tín hiệu gameplay |
| [Demo web](../demos/brotato-main-menu/README.md) | Prototype Concept B có nhạc/cue minh họa riêng | Có thể A/B với hướng đi menu. Cue web tách khỏi inventory AudioClip Unity |
| [GunSkillTest](../Assets/Scenes/GunSkillTest.unity), [cơ chế Súng](../Assets/Scripts/Weapon/GunSkills) | Ba nhánh Shotgun/SMG/Rocket, ba bị động, ba bộ nâng bậc, hai lớp lõi và bốn ultimate. Boss sân thử có loạt quạt và dash | Bắn, sinh projectile và va là các trigger khác nhau. Tài liệu test đời cũ có số khác runtime, ví dụ Pháo đài hiện pulse 1 s và sát thương 12 |
| [HexcoreSkillPreview](../Assets/Scenes/HexcoreSkillPreview.unity), [controller và clip](../Assets/Prefabs/Player/Melee) | 24 kỹ năng gồm 3 bị động, 18 nhánh và 3 ultimate. Basic bốn form. Nguồn clip gồm timeline sprite, frame và effect tự điều khiển, kèm cue WAV prototype | Dùng slot hiển thị Rev2. Bất Động Chiến Thế/Kiếm Giới là tên slot cũ, dùng runtime Bá Vương Đao Vực/Thiên Kiếm Xuyên Tâm để chấm cue |
| [Magical](../Assets/Scenes/Magical.unity), [Mage combat](../Assets/Scripts/Weapon/MageCombat.cs), [Mage progression](../Assets/Scripts/Magical/Player/MageLevelProgression.cs) | Đạn phép cơ bản, 3 kỹ năng khởi đầu, 4 nguyên tố, 12 nâng bậc, 16 lõi và 3 ultimate. Nhận EXP theo quái thật chết, bia tập không cho EXP. Hiệu ứng sprite chạy qua thư viện và trình phát riêng | Gắn cue theo lần áp trạng thái, tick và sự kiện sinh/va. Q/E của Magical có ánh xạ riêng |
| [CharacterProgressionTest](../Assets/Scenes/CharacterProgressionTest.unity), [controller tiến trình](../Assets/Scripts/Core/CharacterProgressionTest.cs) | Sân chung có 10 nhánh, nút lên cấp thử, Q/E/R. Mage chọn kỹ năng khởi đầu khác nhau cho Q/E và ultimate ở R. Dẫn sáng có trong Magical và bị ẩn khỏi lựa chọn của sân chung | Dùng asset cùng tên và cùng trigger bất kể ánh xạ phím. Cue chọn hoặc proc chỉ bật cho lựa chọn còn hiệu lực |
| [Prefab quái](../Assets/Prefabs/Enemy), [kế hoạch bốn map](KE_HOACH_BOT_4_MAP.md) | Lính thường bốn màu có prefab. Cung thủ Tuyết có prefab và logic cung. Art tinh nhuệ/cung khác và ba Boss bản đồ có thể dùng đối chiếu. Prefab Boss có marker phục vụ thử trạng thái, không có bộ AI chiêu tương ứng trong GameScene | Boss sân Súng là bộ riêng. Cue cho Xương khô/Golem/Yeti theo asset và thiết kế đang có, chờ hook gameplay xác nhận |
| Animation và VFX | Kiểm kê theo phần mở rộng trong Assets có 471 tệp .anim và 21 .controller. Logic chạy còn có sprite-sheet thủ công cho lính, Mage và các clip/beat Cận chiến, Gun VFX | Gắn Foley theo chân hoặc phase animation, cast/va/trạng thái theo runtime. Không giả định có sẵn Animation Event âm thanh trong toàn dự án |

Nguồn đối chiếu thêm gồm [hiện trạng](STATE.md), [kiến trúc](ARCHITECTURE.md), [quyết định](DECISIONS.md), [thiết kế gameplay](GAME_DESIGN.md), [Main Menu](HEXCORE_MENU_UI_PLAN.md), [sân Súng](HUONG_DAN_TEST_SUNG.md) và [Cận chiến Rev2](Melee/HEXCORE_MELEE_CURRENT_STATE.md). Kiểm tra tĩnh theo workspace và Unity **6000.3.22f1** trong ProjectVersion. Số lượng tài nguyên là số tệp import, không suy ra số động tác đã được tích hợp.

## 3. Danh mục cue đề xuất

**Trạng thái** trong cột Hệ/Nhánh: **G** có đường thực thi chính hoặc MainMenu, **T** có ở sân thử, **A** có asset/hình minh họa, **P** là đề xuất chờ cơ chế. **P0** làm trước cho vòng chơi và khả năng đọc trận, **P1** nhận diện hệ và khả năng mở rộng, **P2** chờ tích hợp cơ chế hoặc lớp hoàn thiện.

**Nguồn asset**: **R** tái sử dụng cue được chỉ tên hoặc cùng họ đã làm, **M** cần làm riêng bằng thu âm/tổng hợp/AI có quyền, **L** thiết kế hoặc phối nhiều lớp, **F** dùng Impact Sounds hoặc RPG Audio làm Foley thô, **UI** dùng Interface Sounds làm vật liệu, **D** dùng Digital Audio làm vật liệu. Thời lượng tính theo giây một cue/lớp, trừ khi ghi loop hoặc chuỗi. Số biến thể là mục tiêu xuất xưởng, gồm biến thể có thể tách stem và tái phối. Hàng ghép các bậc chỉ số kê rõ từng lớp và trigger, dùng chung đường phát để giữ ngân sách voice.

### Nhân vật, trận đấu và tiến trình

| STT | Hệ/Nhánh | Hành động/Kỹ năng/Sự kiện | Đặc điểm âm thanh đề xuất | Thời điểm phát | Thời lượng | Số biến thể | Nguồn asset | Mức ưu tiên |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| 1 | Nhân vật [G] | Bước chân đi và chạy | Đế giày kèm lớp bề mặt từng map, tăng nhịp theo quãng đường | Chạm chân khi đang dịch chuyển thực tế | 0,08–0,16 | 6 mỗi bề mặt | R Foot + F + L | P0 |
| 2 | Nhân vật [T] | Lướt và dừng lướt | Hơi kéo hướng và một tiếng ghì đế ngắn | Bắt đầu đổi vị trí và khi khóa điểm dừng từng chiêu | 0,12–0,26 | 4 | R Dash + L | P1 |
| 3 | Nhân vật [T/P] | Bật nhảy và tiếp đất | Búng đế khi cất và chạm nặng theo khối lượng | Chỉ ở skill có bật/đáp như Thiên Vẫn Trảm, cue nhảy tự do chờ cơ chế | 0,10 + 0,26 | 3 mỗi lớp | F + L | P1 |
| 4 | Nhân vật [G] | Bị thương | Tiếng chạm mềm có lực, hơi đau phi ngôn ngữ nhẹ | Máu thực giảm, gộp hit gần nhau 100 ms | 0,18–0,30 | 5 | F + M | P0 |
| 5 | Nhân vật [G/P] | Máu thấp | Hai nhịp trầm ngắn, nhường chỗ cảnh báo | Đề xuất một lần khi qua ngưỡng HUD đã duyệt, tắt khi hồi | 0,38–0,60 | 2 | M | P1 |
| 6 | Nhân vật [T] | Hồi máu thực | Hơi ấm đi lên và một hạt sáng | Máu tăng thành công, gộp hồi theo nhịp | 0,24–0,46 | 4 | M | P1 |
| 7 | Nhân vật [G] | Tử trận | Tiếng rơi trầm, cắt nhịp chiến đấu vào stinger | HP lần đầu về 0 | 0,4–0,8 | 2 | F + M | P0 |
| 8 | Trận đấu [G] | Bắt đầu đợt | Trống tay và chuông gõ đôi | WaveManager chuyển active và cho sinh quái | 0,45–0,70 | 2 | M | P0 |
| 9 | Trận đấu [G] | Hoàn thành đợt và chờ chuyển | Ba hạt nhạc gọn, cue đếm đợt sau tùy HUD | Khi đợt hoàn tất, không tái phát theo bộ đếm | 0,55–0,85 | 2 | M | P0 |
| 10 | Trận đấu [G] | Chuyển bản đồ | Luồng dịch chuyển và dấu nhạc riêng từng biome | Teleport Player, bật ambience mới khi vùng đổi | 0,65–1,10 | 4 | M + L | P0 |
| 11 | Trận đấu [G] | Chiến thắng và Chơi lại | Stinger đồng sáng rồi nút reset gõ gọn | Một lần khi Victory, xác nhận lúc nhấn Chơi lại | 1,5–3,0 | 2 | M + UI | P0 |
| 12 | Trận đấu [G] | Thất bại và Chơi lại | Hợp âm hạ có nhịp, mềm hơn vật rơi | Một lần khi Defeat, tách cue retry | 1,3–2,5 | 2 | M + UI | P0 |
| 13 | Trận đấu [T/P] | Tạm dừng và tiếp tục | Hai cue đối xứng với snapshot tắt/mở dần | Ngay khi trạng thái gameplay được xác nhận, Guide chỉ phát cue UI | 0,15–0,28 | 2 mỗi loại | UI + M | P1 |
| 14 | Trận đấu [T/P] | Cảnh báo nguy hiểm và báo Boss | Trống nén và hai âm định hướng, ưu tiên cao | Bắt đầu telegraph được xác nhận ở Boss test hoặc cơ chế tương lai | 0,25–0,60 | 3 | M | P0 |
| 15 | Tiến trình [T] | Nhận EXP từ quái bị hạ | Hạt thủy tinh thấp, gộp đợt chết cùng lúc | MageExperience nhận EXP thật, bỏ qua bia tập | 0,08–0,16 | 5 | D + M | P1 |
| 16 | Tiến trình [P] | Nhặt EXP rơi và vật phẩm | EXP hạt nhỏ về người, vật phẩm chạm nặng hơn theo loại | Chờ xác nhận pickup thật, chỉ báo sau khi nhận thành công | 0,12–0,30 | 4 mỗi nhóm | F + M | P2 |
| 17 | Tiến trình [T] | Tăng cấp và mở bảng nâng cấp | Hợp âm búng ngắn có điểm rơi rồi nhạc nền hạ | LevelReached hoặc thao tác lên cấp thử xác nhận, gộp khi nhảy nhiều cấp | 0,65–1,00 | 2 | M | P1 |
| 18 | Tiến trình [T] | Chọn nâng cấp, Sinh lực, Nhanh nhẹn, Tập trung | Tiếng xác nhận chung và lớp mềm riêng cho máu, tốc, tốc đánh | Thẻ áp dụng thành công. Nếu hồi HP cùng lúc chỉ giữ accent phù hợp | 0,14–0,28 | 3 + 3 lớp | UI + M | P1 |
| 19 | Tiến trình [T] | Tiến hóa, mở lõi và thức tỉnh HEXCORE | Ba cấp cao trào, cho từng hệ chọn lớp riêng | Chọn nhánh, kích hoạt lõi hoặc chiêu cuối thành công. Không lặp ở UI và combat | 0,70–1,50 | 3 motif + 3 lớp | M + L | P1 |

### Súng

| STT | Hệ/Nhánh | Hành động/Kỹ năng/Sự kiện | Đặc điểm âm thanh đề xuất | Thời điểm phát | Thời lượng | Số biến thể | Nguồn asset | Mức ưu tiên |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| 20 | Súng [G/T] | Phát bắn cơ bản và đạn trúng | Snap rõ, thân trầm ngắn, đầu đạn và hit riêng | WeaponAim hoặc loạt hợp lệ trong lab, hit sau va chạm thật | 0,08–0,18 | 6 phát + 4 hit | F + M | P0 |
| 21 | Shotgun [T] | Loạt bắn Shotgun | Một cú nổ dày, khóa cơ khí và chụm impact | Một cue khi mở cụm pellet, gộp hit cùng mục tiêu | 0,20–0,35 | 6 | M + L | P1 |
| 22 | SMG [T] | Loạt bắn SMG | Mỗi viên nhẹ, thay đổi accent đầu và cuối | Tạo đạn thật theo lịch loạt, không bắn sau khi hủy | 0,06–0,15 mỗi viên | 8 | M | P1 |
| 23 | Rocket [T] | Phóng Rocket và vụ nổ | Piston rồi lực đẩy, nổ giòn có đất bắn | Phóng, kéo loop theo tên lửa nếu còn sống, khi nổ một lần | 0,22 + 0,40–0,65 | 4 mỗi lớp | M + F + L | P1 |
| 24 | Súng [T] | Đạn nảy | Tia ngân đổi độ cao theo lần đổi hướng | Từng bounce thật, giảm lực ở các lần sau | 0,08–0,13 | 4 | D + M | P1 |
| 25 | Súng [T] | Xuyên thấu | Tiếng khía kéo qua nhiều bia | Mỗi xuyên gây sát thương, gộp trùng trong cửa sổ 50 ms | 0,07–0,14 | 4 | F + M | P1 |
| 26 | Súng [T] | Đạn bung toả | Pop tách đầu đạn như hạt tỏa | Khi sinh đạn con thật, hit con giảm âm lượng | 0,12–0,20 | 4 | D + L | P1 |
| 27 | Shotgun [T] | Thêm viên, Đạn hẹp, Đẩy lùi | Thêm thân loạt, thu bề rộng stereo, nhấn lực hất | Trên cue bắn hoặc lần đẩy lùi thật, cùng một voice ưu tiên | 0,10–0,22 | 3 lớp | L | P1 |
| 28 | SMG [T] | Đạn băng, Ném lựu đạn, Thêm đạn | Mạt băng lúc áp lạnh, pin lựu đạn rồi pop nổ, thêm nhịp viên | Sau hiệu ứng lạnh, spawn lựu đạn, nổ và từng phát được thêm thật | 0,09–0,45 | 3 mỗi nhóm | M + F | P1 |
| 29 | Rocket [T] | Bùng nổ, Đẩy lùi, Thêm đạn | Nổ rộng và đất văng, lực hất, accent bệ phóng | Lần nổ có bậc, đẩy thật, thêm lần phóng nếu thực xảy ra | 0,10–0,60 | 3 lớp | F + L | P1 |
| 30 | Shotgun [T] | Lưỡi đạn càn quét | Lưỡi khí có cạnh kim loại lẫn hơi nòng | Phát đủ nhịp sinh sóng, hit nhẹ ở tiếp xúc | 0,26–0,38 | 4 | M + L | P1 |
| 31 | Shotgun [T] | Áp lực công phá | Van tụ, ba cầu lửa bật liên tiếp, rạn giáp sắc | Tụ một lần, từng launch thật và từng điểm nổ, nhấn rạn giáp khi áp | 0,30 + 3 × 0,28 | 3 mỗi lớp | M + L | P1 |
| 32 | Shotgun [T] | Phun lửa | Phụt mồi, lửa xé ổn định rồi tắt van | Bật loop theo vùng nón, 8 tick gameplay không phát 8 vụ nổ | 1,6 loop + 0,16 | 3 | M + L | P1 |
| 33 | SMG [T] | Kíp nổ liên hoàn | Kíp gõ rồi cụm tên lửa rít hội tụ và nổ | Kích hoạt theo số loạt trúng cùng bia, impact tại bia | 0,50–0,85 | 3 | M + L | P1 |
| 34 | SMG [T] | Băng đạn cộng hưởng | Khóa hai súng phụ và laser mảnh đối đáp | Khi hai súng ra, từng cặp laser và tắt sau cửa sổ 3 s | 0,15–0,30 lớp | 4 | M + D | P1 |
| 35 | SMG [T] | Hồi máu | Slime bật mềm, hạt tim dịu | Spawn slime, cue hồi chỉ khi HP tăng, đóng khi kết thúc 5 s | 0,14–0,32 | 3 mỗi lớp | M + L | P1 |
| 36 | Rocket [T] | Vết rách không gian | Vỏ không khí rách dọc hướng bay, rền và tàn | Khi vệt lan, loop theo vùng và nhấn hit thưa | 0,35 + loop | 3 | M | P1 |
| 37 | Rocket [T] | Xuyên kích hồi quy | Hạt điện trôi, tia phân nhánh gọn | Spawn quả cầu, từng nhịp xả 0,3 s theo runtime trong 2,4 s | 0,09–0,18 xả | 5 | M + D | P1 |
| 38 | Rocket [T] | Thiên thương | Chuông đánh dấu, cột quang từ trên xuống | Dấu vòng đặt một lần, cột khi phát và hit chỉ nếu có trúng | 0,20 + 0,45 | 3 | M + L | P1 |
| 39 | Shotgun [T] | Giáp phản pháo | Khiên bật vàng, đỡ và trả nòng dứt khoát | Khi tạo khiên, chặn thật, rồi phản pháo thật | 0,15–0,38 mỗi pha | 4 | M + L | P1 |
| 40 | Shotgun [T] | Lưỡi cưa bám đuổi | Đĩa xoáy có răng, tông vòng quay ổn định | Ra đĩa và ba vòng trong 2,4 s, nhấn khi sát thương thật | 0,23 + loop | 3 | M + L | P1 |
| 41 | SMG [T] | Hoả lực leo thang | Khóa chế độ plasma, tiếng bắn nóng hơn và dấu độc | Đổi đạn trong 5 s, độc chỉ accent lần đầu, xuống chế độ | 0,20 + 0,09 mỗi phát | 4 mỗi lớp | M + D | P1 |
| 42 | SMG [T] | Đạn phân rã | Lách giòn tách mảnh kèm âm đếm ngắn | Khi đủ hit kích hoạt, tiếp đó mỗi proc thật đã được gộp | 0,12–0,24 | 4 | M | P1 |
| 43 | Rocket [T] | Cổng xuyên kích | Hai cổng bật đối xứng, tia nối, đạn ra có chirp | Mở và đóng, loop tia, tái xuất từng viên xuyên hợp lệ có giới hạn | 0,18–0,34 + loop | 4 | D + M | P1 |
| 44 | Rocket [T] | Mỏ neo xuyên tâm | Cọc Tesla cắm, dây căng, đứt hoặc nổ hết hạn | Gắn neo, loop căng, hai nhánh kết thúc tách cue, choáng khi hiệu lực | 0,15–0,55 + loop | 4 | M + L | P1 |
| 45 | HEXCORE Súng [T] | Lốc lửa | Theo VFX hiện hành là lốc băng, thân xoáy lạnh và hạt vụn | Khởi lốc, loop bay 7 s, tắt khi hết. Hợp nhất khi có nhiều lốc | 0,26 + loop + 0,32 | 3 | M + L | P1 |
| 46 | HEXCORE Súng [T] | Mưa thiên thạch | Dấu trời, 5 rít rơi và 5 điểm nổ biến thiên | Telegraph rồi từng quả đáp thật, accent cuối riêng | 0,25–0,65 mỗi lớp | 5 impact | M + F + L | P1 |
| 47 | HEXCORE Súng [T] | Pháo đài cơ động | Chân trụ chốt, hum và xung plasma | Đặt trụ hợp lệ, pulse theo tick trụ, tháo loop khi trụ mất | 0,24–0,45 + loop | 4 | M + L | P1 |
| 48 | HEXCORE Súng [T] | Lồng đạn Hexcore | Đỉnh khóa theo vòng, cặp đạn bắn vào tâm | Lồng hình thành, từng cặp ở hai vòng, accent tâm khi có impact | 0,13–0,50 mỗi lớp | 3 | M + L | P1 |

### Cận chiến

| STT | Hệ/Nhánh | Hành động/Kỹ năng/Sự kiện | Đặc điểm âm thanh đề xuất | Thời điểm phát | Thời lượng | Số biến thể | Nguồn asset | Mức ưu tiên |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| 49 | Kiếm sĩ [T] | Đánh thường Kiếm sĩ | Hai nhát thép ngang/ngược, nhát hai gọn có lực | Hai frame vung trong combo và xác nhận từng hit | 0,12–0,25 | 4 mỗi nhát | R Swing, Impact + L | P1 |
| 50 | Sát thủ [T] | Đánh thường Sát thủ | Ba nhát khô, đổi cạnh thép, ít đuôi | Từng nhát thật và hit nếu có | 0,08–0,16 | 4 mỗi nhát | R Swing + M | P1 |
| 51 | Đao sĩ [T] | Đánh thường Đao sĩ | Hai tiếng gió nặng, cán và mép đao riêng | Hai nhịp chém thật, impact theo bậc nặng | 0,16–0,33 | 4 mỗi nhát | R HeavySwing + L | P1 |
| 52 | Kiếm khí [T] | Đánh thường Kiếm khí | Hai sóng ngắn khác cao độ, lưỡi sáng nhẹ | Từng sóng khi sinh, chạm theo từng đường | 0,14–0,23 | 4 mỗi nhát | R Wave + L | P1 |
| 53 | Cận chiến chung [T] | Phong Ảnh Trảm (Kiếm Lướt) | Hơi gió thoát và vệt kiếm bạc bay xuyên | Khi proc đủ 5 hit, phóng rồi trúng theo vệt. Foley đế/lướt chỉ dành cho take Kiếm Lướt cũ | 0,26–0,38 | 4 | R Wave, Swing + L | P1 |
| 54 | Cận chiến chung [T] | Hoành Nguyệt Trảm | Vòng kiếm quét 360° có quán tính và mép sắc | Khi proc đủ 4 hit, cue vòng và hit gộp quanh tâm | 0,28–0,47 | 4 | R Swing + L | P1 |
| 55 | Cận chiến chung [T] | Phản Kiếm Quyết | Chạm đỡ sáng và chém phản hướng nguồn | Tại giao dịch nhận đòn hợp lệ, đỡ rồi vung phản thật | 0,22 + 0,18 | 4 | R Parry + L | P1 |
| 56 | Sát thủ [T] | Ám Bộ Trảm | Hút bóng, bật ra sau mục tiêu, hai nhát đối hướng | Biến mất, tái xuất, nhát thuận và nghịch theo hai cửa sổ hit | 0,10–0,22 mỗi lớp | 4 | R Vanish, Return + M | P1 |
| 57 | Sát thủ [T] | Huyết Ấn | Dấu đỏ mỏng, nhịp căng rồi nổ ấn | Đặt tối đa 5 dấu, nổ tại từng bia, lấy cue đại diện nếu dồn | 0,10 + 0,28 | 4 | M + L | P1 |
| 58 | Sát thủ [T] | Dạ Vũ Liên Trảm | Ba lượt lướt chém, mỗi lượt khác sắc tiếng | Từng lần lao, từng nhát và hit riêng nếu trúng | 0,15–0,24 mỗi nhát | 4 | R Dash, Swing + L | P1 |
| 59 | Sát thủ [T] | Tàng Ảnh | Khói nuốt rồi chém phục kích ngắn | Khi ẩn, đổi vị trí, hiện và chém thật | 0,12–0,30 mỗi lớp | 4 | R Vanish, Return + L | P1 |
| 60 | Sát thủ [T] | Đoạt Mệnh | Lao chặt, nhói định âm khi kết liễu | Lao, lấy lực, va chạm, accent điều kiện HP thấp khi thực áp dụng | 0,25–0,46 | 4 | R Dash, Finisher + L | P1 |
| 61 | Sát thủ [T] | Song Ảnh Truy Sát | Hai tiếng bóng ở hai cánh và dao găm nhanh ở tâm | Từng làn bóng và khi dao giữa rời tay, hit gộp theo làn | 0,10–0,23 mỗi lớp | 4 | M + L | P1 |
| 62 | Đao sĩ [T] | Chấn Địa Trảm | Đao nặng dội đất, vòng chấn gãy nhỏ | Tụ, bổ chạm đất và một accent choáng khi có hiệu lực | 0,30 + 0,52 | 4 | R Charge, Slam + L | P1 |
| 63 | Đao sĩ [T] | Hoành Thiên Đao | Lưỡi quét rộng, dải gió lớn và hit có lực hất | Phóng dải sóng, va theo dải đang bay và đẩy thật | 0,35–0,55 | 4 | R HeavySwing + M | P1 |
| 64 | Đao sĩ [T] | Cuồng Phong Đao | Hai vòng gió nặng khác tông | Từng vòng có cửa sổ trúng, vòng hai nhấn khác vòng một | 0,23–0,40 mỗi vòng | 4 | R HeavySwing + L | P1 |
| 65 | Đao sĩ [T] | Bá Vương Đao Vực (thay slot Bất Động Chiến Thế) | Hút đất về tâm rồi một cú quét nặng | Vào thế, hút quái hợp lệ nếu có và một cú quét | 0,28 + 0,46 | 4 | M + L | P1 |
| 66 | Đao sĩ [T] | Đoạn Sơn Trảm | Dồn lực, bổ dọc, vết nứt lan từng lớp | Tụ theo nhịp lấy đà, đao chạm, lan dọc hành lang | 0,40 + 0,62 | 4 | R Charge, Slam + L | P1 |
| 67 | Đao sĩ [T] | Bất Diệt Chiến Ý | Một tiếng dựng thế, đòn thường cường hóa có xung trầm | Khi buff bắt đầu, từng xung thật, âm tan khi buff hết | 0,15–0,40 mỗi lớp | 4 | M + L | P1 |
| 68 | Kiếm khí [T] | Nguyệt Nha Trảm | Vầng sáng bén kéo dài, va mỏng có accent đẩy | Lúc phóng, đi xuyên và đẩy quái thường khi trúng | 0,30–0,50 | 4 | R Wave + L | P1 |
| 69 | Kiếm khí [T] | Lưỡng Nghi Kiếm | Hai cao độ đối đáp rồi hợp nổ ở giao điểm | Từng đường và điểm giao tương ứng vùng nổ thật | 0,22 + 0,30 | 4 | M + L | P1 |
| 70 | Kiếm khí [T] | Kiếm Vũ Thiên Hà | Mưa 15 kiếm chia ba đợt, gõ hạt gọn | Dấu ngắm rồi 3 đợt rơi. Accent theo điểm đáp nổi bật trong từng đợt | 0,24 chuẩn bị + 0,32 mỗi đợt | 5 | M + L | P1 |
| 71 | Kiếm khí [T] | Thiên Kiếm Xuyên Tâm (thay slot Kiếm Giới) | Khóa thanh, đại kiếm rơi thẳng và chạm gọn | Khi có khóa hợp lệ, rơi và hit trên một mục tiêu | 0,18 + 0,50 | 3 | M + L | P1 |
| 72 | Kiếm khí [T] | Phi Kiếm Truy Hồn | Phóng và tiếng nảy đổi cao độ theo chuỗi | Lúc phóng, 1 bia đầu và tối đa 4 bia khác thực trúng | 0,23 + 0,09 mỗi bia | 5 | M + D | P1 |
| 73 | Kiếm khí [T] | Hồi Nguyệt Kiếm | Sóng đi sáng, hồi về hút tông và bắt chuôi | Khi đi, quay, hit từng lượt hợp lệ và bắt | 0,15–0,35 mỗi lớp | 4 | R Wave, Return + L | P1 |
| 74 | HEXCORE Cận chiến [T] | Thiên Vẫn Trảm | Bật, luồng lao xuống, một nổ đất chủ lực, hai vòng rung và nứt tỏa | Mốc cất, lao, chạm đất trong bán kính thử 3,4. Hai vòng là đuôi VFX, một lần tính impact | 0,26 + 0,34 + 1,15 | 3 mỗi lớp | R Charge, Slam + M + L | P1 |
| 75 | HEXCORE Cận chiến [T] | Tam Thiên Kiếm Quyết | Ba lớp kiếm khí sắc, tăng quãng ngân và thân trầm theo nhát | Ba sóng ứng tầm 3,6/4,95/6,3. Hit thật gộp theo sóng, nhát ba có đuôi xé | 0,26 + 0,36 + 0,65 | 3 mỗi nhát | R Wave + M + L | P1 |
| 76 | HEXCORE Cận chiến [T] | Vạn Ảnh Tuyệt Sát | Hút bóng, 2 đợt bóng tản vị trí, xuất hiện và nhát chốt trầm | Hai lượt bốn bóng lệch nhịp. Một tín hiệu hit đại diện mỗi cụm theo mask, nhát kết riêng | 0,18 + 2 cụm + 0,60 | 4 bóng + 3 chốt | R Vanish, Return, Finisher + M + L | P1 |

### Pháp sư

| STT | Hệ/Nhánh | Hành động/Kỹ năng/Sự kiện | Đặc điểm âm thanh đề xuất | Thời điểm phát | Thời lượng | Số biến thể | Nguồn asset | Mức ưu tiên |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| 77 | Pháp sư [T] | Đạn phép cơ bản | Hạt phép bật tròn, bay nhẹ, va sáng nhỏ | Lúc sinh phi đạn và hit thật | 0,12–0,22 | 6 + 4 hit | D + M | P1 |
| 78 | Pháp sư [T] | Hồi phục | Tụ ấm và chuông dịu khi máu tăng | Kích hoạt thành công, lớp hồi chỉ ở giao dịch tăng HP | 0,30–0,55 | 4 | M + L | P1 |
| 79 | Pháp sư [T] | Sóng xung kích | Thủng khí dạng vòng, hút nhẹ rồi đẩy | Bật xung diện rộng, một impact đại diện nếu trúng | 0,32–0,52 | 4 | M + L | P1 |
| 80 | Pháp sư [T] | Vùng trọng lực | Thấu kính mở, nền xoáy chậm, tắt gọn | Hình thành vùng 4 s tại chuột, loop theo vùng và kết thúc | 0,32 + loop + 0,22 | 3 | M + L | P1 |
| 81 | Hỏa [T] | Cầu lửa và thiêu đốt | Bụp mồi, rít qua khí, nổ và cháy nhẹ | Sinh cầu, contact nổ, chỉ lớp cháy khi áp trạng thái | 0,15–0,40 | 5 | M + F + L | P1 |
| 82 | Hỏa [T] | Sức nổ, Sức cháy, Tầm nổ | Lớp nổ sâu, cháy sáng, bề rộng vùng | Chỉnh layer sẵn theo bậc ở sự kiện tương ứng | 0,14–0,36 | 3 lớp | L | P1 |
| 83 | Hỏa [T] | Cháy lâu | Duy trì lớp khí nóng nhỏ | Đổi độ dài loop đang chạy theo hạn cháy đã áp | loop 1–2 | 2 | L | P1 |
| 84 | Hỏa [T] | Thêm cầu lửa | Hai bụp phụ nhanh khác cao độ | Loạt thứ ba thực sinh hai cầu phụ | 0,12–0,18 | 4 | M + L | P1 |
| 85 | Hỏa [T] | Tập trung đốt | Nhiệt nứt gọn nhận diện mục tiêu đang cháy | Khi gây thêm sát thương thật, giới hạn theo mục tiêu | 0,10–0,18 | 3 | M | P1 |
| 86 | Hỏa [T] | Nổ khi chết | Vụ bụp nóng giòn ngay điểm gục | Quái có trạng thái cháy và chết, một lần | 0,25–0,40 | 4 | F + M | P1 |
| 87 | Băng [T] | Hơi thở băng, làm chậm và đóng băng | Luồng hơi thủy tinh, tích dấu nhỏ, khóa tinh thể giòn | Từng luồng, tích dấu có giới hạn, đạt ngưỡng đóng băng mới khóa | 0,12–0,32 | 5 | M + L | P1 |
| 88 | Băng [T] | Làm chậm, Đóng băng, Sát thương lạnh | Hạ độ sáng luồng nền, tinh thể khóa rõ, vỡ mạt khi đủ hiệu lực | Dùng lớp tương ứng trạng thái và lần chạm hợp lệ | 0,10–0,28 | 3 lớp | L | P1 |
| 89 | Băng [T] | Lạnh sâu | Gió tắt dần lạnh hơn khi áp slow mạnh | Khi áp hiệu lực, có thể dùng biến thể lớp slow | 0,10–0,18 | 3 | L | P1 |
| 90 | Băng [T] | Tụ Hàn | Hai tiếng tích lạnh khác tầm cao | Mỗi tầng hợp lệ tối đa hai, tránh mỗi tick | 0,09–0,14 | 3 | M | P1 |
| 91 | Băng [T] | Giòn vỡ | Nứt kính lạnh ngắn khi trúng quái đông | Khi thêm damage thành công lên mục tiêu đóng băng | 0,11–0,24 | 4 | F + M | P1 |
| 92 | Băng [T] | Lan băng | Lách tinh thể truyền sang bia khác | Sau truyền dấu lạnh thật, gộp lan cùng cụm | 0,13–0,23 | 4 | M + L | P1 |
| 93 | Quang [T] | Cột sáng | Chuông đánh dấu, cột dội theo từng nhịp hit | Ở điểm chuột và các nhịp thực gây sát thương | 0,18–0,28 mỗi nhịp | 5 | M + L | P1 |
| 94 | Quang [T] | Thêm nhịp, Tầm cột sáng, Sức nhịp | Thêm nhịp accent nhẹ, bề rộng và thân sáng theo bậc | Từng nhịp bổ sung và biến thể động trên cue gốc | 0,12–0,30 | 3 lớp | L | P1 |
| 95 | Quang [T] | Dư quang | Một âm cột vọng lùi nhỏ | Ở nhịp sát thương bổ sung thực | 0,16–0,23 | 3 | M | P1 |
| 96 | Quang [T] | Phán Quyết | Chuông trong và gõ sáng mạnh hơn ở nhịp ba | Ở nhịp gây thêm sát thương tương ứng | 0,24–0,37 | 3 | M + L | P1 |
| 97 | Quang [T] | Dẫn sáng | Lướt ánh sáng mỏng báo đổi tốc | Bật buff tốc cho người chơi theo cột, không phát lại mỗi frame | 0,13–0,22 | 3 | M | P1 |
| 98 | Quang [T] | Chói mắt | Ánh sắc mềm, suy yếu có dấu gọn | Khi trạng thái giảm sát thương áp lần đầu | 0,10–0,20 | 3 | M | P1 |
| 99 | Ám [T] | Phi đạn bóng tối và lời nguyền | Phi đạn hút, va tím mềm và dấu bám | Phóng, bay đuổi, mục tiêu nhận nguyền rồi duy trì nhỏ | 0,14–0,28 | 5 | M + L | P1 |
| 100 | Ám [T] | Số mục tiêu, Sức nguyền, Suy yếu | Nhiều dấu gộp, thân trầm dày, thì thầm suy yếu | Theo số mục tiêu thật và lúc áp suy yếu, giữ một cue đại diện | 0,10–0,24 | 3 lớp | L | P1 |
| 101 | Ám [T] | Đa ấn | Một chùm dấu tối gọn | Khi có thêm mục tiêu nguyền thực, gộp khu vực | 0,15–0,23 | 3 | M + L | P1 |
| 102 | Ám [T] | Dày Vò | Nhói trầm ngắn, tăng cảm giác dồn | Ở lần trúng thứ ba gây dồn thực | 0,19–0,29 | 4 | M | P1 |
| 103 | Ám [T] | Suy nhược | Âm hụt xuống báo giảm lực | Khi áp/đổi mức, không reset lớp âm theo mỗi tick | 0,10–0,20 | 3 | L | P1 |
| 104 | Ám [T] | Di chúc | Dấu tan ở bia chết rồi nối qua bia sống | Chỉ khi truyền nguyền thành công | 0,20–0,32 | 4 | M + L | P1 |
| 105 | HEXCORE Pháp sư [T] | Ngưng Đọng Thời Gian | Tiếng khóa, chiến trường lọc nhỏ, giải phóng hit dồn | Khi dừng quái, duy trì khoảng 3 s, unlock/xả hit ở đích thật | 0,30 + loop + 0,45 | 3 | M + L | P1 |
| 106 | HEXCORE Pháp sư [T] | Thiên Tinh Giáng Lâm | Thiên thể báo trước, năm đợt từ airy đến rung mạnh | Dấu vùng, từng đợt sao đáp theo lịch thực, gộp sao cùng đợt | 0,30 + 5 lớp 0,4–0,65 | 5 rơi/nổ | M + L | P1 |
| 107 | HEXCORE Pháp sư [T] | Thiên Cầu Ma Lực | Cầu tụ biến thiên, phóng nhiều tiếng mảnh, va riêng | Tụ theo thời lượng thực, xả 2–8 phi đạn và hit theo từng đích | 0,30 + loop + 0,14/phát | 5 phóng + 5 hit | M + L | P1 |

### Quái và Boss

| STT | Hệ/Nhánh | Hành động/Kỹ năng/Sự kiện | Đặc điểm âm thanh đề xuất | Thời điểm phát | Thời lượng | Số biến thể | Nguồn asset | Mức ưu tiên |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| 108 | Lính thường [G] | Lính Xanh, Nâu, Đỏ, Tuyết xuất hiện và đuổi | Chung họ quái, lớp lá, bụi, than hoặc tuyết nhẹ | Spawn xác nhận, bước gần Player thưa và ngẫu nhiên | 0,10–0,25 | 4 lớp + 4 bước | F + L | P1 |
| 109 | Lính thường [G] | Lấy đà, chém và trúng người chơi | Tiếng kéo dao chậm rồi lưỡi dứt, trúng dùng lớp thương người | EnemyMeleeAttack vào windup, swing tại cửa sổ chém và hit có giảm HP | 0,16 + 0,12 | 5 | F + M | P0 |
| 110 | Lính thường [G] | Trúng đòn và gục | Hơi tịt nhỏ và rơi mềm, theo loại bề mặt | HP giảm và chết thật. Không phát khi dọn đợt hoặc trả pool | 0,11–0,27 | 6 | F + M | P0 |
| 111 | Quái [G/T] | Tăng sức theo tuổi và hiệu ứng khống chế | Tăng chỉ số thụ động im, choáng/chậm/hết đông có lớp rõ | Chỉ phát ở transition trạng thái hợp lệ, không theo mỗi frame hoặc DoT | 0,09–0,22 | 4 | M + L | P1 |
| 112 | Cung thủ Tuyết [A/T] | Căng dây, nhả tên, tên bay và va | Dây siết, twang rỗng, lướt tuyết và chạm | Windup, spawn mũi tên và va chạm thật. Chung bộ biến thể cho màu khi có gameplay | 0,10–0,32 | 5 | F + M | P1 |
| 113 | Cung thủ các màu còn lại [A/P] | Hình thái cung thủ trong Guide và art | Tái dùng bộ cung, thêm lớp bề mặt và động tác có thật | Sau khi prefab và spawn được xác nhận, Guide chỉ xem cue riêng UI | 0,10–0,32 | 4 màu | R bộ cung + L | P2 |
| 114 | Tinh nhuệ [A/P] | Bước, lấy đà, chém, trúng và gục | Chân nặng, xước thép lớn, tông theo biến thể art | Khi AI tinh nhuệ được gắn, hook theo cửa sổ gốc, tăng ưu tiên gần | 0,18–0,48 | 4 mỗi nhóm | F + L | P2 |
| 115 | Boss sân Súng [T] | Loạt quạt đạn và lao tới | Cảnh báo trước quạt, năm đầu đạn mảnh, lao có tiếng kéo đất | Telegraph, tạo quạt 5 đạn hoặc bắt đầu dash, va nếu trúng | 0,25 + 0,15–0,35 | 4 | D + M | P1 |
| 116 | Boss bản đồ [A/P] | Xuất hiện, đổi mạng, hồi lại và bại trận | Ba căn âm xương khô, dung nham, hơi lạnh | Khi state/phase Boss chính được triển khai và xác nhận | 0,50–1,30 | 3 × 3 trạng thái | M + L | P2 |
| 117 | Xương khô [A/P] | Lướt áp sát | Lách xương rồi gió bẹt nhanh | Đề xuất lúc lấy đà, đầu DashTrail, chốt điểm đến | 0,25–0,45 | 4 | F + M | P2 |
| 118 | Xương khô [A/P] | Xoay kiếm vòng tròn | Trục xương xoay và lưỡi thép quét | Đề xuất khi SpinSlash mở, nhấn va thực | 0,40–0,66 | 4 | F + M | P2 |
| 119 | Xương khô [A/P] | Nhảy tới rồi gầm | Bật, đáp giòn và gầm xương rỗng thành sóng | Đề xuất ở Jump, tiếp đất rồi RoarShockwave | 0,18 + 0,66 | 3 | M + L | P2 |
| 120 | Golem dung nham [A/P] | Phun lửa hình nón | Hít trầm, bụp miệng rồi luồng lửa | Đề xuất mở và đóng ConeFire theo vùng hiệu lực | 0,28 + loop | 3 | M + L | P2 |
| 121 | Golem dung nham [A/P] | Hồi máu dung nham | Nhịp nóng tụ vào thân và sáng lên | Đề xuất lúc HealAura xuất hiện và khi hồi thật | 0,45–0,80 | 3 | M | P2 |
| 122 | Golem dung nham [A/P] | Ném cầu lửa, phân tách và hố dung nham | Đá vung, các hạt lửa tách, bọt nền thấp | Đề xuất Throw, mỗi cụm cầu tách, va và hố xuất hiện/mất | 0,25–0,56 + loop | 4 | F + M + L | P2 |
| 123 | Yeti [A/P] | Ném cầu tuyết | Tuyết nén, bay gió rét, chạm bụp bột | Đề xuất cất tay, spawn Snowball, va khi thật | 0,18–0,39 | 4 | F + M | P2 |
| 124 | Yeti [A/P] | Cảnh báo đóng băng và nhà tù băng | Hai dấu lạnh gọn, khóa tảng rõ rồi rạn tan | Đề xuất FreezeWarning, IcePrison áp thật, kết thúc trạng thái | 0,27–0,48 | 4 | M + L | P2 |
| 125 | Yeti [A/P] | Gồng và bão tuyết | Hơi gồng sâu, gió hạt lớn, gỡ bão dịu | Đề xuất bật buff và loop Snowflake theo vùng thật | 0,38 + loop + 0,45 | 3 | M + L | P2 |

### Main Menu, môi trường và nhạc

| STT | Hệ/Nhánh | Hành động/Kỹ năng/Sự kiện | Đặc điểm âm thanh đề xuất | Thời điểm phát | Thời lượng | Số biến thể | Nguồn asset | Mức ưu tiên |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| 126 | Menu [G] | Hover và focus bàn phím | Một tick sáng nhỏ theo phần tử | Khi mục khả dụng nhận focus mới, chống phát kép hover/focus | 0,05–0,10 | 4 | UI | P0 |
| 127 | Menu [G] | Nhấn, xác nhận và hủy | Chạm nén, pop mở, vuốt lùi nhỏ | Một lần cho mỗi thao tác hợp lệ, xác nhận dùng biến thể riêng | 0,08–0,23 | 3 mỗi nhóm | R Confirm + UI + M | P0 |
| 128 | Menu [G] | Bắt đầu | Đòn mở hành trình và nhịp pickup ngắn | Chấp nhận khởi tạo run, fade nhạc menu khi load hoàn tất | 0,45–0,72 | 2 | M + L | P0 |
| 129 | Menu [G/P] | Chơi tiếp và trạng thái khóa | Vô hiệu im, nếu có thao tác báo khóa được hỗ trợ thì gõ nhẹ | Khi có save hợp lệ và click thành công sau tích hợp thật | 0,10–0,30 | 3 | UI + M | P2 |
| 130 | Menu [G] | Chọn màn và duyệt thẻ | Vuốt thẻ, click chọn, accent màu riêng bốn map | Đổi thẻ/focus và xác nhận theo hành động có thật. Khóa không tự bật âm | 0,08–0,22 | 4 map + 3 | UI + L | P1 |
| 131 | Menu [G] | Hướng dẫn, chuyển tab và lọc quái | Gập giấy tối giản, lọc tách nhẹ, mở chi tiết bật một hạt | Đổi tab, lọc, mở/đóng thẻ hoặc nút mẫu pause và resume | 0,08–0,20 | 4 | UI + L | P1 |
| 132 | Menu [G] | Cài đặt, thanh âm lượng, mute và khôi phục | Tick thanh nhẹ, cặp tắt/bật rõ, khôi phục có pop | Khi giá trị thực đổi, lấy mẫu tối đa 5 Hz, mute theo chính sách nghe | 0,07–0,23 | 3 mỗi nhóm | UI + M | P1 |
| 133 | Menu [G] | Thoát, cảnh báo xác nhận và quay lại | Hộp bật âm thấp, hủy/thoát phân biệt | Khi mở modal, đóng hay xác nhận thoát; không giữ tiến trình quit để chờ clip | 0,11–0,26 | 3 | UI + M | P1 |
| 134 | Môi trường [G] | Forest | Lá xào xạc thấp, gió ấm và điểm tự nhiên thưa | Fade khi vào Forest, one-shot nền ngẫu nhiên, không báo nguy hiểm | 30–48 loop | 2 loop + 5 điểm | M + L | P0 |
| 135 | Môi trường [G] | Moon | Bụi mịn stylized và rung khoáng rỗng | Khi vào Moon, tone môi trường hư cấu nhỏ | 30–48 loop | 2 loop + 4 điểm | M + L | P0 |
| 136 | Môi trường [G] | Inferno | Nhiệt thở, đá lách tách và sôi xa | Khi vào Inferno, ambient địa hình, không được giống cảnh báo đòn | 30–48 loop | 2 loop + 5 điểm | M + F + L | P0 |
| 137 | Môi trường [G] | Tundra | Gió lạnh dải mỏng, hạt tuyết và nứt nhỏ xa | Khi vào Tundra. Giữ dải trung trống cho skill băng | 30–48 loop | 2 loop + 5 điểm | M + L | P0 |
| 138 | Nhạc [G] | Main Menu · Đại chiến sinh tồn | Groove nhanh, pluck đồng và synth sạch, motif ba nốt HEXCORE | Khi mở menu, loop xuyên panel, fade sang trận | 48–72 | 1 loop + 2 STEM | R Music làm sketch + M | P0 |
| 139 | Nhạc [G/P] | Chiến đấu bốn map | Chung nhịp, biến tấu nhạc cụ cho Forest, Moon, Inferno, Tundra | Mỗi khu vực vào phách, tăng năng lượng khi wave active | 45–75 | 4 loop + 3 STEM mỗi map | M | P0 |
| 140 | Nhạc [T/P] | Boss và cao trào | Ostinato rõ, trống lớn giữ chỗ cảnh báo, stem tăng theo phase | Khi Boss active thật, đổi cường độ trên phách, nhả khi rời hoặc chết | 45–70 | 1 test + 3 map | M | P2 |
| 141 | Nhạc [G] | Kết thúc trận, chiến thắng/thất bại | Stinger chuyển sang đuôi ngắn, nền menu trở lại khi tải lại menu | Victory/Defeat một lần rồi decay hoặc loop nhẹ theo màn hình | 3–8 | 2 kết quả | M + L | P0 |

### Các sự kiện bổ sung

| STT | Hệ/Nhánh | Hành động/Kỹ năng/Sự kiện | Đặc điểm âm thanh đề xuất | Thời điểm phát | Thời lượng | Số biến thể | Nguồn asset | Mức ưu tiên |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| 142 | Nhân vật [G/T] | Đứng yên, bắt đầu và ngừng di chuyển | Idle im, trọng lượng đi vào bước đầu, bụi/đế nhỏ khi dừng gấp | Chỉ đóng mở bộ đếm bước theo vận tốc thực, không lặp cue khi giữ phím sát vách | 0,08–0,15 | 4 | R Foot + F | P1 |
| 143 | Nhân vật [T/P] | Hồi chiêu sẵn sàng và yêu cầu kỹ năng không hợp lệ | Chime thấp chọn lọc, chú ý phản hồi nhẹ cho phím bị từ chối | Thông báo readiness ưu tiên ultimate. SFX từ chối chỉ nếu HUD đã quyết định báo, giới hạn 1 lần/giây | 0,08–0,20 | 3 mỗi loại | UI + M | P2 |
| 144 | Súng [T] | Hồi lượt Lốc lửa, giới hạn trụ và đạn hết tầm | Tích lượt nhẹ. Trụ thay thế có tắt nguồn ngắn. Đạn hết tầm im | Một lần khi lượt sẵn sàng theo chính sách HUD, hủy vòng trụ khi thay hoặc hết đời | 0,09–0,26 | 3 | D + M | P1 |
| 145 | Môi trường [G] | Điểm môi trường và ambience khi trở lại scene | Cắt thưa theo RNG có khoảng cách, tránh dồn khi load | Theo listener hiện tại, đưa loop cũ về 0 khi rời vùng hoặc retry | 1–3 mỗi điểm | 4–5 mỗi vùng | F + L | P1 |
| 146 | Menu [G] | Quay lại, đóng modal và ESC | Lớp hủy ngắn thống nhất giữa chuột và bàn phím | Khi điều hướng đổi thật. Không phát hover kế tiếp nếu focus được trả tự động cùng frame | 0,08–0,17 | 3 | UI + L | P1 |

## 4. Kiểm kê và xử lý WAV

Hai bộ nguồn trong Unity đều là PCM 16-bit mono. Soi header và mẫu PCM toàn bộ 15 file cho thấy **0 mẫu chạm biên clipping**, mẫu đầu và cuối đều 0. Peak và RMS bên dưới là dBFS đo trên waveform. Chúng không đo loudness cảm nhận, nhiễu nền, lỗi phase hay độ mượt khi loop trong gameplay. Các quyết định giữ/chỉnh/thay là đề xuất sản xuất dựa vào vai trò, thông số và cách đang phát. [Nguồn và công thức 13 cue Cận chiến](Melee/HEXCORE_ASSET_SOURCE_LICENSES.md)

| Clip | Hz | Giây | Peak / RMS dBFS | Xử lý đề xuất |
| --- | ---: | ---: | --- | --- |
| Foot.wav | 24000 | 0,095 | −1,9 / −16,7 | Giữ làm lớp đế, bổ sung bốn bề mặt và đổi mẫu trái/phải |
| Dash.wav | 24000 | 0,215 | −1,9 / −16,4 | Giữ lớp dịch chuyển nhẹ, thêm lớp lao nặng/đổi hướng riêng |
| Swing.wav | 24000 | 0,150 | −1,9 / −16,5 | Giữ cho kiếm cơ bản, dùng trong lớp đầu nhiều nhát có kiểm soát |
| HeavySwing.wav | 24000 | 0,220 | −1,9 / −17,3 | Giữ thân gió cho Đao sĩ, thêm cạnh và hit tách |
| Impact.wav | 24000 | 0,180 | −1,9 / −17,8 | Chỉnh gain và tạo biến thể theo vật liệu, gộp va lặp |
| Parry.wav | 24000 | 0,235 | −1,9 / −16,4 | Giữ lõi nhận diện chặn, cắt đuôi nếu che chém phản |
| Charge.wav | 24000 | 0,480 | −1,9 / −10,4 | Chỉnh level, RMS cao nhất bộ Cận chiến. Chỉ giữ lớp tụ, viết pha lên riêng cho ultimate |
| Wave.wav | 24000 | 0,260 | −1,9 / −13,7 | Giữ cho sóng ngắn. Tam Thiên cần ba wave layer riêng |
| Vanish.wav | 24000 | 0,220 | −1,9 / −14,6 | Giữ hút bóng sau khi A/B gain trong mix |
| Return.wav | 24000 | 0,190 | −1,9 / −14,4 | Giữ lớp xuất hiện, bổ sung bắt chuôi và chốt riêng |
| Slam.wav | 24000 | 0,440 | −1,9 / −16,0 | Dùng làm lớp điểm chạm, thêm crater và nứt đất Thiên Vẫn |
| Burst.wav | 24000 | 0,380 | −1,9 / −16,2 | Giữ cho prototype, thay lớp bùng của từng ultimate theo hình thái |
| Finisher.wav | 24000 | 0,360 | −1,9 / −15,9 | Giữ thân nhát kết, làm lớp cạnh riêng cho Vạn Ảnh |
| Confirm.wav | 22050 | 0,230 | −16,7 / −29,7 | Giữ bản xác nhận tạm. So gain cùng Music, tách click thông thường, hủy, focus |
| Music.wav | 22050 | 6,713 | −13,0 / −33,4 | Giữ sketch Concept B để so duyệt. Thay bằng loop 48–72 s có stem khi duyệt hướng, xác nhận cue point và hồ sơ nguồn trước phân phối |

Clip Cận chiến nằm trong [Shared/Audio](../Assets/Prefabs/Player/Melee/Shared/Audio). Trình phát prototype dùng một AudioSource, volume 0,38 và scale one-shot 0,7 khi nguồn đang phát. Đây là giá trị phát nguồn, cần kiểm bằng capture để chốt gain. Hai clip menu nằm trong [Home](../Assets/Resources/BrotatoMenu/Home). Không tìm thấy AudioMixer hoặc bộ WAV khác cho EXP, chiến đấu chính, quái, Boss và môi trường trong Assets. Các export review, demo HTML và video có âm thanh đính kèm phục vụ tham chiếu, không tính là clip Unity phát trực tiếp. Tăng sample rate từ bản gốc không tạo thêm chi tiết. Dùng bản tổng hợp/thu mới với source chất lượng cao cho lớp cần mở rộng.

### Nguồn đã kiểm tra và quyền sử dụng

- Foley ngắn, va chạm vật liệu và đế bước. Hai gói công bố CC0. [Kenney Impact Sounds](https://kenney.nl/assets/impact-sounds) · [Kenney RPG Audio](https://kenney.nl/assets/rpg-audio)
- Click/focus và texture blaster. Hai gói công bố CC0. [Kenney Interface Sounds](https://kenney.nl/assets/interface-sounds) · [Kenney Digital Audio](https://kenney.nl/assets/digital-audio)
- Nếu thêm thu âm tự nhiên từ Freesound, xác minh giấy phép ở từng clip và nguồn tác giả. Ưu tiên CC0. CC BY cần kèm tác giả, URL, phiên bản giấy phép và ghi chú chỉnh sửa. Loại nội dung NC hoặc thiếu chứng cứ quyền cho bản phát hành dự kiến. [Giải thích giấy phép Freesound](https://freesound.org/help/faq/)
- Nhạc, motif, Boss vocal phi ngôn ngữ và các lớp nhận diện nên làm riêng bằng synth, thu Foley của nhóm hoặc công cụ có quyền sử dụng rõ khi phát hành game. Điền ledger trước import gồm tên nội bộ, bằng chứng URL/license/ảnh chụp, ngày tải, tác giả, sửa đổi, SHA-256, đường dẫn output. Đối với AI thêm model, tool, prompt, điều khoản và bản render gốc.

### Mẫu nghe nội bộ

[Mở cuộn âm thanh WAV 16 giây](HEXCORE_AUDIO_PREVIEW.wav). Tổng hợp PCM nguyên bản ngoài Assets, seed cố định, 48 kHz/16-bit mono, peak −3 dBFS. Đây là phác thảo nhịp và phối lớp. Mốc nghe là vị trí trong file, còn đồng bộ scene dựa vào runtime.

| Mốc | Mẫu | Điều cần nghe |
| ---: | --- | --- |
| 00:00,52 | Shotgun | Một khối âm cho mỗi lần bóp cò dù nhiều pellet |
| 00:02,60 | Ám Bộ Trảm | Hút bóng, tái xuất, hai nhát tách nhau |
| 00:04,50 | Dấu Băng và đóng băng | Dấu tích nhỏ và tiếng khóa giòn dễ phân biệt |
| 00:06,68 | Thiên Vẫn Trảm | Một điểm chạm chủ lực và hai vòng dư chấn nhỏ |
| 00:09,15 | Tam Thiên Kiếm Quyết | Ba wave mở rộng và dày lên độc lập |
| 00:11,87 | Vạn Ảnh Tuyệt Sát | Hai cụm bóng và nhát chốt tách rõ |
| 00:14,65 | Tăng cấp | Motif ba nốt ngắn và dễ nhận ra |

## 5. Hướng dẫn tạo asset và prompt tiếng Anh

Dùng STT trùng bảng ở mục 3. Cột nội dung dưới đây viết riêng cho từng hành động hoặc nhóm lớp cùng trigger. Kèm mục tiêu thời lượng và biến thể của hàng đó trong yêu cầu giao file.

**Đuôi prompt chung cho SFX**

“Produce original stylized cartoon action game audio. Export isolated layers as separate dry WAV files, 48 kHz 24-bit, mono unless stereo movement or ambience is explicitly requested. No speech, music, recognizable samples, or long reverb baked into SFX. Put the main transient close to the start. For loops provide a seamless steady loop plus separate start and stop. Keep equivalent variants matched in perceived level and report layer names.” Với nhạc, yêu cầu 48 kHz/24-bit stereo, cùng độ dài và điểm bắt đầu cho mọi stem, bar alignment, BPM/key, loop sample và bản full mix. Với Foley CC0 pha lại bằng DAW, dùng prompt như bản mô tả sản xuất, duy trì nguồn gốc từng lớp.

### Nhân vật, trận đấu và tiến trình

- **1 · Bước chân đi và chạy** — Design a close dry boot sole in four surface sets: leaf litter and soil, fine mineral dust, scorched gravel, packed snow. Deliver alternating left and right transients.
- **2 · Lướt và dừng lướt** — Make a fast side-moving air rip with a soft sole skid printed separately, clean attacks and short dry tails.
- **3 · Bật nhảy và tiếp đất** — Separate a compressed boot push-off and a dirt landing thump with gritty decay. Keep landing lows tight.
- **4 · Bị thương** — Create a soft armor thud with a very brief nonverbal effort puff as an optional separate take, cartoon scale.
- **5 · Máu thấp** — Make two restrained low heartbeat-like knocks with a soft interval and no sustained pressure.
- **6 · Hồi máu thực** — Create a rounded warm pluck and airy upward sparkle, with a smaller isolated tick for repeat healing.
- **7 · Tử trận** — Make a weight drop with a short descending soft tone. Leave space after the initial hit for a defeat stinger.
- **8 · Bắt đầu đợt** — Create a two-beat hand drum and bright wood-clave start cue with a little upward anticipation.
- **9 · Hoàn thành đợt và chờ chuyển** — Create three close tuned mallet notes ending with an open resolving interval. Provide a short separate countdown tap.
- **10 · Chuyển bản đồ** — Design a soft stereo movement sweep with independent leaf, lunar dust, ember and snow grains. Keep the center clear for UI.
- **11 · Chiến thắng và Chơi lại** — Compose a short original bright brass and mallet run ending in a clean major resolution with a dry last accent.
- **12 · Thất bại và Chơi lại** — Compose a short original descending warm pluck and soft bass stinger, letting the last note settle without melodrama.
- **13 · Tạm dừng và tiếp tục** — Design matching mute and unmute taps, downward felt click and upward bright click with similar loudness.
- **14 · Cảnh báo nguy hiểm và báo Boss** — Create a clear two-part threat warning with a dry hollow knock and short nasal rising note, intelligible on tiny speakers.
- **15 · Nhận EXP từ quái bị hạ** — Make five delicate ascending glass beads with fast transients and almost no tail for grouped XP gains.
- **16 · Nhặt EXP rơi và vật phẩm** — Design separate absorb pips for XP and solid small object catches for items, neutral enough to suit multiple item types.
- **17 · Tăng cấp và mở bảng nâng cấp** — Compose an original three-note upward reward burst, felt mallet opening, bell peak and a quick filtered tail.
- **18 · Chọn nâng cấp, Sinh lực, Nhanh nhẹn, Tập trung** — Design one short choice click plus three separate accents: warm pulse for vitality, forward whisper for agility, crisp double tick for focus.
- **19 · Tiến hóa, mở lõi và thức tỉnh HEXCORE** — Compose an original three-note core motif in three sizes, with separated mechanical ratchet, sword ring and prismatic swell overlays for gun, melee and mage.

### Súng

- **20 · Phát bắn cơ bản và đạn trúng** — Make a punchy stylized single pistol shot with bright plastic-metal snap and compact bass, plus separate soft and armor impact layers.
- **21 · Loạt bắn Shotgun** — Design one chunky scattergun blast, wide noisy bloom, short wooden low body and isolated mechanical clack.
- **22 · Loạt bắn SMG** — Design light compact rapid blaster ticks, distinct first and last accents and six dry mid-shot variants.
- **23 · Phóng Rocket và vụ nổ** — Create separate toy-like launcher piston, filtered rocket propulsion loop and gritty compact explosion with a directional thump.
- **24 · Đạn nảy** — Make four small angular metallic ricochet chirps with an upward glide and very little ringing.
- **25 · Xuyên thấu** — Create a tight passing zipper with a small tactile transient and no explosion.
- **26 · Đạn bung toả** — Design a dry seed-pod snap followed by three micro ticks spreading outward, one cluster per split.
- **27 · Thêm viên, Đạn hẹp, Đẩy lùi** — Deliver independent low blast reinforcement, tight narrow hiss, and a padded knockback thud for a scattergun.
- **28 · Đạn băng, Ném lựu đạn, Thêm đạn** — Design brittle ice flecks, a pulled pin and a bouncy grenade pop as distinct assets, plus a light extra-round accent.
- **29 · Bùng nổ, Đẩy lùi, Thêm đạn** — Design an expanding debris tail, a separate soft push accent, and compact secondary launcher release.
- **30 · Lưỡi đạn càn quét** — Mix a short muzzle air crack with a wide metallic crescent sweep and a separate edge-contact transient.
- **31 · Áp lực công phá** — Create a rising pressure-valve charge, three separately triggered hot projectile barks, and dry armor fracture accents.
- **32 · Phun lửa** — Make separated ignition, a stable but textured flamethrower loop and a short valve cutoff with almost no reverb.
- **33 · Kíp nổ liên hoàn** — Design a short fuse chirp, several converging buzzy rockets and one tight clustered burst.
- **34 · Băng đạn cộng hưởng** — Design two mirrored deploy clicks, thin short laser ticks in matched pairs and a soft shutdown.
- **35 · Hồi máu** — Create a light gummy companion pop and quiet rounded healing beads, with a soft departing bubble.
- **36 · Vết rách không gian** — Design a directional tearing air rip, smoldering crack line with a stable seam loop, and brief ember collapse.
- **37 · Xuyên kích hồi quy** — Create a hollow floating electrical hum and separate dry branching zaps with varied short transients.
- **38 · Thiên thương** — Make a precise targeting chime followed by a focused descending beam crack, with a separate small contact.
- **39 · Giáp phản pháo** — Make independent golden shield fold-out, padded deflection, and short directional counterblast with a metal latch.
- **40 · Lưỡi cưa bám đuổi** — Design a whirring energy saw loop with a smooth seam, a sharp spin-up and quiet grazing teeth taps.
- **41 · Hoả lực leo thang** — Create a rising electronic overdrive latch, short plasma shots and one dry poisonous sizzle on first application.
- **42 · Đạn phân rã** — Design a compact granular breakup stamp and a sharp confirmation pip for later disintegration pulses.
- **43 · Cổng xuyên kích** — Create paired spatial portal opens, a steady violet beam loop, tiny transit pings and a clean double close.
- **44 · Mỏ neo xuyên tâm** — Make a heavy coil stake, modulated tether tension loop, sharp electric snap and separately a round timed-expiry burst.
- **45 · Lốc lửa** — Design an icy moving cyclone with dense snow hiss, layered brittle crystals and a seamless stable spin loop. Export startup and fade separately.
- **46 · Mưa thiên thạch** — Design an overhead warning, five differently pitched falling rock whistles and five crater impacts, with a deeper final hit.
- **47 · Pháo đài cơ động** — Create a mechanical turret drop, idle plasma hum, rounded energy ring pulse and compact despawn release.
- **48 · Lồng đạn Hexcore** — Create hexagonal cage nodes with a tuned lock, paired inward burst ticks, and a convergent center thump separate from the shots.

### Cận chiến

- **49 · Đánh thường Kiếm sĩ** — Create two firm one-handed blade whooshes, a horizontal cut followed by a stronger reverse cut, with separate soft impact variations.
- **50 · Đánh thường Sát thủ** — Create a nimble three-slash dagger sequence in individually exportable crisp strokes with tiny texture differences.
- **51 · Đánh thường Đao sĩ** — Design two heavy cleaver sweeps with air displacement and a matching low wooden-metal contact.
- **52 · Đánh thường Kiếm khí** — Make two short luminous blade waves with contrasting rising pitches, and a tiny crystalline contact for each.
- **53 · Phong Ảnh Trảm (Kiếm Lướt)** — Create a narrow silver sword wave with a quick air release and separate piercing contacts for a five-hit passive proc. Keep optional foot push and dash stems isolated for the older preview take.
- **54 · Hoành Nguyệt Trảm** — Design one circular metallic sword wave with broad airy rotation and a short centered attack.
- **55 · Phản Kiếm Quyết** — Create a tuned bright deflection ping followed by a dry outgoing blade slice as separate layers.
- **56 · Ám Bộ Trảm** — Design a dry shadow suck, soft reappearance, and two quick opposing slashes with distinct attacks and short tails.
- **57 · Huyết Ấn** — Design a low lacquer stamp, faint tension tick, and compact red magic pop with a muffled low end.
- **58 · Dạ Vũ Liên Trảm** — Create three distinct crossing dagger passes, alternating soft rush and sharp edge, with isolated short impacts.
- **59 · Tàng Ảnh** — Design an inward smoky hush, tiny reposition swish, and close ambush stab that emerges clearly from silence.
- **60 · Đoạt Mệnh** — Create a quick piercing dash with a tense rise and a separate hard, dry execution accent.
- **61 · Song Ảnh Truy Sát** — Make two lightweight crossing shadow slashes and a centered hollow dagger launch, all independently triggerable.
- **62 · Chấn Địa Trảm** — Design a heavy blade lift and single ground pound spreading gritty debris, with a tiny isolated stun flick.
- **63 · Hoành Thiên Đao** — Design a heavy broadening blade air fan with a short metallic body and separate knockback accent at contact.
- **64 · Cuồng Phong Đao** — Create two connected but separate spinning cleaver whooshes, one coarse and one lower with a crisp ending.
- **65 · Bá Vương Đao Vực (thay slot Bất Động Chiến Thế)** — Design a low inward terrain suction followed by one deep sweeping cleaver release and dry contact.
- **66 · Đoạn Sơn Trảm** — Create a two-step weighted buildup and a heavy blade crack with short staggered rocks moving forward.
- **67 · Bất Diệt Chiến Ý** — Make a steady stance power-up, grainy short shock pulses for enhanced attacks and quiet release.
- **68 · Nguyệt Nha Trảm** — Design a clear crescent blade launch, bright curved air hiss and tiny blunt edge hit.
- **69 · Lưỡng Nghi Kiếm** — Create two opposing pitched sword strokes and a separate harmonic cross-point burst.
- **70 · Kiếm Vũ Thiên Hà** — Design a sky-mark chime and individually placeable glass-steel sword drops for three waves of five. Keep the batch thin and readable.
- **71 · Thiên Kiếm Xuyên Tâm (thay slot Kiếm Giới)** — Create a focused lock-on ping, thin downward sword whistle and single centered piercing impact.
- **72 · Phi Kiếm Truy Hồn** — Design one dart-like launch and five clean target-hit chirps with a small ascending progression, each usable independently.
- **73 · Hồi Nguyệt Kiếm** — Create outward luminous sword flight, distinct reversed sweep, light contacts and a dry hilt catch.
- **74 · Thiên Vẫn Trảm** — Create a sole launch, steep air dive, one big dirt-and-metal crater impact, two rolling ground-ripple tails and distinct gravel fissures on separate stems.
- **75 · Tam Thiên Kiếm Quyết** — Design three individually triggered widening energy blade sweeps: narrow bright first, wider mid, final broad sustained spatial tear. Give each a separate contact.
- **76 · Vạn Ảnh Tuyệt Sát** — Design a shadow blackout, two staggered groups of four airy ghost slashes, a re-entry tick and a separate dominant finishing cut. Keep ghost voices thin.

### Pháp sư

- **77 · Đạn phép cơ bản** — Design a round beginner magic pellet with a tiny ascending air pop and clear glassy contact.
- **78 · Hồi phục** — Make a warm breath gathering into two soft healing chimes, with the hit printed separately.
- **79 · Sóng xung kích** — Create one outward ring blast with a compressed inward inhale, a dry air pop and a brief low shove.
- **80 · Vùng trọng lực** — Create an inverted gravity opening, a deep clean spinning loop with smooth seam, and short release collapse.
- **81 · Cầu lửa và thiêu đốt** — Make a compact flame launch, an airy flame travel layer, dry hot impact and low steady fire crackle.
- **82 · Sức nổ, Sức cháy, Tầm nổ** — Provide three modular overlays for fire: short punch reinforcement, warm crackle detail and wide airy outward debris.
- **83 · Cháy lâu** — Create a restrained seamless fine ember loop with smooth start and fade stems.
- **84 · Thêm cầu lửa** — Create two subtle hot projectile side launches with contrasting pitch and thinner bodies than the main fireball.
- **85 · Tập trung đốt** — Make a small dry combustible crack accent without flame tail, tuned below the parent impact.
- **86 · Nổ khi chết** — Design a compact ember corpse-pop of hot air and granular debris with a very short tail.
- **87 · Hơi thở băng, làm chậm và đóng băng** — Design icy breath, tiny ice-mark flecks, and a separate brittle lock sound when a target freezes.
- **88 · Làm chậm, Đóng băng, Sát thương lạnh** — Create isolated cold slow hush, sharp freeze crystal lock and brittle damage grain with minimal decay.
- **89 · Lạnh sâu** — Design a darker version of the existing slow hush with a gentle filtered wind downshift.
- **90 · Tụ Hàn** — Create two stackable glass beads, low first and brighter second, tuned as a small recognizable pair.
- **91 · Giòn vỡ** — Make a short sharply defined ice fracture with granular glass shards and no long crackle.
- **92 · Lan băng** — Design a tiny spreading chain of brittle ice points, starting left or right with a clear main transient.
- **93 · Cột sáng** — Create a short target bell and clean luminous pillar pulses with strong attacks, distinct enough for successive strikes.
- **94 · Thêm nhịp, Tầm cột sáng, Sức nhịp** — Design modular light overlays for an extra tick, wide air halo and stronger central ray impact.
- **95 · Dư quang** — Create a delayed soft light-pillar echo with a short distinct transient and minimal stereo wash.
- **96 · Phán Quyết** — Design a bright higher bell accent and a short powerful ray onset specifically for a third-pulse proc.
- **97 · Dẫn sáng** — Make a graceful short airy upward glide with no explosive body.
- **98 · Chói mắt** — Create a clean but softened flash flick, compact and easy to layer under a bright strike.
- **99 · Phi đạn bóng tối và lời nguyền** — Design a hollow seeking shadow projectile, muted contact and a separate dark curse imprint.
- **100 · Số mục tiêu, Sức nguyền, Suy yếu** — Deliver separate clustered curse stamp, soft dark body layer and filtered weakening whisper without voices.
- **101 · Đa ấn** — Create clustered soft shadow stamp clicks with a single clear attack and small tails.
- **102 · Dày Vò** — Make a close bass pinching accent with a dry granular bite, distinct from the curse apply.
- **103 · Suy nhược** — Design a quick downward hollow air exhale that can sit behind a curse impact.
- **104 · Di chúc** — Design a paired shadow handoff: dry vanish stamp and tiny directional arrival stamp at the new target.
- **105 · Ngưng Đọng Thời Gian** — Design a crystal clock stop, low neutral suspended tone and an elastic release impact for buffered damage.
- **106 · Thiên Tinh Giáng Lâm** — Compose a prismatic targeting chime and five waves of star falls with glassy arrivals and a full final ring. Keep each wave independent.
- **107 · Thiên Cầu Ma Lực** — Create an elastic arcane sphere charge loop with adjustable rise, spiral missile launches and separate clean contacts for two to eight bolts.

### Quái và Boss

- **108 · Lính Xanh, Nâu, Đỏ, Tuyết xuất hiện và đuổi** — Create one cartoon minion foot set with separate leaf, dust, ember and snow textures. Keep the spawn scuffle short.
- **109 · Lấy đà, chém và trúng người chơi** — Create a lightly rattling enemy windup and a short threatening cleaver whip, separately triggered. No contact baked into the swing.
- **110 · Trúng đòn và gục** — Design six short cartoon enemy hit puffs and separate weight drops with little residual ringing.
- **111 · Tăng sức theo tuổi và hiệu ứng khống chế** — Create separate tiny stun, thaw and slow-transition accents, with no continuous aging sound.
- **112 · Căng dây, nhả tên, tên bay và va** — Design a short bow draw, dry release twang, restrained arrow hiss and independent soft or missed contact.
- **113 · Hình thái cung thủ trong Guide và art** — Prepare surface accents only: leaf, lunar grit, ember dust and snow. Preserve a common bow family.
- **114 · Bước, lấy đà, chém, trúng và gục** — Design heavier minion footsteps, a low scrape warning, longer swing and weighted collapse. Include dry separated layers.
- **115 · Loạt quạt đạn và lao tới** — Design a lab boss warning, one five-bolt fan accent and a separate heavy lateral dash with turf skid.
- **116 · Xuất hiện, đổi mạng, hồi lại và bại trận** — Create original boss arrivals, phase changes and defeat tails in bone, volcanic and cold families, each clear over combat.
- **117 · Lướt áp sát** — Make dry rattling bones followed by a flat aggressive dash whoosh and a short sole stop.
- **118 · Xoay kiếm vòng tròn** — Create a full spinning rusted sword sweep with ratcheting bones and isolated bladed contact.
- **119 · Nhảy tới rồi gầm** — Design a hollow bone leap, hard landing and an original nonverbal rattly roar shockwave as separate cues.
- **120 · Phun lửa hình nón** — Design a large volcanic inhale, explosive ignition and a smooth rolling flame cone loop with cutoff.
- **121 · Hồi máu dung nham** — Make a dense lava inward swirl and two deep warm stone notes for successful boss healing.
- **122 · Ném cầu lửa, phân tách và hố dung nham** — Create a heavy molten boulder throw, separate child splits and impacts, and a seamless low lava bubble bed.
- **123 · Ném cầu tuyết** — Design compressed snow gathering, a quick cold projectile sweep, and powdery body impact.
- **124 · Cảnh báo đóng băng và nhà tù băng** — Design two distinct ice warning pips, large crystal prison formation, and separate low-shard thaw.
- **125 · Gồng và bão tuyết** — Make a deep snowy creature exertion, wide controlled blizzard loop, and fading ice-grain release.

### Main Menu, môi trường và nhạc

- **126 · Hover và focus bàn phím** — Design soft short UI rollover pips with four close variants and no clicky spikes.
- **127 · Nhấn, xác nhận và hủy** — Create a tactile press, brighter two-tone affirmative pop and a soft falling cancel swipe, individually exported.
- **128 · Bắt đầu** — Design a bold playful action-start stinger using bright pluck, short kick and an upward metallic accent.
- **129 · Chơi tiếp và trạng thái khóa** — Design a soft blocked bump and a distinct warm resume chime. Keep blocked feedback subtle.
- **130 · Chọn màn và duyệt thẻ** — Design a paper-slide UI transition with leaf, lunar glass, ember and snow accent variants.
- **131 · Hướng dẫn, chuyển tab và lọc quái** — Create a dry colorful guide page flick, a thumb-sized filter click and an open-details blip.
- **132 · Cài đặt, thanh âm lượng, mute và khôi phục** — Design five quiet slider detents, two mute toggles and a soft defaults-reset cue without any long tail.
- **133 · Thoát, cảnh báo xác nhận và quay lại** — Make a soft modal opening knock, a short retreat whoosh and a composed low exit tick.
- **134 · Forest** — Create a seamless bright woodland bed with soft leaf wind and separate sparse neutral twigs. Avoid loud wildlife calls.
- **135 · Moon** — Create a quiet fictional lunar dust ambience with hollow mineral ringing and sparse isolated grit, smoothly looped.
- **136 · Inferno** — Create a seamless distant volcanic heat bed with slow bubbling and sparse dry rock pops, much softer than combat fire.
- **137 · Tundra** — Create a seamless cold open snowfield wind bed with sparse fine ice grains, avoiding harsh hiss.
- **138 · Main Menu · Đại chiến sinh tồn** — Compose an original 144 BPM upbeat cartoon action menu loop with syncopated mallets, playful brass and crisp percussion. Deliver separate beat and melody stems.
- **139 · Chiến đấu bốn map** — Compose four original beat-aligned combat loops around a three-note rising motif using wood, glass synth, distorted toms, and cool plucks respectively. Keep midrange clear.
- **140 · Boss và cao trào** — Compose an original readable boss loop with a muscular low ostinato, discrete drum and tension stems and variations for bone, lava and ice.
- **141 · Kết thúc trận, chiến thắng/thất bại** — Compose original companion victory and defeat tails, sharing interval material and leaving room for narration-free result UI.

### Các sự kiện bổ sung

- **142 · Đứng yên, bắt đầu và ngừng di chuyển** — Create separate first-step weight shifts and small abrupt-stop shoe brushes. Leave standing idle silent.
- **143 · Hồi chiêu sẵn sàng và yêu cầu kỹ năng không hợp lệ** — Design one soft cooldown-ready chime and a low neutral rejected-input tap that stays pleasant if heard repeatedly.
- **144 · Hồi lượt Lốc lửa, giới hạn trụ và đạn hết tầm** — Design a collectible-frost recharge pip and a different mechanical turret power-down, leaving ordinary projectile timeout silent.
- **145 · Điểm môi trường và ambience khi trở lại scene** — Prepare isolated quiet woodland leaves, lunar grit, offscreen ember crackle and light snow grains with no dramatic attacks.
- **146 · Quay lại, đóng modal và ESC** — Design a light backwards UI sweep with a small low click landing, no dramatic tail.

## 6. Đồng bộ Unity với animation và gameplay

### Mốc gửi cue

| Loại cue | Vị trí đề xuất và điều kiện | Kiểm tra |
| --- | --- | --- |
| Bộ chân và trang bị | Sprite frame chân chạm, chỉ khi vận tốc thực vượt ngưỡng. Nếu clip không có mốc chân, dùng tích lũy quãng đường kèm phase qua PlayerSpriteAnimator. Chọn surface từ map active | Đứng yên, đổi hướng và va vách. Không phát vì bàn phím còn giữ |
| Tiếng vung, cast và projectile | Mốc đã tạo slash/projectile hoặc vùng hiệu lực. Prep ở trạng thái được chấp nhận. Stop loop theo chủ thể, reset, cancel hoặc ra khỏi phase. Tránh tạo nguồn on/off theo mỗi frame | Cast hụt có vung và phóng, không có hit. Cùng một cast chạy 20/60/120 FPS giữ đủ nhịp |
| Tiếng trúng và trạng thái | Sự kiện damage/HP thực giảm, hoặc API áp trạng thái trả hiệu lực mới. VFX spark có thể điều khiển layer thời trang trí, không tạo lại một hit. Đạn nảy giữ chỉ số bounce. Multi-hit dùng chỉ số beat và mục tiêu | Trượt, miễn nhiễm, chết cùng frame, chặn, đạn hủy, mục tiêu trong/ngoài AOE, gộp đám đông |
| Basic và bị động Cận chiến | Đọc mốc frame hiện hành từ catalog, PlayBasicFrame và mốc proc ở runtime. Cue preview cũ Q1 dùng các frame 3 Foot, 4 Dash, 6 Swing, 7 Impact, 9 Foot làm điểm so. Đánh thật quyết định khi phát impact | Một ID cast và nhịp theo từng form, direction, combo, passive, tập tua preview |
| Ultimate Cận chiến | Thiên Vẫn tại Land/điểm đáp, hai vòng chỉ là tail. Tam Thiên lấy ba phase Wave với range tăng thật; Burst cuối là dư âm, không tự sinh hit mới. Vạn Ảnh phát SFX bóng lệch theo hai cụm Clone; khóa mask per cụm và giữ nhát Hit chốt riêng | Kiểm thử với nhát hụt, góc chéo, 0/1/n mục tiêu, slow motion, đổi scene hoặc retry vào giữa cast |
| Ultimate Súng và Pháp sư | Meteor Súng stagger 0,22 s, impactFrame 19 ở 40 fps tính riêng mỗi quả. Pháo đài pulse theo interval 1 s hiện hành. Sao Mage 5 nhịp theo coroutine, mốc nhìn ở MageEffectPlayer. Cầu ma lực giữ loop theo 8 s ở bản thử hiện hành và thoát khi kết thúc hoặc hủy | Mốc thời gian lấy từ source chạy và serialized runtime đang dùng. Không kéo đuôi SFX để ép timing từ clip tham chiếu |
| Quái/Boss | Windup AI là đầu âm cảnh báo. Prefab lính đặt 0,35 s, Cung thủ Tuyết 0,30 s. Sau WaitForSeconds và kiểm tra khoảng cách/sống mới gọi damage/spawn. Death lấy Died, phân biệt teardown. Yeti/Golem/Xương khô chờ hợp đồng sự kiện phase và từng chiêu | Bắn địch trong lúc lấy đà, đổi mục tiêu, dọn quái khi chuyển màn. Lab Boss theo hook riêng |
| UI/wave/result | UI dựa unscaled time, dùng một đường xử lý chung cho pointer và keyboard. GameManager phát một lần khi trạng thái đổi, không gọi từ OnGUI. Duy trì stinger và menu UI qua intermission timeScale 0 | Nhấn nhanh, Back/ESC, modal, retry, chuyển scene. Wave cuối đi thẳng kết quả, giữ một stinger |
| Nhạc và ambience | Dùng hai nguồn nhạc, điểm vào theo DSP sample bằng PlayScheduled, đồng bộ stem. Ambience crossfade 0,8–1,5 s, loop ngắn gỡ tại đổi map, outro hủy khi retry | Kiểm tra seam bằng waveform và nghe hai vòng ở thiết bị, load chậm vẫn giữ nhịp và một AudioListener |

Mốc implementation đọc song song từ [MeleeSkillController](../Assets/Prefabs/Player/Melee/Scripts/MeleeSkillController.cs), [MeleeController](../Assets/Prefabs/Player/Melee/Scripts/MeleeController.cs), [GunHexcoreUltimate](../Assets/Scripts/Weapon/GunSkills/GunHexcoreUltimate.cs), [MageUltimate](../Assets/Scripts/Weapon/MageUltimate.cs), [WaveManager](../Assets/Scripts/Wave/WaveManager.cs), [GameManager](../Assets/Scripts/Core/GameManager.cs), [EnemyMeleeAttack](../Assets/Scripts/Enemy/EnemyMeleeAttack.cs), [EnemyRangedAttack](../Assets/Scripts/Enemy/EnemyRangedAttack.cs) và giá trị Inspector. Animation Event phù hợp khi sprite thật đã có mốc ổn định như chân. Với code tự chạy sprite, dùng crossing qua mốc giữa frame trước và hiện tại, chặn phát lặp theo cast/beat. Trong chế độ scrub tắt cue, chỉ nghe khi phát timeline.

### Tổ chức scene và cấu hình dự kiến

- Một prefab AudioRig cho Unity, có AudioMixer và pool AudioSource. Nếu giữ nhạc xuyên scene thì AudioRig có vòng đời rõ và chặn bản trùng khi load/retry. Một AudioListener hoạt động. Menu và từng sandbox có thể chạy độc lập khi mở trực tiếp.
- Danh mục cue dùng asset khai báo clip/lớp, gain, pitch, nhóm, cap, cooldown gộp, spatial blend và quyền ưu tiên. Gameplay báo sự kiện nhỏ tại nguồn hiện có. Lớp audio nhận event, quyết định có phát và trả voice khi kết thúc. Thanh volume lưu theo bus và áp ngay khi load, mute Music cho phép UI tiếp tục. Cài đặt mới cần quyết định về migration giá trị Menu hiện tại.
- Đề xuất mixer `Master > Music, Ambience, UI, Player, Skills, Enemies`. Bên trong Skills có nhóm Ultimate. Snapshot `Menu, Combat, Intermission, Paused, Result`. Snapshot Parry/Ultimate như nhấn tạm theo ưu tiên, hạ Music/Ambience 3–5 dB trong 200–450 ms rồi nhả. Giữ báo nguy và hụt HP nghe rõ. Tránh đóng toàn bộ AudioListener khi vẫn cần cue UI.
- Positional với quái và va chạm. Listener gần người chơi hoặc camera theo kết quả A/B, pan giới hạn để hai loa nhỏ vẫn nghe được. UI, motif progression và nhạc ở giữa. Các vụ lớn giữ một lớp trầm ở giữa, lớp không gian là phụ. Ở cụm ngoài tầm nghe chỉ giữ cảnh báo có tác dụng.
- Khởi điểm pool 24 nguồn, 2 nguồn Music, tối đa 2 ambience khi fade, 3 ưu tiên UI/threat và 1 ưu tiên ultimate. Giới hạn 6 voice quái cùng lúc, 4 hit nhẹ cùng lúc, 1 loop trạng thái mỗi cụm và 1 loop đại diện nếu có nhiều hiệu ứng cùng loại. Một loạt Shotgun/chùm nổ có một voice phân cụm. SMG có cửa sổ min 35–50 ms cho thân âm. DoT ưu tiên dấu đầu, đuôi riêng và accent thưa. Nếu đầy, thu hồi voice nhỏ/xa/cũ, nhường voice cho cảnh báo, trúng Player và kết quả. Dùng profiler để chốt ngưỡng trên build mục tiêu.
- Pitch thay đổi nhẹ khoảng ±2–5% cho Foley và hit. Không random tới mức mất motif hay nhịp đã chủ ý. Chọn biến thể tránh lặp ngay lần trước. Cue mono tần suất cao có thể preload và decompress khi số liệu hợp lý. Music/ambience dài ưu tiên streaming hoặc compressed theo profiler. WAV master 48 kHz/24-bit, export Unity theo platform, có log QC visual/ear và hồ sơ asset. Không đổi Unity hay phụ thuộc package.
- Mix đầu tiên để peak tổng giữ headroom khoảng −3 dBTP dưới ca stress, master có limiter nhẹ bắt spike thay vì ép hết transient. Tham chiếu mix chiến đấu −17 đến −14 LUFS-I và menu −20 đến −17 LUFS-I trong các đoạn dài ổn định, đo lại bằng capture thực. High-pass cho rác thấp, giữ lực sub song song lớp trung tần khi nghe loa laptop. Các số này là mục tiêu duyệt, không là giá trị hiện có. [Unity Audio Mixer](https://docs.unity3d.com/6000.0/Documentation/Manual/AudioMixer.html) · [PlayScheduled](https://docs.unity3d.com/6000.0/Documentation/ScriptReference/AudioSource.PlayScheduled.html) · [Audio Profiler](https://docs.unity3d.com/6000.0/Documentation/Manual/ProfilerAudio.html)

## 7. Triển khai sau khi duyệt và nghiệm thu

| Đợt | Phạm vi | Điều kiện qua đợt |
| --- | --- | --- |
| 0 · Duyệt hướng | Nghe 7 phác thảo, chốt motif, gain tương đối và palette từng hệ. Lập ledger nguồn | Phân biệt Súng, bóng, băng, một cú giáng, ba sóng, hai cụm trong thử nghe |
| 1 · Vòng chơi chính | Mix, loop Concept B, bắn cơ bản, bị thương/gục, quái thường, 4 ambience, wave/chuyển map và kết quả | Play Mode MainMenu và GameScene từ đầu đến retry. Volume/mute độc lập. Đúng chuột/bàn phím. Không mất cảnh báo khi dồn hit |
| 2 · Ba hệ trong sân thử | Một vertical slice mỗi hệ trước, sau đó từng nhánh, bị động và ultimate. Đọc marker frame và capture mix | Run tập hợp 24 Cận chiến, 3 + 4 + 16 + 3 nhóm kỹ năng Mage cùng bậc chỉ số, 3 + 9 + 6 + 4 nhóm nâng cấp/chiêu Súng. Xác nhận số nhịp đúng animation/cơ chế cho từng entry |
| 3 · Midgame và Boss | Cue EXP thật, level up, gear pickup nếu được tích hợp, boss pattern laboratory, cue map boss sau khi AI sẵn sàng | Xác nhận mỗi chuyển trạng thái thật, kiểm tra cuối wave và đợt chờ. Phase cũ tắt đúng khi death/retry |
| 4 · Master QA | Build đúng Unity ghi trong branch. Tối ưu voice, mixer, import và loop theo capture | Kiểm tra 20/60/120 FPS, tai nghe, loa laptop, mono. Profile 10 quái trong build hiện hành và ca quá tải sandbox, đối chiếu trước/sau. Dọn asset không dùng và ký duyệt ledger |

Tiêu chí chốt:

- Cue cast/impact lệch tối đa một frame capture 60 FPS hoặc khoảng 20 ms trong mục tiêu thử. Cue chuyển trạng thái và gameplay cùng frame logic. Cập nhật SKILL timeline chỉ sau khi xác nhận từng sai lệch. Với slow motion kiểm lại dấu frame thực thay vì đổi thời lượng clip tùy ý.
- Nghe được cảnh báo, Player hurt và cast chủ lực trong tình huống nhiều hiệu ứng. Kiểm riêng Thiên Vẫn 1 điểm giáng, Tam Thiên đúng 3 release, Vạn Ảnh hai cụm bốn bóng với một chốt và đủ nhịp khác như Ám Bộ, Phi Kiếm, 5 thiên thạch và 5 sao. Số voice bóng nghe được có thể giảm theo mix, mốc động tác vẫn khớp.
- Không có clipping nghe được, click loop, giật CPU/DSP, lệch kênh gây khó nghe, cue lặp khi tạm dừng/đổi map, AudioListener thứ hai, loop mồ côi hoặc rò âm qua bus đã mute. So gain ở đầu, giữa và cuối phiên thử 10 phút, gồm tải lại và dừng đột ngột.
- Nghiệm thu tên tiếng Việt và ledger của mọi clip import. Tune cuối dựa nghe A/B ở thiết bị, cạnh tranh bus và timing capture. Kiểm kê lần này đã đọc tĩnh source/scene/prefab/tài liệu liên quan và đo PCM. **Cần xác minh** nghe từng clip gốc, seam Music, cách đọc cue trong Game View và hiệu năng Play Mode khi có tích hợp. Mẫu nghe hiện tại được tổng hợp độc lập và đã đo thông số export.

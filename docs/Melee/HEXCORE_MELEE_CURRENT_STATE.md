# Hiện trạng hệ Cận chiến HEXCORE sau refactor

Giai đoạn 1–2 v2.1 có bộ mẫu Basic bốn form, P2, Huyết Ấn, Chấn Địa Trảm và Nguyệt Nha Trảm để duyệt phong cách. [Hình và kết quả kiểm thử mẫu](HEXCORE_VISUAL_POLISH_REPORT.md). [Đặc tả v2.1](HEXCORE_SKILL_VISUAL_POLISH_V2_1.md).

Phần phân bổ kỹ năng và mô phỏng mới nhất nằm trong [báo cáo triển khai v2](HEXCORE_SKILL_REDESIGN_V2_IMPLEMENTATION.md). Các bảng tra legacy bên dưới mô tả mã nguồn, clip và vị trí asset v1 để đối chiếu.

Snapshot trên nhánh `main`, gốc `D:\LT game\CKGame\game_brotato`, HEAD trước thay đổi `9611f09`. Đây là hệ demo độc lập cho Kiếm sĩ, Sát thủ, Đao sĩ và Kiếm khí. [Đặc tả](HEXCORE_SKILL_ASSET_DEMO_PLAN.md), [QA](HEXCORE_ANIMATION_QA.md) và [gallery](HexcoreVisual/index.html) nằm cạnh báo cáo.

## 1. Cây thư mục hiện tại

```text
Assets/Prefabs/Player/Melee/
├─ AGENTS.md
├─ Swordsman/       Frames/, Clips/, HexcorePreview/, controller, Visual.prefab
├─ Assassin/        Frames/, Clips/, HexcorePreview/, controller, Visual.prefab
├─ DaoWarrior/      Frames/, Clips/, HexcorePreview/, controller, Visual.prefab
├─ QiSwordsman/     Frames/, Clips/, HexcorePreview/, controller, Visual.prefab
├─ Shared/
│  ├─ Audio/        13 cue WAV
│  └─ VFX/          MeleeSkills/, MeleeUltimates/, QiWave/, shader/material
├─ Scripts/         MeleeController.cs, MeleeSkillController.cs
├─ Resources/       HexcoreDaoSpin/, HexcoreVisualAnchors.json
├─ Editor/          công cụ dựng form, VFX, scene, QA tương tác và camera
└─ Demo/            Capture/
docs/Melee/
├─ HEXCORE_*.md     đặc tả, kiểm kê, báo cáo hiện trạng, QA, nguồn và giấy phép
├─ Source/          sheet gốc 4 form, QiWave, VFX, tham chiếu và master VFX đã nghỉ
├─ HexcoreVisual/   gallery, CSV, GIF, video và tool xuất
├─ HexcoreInteraction/  ảnh đối chiếu UI/alpha
└─ HexcoreForm01/, HexcoreProduction/, HexcoreRemaining/, HexcoreFinal/,
   *Full/, Swordsman/, MeleeSkills/, MeleeUltimates/  nguồn, tool và bộ review theo đợt
```

Bốn form cùng cấu trúc. `Frames/` và `Clips/` phục vụ Animator bốn hướng chính. `HexcorePreview/` giữ pose theo hướng, Basic, các take kỹ năng và clip tương ứng. Các source sheet nằm trong [Source](Source), cùng metadata gốc để tra lại import. `Shared/VFX/QiWave/` có một prefab/clip/controller và bốn sprite dùng cho Basic Kiếm khí và các slot projectile/core. `Shared/VFX/MeleeSkills/` chứa tám bundle đang dùng, `Shared/VFX/MeleeUltimates/` giữ Thiên Vẫn và Vạn Ảnh. R2 dùng sóng thủ tục. Bộ PNG/clip của từng form trong `HexcorePreview/` giữ số lượng trước refactor: Kiếm sĩ 536/70, các form còn lại 760/98 mỗi form, tổng 2.816 PNG và 364 clip.

[Scene preview](../../Assets/Scenes/HexcoreSkillPreview.unity) lưu bốn prefab, mảng pose, điểm neo, VFX và cue. `GameScene.unity`, `Player.prefab`, `PlayerUnlit.mat`, cấu hình build, package và Unity version được đối chiếu byte trước/sau. Material Player chung và sprite Brown vẫn nằm ở vị trí gameplay quản lý. Preview dùng hình Brown của builder cho bia để thay tham chiếu sprite nhện cũ đã thiếu. Bia vẫn là thành phần trình diễn.

## 2. Runtime và nơi chỉnh sửa

| Ký hiệu | File | Điểm sửa |
|---|---|---|
| C | [MeleeSkillController.cs](../../Assets/Prefabs/Player/Melee/Scripts/MeleeSkillController.cs) | Catalog gồm 24 tên, form, pha, timing và đường dẫn tương đối của VFX. |
| S, F, I | [MeleeController.cs](../../Assets/Prefabs/Player/Melee/Scripts/MeleeController.cs) | Thành phần gắn scene giữ GUID cũ. Playback, Q1, điều khiển tự do, UI và vòng đời. |
| R, D, A | [MeleeSkillController.cs](../../Assets/Prefabs/Player/Melee/Scripts/MeleeSkillController.cs) | Các phần của cùng `MeleeController` xử lý form, locomotion, Basic, hướng, neo, VFX và SFX. Ký hiệu trong bảng kỹ năng quy về các region tương ứng. |
| B | [HexcoreSkillPreviewSetup.cs](../../Assets/Prefabs/Player/Melee/Editor/HexcoreSkillPreviewSetup.cs) | Dựng và gán scene, hiệu ứng, take và audio. Các file Interaction và Movies giữ kiểm thử riêng. |

`P = Assets/Prefabs/Player/Melee`, `H = P/<Form>/HexcorePreview`. Idle/Run N/S/E/W từ `P/<Form>/Clips` qua Animator. Idle/Run chéo đọc `H/IdleLoops` và `H/Locomotion`. Turn chọn frame theo góc từ `H/Turn`. Basic lấy `H/Basic`, combo 0,9 s và 12 frame hướng chính hoặc 8 frame chéo. Scene nhận WASD/mũi tên ở Tự do, Space dùng kỹ năng, R về điểm đầu. Từ Trình diễn chọn form/hướng, Basic, skill hoặc chuỗi. Công cụ có phát 1×/0,5×, VFX/SFX, hit-stop, rung, replay và tua khung trong Trình diễn. Toàn bộ 24 mục là chủ động; cơ chế buff, parry và mark ở đây là trình diễn.

Mốc và đường đi dùng trong 24 hàng bên dưới là thông số preview. `Resources.Load` giữ khóa `HexcoreVisualAnchors` và `HexcoreDaoSpin` sau khi đổi vị trí vật lý của thư mục. `AssignDirectionalTakes` đọc JSON cạnh take. Tên file và ID dùng chung với ma trận và gallery. Một phần clip là bản xuất để duyệt; runtime sample mảng sprite đã serialize.


## 3. Tra cứu vị trí sửa từng kỹ năng

Áp dụng cho mọi hàng: **tên, form và timing** sửa trong **C**; **logic và phạm vi hình ảnh** dùng hàm ở cột Logic, kiểm tra tiếp **F.SkillDisplacement** nếu sửa quãng dịch chuyển cuối cast; **SFX** sửa quy tắc trong **A** và WAV cùng tên tại [Assets/Prefabs/Player/Melee/Shared/Audio](../../Assets/Prefabs/Player/Melee/Shared/Audio). **Animation** sửa cả sprite/clip tại cột tương ứng và wiring **B** nếu thêm take. Prefix E trong Continuous là `<mã>_East_*.png`; hướng trong Directional là `<mã>_<N|NE|E|SE|S|SW|W|NW>_*.png` và clip tương ứng. Với lõi R, kiểm tra take theo từng form trước fallback. VFX trong bảng là hit/trail/aura đang được code gọi hoặc slot catalog có ghi chú. Không có một script combat hay prefab kỹ năng hoàn chỉnh riêng cho mỗi mục.

### Đường dẫn VFX và SFX

| Tên ở bảng kỹ năng | Prefab/asset đích |
|---|---|
| HoanhTao / DiaLietTram / TrongDao | [DaoWarrior](../../Assets/Prefabs/Player/Melee/Shared/VFX/MeleeSkills/DaoWarrior) → `<tên>/<tên>Visual.prefab`, `<tên>.anim` và `<tên>.controller` cùng thư mục. |
| AnTuHuyet / TamLienThu | [Assassin](../../Assets/Prefabs/Player/Melee/Shared/VFX/MeleeSkills/Assassin) → `<tên>/<tên>Visual.prefab`, clip và controller cùng thư mục. |
| PhanQuang / KiemAn / HoiNguyet | [QiSwordsman](../../Assets/Prefabs/Player/Melee/Shared/VFX/MeleeSkills/QiSwordsman) → `<tên>/<tên>Visual.prefab`, clip và controller cùng thư mục. |
| QiWave | [QiWaveVisual.prefab](../../Assets/Prefabs/Player/Melee/Shared/VFX/QiWave/QiWaveVisual.prefab), [QiWave.anim](../../Assets/Prefabs/Player/Melee/Shared/VFX/QiWave/QiWave.anim), `WaveFrames/`. |
| ThienVan | [ThienVanVisual.prefab](../../Assets/Prefabs/Player/Melee/Shared/VFX/MeleeUltimates/ThienVan/ThienVanVisual.prefab), `ThienVan.anim`, `ThienVan.controller` và frame cùng thư mục. |
| VanAnh | [VanAnhVisual.prefab](../../Assets/Prefabs/Player/Melee/Shared/VFX/MeleeUltimates/VanAnh/VanAnhVisual.prefab), `VanAnh.anim` và controller; [HexcoreGoldVfx.mat](../../Assets/Prefabs/Player/Melee/Shared/VFX/MeleeUltimates/VanAnh/HexcoreGoldVfx.mat) dùng [HexcoreGoldVfx.shader](../../Assets/Prefabs/Player/Melee/Shared/VFX/HexcoreGoldVfx.shader). |
| Trail, bụi, vòng, flash và VFX thủ tục | S/R/D dựng bằng giá trị tại thời điểm sample. R gọi ConfigureEffect với hiệu ứng sprite prefab ở các pha tương ứng. |
| WAV | `Foot.wav`, `Dash.wav`, `Swing.wav`, `HeavySwing.wav`, `Impact.wav`, `Parry.wav`, `Charge.wav`, `Wave.wav`, `Vanish.wav`, `Return.wav`, `Slam.wav`, `Burst.wav`, `Finisher.wav` trong thư mục audio ở trên. `A` kiểm soát gain; capture mix qua [HexcorePreviewMixCapture.cs](../../Assets/Prefabs/Player/Melee/Demo/Capture/HexcorePreviewMixCapture.cs). |

### Ma trận chỉnh sửa và cơ chế đang chạy

| ID và kỹ năng | Form | Logic và điểm sửa phạm vi | Animation | VFX | SFX theo thứ tự pha có cue | Cự ly, mục tiêu và hành động | Trạng thái theo tổ hợp |
|---|---|---|---|---|---|---|---|
| 01 Q1 · Kiếm Lướt / Phong Ảnh Trảm | Kiếm sĩ | `Q1; S.DashOffset, S.Render` | Q1 → `H/Q1/` | Trail/bụi/impact theo frame do S dựng | Foot ×2, Dash, Swing, Impact | Lao 2,05 unit theo hướng đánh; chạm ở frame 7, hãm sau đó. | 1 In Review (E), 7 AuthoredBlockout |
| 02 Q2 · Hoành Nguyệt Trảm | Kiếm sĩ | `Q2; R.DrawBeat, R.ConfigureEffect` | E: `H/Continuous/`; 7 hướng còn lại: `H/SpinAtlas/` | HoanhTao; vòng chém từ pose xoay | Swing, Impact | Một vòng 360° quanh người; root tại chỗ. | 8 AuthoredBlockout |
| 03 Q3 · Phản Kiếm Quyết | Kiếm sĩ | `Q3; R.RenderOther, R.DrawBeat` | E: `H/Continuous/`; còn lại: `H/Directional/` | Guard và phản kiếm thủ tục | Parry nếu đỡ trúng; Swing, Impact khi phản | Giữ vị trí; tùy chọn mô phỏng đỡ trúng. Không có hitbox. | 8 AuthoredBlockout |
| 04 E1 · Ám Bộ Trảm | Sát thủ | `AssassinE1; R.RenderOther, R.Travel` | `H/Continuous/` E; `H/Directional/` hướng khác | Fade thân, bụi chân và dấu chạm hai nhát | Vanish, Return, Swing ×2, Impact ×2 | Dịch 1,96 unit và đổi hướng thân để chém ngược sau khi xuất hiện. | 8 AuthoredBlockout |
| 05 E2 · Huyết Ấn | Sát thủ | `AssassinE2; R.DrawBeat, D.StageContactPoint` | `H/Continuous/` E; `H/Directional/` hướng khác | AnTuHuyet; vòng dấu, phát sáng và nổ | Swing, Charge, Burst | Mark neo tại bia; dao động bán kính vòng 0,14 ± 0,04 unit. | 8 AuthoredBlockout |
| 06 E3 · Tàng Ảnh | Sát thủ | `AssassinE3; R.Travel, R.DrawBeat` | `H/Continuous/` E; `H/Directional/` hướng khác | Fade thân, bụi khi ẩn/lao và dấu chạm phục kích | Vanish, Dash, Return, Swing, Impact | Hai đoạn tiến 1,20 + 1,20 unit trên hướng ngắm. | 8 AuthoredBlockout |
| 07 E1 · Chấn Địa Trảm | Đao sĩ | `DaoE1; R.DrawBeat, D.DrawImpactSignature` | `H/Continuous/` E; `H/Directional/` hướng khác | DiaLietTram, đường nứt và xung kích điểm chạm | Charge, HeavySwing, Slam | Bổ tại chỗ; hiệu ứng ở điểm chạm đất. | 8 AuthoredBlockout |
| 08 E2 · Hoành Thiên Đao | Đao sĩ | `DaoE2; R.DrawBeat, D.DrawHeavyCleave` | `H/Continuous/` E; `H/Directional/` hướng khác | HoanhTao là slot catalog; cung chém/impact ở nhánh thủ tục | Charge, HeavySwing, Impact | Cung nặng ngang từ chuôi; root tại chỗ. | 8 AuthoredBlockout |
| 09 E3 · Bất Động Chiến Thế | Đao sĩ | `DaoE3; R.DrawBeat, R.ConfigureEffect` | `H/Continuous/` E; `H/Directional/` hướng khác | TrongDao; aura tự thân, hai nhịp vung | Parry, Charge, HeavySwing ×2, Impact | Không buff thật; tâm aura đi theo thân. | 8 AuthoredBlockout |
| 10 E1 · Nguyệt Nha Trảm | Kiếm khí | `QiE1; R.DrawBeat, R.ConfigureEffect` | `H/Continuous/` E; `H/Directional/` hướng khác | QiWave; bán nguyệt rời mũi kiếm | Swing, Wave, Impact | Sóng đi thêm 1,80 unit từ mũi kiếm. | 8 AuthoredBlockout |
| 11 E2 · Lưỡng Nghi Kiếm | Kiếm khí | `QiE2; R.DrawBeat, D.DrawImpactSignature` | `H/Continuous/` E; `H/Directional/` hướng khác | PhanQuang và hai nét giao nhau ở Burst | Swing ×2, Wave, Impact, Burst | Wave 1,80 unit; dấu giao nhằm vào điểm va chạm. | 8 AuthoredBlockout |
| 12 E3 · Kiếm Giới | Kiếm khí | `QiE3; R.DrawBeat, R.ConfigureEffect` | `H/Continuous/` E; `H/Directional/` hướng khác | KiemAn; mark/aura và hai lần phát trận | Swing, Charge, Burst ×2 | Đặt ấn ở sân khấu mục tiêu; không duy trì vùng gameplay. | 8 AuthoredBlockout |
| 13 R1 · Thiên Vẫn Trảm | Cả 4 form | `R1; R.DrawBeat, D.DrawLeap, D.DrawMeteorImpact` | `H/Directional/`; kiểm tra take `H/Slam/` và `H/HeroWest/` cho E/W | ThienVan, ground_slam, nứt đất tỏa nhiều hướng và hai vòng chấn động | Foot, Swing hoặc HeavySwing, Slam | Tiến 0,72 unit, đáp xuống gây sát thương trong bán kính 3,4 unit. | 32 AuthoredBlockout |
| 14 R2 · Tam Thiên Kiếm Quyết | Cả 4 form | `R2; R.DrawBeat, D.DrawCoreWave` | `H/Directional/`; `H/Hero/`, `H/HeroWest/` cho pose dự phòng | Lưỡi kiếm khí hai lớp và vệt kéo dài về sau | Charge ×2, Swing hoặc HeavySwing ×3, Wave ×3, Burst | Ba sóng đi 3,6 / 4,95 / 6,3 unit với bề rộng 1,72 / 2,56 / 3,4 unit. | 32 AuthoredBlockout |
| 15 R3 · Vạn Ảnh Tuyệt Sát | Cả 4 form | `R3; R.DrawBeat, D.DrawShadowCrossing` | `H/Directional/`; `H/Hero/`, `H/HeroWest/` cho pose dự phòng | VanAnh + HexcoreGoldVfx, bốn bóng mỗi lượt, vệt cắt chéo và nhát kết | Vanish, Return, Swing hoặc HeavySwing, Finisher | Hai lượt cắt dài 6,7 unit mỗi đường, nhát kết bán kính 2,55 unit. Root tiến tổng 1,28 unit. | 32 AuthoredBlockout |
| 16 F1 · Dạ Vũ Liên Trảm | Sát thủ | `AssassinF1; R.Travel, R.AssassinSlashPath` | `H/Continuous/` E; `H/Directional/` hướng khác | Ba vệt lao/chém thủ tục | Dash ×3, Swing ×3, Impact ×3 | Ba đích (trước, ngang) (1,95; +0,40), (1,23; −0,52), (1,73; +0,16) unit. | 8 AuthoredBlockout |
| 17 F2 · Đoạt Mệnh | Sát thủ | `AssassinF2; R.Travel, R.DrawBeat` | `H/Continuous/` E; `H/Directional/` hướng khác | AnTuHuyet; vệt xuyên và nổ cuối | Dash, Charge, Swing, Finisher, Burst | Áp sát 1,20 unit rồi nhấn tại điểm xuyên. | 8 AuthoredBlockout |
| 18 F3 · Song Ảnh Truy Sát | Sát thủ | `AssassinF3; R.DrawShadows, R.DrawBeat` | `H/Continuous/` E; `H/Directional/` hướng khác | TamLienThu ở impact; bóng lấy frame theo form hiện tại | Swing, Impact | Hai lượt bóng lệch làn ±0,36 unit; thân chính giữ root. | 8 AuthoredBlockout |
| 19 F1 · Cuồng Phong Đao | Đao sĩ | `DaoF1; D.DrawCyclone, R.DrawBeat` | `H/Continuous/` E; `H/Directional/` hướng khác; `Assets/Prefabs/Player/Melee/Resources/HexcoreDaoSpin/` bổ sung | HoanhTao là slot catalog; hai vòng lốc dựng thủ tục | Charge, HeavySwing ×2, Impact ×2 | Hai nhịp xoay quanh root, khác Q2 một vòng. | 8 AuthoredBlockout |
| 20 F2 · Đoạn Sơn Trảm | Đao sĩ | `DaoF2; R.DrawBeat, D.DrawImpactSignature` | `H/Continuous/` E; `H/Directional/` hướng khác | DiaLietTram; nhấn bổ rồi nứt/nổ | Charge ×2, HeavySwing, Slam, Burst | Hai nhịp vận lực; bổ tại chỗ rồi mở hiệu ứng tại điểm chạm. | 8 AuthoredBlockout |
| 21 F3 · Bất Diệt Chiến Ý | Đao sĩ | `DaoF3; R.DrawBeat, R.ConfigureEffect` | `H/Continuous/` E; `H/Directional/` hướng khác | TrongDao; aura và hai xung kích | Parry, Charge, HeavySwing ×2, Impact ×2 | Hai nhát có điểm chạm trong một chu kỳ aura; không có buff gameplay. | 8 AuthoredBlockout |
| 22 F1 · Kiếm Vũ Thiên Hà | Kiếm khí | `QiF1; R.DrawBeat` | `H/Continuous/` E; `H/Directional/` hướng khác | KiemAn là slot catalog; Rain dựng đường kiếm rơi | Charge, Swing, Wave ×2, Impact | Hai loạt rơi lệch ngang ±0,42 unit; cao ban đầu 1,40 unit. | 8 AuthoredBlockout |
| 23 F2 · Phi Kiếm Truy Hồn | Kiếm khí | `QiF2; R.DrawBeat, R.ConfigureEffect` | `H/Continuous/` E; `H/Directional/` hướng khác | QiWave; projectile theo đường cong tới bia | Swing, Wave, Impact | Lệch ngang sin(πu) × 0,38 unit khi bám bia; không có AI/homing gameplay. | 8 AuthoredBlockout |
| 24 F3 · Hồi Nguyệt Kiếm | Kiếm khí | `QiF3; R.DrawBeat, R.ConfigureEffect` | `H/Continuous/` E; `H/Directional/` hướng khác | HoiNguyet; sóng ra, quay lại và đón bằng kiếm | Swing, Wave, Parry | Đi 1,80 unit rồi thu về theo nhịp Recall; không sinh projectile chiến đấu. | 8 AuthoredBlockout |

### Thời gian thi triển

Giây ở tốc độ 1× của timeline, trước hit-stop; đọc từ C, ngoại lệ runtime ghi sau bảng. Hàng theo ID ở trên. Tốc độ 0,5× thay đổi thời gian phát; timeline không phải dữ liệu cooldown.

| ID | Pha và thời lượng (s) | Tổng (s) |
|---|---|---:|
| 01 | 12 frame × 0,075 s (xem dưới) | 0,9 |
| 02 | Prepare 0,24 → Swing 0,6 → Hit 0,18 → Recover 0,3 | 1,32 |
| 03 | Prepare 0,22 → Guard 0,33 → Swing 0,14 → Hit 0,15 → Recover 0,32 | 1,16 khi đỡ trúng / 0,8 khi không có va chạm |
| 04 | Prepare 0,17 → Vanish 0,24 → Return 0,15 → Swing 0,13 → Hit 0,12 → Swing 0,11 → Hit 0,14 → Recover 0,25 | 1,31 |
| 05 | Prepare 0,29 → Swing 0,16 → Mark 0,27 → Charge 0,36 → Burst 0,19 → Recover 0,26 | 1,53 |
| 06 | Prepare 0,22 → Vanish 0,26 → Advance 0,29 → Return 0,2 → Swing 0,15 → Hit 0,15 → Recover 0,28 | 1,55 |
| 07 | Prepare 0,28 → Charge 0,26 → Swing 0,15 → Land 0,24 → Recover 0,35 | 1,28 |
| 08 | Prepare 0,25 → Charge 0,29 → Swing 0,21 → Hit 0,19 → Recover 0,36 | 1,3 |
| 09 | Prepare 0,23 → Guard 0,24 → Aura 0,35 → Swing 0,19 → Hit 0,16 → Swing 0,23 → Recover 0,33 | 1,73 |
| 10 | Prepare 0,23 → Swing 0,17 → Wave 0,32 → Hit 0,14 → Recover 0,28 | 1,14 |
| 11 | Prepare 0,22 → Swing 0,15 → Wave 0,24 → Swing 0,17 → Hit 0,14 → Burst 0,25 → Recover 0,3 | 1,47 |
| 12 | Prepare 0,23 → Swing 0,22 → Mark 0,23 → Aura 0,31 → Burst 0,21 → Burst 0,23 → Recover 0,33 | 1,76 |
| 13 | Prepare 0,23 → Rise 0,28 → Dive 0,19 → Swing 0,22 → Land 0,24 → Recover 0,39 | 1,55 |
| 14 | Prepare 0,19 → Charge 0,32 → Swing 0,14 → Wave 0,23 → Swing 0,17 → Wave 0,23 → Charge 0,23 → Swing 0,2 → Wave 0,27 → Burst 0,2 → Recover 0,34 | 2,52 |
| 15 | Prepare 0,24 → Vanish 0,23 → Clone 0,24 → Clone 0,2 → Return 0,23 → Swing 0,15 → Hit 0,2 → Recover 0,32 | 1,81 |
| 16 | Prepare 0,19 → Advance 0,23 → Swing 0,115 → Hit 0,115 → Advance 0,22 → Swing 0,115 → Hit 0,115 → Advance 0,23 → Swing 0,13 → Hit 0,17 → Recover 0,28 | 1,91 |
| 17 | Prepare 0,19 → Advance 0,27 → Charge 0,18 → Swing 0,13 → Hit 0,13 → Burst 0,21 → Recover 0,26 | 1,37 |
| 18 | Prepare 0,21 → Clone 0,29 → Swing 0,19 → Clone 0,27 → Hit 0,18 → Recover 0,3 | 1,44 |
| 19 | Prepare 0,23 → Charge 0,22 → Swing 0,48 → Hit 0,14 → Swing 0,55 → Hit 0,17 → Recover 0,34 | 2,13 |
| 20 | Prepare 0,27 → Charge 0,24 → Charge 0,26 → Swing 0,15 → Land 0,19 → Burst 0,2 → Recover 0,37 | 1,68 |
| 21 | Prepare 0,24 → Guard 0,23 → Aura 0,28 → Swing 0,18 → Hit 0,14 → Swing 0,2 → Hit 0,17 → Recover 0,35 | 1,79 |
| 22 | Prepare 0,23 → Charge 0,31 → Swing 0,17 → Rain 0,24 → Rain 0,24 → Hit 0,2 → Recover 0,32 | 1,71 |
| 23 | Prepare 0,22 → Swing 0,17 → Chase 0,38 → Hit 0,18 → Recover 0,29 | 1,24 |
| 24 | Prepare 0,25 → Swing 0,15 → Wave 0,3 → Recall 0,33 → Catch 0,18 → Recover 0,27 | 1,48 |

Q1 chạy riêng trong S: frame zero-based 0–2 Anticipation, 3 FootPush, 4–5 Dash, 6 Sword Swing, 7 Impact, 8–9 Braking, 10–11 Recovery. Motion bắt đầu tại frame 3, đạt cuối ở frame 9; hit-stop 0,045 s tại frame 7. Q3 không va chạm kết thúc theo `Starts[2] + 0,25` s, rút hai pha phản đòn. Khi chỉnh timing, kiểm tra thêm `R.ContinuousBeats`, các mốc body/neo trong D, `D.ImpactPause`, SFX và phần dịch chuyển riêng của F.

## 4. Các nhóm đã di chuyển, gộp và dọn

| Nguồn hoặc nhóm | Kết quả |
|---|---|
| `Assets/Prefabs/Player/{Swordsman,Assassin,DaoWarrior,QiSwordsman}` | Chuyển bốn thư mục và toàn bộ metadata bên cạnh sang `Melee/`. Sheet gốc đưa sang `docs/Melee/Source/<Form>/Sheets`. |
| `MeleeSkills`, `MeleeUltimates`, QiWave và shader CleanVfx/HexcoreGoldVfx | Chuyển sang `Melee/Shared/VFX`, cập nhật builder, giữ một bản mỗi bundle. Mười sheet VFX hiện hành và source QiWave chuyển sang `docs/Melee/Source`; tool export đọc tại vị trí mới. |
| `Assets/Audio/HexcorePreview`, `Assets/Resources/HexcoreDaoSpin`, `HexcoreVisualAnchors.json` | Chuyển sang `Melee/Shared/Audio` và `Melee/Resources`. Xóa hai thư mục nhập cũ đã rỗng sau khi kiểm tra GUID. |
| `Assets/Scenes/HexcoreSkillPreview.unity`, các builder Kiếm sĩ/Sát thủ/Đao sĩ/Kiếm khí, builder hiệu ứng và ba phần của `HexcoreSkillPreviewSetup` | Đợt refactor chuyển chín công cụ sang `Melee/Editor`. Scene hiện ở `Assets/Scenes/HexcoreSkillPreview.unity`, giữ nguyên GUID scene và component. |
| `HexcoreSkillPreview.cs`, `Roster`, `Audio`, `Directional`, `FreePlay`, `Interface`, `HexcorePreviewCatalog.cs` | Gộp vào hai file runtime tại `Melee/Scripts`. Giữ `.meta` của component chính và file Roster qua đổi tên. Bỏ năm bản nguồn cũ cùng metadata phụ không có tham chiếu. `HexcorePreviewMixCapture.cs` dùng riêng khi thu QA, nằm trong `Melee/Demo/Capture`. |
| Ba VFX Stage1, Anh Hồi Kích, Tam Thiên VFX đời cũ, BarrierShield, GoldVfxOnly | Bỏ bundle Unity, shader/material và GIF chỉ dùng cho demo đời cũ sau khi rà GUID, catalog và builder. Năm sheet gốc giữ trong `Source/RetiredVfx` để bảo toàn nguồn. |
| `MeleeDemoCombat`, `MeleeSkillDemo`, `MeleeDemoSetup`, `MeleeDemoVerification`, `docs/player-art/MeleeDemo` | Bỏ bốn script và 17 screenshot của scene đời cũ vốn đã vắng trong checkout. |
| `docs/HEXCORE_*.md`, bộ review Cận chiến trong `docs/player-art`, hai ảnh tham chiếu | Chuyển sang `docs/Melee`; giữ GIF trước/sau và CSV dùng cho gallery. Giữ một bản ảnh tham chiếu sau khi so SHA-256 với bản import trùng. Data Player chung và dữ liệu Pháp sư còn tại thư mục ban đầu. |
| `Library/HexcoreValidation` | Xóa bản sao kiểm thử cũ. Không sử dụng để chỉnh sửa hay chạy QA trong đợt này. |

[Danh sách từng file](HEXCORE_REFACTOR_FILES.txt) ghi 10.699 đường dẫn chuyển, bảy nguồn gộp và 186 mục dọn (tính cả metadata và clone là một mục).

Giữ `Signature*`, `Hero*`, các atlas Q1/Q2, frame chưa được chọn trực tiếp nhưng builder vẫn đọc, nguồn sản xuất và bộ review có tham chiếu. Có thể kiểm tra danh sách từng tệp bằng `git status --short --untracked-files=all`; Git sẽ biểu diễn nhiều mục đã chuyển dưới dạng xoá ở nguồn và mới ở đích khi chưa stage.

## 5. Kiểm thử và giới hạn

| Kiểm tra | Kết quả |
|---|---|
| Roslyn, project chính trước / sau | `Assembly-CSharp` 47 → 40 file, `Assembly-CSharp-Editor` 20 → 18 file, cả hai đều exit 0 ở hai mốc. Dùng Roslyn và assembly dependency trong cache của Editor 6000.3.25f1. |
| GUID, prefab, Animator và scene | Đối chiếu 3.377 GUID theo chuỗi tham chiếu. Chỉ thay GUID sprite bia thiếu bằng GUID sprite Brown hợp lệ. Các GUID Unity nội bộ và shader URP nằm ngoài `Assets` là tham chiếu hợp lệ. Scene serialize giữ nguyên ngoài tên lớp và sprite bia. |
| Catalog, đường dẫn và QA dữ liệu | 24 định nghĩa cùng 33 cặp form–skill và 264 hàng hướng. VFX catalog phân giải được theo builder mới. 13 WAV và 16 pose Cyclone còn đủ. 263 `AuthoredBlockout`, 1 `In Review`, 0 `Approved` theo CSV hiện hành. |
| Unity Editor và Play Mode trước / sau | Cả hai lần gọi `VerifyExisting` trên project chính bị khóa project. `ProjectVersion.txt` yêu cầu 6000.3.22f1, chưa được cài. CLI không kết nối được dịch vụ release để tìm bản đó. Chưa xác minh hình ảnh/âm thanh trực tiếp sau refactor. |
| Gameplay và cấu hình chung | SHA-256 của GameScene, Player.prefab, PlayerUnlit.mat, ProjectVersion, EditorBuildSettings, manifest và packages-lock trùng trước/sau. `Packages/packages-lock.json` đã có sửa từ trước đợt này. |

### Quy định kiểm thử Unity hiện hành

Kiểm thử trên project chính bằng Editor đã có nếu tương thích và có thể chạy mà vẫn giữ nguyên `ProjectVersion.txt`, package và cấu hình project. Nếu project bị khóa hoặc Editor không tương thích, dừng bước chạy Unity và ghi rõ giới hạn. Giữ nguyên khóa đang hoạt động. Không tự nâng hoặc hạ phiên bản Unity của project, đổi package, chỉnh cấu hình để chạy thử hay tạo bản sao project.

Lần kiểm tra chỉ đọc ngày 26/11/2026 xác nhận project khai báo Unity `6000.3.22f1`. Các Editor hiện có là `6000.3.25f1` và `6000.6.0f1`. Thử mở `Temp/UnityLockfile` ở chế độ chỉ đọc, độc quyền báo file đang do tiến trình khác sử dụng. Play Mode chưa được chạy lại trong lần cập nhật này.

Khi project chính rảnh và có Editor tương thích, chạy `HexcoreSkillPreviewSetup.VerifyExisting` và `HexcoreSkillPreviewSetup.VerifyInteraction` bằng `-executeMethod` hoặc mở scene trong Unity. Chọn 4 form, 8 hướng và so lại các đoạn từ [gallery](HexcoreVisual/index.html), ưu tiên replay, bia, Q1 và ba lõi R rồi kiểm tra 24 kỹ năng. Các MP4/GIF/CSV hiện có được ghi từ các đợt trước và dùng làm mẫu so sánh.

Quy tắc làm việc tiếp tục nằm ở [AGENTS.md](../../Assets/Prefabs/Player/Melee/AGENTS.md). Đợt này chỉ thay tổ chức file, liên kết asset và dọn bản cũ. Các timing, trạng thái artwork và cơ chế kỹ năng giữ như dữ liệu gốc.

# Quy định làm việc với hệ Cận chiến HEXCORE

Đọc [hướng dẫn project](../../../../AGENTS.md), [hiện trạng](../../../../docs/Melee/HEXCORE_MELEE_CURRENT_STATE.md), [đặc tả](../../../../docs/Melee/HEXCORE_SKILL_ASSET_DEMO_PLAN.md) và [QA](../../../../docs/Melee/HEXCORE_ANIMATION_QA.md) trước khi sửa.

## Vị trí làm việc

- Làm việc và kiểm thử trên project chính. Chạy một Editor trên mỗi project tại một thời điểm.
- `Swordsman/`, `Assassin/`, `DaoWarrior/`, `QiSwordsman/` dùng cùng cấu trúc `Frames/`, `Clips/`, `HexcorePreview/`, Visual prefab và Animator Controller. Đặt pose hoặc clip riêng của từng form cạnh form đó.
- `Shared/Audio` và `Shared/VFX` là một nguồn cho cue, prefab và material dùng lại giữa các kỹ năng. Tài nguyên gameplay dùng chung ngoài thư mục này tiếp tục do hệ gốc quản lý.
- `Scripts/MeleeController.cs` quản lý vòng đời scene, playback và input. `Scripts/MeleeSkillController.cs` chứa catalog và các phần runtime về form, pose, VFX, SFX. Công cụ dựng/kiểm thử ở `Editor/`; component dùng riêng cho camera/audio capture ở `Demo/Capture`.
- `Resources/` chứa dữ liệu tải động. Bảo toàn khóa `HexcoreDaoSpin` và `HexcoreVisualAnchors` khi di chuyển. Đặt sheet gốc, prompt, license, báo cáo, QA và bản review bên ngoài Assets tại `docs/Melee/`.

## Điều kiện kiểm thử Unity

- Dùng Editor đã có nếu tương thích với phiên bản ghi trong `ProjectSettings/ProjectVersion.txt` và có thể mở project mà vẫn giữ nguyên file này, package và cấu hình project.
- Nếu project đang bị khóa hoặc Editor hiện có không tương thích, dừng bước chạy Unity và ghi rõ giới hạn vào báo cáo hiện trạng. Giữ nguyên khóa đang hoạt động. Không tự nâng hoặc hạ phiên bản Unity của project, đổi package, chỉnh cấu hình để chạy thử hay tạo bản sao project.

## Trước và sau thay đổi

- Kiểm tra Git, `.meta`, GUID và `fileID` của scene, prefab, Animator, clip, sprite và material. Di chuyển asset cùng `.meta`; kiểm tra thêm `AssetDatabase`, `Resources.Load`, công cụ xuất và file nguồn dùng để dựng lại.
- Giữ thứ tự catalog 24 kỹ năng, quy tắc form–skill, hướng N/NE/E/SE/S/SW/W/NW và dữ liệu serialize. Nếu phải đổi hằng hoặc thứ tự, cập nhật builder, dữ liệu neo, CSV và gallery trong cùng thay đổi.
- Chỉ dọn asset đã kiểm tra tham chiếu serialize, tải động, builder và vai trò nguồn/review. Bảo toàn asset fallback còn được đọc. Bản review trước/sau có liên kết trong gallery là đầu vào của quy trình duyệt.
- Mở `Assets/Scenes/HexcoreSkillPreview.unity` trong project chính. Dùng `HexcoreSkillPreviewSetup.VerifyExisting` và `HexcoreSkillPreviewSetup.VerifyInteraction` cho kiểm thử khi Unity rảnh. Menu Create hoặc `BuildAndVerify` dựng lại scene và có thể ghi asset, nên kiểm tra Git trước khi dùng.
- Với catalog P/E/R/F hiện tại, xác minh 4 form, 24 kỹ năng, 336 tổ hợp kỹ năng và 32 tổ hợp Basic. Kiểm tra chuỗi Basic/Skill, replay, VFX/SFX và điều khiển tám hướng. Phân biệt kết quả compile, Play Mode, hình ảnh và nghe âm thanh. Ghi giới hạn thực tế. Trạng thái Approved cần người thiết kế xác nhận. [Đặc tả Rev2](../../../../docs/Melee/HEXCORE_SKILL_VISUAL_POLISH_V2_1.md)
- Giữ thay đổi ngoài phạm vi của người dùng. Cập nhật báo cáo hiện trạng và QA tại `docs/Melee/` khi thay đổi đường dẫn hay quy trình. Dùng `Library/`, `Temp/` và `Logs/` cho đầu ra tạm, không dùng chúng làm nguồn sản xuất.

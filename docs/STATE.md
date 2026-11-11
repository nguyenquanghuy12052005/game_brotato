# Current project state

## Sân thử hệ Súng - 2026-11-10

- Cấp 11 chạy trong GunSkillTest với chín kỹ năng và sprite lấy từ bộ art đã duyệt. Shift+1/2/3 chọn kỹ năng theo nhánh, Shift+7 bật một bia boss 1.500 HP miễn đẩy lùi. Play Mode headless trên bản sao Unity 6000.3.22f1 đã xác nhận đủ chín animation đổi frame và tạo sát thương trên bia, gồm hai lượt của Xuyên kích hồi quy. Chưa xác nhận phím vật lý và hình ảnh trong Game View.

- Art cấp 11 đã chuẩn bị trong `public/gun/level-11/`. Có 9 sprite sheet kỹ năng và 2 sheet lặp riêng cho vòng quỹ đạo và đạn vệ tinh. Mỗi sheet có 32 ô RGBA 256 × 256. Đã kiểm tra kích thước, mép ô, bản xem trước và SHA-256 sau khi chép. Cần kiểm tra trong Unity khi nhập asset và nối gameplay.

- Dư chấn dùng sprite xanh ngọc nhập từ public/gun/piercing-shockwave.png. Sóng lan và mờ dần trong 0,38 giây. Play Mode tự động trong bản sao Unity xác nhận sprite được gắn đúng, xuất hiện ở mục tiêu thứ hai, nở ra, giảm alpha và được dọn sau khi phát. Chưa xem Game View.

- Giảm tốc độ bắn ba nhánh hai lần, mỗi lần còn một nửa. Đạn tỏa cách 2,60 giây mỗi phát, Liên thanh cách 0,32 giây mỗi viên và nghỉ 1,20 giây sau loạt, Xuyên thấu cách 0,80 giây mỗi phát.

- Thêm chín nâng cấp bậc 0–3 theo ba nhánh. Phím 7–9 tăng bậc, Shift+8 đổi tự bắn, Shift+9 đổi máu và Shift+0 bật bia di chuyển. Play Mode trên bản sao Unity 6000.3.22f1 xác nhận số đạn, góc, đẩy lùi, số lần nảy và mảnh, sát thương xuyên, giảm tốc và hồi tốc, bán kính và giới hạn một Dư chấn. Bài hồi quy va chạm cho các hiệu ứng cũ cũng PASS. Cần xem Game View để đánh giá độ rõ của vòng Dư chấn.

- Súng mặc định tự bắn ngay khi vào Play. Sau khi hạ bia cuối, cả dàn bia hồi sinh sau 0,75 giây mà không khởi động lại nhịp bắn. Kiểm tra Play Mode trong bản sao Unity 6000.3.22f1 đã xác nhận tự bắn, không sinh sớm khi còn bia và hồi sinh đủ 8 bia trong hai lượt liên tiếp.

- Bảng sân thử dùng phím số 0–9 để chọn kỹ năng, chọn nhánh, bật nâng cấp, tự bắn, đổi máu bia và đặt lại sân. C# biên dịch qua Roslyn. Phím số chưa được kiểm tra trực tiếp trong Play Mode.

- Thêm `Assets/Scenes/GunSkillTest.unity` và ba script dưới `Assets/Scripts/Weapon/GunSkills/`. Scene thử dùng lại prefab Player và Enemy, dựng bia đứng yên, cho chọn kỹ năng cấp 4 và nhánh riêng với công tắc nâng cấp. Xem `docs/HUONG_DAN_TEST_SUNG.md`.
- Nhập sáu PNG hiện có từ `public/gun` vào `Assets/Art/GunSkills/`. Đạn xuyên dùng alpha có sẵn và hiệu ứng Ngắm điểm yếu phát bốn frame lửa ở 12 FPS.
- C# biên dịch bằng Roslyn với Unity 6000.3.22f1 và thư viện hiện có của dự án. Đã kiểm tra các GUID mới trong scene có đúng một asset tương ứng, và sáu PNG khớp SHA-256 với public. Play Mode tự động trong bản sao Unity 6000.3.22f1 đã PASS: nảy sang bia sống gần nhất, sinh 3 đạn phụ khi hạ bia, frame lửa chạy, Đạn tỏa 3/4, Liên thanh 5/7 và Xuyên thấu nâng cấp gây 10/13/16 qua ba bia. Chưa xem hình ảnh trong Game View.

## Player artwork installed - 2026-11-09

- User follow-up authorized completing Unity setup directly. Applied and saved the existing Player prefab: Idle_Down_00 sprite, PlayerCharacter Animator controller, PlayerSpriteAnimator on PlayerVisual, centered/unit visual transform, root motion off, normal update mode/always animate, old PlayerController.playerVisual rotation reference cleared. GameScene inherits the updated prefab; existing scene Player/tag/camera/weapon/manager references remain intact.
- Added Tools > Brotato > Apply Player Artwork as a separate explicit setup command in PlayerSpritePreparation. Prepare Player Sprites still only generates artwork assets. Unity's prefab save also dropped obsolete serialized respawnForTesting/respawnDelay fields, which are absent from current PlayerHealth code; no HP/respawn behavior was changed.
- Checks actually executed in Unity 6000.3.25f1: script compiled; setup command completed; entered real GameScene Play Mode and observed the new character, Idle_Down_02 frame progression, independently aimed gun and spawned bullets. Natural lethal enemy contact reached Game Over with Death_Down_03 and Animator Unscaled Time, confirming the death clip reached its final frame during combat freeze. Clicked the actual PLAY AGAIN button and observed a fresh standing character and firing. Stopped Play Mode afterward; prefab/setup remained saved.
- Needs verification: sustained directional Run/Idle playback and intermission animation freeze were not conclusively observed in this short manual run; no full-duration balance test, new automated regression or Windows build. Existing unrelated changes preserved; no commit/push.

## Player sprite preparation - 2026-11-09

- Request: cut balanced frames for each action and provide step-by-step Unity setup. Completed asset preparation; prefab/scene installation is deliberately manual for the user's learning workflow.
- Added `Assets/Prefabs/Player/CharacterArt/`: transparent extraction sheet, 52 individual PNG frames (Idle 4 / Run 5 / Death 4 per direction Down/Left/Right/Up), 12 AnimationClips and PlayerCharacter.controller. All Unity-generated .meta files accompany the assets.
- Used built-in imagegen for background extraction; prompt saved in `docs/player-art/extraction-prompt.txt`. Original source/imported source remain intact. AI extraction is not guaranteed pixel-identical to the original artwork.
- Editor preparation identifies 52 separate strong-alpha components, retains each component and its nearby soft outline instead of copying neighboring poses inside overlapping bounding rectangles, then centers each pose horizontally on a transparent 192x224 canvas. No per-frame resizing. PPU120, Center pivot, Point/no compression; strong-alpha ground is y34 in all 52 outputs, with no opaque pixel touching a canvas edge.
- Added `Assets/Editor/PlayerSpritePreparation.cs`, menu Tools > Brotato > Prepare Player Sprites. Generates/validates frames, clips, controller, bounds CSV and contact sheet; never edits Player prefab/GameScene. Rerunning overwrites this generated set's imagery/clip curves, preserving their GUIDs; do not rerun over later hand edits without intent.
- Added optional `Assets/Scripts/Player/PlayerSpriteAnimator.cs` for PlayerVisual. Reads current WASD for visual direction, plays named Idle/Run/Death states, subscribes to parent PlayerHealth.Died, freezes normal clips with scaled time and uses unscaled animation only for terminal Death. Existing PlayerController/PlayerHealth/GameManager/WeaponAim were not modified.
- User wiring: assign Idle_Down_00 + controller + Animator + PlayerSpriteAnimator to existing PlayerVisual; clear PlayerController.playerVisual to None so the old Atan2 rotation is skipped; keep Player root physics and scene-added WeaponHolder. Detailed Vietnamese guide: `docs/HUONG_DAN_PLAYER_ANIMATION.md`.
- Checks actually executed: Unity 6000.3.25f1 imported/compiled both scripts; ran preparation in the existing Editor via computer-use; Unity logged PLAYER SPRITE ASSET VALIDATION PASSED and 52 frames / 12 clips / 1 controller. Validator checked all clip sprite references and 12 states. Console showed 0 errors and 2 editor automation/account API warnings, not gameplay errors. Preview inspected and neighbor-bleed defect corrected before final output. Read-only pixel checks confirmed 52 PNGs at 192x224, common strong-alpha ground y34 and zero opaque edge pixels; 52 corresponding PNG .meta files verified.
- Needs verification: user wiring, actual Idle/Run/Death playback in Play Mode, intermission freeze/death-at-timeScale-zero, aim/fire and retry after wiring. No new Play Mode regression or build was executed. Generated assets alone do not change current scene visuals.
- Preserved pre-existing EditorBuildSettings and Assets/public changes. Unity's incidental ShaderGraphSettings line-ending rewrite was restored to original checkout formatting with identical content. No commit/push.

## Asset guidance checkpoint - 2026-11-09

- Request: explain how to replace the Player artwork using `public/player` and see it in GameScene. This checkpoint provides instructions only; no runtime code, imports, prefab or scene changes were applied.
- Inspected current code, Player prefab, GameScene prefab-instance overrides and texture import metadata. Scene Player inherits PlayerVisual's sprite; WeaponHolder is still a scene-added child of Player. Preserve that instance and its references when replacing artwork.
- Source `public/player/ChatGPT Image 19_25_43 25 thg 9, 2026.png` and the already-imported `Assets/public/player` copy have identical SHA256 hashes. Image is 1448x1086, RGB, every pixel alpha 255: the purple checkerboard is painted background, not transparency. Background removal requires a separately prepared transparent PNG; importer transparency settings alone cannot remove it.
- Imported sheet uses Multiple mode but has only one full-sheet rectangle, PPU 100 and Bilinear filtering. Guide recommends manually defining one upright frame first, Point filtering, no compression, and tuning PPU for roughly 1.3-1.6 world-unit height before any animation work.
- PlayerController.UpdateFacingDirection currently rotates PlayerVisual with Atan2. Guide proposes replacing that visual rotation with horizontal SpriteRenderer flipping for a right-facing upright frame; no code change or animation implementation was performed.
- Saved camera orthographic size is 5; PlayerVisual sorting order is 0 and GunVisual order is 1. Player root scale is 1; keep root physics/weapon scale intact while tuning artwork.
- Current ProjectVersion.txt specifies Unity 6000.3.25f1, superseding the older 6000.3.22f1 notes below for this checkout. No editor/package upgrade was performed.
- Checks: read-only source/serialized inspection, image preview, pixel alpha inspection and source/imported-copy hash comparison. Needs verification: compilation after any user-applied code change, actual sprite import, sizing, facing and shooting in Play Mode. No new compilation, Play Mode run or build was performed.
- Preserved pre-existing `ProjectSettings/EditorBuildSettings.asset` status and untracked `Assets/public/` plus `Assets/public.meta`; no commit/push.

Session checkpoint: 2026-09-18. Git root: D:/lap_trinh_game/cuoi_ky/game. Current HEAD: 60c4f25 (làm wawe quái và máu nhân vật); working tree was clean immediately before this documentation update. Refresh status every session.

## Goal and phase

Stabilize the university project's 3-wave MVP before adding Week 4/5 features. The reported difficulty, missing spawner reference, instant transitions, duplicate-hit risk and missing camera-follow implementation have now been addressed. Unity Play Mode regression passed; full-duration balance playtest and Windows build remain unverified.

## Completed today

- Fixed the requested combat/difficulty issues and saved GameScene/Bullet prefab; exact gameplay settings are recorded below.
- Added and ran reusable Editor-only combat regression in real Unity Play Mode.
- Updated original plan section 24 and project memory to reflect the fixes, tests and remaining verification.
- Reviewed progress with the user: Week 3 features are implemented; most Week 4 wave behavior exists, but WaveData ScriptableObjects do not.
- Explained WaveData in Vietnamese: separate per-wave configuration assets read by WaveManager; existing inline WaveSettings already contain those values, so migration changes data organization rather than adding new gameplay.
- Explained PLAY AGAIN: drawn by GameManager.OnGUI in Game View after Game Over or final Victory; no Button object exists in Hierarchy/Canvas. Intermediate Wave Complete has no retry button.
- This checkpoint edits documentation only; no implementation, scene or prefab changes.

## Currently in progress

- MVP validation/handoff: accelerated regression has passed, but full-duration playtest and Windows-build verification remain pending.
- User is learning the next Week 4 task and the current prototype UI. No WaveData implementation is underway or authorized by the explanation/checkpoint requests.
- No additional feature is being implemented. Do not treat discussion of Week 5 as authorization to add upgrades.

## Implemented and saved

- GameScene remains the gameplay/build scene. Existing normalized WASD, mouse aim and continuous auto-fire are preserved.
- WaveManager owns a serialized WaveSettings array, editable directly in Inspector (not ScriptableObjects yet).
- Wave 1: 30 seconds, cap 10 living enemies, 10 HP each, spawn interval 1.5 seconds, speed 2 units/s.
- Wave 2: 30 seconds, cap 15, 20 HP, interval 1 second, speed 2.5 units/s.
- Wave 3: 45 seconds, cap 30, 30 HP, interval 0.6 seconds, speed 3 units/s.
- Counts are simultaneous living caps, NOT total enemies for a wave. Death frees a slot; spawning continues until time expires.
- Explicit WaveManager -> EnemySpawner and other manager references saved in GameScene.
- Wave 1/2 completion freezes combat and shows WAVE COMPLETE for 2 real seconds; then the next wave starts automatically.
- Old enemies and bullets are deactivated/destroyed at transitions and terminal outcomes. Player HP and position persist across waves; no free healing.
- Final completion uses Victory state / ALL WAVES COMPLETE. Death uses Game Over. Both allow PLAY AGAIN.
- Enemy initialization sets absolute HP/speed, including the placed scene enemy; no multiplier switch or compounded scaling.
- Bullet guards its first accepted hit before damage/Destroy. Circle trigger remains enabled; redundant Box trigger disabled in saved prefab, retained for recovery.
- CameraFollow on Main Camera tracks Player XY in LateUpdate, preserving camera Z. Spawning uses outside-camera positions, margin 1 and minimum Player distance 3.
- Player movement is intentionally unbounded for now: camera follows, no invented wall/map rectangle.

## Checks actually executed

- Runtime scripts compiled with Roslyn against project Unity assemblies: exit 0; Inspector-field CS0649 warnings only.
- Unity Editor imported/compiled changes and saved scene/prefab via Tools > Brotato > Apply Combat Setup.
- Ran Tools > Brotato > Run Combat Regression in actual Unity Play Mode: ALL CHECKS PASSED.
- Confirmed serialized references, all 3 durations/intervals, living caps 10/15/30, HP/speed on spawned enemies, spawn positions outside camera and replacement after a kill.
- Real Physics2D overlap with BOTH bullet colliders temporarily enabled did exactly 10 damage; explicitly repeating callback also did one hit only. Enemy HP clamps to zero.
- Camera followed a displaced Player and retained Z; Player hit cooldown blocked a consecutive hit.
- Timer expiry -> 2-second intermission -> next wave; cleanup/freeze, preserved/protected HP, final Victory, lethal HP zero -> Game Over.
- Restart method used by PLAY AGAIN successfully reloaded scene after both results, resetting wave/timer/HP/timeScale and controls.
- No gameplay runtime errors during the regression; final Console showed zero errors/warnings.
- Reusable Editor-only test: Assets/Editor/CombatRegression.cs. Local detailed result: Temp/CombatRegression.txt (generated/ignored, not source).

## Verification limits / next steps

1. Play an ordinary full-duration 30/30/45-second round to assess difficulty, moving/aiming feel and camera-follow feel. Automated tests shorten timers and force spawn scheduling to exercise caps; they are not a human balance playtest.
2. Build/test Windows executable; not done in this session.
3. Arena boundaries are still unspecified. Current choice is free movement + following camera; don't invent map limits without design.
4. When requested, migrate inline WaveSettings to reusable WaveData ScriptableObjects. Keep current values/transition policy unless user changes them.
5. Only then proceed to Kill Count / gun upgrades and further content at user's request.

Concrete continuation for the next session:

1. Open GameScene in Unity 6000.3.22f1; play normal 30/30/45-second waves while moving and aiming. Assess spawn pressure, damage cooldown and camera behavior; record actual observations.
2. Exercise the visible PLAY AGAIN button after Game Over and final completion. Earlier regression invoked its restart method; it did not click the rendered button.
3. Build a Windows player with GameScene enabled and verify launch, gameplay and retry outside the Editor.
4. When the user asks to begin WaveData, explain and implement a small ScriptableObject with duration, maxEnemiesAlive, enemyHealth, spawnInterval and enemySpeed. Create Wave1/Wave2/Wave3 assets using the current values; wire the list into WaveManager and pass the selected data to EnemySpawner. Preserve the two-second cleanup transition and one-life HP policy. Regression must still pass.
5. Once Week 4 is validated, request-directed Week 5 work is Kill Count, WeaponLevelData, WeaponUpgradeManager and weapon-level HUD; keep 3 waves until explicitly expanded.

## Known bugs/issues and uncertainty

- Previously confirmed multiplier-case bug is fixed; the earlier instant-transition/missing-reference/duplicate-hit concerns are addressed. No additional gameplay bug was confirmed by this checkpoint.
- WaveManager assumes a nonempty waves array with valid entries; clearing the Inspector list or null entries can cause initialization errors. Current saved scene has three valid entries. Validation hardening is not implemented.
- Full-duration balance, moving/aiming feel and Windows build are still Needs verification. Automated spawn/timer acceleration does not establish human-play difficulty.
- Arena dimensions/walls remain unspecified; camera follows free Player movement. This is the current prototype policy, not a confirmed bug.
- Prototype UI is OnGUI, not Canvas: PLAY AGAIN exists only after terminal outcomes and is not discoverable as a Hierarchy Button.
- Applying CombatSetup again resets wave tuning to this request's defaults; use deliberately.
- Original plan section 24 contains an explicitly marked historical pre-fix log. Its old bug list and all-30-second values are superseded by the later update.

## Important paths and setup

- Assets/Scenes/GameScene.unity: main scene, saved wave values and camera/spawner wiring.
- Assets/Scripts/Core/{GameManager,CameraFollow}.cs: game state/UI/retry and camera.
- Assets/Scripts/Wave/WaveManager.cs: timer, inline settings, unscaled intermission.
- Assets/Scripts/Enemy/{EnemySpawner,EnemyController,EnemyHealth,EnemyContactDamage}.cs: spawning, absolute stats, pursuit, contact.
- Assets/Scripts/Player/{PlayerController,PlayerHealth}.cs and Weapon/{WeaponAim,Bullet}.cs: controls/combat.
- Assets/Editor/CombatSetup.cs: one-time setup helper. Reapplying it resets wave tuning to this request's defaults; not needed on every launch.
- Player weapon hierarchy remains scene-added, not fully inside Player prefab.
- OnGUI is prototype UI; no finished Canvas/main menu/pause/items/upgrades yet.

## Important files changed earlier today

- Assets/Scripts/Enemy/EnemySpawner.cs: per-wave absolute stats, active-living cap, spawn distance and cleanup.
- Assets/Scripts/Enemy/EnemyController.cs and EnemyHealth.cs: absolute speed/HP initialization and exposed test/HUD properties.
- Assets/Scripts/Wave/WaveManager.cs: inline WaveSettings, 30/30/45 timers and unscaled intermission.
- Assets/Scripts/Core/GameManager.cs: intermediate/final states, cleanup, freeze/resume and result/retry UI.
- Assets/Scripts/Core/CameraFollow.cs (+ .meta): new camera target tracking.
- Assets/Scripts/Player/PlayerHealth.cs: damage protection while combat is stopped.
- Assets/Scripts/Weapon/Bullet.cs and Assets/Prefabs/Weapon/Bullet.prefab: one-hit guard and disabled redundant Box trigger.
- Assets/Scenes/GameScene.unity: explicit references, camera component/target and saved wave tuning.
- Assets/Editor/CombatSetup.cs and CombatRegression.cs (+ generated .meta): scene/prefab setup and reusable Play Mode checks.
- AGENTS.md, docs/STATE.md, DECISIONS.md, ARCHITECTURE.md, GAME_DESIGN.md and ke_hoach_do_an_game_2d_brotato_like.md: updated rules, handoff and dated progress. Older prefab/build-setting/weapon changes were preserved rather than attributed to this checkpoint.

Checkpoint checks: reread project guidance/memory, inspected current WaveManager and retry UI code, refreshed HEAD/status, and confirmed the existing Temp/CombatRegression.txt ends with ALL CHECKS PASSED. No new compile, Play Mode run or Windows build was performed for this documentation-only checkpoint. Other memory files already reflect current decisions/architecture/design and need no further edits.

## Git handoff

Earlier implementation preserved pre-existing changes and did not itself commit/push. At checkpoint, git status was clean and HEAD had advanced to 60c4f25; earlier work is now represented in the local repository. This checkpoint modifies only docs/STATE.md and does not commit/push. Remote push status was not checked. Include source/.meta/docs when sharing; exclude Library, Temp and Logs.

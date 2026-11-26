# Actual architecture

`MageAim.CastPoint` uses the Staff transform and a serialized local sprite-tip offset, so projectile origins follow staff rotation and scale. `MageCombat` uses this point for Fire, Dark and Ice casting; `MageUltimate` uses it for Arcane missiles. Projectile sprite pivots align their visible rear with the origin, and `MageProjectile` checks collisions immediately on spawn as well as during movement.

`Magical.unity` uses `MageDemoHUD` as its training control surface. It references the existing Enemy prefab, the `Preview Enemies` container and Mage transform to rebuild stationary, non-damaging targets at runtime. `EnemyController` accepts a training-dummy flag and adds gravity velocity supplied by `MageEnemyStatus`; the same status component animates one progressive Ice Mark overlay. `MageEffectPlayer` can follow `MageAim` and animate visual scale for Ice Breath and Shockwave/Orb presentation. The training setup is confined to the Magical scene; `GameScene.unity` retains its wave loop.

## Separate Mage preview (2026-11-11)

`Magical.unity` is a self-contained skill preview. Its Mage prefab uses the existing `PlayerController`/`PlayerHealth` with `MageVisual`, `MageAim`, `MageLoadout`, `MageCombat`, `MageUltimate` and `MageEffectLibrary`. `MageLoadout` stores one Q skill, one element, one E ultimate and optional tier/core settings; its values can be changed in Inspector or by the Play Mode test panel. It does not manage EXP or progression. `MageAim` positions the staff at a serialized hand offset, rotates it toward the mouse, and exposes a cast point near its tip. `MageEffectLibrary` cuts the imported PNG sheets into sprites at runtime and `MageEffectPlayer` plays them with a display-scale multiplier; `MageProjectile` moves animated shots. `MageEnemyStatus` holds burn, curse, ice marks, slow, freeze, dazzle and deferred TimeStop damage, attached when a Mage hit reaches an enemy. `MageBossMarker` marks the three existing boss prefabs so control effects can skip them. `MageDemoHUD` provides status, temporary build selection, invulnerability and scene reset for tests. The scene's existing `CameraFollow` targets Mage with orthographic size 4.5. `MagePreviewSetup` is Editor-only and assembles the prefab/scene or applies presentation values without resetting the loadout; `MageRegression` runs a transient Play Mode integration check. No Mage component is wired into GameScene.

Updated 2026-09-18. Unity 6000.3.22f1, URP 2D, existing Input System. Main enabled build scene: Assets/Scenes/GameScene.unity.

## Scene and components

### HEXCORE Cận chiến

Rev2 giữ nguồn pose cũ và đổi slot trong catalog demo. Cùng hai file runtime tiếp tục quản lý pool sprite, bbox chém, tiến trình bay của projectile và các cờ hit theo nhịp/lượt. Tick kéo bia và hit kết thúc của Bá Vương là hai pha độc lập. [Đối chiếu Rev2](Melee/HEXCORE_VISUAL_POLISH_REPORT.md)

Các mẫu v2.1 tải một atlas pixel và JSON timing vào pool renderer của MeleeController. P2 quét dải giữa hai bán kính liên tiếp, Chấn Địa dùng tâm đáp, còn Basic Kiếm khí và Nguyệt Nha quét theo biên cong của bán nguyệt. Outline debug dùng hình học va chạm của bản demo. [Phạm vi và bằng chứng](Melee/HEXCORE_VISUAL_POLISH_REPORT.md)

Trong demo v2, ID nguồn vẫn xác định clip, cue và asset serialize. Lớp preview ánh xạ các ID sang P/E/R/F, giữ lựa chọn từng nhóm và xử lý các mốc đòn thường, nhận đòn, vùng tác động và HP của bia mô phỏng. Ba lõi R dùng timeline nguồn. [Mapping và kiểm thử v2](Melee/HEXCORE_SKILL_REDESIGN_V2_IMPLEMENTATION.md)

Scene nằm tại Assets/Scenes/HexcoreSkillPreview.unity. MeleeController điều phối input, playback và UI. MeleeSkillController.cs chứa catalog 24 kỹ năng và phần thực thi của cùng controller. Một timeline sample Animator hoặc sprite đã gán, đồng bộ pose, điểm neo, VFX, SFX, hit-stop và camera. Tự do dùng Input System và giữ vị trí sau chiêu. Trình diễn hỗ trợ chọn riêng hướng chạy/hướng đánh và xem chuỗi Basic.

Form, pose tám hướng, audio, clip, VFX và Resources riêng nằm trong Melee/. Preview chia sẻ hình bia Brown và material PlayerUnlit với tài nguyên ngoài thư mục. Gameplay chính không phụ thuộc vào code preview. Công cụ Editor VerifyExisting và VerifyInteraction mở scene đã lưu. Tool Movies lấy camera và âm thanh từ playback. [Hiện trạng và vị trí sửa](Melee/HEXCORE_MELEE_CURRENT_STATE.md) | [Gallery](Melee/HexcoreVisual/index.html)

GameScene contains Main Camera + CameraFollow, Player + PlayerHealth, scene-added WeaponHolder/GunVisual/FirePoint, Enemies with one placed enemy, EnemySpawner, WaveManager and GameManager. Manager references and camera target are explicitly saved. Weapon setup is not fully part of the standalone Player prefab.

- PlayerController: normalized keyboard WASD in Update; Rigidbody2D movement in FixedUpdate. Saved playerVisual reference is now None to skip the old Atan2 rotation; PlayerSpriteAnimator on PlayerVisual handles upright directional animation.
- PlayerHealth: initializes HP; positive-damage/dead/cooldown checks; HP clamp; single Died event. SetDamageEnabled prevents hits during intermission/results.
- WeaponAim: mouse world-position aim and automatic shots on a Time.time interval. Requires Camera.main, Bullet and FirePoint.
- Bullet: Rigidbody2D movement/lifetime; GetComponentInParent<EnemyHealth> on trigger; consumed flag before damage; deactivate immediately and Destroy. Saved prefab only enables the Circle trigger.
- EnemyController: explicit Player target (tag fallback for standalone use), pursuit, absolute SetMoveSpeed.
- EnemyHealth: InitializeHealth sets absolute max/current HP; TakeDamage ignores nonpositive damage/dead enemies and clamps at zero; death destroys.
- EnemyContactDamage: enter/stay trigger calls PlayerHealth.TakeDamage(10); Player owns shared 0.75-second immunity.
- EnemySpawner: accepts WaveSettings, schedules spawn with Time.time, counts active living EnemyHealth under Enemies, respects cap, assigns target and absolute stats. Initializes placed enemies too. Four-side camera-exterior positions, margin and minimum Player distance. ClearEnemies deactivates before delayed destruction.
- WaveSettings: serializable inline data (duration, maxEnemiesAlive, enemyHealth, spawnInterval, enemySpeed), owned by WaveManager; no ScriptableObject asset.
- WaveManager: configures wave, decrements scaled timer while Playing, decrements unscaled 2-second intermission while WaveComplete, starts next wave once.
- GameManager: Playing/WaveComplete/GameOver/Victory, health death subscription, cleanup, combat enable/disable, prototype OnGUI and retry.
- CameraFollow: Start/LateUpdate copies Player XY and keeps original camera Z; no map clamp or smoothing.

## State transitions

Playing + timer zero:
- Wave 1/2 -> WaveComplete -> freeze/cleanup -> wait 2 unscaled seconds -> configure next wave -> Playing.
- Wave 3 -> Victory -> freeze/cleanup -> PLAY AGAIN reload.
Playing + HP zero -> GameOver -> freeze/cleanup -> PLAY AGAIN reload.

Freeze disables spawner, PlayerController and WeaponAim, blocks Player damage and sets timeScale=0. Resume restores previous movement/weapon enable states, enables spawner/damage and restores timeScale=1. OnDestroy and retry also restore timeScale. Player HP/position persist through intermediate waves. Old bullets/enemies do not persist.

## Current configuration

Wave 1: 30s / cap10 / HP10 / spawn1.5s / speed2.
Wave 2: 30s / cap15 / HP20 / spawn1s / speed2.5.
Wave 3: 45s / cap30 / HP30 / spawn0.6s / speed3.
Player HP100, speed5, contact10, cooldown0.75s. Bullet damage10, speed10, lifetime3s; gun interval0.2s.

## Editor utilities and tests

- MeleeSkillAssetBuilder imports 96 stage1/stage2 VFX sprites and generates twelve controllers/clips/visual prefabs under Assets/Prefabs/Player/MeleeSkills. Each eight-frame preview is 0.64s; orbit blades loop. Passive condition logic and gameplay attachment remain pending.

- MeleeUltimateAssetBuilder imports 48 ultimate VFX frames under Assets/Prefabs/Player/MeleeUltimates at PPU48 and builds three one-shot clips/controllers/visual prefabs. Independent preview artwork for fire landing/sweep, charged cone and five-apparition star; combat integration is pending.

- QiSwordsmanAssetBuilder imports the qi swordsman visual package under Assets/Prefabs/Player/QiSwordsman: 80 directional character PNGs at PPU120, 16 character clips, controller and visual prefab. A separate four-frame right-facing QiWave has its own clip/controller/prefab. Wave clip clears its sprite after 0.36 seconds. Attack uses seven frames over 0.78 seconds, with suggested wave release at frame03/time0.31. Projectile movement and combat remain future integration work.
- DaoWarriorAssetBuilder imports the separate heavy curved-saber package under Assets/Prefabs/Player/DaoWarrior: 84 PNGs at PPU120, 16 named directional clips, controller and visual prefab. Uses a 320x320 canvas to accommodate the weapon while maintaining approximately 160-pixel body height. Attack has eight frames and a 1.10-second weighted windup/chop/recovery cycle. Preview assets are independent of gameplay combat integration.
- AssassinAssetBuilder imports the separate dagger assassin package under Assets/Prefabs/Player/Assassin: 72 PNGs at PPU120, 16 directional clips, Assassin.controller and AssassinVisual prefab. Per direction Idle4 / Run5 / Attack5 / Death4. Attack uses a 0.48-second nonuniform stab/retract cycle. The visual prefab previews artwork through SpriteRenderer/Animator; melee gameplay integration is a later task.
- SwordsmanAssetBuilder imports the separate swordsman visual package under Assets/Prefabs/Player/Swordsman: 76 PNGs at PPU120, 16 named directional clips, controller and SwordsmanVisual prefab. Idle/Run loop; Attack/Death are one-shot clips. Attack uses nonuniform 0.74-second timing. The prefab contains only the renderer/Animator needed to preview this artwork. Its animation state names follow the existing directional convention, with Attack added for future melee integration.
- PlayerSpritePreparation (added 2026-11-09): Tools > Brotato > Prepare Player Sprites. Generates 52 centered/ground-aligned 192x224 PNGs at PPU120, 12 Idle/Run/Death clips and PlayerCharacter.controller from the transparent extraction sheet in Assets/Prefabs/Player/CharacterArt. Separates neighboring alpha components even where bounding boxes overlap. Validates sprite references/state counts; creates preview/bounds report under docs/player-art. Does not modify scene/prefab; rerunning rewrites this generated set, so preserve later manual artwork edits before using it.
- PlayerSpriteAnimator on PlayerVisual (installed in saved Player prefab, inherited by GameScene): obtains parent PlayerController/PlayerHealth; reads existing WASD keys for visual state only and directly plays the 12 named Animator states. Idle/Run loop with scaled time; one-shot Death uses UnscaledTime after PlayerHealth.Died because GameManager immediately freezes combat. No transitions/parameters required. Old PlayerController.playerVisual rotation reference is cleared; movement/physics remain in PlayerController. Tools > Brotato > Apply Player Artwork explicitly reapplies visual wiring to the existing prefab/scene without replacing Player or its scene-added WeaponHolder. See docs/HUONG_DAN_PLAYER_ANIMATION.md.
- CombatSetup: Tools > Brotato > Apply Combat Setup, only in edit-mode GameScene. Uses SerializedObject/PrefabUtility and saves scene/prefab. Resets wave settings to defaults; avoid invoking after later custom tuning unless intended.
- CombatRegression: Tools > Brotato > Run Combat Regression, edit mode. Enters real Play Mode, tests physics/camera/stats/caps/transitions/retries, writes Temp/CombatRegression.txt and exits Play Mode. Test changes never save to scene; shortened timers and forced spawn clock are deliberate test-only acceleration.
- Helpers are under Assets/Editor and excluded from player builds.

## Constraints and remaining coupling

GameManager and WaveManager require valid saved references and a nonempty settings array. No dependency-injection framework. Enemies should remain parented under Enemies; ClearEnemies owns that subtree. Camera must be orthographic; no authored arena bounds yet. Global timeScale is owned by GameManager; a future pause system must coordinate with it. MainCamera tag is required for aiming. Player root prefab is Untagged, scene overrides Player tag; spawner explicitly assigns target.

No save/load, inventory, pickups, kill count, gun upgrades, enemy variants, finished UI/menu/pause, or Windows-build verification yet. See STATE for actual tests and limits.

## Sân thử hệ Súng

`GunSkillTest.unity` có `GunSkillTestArena` dựng camera, Player, súng và các bia khi vào Play. Nó dùng lại `WeaponAim` để ngắm chuột. `GunSkillWeapon` phát đạn theo nhánh, còn `GunSkillProjectile` xử lý va chạm với `EnemyHealth` và các hiệu ứng cấp 4. Scene thử nằm ngoài Build Profiles và có hướng dẫn tại [HUONG_DAN_TEST_SUNG.md](HUONG_DAN_TEST_SUNG.md).

Sân thử lưu ba bậc nâng cấp riêng theo từng nhánh. Mỗi viên đạn giữ cấu hình hiệu ứng của phát bắn. GunSkillTargetEffects áp dụng đẩy lùi, giảm tốc và màu báo trạng thái lên bia trong sân.

GunCoreTwo nhận lượt bắn và va chạm từ GunSkillWeapon/GunSkillProjectile để chạy song song với GunLevel11. GunLabBoss dùng các đòn thử riêng trong GunSkillTest, với máu người chơi và đặt lại sân do GunSkillTestArena quản lý.

GunHexcoreUltimate phụ trách bốn chiêu cuối trong sân thử. GunSkillTestArena chuyển lựa chọn và điểm ngắm cuối từ bảng điều khiển. Chiêu cuối dùng mục tiêu của sân thử. Mỗi trụ năng lượng vận hành độc lập, dùng atlas Energy Core và Plasma Explosion trong GunNewSkillArt, đồng thời gọi sát thương vùng trực tiếp theo nhịp. Lốc lửa cũng gây sát thương vùng theo nhịp và dùng GunSkillTargetEffects để kéo quái về tâm trong lúc di chuyển.

GunNewSkillArt dùng chung tám sheet public/new_asset được nhập vào Assets/Art/GunSkills/UpdatedSkills. Nó cắt ô đúng kích thước, giữ pivot tiếp xúc và lưu cache sprite cho GunLevel11, GunCoreTwo và GunHexcoreUltimate. Khiên và đạn phản pháo có renderer và luồng tạo đạn riêng.


## Gun pixel VFX layers (2026-11-18)

GunSkillTestArena.pixelSkillSheets supplies 16 pixel atlases appended after the eight existing imported sheets in GunNewSkillArt. PixelSkills/manifest.json records the source cells, top-left pivots and PPU; cached Sprite.Create frames use those values. GunLevel11 handles sweep/fuse and the orbiting lightning ball, GunCoreTwo handles saw/muzzle/heavy/decay and Tesla anchors, and GunHexcoreUltimate handles separate cage field/pulse/collapse visuals. Existing hit and cooldown logic owns the timing; review montage sheets are retained only under public/new_asset/pixel-skills-v2. No changes to main GameScene architecture.


GunSkillTestArena.skillNoGlowMaterial supplies a dedicated Sprite-compatible alpha-filter material via GunNewSkillArt.NoGlowMaterial. GunCoreTwo uses it for the portals and Tesla tether, GunLevel11 for the lightning ball's cropped impact sparks, and GunHexcoreUltimate for the blizzard vortex. Ball lightning draws a branched discharge with LineRenderers on each damage tick; Tesla keeps its continuous atlas tether. NoGlowSprite clips source alpha below 0.25 before renderer tint/fade, with premultiplied sprite blending; other effects keep their existing materials.

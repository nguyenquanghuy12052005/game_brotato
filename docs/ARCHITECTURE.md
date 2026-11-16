# Actual architecture

Updated 2026-09-18. Unity 6000.3.22f1, URP 2D, existing Input System. Main enabled build scene: Assets/Scenes/GameScene.unity.

## Scene and components

Assets/Scenes/MeleeDemo.unity is a standalone visual gallery with MeleeSkillDemo and an orthographic camera. Its serialized 15 entries reference existing stage1/stage2/ultimate visual prefabs directly. Runtime sidebar selects one instance, uses its original Animator and offers replay/pause/repeat/zoom. Camera viewport reserves header/control space and keeps uniform sprite scale. MeleeDemoSetup creates/wires this scene; MeleeDemoVerification exercises all selections in actual Play Mode.

MeleeDemoCombat owns a replaceable encounter containing the correct branch actor, original selected VFX prefab and four SpriteRenderers sourced from Enemy artwork. A shared preview clock samples original animation states and scripted movement/reactions, permitting pause/replay without affecting global timeScale. Ultimate actor selection is shared across three ultimate entries. This presentation timeline is independent of GameScene combat systems.

Selected sweep/wave/fan frames are serialized for continuous transform motion instead of generated-sheet jumps. VanAnh uses GoldVfxOnly material to suppress the source field while retaining gold traces, with invisible positional checks for demo reactions. UI clears areas outside the world camera and scales with Game View height.

Demo WASD control uses existing Input System, takes over scripted actor position on first input, clamps to the visible arena and resets on replay. BarrierShield renders on a runtime full-rectangle sprite derived from existing artwork texture, follows actor center and receives the preview clock via MaterialPropertyBlock so pause freezes its pulse.

CleanVfx material filters neutral background haze from the other Demo effects while retaining chromatic details, adjacent white highlights and opaque dark metal/stone. BarrierShield and GoldVfxOnly retain their dedicated rendering paths. Source PNGs and original prefab artwork remain available.

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

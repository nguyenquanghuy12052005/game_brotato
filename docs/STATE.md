# Current project state

## All-skill background cleanup audit — 2026-11-13

- Added CleanVfx shader/material to Demo effects except the dedicated surrounding barrier and gold-only VanAnh materials. Suppresses low-saturation background haze, keeps colored cuts/particles and nearby white highlights, and retains dark opaque steel/stone details. Uses existing PNGs without rewriting source artwork.
- MeleeDemoSetup wires the shared cleanup material. Verification capture now supports all fifteen selections. Graphics-enabled Unity batch BuildAndVerify passed all fifteen checks, including character movement/barrier follow checks, with no reported shader/C# errors.
- Rendered all fifteen skills and inspected labelled docs/player-art/MeleeDemo/CleanupAudit.png. No grey backing panels visible in these representative renders; original shield outline, blade trails and fire/ground fragments remain visible. Material filtering applies throughout each clip. Subtle translucent glow is deliberately retained around actual cuts.

## Surrounding barrier and WASD demo control — 2026-11-13

- Opening shield Demo now renders a transparent turquoise/gold elliptical barrier around the whole character using BarrierShield shader/material. Original sprite texture is reused on a runtime FullRect sprite so the old tight shield mesh cannot clip the barrier. Barrier stays visible, pulses on the paused demo clock and follows actor center.
- WASD uses existing Input System Keyboard input with normalized diagonal movement at 2.8 units/s. First input takes control from scripted movement, position stays under player control through demo cycles, and arena bounds keep the actor visible. Run direction follows movement. Pause freezes control; replay resets position. Body-attached barrier/orbit effects follow the actor.
- MeleeDemoSetup wires the barrier material and includes Rebuild and Verify Melee Demo (Ctrl+Alt+F8). Footer explains Game View focus/WASD. Demo retains scripted skill playback and illustrative enemy reactions.
- Unity Play Mode passed all 15 skill checks plus normalized diagonal displacement and barrier-to-actor centering. Rendered and visually inspected docs/player-art/MeleeDemo/CombatPreview0.png with a complete surrounding barrier and visible character. This remains a controllable visual demonstration with scripted skill timing/reactions.

## Demo corrections from screenshots — 2026-11-13

- Fixed UI trails by clearing header/sidebar/footer outside the camera viewport on every repaint. UI and camera reserve space using the same responsive scale, including small Game View sizes. Inspected live Unity Game View with clean text and visible actor/enemies.
- Reworked existing KhienChan sheet into a recognizable pointed shield with metal rim using built-in imagegen. Re-exported original eight frame paths/GIF; prompt and prior source retained under docs/player-art/MeleeSkills. Updated shield description.
- Removed assassin retreat discontinuity at 1.4s and makes retreat face left. HoanhTao uses continuous rotation of original sweep frame; HoiNguyet uses one original crescent on a smooth out-and-back trajectory with return flip. PhanQuang uses a stable original fan frame moving from the weapon. Heavy impact waits for the actual heavy attack release at 2.41s; qi ultimate release and weapon offsets aligned. KiemAn stays centered on its target.
- Added GoldVfxOnly shader/material for VanAnh demo. Filters neutral/teal field while retaining gold cuts; rendered CombatPreview14.png inspected with grey rectangle removed. Reaction region remains invisible and checks target positions against its elliptical footprint, including an outside enemy.
- Interactive Unity Play Mode verification passed all 15 scenes, accepting sprite or transform animation for continuously moving effects. Repeatable verifier now resets internal state between runs. Captured and inspected actual rendered effects; live Game View text was reviewed. Detailed subjective timing/pose fit still benefits from user review.

## Character/enemy melee demonstrations — 2026-11-13

- MeleeDemo now instantiates existing Swordsman/Assassin/DaoWarrior/QiSwordsman visual prefabs and four small enemies using the actual Enemy prefab sprite/material. Stage1 uses Swordsman; each passive uses its branch actor; ultimate toolbar selects any of three branches.
- Added MeleeDemoCombat, a deterministic 5.2-second demonstration timeline. Samples original Idle/Run/Attack clips and VFX on one clock. Includes preliminary strikes for passives, stab/retreat, circling weakpoint, heavy windup, approach to break qi marks, dash and landing arc. Enemies flash red/recoil; landing also lifts them briefly. Dash demonstration avoids damage. Replay resets whole encounter and pause freezes actors, effects and enemies together.
- MeleeDemoSetup serializes four character prefab refs, existing enemy sprite/material and combat presenter into saved MeleeDemo scene. No new character or effect artwork. Demo timing and reactions are illustrative choreography; these changes do not implement full combat rules in GameScene.
- Actual Unity Play Mode passed all 15 selections with correct branch actors, four enemies, advancing VFX and six active renderers per encounter. Runtime validation report Temp/MeleeDemoCombatVerification.txt. Visual review of full OnGUI layout remains Needs verification.
- Final graphics-enabled batch Play Mode passed after timing fixes. Rendered and inspected camera-only combat frame docs/player-art/MeleeDemo/CombatPreview.png, showing DaoWarrior, four original spider sprites and circular slash. This render excludes OnGUI.

## Melee demo scene — 2026-11-13

- Added Assets/Scenes/MeleeDemo.unity, runtime MeleeSkillDemo and Editor MeleeDemoSetup. Uses all 15 existing skill visual prefabs (3 opening, 9 branch passives, 3 ultimates), original sprites/controllers/materials. Left scrollable grouped sidebar switches one active prefab; replay/pause/repeat and uniform zoom controls in main area.
- Uses existing OnGUI approach, dark forest-color background and original Point-filtered VFX. Camera viewport reserves header/footer and fits both dimensions. Scene is independent and can be opened directly in Unity and played.
- New artwork was not generated. C# compiled and scene build verified with MELEE DEMO READY for 15 wired prefabs. MeleeDemoVerification added to verify selection replacement and sprite animation in Play Mode.
- Actual headless Unity Play Mode verification passed all 15 selections: each has a non-null sprite, advances animation and leaves exactly one active effect. Log Temp/MeleeDemoPlayMode.log contains MELEE DEMO PLAY MODE PASSED. Headless verification does not assess rendered UI layout.
- Needs verification: manual visual assessment of sidebar/control layout in Unity Game view at the user's display size. Demo displays supplied animation clips; combat conditions and damage remain outside the visual gallery.

## Stage 1 and 2 melee VFX — 2026-11-13

- Added Assets/Prefabs/Player/MeleeSkills with three Stage1 effects (KhienChan, AnhBo, LuoiDaoHoThe) and nine branch passives. Assassin: TamLienThu, AnhHoiKich, AnTuHuyet. DaoWarrior: DiaLietTram, HoanhTao, TrongDao. QiSwordsman: HoiNguyet, PhanQuang, KiemAn.
- Each uses eight transparent 256px frames, PPU48, Point filter and a 0.64-second visual clip. Twelve controllers and visual prefabs generated by MeleeSkillAssetBuilder. Orbiting blades loop; other clips clear sprite at end. Timing chosen for visual preview and requires alignment with combat later.
- Sources generated through built-in imagegen; prompts, extraction script, twelve GIFs and labelled Overview.png in docs/player-art/MeleeSkills. Export cleans neutral haze and cell edges; overview visually inspected and ground-crack haze corrected.
- Verified 96 PNG dimensions, all twelve eight-frame GIFs, 96 frame metas, twelve clips/prefabs. Final Unity 6000.3.25f1 batch compile/import reports twelve sets built successfully and successful batchmode exit.
- Needs verification: gameplay-scale visual review, attachment offsets and real Play Mode. Passive conditions, damage, movement, shield blocking and duration controls require gameplay integration. No gameplay logic added.

## Ultimate video reference revisions — 2026-11-13

- Viewed sampled frames from user videos 165813 (gold-white crossing slashes over teal arena) and 170126 (three long pointed sword lanes in a fan). Updated VanAnh to warm gold ghost/slash traces with teal arena and five-point path. Updated TamThien to pale blue charging lanes transitioning to straight gold-white narrow blades released in a fan. Both retain 16-frame visual clips.
- Built-in imagegen used video frame references. Prompts VanAnh-v2/TamThien-v3 and reference sampling script are in docs/player-art/MeleeUltimates; earlier sheets retained there. Export removes neutral sheet residue for TamThien. GIFs updated. Inspected dark-background composites.
- Needs verification: gameplay-scale human review. Five-ghost physical motion and hold-R variable charge remain preview artwork awaiting gameplay integration. No Play Mode combat test performed for these revisions.
- Unity 6000.3.25f1 compiled/imported with MELEE ULTIMATES READY and return code0 after both revisions.

## TamThien art revision — 2026-11-13

- Replaced TamThien sheet/16 frame PNGs from user-provided gold-white sword fan reference. New blades have pointed ivory edges, gold rims and charcoal trails; transparent fan wash reveals targets. Inspected composited frame on dark background. Updated TamThien.gif and saved v2 prompt/preview in docs/player-art/MeleeUltimates; previous source backed up there.
- Export accepts Names to target one set. Builder has BuildTamThien menu/entry point for isolated import. Character skill triggering and variable charge remain pending gameplay integration.
- Verified Unity 6000.3.25f1 compilation/import completed with MELEE ULTIMATES READY for one set and successful batchmode exit. No gameplay Play Mode test performed.

## Melee ultimate VFX assets — 2026-11-13

- Added Assets/Prefabs/Player/MeleeUltimates with ThienVan (descending fire/impact/circular sweep), TamThien (charge then three-blade filled cone facing +X), VanAnh (five apparitions and progressively drawn pentagram/filled center). Each contains 16 transparent 256px frames. Shared preview timing is 1.28 seconds, PPU48, Point filtering, final sprite cleared.
- Built-in imagegen source sheets and 48 extracted frames retained. Prompts, extraction script and three dark-background GIFs are in docs/player-art/MeleeUltimates. Extraction preserves cell placement with edge feathering to soften source-sheet halo seams.
- MeleeUltimateAssetBuilder creates three one-shot clips/controllers/visual prefabs through Tools > Brotato > Build Melee Ultimate Assets.
- Verified Unity 6000.3.25f1 C# compilation and asset import; log reports MELEE ULTIMATES READY and Exiting batchmode successfully. All three prefabs generated. No gameplay Play Mode test performed.
- Needs verification: visual polish at gameplay scale, character jump/landing and physical five-ghost movement. Current sheet animates star formation with apparitions at vertices. Hold-R charge/release, aiming, boss selection, hit areas, damage, cooldown and invulnerability still require gameplay integration. These are visual asset previews.

## Full qi swordsman visual set — 2026-11-12

- Added Assets/Prefabs/Player/QiSwordsman with 80 transparent 320x320 character PNGs, four 256x256 wave PNGs, 17 clips, two controllers and two visual prefabs. PPU120; character ground y240, center pivot. Per direction Idle4 / Run5 / Attack7 / Death4. Attack lasts 0.78 seconds with suggested wave release at frame03/time0.31.
- Separate right-facing +X QiWave preserves translucent cyan glow and fades over four frames/0.36 seconds; final curve key clears Sprite. Directional rotation and projectile movement/damage are future combat integration. ReleaseDemo.gif is a composited visual illustration of the proposed release/flight timing.
- Built-in imagegen sources follow the approved long-black-hair/teal-robe swordsman. Export normalizes scale/ground, cleans character alpha haze and detached neighbor fragments, and pads wave bounds to preserve soft glow. Prompts, reproducible export scripts and 18 preview GIFs are in docs/player-art/QiSwordsmanFull.
- Unity 6000.3.25f1 batch compiled/imported the builder with QI SWORDSMAN READY and exit0. Verified 80 character/4 wave PNG dimensions, 18 GIF frame counts, no strongly opaque canvas edge pixels, 17 clip assets and both prefabs. Character clip references and first four wave sprite references are checked during construction; final null wave key is intentional. Composite preview inspected and stray neighbor glint removed.
- Needs verification: human assessment of all directional cycles and actual GameScene/Play Mode combat integration. AI-generated poses can retain small drawing variations. Existing gameplay remains at its prior implementation.

## Full heavy curved-saber asset set — 2026-11-12

- Added Assets/Prefabs/Player/DaoWarrior with 84 transparent 320x320 PNGs at PPU120, 16 clips, DaoWarrior.controller and DaoWarriorVisual.prefab. Per direction Down/Left/Right/Up: Idle4 / Run5 / Attack8 / Death4. Body height is normalized near 160 pixels, ground y240, center pivot. Wider canvas accommodates the large curved blade.
- Attack is a 1.10-second weighted windup/diagonal chop/recovery cycle; Run loops at 0.60 seconds. Left Attack frames are mirrored to correct generated source orientation. Alpha haze and small detached specks are cleaned. Attack source rectangles include horizontal padding to preserve the full blade.
- Four imagegen source sheets follow the approved topknot/bronze-armor/red-sash character. Prompts, export script and 16 looping forest GIF previews are in docs/player-art/DaoWarriorFull.
- Checks completed: 84 PNG dimensions, zero strongly opaque pixels on output canvas edges, all 16 GIF frame counts, 84 sprite meta files, 16 clip sprite references and preview prefab. Unity 6000.3.25f1 batch compiled/imported with DAO WARRIOR READY and exit 0.
- Needs verification: human review of complete animation cycles and gameplay combat integration in Play Mode. This delivery provides visual assets and an Animator preview prefab. AI-generated poses can retain small drawing differences.

## Full dagger assassin asset set — 2026-11-12

- Added Assets/Prefabs/Player/Assassin with 72 transparent 256x256 PNG frames at PPU120, 16 clips, Assassin.controller and AssassinVisual.prefab. Per direction Down/Left/Right/Up: Idle4 / Run5 / Attack5 / Death4. Attack is a fast 0.48-second stab/retract cycle. Idle/Run loop; Attack/Death play once.
- Built-in imagegen used the approved silver-haired/purple-scarf assassin reference. Export centers frames, aligns ground, normalizes action scale, removes faint alpha haze and small disconnected specks. Expanded Attack source rectangles preserve the dagger tip crossing nominal cell boundaries. Verified no strongly opaque pixels touch any output canvas edge.
- AssassinAssetBuilder completed in Unity 6000.3.25f1 batch mode with ASSASSIN READY and exit 0. Checked 72 PNG dimensions, 16 GIF frame counts and clip sprite references during construction. Source prompts, reproducible export script and 16 forest GIF previews are under docs/player-art/AssassinFull.
- Visual asset package is ready for preview. AI artwork can retain small proportion/pose differences. Gameplay stab triggering/damage and Play Mode combat integration remain Needs verification. Existing Player/GameScene gameplay is not changed by this visual package.

## Full swordsman asset set — 2026-11-12

- Added Assets/Prefabs/Player/Swordsman with 76 transparent 256x256 PNG frames, PPU120, 16 clips, Swordsman.controller and SwordsmanVisual.prefab. Four directions each have Idle4 / Run5 / Attack6 / Death4. Idle/Run loop, Attack/Death play once. Attack uses reviewed 0.74-second timing. The wider common canvas accommodates the sword arc.
- Built-in imagegen produced four action sheets from approved reference. Export aligns ground and normalizes action scale; left Attack/Death are mirrored to correct wrong-facing generated rows. Alpha haze and small disconnected specks are removed. AI-generated artwork can retain small pose/proportion differences.
- Unity 6000.3.25f1 batch build compiled SwordsmanAssetBuilder and completed with SWORDSMAN READY and exit 0. Verified 76 frame metas, 16 clips and visual prefab. Sixteen looping forest GIFs and generation prompts are under docs/player-art/SwordsmanFull. Play Mode combat integration and human assessment of all animation cycles remain Needs verification.

Swordsman motion review: `docs/player-art/Swordsman/` contains the six right-facing attack poses, aligned PNG frames and a looping forest GIF. One cycle is 0.74 seconds including the recovery hold. GIF decoded with six frames. Timing and silhouette await user review before directional production or Unity integration.

## Melee character art review — 2026-11-12

`docs/player-art/MeleeLineup-v1.png` contains the first eight-pose forest concept sheet. Columns show swordsman, assassin, heavy saber fighter and qi swordsman. Rows show right-facing idle and peak attack. User review of silhouettes and weapons precedes the four Attack_Right motion samples. In-game scale and timing still need Unity verification.

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

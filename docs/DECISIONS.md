# Decisions

Accepted intent unless marked temporary. Actual defects/checks belong in [STATE](STATE.md). Full roadmap remains in [original plan](../ke_hoach_do_an_game_2d_brotato_like.md).

## D01 - Simple, scoped implementation

- Decision: small readable MonoBehaviours, private fields/Inspector tunables and existing packages; defer optional features/frameworks/pooling.
- Context: three students learning Unity; project must be finishable and explainable.
- Reason: reliable core loop takes priority over complexity/content volume.
- Alternatives: advanced patterns/AI/pooling are optional in plan, not initial requirements.
- Tradeoff: direct dependencies and temporary hardcoding need documentation.
- Preserve: no Unity/package upgrades, new third-party dependencies or automatic next-feature work without user direction.

## D02 - GameScene is the gameplay entry

- Decision: continue in Assets/Scenes/GameScene.unity; enabled build scene.
- Context: older Gameplay.unity did not contain full weapon setup.
- Reason: consolidate progress in existing combat scene.
- Alternatives: migrating older scene was not selected; deleting it was unnecessary.
- Tradeoff: legacy scenes remain and may confuse sessions.
- Preserve: keep scenes/assets; do not silently change main scene or create one scene per wave.

## D03 - Mouse aim and continuous automatic fire

- Decision: mouse world-position aim, fire without holding left button.
- Context: detailed weapon/control design and conversation specify this; introductory plan text mentions auto-targeting inconsistently.
- Reason: user moves/dodges while directing aim.
- Alternatives: held-button firing existed initially; nearest-enemy targeting is not current design.
- Tradeoff: depends on Mouse and Camera.main; fireRate means interval in seconds, unlike planned shots/sec examples.
- Preserve: no click-to-fire or enemy auto-aim without a new design request; retain units when migrating data.

## D04 - Three configurable waves for MVP (updated 2026-09-18)

- Decision: 3 waves in one scene: duration 30/30/45 seconds, living caps 10/15/30, enemy HP 10/20/30, spawn intervals 1.5/1/0.6 seconds and speed 2/2.5/3 units/s.
- Context: explicit user request changed 60 seconds to 30 during Milestone 2 work.
- Reason: current approved scope; 9 waves follow later.
- Alternatives: original 45-60-second plan range is superseded for duration; final nine-wave scope remains future.
- Latest user request supersedes the earlier all-30-second setting and multiplier examples.
- Tradeoff: inline serializable WaveSettings are editable in Inspector; reusable WaveData ScriptableObjects remain planned.
- Preserve: absolute per-wave stats, not compounded multipliers; counts mean concurrent living caps, not a total spawn budget.

## D05 - One life replaces test respawn

- Decision: HP zero emits PlayerHealth.Died and ends round; PLAY AGAIN reloads a fresh scene.
- Context: automatic full-HP respawn was briefly added for testing, removed for survival-loop implementation.
- Reason: original one-life rule and meaningful fail condition.
- Alternatives: multiple lives/permanent automatic respawn not selected.
- Tradeoff: death testing requires restart; dead Player remains visible while combat freezes.
- Preserve: nonnegative HP, shared cooldown across attackers, one death notification, no unsolicited respawn.

## D06 - Minimal UI and global round freeze (temporary)

- Decision: GameManager.OnGUI displays HUD/outcomes; end-round disables spawner/movement/weapon and sets Time.timeScale=0.
- Context: testable win/lose loop before UI polish.
- Reason: little scene/UI setup needed.
- Alternatives: Canvas/UIManager is planned, not yet implemented.
- Tradeoff: UI/game logic coupled; timeScale pauses physics/scaled clocks, NOT Update callbacks. Retry/cleanup must restore timeScale.
- Preserve: explicit shutdown/reset behavior; do not assume timeScale alone stops shooting.

## D07 - Prefab spawning and living-enemy cap (updated 2026-09-18)

- Decision: instantiate Enemy outside orthographic camera, parent under Enemies and count active EnemyHealth with HP > 0.
- Context: beginner-friendly bounded spawning.
- Reason: minimal bookkeeping; destroying a child frees cap space.
- Alternatives: registry/events/pooling not selected for this phase.
- Tradeoff: scan the small bounded enemy subtree instead of maintaining a registry; minimum distance from Player is 3 units.
- Preserve: absolute stat initialization for new/placed enemies; clear old enemies before the next wave, never heal survivors by reconfiguring mid-wave.

## D08 - Repository-backed memory

- Decision: concise AGENTS plus STATE/DECISIONS/ARCHITECTURE/GAME_DESIGN under docs; keep original plan.
- Context: explicit user request to continue future sessions without this long chat.
- Reason: inspectable Git-shareable facts, intent and uncertainty.
- Alternatives: chat-only memory rejected by request; duplicate .cursor rules unnecessary.
- Tradeoff: refresh docs after meaningful tasks; current code/serialized values and later instructions override stale summaries.
- Preserve: no invented verification; session workflow refreshes relevant memory files.

## D09 - Intermediate cleanup and one-life HP

- Decision: show WAVE COMPLETE for 2 unscaled seconds, freeze combat, remove old enemies/bullets, preserve Player HP/position, then resume.
- Reason: no old weaker enemies or projectiles leaking into a new difficulty level; no damage during the announcement.
- Final wave uses Victory; intermediate WaveComplete is not a terminal win.
- Tradeoff: survivors disappear without kill rewards (no reward/drop system exists yet). Revisit with that future feature.

## D10 - Follow camera, no arbitrary arena wall

- Decision: Main Camera follows Player XY in LateUpdate and preserves Z; Player remains unbounded until a map rectangle is designed.
- Context: user explicitly requested camera-follow verification; no authored map bounds were provided.
- Reason: Player remains visible and spawn region travels with gameplay.
- Tradeoff: flat prototype background makes movement less visually obvious; no camera smoothing/map edge clamp yet.

## D11 - One accepted hit per bullet

- Decision: consumed-hit flag before damage, immediate deactivation, delayed Destroy; disable redundant BoxCollider2D while retaining CircleCollider2D.
- Reason: multiple collider callbacks must not multiply damage. Real Play Mode physics regression passed with both colliders temporarily enabled.

## D12 - Prepared directional artwork, manual installation (2026-11-09)

- Follow-up supersedes manual installation: user explicitly asked Codex to complete Unity setup. Artwork/controller/script are now installed in Player prefab and inherited by GameScene. The separate Apply Player Artwork command performs this setup; Prepare remains asset generation only.
- Decision: prepare separate centered PNG frames and named Idle/Run/Death clips for four directions, with a small optional PlayerSpriteAnimator on PlayerVisual. Install through the Vietnamese guide rather than automatically changing the user's prefab/scene.
- Context: user requested balanced cuts for each action and step-by-step instructions. Source is an opaque checkerboard sheet; built-in imagegen produced an alpha extraction, followed by deterministic Unity component-based cropping/ground alignment. Extraction may slightly alter original pixels; preserve the source.
- Reason: individual PNGs and a ready controller avoid repeated manual slicing/keyframe wiring. Direct Animator.Play on state changes keeps the 12-state setup readable without unnecessary transition logic.
- Preserve: Player root Rigidbody2D/collider/movement/HP, scene-added WeaponHolder, mouse aim and auto-fire. Clear the old visual-rotation reference only when installing this upright animation set. Death uses unscaled animation time without delaying Game Over or adding lives. Saved scene still uses its existing artwork until the user follows the guide.

## D13 - Thử hệ Súng trong scene riêng (2026-11-10)

Thử hiệu ứng cấp 4 và các nhánh bằng bia cố định cùng bộ chọn trực tiếp. Thông số thử gồm một lần nảy, ba đạn phụ gây nửa sát thương khi kết liễu và mức cộng 3 sát thương sau mỗi mục tiêu xuyên qua khi bật nâng cấp. Dùng lại PNG trong `public/gun` và prefab đã có. Phần kết nối lên cấp sẽ dùng luật đã chốt sau khi nhóm xem bản thử.

Bản thử nâng cấp mỗi nhánh theo ba lựa chọn độc lập, mỗi lựa chọn có bậc 0–3. Nhánh Đạn tỏa có thêm viên, đạn hẹp và đẩy lùi. Nhánh Liên thanh có thêm viên, đạn nảy và bung tỏa. Nhánh Xuyên thấu có tăng sát thương qua địch, Dấu xuyên và Dư chấn. Bảng điều khiển và thông số thử nằm trong HUONG_DAN_TEST_SUNG.md.

## M12 - Mage effects follow teammate's art style (2026-11-11)

- Decision: use `origin/huy:Assets/Art` as the visual reference for future mage skill VFX. Match its thin luminous lines, restrained glow, sparse centered composition and stable animation framing while preserving each skill's meaning.
- Context: user asked for the mage skills to look consistent with a teammate's art. See [ART_STYLE](ART_STYLE.md) for inspected examples and the pinned source commit.
- On the user's later request, the three 8-frame PNGs were replaced by 30-frame restyled exports. Their visual match still needs a Unity Play Mode review.

## M13 - Level 6 mage branches (2026-11-11)

- Decision: record the user's four level 6 mage branch outcomes and exact supplied damage/status values in [GAME_DESIGN](GAME_DESIGN.md). Fire, Ice, Light and Dark have different combat roles and should have visually distinct effect assets in the established mage style.
- This is design scope only; none of the level 6 behavior, targeting or status systems exists in the current game. Unspecified durations, stacking and trigger details remain open.
- Superseded Ice visual revision: the earlier three-hit shell concept is replaced by the six-hit Ice rule in M15.
- Superseded Ice attack revision: the earlier individual shard and three-hit freeze are replaced by the Ice breath in M15. The old shard asset discussion remains historical only.
- Ice core revision in `magicstree.drawio`: replace the piercing-spike `Băng Thương` with `Tụ Hàn` in both repeated Ice-core nodes. Repeated Ice hits on one enemy build up to 2 stacks; the next hit gains 10% damage per stack, and stacks clear on freeze or slow expiry. This avoids requiring a separate piercing-spike effect.
- Burn status visual revision: show separate small flames and rising embers at the enemy's lower/sides, with an open center and no circular rune or border. The visual overlay does not determine burn damage or timing.
- Curse status visual revision: replace the circular curse emblem with sparse dark-violet cracks attached to each affected enemy. A tiny violet glint moves along the cracks with faint smoke; the mostly transparent footprint keeps the underlying enemy readable even when up to three enemies are cursed. Damage, slow and duration remain code-controlled.
- Dark target cap is upgradeable through future breakthroughs: three is the current level 6 cap, not a permanent art limit. Any future attack VFX should be one reusable projectile/impact unit instantiated for however many targets code selects; do not paint a fixed count of three into one sheet. Targeting behavior and later cap values remain unspecified.
- User supplied three Dark breakthrough options (select one of three, up to three times): increase curse target cap, increase curse damage-over-time, or increase slow plus reduce cursed enemies' outgoing damage. Exact per-tier increments were omitted and remain unspecified. The target-cap upgrade makes a fixed three-branch art sheet unsuitable.
- On the user's asset request, replace the long DarkRay export with a single reusable compact CurseWisp projectile VFX. It visually supports movement to a first target and short transfers between further targets; the actual targeting and transfer rule is not yet a gameplay decision. Continue to use the fissure overlay on affected enemies.

## M14 - Shared Mage ultimate asset candidates (2026-11-11)

- The user kept three active ultimate concepts available to the Mage regardless of elemental branch: Ngưng Đọng Thời Gian, Thiên Tinh Giáng Lâm, and Thiên Cầu Ma Lực. The earlier fourth Cổng Lưu Đày concept was dropped. Initial damage, cooldown and trigger rules were later set in M15 on the user's request; they still require Play Mode balancing.
- On request, preserve the original PixelFX Arcane PNG sprite sheets and JSON timing/rect metadata for the five effects mapped to these concepts in `public/skill/Magical/`. Keep the upstream MIT notice alongside them. Asset acquisition does not authorize gameplay implementation or establish that the original PixelFX look matches the team's existing Mage art.

## M15 - Mage tree baseline for four stages (2026-11-11)

- The user confirmed 20 Mage levels across four stages: levels 1-4, 5-8, 9-14 and 15-20. Levels 13-14 select two distinct cores; levels 19-20 select the two remaining cores. This is planned Mage scope; the existing three-wave MVP is unchanged.
- The user directed GAME_DESIGN to match the tree's Ice breath and six accepted hits per enemy. The Ice mark will reuse selected formation frames from the existing `freeze-v2` sheet; the sixth hit plays the completed freeze effect. One enemy can count only once per breath cast. The earlier shard/three-hit plan is superseded.
- On the user's request to fill missing values, GAME_DESIGN and `magicstree.drawio` now give the three level-5 Q skills and three level-15 E ultimates concrete initial range, damage/heal, duration and cooldown values. These are design baselines for implementation and require balance checks in Play Mode. No Mage code, scene wiring or imported VFX was added in this decision.

## M16 - Mage preview before progression (2026-11-11)

- User chose character plus every skill and ultimate in the tree before EXP/leveling. Keep the current GameScene MVP intact and use `Magical.unity` as a separate preview; `MageLoadout` Inspector fields stand in for future level choices. This allows every branch and core to be exercised without claiming the 20-level flow exists.
- Use the supplied PixelFX PNG sheets directly through a shared runtime sprite-sheet player. Keep source PNGs in `public/`, import copies into `Assets/Art/Mage`, and retain the PixelFX MIT notice. Runtime status code determines damage/timing independently of animation frames.
- Temporary Dark attack default: nearest target within 7 m, prioritize uncursed while under the current cap, 4-second curse refreshed on hit. Attack intervals Fire 0.8 s, Ice 1 s, Light 2.5 s, Dark 1 s are Inspector tunables for preview balancing, not final progression rules.

## M17 - Mage preview scale and test controls (2026-11-11)

- User found the Mage and effects too small and requested follow camera plus a skill test panel. Keep the already-wired `CameraFollow`, confirm its target in Play Mode, use orthographic size 4.5, enlarge the Mage visual to 1.5× and initially apply a 1.4× visual multiplier to Mage effects. The multiplier changes appearance only, not hit radii or damage.
- The Play Mode panel changes Q, element, ultimate, tiers and cores immediately. It clears the relevant preview cooldown on a base-skill switch so each option can be tried. Default invulnerability is confined to `Magical.unity` preview and can be turned off; GameScene's one-life combat is unchanged. Scene reset restores the saved Inspector loadout. This panel is not the future level-choice UI.

## M18 - Mage effect and staff placement (2026-11-11)

- After a second size review, increase the shared Mage sheet-effect display multiplier from 1.4× to 2.2× and the separate Ice Mark overlay from 0.5 to 0.75. Keep damage and hit radii as configured by each skill.
- Place the staff sprite pivot on its grip, position that grip at the Mage's visible hand, and spawn aimed projectiles near the staff tip. Keep the offsets serialized so the team can tune them after checking other Mage animations and resolutions.

## M19 - Magical training room and effect motion (2026-11-11)

- Keep all training controls in `Magical.unity`: stationary, non-damaging dummies with configurable count, HP and radius, plus a preview HP reduction button. These controls do not change `GameScene.unity` or the one-life rule.
- Heal requires missing HP. One Ice Mark overlay grows through the freeze asset's formation frames for hits 1-5; hit 6 triggers the full freeze effect. Gravity pulls ordinary enemies toward its center while they are in range; bosses remain immune to control.
- The held staff sprite must be explicitly reassigned when the importer is refreshed. Use a larger preview scale of 1.45. Ice Breath follows mouse aim during its animation; Shockwave expands from the Mage; Arcane Orb has a smaller charging visual and larger release visual. These are presentation changes, with existing damage and cooldown values retained.

## M20 - Mage projectile origin at staff tip (2026-11-11)

- Derive the Mage cast point from the Staff transform and a serialized local offset of 0.64 that matches the visible crystal tip. This keeps the origin attached when the staff rotates or is rescaled. Aim updates before combat. Align the rear edge of Fire, Dark and Arcane missile sprites with the cast point; start Ice Breath's visible left edge there as well.
- Check projectile collision at spawn before moving, since a nearby target may already overlap the staff tip. Preserve one-hit consumption. Damage and cooldown values do not change.

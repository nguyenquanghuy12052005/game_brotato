# Shared skill VFX style

Reference requested by the user: [`huy/Assets/Art`](https://github.com/nguyenquanghuy12052005/game_brotato/tree/huy/Assets/Art), inspected at commit `60f25635861ebb0d77eed5473f13bad2af424f9e` on 2026-11-11. This branch is available as `origin/huy`; it is not the current working-tree branch. Use its actual images as visual references when generating future mage skill effects.

## Observed visual language

- `Assets/Art/GunSkills/Level11/` has ten 2048 x 1024 RGBA animation sheets, visually arranged as 8 columns by 4 rows of 256 x 256 cells. Effects occupy a modest central area with generous transparent space and a stable anchor.
- The dominant look is clean luminous 2D FX: thin arcs and rune lines, a bright core or leading edge, restrained soft outer glow, and a dark/transparent interior. Highlights move between neighboring frames without large changes to the silhouette.
- Cool skills use deep blue/cyan with occasional violet accents (`burst-chain-detonator`, `pierce-sky-lance`, `pierce-spatial-rift`). Warm attacks use amber/orange (`spread-sunder-pressure`, `spread-sweeping-blade`). Keep each effect to one coherent hue family with a bright highlight and darker edge.
- Smaller examples include `bullet-piercing.png` (28 x 12) and four `weakpoint-flame-00..03.png` frames (24 x 16). `piercing-shockwave.png` is a single large cyan ring. These confirm that sparse shapes and clear silhouettes matter more than filling each cell.
- These references look smoother and more softly glowing than strict binary-alpha pixel sprites. If a new request also says “pixel art,” match the teammate's line weight, glow, spacing and animation first, while keeping the game-size result crisp. Avoid chunky jagged borders, thick filled disks and heavy opaque halos unless the referenced effect uses them.

## Future mage effects

- Attach relevant images from `origin/huy:Assets/Art/GunSkills/Level11/` as style references to the generator; a path in prompt prose alone does not supply the pixels. Use `pierce-sky-lance` for violet/cyan rune style and `burst-chain-detonator` for blue circular magic. Pick a warm reference only for warm-colored skills.
- Keep the mage effect's own gameplay meaning and color, but align line thickness, glow softness, detail density, frame spacing and perceived size with these references. Use transparent backgrounds, identical square cells and one stable center/pivot.
- On the user's request, the three earlier 8-frame mage sheets were replaced by `public/skill/Magical/{HealingEffect,ShockwaveEffect,GravityFieldEffect}_30frames.png`. Each has 30 frames in a 10-column x 3-row grid of 256 x 256 cells. After the first 30-frame drafts were rejected, each was regenerated from a distinct multi-frame FX sheet with the teammate's art attached as a style reference. The current sheets preserve fine luminous lines, soft alpha and per-frame shape changes. Check them at actual game scale before treating the visual match as final.
- Save final mage exports directly in `public/skill/Magical/`, with no persistent intermediate folders, per user preference.

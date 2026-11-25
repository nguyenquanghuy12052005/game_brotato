# Game design: current and planned

Sources: [original plan](../ke_hoach_do_an_game_2d_brotato_like.md), scripts/prefabs/GameScene and explicit conversation decisions. Distinguish current behavior, defective intended behavior and planned scope.

## Concept and controls

Single-player 2D top-down survival shooter inspired by Brotato, for three student developers. Prioritize a complete explainable loop over content volume.

Current: WASD movement, mouse-directed aim, continuous automatic fire; no click-to-fire/reload/weapon switching. ESC Pause is planned, not implemented. Detailed mouse-aim design and explicit user work override inconsistent introductory automatic-target wording.

## Current loop/outcomes

Open GameScene -> move/dodge/shoot -> enemies spawn/pursue -> bullets damage/kill -> survive each timer -> 2-second Wave Complete -> next wave -> finish wave 3 -> ALL WAVES COMPLETE.

Player HP zero -> GAME OVER. Both terminal outcomes freeze combat and offer PLAY AGAIN (fresh scene reload). One life; temporary test respawn removed. Final completion now has a distinct Victory state; Main Menu is not implemented.

Three waves share one scene. Intermediate completion freezes combat for 2 real seconds, clears remaining enemies/bullets and protects Player from damage. Player position/HP persist without healing. Resume movement, shooting and spawning after the announcement.

## Current values

- Player max HP 100, movement speed 5 units/s; normalized WASD prevents faster diagonals.
- Contact damage 10; global Player immunity 0.75s after accepted hit, including multiple attackers.
- PlayerVisual uses upright four-direction Idle/Run sprites based on WASD; diagonals prefer the horizontal visual direction and standing keeps the last direction. Gun independently aims at mouse and flips left-facing sprite.
- At HP zero, Game Over still freezes combat immediately; a one-shot directional Death animation continues with unscaled animation time and holds its last frame. No extra life or gameplay delay.
- One gun, interval 0.2s/shot (5 shots/s); bullet damage 10, speed 10 units/s, lifetime 3s.
- Bullet disappears after enemy hit/lifetime. Consumed-hit guard prevents repeated damage; redundant Box trigger disabled in prefab. Real Play Mode test with both triggers enabled confirmed one 10-damage hit.
- HP is nonnegative; HP zero is terminal for the current round.

## Enemies/difficulty

Only one Enemy prefab, spider visual; base HP 30, speed 2 units/s, contact damage 10. No enemy shooting/kill reward/drop.

Spawn occurs on four sides outside orthographic camera, margin 1, minimum distance 3 from Player. Cap counts active living EnemyHealth under Enemies, not total spawns. Kills free room for replacements until timer expires.

Current explicit user-approved tuning, saved on WaveManager:

- Wave 1: duration 30s, cap 10, enemy HP 10, spawn interval 1.5s, speed 2 units/s.
- Wave 2: duration 30s, cap 15, enemy HP 20, spawn interval 1s, speed 2.5 units/s.
- Wave 3: duration 45s, cap 30, enemy HP 30, spawn interval 0.6s, speed 3 units/s.

Absolute WaveSettings replace the defective multiplier switch. New spawns and the initially placed enemy receive current wave HP/speed; prefab base values are not multiplied. Settings are inline serialized data, not WaveData assets yet.

Latest user request changes only Wave 3 from 30 to 45 seconds. Balance numbers require a full-duration human playtest; the accelerated Unity regression confirms functionality, not difficulty/fun.

## Planned final scope (not implemented)

- 9 waves in one gameplay scene: Forest 1-3, Desert 4-6, Dungeon 7-9. Themes change visuals, not mechanics.
- Normal/Fast/Tank enemies; no ranged enemies in main version.
- Four gun levels tied to kills, Kill Count/level HUD. Threshold/damage/rate tables in plan are examples, not active configs.
- Health, temporary Speed and temporary Damage pickups; chance-based drops. Example 20% chance/weights/effects are proposed, not present.
- WaveData and selected ScriptableObject tunables; avoid elaborate data frameworks for every value.
- Main Menu/Pause/Wave Complete/Game Over/Retry/Menu/final Victory, finished HUD, audio/effects/basic animation and Windows executable/demo/report.
- No inventory/shop/economy/save-load in first scope. Boss/multiple guns/multiplayer/quests/leaderboards/advanced AI/pooling only optional after core completion.

## Milestones and open rules

Week 1-2 foundations/combat and Week 3 survival loop exist. Requested MVP fixes passed actual Play Mode regression, including transitions and win/loss retries. Week 4 transition is implemented; inline Inspector wave data exists but reusable ScriptableObject WaveData remains a future task.

Current camera policy: track Player XY, preserve Z, free movement without arbitrary arena walls. A future authored map may define boundaries/clamping. Leftovers are cleared at intermissions; Player HP persists. Windows build and full-duration balance remain unverified. Add only the requested feature; stabilize core before extra polish/content.

## Cây Pháp sư 20 cấp, 4 màn (thiết kế; bản thử kỹ năng đã có)

`magicstree.drawio` là bản phân cấp hiện hành cho nhánh Pháp sư. Theo phân chia người dùng chốt: màn 1 cấp 1-4, màn 2 cấp 5-8, màn 3 cấp 9-14, màn 4 cấp 15-20. Bản thử trong `Magical.unity` có nhân vật, skill Q, bốn nhánh đánh tự động, nâng cấp/lõi và ba ultimate E. Có thể chọn cấu hình bằng `MageLoadout` Inspector hoặc bảng test trong Game View khi Play Mode; bảng test có nút bất tử tạm thời và đặt lại cảnh. Chưa có EXP, lên cấp, UI chọn skill theo cấp hay tích hợp vào vòng chơi GameScene. Các trị số là mốc cân bằng ban đầu; cần playtest thêm.

Boss miễn nhiễm các hiệu ứng khống chế của Pháp sư (đẩy lùi, làm chậm, đóng băng, dừng thời gian) nhưng vẫn nhận sát thương trực tiếp và sát thương theo thời gian.

| Mốc | Lựa chọn |
| --- | --- |
| Cấp 2 | Chọn Pháp sư, nhận thêm 30% EXP. |
| Cấp 3-4 và 6-8 | Mỗi cấp chọn một chỉ số nền theo sơ đồ. |
| Cấp 5 | Chọn **một** trong ba skill chủ động bên dưới, dùng phím Q. |
| Cấp 9 | Chọn **một** nhánh Hỏa, Băng, Quang hoặc Ám. |
| Cấp 10-12 | Mỗi cấp chọn một trong ba nâng cấp của nhánh đã chọn. |
| Cấp 13-14 | Mỗi cấp chọn một lõi khác nhau trong bốn lõi của nhánh, tổng hai lõi. |
| Cấp 15 | Chọn **một** trong ba ultimate dùng chung, dùng phím E. |
| Cấp 16-18 | Mỗi cấp chọn một chỉ số nền theo sơ đồ. |
| Cấp 19-20 | Mỗi cấp chọn một trong **hai lõi còn lại** của nhánh; đến cấp 20 có đủ bốn lõi, không mua trùng lõi. |

Trong phòng thử `Magical.unity`, bảng test cho tạo lại 1-30 bù nhìn đứng yên, không gây sát thương, chỉnh HP và bán kính đặt quanh Pháp sư. Có nút trừ 25 HP để thử Hồi phục dù chế độ bất tử đang bật. Các nút này chỉ phục vụ thử skill.

#### Skill cấp 5

| Skill | Kích hoạt và hiệu quả | Hồi chiêu |
| --- | --- | --- |
| Hồi phục | Q: hồi ngay 25 HP, không vượt HP tối đa; chỉ kích hoạt khi chưa đầy máu. | 25 giây |
| Sóng xung kích | Q: gây 15 sát thương một lần trong bán kính 2,5 m quanh Pháp sư và đẩy quái thường ra 1 m. Boss chỉ nhận sát thương. | 12 giây |
| Vùng trọng lực | Q tại vị trí chuột cách Pháp sư tối đa 7 m: tạo vùng bán kính 2,5 m trong 4 giây, gây 3 sát thương/giây và làm chậm quái thường 35% khi ở trong vùng. | 20 giây |

Mỗi skill Q là một lần kích hoạt; hồi chiêu tính theo thời gian gameplay, dừng khi tạm dừng hoặc chuyển màn. Các skill này không tự hồi HP, gây sát thương hay áp dụng hiệu ứng khi chưa được kích hoạt.

#### Nhánh cấp 9 và trạng thái

- **Hỏa:** cầu lửa nổ bán kính 1,5 m, gây 20 sát thương trực tiếp và đốt 4 sát thương/giây trong 3 giây (thêm tối đa 12 sát thương nếu cháy đủ thời gian).
- **Băng:** Hơi thở băng gây 15 sát thương cho mỗi quái trúng, làm chậm 30%. Hiệu ứng hơi thở bám theo nhân vật và hướng chuột khi đang phát. Mỗi lần thi triển chỉ ghi **một lần trúng trên mỗi quái**, dù animation có nhiều frame. Sau **6 lần trúng trên cùng quái**, quái thường đóng băng cứng 2 giây; boss chỉ nhận sát thương, không nhận làm chậm hay đóng băng. Chậm kéo dài 2,5 giây và được làm mới khi trúng lại. Số lần trúng tích lũy hết hạn nếu 6 giây không trúng Băng; khi đóng băng thì đặt lại về 0. **Ấn băng** là một lớp băng duy nhất lớn dần trên quái qua các frame hình thành 10, 19, 24, 30, 35 (đếm từ 0) của `freeze-v2-d9694c96.png`, không phải nhiều biểu tượng ấn riêng; lần thứ 6 phát toàn bộ sheet đóng băng. Cần kiểm tra hình ấn ở kích thước thật trong Play Mode.
- **Quang:** cột sáng gây 8 sát thương mỗi nhịp trong 4 nhịp, tối đa 32 sát thương nếu cùng quái nhận đủ bốn nhịp.
- **Ám:** nguyền tối đa 3 quái cùng lúc; mỗi quái bị nguyền nhận 5 sát thương/giây và bị chậm 15%. Bản thử bắn đạn tự tìm quái gần nhất trong 7 m, ưu tiên quái chưa bị nguyền khi còn chỗ; lời nguyền kéo dài 4 giây và được làm mới khi trúng lại. Giới hạn mục tiêu có thể tăng qua nâng cấp. Các thông số chọn mục tiêu/thời gian này là mặc định thử nghiệm, cần cân bằng tiếp.

`Tụ Hàn` ở nhóm lõi Băng: cùng một quái trúng Băng sẽ tích tối đa 2 tầng. Đòn Băng kế tiếp gây thêm 10% sát thương mỗi tầng; xóa tầng khi quái đóng băng hoặc hết chậm. Tầng `Tụ Hàn` là trạng thái tính bằng code và dùng lại hiệu ứng Băng hiện có.

Các lựa chọn nâng cấp cấp 10-12 và bốn lõi từng nhánh giữ nguyên tác dụng ghi trong sơ đồ, trừ `Băng Thương` đã đổi thành `Tụ Hàn`. Hiệu ứng cháy/nguyền không quyết định sát thương bằng animation; sát thương và thời gian do code quản lý. Bản sao 20 PNG đã được nhập vào `Assets/Art/Mage`; file gốc trong `public/` được giữ nguyên. Bản thử dùng sheet trực tiếp bằng `Sprite.Create`, chưa tạo AnimationClip riêng.

### Ultimate dùng chung ở cấp 15 (đã có trong bản thử)

Chọn một ultimate bất kể nhánh Hỏa/Băng/Quang/Ám; nhấn E để kích hoạt. Boss không bị dừng thời gian hay khống chế cứng. Hồi chiêu bắt đầu khi hiệu ứng kết thúc hoặc khi quả cầu được phóng; hồi chiêu dừng trong Pause/chuyển màn. Các con số là mốc cân bằng ban đầu cần playtest.

| Ultimate | Quy tắc kích hoạt và hiệu quả | Hồi chiêu |
| --- | --- | --- |
| Ngưng Đọng Thời Gian | E: dừng di chuyển và tấn công của quái thường trong 3 giây; Pháp sư vẫn di chuyển và đánh. Sát thương gây lên quái bị dừng được cộng dồn và áp dụng một lần khi thời gian chạy tiếp, không có hệ số thưởng. Boss tiếp tục hoạt động và nhận sát thương ngay. | 60 giây |
| Thiên Tinh Giáng Lâm | E tại điểm chuột trong tầm 8 m: đánh dấu vùng bán kính 3 m trong 2 giây. Gọi 5 đợt sao rơi, cách nhau 0,4 giây; mỗi đợt chọn một quái còn sống trong vùng làm tâm và gây 12 sát thương trong bán kính 1 m, mỗi quái chỉ nhận tối đa một lần sát thương mỗi đợt. Nếu vùng trống, đợt đó không gây sát thương. | 45 giây |
| Thiên Cầu Ma Lực | E tạo quả cầu trong tối đa 8 giây. Mỗi đòn Pháp sư đánh trúng quái trong lúc đó cộng 1 năng lượng, tối đa 6. Nhấn E lần nữa hoặc hết 8 giây sẽ phóng `2 + năng lượng` phi đạn (2-8), mỗi đạn tự tìm quái gần nhất trong 8 m và gây 12 sát thương một lần; không có mục tiêu thì đạn biến mất. | 45 giây sau khi phóng |

Ba ultimate dùng PNG PixelFX trong `Assets/Art/Mage`, phát qua `MageEffectLibrary` và chọn trong `MageLoadout` Inspector. Play Mode regression đã kiểm tra logic TimeStop, StarFall và ArcaneOrb, nhưng cần playtest thao tác phím E, hình ảnh và cân bằng. Chưa có lựa chọn UI. Cổng Lưu Đày đã bị loại khỏi cây skill.

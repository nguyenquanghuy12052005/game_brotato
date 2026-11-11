# Thêm nhân vật mới vào GameScene từng bước

Bộ ảnh ở `Assets/Prefabs/Player/CharacterArt/` đã được gắn vào prefab Player; GameScene kế thừa prefab mới. Unity đã xuất từng PNG riêng, nên **không cần Slice lại**. Các bước dưới đây là tài liệu kiểm tra hoặc gắn lại khi cần, không phải việc còn bắt buộc phải làm.

## 1. Nhận biết các file

```text
Assets/Prefabs/Player/CharacterArt/
├── PlayerSheet_Transparent_v1.png    Bản tách nền, giữ ảnh gốc riêng
├── Frames/
│   ├── Idle/{Down,Left,Right,Up}/    Đứng, 4 hình mỗi hướng
│   ├── Run/{Down,Left,Right,Up}/     Chạy, 5 hình mỗi hướng
│   └── Death/{Down,Left,Right,Up}/   Ngã/chết, 4 hình mỗi hướng
├── Clips/                          12 animation đã nối đủ hình
└── PlayerCharacter.controller      Controller đã có 12 state
```

`Down` = nhìn về phía người xem, `Up` = quay lưng, `Left`/`Right` = nhìn trái/phải, dựa theo bốn hàng ảnh gốc. Bộ gốc không có animation tấn công riêng. Súng hiện có vẫn ngắm chuột và tự bắn.

| Hành động | Số hình mỗi hướng | Tốc độ | Lặp |
|---|---:|---:|---|
| Idle | 4 | 6 hình/giây | Có |
| Run | 5 | 10 hình/giây | Có |
| Death | 4 | 8 hình/giây | Không |

Mỗi PNG có canvas 192×224, nền trong suốt, PPU 120, Pivot Center, Point filtering và không nén. Nhân vật được căn giữa ngang và căn đáy về cùng mốc; không kéo giãn từng tư thế để ép chúng bằng chiều cao. Khi ngã, người thấp xuống là đúng. PNG có phần trong suốt lớn hơn người thật; không suy ra kích thước collider từ cả canvas.

Xem `docs/player-art/PlayerFramesPreview.png`: mỗi hàng tương ứng Down/Left/Right/Up; mỗi hàng có 4 Idle, 5 Run, 4 Death từ trái sang phải. Một số chi tiết có thể khác nhẹ sau khi dùng AI tách nền; ảnh nguồn trong `public/player` được giữ nguyên.

## 2. Mở đúng prefab

1. Dừng Play Mode nếu đang chạy.
2. Trong Project, mở `Assets/Prefabs/Player`.
3. Nhấp đúp `Player.prefab` để vào Prefab Mode.
4. Mở object gốc `Player`, chọn object con `PlayerVisual`.

Không tạo Player mới trong GameScene. Player hiện có trong scene đang được nối với camera, máu, manager và WeaponHolder; thay cả object có thể làm mất các liên kết này.

## 3. Đặt hình ban đầu

Trên `PlayerVisual`:

1. Đặt Transform: Local Position `(0,0,0)`, Rotation `(0,0,0)`, Scale `(1,1,1)`.
2. Trong Sprite Renderer, kéo `Frames/Idle/Down/Idle_Down_00.png` vào ô **Sprite**.
3. Giữ Material **PlayerUnlit**, Color trắng và Alpha đầy đủ.
4. Giữ Sorting Layer **Default**, Order in Layer **0**.
5. Bỏ chọn Flip X và Flip Y. Bộ này đã có ảnh riêng cho trái/phải.

Không kéo cả `PlayerSheet_Transparent_v1.png` vào Sprite Renderer: file đó vẫn là cả bảng tư thế. Dùng PNG trong `Frames`.

## 4. Gắn Animator và script

Vẫn chọn **PlayerVisual**:

1. Nhấn **Add Component**, tìm và thêm **Animator** nếu chưa có.
2. Kéo `CharacterArt/PlayerCharacter.controller` vào ô **Controller** của Animator.
3. Bỏ chọn **Apply Root Motion**. Để Update Mode **Normal**, Culling Mode **Always Animate**.
4. Nhấn **Add Component**, tìm và thêm **Player Sprite Animator**.

Script đã có tại `Assets/Scripts/Player/PlayerSpriteAnimator.cs`; không phải tự chép code trong chat. Script tự tìm PlayerController và PlayerHealth ở object cha.

Controller có sẵn các state `Idle_Down`, `Idle_Left`, `Idle_Right`, `Idle_Up`, `Run_...`, `Death_...`. Script gọi Animator.Play khi đổi hành động/hướng, nên không cần tự tạo Parameters hoặc vẽ Transition. Chỉ một Animator và một PlayerSpriteAnimator trên PlayerVisual; không thêm Animator khác trên Player gốc để điều khiển cùng Sprite Renderer.

## 5. Tắt cách xoay hình cũ

1. Chọn object **Player gốc** trong Prefab Mode.
2. Tìm component **Player Controller**.
3. Giữ **Move Speed = 5**.
4. Xóa tham chiếu ở ô **Player Visual** để nó thành **None (Transform)**. Có thể chọn dấu tròn bên phải và chọn None, hoặc nhấp phải ô tham chiếu > Clear nếu Unity hiển thị lệnh đó.

Đây là bước bắt buộc với code hiện tại: PlayerController chỉ dùng trường này để xoay hình bằng Atan2. Khi để None, nó vẫn đọc WASD và di chuyển Rigidbody2D như cũ, nhưng bỏ qua việc xoay hình. PlayerSpriteAnimator đảm nhận đổi hình bốn hướng.

Không cần thay hàm UpdateFacingDirection bằng đoạn flipX trong hướng dẫn trước: đoạn đó dành cho một sprite tĩnh, còn bộ hiện tại có đủ hình bốn hướng. Nếu bạn đã sửa hàm đó theo hướng dẫn trước, đặt trường Player Visual thành None vẫn làm nó bỏ qua phần lật.

Giữ Rigidbody2D Gravity Scale = 0, Freeze Rotation Z, Scale của Player gốc = `(1,1,1)`; giữ PlayerHealth và collider hiện có. Bộ ảnh không thay luật di chuyển, HP hoặc bắn.

## 6. Lưu và quay lại GameScene

1. Lưu prefab bằng Ctrl+S, thoát Prefab Mode.
2. Mở `Assets/Scenes/GameScene.unity`.
3. Mở Player trong Hierarchy, kiểm tra cấu trúc:

```text
Player                          Rigidbody2D, collider, PlayerController, PlayerHealth
├── PlayerVisual                SpriteRenderer, Animator, PlayerSpriteAnimator
└── WeaponHolder                WeaponAim
    ├── GunVisual
    └── FirePoint
```

4. Kiểm tra Player Controller trong scene cũng có **Player Visual = None**. Nếu trường này có override riêng ở instance, chỉ Revert trường đó hoặc đặt None trên instance.
5. Kiểm tra PlayerVisual có Controller mới và Sprite ban đầu. Nếu có override giữ ảnh cũ, chỉ Revert thuộc tính Sprite hoặc gán lại Idle_Down_00.
6. Giữ GunVisual Order in Layer = **1**. Không chuyển WeaponHolder xuống làm con PlayerVisual và không xóa/revert toàn bộ Player instance: súng đang là phần thêm riêng trong scene.
7. Lưu scene bằng Ctrl+S trước khi Play.

## 7. Chạy thử

1. Mở tab Game và nhấn Play.
2. Đứng yên: chạy Idle_Down.
3. Nhấn A/D: chạy và nhìn trái/phải. W: quay lưng; S: quay về phía người xem.
4. Thả phím: về Idle theo hướng vừa di chuyển. Đi chéo ưu tiên hình hướng ngang; tốc độ di chuyển vẫn do PlayerController chuẩn hóa.
5. Di chuột: súng vẫn xoay riêng; đạn tự bắn.
6. Khi hết giờ wave: animation thường dừng cùng timeScale, tiếp tục khi wave sau bắt đầu.
7. Để quái đánh đến HP = 0: Game Over vẫn xảy ra ngay; Death phát một lần rồi giữ khung cuối. Riêng animation Death dùng UnscaledTime để chạy được khi GameManager đặt timeScale = 0. Đây không phải thêm thời gian sống hay hồi sinh.
8. PLAY AGAIN tải lại scene, HP và Idle trở về trạng thái ban đầu.

Đã gắn/lưu prefab và vào Play Mode: thấy hình mới, Idle đổi frame, súng/đạn hoạt động; khi chết thấy Death_Down_03 trong Game Over và bấm PLAY AGAIN đã tải lại thành công. Còn cần kiểm tra kỹ Run/Idle đủ bốn hướng và đóng băng animation ở chuyển wave; chưa kiểm tra build Windows.

## 8. Nếu gặp lỗi hiển thị

| Hiện tượng | Kiểm tra |
|---|---|
| Hiện cả bảng nhân vật | Sprite phải lấy từ Frames, không phải PlayerSheet |
| Vẫn thấy ảnh cũ | Sprite override của PlayerVisual trong GameScene; đúng prefab Player chưa? |
| Người xoay ngang/lộn ngược | Player Controller > Player Visual phải là None; Rotation của PlayerVisual = 0 |
| Chỉ đứng, không chạy | PlayerSpriteAnimator phải trên PlayerVisual, PlayerController ở cha; Game View có focus; Controller đúng PlayerCharacter |
| Animator báo không tìm thấy state | Dùng controller đi kèm, giữ nguyên tên 12 state |
| Người bị lật sai | Tắt Flip X/Y; không dùng thêm script flip từ ví dụ một hình tĩnh |
| Quá nhỏ/quá lớn | Các PNG đã cùng PPU 120; nếu chỉnh PPU, chọn toàn bộ 52 PNG và chỉnh cùng giá trị. PPU giảm thì hình lớn lên. Giữ Scale Player gốc = 1 |
| Người ngã rồi bật dậy | Death clip phải bỏ Loop Time; không thêm transition quay về Idle |
| Súng lệch khỏi tay | Chỉnh Local Position GunVisual/FirePoint trong Edit Mode, giữ WeaponHolder là con Player; kiểm tra đường đạn sau khi chỉnh |

Nếu sau khi đổi kích thước bạn muốn chỉnh vùng va chạm: chọn CapsuleCollider2D trên Player > Edit Collider, căn quanh thân người ở pose đứng. Không ép collider theo toàn bộ vùng PNG trong suốt và không tự thay collider trong mỗi animation.

## Tái tạo và nguồn

`Tools > Brotato > Prepare Player Sprites` xuất lại đúng bộ trong CharacterArt và giữ GUID của sprite/clip đã có. Lệnh này ghi lại các PNG/clip trong bộ đã tạo, nên không chạy lại sau khi tự chỉnh chúng nếu muốn giữ chỉnh sửa đó. Lệnh không sửa prefab/scene. Không chạy Apply Combat Setup để thay ảnh: lệnh đó phục vụ cấu hình combat và có thể đặt lại thông số wave.

`Tools > Brotato > Apply Player Artwork` là lệnh riêng để gắn lại hình, Animator, PlayerSpriteAnimator và xóa tham chiếu xoay hình cũ trên Player prefab/instance trong GameScene. Lệnh lưu prefab/scene, giữ Player gốc và WeaponHolder hiện có. Không cần chạy lại mỗi lần mở game.

Ảnh nguồn: `public/player/ChatGPT Image 19_25_43 25 thg 9, 2026.png`, 1448×1086 RGB, nền caro tím được vẽ trong ảnh. Dùng built-in imagegen để tách nền thành alpha, sau đó Unity Editor tách từng vùng nhân vật, loại pixel của hình lân cận, căn canvas và xuất PNG/clip. Prompt tách nền được lưu trong `docs/player-art/extraction-prompt.txt`.

# HEXCORE — KẾ HOẠCH SẢN XUẤT ASSET DEMO KỸ NĂNG KIẾM SĨ

> **Trạng thái:** Bản đặc tả triển khai asset demo v1.0  
> **Dự án:** Brotato-Game / HEXCORE · Unity 2D · Pixel Art · Top-down Action Roguelite  
> **Mục tiêu:** Tạo các bản demo **hoàn chỉnh về hình ảnh, chuyển động và cảm giác ra đòn** để xem, góp ý và chốt 24 kỹ năng.  
> **Ngoài phạm vi:** Logic cấp độ, chọn nhánh trong trận, sát thương thực tế, cân bằng, cooldown thật, AI, điều kiện mở khóa, tương tác hiệu ứng cấp 10–11 và tích hợp gameplay chính thức.

---

## 1. Tóm tắt các quyết định đã thống nhất

1. **Có 4 tạo hình nhân vật**; animation phải đúng với tạo hình đang dùng. Bốn tạo hình chưa được cung cấp/kiểm kê đầy đủ trong tài liệu này; tạm dùng `Form_01`–`Form_04` làm mã định danh, **không tự đồng nhất chúng với 3 chuyên nhánh**.
2. **Hỗ trợ 8 hướng** cho Idle, Run/Walk và đánh thường. Các kỹ năng Q/E/R/F phải thể hiện **đúng hướng thi triển** (8 hướng hoặc cơ chế ngắm phù hợp từng chiêu), không nhất thiết vẽ 8 bộ sprite tách biệt cho mọi VFX nếu có thể xoay/lật mà vẫn hợp lý.
3. **Có hệ thống đánh thường riêng**: Kiếm sĩ ban đầu, Sát thủ, Đao sĩ, Kiếm khí; mỗi phong cách được điều chỉnh cho các tạo hình tương thích.
4. **24 kỹ năng chủ động**, chia thành Q (3), E theo 3 nhánh (9), R dùng chung (3), F theo 3 nhánh (9). Một build hoàn chỉnh sở hữu tối đa **4 kỹ năng Q/E/R/F**, mỗi nhóm chọn 1. Kỹ năng đã chọn tiếp tục tồn tại, không bị thay thế khi lên giai đoạn sau.
5. **Ba nhánh:** Sát thủ, Đao sĩ, Kiếm khí. **Ba lõi R** có thể được chọn bởi cả ba nhánh: Thiên Vẫn Trảm, Tam Thiên Kiếm Quyết, Vạn Ảnh Tuyệt Sát.
6. **Không được tách rời cảm giác animation nhân vật và asset kỹ năng.** Có thể dùng layer/prefab riêng về mặt kỹ thuật nhưng khi xem phải là **một hành động thống nhất**, có lấy đà, ra đòn, va chạm, phục hồi.
7. **PixelFX** (https://pixelfx.io/effects/) chỉ là thư viện tham khảo và có thể tận dụng asset phù hợp; nếu thiếu key pose, frame chuyển tiếp hay VFX cần thiết thì **bổ sung mới**. Kiểm tra giấy phép/quyền sử dụng đối với mọi asset nhập vào dự án.
8. Ưu tiên chất lượng **mượt, logic, rõ lực và nhất quán** trước khi tối ưu tái sử dụng asset. Không dùng VFX dày đặc để che animation lỗi.
9. **Demo trước, gameplay sau**: xây một Unity Skill Preview Scene để thử và chốt hình ảnh, chưa dựng toàn bộ hệ thống HEXCORE hoặc chính thức hóa các thông số chiến đấu.

### 1.1. Cấu trúc kỹ năng

| Giai đoạn | Nhóm | Số lựa chọn | Quy tắc demo |
|---|---|---:|---|
| Kiếm sĩ ban đầu | Q | 3 | Chọn 1; theo nhân vật xuyên suốt |
| Chọn nhánh | E | 9 (3 × 3 nhánh) | Chọn 1 trong 3 của nhánh tương ứng |
| Chọn lõi HEXCORE | R | 3 | Chọn 1; **dùng chung mọi nhánh** |
| Giai đoạn cuối | F | 9 (3 × 3 nhánh) | Chọn 1 trong 3 của nhánh tương ứng |
| **Tổng** | | **24** | **4 ô kỹ năng cho một build** |

> Mốc trước đây: Q dự kiến cấp 4, chọn nhánh E dự kiến cấp 6, R dự kiến cấp 12. **Mốc mở F chưa chốt** và không ảnh hưởng đến sản xuất asset demo.

---

## 2. Phạm vi công việc / Sản phẩm bàn giao

**Có trong scope**

- Kiểm kê và chuẩn hóa sprite/animation của **4 tạo hình**.
- Idle, Run/Walk, chuyển hướng 8 hướng; đánh thường 8 hướng và combo cơ bản theo từng phong cách chiến đấu.
- Animation hoàn chỉnh cho 24 kỹ năng, gồm chuẩn bị → thi triển → va chạm → kết thúc; asset VFX/SFX và camera feedback cần thiết.
- Mốc sự kiện trực quan (`windup`, `release`, `impact`, `recover`) đồng bộ với hình ảnh.
- Unity Demo Scene để chọn tạo hình, hướng, nhánh và kỹ năng; phát lại, xem chậm, ẩn/hiện VFX, đặt mục tiêu giả, xem combo.
- Sprite sheet PNG, animation clip, prefab, cấu hình hiệu ứng, file nguồn khi có, video/GIF review cho mỗi kỹ năng.
- Bảng ma trận tạo hình × trạng thái × hướng × kỹ năng; bảng tình trạng asset, nguồn và giấy phép.

**Không có trong scope**

- Cây nâng cấp, lưu tiến trình, UI tăng cấp/cấp 10–11, điều kiện chọn nhánh/lõi.
- Hệ thống sát thương thật, AI, boss, loot, cân bằng, cooldown chiến đấu chính thức.
- Thay đổi các scene và hệ thống gameplay đang vận hành nếu không thật sự cần cho preview.

---

## 3. Bốn tạo hình nhân vật và animation 8 hướng

### 3.1. Kiểm kê đầu vào bắt buộc

| Mã | Sprite gốc | Các hướng đang có | Idle/Run/Hurt/Death | Cầm vũ khí / pivot | Trạng thái |
|---|---|---|---|---|---|
| `Form_01` | Chờ kiểm kê | Chờ kiểm kê | Chờ kiểm kê | Chờ kiểm kê | Chưa xác nhận |
| `Form_02` | Chờ kiểm kê | Chờ kiểm kê | Chờ kiểm kê | Chờ kiểm kê | Chưa xác nhận |
| `Form_03` | Chờ kiểm kê | Chờ kiểm kê | Chờ kiểm kê | Chờ kiểm kê | Chưa xác nhận |
| `Form_04` | Chờ kiểm kê | Chờ kiểm kê | Chờ kiểm kê | Chờ kiểm kê | Chưa xác nhận |

**Lưu ý:** Trong hội thoại đã có ảnh sprite nhân vật gốc tóc nâu, khăn đỏ, nhưng chưa có đủ hồ sơ kiểm kê cả bốn tạo hình. Không được bịa đặc điểm ngoại hình/vũ khí của ba tạo hình còn lại. Khi bắt đầu sản xuất, gắn tệp nguồn thực tế cho từng mã.

### 3.2. Bộ 8 hướng bắt buộc

- Bắc (`N`), Đông Bắc (`NE`), Đông (`E`), Đông Nam (`SE`), Nam (`S`), Tây Nam (`SW`), Tây (`W`), Tây Bắc (`NW`).
- Các animation **Idle, Run/Walk, Basic Attack** phải được kiểm tra ở cả tám hướng cho mỗi tạo hình tương thích.
- Với kỹ năng, hướng **phát chiêu, nhìn và VFX** phải phù hợp: kỹ năng định hướng phải đúng 8 hướng hoặc xoay 360° theo aim; kỹ năng vòng tròn/AoE có thể dùng asset hướng độc lập nhưng **nhân vật vẫn có pose thi triển đúng hướng**.
- Chỉ được lật sprite khi không làm đổi tay cầm kiếm, giáp lệch vai, khăn, phụ kiện hoặc thứ tự lớp bất hợp lý. Nếu lật gây lỗi silhouette, cần vẽ hướng riêng.
- Pivot chân cố định để nhân vật không bị nhảy vị trí giữa các frame; pivot tay/weapon anchor theo animation để VFX sinh đúng vị trí.

### 3.3. Phân biệt hướng di chuyển và hướng tấn công

**Movement Direction** và **Facing/Aim Direction** là hai biến khác nhau. Ví dụ nhân vật chạy sang trái nhưng chém về phía trên: animation phải chọn một cách thể hiện nhất quán (quay/thay pose, hướng thân/đầu, upper-body overlay nếu art cho phép) thay vì vừa chạy trái vừa xoay toàn bộ sprite tức thời gây giật.

**Bộ kiểm thử tối thiểu:** 8 hướng đứng, 8 hướng chạy, 8 hướng đánh; tám tình huống đổi hướng khi di chuyển; chạy một hướng và ngắm hướng khác; đang đánh chuyển sang kỹ năng, kết thúc kỹ năng trở lại chạy/đánh thường.

### 3.4. Bộ trạng thái nhân vật

| Nhóm | Trạng thái ưu tiên | Yêu cầu |
|---|---|---|
| Locomotion | Idle / Run hoặc Walk / Turn | 8 hướng; chuyển động không trượt chân |
| Combat | Attack Prepare / Swing / Recover | 8 hướng; hợp pose và vũ khí |
| Skill | Wind-up / Cast / Impact / Recovery | Đúng với từng tạo hình và từng chiêu |
| Special | Dash / Vanish / Jump / Land / Guard / Charge | Chỉ tạo khi được sử dụng thật trong kỹ năng |
| Damage | Hurt / Knockback / Death | Giữ sprite và pivot nhất quán; phạm vi hướng xác định khi audit |

### 3.5. Cơ sở đồng bộ nhân vật–VFX

VFX có thể là lớp riêng để chỉnh sửa, nhưng **không bao giờ là một màn trình diễn độc lập khỏi nhân vật**. Mỗi kỹ năng cần định nghĩa:

1. Pose bắt đầu, pose kết thúc và cầu nối với Idle/Run/Basic Attack.
2. Điểm gắn VFX: tay, vũ khí, mũi kiếm, thân, chân, ground impact, mục tiêu.
3. Frame/thời điểm phát hiệu ứng đúng lúc cơ thể truyền lực.
4. Đường di chuyển nhân vật/đòn chém phù hợp vị trí và góc nhìn top-down.
5. Chiều sâu/layer để VFX không vô lý khi nhân vật quay Bắc/Nam.
6. Trường hợp skill bị hủy/cắt ngang trong demo hoặc mục tiêu giả biến mất: trả về pose hợp lệ, không đứng kẹt.

---

## 4. Hệ thống animation đánh thường (Basic Attack)

> Đây là hạng mục **riêng**, bổ sung vào kế hoạch 24 active skill; không được bỏ qua vì chỉ làm VFX kỹ năng.

| Phong cách | Chuỗi đánh thường đề xuất | Cảm giác | Asset chủ yếu |
|---|---|---|---|
| Kiếm sĩ cơ bản | Chém ngang → chém ngược | Cân bằng, dễ đọc | Hai nhịp sword slash; footwork nhẹ |
| Sát thủ | Chém nhanh → chém chéo → đâm | Nhịp nhanh, ngắn, chính xác | Ba pose liên kích, vệt chém ngắn |
| Đao sĩ | Chém ngang nặng → bổ dọc | Mạnh, có sức nặng, phục hồi rõ | Wind-up nặng, cleave, ground hit nhẹ |
| Kiếm khí | Chém ngang → chém chéo kèm sóng kiếm | Kiếm thuật kết hợp năng lượng | Hai swing, weapon trail, release wave |

**Quy tắc kỹ thuật/demo:**

- 8 hướng cho mỗi chuỗi trên những tạo hình tương thích.
- Đòn 2/3 phải **nối trực tiếp** từ tư thế đòn trước; không buộc trở về Idle giữa các nhịp.
- Cho phép vừa di chuyển vừa đánh theo cách hợp lý với nghệ thuật dự án; không để chân trượt trên nền.
- Có mẫu chuyển `Run → Attack`, `Attack → Run`, `Attack → Q/E/R/F`, `Skill → Attack`.
- Điểm phát hiệu ứng được đồng bộ với đường đi lưỡi kiếm; không tự phát sóng kiếm khi nhân vật chưa vung kiếm.
- Tăng giảm playback speed để thử nhịp đánh là đủ ở demo; **chưa cần** kết nối chỉ số tốc đánh thật.

**Không mặc định** mỗi tạo hình tương ứng một phong cách. Ma trận tương thích sẽ quyết định bộ animation nào cần điều chỉnh riêng cho từng form.

---

## 5. Đặc tả 24 kỹ năng chủ động (bản demo hình ảnh)

**Quy ước:** Mọi kỹ năng đều phải có ít nhất `Prepare → Execute → Impact → Recover`. Bảng dưới mô tả hành động, nhịp hình ảnh và nhóm asset cần có. Các con số sát thương/thời gian cooldown ở các bản ý tưởng trước **không phải đầu vào bắt buộc cho giai đoạn demo**. Chiêu hướng mục tiêu phải có target dummy hoặc marker mô phỏng.

### 5.1. Kiếm sĩ ban đầu — Q (3 kỹ năng)

#### Q1 · Kiếm Lướt / Phong Ảnh Trảm

- **Chuyển động:** Hạ trọng tâm → rướn người và lao về trước → vung kiếm theo quãng lướt → giảm tốc, thu kiếm, về di chuyển.
- **VFX:** Lướt/tốc độ, vệt kiếm bám lưỡi, va chạm, bụi chân nhỏ.
- **Cần bổ sung nếu thiếu:** Pose lao và đạp chân, pose dừng/thu kiếm theo hướng.
- **Điểm nghiệm thu:** Đường lướt ăn khớp thân–chân; cuối chiêu không giật trở lại vị trí cũ.

#### Q2 · Hoành Nguyệt Trảm

- **Chuyển động:** Xoay vai lấy đà → vung kiếm xoay một vòng 360° → hãm đà và lấy lại thăng bằng.
- **VFX:** Vệt chém vòng đồng bộ quỹ đạo kiếm; hit spark quanh bán kính.
- **Cần bổ sung nếu thiếu:** Các key pose xoay thân và chuyển chân.
- **Điểm nghiệm thu:** Không gắn vòng tròn chém vào nhân vật đứng yên; nhịp xoay nhất quán khi xem chậm.

#### Q3 · Phản Kiếm Quyết

- **Chuyển động:** Đưa kiếm lên thủ thế → chạm đòn giả lập → đỡ và chuyển trọng tâm → phản kích → trở về tư thế sẵn sàng.
- **VFX:** Tia đỡ đòn, nhát phản kiếm và flash tác động.
- **Cần bổ sung nếu thiếu:** Guard stance, phản đòn, cầu nối từ Guard sang Swing.
- **Điểm nghiệm thu:** Phản kiếm chỉ bắt đầu sau dấu hiệu đỡ đòn; có bản demo trường hợp không bị đánh để thu thế tự nhiên.

### 5.2. Nhánh Sát thủ — E (3 kỹ năng)

#### E1 · Ám Bộ Trảm

- **Chuyển động:** Co người → tăng tốc và tan vào bóng ảnh → xuất hiện sau mục tiêu → hai nhát chém nhanh → dừng đúng hướng.
- **VFX:** Vanish/reappear, dash blur, hai đường kiếm, hit spark.
- **Chú ý:** Đích di chuyển rõ ràng; hai nhát chém là hành động có pose, không chỉ hai tia sáng.

#### E2 · Huyết Ấn

- **Chuyển động:** Vươn kiếm/chém khắc ấn về phía mục tiêu → ấn xuất hiện trên địch → phát sáng theo nhịp → nổ ấn khi người xem kích hoạt sự kiện giả lập.
- **VFX:** Blood rune/mark, pulse, burst.
- **Chú ý:** Cần pose đặt ấn; ấn phải bám mục tiêu đúng vị trí, không bay lơ lửng sai chỗ.

#### E3 · Tàng Ảnh

- **Chuyển động:** Hạ thấp người → khói che, nhân vật mờ đi → di chuyển/ẩn ngắn → tái xuất với một đòn đánh bất ngờ → ổn định.
- **VFX:** Smoke, fade, silhouette/afterimage, ambush slash.
- **Chú ý:** Không khiến nhân vật hiện hình bất ngờ ở tư thế không liên quan tới hướng tấn công.

### 5.3. Nhánh Đao sĩ — E (3 kỹ năng)

#### E1 · Chấn Địa Trảm

- **Chuyển động:** Nâng đao/lấy đà → bổ mạnh xuống đất → khuỵu và bật lại → thu thế.
- **VFX:** Ground slam, nứt đất, đá/bụi, vòng xung kích.
- **Chú ý:** Xung kích xuất phát từ điểm va đất; pose thân mang sức nặng.

#### E2 · Hoành Thiên Đao

- **Chuyển động:** Xoay vai và chân lấy đà → chém ngang rộng → giữ đà một nhịp → hãm động tác.
- **VFX:** Heavy cleave, vệt đao rộng, va chạm nhiều mục tiêu.
- **Chú ý:** Không dùng mỗi việc phóng to hiệu ứng kiếm thường; quỹ đạo vũ khí và cơ thể phải đúng.

#### E3 · Bất Động Chiến Thế

- **Chuyển động:** Dậm chân/giữ thế thủ → aura dâng lên → vài đòn đánh thử trong trạng thái cường hóa → aura tắt và thu thế.
- **VFX:** Battle aura, guard pulse, xung kích nhỏ theo cú vung đao.
- **Chú ý:** Aura luôn đi cùng nhân vật; cường hóa không khóa nhân vật ở tư thế đứng yên nếu đang tấn công.

### 5.4. Nhánh Kiếm khí — E (3 kỹ năng)

#### E1 · Nguyệt Nha Trảm

- **Chuyển động:** Thu kiếm lấy đà → chém theo cung → giải phóng sóng kiếm đúng lúc mũi kiếm vượt điểm phóng → thu thế.
- **VFX:** Crescent wave, sword trail, hit pulse.
- **Chú ý:** Vị trí sinh wave trùng quỹ đạo lưỡi kiếm, không xuất hiện từ ngực nhân vật.

#### E2 · Lưỡng Nghi Kiếm

- **Chuyển động:** Vung chéo lần một → chuyển thân sang lần hai → vung chéo ngược → kết thúc.
- **VFX:** Hai đường slash chéo, tia giao nhau, nổ tại điểm giao.
- **Chú ý:** Giữa hai nhát cần key pose chuyển trọng tâm tự nhiên.

#### E3 · Kiếm Giới

- **Chuyển động:** Hướng/cắm kiếm xuống → pháp trận lan ra → giữ một thế điều khiển ngắn → từng nhịp kiếm khí xuất hiện trong trận → thu kiếm.
- **VFX:** Rune circle, vòng năng lượng, nhịp slash/particle lặp.
- **Chú ý:** Pháp trận có tâm và chiều sâu đúng với mặt sàn top-down; character cast không bị đứng hình vô lý.

### 5.5. Lõi HEXCORE — R dùng chung mọi nhánh (3 kỹ năng)

> Ba R là **hero assets**, sản xuất và duyệt với tiêu chuẩn cao nhất. Cùng một hiệu ứng lõi nhưng animation cơ thể phải tương thích với tạo hình đang sử dụng. Mỗi lõi có điểm nhấn riêng để không giống ba biến thể của cùng một vụ nổ.

#### R1 · THIÊN VẪN TRẢM — giáng xuống / hủy diệt diện rộng

**Trình tự chuyển động**

1. **Anticipation:** Hạ trọng tâm, kéo vũ khí về sau, khăn/tóc chuyển động theo lực.
2. **Takeoff:** Bật mạnh khỏi mặt đất; có bụi và bóng dưới chân.
3. **Airborne:** Nhân vật ở pose bay; xác định marker đáp trên sàn; chuẩn bị lao xuống.
4. **Dive:** Tạo đường năng lượng theo hướng rơi; tư thế chém gắn với đường rơi.
5. **Impact:** Vũ khí chạm đất; camera feedback/hit-stop có chủ đích; đất nứt, xung kích và bụi bắn ra **đúng cùng thời điểm**.
6. **Recovery:** Đứng dậy, rút vũ khí, về thế chiến đấu.

**Asset:** Jump anticipation, takeoff, airborne attack pose, dive trail, landing shadow/marker, ground crack, shockwave, rubble/dust, impact pose, get-up/recovery, SFX đáp đất.

**Điểm WOW:** Người chơi thấy rõ lực bật lên và sức nặng giáng xuống — không phải teleport từ trên cao rồi hiện vụ nổ.

**Kiểm tra:** Điểm đáp và thân nhân vật khớp; chân/weapon không xuyên sai lớp nền; sprite từng tạo hình vẫn nhận diện được ở trên không.

#### R2 · TAM THIÊN KIẾM QUYẾT — tụ lực / thần quang / ba nhát kiếm

**Trình tự chuyển động**

1. **Anticipation:** Đứng thế tụ lực; nguồn sáng dồn quanh kiếm và cơ thể.
2. **First swing:** Nhát thứ nhất phát sóng kiếm, dáng vừa mở thế.
3. **Transition:** Thu kiếm và xoay thân sang nhát thứ hai; không trở về Idle.
4. **Second swing:** Nhát thứ hai mạnh hơn, luồng sáng tăng cường.
5. **Final charge:** Chuyển sang pose giữ năng lượng cho nhát cuối.
6. **Final release:** Vung nhát thứ ba, kiếm khí lớn nhất, nổ trên mục tiêu giả.
7. **Recovery:** Ánh sáng tắt dần; nhân vật hạ vũ khí, về Run/Idle.

**Asset:** Charge pose, aura/halo, ba swing nối nhau, ba swordwave với biến thể kích thước/độ sáng, final impact, end recovery, SFX tụ lực–phóng–kết thúc.

**Điểm WOW:** Nhịp **1 → 2 → 3 tăng dần**, nhát cuối có khoảng nghỉ rất ngắn để nhấn trọng lượng, không kéo dài đến mức đứng khựng.

**Kiểm tra:** Wave chỉ xuất hiện khi kiếm phóng năng lượng; cả ba nhát liền mạch; góc phóng và thứ tự layer hợp với 8 hướng.

#### R3 · VẠN ẢNH TUYỆT SÁT — biến mất / phân thân / liên trảm

**Trình tự chuyển động**

1. **Anticipation:** Nhân vật vào thế tốc độ, khóa hướng hoặc mục tiêu cho bản demo.
2. **Vanish:** Tăng tốc, để dư ảnh và biến mất.
3. **Chain slashes:** Các bóng ảnh của **đúng tạo hình đang dùng** xuất hiện theo các quỹ đạo khác nhau, **mỗi bóng có pose chém thật**.
4. **Cross-cut climax:** Vệt chém giao nhau trong vùng mục tiêu; flash dấu ấn theo nhịp.
5. **Reappear:** Nhân vật thật trở lại vị trí xác định, nối với nhát chém cuối.
6. **Finisher/recovery:** Các dấu chém bùng nổ; nhân vật thu kiếm, ổn định tư thế.

**Asset:** Vanish/reappear, các key pose phân thân đúng theo form, afterimage, nhiều slash trail hướng khác nhau, finisher impact, recovery, âm thanh liên trảm.

**Điểm WOW:** Tốc độ cực nhanh nhưng **vẫn đọc được quỹ đạo và người nào đang ra đòn**, không spam hình nhân vật đứng yên và tia sáng ngẫu nhiên.

**Kiểm tra:** Số bóng ảnh/đường đi khớp chuỗi attack; không nhảy đột ngột về tọa độ cũ; mỗi bóng có cùng tạo hình/trang phục với nhân vật nguồn.

### 5.6. Tuyệt kỹ Sát thủ — F (3 kỹ năng)

#### F1 · Dạ Vũ Liên Trảm

- **Chuyển động:** Ba nhịp lao qua mục tiêu, mỗi nhịp có swing riêng và chuyển hướng mềm → dừng sau nhát cuối.
- **VFX:** Rapid dash, slash trails nối tiếp, afterimage.
- **Chú ý:** Tránh teleport sai vị trí giữa ba nhịp; mỗi hướng lướt cần pose phù hợp.

#### F2 · Đoạt Mệnh

- **Chuyển động:** Tiếp cận từ góc đánh → lấy đà ngắn → một nhát xuyên kích mạnh → dừng phía bên kia mục tiêu.
- **VFX:** Critical slash, piercing flash, finisher spark.
- **Chú ý:** Đòn kết liễu phải có frame tác động rõ, không chỉ sốc sáng màn hình.

#### F3 · Song Ảnh Truy Sát

- **Chuyển động:** Triệu hồi hai bản sao của form hiện tại → cả ba có nhịp tấn công có chủ ý → bóng tan và nhân vật về thế ban đầu.
- **VFX:** Clone spawn, shadow silhouette, copied slash, fade out.
- **Chú ý:** Bản sao phải theo ngoại hình, màu sắc, hướng và animation của form tương ứng.

### 5.7. Tuyệt kỹ Đao sĩ — F (3 kỹ năng)

#### F1 · Cuồng Phong Đao

- **Chuyển động:** Lấy đà → xoay và chém liên tục → hãm đà để dừng.
- **VFX:** Whirlwind, circular slash, trailing arcs.
- **Chú ý:** Phối hợp chân và thân để chuyển động xoay không giống một sprite tĩnh bị xoay bằng code.

#### F2 · Đoạn Sơn Trảm

- **Chuyển động:** Giữ thế vận lực → bổ dọc nặng → va chạm → rút vũ khí.
- **VFX:** Heavy cleave, armor-break/cracked ground, debris.
- **Chú ý:** Phải có thời gian lấy đà và dừng lực đúng điểm chạm.

#### F3 · Bất Diệt Chiến Ý

- **Chuyển động:** Hạ trọng tâm rồi đứng lên trong trạng thái cường hóa → đánh thử vài đòn trong aura → thu thế.
- **VFX:** Empowered aura, attack impact pulse, fade-out.
- **Chú ý:** Aura đi cùng nhân vật khi chạy/đánh; không cắt ngắn animation bất thường.

### 5.8. Tuyệt kỹ Kiếm khí — F (3 kỹ năng)

#### F1 · Kiếm Vũ Thiên Hà

- **Chuyển động:** Hướng kiếm lên cao → triệu hồi các kiếm năng lượng → kiếm rơi theo nhịp xuống vùng chỉ định → hạ kiếm.
- **VFX:** Summon glow, rain of energy blades, multi-hit impacts.
- **Chú ý:** Nhân vật có hành vi điều khiển/triệu hồi, không đứng ngoài cuộc khi mưa kiếm diễn ra.

#### F2 · Phi Kiếm Truy Hồn

- **Chuyển động:** Ra dấu/vung kiếm phóng năng lượng → projectile bay và uốn về mục tiêu giả → va chạm → thu thế.
- **VFX:** Homing swordwave/projectile, curved trail, hit burst.
- **Chú ý:** Đổi hướng của kiếm phải mượt; hình mũi kiếm luôn hướng theo quỹ đạo bay.

#### F3 · Hồi Nguyệt Kiếm

- **Chuyển động:** Vung kiếm phóng sóng → sóng bay ra rồi quay về → nhân vật có pose đón/thu năng lượng → về Idle/Run.
- **VFX:** Outgoing crescent, return trail, catch spark.
- **Chú ý:** Quỹ đạo đi–về có liên hệ trực quan với nhân vật, không tự biến mất ở giữa đường.

---

## 6. Chuẩn chất lượng animation / VFX / SFX

### 6.1. Chuỗi hành động tối thiểu

`Idle/Run → Anticipation → Execution → Impact → Recovery → Idle/Run`  
Chiêu nhiều bước (nhảy, tụ lực, liên kích, phân thân) phải có trạng thái chuyển tiếp riêng. **Không bỏ Recovery**, trừ khi cố ý nối sang động tác kế tiếp và có animation bridge phù hợp.

### 6.2. Lỗi không được chấp nhận

- Nhân vật chạy nhưng VFX tự chém mà không có animation ra đòn.
- Đột ngột đổi sprite sang tư thế không liên quan; trượt chân hoặc nhảy pivot.
- Sóng kiếm xuất hiện ở ngực/giữa người thay vì từ vũ khí mà không có chủ đích.
- VFX tác động trước lúc lưỡi kiếm/motion chạm đích.
- Lật sprite làm đảo tay cầm vũ khí, khăn, giáp hoặc phụ kiện.
- Phân thân không đúng tạo hình hay đứng bất động khi 'chém'.
- Đòn chém theo hướng Bắc nhưng hit flash/quỹ đạo lại hướng Nam.
- Hoàn tất chiêu nhưng kẹt pose hoặc giật về vị trí cũ.
- Dùng VFX quá sáng/đậm để che lỗi chuyển động.

### 6.3. Nhịp và nguyên tắc kỹ thuật

- Có thể thử animation nhân vật ở **12–16 fps theo phong cách pixel art** rồi điều chỉnh sau review; chuyển động vị trí/camera và tween VFX cập nhật mượt theo engine, không ép cố định 12 fps.
- Lưu keyframe `VFXSpawn`, `WeaponRelease`, `Impact`, `FootPlant`, `RecoverEnd` (hoặc tên tương đương) để demo đồng bộ.
- Chỉ dùng screen shake/hit-stop rất ngắn ở điểm nhấn; có tuỳ chọn tắt khi review.
- Camera top-down phải giữ nhân vật và khu vực va chạm trong khung, tránh mất mục tiêu trong chiêu lớn.
- SFX theo nhịp: chuẩn bị, vung/di chuyển, va chạm, kết thúc; không phát nhiều âm thanh sai mốc.
- Có thể làm animation clip và VFX prefab tách file để chỉnh sửa, **nhưng review và nghiệm thu trên bản tổng hợp**.

---

## 7. Nguồn asset và quy tắc tái sử dụng

**Nguồn tham khảo chính:** https://pixelfx.io/effects/  
Lấy những hiệu ứng phù hợp sau khi kiểm tra phiên bản, nguồn gốc và giấy phép hiện hành; **không mặc định thư viện có đủ** tất cả VFX mong muốn hoặc thay thế được animation nhân vật.

| Nhóm tham khảo | Ứng dụng | Chính sách |
|---|---|---|
| Sword slash / Spin slash / Heavy cleave | Basic Attack, Q, E, F | Tái sử dụng khi quỹ đạo vũ khí khớp |
| Dash / Blink / Smoke | Sát thủ và lướt | Chỉ là lớp hỗ trợ cho pose/chuyển động thật |
| Ground slam / Impact / Dust | Đao sĩ, Thiên Vẫn Trảm | Đúng điểm và thời điểm tiếp đất |
| Energy wave / Disc | Kiếm khí, Tam Thiên | Hình dạng và hướng bay phải theo nhát kiếm |
| Rune circle / Charge / Aura | Tụ lực, Kiếm Giới, cường hóa | Đồng bộ tư thế và hướng thi triển |
| Afterimage / Flash | Vạn Ảnh và các chiêu nhanh | Dùng **đúng tạo hình** của nhân vật |

**Bổ sung asset mới khi:** (a) thiếu tư thế truyền lực; (b) không nối được hai pose; (c) VFX không đúng quỹ đạo; (d) sai phối cảnh 2D top-down; (e) hiệu ứng không đọc được ở tám hướng; hoặc (f) chiêu lõi R chưa tạo được điểm nhấn rõ.

**Theo dõi nguồn và giấy phép:** Tên asset, URL, tác giả/nhà phát hành, ngày lấy, loại giấy phép, yêu cầu ghi công, chỉnh sửa đã thực hiện. Không tái phân phối nội dung không có quyền sử dụng.

---

## 8. Unity Skill Preview Scene — công cụ để duyệt, không phải gameplay chính

### 8.1. Chức năng bắt buộc

- Dropdown **Form 01–04** (chỉ hiện form đã nhập asset thật).
- Chọn phong cách **Kiếm sĩ / Sát thủ / Đao sĩ / Kiếm khí**.
- Chọn một trong **24 kỹ năng** tương thích; nút Play/Restart.
- Chọn **8 hướng** và tuỳ chọn ngắm chuột nếu chiêu cần định hướng liên tục.
- Preview **Idle / Run / Basic Attack** độc lập; phát chuỗi `Basic → Skill → Basic`.
- Phát lại 1×, 0,5× và tạm dừng/tua frame nếu có thể.
- Bật/tắt VFX, SFX, camera shake, hit-stop để kiểm tra từng lớp.
- Đặt 1 hoặc nhiều dummy, test target marker, kỹ năng định hướng, xoay camera cố định top-down.
- Hiển thị phase đang chạy: Prepare / Execute / Impact / Recovery; không cần HUD gameplay.
- Cho phép thử nhiều lần không tồn đọng phân thân, hiệu ứng, hitbox/marker hoặc coroutine giả lập.

### 8.2. Dữ liệu mỗi Skill Demo

| Trường | Ý nghĩa |
|---|---|
| `skillId`, `displayName`, `group` | ID, tên và nhóm Q/E/R/F |
| `compatibleForms` | Danh sách form có animation đạt yêu cầu |
| `supportedDirections` | 8 hướng hoặc kiểu định hướng cụ thể |
| `animationSequence` | Các clip và mốc chuyển tiếp |
| `vfxSequence` | Prefab + attach point + timing |
| `sfxSequence` | Âm thanh theo mốc |
| `movementPreview` | Đường đi nhân vật cho demo |
| `targetPreview` | Điểm ngắm, dummy, va chạm giả lập |
| `reviewStatus` | Draft / Needs Revision / Approved |

Các tên trên mang tính **định hướng schema**; khi xây dựng phải kiểm tra code/Animator hiện tại để dùng cơ chế thích hợp, không tự tạo thêm hệ thống trùng lặp.

---

## 9. Lộ trình sản xuất và cổng nghiệm thu

| Đợt | Công việc | Đầu ra cần review | Điều kiện chuyển bước |
|---|---|---|---|
| **0. Audit** | Kiểm kê 4 form, sprite/clip/anchor, 8 hướng, giấy phép asset | Asset inventory và ma trận tương thích | Xác định rõ phần sẵn có/thiếu |
| **1. Locomotion** | Idle, Run/Walk, chuyển hướng 8 hướng | Preview từng form × 8 hướng | Không giật, trượt chân, lệch pivot |
| **2. Basic Attack** | Chuỗi đánh thường cho 4 phong cách, tùy form | Basic Attack 8 hướng và nối Run | Combo liền mạch, slash đúng tay/kiếm |
| **3. Storyboard** | Key pose và timing cho 24 skill | 24 animatic/blockout | Từng hành động có nguyên nhân, va chạm, kết thúc |
| **4. Hero R** | Hoàn thiện 3 lõi R dùng chung | 3 demo R trên form tương thích | Đạt mức WOW, logic và chuyển tiếp |
| **5. Q/E/F** | Hoàn thiện 21 chiêu còn lại | Preview đủ 24 chiêu | Đúng hướng, đúng form, không lặp lỗi |
| **6. Polish** | VFX, SFX, hit feedback, camera, cleanup | Skill Preview Scene hoàn chỉnh | Đáp ứng checklist nghiệm thu |
| **7. Chốt** | Review, sửa và khóa phiên bản duyệt | Video/GIF + asset packs + báo cáo | Từng chiêu được đánh dấu Approved |

**Khuyến nghị sản xuất:** Có thể dựng **bản nháp 24 kỹ năng trước** để chốt động tác, nhưng nên **hoàn thiện chất lượng ba lõi R trước**, sau đó mới polish 21 skill còn lại theo chuẩn đã chốt.

### 9.1. Tiêu chí nghiệm thu cho mỗi skill × form tương thích × hướng hỗ trợ

- [ ] Đúng tạo hình, tỷ lệ, trang phục và vũ khí.
- [ ] Pose chuẩn bị, ra đòn, va chạm và thu thế liên tục, hợp lý.
- [ ] Mọi mốc VFX/SFX theo đúng hành động nhân vật.
- [ ] Hướng ra đòn, hướng vệt kiếm, hit flash, độ sâu lớp chính xác.
- [ ] Không giật pivot, không trượt chân hoặc nhảy lại vị trí cũ.
- [ ] Có thể chạy thử liên tiếp; không kẹt trạng thái hay để sót hiệu ứng.
- [ ] Có thể chuyển Idle/Run ↔ Basic Attack ↔ Skill mượt.
- [ ] Đạt yêu cầu rõ hình khi có nhiều dummy và có bản xem chậm.
- [ ] Tài sản/nguồn/license và file chỉnh sửa được ghi lại.
- [ ] Có preview MP4/GIF và reviewer xác nhận `Approved`.

**Trạng thái duyệt:** `Not Started` → `Blockout` → `In Review` → `Needs Revision` hoặc `Approved`.

---

## 10. Cấu trúc thư mục dự kiến (cần đối chiếu repository thực tế)

```text
Assets/
  HEXCORE/
    Characters/
      Form_01/
      Form_02/
      Form_03/
      Form_04/
    BasicAttacks/
      Swordsman/
      Assassin/
      BladeWarrior/
      SwordEnergy/
    Skills/
      Q_BaseSwordsman/
      E_Assassin/
      E_BladeWarrior/
      E_SwordEnergy/
      R_SharedCores/
      F_Assassin/
      F_BladeWarrior/
      F_SwordEnergy/
    SharedVFX/
      Slash/
      Impact/
      DashBlink/
      ChargeAura/
      Projectile/
      Afterimage/
    Animation/
      Shared/
      FormSpecific/
      Transitions/
    Prefabs/
    Audio/
    Demo/
      Scenes/
      Scripts/
      Configs/
    Documentation/
      AssetInventory.md
      FormSkillMatrix.md
      VFXSourceLicenses.md
      ReviewChecklist.md
      ReviewLog.md
```

**Không được áp đặt** cây thư mục mới mà làm hỏng cấu trúc hiện tại; sau audit mới quyết định vị trí và tên file cuối cùng.

---

## 11. Gợi ý cấu trúc gói bàn giao để chốt asset

```text
Delivery/
  PreviewVideos/
    Q01_PhongAnhTram.mp4
    ...
    R01_ThienVanTram.mp4
    R02_TamThienKiemQuyet.mp4
    R03_VanAnhTuyetSat.mp4
    ...
  SourceAssets/
  ExportedSpriteSheets/
  UnitySkillDemo/
  AssetInventory.md
  FormSkillMatrix.md
  VFXSourceLicenses.md
  ReviewLog.md
```

Đây là **danh mục dự kiến**, không khẳng định những asset/video đã được tạo. Mỗi sản phẩm chỉ được xem là hoàn thành sau khi xuất file và kiểm chứng thật.

---

## 12. Những điểm cần có dữ liệu nguồn trước khi sản xuất chính thức

1. **Đủ 4 tạo hình:** sprite sheet/PSD/PNG nguồn và danh sách animation đang có, kích thước pixel/frame, pivot.
2. **Hướng art thực tế:** art style chính xác, palette, độ lớn nhân vật trên màn hình, kích thước vũ khí; căn theo sprite gốc, không vẽ lại tùy ý.
3. **Tương thích:** form nào dùng bộ chuyển động/nhánh nào; nếu cả 4 form dùng được toàn bộ thì phải duyệt từng form, không mặc định tự động đúng.
4. **Gói PixelFX thực tế:** chỉ nhập những hiệu ứng thực sự có file nguồn và được phép sử dụng; thiếu thì tạo hoặc thay bằng asset khác phù hợp.
5. **Demo output:** Unity project/scene hiện có và phương thức quay video/GIF, để Codex hoặc người triển khai không dựng trùng nền tảng.

---

## 13. Yêu cầu cho bước viết prompt Codex sau khi chốt tài liệu

Bước tiếp theo **chỉ** yêu cầu Codex kiểm tra dự án, chuẩn hóa pipeline asset và triển khai **Skill Preview Scene**. Không được tự ý lập trình hệ thống nâng cấp HEXCORE trong game thật.

Các yêu cầu bắt buộc trong prompt:

- Đọc repo/Animator/asset hiện có trước, không tự giả định dữ liệu.
- Thiết kế **character-integrated skill animation**: nhân vật, vũ khí, VFX, va chạm, phục hồi là một hành động mượt.
- Có 4 form, 8 hướng, Basic Attack và 24 Active Skill trong scope.
- Không nhân vô điều kiện thành 96 sprite sheet riêng; dùng ma trận tương thích, tái sử dụng VFX khi đúng và tạo pose riêng khi cần.
- Chia công việc nhỏ theo từng đợt; mỗi đợt có demo và checklist kiểm chứng.
- Báo cáo rõ tài nguyên đã có, placeholder, tài nguyên còn thiếu; **không tuyên bố một kỹ năng đã hoàn thiện nếu còn dùng placeholder**.
- Không can thiệp gameplay production ngoài những thành phần demo đã phê duyệt.

---

## 14. Kết luận thiết kế

**HEXCORE Asset Demo không phải 24 hiệu ứng rời rạc đặt lên sprite.** Đó là hệ thống biểu diễn chiến đấu nhất quán gồm **4 tạo hình**, **di chuyển 8 hướng**, **đánh thường có combo**, **24 kỹ năng chủ động** và các chuyển tiếp liền mạch giữa mọi trạng thái.

**Nguyên tắc quyết định:** *Thiết kế hành động nhân vật trước, đồng bộ VFX theo hành động đó, nghiệm thu cả hai như một kỹ năng thống nhất.* Đẹp nhưng sai pose/đứt mạch = chưa đạt. Dùng thêm asset khi cần để đạt đúng cảm giác chiến đấu.

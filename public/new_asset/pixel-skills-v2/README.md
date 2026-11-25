# Pixel skills v2 · đã duyệt 2026-11-18

Bảy bản duyệt trong seven-skills-pixel-review-v2 và Lồng đạn trong hexcore-cage-pixel-review là bản tham chiếu hình ảnh. runtime-layers là các lớp xuất riêng từ cùng bộ dựng và cụm lửa/khói nguồn, dùng để nối vị trí thế giới trong Unity; không đưa cả montage xem trước lên một viên đạn đang di chuyển.

Các lớp cài đặt ở Assets/Art/GunSkills/PixelSkills. manifest.json mô tả kích thước frame, số frame, pivot theo góc trái trên và PPU. Bộ nhớ đệm GunNewSkillArt giữ thứ tự 16 lớp theo manifest sau 8 atlas nhập trước đó. GunSkillTest.unity đã nối sẵn pixelSkillSheets. Import Point, không mipmap, không nén, NPOT None, max texture size 8192 để giữ nguyên kích thước atlas.

Code dùng riêng đạn đi/về, đầu nòng/đạn cường hoá, aura/hit phân rã, thân neo/dây/đứt và field/shot/collapse của lồng. Dây neo gồm các sprite pixel nối từ điểm đóng neo đến vị trí địch, thân neo đứng thẳng. Lồng dùng sáu điểm trên cùng bán kính 2.6 của cơ chế cũ, thay footprint elip của bảng preview để các điểm bắn khớp với đường gây sát thương.

Chỉ thay hình ảnh và nhịp đọc frame để khớp gameplay. Giữ tốc độ bắn, nâng cấp, cooldown, số lượt, trigger và sát thương cũ. Băng đạn cộng hưởng vẫn dùng art cũ theo yêu cầu hoãn trước đó.

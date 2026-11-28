# Bảng đối chiếu hình và âm theo 146 mục

Các số là thời điểm bắt đầu lớp theo giây trong **bản phối đầu tiên**. Với đòn có nhiều mục tiêu, mốc h trong WAV là fixture để nghe. Trong gameplay, hit phải đến từ va chạm hoặc sự kiện thật. Dấu trạng thái chỉ ghi mức bằng chứng đã có.

Lớp **Loop rời** cho 45, 47, 48, 105 và 107 được nghe độc lập, ngoài trục mốc của bản phối.

- **Có take nguồn** là video từ runtime của scene nguồn với fixture bia. **Đã chạy 1×/0,5×** nghĩa là bộ chạy đã chọn cấu hình, lấy mẫu ảnh và ghi nhận thông tin runtime. Ảnh mẫu không chứng minh mọi proc, mọi hướng hoặc mọi frame đã được xác nhận.
- **Chờ kiểm tra riêng** gồm các mục chưa có take/fixture riêng trong đợt này. **Chờ gameplay** gồm hành động còn ở trạng thái thiết kế/art, hoặc thiếu đường chạy chính thức. Giữ cue dưới dạng tài liệu tham chiếu.
- [Báo cáo, sai lệch và giới hạn nghe](HEXCORE_AUDIO_QUALITY_REPORT.md)

| ID | Sự kiện | Nguồn hình/VFX | Mốc trong WAV | Tình trạng |
| ---: | --- | --- | --- | --- |
| 1 | Bước chân đi và chạy | CharacterArt Run | Chuẩn bị 0 · Thi triển 0.31, 0.59 · Dư âm / kết thúc 0.87 | Chờ kiểm tra riêng · Giữ bản thử trước |
| 2 | Lướt và dừng lướt | Pose biến thể lướt ở sân Cận chiến | Chuẩn bị 0 · Thi triển 0.09 · Di chuyển / duy trì 0.18 · Dư âm / kết thúc 0.34 | Chờ kiểm tra riêng · Giữ bản thử trước |
| 3 | Bật nhảy và tiếp đất | Pose cất/đáp của Thiên Vẫn Trảm | Chuẩn bị 0 · Thi triển 0.1 · Va chạm / có hiệu lực 0.48 · Dư âm / kết thúc 0.52 | Chờ kiểm tra riêng · Giữ bản thử trước |
| 4 | Bị thương | Scene gameplay/tiến trình hoặc hình gốc theo mục | Thi triển 0 · Va chạm / có hiệu lực 0.02 · Dư âm / kết thúc 0.1 | Chờ kiểm tra riêng · Giữ bản thử trước |
| 5 | Máu thấp | Scene gameplay/tiến trình hoặc hình gốc theo mục | Thi triển 0, 0.24 · Dư âm / kết thúc 0.27 | Chờ kiểm tra riêng · Giữ bản thử trước |
| 6 | Hồi máu thực | Scene gameplay/tiến trình hoặc hình gốc theo mục | Chuẩn bị 0 · Thi triển 0.08 · Va chạm / có hiệu lực 0.17 · Dư âm / kết thúc 0.23 | Chờ kiểm tra riêng · Giữ bản thử trước |
| 7 | Tử trận | CharacterArt Death | Chuẩn bị 0 · Va chạm / có hiệu lực 0.14 · Dư âm / kết thúc 0.22 | Chờ kiểm tra riêng · Giữ bản thử trước |
| 8 | Bắt đầu đợt | Scene gameplay/tiến trình hoặc hình gốc theo mục | Chuẩn bị 0 · Thi triển 0.13, 0.24 · Dư âm / kết thúc 0.39 | Chờ kiểm tra riêng · Giữ bản thử trước |
| 9 | Hoàn thành đợt và chờ chuyển | Scene gameplay/tiến trình hoặc hình gốc theo mục | Thi triển 0, 0.15, 0.31 · Dư âm / kết thúc 0.37 | Chờ kiểm tra riêng · Giữ bản thử trước |
| 10 | Chuyển bản đồ | Scene gameplay/tiến trình hoặc hình gốc theo mục | Chuẩn bị 0 · Thi triển 0.14 · Di chuyển / duy trì 0.33 · Dư âm / kết thúc 0.55 | Chờ kiểm tra riêng · Giữ bản thử trước |
| 11 | Chiến thắng và Chơi lại | Scene gameplay/tiến trình hoặc hình gốc theo mục | Trống & Foley · Bass & hòa âm · Giai điệu | Chờ kiểm tra riêng · Giữ bản thử trước |
| 12 | Thất bại và Chơi lại | Scene gameplay/tiến trình hoặc hình gốc theo mục | Trống & Foley · Bass & hòa âm · Giai điệu | Chờ kiểm tra riêng · Giữ bản thử trước |
| 13 | Tạm dừng và tiếp tục | Scene gameplay/tiến trình hoặc hình gốc theo mục | Thi triển 0, 0.62 · Dư âm / kết thúc 0.05, 0.66 | Chờ kiểm tra riêng · Giữ bản thử trước |
| 14 | Cảnh báo nguy hiểm và báo Boss | Scene gameplay/tiến trình hoặc hình gốc theo mục | Chuẩn bị 0 · Thi triển 0.16, 0.3 | Chờ kiểm tra riêng · Giữ bản thử trước |
| 15 | Nhận EXP từ quái bị hạ | Scene gameplay/tiến trình hoặc hình gốc theo mục | Thi triển 0 · Dư âm / kết thúc 0.08 | Chờ kiểm tra riêng · Giữ bản thử trước |
| 16 | Nhặt EXP rơi và vật phẩm | Scene gameplay/tiến trình hoặc hình gốc theo mục | Chuẩn bị 0 · Thi triển 0.05 · Va chạm / có hiệu lực 0.14 · Dư âm / kết thúc 0.2 | Chờ gameplay · Giữ bản thử trước |
| 17 | Tăng cấp và mở bảng nâng cấp | Scene gameplay/tiến trình hoặc hình gốc theo mục | Thi triển 0, 0.13, 0.27 · Dư âm / kết thúc 0.34 | Chờ kiểm tra riêng · Giữ bản thử trước |
| 18 | Chọn nâng cấp, Sinh lực, Nhanh nhẹn, Tập trung | Scene gameplay/tiến trình hoặc hình gốc theo mục | Cue đơn hoặc nhạc/ambience | Chờ kiểm tra riêng · Giữ bản thử trước |
| 19 | Tiến hóa, mở lõi và thức tỉnh HEXCORE | Scene gameplay/tiến trình hoặc hình gốc theo mục | Chuẩn bị 0 · Thi triển 0.25, 0.39, 0.53 · Va chạm / có hiệu lực 0.68 · Dư âm / kết thúc 0.8 | Chờ kiểm tra riêng · Giữ bản thử trước |
| 20 | Phát bắn cơ bản và đạn trúng | GunSkillTest · pistol tracer | Chuẩn bị 0 · Thi triển 0.1 · Di chuyển / duy trì 0.13 · Va chạm / có hiệu lực 0.2 · Dư âm / kết thúc 0.36 | Đã chạy 1×/0,5× · ảnh mẫu · Giữ bản thử trước |
| 21 | Loạt bắn Shotgun | GunSkillTest · Shotgun | Chuẩn bị 0 · Thi triển 0.19 · Di chuyển / duy trì 0.22 · Va chạm / có hiệu lực 0.28 · Dư âm / kết thúc 0.47 | Đã chạy 1×/0,5× · ảnh mẫu · Giữ bản thử trước |
| 22 | Loạt bắn SMG | GunSkillTest · SMG | Thi triển 0, 0.085, 0.17, 0.255, 0.34 · Di chuyển / duy trì 0.024 · Va chạm / có hiệu lực 0.05, 0.13, 0.3 · Dư âm / kết thúc 0.48 | Đã chạy 1×/0,5× · ảnh mẫu · Giữ bản thử trước |
| 23 | Phóng Rocket và vụ nổ | GunSkillTest · Rocket | Chuẩn bị 0 · Thi triển 0.17 · Di chuyển / duy trì 0.24 · Va chạm / có hiệu lực 0.63 · Dư âm / kết thúc 0.71 | Đã chạy 1×/0,5× · ảnh mẫu · Giữ bản thử trước |
| 24 | Đạn nảy | GunSkillTest · atlas và đạn/VFX của nhánh | Thi triển 0 · Di chuyển / duy trì 0.15, 0.32, 0.49 · Dư âm / kết thúc 0.61 | Đã thử · cần bắt riêng proc · Giữ bản thử trước |
| 25 | Xuyên thấu | GunSkillTest · atlas và đạn/VFX của nhánh | Thi triển 0 · Di chuyển / duy trì 0.09 · Va chạm / có hiệu lực 0.14, 0.23, 0.34 | Đã thử · cần bắt riêng proc · Giữ bản thử trước |
| 26 | Đạn bung toả | GunSkillTest · atlas và đạn/VFX của nhánh | Thi triển 0 · Di chuyển / duy trì 0.06, 0.2 · Va chạm / có hiệu lực 0.26 · Dư âm / kết thúc 0.38 | Đã thử · cần bắt riêng proc · Giữ bản thử trước |
| 27 | Thêm viên, Đạn hẹp, Đẩy lùi | GunSkillTest · atlas và đạn/VFX của nhánh | Thi triển 0, 0.72 · Va chạm / có hiệu lực 1.08 | Đã thử · cần bắt riêng proc · Giữ bản thử trước |
| 28 | Đạn băng, Ném lựu đạn, Thêm đạn | GunSkillTest · atlas và đạn/VFX của nhánh | Chuẩn bị 0 · Thi triển 0.16, 0.31 · Di chuyển / duy trì 0.42 · Va chạm / có hiệu lực 0.68 | Đã thử · cần bắt riêng proc · Giữ bản thử trước |
| 29 | Bùng nổ, Đẩy lùi, Thêm đạn | GunSkillTest · atlas và đạn/VFX của nhánh | Thi triển 0, 0.74 · Di chuyển / duy trì 0.12 · Va chạm / có hiệu lực 0.46, 0.52 · Dư âm / kết thúc 0.86 | Đã thử · cần bắt riêng proc · Giữ bản thử trước |
| 30 | Lưỡi đạn càn quét | GunSkillTest · atlas và đạn/VFX của nhánh | Thi triển 0, 0.12 · Di chuyển / duy trì 0.24 · Va chạm / có hiệu lực 0.36 | Đã chạy 1×/0,5× · ảnh mẫu · Giữ bản thử trước |
| 31 | Áp lực công phá | GunSkillTest · atlas và đạn/VFX của nhánh | Chuẩn bị 0 · Thi triển 0.3, 0.46, 0.62 · Va chạm / có hiệu lực 0.48, 0.64, 0.8 · Dư âm / kết thúc 0.87 | Đã chạy 1×/0,5× · ảnh mẫu · Giữ bản thử trước |
| 32 | Phun lửa | GunSkillTest · atlas và đạn/VFX của nhánh | Chuẩn bị 0 · Thi triển 0.19 · Di chuyển / duy trì 0.24 · Va chạm / có hiệu lực 0.62, 1.1 · Dư âm / kết thúc 1.85 | Đã chạy 1×/0,5× · ảnh mẫu · Giữ bản thử trước |
| 33 | Kíp nổ liên hoàn | GunSkillTest · atlas và đạn/VFX của nhánh | Chuẩn bị 0 · Thi triển 0.14 · Di chuyển / duy trì 0.28, 0.39 · Va chạm / có hiệu lực 0.63 · Dư âm / kết thúc 0.77 | Đã chạy 1×/0,5× · ảnh mẫu · Giữ bản thử trước |
| 34 | Băng đạn cộng hưởng | GunSkillTest · atlas và đạn/VFX của nhánh | Chuẩn bị 0, 0.1 · Thi triển 0.24, 0.45, 0.67, 0.88, 1.09 · Dư âm / kết thúc 1.35 | Đã chạy 1×/0,5× · ảnh mẫu · Giữ bản thử trước |
| 35 | Hồi máu | GunSkillTest · atlas và đạn/VFX của nhánh | Thi triển 0 · Va chạm / có hiệu lực 0.3, 0.87, 1.45 · Dư âm / kết thúc 1.75 | Đã thử · cần bắt riêng proc · Giữ bản thử trước |
| 36 | Vết rách không gian | GunSkillTest · atlas và đạn/VFX của nhánh | Chuẩn bị 0 · Thi triển 0.19 · Di chuyển / duy trì 0.31 · Va chạm / có hiệu lực 0.53, 0.9 · Dư âm / kết thúc 1.18 | Đã chạy 1×/0,5× · ảnh mẫu · Giữ bản thử trước |
| 37 | Xuyên kích hồi quy | GunSkillTest · atlas và đạn/VFX của nhánh | Thi triển 0 · Di chuyển / duy trì 0.1 · Va chạm / có hiệu lực 0.3, 0.6, 0.9, 1.2, 1.5, 1.8, 2.1, 2.4 | Đã chạy 1×/0,5× · ảnh mẫu · Giữ bản thử trước |
| 38 | Thiên thương | GunSkillTest · atlas và đạn/VFX của nhánh | Chuẩn bị 0 · Thi triển 0.23 · Di chuyển / duy trì 0.39 · Va chạm / có hiệu lực 0.55 · Dư âm / kết thúc 0.66 | Đã chạy 1×/0,5× · ảnh mẫu · Giữ bản thử trước |
| 39 | Giáp phản pháo | GunSkillTest · atlas và đạn/VFX của nhánh | Thi triển 0, 0.55 · Di chuyển / duy trì 0.6 · Va chạm / có hiệu lực 0.46 | Đã thử · cần bắt riêng proc · Giữ bản thử trước |
| 40 | Lưỡi cưa bám đuổi | GunSkillTest · atlas và đạn/VFX của nhánh | Chuẩn bị 0 · Thi triển 0.16 · Di chuyển / duy trì 0.21 · Va chạm / có hiệu lực 0.56, 1.17, 1.8 · Dư âm / kết thúc 2.36 | Đã chạy 1×/0,5× · ảnh mẫu · Giữ bản thử trước |
| 41 | Hoả lực leo thang | GunSkillTest · atlas và đạn/VFX của nhánh | Chuẩn bị 0 · Thi triển 0.15, 0.34, 0.46, 0.58 · Va chạm / có hiệu lực 0.65 · Dư âm / kết thúc 0.78 | Đã chạy 1×/0,5× · ảnh mẫu · Giữ bản thử trước |
| 42 | Đạn phân rã | GunSkillTest · atlas và đạn/VFX của nhánh | Chuẩn bị 0 · Thi triển 0.18 · Va chạm / có hiệu lực 0.42, 0.61 · Dư âm / kết thúc 0.72 | Đã chạy 1×/0,5× · ảnh mẫu · Giữ bản thử trước |
| 43 | Cổng xuyên kích | GunSkillTest · atlas và đạn/VFX của nhánh | Chuẩn bị 0 · Thi triển 0.17, 0.28 · Di chuyển / duy trì 0.34 · Va chạm / có hiệu lực 0.68, 1.03, 1.45 · Dư âm / kết thúc 2.65 | Đã chạy 1×/0,5× · ảnh mẫu · Giữ bản thử trước |
| 44 | Mỏ neo xuyên tâm | GunSkillTest · atlas và đạn/VFX của nhánh | Chuẩn bị 0 · Thi triển 0.16 · Di chuyển / duy trì 0.28 · Va chạm / có hiệu lực 0.91 · Dư âm / kết thúc 1 | Đã chạy 1×/0,5× · ảnh mẫu · Giữ bản thử trước |
| 45 | Lốc lửa | GunSkillTest · atlas và đạn/VFX của nhánh | Thi triển 0, 0.78, 2.2, 4.06, 5.61 · Di chuyển / duy trì 0.06 · Va chạm / có hiệu lực 4.48 · Dư âm / kết thúc 6.82 | Có take nguồn · Phối mới, chờ nghe |
| 46 | Mưa thiên thạch | GunSkillTest · atlas và đạn/VFX của nhánh | Thi triển 0.475, 0.695, 0.915, 1.135, 1.355 · Di chuyển / duy trì 0, 0.22, 0.44, 0.66, 0.88 · Va chạm / có hiệu lực 0.475, 0.695, 0.915, 1.135, 1.355 · Dư âm / kết thúc 1.38 | Có take nguồn · Phối mới, chờ nghe |
| 47 | Pháo đài cơ động | GunSkillTest · atlas và đạn/VFX của nhánh | Thi triển 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14 · Di chuyển / duy trì 0.12 · Va chạm / có hiệu lực 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14 · Dư âm / kết thúc 14.92 | Có take nguồn · Phối mới, chờ nghe |
| 48 | Lồng đạn Hexcore | GunSkillTest · atlas và đạn/VFX của nhánh | Thi triển 0, 0.44, 0.72, 1, 1.28, 1.56, 1.84 · Di chuyển / duy trì 0.13 · Va chạm / có hiệu lực 0.44, 0.72, 1, 1.28, 1.56, 1.84 · Dư âm / kết thúc 2.14 | Có take nguồn · Phối mới, chờ nghe |
| 49 | Đánh thường Kiếm sĩ | Rig Cận chiến · sprite/timeline + VFX gốc | Thi triển 0, 0.37 · Va chạm / có hiệu lực 0.1, 0.47 | Đã chạy 1×/0,5× · ảnh mẫu · Giữ bản thử trước |
| 50 | Đánh thường Sát thủ | Rig Cận chiến · sprite/timeline + VFX gốc | Thi triển 0, 0.28, 0.54 · Va chạm / có hiệu lực 0.08, 0.36, 0.62 | Đã chạy 1×/0,5× · ảnh mẫu · Giữ bản thử trước |
| 51 | Đánh thường Đao sĩ | Rig Cận chiến · sprite/timeline + VFX gốc | Chuẩn bị 0 · Thi triển 0.1, 0.52 · Va chạm / có hiệu lực 0.23, 0.68 · Dư âm / kết thúc 0.74 | Đã chạy 1×/0,5× · ảnh mẫu · Giữ bản thử trước |
| 52 | Đánh thường Kiếm khí | Rig Cận chiến · sprite/timeline + VFX gốc | Thi triển 0, 0.42 · Di chuyển / duy trì 0.05, 0.47 · Va chạm / có hiệu lực 0.2, 0.62 | Đã chạy 1×/0,5× · ảnh mẫu · Giữ bản thử trước |
| 53 | Phong Ảnh Trảm (Kiếm Lướt) | Pose Kiếm Lướt cũ | Chuẩn bị 0 · Thi triển 0.225, 0.45 · Di chuyển / duy trì 0.3 · Va chạm / có hiệu lực 0.525 · Dư âm / kết thúc 0.675 | Đã chạy 1×/0,5× · ảnh mẫu · Phối mới, chờ nghe |
| 54 | Hoành Nguyệt Trảm | Vòng proc Rev2 | Thi triển 0 · Di chuyển / duy trì 0.14 · Va chạm / có hiệu lực 0.24 · Dư âm / kết thúc 0.4 | Đã thử · cần bắt riêng proc · Phối mới, chờ nghe |
| 55 | Phản Kiếm Quyết | Proc phản kiếm Rev2 | Thi triển 0.14, 0.36 · Va chạm / có hiệu lực 0.48 · Dư âm / kết thúc 0.53 | Đã thử · cần bắt riêng proc · Phối mới, chờ nghe |
| 56 | Ám Bộ Trảm | Rig Cận chiến · sprite/timeline + VFX gốc | Chuẩn bị 0 · Thi triển 0.28, 0.5 · Di chuyển / duy trì 0.17 · Va chạm / có hiệu lực 0.36, 0.59 | Đã chạy 1×/0,5× · ảnh mẫu · Phối mới, chờ nghe |
| 57 | Huyết Ấn | Rig Cận chiến · sprite/timeline + VFX gốc | Chuẩn bị 0.4 · Thi triển 0 · Va chạm / có hiệu lực 0.18, 0.27, 0.36, 0.68, 0.76 · Dư âm / kết thúc 0.89 | Đã chạy 1×/0,5× · ảnh mẫu · Giữ bản thử trước |
| 58 | Dạ Vũ Liên Trảm | Rig Cận chiến · sprite/timeline + VFX gốc | Thi triển 0.11, 0.4, 0.7 · Di chuyển / duy trì 0, 0.31, 0.6 · Va chạm / có hiệu lực 0.17, 0.46, 0.8 | Đã chạy 1×/0,5× · ảnh mẫu · Giữ bản thử trước |
| 59 | Tàng Ảnh | Rig Cận chiến · sprite/timeline + VFX gốc | Chuẩn bị 0 · Thi triển 0.47 · Di chuyển / duy trì 0.15, 0.34 · Va chạm / có hiệu lực 0.59 · Dư âm / kết thúc 0.64 | Đã chạy 1×/0,5× · ảnh mẫu · Giữ bản thử trước |
| 60 | Đoạt Mệnh | Rig Cận chiến · sprite/timeline + VFX gốc | Chuẩn bị 0 · Thi triển 0.26 · Di chuyển / duy trì 0.17 · Va chạm / có hiệu lực 0.41, 0.48 · Dư âm / kết thúc 0.52 | Đã chạy 1×/0,5× · ảnh mẫu · Giữ bản thử trước |
| 61 | Song Ảnh Truy Sát | Rig Cận chiến · sprite/timeline + VFX gốc | Chuẩn bị 0 · Thi triển 0.2, 0.22 · Di chuyển / duy trì 0.27 · Va chạm / có hiệu lực 0.36, 0.39, 0.48 | Đã chạy 1×/0,5× · ảnh mẫu · Giữ bản thử trước |
| 62 | Chấn Địa Trảm | Rig Cận chiến · sprite/timeline + VFX gốc | Chuẩn bị 0 · Thi triển 0.24, 0.41 · Va chạm / có hiệu lực 0.43 · Dư âm / kết thúc 0.46 | Đã chạy 1×/0,5× · ảnh mẫu · Phối mới, chờ nghe |
| 63 | Hoành Thiên Đao | Rig Cận chiến · sprite/timeline + VFX gốc | Chuẩn bị 0 · Thi triển 0.21 · Di chuyển / duy trì 0.37 · Va chạm / có hiệu lực 0.56 · Dư âm / kết thúc 0.59 | Đã chạy 1×/0,5× · ảnh mẫu · Giữ bản thử trước |
| 64 | Cuồng Phong Đao | Rig Cận chiến · sprite/timeline + VFX gốc | Chuẩn bị 0 · Thi triển 0.16, 0.55 · Va chạm / có hiệu lực 0.3, 0.69 · Dư âm / kết thúc 0.73 | Đã chạy 1×/0,5× · ảnh mẫu · Giữ bản thử trước |
| 65 | Bá Vương Đao Vực (thay slot Bất Động Chiến Thế) | Rig Cận chiến · sprite/timeline + VFX gốc | Chuẩn bị 0 · Thi triển 0.45 · Di chuyển / duy trì 0.17 · Va chạm / có hiệu lực 0.6 | Đã chạy 1×/0,5× · ảnh mẫu · Giữ bản thử trước |
| 66 | Đoạn Sơn Trảm | Rig Cận chiến · sprite/timeline + VFX gốc | Chuẩn bị 0, 0.19 · Thi triển 0.43, 0.64 · Va chạm / có hiệu lực 0.67 · Dư âm / kết thúc 0.73, 0.84, 1.05 | Đã chạy 1×/0,5× · ảnh mẫu · Giữ bản thử trước |
| 67 | Bất Diệt Chiến Ý | Rig Cận chiến · sprite/timeline + VFX gốc | Thi triển 0, 0.56, 1.23 · Di chuyển / duy trì 0.75, 1.43 · Va chạm / có hiệu lực 0.7, 1.37 · Dư âm / kết thúc 1.8 | Đã chạy 1×/0,5× · ảnh mẫu · Giữ bản thử trước |
| 68 | Nguyệt Nha Trảm | Rig Cận chiến · sprite/timeline + VFX gốc | Thi triển 0 · Di chuyển / duy trì 0.13 · Va chạm / có hiệu lực 0.32 · Dư âm / kết thúc 0.4 | Đã chạy 1×/0,5× · ảnh mẫu · Phối mới, chờ nghe |
| 69 | Lưỡng Nghi Kiếm | Rig Cận chiến · sprite/timeline + VFX gốc | Thi triển 0, 0.29 · Di chuyển / duy trì 0.36 · Va chạm / có hiệu lực 0.53, 0.55 · Dư âm / kết thúc 0.65 | Đã chạy 1×/0,5× · ảnh mẫu · Giữ bản thử trước |
| 70 | Kiếm Vũ Thiên Hà | Rig Cận chiến · sprite/timeline + VFX gốc | Chuẩn bị 0 · Di chuyển / duy trì 0.3, 0.35, 0.4, 0.45, 0.5, 0.57, 0.62, 0.67, 0.72, 0.77, 0.84, 0.89, 0.94, 0.99, 1.04 · Va chạm / có hiệu lực 0.42, 0.62, 0.69, 0.89, 0.96, 1.16 · Dư âm / kết thúc 1.22 | Đã chạy 1×/0,5× · ảnh mẫu · Giữ bản thử trước |
| 71 | Thiên Kiếm Xuyên Tâm (thay slot Kiếm Giới) | Rig Cận chiến · sprite/timeline + VFX gốc | Chuẩn bị 0 · Di chuyển / duy trì 0.23 · Va chạm / có hiệu lực 0.62, 0.64 · Dư âm / kết thúc 0.7 | Đã chạy 1×/0,5× · ảnh mẫu · Giữ bản thử trước |
| 72 | Phi Kiếm Truy Hồn | Rig Cận chiến · sprite/timeline + VFX gốc | Thi triển 0 · Di chuyển / duy trì 0.08 · Va chạm / có hiệu lực 0.21, 0.38, 0.55, 0.71, 0.86 | Đã chạy 1×/0,5× · ảnh mẫu · Giữ bản thử trước |
| 73 | Hồi Nguyệt Kiếm | Rig Cận chiến · sprite/timeline + VFX gốc | Thi triển 0 · Di chuyển / duy trì 0.12, 0.56 · Va chạm / có hiệu lực 0.31, 0.7 · Dư âm / kết thúc 0.86 | Đã chạy 1×/0,5× · ảnh mẫu · Giữ bản thử trước |
| 74 | Thiên Vẫn Trảm | Rig Cận chiến · sprite/timeline + VFX gốc | Chuẩn bị 0, 0.11 · Thi triển 0.53, 0.77 · Di chuyển / duy trì 0.4 · Va chạm / có hiệu lực 0.79 · Dư âm / kết thúc 0.81, 0.96, 1.2 | Đã chạy 1×/0,5× · ảnh mẫu · Phối mới, chờ nghe |
| 75 | Tam Thiên Kiếm Quyết | Rig Cận chiến · sprite/timeline + VFX gốc | Chuẩn bị 0, 1.44 · Thi triển 0.38, 0.97, 1.81 · Va chạm / có hiệu lực 0.59, 1.24, 2.12 · Dư âm / kết thúc 2.17 | Đã chạy 1×/0,5× · ảnh mẫu · Phối mới, chờ nghe |
| 76 | Vạn Ảnh Tuyệt Sát | Rig Cận chiến · sprite/timeline + VFX gốc | Chuẩn bị 0 · Thi triển 0.29, 0.36, 0.43, 0.5, 0.76, 0.83, 0.9, 0.97, 1.3, 1.42 · Di chuyển / duy trì 1.14 · Va chạm / có hiệu lực 0.51, 0.98, 1.44 · Dư âm / kết thúc 1.55 | Đã chạy 1×/0,5× · ảnh mẫu · Phối mới, chờ nghe |
| 77 | Đạn phép cơ bản | Animation/đạn phép cơ bản cần fixture riêng | Thi triển 0 · Di chuyển / duy trì 0.07 · Va chạm / có hiệu lực 0.23 | Chờ kiểm tra riêng · Giữ bản thử trước |
| 78 | Hồi phục | MageEffectLibrary · Heal | Chuẩn bị 0 · Thi triển 0.18 · Va chạm / có hiệu lực 0.32 · Dư âm / kết thúc 0.47 | Đã thử · cần bắt riêng proc · Giữ bản thử trước |
| 79 | Sóng xung kích | MageEffectLibrary · Shockwave | Chuẩn bị 0 · Thi triển 0.18 · Va chạm / có hiệu lực 0.25 · Dư âm / kết thúc 0.35 | Đã thử · cần bắt riêng proc · Giữ bản thử trước |
| 80 | Vùng trọng lực | MageEffectLibrary · Gravity | Chuẩn bị 0 · Thi triển 0.23 · Di chuyển / duy trì 0.35 · Dư âm / kết thúc 4.34 | Đã thử · cần bắt riêng proc · Giữ bản thử trước |
| 81 | Cầu lửa và thiêu đốt | MageEffectLibrary · Fireball + Explosion + Burn | Thi triển 0 · Di chuyển / duy trì 0.04 · Va chạm / có hiệu lực 0.2, 0.28 | Đã chạy 1×/0,5× · ảnh mẫu · Phối mới, chờ nghe |
| 82 | Sức nổ, Sức cháy, Tầm nổ | Magical · atlas nguyên tố và trạng thái | Di chuyển / duy trì 1.24 · Va chạm / có hiệu lực 0 · Dư âm / kết thúc 0.65 | Đã chạy 1×/0,5× · ảnh mẫu · Giữ bản thử trước |
| 83 | Cháy lâu | Magical · atlas nguyên tố và trạng thái | Thi triển 0 · Di chuyển / duy trì 0.15 · Dư âm / kết thúc 1.77 | Đã chạy 1×/0,5× · ảnh mẫu · Giữ bản thử trước |
| 84 | Thêm cầu lửa | Magical · atlas nguyên tố và trạng thái | Thi triển 0, 0.14, 0.27 · Va chạm / có hiệu lực 0.51, 0.61, 0.72 | Đã chạy 1×/0,5× · ảnh mẫu · Giữ bản thử trước |
| 85 | Tập trung đốt | Magical · atlas nguyên tố và trạng thái | Chuẩn bị 0 · Va chạm / có hiệu lực 0.22 | Đã chạy 1×/0,5× · ảnh mẫu · Giữ bản thử trước |
| 86 | Nổ khi chết | Magical · atlas nguyên tố và trạng thái | Va chạm / có hiệu lực 0 · Dư âm / kết thúc 0.12, 0.24 | Đã chạy 1×/0,5× · ảnh mẫu · Giữ bản thử trước |
| 87 | Hơi thở băng, làm chậm và đóng băng | MageEffectLibrary · IceBreath và Ice Mark | Thi triển 0, 0.065 · Va chạm / có hiệu lực 0 · Dư âm / kết thúc 0.24 | Đã chạy 1×/0,5× · ảnh mẫu · Phối mới, chờ nghe |
| 88 | Làm chậm, Đóng băng, Sát thương lạnh | Magical · atlas nguyên tố và trạng thái | Thi triển 0 · Va chạm / có hiệu lực 0.5, 1.02 | Đã chạy 1×/0,5× · ảnh mẫu · Giữ bản thử trước |
| 89 | Lạnh sâu | Magical · atlas nguyên tố và trạng thái | Thi triển 0 · Va chạm / có hiệu lực 0.28 · Dư âm / kết thúc 0.36 | Đã chạy 1×/0,5× · ảnh mẫu · Giữ bản thử trước |
| 90 | Tụ Hàn | Magical · atlas nguyên tố và trạng thái | Thi triển 0, 0.35 · Dư âm / kết thúc 0.4 | Đã chạy 1×/0,5× · ảnh mẫu · Giữ bản thử trước |
| 91 | Giòn vỡ | Magical · atlas nguyên tố và trạng thái | Chuẩn bị 0 · Va chạm / có hiệu lực 0.11, 0.14 · Dư âm / kết thúc 0.2 | Đã chạy 1×/0,5× · ảnh mẫu · Giữ bản thử trước |
| 92 | Lan băng | Magical · atlas nguyên tố và trạng thái | Thi triển 0 · Di chuyển / duy trì 0.1, 0.24 · Va chạm / có hiệu lực 0.36 | Đã chạy 1×/0,5× · ảnh mẫu · Giữ bản thử trước |
| 93 | Cột sáng | MageEffectLibrary · LightPillar | Thi triển 0, 0.25, 0.5, 0.75 · Va chạm / có hiệu lực 0, 0.25, 0.5, 0.75 · Dư âm / kết thúc 0.79 | Đã chạy 1×/0,5× · ảnh mẫu · Phối mới, chờ nghe |
| 94 | Thêm nhịp, Tầm cột sáng, Sức nhịp | Magical · atlas nguyên tố và trạng thái | Thi triển 0 · Di chuyển / duy trì 0.53 · Va chạm / có hiệu lực 1.08 | Đã chạy 1×/0,5× · ảnh mẫu · Giữ bản thử trước |
| 95 | Dư quang | Magical · atlas nguyên tố và trạng thái | Thi triển 0 · Va chạm / có hiệu lực 0.33 · Dư âm / kết thúc 0.4 | Đã chạy 1×/0,5× · ảnh mẫu · Giữ bản thử trước |
| 96 | Phán Quyết | Magical · atlas nguyên tố và trạng thái | Chuẩn bị 0.42 · Thi triển 0, 0.25 · Va chạm / có hiệu lực 0.5 · Dư âm / kết thúc 0.57 | Đã chạy 1×/0,5× · ảnh mẫu · Giữ bản thử trước |
| 97 | Dẫn sáng | Magical · atlas nguyên tố và trạng thái | Thi triển 0 · Di chuyển / duy trì 0.07 · Dư âm / kết thúc 0.16 | Đã chạy 1×/0,5× · ảnh mẫu · Giữ bản thử trước |
| 98 | Chói mắt | Magical · atlas nguyên tố và trạng thái | Thi triển 0 · Va chạm / có hiệu lực 0.1 · Dư âm / kết thúc 0.19 | Đã chạy 1×/0,5× · ảnh mẫu · Giữ bản thử trước |
| 99 | Phi đạn bóng tối và lời nguyền | MageEffectLibrary · DarkMissile và Curse | Thi triển 0 · Di chuyển / duy trì 0.055 · Va chạm / có hiệu lực 0.22, 0.31 | Đã chạy 1×/0,5× · ảnh mẫu · Phối mới, chờ nghe |
| 100 | Số mục tiêu, Sức nguyền, Suy yếu | Magical · atlas nguyên tố và trạng thái | Va chạm / có hiệu lực 0, 0.43 · Dư âm / kết thúc 0.84 | Đã chạy 1×/0,5× · ảnh mẫu · Giữ bản thử trước |
| 101 | Đa ấn | Magical · atlas nguyên tố và trạng thái | Thi triển 0 · Va chạm / có hiệu lực 0.08, 0.15, 0.23 · Dư âm / kết thúc 0.29 | Đã chạy 1×/0,5× · ảnh mẫu · Giữ bản thử trước |
| 102 | Dày Vò | Magical · atlas nguyên tố và trạng thái | Thi triển 0, 0.23 · Va chạm / có hiệu lực 0.49 · Dư âm / kết thúc 0.53 | Đã chạy 1×/0,5× · ảnh mẫu · Giữ bản thử trước |
| 103 | Suy nhược | Magical · atlas nguyên tố và trạng thái | Thi triển 0 · Va chạm / có hiệu lực 0.13 · Dư âm / kết thúc 0.19 | Đã chạy 1×/0,5× · ảnh mẫu · Giữ bản thử trước |
| 104 | Di chúc | Magical · atlas nguyên tố và trạng thái | Chuẩn bị 0 · Di chuyển / duy trì 0.14 · Va chạm / có hiệu lực 0.32 | Đã chạy 1×/0,5× · ảnh mẫu · Giữ bản thử trước |
| 105 | Ngưng Đọng Thời Gian | MageEffectLibrary · TimeStop | Thi triển 0 · Di chuyển / duy trì 0.07 · Va chạm / có hiệu lực 3.02 · Dư âm / kết thúc 3 | Có take nguồn · Phối mới, chờ nghe |
| 106 | Thiên Tinh Giáng Lâm | MageEffectLibrary · RuneCircle + StarFall | Chuẩn bị 0 · Thi triển 0, 0.4, 0.8, 1.2, 1.6 · Va chạm / có hiệu lực 0, 0.4, 0.8, 1.2, 1.6 · Dư âm / kết thúc 1.63 | Có take nguồn · Phối mới, chờ nghe |
| 107 | Thiên Cầu Ma Lực | MageEffectLibrary · ArcaneOrb + ArcaneMissiles | Thi triển 0, 8 · Di chuyển / duy trì 0.08, 8 · Va chạm / có hiệu lực 8.4, 8.53 · Dư âm / kết thúc 8.57 | Có take nguồn · Phối mới, chờ nghe |
| 108 | Lính Xanh, Nâu, Đỏ, Tuyết xuất hiện và đuổi | Prefab/sprite quái | Thi triển 0 · Di chuyển / duy trì 0.2, 0.51, 0.82 | Chờ kiểm tra riêng · Giữ bản thử trước |
| 109 | Lấy đà, chém và trúng người chơi | Prefab/sprite quái | Chuẩn bị 0 · Thi triển 0.35 · Va chạm / có hiệu lực 0.42 | Chờ kiểm tra riêng · Giữ bản thử trước |
| 110 | Trúng đòn và gục | Prefab/sprite quái | Chuẩn bị 0 · Va chạm / có hiệu lực 0.12 · Dư âm / kết thúc 0.28 | Chờ kiểm tra riêng · Giữ bản thử trước |
| 111 | Tăng sức theo tuổi và hiệu ứng khống chế | Prefab/sprite quái | Thi triển 0 · Va chạm / có hiệu lực 0.49 · Dư âm / kết thúc 0.91 | Chờ kiểm tra riêng · Giữ bản thử trước |
| 112 | Căng dây, nhả tên, tên bay và va | Prefab/sprite quái | Chuẩn bị 0 · Thi triển 0.3 · Di chuyển / duy trì 0.39 · Va chạm / có hiệu lực 0.58 | Chờ kiểm tra riêng · Giữ bản thử trước |
| 113 | Hình thái cung thủ trong Guide và art | Prefab/sprite quái | Chuẩn bị 0, 0.3, 0.54 · Thi triển 0.84 | Chờ gameplay · Giữ bản thử trước |
| 114 | Bước, lấy đà, chém, trúng và gục | Prefab/sprite quái | Chuẩn bị 0.19 · Thi triển 0.54 · Di chuyển / duy trì 0 · Va chạm / có hiệu lực 0.66 · Dư âm / kết thúc 0.89 | Chờ gameplay · Giữ bản thử trước |
| 115 | Loạt quạt đạn và lao tới | GunSkillTest · GunLabBoss | Chuẩn bị 0 · Thi triển 0.3 · Di chuyển / duy trì 0.33, 0.35, 0.37, 0.39, 0.41 · Va chạm / có hiệu lực 0.99 · Dư âm / kết thúc 0.75 | Đã chạy 1×/0,5× · ảnh mẫu · Giữ bản thử trước |
| 116 | Xuất hiện, đổi mạng, hồi lại và bại trận | Prefab/sprite quái | Chuẩn bị 0 · Thi triển 0.5, 1.1 · Va chạm / có hiệu lực 1.6 · Dư âm / kết thúc 1.82 | Chờ gameplay · Giữ bản thử trước |
| 117 | Lướt áp sát | Prefab/Sprite Boss · SkillAssets | Chuẩn bị 0 · Thi triển 0.21 · Dư âm / kết thúc 0.44 | Chờ gameplay · Giữ bản thử trước |
| 118 | Xoay kiếm vòng tròn | Prefab/Sprite Boss · SkillAssets | Chuẩn bị 0 · Thi triển 0.2 · Di chuyển / duy trì 0.3 · Va chạm / có hiệu lực 0.52 | Chờ gameplay · Giữ bản thử trước |
| 119 | Nhảy tới rồi gầm | Prefab/Sprite Boss · SkillAssets | Chuẩn bị 0 · Di chuyển / duy trì 0.2 · Va chạm / có hiệu lực 0.42 · Dư âm / kết thúc 0.54 | Chờ gameplay · Giữ bản thử trước |
| 120 | Phun lửa hình nón | Prefab/Sprite Boss · SkillAssets | Chuẩn bị 0 · Thi triển 0.3 · Di chuyển / duy trì 0.42 · Dư âm / kết thúc 1.85 | Chờ gameplay · Giữ bản thử trước |
| 121 | Hồi máu dung nham | Prefab/Sprite Boss · SkillAssets | Chuẩn bị 0 · Di chuyển / duy trì 0.16 · Va chạm / có hiệu lực 0.41, 0.56 · Dư âm / kết thúc 0.66 | Chờ gameplay · Giữ bản thử trước |
| 122 | Ném cầu lửa, phân tách và hố dung nham | Prefab/Sprite Boss · SkillAssets | Chuẩn bị 0 · Thi triển 0.24 · Di chuyển / duy trì 0.46, 0.56 · Va chạm / có hiệu lực 0.7 · Dư âm / kết thúc 0.78 | Chờ gameplay · Giữ bản thử trước |
| 123 | Ném cầu tuyết | Prefab/Sprite Boss · SkillAssets | Chuẩn bị 0 · Thi triển 0.2 · Di chuyển / duy trì 0.3 · Va chạm / có hiệu lực 0.55 · Dư âm / kết thúc 0.63 | Chờ gameplay · Giữ bản thử trước |
| 124 | Cảnh báo đóng băng và nhà tù băng | Prefab/Sprite Boss · SkillAssets | Chuẩn bị 0, 0.29 · Thi triển 0.65 · Va chạm / có hiệu lực 0.75 · Dư âm / kết thúc 1.34 | Chờ gameplay · Giữ bản thử trước |
| 125 | Gồng và bão tuyết | Prefab/Sprite Boss · SkillAssets | Chuẩn bị 0 · Thi triển 0.43 · Di chuyển / duy trì 0.55 · Dư âm / kết thúc 2.07 | Chờ gameplay · Giữ bản thử trước |
| 126 | Hover và focus bàn phím | MainMenu/Guide · UI hoặc nút Studio | Cue đơn hoặc nhạc/ambience | Chờ kiểm tra riêng · Giữ bản thử trước |
| 127 | Nhấn, xác nhận và hủy | MainMenu/Guide · UI hoặc nút Studio | Chuẩn bị 0 · Thi triển 0.34, 0.72 | Chờ kiểm tra riêng · Giữ bản thử trước |
| 128 | Bắt đầu | MainMenu/Guide · UI hoặc nút Studio | Chuẩn bị 0 · Thi triển 0.13, 0.29 · Dư âm / kết thúc 0.4 | Chờ kiểm tra riêng · Giữ bản thử trước |
| 129 | Chơi tiếp và trạng thái khóa | MainMenu/Guide · UI hoặc nút Studio | Thi triển 0, 0.48 · Dư âm / kết thúc 0.54 | Chờ kiểm tra riêng · Giữ bản thử trước |
| 130 | Chọn màn và duyệt thẻ | MainMenu/Guide · UI hoặc nút Studio | Thi triển 0 · Dư âm / kết thúc 0.07 | Chờ kiểm tra riêng · Giữ bản thử trước |
| 131 | Hướng dẫn, chuyển tab và lọc quái | MainMenu/Guide · UI hoặc nút Studio | Cue đơn hoặc nhạc/ambience | Chờ kiểm tra riêng · Giữ bản thử trước |
| 132 | Cài đặt, thanh âm lượng, mute và khôi phục | MainMenu/Guide · UI hoặc nút Studio | Cue đơn hoặc nhạc/ambience | Chờ kiểm tra riêng · Giữ bản thử trước |
| 133 | Thoát, cảnh báo xác nhận và quay lại | MainMenu/Guide · UI hoặc nút Studio | Thi triển 0, 0.43 · Dư âm / kết thúc 0.79 | Chờ kiểm tra riêng · Giữ bản thử trước |
| 134 | Forest | Ảnh biome Forest/Moon/Inferno/Tundra | Gió / không gian · Nền địa hình · Điểm môi trường | Chờ kiểm tra riêng · Giữ bản thử trước |
| 135 | Moon | Ảnh biome Forest/Moon/Inferno/Tundra | Gió / không gian · Nền địa hình · Điểm môi trường | Chờ kiểm tra riêng · Giữ bản thử trước |
| 136 | Inferno | Ảnh biome Forest/Moon/Inferno/Tundra | Gió / không gian · Nền địa hình · Điểm môi trường | Chờ kiểm tra riêng · Giữ bản thử trước |
| 137 | Tundra | Ảnh biome Forest/Moon/Inferno/Tundra | Gió / không gian · Nền địa hình · Điểm môi trường | Chờ kiểm tra riêng · Giữ bản thử trước |
| 138 | Main Menu · Đại chiến sinh tồn | Concept B hoặc màn kết quả | Trống & Foley · Bass & hòa âm · Giai điệu | Chờ kiểm tra riêng · Giữ bản thử trước |
| 139 | Chiến đấu bốn map | Concept B hoặc màn kết quả | Trống & Foley · Bass & hòa âm · Giai điệu | Chờ kiểm tra riêng · Giữ bản thử trước |
| 140 | Boss và cao trào | Concept B hoặc màn kết quả | Trống & Foley · Bass & hòa âm · Giai điệu | Chờ kiểm tra riêng · Giữ bản thử trước |
| 141 | Kết thúc trận, chiến thắng/thất bại | Concept B hoặc màn kết quả | Trống & Foley · Bass & hòa âm · Giai điệu | Chờ kiểm tra riêng · Giữ bản thử trước |
| 142 | Đứng yên, bắt đầu và ngừng di chuyển | CharacterArt Run/Idle | Chuẩn bị 0 · Thi triển 0.08 · Dư âm / kết thúc 0.42 | Chờ kiểm tra riêng · Giữ bản thử trước |
| 143 | Hồi chiêu sẵn sàng và yêu cầu kỹ năng không hợp lệ | Scene gameplay/tiến trình hoặc hình gốc theo mục | Cue đơn hoặc nhạc/ambience | Chờ kiểm tra riêng · Giữ bản thử trước |
| 144 | Hồi lượt Lốc lửa, giới hạn trụ và đạn hết tầm | Scene gameplay/tiến trình hoặc hình gốc theo mục | Thi triển 0 · Dư âm / kết thúc 0.51 | Chờ kiểm tra riêng · Giữ bản thử trước |
| 145 | Điểm môi trường và ambience khi trở lại scene | Ảnh biome Forest/Moon/Inferno/Tundra | Điểm môi trường | Chờ kiểm tra riêng · Giữ bản thử trước |
| 146 | Quay lại, đóng modal và ESC | MainMenu/Guide · UI hoặc nút Studio | Thi triển 0 · Dư âm / kết thúc 0.09 | Chờ kiểm tra riêng · Giữ bản thử trước |

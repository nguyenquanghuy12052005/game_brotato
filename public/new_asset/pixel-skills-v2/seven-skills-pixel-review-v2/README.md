# 7 kỹ năng · bản duyệt pixel v2

Cùng quy trình với Lồng đạn Hexcore đã duyệt: tái sử dụng cụm pixel lửa/khói của fireball-throw.png, quake.png và muzzle-smg.png trong public/new_asset. Chuyển động, hình lưỡi đạn, đầu xuyên kích, cưa, mỏ neo, dây và mảnh vỡ được phối riêng. Không dùng bộ dựng vector/neon của bản bị loại.

Mỗi thư mục có sprite sheet RGBA nền trong, preview GIF nền tối và JSON thông số. Tất cả frame 320×288, 10 cột, đọc trái sang phải rồi xuống. 30 FPS, Point/Nearest, pivot từ góc trên trái (160,181). Frame trống cuối và ô dư không phải lỗi. Không làm mịn hay blur thêm; nền tối và nhãn chỉ nằm trong preview.

Đây là bản duyệt chuyển động, chưa thay asset/code Unity. Các preview có chuyển động vị trí minh hoạ, chưa là atlas sản xuất độc lập theo layer. Khi nối code cần tách lưỡi đạn/đầu xuyên kích, aura/mảnh trúng, cưa/tiếp xúc, mỏ neo/dây/đứt theo các game object đang điều khiển chúng; không dùng cả montage như sprite quay/bám theo nhân vật. Nhịp preview không thay đổi thời gian sát thương, tốc độ bắn hay hồi chiêu thực tế.

Kíp nổ: tích lửa rồi nổ một lần. Xuyên kích: đi rồi về mạnh hơn. Cưa: xoay quanh điểm tiếp xúc và bật lửa theo nhịp sát thương. Hoả lực: đầu nòng nóng dần, vệt đạn dày. Phân rã: dấu trạng thái vỡ vụn mỗi nhịp trúng. Mỏ neo: đóng xuống, dây căng, rồi đứt; bản này minh hoạ trường hợp đứt dây.

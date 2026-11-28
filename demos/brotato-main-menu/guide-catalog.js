"use strict";

const guideStarter = {
  id: "starter", name: "Nhân vật khởi đầu", sprite: "player", weapon: "Pistol",
  intro: "Chiến binh khăn đỏ bắt đầu hành trình ở Forest với súng cơ bản.",
  style: "Di chuyển liên tục, ngắm chuột và giữ khoảng cách để duy trì hỏa lực.",
  signature: "Tự bắn theo hướng ngắm.",
  stats: ["100 HP tối đa", "5 unit/s", "10 sát thương/viên"]
};

const guideFamilies = [
  { name: "Súng", icon: "◈", tone: "gun", branches: [
    ["shotgun", "Shotgun", "player", "Shotgun", "Đạn tỏa theo hình quạt.", "Lưỡi đạn càn quét"],
    ["smg", "SMG", "player", "SMG", "Bắn liên thanh, dồn nhiều phát vào cùng một mục tiêu.", "Kíp nổ liên hoàn"],
    ["rocket", "Rocket", "player", "Rocket", "Đạn nặng, có thể mở rộng sát thương vùng qua nâng cấp.", "Bùng nổ"]
  ] },
  { name: "Cận chiến", icon: "⚔", tone: "melee", branches: [
    ["assassin", "Sát thủ", "assassin", "Dao găm", "Tiếp cận nhanh, tái định vị và chém từ góc bất ngờ.", "Ám Bộ Trảm"],
    ["dao", "Đao sĩ", "dao", "Đao lớn", "Đòn nặng, quét rộng và khống chế khoảng trống quanh mình.", "Chấn Địa Trảm"],
    ["qi", "Kiếm khí", "qi", "Kiếm", "Phóng kiếm khí theo hướng đánh để chạm mục tiêu từ xa.", "Nguyệt Nha Trảm"]
  ] },
  { name: "Pháp sư", icon: "✦", tone: "mage", branches: [
    ["fire", "Hỏa", "mage", "Trượng", "Cầu lửa nổ và để lại sát thương đốt.", "Cầu lửa"],
    ["ice", "Băng", "mage", "Trượng", "Làm chậm và tích dấu để đóng băng quái thường.", "Hơi thở băng"],
    ["light", "Quang", "mage", "Trượng", "Giáng cột sáng tại điểm ngắm, gây nhiều nhịp sát thương.", "Cột sáng"],
    ["dark", "Ám", "mage", "Trượng", "Đạn tự tìm mục tiêu và để lại lời nguyền.", "Ma đạn nguyền"]
  ] }
].map(family => ({
  ...family,
  branches: family.branches.map(([id, name, sprite, weapon, style, signature]) => ({
    id, name, sprite, weapon, style, signature, family: family.name, tone: family.tone,
    intro: `Nhánh ${name} của hệ ${family.name} trong sân thử HEXCORE.`
  }))
}));
const guideBranches = guideFamilies.flatMap(family => family.branches);

const enemyTypes = {
  normal: { name: "Lính thường", icon: "◆", tone: "gun" },
  bow: { name: "Cung thủ", icon: "➶", tone: "melee" },
  elite: { name: "Tinh nhuệ", icon: "✹", tone: "mage" },
  boss: { name: "Boss", icon: "♛", tone: "boss" }
};
const enemyColors = [
  ["brown", "Nâu", "Forest"], ["green", "Xanh", "Moon"],
  ["red", "Đỏ", "Inferno"], ["snow", "Tuyết", "Tundra"]
];
const artEnemy = {
  stats: ["Chưa có chỉ số chiến đấu đã nối"],
  behavior: "Có tạo hình và atlas trong dự án. Hành vi trong trận chưa được cấu hình cho mục này.",
  signature: "Chưa có kỹ năng chạy trong trận đã xác nhận.",
  counter: "Chờ cấu hình tầm đánh và dấu báo đòn trước khi ghi cách đối phó."
};

const guideEnemies = [
  ...enemyColors.map(([color, label, biome]) => ({
    id: `normal-${color}`, sprite: `normal-${color}`, name: `Goblin ${label}`, type: "normal", biome,
    intro: `Lính goblin cầm dao xuất hiện ở ${biome}.`,
    stats: ["30 HP gốc", "2 unit/s", "10 sát thương/chém"],
    behavior: "Đuổi theo, dừng trong tầm 1,2 unit rồi lấy đà 0,35 giây trước khi chém.",
    signature: "Trong trận chính, wave khởi tạo quái với 10 HP. EnemyGrowth có thể tăng HP và sát thương theo thời gian sống.",
    counter: "Lùi khỏi tầm khi goblin giơ dao, sau đó bắn khi nó đang hồi đòn."
  })),
  ...enemyColors.map(([color, label, biome]) => ({
    id: `bow-${color}`, sprite: `bow-${color}`, name: `Cung thủ ${label}`, type: "bow", biome,
    intro: color === "snow" ? "Cung thủ có prefab bắn xa trong dự án." : "Biến thể cung thủ có atlas Idle trong dự án.",
    ...(color === "snow" ? {
      stats: ["30 HP gốc", "8 sát thương/mũi tên", "Tầm bắn 6 unit"],
      behavior: "Giữ khoảng cách, giơ cung 0,3 giây rồi bắn theo hướng người chơi.",
      signature: "Lùi khi mục tiêu đến gần hơn 3,5 unit.",
      counter: "Đổi hướng ngang ngay khi thấy động tác giương cung."
    } : artEnemy)
  })),
  ...enemyColors.filter(([color]) => color !== "brown").map(([color, label, biome]) => ({
    ...artEnemy, id: `elite-${color}`, sprite: `elite-${color}`, name: `Goblin tinh nhuệ ${label}`, type: "elite", biome,
    intro: "Goblin tinh nhuệ có atlas gốc và Idle trong dự án."
  })),
  ...[
    ["skeleton", "Skeleton", "Moon", "Bộ xương cầm kiếm"],
    ["golem", "Lava Golem", "Inferno", "Golem đá và dung nham"],
    ["yeti", "Yeti", "Tundra", "Quái tuyết"]
  ].map(([id, name, biome, form]) => ({
    ...artEnemy, id, sprite: id, name, type: "boss", biome,
    intro: `${form}. Prefab có dấu nhận diện boss cho hệ Pháp sư.`,
    signature: "MageBossMarker dùng để xử lý miễn nhiễm một số khống chế trong các cơ chế Pháp sư."
  }))
];

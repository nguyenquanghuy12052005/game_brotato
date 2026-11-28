"use strict";

function createGuideB({ panel, content, setPanel, sound, toolsEnabled }) {
  const storageKey = "brotato-concept-b.guide.v1";
  const tabs = [["overview", "◈", "Tổng quan"], ["controls", "⌨", "Điều khiển"],
    ["characters", "⚔", "Nhân vật"], ["bestiary", "✹", "Quái vật"]];
  const branchIds = new Set(guideBranches.map(item => item.id));
  const enemyIds = new Set(guideEnemies.map(item => item.id));
  const fixture = { branches: ["shotgun", "dao", "fire"], enemies: ["normal-brown", "normal-green", "bow-snow"] };
  const journey = [
    ["Forest", "#a7d68d", "Rừng im tiếng. Cuộc săn bắt đầu.", "Tán cây và những cụm đá chia đồng cỏ thành từng khoảng trống. Khi goblin kéo đến, hãy tìm một lối thoát trước khi chúng khép vòng vây."],
    ["Moon", "#b8baff", "Vết nứt kéo dài qua miền đá xám.", "Qua khỏi rừng, mặt đất tối lại. Những khối đá đứng lẻ giữa đồng trống. Đổi hướng sớm khi bị dồn về phía một cụm đá."],
    ["Inferno", "#ff9f74", "Dung nham đỏ rực giữa nền tro.", "Vệt đỏ nổi lên giữa những mỏm đá đen. Quan sát khoảng trống giữa các cụm địa hình để giữ đường di chuyển khi quái áp sát."],
    ["Tundra", "#a8e4f4", "Chặng cuối nằm sau những mảng tuyết trắng.", "Tuyết phủ lên cây và đá, để lộ từng khoảng đất tối. Luồn qua những cụm đá phủ tuyết và giữ khoảng cách cho đến hết đợt cuối."]
  ];
  let records;
  let active = "overview";
  let selectedCharacter = guideStarter.id;
  let selectedEnemy = guideEnemies[0].id;
  let filter = "all";
  let reveal = "";
  let resetPending = false;
  let pausedExample = false;
  let storageAvailable = true;

  function persist() {
    try {
      localStorage.setItem(storageKey, JSON.stringify(records));
      storageAvailable = true;
    }
    catch { storageAvailable = false; }
  }

  function readRecords() {
    try {
      const raw = localStorage.getItem(storageKey);
      if (raw !== null) {
        const parsed = JSON.parse(raw);
        const validate = (value, ids) => Array.isArray(value) ? [...new Set(value.filter(id => ids.has(id)))] : [];
        return { branches: validate(parsed?.branches, branchIds), enemies: validate(parsed?.enemies, enemyIds) };
      }
    } catch { storageAvailable = false; }
    return { branches: [...fixture.branches], enemies: [...fixture.enemies] };
  }

  function open() {
    if (!records) { records = readRecords(); persist(); }
    active = "overview";
    reveal = "";
    resetPending = false;
    pausedExample = false;
    setPanel("Hướng dẫn", `<nav class="guide-tabs" role="tablist" aria-label="Hướng dẫn chiến đấu">
      ${tabs.map(([id, icon, label]) => `<button type="button" id="guide-tab-${id}" data-tab="${id}" role="tab" aria-controls="guide-view"><span aria-hidden="true">${icon}</span>${label}</button>`).join("")}
      </nav><div id="guide-view" role="tabpanel"></div><div class="guide-notice" role="status" aria-live="polite" hidden></div>`);
    panel.classList.add("guide-b");
    content.querySelector(".guide-tabs").addEventListener("keydown", switchTabWithKey);
    content.addEventListener("click", handleClick);
    render();
    content.querySelector('[aria-selected="true"]').focus();
  }

  panel.addEventListener("close", () => {
    if (panel.open) return;
    content.removeEventListener("click", handleClick);
    panel.classList.remove("guide-b");
  });
  window.addEventListener("storage", event => {
    if (event.key !== storageKey && event.key !== null) return;
    records = readRecords();
    reveal = "";
    if (panel.open && panel.classList.contains("guide-b")) render();
  });

  function changeTab(id) {
    if (id === active) return;
    sound("confirm");
    active = id;
    reveal = "";
    resetPending = false;
    const view = content.querySelector("#guide-view");
    view.scrollTop = 0;
    const list = view.querySelector(".guide-list");
    if (list) list.scrollTop = 0;
    render();
    if (!panel.closest('[data-motion="off"]')) {
      content.querySelector("#guide-view > section").animate(
        [{ opacity: .55, translate: "0 5px" }, { opacity: 1, translate: "0 0" }],
        { duration: 180, easing: "ease-out" });
    }
    content.querySelector('[aria-selected="true"]').focus();
  }

  function switchTabWithKey(event) {
    const index = tabs.findIndex(([id]) => id === active);
    const indexes = {
      ArrowLeft: (index + tabs.length - 1) % tabs.length, ArrowRight: (index + 1) % tabs.length,
      Home: 0, End: tabs.length - 1
    };
    if (!Object.hasOwn(indexes, event.key)) return;
    event.preventDefault();
    changeTab(tabs[indexes[event.key]][0]);
  }

  function render(focusId) {
    const view = content.querySelector("#guide-view");
    const oldScroll = view.querySelector(".guide-list")?.scrollTop ?? 0;
    for (const tab of content.querySelectorAll("[data-tab]")) {
      const selected = tab.dataset.tab === active;
      tab.setAttribute("aria-selected", String(selected));
      tab.tabIndex = selected ? 0 : -1;
    }
    view.setAttribute("aria-labelledby", `guide-tab-${active}`);
    if (active === "overview") view.innerHTML = overview();
    if (active === "controls") {
      view.innerHTML = controls();
      updatePauseExample();
    }
    if (active === "characters") view.innerHTML = characters();
    if (active === "bestiary") view.innerHTML = bestiary();
    const list = view.querySelector(".guide-list");
    if (list) list.scrollTop = oldScroll;
    if (focusId) document.getElementById(focusId)?.focus({ preventScroll: true });
  }

  function overview() {
    return `<section class="guide-overview">
      <div class="guide-cover"><div class="guide-cover-copy"><h3>BROTATO 2D</h3><p>Hành động sinh tồn góc nhìn từ trên xuống. Điều khiển chiến binh khăn đỏ qua bốn vùng bị goblin bao vây.</p></div></div>
      <div class="guide-overview-info">
        <article><span class="guide-medal" aria-hidden="true">◈</span><div><h3>Bối cảnh hành trình</h3><p>Chiến binh bị goblin bao vây ở Forest. Từ đó, người chơi mở đường qua Moon và Inferno tới Tundra.</p></div></article>
        <article><span class="guide-medal" aria-hidden="true">◎</span><div><h3>Mục tiêu</h3><p>Di chuyển, né đòn và sống sót tới khi hết giờ. Bản chơi hiện tại có một wave 30 giây mỗi vùng.</p></div></article>
        <article><span class="guide-medal win" aria-hidden="true">✓</span><div><h3>Chiến thắng</h3><p>Sống sót hết wave cuối ở Tundra để hoàn thành cả bốn vùng.</p></div></article>
        <article><span class="guide-medal lose" aria-hidden="true">!</span><div><h3>Thất bại</h3><p>HP về 0 thì Game Over. Dùng nút Chơi lại để bắt đầu lượt mới.</p></div></article>
      </div>
      <h3 class="guide-journey-title">Bốn vùng trên đường sinh tồn</h3>
      <ol class="guide-journey" aria-label="Thứ tự các vùng">${journey.map(([name, tone, lead, story]) =>
        `<li class="guide-biome" style="--biome-tone:${tone}">
          <img src="assets/guide-b/maps/${name.toLowerCase()}.jpg" width="960" height="576" alt="Toàn cảnh ${name}">
          <div><h4>${name}</h4><strong>${lead}</strong><p>${story}</p></div>
        </li>`).join("")}</ol>
      </section>`;
  }

  const keycap = label => `<kbd>${label}</kbd>`;

  function controls() {
    return `<section class="guide-control-columns">
      <article class="guide-control-card">
        <h3><span aria-hidden="true">⌨</span> Di chuyển</h3>
        <div class="guide-wasd">
          ${[["W", "Lên", "↑"], ["A", "Trái", "←"], ["S", "Xuống", "↓"], ["D", "Phải", "→"]]
            .map(([key, label, arrow]) => `<div>${keycap(key)}<span aria-hidden="true">${arrow}</span><strong>${label}</strong></div>`).join("")}
        </div>
      </article>
      <article class="guide-control-card">
        <h3><span aria-hidden="true">✦</span> Kỹ năng</h3>
        <div class="guide-skillcaps">
          ${[["Q", "Lõi 1"], ["E", "Lõi 2"], ["R", "Chiêu cuối"]].map(([key, label]) => `<div>${keycap(key)}<strong>${label}</strong></div>`).join("")}
        </div>
      </article>
      <article class="guide-control-card">
        <h3><span aria-hidden="true">◎</span> Tương tác trong trận</h3>
        <ul class="guide-interactions">
          <li><span class="guide-mouse" aria-hidden="true"></span><div><strong>Chuột</strong><p>Đưa chuột để ngắm. Súng tự bắn theo hướng ngắm.</p></div></li>
          <li><span class="guide-command-label">Chọn</span><div><strong>Chọn nâng cấp</strong><p>Bấm vào thẻ khi có lựa chọn.</p></div></li>
          <li><span class="guide-command-label">Chơi lại</span><div><strong>Bắt đầu lượt mới</strong><p>Có trên màn hình kết quả.</p></div></li>
        </ul>
      </article>
      <article class="guide-control-card guide-pause-preview">
        <h3><span aria-hidden="true">Ⅱ</span> Tạm dừng trận</h3>
        <div class="guide-pause-scene"><span class="guide-pause-status" role="status"></span></div>
        <div class="guide-pause-actions">
          <button type="button" data-pause-example="pause"><span aria-hidden="true">Ⅱ</span> Tạm dừng</button>
          <button type="button" data-pause-example="resume"><span aria-hidden="true">▶</span> Tiếp tục</button>
        </div>
      </article>
      </section>`;
  }

  function updatePauseExample() {
    const preview = content.querySelector(".guide-pause-preview");
    preview.classList.toggle("paused", pausedExample);
    preview.querySelector(".guide-pause-status").textContent = pausedExample ? "Đang tạm dừng" : "Đang chơi";
    preview.querySelector('[data-pause-example="pause"]').disabled = pausedExample;
    preview.querySelector('[data-pause-example="resume"]').disabled = !pausedExample;
  }

  function sprite(item, locked = false, animate = false) {
    const weapon = item.sprite === "player" ? "pistol" : item.sprite === "mage" ? "staff" : "";
    return `<span aria-hidden="true" class="guide-portrait ${locked ? "silhouette" : ""}">
      <span class="guide-sprite ${animate && !locked ? "idle" : ""}" style="--sheet:url('assets/guide-b/${item.sprite}.png')"></span>
      ${weapon ? `<img class="guide-weapon ${weapon}" src="assets/${weapon}.png" alt="">` : ""}</span>`;
  }

  const isOpen = (item, group) => item.id === guideStarter.id || records[group].includes(item.id);
  const displayedName = (item, group, locked) => group === "enemies" && locked ? "???" : item.name;
  function card(item, group, selected) {
    const locked = !isOpen(item, group);
    const name = displayedName(item, group, locked);
    return `<button type="button" id="guide-item-${item.id}" class="guide-card ${locked ? "locked" : ""} ${reveal === item.id ? "just-unlocked" : ""}"
       data-item="${item.id}" aria-pressed="${selected === item.id}" aria-label="${name}${locked ? " · Đang khóa" : ""}">
       ${sprite(item, locked)}${locked ? '<i class="guide-lock" aria-hidden="true"></i>' : ""}<span class="guide-card-name">${name}</span></button>`;
  }

  function progress(group, total, label) {
    const count = records[group].length;
    return `<div class="guide-progress"><div><strong>${label}</strong><span>${count} / ${total}</span></div>
      <progress value="${count}" max="${total}" aria-label="${label}"></progress></div>`;
  }

  function characters() {
    const items = [guideStarter, ...guideBranches];
    const selected = items.find(item => item.id === selectedCharacter) ?? guideStarter;
    return `<section class="guide-collection"><div class="guide-list" aria-label="Hình thái nhân vật">
      ${progress("branches", guideBranches.length, "Hình thái đã khám phá")}
      <div class="guide-starter">${card(guideStarter, "branches", selectedCharacter)}<span>Luôn mở · Hình thái xuất phát</span></div>
      ${guideFamilies.map(family => `<section class="guide-family tone-${family.tone}" aria-label="Hệ ${family.name}">
      <h3><span aria-hidden="true">${family.icon}</span>${family.name}</h3>
      <div class="guide-grid" style="--columns:${family.branches.length}">${family.branches.map(item => card(item, "branches", selectedCharacter)).join("")}</div></section>`).join("")}
      </div>${detail(selected, "branches")}</section>`;
  }

  function bestiary() {
    const items = guideEnemies.filter(item => filter === "all" || item.type === filter);
    const selected = items.find(item => item.id === selectedEnemy) ?? items[0];
    selectedEnemy = selected.id;
    const filters = [["all", "Tất cả"], ...Object.entries(enemyTypes).map(([id, type]) => [id, type.name])];
    return `<section class="guide-collection"><div class="guide-list" aria-label="Danh sách quái vật">
      ${progress("enemies", guideEnemies.length, "Quái đã gặp")}
      <div class="guide-filters" role="group" aria-label="Lọc loại quái">
      ${filters.map(([id, label]) => `<button type="button" id="guide-filter-${id}" data-filter="${id}" aria-pressed="${id === filter}">${label}</button>`).join("")}</div>
      <div class="guide-grid monster-grid" style="--columns:4">
      ${items.map(item => `<div class="tone-${enemyTypes[item.type].tone}">${card(item, "enemies", selectedEnemy)}</div>`).join("")}</div>
      </div>${detail(selected, "enemies")}</section>`;
  }

  function detail(item, group) {
    const monster = group === "enemies";
    const locked = !isOpen(item, group);
    const name = displayedName(item, group, locked);
    const label = monster ? enemyTypes[item.type].name : item.family ?? "Khởi đầu";
    const tone = monster ? enemyTypes[item.type].tone : item.tone ?? "gun";
    let body;
    if (locked) {
      body = `<section class="guide-locked-copy"><strong>Đang khóa</strong><p>${monster ? "Gặp quái này trong trận để mở mục ghi chép." : `Chọn và tiến hóa thành ${item.name} trong trận để khám phá hình thái.`}</p></section>`;
    } else if (monster) {
      body = `<p>${item.intro}</p><div class="guide-stats">${item.stats.map(stat => `<span>${stat}</span>`).join("")}</div>
        <dl><dt>Tấn công</dt><dd>${item.behavior}</dd><dt>Đặc biệt</dt><dd>${item.signature}</dd><dt>Đối phó</dt><dd>${item.counter}</dd></dl>`;
    } else {
      body = `<p>${item.intro}</p>${item.stats ? `<div class="guide-stats">${item.stats.map(stat => `<span>${stat}</span>`).join("")}</div>` : ""}
        <dl><dt>Vũ khí</dt><dd>${item.weapon}</dd><dt>Phong cách</dt><dd>${item.style}</dd><dt>Đặc trưng</dt><dd>${item.signature}</dd></dl>`;
    }
    return `<article class="guide-detail tone-${tone} ${reveal === item.id ? "just-unlocked" : ""}" aria-label="Chi tiết">
      <div class="guide-detail-art">${sprite(item, locked, true)}${locked ? '<i class="guide-lock" aria-hidden="true"></i>' : ""}</div>
      <div class="guide-detail-copy">${!monster || !locked ? `<span class="guide-tag">${label}</span>` : ""}
      <h3>${name}</h3>${body}</div>${toolsEnabled ? toolbox(item, group, locked) : ""}</article>`;
  }

  function toolbox(item, group, locked) {
    const verb = group === "branches" ? "Thử chọn nhánh" : "Thử gặp quái";
    return `<div class="guide-tools"><span>Công cụ demo</span>
      <button type="button" id="guide-unlock" data-unlock="${item.id}" ${locked ? "" : "disabled"}>${verb}</button>
      ${resetPending ? '<button type="button" id="guide-confirm-reset" data-reset="confirm">Xác nhận reset</button><button type="button" data-reset="cancel">Hủy</button>' : '<button type="button" id="guide-reset" data-reset="ask">Reset bộ sưu tập</button>'}
      ${storageAvailable ? "" : "<small>Chỉ lưu trong phiên này.</small>"}</div>`;
  }

  function announce(message) {
    const notice = content.querySelector(".guide-notice");
    notice.textContent = message;
    notice.hidden = false;
    notice.onanimationend = () => { notice.hidden = true; };
    notice.classList.remove("flash");
    void notice.offsetWidth;
    notice.classList.add("flash");
  }

  function handleClick(event) {
    const button = event.target.closest("button");
    if (!button || button.disabled) return;
    const { tab, filter: choice, item, unlock, reset, pauseExample } = button.dataset;
    if (tab) changeTab(tab);
    if (pauseExample) {
      pausedExample = pauseExample === "pause";
      sound("confirm");
      updatePauseExample();
      content.querySelector(`[data-pause-example="${pausedExample ? "resume" : "pause"}"]`).focus();
    }
    if (choice) {
      if (choice === filter) return;
      filter = choice;
      reveal = "";
      sound("confirm");
      render(button.id);
    }
    if (item) {
      if (active === "characters") selectedCharacter = item;
      else selectedEnemy = item;
      reveal = "";
      sound("confirm");
      render(button.id);
    }
    if (!toolsEnabled) return;
    if (unlock) {
      const group = active === "characters" ? "branches" : "enemies";
      const pool = group === "branches" ? guideBranches : guideEnemies;
      const match = pool.find(entry => entry.id === unlock);
      if (!match || isOpen(match, group)) return;
      records[group].push(match.id);
      persist();
      reveal = match.id;
      sound("unlock");
      render(`guide-item-${match.id}`);
      announce(`Đã khám phá: ${match.name}`);
    }
    if (reset) {
      resetPending = reset === "ask";
      if (reset === "confirm") {
        records = { branches: [], enemies: [] };
        persist();
        reveal = "";
      }
      sound("confirm");
      render(resetPending ? "guide-confirm-reset" : "guide-reset");
      if (reset === "confirm") announce("Đã reset bộ sưu tập demo");
    }
  }

  return { open };
}

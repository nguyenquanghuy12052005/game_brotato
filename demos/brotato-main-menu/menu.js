"use strict";

const concepts = {
  a: { title: "Hội tụ chiến binh", gun: [.36, .51, .22, .47], sword: [.57, .68, -2.15], mage: [.87, .34, "#dd84ff"], hit: [.26, .59], tempo: 117, root: 62 },
  b: { title: "Đại chiến sinh tồn", gun: [.35, .49, .61, .50], sword: [.43, .65, -.54], mage: [.45, .34, "#69faff"], hit: [.57, .51], tempo: 143, root: 60 },
  c: { title: "Đấu trường HEXCORE", gun: [.46, .40, .72, .35], sword: [.67, .61, -.68], mage: [.91, .30, "#e3a1ff"], hit: [.59, .29], tempo: 131, root: 65 }
};
const query = new URLSearchParams(location.search);
const key = query.get("concept");
const concept = Object.hasOwn(concepts, key) ? key : "a";
const design = concepts[concept];
const stage = document.querySelector("#stage");
const panel = document.querySelector("#panel");
const title = document.querySelector("#panel-title");
const content = document.querySelector("#panel-content");
const actions = document.querySelector("#panel-actions");
const buttons = [...stage.querySelectorAll(".command")];
const reduce = matchMedia("(prefers-reduced-motion: reduce)");
stage.dataset.concept = concept;
stage.dataset.motion = reduce.matches ? "off" : "on";
document.title = `BROTATO 2D · ${design.title}`;
let showMotion = !reduce.matches;
let caller = null;
const demoSave = query.get("save") === "1";
const guideB = concept === "b" ? createGuideB({ panel, content, setPanel, sound, toolsEnabled: query.get("tools") === "1" }) : null;
if (demoSave) {
  buttons[1].removeAttribute("aria-disabled");
  document.querySelector("#resume-status").textContent = "Forest · Lưu mẫu";
}

const icons = {
  start: [[[8,5],[21,13],[8,21],[8,5]]],
  continue: [[[7,5],[16,5],[20,9],[20,20],[5,20],[5,5],[7,5]], [[9,5],[9,10],[16,10],[16,5]], [[9,20],[9,15],[16,15],[16,20]]],
  stages: [[[4,7],[10,4],[17,8],[22,5],[22,19],[16,22],[10,18],[4,21],[4,7]], [[10,4],[10,18]], [[17,8],[16,22]]],
  guide: [[[3,6],[8,6],[13,8],[18,6],[23,6],[23,20],[18,20],[13,22],[8,20],[3,20],[3,6]], [[13,8],[13,22]]],
  settings: [[[5,6],[21,6]], [[5,13],[21,13]], [[5,20],[21,20]], [[10,3],[10,9]], [[16,10],[16,16]], [[9,17],[9,23]]],
  exit: [[[11,5],[5,5],[5,21],[11,21]], [[10,13],[22,13]], [[17,8],[22,13],[17,18]]]
};

for (const button of buttons) {
  const icon = button.querySelector("canvas");
  icon.width = icon.height = 52;
  icon.setAttribute("aria-hidden", "true");
  const brush = icon.getContext("2d");
  brush.scale(2, 2);
  brush.strokeStyle = button.dataset.action === "start" ? "#36222d" : "#f3fbff";
  brush.lineWidth = 2.05;
  brush.lineCap = brush.lineJoin = "round";
  for (const path of icons[button.dataset.action]) {
    brush.beginPath();
    path.forEach(([x, y], i) => i ? brush.lineTo(x, y) : brush.moveTo(x, y));
    if (button.dataset.action === "start") { brush.fillStyle = brush.strokeStyle; brush.fill(); }
    brush.stroke();
  }
  button.addEventListener("pointerenter", () => {
    if (button.getAttribute("aria-disabled") !== "true") sound("hover");
  });
  button.addEventListener("click", () => {
    if (button.getAttribute("aria-disabled") === "true") return;
    startAudio();
    sound("confirm");
    button.classList.remove("bounce");
    requestAnimationFrame(() => button.classList.add("bounce"));
    caller = button;
    openAction(button.dataset.action);
  });
}

const maps = [
  ["Forest", "Tán rừng và tàn tích xanh.", true],
  ["Moon", "Thung lũng đá với mặt đất rạn nứt.", false],
  ["Inferno", "Địa hình núi lửa và dòng dung nham.", false],
  ["Tundra", "Đồng tuyết và đá lạnh.", false]
];
const cast = [["Súng", "gun"], ["Cận chiến", "sword"], ["Pháp sư", "mage"], ["Goblin", "goblin"]];
const castMarkup = guideB ? "" : `<div class="cast">${cast.map(([name, role]) => `<figure><canvas data-cast="${role}" width="180" height="122" aria-hidden="true"></canvas><figcaption>${name}</figcaption></figure>`).join("")}</div>`;

function setPanel(heading, html, label, onConfirm) {
  panel.classList.remove("guide-b");
  title.textContent = heading;
  content.innerHTML = html;
  actions.replaceChildren();
  if (label) {
    const button = document.createElement("button");
    button.type = "button";
    button.className = "confirm";
    button.textContent = label;
    button.disabled = !onConfirm;
    if (onConfirm) button.addEventListener("click", () => { sound("confirm"); onConfirm(); });
    actions.append(button);
  }
  if (!panel.open) panel.showModal();
  panel.querySelector(".panel-footer [data-close]").focus();
}

function openAction(action) {
  switch (action) {
    case "start":
      if (guideB) {
        setPanel(demoSave ? "Bắt đầu mới?" : "Bắt đầu", demoSave
          ? "<p>Điểm lưu mẫu hiện ở Forest. Bắt đầu hành trình mới ở Forest?</p>"
          : "<p>Forest · Hành trình mới</p>", demoSave ? "Bắt đầu mới" : "Vào Forest", () => showDemoDestination("Forest"));
        break;
      }
      if (demoSave) {
        setPanel("Bắt đầu mới?", "<p>Điểm lưu mẫu hiện tại ở Forest. Bạn có muốn xem luồng bắt đầu hành trình mới?</p>",
          "Bắt đầu mới", showStartingLineup);
      } else {
        setPanel("Bắt đầu", "<p>Forest · Hành trình mới</p><p>Bản xem thử giao diện trước khi vào trận.</p>",
          "Xem đội hình", showStartingLineup);
      }
      break;
    case "continue":
      if (guideB) {
        setPanel("Chơi tiếp", "<p>Forest · Đầu màn · Điểm lưu mẫu</p>", "Tiếp tục", () => showDemoDestination("Forest"));
        break;
      }
      setPanel("Chơi tiếp", "<p>Forest · Đầu màn</p><p>Dữ liệu mẫu để xem trạng thái có tiến trình.</p>",
        "Xem đội hình", () => setPanel("Đội hình tại Forest", castMarkup));
      break;
    case "stages":
      showStage(0);
      break;
    case "guide":
      if (guideB) { guideB.open(); break; }
      setPanel("Hướng dẫn", "<p>WASD để di chuyển, dùng chuột để ngắm. Vũ khí tự bắn trong gameplay hiện tại.</p>" + castMarkup +
        "<p>HEXCORE là hệ thống phát triển sức mạnh. Ba hệ được minh họa bằng sprite gốc trong khung này.</p>");
      break;
    case "settings":
      setPanel("Cài đặt", `<label class="option"><span>Nhạc nền</span><input id="music" type="checkbox" ${musicEnabled ? "checked" : ""}></label>
        <label class="option"><span>Âm lượng</span><input id="volume" type="range" min="0" max="100" value="${Math.round(volume * 100)}"></label>
        <label class="option"><span>Chuyển động chiến trường</span><input id="motion" type="checkbox" ${showMotion ? "checked" : ""}></label>`);
      content.querySelector("#music").addEventListener("change", (event) => { musicEnabled = event.target.checked; updateAudio(); });
      content.querySelector("#volume").addEventListener("input", (event) => { volume = event.target.value / 100; updateAudio(); });
      content.querySelector("#motion").addEventListener("change", (event) => {
        showMotion = event.target.checked;
        stage.dataset.motion = showMotion ? "on" : "off";
        if (!showMotion) resetPerspective();
      });
      break;
    case "exit":
      setPanel("Thoát?", "<p>Kết thúc lượt xem thử menu?</p>", "Kết thúc lượt xem",
        () => setPanel("Đã kết thúc lượt xem", "<p>Chọn Quay lại để tiếp tục xem concept này.</p>"));
      break;
  }
}

function showStartingLineup() { setPanel("Đội hình khởi đầu", castMarkup); }

function showDemoDestination(name) {
  setPanel(name, "<p>Đã xác nhận điểm vào trận trong bản demo.</p>");
}

function showStage(index, keepFocus = false) {
  const [name, detail, open] = maps[index];
  const tabs = maps.map(([tab], i) => `<button type="button" class="map-tab" role="tab" id="biome-tab-${i}" aria-controls="biome-detail" data-index="${i}" aria-selected="${i === index}">${tab}</button>`).join("");
  setPanel("Chọn màn", `<div class="map-tabs" role="tablist" aria-label="Biome">${tabs}</div>
    <div class="map-detail" id="biome-detail" role="tabpanel" aria-labelledby="biome-tab-${index}" style="--biome:url('assets/${name.toLowerCase()}-tiles.png')"><div>
      <h3>${name}</h3><p>${detail}</p><p>${open ? "Mở trong demo" : "Đang khóa · Hoàn thành màn trước (minh họa)"}</p>
    </div></div>`, guideB ? "Vào màn" : "Xem đội hình",
    open ? () => guideB ? showDemoDestination(name) : setPanel(name, castMarkup) : null);
  for (const tab of content.querySelectorAll(".map-tab")) {
    tab.addEventListener("click", () => { sound("hover"); showStage(Number(tab.dataset.index), true); });
    tab.addEventListener("keydown", (event) => {
      if (!["ArrowRight", "ArrowLeft"].includes(event.key)) return;
      event.preventDefault();
      showStage((index + (event.key === "ArrowRight" ? 1 : maps.length - 1)) % maps.length, true);
    });
  }
  if (keepFocus) content.querySelector('[aria-selected="true"]').focus();
}

function dismiss() { sound("close"); panel.close(); }
for (const button of panel.querySelectorAll("[data-close]")) button.addEventListener("click", dismiss);
panel.addEventListener("cancel", (event) => { event.preventDefault(); dismiss(); });
panel.addEventListener("close", () => caller?.focus());
document.addEventListener("keydown", (event) => {
  if (panel.open) return;
  if (event.key === "Escape") {
    event.preventDefault();
    caller = buttons.at(-1);
    openAction("exit");
    return;
  }
  const direction = ["ArrowDown", "ArrowRight"].includes(event.key) ? 1 : ["ArrowUp", "ArrowLeft"].includes(event.key) ? -1 : 0;
  if (!direction) return;
  event.preventDefault();
  const current = buttons.indexOf(document.activeElement);
  buttons[current === -1 ? (direction > 0 ? 0 : buttons.length - 1) : (current + direction + buttons.length) % buttons.length].focus();
  sound("hover");
});

function resetPerspective() {
  stage.style.setProperty("--mx", "0px");
  stage.style.setProperty("--my", "0px");
  stage.style.setProperty("--near-x", "0px");
  stage.style.setProperty("--near-y", "0px");
}
stage.addEventListener("pointermove", (event) => {
  if (!showMotion || panel.open) return;
  const box = stage.getBoundingClientRect();
  const x = (event.clientX - box.left) / box.width - .5;
  const y = (event.clientY - box.top) / box.height - .5;
  stage.style.setProperty("--mx", `${x * 12}px`);
  stage.style.setProperty("--my", `${y * 8}px`);
  stage.style.setProperty("--near-x", `${x * 13}px`);
  stage.style.setProperty("--near-y", `${y * 10}px`);
});
stage.addEventListener("pointerleave", resetPerspective);

const canvas = document.querySelector("#battle");
const fx = canvas.getContext("2d");
const heroLayers = [...stage.querySelectorAll(".living")].slice(0, 3);
const motes = Array.from({ length: 38 }, (_, i) => ({
  x: ((i * 73 + 18) % 101) / 100, y: ((i * 37 + 21) % 97) / 96,
  rate: .023 + (i % 7) * .009, size: 1 + (i % 4), drift: (i % 5) * .031 + .015
}));
function resize() {
  const density = Math.min(2, devicePixelRatio);
  const view = stage.getBoundingClientRect();
  canvas.width = Math.round(view.width * density);
  canvas.height = Math.round(view.height * density);
}
new ResizeObserver(resize).observe(stage);
resize();

function segment(x, y, toX, toY, color, width, opacity) {
  fx.globalAlpha = opacity;
  fx.strokeStyle = color;
  fx.lineWidth = width;
  fx.lineCap = "round";
  fx.beginPath();
  fx.moveTo(x, y);
  fx.lineTo(toX, toY);
  fx.stroke();
}

function drawBattle(time) {
  if (!showMotion || panel.open) return;
  fx.setTransform(canvas.width / 1920, 0, 0, canvas.height / 1080, 0, 0);
  fx.clearRect(0, 0, 1920, 1080);
  fx.globalCompositeOperation = "screen";
  for (const [i, mote] of motes.entries()) {
    const x = ((mote.x + time * mote.drift * .38) % 1.1 - .05) * 1920;
    const y = (1 - (mote.y + time * mote.rate) % 1) * 1080;
    if (concept !== "b" && x < 590 && y > 380) continue;
    fx.globalAlpha = (.42 + .32 * Math.sin(time * 1.8 + i)) * (i % 3 === 0 ? .55 : .83);
    fx.fillStyle = i % 4 === 0 ? "#82edff" : i % 5 === 0 ? "#98e385" : "#ffe599";
    fx.beginPath(); fx.ellipse(x, y, mote.size * 1.55, mote.size * .73, time + i, 0, Math.PI * 2); fx.fill();
  }
  const [sx, sy, tx, ty] = design.gun.map((n, i) => n * (i % 2 ? 1080 : 1920));
  const cycle = time % (concept === "b" ? 2.95 : 4.1);
  const kick = cycle < .32 ? Math.max(0, Math.sin(cycle * 58)) * (1 - cycle / .32) * 5 : 0;
  heroLayers[0].style.translate = `${(sx > tx ? 1 : -1) * kick}px 0`;
  for (let i = 0; i < 3; i++) {
    const age = (cycle - .12 * i) / .41;
    if (age < 0 || age > 1) continue;
    const x = sx + (tx - sx) * age;
    const y = sy + (ty - sy) * age - i * 5;
    segment(x, y, x - (tx - sx) * .17, y - (ty - sy) * .17, "#ff8b46", 11, (1 - age) * .57);
    segment(x, y, x - (tx - sx) * .12, y - (ty - sy) * .12, "#fff9ba", 4, .92);
  }
  const cut = (time + 3.6) % 5.4 / .73;
  const lean = cut < 1 ? Math.sin(cut * Math.PI) * 5 : 0;
  heroLayers[1].style.translate = `${lean}px ${lean * -.5}px`;
  if (cut < 1) {
    const [x, y, angle] = design.sword;
    fx.strokeStyle = concept === "b" ? "#ffdf89" : "#9effee";
    fx.lineWidth = 4 + 17 * (1 - cut);
    fx.globalAlpha = Math.sin(cut * Math.PI) * .7;
    fx.beginPath();
    fx.ellipse(x * 1920, y * 1080, 133 + cut * 94, 79 + cut * 40, angle, .15 + cut, 1.48 + cut);
    fx.stroke();
  }
  const [mx, my, hue] = design.mage;
  heroLayers[2].style.translate = `0 ${-2.7 - 2.1 * Math.sin(time * 2)}px`;
  for (let i = 0; i < 11; i++) {
    const a = time * (concept === "c" ? 1.45 : 2.15) + i * .61;
    const span = 54 + 7 * i;
    const x = mx * 1920 + Math.cos(a) * span;
    const y = my * 1080 + Math.sin(a) * span * .43;
    segment(x, y, x - Math.sin(a) * 15, y + Math.cos(a) * 9, hue, 2 + i % 3, .28 + .22 * Math.sin(a + i));
  }
  const pop = (time + .9) % 4.7 / .63;
  if (pop < 1) {
    const [x, y] = design.hit;
    for (let i = 0; i < 12; i++) {
      const a = i * Math.PI / 6;
      const reach = 9 + pop * (53 + i % 4 * 14);
      const dx = Math.cos(a) * reach;
      const dy = Math.sin(a) * reach * .7;
      segment(x * 1920 + dx * .68, y * 1080 + dy * .68, x * 1920 + dx, y * 1080 + dy,
        i % 2 ? "#ffffa8" : "#ffa264", 3, (1 - pop) * .8);
    }
  }
  fx.globalAlpha = 1;
  fx.globalCompositeOperation = "source-over";
}

const loadImage = (path) => { const image = new Image(); image.src = `assets/${path}.png`; return image; };
const frames = (name, count) => Array.from({ length: count }, (_, i) => loadImage(`${name}-${i}`));
const idleGun = guideB ? [] : frames("gun", 4);
const idleSword = guideB ? [] : frames("sword", 4);
const attack = guideB ? [] : frames("attack", 6);
const mage = guideB ? null : loadImage("mage"), pistol = guideB ? null : loadImage("pistol");
const staff = guideB ? null : loadImage("staff"), goblin = guideB ? null : loadImage("goblin");
function ready(image) { return image.complete && image.naturalWidth > 0; }
function paintCast(time) {
  if (!panel.open) return;
  for (const card of content.querySelectorAll("canvas[data-cast]")) {
    const ctx = card.getContext("2d");
    ctx.imageSmoothingEnabled = false;
    ctx.clearRect(0, 0, 180, 122);
    const frame = Math.floor(time * 7) % 4;
    const action = time % 3.6;
    const moved = showMotion && action > 2.65;
    const role = card.dataset.cast;
    if (role === "gun" && ready(idleGun[frame])) {
      ctx.drawImage(idleGun[showMotion ? frame : 0], 37, 1, 106, 116);
      if (ready(pistol)) ctx.drawImage(pistol, moved ? 90 : 94, 63, 34, 28);
      if (moved && action < 2.83) { ctx.fillStyle = "#ffce55"; ctx.fillRect(119, 70, 27, 3); }
    }
    if (role === "sword") {
      const image = moved ? attack[Math.min(5, Math.floor((action - 2.65) * 8))] : idleSword[showMotion ? frame : 0];
      if (ready(image)) ctx.drawImage(image, 34, 1, 112, 112);
    }
    if (role === "mage" && ready(mage)) {
      ctx.drawImage(mage, 53, 7, 67, 98);
      if (ready(staff)) {
        ctx.save(); ctx.translate(92, 72); ctx.rotate(moved ? -.53 : -.27);
        ctx.drawImage(staff, -9, -8, 63, 15); ctx.restore();
      }
      if (moved) {
        ctx.strokeStyle = "#af9cff"; ctx.lineWidth = 2; ctx.beginPath();
        ctx.ellipse(131, 51, 9 + 5 * Math.sin(time * 8), 7, time, 0, 6.29); ctx.stroke();
      }
    }
    if (role === "goblin" && ready(goblin)) {
      const cell = goblin.naturalWidth / 4;
      ctx.drawImage(goblin, (showMotion ? frame : 0) * cell, moved ? cell * 2 : 0, cell, cell, 40, 10, 100, 100);
    }
  }
}
function animate(ms) {
  drawBattle(ms * .001);
  if (!guideB) paintCast(ms * .001);
  requestAnimationFrame(animate);
}
requestAnimationFrame(animate);

// Authored music and cue synthesis. The browser unlocks audio on the first interaction.
let audio, master, bed, scheduler;
let volume = .58, musicEnabled = true, beat = 0, nextNote = 0, hoverTime = 0;
const steps = [0, 7, 12, 7, 2, 9, 14, 9, 4, 11, 16, 11, 7, 12, 16, 19];
const roots = [0, -5, -3, 7];
const hz = midi => 440 * 2 ** ((midi - 69) / 12);

function note(frequency, time, length, level, type, destination) {
  const oscillator = audio.createOscillator();
  const shape = audio.createGain();
  oscillator.type = type;
  oscillator.frequency.value = frequency;
  shape.gain.setValueAtTime(.0001, time);
  shape.gain.exponentialRampToValueAtTime(level, time + .012);
  shape.gain.exponentialRampToValueAtTime(.0001, time + length);
  oscillator.connect(shape).connect(destination);
  oscillator.start(time);
  oscillator.stop(time + length + .015);
}

function startAudio() {
  if (!audio) {
    const Audio = window.AudioContext || window.webkitAudioContext;
    if (!Audio) return;
    audio = new Audio();
    master = audio.createGain();
    bed = audio.createGain();
    bed.connect(master);
    master.connect(audio.destination);
    nextNote = audio.currentTime + .08;
    scheduler = setInterval(() => {
      if (audio.state !== "running") return;
      if (nextNote < audio.currentTime) nextNote = audio.currentTime + .03;
      const eighth = 30 / design.tempo;
      while (nextNote < audio.currentTime + .12) {
        const chord = roots[Math.floor(beat / 8) % roots.length];
        if (musicEnabled) {
          note(hz(design.root + chord + steps[beat % steps.length]), nextNote, .19, .067, "triangle", bed);
          if (beat % 4 === 0) note(hz(design.root + chord - 12), nextNote, .31, .077, "sine", bed);
          if (beat % 4 === 2) note(hz(design.root + chord + 7), nextNote, .15, .027, "triangle", bed);
          note(beat % 4 === 0 ? 58 : 1320 + (beat % 4) * 360, nextNote, .065, beat % 4 === 0 ? .13 : .024,
            beat % 4 === 0 ? "sine" : "triangle", bed);
        }
        beat++;
        nextNote += eighth;
      }
    }, 40);
  }
  audio.resume().catch(() => {});
  updateAudio();
}
function updateAudio() {
  if (!audio) return;
  master.gain.setTargetAtTime(volume, audio.currentTime, .05);
  bed.gain.setTargetAtTime(musicEnabled ? .72 : 0, audio.currentTime, .06);
}
function sound(kind) {
  if (!audio || audio.state !== "running") return;
  const time = audio.currentTime + .005;
  if (kind === "unlock") {
    note(523, time, .19, .12, "triangle", master);
    note(659, time + .077, .22, .11, "triangle", master);
    note(1047, time + .154, .36, .095, "sine", master);
    return;
  }
  if (kind === "hover") {
    if (time - hoverTime < .10) return;
    hoverTime = time;
    note(466, time, .07, .065, "sine", master);
  } else {
    note(kind === "confirm" ? 523 : 330, time, .10, .10, "triangle", master);
    note(kind === "confirm" ? 784 : 247, time + .055, .12, .085, "sine", master);
  }
}
window.addEventListener("pagehide", () => { clearInterval(scheduler); audio?.close(); }, { once: true });

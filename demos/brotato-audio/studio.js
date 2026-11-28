"use strict";

const catalog = window.audioCatalog;
const categoryNames = [
  ["Nhân vật", "◆"], ["Hệ Súng", "✦"], ["Hệ Cận chiến", "⚔"], ["Hệ Pháp sư", "✧"],
  ["Quái vật và Boss", "◒"], ["HEXCORE và tiến trình", "⬢"], ["Sự kiện gameplay", "⚑"],
  ["Menu và UI", "▥"], ["Môi trường", "◈"], ["Nhạc nền", "♫"],
];
const byId = (id) => document.getElementById(id);
const {
  player, categories, search, results, variant, review, volume, play, stop, seek, loop,
  modes, layers, wave, download, time, detail,
} = Object.fromEntries([
  "player", "categories", "search", "results", "variant", "review", "volume", "play",
  "stop", "seek", "loop", "modes", "layers", "wave", "download", "time", "detail",
].map((id) => [id, byId(id)]));

function read(key, fallback) {
  try { return JSON.parse(localStorage.getItem("brotato-audio." + key)) ?? fallback; }
  catch { return fallback; }
}

function save(key, value) {
  try { localStorage.setItem("brotato-audio." + key, JSON.stringify(value)); }
  catch { /* Review still works in this tab when browsing storage is unavailable. */ }
}

function element(tag, className, text) {
  const node = document.createElement(tag);
  if (className) node.className = className;
  if (text != null) node.textContent = text;
  return node;
}

const params = new URLSearchParams(location.hash.slice(1));
let item = catalog.find((cue) => cue.id === Number(params.get("cue"))) ?? catalog.find((cue) => cue.id === 74);
let mode = "mix";
let category = item.category;
let variantIndex = Math.min(item.variants.length - 1, Math.max(0, Number(params.get("variant")) || 0));
let marks = read("review", {});
let playing = false;
let animationCount = 0;
let request = 0;
const status = byId("playback-status");

function option() { return item.variants[variantIndex]; }

function fold(text) {
  return text.toLowerCase().normalize("NFD").replace(/\p{Diacritic}/gu, "").replaceAll("đ", "d");
}

function storePosition() {
  const target = "#cue=" + item.id + "&variant=" + variantIndex;
  if (location.hash === target) return;
  try { history.replaceState(null, "", target); }
  catch { /* Some file:// contexts restrict History state. */ }
}

function drawCategories() {
  categories.replaceChildren(...categoryNames.map(([name, symbol]) => {
    const button = element("button", "category");
    button.type = "button";
    button.setAttribute("aria-current", String(name === category && !search.value.trim()));
    button.append(element("span", "symbol", symbol), element("span", "", name),
      element("span", "count", catalog.filter((cue) => cue.category === name).length));
    button.firstChild.setAttribute("aria-hidden", "true");
    button.addEventListener("click", () => {
      search.value = "";
      category = name;
      if (category !== item.category) selectItem(catalog.find((cue) => cue.category === name));
      else {
        drawCategories();
        drawResults();
      }
      results.scrollTop = 0;
    });
    return button;
  }));
}

function drawResults() {
  const query = fold(search.value.trim());
  const rows = query ? catalog.filter((cue) => fold(cue.name + " " + cue.branch + " " + cue.category).includes(query))
    : catalog.filter((cue) => cue.category === category);
  byId("category-title").textContent = query ? "Kết quả · " + rows.length : category;
  results.replaceChildren(...rows.map((cue) => {
    const button = element("button", "entry");
    button.type = "button";
    button.setAttribute("aria-current", String(cue.id === item.id));
    button.append(element("span", "idx", String(cue.id).padStart(3, "0")), element("span", "entry-label", cue.name));
    if (cue.variants.some((_, index) => marks[cue.id + "-" + index] === "selected")) {
      button.append(element("span", "badge", "Đã chọn"));
    }
    button.addEventListener("click", () => {
      selectItem(cue);
      results.querySelector('[aria-current="true"]')?.focus({ preventScroll: true });
      if (matchMedia("(max-width: 540px)").matches) {
        detail.scrollIntoView({ block: "start" });
      }
    });
    return button;
  }));
  if (!(rows.length)) results.append(element("p", "empty", "Không tìm thấy mục phù hợp."));
}

function selectItem(cue) {
  resetPlayer();
  item = cue;
  category = cue.category;
  variantIndex = 0;
  mode = "mix";
  drawCategories();
  drawResults();
  drawDetail();
}

function selectedPath() {
  if (mode === "old") return item.old;
  if (mode === "prior") return option().prior;
  if (mode === "miss") return option().miss;
  if (mode.startsWith("layer")) return option().layers[Number(mode.slice(5))].file;
  return option().mix;
}

function filesForComparison() {
  return [["mix", option().miss ? "Bản phối · Trúng" : "Bản phối"],
          ...(option().miss ? [["miss", "Bản phối · Hụt"]] : []),
          ...(item.old ? [["old", "Cue cũ"]] : []),
          ...(option().prior ? [["prior", "Bản phối trước"]] : [])];
}

function storeVariant() {
  review.value = marks[item.id + "-" + variantIndex] || "pending";
  storePosition();
}

function drawDetail() {
  byId("cue-category").textContent = item.category + " / " + item.branch.split(" [")[0];
  byId("cue-title").textContent = item.name;
  byId("description").textContent = item.description;
  byId("timing").textContent = item.timing;
  const art = byId("preview-art");
  art.src = item.visual;
  art.alt = item.visual.endsWith(".gif") ? "GIF tham chiếu động tác từ dự án" : "Hình tham chiếu từ dự án";
  variant.replaceChildren(...item.variants.map((row,index) => {
    const suffix = row.bpm ? " · " + row.bpm + " BPM" : "";
    const choice = element("option", "", row.name + suffix);
    choice.value = String(index);
    return choice;
  }));
  variant.value = String(variantIndex);
  const source = byId("sources");
  source.textContent = item.sources.length
    ? "Thiết kế nguyên bản + Foley CC0. Nguồn: " + item.sources.map((name) => name + (name.startsWith("swish-") ? ".wav" : ".ogg")).join(", ") + "."
    : "Thiết kế âm thanh nguyên bản của demo.";
  if (item.projectLayers?.length) {
    source.textContent += " Lớp tái dùng từ dự án: " + item.projectLayers.map((name) => name + ".wav").join(", ") + ".";
  }
  storeVariant();
  drawModes();
}

function chooseMode(next) {
  if (mode !== next) {
    resetPlayer();
    mode = next;
    drawModes();
  }
  startPlayer();
}

function drawModes() {
  const active = option();
  modes.replaceChildren(...filesForComparison().map(([value, label]) => {
    const button = element("button", "", label);
    button.type = "button";
    button.setAttribute("aria-pressed", String(mode === value));
    button.addEventListener("click", () => {
      chooseMode(value);
      const newButton = [...modes.children].find((node) => node.textContent === label);
      newButton?.focus();
    });
    return button;
  }));
  layers.replaceChildren(...active.layers.map((layer, index) => {
    const value = "layer" + index;
    const button = element("button", "layer");
    button.type = "button";
    button.setAttribute("aria-pressed", String(mode === value));
    const title = element("span", "", "");
    title.append(element("span", "layer-symbol", "▶"), document.createTextNode(layer.label));
    title.firstChild.setAttribute("aria-hidden", "true");
    const marksText = layer.marks?.map((stamp) => stamp.toFixed(2).replace(".", ",")).join(" · ");
    button.append(title, element("small", "", marksText ? marksText + " s" : "Solo"));
    button.addEventListener("click", () => { chooseMode(value); layers.children[index]?.focus(); });
    return button;
  }));
  if (!active.layers.length) layers.append(element("p", "", "Cue đơn, nghe toàn bộ bằng nút Phát."));
  const showWave = mode === "mix";
  wave.replaceChildren(...(showWave ? active.wave.map((peak) => {
    const bar = element("i");
    bar.style.height = Math.max(4,peak * 88) + "%";
    return bar;
  }) : []));
  const focusedLayer = mode.startsWith("layer") ? active.layers[Number(mode.slice(5))] : null;
  const repeat = (active.loop || focusedLayer?.loop) && mode !== "old" && mode !== "prior";
  loop.parentElement.hidden = !repeat;
  loop.checked = Boolean(repeat);
  player.loop = loop.checked;
  download.href = selectedPath();
  updateMeter();
}

function resetPlayer() {
  ++request;
  player.pause();
  if (player.hasAttribute("src")) {
    player.removeAttribute("src");
    player.load();
  }
  playing = false;
  status.textContent = "";
}

function restartAnimation() {
  if (item.visual.endsWith(".gif")) byId("preview-art").src = item.visual + "?play=" + ++animationCount;
}

async function startPlayer() {
  const path = selectedPath();
  if (!path) return;
  const id = ++request;
  if (player.getAttribute("src") !== path) player.src = path;
  try {
    player.currentTime = 0;
    await player.play();
    if (id !== request) return;
    playing = true;
    status.textContent = "Đang phát " + (mode === "old" ? "cue cũ" : (mode === "miss" ? "bản hụt" : mode.startsWith("layer") ? option().layers[Number(mode.slice(5))].label : "bản phối"));
    restartAnimation();
    requestAnimationFrame(animate);
  } catch {
    if (id === request) status.textContent = "Trình duyệt chưa phát được tệp này. Hãy thử mở WAV bằng nút tải.";
  }
}

function clock(seconds) {
  return Math.floor(seconds / 60) + ":" + String(Math.floor(seconds % 60)).padStart(2, "0");
}

function updateMeter() {
  const end = Number.isFinite(player.duration) && player.duration>0 ? player.duration
    : (mode === "old" || mode === "prior" ? 0 :
      mode.startsWith("layer") ? option().layers[Number(mode.slice(5))].seconds || option().seconds : option().seconds);
  const now = player.currentTime || 0;
  const fraction = end ? Math.max(0, Math.min(1, now / end)) : 0;
  seek.value = Math.round(fraction * 1000);
  time.textContent = clock(now) + " / " + clock(end);
  [...wave.children].forEach((bar, i) => bar.classList.toggle("played", i < fraction * wave.children.length));
}

function animate() {
  updateMeter();
  if (playing && !player.paused) requestAnimationFrame(animate);
}

stop.addEventListener("click", () => {
  ++request;
  player.pause();
  if (player.hasAttribute("src")) player.currentTime = 0;
  playing = false;
  status.textContent = "Đã dừng";
  updateMeter();
});
play.addEventListener("click", startPlayer);
player.addEventListener("loadedmetadata", updateMeter);
player.addEventListener("ended", () => {
  playing = false;
  status.textContent = "Đã phát hết";
  updateMeter();
});
seek.addEventListener("input", () => {
  if (Number.isFinite(player.duration)) player.currentTime = Number(seek.value) * player.duration / 1000;
  updateMeter();
});
loop.addEventListener("change", () => { player.loop = loop.checked; });
variant.addEventListener("change", () => {
  resetPlayer();
  variantIndex = Number(variant.value);
  mode = "mix";
  storeVariant();
  drawModes();
});
review.addEventListener("change", () => {
  marks[item.id + "-" + variantIndex] = review.value;
  save("review", marks);
  drawResults();
});
volume.value = read("volume", 65);
function changeVolume() {
  byId("volume-label").value = volume.value + "%";
  player.volume = Number(volume.value) / 100;
  save("volume", Number(volume.value));
}
volume.addEventListener("input", changeVolume);
search.addEventListener("input", () => {
  drawResults();
  drawCategories();
  results.scrollTop = 0;
});
byId("total").textContent = catalog.length + " mục âm thanh";
changeVolume();
drawCategories();
drawResults();
drawDetail();

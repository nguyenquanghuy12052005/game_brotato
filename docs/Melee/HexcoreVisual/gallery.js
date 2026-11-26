"use strict";

const { rows, forms, headings, statuses } = window.hexcoreReview;
const byId = (id) => document.getElementById(id);
const form = byId("form");
const skill = byId("skill");
const status = byId("status");
const speed = byId("speed");
const mode = byId("mode");
const query = new URLSearchParams(location.search);
let heading = headings.includes(query.get("heading")) ? query.get("heading") : "E";
let timer;
let replay = 0;

function option(value, label) {
  return new Option(label, value);
}

forms.forEach((name, index) => form.add(option(String(index + 1), name)));
statuses.forEach((name) => status.add(option(name, name)));
form.value = forms[Number(query.get("form")) - 1] ? query.get("form") : "1";

function formRows() {
  return rows.filter((row) => row.form === form.value &&
    (!status.value || row.art_status === status.value));
}

function updateSkills(preferred) {
  const skills = new Map(formRows().map((row) => [row.id, row.skill]));
  skill.replaceChildren(...Array.from(skills, ([id, name]) => option(id, name)));
  if (skills.has(preferred)) skill.value = preferred;
}

function showMedia(player, label, source, filename) {
  const figure = document.createElement("figure");
  if (source) {
    const img = document.createElement("img");
    img.src = replay ? `${source}?r=${replay}` : source;
    img.alt = filename;
    img.addEventListener("error", () => showUnavailable(img, "Không mở được tệp GIF."));
    figure.append(img);
  } else {
    const empty = document.createElement("div");
    showUnavailable(empty, "Chưa có bản ghi ở chế độ này.");
    figure.append(empty);
  }
  const caption = document.createElement("figcaption");
  caption.textContent = label;
  figure.append(caption);
  player.append(figure);
}

function showUnavailable(element, message) {
  const text = document.createElement("div");
  text.className = "missing";
  text.textContent = message;
  if (element.isConnected) element.replaceWith(text);
  else {
    element.className = text.className;
    element.textContent = message;
  }
}

function render() {
  clearTimeout(timer);
  const allowed = formRows();
  const available = allowed.filter((row) => row.id === skill.value);
  const selected = available.find((item) => item.heading === heading) || available[0];
  if (selected) heading = selected.heading;
  const directions = byId("directions");
  directions.replaceChildren();
  for (const direction of headings) {
    const button = document.createElement("button");
    button.textContent = direction;
    button.disabled = !available.some((row) => row.heading === direction);
    button.setAttribute("aria-pressed", String(Boolean(selected && direction === heading)));
    button.onclick = () => { heading = direction; render(); };
    directions.append(button);
  }
  const catalog = byId("catalog");
  catalog.replaceChildren();
  for (const row of allowed) {
    const button = document.createElement("button");
    button.textContent = `${row.skill.split(" · ")[0]} · ${row.heading}`;
    button.title = row.art_status;
    if (row === selected) button.setAttribute("aria-current", "true");
    button.onclick = () => { skill.value = row.id; heading = row.heading; render(); };
    catalog.append(button);
  }
  const player = byId("players");
  player.replaceChildren();
  const index = allowed.indexOf(selected);
  byId("position").textContent = index < 0 ? "0 / 0" : `${index + 1} / ${allowed.length}`;
  byId("prev").disabled = index <= 0;
  byId("next").disabled = index < 0 || index >= allowed.length - 1;
  byId("title").textContent = selected?.skill || "Không có biến thể phù hợp";
  byId("context").textContent = selected ? `${forms[Number(selected.form) - 1]} · ${heading}` : "";
  const badge = byId("badge");
  badge.hidden = !selected;
  badge.textContent = selected?.art_status || "";
  badge.classList.toggle("review", Boolean(selected?.art_status.includes("Review")));
  byId("notes").textContent = selected?.notes || "Hãy chọn trạng thái khác để tiếp tục.";
  const evidence = byId("evidence");
  evidence.textContent = selected?.evidence || "";
  if (selected?.movie) {
    const link = document.createElement("a");
    link.href = speed.value === "slow" ? selected.movie.slow : selected.movie.normal;
    link.textContent = "Xem video có tiếng";
    evidence.append(" ", link);
  }
  byId("media-status").textContent = "";
  if (!selected) return;
  const slow = speed.value === "slow";
  const clipMode = mode.value;
  const media = selected.media;
  if (clipMode === "compare") {
    showMedia(player, slow && media.beforeSlow ? "Trước · 0,5×" : "Trước · 1×",
      slow && media.beforeSlow ? media.beforeSlow : media.before, `Bản trước, ${selected.skill}`);
    showMedia(player, slow ? "Hiện tại · 0,5×" : "Hiện tại · 1×",
      slow ? media.currentSlow : media.current, `Bản hiện tại, ${selected.skill}`);
  } else {
    const label = { current: "Hiện tại", clean: "Tắt VFX", chain: "Chuỗi Basic và Skill" }[clipMode];
    const source = media[slow ? clipMode + "Slow" : clipMode];
    showMedia(player, `${label} · ${slow ? "0,5×" : "1×"}`, source, `${selected.skill}, ${heading}`);
  }
  if (clipMode === "compare" && slow && !media.beforeSlow) {
    byId("media-status").textContent = "Mốc trước chỉ có bản 1× cho biến thể này.";
  }
  if (byId("auto").checked) timer = setTimeout(() => navigate(1, true), slow ? 13000 : 7500);
}

function navigate(offset, wrap = false) {
  const allowed = formRows();
  const index = allowed.findIndex((row) => row.id === skill.value && row.heading === heading);
  if (index < 0) return;
  const target = allowed[wrap ? (index + offset + allowed.length) % allowed.length : index + offset];
  if (!target) return;
  skill.value = target.id;
  heading = target.heading;
  render();
}

form.onchange = status.onchange = () => { updateSkills(skill.value); render(); };
skill.onchange = mode.onchange = speed.onchange = byId("auto").onchange = render;
byId("replay").onclick = () => { replay++; render(); };
byId("prev").onclick = () => navigate(-1);
byId("next").onclick = () => navigate(1);
updateSkills(query.get("skill") || "01");
render();

from array import array
from hashlib import sha256
import json
from pathlib import Path
import re
import wave

ROOT = Path(__file__).resolve().parent
CATALOG = (ROOT/"catalog.js").read_text(encoding="utf-8")
entries = json.loads(CATALOG.removeprefix("window.audioCatalog = ").rstrip(";\n"))
sources = json.loads((ROOT/"used-foley.json").read_text())
assert [entry["id"] for entry in entries] == list(range(1,147))
assert len({row["category"] for row in entries}) == 10

seen = set()
peak = 0
pairs = variants = stems = comparisons = 0

def check_audio(path, duration):
    global peak
    file = ROOT/path
    assert path not in seen, path
    seen.add(path)
    with wave.open(str(file)) as reader:
        assert reader.getnchannels() in {1,2} and reader.getframerate() == 32000 and reader.getsampwidth() == 2, path
        assert abs(reader.getnframes()/32000-duration) <= .0006, path
        data = array("h",reader.readframes(reader.getnframes()))
    found = max(map(abs,data))
    assert 60 < found < 32767, (path,found)
    peak = max(peak,found)
    return data

records = []
for row in entries:
    assert (ROOT/row["visual"]).is_file(), row["id"]
    if row["old"]:
        assert (ROOT/row["old"]).is_file(), row["id"]
        comparisons += 1
    misses = 0
    stem_total = 0
    for variant in row["variants"]:
        variants += 1
        samples = check_audio(variant["mix"],variant["seconds"])
        if variant.get("prior"):
            prior = ROOT/variant["prior"]
            assert prior.is_file() and sha256(prior.read_bytes()).digest()!=sha256((ROOT/variant["mix"]).read_bytes()).digest()
        for layer in variant["layers"]:
            stem_total += 1
            stems += 1
            stem = check_audio(layer["file"],layer.get("seconds") or variant["seconds"])
            if layer.get("loop"):
                assert abs(stem[0]-stem[-1])<2400, layer
            assert all(0<=start<variant["seconds"] for start in layer.get("marks",[])), layer
        if variant.get("miss"):
            assert samples != check_audio(variant["miss"],variant["seconds"]), row["name"]
            pairs += 1
            misses += 1
    for key in row["sources"]:
        assert key+".ogg" in sources or ("swishes-original/"+key+".wav") in sources, (row["id"],key)
    if row["id"] == 138:
        assert len(row["variants"]) == 3
        assert len({sha256((ROOT/t["mix"]).read_bytes()).digest() for t in row["variants"]}) == 3
    if row["id"] == 74:
        with wave.open(str(ROOT/row["variants"][0]["miss"])) as reader:
            reader.setpos(round(.82*32000))
            fall = array("h",reader.readframes(6800))
            assert max(map(abs,fall)) > 2500
    mix = row["variants"][0]["mix"]
    abbreviations = ", ".join(row["sources"]) or "—"
    project = ", ".join(row.get("projectLayers",[])) or "—"
    state = "Phối Foley mới · Chờ nghe" if row.get("revision") else "Giữ bản thử · Chờ nghe"
    records.append(f'| {row["id"]} | [{row["name"]}](index.html#cue={row["id"]}) | {row["branch"]} | [{len(row["variants"])}]({mix}) | {stem_total} | {misses} | {"Có" if row["old"] or row["variants"][0].get("prior") else "—"} | {project} | {abbreviations} | {state} |')

actual = {p.relative_to(ROOT).as_posix() for bank in ("audio","review-audio") for p in (ROOT/bank).glob("*.wav")}
assert seen.issubset(actual)
assert all(path.startswith("audio/") for path in actual-seen), sorted(actual-seen)
for name,digest in sources.items():
    assert sha256((ROOT/"foley"/name).read_bytes()).hexdigest()==digest, name
size = sum((ROOT/p).stat().st_size for p in seen)/1024**2
report = f"""# Kiểm kê Audio Preview

Bản nghe thử ngày 28/11/2026. Đây là tình trạng trước khi duyệt trên máy người nghe. Trạng thái có thể thay đổi theo biến thể trong demo. Ký hiệu `[G/T/A/P]` chỉ nguồn sự kiện trong game, theo [kế hoạch](../../docs/HEXCORE_AUDIO_SYSTEM_PLAN.md).

**{len(entries)} mục · {variants} bản phối · {stems} stem · {pairs} bản hụt · {comparisons} mục có cue gốc.** Tổng {len(seen)} WAV đang tham chiếu, {size:.1f} MiB. Có {len(sources)} tệp nguồn Foley được liệt kê cho bản phối. Tên cột nguồn bỏ đuôi .ogg/.wav để dễ quét. [Hồ sơ giấy phép](LICENSES.md)

| STT | Mục | Phạm vi | Bản phối | Stem | Bản hụt | Cue cũ | Lớp WAV dự án | Foley CC0 | Duyệt |
| --- | --- | --- | ---: | ---: | ---: | --- | --- | --- | --- |
""" + "\n".join(records)+"\n"
(ROOT/"INVENTORY.md").write_text(report,encoding="utf-8")
print(f"PASS · {len(entries)} cues, {variants} mixes, {stems} stems, {pairs} miss renders, {comparisons} A/B cues")
print(f"{len(seen)} WAVs, {size:.1f} MiB, greatest sample {peak}/32767; {len(sources)} source checksums")

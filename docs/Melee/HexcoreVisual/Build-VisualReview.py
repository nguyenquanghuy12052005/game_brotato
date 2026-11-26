"""Compile the offline gallery from Unity evidence and the visual reviews."""

import csv
import json
import re
from collections import Counter
from pathlib import Path

HERE = Path(__file__).resolve().parent
REPO = HERE.parents[2]
PREVIOUS = HERE.parent / "HexcoreRemaining" / "HexcoreSkillMatrix.csv"
BEFORE_MEDIA = HERE.parent / "HexcoreFinal" / "Review"

FORMS = ["Kiếm sĩ", "Sát thủ", "Đao sĩ", "Kiếm khí"]
HEADINGS = "N NE E SE S SW W NW".split()
STATUSES = ["MissingArt", "SourcePoseBlockout", "AuthoredBlockout", "Ready for Review", "In Review", "Approved"]
CRITERIA = ["continuity", "body_weapon", "foot", "readability", "identity", "impact", "vfx_sync", "wow"]
MOVIES = {"13": "R1-Thien-Van-Tram", "14": "R2-Tam-Thien-Kiem-Quyet", "15": "R3-Van-Anh-Tuyet-Sat"}


def read_csv(path):
    with path.open(encoding="utf-8-sig", newline="") as source:
        return list(csv.DictReader(source))


def key(row):
    return row["form"], row["skill"], row["heading"]


def status(row):
    return {"InReview": "In Review", "ReadyForReview": "Ready for Review"}.get(row["art_status"], row["art_status"])


def relative_media(folder, stem):
    path = folder / (stem + ".gif")
    if not path.is_file():
        return ""
    if folder == BEFORE_MEDIA:
        return "../HexcoreFinal/Review/" + path.name
    return "Media/" + path.name


def media_for(row, skill_id):
    suffix = f"{skill_id}-{row['form']}-{row['heading']}"
    names = {"current": "Skill", "clean": "Clean", "chain": "Chain"}
    media = {}
    for mode, prefix in names.items():
        media[mode] = relative_media(HERE / "Media", f"{prefix}-{suffix}")
        media[mode + "Slow"] = relative_media(HERE / "Media", f"{prefix}-{suffix}-Slow")
    media["before"] = relative_media(HERE / "Media", f"Before-{suffix}") or relative_media(BEFORE_MEDIA, f"Skill-{suffix}")
    media["beforeSlow"] = relative_media(HERE / "Media", f"Before-{suffix}-Slow") or relative_media(BEFORE_MEDIA, f"Skill-{suffix}-Slow")
    if status(row) not in {"MissingArt"} and not all(media[name] for name in
        ("current", "currentSlow", "clean", "cleanSlow", "chain", "chainSlow", "before")):
        raise ValueError(f"Missing camera evidence {suffix}")
    return media


def main():
    catalog = (REPO / "Assets/Prefabs/Player/Melee/Scripts/MeleeSkillController.cs").read_text(encoding="utf-8-sig")
    roster = re.findall(r'Entry\("([^"]+)"', catalog)
    skill_ids = {name: f"{index:02d}" for index, name in enumerate(roster, 1)}
    baseline = {key(row): row for row in read_csv(PREVIOUS)}
    rows = read_csv(HERE / "HexcoreRuntimeReview.csv")
    scores = {row["id"]: row for row in read_csv(HERE / "VisualScores.csv")}
    assert len(set(map(key, rows))) == len(rows) == len(baseline) == 264
    assert set(map(key, rows)) == set(baseline)
    assert len(skill_ids) == len(scores) == 24
    for review in scores.values():
        assert all(1 <= int(review[field]) <= 5 for field in CRITERIA)

    gallery = []
    results = []
    for row in rows:
        skill_id = skill_ids[row["skill"]]
        sample = scores[skill_id]
        before = status(baseline[key(row)])
        current = status(row)
        assert current in STATUSES
        assets = media_for(row, skill_id)
        reviewed = row["form"] == sample["form"] and row["heading"] == sample["heading"]
        hero = skill_id in {"13", "14", "15"}
        if reviewed:
            evidence = "Nhận xét trực tiếp từ clip đại diện. Cột điểm ghi trong VisualScores.csv."
        elif hero:
            evidence = "Có clip từng hướng. Nhận xét chung của kỹ năng nằm trong VisualScores.csv."
        else:
            evidence = "Có clip từng hướng. Biến thể này giữ trạng thái từ mốc trước và cần duyệt pose riêng."
        notes = sample["notes"] if reviewed else "Điểm cần rà của kỹ năng: " + sample["notes"]
        record = dict(row, id=skill_id, previous_status=before, art_status=current, visual_sample=reviewed,
                      visual_notes=notes, current_gif=assets["current"], clean_gif=assets["clean"], chain_gif=assets["chain"])
        results.append(record)
        item = dict(form=row["form"], id=skill_id, skill=row["skill"], heading=row["heading"], art_status=current,
                    media=assets, notes=notes, evidence=evidence)
        if reviewed and hero:
            movie = {rate: f"Videos/{MOVIES[skill_id]}-{suffix}.mp4" for rate, suffix in (("normal", "1x"), ("slow", "05x"))}
            if all((HERE / path).is_file() for path in movie.values()):
                item["movie"] = movie
        gallery.append(item)
    with (HERE / "HexcoreVisualReview.csv").open("w", encoding="utf-8-sig", newline="") as output:
        writer = csv.DictWriter(output, fieldnames=list(results[0]))
        writer.writeheader()
        writer.writerows(results)
    data = dict(forms=FORMS, headings=HEADINGS, statuses=STATUSES, rows=gallery)
    (HERE / "review-data.js").write_text("window.hexcoreReview = " + json.dumps(data, ensure_ascii=False) + ";\n", encoding="utf-8")
    print("Before:", Counter(status(row) for row in baseline.values()))
    print("After:", Counter(row["art_status"] for row in results))


if __name__ == "__main__":
    main()

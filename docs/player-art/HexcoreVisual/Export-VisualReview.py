"""Encode camera captures as looping GIFs with a shared palette and frame deltas."""

import argparse
from concurrent.futures import ProcessPoolExecutor
from pathlib import Path
import re

from PIL import Image

CLIP = re.compile(r"^(Skill|Clean|Chain)-(\d{2})-[1-4]-(N|NE|E|SE|S|SW|W|NW)$")
MEDIA = Path(__file__).resolve().parent / "Media"


def encode(job):
    folder, stem = job
    images = []
    for path in sorted(folder.glob("*.png")):
        with Image.open(path) as shot:
            images.append(shot.convert("RGB"))
    if len(images) < 2:
        raise ValueError(f"Missing capture frames: {folder}")
    width, height = images[0].size
    sample_count = min(12, len(images))
    sample = Image.new("RGB", (width, height * sample_count))
    for i in range(sample_count):
        frame = i * (len(images) - 1) // (sample_count - 1)
        sample.paste(images[frame], (0, i * height))
    palette = sample.quantize(colors=256)
    frames = [image.quantize(palette=palette, dither=Image.Dither.NONE) for image in images]
    delays = [70 if i % 2 == 0 else 80 for i in range(len(images))]
    delays[-1] = 400
    for suffix, scale in (("", 1), ("-Slow", 2)):
        target = MEDIA / f"{stem}{suffix}.gif"
        temp = target.with_suffix(".gif.tmp")
        frames[0].save(temp, format="GIF", append_images=frames[1:], save_all=True, loop=0,
                       duration=[delay * scale for delay in delays], optimize=True, disposal=1)
        temp.replace(target)


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("camera_directory", type=Path)
    parser.add_argument("--skill", type=int, choices=range(1, 25))
    parser.add_argument("--baseline", action="store_true", help="Re-encode the previous Unity camera captures")
    args = parser.parse_args()
    folders = {p: CLIP.fullmatch(p.name) for p in args.camera_directory.iterdir() if p.is_dir()}
    folders = {p: m for p, m in folders.items() if m}
    if args.baseline:
        folders = {p: m for p, m in folders.items() if m[1] == "Skill"}
    expected = 264 if args.baseline else 792
    if len(folders) != expected:
        raise ValueError(f"Expected {expected} camera clips, found {len(folders)}.")
    selected = [(p, "Before-" + p.name.removeprefix("Skill-") if args.baseline else p.name)
                for p, m in sorted(folders.items()) if args.skill is None or int(m[2]) == args.skill]
    MEDIA.mkdir(exist_ok=True)
    with ProcessPoolExecutor(max_workers=4) as pool:
        for _ in pool.map(encode, selected, chunksize=4):
            pass
    print(f"Encoded {len(selected) * 2} GIFs")


if __name__ == "__main__":
    main()

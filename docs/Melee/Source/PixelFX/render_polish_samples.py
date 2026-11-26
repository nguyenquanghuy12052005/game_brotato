"""Raster recipes for the HEXCORE v2.1 samples. Run from any directory with Pillow."""
import hashlib
import html
import json
import math
import random
import sys
from pathlib import Path

from PIL import Image, ImageDraw, ImageFont

SOURCE = Path(__file__).resolve().parent
ROOT = SOURCE.parents[3]
MELEE = ROOT / "Assets/Prefabs/Player/Melee"
OUTPUT = MELEE / "Shared/VFX/PixelFX/Resources/HexcoreV21"
REVIEW = ROOT / "docs/Melee/HexcoreVisual/V2_1"
CELL, PIVOT, COLUMNS = 112, 56, 18
TAU = 2 * math.pi

SILVER = ("#335e85", "#81b8da", "#e5f2fc", "#ffffff")
DAGGER = ("#3c1b69", "#9659ca", "#e3a8f3", "#faf0ff")
GOLD = ("#893818", "#e67d26", "#ffd38a", "#fff3d6")
TEAL = ("#116c82", "#22bcbf", "#8cf4e9", "#edffff")
BLOOD = ("#4c1d58", "#ac2d74", "#fb7198", "#ffd4e4")


def curve(x, y, radius, angle):
    return round(x + radius * math.cos(angle)), round(y + radius * math.sin(angle))


def ribbon(draw, x, y, angle, span, radius, width, palette, tail=0.0):
    steps = 32
    factors = [k / steps for k in range(steps + 1)]
    for layer, color in enumerate(palette):
        rim = radius - layer * width * .12
        thickness = width * (1 - layer * .23)
        outer = [curve(x, y, rim + tail * (1 - p), angle + span * (p - .5)) for p in factors]
        inner = [
            curve(x, y, rim + tail * (1 - p) - thickness * math.sin(math.pi * p) ** .65, angle + span * (p - .5))
            for p in reversed(factors)
        ]
        draw.polygon(outer + inner, fill=color)


def particles(draw, rng, x, y, age, colors, count, spread, distance=38, angle=0):
    for k in range(count):
        a = angle + rng.uniform(-spread, spread)
        speed, delay = rng.uniform(.32, 1), rng.uniform(0, .22)
        t = min(1, max(0, age - delay))
        if t == 0 or age > rng.uniform(.66, 1.12):
            continue
        px, py = curve(x, y, 3 + t * speed * distance, a)
        size = 2 if k % 4 == 0 and t < .5 else 1
        draw.polygon([(px-size, py), (px, py-size), (px+size*2, py), (px, py+size)], fill=colors[k % len(colors)])


def cut(draw, name, t, rng):
    expand = min(1, (t + .20) * 3.8)
    if name == "basic_silver":
        ribbon(draw, PIVOT, PIVOT, .48 - t, 1.72 * expand, 45, 6 * (1 - .72*t), SILVER)
        particles(draw, rng, 85, 56, t, SILVER[1:], 11, .9, 25)
    elif name == "basic_dagger":
        # A broken diagonal pair reads as a quick knife flick, with a separate stab recipe.
        ribbon(draw, PIVOT, PIVOT, -.20 + .20*t, .91 * expand, 44, 6, DAGGER, 3)
        ribbon(draw, PIVOT, PIVOT, .52 + .24*t, .51 * expand, 42, 4, DAGGER[:3], 2)
        draw.line([(77, 74), (86, 65), (99, 59)], fill=DAGGER[1], width=2)
        particles(draw, rng, 85, 54, t, DAGGER[1:], 12, .54, 22)
    elif name == "basic_thrust":
        tip, h = 78 + int(28 * expand), 10 * (1 - .73*t)
        for k, color in enumerate(DAGGER):
            draw.polygon([(60+3*k, 56), (tip-18, 56-h+1.8*k), (tip, 56),
                          (tip-20, 56+h-1.8*k), (64+3*k, 56+3-k)], fill=color)
        particles(draw, rng, 91, 56, t, DAGGER[1:], 11, .36, 19)
    elif name == "basic_heavy":
        ribbon(draw, PIVOT, PIVOT, .05 + .8*t, 2.13 * expand, 47, 16 * (1 - .62*t), GOLD, 2)
        particles(draw, rng, 81, 60, t, GOLD, 23, 1.06, 34)
    elif name == "basic_drop":
        for i, color in enumerate(GOLD):
            x, h = 100-i, 10-i*2
            draw.polygon([(72+i*4, 27), (x-3, 52), (x, 72), (x-9, 63+h), (82-i, 52)], fill=color)
        particles(draw, rng, 88, 66, t, GOLD, 22, math.pi, 21)
    elif name == "basic_qi":
        ribbon(draw, PIVOT, PIVOT, .42-t*.8, 1.48 * expand, 45, 9 * (1 - .63*t), TEAL)
        ribbon(draw, PIVOT, PIVOT, .25-t, .83 * expand, 36, 2, TEAL[:2])
        particles(draw, rng, 86, 55, t, TEAL[1:], 16, .98, 25)


def orbit(draw, t, rng):
    # Partial cuts with uneven spacing read as blades passing around a foot plane.
    for radius, thickness, start, palette in [(45, 9, t*1.7+.42, SILVER), (41, 3, -t*1.9+2.7, TEAL)]:
        for i, span in enumerate((1.61, 1.15, .58)):
            a = start + i * TAU/3
            colors = palette if math.sin(a)>-.28 else palette[:2]
            ribbon(draw, PIVOT, PIVOT, a, span, radius, thickness*(1-.53*t), colors, 2.1)
            px, py = curve(PIVOT, PIVOT, radius+.5, a+.42)
            draw.polygon([(px-3, py), (px, py-1), (px+4, py), (px, py+2)], fill=colors[-1])
    particles(draw, rng, PIVOT, PIVOT, .46+t*.46, SILVER[1:], 19, math.pi, 53)


def seal(draw, t, rng):
    r = 27 + 3*math.sin(TAU*t)
    for n, palette in [(6, BLOOD[:3]), (3, BLOOD[1:])]:
        for k in range(n):
            a = k*TAU/n - math.pi/2
            p = [curve(PIVOT, PIVOT, r+spread, a+offset) for spread,offset in [(0,-.33),(7,0),(0,.33),(-5,0)]]
            draw.polygon(p, fill=palette[k%len(palette)])
    draw.polygon([(56,38),(64,52),(61,67),(56,73),(51,67),(48,52)], fill=BLOOD[1])
    draw.polygon([(56,45),(60,55),(56,65),(52,55)], fill=BLOOD[-1])
    particles(draw, rng, 56, 56, .30+t*.45, BLOOD[1:], 9, math.pi, 40)


def rupture(draw, t, rng):
    radius = 43 + 6*t
    for i in range(8):
        a = i*TAU/8 + .17
        if i%2 or t < .7:
            palette = BLOOD if t < .58 else BLOOD[:2]
            ribbon(draw, 56, 56, a, .6, radius, max(2, 10*(1-t)), palette)
    if t < .30:
        r = (1-t)*19
        draw.polygon([curve(56, 56, r if k%2==0 else r*.3, k*TAU/12) for k in range(12)], fill=BLOOD[-1])
    particles(draw, rng, 56, 56, .16+t*.7, BLOOD[1:], 40, math.pi, 62)


def slam(draw, t, rng):
    for i in range(9):
        a = TAU*i/9 + rng.uniform(-.18,.18)
        end = curve(56, 56, rng.uniform(31,49), a)
        middle = curve(56, 56, rng.uniform(14,26), a-.16)
        draw.line([(56,56), middle, end], fill=GOLD[0], width=4)
        if t < .60:
            draw.line([(56,56), middle, end], fill=GOLD[2 if t < .23 else 1], width=1)
            draw.line([middle, curve(56,56,35,a+.25)], fill=GOLD[1], width=1)
    for i, span in enumerate((.42,.76,.21,.52,.22)):
        ribbon(draw, 56, 56, TAU*i/5 + .05, span, 45 + t*4, 5*(1-t*.85), GOLD[1:3])
    for i in range(22):
        a = i * 2.39996
        r = (10 + (i%8)*4) + t*11
        x,y = curve(56,56,r,a)
        y -= round(math.sin(t*math.pi) * (i%5+2)*2)
        d = 4 - i%3
        if i%6 < (1-t)*6:
            draw.polygon([(x-d,y),(x-1,y-d),(x+d,y-2),(x+d,y+d),(x,y+d+1)], fill=GOLD[i%3])
    particles(draw, rng, 56, 56, .15+t*.84, GOLD[1:], 30, math.pi, 64)


def crescent(draw, t, rng):
    # Beam nose x=70 is the pivot; the flanks recede by 27 px at y=9 and 103.
    profile = [(round(70-27*q*q),round(56+47*q)) for q in (i/24 for i in range(-24,25))]
    for width, color in [(18,TEAL[0]),(12,TEAL[1]),(7,TEAL[2]),(3,TEAL[3])]:
        back = [(x-round(width*math.sin(math.pi*i/48)**.67),y) for i,(x,y) in enumerate(profile)]
        draw.polygon(profile + back[::-1],fill=color)
    for i in range(5,44,5):
        px,py=profile[i]
        d=2+(i%3)
        draw.line([(px-7-d,py),(px-3,py)],fill=TEAL[1],width=1)
    for i in range(25):
        y = rng.randint(17,94)
        x = 66 - int(abs(y-56)*.56) - rng.randint(7,42)
        shift = int((t+.09) * (6+i%8))
        draw.line([(max(1,x-shift),y),(x+3,y)], fill=TEAL[i%3], width=1+(i%6==0))


def rev2(draw, name, t, rng):
    if name == "dao_fan":
        # World origin is the center texel. The lit edge spans exactly -40..+40 degrees.
        ribbon(draw, 56, 56, 0, math.radians(80), 55, 8-3*t, GOLD)
        ribbon(draw, 56, 56, 0, math.radians(61), 46-5*t, 2.5, GOLD[:2])
        for i in range(24):
            angle = rng.uniform(-.66,.66)
            x,y = curve(56,56,rng.uniform(26,47)-t*7,angle)
            draw.line([(x-2-i%3,y),(x+1,y-i%2)],fill=GOLD[i%3],width=1)
    elif name == "ambush_cut":
        ribbon(draw, 54,56,.33-.59*t,1.48,47,8-3*t,DAGGER,3)
        ribbon(draw, 54,56,.39-.65*t,1.08,40-4*t,3,DAGGER[:3],4)
        particles(draw,rng,83,55,t,DAGGER[1:],15,.97,22)
    elif name == "shadow_dagger":
        draw.polygon([(90,56),(72,48),(47,52),(45,45),(40,46),(36,52),(23,53),
                      (19,56),(23,59),(36,60),(40,66),(45,67),(47,60),(73,63)],fill=DAGGER[0])
        draw.polygon([(89,56),(71,50),(47,54),(43,55),(47,58),(73,61)],fill=DAGGER[2])
        draw.polygon([(89,56),(68,54),(48,56),(69,58)],fill=DAGGER[3])
        draw.polygon([(40,47),(45,49),(44,62),(40,65),(37,57)],fill=DAGGER[1])
        draw.line([(21,56),(35,56)],fill=DAGGER[2],width=3)
        for i in range(17):
            x,y = rng.randrange(6,48)-round(t*6),rng.randrange(46,67)
            draw.line([(x-3-i%4,y),(x,y)],fill=DAGGER[i%3],width=1)
    elif name in ("qi_sword", "dagger_mark"):
        colors = TEAL if name == "qi_sword" else DAGGER
        draw.polygon([(102,56),(78,46),(34,50),(35,43),(29,42),(24,51),(10,52),
                      (6,56),(10,60),(24,61),(29,70),(35,69),(34,62),(78,66)], fill=colors[0])
        draw.polygon([(99,56),(77,49),(35,52),(31,56),(34,60),(77,63)], fill=colors[2])
        draw.polygon([(100,56),(69,54),(40,55),(69,58)], fill=colors[3])
        draw.polygon([(32,44),(34,50),(30,57),(33,67),(27,65),(27,47)], fill=colors[1])
        draw.rectangle((10,55,25,58),fill=GOLD[2] if name=="qi_sword" else DAGGER[2])
        particles(draw,rng,55,56,t,colors[1:],16,math.pi,29)
    elif name in ("gravity", "war_flame"):
        for i in range(16):
            a = i * TAU/16 + t * (.8 if name=="gravity" else -.8)
            radius = (45 - 22*((t+i*.17)%1)) if name=="gravity" else (24+10*math.sin(i*1.7+t*4))
            x,y = curve(56,56,radius,a)
            d = 4+i%5
            tx,ty = curve(x,y,-d*2,a) if name=="gravity" else (x+d*2,y+(i%3-1)*4)
            draw.polygon([(x,y),(tx,ty-2),(tx+2,ty+2)],fill=GOLD[i%3])
    elif name in ("ambush_finish", "night_finish", "execute_thrust", "cross_blade", "rift", "counter_wave"):
        if name == "ambush_finish":
            ribbon(draw, 55,56,-.3,1.96,46,8,DAGGER,2)
            ribbon(draw, 56,53,math.pi+.20,1.52,43,6,DAGGER[:3],1)
        elif name == "night_finish":
            ribbon(draw, 54,55,.31,1.61,48,11,DAGGER,5)
            draw.polygon([(2,62),(79,49),(111,46),(82,55),(28,67)],fill=DAGGER[2])
        elif name == "execute_thrust":
            draw.polygon([(2,59),(64,48),(111,56),(77,61),(22,63)],fill=DAGGER[1])
            draw.polygon([(19,57),(82,53),(111,56),(73,59)],fill=DAGGER[3])
        elif name == "cross_blade":
            draw.polygon([(4,59),(17,56),(88,48),(111,52),(104,60),(12,62)],fill=TEAL[0])
            draw.polygon([(13,59),(91,51),(108,53),(96,57)],fill=TEAL[3])
        elif name == "counter_wave":
            draw.polygon([(1,58),(27,51),(68,55),(88,46),(71,57),(99,64),(66,60),(26,60)],fill=SILVER[1])
            draw.polygon([(9,58),(58,55),(80,52),(69,57),(84,62),(55,58)],fill=SILVER[3])
        else:
            points=[(1,55),(24,49),(38,60),(63,49),(79,57),(104,49),(111,55)]
            for width,color in zip((14,9,4,1),GOLD):
                draw.line(points,fill=color,width=width,joint="curve")
        particles(draw,rng,65,56,t,TEAL[1:] if name=="cross_blade" else GOLD if name=="rift" else DAGGER[1:],18,.55,42)


RECIPES = {
    **{name: (lambda d,t,r,n=name: cut(d,n,t,r))
       for name in ("basic_silver","basic_dagger","basic_thrust","basic_heavy","basic_drop","basic_qi")},
    "orbit": orbit, "blood_seal": seal, "blood_burst": rupture, "ground_slam": slam, "crescent": crescent,
    **{name: (lambda d,t,r,n=name: rev2(d,n,t,r)) for name in (
       "dao_fan","qi_sword","dagger_mark","gravity","war_flame","ambush_finish","night_finish",
       "execute_thrust","cross_blade","rift","counter_wave","ambush_cut","shadow_dagger")},
    "qi_seal": seal, "qi_burst": rupture, "shadow_gate": rupture,
    "dao_wave": crescent, "passive_wave": crescent, "dao_orbit": orbit,
}



# Upstream atlases are decoded once and sampled by export durations.
# The arguments select a window of the upstream performance and place its pivot in the local cell.
LAYERS = {
    "basic_silver": ("melee_sword-slash", .10, .59, (52, 66), (.62, .62), SILVER, .93, None),
    "basic_dagger": ("melee_dagger-flurry", .13, .43, (55, 63), (.51, .57), DAGGER, .96, None),
    "basic_thrust": ("melee_thrust", .02, .57, (51, 57), (.34, .78), DAGGER, .98, None),
    "basic_heavy": ("melee_heavy-cleave", .115, .43, (52, 64), (.62, .55), GOLD, .97, None),
    "basic_drop": ("melee_heavy-cleave", .23, .69, (52, 65), (.46, .71), GOLD, .96, None),
    "basic_qi": ("melee_sword-slash", .05, .52, (52, 63), (.58, .65), TEAL, .96, None),
    "orbit": ("melee_spin-slash", .16, .58, (56, 72), (.48, .58), TEAL, .54, None),
    "dao_orbit": ("melee_spin-slash", .16,.58,(56,72),(.48,.58),GOLD,.68,None),
    "blood_seal": ("shadow_hex-sigil", .32, .67, (56, 56), (.91, .91), BLOOD, .99, (29, 20, 139, 112, 84, 62)),
    "blood_burst": ("shadow_dark-explosion", .075, .63, (56, 73), (.43, .40), BLOOD, .96, None),
    "ground_slam": ("impacts_ground-slam", .014, .47, (56, 63), (.37, .41), GOLD, .92, None),
    "crescent": ("projectiles_energy-disc", 0, 1, (70, 56), (.85, 1.55), TEAL, .77, (0, 3, 76, 49, 92, 26)),
    "dao_fan": ("melee_heavy-cleave", .11, .47, (59, 56), (.33, .38), GOLD, .50, None),
    "ambush_cut": ("melee_dagger-flurry", .24,.49,(55,62),(.49,.48),DAGGER,.66,None),
    "shadow_dagger": ("melee_thrust",.04,.61,(28,56),(.45,.31),DAGGER,.62,None),
    "qi_sword": ("projectiles_energy-disc", .10,.80,(46,56),(.69,.48),TEAL,.45,(0,3,76,49,92,26)),
    "dagger_mark": ("shadow_hex-sigil", .36,.59,(56,56),(.53,.48),DAGGER,.65,(29,20,139,112,84,62)),
    "gravity": ("melee_spin-slash", .57,.25,(56,62),(.48,.52),GOLD,.81,None),
    "war_flame": ("impacts_ground-slam", .10,.42,(57,55),(.31,.30),GOLD,.70,None),
    "ambush_finish": ("melee_dagger-flurry",.37,.76,(56,57),(.68,.59),DAGGER,.94,None),
    "night_finish": ("melee_sword-slash",.11,.58,(45,60),(.76,.45),DAGGER,.90,None),
    "execute_thrust": ("melee_thrust",.02,.61,(33,56),(.47,.44),DAGGER,.98,None),
    "cross_blade": ("melee_thrust",.10,.49,(31,56),(.41,.31),TEAL,.81,None),
    "rift": ("impacts_ground-slam", .12,.55,(50,56),(.35,.40),GOLD,.82,None),
    "qi_seal": ("shadow_hex-sigil", .22,.54,(56,56),(.91,.65),TEAL,.96,(29,20,139,112,84,62)),
    "qi_burst": ("shadow_dark-explosion", .06,.62,(56,66),(.38,.37),TEAL,.92,None),
    "shadow_gate": ("shadow_dark-explosion",.38,.84,(56,65),(.50,.31),DAGGER,.73,None),
    "dao_wave": ("projectiles_energy-disc",0,1,(70,56),(.8,1.45),GOLD,.78,(0,3,76,49,92,26)),
    "counter_wave": ("melee_thrust",.04,.64,(46,56),(.42,.47),SILVER,.64,None),
    "passive_wave": ("projectiles_energy-disc",0,1,(70,56),(.8,1.45),SILVER,.73,(0,3,76,49,92,26)),
}


def upstream_source(effect_id):
    root = SOURCE / "Upstream" / effect_id
    sheet = Image.open(root / "sheet.png").convert("RGBA")
    data = json.loads((root / "sheet.json").read_text(encoding="utf-8"))
    assert data["meta"]["pixelfx"]["id"] == effect_id.replace("_", "/", 1)
    frames = []
    elapsed = 0
    for item in data["frames"]:
        b = item["frame"]
        frames.append((elapsed, sheet.crop((b["x"], b["y"], b["x"]+b["w"], b["y"]+b["h"]))))
        elapsed += item["duration"]
    return frames, elapsed, data["meta"]["pixelfx"]["pivot"]


def tint_fx(image, palette, opacity):
    stops = [tuple(bytes.fromhex(x[1:])) for x in palette]
    pixels = []
    for r, g, b, a in image.get_flattened_data() if hasattr(image,"get_flattened_data") else image.getdata():
        # Drop weak matte; retain the ordered glow and palette bands of the upstream effect.
        if a < 12:
            pixels.append((0,0,0,0))
            continue
        level = min(2.9999, ((max(r,g,b)*.49 + (r+g+b)/3*.51)/255) ** 1.08 * 3)
        j = int(level)
        ramp = tuple(round(p+(q-p)*(level-j)) for p,q in zip(stops[j],stops[j+1]))
        pixels.append((*ramp, round(a*opacity)))
    tinted = Image.new("RGBA", image.size)
    tinted.putdata(pixels)
    return tinted


def sample_upstream(sources, name, progress):
    effect, start, end, anchor, (sx, sy), palette, opacity, crop = LAYERS[name]
    frames, duration, pivot = sources[effect]
    time = (start+(end-start)*progress)*duration
    index = len(frames)-1
    while index > 0 and frames[index][0] > time:
        index -= 1
    selected = frames[index][1]
    px,py = pivot
    if crop is not None:
        left, top, right, bottom, pivot_x, pivot_y = crop
        selected = selected.crop((left,top,right,bottom))
        px,py = pivot_x-left,pivot_y-top
    foreground = tint_fx(selected, palette, opacity)
    foreground = foreground.resize((round(foreground.width*sx),round(foreground.height*sy)),Image.Resampling.NEAREST)
    overlay = Image.new("RGBA",(CELL,CELL))
    overlay.alpha_composite(foreground,(round(anchor[0]-px*sx),round(anchor[1]-py*sy)))
    return overlay


def fade(picture, a):
    result = picture.copy()
    result.putalpha(result.getchannel("A").point(lambda v: round(v*a)))
    return result


def compose_clip(name, recipe, sources, frame_count):
    frames = []
    for i in range(frame_count):
        t = i/(frame_count-1)
        silhouette = Image.new("RGBA",(CELL,CELL))
        recipe(ImageDraw.Draw(silhouette),t,random.Random(271))
        if name in ("dao_wave","passive_wave","qi_seal","qi_burst","shadow_gate","war_flame","dao_orbit"):
            palette = LAYERS[name][5]
            pixels = {(17,108,130),(34,188,191),(140,244,233),(237,255,255)} if "wave" in name else set()
            if pixels:
                colors = {tuple(bytes.fromhex(src[1:])):tuple(bytes.fromhex(dest[1:])) for src,dest in zip(TEAL,palette)}
                silhouette.putdata([(*colors.get((r,g,b),(r,g,b)),a) for r,g,b,a in silhouette.get_flattened_data()])
            else:
                silhouette = tint_fx(silhouette, palette, .65)
        upstream = sample_upstream(sources,name,t)
        if name in ("basic_silver","basic_dagger","basic_thrust","basic_heavy","basic_drop","basic_qi"):
            # Keep the contact arc, but let the upstream blade and its grit supply the texture.
            frame = fade(silhouette,.68 if name in ("basic_heavy","basic_drop") else .61)
            frame.alpha_composite(upstream)
        elif name == "blood_seal":
            frame = fade(silhouette,.50)
            frame.alpha_composite(upstream)
        else:
            # The custom outer edge tracks the combat footprint. Texture behind it may fade independently.
            frame = upstream
            frame.alpha_composite(fade(silhouette,.88 if name=="crescent" else .77))
        if name != "blood_seal":
            frame = fade(frame, min(1,(1-t)*3.4+.16))
        frames.append(frame)
    return frames


def create_clips():
    sources = {effect: upstream_source(effect) for effect in sorted({row[0] for row in LAYERS.values()})}
    clips = {}
    for name, recipe in RECIPES.items():
        timing = [20,20,22,24,26,26,30,30,33,35,36,38] if name.startswith("basic") else [27,29,30,31,33,33,33,35,35,35,37,40]
        if name not in ("orbit","blood_seal","blood_burst","ground_slam","crescent") and not name.startswith("basic"):
            timing = [32,35,36,38,41,42,43,48,50]
        clips[name] = (compose_clip(name, recipe, sources, len(timing)), timing)
    with Image.open(SOURCE / "HitSpark/sheet.png") as source:
        index = json.loads((SOURCE / "HitSpark/sheet.json").read_text(encoding="utf-8"))
        assert index["meta"]["pixelfx"]["id"] == "impacts/hit-spark"
        assert source.size == (index["meta"]["size"]["w"], index["meta"]["size"]["h"])
        pivot_x,pivot_y = index["meta"]["pixelfx"]["pivot"]
        frames = []
        scale = .60
        for entry in index["frames"]:
            box = entry["frame"]
            art = source.crop((box["x"], box["y"], box["x"]+box["w"], box["y"]+box["h"])).convert("RGBA")
            art = art.resize((round(art.width*scale), round(art.height*scale)), Image.Resampling.NEAREST)
            frame = Image.new("RGBA",(CELL,CELL))
            frame.alpha_composite(art,(PIVOT-round(pivot_x*scale),PIVOT-round(pivot_y*scale)))
            frames.append(frame)
        clips["hit_spark"] = (frames, [entry["duration"] for entry in index["frames"]])
    return clips


def component(clips, name, amount):
    frames, durations = clips[name]
    time = max(0, min(.9999, amount))*sum(durations)
    i = 0
    while time >= durations[i] and i+1 < len(frames):
        time -= durations[i]
        i += 1
    return frames[i]


def export_atlas(clips):
    OUTPUT.mkdir(parents=True, exist_ok=True)
    count = sum(len(frames) for frames,_ in clips.values())
    atlas = Image.new("RGBA", (CELL*COLUMNS, CELL*math.ceil(count/COLUMNS)))
    entries, offset = [], 0
    for name, (frames, times) in clips.items():
        entries.append(dict(id=name, first=offset, durationMs=times))
        for frame in frames:
            atlas.paste(frame, (offset % COLUMNS*CELL, offset//COLUMNS*CELL))
            offset += 1
    atlas.save(OUTPUT/"StyleSheet.png", optimize=True)
    (OUTPUT/"StyleTiming.json").write_text(json.dumps(dict(cell=CELL, columns=COLUMNS, clips=entries), indent=2)+"\n",encoding="utf-8")


def composite(image, sprite, x, y, size=None, flip=False):
    if flip:
        sprite = sprite.transpose(Image.Transpose.FLIP_TOP_BOTTOM)
    if size:
        sprite = sprite.resize((size,size), Image.Resampling.NEAREST)
    image.alpha_composite(sprite,(round(x-sprite.width/2),round(y-sprite.height/2)))


def pose(form, kind, i):
    prefix = MELEE/form/"HexcorePreview"
    suffix = f"{i:02}.png"
    target = prefix/"Basic"/("Basic_East_"+suffix) if kind == "Basic" else prefix/"Continuous"/(kind+"_East_"+suffix)
    return Image.open(target).convert("RGBA")



CASES = (
    ("01-Basic-Kiem-si","Swordsman","Basic",(("basic_silver", .225),("basic_silver",.525))),
    ("02-Basic-Sat-thu","Assassin","Basic",(("basic_dagger",.150),("basic_dagger",.450),("basic_thrust",.750))),
    ("03-Basic-Dao-si","DaoWarrior","Basic",(("basic_heavy",.225),("basic_drop",.675))),
    ("04-Basic-Kiem-khi","QiSwordsman","Basic",(("basic_qi",.225),("crescent",.675))),
    ("05-P2-Hoanh-Nguyet","Swordsman","Basic",()),
    ("06-E2-Huyet-An","Assassin","E2",()),
    ("07-E1-Chan-Dia","DaoWarrior","E1",()),
    ("08-E1-Nguyet-Nha","QiSwordsman","E1",()),
)
SAMPLE_RATE = 24
WIDTH, HEIGHT = 500, 272
FRAME_COUNT = 42


def source_gif(path):
    frames = []
    ends = []
    with Image.open(path) as gif:
        time = 0
        for i in range(gif.n_frames):
            gif.seek(i)
            frames.append(gif.convert("RGB"))
            time += gif.info.get("duration", 42)
            ends.append(time)
    return frames, ends


def save_comparison(before, after, path, label="MAU OFFLINE DA BAN GIAO"):
    before_frames, ends = before
    scenes = []
    for i, composed in enumerate(after):
        ms = i/SAMPLE_RATE*1000
        before_i = next((j for j,end in enumerate(ends) if ms < end),len(ends)-1)
        frame = Image.new("RGB",(2*WIDTH+12,HEIGHT+32),(11,17,25))
        frame.paste(before_frames[before_i].resize((WIDTH,HEIGHT),Image.Resampling.NEAREST),(0,32))
        frame.paste(composed,(WIDTH+12,32))
        draw = ImageDraw.Draw(frame)
        draw.text((10,8),"TRUOC  |  "+label,fill="#91b0c6",font=ImageFont.load_default(size=14))
        draw.text((WIDTH+22,8),"SAU  |  PHOI PIXELFX + POSE GOC",fill="#9aebd4",font=ImageFont.load_default(size=14))
        scenes.append(frame)
    palette_image = Image.new("RGB",(360,117*len(scenes[::4])))
    for i, frame in enumerate(scenes[::4]):
        palette_image.paste(frame.resize((360,117)),(0,117*i))
    palette = palette_image.quantize(colors=256)
    indexed = [frame.quantize(palette=palette,dither=Image.Dither.NONE) for frame in scenes]
    for speed, multiplier in [("1x",1),("05x",2)]:
        # GIF has 10 ms granularity. Round cumulative times to avoid playback drift.
        time = [round(i*100*multiplier/SAMPLE_RATE)*10 for i in range(len(indexed)+1)]
        delays = [b-a for a,b in zip(time,time[1:])]
        indexed[0].save(path.with_name(path.name+"-"+speed+".gif"),save_all=True,append_images=indexed[1:],
                        duration=delays,loop=0,optimize=True,disposal=1)


def review(clips):
    output=REVIEW/"Compare"
    output.mkdir(parents=True, exist_ok=True)
    plate = Image.new("RGB",(WIDTH*2,(HEIGHT+29)*4),(13,20,28))
    title = ImageFont.load_default(size=14)
    highlights = [.275,.81,.75,.78,.79,1.17,.82,.62]
    for case,(key,form,kind,moves) in enumerate(CASES):
        body = [pose(form,kind,i) for i in range(12)]
        sequence = []
        for f in range(FRAME_COUNT):
            t = f/SAMPLE_RATE
            canvas = Image.new("RGBA",(WIDTH,HEIGHT),(21,31,43,255))
            d = ImageDraw.Draw(canvas)
            for y in range(121,HEIGHT,16):
                for x in range(0,WIDTH,16):
                    d.rectangle((x,y,x+15,y+15),fill=("#29383f" if ((x+y)//16)%2 else "#2d3b41"))
            x = 164 if case!=4 else 220
            floor = 166
            targets = [(265,153),(316,169),(357,154),(393,179),(433,150)] if case==5 else [(239,166),(299,153),(386,172)]
            if case==4: targets=[(151,158),(270,171),(319,153),(373,178)]
            if case==7: targets=[(228,165),(307,154),(374,187),(444,223)]
            for j,(dx,dy) in enumerate(targets):
                d.ellipse((dx-8,dy-5,dx+8,dy+5), fill="#17242c", outline="#697881")
                d.rectangle((dx-5,dy-25,dx+5,dy-5), fill="#695954", outline="#8e7e6f")
                d.rectangle((dx-7,dy-20,dx+7,dy-15), fill="#9f7b66")
            frame_pos=t/(.075 if kind=="Basic" else .117)
            character=body[min(11,int(frame_pos))]
            # Use one fixed composition anchor for the pose series. Terrain projects into the same floor plane.
            composite(canvas,character,x,119,round(character.width*.49))
            if moves:
                for n,(effect,release) in enumerate(moves):
                    duration=.37 if effect=="crescent" else .29 if form=="DaoWarrior" else .245
                    age=(t-release)/duration
                    if not 0<=age<1: continue
                    center=x+(43+121*age if effect=="crescent" else -4)
                    width=112 if effect=="crescent" else 153 if form=="DaoWarrior" else 148
                    composite(canvas,component(clips,effect,age),center,
                              floor-33 if effect=="crescent" else floor-16,width,flip=n%2==1 and effect not in ("crescent","basic_drop"))
                    for dx,dy in targets:
                        contact = age-.12 if effect!="crescent" and dx-x<=90 else (center-dx)/95 if effect=="crescent" else -1
                        if 0<=contact<.57 and abs(dy-floor)<=23:
                            composite(canvas,component(clips,"hit_spark",contact/.57),dx,dy-10,84)
            elif case==4 and .38<=t<.96:
                age=(t-.38)/.58
                size=round(28+235*age)
                composite(canvas,component(clips,"orbit",age),x,floor-18,size)
                for dx,dy in targets:
                    elapsed=(size*.40-math.hypot(dx-x,dy-(floor-18)))/50
                    if 0<=elapsed<1:
                        composite(canvas,component(clips,"hit_spark",elapsed),dx,dy-8,69)
            elif case==5:
                for dx,dy in targets:
                    if .45<=t<1.08:
                        age=(t-.45)/.63
                        composite(canvas,component(clips,"blood_seal",age),dx,dy-14,round(47+17*age))
                    if 1.08<=t<1.52:
                        composite(canvas,component(clips,"blood_burst",(t-1.08)/.44),dx,dy,105)
            elif case==6 and .69<=t<1.18:
                age=(t-.69)/.49
                composite(canvas,component(clips,"ground_slam",age),265,floor,209)
                for dx,dy in targets:
                    if age<.62 and math.hypot(dx-265,dy-floor)<=87:
                        composite(canvas,component(clips,"hit_spark",age/.62),dx,dy-8,79)
            elif case==7 and .4<=t<.89:
                age=(t-.4)/.49
                composite(canvas,component(clips,"crescent",age),x+28+260*age,floor-13,160)
                half_width=160*.84/2
                for dx,dy in targets:
                    side=(dy-(floor-13))/half_width
                    elapsed=(x+28+260*age-dx-.57*half_width*side*side)/78
                    if abs(side)<=1 and 0<=elapsed<1:
                        composite(canvas,component(clips,"hit_spark",elapsed),dx,dy-9,77)
            sequence.append(canvas.convert("RGB"))
        save_comparison(source_gif(REVIEW/(key+".gif")),sequence,output/key)
        px,py=(case%2)*WIDTH,(case//2)*(HEIGHT+29)
        plate.paste(sequence[min(FRAME_COUNT-1,round(highlights[case]*SAMPLE_RATE))],(px,py+29))
        ImageDraw.Draw(plate).text((px+11,py+7),key,fill="#e2e8f0",font=title)
    plate.save(output/"ContactSheet.png",optimize=True)


REV2_CASES = (
    (1,"Swordsman","Basic","Basic Kiếm sĩ",("basic_silver",)),
    (1,"Assassin","Basic","Basic Sát thủ",("basic_dagger","basic_thrust")),
    (1,"DaoWarrior","Basic","Basic Đao sĩ",("basic_heavy",)),
    (1,"QiSwordsman","Basic","Basic Kiếm khí",("crescent",)),
    (1,"Swordsman","Basic","P1 Phong Ảnh Trảm",("passive_wave",)),
    (2,"Swordsman","Basic","P2 Hoành Nguyệt Trảm",("orbit",)),
    (3,"Swordsman","Basic","P3 Phản Kiếm Quyết",("hit_spark","counter_wave")),
    (4,"Assassin","E1","E1 Ám Bộ Trảm",("basic_dagger","ambush_finish")),
    (5,"Assassin","E2","E2 Huyết Ấn",("blood_seal","blood_burst")),
    (16,"Assassin","F1","E3 Dạ Vũ Liên Trảm",("basic_dagger","basic_dagger","night_finish")),
    (6,"Assassin","E3","F1 Tàng Ảnh",("shadow_gate","basic_dagger")),
    (17,"Assassin","F2","F2 Đoạt Mệnh",("shadow_gate","execute_thrust")),
    (18,"Assassin","F3","F3 Song Ảnh Truy Sát",("shadow_gate","night_finish","dagger_mark")),
    (7,"DaoWarrior","E1","E1 Chấn Địa Trảm",("ground_slam",)),
    (8,"DaoWarrior","E2","E2 Hoành Thiên Đao",("dao_fan",)),
    (19,"DaoWarrior","F1","E3 Cuồng Phong Đao",("dao_orbit","dao_orbit")),
    (9,"DaoWarrior","E3","F1 Bá Vương Đao Vực",("ground_slam","gravity","dao_orbit")),
    (20,"DaoWarrior","F2","F2 Đoạn Sơn Trảm",("rift",)),
    (21,"DaoWarrior","F3","F3 Bất Diệt Chiến Ý",("war_flame","dao_wave")),
    (10,"QiSwordsman","E1","E1 Nguyệt Nha Trảm",("crescent",)),
    (11,"QiSwordsman","E2","E2 Lưỡng Nghi Kiếm",("cross_blade","cross_blade","qi_burst")),
    (22,"QiSwordsman","F1","E3 Kiếm Vũ Thiên Hà",("qi_seal","qi_sword","qi_burst")),
    (12,"QiSwordsman","E3","F1 Thiên Kiếm Xuyên Tâm",("qi_seal","qi_sword","qi_burst")),
    (23,"QiSwordsman","F2","F2 Phi Kiếm Truy Hồn",("qi_sword","hit_spark")),
    (24,"QiSwordsman","F3","F3 Hồi Nguyệt Kiếm",("crescent","crescent")),
)


def review_rev2(clips):
    output = REVIEW/"Rev2"
    output.mkdir(parents=True,exist_ok=True)
    forms = ["Swordsman","Assassin","DaoWarrior","QiSwordsman"]
    font_path = Path("C:/Windows/Fonts/arial.ttf")
    if font_path.is_file():
        title_font = ImageFont.truetype(str(font_path), 12)
    else:
        title_font = ImageFont.load_default(size=12)
    plate = Image.new("RGB",(4*250,math.ceil(len(REV2_CASES)/4)*149),(12,19,27))
    cards = []
    for index,(source,form,kind,title,moves) in enumerate(REV2_CASES):
        label = f"{index+1:02d}"
        if index < 4:
            previous = REVIEW/(CASES[index][0]+".gif")
        else:
            previous = REVIEW.parent/"Media"/f"Skill-{source:02d}-{forms.index(form)+1}-E.gif"
        body = [pose(form,kind,i) for i in range(12)]
        targets = [(273,167),(322,196),(366,169),(397,213),(298,228)]
        frames = []
        for i in range(48):
            t = i/47
            canvas = Image.new("RGBA",(WIDTH,HEIGHT),(22,33,46,255))
            d = ImageDraw.Draw(canvas)
            for y in range(121,HEIGHT,16):
                for x in range(0,WIDTH,16):
                    d.rectangle((x,y,x+15,y+15),fill=("#29383f" if ((x+y)//16)%2 else "#2d3b41"))
            x = 133
            if source in (4,6,16,17) and index > 4:
                x += round(160*max(0,min(1,(t-.21)/(.50 if source == 16 else .27))))
            for j,(dx,dy) in enumerate(targets):
                if title.startswith("F1 Bá") and j:
                    pull = max(0,min(1,(t-.24)/.4))
                    dx,dy = (round(dx+(183-dx)*pull*.75),round(dy+(165-dy)*pull*.75))
                d.ellipse((dx-12,dy-3,dx+12,dy+4),fill="#14202a")
                d.rounded_rectangle((dx-7,dy-25,dx+7,dy-7),radius=4,fill="#756157",outline="#b39a85")
                d.rectangle((dx-5,dy-21,dx+5,dy-17),fill="#a28a72")
            sprite = body[min(11,int(t*12))]
            composite(canvas,sprite,x,137,174)
            if title.startswith("F3 Song") and .14<t<.84:
                for lane,yy in enumerate((126,208)):
                    copy_x = 140+round(220*max(0,min(1,(t-.31)/.43)))
                    composite(canvas,fade(tint_fx(sprite,DAGGER,.9),min(1,4*(.84-t))),copy_x,yy,156)
                    if .34<t<.75:
                        composite(canvas,component(clips,"night_finish",(t-.34)/.41),copy_x+33,yy+23,136,flip=lane==0)
            for j,move in enumerate(moves):
                start = .23 + .20*j if len(moves)>1 else .38
                length = .45 if move in ("orbit","dao_orbit","gravity","war_flame","shadow_gate","qi_seal") else .29
                age = (t-start)/length
                if not 0<=age<1:
                    continue
                art = component(clips,move,age)
                pos_x,pos_y,size,angle = x+29,157,150,0
                if move in ("orbit","dao_orbit","ground_slam","gravity","war_flame","dao_fan"):
                    pos_x,pos_y,size = (x if move!="gravity" else 168),179,205 if move!="war_flame" else 99
                if move in ("crescent","counter_wave","passive_wave","dao_wave"):
                    pos_x += round(age*219)
                    if source==24 and j==1:
                        pos_x = 405-round(245*age)
                        angle = 180
                if move in ("blood_seal","blood_burst","dagger_mark"):
                    for dx,dy in targets:
                        composite(canvas,art,dx,dy if move=="blood_burst" else dy-15,104 if move=="blood_burst" else 41)
                    continue
                if move == "qi_sword":
                    if source==22:
                        for k,(p,q) in enumerate(targets):
                            for wave in range(3):
                                fall = age*3-wave-k*.035
                                if 0<=fall<=1:
                                    composite(canvas,component(clips,move,fall*.75).rotate(-90),p+wave*15,q-21-105*(1-fall),72)
                        continue
                    pos_x,pos_y,size,angle = (322,166-round((1-age)*155),168,-90) if source==12 else (
                        270+round(age*127),168+round(28*math.sin(age*math.pi)),96,-21+round(age*37))
                if move in ("qi_seal","qi_burst"):
                    pos_x,pos_y,size = 323,184,173 if move=="qi_seal" else 104
                if move == "cross_blade":
                    pos_x,pos_y,angle,size = 318,177,36 if j else -36,184
                    if j: composite(canvas,component(clips,move,.69).rotate(-36),318,177,184)
                if angle: art = art.rotate(angle)
                composite(canvas,art,pos_x,pos_y,size,flip=(j==1 and kind=="Basic" and form!="QiSwordsman"))
            frames.append(canvas.convert("RGB"))
        if previous.is_file():
            baseline = source_gif(previous)
            before_label = "OFFLINE V2.1" if index<4 else "CAPTURE LICH SU CUNG ID NGUON"
        else:
            baseline = ([Image.new("RGB",(WIDTH,HEIGHT),"#111924")],[99999])
            before_label = "CHUA CO BASELINE"
        save_comparison(baseline,frames,output/label,before_label)
        p,q = index%4*250,index//4*149
        chosen = 16 if source==23 else 27 if source==22 else 30 if len(moves)>1 else 24
        plate.paste(frames[chosen].resize((250,136)),(p,q+13))
        ImageDraw.Draw(plate).text((p+6,q),title,font=title_font,fill="white")
        cards.append(f'<figure><figcaption>{html.escape(title)} · {html.escape(form)}</figcaption>'
                     f'<a href="{label}-1x.gif"><img src="{label}-1x.gif" loading="lazy" alt="{html.escape(title)}"></a></figure>')
    plate.save(output/"ContactSheet.png",optimize=True)
    (output/"index.html").write_text(
        '<!doctype html><html lang="vi"><meta charset="utf-8"><title>HEXCORE Rev2</title>'
        '<style>body{font:16px system-ui;background:#121923;color:#eff5ff;margin:28px} a{color:#90e9dc}'
        'header{margin-bottom:22px}main{display:grid;grid-template-columns:repeat(auto-fit,minmax(450px,1fr));gap:20px}'
        'figure{margin:0;background:#202b39;padding:10px} img{width:100%;display:block;margin-top:8px}'
        'button{margin-right:8px;padding:6px 12px}</style>'
        '<header><h1>HEXCORE Cận chiến · v2.1 Rev2</h1>'
        '<p>Trái là bản lịch sử cùng ID nguồn. Phải là dựng offline hướng Đông, bố cục minh họa.</p>'
        '<button data-speed="1x">1×</button><button data-speed="05x">0,5×</button> '
        '<a href="ContactSheet.png">Contact sheet</a> · <a href="../Compare/index.html">8 mẫu vòng trước</a> · '
        '<a href="../../index.html">Gallery Unity lịch sử, gồm ba lõi R</a></header><main>'
        + "".join(cards) +
        '</main><script>for(const button of document.querySelectorAll("button")) button.onclick=()=>{'
        'for(const img of document.images){img.src=img.getAttribute("src").replace(/-(1x|05x)\.gif/,"-"+button.dataset.speed+".gif");'
        'img.parentElement.href=img.src;}}</script></html>',encoding="utf8")


def review_feedback(clips):
    output = REVIEW/"Rev2/Feedback"
    output.mkdir(parents=True, exist_ok=True)
    cases = [
        ("Basic-Kiem-khi", "Basic Kiếm khí", 4, "QiSwordsman", "Basic", (8, 18)),
        ("Nguyet-Nha", "Nguyệt Nha Trảm", 20, "QiSwordsman", "E1", (14, 19)),
        ("Phi-Kiem", "Phi Kiếm Truy Hồn", 24, "QiSwordsman", "F2", (14, 29)),
        ("Am-Bo", "Ám Bộ Trảm", 8, "Assassin", "E1", (18, 24)),
        ("Song-Anh", "Song Ảnh Truy Sát", 13, "Assassin", "F3", (19, 24)),
        ("Hoanh-Thien", "Hoành Thiên Đao", 15, "DaoWarrior", "E2", (21, 26)),
    ]
    font_path = Path("C:/Windows/Fonts/arial.ttf")
    title_font = ImageFont.truetype(str(font_path), 15) if font_path.is_file() else ImageFont.load_default(size=15)
    plate = Image.new("RGB",(WIDTH*2,(HEIGHT+27)*len(cases)), "#121923")

    def show(canvas, clip, age, x, y, size, angle=0, mirror=False, tip_x=56):
        if not 0<=age<1 or size<2:
            return
        art = component(clips,clip,age)
        if tip_x!=56:
            centered=Image.new("RGBA",(CELL,CELL))
            centered.alpha_composite(art,(56-tip_x,0))
            art=centered
        if mirror:
            art=art.transpose(Image.Transpose.FLIP_TOP_BOTTOM)
        if angle:
            art=art.rotate(angle)
        composite(canvas,art,x,y,round(size))

    for row,(name,title,baseline_id,form,kind,keyframes) in enumerate(cases):
        body = [pose(form,kind,i) for i in range(12)]
        old, ends = source_gif(REVIEW/"Rev2"/f"{baseline_id:02}-1x.gif")
        old = [im.crop((WIDTH+12,32,2*WIDTH+12,HEIGHT+32)) for im in old]
        frames = []
        for frame in range(54):
            t = frame/SAMPLE_RATE
            canvas = Image.new("RGBA",(WIDTH,HEIGHT),"#182434")
            d = ImageDraw.Draw(canvas)
            for y in range(91,HEIGHT,16):
                for x in range(0,WIDTH,16):
                    d.rectangle((x,y,x+15,y+15),fill="#29383f" if ((x+y)//16)%2 else "#2d3b41")

            x,y = 125,133
            fx,fy = 146,162
            if name == "Phi-Kiem":
                targets=[(211,155),(244,201),(284,168),(326,212),(370,160),(411,206)]
            elif name == "Am-Bo":
                targets=[(235,154),(256,183),(359,169)]
                x = 108 + round(199*max(0,min(1,(t-.17)/.24)))
            elif name == "Song-Anh":
                targets=[(265,112),(299,146),(327,170),(359,223),(393,256)]
            elif name == "Hoanh-Thien":
                targets=[(215,161),(324,157),(310,231),(190,238),(450,169)]
            elif name == "Nguyet-Nha":
                targets=[(218,160),(285,193),(350,146)]
            else:
                targets=[(196,174),(238,161),(300,183)]
            for i,(dx,dy) in enumerate(targets):
                push_start = ((dx-fx)/620 + .40 if name=="Nguyet-Nha" else
                              (math.hypot(dx-fx,dy-fy)/520 + .75 if name=="Hoanh-Thien" and i<3 else 9))
                if name!="Nguyet-Nha" or i:
                    dx += round(27*max(0,min(1,(t-push_start)/.19)))
                d.ellipse((dx-10,dy-4,dx+10,dy+4),fill="#15212b")
                d.rounded_rectangle((dx-7,dy-23,dx+7,dy-6),radius=4,
                    fill="#365b79" if name=="Nguyet-Nha" and i==0 else "#756157",
                    outline="#87cafa" if name=="Nguyet-Nha" and i==0 else "#b39a85")
                if name in ("Nguyet-Nha","Hoanh-Thien") and (0<=t-push_start<.17):
                    show(canvas,"hit_spark",(t-push_start)/.17,dx,dy-9,52)

            actor_pose = body[min(11,int((t/.90 if kind=="Basic" else t/1.48)*12))]
            if name=="Am-Bo" and t>=.41:
                turn = MELEE/form/"HexcorePreview/Turn"
                turns = sorted(turn.glob("*.png"))
                if t<.56 and len(turns)==16:
                    actor_pose=Image.open(turns[min(12,4+int((t-.41)/.15*8))]).convert("RGBA")
                else:
                    west = MELEE/form/"HexcorePreview/Basic"
                    frame_index = min(11,max(0,int((t-.56)*13)))
                    if t>=1.07: frame_index= min(11,7+int((t-1.07)*12))
                    actor_pose = Image.open(west/f"Basic_West_{frame_index:02}.png").convert("RGBA")
            composite(canvas,actor_pose,x,y,174)

            if name=="Basic-Kiem-khi":
                for j,release in enumerate((.23,.68)):
                    age=(t-release)/.21
                    show(canvas,"crescent",age,fx+105*age,fy-5,63,mirror=bool(j),tip_x=70)
                    if .28<age<.94: show(canvas,"hit_spark",(age-.28)/.66,196,160,49)
            elif name=="Nguyet-Nha":
                age=(t-.40)/.43
                show(canvas,"crescent",age,fx+265*age,fy-3,141,tip_x=70)
            elif name=="Phi-Kiem":
                path=[(fx,fy)]+targets[:5]
                for i in range(5):
                    age=(t-.39-i*.22)/.22
                    ax,ay=path[i]
                    bx,by=path[i+1]
                    if 0<=age<1:
                        bend=18*math.sin(math.pi*age)*(1 if i%2==0 else -1)
                        angle=-math.degrees(math.atan2(by-ay+(1 if i%2==0 else -1)*18*math.pi*math.cos(math.pi*age),bx-ax))
                        show(canvas,"qi_sword",.43,ax+(bx-ax)*age,ay+(by-ay)*age+bend,73,angle)
                    show(canvas,"hit_spark",(age-1)*1.14,bx,by-7,62)
            elif name=="Am-Bo":
                show(canvas,"shadow_gate",(t-.17)/.24,x-18,fy,99)
                for j,release in enumerate((.69,.92)):
                    show(canvas,"ambush_cut",(t-release)/.14,x-19,fy-4,147,180,mirror=bool(j))
                    show(canvas,"hit_spark",(t-release)/.18,269,166,66,180)
            elif name=="Song-Anh":
                age=(t-.69)/.27
                for j,lane_y in enumerate((105,216)):
                    show(canvas,"shadow_gate",(t-.21)/.40,x,lane_y,98)
                    if 0<=age<1:
                        blade_x = x+round(238*age)
                        painted=tint_fx(actor_pose,DAGGER,.88)
                        composite(canvas,fade(painted,.20),blade_x-24,lane_y-24,125)
                        composite(canvas,fade(painted,.76),blade_x,lane_y-24,148)
                        show(canvas,"night_finish",age,blade_x+16,lane_y,118,mirror=bool(j))
                show(canvas,"shadow_dagger",age,fx+238*age,164,71,tip_x=90)
                for i in (0,1,2,3):
                    dx,dy=targets[i]
                    hit=.69+(dx-fx)/880
                    show(canvas,"dagger_mark",(t-hit)/.50,dx,dy-24,45,-90)
                    show(canvas,"hit_spark",(t-hit)/.18,dx,dy-8,62)
            else:
                age=(t-.75)/.47
                show(canvas,"dao_fan",age*.82,fx,fy,440*age)
            frames.append(canvas.convert("RGB"))
        save_comparison((old,ends),frames,output/name,"OFFLINE REV2 BAN TRUOC")
        for column,keyframe in enumerate(keyframes):
            plate.paste(frames[keyframe],(column*WIDTH,row*(HEIGHT+27)+27))
            ImageDraw.Draw(plate).text((column*WIDTH+12,row*(HEIGHT+27)+6),title,fill="#ecf3fc",font=title_font)
    plate.save(output/"ContactSheet.png",optimize=True)


if __name__ == "__main__":
    clips = create_clips()
    export_atlas(clips)
    if "--feedback" in sys.argv:
        review_feedback(clips)
    else:
        review_rev2(clips)
    print(f"{len(clips)} layered clips")
    for file in ("sheet.png","sheet.json"):
        print(file, hashlib.sha256((SOURCE/"HitSpark"/file).read_bytes()).hexdigest())


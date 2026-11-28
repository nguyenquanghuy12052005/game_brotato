from array import array
from functools import lru_cache
from hashlib import sha256
import json
import math
from pathlib import Path
import random
import re
import sys
import wave

ROOT = Path(__file__).resolve().parent
PROJECT = ROOT.parents[1]
OUT = ROOT / "audio"
RATE = 32000
TAU = 2 * math.pi
STAGES = {"p": "Chuẩn bị", "c": "Thi triển", "m": "Di chuyển / duy trì",
          "h": "Va chạm / có hiệu lực", "t": "Dư âm / kết thúc"}
MATERIALS = {}
for line in ((ROOT / "materials.txt").read_text()+"\n"+(ROOT/"review-materials.txt").read_text()).splitlines():
    if not line or line.startswith("#"):
        continue
    name, *values, palette = line.split()
    MATERIALS[name] = (*map(float, values), palette)
REVISED = {}
for line in (ROOT/"review-recipes.txt").read_text().splitlines():
    if line and not line.startswith("#"):
        n,flavor,family,*events=line.split()
        REVISED[int(n)] = (flavor,family,events)
SUSTAIN = {"blizzard_flow","plasma_hum","cage_hum","time_bed","orb_current"}
AUDITION_LOOPS = {45:("blizzard_flow",.35,1.85), 47:("plasma_hum",1.12,2.2),
                  48:("cage_hum",.23,1.15), 105:("time_bed",.34,1.65), 107:("orb_current",.74,2.25)}
MODAL = {"steel_wire","steel_lock","crystal_tail","star_mark","star_impact","crystal_touch","star_tail",
         "glass_debt","ice_grain","ice_pin","clock_break","orb_ignite","ice_contact","column_tone"}
BODY = {"stone_impact","fault_release","target_thud","ground_ring","shadow_verdict","turret_seat",
        "orb_release","fire_mouth","fire_bloom","curse_clasp"}

TONAL = set("harp glass shimmer brass bell chord marimba horn ricochet light qi ice parry rip1 rip2 rip3 charge".split())
PUNCH = set("body pulse pistol shotgun smg fire smallfire rocket explode slam crater shock dirt drum implode".split())
SWEEP = set("reverse air jet suction tear slice blade dash ghost meteor dive heavy orbit rip1 rip2 rip3".split())
TEXTURE = set("flame cyclone snow field saw gravity still".split())
FOLEY = {}
for path in sorted((ROOT / "foley").glob("*.wav")):
    with wave.open(str(path)) as reader:
        if (reader.getframerate(), reader.getnchannels(), reader.getsampwidth()) != (RATE, 1, 2):
            raise ValueError(path)
        sample = array("h", reader.readframes(reader.getnframes()))
    peak = max(map(abs, sample))
    leading = next((i for i, v in enumerate(sample) if abs(v) > peak * .027), 0)
    FOLEY[path.stem] = tuple(x / max(peak, 1) for x in sample[max(0, leading-24):])
if not FOLEY:
    raise RuntimeError("Missing decoded CC0 Foley. See README.")

PROJECT_LAYERS = {
    (1, "step"): "Foot", (2, "slice"): "Dash", (49, "blade"): "Swing",
    (52, "qi"): "Wave", (55, "parry"): "Parry", (62, "slam"): "Slam",
    (74, "crater"): "Slam", (75, "rip1"): "Wave", (127, "marimba"): "Confirm",
}

@lru_cache(maxsize=10)
def project_audio(name):
    folder = ("Assets/Resources/BrotatoMenu/Home" if name == "Confirm"
              else "Assets/Prefabs/Player/Melee/Shared/Audio")
    with wave.open(str(PROJECT/folder/(name+".wav"))) as reader:
        return reader.getframerate(), array("h", reader.readframes(reader.getnframes()))

def apply_project_layer(dst, name):
    rate, pcm = project_audio(name)
    ratio = rate/RATE
    limit = min(len(dst),math.floor((len(pcm)-1)/ratio))
    peak = max(map(abs,pcm))
    for i in range(limit):
        at = i*ratio
        a = int(at)
        fraction = at-a
        value = pcm[a]*(1-fraction) + pcm[a+1]*fraction
        dst[i] += value / peak * .144 * min(1,i/95,(limit-i)/500)

def stable(value):
    return int.from_bytes(sha256(value.encode()).digest()[:8], "big")

def zeros(size):
    return array("f", [0.0]) * size

def write(path, left, right, gain):
    pcm = array("h")
    if right is None:
        pcm.extend(round(32767 * math.tanh(value * gain)) for value in left)
    else:
        for a, b in zip(left, right):
            pcm.append(round(32767 * math.tanh(a * gain)))
            pcm.append(round(32767 * math.tanh(b * gain)))
    bank = ROOT/"review-audio" if int(path.split("-")[0]) in REVISED else OUT
    bank.mkdir(exist_ok=True)
    filename = bank / path
    with wave.open(str(filename), "wb") as writer:
        writer.setparams((1 if right is None else 2, 2, RATE, 0, "NONE", "not compressed"))
        writer.writeframes(pcm.tobytes())
    return bank.name + "/" + path

def add(dst, data, start, amount=1, wrap=False):
    offset = round(start * RATE)
    if wrap:
        for i, value in enumerate(data):
            dst[(offset + i) % len(dst)] += value * amount
    else:
        for i, value in enumerate(data[:max(0, len(dst)-offset)], offset):
            dst[i] += value * amount

def comb(left, right, data, start, pan, weight=1, wrap=False):
    add(left, data, start, weight * math.sqrt((1-pan)*.5), wrap)
    add(right, data, start, weight * math.sqrt((1+pan)*.5), wrap)

def gain_for(left, right, target=.78):
    peak = max(max(map(abs, left)), max(map(abs, right)) if right is not None else 0)
    return math.atanh(target) / max(peak, .01)

def waveform(left, right):
    step = max(1, len(left) // 104)
    view = []
    for i in range(0, len(left), step):
        a = max(map(abs, left[i:i+step]))
        b = max(map(abs, right[i:i+step])) if right is not None else 0
        view.append(max(a,b))
    peak = max(view)
    return [round(v/max(.001,peak),3) for v in view]

def foley_layer(dst, palette, rng, strong=.45):
    keys = [key for key in FOLEY if key.startswith(palette)]
    if not keys:
        return None
    key = rng.choice(keys)
    data = FOLEY[key]
    ratio = rng.uniform(.78, 1.23)
    limit = min(len(dst), int(len(data)/ratio))
    low = 0
    for i in range(limit):
        at = i*ratio
        lo = int(at)
        mix = at-lo
        value = data[lo] + (data[min(lo+1,len(data)-1)]-data[lo])*mix
        low += .31*(value-low)
        dst[i] += (value*.45 + low*.55)*strong*min(1,i/65,(limit-i)/350)
    return key

class Style:
    def __init__(self, cue_id, flavor, family, variant):
        self.number = cue_id
        self.variant = variant
        self.family = family
        self.seed = stable(f"{cue_id}:{flavor}:{variant}")
        self.rng = random.Random(self.seed)
        self.base = self.rng.uniform(.83, 1.19)
        self.grain_spacing = self.rng.uniform(.011, .030)
        self.grain = self.rng.uniform(.50, 1.15)
        self.pan = self.rng.choice((-1,1))
        self.hue = self.rng.uniform(.70,1.40)

def texture(name, style, rng, sample, elapsed, phase, white, low, slow, env, noise):
    grain = math.sin(TAU * style.grain_spacing * sample/RATE * 1000 + .7*phase)
    sweep = (0.5+0.5*math.sin(TAU*(elapsed*2.5) + style.hue)) ** 2
    if name in PUNCH:
        sub = math.sin(phase) + .21*math.sin(phase*2.38) + .13*math.sin(phase*3.12)
        snap = (white-low)*math.exp(-elapsed*43)
        return env*(.52*sub*(1-noise*.5)+.52*noise*(.65*low+.23*white)+.35*snap)
    if name in TONAL:
        body = math.sin(phase) + .34*math.sin(phase*(2.01+style.hue*.024)) + .21*math.sin(phase*3.08)
        return env*(.43*body*(1-noise)+noise*(low+.18*white)*(.8+.2*grain))
    if name in SWEEP:
        band = low - slow
        return env*(noise*(1.55*band+.16*white)*(0.46+.54*sweep)+.33*(1-noise)*math.sin(phase+2.4*slow))
    if name in TEXTURE:
        return env*(noise*(.94*(low-slow)+.15*white)*(0.55+.45*sweep)+.24*(1-noise)*math.sin(phase+2.0*grain))
    trem = .78+.22*grain
    if name in {"bone","rocks","grit","crumble","ember","ice"}:
        trem *= .58+.42*sweep
    return env*(.42*(1-noise)*math.sin(phase+2*low)+noise*(.76*(low-slow)+.21*white))*trem

def event_audio(name, style, ordinal):
    length, a, b, noise, decay, width, palette = MATERIALS[name]
    rng = random.Random(style.seed + 913*ordinal)
    if name not in SUSTAIN:
        length *= rng.uniform(.93,1.07)
    factor = style.base*rng.uniform(.93,1.07)
    if style.number in {1, 108, 113} and name in {"step","scuff"}:
        palette = ["footstep0","footstep0","footstep0","footstep0"][style.variant % 4]
        noise = [.49,.67,.81,.60][style.variant % 4]
        a *= [1,.90,.81,1.12][style.variant % 4]
    if style.number in {10,130} and name in {"pebble","glass"}:
        noise = [.38,.15,.60,.27][style.variant % 4]
    if style.number == 116:
        noise *= [.91,1.19,.74][style.variant % 3]
    if name == "missile":
        factor *= 1 + ordinal%5*.048
    if style.number == 72 and name == "ricochet":
        factor *= .86 + min(ordinal,5)*.09
    duration = max(1,int(length*RATE))
    result = zeros(duration)
    low = slow = phase = 0.0
    attack = .005
    if name in SWEEP:
        attack = min(.14, length*(.36 if name in {"suction","reverse","dive"} else .14))
    if name in TEXTURE:
        attack = .13
    if name in {"crater","slam","explode"}:
        attack = .006
    if name in {"qi","rip1","rip2","rip3","chord","portal"}:
        attack = .018
    for i in range(duration):
        t = i/RATE
        u = i/duration
        white = rng.uniform(-1,1)
        low += (.15 + style.hue*.055)*(white-low)
        slow += .022*(low-slow)
        phase += TAU*factor*(a+(b-a)*math.sqrt(u))/RATE
        env = math.exp(-decay*u)*min(1, t/attack, (length-t)/.028)
        if name in SUSTAIN:
            movement=.65+.19*math.sin(TAU*.71*t)+.13*math.sin(TAU*2.57*t+style.hue)
            warm=math.sin(phase)+.28*math.sin(phase*2.018)+.12*math.sin(phase*3.15)
            attack= .18 if name in {"time_bed","orb_current"} else .10
            env=math.exp(-decay*u)*min(1,t/attack,(length-t)/.18)
            result[i]=env*movement*(.51*noise*(low-slow)+.21*(1-noise)*warm)
        elif name in MODAL:
            metal=math.sin(phase)+.43*math.exp(-t*12)*math.sin(phase*2.33)+.24*math.exp(-t*22)*math.sin(phase*3.79)
            result[i]=env*(.37*(1-noise)*metal+.52*noise*(low-slow)+.19*noise*white)
        elif name in BODY:
            pressure=.58*math.sin(phase)+.26*math.sin(phase*1.46)+.13*math.sin(phase*2.38)
            result[i]=env*(.67*pressure*(1-noise*.52)+.66*noise*(low+.23*white)+.23*(white-low)*math.exp(-47*t))
        else:
            result[i] = texture(name,style,rng,i,t,phase,white,low,slow,env,noise)
    if palette != "-":
        if style.number in {1,108,113} and name in {"step","scuff"}:
            candidates = [f"footstep0{i}" for i in [(0,1,4),(2,3,6),(7,8,9),(4,5,6)][style.variant % 4]]
            key = foley_layer(result, candidates[ordinal%3], rng, .62)
        else:
            key = foley_layer(result, palette, rng, .72 if palette.startswith("swish-")
                              else .56 if name in SWEEP | PUNCH | BODY else .46)
    else:
        key = None
    return result, width, key

def entry_variants(n, family):
    if n in {1,10,108,113,130}:
        return ["Forest", "Moon", "Inferno", "Tundra"]
    if n == 116:
        return ["Xương khô", "Golem dung nham", "Yeti"]
    if n == 44:
        return ["Dây đứt", "Hết thời hạn"]
    if n == 107:
        return ["Tụ đủ · 8 phi đạn", "Tụ thấp · 2 phi đạn"]
    if n in {20,21,22,49,50,51,52,109,110,126}:
        return [f"Biến thể {i}" for i in range(1, 7 if family == "gun" else 5)]
    if family in {"arcane","blast","blade","hex","enemy","movement","damage","ui"}:
        return ["Phối A", "Phối B"]
    return ["Phối A"]

def render_action(n, flavor, family, events):
    output = []
    source_keys = set()
    for variant,label in enumerate(entry_variants(n, family)):
        style = Style(n, flavor, family, variant)
        picked = [item for item in events if not (n == 107 and variant and item[-1] == "orb_bolt")]
        if n == 107 and variant:
            picked += [("m", 8, "orb_bolt")]*2
        if n == 44 and variant:
            picked = [(stage, 1.03 if name == "snap" else start, "explode" if name == "snap" else name)
                      for stage,start,name in picked if name != "zap"]
        size = round((max(start+MATERIALS[name][0]*1.09 for _,start,name in picked)+.12)*RATE)
        stereo = family in {"hex","blast"} or n in {10, 23, 70, 76, 106, 107, 115}
        stems = {}
        marks = {}
        for ordinal,(stage,start,name) in enumerate(picked):
            data,width,key = event_audio(name,style,ordinal)
            original = PROJECT_LAYERS.get((n,name))
            if original:
                apply_project_layer(data,original)
            if key:
                source_keys.add(key)
            if stage not in stems:
                stems[stage] = (zeros(size), zeros(size) if stereo else None)
            left,right = stems[stage]
            if stereo:
                pan = width * style.pan * (-1 if ordinal%2 else 1)
                if n == 74 and stage == "h":
                    pan = 0
                comb(left,right,data,start,pan)
                if name in {"crater","rip3","light","shock","portal"}:
                    comb(left,right,data,start+.065,-pan,.145)
            else:
                add(left,data,start)
            marks.setdefault(stage,[]).append(round(start,3))
        left,right = zeros(size), zeros(size) if stereo else None
        for a,b in stems.values():
            add(left,a,0)
            if stereo:
                add(right,b,0)
        base = f"{n:03d}-{variant+1}"
        gain = gain_for(left,right,.80 if family == "hex" else .72)
        layers = []
        if len(stems)>1:
            for stage in STAGES:
                if stage in stems:
                    a,b = stems[stage]
                    layers.append({"label": STAGES[stage], "file": write(f"{base}-{stage}.wav",a,b,gain),
                                   "marks": sorted(set(marks[stage]))})
        result = {"name": label, "mix": write(f"{base}.wav",left,right,gain),
                  "layers": layers, "seconds": round(size/RATE,3), "wave": waveform(left,right)}
        prior = f"audio/{base}.wav"
        if n in REVISED and (ROOT/prior).is_file():
            result["prior"] = prior
        if n in AUDITION_LOOPS:
            material, offset, duration = AUDITION_LOOPS[n]
            loop, _, key = event_audio(material, style, 90)
            if key:
                source_keys.add(key)
            first = round(offset*RATE)
            count = round(duration*RATE)
            overlap = round(.1*RATE)
            section = loop[first:first+count+overlap]
            body = section[overlap:]
            for step in range(overlap):
                mix = step/(overlap-1)
                pos = len(body)-overlap+step
                body[pos] = body[pos]*(1-mix)+section[step]*mix
            result["layers"].append({"label":"Loop rời","file":write(f"{base}-loop.wav",body,None,gain_for(body,None,.44)),
                                     "marks":[],"loop":True,"seconds":len(body)/RATE})
        if "h" in stems and len(stems)>1 and family in {"hex","blast","blade","gun","arcane","enemy"} and n not in {82, 88, 94, 100, 110, 116, 121}:
            miss_left,miss_right = zeros(size), zeros(size) if stereo else None
            for stage,(a,b) in stems.items():
                if stage != "h":
                    add(miss_left,a,0)
                    if stereo:
                        add(miss_right,b,0)
            result["miss"] = write(f"{base}-miss.wav",miss_left,miss_right,gain)
        output.append(result)
    return output, source_keys
# Instrument design and original scores. Every option has its own rhythm and harmony.
SCORES = {
    "menu": [
        ("01 · Hẻm nổ", 142, [45,48,43,50], [0,3,7,10], [0,3,6,8,11,14], "pluck", 16, False),
        ("02 · Đường băng", 126, [48,53,45,55], [0,4,7,9], [1,5,7,10,13], "brass", 16, True),
        ("03 · Lõi vượt tốc", 158, [43,46,50,48], [0,3,7,10], [0,2,5,8,10,12,15], "mallet", 16, False)],
    "combat": [
        ("Forest · Gỗ và trống tay", 138, [45,48,50,43], [0,3,7,10], [0,3,6,9,13], "mallet", 12, False),
        ("Moon · Thủy tinh và bass", 138, [43,41,46,48], [0,3,7,10], [0,4,6,10,11,14], "glass", 12, False),
        ("Inferno · Trống nặng", 138, [40,40,43,38], [0,3,7,10], [0,2,4,7,8,12], "brass", 12, False),
        ("Tundra · Gảy dây lạnh", 138, [46,48,43,45], [0,4,7,9], [1,4,7,9,13], "pluck", 12, True)],
    "boss": [
        ("Xương khô · Ostinato", 150, [38,38,41,36], [0,3,7,10], [0,3,5,8,10,13], "mallet", 12, False),
        ("Golem dung nham · Trống đá", 138, [36,36,39,34], [0,3,7,10], [0,2,6,8,11,12], "brass", 12, False),
        ("Yeti · Khối băng", 130, [40,40,45,43], [0,3,7,10], [0,5,7,10,14], "glass", 12, False)],
    "results": [
        ("Chiến thắng", 143, [48,53,55,60], [0,4,7,9], [0,3,8,12], "brass", 3, False),
        ("Thất bại", 103, [45,41,38,33], [0,3,7,10], [0,6,11], "mallet", 3, False)],
    "victory": [("Chiến thắng", 154, [48,53,55,60], [0,4,7,9], [0,3,7,12], "brass", 2, False)],
    "defeat": [("Thất bại", 125, [45,41,38,33], [0,3,7,10], [0,6,11], "mallet", 2, False)]
}

@lru_cache(maxsize=220)
def instrument(name, midi, length, accent=1):
    hz = 440 * 2 ** ((midi-69)/12)
    seed = stable(f"{name}{midi}{length}{accent}")
    rng = random.Random(seed)
    count = round(length*RATE)
    data = zeros(count)
    if name == "pluck":
        lines = max(3,round(RATE/hz))
        string = [rng.uniform(-1,1) for _ in range(lines)]
        for i in range(count):
            index = i%lines
            following = (index+1)%lines
            val = string[index]
            string[index] = (val+string[following])*.4984
            t = i/RATE
            data[i] = accent*(.8*val+.08*math.sin(TAU*hz*t))*min(1,i/110,(count-i)/950)*math.exp(-4*t)
        return data
    if name in {"kick","snare","hat","rim"}:
        low = 0
        for i in range(count):
            t = i/RATE
            noise = rng.uniform(-1,1)
            low += .12*(noise-low)
            if name == "kick":
                phase = TAU*(46*t+2.7*(1-math.exp(-38*t)))
                val = math.sin(phase)*math.exp(-10*t)+noise*.25*math.exp(-120*t)
            elif name == "snare":
                val = (.18*math.sin(TAU*183*t)+.70*(noise-low))*math.exp(-24*t)
            elif name == "rim":
                val = (.23*math.sin(TAU*427*t)+.62*noise)*math.exp(-53*t)
            else:
                val = .56*(noise-low)*math.exp(-47*t)
            data[i] = val*accent*min(1,i/52,(count-i)/480)
        return data
    for i in range(count):
        t = i/RATE
        p = TAU*hz*t
        env = min(1,i/260,(count-i)/1900)
        if name == "bass":
            val = .85*math.sin(p)+.20*math.sin(p*2.01)+.06*math.sin(p*3)
            env *= math.exp(-3*t)
        elif name == "brass":
            drift = .22*math.sin(TAU*5.1*t)
            val = (.63*math.sin(p+drift)+.28*math.sin(p*2+.5*drift)+.2*math.sin(p*3)+.09*math.sin(p*4))
            env *= (1-math.exp(-60*t))*math.exp(-4*t)
        elif name == "glass":
            val = .58*math.sin(p)+.25*math.sin(p*2.012)*math.exp(-7*t)+.16*math.sin(p*3.79)*math.exp(-12*t)
            env *= math.exp(-3.8*t)
        elif name == "mallet":
            val = .72*math.sin(p)+.22*math.sin(p*2.008)*math.exp(-16*t)+.24*math.sin(p*4.016)*math.exp(-26*t)
            env *= math.exp(-6*t)
        else:
            val = .61*math.sin(p)+.25*math.sin(p*1.004)+.10*math.sin(p*2.003)
            env *= .7+.3*math.sin(math.pi*i/count)
        data[i] = accent*val*env
    return data

def assemble_mix(n, idx, label, layers, loop, extra=None):
    size = len(layers[0][1])
    left, right = zeros(size), zeros(size)
    for _,a,b in layers:
        add(left,a,0)
        add(right,b,0)
    peak = gain_for(left,right,.70)
    base = f"{n:03d}-{idx+1}"
    result = {"name": label, "seconds": round(size/RATE,3), "loop": loop,
              "mix": write(base+".wav",left,right,peak), "wave": waveform(left,right),
              "layers": [{"label": title,"file": write(f"{base}-stem-{i+1}.wav", a,b,peak)}
                         for i,(title,a,b) in enumerate(layers)]}
    if extra:
        result.update(extra)
    return result

def score(n, family, profile, index):
    label, bpm, roots, chord, riff, lead, bars, swing = profile
    beat = 60/bpm
    step = beat/4
    size = round(4*beat*bars*RATE)
    loop = family not in {"results","victory","defeat"}
    drums = (zeros(size),zeros(size))
    rhythm = (zeros(size),zeros(size))
    melody = (zeros(size),zeros(size))
    source_keys = set()
    for bar in range(bars):
        start = 4*bar*beat
        root = roots[bar%len(roots)]
        def note(target, kind, midi, length, offset, level, pan=0):
            data = instrument(kind,midi,length)
            comb(*target,data,start+offset,pan,level,loop)
        if loop:
            kick = [0,6,10] if swing else ([0,3,8,10,14] if bpm>150 else [0,4,7,10])
            for pos in kick:
                note(drums,"kick",34,.22,pos*step,.58)
            for pos in (4,12):
                note(drums,"snare",48,.16,pos*step,.37,.11)
                foley = zeros(round(.075*RATE))
                key = foley_layer(foley,"doorClose_" if bpm>135 else "bookFlip",random.Random(n*1000+bar*7+pos),.5)
                if key:
                    source_keys.add(key)
                comb(*drums,foley,start+pos*step,-.16,.20,loop)
            for pos in range(0,16,2 if bpm<150 else 1):
                note(drums,"hat" if pos%4 else "rim",45,.09,pos*step,.10+(.055 if pos%4==2 else 0),-.25 if pos%4 else .28)
        else:
            note(drums,"kick",32,.20,0,.52)
            note(drums,"rim",43,.11,8*step,.24,.20)
        bass_positions = [0,5,8,13] if swing else [0,3,8,11,14]
        for k,pos in enumerate(bass_positions):
            note(rhythm,"bass",root+(7 if k==3 else 0),.20,pos*step,.29)
        for pos in (0,8) if bar%4 in (1,3) else (2,6,12):
            for ni, interval in enumerate(chord):
                note(rhythm,"pad" if lead=="glass" else "pluck",root+12+interval,.31+ni*.07,pos*step,.13-ni*.016,(-1)**ni*.36)
        notes = [0,7,9,4,2,7,12] if lead=="brass" else ([0,7,3,10,7,3,0] if lead=="mallet" else [0,2,7,5,12,7,2])
        for j,pos in enumerate(riff):
            if bar%4==2 and j%3==1:
                continue
            delay = (0.21*step if swing and pos%2 else 0)
            pitch = root+12+notes[(j+bar%4)%len(notes)]
            note(melody,lead,pitch,.19 if bpm>=145 else .29,pos*step+delay,.24 if j in (0,3) else .17,(-1)**j*.25)
            if j in (0,len(riff)-1) and bar%2==1:
                note(melody,"glass",pitch+12,.31,pos*step+delay,.055,.47)
    if not loop:
        edge = min(RATE//2, size)
        for track in (*drums,*rhythm,*melody):
            for i in range(edge):
                track[-i-1] *= i/edge
    return assemble_mix(n,index,label,[("Trống & Foley",*drums),("Bass & hòa âm",*rhythm),("Giai điệu",*melody)],loop,{"bpm":bpm}), source_keys

def ambience(n, biome, index):
    settings = {"forest":(.52,.28,["cloth","step","paper"]),
                "moon":(.22,.43,["glass","pebble","air"]),
                "inferno":(.15,.51,["ember","rocks","dirt"]),
                "tundra":(.68,.21,["snow","frost","thaw"])}
    amount, warmth, palette = settings[biome]
    seconds = 14 if n!=145 else 5
    size = seconds*RATE
    air, bed, points = [(zeros(size),zeros(size)) for _ in range(3)]
    rng = random.Random(stable(f"{biome}{index}"))
    smooth = [0,0]
    for i in range(size):
        u = i/size
        wind = .62+.22*math.sin(TAU*u*3)+.14*math.cos(TAU*u*7)
        for c in (0,1):
            noise = rng.uniform(-1,1)
            smooth[c] += amount*.17*(noise-smooth[c])
            air[c][i] = (smooth[c]*.26 + noise*amount*.018)*wind
            bed[c][i] = warmth*(.039*math.sin(TAU*(seconds*4*u+.3*math.sin(TAU*u*2+c)))+
                                .01*math.sin(TAU*(seconds*39*u+.5*math.sin(TAU*u*5+c))))
    seam = RATE//6
    for a in (*air,*bed):
        first,last = a[0],a[-1]
        for j in range(seam):
            amount = .5*(1-j/seam)
            a[j] += (last-first)*amount
            a[-1-j] += (first-last)*amount
    style = Style(n,biome,"ambience",index)
    keys = set()
    for k in range(8 if n!=145 else 5):
        data,width,key = event_audio(palette[k%len(palette)],style,k)
        if key:
            keys.add(key)
        comb(*points,data,.34 + (seconds-.8)*k/(8 if n!=145 else 5),(-1)**k*width,
             rng.uniform(.08,.20) if n!=145 else rng.uniform(.28,.54),True)
    layers = [("Điểm môi trường",*points)]
    if n!=145:
        layers = [("Gió / không gian",*air),("Nền địa hình",*bed),*layers]
    return assemble_mix(n,index,biome.capitalize(),layers,n!=145), keys
def category(n):
    if n<=7 or n==142:
        return "Nhân vật"
    if 15<=n<=19 or n in {*range(45,49),74,75,76,105,106,107}:
        return "HEXCORE và tiến trình"
    if n<=14 or 143<=n<=144:
        return "Sự kiện gameplay"
    if n<=44:
        return "Hệ Súng"
    if n<=73:
        return "Hệ Cận chiến"
    if n<=104:
        return "Hệ Pháp sư"
    if n<=125:
        return "Quái vật và Boss"
    if n<=133 or n==146:
        return "Menu và UI"
    if n<=137 or n==145:
        return "Môi trường"
    return "Nhạc nền"

def old_clip(n):
    mapping = {1:"Foot",2:"Dash",3:"Slam",4:"Impact",49:"Swing",50:"Swing",51:"HeavySwing",
               52:"Wave",53:"Wave",54:"Swing",55:"Parry",56:"Vanish",57:"Burst",58:"Dash",
               59:"Return",60:"Finisher",61:"Swing",62:"Slam",63:"HeavySwing",64:"HeavySwing",
               65:"Charge",66:"Slam",67:"Charge",68:"Wave",69:"Wave",70:"Wave",71:"Charge",
               72:"Wave",73:"Return",74:"Slam",75:"Wave",76:"Finisher"}
    if n==138:
        return "../../../Assets/Resources/BrotatoMenu/Home/Music.wav"
    if n==127:
        return "../../../Assets/Resources/BrotatoMenu/Home/Confirm.wav"
    if n in mapping:
        return "../../../Assets/Prefabs/Player/Melee/Shared/Audio/"+mapping[n]+".wav"
    return None

def visual(n, branch):
    guide = "../brotato-main-menu/assets/guide-b/"
    if n in {74,75,76}:
        return f"../../docs/Melee/HexcoreProduction/R{n-73}_Form01_East.gif"
    if 8<=n<=14 or 126<=n<=133 or n in {138,146}:
        return "../brotato-main-menu/assets/concept-b.jpg"
    if 134<=n<=137:
        return guide+"maps/"+["forest","moon","inferno","tundra"][n-134]+".jpg"
    if n in {108,109,110,111}:
        return guide+"normal-green.png"
    if n in {112,113}:
        return guide+"bow-snow.png"
    if n in {114,115,116,140}:
        return guide+"elite-red.png"
    if 117<=n<=125:
        return guide+("skeleton.png" if n<=119 else "golem.png" if n<=122 else "yeti.png")
    if 77<=n<=107:
        return guide+"mage.png"
    if 49<=n<=73:
        return guide+("assassin.png" if "Sát thủ" in branch else "dao.png" if "Đao sĩ" in branch else "qi.png" if "Kiếm khí" in branch else "player.png")
    return guide+"player.png"

def plan_rows():
    plan = (PROJECT/"docs/HEXCORE_AUDIO_SYSTEM_PLAN.md").read_text(encoding="utf-8")
    for line in plan.splitlines():
        if re.match(r"^\| \d+ \| ",line):
            yield [part.strip() for part in line.strip("|").split("|")]

def event_parts(spec):
    stage, remainder = spec.split(":")
    name, onset = remainder.split("@")
    return stage,float(onset),name

def main():
    OUT.mkdir(exist_ok=True)
    subset = {int(number) for number in sys.argv[1:]}
    catalog_path = ROOT/"catalog.js"
    previous = {}
    if subset:
        source = catalog_path.read_text(encoding="utf-8")
        previous = {row["id"]:row for row in json.loads(source.removeprefix("window.audioCatalog = ").rstrip(";\n"))}
    recipes = {}
    for line in (ROOT/"cues.txt").read_text().splitlines():
        n, flavor, family, *events = line.split()
        recipes[int(n)] = (flavor,family,events)
    recipes.update(REVISED)
    manifest = []
    all_sources = set()
    for id_, branch, title, description, timing, duration, desired_variants, _, priority in plan_rows():
        n = int(id_)
        flavor, family, events = recipes.pop(n)
        if subset and n not in subset:
            manifest.append(previous[n])
            all_sources.update(previous[n]["sources"])
            continue
        source_keys = set()
        if family.startswith("ambience-"):
            options = [ambience(n, family.split("-")[1],0)]
        elif flavor in {"spots"}:
            options = [ambience(n, biome,i) for i,biome in enumerate(("forest","moon","inferno","tundra"))]
        elif "music" in family:
            options = [score(n, flavor, profile, i) for i,profile in enumerate(SCORES[flavor])]
        else:
            items,source_keys = render_action(n,flavor,family,[event_parts(x) for x in events])
            options = [(x,set()) for x in items]
        for _, keys in options:
            source_keys.update(keys)
        all_sources.update(source_keys)
        existing = old_clip(n)
        if existing:
            existing = existing.replace("../../../Assets","../../Assets")
        manifest.append({"id":n,"category":category(n),"branch":branch,"name":title,
                         "description":description,"timing":timing,"priority":priority,
                         "target":desired_variants,"visual":visual(n,branch),
                         "old":existing,"variants":[x for x,_ in options],"sources":sorted(source_keys),
                         "projectLayers":sorted({clip for (cue, _),clip in PROJECT_LAYERS.items() if cue==n}),
                         "revision":"foley-pass" if n in REVISED else ""})
        if n in {19,48,76,107,125,146}:
            print(f"Rendered {n}/146", flush=True)
    assert not recipes and len(manifest)==146
    catalog_path.write_text("window.audioCatalog = "+json.dumps(manifest, ensure_ascii=False,separators=(",",":"))+";\n",encoding="utf-8")
    files = {p.name:sha256(p.read_bytes()).hexdigest() for p in (ROOT/"foley").glob("*.ogg") if p.stem in all_sources}
    files.update({p.relative_to(ROOT/"foley").as_posix():sha256(p.read_bytes()).hexdigest()
                  for p in (ROOT/"foley"/"swishes-original").glob("*.wav") if p.stem in all_sources})
    (ROOT/"used-foley.json").write_text(json.dumps(files, indent=2)+"\n")
    print(f"Used {len(files)} Foley originals. Exported {sum(1 for _ in OUT.glob('*.wav'))} WAVs.")

if __name__ == "__main__":
    main()

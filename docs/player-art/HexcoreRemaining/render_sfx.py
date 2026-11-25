"""Render original mono PCM cues for the HEXCORE preview. No external samples."""
from math import exp, pi, sin, tanh
from pathlib import Path
from random import Random
import struct
import wave

RATE = 24000
OUTPUT = Path(__file__).resolve().parents[3] / "Assets/Audio/HexcorePreview"

# Duration, brightness, tone (start/end), decay, filtered grit, pitched layer, rear echo.
CUES = {
    "Foot": (.095, .12, 116, 47, 35, .46, .62, 0),
    "Dash": (.215, .12, 226, 110, 9, .79, .30, .10),
    "Swing": (.15, .37, 930, 230, 16, .80, .20, .03),
    "HeavySwing": (.22, .19, 480, 130, 12, .78, .28, .08),
    "Impact": (.18, .38, 870, 180, 25, .67, .48, .09),
    "Parry": (.235, .54, 2400, 1510, 19, .34, .72, .13),
    "Charge": (.48, .14, 137, 430, 2.2, .12, .70, .12),
    "Wave": (.26, .28, 1010, 290, 6.1, .42, .63, .12),
    "Vanish": (.22, .46, 1200, 190, 8, .73, .24, .09),
    "Return": (.19, .36, 260, 1401, 11, .49, .55, .09),
    "Slam": (.44, .20, 146, 32, 8, .66, .78, .17),
    "Burst": (.38, .34, 780, 84, 8, .67, .62, .16),
    "Finisher": (.36, .34, 1760, 130, 9, .53, .75, .18),
}

def render(name, config):
    duration, brightness, start, end, decay, grit, musical, echo = config
    noise = Random(name)
    phase = filtered = 0.
    values = []
    for i in range(int(RATE * duration)):
        t = i / RATE
        fraction = t / duration
        phase += 2 * pi * (end + (start - end) * exp(-4 * fraction)) / RATE
        white = noise.uniform(-1, 1)
        filtered += brightness * (white - filtered)
        transient = (white - filtered) * exp(-t * 135) * .25
        attack = .072 if name == "Charge" else .006 if name in {"Swing", "Dash", "Vanish"} else .0015
        envelope = min(t / attack, 1) * exp(-decay * t) * min((duration - t) / .032, 1)
        resonances = sin(phase) + .28 * sin(phase * 1.61) + .13 * sin(phase * 2.67)
        raw = (grit * filtered + .55 * musical * resonances + transient) * envelope
        tap = i - 1130
        if tap >= 0:
            raw += echo * values[tap]
        values.append(tanh(raw))
    fade = int(RATE * .015)
    for i in range(fade):
        values[-i - 1] *= i / fade
    limit = .80 / max(map(abs, values))
    with wave.open(str(OUTPUT / f"{name}.wav"), "wb") as stream:
        stream.setparams((1, 2, RATE, 0, "NONE", "not compressed"))
        stream.writeframes(struct.pack(f"<{len(values)}h", *(round(x * limit * 32767) for x in values)))

if __name__ == "__main__":
    OUTPUT.mkdir(parents=True, exist_ok=True)
    for cue, settings in CUES.items():
        render(cue, settings)

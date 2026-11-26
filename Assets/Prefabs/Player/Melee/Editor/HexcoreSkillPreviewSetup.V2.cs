using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Object = UnityEngine.Object;

public static partial class HexcoreSkillPreviewSetup
{
    private const string VerifyingV2 = "HexcoreSkillPreview.VerifyingV2";
    private const string VerifyingV21 = "HexcoreSkillPreview.VerifyingV21";
    private const string V2ReviewFolder = "Library/HexcoreV2";
    private const float V2FrameStep = 1f / 16;
    private static readonly string[] V2Headings = { "N", "NE", "E", "SE", "S", "SW", "W", "NW" };

    private static void VerifyV2Catalog(MeleeController preview)
    {
        Directory.CreateDirectory(V2ReviewFolder);
        VerifyV2Slots();
        var results = new List<string> { "form,source_index,direction,result,dummy_hit_events,passive_triggers" };
        for (int form = 0; form < 4; form++)
        for (int skill = 0; skill < MeleeController.SkillCount; skill++)
        {
            if (!MeleeController.PreviewV2Compatible(skill, form)) continue;
            for (int direction = 0; direction < 8; direction++)
            {
                string row = $"{form + 1},{skill + 1},{V2Headings[direction]}";
                try
                {
                    VerifyV2Case(preview, form, skill, direction);
                    results.Add($"{row},PASS,{preview.ReviewPracticeHits},{preview.ReviewPassiveProcs}");
                }
                catch (Exception ex)
                {
                    results.Add($"{row},FAIL,{preview.ReviewPracticeHits},{preview.ReviewPassiveProcs}");
                    Debug.LogError($"{row}: {ex.Message}");
                }
            }
        }
        File.WriteAllLines(V2ReviewFolder + "/UnityMatrix.csv", results);
        var basics = new List<string> { "form,direction,normal_speed,half_speed" };
        for (int form = 0; form < 4; form++)
        for (int direction = 0; direction < 8; direction++)
        {
            string Result(float delta)
            {
                if (!preview.SelectV2Review(form, 0, direction, usePolish: true)) return "FAIL";
                preview.SelectBasic(form, direction);
                preview.EnableV2BasicReview();
                for (float time = 0; time < 1.5f; time += delta)
                {
                    preview.SampleV2Review(time, delta);
                    if (preview.CurrentSprite == null) return "FAIL";
                }
                return preview.ReviewPracticeHits > 0 ? "PASS" : "FAIL";
            }
            basics.Add($"{form + 1},{V2Headings[direction]},{Result(V2FrameStep)},{Result(V2FrameStep / 2)}");
        }
        File.WriteAllLines(V2ReviewFolder + "/UnityBasics.csv", basics);
        VerifyDaoPull(preview);
        VerifyFollowUpCombat(preview);
        if (Array.IndexOf(Environment.GetCommandLineArgs(), "-hexcoreV2Capture") >= 0)
            CaptureV2Comparisons(preview);
        if (results.Count != 337 || results.Exists(row => row.Contains(",FAIL,")) || basics.Exists(row => row.Contains("FAIL")))
            throw new InvalidOperationException("HEXCORE v2 matrix failed. See UnityMatrix.csv.");
        Debug.Log("HEXCORE V2 QA PASSED: 24 catalog slots, 336 samples, shared passives, branch target simulation and unchanged core poses.");
    }

    private static void VerifyFollowUpCombat(MeleeController preview)
    {
        const float epsilon = .002f;
        for (int direction = 0; direction < 8; direction++)
        foreach (float step in new[] { 1f / 60, 1f / 120 })
        {
            void Setup(int form, int skill, int count, bool boss = false)
            {
                if (!preview.SelectV2Review(form, skill, direction, usePolish: true))
                    throw new InvalidOperationException($"Missing feedback fixture skill {skill + 1}, {V2Headings[direction]}");
                preview.ConfigureReviewDummies(count, boss);
            }

            void Sample(float duration)
            {
                for (float time = step; time <= duration + .35f; time += step)
                    preview.SampleV2Review(time, step);
            }

            void Health(int dummy, float expected, string message)
            {
                if (Mathf.Abs(preview.ReviewDummyHp(dummy) - expected) >= epsilon)
                    throw new InvalidOperationException($"{message}, {V2Headings[direction]}, step {step:0.000}");
            }

            Setup(3, 0, 2);
            preview.PlaceReviewDummy(0, 1.1f, .20f);
            preview.PlaceReviewDummy(1, 1.7f, -.18f);
            Sample(MeleeController.SkillDuration(0));
            Health(0, 86, "Qi Basic must hit once per wave");
            Health(1, 16, "Both Qi Basic waves must pierce the second dummy");
            if (preview.ReviewPassiveHits != 2)
                throw new InvalidOperationException("A Qi Basic wave counted more than once toward the passive.");

            Setup(3, 9, 2, boss: true);
            preview.PlaceReviewDummy(0, 1.6f, 0);
            preview.PlaceReviewDummy(1, 2.5f, .45f);
            Vector3 bossStart = preview.ReviewDummyGround(0);
            Vector3 ordinaryStart = preview.ReviewDummyGround(1);
            Sample(MeleeController.SkillDuration(9));
            Health(0, 86, "Nguyet Nha must damage the boss");
            Health(1, 16, "Nguyet Nha must damage an ordinary dummy");
            if (Vector3.Distance(bossStart, preview.ReviewDummyGround(0)) >= epsilon ||
                Mathf.Abs(Vector3.Distance(ordinaryStart, preview.ReviewDummyGround(1)) - .63f) >= epsilon)
                throw new InvalidOperationException("Nguyet Nha displaced the boss or failed to push a normal dummy.");

            Setup(3, 22, 6);
            for (int dummy = 0; dummy < 6; dummy++) preview.PlaceReviewDummy(dummy, 1.8f + dummy * .63f, 0);
            Sample(MeleeController.SkillDuration(22));
            if (preview.ReviewChainHits != 5 || preview.ReviewPracticeHits != 5)
                throw new InvalidOperationException("The seeking sword must hit five distinct dummies.");
            Health(5, 100, "The sixth target must be untouched");

            Setup(1, 3, 2);
            preview.PlaceReviewDummy(0, 2.10f, 0);
            preview.PlaceReviewDummy(1, 4.25f, 0);
            Sample(MeleeController.SkillDuration(3));
            Health(0, 68, "Both ambush cuts must face the target behind the dash");
            Health(1, 30, "Ambush must not reuse the forward lane after the turn");

            Setup(1, 17, 5);
            for (int dummy = 0; dummy < 5; dummy++)
                preview.PlaceReviewDummy(dummy, 2.25f, new[] { 0, .27f, -1f, 1f, 2.4f }[dummy]);
            Sample(MeleeController.SkillDuration(17));
            Health(0, 82, "The middle dagger must hit");
            Health(1, 12, "Overlapping dagger and clone must count once");
            Health(2, 82, "The left clone must hit");
            Health(3, 82, "The right clone must hit");
            Health(4, 100, "An outsider must stay untouched");

            Setup(2, 7, 4);
            preview.PlaceReviewDummy(0, 1.25f, 0);
            preview.PlaceReviewDummy(1, 4.60f, 0);
            preview.PlaceReviewDummy(2, 1f, 1.95f);
            preview.PlaceReviewDummy(3, 5.7f, 0);
            float daoDuration = MeleeController.SkillDuration(7);
            for (float time = step; time <= daoDuration + .35f; time += step)
            {
                preview.SampleV2Review(time, step);
                if (time < 1f) Health(1, 30, "The far dummy must wait for the DAO wave");
            }
            Health(0, 84, "The near dummy must be hit");
            Health(1, 14, "The far dummy must be hit");
            Health(2, 100, "The outside-angle dummy must be untouched");
            Health(3, 100, "The outside-range dummy must be untouched");
        }
    }

    private static void VerifyDaoPull(MeleeController preview)
    {
        for (int direction = 0; direction < 8; direction++)
        foreach (int count in new[] { 0, 1, 5, 9 })
        {
            if (!preview.SelectV2Review(2, 8, direction, usePolish: true))
                throw new InvalidOperationException("Dao pull pose unavailable.");
            preview.ConfigureReviewDummies(count, resistPull: true);
            Vector3 boss = count > 0 ? preview.ReviewDummyGround(0) : Vector3.zero;
            Vector3 ordinary = count > 1 ? preview.ReviewDummyGround(1) : Vector3.zero;
            const float sweepStarts = .23f + .24f + .67f + .19f;
            float duration = MeleeController.SkillDuration(8);
            const float step = 1f / 60;
            for (float time = 0; time < duration; time += step)
            {
                preview.SampleV2Review(time, step);
                if (time < sweepStarts - .0001f && preview.ReviewPracticeHits != 0)
                    throw new InvalidOperationException("Dao pull dealt damage before the sweep.");
                if (count > 0 && Vector3.Distance(boss, preview.ReviewDummyGround(0)) > .0001f)
                    throw new InvalidOperationException("A pull-resistant dummy moved.");
            }
            if (count > 1 && Vector3.Distance(ordinary, preview.ReviewDummyGround(1)) < .05f)
                throw new InvalidOperationException("A nearby ordinary dummy was not pulled.");
            if (preview.ReviewPracticeHits > count || count == 1 && preview.ReviewPracticeHits != 0)
                throw new InvalidOperationException("The sweep counted a target twice or damaged the distant resistant target.");
        }
        preview.ConfigureReviewDummies(9, resistPull: false);
    }

    private static void VerifyV2Slots()
    {
        var ids = new HashSet<int>();
        for (int group = 0; group < 4; group++)
        for (int branch = 1; branch <= (group is 1 or 3 ? 3 : 1); branch++)
        for (int slot = 0; slot < 3; slot++)
            if (!ids.Add(MeleeController.PreviewV2Source(group, branch, slot)))
                throw new InvalidOperationException("Duplicate source ID in v2 loadout.");
        if (ids.Count != 24 || MeleeController.PreviewV2Source(1, 1, 2) != 15 ||
            MeleeController.PreviewV2Source(3, 1, 0) != 5 ||
            MeleeController.PreviewV2Source(1, 2, 2) != 18 ||
            MeleeController.PreviewV2Source(3, 2, 0) != 8 ||
            MeleeController.PreviewV2Source(1, 3, 2) != 21 ||
            MeleeController.PreviewV2Source(3, 3, 0) != 11)
            throw new InvalidOperationException("Approved E/F slot mapping was changed.");
    }

    private static void VerifyV2Case(MeleeController preview, int form, int skill, int direction)
    {
        bool core = skill is >= 12 and <= 14;
        float duration = MeleeController.SkillDuration(skill);
        var poses = new List<(Sprite Sprite, Vector3 Position, string Phase, bool Copy, bool Effect)>();
        if (core)
        {
            if (!preview.SelectSkill(form, skill, direction, direction, false, false))
                throw new InvalidOperationException("Legacy core fixture is unavailable.");
            for (int i = 0; i <= 24; i++)
            {
                preview.ReviewAt(duration * i / 24, true);
                poses.Add((preview.CurrentSprite, preview.ActorPosition, preview.Phase, preview.CopiesActive, preview.HasEffect));
            }
        }
        if (!preview.SelectV2Review(form, skill, direction, usePolish: true))
            throw new InvalidOperationException("Missing animation coverage or invalid combination.");
        Vector3 origin = preview.ActorPosition;
        if (skill < 2)
        {
            int strikes = form == 1 ? 3 : 2;
            int threshold = skill == 0 ? 5 : 4;
            for (int combo = 0; combo < 3; combo++)
            {
                preview.Replay();
                SampleV2Basic(preview);
            }
            if (preview.ReviewPassiveProcs != strikes * 3 / threshold ||
                preview.ReviewPassiveHits != strikes * 3 % threshold ||
                Vector3.Distance(origin, preview.ActorPosition) > .0001f)
                throw new InvalidOperationException("Passive counts targets, misses a Basic strike, or moves the actor.");
        }
        else if (skill == 2)
        {
            Sprite idle = preview.CurrentSprite;
            preview.ReviewReceiveHit();
            preview.ReviewReceiveHit();
            if (preview.ReviewPassiveProcs != 1 || Mathf.Abs(preview.ReviewPracticeHp - 79) > .001f ||
                preview.CurrentSprite != idle || Vector3.Distance(origin, preview.ActorPosition) > .0001f)
                throw new InvalidOperationException("Counter changed the pose/position or ignored its internal timer.");
            preview.SampleV2Review(0, 2.05f);
            preview.ReviewReceiveHit();
            if (preview.ReviewPassiveProcs != 2)
                throw new InvalidOperationException("Counter did not become ready again.");
        }
        else
        {
            int steps = core ? 24 : 120;
            for (int i = 0; i <= steps; i++)
            {
                preview.SampleV2Review(duration * i / steps, duration / steps);
                if (preview.CurrentSprite == null) throw new InvalidOperationException("Missing action sprite.");
                if (core)
                {
                    var baseline = poses[i];
                    if (preview.CurrentSprite != baseline.Sprite || preview.Phase != baseline.Phase ||
                        Vector3.Distance(preview.ActorPosition, baseline.Position) > .0001f ||
                        preview.CopiesActive != baseline.Copy || preview.HasEffect != baseline.Effect)
                        throw new InvalidOperationException("Core pose, displacement or effect visibility differs from the source timeline.");
                }
            }
            if (skill == 20)
            {
                if (!preview.ReviewEmpowered) throw new InvalidOperationException("Attack buff did not activate.");
                preview.SelectBasic(form, direction);
                preview.EnableV2BasicReview();
                SampleV2Basic(preview);
            }
            if (skill == 11 && preview.ReviewPracticeHits != 1)
                throw new InvalidOperationException("The aimed greatsword must hit one dummy.");
            if (skill == 21 && preview.ReviewRainBlades != (1 << 15) - 1)
                throw new InvalidOperationException("The sword rain must land all fifteen swords.");
            if (skill == 22 && preview.ReviewChainHits is < 1 or > 5)
                throw new InvalidOperationException("The seeking sword chain is outside the one-to-five target limit.");
            if (!core && preview.ReviewPracticeHits < 1)
                throw new InvalidOperationException("No simulated target was affected.");
        }
        preview.SampleV2Review(duration + .75f, .8f);
        if (preview.CopiesActive || preview.HasEffect)
            throw new InvalidOperationException("An action effect persisted after recovery.");
    }

    private static void SampleV2Basic(MeleeController preview)
    {
        for (int frame = 0; frame < 15; frame++) preview.SampleV2Review(frame * V2FrameStep, V2FrameStep);
    }

    private static void VerifyPolishSamples(MeleeController preview)
    {
        const string output = "Library/HexcoreV21";
        Directory.CreateDirectory(output);
        var cases = new (int form, int skill, string name)[]
        {
            (0, 0, "Basic-Swordsman"), (1, 0, "Basic-Assassin"),
            (2, 0, "Basic-DaoWarrior"), (3, 0, "Basic-QiSwordsman"),
            (0, 1, "P2"), (1, 4, "Huyet-An"), (2, 6, "Chan-Dia"), (3, 9, "Nguyet-Nha")
        };
        bool capture = Array.IndexOf(Environment.GetCommandLineArgs(), "-hexcoreV21Capture") >= 0;
        var camera = Object.FindFirstObjectByType<Camera>();
        var rows = new List<string> { "sample,heading,sprites,visual_samples,hit_events,status" };
        bool failed = false;
        foreach (var sample in cases)
        for (int heading = 0; heading < 8; heading++)
        {
            if (!preview.SelectV2Review(sample.form, sample.skill, heading, usePolish: true))
            {
                rows.Add($"{sample.name},{V2Headings[heading]},0,0,0,FAIL");
                failed = true;
                continue;
            }
            if (sample.skill == 1)
            {
                SampleV2Basic(preview);
                preview.Replay();
            }
            float duration = sample.skill < 3 ? 1.48f : MeleeController.SkillDuration(sample.skill) + .5f;
            var sprites = new HashSet<Sprite>();
            int visible = 0;
            for (int frame = 0; frame <= Mathf.CeilToInt(duration / V2FrameStep); frame++)
            {
                preview.SampleV2Review(frame * V2FrameStep, V2FrameStep);
                sprites.Add(preview.CurrentSprite);
                if (preview.HasEffect) visible++;
                if (capture && heading == 2) CaptureFrame(camera, $"{output}/{sample.name}", $"{frame:000}", 640, 360, true);
            }
            bool ok = sprites.Count >= 3 && !sprites.Contains(null) && visible > 0 &&
                preview.ReviewPracticeHits > 0 && (sample.skill != 1 || preview.ReviewPassiveProcs == 1) &&
                !preview.HasEffect && !preview.CopiesActive;
            rows.Add($"{sample.name},{V2Headings[heading]},{sprites.Count},{visible},{preview.ReviewPracticeHits},{(ok ? "PASS" : "FAIL")}");
            failed |= !ok;
        }
        File.WriteAllLines(output + "/Samples.csv", rows);
        if (failed) throw new InvalidOperationException("HEXCORE v2.1 sample validation failed; see Samples.csv.");
        Debug.Log("HEXCORE v2.1: 64 sample timelines passed automated checks.");
    }

    private static void CaptureV2Comparisons(MeleeController preview)
    {
        var camera = Object.FindFirstObjectByType<Camera>();
        for (int skill = 0; skill < MeleeController.SkillCount; skill++)
        {
            int form = Mathf.Max(0, MeleeController.PrimaryForm(skill));
            float duration = MeleeController.SkillDuration(skill);
            if (!preview.SelectSkill(form, skill, 2, 2, false, false))
                throw new InvalidOperationException("Baseline capture selection failed.");
            for (int frame = 0; frame <= Mathf.CeilToInt((duration + .24f) / V2FrameStep); frame++)
            {
                preview.ReviewAt(frame * V2FrameStep, true);
                CaptureFrame(camera, $"{V2ReviewFolder}/Before/{skill + 1:00}", $"{frame:000}", 640, 360, true);
            }

            if (!preview.SelectV2Review(form, skill, 2)) throw new InvalidOperationException("V2 capture selection failed.");
            if (skill < 2)
            {
                int primingCombos = skill == 0 ? 2 : 1;
                for (int i = 0; i < primingCombos; i++)
                {
                    SampleV2Basic(preview);
                    preview.Replay();
                }
                duration = 1.12f;
            }
            if (skill == 2)
            {
                preview.ReviewReceiveHit();
                duration = .92f;
            }
            for (int frame = 0; frame <= Mathf.CeilToInt((duration + .24f) / V2FrameStep); frame++)
            {
                preview.SampleV2Review(frame * V2FrameStep, V2FrameStep);
                CaptureFrame(camera, $"{V2ReviewFolder}/After/{skill + 1:00}", $"{frame:000}", 640, 360, true);
            }
        }
    }
}

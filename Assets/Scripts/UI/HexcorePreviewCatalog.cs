using System;

internal static class HexcorePreviewCatalog
{
    internal enum Action { Prepare, Advance, Vanish, Return, Swing, Hit, Guard, Charge, Rise, Dive, Land, Mark, Burst,
        Aura, Wave, Chase, Recall, Catch, Clone, Rain, Recover }

    internal sealed class Skill
    {
        internal readonly string Name;
        internal readonly int Form;
        internal readonly string Effect;
        internal readonly Action[] Beats;
        internal float[] Starts;

        internal Skill(string name, int form, string effect, params Action[] beats)
        {
            Name = name;
            Form = form;
            Effect = effect;
            Beats = beats;
        }

        internal bool Supports(int form) => Form == -1 || Form == form;
    }

    private static Skill Entry(string name, int form, string effect, params Action[] actions)
    {
        var beats = new Action[actions.Length + 2];
        beats[0] = Action.Prepare;
        Array.Copy(actions, 0, beats, 1, actions.Length);
        beats[^1] = Action.Recover;
        return new Skill(name, form, effect, beats);
    }

    internal static readonly Skill[] Skills =
    {
        Entry("Q1 · Kiếm Lướt / Phong Ảnh Trảm", 0, "", Action.Advance, Action.Swing, Action.Hit),
        Entry("Q2 · Hoành Nguyệt Trảm", 0, "MeleeSkills/DaoWarrior/HoanhTao", Action.Swing, Action.Hit),
        Entry("Q3 · Phản Kiếm Quyết", 0, "", Action.Guard, Action.Swing, Action.Hit),
        Entry("E1 · Ám Bộ Trảm", 1, "", Action.Vanish, Action.Return, Action.Swing, Action.Hit, Action.Swing, Action.Hit),
        Entry("E2 · Huyết Ấn", 1, "MeleeSkills/Assassin/AnTuHuyet", Action.Swing, Action.Mark, Action.Charge, Action.Burst),
        Entry("E3 · Tàng Ảnh", 1, "", Action.Vanish, Action.Advance, Action.Return, Action.Swing, Action.Hit),
        Entry("E1 · Chấn Địa Trảm", 2, "MeleeSkills/DaoWarrior/DiaLietTram", Action.Charge, Action.Swing, Action.Land),
        Entry("E2 · Hoành Thiên Đao", 2, "MeleeSkills/DaoWarrior/HoanhTao", Action.Charge, Action.Swing, Action.Hit),
        Entry("E3 · Bất Động Chiến Thế", 2, "MeleeSkills/DaoWarrior/TrongDao", Action.Guard, Action.Aura, Action.Swing, Action.Hit, Action.Swing),
        Entry("E1 · Nguyệt Nha Trảm", 3, "QiSwordsman/QiWave", Action.Swing, Action.Wave, Action.Hit),
        Entry("E2 · Lưỡng Nghi Kiếm", 3, "MeleeSkills/QiSwordsman/PhanQuang", Action.Swing, Action.Wave, Action.Swing, Action.Hit, Action.Burst),
        Entry("E3 · Kiếm Giới", 3, "MeleeSkills/QiSwordsman/KiemAn", Action.Swing, Action.Mark, Action.Aura, Action.Burst, Action.Burst),
        Entry("R1 · Thiên Vẫn Trảm", -1, "MeleeUltimates/ThienVan", Action.Rise, Action.Dive, Action.Swing, Action.Land),
        Entry("R2 · Tam Thiên Kiếm Quyết", -1, "QiSwordsman/QiWave", Action.Charge, Action.Swing, Action.Wave, Action.Swing, Action.Wave, Action.Charge, Action.Swing, Action.Wave, Action.Burst),
        Entry("R3 · Vạn Ảnh Tuyệt Sát", -1, "MeleeUltimates/VanAnh", Action.Vanish, Action.Clone, Action.Clone, Action.Return, Action.Swing, Action.Hit),
        Entry("F1 · Dạ Vũ Liên Trảm", 1, "", Action.Advance, Action.Swing, Action.Hit, Action.Advance, Action.Swing, Action.Hit, Action.Advance, Action.Swing, Action.Hit),
        Entry("F2 · Đoạt Mệnh", 1, "MeleeSkills/Assassin/AnTuHuyet", Action.Advance, Action.Charge, Action.Swing, Action.Hit, Action.Burst),
        Entry("F3 · Song Ảnh Truy Sát", 1, "MeleeSkills/Assassin/TamLienThu", Action.Clone, Action.Swing, Action.Clone, Action.Hit),
        Entry("F1 · Cuồng Phong Đao", 2, "MeleeSkills/DaoWarrior/HoanhTao", Action.Charge, Action.Swing, Action.Hit, Action.Swing, Action.Hit),
        Entry("F2 · Đoạn Sơn Trảm", 2, "MeleeSkills/DaoWarrior/DiaLietTram", Action.Charge, Action.Charge, Action.Swing, Action.Land, Action.Burst),
        Entry("F3 · Bất Diệt Chiến Ý", 2, "MeleeSkills/DaoWarrior/TrongDao", Action.Guard, Action.Aura, Action.Swing, Action.Hit, Action.Swing, Action.Hit),
        Entry("F1 · Kiếm Vũ Thiên Hà", 3, "MeleeSkills/QiSwordsman/KiemAn", Action.Charge, Action.Swing, Action.Rain, Action.Rain, Action.Hit),
        Entry("F2 · Phi Kiếm Truy Hồn", 3, "QiSwordsman/QiWave", Action.Swing, Action.Chase, Action.Hit),
        Entry("F3 · Hồi Nguyệt Kiếm", 3, "MeleeSkills/QiSwordsman/HoiNguyet", Action.Swing, Action.Wave, Action.Recall, Action.Catch)
    };

    static HexcorePreviewCatalog()
    {
        // Prepare first, recover last. Short strikes follow longer anticipation.
        float[][] timing =
        {
            new[] { .20f, .16f, .10f, .13f, .31f },
            new[] { .24f, .60f, .18f, .30f },
            new[] { .22f, .33f, .14f, .15f, .32f },
            new[] { .17f, .24f, .15f, .13f, .12f, .11f, .14f, .25f },
            new[] { .29f, .16f, .27f, .36f, .19f, .26f },
            new[] { .22f, .26f, .29f, .20f, .15f, .15f, .28f },
            new[] { .28f, .26f, .15f, .24f, .35f },
            new[] { .25f, .29f, .21f, .19f, .36f },
            new[] { .23f, .24f, .35f, .19f, .16f, .23f, .33f },
            new[] { .23f, .17f, .32f, .14f, .28f },
            new[] { .22f, .15f, .24f, .17f, .14f, .25f, .30f },
            new[] { .23f, .22f, .23f, .31f, .21f, .23f, .33f },
            new[] { .23f, .28f, .19f, .22f, .24f, .39f },
            new[] { .19f, .32f, .14f, .23f, .17f, .23f, .23f, .20f, .27f, .20f, .34f },
            new[] { .24f, .23f, .24f, .20f, .23f, .15f, .20f, .32f },
            new[] { .19f, .23f, .115f, .115f, .22f, .115f, .115f, .23f, .13f, .17f, .28f },
            new[] { .19f, .27f, .18f, .13f, .13f, .21f, .26f },
            new[] { .21f, .29f, .19f, .27f, .18f, .30f },
            new[] { .23f, .22f, .48f, .14f, .55f, .17f, .34f },
            new[] { .27f, .24f, .26f, .15f, .19f, .20f, .37f },
            new[] { .24f, .23f, .28f, .18f, .14f, .20f, .17f, .35f },
            new[] { .23f, .31f, .17f, .24f, .24f, .20f, .32f },
            new[] { .22f, .17f, .38f, .18f, .29f },
            new[] { .25f, .15f, .30f, .33f, .18f, .27f },
        };
        for (int i = 0; i < Skills.Length; i++)
        {
            var skill = Skills[i];
            if (timing[i].Length != skill.Beats.Length) throw new InvalidOperationException(skill.Name);
            skill.Starts = new float[skill.Beats.Length + 1];
            for (int beat = 0; beat < timing[i].Length; beat++)
                skill.Starts[beat + 1] = skill.Starts[beat] + timing[i][beat];
        }
    }
}

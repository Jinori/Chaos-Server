using Chaos.Extensions;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Scripting.MonsterScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Boss.EventBoss.Christmas.Snowman.MtMerry80;

public sealed class MtMerry80BossEnrageScript : MonsterScriptBase
{
    private readonly IIntervalTimer ActionTimer;
    private readonly ISpellFactory SpellFactory;
    private readonly IIntervalTimer SpellTimer;
    private readonly Spell SpellToCast;
    private readonly Spell SpellToCast2;
    private readonly Spell SpellToCast3;

    private bool Bonus30Applied;
    private bool Bonus50Applied;
    private bool Bonus75Applied;

    private bool HasCastedSpell1;
    private bool HasCastedSpell2;
    private bool HasCastedSpell3;

    // Add a counter for how many times we've cast SpellToCast3
    private int Spell3Casts;

    private Animation UpgradeAnimation { get; } = new()
    {
        AnimationSpeed = 100,
        TargetAnimation = 189
    };

    public MtMerry80BossEnrageScript(Monster subject, ISpellFactory spellFactory)
        : base(subject)
    {
        SpellFactory = spellFactory;
        ActionTimer = new IntervalTimer(TimeSpan.FromSeconds(20), false);
        SpellTimer = new IntervalTimer(TimeSpan.FromSeconds(2), false);
        SpellToCast = SpellFactory.Create("SnowmanSpell140");
        SpellToCast2 = SpellFactory.Create("cradh");
        SpellToCast3 = SpellFactory.Create("SnowmanSpell240");
    }

    public override void Update(TimeSpan delta)
    {
        ActionTimer.Update(delta);
        SpellTimer.Update(delta);

        if (ActionTimer.IntervalElapsed)
        {
            if (!HasCastedSpell1)
            {
                Subject.Say("Your jolly Santa is a fraud!");
                HasCastedSpell1 = true;

                foreach (var aisling in Subject.MapInstance
                                               .GetEntities<Aisling>()
                                               .Where(x => x.WithinRange(Subject, 10) && !x.IsGodModeEnabled() && !x.IsDead))
                    Subject.TryUseSpell(SpellToCast2, aisling.Id);

                return;
            }

            switch (HasCastedSpell2)
            {
                case false when HasCastedSpell1:
                    Subject.Say("Merry? Bah! Meet my cold revenge!");
                    Subject.TryUseSpell(SpellToCast3);
                    HasCastedSpell2 = true;

                    return;
                case true when !HasCastedSpell3:
                    Subject.Say("Santa is too soft, I am all freeze!");
                    Subject.TryUseSpell(SpellToCast);

                    // First cast of SpellToCast3 done, set HasCastedSpell3 and start counting
                    HasCastedSpell3 = true;
                    Spell3Casts = 1;

                    return;
            }
        }

        // If we've cast SpellToCast3 once and we still need more casts
        // SpellTimer at 2s interval will trigger repeats until we reach 5 casts total
        if (HasCastedSpell3 && SpellTimer.IntervalElapsed && (Spell3Casts < 5))
        {
            Subject.TryUseSpell(SpellToCast);
            Spell3Casts++;

            if (Spell3Casts >= 4)
            {
                // After 5 casts, reset these flags so the sequence can happen again if needed
                HasCastedSpell1 = false;
                HasCastedSpell2 = false;
                HasCastedSpell3 = false;
            }
        }

        if (!Bonus75Applied && (Subject.StatSheet.HealthPercent <= 75))
        {
            Bonus75Applied = true;

            var attrib = new Attributes
            {
                AtkSpeedPct = 10
            };
            Subject.StatSheet.AddBonus(attrib);
            Subject.Animate(UpgradeAnimation);
        }

        if (!Bonus50Applied && (Subject.StatSheet.HealthPercent <= 50))
        {
            Bonus50Applied = true;

            var attrib = new Attributes
            {
                Dmg = 2,
                MagicResistance = 10,
                SkillDamagePct = 5,
                SpellDamagePct = 5
            };

            Subject.StatSheet.AddBonus(attrib);
            Subject.Animate(UpgradeAnimation);
        }

        if (!Bonus30Applied && (Subject.StatSheet.HealthPercent <= 30))
        {
            Bonus30Applied = true;

            var attrib = new Attributes
            {
                Str = 10,
                Ac = -10
            };

            Subject.StatSheet.AddBonus(attrib);
            Subject.Animate(UpgradeAnimation);
        }
    }
}
using Chaos.Common.Definitions;
using Chaos.Common.Utilities;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Scripting.MonsterScripts.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Boss.UndineFields;

public sealed class UFOrcEnrageScript : MonsterScriptBase
{
    private readonly IIntervalTimer SkillUseTimer;
    private bool Bonus30Applied;
    private bool Bonus50Applied;
    private bool Bonus75Applied;

    private Animation UpgradeAnimation { get; } = new()
    {
        AnimationSpeed = 100,
        TargetAnimation = 189
    };

    /// <inheritdoc />
    public UFOrcEnrageScript(Monster subject)
        : base(subject)
        => SkillUseTimer = new RandomizedIntervalTimer(
            TimeSpan.FromSeconds(20),
            20,
            RandomizationType.Balanced,
            false);

    public override void Update(TimeSpan delta)
    {
        SkillUseTimer.Update(delta);

        if (SkillUseTimer.IntervalElapsed)
        {
            var roll = IntegerRandomizer.RollSingle(100);

            switch (roll)
            {
                case < 40:
                    Subject.Say("GIT OUT OF MY FIELDS!");

                    break;

                case < 80:
                    Subject.Say("Wuht do u thnk ur doin here?");

                    break;

                case < 101:
                    Subject.StatSheet.AddHealthPct(10);
                    Subject.Say("Im harder to kill than that.");

                    break;
            }
        }

        if (!Bonus75Applied && (Subject.StatSheet.HealthPercent <= 75))
        {
            Bonus75Applied = true;

            //Give Bonuses
            var attrib = new Attributes
            {
                AtkSpeedPct = 50
            };
            Subject.StatSheet.AddBonus(attrib);
            Subject.Animate(UpgradeAnimation);

            //Spawn Monsters
        }

        if (!Bonus50Applied && (Subject.StatSheet.HealthPercent <= 50))
        {
            Bonus50Applied = true;

            var attrib = new Attributes
            {
                Dmg = 15,
                SkillDamagePct = 20
            };

            Subject.StatSheet.AddBonus(attrib);
            Subject.Animate(UpgradeAnimation);
        }

        if (!Bonus30Applied && (Subject.StatSheet.HealthPercent <= 30))
        {
            Bonus30Applied = true;

            var attrib = new Attributes
            {
                FlatSkillDamage = 5,
                FlatSpellDamage = 5,
                SkillDamagePct = 20,
                SpellDamagePct = 20
            };

            Subject.StatSheet.AddBonus(attrib);
            Subject.Animate(UpgradeAnimation);
        }
    }
}
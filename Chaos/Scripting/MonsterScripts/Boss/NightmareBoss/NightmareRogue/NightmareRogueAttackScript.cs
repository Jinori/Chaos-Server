using Chaos.Common.Definitions;
using Chaos.Extensions;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Scripting.MonsterScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Boss.NightmareBoss.NightmareRogue;

public sealed class NightmareRogueAttackScript : MonsterScriptBase
{
    private readonly ISkillFactory SkillFactory;
    private readonly ISpellFactory SpellFactory;
    private readonly Skill Throw;
    private readonly Skill Ambush;
    private readonly Spell SpringTrap;
    private readonly IIntervalTimer WarningTimer;
    private readonly IIntervalTimer AmbushThrowTimer;
    private bool WarningGiven;
    
    /// <inheritdoc />
    public NightmareRogueAttackScript(Monster subject, ISkillFactory skillFactory, ISpellFactory spellFactory)
        : base(subject)
    {
        SkillFactory = skillFactory;
        SpellFactory = spellFactory;
        Throw = SkillFactory.Create("throw");
        Throw.Cooldown = null;
        Ambush = SkillFactory.Create("ambush");
        Ambush.Cooldown = null;
        SpringTrap = SpellFactory.Create("springtrap");
        SpringTrap.Cooldown = null;

        WarningTimer = new RandomizedIntervalTimer(
            TimeSpan.FromSeconds(8),
            20,
            RandomizationType.Balanced,
            false);

        AmbushThrowTimer = new IntervalTimer(
            TimeSpan.FromSeconds(3),
            false);
    }

    public override void Update(TimeSpan delta)
    {
        base.Update(delta);

        if (Target is null || !Subject.WithinRange(Target, 1))
        {
            WarningTimer.Reset();
            AmbushThrowTimer.Reset();

            return;
        }

        if (!WarningGiven)
        {
            WarningTimer.Update(delta);

            if (WarningTimer.IntervalElapsed)
            {
                Subject.Say("I've got something for you!");
                WarningGiven = true;
                Subject.TryUseSpell(SpringTrap);
            }
        } else
        {
            AmbushThrowTimer.Update(delta);

            if (AmbushThrowTimer.IntervalElapsed)
            {
                Subject.TryUseSkill(Ambush);
                Subject.TryUseSkill(Throw);
                Subject.Say("Play with that!");
                WarningGiven = false;
            }
        }
    }
}
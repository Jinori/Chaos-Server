using Chaos.Geometry.Abstractions.Definitions;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Scripting.MonsterScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Boss.MainStory.TrialOfSacrifice.sacrificebossmob;

public sealed class SacrificeBossEnrageScript : MonsterScriptBase
{
    private readonly IIntervalTimer Saying1Timer;
    private readonly IIntervalTimer Saying2Timer;
    private readonly IIntervalTimer SpellCastTimer;
    private readonly Spell SpellToCast;

    /// <inheritdoc />
    public SacrificeBossEnrageScript(Monster subject, ISpellFactory spellFactory)
        : base(subject)
    {
        SpellToCast = spellFactory.Create("fireline");
        SpellCastTimer = new IntervalTimer(TimeSpan.FromSeconds(14), false);
        Saying1Timer = new IntervalTimer(TimeSpan.FromSeconds(5), false);
        Saying2Timer = new IntervalTimer(TimeSpan.FromSeconds(8), false);
    }

    public override void Update(TimeSpan delta)
    {
        SpellCastTimer.Update(delta);
        Saying1Timer.Update(delta);
        Saying2Timer.Update(delta);

        if (Saying1Timer.IntervalElapsed)
        {
            Saying1Timer.Reset();
            const Direction DIRECTION = Direction.Up;
            Subject.Direction = DIRECTION;
            Subject.Say("There you are Zoe, I've been looking for you.");
        }

        if (Saying2Timer.IntervalElapsed)
        {
            Saying1Timer.Reset();
            Saying2Timer.Reset();
            Subject.Say("You can't stop me Aisling! Time to die Zoe!");
        }

        if (SpellCastTimer.IntervalElapsed)
        {
            Saying1Timer.Reset();
            Saying2Timer.Reset();
            SpellCastTimer.Reset();
            Subject.Say("You must die for your sins Zoe!");
            Subject.TryUseSpell(SpellToCast);
        }
    }
}
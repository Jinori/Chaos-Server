using Chaos.Extensions;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Scripting.MonsterScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Limbo.Monk;

public class LimboMonkScript : MonsterScriptBase
{
    private readonly IIntervalTimer ActionTimer;
    private readonly Spell CureAilments;
    private readonly ISpellFactory SpellFactory;

    public LimboMonkScript(Monster subject, ISpellFactory spellFactory)
        : base(subject)
    {
        SpellFactory = spellFactory;
        ActionTimer = new RandomizedIntervalTimer(TimeSpan.FromMilliseconds(1000), 10, startAsElapsed: false);
        CureAilments = SpellFactory.Create("cureailments");
    }

    public override void Update(TimeSpan delta)
    {
        ActionTimer.Update(delta);

        if (!ActionTimer.IntervalElapsed)
            return;

        if (Subject.IsBlind || Subject.IsSuained() || Subject.Effects.Contains("Poison"))
            Subject.TryUseSpell(CureAilments);
    }
}
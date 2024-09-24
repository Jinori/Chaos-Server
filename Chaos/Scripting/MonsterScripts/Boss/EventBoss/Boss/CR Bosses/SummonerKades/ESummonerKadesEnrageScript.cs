using Chaos.Common.Definitions;
using Chaos.Common.Utilities;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Scripting.MonsterScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Boss.EventBoss.Boss.CR_Bosses.SummonerKades;

public sealed class ESummonerKadesEnrageScript : MonsterScriptBase
{
    private readonly IIntervalTimer SpellCastTimer;
    private readonly Spell SpellToCast;

    /// <inheritdoc />
    public ESummonerKadesEnrageScript(Monster subject, ISpellFactory spellFactory)
        : base(subject)
    {
        SpellToCast = spellFactory.Create("entangle");
        SpellCastTimer = new RandomizedIntervalTimer(TimeSpan.FromSeconds(15), 30, RandomizationType.Balanced, false);
    }

    public override void Update(TimeSpan delta)
    {
        SpellCastTimer.Update(delta);

        if (SpellCastTimer.IntervalElapsed)
        {
            var roll = IntegerRandomizer.RollSingle(100);
            
            switch (roll)
            {
                case < 40:
                    Subject.Say("Death is easier if you welcome it!");
                    Subject.TryUseSpell(SpellToCast);

                    break;
                case < 75:
                    Subject.Say("You will not get out of here alive!");
                    Subject.TryUseSpell(SpellToCast);

                    break;
                case < 101:
                    Subject.Say("Let the shadows consume you!");
                    Subject.TryUseSpell(SpellToCast);
                    break;
            }
        }
    }
}
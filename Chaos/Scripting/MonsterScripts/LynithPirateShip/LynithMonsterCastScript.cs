using Chaos.Common.Definitions;
using Chaos.Common.Utilities;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Scripting.MonsterScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.MonsterScripts.LynithPirateShip;

public sealed class LynithMonsterCastScript : MonsterScriptBase
{
    private readonly IIntervalTimer SpellCastTimer;
    private readonly Spell SpellToCast;
    private readonly Spell SpellToCast1;
    private readonly Spell SpellToCast2;

    /// <inheritdoc />
    public LynithMonsterCastScript(Monster subject, ISpellFactory spellFactory)
        : base(subject)
    {
        SpellToCast = spellFactory.Create("morcradh");
        SpellToCast1 = spellFactory.Create("SeaMonsterSpell2");
        SpellToCast2 = spellFactory.Create("SeaMonsterSpell3");
        SpellCastTimer = new RandomizedIntervalTimer(TimeSpan.FromSeconds(15), 20, RandomizationType.Balanced, false);
    }

    public override void Update(TimeSpan delta)
    {
        SpellCastTimer.Update(delta);

        if (SpellCastTimer.IntervalElapsed)
        {
            var roll = IntegerRandomizer.RollSingle(100);
            
            switch (roll)
            {
                case < 25:
                    Subject.TryUseSpell(SpellToCast);

                    break;
                case < 50:
                    Subject.TryUseSpell(SpellToCast1);
                    break;
                
                    case < 75:
                    Subject.TryUseSpell(SpellToCast2);
                    break;
                    
                case < 101:
                    foreach (var target in Subject.MapInstance.GetEntitiesWithinRange<Aisling>(Subject, 10))
                    {
                        if (target.IsDead)
                            continue;
                        
                        Subject.TryUseSpell(SpellToCast, target.Id);
                        Subject.TryUseSpell(SpellToCast1, target.Id);
                    }

                    break;
            }
        }
    }
}
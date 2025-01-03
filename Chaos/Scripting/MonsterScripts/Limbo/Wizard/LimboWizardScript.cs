using Chaos.Common.Utilities;
using Chaos.Extensions;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Scripting.MonsterScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Limbo.Wizard;

public class LimboWizardScript : MonsterScriptBase
{
    private readonly IIntervalTimer ActionTimer;
    private readonly Spell ArdAtharMeall;
    private readonly Spell ArdCradh;
    private readonly Spell ArdCreagMeall;
    private readonly Spell ArdSalMeall;
    private readonly Spell ArdSradMeall;
    private readonly Spell ChainLightning;
    private readonly Spell Dall;
    private readonly Spell MagmaSurge;
    private readonly Spell[] MeallSpells;
    private readonly ISpellFactory SpellFactory;
    private readonly Dictionary<Func<List<Aisling>, bool>, int> WeightedActions;

    public LimboWizardScript(Monster subject, ISpellFactory spellFactory)
        : base(subject)
    {
        SpellFactory = spellFactory;
        ActionTimer = new RandomizedIntervalTimer(TimeSpan.FromMilliseconds(1000), 10, startAsElapsed: false);

        MagmaSurge = SpellFactory.Create("magmasurge");
        ArdCradh = SpellFactory.Create("ardCradh");
        Dall = SpellFactory.Create("dall");
        ArdSalMeall = SpellFactory.Create("ardSalMeall");
        ArdSradMeall = SpellFactory.Create("ardSradMeall");
        ArdAtharMeall = SpellFactory.Create("ardAtharMeall");
        ArdCreagMeall = SpellFactory.Create("ardCreagMeall");
        ChainLightning = SpellFactory.Create("chainLightning");

        MeallSpells =
        [
            ArdSalMeall,
            ArdSradMeall,
            ArdAtharMeall,
            ArdCreagMeall
        ];

        WeightedActions = new Dictionary<Func<List<Aisling>, bool>, int>
        {
            {
                DoChainLightning, 2
            },
            {
                DoArdCradh, 3
            },
            {
                DoDall, 1
            },
            {
                DoMeall, 1
            }
        };
    }

    private bool DoArdCradh(List<Aisling> nearbyAislings)
    {
        var notArdCradh = nearbyAislings.Where(aisling => !aisling.Effects.Contains("ard cradh"))
                                        .ToList();

        if (notArdCradh.Count == 0)
            return false;

        var target = notArdCradh.PickRandom();

        if (Subject.TryUseSpell(ArdCradh, target.Id))
            return true;

        return false;
    }

    private bool DoChainLightning(List<Aisling> nearbyAislings)
    {
        var target = Subject.Target;

        if (target is not null)
            if (Subject.TryUseSpell(ChainLightning, target.Id))
                return true;

        return false;
    }

    private bool DoDall(List<Aisling> nearbyAislings)
    {
        var notDalled = nearbyAislings.Where(aisling => !aisling.IsBlind)
                                      .ToList();

        if (notDalled.Count == 0)
            return false;

        var target = notDalled.PickRandom();

        if (Subject.TryUseSpell(Dall, target.Id))
            return true;

        return false;
    }

    private bool DoMeall(List<Aisling> nearbyAislings)
    {
        var possibleMealls = MeallSpells.Where(
                                            spell => Subject.CanUse(
                                                spell,
                                                Subject,
                                                null,
                                                out _))
                                        .ToList();

        if (possibleMealls.Count == 0)
            return false;

        var meallToCast = possibleMealls.PickRandom();

        //target the aisling that will cause the meall to hit the most targets
        var optimalTarget = nearbyAislings.ToDictionary(aisling => aisling, aisling => nearbyAislings.Count(x => aisling.WithinRange(x, 1)))
                                          .MaxBy(kvp => kvp.Value)
                                          .Key;

        if (Subject.TryUseSpell(meallToCast, optimalTarget.Id))
            return true;

        return false;
    }

    public override void Update(TimeSpan delta)
    {
        ActionTimer.Update(delta);

        if (!ActionTimer.IntervalElapsed)
            return;

        var nearbyAislings = Map.GetEntitiesWithinRange<Aisling>(Subject)
                                .ThatAreObservedBy(Subject)
                                .ThatAreVisibleTo(Subject)
                                .ToList();

        if (nearbyAislings.Count == 0)
            return;

        //cast magma surge if any aislings within 3 spaces
        if (nearbyAislings.Any(aisling => Subject.WithinRange(aisling, 3)))
            if (Subject.TryUseSpell(MagmaSurge))
                return;

        var counter = 0;

        //try up to 10 times picking random actions until one of them succeeds
        while (counter++ < 10)
        {
            var randomAction = WeightedActions.PickRandomWeighted();

            if (randomAction(nearbyAislings))
                break;
        }
    }
}
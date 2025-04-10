using Chaos.Common.Utilities;
using Chaos.Extensions;
using Chaos.Extensions.Common;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.MonsterScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Limbo.Wizard;

public class LimboWizardScript : MonsterScriptBase
{
    private readonly IIntervalTimer ActionTimer;
    private readonly Spell[] AllSpells;
    private readonly Spell ArdAtharMeall;
    private readonly Spell ArdCreagMeall;
    private readonly Spell ArdSalMeall;
    private readonly Spell ArdSradMeall;
    private readonly Spell ChainLightning;
    private readonly Spell Dall;
    private readonly Spell DiaCradh;
    private readonly TimeSpan FizzleCooldown;
    private readonly Spell MagmaSurge;
    private readonly Spell[] MeallSpells;
    private readonly ISpellFactory SpellFactory;
    private readonly Dictionary<Func<List<Aisling>, bool>, int> WeightedActions;
    private TimeSpan TimeSinceLastFizzle;

    public LimboWizardScript(Monster subject, ISpellFactory spellFactory)
        : base(subject)
    {
        SpellFactory = spellFactory;
        ActionTimer = new RandomizedIntervalTimer(TimeSpan.FromMilliseconds(1000), 10, startAsElapsed: false);
        TimeSinceLastFizzle = TimeSpan.Zero;
        FizzleCooldown = TimeSpan.FromSeconds(5);

        MagmaSurge = SpellFactory.Create("magmasurge");
        DiaCradh = SpellFactory.Create("diaCradh");
        Dall = SpellFactory.Create("dall");
        ArdSalMeall = SpellFactory.Create("ardSalMeall");
        ArdSradMeall = SpellFactory.Create("ardSradMeall");
        ArdAtharMeall = SpellFactory.Create("ardAtharMeall");
        ArdCreagMeall = SpellFactory.Create("ardCreagMeall");
        ChainLightning = SpellFactory.Create("chainLightning");

        AllSpells =
        [
            MagmaSurge,
            DiaCradh,
            Dall,
            ArdSalMeall,
            ArdSradMeall,
            ArdAtharMeall,
            ArdCreagMeall,
            ChainLightning
        ];

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
                DoChainLightning, 30
            },
            {
                DoDiaCradh, 35
            },
            {
                DoDall, 1
            },
            {
                DoMeall, 25
            },
            {
                //regenerate mana
                _ =>
                {
                    if (Subject.StatSheet.ManaPercent <= 60)
                    {
                        if (Subject.StatSheet.HealthPercent >= 75)
                        {
                            Subject.StatSheet.SubtractHealthPct(3);
                            Subject.StatSheet.AddManaPct(10);
                        } else
                            Subject.StatSheet.AddManaPct(5);
                    }

                    return true;
                }, 9
            }
        };
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
        var notDalled = nearbyAislings.Where(aisling => !aisling.IsDall)
                                      .ToList();

        if (notDalled.Count == 0)
            return false;

        var target = notDalled.PickRandom();

        if (Subject.TryUseSpell(Dall, target.Id))
        {
            Dall.SetTemporaryCooldown(TimeSpan.FromSeconds(25));

            return true;
        }

        return false;
    }

    private bool DoDiaCradh(List<Aisling> nearbyAislings)
    {
        var notDiaCradh = nearbyAislings.Where(aisling => !aisling.Effects.Contains("dia cradh"))
                                        .ToList();

        if (notDiaCradh.Count == 0)
            return false;

        var target = notDiaCradh.PickRandom();

        if (Subject.TryUseSpell(DiaCradh, target.Id))
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
                                          .Shuffle()
                                          .MaxBy(kvp => kvp.Value)
                                          .Key;

        if (Subject.TryUseSpell(meallToCast, optimalTarget.Id))
            return true;

        return false;
    }

    public override void OnAttacked(Creature source, int damage)
    {
        if (TimeSinceLastFizzle > FizzleCooldown)
        {
            TimeSinceLastFizzle = TimeSpan.Zero;

            ActionTimer.Reset();
            ResetMovementTimer();
        }
    }

    private void ResetMovementTimer()
    {
        Subject.WanderTimer.Reset();
        Subject.MoveTimer.Reset();
    }

    public override void Update(TimeSpan delta)
    {
        foreach (var spell in AllSpells)
            spell.Update(delta);

        ActionTimer.Update(delta);

        if (!ActionTimer.IntervalElapsed)
            return;

        var nearbyAislings = Map.GetEntitiesWithinRange<Aisling>(Subject, 10)
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

        //if no nearby aislings within 8 spaces
        //then dont cast on anyone
        //this forces them to walk up some rather than casting from edge of screen
        if (!nearbyAislings.Any(x => Subject.WithinRange(x, 8)))
            return;

        //try up to 5 times picking random actions until one of them succeeds
        while (counter++ < 5)
        {
            var randomAction = WeightedActions.PickRandomWeighted();

            if (randomAction(nearbyAislings))
            {
                ResetMovementTimer();

                break;
            }
        }
    }
}
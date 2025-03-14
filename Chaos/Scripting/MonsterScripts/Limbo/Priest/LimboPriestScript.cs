using Chaos.Common.Utilities;
using Chaos.Extensions;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Scripting.MonsterScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Limbo.Priest;

public class LimboPriestScript : MonsterScriptBase
{
    private static readonly int[] ClassWeights =
    [
        1,
        1,
        3,
        3,
        2,
        1
    ];

    private readonly IIntervalTimer ActionTimer;
    private readonly Spell AoArdCradh;
    private readonly Spell AoDall;
    private readonly Spell AoPoison;
    private readonly Spell AoSuain;
    private readonly Spell Dinarcoli;
    private readonly Spell Nuadhaich;
    private readonly Spell Pramh;
    private readonly ISpellFactory SpellFactory;
    private readonly Spell[] AllSpells;

    public LimboPriestScript(Monster subject, ISpellFactory spellFactory)
        : base(subject)
    {
        SpellFactory = spellFactory;
        Nuadhaich = SpellFactory.Create("nuadhaich");
        Pramh = SpellFactory.Create("pramh");
        AoDall = SpellFactory.Create("aodall");
        AoPoison = SpellFactory.Create("aopoison");
        AoSuain = SpellFactory.Create("aosuain");
        Dinarcoli = SpellFactory.Create("dinarcoli");
        AoArdCradh = SpellFactory.Create("aoardcradh");
        ActionTimer = new RandomizedIntervalTimer(TimeSpan.FromMilliseconds(1000), 10, startAsElapsed: false);
        AllSpells = [Nuadhaich, Pramh, AoDall, AoPoison, AoSuain, Dinarcoli, AoArdCradh];
    }

    public override void Update(TimeSpan delta)
    {
        foreach (var spell in AllSpells)
            spell.Update(delta);
        
        ActionTimer.Update(delta);

        if (!ActionTimer.IntervalElapsed)
            return;

        var shouldPramh = IntegerRandomizer.RollChance(25);

        //cure self of control effects
        if (Subject.IsPramhed())
        {
            Subject.TryUseSpell(Dinarcoli, Subject.Id);

            return;
        }

        if (Subject.IsSuained())
        {
            Subject.TryUseSpell(AoSuain, Subject.Id);

            return;
        }

        var nearbyMonsters = Map.GetEntitiesWithinRange<Monster>(Subject)
                                .ThatAreObservedBy(Subject)
                                .ThatAreVisibleTo(Subject)
                                .ToList();

        //always heal lowest monster on screen
        var lowestNearbyMonster = nearbyMonsters.MinBy(monster => monster.StatSheet.HealthPercent);

        if (lowestNearbyMonster is not null && (lowestNearbyMonster.StatSheet.HealthPercent < 100))
            Subject.TryUseSpell(Nuadhaich, lowestNearbyMonster.Id);

        //dont return, priests always heal on top of any other spells

        if (shouldPramh)
        {
            var possibleTargets = Map.GetEntitiesWithinRange<Aisling>(Subject)
                                     .Where(aisling => !aisling.IsPramhed())
                                     .ThatAreObservedBy(Subject)
                                     .ThatAreVisibleTo(Subject)
                                     .ToList();

            if (possibleTargets.Count > 0)
            {
                //select distinct classes from possible targets
                //pick a random class based on weights
                var targetClass = possibleTargets.Select(aisling => aisling.UserStatSheet.BaseClass)
                                                 .Distinct()
                                                 .ToDictionary(@class => @class, @class => ClassWeights[(int)@class])
                                                 .PickRandomWeighted();

                //choose a random person of the target class
                var target = possibleTargets.Where(aisling => aisling.UserStatSheet.BaseClass == targetClass)
                                            .ToList()
                                            .PickRandom();

                Subject.TryUseSpell(Pramh, target.Id);

                //dont return, priests always have a chance to cast pramh on top of any other spells
            }
        }

        //ao self curse
        if (Subject.Effects.Contains("ard cradh") || Subject.Effects.Contains("Mor Cradh") || Subject.Effects.Contains("Cradh") || Subject.Effects.Contains("Beag Cradh"))
        {
            Subject.TryUseSpell(AoArdCradh, Subject.Id);

            return;
        }

        //ao self blind
        if (Subject.IsBlind)
        {
            Subject.TryUseSpell(AoDall, Subject.Id);

            return;
        }

        foreach (var monster in nearbyMonsters)
        {
            if (monster.Effects.Contains("ard cradh"))
            {
                Subject.TryUseSpell(AoArdCradh, monster.Id);

                return;
            }

            if (monster.IsPramhed())
            {
                Subject.TryUseSpell(Dinarcoli, monster.Id);

                return;
            }

            if (monster.IsSuained())
            {
                Subject.TryUseSpell(AoSuain, monster.Id);

                return;
            }

            if (monster.IsBlind)
            {
                Subject.TryUseSpell(AoDall, monster.Id);

                return;
            }
        }

        if (Subject.Effects.Contains("Poison"))
        {
            Subject.TryUseSpell(AoPoison, Subject.Id);

            return;
        }

        foreach (var monster in nearbyMonsters)
            if (monster.Effects.Contains("Poison"))
            {
                Subject.TryUseSpell(AoPoison, monster.Id);

                return;
            }
    }
}
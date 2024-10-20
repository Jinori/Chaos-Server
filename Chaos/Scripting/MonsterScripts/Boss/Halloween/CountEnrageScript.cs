using Chaos.Common.Definitions;
using Chaos.Common.Utilities;
using Chaos.Extensions;
using Chaos.Extensions.Geometry;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Scripting.MonsterScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Boss.Halloween;

public sealed class CountEnrageScript : MonsterScriptBase
{
    private readonly IEffectFactory EffectFactory;
    private readonly IIntervalTimer InvulnerableTimer;
    private readonly IMonsterFactory MonsterFactory;
    private readonly IIntervalTimer RandomAbilityTimer;
    private readonly IIntervalTimer RegenerateTimer;
    private readonly ISpellFactory SpellFactory;
    private readonly IIntervalTimer SpellTimer;
    private readonly Spell SpellToCast;
    private readonly Spell SpellToCast2;
    private readonly Spell SpellToCast3;

    private Animation UpgradeAnimation { get; } = new()
    {
        AnimationSpeed = 100,
        TargetAnimation = 189
    };

    /// <inheritdoc />
    public CountEnrageScript(
        Monster subject,
        IMonsterFactory monsterFactory,
        ISpellFactory spellFactory,
        IEffectFactory effectFactory)
        : base(subject)
    {
        SpellFactory = spellFactory;
        EffectFactory = effectFactory;
        SpellToCast = SpellFactory.Create("grasp");
        SpellToCast2 = SpellFactory.Create("ghastlypain");
        SpellToCast3 = SpellFactory.Create("ardcradh");
        MonsterFactory = monsterFactory;
        RegenerateTimer = new IntervalTimer(TimeSpan.FromSeconds(6), false);
        InvulnerableTimer = new IntervalTimer(TimeSpan.FromSeconds(1), false);

        RandomAbilityTimer = new RandomizedIntervalTimer(
            TimeSpan.FromSeconds(45),
            20,
            RandomizationType.Balanced,
            false);

        SpellTimer = new RandomizedIntervalTimer(
            TimeSpan.FromSeconds(10),
            50,
            RandomizationType.Balanced,
            false);
    }

    private void CastASpell()
    {
        var random = IntegerRandomizer.RollSingle(101);

        var randomAisling = Subject.MapInstance
                                   .GetEntities<Aisling>()
                                   .Where(x => x.IsAlive && !x.IsGodModeEnabled())
                                   .ToList();

        if (randomAisling.Count == 0)
            return;

        var pickedAisling = randomAisling.PickRandom();

        if (random < 33)
        {
            Subject.TryUseSpell(SpellToCast, pickedAisling.Id);

            return;
        }

        if (random < 66)
        {
            foreach (var player in randomAisling)
            {
                Subject.TryUseSpell(SpellToCast, player.Id);

                return;
            }   
        }

        if (random < 101)
        {
            Subject.TryUseSpell(SpellToCast3, pickedAisling.Id);
            return;
        }
    }

    private void RegenerateFromBats()
    {
        var amountBats = Subject.MapInstance
                                .GetEntities<Monster>()
                                .Count(x => x.Name == "Macabre Bat");

        if (amountBats > 0)
        {
            var healamt = amountBats * 0.002;
            var amountToHeal = Subject.StatSheet.EffectiveMaximumHp * healamt;

            var newHp = Subject.StatSheet.CurrentHp + amountToHeal;

            if (Subject.StatSheet.MaximumHp < newHp)
            {
                Subject.StatSheet.SetHp(Subject.StatSheet.MaximumHp);

                return;
            }

            Subject.StatSheet.SetHp((int)newHp);
        }
    }

    public override void Update(TimeSpan delta)
    {
        RandomAbilityTimer.Update(delta);
        RegenerateTimer.Update(delta);
        SpellTimer.Update(delta);
        InvulnerableTimer.Update(delta);

        if (InvulnerableTimer.IntervalElapsed)
        {
            var countMonster = Subject.MapInstance
                                      .GetEntitiesWithinRange<Monster>(Subject, 12) // Search within a range of 12
                                      .FirstOrDefault(x => x.Name == "Countess"); // Look for the monster named "Count"

            // If the Count is found, apply the invulnerable effect
            if (countMonster != null)
            {
                if (!countMonster.Effects.Contains("invulnerability"))
                {
                    var invulnerable = EffectFactory.Create("invulnerability");
                    Subject.Effects.Apply(Subject, invulnerable);
                }
            } else
                Subject.Effects.Terminate("invulnerability");
        }

        if (SpellTimer.IntervalElapsed)
            CastASpell();

        if (RegenerateTimer.IntervalElapsed)
            RegenerateFromBats();

        if (RandomAbilityTimer.IntervalElapsed)
        {
            var random = IntegerRandomizer.RollSingle(101);

            if (random < 50)
            {
                var batCount = Subject.MapInstance
                                      .GetEntities<Monster>()
                                      .Count(x => x.Name == "Macabre Bat");

                if (batCount > 5)
                    return;

                Subject.Say("There can never be enough bats!");

                var rectangle = new Rectangle(Subject, 12, 12);

                for (var i = 0; i <= 7; i++)
                {
                    if (!rectangle.TryGetRandomPoint(x => Subject.MapInstance.IsWalkable(x, Subject.Type), out var point))
                        continue;

                    var mobs = MonsterFactory.Create("count_bat", Subject.MapInstance, point);
                    Subject.MapInstance.AddEntity(mobs, point);

                    return;
                }
            }

            if (random < 101)
            {
                Subject.Say("How about we rearrange the room a little bit?");

                foreach (var target in Subject.MapInstance
                                              .GetEntities<Aisling>()
                                              .Where(x => !x.IsGodModeEnabled())
                                              .ToList())
                {
                    if (target.IsDead)
                        continue;

                    var point = target.MapInstance.TryGetRandomWalkablePoint(out var walkablePoint, CreatureType.Aisling);

                    if (walkablePoint != null)
                        target.WarpTo(walkablePoint);
                }
            }
        }
    }
}
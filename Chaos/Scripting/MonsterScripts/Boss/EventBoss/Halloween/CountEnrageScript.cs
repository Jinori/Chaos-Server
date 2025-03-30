using Chaos.Common.Definitions;
using Chaos.Common.Utilities;
using Chaos.DarkAges.Definitions;
using Chaos.Extensions;
using Chaos.Extensions.Geometry;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Scripting.MonsterScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Boss.EventBoss.Halloween;

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
            TimeSpan.FromSeconds(80),
            20,
            RandomizationType.Balanced,
            false);

        SpellTimer = new RandomizedIntervalTimer(
            TimeSpan.FromSeconds(8),
            20,
            RandomizationType.Balanced,
            false);
    }

    private void CastASpell()
    {
        var random = IntegerRandomizer.RollSingle(101);

        var randomAisling = Subject.MapInstance
                                   .GetEntities<Aisling>()
                                   .Where(x => x.MapInstance.IsWithinMap(Subject))
                                   .ToList();

        if (randomAisling.Count == 0)
            return;

        var pickedAisling = randomAisling.PickRandom();

        if (random < 20)
        {
            Subject.TryUseSpell(SpellToCast, pickedAisling.Id);

            return;
        }

        if (random < 70)
        {
            foreach (var player in randomAisling)
                Subject.TryUseSpell(SpellToCast2, player.Id);

            return;
        }

        if (random < 101)
            Subject.TryUseSpell(SpellToCast3, pickedAisling.Id);
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

                if (batCount > 2)
                    return;

                Subject.Say("There can never be enough bats!");

                var rectangle = new Rectangle(Subject, 12, 12);

                var mobsSpawned = batCount;

                // Continue spawning until 5 mobs are spawned
                while (mobsSpawned < 5)
                    if (rectangle.TryGetRandomPoint(x => Subject.MapInstance.IsWalkable(x, collisionType: Subject.Type), out var point))
                    {
                        var mob = MonsterFactory.Create("count_bat", Subject.MapInstance, point);
                        Subject.MapInstance.AddEntity(mob, point);
                        mobsSpawned++; // Increment count when a mob is successfully spawned
                    }

                return;
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
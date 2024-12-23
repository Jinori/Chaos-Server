using Chaos.Common.Definitions;
using Chaos.Common.Utilities;
using Chaos.Extensions;
using Chaos.Extensions.Geometry;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Scripting.MonsterScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Boss.EventBoss.Halloween;

public sealed class CountessEnrageScript : MonsterScriptBase
{
    private readonly IEffectFactory EffectFactory;
    private readonly IMonsterFactory MonsterFactory;
    private readonly IIntervalTimer RandomAbilityTimer;
    private readonly IIntervalTimer RegenerateTimer;
    private readonly ISpellFactory SpellFactory;
    private readonly IIntervalTimer SpellTimer;
    private readonly Spell SpellToCast;
    private readonly Spell SpellToCast2;
    private readonly Spell SpellToCast3;

    /// <inheritdoc />
    public CountessEnrageScript(
        Monster subject,
        IMonsterFactory monsterFactory,
        ISpellFactory spellFactory,
        IEffectFactory effectFactory)
        : base(subject)
    {
        SpellFactory = spellFactory;
        EffectFactory = effectFactory;
        SpellToCast = SpellFactory.Create("bind");
        SpellToCast2 = SpellFactory.Create("chainlightning");
        SpellToCast3 = SpellFactory.Create("ardcradh");
        MonsterFactory = monsterFactory;
        RegenerateTimer = new IntervalTimer(TimeSpan.FromSeconds(6), false);

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

        if (random < 33)
        {
            Subject.TryUseSpell(SpellToCast, pickedAisling.Id);

            return;
        }

        if (random < 66)
        {
            Subject.TryUseSpell(SpellToCast2, pickedAisling.Id);

            return;
        }

        if (random < 100)
            Subject.TryUseSpell(SpellToCast3, pickedAisling.Id);
    }

    private void RegenerateFromDolls()
    {
        var amountdolls = Subject.MapInstance
                                 .GetEntities<Monster>()
                                 .Count(x => x.Name == "Macabre Doll");

        if (amountdolls > 0)
        {
            var healamt = amountdolls * 0.002;
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

        if (SpellTimer.IntervalElapsed)
            CastASpell();

        if (RegenerateTimer.IntervalElapsed)
            RegenerateFromDolls();

        if (RandomAbilityTimer.IntervalElapsed)
        {
            var random = IntegerRandomizer.RollSingle(101);

            if (random < 50)
            {
                var dollCount = Subject.MapInstance
                                       .GetEntities<Monster>()
                                       .Count(x => x.Name == "Macabre Doll");

                if (dollCount > 2)
                    return;

                Subject.Say("Dolls! Come play...");

                var rectangle = new Rectangle(Subject, 12, 12);

                var mobsSpawned = dollCount;

                // Continue spawning until 5 mobs are spawned
                while (mobsSpawned < 5)
                    if (rectangle.TryGetRandomPoint(x => Subject.MapInstance.IsWalkable(x, Subject.Type), out var point))
                    {
                        var mob = MonsterFactory.Create("countess_doll", Subject.MapInstance, point);
                        Subject.MapInstance.AddEntity(mob, point);
                        mobsSpawned++; // Increment count when a mob is successfully spawned
                    }

                return;
            }

            if (random < 100)
            {
                Subject.Say("There's no where to go. Stay awhile.");

                foreach (var target in Subject.MapInstance
                                              .GetEntities<Aisling>()
                                              .Where(x => !x.IsGodModeEnabled())
                                              .ToList())
                {
                    if (target.IsDead)
                        continue;

                    var root = EffectFactory.Create("root");
                    target.Effects.Apply(target, root);
                }
            }
        }
    }
}
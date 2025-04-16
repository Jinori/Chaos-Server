using Chaos.Common.Definitions;
using Chaos.Common.Utilities;
using Chaos.Extensions;
using Chaos.Extensions.Geometry;
using Chaos.Geometry.Abstractions;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Scripting.MonsterScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Boss.EventBoss.Easter.MrHopps98;

public sealed class MrHopps98EnrageScript : MonsterScriptBase
{
    private const int MAX_BOUNCES_PER_PHASE = 5;
    private readonly IIntervalTimer BounceTimer;
    private readonly IEffectFactory EffectFactory;
    private readonly IMonsterFactory MonsterFactory;
    private readonly IIntervalTimer RandomAbilityTimer;
    private readonly IIntervalTimer RegenerateTimer;
    private readonly ISpellFactory SpellFactory;
    private readonly IIntervalTimer SpellTimer;
    private readonly Spell SpellToCast;
    private readonly Spell SpellToCast2;
    private readonly Spell SpellToCast3;
    private bool BouncePhase;
    private int BouncesRemaining;

    /// <inheritdoc />
    public MrHopps98EnrageScript(
        Monster subject,
        IMonsterFactory monsterFactory,
        ISpellFactory spellFactory,
        IEffectFactory effectFactory)
        : base(subject)
    {
        SpellFactory = spellFactory;
        EffectFactory = effectFactory;
        SpellToCast = SpellFactory.Create("bunnypounce98");
        SpellToCast2 = SpellFactory.Create("bunnyseismicshift98");
        SpellToCast3 = SpellFactory.Create("bunnychainlightning98");
        MonsterFactory = monsterFactory;
        RegenerateTimer = new IntervalTimer(TimeSpan.FromSeconds(6), false);

        RandomAbilityTimer = new RandomizedIntervalTimer(
            TimeSpan.FromSeconds(80),
            20,
            RandomizationType.Balanced,
            false);

        SpellTimer = new RandomizedIntervalTimer(
            TimeSpan.FromSeconds(6),
            20,
            RandomizationType.Balanced,
            false);

        BounceTimer = new RandomizedIntervalTimer(
            TimeSpan.FromSeconds(4),
            10,
            RandomizationType.Balanced,
            false);
    }

    private void CastASpell()
    {
        var random = IntegerRandomizer.RollSingle(101);

        var randomAisling = Subject.MapInstance
                                   .GetEntitiesWithinRange<Aisling>(Subject)
                                   .ThatAreObservedBy(Subject)
                                   .ThatAreVisibleTo(Subject)
                                   .Where(x => !x.IsDead && !x.IsGodModeEnabled())
                                   .ToList();

        if (randomAisling.Count == 0)
            return;

        var pickedAisling = randomAisling.PickRandom();

        switch (random)
        {
            case < 50:
                Subject.TryUseSpell(SpellToCast2);

                return;
            case < 101:
                Subject.TryUseSpell(SpellToCast3, pickedAisling.Id);

                break;
        }
    }

    private void DoBounce()
    {
        if (--BouncesRemaining < 0) // already finished?  Bail.
            return;

        var eligible = Subject.MapInstance
                              .GetEntities<Aisling>()
                              .ThatAreVisibleTo(Subject)
                              .ThatAreObservedBy(Subject)
                              .Where(a => !a.IsDead && !a.IsGodModeEnabled())
                              .ToList();

        if (eligible.Count == 0)
            return;

        // Shuffle and keep ≤5 distinct victims
        var rnd = new Random();

        var chosenTargets = eligible.OrderBy(_ => rnd.Next())
                                    .Take(Math.Min(5, eligible.Count))
                                    .ToList();

        foreach (var victim in chosenTargets)
        {
            var point = victim.GenerateCardinalPoints()
                              .OfType<IPoint>()
                              .FirstOrDefault(x => Subject.MapInstance.IsWalkable(x, collisionType: Subject.Type));

            if (point is null)
                continue;

            // 2) Warp, face, and blast
            Subject.WarpTo(point);
            Subject.Turn(victim.DirectionalRelationTo(Subject));

            // Cast the attack spell once
            Subject.TryUseSpell(SpellToCast); // replace with real spell key

            if (BouncesRemaining == 0)
                BouncePhase = false;
        }
    }

    private void RegenerateFromBats()
    {
        var amountBats = Subject.MapInstance
                                .GetEntities<Monster>()
                                .Count(x => x.Name == "Field Floppy");

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
        if (!Map.GetEntities<Aisling>()
                .Any())
            return;

        RandomAbilityTimer.Update(delta);
        RegenerateTimer.Update(delta);
        SpellTimer.Update(delta);

        if (BouncePhase)
        {
            BounceTimer.Update(delta);

            if (BounceTimer.IntervalElapsed)
                DoBounce();

            return;
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
                var floppyCount = Subject.MapInstance
                                         .GetEntities<Monster>()
                                         .ThatAreWithinRange(Subject)
                                         .Count(x => x.Name == "Field Floppy");

                if (floppyCount > 5)
                    return;

                Subject.Say("Aid me young floppies!");

                var rectangle = new Rectangle(Subject, 12, 12);

                var mobsSpawned = floppyCount;

                // Continue spawning until 5 mobs are spawned
                while (mobsSpawned < 5)
                    if (rectangle.TryGetRandomPoint(x => Subject.MapInstance.IsWalkable(x, collisionType: Subject.Type), out var point))
                    {
                        var mob = MonsterFactory.Create("fieldfloppy98", Subject.MapInstance, point);
                        Subject.MapInstance.AddEntity(mob, point);
                        mobsSpawned++; // Increment count when a mob is successfully spawned
                    }
            } else
            {
                Subject.Say("Now you are making me angry Aisling...");
                BouncePhase = true;
                BouncesRemaining = MAX_BOUNCES_PER_PHASE;
                BounceTimer.Reset();
                DoBounce();
            }
        }
    }
}
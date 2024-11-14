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

namespace Chaos.Scripting.MonsterScripts.Boss.EventBoss.Thanksgiving;

public sealed class TurkeyEnrageScript : MonsterScriptBase
{
    private readonly IIntervalTimer InvulnerableTimer;
    private readonly IMonsterFactory MonsterFactory;
    private readonly IIntervalTimer RandomAbilityTimer;
    private readonly IIntervalTimer RegenerateTimer;
    private readonly ISpellFactory SpellFactory;
    private readonly IIntervalTimer SpellTimer;
    private readonly Spell SpellToCast;
    private readonly Spell SpellToCast2;

    /// <inheritdoc />
    public TurkeyEnrageScript(Monster subject, IMonsterFactory monsterFactory, ISpellFactory spellFactory)
        : base(subject)
    {
        SpellFactory = spellFactory;
        SpellToCast = SpellFactory.Create("grasp");
        SpellToCast2 = SpellFactory.Create("ardcradh");
        MonsterFactory = monsterFactory;
        RegenerateTimer = new IntervalTimer(TimeSpan.FromSeconds(6), false);
        InvulnerableTimer = new IntervalTimer(TimeSpan.FromSeconds(1), false);

        RandomAbilityTimer = new RandomizedIntervalTimer(
            TimeSpan.FromSeconds(100),
            50,
            RandomizationType.Balanced,
            false);

        SpellTimer = new RandomizedIntervalTimer(
            TimeSpan.FromSeconds(4),
            20,
            RandomizationType.Balanced,
            false);
    }

    private void CastASpell()
    {
        var random = IntegerRandomizer.RollSingle(101);

        var randomAisling = Subject.MapInstance
                                   .GetEntities<Aisling>()
                                   .Where(
                                       x => x.MapInstance
                                             .GetEntitiesWithinRange<Aisling>(Subject)
                                             .Any(a => !a.IsGodModeEnabled()))
                                   .ToList();

        if (randomAisling.Count == 0)
            return;

        var pickedAisling = randomAisling.PickRandom();

        if (random < 50)
        {
            Subject.TryUseSpell(SpellToCast, pickedAisling.Id);

            return;
        }

        if (random < 101)
            foreach (var player in randomAisling)
                Subject.TryUseSpell(SpellToCast2, player.Id);
    }

    private void RegenerateFromTurkeys()
    {
        var amountTurkeys = Subject.MapInstance
                                   .GetEntitiesWithinRange<Monster>(Subject)
                                   .Count(x => x.Name == "Turkey");

        if (amountTurkeys > 0)
        {
            var healamt = amountTurkeys * 0.001;
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

        if (SpellTimer.IntervalElapsed)
            CastASpell();

        if (RegenerateTimer.IntervalElapsed)
            RegenerateFromTurkeys();

        if (RandomAbilityTimer.IntervalElapsed)
        {
            var turkeyCount = Subject.MapInstance
                                     .GetEntities<Monster>()
                                     .Count(x => x.Name == "Turkey");

            if (turkeyCount > 2)
                return;

            Subject.Say("Gobble Gobble!");

            var rectangle = new Rectangle(Subject, 12, 12);

            var mobsSpawned = turkeyCount;

            // Continue spawning until 5 mobs are spawned
            while (mobsSpawned < 4)
                if (rectangle.TryGetRandomPoint(x => Subject.MapInstance.IsWalkable(x, Subject.Type), out var point))
                {
                    var mob = MonsterFactory.Create("tg_turkey", Subject.MapInstance, point);
                    Subject.MapInstance.AddEntity(mob, point);
                    mobsSpawned++; // Increment count when a mob is successfully spawned
                }
        }
    }
}
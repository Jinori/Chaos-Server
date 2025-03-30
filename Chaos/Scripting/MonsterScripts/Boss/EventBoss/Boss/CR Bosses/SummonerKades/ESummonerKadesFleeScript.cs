using Chaos.DarkAges.Definitions;
using Chaos.Extensions.Geometry;
using Chaos.Models.World;
using Chaos.Scripting.MonsterScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Boss.EventBoss.Boss.CR_Bosses.SummonerKades;

public sealed class ESummonerKadesFleeScript : MonsterScriptBase
{
    private readonly IEffectFactory EffectFactory;
    private readonly IIntervalTimer InvulnerableTimer;
    private readonly IMonsterFactory MonsterFactory;
    private readonly IIntervalTimer SummonTimer;
    private readonly IIntervalTimer WalkTimer;
    private bool BossVulnerable;
    private bool HitFirstHp;
    private bool HitFourthHp;
    private bool HitSecondHp;
    private bool HitThirdHp;

    /// <inheritdoc />
    public ESummonerKadesFleeScript(Monster subject, IMonsterFactory monsterFactory, IEffectFactory effectFactory)
        : base(subject)
    {
        MonsterFactory = monsterFactory;
        EffectFactory = effectFactory;
        InvulnerableTimer = new IntervalTimer(TimeSpan.FromSeconds(1), false);
        WalkTimer = new IntervalTimer(TimeSpan.FromSeconds(1), false);
        SummonTimer = new IntervalTimer(TimeSpan.FromSeconds(30), false);
    }

    // Make boss invulnerable
    private void MakeBossInvulnerable()
    {
        var invulnerability = EffectFactory.Create("invulnerability");
        Subject.Effects.Apply(Subject, invulnerability);
        BossVulnerable = false;
    }

    // Make boss vulnerable
    private void MakeBossVulnerable()
    {
        Subject.Effects.Terminate("invulnerability");
        BossVulnerable = true;
    }

    // Spawn a pair of monsters for the stage
    private void SpawnMonsters(string monsterType)
    {
        var rectangle = new Rectangle(
            Subject.X,
            Subject.Y,
            7,
            7);
        rectangle.TryGetRandomPoint(x => Subject.MapInstance.IsWalkable(x, CreatureType.Normal), out var point1);
        rectangle.TryGetRandomPoint(x => Subject.MapInstance.IsWalkable(x, CreatureType.Normal), out var point2);

        if ((point1 != null) && (point2 != null))
        {
            var monster1 = MonsterFactory.Create(monsterType, Subject.MapInstance, point1.Value);
            var monster2 = MonsterFactory.Create(monsterType, Subject.MapInstance, point2.Value);
            Subject.MapInstance.AddEntity(monster1, point1.Value);
            Subject.MapInstance.AddEntity(monster2, point2.Value);
        }
    }

    // Trigger final stage with continuous summoning
    private void TriggerFinalStage(TimeSpan delta)
    {
        SummonTimer.Update(delta);

        // Summon one of each guardian every 30 seconds
        if (SummonTimer.IntervalElapsed)
        {
            Subject.Say("Summons! Attack!");
            SpawnMonsters("ignis_guardian");
            SpawnMonsters("gale_guardian");
            SpawnMonsters("terra_guardian");
            SpawnMonsters("tide_guardian");
        }

        if ((Subject.StatSheet.CurrentHp < 200000) && BossVulnerable)
        {
            // Get the first available guardian monster
            var guardian = Subject.MapInstance
                                  .GetEntities<Monster>()
                                  .FirstOrDefault(x => x.Template.TemplateKey.Contains("guardian"));

            if (guardian != null)
            {
                // Heal the boss for 15% of its maximum health
                var healAmount = (int)(Subject.StatSheet.MaximumHp * 0.08);
                Subject.StatSheet.AddHp(healAmount);

                // Say a message to inform players
                Subject.Say("I consume the power of my guardian!");

                // Remove the consumed guardian from the map
                Subject.MapInstance.RemoveEntity(guardian);
            }
        }
    }

    // Trigger a stage with specified guardian type and message
    private void TriggerStage(string guardianType, string message)
    {
        Subject.Say(message);
        SpawnMonsters(guardianType);
    }

    public override void Update(TimeSpan delta)
    {
        WalkTimer.Update(delta);
        InvulnerableTimer.Update(delta);

        if (InvulnerableTimer.IntervalElapsed)
        {
            var guardiansPresent = Subject.MapInstance
                                          .GetEntities<Monster>()
                                          .Any(x => x.Template.TemplateKey.Contains("guardian"));

            if (!BossVulnerable && !guardiansPresent)

                // Reset invulnerability when all guardian monsters are killed
                MakeBossVulnerable();

            if (BossVulnerable && guardiansPresent)

                // Make boss Invulnerable when Guardians are present.
                MakeBossInvulnerable();
        }

        // Check if the boss reaches specific HP thresholds and trigger corresponding phases
        if ((Subject.StatSheet.CurrentHp <= 1508320) && !HitFirstHp)
        {
            HitFirstHp = true;
            TriggerStage("terra_guardian", "You will see my true power.");
        } else if ((Subject.StatSheet.CurrentHp <= 1131240) && !HitSecondHp && BossVulnerable)
        {
            HitSecondHp = true;
            TriggerStage("gale_guardian", "Slice them to pieces.");
        } else if ((Subject.StatSheet.CurrentHp <= 754160) && !HitThirdHp && BossVulnerable)
        {
            HitThirdHp = true;
            TriggerStage("tide_guardian", "Drown them all.");
        } else if ((Subject.StatSheet.CurrentHp <= 377080) && !HitFourthHp && BossVulnerable)
        {
            HitFourthHp = true;
            TriggerStage("ignis_guardian", "Handle this minions!");
        } else if ((Subject.StatSheet.CurrentHp <= 300000) && HitFourthHp)
            TriggerFinalStage(delta);
    }
}
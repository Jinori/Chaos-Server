using Chaos.Extensions.Geometry;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Scripting.MonsterScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Boss.DragonScaleBoss;

public class dragonScaleMoveToTargetScript : MonsterScriptBase
{
    public readonly Location FightSpot = new("wilderness", 18, 16);
    private readonly Spell SpellToCast;
    private readonly IIntervalTimer TimeToGetToSpot;
    private readonly IIntervalTimer WalkTimer;

    private bool ReachedPoint;

    /// <inheritdoc />
    public dragonScaleMoveToTargetScript(Monster subject, ISpellFactory spellFactory)
        : base(subject)
    {
        SpellToCast = spellFactory.Create("fireblast");
        TimeToGetToSpot = new IntervalTimer(TimeSpan.FromSeconds(15), false);
        WalkTimer = new IntervalTimer(TimeSpan.FromMilliseconds(300), false);
    }

    private void ResetAttackTimerIfMoved()
    {
        var now = DateTime.UtcNow;
        var lastWalk = Subject.Trackers.LastWalk;
        var lastTurn = Subject.Trackers.LastTurn;

        var walkedRecently = lastWalk.HasValue
                             && (now.Subtract(lastWalk.Value)
                                    .TotalMilliseconds
                                 < Subject.Template.MoveIntervalMs);

        var turnedRecently = lastTurn.HasValue
                             && (now.Subtract(lastTurn.Value)
                                    .TotalMilliseconds
                                 < Subject.Template.MoveIntervalMs);

        if (walkedRecently || turnedRecently)
        {
            Subject.WanderTimer.Reset();
            Subject.SkillTimer.Reset();
        }
    }

    /// <inheritdoc />
    public override void Update(TimeSpan delta)
    {
        base.Update(delta);

        if ((Subject.ManhattanDistanceFrom(FightSpot) > 0) && !ReachedPoint)
        {
            TimeToGetToSpot.Update(delta);
            WalkTimer.Update(delta);
            Subject.StatSheet.SetHealthPct(100);

            if (WalkTimer.IntervalElapsed)
                Subject.Pathfind(FightSpot);

            if (TimeToGetToSpot.IntervalElapsed)
                Subject.MapInstance.RemoveEntity(Subject);

            if (Subject.ManhattanDistanceFrom(FightSpot) <= 1)
            {
                var rockFish = Subject.MapInstance
                                      .GetEntitiesAtPoints<GroundItem>(FightSpot)
                                      .FirstOrDefault(x => x.Name == "Lion Fish");

                if (rockFish != null)
                {
                    Subject.Say("DEATH TO ALL WHO DISTURB MY MEAL.");
                    Subject.TryUseSpell(SpellToCast);
                    Map.RemoveEntity(rockFish);
                    ReachedPoint = true;
                }

                if (rockFish == null)
                    Map.RemoveEntity(Subject);
            }

            return;
        }

        if ((Target == null) || !ShouldMove)
            return;

        if (!Map.GetEntities<Aisling>()
                .Any())
            return;

        var distance = Subject.ManhattanDistanceFrom(Target);

        switch (distance)
        {
            case > 1:
                Subject.Pathfind(Target);

                break;
            case 1:
            {
                var direction = Target.DirectionalRelationTo(Subject);
                Subject.Turn(direction);

                break;
            }
            case 0:
            {
                Subject.Wander();

                break;
            }
        }

        ResetAttackTimerIfMoved();
    }
}
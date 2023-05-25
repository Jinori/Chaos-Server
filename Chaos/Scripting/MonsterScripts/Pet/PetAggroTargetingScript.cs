using Chaos.Extensions;
using Chaos.Extensions.Geometry;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Networking.Abstractions;
using Chaos.Scripting.MonsterScripts.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Pet;

// ReSharper disable once ClassCanBeSealed.Global
public class PetAggroTargetingScript : MonsterScriptBase
{
    private readonly IClientRegistry<IWorldClient> ClientRegistry;
    private readonly IIntervalTimer TargetUpdateTimer;
    private int InitialAggro = 10;

    /// <inheritdoc />
    public PetAggroTargetingScript(Monster subject, IClientRegistry<IWorldClient> clientRegistry)
        : base(subject)
    {
        TargetUpdateTimer =
            new IntervalTimer(TimeSpan.FromMilliseconds(Math.Min(250, Subject.Template.SkillIntervalMs)));
        ClientRegistry = clientRegistry;
    }

    /// <inheritdoc />
    public override void OnAttacked(Creature source, int damage, int? aggroOverride)
    {
        if (source.Equals(Subject) || source is Aisling)
            return;

        var aggro = aggroOverride ?? damage;

        if (aggro == 0)
            return;

        AggroList.AddOrUpdate(source.Id, _ => aggro, (_, currentAggro) => currentAggro + aggro);
    }

    /// <inheritdoc />
    public override void Update(TimeSpan delta)
    {
        base.Update(delta);

        TargetUpdateTimer.Update(delta);

        if ((Target != null) && (!Target.IsAlive || !Target.OnSameMapAs(Subject)))
        {
            AggroList.Remove(Target.Id, out _);
            Target = null;
        }

        if (!TargetUpdateTimer.IntervalElapsed)
            return;

        Target = null;

        var substringEndIndex = Subject.Name.IndexOf("'s Gloop", StringComparison.Ordinal);
        var substring = Subject.Name[..substringEndIndex];

        var player = ClientRegistry.Where(x => x.Aisling.IsAlive && (x.Aisling.Name == substring))
                                   .Select(x => x.Aisling)
                                   .FirstOrDefault();

        if (player is not null)
        {
            var target = Map.GetEntitiesWithinRange<Monster>(player)
                            .Where(x => x.IsAlive && x.AggroList.ContainsKey(player.Id));

            foreach (var kvp in target)
            {
                if (!Map.TryGetObject<Monster>(kvp.Id, out var possibleTarget))
                    continue;
                
                if (!possibleTarget.IsAlive || !Subject.CanObserve(possibleTarget) || !possibleTarget.WithinRange(Subject))
                    continue;

                if (possibleTarget.DistanceFrom(player) > 5)
                    continue;

                Target = possibleTarget;
                break;
            }
        }

        if (Target != null)
            return;

        //if we failed to get a target via aggroList, grab the closest aisling within aggro range
        Target ??= Map.GetEntitiesWithinRange<Monster>(Subject, AggroRange)
                      .ThatAreObservedBy(Subject)
                      .Where(
                          obj => !obj.Equals(Subject)
                                 && obj.IsAlive
                                 && Subject.ApproachTime.TryGetValue(obj.Id, out var time)
                                 && ((DateTime.UtcNow - time).TotalSeconds >= 1.5))
                      .ClosestOrDefault(Subject);

        //since we grabbed a new target, give them some initial aggro so we stick to them
        if (Target != null)
            AggroList[Target.Id] = InitialAggro++;
    }
}
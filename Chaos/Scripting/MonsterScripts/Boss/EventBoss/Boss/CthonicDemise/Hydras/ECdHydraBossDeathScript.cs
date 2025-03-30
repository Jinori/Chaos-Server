using Chaos.Common.Definitions;
using Chaos.DarkAges.Definitions;
using Chaos.Extensions;
using Chaos.Extensions.Geometry;
using Chaos.Models.World;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ExperienceDistribution;
using Chaos.Scripting.MonsterScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Services.Servers.Options;

namespace Chaos.Scripting.MonsterScripts.Boss.EventBoss.Boss.CthonicDemise.Hydras;

// ReSharper disable once ClassCanBeSealed.Global
public class ECdHydraBossDeathScript : MonsterScriptBase
{
    protected IExperienceDistributionScript ExperienceDistributionScript { get; set; }
    private readonly IMonsterFactory MonsterFactory;

    /// <inheritdoc />
    public ECdHydraBossDeathScript(Monster subject, IMonsterFactory monsterFactory)
        : base(subject)
    {
        MonsterFactory = monsterFactory;
        ExperienceDistributionScript = DefaultExperienceDistributionScript.Create();
    }

    /// <inheritdoc />
    public override void OnDeath()
    {
        if (!Map.RemoveEntity(Subject))
            return;

        var anyaisling = Subject.MapInstance.GetEntities<Aisling>()
            .FirstOrDefault(x => x.MapInstance == Subject.MapInstance);
        if (anyaisling != null)
        {
            var rectangle = new Rectangle(anyaisling, 3, 3);
            Point point2;
            do
            {
                point2 = rectangle.GetRandomPoint();
            } while (!Subject.MapInstance.IsWalkable(point2, collisionType: CreatureType.Normal));
            
            Point point3;
            do
            {
                point3 = rectangle.GetRandomPoint();
            } while (!Subject.MapInstance.IsWalkable(point3, collisionType: CreatureType.Normal));

            if (Subject.Template.TemplateKey == "cthonic_hydra")
            {
                var point = new Point(Subject.X, Subject.Y);
                var nextHydra = MonsterFactory.Create("cthonic_hydra2", Subject.MapInstance, point);
                Subject.MapInstance.AddEntity(nextHydra, point);
                
                var nextHydra2 = MonsterFactory.Create("cthonic_hydra3", Subject.MapInstance, point2);
                Subject.MapInstance.AddEntity(nextHydra2, point2);
            }

            if (Subject.Template.TemplateKey == "cthonic_hydra2")
            {
                var point = new Point(Subject.X, Subject.Y);
                var nextHydra2 = MonsterFactory.Create("cthonic_hydra3", Subject.MapInstance, point);
                Subject.MapInstance.AddEntity(nextHydra2, point);
                
                var nextHydra3 = MonsterFactory.Create("cthonic_hydra4", Subject.MapInstance, point2);
                Subject.MapInstance.AddEntity(nextHydra3, point2);
            }
        }

        //this code will set the reward target to the person at the top of the aggro list
        //var rewardTarget = Subject.AggroList
        //                          .OrderByDescending(kvp => kvp.Value)
        //                          .Select(kvp => Map.TryGetEntity<Aisling>(kvp.Key, out var a) ? a : null)
        //                          .FirstOrDefault(a => a is not null);

        //get the highest contributor
        //if there are no contributor, try getting the highest aggro
        var rewardTarget = Subject.Contribution
            .OrderByDescending(kvp => kvp.Value)
            .Select(kvp => Map.TryGetEntity<Aisling>(kvp.Key, out var a) ? a : null)
            .FirstOrDefault(a => a is not null);

        Aisling[]? rewardTargets = null;

        if (rewardTarget != null)
            rewardTargets = (rewardTarget.Group ?? (IEnumerable<Aisling>)new[] { rewardTarget })
                .ThatAreWithinRange(rewardTarget)
                .ToArray();

        Subject.Items.AddRange(Subject.LootTable.GenerateLoot());

        var droppedGold = Subject.TryDropGold(Subject, Subject.Gold, out var money);
        var droppedITems = Subject.TryDrop(Subject, Subject.Items, out var groundItems);

        if (rewardTargets is not null)
        {
            if (WorldOptions.Instance.LootDropsLockToRewardTargetSecs.HasValue)
            {
                var lockSecs = WorldOptions.Instance.LootDropsLockToRewardTargetSecs.Value;

                if (droppedGold)
                    money!.LockToAislings(lockSecs, rewardTargets);

                if (droppedITems)
                    foreach (var groundItem in groundItems!)
                        groundItem.LockToAislings(lockSecs, rewardTargets);
            }

            ExperienceDistributionScript.DistributeExperience(Subject, rewardTargets);
        }
    }
}
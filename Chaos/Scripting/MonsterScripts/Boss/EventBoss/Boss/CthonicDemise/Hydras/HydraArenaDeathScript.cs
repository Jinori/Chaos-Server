using Chaos.Common.Utilities;
using Chaos.DarkAges.Definitions;
using Chaos.Definitions;
using Chaos.Extensions.Geometry;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Scripting.MonsterScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Boss.EventBoss.Boss.CthonicDemise.Hydras;

public class HydraArenaDeathScript : MonsterScriptBase
{
    private readonly Animation DeathAnimation = new()
    {
        SourceAnimation = 82,
        AnimationSpeed = 100
    };

    private readonly IMonsterFactory MonsterFactory;

    /// <inheritdoc />
    public HydraArenaDeathScript(Monster subject, IMonsterFactory monsterFactory)
        : base(subject)
        => MonsterFactory = monsterFactory;

    /// <inheritdoc />
    public override void OnDeath()
    {
        if (!Map.RemoveEntity(Subject))
            return;

        var anyaisling = Subject.MapInstance
                                .GetEntities<Aisling>()
                                .FirstOrDefault(x => x.MapInstance == Subject.MapInstance);

        if (anyaisling != null)
        {
            var rectangle = new Rectangle(anyaisling, 3, 3);
            Point point2;

            do
                point2 = rectangle.GetRandomPoint();
            while (!Subject.MapInstance.IsWalkable(point2, CreatureType.Normal));

            Point point3;

            do
                point3 = rectangle.GetRandomPoint();
            while (!Subject.MapInstance.IsWalkable(point3, CreatureType.Normal));

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

        Subject.Items.AddRange(Subject.LootTable.GenerateLoot());

        var playersOnArenaMap = Map.GetEntities<Aisling>()
                                   .Where(x => !x.Trackers.Enums.HasValue(ArenaHost.Host))
                                   .ToList();

        foreach (var item in Subject.Items)
        {
            if (item.Template.Name == "cdbell")
                continue;

            var randomMember = playersOnArenaMap.PickRandom();
            randomMember.GiveItemOrSendToBank(item);
            randomMember.SendOrangeBarMessage($"You received {item.DisplayName} from {Subject.Name}");
        }
    }
}
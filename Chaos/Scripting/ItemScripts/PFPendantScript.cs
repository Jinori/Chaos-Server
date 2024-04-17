using Chaos.Collections;
using Chaos.Common.Utilities;
using Chaos.Definitions;
using Chaos.Extensions.Common;
using Chaos.Extensions.Geometry;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.ItemScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.ItemScripts;

public class PFPendantScript : ItemScriptBase
{
    private readonly IMonsterFactory MonsterFactory;
    private readonly ISimpleCache SimpleCache;

    public PFPendantScript(Item subject, ISimpleCache simpleCache, IMonsterFactory monsterFactory)
        : base(subject)
    {
        SimpleCache = simpleCache;
        MonsterFactory = monsterFactory;
    }

    private bool CanSpawnMantis(Aisling source, out MapInstance mapInstance)
    {
        mapInstance = SimpleCache.Get<MapInstance>("PF_peak");
        source.Trackers.Enums.TryGetValue(out PFQuestStage stage);

        if (!source.IsAlive || !source.Inventory.Contains("Turuc Pendant") || !source.MapInstance.Name.EqualsI(mapInstance.Name))
            return false;

        var mantisIsSpawned = mapInstance.GetEntities<Monster>().Any(x => x.Template.TemplateKey.EqualsI("PF_giant_mantis"));

        if (mantisIsSpawned)
        {
            source.SendOrangeBarMessage("The Giant Mantis is already on the map.");

            return false;
        }

        return true;
    }

    private Monster CreateMantis(MapInstance mapInstance, Aisling source) =>
        MonsterFactory.Create("pf_giant_mantis", mapInstance, Point.From(source));

    private Animation GetMonsterAnimation() => new()
        { AnimationSpeed = 100, TargetAnimation = 97 };

    private Animation GetPlayerAnimation() => new()
        { AnimationSpeed = 100, TargetAnimation = 160 };

    private Point GetSpawnPoint(Creature mantis, MapEntity source)
    {
        var monsterSpawn = new Rectangle(source, 10, 10);
        var outline = monsterSpawn.GetOutline().ToList();
        Point point;

        do
            point = outline.PickRandom();
        while (!source.MapInstance.IsWalkable(point, mantis.Type));

        return point;
    }

    public override void OnUse(Aisling source)
    {
        if (!CanSpawnMantis(source, out var mapInstance))
            return;

        SpawnMantis(source, mapInstance);
    }

    private void SpawnMantis(Aisling source, MapInstance mapInstance)
    {
        var mantis = CreateMantis(mapInstance, source);
        var point = GetSpawnPoint(mantis, source);

        mantis.AggroRange = 12;
        mantis.Experience = 1000;
        mantis.SetLocation(point);

        source.MapInstance.AddEntity(mantis, point);
        mantis.Animate(GetMonsterAnimation());

        foreach (var player in mapInstance.GetEntities<Aisling>())
        {
            player.SendOrangeBarMessage("You feel the ground shake, a quick shadow overcasts you");
            player.Animate(GetPlayerAnimation());
        }
    }
}
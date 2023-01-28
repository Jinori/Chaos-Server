using Chaos.Common.Utilities;
using Chaos.Containers;
using Chaos.Data;
using Chaos.Definitions;
using Chaos.Extensions.Common;
using Chaos.Extensions.Geometry;
using Chaos.Objects.Panel;
using Chaos.Objects.World;
using Chaos.Scripts.ItemScripts.Abstractions;
using Chaos.Services.Factories;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripts.ItemScripts;

public class PFPendantScript : ItemScriptBase
{
    private readonly ISimpleCache SimpleCache;
    private readonly IMonsterFactory MonsterFactory;

    public PFPendantScript(Item subject, ISimpleCache simpleCache, IMonsterFactory monsterFactory)
        : base(subject)
    {
        SimpleCache = simpleCache;
        MonsterFactory = monsterFactory;
    }

    public override void OnUse(Aisling source)
    {
        var mapInstance = SimpleCache.Get<MapInstance>("PF_peak");
        var hasStage = source.Enums.TryGetValue(out PFQuestStage stage);

        if (!source.IsAlive || (stage != PFQuestStage.FoundPendant) || !source.MapInstance.Name.EqualsI(mapInstance.Name))
            return;
        
        var mantisisspawned = mapInstance.GetEntities<Monster>().Any(x => x.Template.TemplateKey.EqualsI("PF_giant_mantis"));

        if (mantisisspawned)
        {
            source.SendOrangeBarMessage("The Giant Mantis is already on the map.");

            return;
        }

        var animation = new Animation
        {
            AnimationSpeed = 100,
            TargetAnimation = 160
        };

        var monsteranimation = new Animation
        {
            AnimationSpeed = 100,
            TargetAnimation = 97
        };
        
        var monsterSpawn = new Rectangle(source, 10, 10);
        var outline = monsterSpawn.GetOutline().ToList();
        var mantis = MonsterFactory.Create("pf_giant_mantis", mapInstance, Point.From(source));
        Point point;

        do
            point = outline.PickRandom();
        while (!mapInstance.IsWalkable(point, mantis.Type));

        mantis.AggroRange = 12;
        mantis.Experience = 1000;
        mantis.SetLocation(point);
        source.MapInstance.AddObject(mantis, point);
        mantis.Animate(monsteranimation);

        foreach (var players in mapInstance.GetEntities<Aisling>())
        {
            source.SendOrangeBarMessage("You feel the ground shake, a quick shadow overcasts you");
            source.Animate(animation);
        }
    }
}
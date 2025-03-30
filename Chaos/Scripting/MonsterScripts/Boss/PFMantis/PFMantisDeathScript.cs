using Chaos.Collections;
using Chaos.Definitions;
using Chaos.Extensions.Geometry;
using Chaos.Models.World;
using Chaos.Scripting.MonsterScripts.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Boss.PFMantis;

[SuppressMessage("ReSharper", "UnusedVariable")]
public sealed class PfMantisDeathScript : MonsterScriptBase
{
    private readonly ISimpleCache SimpleCache;

    /// <inheritdoc />
    public PfMantisDeathScript(Monster subject, ISimpleCache simpleCache)
        : base(subject)
        => SimpleCache = simpleCache;

    public override void OnDeath()
    {
        var rectangle = new Rectangle(
            9,
            5,
            3,
            4);

        foreach (var member in Subject.MapInstance
                                      .GetEntities<Aisling>()
                                      .ToList())
        {
            var hasStage = member.Trackers.Enums.TryGetValue(out PFQuestStage stage);

            var mapInstance = SimpleCache.Get<MapInstance>("pf_giantmantisunderground");
            Point point;

            do
                point = rectangle.GetRandomPoint();
            while (!mapInstance.IsWalkable(point, collisionType: member.Type));

            member.TraverseMap(mapInstance, point);
            member.Trackers.Enums.Set(PFQuestStage.KilledGiantMantis);
            member.SendOrangeBarMessage("The ground underneath you caves...");
            member.Inventory.Remove("Turuc Pendant");
            member.Inventory.Remove("silver wolf leather");
        }
    }
}
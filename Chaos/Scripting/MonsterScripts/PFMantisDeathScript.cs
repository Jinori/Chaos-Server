using Chaos.Containers;
using Chaos.Definitions;
using Chaos.Extensions.Geometry;
using Chaos.Objects.World;
using Chaos.Scripting.MonsterScripts.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.MonsterScripts;

[SuppressMessage("ReSharper", "UnusedVariable")]
public class PFMantisDeathScript : MonsterScriptBase
{
    private readonly ISimpleCache SimpleCache;

    /// <inheritdoc />
    public PFMantisDeathScript(Monster subject, ISimpleCache simpleCache)
        : base(subject) =>
        SimpleCache = simpleCache;

    public override void OnDeath()
    {
        var rectangle = new Rectangle(
            9,
            5,
            3,
            4);

        foreach (var member in Subject.MapInstance.GetEntities<Aisling>().ToList())
        {
            var hasStage = member.Trackers.Enums.TryGetValue(out PFQuestStage stage);

            if (stage != PFQuestStage.FoundPendant)
                return;

            var mapInstance = SimpleCache.Get<MapInstance>("pf_giantmantisunderground");
            Point point;

            do
                point = rectangle.GetRandomPoint();
            while (!mapInstance.IsWalkable(point, member.Type));

            member.TraverseMap(mapInstance, point);
            member.Trackers.Enums.Set(PFQuestStage.KilledGiantMantis);
            member.SendOrangeBarMessage("The ground underneath you caves...");
            member.Inventory.Remove("Turuc Pendant");
            member.Inventory.Remove("silver wolf leather");
        }
    }
}
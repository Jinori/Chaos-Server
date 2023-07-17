using Chaos.Collections;
using Chaos.Definitions;
using Chaos.Extensions.Geometry;
using Chaos.Models.World;
using Chaos.Scripting.MonsterScripts.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.MonsterScripts;

[SuppressMessage("ReSharper", "UnusedVariable")]
public class QueenOctopusDeathScript : MonsterScriptBase
{
    private readonly ISimpleCache SimpleCache;

    /// <inheritdoc />
    public QueenOctopusDeathScript(Monster subject, ISimpleCache simpleCache)
        : base(subject) =>
        SimpleCache = simpleCache;

    public override void OnDeath()
    {
        var rectangle = new Rectangle(
            21,
            5,
            3,
            3);

        foreach (var member in Subject.MapInstance.GetEntities<Aisling>().ToList())
        {

            var mapInstance = SimpleCache.Get<MapInstance>("karloposn");
            Point point;

            do
                point = rectangle.GetRandomPoint();
            while (!mapInstance.IsWalkable(point, member.Type));

            member.TraverseMap(mapInstance, point);
            
            var hasStage = member.Trackers.Enums.TryGetValue(out QueenOctopusQuest stage);

            if (stage == QueenOctopusQuest.Pendant3)
            {
                member.Trackers.Enums.Set(QueenOctopusQuest.Queen);
                member.Inventory.Remove("Coral Pendant");
                member.Inventory.Remove("Red Pearl");
            }
            
            break;
        }
    }
}
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
            2,
            2);

        foreach (var member in Subject.MapInstance.GetEntities<Aisling>().ToList())
        {

            var mapInstance = SimpleCache.Get<MapInstance>("karloposn");
            Point point;

            do
                point = rectangle.GetRandomPoint();
            while (!mapInstance.IsWalkable(point, member.Type));
            
            member.Trackers.TimedEvents.AddEvent("QueenOctopusCD", TimeSpan.FromHours(24), true);
            
            var hasStage = member.Trackers.Enums.TryGetValue(out QueenOctopusQuest stage);

            if (stage == QueenOctopusQuest.Pendant3)
            {
                member.Trackers.Enums.Set(QueenOctopusQuest.Queen);
            }
            
            member.TraverseMap(mapInstance, point);
            
        }
    }
}
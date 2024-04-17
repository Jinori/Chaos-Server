using Chaos.Collections;
using Chaos.Definitions;
using Chaos.Extensions.Geometry;
using Chaos.Models.World;
using Chaos.Scripting.MonsterScripts.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Boss.UndineFields;
public sealed class CarnunDeathScript : MonsterScriptBase
{
    private readonly ISimpleCache SimpleCache;

    /// <inheritdoc />
    public CarnunDeathScript(Monster subject, ISimpleCache simpleCache)
        : base(subject) =>
        SimpleCache = simpleCache;

    public override void OnDeath()
    {
        var rectangle = new Rectangle(
            6,
            6,
            3,
            4);

        foreach (var member in Subject.MapInstance.GetEntities<Aisling>().ToList())
        {
            var mapInstance = SimpleCache.Get<MapInstance>("undine_field_entrance");
            Point point;

            do
                point = rectangle.GetRandomPoint();
            while (!mapInstance.IsWalkable(point, member.Type));

            member.TraverseMap(mapInstance, point);
            member.Trackers.Enums.Set(UndineFieldDungeon.KilledCarnun);
            member.SendOrangeBarMessage("The Carnun champion falls... You quickly run out.");
        }
    }
}
using Chaos.Collections;
using Chaos.Models.World;
using Chaos.Scripting.MonsterScripts.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Boss.MainStory.TrialOfSacrifice.zoe;

public sealed class ZoeDeathScript : MonsterScriptBase
{
    private readonly ISimpleCache SimpleCache;

    /// <inheritdoc />
    public ZoeDeathScript(Monster subject, ISimpleCache simpleCache)
        : base(subject)
    {
        SimpleCache = simpleCache;
    }

    /// <inheritdoc />
    public override void OnDeath()
    {
        if (!Map.RemoveEntity(Subject))
            return;


        foreach (var aisling in Map.GetEntities<Aisling>())
        {
            var mapInstance = SimpleCache.Get<MapInstance>("godsrealm");
            var point = new Point(16, 16);
            aisling.TraverseMap(mapInstance, point);
            aisling.Trackers.TimedEvents.AddEvent("sacrificetrialcd", TimeSpan.FromHours(1), true);
            aisling.SendActiveMessage("Skandara looks disappointed and pulls you out.");
            aisling.Refresh();
        }
    }
}
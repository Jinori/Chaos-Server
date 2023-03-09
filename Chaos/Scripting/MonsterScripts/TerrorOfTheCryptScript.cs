using Chaos.Common.Definitions;
using Chaos.Containers;
using Chaos.Objects.World;
using Chaos.Scripting.MonsterScripts.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.MonsterScripts;

public class TerrorOfTheCryptScript : MonsterScriptBase
{
    private readonly ISimpleCache SimpleCache;

    public TerrorOfTheCryptScript(Monster subject, ISimpleCache simpleCache)
        : base(subject) => SimpleCache = simpleCache;

    public override void OnDeath()
    {
        var mapInstance = SimpleCache.Get<MapInstance>("cryptTerrorReward");

        foreach (var aisling in Subject.MapInstance.GetEntities<Aisling>())
        {
            aisling.TraverseMap(mapInstance, new Point(4, 5));
            aisling.Trackers.Flags.RemoveFlag(QuestFlag1.TerrorOfCryptHunt);
            aisling.Trackers.Flags.AddFlag(QuestFlag1.TerrorOfCryptComplete);
            aisling.Client.SendServerMessage(ServerMessageType.OrangeBar1, "The terror will no longer make the Old Man suffer.");
        }
    }
}
using Chaos.Collections;
using Chaos.Common.Definitions;
using Chaos.DarkAges.Definitions;
using Chaos.Models.World;
using Chaos.Scripting.MonsterScripts.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.MonsterScripts;

public class TerrorOfTheCryptScript : MonsterScriptBase
{
    private readonly ISimpleCache SimpleCache;

    public TerrorOfTheCryptScript(Monster subject, ISimpleCache simpleCache)
        : base(subject)
        => SimpleCache = simpleCache;

    public override void OnDeath()
    {
        // Get the map instance from the SimpleCache
        var mapInstance = SimpleCache.Get<MapInstance>("cryptTerrorReward");

        // Iterate through all Aislings in the Subject's MapInstance
        foreach (var aisling in Subject.MapInstance.GetEntities<Aisling>())
        {
            // Move the Aisling to the map instance
            aisling.TraverseMap(mapInstance, new Point(4, 5));

            // Remove the Terror of Crypt Hunt flag
            aisling.Trackers.Flags.RemoveFlag(QuestFlag1.TerrorOfCryptHunt);

            // Add the Terror of Crypt Complete flag
            aisling.Trackers.Flags.AddFlag(QuestFlag1.TerrorOfCryptComplete);

            // Send a message to the Aisling's client
            aisling.Client.SendServerMessage(ServerMessageType.OrangeBar1, "The terror will no longer make the Old Man suffer.");
        }
    }
}
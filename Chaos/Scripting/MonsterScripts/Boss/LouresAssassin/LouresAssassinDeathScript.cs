using Chaos.Collections;
using Chaos.Definitions;
using Chaos.Models.World;
using Chaos.Scripting.MonsterScripts.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Boss.LouresAssassin;

public sealed class LouresAssassinDeathScript : MonsterScriptBase
{
    private readonly ISimpleCache SimpleCache;

    /// <inheritdoc />
    public LouresAssassinDeathScript(Monster subject, ISimpleCache simpleCache)
        : base(subject)
        => SimpleCache = simpleCache;

    public override void OnDeath()
    {
        foreach (var member in Subject.MapInstance
                                      .GetEntities<Aisling>()
                                      .ToList())
        {
            var mapInstance = SimpleCache.Get<MapInstance>("loures_training_camp");

            member.Trackers.Enums.Set(SupplyLouresStage.KilledAssassin);
            member.TraverseMap(mapInstance, member);
            member.SendOrangeBarMessage("The Assassin flees.");
        }
    }
}
using Chaos.Collections;
using Chaos.Common.Utilities;
using Chaos.Models.World;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ApplyDamage;
using Chaos.Scripting.MapScripts.Abstractions;
using Chaos.Storage.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.MapScripts.MainStoryLine.Creants.Phoenix;

// the sky above the phoenix shard is also a shard
// if someone relogs while in the sky, we want to port them back to the correct phoenix shard
// and deal the damage they should have taken from the phoenix script
// so we need this script to store the phoenix shard that created it, and a location to port players back to
// HOWEVER, we dont want to port aislings back that are being carried by phoenix as a natural part of the script
// so we need a whitelist of aislings to ignore
// the whitelist will hold the player currently being held by phoenix
// and that player will be removed from the whitelist when phoenix teleports back to it's map

public class PhoenixSkyDedicatedShardScript : MapScriptBase
{
    public Location FromLocation { get; set; } = null!;
    public List<string> WhiteList { get; set; } = [];
    public Monster Phoenix { get; set; } = null!;
    private readonly IIntervalTimer CheckTimer = new IntervalTimer(TimeSpan.FromSeconds(5));
    private readonly ISimpleCache SimpleCache;
    private readonly IApplyDamageScript ApplyDamageScript;

    /// <inheritdoc />
    public PhoenixSkyDedicatedShardScript(MapInstance subject, ISimpleCache simpleCache)
        : base(subject)
    {
        SimpleCache = simpleCache;
        ApplyDamageScript = ApplyNonAttackDamageScript.Create();
    }

    /// <inheritdoc />
    public override void Update(TimeSpan delta)
    {
        CheckTimer.Update(delta);

        if (CheckTimer.IntervalElapsed)
        {
            var aislings = Subject.GetEntities<Aisling>()
                                  .ExceptBy(WhiteList, aisling => aisling.Name);

            var owningMapInstance = SimpleCache.Get<MapInstance>(FromLocation.Map);

            foreach (var aisling in aislings)
                aisling.TraverseMap(
                    owningMapInstance,
                    FromLocation,
                    true,
                    onTraverse: () => ApplyFallDamageAsync(aisling));
        }
    }

    private Task ApplyFallDamageAsync(Aisling aisling)
    {
        var damage = MathEx.GetPercentOf<int>((int)aisling.StatSheet.EffectiveMaximumHp, 75);

        ApplyDamageScript.ApplyDamage(
            Phoenix,
            aisling,
            this,
            damage);

        return Task.CompletedTask;
    }
}
using Chaos.Extensions;
using Chaos.Scripting.MapScripts.MainStoryLine;
using Chaos.Scripting.MapScripts.MainStoryLine.Creants.Phoenix;

namespace Chaos.Scripting.EffectScripts;

public class CreantPowerLimitEffect : PowerLimitEffect
{
    /// <inheritdoc />
    public override string Name => "Creant Power Limit";

    /// <inheritdoc />
    public override void Update(TimeSpan delta)
    {
        if (!Subject.MapInstance.Script.Is<CreantBossMapScript>() && !Subject.MapInstance.Script.Is<PhoenixSkyDedicatedShardScript>())
        {
            Subject.Effects.Terminate(Name);

            return;
        }
        
        base.Update(delta);
    }
}
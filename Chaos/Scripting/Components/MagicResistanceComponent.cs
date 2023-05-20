using Chaos.Common.Utilities;
using Chaos.Models.Data;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Components.Abstractions;
using Chaos.Scripting.Components.Utilities;

namespace Chaos.Scripting.Components;

public class MagicResistanceComponent : IConditionalComponent
{
    public bool Execute(ActivationContext context, ComponentVars vars)
    {
        var targets = vars.GetTargets<Creature>();
        var options = vars.GetOptions<IMagicResistanceComponentOptions>();

        //Immediately cast spell
        if (options.IgnoreMagicResistance)
            return true;

        foreach (var target in targets)
        {
            if (Randomizer.RollChance(100 - target.StatSheet.EffectiveMagicResistance))
                return true;
            
            target.Animate(MissAnimation);
        }
        return false;
    }

    public interface IMagicResistanceComponentOptions
    {
        bool IgnoreMagicResistance { get; init; }
    }
    
    private static readonly Animation MissAnimation = new()
    {
        TargetAnimation = 33,
        AnimationSpeed = 100
    };
}
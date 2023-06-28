using Chaos.Common.Utilities;
using Chaos.Models.Data;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Components.Abstractions;
using Chaos.Scripting.Components.Utilities;

namespace Chaos.Scripting.Components;

public class MagicResistanceComponent : IComponent
{
    public void Execute(ActivationContext context, ComponentVars vars)
    {
        var targets = vars.GetTargets<Creature>().ToList();
        var options = vars.GetOptions<IMagicResistanceComponentOptions>();

        //Immediately cast spell
        if (options.IgnoreMagicResistance)
            return;

        foreach (var target in targets.ToList())
            if (!IntegerRandomizer.RollChance(100 - target.StatSheet.EffectiveMagicResistance))
            {
                targets.Remove(target);
                target.Animate(MissAnimation);  
            }
        
        vars.SetTargets(targets);

        return;
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
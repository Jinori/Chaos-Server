using Chaos.Common.Utilities;
using Chaos.Models.Data;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Components.Abstractions;
using Chaos.Scripting.Components.Utilities;

namespace Chaos.Scripting.Components;

public sealed class MagicResistanceComponent : IComponent
{
    public void Execute(ActivationContext context, ComponentVars vars)
    {
        var userHit = context.Source.StatSheet.EffectiveHit;
        var targets = vars.GetTargets<Creature>().ToList();
        var options = vars.GetOptions<IMagicResistanceComponentOptions>();

        //Immediately cast spell
        if (options.IgnoreMagicResistance)
            return;

        // Calculate additional Hit
        var additionalHitChance = (userHit / 6) * 2;

        foreach (var target in targets.ToList())
        {
            // Calculate chance to hit, ensuring it's between 0% and 100%
            var rawChanceToHit = 100 - target.StatSheet.EffectiveMagicResistance + additionalHitChance;
            var chanceToHit = Math.Clamp(rawChanceToHit, 0, 100);

            if (!IntegerRandomizer.RollChance(chanceToHit))
            {
                targets.Remove(target);
                target.Animate(MissAnimation);  
            }
        }

        vars.SetTargets(targets);
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
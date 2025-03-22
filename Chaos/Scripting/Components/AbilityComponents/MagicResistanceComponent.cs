using Chaos.Common.Utilities;
using Chaos.Models.Data;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Components.Abstractions;
using Chaos.Scripting.Components.Execution;

namespace Chaos.Scripting.Components.AbilityComponents;

public sealed class MagicResistanceComponent : IConditionalComponent
{
    private static readonly Animation MissAnimation = new()
    {
        TargetAnimation = 33,
        AnimationSpeed = 100
    };

    public bool Execute(ActivationContext context, ComponentVars vars)
    {
        var userHit = context.Source.StatSheet.EffectiveHit;

        //must have MapEntity here, because targets can be anything
        var targets = vars.GetTargets<MapEntity>()
                          .ToList();
        var options = vars.GetOptions<IMagicResistanceComponentOptions>();

        // Immediately cast spell if ignoring magic resistance
        if (options.IgnoreMagicResistance)
            return true;

        //OfType<Creature> here because MR only applies to creatures
        foreach (var target in targets.OfType<Creature>().ToList())
        {
            // Step 1: Calculate the difference between base magic resistance and user's hit
            var baseMagicResistance = target.StatSheet.EffectiveMagicResistance;
            var adjustedHit = baseMagicResistance - userHit;

            // Step 2: Calculate the raw chance to hit after applying adjusted hit value
            var rawChanceToHit = 100 - adjustedHit;

            // Step 3: Apply minimum hit chance threshold of 20%
            if (rawChanceToHit < 20)
                rawChanceToHit = 20;

            // Step 4: Clamp the chance to hit between 0 and 100
            var finalChanceToHit = Math.Clamp(rawChanceToHit, 0, 100);

            // Step 5: Determine whether the spell hits or misses
            if (!IntegerRandomizer.RollChance(finalChanceToHit))
            {
                // If miss, remove the target and play miss animation
                targets.Remove(target);
                target.Animate(MissAnimation);
            }
        }

        // Update the target list with valid hits
        vars.SetTargets(targets);
        
        // If no targets were found, return false
        return targets.Count != 0;
    }

    public interface IMagicResistanceComponentOptions
    {
        bool IgnoreMagicResistance { get; init; }
    }
}
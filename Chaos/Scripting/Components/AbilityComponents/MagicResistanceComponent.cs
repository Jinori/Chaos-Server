using Chaos.Common.Utilities;
using Chaos.Models.Data;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Components.Abstractions;
using Chaos.Scripting.Components.Execution;

namespace Chaos.Scripting.Components.AbilityComponents;

public sealed class MagicResistanceComponent : IComponent
{
    private static readonly Animation MissAnimation = new()
    {
        TargetAnimation = 33,
        AnimationSpeed = 100
    };

    public void Execute(ActivationContext context, ComponentVars vars)
    {
        var userHit = context.Source.StatSheet.EffectiveHit;
        var targets = vars.GetTargets<Creature>().ToList();
        var options = vars.GetOptions<IMagicResistanceComponentOptions>();

        // Immediately cast spell if ignoring magic resistance
        if (options.IgnoreMagicResistance)
            return;

        foreach (var target in targets.ToList())
        {
            // Calculate effective magic resistance, capped at 70%
            var baseMagicResistance = target.StatSheet.EffectiveMagicResistance;
            var maxMagicResistance = 70;
            var effectiveMagicResistance = Math.Min(baseMagicResistance, maxMagicResistance);

            // Calculate chance to hit based on magic resistance and additional hit chance
            var rawChanceToHit = 100 - effectiveMagicResistance;

            // Adjust raw chance based on user's hit ability
            if (userHit > 0)
            {
                rawChanceToHit += userHit;
            }
            else
            {
                rawChanceToHit = 100; // No hit ability, so 100% chance to hit
            }

            // Calculate final chance to hit, clamped between 0 and 100
            var finalChanceToHit = Math.Clamp(rawChanceToHit, 0, 100);

            if (!IntegerRandomizer.RollChance(finalChanceToHit))
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
}

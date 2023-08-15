using Chaos.Common.Utilities;
using Chaos.Models.Data;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Components.Abstractions;
using Chaos.Scripting.Components.Utilities;

namespace Chaos.Scripting.Components;

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

        // Immediately cast spell
        if (options.IgnoreMagicResistance)
            return;

        // Calculate additional Hit
        var additionalHitChance = userHit / 6 * 2;

        foreach (var target in targets.ToList())
        {
            // Calculate effective magic resistance, capped at 70%
            var effectiveMagicResistance = Math.Min((int)target.StatSheet.EffectiveMagicResistance, 70);

            // Calculate chance to hit based on magic resistance and additional hit chance
            var rawChanceToHit = 100 - effectiveMagicResistance + additionalHitChance;

            // Calculate final chance to hit, taking into account the user's hit chance
            var finalChanceToHit = (rawChanceToHit * userHit) / 100;
            finalChanceToHit = Math.Clamp(finalChanceToHit, 0, 100);

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
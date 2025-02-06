using Chaos.Models.Data;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Components.Abstractions;
using Chaos.Scripting.Components.Execution;

namespace Chaos.Scripting.Components.AbilityComponents
{
    public class RemoveEffectComponent : IComponent
    {
        private static readonly HashSet<string> SkippedEffects =
        [
            "Arena Revive", "Hot Chocolate", "ValentinesCandy",
            "Fury1", "Fury2", "Fury3", "Fury4", "Fury5", "Fury6",
            "Cunning1", "Cunning2", "Cunning3", "Cunning4", "Cunning5", "Cunning6",
            "Invulnerability", "Mount", "GM Knowledge", "Strong Knowledge",
            "Stoned", "Knowledge", "Werewolf", "Fishing", "Foraging"
        ];

        private static readonly HashSet<string> NegativeEffects = new()
        {
            "Poison", "Blind", "Burn"
        };

        /// <inheritdoc />
        public void Execute(ActivationContext context, ComponentVars vars)
        {
            var options = vars.GetOptions<IRemoveEffectComponentOptions>();
            var targets = vars.GetTargets<Creature>();

            if (options.RemoveAllEffects == true)
            {
                RemoveAllEffects(targets);
                return;
            }

            if (options.NegativeEffect == true)
            {
                RemoveSpecificEffects(targets, NegativeEffects);
                return;
            }

            if (!string.IsNullOrEmpty(options.EffectKey))
            {
                RemoveSpecificEffects(targets, [options.EffectKey]);
            }
        }

        private static void RemoveAllEffects(IEnumerable<Creature> targets)
        {
            foreach (var target in targets)
            {
                var removableEffects = target.Effects
                    .Where(effect => !SkippedEffects.Contains(effect.Name))
                    .Select(effect => effect.Name)
                    .ToList();

                foreach (var effectName in removableEffects)
                {
                    target.Effects.Dispel(effectName);
                }
            }
        }

        private static void RemoveSpecificEffects(IEnumerable<Creature> targets, HashSet<string> effectsToRemove)
        {
            foreach (var target in targets)
            {
                foreach (var effect in effectsToRemove)
                {
                    target.Effects.Dispel(effect);
                }
            }
        }

        public interface IRemoveEffectComponentOptions
        {
            string? EffectKey { get; init; }
            bool? NegativeEffect { get; init; }
            bool? RemoveAllEffects { get; init; }
        }
    }
}

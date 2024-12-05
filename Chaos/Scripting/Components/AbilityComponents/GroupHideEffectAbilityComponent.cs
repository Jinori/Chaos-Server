using Chaos.Common.Utilities;
using Chaos.Extensions;
using Chaos.Models.Data;
using Chaos.Scripting.Components.Abstractions;
using Chaos.Scripting.Components.Execution;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.Components.AbilityComponents;

public struct GroupHideEffectAbilityComponent : IComponent
{
    /// <inheritdoc />
    public void Execute(ActivationContext context, ComponentVars vars)
    {
        var options = vars.GetOptions<IApplyEffectComponentOptions>();

        if (string.IsNullOrEmpty(options.EffectKey) || (options.EffectKey != "Hide"))
            return;

        var caster = context.SourceAisling;

        if (caster == null)
            return;

        var groupMembers = caster.Group;

        if (groupMembers != null)
            foreach (var member in groupMembers)
            {
                if (member.Name == caster.Name)
                    continue;

                if (!member.MapInstance.IsWithinMap(caster) || !member.WithinRange(caster, 12))
                    continue;

                if (member.Effects.Contains("Hide"))
                    continue;

                if (member.Effects.Contains("Mount"))
                    member.Effects.Terminate("Mount");

                if (options.EffectApplyChance.HasValue && !IntegerRandomizer.RollChance(options.EffectApplyChance.Value))
                    continue;

                var effect = options.EffectFactory.Create(options.EffectKey);

                if (options.EffectDurationOverride.HasValue)
                    effect.SetDuration(options.EffectDurationOverride.Value);

                member.Effects.Apply(context.Source, effect);
                member.SendOrangeBarMessage($"{caster.Name} casts hide on you.");
            }
        else
            caster.SendOrangeBarMessage("You are not grouped to anyone.");
    }

    public interface IApplyEffectComponentOptions
    {
        int? EffectApplyChance { get; init; }
        TimeSpan? EffectDurationOverride { get; init; }
        IEffectFactory EffectFactory { get; init; }
        string? EffectKey { get; init; }
    }
}
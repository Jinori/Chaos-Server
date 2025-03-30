using Chaos.Models.Data;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Components.Abstractions;
using Chaos.Scripting.Components.Execution;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.Components.AbilityComponents;

public struct FuryEffectAbilityComponent : IComponent
{
    private readonly IEffectFactory EffectFactory;

    public FuryEffectAbilityComponent(IEffectFactory effectFactory) => EffectFactory = effectFactory;

    /// <inheritdoc />
    public void Execute(ActivationContext context, ComponentVars vars)
    {
        var targets = vars.GetTargets<Creature>();
        var effect1 = EffectFactory.Create("Fury1");
        var effect2 = EffectFactory.Create("Fury2");
        var effect3 = EffectFactory.Create("Fury3");
        var effect4 = EffectFactory.Create("Fury4");
        var effect5 = EffectFactory.Create("Fury5");
        var effect6 = EffectFactory.Create("Fury6");

        foreach (var target in targets)
        {
            if (!target.Effects.Contains("Fury1")
                || !target.Effects.Contains("Fury2")
                || !target.Effects.Contains("Fury3")
                || !target.Effects.Contains("Fury4")
                || !target.Effects.Contains("Fury5")
                || (!target.Effects.Contains("Fury6") && (target.StatSheet.EffectiveMaximumHp >= 8000)))
            {
                target.Effects.Apply(context.Source, effect1);

                return;
            }

            if (target.Effects.Contains("Fury1") && (target.StatSheet.EffectiveMaximumHp >= 16000))
            {
                target.Effects.Terminate("Fury1");
                target.Effects.Apply(context.Source, effect2);

                return;
            }

            if (target.Effects.Contains("Fury2") && (target.StatSheet.EffectiveMaximumHp >= 32000))
            {
                target.Effects.Terminate("Fury2");
                target.Effects.Apply(context.Source, effect3);

                return;
            }

            if (target.Effects.Contains("Fury3") && (target.StatSheet.EffectiveMaximumHp >= 64000))
            {
                target.Effects.Terminate("Fury3");
                target.Effects.Apply(context.Source, effect4);

                return;
            }

            if (target.Effects.Contains("Fury4") && (target.StatSheet.EffectiveMaximumHp >= 128000))
            {
                target.Effects.Terminate("Fury4");
                target.Effects.Apply(context.Source, effect5);

                return;
            }

            if (target.Effects.Contains("Fury5") && (target.StatSheet.EffectiveMaximumHp >= 256000))
            {
                target.Effects.Terminate("Fury5");
                target.Effects.Apply(context.Source, effect6);

                return;
            }

            context.SourceAisling?.SendOrangeBarMessage("You do not have the health to increase your Fury.");
        }
    }

    public interface IApplyEffectComponentOptions
    {
        int? EffectApplyChance { get; init; }
        TimeSpan? EffectDurationOverride { get; init; }
        IEffectFactory EffectFactory { get; init; }
        string? EffectKey { get; init; }
    }
}
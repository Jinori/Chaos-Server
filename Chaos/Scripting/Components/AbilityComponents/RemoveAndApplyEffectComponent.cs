using Chaos.Models.Data;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Components.Abstractions;
using Chaos.Scripting.Components.Execution;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.Components.AbilityComponents;

public class RemoveAndApplyEffectComponent : IConditionalComponent
{
    /// <inheritdoc />
    public bool Execute(ActivationContext context, ComponentVars vars)
    {
        var options = vars.GetOptions<IRemoveAndApplyEffectComponentOptions>();
        var targets = vars.GetTargets<Creature>();

        if (string.IsNullOrEmpty(options.EffectKeyToRemove))
            return false;

        if (options.RemoveAllEffects.HasValue)
            foreach (var target in targets)
            {
                foreach (var effect in target.Effects)
                    target.Effects.Dispel(effect.Name);
            }

        if (string.IsNullOrEmpty(options.EffectKeyToAddAfterRemoval))
            return false;

        foreach (var target in targets)
        {
            if (options.EffectKeyToRemove is "beag cradh")
            {
                if (target.Effects.Contains("cradh"))
                {
                    context.SourceAisling?.SendOrangeBarMessage("Target is already under another effect. [Cradh]");

                    return false;
                }

                if (target.Effects.Contains("Mor Cradh"))
                {
                    context.SourceAisling?.SendOrangeBarMessage("Target is already under another effect. [Mor Cradh]");

                    return false;
                }

                if (target.Effects.Contains("Ard Cradh"))
                {
                    context.SourceAisling?.SendOrangeBarMessage("Target is already under another effect. [Ard Cradh]");

                    return false;
                }

                if (target.Effects.Contains("Dia Cradh"))
                {
                    context.SourceAisling?.SendOrangeBarMessage("Target is already under another effect. [Dia Cradh]");

                    return false;
                }

                if (target.Effects.Contains("Beag Cradh") && options.EffectKeyToAddAfterRemoval is "preventrecradh")
                {
                    var effect = options.EffectFactory.Create(options.EffectKeyToAddAfterRemoval);
                    target.Effects.Apply(context.Source, effect, vars.GetSourceScript());
                    target.Effects.Dispel(options.EffectKeyToRemove);
                    context.SourceAisling?.SendOrangeBarMessage($"{target.Name}'s curse healed and prevent recradh is now active.");

                    return true;
                }
            }

            if (options.EffectKeyToRemove is "cradh")
            {
                if (target.Effects.Contains("Mor Cradh"))
                {
                    context.SourceAisling?.SendOrangeBarMessage("Target is already under another effect. [Mor Cradh]");

                    return false;
                }

                if (target.Effects.Contains("Ard Cradh"))
                {
                    context.SourceAisling?.SendOrangeBarMessage("Target is already under another effect. [Ard Cradh]");

                    return false;
                }

                if (target.Effects.Contains("Dia Cradh"))
                {
                    context.SourceAisling?.SendOrangeBarMessage("Target is already under another effect. [Dia Cradh]");

                    return false;
                }

                if (target.Effects.Contains("Cradh")
                    || (target.Effects.Contains("Beag Cradh") && options.EffectKeyToAddAfterRemoval is "preventrecradh"))
                {
                    var effect = options.EffectFactory.Create(options.EffectKeyToAddAfterRemoval);
                    target.Effects.Apply(context.Source, effect, vars.GetSourceScript());
                    target.Effects.Dispel("Beag Cradh");
                    target.Effects.Dispel(options.EffectKeyToRemove);
                    context.SourceAisling?.SendOrangeBarMessage($"{target.Name}'s curse healed and prevent recradh is now active.");

                    return true;
                }
            }

            if (options.EffectKeyToRemove is "mor cradh")
            {
                if (target.Effects.Contains("Ard Cradh"))
                {
                    context.SourceAisling?.SendOrangeBarMessage("Target is already under another effect. [Ard Cradh]");

                    return false;
                }

                if (target.Effects.Contains("Dia Cradh"))
                {
                    context.SourceAisling?.SendOrangeBarMessage("Target is already under another effect. [Dia Cradh]");

                    return false;
                }

                if (target.Effects.Contains("Mor Cradh")
                    || target.Effects.Contains("Cradh")
                    || (target.Effects.Contains("Beag Cradh") && options.EffectKeyToAddAfterRemoval is "preventrecradh"))
                {
                    var effect = options.EffectFactory.Create(options.EffectKeyToAddAfterRemoval);
                    target.Effects.Apply(context.Source, effect, vars.GetSourceScript());
                    target.Effects.Dispel("Beag Cradh");
                    target.Effects.Dispel("Cradh");
                    target.Effects.Dispel(options.EffectKeyToRemove);
                    context.SourceAisling?.SendOrangeBarMessage($"{target.Name}'s curse healed and prevent recradh is now active.");

                    return true;
                }
            }

            if (options.EffectKeyToRemove is "ard cradh")
            {
                if (target.Effects.Contains("Dia Cradh"))
                {
                    context.SourceAisling?.SendOrangeBarMessage("Target is already under another effect. [Dia Cradh]");

                    return false;
                }

                if (target.Effects.Contains("Ard Cradh")
                    || target.Effects.Contains("Mor Cradh")
                    || target.Effects.Contains("Cradh")
                    || (target.Effects.Contains("Beag Cradh") && options.EffectKeyToAddAfterRemoval is "preventrecradh"))
                {
                    var effect = options.EffectFactory.Create(options.EffectKeyToAddAfterRemoval);
                    target.Effects.Apply(context.Source, effect, vars.GetSourceScript());
                    target.Effects.Dispel("Beag Cradh");
                    target.Effects.Dispel("Cradh");
                    target.Effects.Dispel("Mor Cradh");
                    target.Effects.Dispel(options.EffectKeyToRemove);
                    context.SourceAisling?.SendOrangeBarMessage($"{target.Name}'s curse healed and prevent recradh is now active.");

                    return true;
                }
            }

            if (options.EffectKeyToRemove is "dia cradh")
            {
                if (target.Effects.Contains("Dia Cradh") && options.EffectKeyToAddAfterRemoval is "preventrecradh")
                {
                    var effect = options.EffectFactory.Create(options.EffectKeyToAddAfterRemoval);
                    target.Effects.Apply(context.Source, effect, vars.GetSourceScript());
                    target.Effects.Dispel("Beag Cradh");
                    target.Effects.Dispel("Cradh");
                    target.Effects.Dispel("Mor Cradh");
                    target.Effects.Dispel("Ard Cradh");
                    target.Effects.Dispel(options.EffectKeyToRemove);
                    context.SourceAisling?.SendOrangeBarMessage($"{target.Name}'s curse healed and prevent recradh is now active.");

                    return true;
                }
            } else if (!string.IsNullOrEmpty(options.EffectKeyToRemove) && target.Effects.Contains(options.EffectKeyToRemove))
            {
                target.Effects.Dispel(options.EffectKeyToRemove);

                if (!string.IsNullOrEmpty(options.EffectKeyToAddAfterRemoval))
                {
                    var effect = options.EffectFactory.Create(options.EffectKeyToAddAfterRemoval);
                    target.Effects.Apply(context.Source, effect, vars.GetSourceScript());
                }

                return true;
            }
        }

        return true;
    }

    public interface IRemoveAndApplyEffectComponentOptions
    {
        IEffectFactory EffectFactory { get; init; }
        string? EffectKeyToAddAfterRemoval { get; init; }
        string? EffectKeyToRemove { get; init; }
        bool? RemoveAllEffects { get; init; }
    }
}
using Chaos.Common.Utilities;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Scripting.Abstractions;
using Chaos.Scripting.Components.AbilityComponents;

namespace Chaos.Scripting.SpellScripts.Abstractions;

public abstract class ConfigurableSpellScriptBase : ConfigurableScriptBase<Spell>, ISpellScript
{
    /// <inheritdoc />
    protected ConfigurableSpellScriptBase(Spell subject)
        : base(subject, scriptKey => subject.Template.ScriptVars[scriptKey]) { }

    /// <inheritdoc />
    public virtual bool CanUse(SpellContext context)
    {
        if (this is HealthCostAbilityComponent.IHealthCostComponentOptions options1)
        {
            var cost = options1.HealthCost ?? 0;
            cost += MathEx.GetPercentOf<int>((int)context.Source.StatSheet.EffectiveMaximumHp, options1.PctHealthCost);

            if (context.Source.StatSheet.CurrentHp < cost)
            {
                context.SourceAisling?.SendActiveMessage("You don't have enough health.");

                return false;
            }
        }
        
        if (this is ManaCostAbilityComponent.IManaCostComponentOptions options)
        {
            var cost = options.ManaCost ?? 0;
            cost += MathEx.GetPercentOf<int>((int)context.Source.StatSheet.EffectiveMaximumMp, options.PctManaCost);

            if (context.Source.StatSheet.CurrentMp < cost)
            {
                context.SourceAisling?.SendActiveMessage("You don't have enough mana.");

                return false;
            }
        }

        return true;
    }

    /// <inheritdoc />
    public virtual void OnUse(SpellContext context) { }

    /// <inheritdoc />
    public virtual void Update(TimeSpan delta) { }
}
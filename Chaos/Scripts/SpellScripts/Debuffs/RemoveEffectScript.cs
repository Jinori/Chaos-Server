using Chaos.Data;
using Chaos.Definitions;
using Chaos.Extensions.Common;
using Chaos.Objects.Panel;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripts.Components;
using Chaos.Scripts.SpellScripts.Abstractions;

namespace Chaos.Scripts.SpellScripts.Debuffs;

[SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global")]
public class RemoveEffectScript : BasicSpellScriptBase, ManaCostComponent.IManaCostComponentOptions
{
    protected ManaCostComponent ManaCostComponent { get; }

    /// <inheritdoc />
    public RemoveEffectScript(Spell subject)
        : base(subject) =>
        ManaCostComponent = new ManaCostComponent();

    /// <inheritdoc />
    public override void OnUse(SpellContext context)
    {
        if (!ManaCostComponent.TryApplyManaCost(context, this))
            return;

        var targets = AbilityComponent.Activate<Creature>(context, this);

        foreach (var target in targets.TargetEntities)
        {
            if (EffectKey.EqualsI("dinarcoli"))
            {
                if (target.Effects.Contains("pramh"))
                {
                    target.Effects.Dispel("pramh");
                    target.Status &= ~Status.Pramh;
                }

                if (target.Effects.Contains("beagpramh"))
                {
                    target.Effects.Dispel("beagpramh");
                    target.Status &= ~Status.Pramh;
                }
            } 
            else
            {
                if (target.Effects.Contains(EffectKey))
                {
                    target.Effects.Dispel(EffectKey);   
                }
            }
        }

        context.SourceAisling?.SendActiveMessage($"You cast {Subject.Template.Name}.");
    }

    #region ScriptVars
    protected string EffectKey { get; init; } = null!;
    public int? ManaCost { get; init; }
    public decimal PctManaCost { get; init; }
    #endregion
}
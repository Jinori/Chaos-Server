using Chaos.Data;
using Chaos.Objects.Panel;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripting.Components;
using Chaos.Scripting.SpellScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.SpellScripts.Debuffs;

[SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global")]
public class ApplyHostileEffectScript : BasicSpellScriptBase, ManaCostComponent.IManaCostComponentOptions
{
    protected IEffectFactory EffectFactory { get; }
    protected ManaCostComponent ManaCostComponent { get; }
    protected MagicResistanceComponent MagicResistComponent { get; }

    /// <inheritdoc />
    public ApplyHostileEffectScript(Spell subject, IEffectFactory effectFactory)
        : base(subject)
    {
        ManaCostComponent = new ManaCostComponent();
        EffectFactory = effectFactory;
        MagicResistComponent = new MagicResistanceComponent();
    }

    /// <inheritdoc />
    public override void OnUse(SpellContext context)
    {
        
        if (!ManaCostComponent.TryApplyManaCost(context, this))
            return;
        
        if (!MagicResistComponent.TryCastSpell(context, this))
            return;
        
        var targets = AbilityComponent.Activate<Creature>(context, this);

        context.SourceAisling?.SendActiveMessage($"You cast {Subject.Template.Name}");

        foreach (var target in targets.TargetEntities)
        {
            var effect = EffectFactory.Create(EffectKey);
            target.Effects.Apply(context.Source, effect);
        }
    }

    #region ScriptVars
    public int? ManaCost { get; init; }
    public decimal PctManaCost { get; init; }
    protected string EffectKey { get; init; } = null!;
    #endregion
}
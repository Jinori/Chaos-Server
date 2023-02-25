using Chaos.Data;
using Chaos.Objects.Panel;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripting.Components;
using Chaos.Scripting.SpellScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.SpellScripts.Buffs;

[SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global")]
public class ApplyEffectScript : BasicSpellScriptBase, ManaCostComponent.IManaCostComponentOptions
{
    protected IEffectFactory EffectFactory { get; }
    protected ManaCostComponent ManaCostComponent { get; }

    /// <inheritdoc />
    public ApplyEffectScript(Spell subject, IEffectFactory effectFactory)
        : base(subject)
    {
        ManaCostComponent = new ManaCostComponent();
        EffectFactory = effectFactory;
    }

    /// <inheritdoc />
    public override void OnUse(SpellContext context)
    {
        if (EffectKey is "Wrath")
            if (context.SourceAisling!.Effects.Contains("Wrath"))
            {
                context.SourceAisling?.Effects.Terminate("Wrath");
                return;
            }

        if (!ManaCostComponent.TryApplyManaCost(context, this))
            return;

        var targets = AbilityComponent.Activate<Creature>(context, this);

        foreach (var target in targets.TargetEntities)
        {
            var effect = EffectFactory.Create(EffectKey);
            target.Effects.Apply(context.Source, effect);
        }

        context.SourceAisling?.SendActiveMessage($"You cast {Subject.Template.Name}");
    }

    #region ScriptVars
    public int? ManaCost { get; init; }
    public decimal PctManaCost { get; init; }
    protected string EffectKey { get; init; } = null!;
    #endregion
}
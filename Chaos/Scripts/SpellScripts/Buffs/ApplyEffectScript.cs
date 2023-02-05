using Chaos.Common.Definitions;
using Chaos.Data;
using Chaos.Objects.Panel;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripts.Components;
using Chaos.Scripts.SpellScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripts.SpellScripts.Buffs;

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
        if (EffectKey is "stench")
        {
            if (context.SourceAisling!.Effects.Contains("stench"))
            {
                context.SourceAisling?.Effects.Terminate("stench");
                return;
            }
        }
        
        ManaCostComponent.ApplyManaCost(context, this);
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
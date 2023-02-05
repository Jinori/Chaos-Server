using Chaos.Data;
using Chaos.Extensions;
using Chaos.Objects.Panel;
using Chaos.Scripts.Components;
using Chaos.Scripts.SpellScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripts.SpellScripts.Buffs;

[SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global")]
public class ApplyGroupEffectScript : BasicSpellScriptBase, ManaCostComponent.IManaCostComponentOptions
{
    protected readonly IEffectFactory EffectFactory;
    protected string EffectKey { get; init; } = null!;
    protected ManaCostComponent ManaCostComponent { get; }

    /// <inheritdoc />
    public ApplyGroupEffectScript(Spell subject, IEffectFactory effectFactory)
        : base(subject)
    {
        EffectFactory = effectFactory;
        ManaCostComponent = new ManaCostComponent();
    }

    /// <inheritdoc />
    public override void OnUse(SpellContext context)
    {
        if (!ManaCostComponent.TryApplyManaCost(context, this))
            return;

        var group = context.SourceAisling?.Group?.Where(x => x.WithinRange(context.SourcePoint));

        if (group != null)
            foreach (var member in group)
            {
                var effect = EffectFactory.Create(EffectKey);
                member.Effects.Apply(member, effect);
            }
        else
        {
            var effect = EffectFactory.Create(EffectKey);
            context.Source.Effects.Apply(context.Source, effect);
        }

        context.SourceAisling?.SendActiveMessage($"You cast {Subject.Template.Name}");
    }

    #region ScriptVars
    public int? ManaCost { get; init; }
    public decimal PctManaCost { get; init; }
    #endregion
}
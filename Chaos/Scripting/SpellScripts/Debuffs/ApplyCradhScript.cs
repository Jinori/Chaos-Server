using Chaos.Common.Definitions;
using Chaos.Data;
using Chaos.Definitions;
using Chaos.Objects.Panel;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripting.Components;
using Chaos.Scripting.SpellScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.SpellScripts.Debuffs;

public class ApplyCradhScript : BasicSpellScriptBase, ManaCostComponent.IManaCostComponentOptions
{
    protected readonly IEffectFactory EffectFactory;
    protected ManaCostComponent ManaCostComponent { get; }

    public ApplyCradhScript(Spell subject, IEffectFactory effectFactory)
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

        {
            
        }

        if (context.Target.Status.HasFlag(Status.PreventAffliction))
        {
            context.SourceAisling?.Client.SendServerMessage(
                ServerMessageType.OrangeBar1,
                "Your magic temporarily fizzles. (Prevent Affliction)");

            return;
        }

        var targets = AbilityComponent.Activate<Creature>(context, this);

        foreach (var target in targets.TargetEntities)
        {
            var effect = EffectFactory.Create(EffectKey);
            target.Effects.Apply(context.Source, effect);
        }

        context.SourceAisling?.SendActiveMessage($"You cast {Subject.Template.Name}");
    }

    #region ScriptVars
    protected string EffectKey { get; init; } = null!;
    public int? ManaCost { get; init; }
    public decimal PctManaCost { get; init; }
    #endregion
}
using Chaos.Data;
using Chaos.Objects.Panel;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripts.SpellScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripts.SpellScripts.Buffs;

[SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global")]
public class ApplyEffectScript : BasicSpellScriptBase
{
    protected string EffectKey { get; init; } = null!;
    protected IEffectFactory EffectFactory { get; }

    /// <inheritdoc />
    public ApplyEffectScript(Spell subject, IEffectFactory effectFactory)
        : base(subject) =>
        EffectFactory = effectFactory;

    /// <inheritdoc />
    public override void OnUse(SpellContext context)
    {
        var targets = AbilityComponent.Activate<Creature>(context, AbilityComponentOptions);

        foreach (var target in targets.TargetEntities)
        {
            var effect = EffectFactory.Create(EffectKey);
            target.Effects.Apply(context.Source, effect);
        }

        context.SourceAisling?.SendActiveMessage($"You cast {Subject.Template.Name}");
    }
}
using Chaos.Data;
using Chaos.Objects.Panel;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripts.SkillScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripts.SkillScripts;

public class ApplyEffectScript : BasicSkillScriptBase
{
    private readonly IEffectFactory EffectFactory;
    private string EffectKey { get; } = null!;

    /// <inheritdoc />
    public ApplyEffectScript(Skill subject, IEffectFactory effectFactory)
        : base(subject) =>
        EffectFactory = effectFactory;

    public override void OnUse(ActivationContext context)
    {
        var targets = AbilityComponent.Activate<Creature>(context, this);

        foreach (var target in targets.TargetEntities)
        {
            var effect = EffectFactory.Create(EffectKey);
            target.Effects.Apply(context.Source, effect);
        }
    }
}
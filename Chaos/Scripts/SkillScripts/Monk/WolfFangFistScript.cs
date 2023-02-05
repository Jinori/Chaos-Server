using Chaos.Data;
using Chaos.Objects.Panel;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripts.FunctionalScripts.Abstractions;
using Chaos.Scripts.FunctionalScripts.ApplyDamage;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripts.SkillScripts.Monk;

public class WolfFangFistScript : DamageScript
{
    protected new IApplyDamageScript ApplyDamageScript { get; }
    protected readonly IEffectFactory EffectFactory;

    /// <inheritdoc />
    public WolfFangFistScript(Skill subject, IEffectFactory effectFactory)
        : base(subject)
    {
        EffectFactory = effectFactory;
        ApplyDamageScript = DefaultApplyDamageScript.Create();
    }

    /// <inheritdoc />
    public override void OnUse(ActivationContext context)
    {
        var targets = AbilityComponent.Activate<Creature>(context, this);
        DamageComponent.ApplyDamage(context, targets.TargetEntities, this);
        var effect = EffectFactory.Create("wolfFangFist");
        foreach (var target in targets.TargetEntities) 
            target.Effects.Apply(context.Source, effect);
    }
}
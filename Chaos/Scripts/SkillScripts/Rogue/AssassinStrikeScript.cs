using Chaos.Data;
using Chaos.Objects.Panel;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripts.FunctionalScripts.Abstractions;
using Chaos.Scripts.FunctionalScripts.ApplyDamage;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripts.SkillScripts.Rogue;

public class AssassinStrikeScript : DamageScript
{
    protected new IApplyDamageScript ApplyDamageScript { get; }

    protected readonly IEffectFactory EffectFactory;

    /// <inheritdoc />
    public AssassinStrikeScript(Skill subject, IEffectFactory effectFactory)
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
        var effect = EffectFactory.Create("AssassinStrike");
        foreach (var target in targets.TargetEntities) 
            target.Effects.Apply(context.Source, effect);
    }
}
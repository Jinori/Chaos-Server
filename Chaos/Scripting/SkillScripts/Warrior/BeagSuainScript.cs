using Chaos.Data;
using Chaos.Objects.Panel;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ApplyDamage;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.SkillScripts.Warrior;

public class BeagSuainScript : DamageScript
{
    protected readonly IEffectFactory EffectFactory;
    protected new IApplyDamageScript ApplyDamageScript { get; }

    /// <inheritdoc />
    public BeagSuainScript(Skill subject, IEffectFactory effectFactory)
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
        var effect = EffectFactory.Create("beagSuain");

        foreach (var target in targets.TargetEntities)
            target.Effects.Apply(context.Source, effect);
    }
}
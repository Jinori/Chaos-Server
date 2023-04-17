using Chaos.Data;
using Chaos.Objects.Panel;
using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ApplyDamage;
using Chaos.Scripting.MonsterScripts;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.SkillScripts.Rogue;

public class AmnesiaScript : DamageScript
{
    protected new IApplyDamageScript ApplyDamageScript { get; }

    /// <inheritdoc />
    public AmnesiaScript(Skill subject, IEffectFactory effectFactory)
        : base(subject) =>
        ApplyDamageScript = ApplyAttackDamageScript.Create();

    /// <inheritdoc />
    public override void OnUse(ActivationContext context)
    {
        var targets = AbilityComponent.Activate<Creature>(context, this);

        foreach (var target in targets.TargetEntities)
        {
            var monster = target as Monster;
            monster?.AggroList.Clear();
            
        }

        context.SourceAisling?.SendActiveMessage("The creature loses all focus");
    }

}

using Chaos.Common.Utilities;
using Chaos.Data;
using Chaos.Extensions;
using Chaos.Objects.Panel;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ApplyDamage;

namespace Chaos.Scripting.SkillScripts.Rogue;

public class SapNeedleScript : DamageScript
{
    protected readonly Animation SuccessfulSap = new()
        { AnimationSpeed = 100, TargetAnimation = 127 };
    protected new IApplyDamageScript ApplyDamageScript { get; }

    /// <inheritdoc />
    public SapNeedleScript(Skill subject)
        : base(subject) =>
        ApplyDamageScript = ApplyAttackDamageScript.Create();

    /// <inheritdoc />
    public override void OnUse(ActivationContext context)
    {
        var targets = AbilityComponent.Activate<Creature>(context, this);
        var manaToGive = 0;

        foreach (var target in targets.TargetEntities)
        {
            DamageComponent.ApplyDamage(context, targets.TargetEntities, this);
            var halfOfAttackersMana = MathEx.GetPercentOf<int>((int)context.Source.StatSheet.EffectiveMaximumMp, 50);
            var halfOfDefendersMana = MathEx.GetPercentOf<int>((int)target.StatSheet.EffectiveMaximumMp, 50);
            manaToGive = Math.Min(halfOfDefendersMana, halfOfAttackersMana);
            target.StatSheet.SubtractManaPct(50);
        }

        var group = context.SourceAisling?.Group?.Where(x => x.WithinRange(context.SourceAisling)).ToList();

        if (group != null)
            foreach (var member in group)
            {
                member.ApplyMana(member, manaToGive / group.Count);
                member.MapInstance.ShowAnimation(SuccessfulSap.GetTargetedAnimation(member.Id));
            }
        else
        {
            context.Source.ApplyMana(context.Source, manaToGive / 2);
            context.Source.MapInstance.ShowAnimation(SuccessfulSap.GetTargetedAnimation(context.Source.Id));
        }
    }
}
using Chaos.Common.Definitions;
using Chaos.Common.Utilities;
using Chaos.Data;
using Chaos.Extensions;
using Chaos.Objects.Panel;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripts.Components;
using Chaos.Scripts.FunctionalScripts.Abstractions;
using Chaos.Scripts.FunctionalScripts.ApplyDamage;
using Chaos.Scripts.SkillScripts.Abstractions;

namespace Chaos.Scripts.SkillScripts.PeasantClass;

public class SapNeedleScript : DamageScript
{
    protected new IApplyDamageScript ApplyDamageScript { get; }

    protected readonly Animation SuccessfulSap = new Animation { AnimationSpeed = 100, TargetAnimation = 127 };
    
    /// <inheritdoc />
    public SapNeedleScript(Skill subject)
        : base(subject)
    {
        ApplyDamageScript = DefaultApplyDamageScript.Create();
    }

    /// <inheritdoc />
    public override void OnUse(ActivationContext context)
    {
        var targets = AbilityComponent.Activate<Creature>(context, AbilityComponentOptions);
        var manaToGive = 0;
        foreach (var target in targets.TargetEntities)
        {
            DamageComponent.ApplyDamage(context, targets.TargetEntities, DamageComponentOptions);
            var halfOfAttackersMana = MathEx.GetPercentOf<int>((int)context.Source.StatSheet.EffectiveMaximumMp, 50);
            var halfOfDefendersMana = MathEx.GetPercentOf<int>((int)target.StatSheet.EffectiveMaximumMp, 50);
            manaToGive = Math.Min(halfOfDefendersMana, halfOfAttackersMana);
            target.StatSheet.SubtractManaPct(50);
        }
        
        var group = context.SourceAisling?.Group?.Where(x => x.WithinRange(context.SourceAisling)).ToList();
        if (group != null)
        {
            foreach (var member in group)
            {
                member.ApplyMana(member, manaToGive / group.Count);
                member.MapInstance.ShowAnimation(SuccessfulSap.GetTargetedAnimation(member.Id));
            }
        }
        else
        {
            context.Source.ApplyMana(context.Source, manaToGive / 2);
            context.Source.MapInstance.ShowAnimation(SuccessfulSap.GetTargetedAnimation(context.Source.Id));
        }
    }
}
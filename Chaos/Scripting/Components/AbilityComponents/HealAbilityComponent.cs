#region
using Chaos.Common.Utilities;
using Chaos.DarkAges.Definitions;
using Chaos.Models.Data;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Components.Abstractions;
using Chaos.Scripting.Components.Execution;
using Chaos.Scripting.FunctionalScripts.Abstractions;
#endregion

namespace Chaos.Scripting.Components.AbilityComponents;

public struct HealAbilityComponent : IComponent
{
    /// <inheritdoc />
    public void Execute(ActivationContext context, ComponentVars vars)
    {
        var options = vars.GetOptions<IHealComponentOptions>();
        var targets = vars.GetTargets<Creature>();
        var sourceScript = vars.GetSourceScript();

        foreach (var target in targets)
        {
            var heal = CalculateHeal(
                context.Source,
                target,
                options.BaseHeal,
                options.PctHpHeal,
                options.HealStat,
                options.HealStatMultiplier,
                options.PctMptoHpHeal);

            if (heal <= 0)
                continue;

            if (target.Effects.Contains("Prevent Heal"))
            {
                if (target.Name != context.SourceAisling?.Name)
                {
                    context.SourceAisling?.SendOrangeBarMessage($"{target.Name} is currently resisting heals.");

                    continue;
                }

                context.SourceAisling?.SendOrangeBarMessage("You are currently resisting heals.");

                continue;
            }

            options.ApplyHealScript.ApplyHeal(
                context.Source,
                target,
                sourceScript,
                heal);
        }
    }

    private int CalculateHeal(
        Creature source,
        Creature target,
        int? baseHeal = null,
        decimal? pctHpHeal = null,
        Stat? healStat = null,
        decimal? healStatMultiplier = null,
        decimal? pctMptoHpHeal = null)
    {
        var finalHeal = baseHeal ?? 0;

        finalHeal += MathEx.GetPercentOf<int>((int)target.StatSheet.EffectiveMaximumHp, pctHpHeal ?? 0);

        if (healStat.HasValue)
            if (healStatMultiplier.HasValue)
                finalHeal += Convert.ToInt32(source.StatSheet.GetEffectiveStat(healStat.Value) * healStatMultiplier.Value);

        if (pctMptoHpHeal.HasValue)
        {
            var pctMpToHealPct = pctMptoHpHeal / 100;
            finalHeal += Convert.ToInt32(source.StatSheet.EffectiveMaximumMp * pctMpToHealPct);
        }

        if (source.StatSheet.EffectiveHealBonusPct > 0)
        {
            var healbonuspct = source.StatSheet.EffectiveHealBonusPct / 100m;
            var healbonus = finalHeal * healbonuspct;
            finalHeal += (int)healbonus;
        }

        if (source.StatSheet.EffectiveHealBonus > 0)
            finalHeal += source.StatSheet.EffectiveHealBonus;

        return finalHeal;
    }

    public interface IHealComponentOptions
    {
        IApplyHealScript ApplyHealScript { get; init; }
        int? BaseHeal { get; init; }
        Stat? HealStat { get; init; }
        decimal? HealStatMultiplier { get; init; }
        decimal? PctHpHeal { get; init; }

        decimal? PctMptoHpHeal { get; init; }
    }
}
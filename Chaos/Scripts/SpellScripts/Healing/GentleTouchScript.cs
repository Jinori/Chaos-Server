using Chaos.Common.Definitions;
using Chaos.Data;
using Chaos.Objects.Panel;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripts.Components;
using Chaos.Scripts.SpellScripts.Abstractions;

namespace Chaos.Scripts.SpellScripts.Healing;

internal class GentleTouchScript : BasicSpellScriptBase, ManaCostComponent.IManaCostComponentOptions
{
    protected ManaCostComponent ManaCostComponent { get; }

    public GentleTouchScript(Spell subject)
        : base(subject) => ManaCostComponent = new ManaCostComponent();

    protected virtual void ApplyHealing(SpellContext context, IEnumerable<Creature> targetEntities)
    {
        foreach (var target in targetEntities)
        {
            var heals = CalculateHealing(context, target);
            target.ApplyHealing(target, heals);
        }
    }

    protected virtual int CalculateHealing(SpellContext context, Creature target)
    {
        var heals = BaseHealing ?? 0;

        if (HealStat.HasValue)
        {
            var multiplier = HealStatMultiplier ?? 1;

            heals += Convert.ToInt32(context.Source.StatSheet.GetEffectiveStat(HealStat.Value) * multiplier);
        }

        return heals;
    }

    /// <inheritdoc />
    public override void OnUse(SpellContext context)
    {
        if (!ManaCostComponent.TryApplyManaCost(context, this))
            return;

        var targets = AbilityComponent.Activate<Creature>(context, this);
        ApplyHealing(context, targets.TargetEntities);
    }

    #region ScriptVars
    protected int? BaseHealing { get; init; }
    protected Stat? HealStat { get; init; }
    protected decimal? HealStatMultiplier { get; init; }
    public int? ManaCost { get; init; }
    public decimal PctManaCost { get; init; }
    #endregion
}
using Chaos.Data;
using Chaos.Objects.Panel;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripting.Components;
using Chaos.Scripting.SpellScripts.Abstractions;

namespace Chaos.Scripting.SpellScripts.Healing;

internal class SalvationScript : BasicSpellScriptBase, ManaCostComponent.IManaCostComponentOptions
{
    public int? ManaCost { get; init; }
    public decimal PctManaCost { get; init; }
    protected ManaCostComponent ManaCostComponent { get; }

    public SalvationScript(Spell subject)
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
        var heals = (int)target.StatSheet.EffectiveMaximumHp;

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
}
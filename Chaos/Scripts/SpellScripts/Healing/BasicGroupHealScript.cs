using Chaos.Common.Definitions;
using Chaos.Data;
using Chaos.Objects.Panel;
using Chaos.Objects.World;
using Chaos.Scripts.Components;
using Chaos.Scripts.SpellScripts.Abstractions;

namespace Chaos.Scripts.SpellScripts.Healing;

internal class BasicGroupHealScript : BasicSpellScriptBase, ManaCostComponent.IManaCostComponentOptions
{
    protected GroupComponent GroupComponent { get; }
    protected GroupComponent.GroupComponentOptions GroupComponentOptions { get; }

    protected ManaCostComponent ManaCostComponent { get; }

    public BasicGroupHealScript(Spell subject)
        : base(subject)
    {
        ManaCostComponent = new ManaCostComponent();
        GroupComponent = new GroupComponent();

        GroupComponentOptions = new GroupComponent.GroupComponentOptions
        {
            SourceScript = this,
            BaseHealing = BaseHealing,
            HealStat = HealStat,
            HealStatMultiplier = HealStatMultiplier,
            Shape = Shape,
            Range = Range,
            BodyAnimation = null,
            Animation = Animation,
            Sound = Sound,
            AnimatePoints = AnimatePoints,
            MustHaveTargets = MustHaveTargets,
            IncludeSourcePoint = IncludeSourcePoint
        };
    }

    /// <inheritdoc />
    public override void OnUse(SpellContext context)
    {
        if (context.SourceAisling?.Group is null)
        {
            context.SourceAisling?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You are not in a group.");

            return;
        }

        if (!ManaCostComponent.TryApplyManaCost(context, this))
            return;

        var targets = GroupComponent.Activate<Aisling>(context, GroupComponentOptions);
        GroupComponent.ApplyHealing(context, targets.targetEntities!, GroupComponentOptions);
    }

    #region ScriptVars
    protected int? BaseHealing { get; init; }
    protected Stat? HealStat { get; init; }
    protected decimal? HealStatMultiplier { get; init; }
    public int? ManaCost { get; init; }
    public decimal PctManaCost { get; init; }
    #endregion
}
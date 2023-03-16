using Chaos.Common.Definitions;
using Chaos.Data;
using Chaos.Objects.Panel;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripting.Abstractions;
using Chaos.Scripting.Components;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.SpellScripts.Abstractions;

namespace Chaos.Scripting.SpellScripts;

public class HealScript : BasicSpellScriptBase, HealComponent.IHealComponentOptions, ManaCostComponent.IManaCostComponentOptions
{
    public IApplyHealScript ApplyHealScript { get; init; }
    public IScript SourceScript { get; init; }
    protected HealComponent HealComponent { get; }
    protected GroupComponent GroupComponent { get; }
    protected GroupComponent.GroupComponentOptions GroupComponentOptions { get; }
    protected ManaCostComponent ManaCostComponent { get; }

    /// <inheritdoc />
    public HealScript(Spell subject)
        : base(subject)
    {
        ManaCostComponent = new ManaCostComponent();
        ApplyHealScript = FunctionalScripts.ApplyHealing.HealScript.Create();
        GroupComponent = new GroupComponent();
        GroupComponentOptions = new GroupComponent.GroupComponentOptions
        {
            SourceScript = this,
            Shape = Shape,
            Range = Range,
            BodyAnimation = null,
            Animation = Animation,
            Sound = Sound,
            AnimatePoints = AnimatePoints,
            MustHaveTargets = MustHaveTargets,
            IncludeSourcePoint = IncludeSourcePoint
        };
        HealComponent = new HealComponent();
        SourceScript = this;
    }

    /// <inheritdoc />
    public override void OnUse(SpellContext context)
    {
        if (!ManaCostComponent.TryApplyManaCost(context, this))
            return;

        if (TargetGroup is true)
        {
            if (context.SourceAisling?.Group is null)
            {
                context.SourceAisling?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You are not in a group.");
                return;
            }
            
            var targets1 = GroupComponent.Activate<Creature>(context, GroupComponentOptions);
            HealComponent.ApplyHeal(context, targets1.targetEntities!, this);
            context.SourceAisling?.SendActiveMessage($"You cast {Subject.Template.Name}");
            return;
        }
        
        var targets = AbilityComponent.Activate<Creature>(context, this);
        HealComponent.ApplyHeal(context, targets.TargetEntities, this);
        context.SourceAisling?.SendActiveMessage($"You cast {Subject.Template.Name}");
    }

    #region ScriptVars

    private bool? TargetGroup { get; init; }
    public int? BaseHeal { get; init; }
    public Stat? HealStat { get; init; }
    public decimal? HealStatMultiplier { get; init; }
    public decimal? PctHpHeal { get; init; }
    public int? ManaCost { get; init; }
    public decimal PctManaCost { get; init; }
    #endregion
}
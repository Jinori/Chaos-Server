using Chaos.Common.Definitions;
using Chaos.Data;
using Chaos.Objects.Panel;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripting.Abstractions;
using Chaos.Scripts.Components;
using Chaos.Scripts.FunctionalScripts.Abstractions;
using Chaos.Scripts.FunctionalScripts.ApplyDamage;
using Chaos.Scripts.SpellScripts.Abstractions;

namespace Chaos.Scripts.SpellScripts.Damage;

public class DamageScript : BasicSpellScriptBase, DamageComponent.IDamageComponentOptions, ManaCostComponent.IManaCostComponentOptions
{
    protected DamageComponent DamageComponent { get; }
    protected ManaCostComponent ManaCostComponent { get; }

    /// <inheritdoc />
    public DamageScript(Spell subject)
        : base(subject)
    {
        ApplyDamageScript = DefaultApplyDamageScript.Create();
        DamageComponent = new DamageComponent();
        ManaCostComponent = new ManaCostComponent();
        SourceScript = this;
    }

    /// <inheritdoc />
    public override void OnUse(SpellContext context)
    {
        if (!ManaCostComponent.TryApplyManaCost(context, this))
            return;

        context.Source.StatSheet.SetOffensiveCastElement(OffensiveElement ?? Element.None);

        var targets = AbilityComponent.Activate<Creature>(context, this);
        DamageComponent.ApplyDamage(context, targets.TargetEntities, this);
        context.SourceAisling?.SendActiveMessage($"You cast {Subject.Template.Name}");
    }

    #region ScriptVars
    public IApplyDamageScript ApplyDamageScript { get; init; }
    public int? BaseDamage { get; init; }
    public Stat? DamageStat { get; init; }
    
    public Element? OffensiveElement { get; init; }
    public decimal? DamageStatMultiplier { get; init; }
    public decimal? PctHpDamage { get; init; }
    public IScript SourceScript { get; init; }
    public int? ManaCost { get; init; }
    public decimal PctManaCost { get; init; }
    
    #endregion
}
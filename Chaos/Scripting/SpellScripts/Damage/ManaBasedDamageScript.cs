using Chaos.Common.Definitions;
using Chaos.Data;
using Chaos.Objects.Panel;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripting.Abstractions;
using Chaos.Scripting.Components;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ApplyDamage;
using Chaos.Scripting.SpellScripts.Abstractions;

namespace Chaos.Scripting.SpellScripts.Damage;

public class ManaBasedDamageScript : BasicSpellScriptBase, ManaBasedDamageComponent.IManaBasedDamageComponentOptions, ManaCostComponent.IManaCostComponentOptions
{
    protected ManaBasedDamageComponent ManaBasedDamageComponent { get; }
    protected ManaCostComponent ManaCostComponent { get; }

    public ManaBasedDamageScript(Spell subject)
        : base(subject)
    {
        ApplyDamageScript = DefaultApplyDamageScript.Create();
        ManaBasedDamageComponent = new ManaBasedDamageComponent();
        ManaCostComponent = new ManaCostComponent();
        SourceScript = this;
    }

    public override void OnUse(SpellContext context)
    {
       
        if (!ManaCostComponent.TryApplyManaCost(context, this))
            return;
        
        var targets = AbilityComponent.Activate<Creature>(context, this);
        context.SourceAisling?.SendActiveMessage($"You cast {Subject.Template.Name}");
        ManaBasedDamageComponent.ApplyDamage(context, targets.TargetEntities, this);
    }

    public IScript SourceScript { get; }
    public IApplyDamageScript ApplyDamageScript { get; }

    #region ScriptVars
    public int? BaseDamage { get; }
    public decimal? BaseDamageMultiplier { get; }
    public decimal? PctOfMana { get; }
    public decimal? PctOfManaMultiplier { get; }
    public decimal? FinalMultiplier { get; }
    public Element? Element { get; }
    public int? ManaCost { get; init; }
    public decimal PctManaCost { get; init; }
    
    #endregion
    
}
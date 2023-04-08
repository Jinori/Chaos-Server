using Chaos.Common.Definitions;
using Chaos.Data;
using Chaos.Objects.Panel;
using Chaos.Objects.World;
using Chaos.Scripting.Abstractions;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.SpellScripts.Abstractions;


namespace Chaos.Scripting.SpellScripts.Damage;



public class CreantDamageScript : BasicSpellScriptBase
{
    public IApplyDamageScript ApplyDamageScript { get; init; }
    public IScript SourceScript { get; init; }

    /// <inheritdoc />
    public CreantDamageScript(Spell subject)
        : base(subject)
    {
        SourceScript = this;
    }

    
    private readonly Animation _animation = new()
    {
        TargetAnimation = 7,
        AnimationSpeed = 100
    };
    
    /// <inheritdoc />
    public override void OnUse(SpellContext context)
    {
        foreach (var aisling in context.Source.MapInstance.GetEntities<Aisling>())
        {
            if (aisling.Inventory.Contains("Silver Wolf Leather"))
                return;
            
            aisling.StatSheet.SetHp(1);
            aisling.Client.SendAttributes(StatUpdateType.Vitality);
            aisling.SendOrangeBarMessage("Creant's Grasp has infected your body! Health depleted.");
            aisling.Animate(_animation, aisling.Id);
        }
    }

    #region ScriptVars
    public int? BaseDamage { get; init; }
    public Stat? DamageStat { get; init; }
    public decimal? DamageStatMultiplier { get; init; }
    public decimal? PctHpDamage { get; init; }
    public Element? Element { get; init; }
    public int? ManaCost { get; init; }
    public decimal PctManaCost { get; init; }
    
    #endregion
}
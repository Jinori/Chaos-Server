using Chaos.Common.Utilities;
using Chaos.DarkAges.Definitions;
using Chaos.Extensions;
using Chaos.Formulae;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Components.EffectComponents;
using Chaos.Scripting.Components.Execution;
using Chaos.Scripting.EffectScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ApplyDamage;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.EffectScripts.Wizard;

public class BurnEffect : ContinuousAnimationEffectBase, HierarchicalEffectComponent.IHierarchicalEffectComponentOptions
{
    /// <inheritdoc />
    protected override TimeSpan Duration { get; set; } = TimeSpan.FromSeconds(10);

    public List<string> EffectNameHierarchy { get; init; } =
        [
            "Burn",
            "Firestorm",
            "Fire Punch",
            "Small Firestorm"
        ];

    private readonly IApplyDamageScript ApplyDamageScript;

    protected virtual decimal AislingBurnPercentage => 2.5m;

    /// <inheritdoc />
    protected override Animation Animation { get; } = new()
    {
        AnimationSpeed = 100,
        TargetAnimation = 60,
        Priority = 15
    };

    /// <inheritdoc />
    /// <inheritdoc />
    protected override IIntervalTimer AnimationInterval { get; } = new IntervalTimer(TimeSpan.FromSeconds(1), false);

    protected virtual decimal BurnPercentage => 5m;

    /// <inheritdoc />
    protected override IIntervalTimer Interval { get; } = new IntervalTimer(TimeSpan.FromMilliseconds(1000), false);

    /// <inheritdoc />
    public override sealed byte Icon => 31;

    /// <inheritdoc />
    public override string Name => "Burn";

    private int DmgPerTick;

    public override void OnApplied()
    {
        AislingSubject?.SendOrangeBarMessage("Your body catches fire.");
        DmgPerTick = GetVar<int>("dmgPerTick");
    }

    public override void OnTerminated()
        => AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You are no longer burning.");

    public BurnEffect()
    {
        var applyDamageScript = ApplyNonAttackDamageScript.Create();
        var formula = DamageFormulae.ElementalEffect;
        
        applyDamageScript.DamageFormula = formula;
        ApplyDamageScript = applyDamageScript;
    }
    
    public override bool ShouldApply(Creature source, Creature target)
    {
        if (target.Name == "Shamensyth")
            return false;

        if (target.StatSheet.DefenseElement == Element.Fire)
            return false;

        if (target.Effects.TryGetEffect("burn", out var effect) && effect is BurnEffect burnEffect)
        {
            var existingDmgPerTick = burnEffect.GetVar<int>("dmgPerTick");

            if (DmgPerTick > existingDmgPerTick)
            {
                target.Effects.Dispel("burn");

                return false;
            }
        }

        var execution = new ComponentExecutor(source, target).WithOptions(this)
                                                             .ExecuteAndCheck<HierarchicalEffectComponent>();

        return execution is not null;
    }
    
    /// <inheritdoc />
    protected override void OnIntervalElapsed()
    {
        if (Subject.IsGodModeEnabled() || Subject.Effects.Contains("invulnerability"))
        {
            Subject.Effects.Terminate("burn");
            
            return;
        }

        if (Subject.StatSheet.DefenseElement == Element.Fire)
            return;

        var maxPct = Subject is Aisling ? 3.3m : 5m;
        var maxPctDmg = MathEx.GetPercentOf<int>((int)Subject.StatSheet.EffectiveMaximumHp, maxPct);
        var dmgPerTick = Math.Min(maxPctDmg, DmgPerTick);

        ApplyDamageScript.ApplyDamage(
            Source,
            Subject,
            SourceScript ?? this,
            dmgPerTick,
            Element.Fire);
    }
}
using Chaos.DarkAges.Definitions;
using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Formulae;
using Chaos.Geometry.Abstractions;
using Chaos.Geometry.Abstractions.Definitions;
using Chaos.Models.Data;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.EffectScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ApplyDamage;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.EffectScripts.Wizard;

public class SalSplashEffect : ContinuousAnimationEffectBase
{
    /// <inheritdoc />
    protected override TimeSpan Duration { get; set; } = TimeSpan.FromSeconds(6);
    
    /// <inheritdoc />
    protected override Animation Animation { get; } = new()
    {
        AnimationSpeed = 100,
        TargetAnimation = 234
    };

    /// <inheritdoc />
    protected override IIntervalTimer AnimationInterval { get; } = new IntervalTimer(TimeSpan.FromSeconds(2), false);

    private readonly IApplyDamageScript ApplyDamageScript;

    protected Animation CreatureAnimation { get; } = new()
    {
        AnimationSpeed = 100,
        TargetAnimation = 58
    };

    /// <inheritdoc />
    protected override IIntervalTimer Interval { get; } = new IntervalTimer(TimeSpan.FromSeconds(1), false);

    /// <inheritdoc />
    public override byte Icon => 38;

    /// <inheritdoc />
    public override string Name => "Sal Splash";
    
    private int DmgPerTick => GetVar<int>("dmgPerTick");

    public override void OnApplied()
        => AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Powerful water surrounds you.");
    
    public SalSplashEffect()
    {
        var applyDamageScript = ApplyNonAttackDamageScript.Create();
        var formula = DamageFormulae.ElementalEffect;
        
        applyDamageScript.DamageFormula = formula;
        ApplyDamageScript = applyDamageScript;
    }
    
    /// <inheritdoc />
    protected override void OnIntervalElapsed()
    {
        if (Subject.MapInstance != Source.MapInstance)
        {
            Subject.Effects.Terminate("Sal Splash");

            return;
        }

        // Animate the subject of the effect
        Subject.Animate(Animation);
        
        // Retrieve and filter targets at those points
        var targets = Subject.MapInstance
                             .GetEntitiesWithinRange<Creature>(Subject, 1)
                             .WithFilter(Source, TargetFilter.HostileOnly | TargetFilter.AliveOnly)
                             .Where(x => !x.Equals(Subject))
                             .ToList();
        
        // Apply damage to each valid target
        foreach (var target in targets)
        {
            if (target.StatSheet.DefenseElement == Element.Water)
                continue;

            ApplyDamageScript.ApplyDamage(
                Source,
                target,
                SourceScript ?? this,
                DmgPerTick,
                Element.Water);
            
            target.Animate(CreatureAnimation);
        }
    }

    public override void OnTerminated() => AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Splash has worn off.");

    /// <inheritdoc />
    public override bool ShouldApply(Creature source, Creature target)
    {
        if (target.IsFriendlyTo(source) || target.IsGodModeEnabled() || target.Effects.Contains("Invulnerability"))
            return false;

        if (target.StatSheet.DefenseElement == Element.Water)
            return false;
        
        if (target.Effects.TryGetEffect("salsplash", out var effect) && effect is SalSplashEffect salSplashEffect)
        {
            var existingDmgPerTick = salSplashEffect.GetVar<int>("dmgPerTick");

            if (DmgPerTick > existingDmgPerTick)
            {
                target.Effects.Dispel("salsplash");

                return false;
            }
        }

        var splashEffects = new[]
        {
            "Beag Sal Splash",
            "Sal Splash",
            "Mor Sal Splash",
            "Ard Sal Splash"
        };

        return !splashEffects.Any(target.Effects.Contains);
    }
}
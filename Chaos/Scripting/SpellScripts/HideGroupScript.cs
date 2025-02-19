using Chaos.DarkAges.Definitions;
using Chaos.Definitions;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Components.AbilityComponents;
using Chaos.Scripting.Components.Execution;
using Chaos.Scripting.SpellScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.SpellScripts;

public class HideGroupScript : ConfigurableSpellScriptBase,
                               SpellComponent<Creature>.ISpellComponentOptions,
                               GroupHideEffectAbilityComponent.IApplyEffectComponentOptions
{
    public bool AnimatePoints { get; init; }
    public Animation? Animation { get; init; }
    public ushort? AnimationSpeed { get; init; }
    public BodyAnimation BodyAnimation { get; init; }
    public int? EffectApplyChance { get; init; }
    public TimeSpan? EffectDurationOverride { get; init; }
    public IEffectFactory EffectFactory { get; init; }
    public string? EffectKey { get; init; }

    public List<string>? EffectKeysToBreak { get; set; }

    public int? ExclusionRange { get; init; }

    public TargetFilter Filter { get; init; }
    public bool IgnoreMagicResistance { get; init; }
    public int? ManaCost { get; init; }
    public bool MustHaveTargets { get; init; }
    public decimal PctManaCost { get; init; }
    public int Range { get; init; }
    public bool? ScaleBodyAnimationSpeedByAttackSpeed { get; init; }
    public AoeShape Shape { get; init; }
    public bool SingleTarget { get; init; }
    public byte? Sound { get; init; }
    public int SplashChance { get; init; }
    public int SplashDistance { get; init; }
    public TargetFilter SplashFilter { get; init; }
    public bool StopOnFirstHit { get; init; }
    public bool StopOnWalls { get; init; }

    public HideGroupScript(Spell subject, IEffectFactory effectFactory)
        : base(subject)
        => EffectFactory = effectFactory;

    /// <inheritdoc />
    public override void OnUse(SpellContext context)
        => new ComponentExecutor(context).WithOptions(this)
                                         .ExecuteAndCheck<SpellComponent<Creature>>()
                                         ?.Execute<GroupHideEffectAbilityComponent>();
}
using Chaos.DarkAges.Definitions;
using Chaos.Definitions;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Components.AbilityComponents;
using Chaos.Scripting.SkillScripts.Abstractions;

namespace Chaos.Scripting.SkillScripts.Priest;

public class SummonPet : ConfigurableSkillScriptBase, GenericAbilityComponent<Creature>.IAbilityComponentOptions
{
    /// <inheritdoc />
    public bool AnimatePoints { get; init; }

    /// <inheritdoc />
    public Animation? Animation { get; init; }

    /// <inheritdoc />
    public ushort? AnimationSpeed { get; init; }

    /// <inheritdoc />
    public BodyAnimation BodyAnimation { get; init; }

    public List<string>? EffectKeysToBreak { get; set; }

    /// <inheritdoc />

    public int? ExclusionRange { get; init; }

    /// <inheritdoc />
    public TargetFilter Filter { get; init; }

    /// <inheritdoc />
    public int? ManaCost { get; init; }

    /// <inheritdoc />
    public bool MustHaveTargets { get; init; }

    /// <inheritdoc />
    public decimal PctManaCost { get; init; }

    /// <inheritdoc />
    public int Range { get; init; }

    public bool? ScaleBodyAnimationSpeedByAttackSpeed { get; init; }

    /// <inheritdoc />
    public AoeShape Shape { get; init; }

    /// <inheritdoc />
    public bool SingleTarget { get; init; }

    /// <inheritdoc />
    public byte? Sound { get; init; }

    public int SplashChance { get; init; }
    public int SplashDistance { get; init; }
    public TargetFilter SplashFilter { get; init; }
    public bool StopOnFirstHit { get; init; }
    public bool StopOnWalls { get; init; }

    /// <inheritdoc />
    public SummonPet(Skill subject)
        : base(subject) { }

    /// <inheritdoc />
    public override void OnUse(ActivationContext context)
        => context.SourceAisling?.SendActiveMessage("You've learned to care for a pet. Please use the collar.");
}
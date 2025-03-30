using Chaos.DarkAges.Definitions;
using Chaos.Definitions;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Components.AbilityComponents;
using Chaos.Scripting.Components.Execution;
using Chaos.Scripting.SpellScripts.Abstractions;

namespace Chaos.Scripting.SpellScripts.Damage;

public class PullScript : ConfigurableSpellScriptBase,
                          SpellComponent<Creature>.ISpellComponentOptions,
                          PullAggroComponent.IAddAggroComponentOptions,
                          NotifyTargetComponent.INotifyTargetComponentOptions
{
    public List<string>? EffectKeysToBreak { get; set; }

    public int? HealthCost { get; init; }
    public decimal PctHealthCost { get; init; }

    /// <inheritdoc />
    public PullScript(Spell subject)
        : base(subject) { }

    /// <inheritdoc />
    public override void OnUse(SpellContext context)
        => new ComponentExecutor(context).WithOptions(this)
                                         .ExecuteAndCheck<SpellComponent<Creature>>()
                                         ?.Execute<PullAggroComponent>()
                                         .Execute<NotifyTargetComponent>();

    #region ScriptVars
    public int? ExclusionRange { get; init; }
    public bool StopOnWalls { get; init; }
    public bool StopOnFirstHit { get; init; }

    public TargetFilter Filter { get; init; }
    public int SplashChance { get; init; }
    public int SplashDistance { get; init; }
    public TargetFilter SplashFilter { get; init; }
    public bool MustHaveTargets { get; init; }
    public int Range { get; init; }
    public AoeShape Shape { get; init; }
    public bool SingleTarget { get; init; }
    public bool IgnoreMagicResistance { get; init; }
    public byte? Sound { get; init; }
    public ushort? AnimationSpeed { get; init; }
    public BodyAnimation BodyAnimation { get; init; }
    public bool? ScaleBodyAnimationSpeedByAttackSpeed { get; init; }
    public bool AnimatePoints { get; init; }
    public Animation? Animation { get; init; }
    public int? ManaCost { get; init; }
    public decimal PctManaCost { get; init; }
    public int? AggroAmount { get; init; }
    public Stat? AggroMultiplier { get; init; }
    #endregion
}
using Chaos.Common.Definitions;
using Chaos.Definitions;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Components.AbilityComponents;
using Chaos.Scripting.Components.Execution;
using Chaos.Scripting.SkillScripts.Abstractions;

namespace Chaos.Scripting.SkillScripts.Monk;

public class PullSkillScript : ConfigurableSkillScriptBase,
                             GenericAbilityComponent<Creature>.IAbilityComponentOptions,
                             PullAggroComponent.IAddAggroComponentOptions

{
    /// <inheritdoc />
    public PullSkillScript(Skill subject)
        : base(subject)
    {
    }

    public override void OnUse(ActivationContext context) =>
        new ComponentExecutor(context)
            .WithOptions(this)
            .ExecuteAndCheck<GenericAbilityComponent<Creature>>()
            ?
            .Execute<PullAggroComponent>();

    public int SplashChance { get; init; }
    public int SplashDistance { get; init; }
    public TargetFilter SplashFilter { get; init; }
    public int? AggroAmount { get; init; }
    public Stat? AggroMultiplier { get; init; }
    public bool ExcludeSourcePoint { get; init; }
    public TargetFilter Filter { get; init; }
    public bool MustHaveTargets { get; init; }
    public int Range { get; init; }
    public AoeShape Shape { get; init; }
    public bool SingleTarget { get; init; }
    public byte? Sound { get; init; }
    public ushort? AnimationSpeed { get; init; }
    public BodyAnimation BodyAnimation { get; init; }
    public bool AnimatePoints { get; init; }
    public Animation? Animation { get; init; }
    public int? ManaCost { get; init; }
    public decimal PctManaCost { get; init; }
    public bool ShouldNotBreakHide { get; init; }
}
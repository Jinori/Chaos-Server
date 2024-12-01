using Chaos.Common.Utilities;
using Chaos.DarkAges.Definitions;
using Chaos.Extensions;
using Chaos.Models.Data;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Components.EffectComponents;
using Chaos.Scripting.Components.Execution;
using Chaos.Scripting.EffectScripts.Abstractions;
using Chaos.Scripting.MonsterScripts.Boss;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.EffectScripts.Warrior;

public sealed class BeagSuainEffect : ContinuousAnimationEffectBase, HierarchicalEffectComponent.IHierarchicalEffectComponentOptions
{
    /// <inheritdoc />
    protected override TimeSpan Duration { get; set; } = TimeSpan.FromSeconds(8);

    /// <inheritdoc />
    public List<string> EffectNameHierarchy { get; init; } =
        [
            "Beag Suain",
            "Earth Punch"
        ];

    /// <inheritdoc />
    protected override Animation Animation { get; } = new()
    {
        AnimationSpeed = 100,
        TargetAnimation = 691,
        Priority = 90
    };

    /// <inheritdoc />
    protected override IIntervalTimer AnimationInterval { get; } = new IntervalTimer(TimeSpan.FromSeconds(1), false);

    /// <inheritdoc />
    protected override IIntervalTimer Interval { get; } = new IntervalTimer(TimeSpan.FromMilliseconds(1000), false);

    /// <inheritdoc />
    public override byte Icon => 117;

    /// <inheritdoc />
    public override string Name => "Beag Suain";

    public override void OnApplied()
        => AislingSubject?.Client.SendServerMessage(
            ServerMessageType.OrangeBar1,
            "After taking a strike, you feel as though you cannot move.");

    public override void OnTerminated() => AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You feel fine again.");

    public override bool ShouldApply(Creature source, Creature target)
    {
        if (target.Script.Is<ThisIsABossScript>())
            return false;

        if (target.IsGodModeEnabled())
            return false;

        if ((target.IsAsgalled() && IntegerRandomizer.RollChance(50))
            || (target.IsEarthenStanced() && IntegerRandomizer.RollChance(30))
            || (target.IsRockStanced() && IntegerRandomizer.RollChance(70)))
            return false;

        var execution = new ComponentExecutor(source, target).WithOptions(this)
                                                             .ExecuteAndCheck<HierarchicalEffectComponent>();

        return execution is not null;
    }

    /// <inheritdoc />
    protected override void OnIntervalElapsed() => Subject.Animate(Animation);
}
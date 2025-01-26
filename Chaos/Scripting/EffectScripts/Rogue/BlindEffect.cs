using Chaos.DarkAges.Definitions;
using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.EffectScripts.Abstractions;
using Chaos.Scripting.MonsterScripts.Boss;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.EffectScripts.Rogue;

public sealed class BlindEffect : ContinuousAnimationEffectBase
{
    /// <inheritdoc />
    protected override TimeSpan Duration { get; set; } = TimeSpan.FromSeconds(20);

    /// <inheritdoc />
    protected override Animation Animation { get; } = new()
    {
        AnimationSpeed = 75,
        TargetAnimation = 391,
        Priority = 80
    };

    /// <inheritdoc />
    protected override IIntervalTimer AnimationInterval { get; } = new IntervalTimer(TimeSpan.FromSeconds(1), false);

    /// <inheritdoc />
    protected override IIntervalTimer Interval { get; } = new IntervalTimer(TimeSpan.FromMilliseconds(1000), false);

    /// <inheritdoc />
    public override byte Icon => 2;

    /// <inheritdoc />
    public override string Name => "Blind";

    public override void OnApplied()
    {
        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You are unable to see.");
        Subject.SetVision(VisionType.TrueBlind);
    }

    /// <inheritdoc />
    protected override void OnIntervalElapsed() { }

    public override void OnTerminated()
    {
        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You can see again.");
        Subject.SetVision(VisionType.Normal);
    }

    public override bool ShouldApply(Creature source, Creature target)
    {
        if (target.IsGodModeEnabled())
            return false;

        if (target.Script.Is<ThisIsABossScript>())
            return false;

        if (target.StatSheet.DefenseElement == Element.Holy)
            return false;

        if (target.Effects.Contains("Blind"))
        {
            (source as Aisling)?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "That target is already blinded.");

            return false;
        }

        if (source is Aisling && target is Aisling)
            Duration = TimeSpan.FromSeconds(4);

        return true;
    }
}
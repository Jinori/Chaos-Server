using System.Collections.Immutable;
using Chaos.Common.Definitions;
using Chaos.Definitions;
using Chaos.Models.Data;
using Chaos.Scripting.EffectScripts.Abstractions;

namespace Chaos.Scripting.EffectScripts.Priest;

public class BeagAiteEffect : HierarchicalEffectBase
{
    protected override TimeSpan Duration { get; set; } = TimeSpan.FromMinutes(8);
    protected override ImmutableArray<string> ReplaceHierarchy { get; } =
    [
        "ard naomh aite",
        "mor naomh aite",
        "naomh aite",
        "beag naomh aite"
    ];
    
    private Animation? Animation { get; } = new()
    {
        TargetAnimation = 125,
        AnimationSpeed = 100
    };
    
    protected byte? Sound => 123;
    public override byte Icon => 9;
    public override string Name => "beag naomh aite";

    public override void OnApplied()
    {
        base.OnApplied();

        if (!Subject.Status.HasFlag(Status.BeagAite))
            Subject.Status = Status.BeagAite;

        Subject.Animate(Animation!);
        AislingSubject?.Client.SendAttributes(StatUpdateType.Full);
        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Your defenses have been blessed.");
    }

    public override void OnDispelled() => OnTerminated();

    public override void OnTerminated()
    {
        if (Subject.Status.HasFlag(Status.BeagAite))
            Subject.Status &= ~Status.BeagAite;

        AislingSubject?.Client.SendAttributes(StatUpdateType.Full);
        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Your defenses have returned to normal.");
    }
}
using System.Collections.Immutable;
using Chaos.Common.Definitions;
using Chaos.Models.Data;
using Chaos.Scripting.EffectScripts.Abstractions;

namespace Chaos.Scripting.EffectScripts.Priest;

public class ArmachdEffect : HierarchicalEffectBase
{
    protected override TimeSpan Duration { get; set; } = TimeSpan.FromMinutes(8);
    protected Animation? Animation { get; } = new()
    {
        TargetAnimation = 20,
        AnimationSpeed = 100
    };
    protected override ImmutableArray<string> ReplaceHierarchy { get; } =
    [
        "armachd"
    ];
    public override byte Icon => 0;
    public override string Name => "armachd";

    protected byte? Sound => 122;

    public override void OnApplied()
    {
        base.OnApplied();

        var attributes = new Attributes
        {
            Ac = 10
        };

        Subject.Animate(Animation!);
        Subject.StatSheet.SubtractBonus(attributes);
        AislingSubject?.Client.SendAttributes(StatUpdateType.Full);
        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Armor increased.");
    }

    public override void OnDispelled() => OnTerminated();

    public override void OnTerminated()
    {
        var attributes = new Attributes
        {
            Ac = -10
        };

        Subject.StatSheet.SubtractBonus(attributes);
        AislingSubject?.Client.SendAttributes(StatUpdateType.Full);
        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Armor has returned to normal.");
    }
}
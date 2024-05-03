using System.Collections.Immutable;
using Chaos.Common.Definitions;
using Chaos.Models.Data;
using Chaos.Scripting.EffectScripts.Abstractions;

namespace Chaos.Scripting.EffectScripts.Priest;

public class BeannaichEffect : HierarchicalEffectBase
{
    protected override TimeSpan Duration { get; set; } = TimeSpan.FromMinutes(5);

    public override byte Icon => 105;
    public override string Name => "beannaich";
    
    private Animation? Animation { get; } = new()
    {
        TargetAnimation = 125,
        AnimationSpeed = 100
    };
    
    protected byte? Sound => 123;
    protected override ImmutableArray<string> ReplaceHierarchy { get; } =
    [
        "mor beannaich",
        "beannaich"
    ];

    public override void OnApplied()
    {
        base.OnApplied();

        var attributes = new Attributes
        {
            Hit = 10
        };

        Subject.Animate(Animation!);
        Subject.StatSheet.AddBonus(attributes);
        AislingSubject?.Client.SendAttributes(StatUpdateType.Full);
        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Hit increased.");
    }

    public override void OnDispelled() => OnTerminated();

    public override void OnTerminated()
    {
        var attributes = new Attributes
        {
            Hit = 10
        };

        Subject.StatSheet.SubtractBonus(attributes);
        AislingSubject?.Client.SendAttributes(StatUpdateType.Full);
        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Hit has returned to normal.");
    }
}
using System.Diagnostics;
using Chaos.Common.Definitions;
using Chaos.Data;
using Chaos.Scripting.EffectScripts.Abstractions;

namespace Chaos.Scripting.EffectScripts.Wizard;

public class BeagCradhEffect : NonOverwritableEffectBase
{
    /// <inheritdoc />
    public override byte Icon => 5;
    /// <inheritdoc />
    public override string Name => "beag cradh";
    /// <inheritdoc />
    protected override Animation? Animation { get; } = new()
    {
        TargetAnimation = 45,
        AnimationSpeed = 100
    };
    /// <inheritdoc />
    protected override IReadOnlyCollection<string> ConflictingEffectNames { get; } = new[]
    {
        "ard cradh",
        "mor cradh",
        "cradh",
        "beag cradh"
    };
    /// <inheritdoc />
    protected override TimeSpan Duration { get; } = TimeSpan.FromMinutes(2);
    /// <inheritdoc />
    protected override byte? Sound => 27;
    public override void OnDispelled() => OnTerminated();
    
    public override void OnApplied()
    {
        base.OnApplied();

        var attributes = new Attributes
        {
            Ac = -20
        };

        Subject.StatSheet.SubtractBonus(attributes);
        AislingSubject?.Client.SendAttributes(StatUpdateType.Full);
        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You've been cursed by beag cradh! AC lowered!");
    }
    public override void OnTerminated()
    {
        var attributes = new Attributes
        {
            Ac = -20
        };

        Subject.StatSheet.AddBonus(attributes);
        AislingSubject?.Client.SendAttributes(StatUpdateType.Full);
        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "beag cradh curse has been lifted.");
    }
}

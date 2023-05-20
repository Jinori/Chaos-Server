using Chaos.Common.Definitions;
using Chaos.Models.Data;
using Chaos.Scripting.EffectScripts.Abstractions;

namespace Chaos.Scripting.EffectScripts.Wizard;

public class MorCradhEffect : NonOverwritableEffectBase
{
    /// <inheritdoc />
    public override byte Icon => 83;
    /// <inheritdoc />
    public override string Name => "mor cradh";
    /// <inheritdoc />
    protected override Animation? Animation { get; } = new()
    {
        TargetAnimation = 18,
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
    protected override TimeSpan Duration { get; } = TimeSpan.FromMinutes(5);
    /// <inheritdoc />
    protected override byte? Sound => 27;
    public override void OnDispelled() => OnTerminated();
    
    public override void OnApplied()
    {
        base.OnApplied();

        var attributes = new Attributes
        {
            Ac = -50,
            MagicResistance = 2
        };
        
        Subject.StatSheet.SubtractBonus(attributes);
        AislingSubject?.Client.SendAttributes(StatUpdateType.Full);
        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You've been cursed by cradh! AC and MR lowered!");
    }
    public override void OnTerminated()
    {
        var attributes = new Attributes
        {
            Ac = -50,
            MagicResistance = 2
        };

        Subject.StatSheet.AddBonus(attributes);
        AislingSubject?.Client.SendAttributes(StatUpdateType.Full);
        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "mor cradh curse has been lifted.");
    }
}

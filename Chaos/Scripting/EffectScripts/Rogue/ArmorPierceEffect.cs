using Chaos.Common.Definitions;
using Chaos.Models.Data;
using Chaos.Scripting.EffectScripts.Abstractions;

namespace Chaos.Scripting.EffectScripts.Rogue;

public class ArmorPierceEffect : NonOverwritableEffectBase
{
    /// <inheritdoc />
    protected override TimeSpan Duration { get; set; } = TimeSpan.FromSeconds(6);
    /// <inheritdoc />
    protected override Animation? Animation { get; } = new()
    {
        TargetAnimation = 204,
        AnimationSpeed = 100
    };
    /// <inheritdoc />
    protected override IReadOnlyCollection<string> ConflictingEffectNames { get; } = new[]
    {
        "Armor Pierce"
    };
    /// <inheritdoc />
    public override byte Icon => 65;
    /// <inheritdoc />
    public override string Name => "Armor Pierce";
    /// <inheritdoc />
    protected override byte? Sound => 22;

    public override void OnApplied()
    {
        base.OnApplied();

        var attributes = new Attributes
        {
            Ac = -15,
            MagicResistance = 10
        };

        Subject.StatSheet.SubtractBonus(attributes);
        AislingSubject?.Client.SendAttributes(StatUpdateType.Full);
        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Your armor has been pierced! AC and MR lowered!");
    }

    public override void OnDispelled() => OnTerminated();

    public override void OnTerminated()
    {
        var attributes = new Attributes
        {
            Ac = -15,
            MagicResistance = 10
        };

        Subject.StatSheet.AddBonus(attributes);
        AislingSubject?.Client.SendAttributes(StatUpdateType.Full);
        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Armor has been repaired, AC and MR restored.");
    }
}
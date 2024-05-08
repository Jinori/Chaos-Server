using Chaos.Common.Definitions;
using Chaos.Definitions;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.EffectScripts.Abstractions;

namespace Chaos.Scripting.EffectScripts.Wizard;

public class ArdCradhEffect : NonOverwritableEffectBase
{
    /// <inheritdoc />
    protected override TimeSpan Duration { get; set; } = TimeSpan.FromMinutes(3);
    /// <inheritdoc />
    protected override Animation? Animation { get; } = new()
    {
        TargetAnimation = 43,
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
    public override byte Icon => 63;
    /// <inheritdoc />
    public override string Name => "ard cradh";
    /// <inheritdoc />
    protected override byte? Sound => 27;

    public override void OnApplied()
    {
        base.OnApplied();

        var attributes = new Attributes
        {
            Ac = -50,
            MagicResistance = 30
        };

        Subject.StatSheet.SubtractBonus(attributes);
        AislingSubject?.Client.SendAttributes(StatUpdateType.Full);
        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, $"You've been cursed by {Subject.Name} AC and MR lowered!");
    }

    public override void OnDispelled() => OnTerminated();

    public override void OnTerminated()
    {
        var attributes = new Attributes
        {
            Ac = -50,
            MagicResistance = 30
        };

        Subject.StatSheet.AddBonus(attributes);
        AislingSubject?.Client.SendAttributes(StatUpdateType.Full);
        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Ard Cradh curse has been lifted.");
    }

    /// <inheritdoc />
    public override bool ShouldApply(Creature source, Creature target)
    {
        if (source.Status.HasFlag(Status.PreventAffliction))
        {
            (source as Aisling)?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You are prevented from afflicting curses.");

            return false;
        }

        if (target.Effects.Contains("preventrecradh"))
        {
            (source as Aisling)?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Target cannot be cursed at this time.");

            return false;
        }

        if (target.Effects.Contains("ard cradh")
            || target.Effects.Contains("mor cradh")
            || target.Effects.Contains("cradh")
            || target.Effects.Contains("beag cradh"))
        {
            (source as Aisling)?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Target is already cursed.");

            return false;
        }

        return true;
    }
}
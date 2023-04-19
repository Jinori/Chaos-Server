using Chaos.Common.Definitions;
using Chaos.Data;
using Chaos.Scripting.EffectScripts.Abstractions;

namespace Chaos.Scripting.EffectScripts.Rogue;

public class FocusEffect : NonOverwritableEffectBase
{
    public override byte Icon => 126;
    public override string Name => "focus";

    protected override TimeSpan Duration { get; } = TimeSpan.FromSeconds(25);

    protected override Animation? Animation { get; } = new()
    {
        TargetAnimation = 88,
        AnimationSpeed = 100
    };
    protected override IReadOnlyCollection<string> ConflictingEffectNames { get; } = new[]
    {
        "Focus",
    };
    
    protected override byte? Sound => 140;

    public override void OnApplied()
    {
        base.OnApplied();

        var atkSpeed = Math.Ceiling(((double)Subject.StatSheet.EffectiveDex / 10) + 10);

        var attributes = new Attributes
        {
            AtkSpeedPct = (int)atkSpeed
        };
        
        Subject.StatSheet.AddBonus(attributes);
        AislingSubject?.Client.SendAttributes(StatUpdateType.Full);
        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You are now focused.");
    }

    public override void OnDispelled() => OnTerminated();

    public override void OnTerminated()
    {
        var attributes = new Attributes
        {
            AtkSpeedPct = 10 + (Subject.StatSheet.Dex / 10)
        };

        Subject.StatSheet.SubtractBonus(attributes);
        AislingSubject?.Client.SendAttributes(StatUpdateType.Full);
        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You lost your focus.");
    }
}
using Chaos.DarkAges.Definitions;
using Chaos.Extensions;
using Chaos.Models.Data;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Components.EffectComponents;
using Chaos.Scripting.Components.Execution;
using Chaos.Scripting.EffectScripts.Abstractions;

namespace Chaos.Scripting.EffectScripts;

public class MountEffect : EffectBase, NonOverwritableEffectComponent.INonOverwritableEffectComponentOptions
{
    public List<string> ConflictingEffectNames { get; init; } =
        [
            "Mount",
            "Werewolf"
        ];

    protected override TimeSpan Duration { get; set; } = TimeSpan.FromHours(999);

    protected Animation? Animation { get; } = new()
    {
        TargetAnimation = 6,
        AnimationSpeed = 100
    };

    public override byte Icon => 92;
    public override string Name => "Mount";
    protected byte? Sound => 115;

    public override void OnApplied()
    {
        base.OnApplied();

        var attributes = new Attributes
        {
            MagicResistance = 50
        };

        Subject.StatSheet.AddBonus(attributes);
        AislingSubject?.Client.SendAttributes(StatUpdateType.Full);
    }

    public override void OnTerminated()
    {
        if (AislingSubject != null)
        {
            AislingSubject.Sprite = 0;
            AislingSubject.Display();

            var attributes = new Attributes
            {
                MagicResistance = 50
            };

            Subject.StatSheet.SubtractBonus(attributes);

            if (!AislingSubject.Trackers.TimedEvents.HasActiveEvent("mount", out _) && !AislingSubject.IsGodModeEnabled())
                AislingSubject.Trackers.TimedEvents.AddEvent("mount", TimeSpan.FromSeconds(5), true);
        }
    }

    public override bool ShouldApply(Creature source, Creature target)
    {
        var execution = new ComponentExecutor(source, target).WithOptions(this)
                                                             .ExecuteAndCheck<NonOverwritableEffectComponent>();

        return execution is not null;
    }
}
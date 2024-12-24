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

        var stepCount = GetStepsPerSecond(Subject.Sprite);
        AislingSubject?.WalkCounter.SetMaxCount(stepCount);
    }

    public override void OnTerminated()
    {
        if (AislingSubject != null)
        {
            AislingSubject.Sprite = 0;
            AislingSubject.Display();

            var stepCount = GetStepsPerSecond(Subject.Sprite);
            AislingSubject?.WalkCounter.SetMaxCount(stepCount);

            //reset so that dismounting doesnt lock them in place until the counter resets
            AislingSubject?.WalkCounter.Reset();

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

    private int GetStepsPerSecond(int spriteId)
        => spriteId switch
        {
            1296 or (>= 1331 and <= 1334) => 7,
            1297 or (>= 1323 and <= 1326) => 9,
            1312 or (>= 1327 and <= 1330) => 9,
            >= 1335 and <= 1339           => 13,
            >= 1318 and <= 1322           => 13,
            >= 1313 and <= 1317           => 13,
            _                             => 4
        };
}
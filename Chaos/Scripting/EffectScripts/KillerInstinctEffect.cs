using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.EffectScripts.Abstractions;
using Chaos.Scripting.MapScripts.Arena.Non_Combat;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.EffectScripts;

public class KillerInstinctEffect : IntervalEffectBase
{
    /// <inheritdoc />
    protected override TimeSpan Duration { get; set; } = TimeSpan.FromDays(1);

    /// <inheritdoc />
    protected override IIntervalTimer Interval { get; } = new IntervalTimer(TimeSpan.FromSeconds(1));

    /// <inheritdoc />
    public override byte Icon => 40;

    /// <inheritdoc />
    public override string Name => "Killer Instinct";

    /// <inheritdoc />
    protected override void OnIntervalElapsed()
    {
        if (Subject.IsOnArenaMap())
        {
            if (Subject is Aisling aisling)
                aisling.SendOrangeBarMessage("Killer Instinct cannot be used in the arena.");

            Subject.Effects.Dispel("Killer Instinct");

            return;
        }

        var targets = Subject.MapInstance
                             .GetEntitiesWithinRange<Creature>(Subject, 3)
                             .ThatAreObservedBy(Subject)
                             .ThatAreVisibleTo(Subject)
                             .WithFilter(Subject, TargetFilter.AliveOnly | TargetFilter.HostileOnly);

        foreach (var target in targets)
            target.Chant(target.StatSheet.DefenseElement.ToString());
    }
}
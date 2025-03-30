using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.ReactorTileScripts.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.ReactorTileScripts;

public class PortalAnimationScript : ReactorTileScriptBase
{
    protected IIntervalTimer AnimationTimer { get; set; }
    protected IIntervalTimer? Timer { get; set; }

    protected Animation PortalAnimation { get; } = new()
    {
        AnimationSpeed = 145,
        TargetAnimation = 410
    };

    /// <inheritdoc />
    public PortalAnimationScript(ReactorTile subject)
        : base(subject)
    {
        AnimationTimer = new IntervalTimer(TimeSpan.FromSeconds(1), false);
        Timer = new IntervalTimer(TimeSpan.FromSeconds(3), false);
    }

    /// <inheritdoc />
    public override void OnWalkedOn(Creature source)
    {
        if (source is Merchant)
            Subject.MapInstance.RemoveEntity(source);
    }

    /// <inheritdoc />
    public override void Update(TimeSpan delta)
    {
        AnimationTimer.Update(delta);

        if (AnimationTimer.IntervalElapsed)
        {
            var aislings = Subject.MapInstance.GetEntitiesWithinRange<Aisling>(Subject, 12);

            foreach (var aisling in aislings)
                aisling.MapInstance.ShowAnimation(PortalAnimation.GetPointAnimation(new Point(Subject.X, Subject.Y)));
        }

        if (Timer != null)
        {
            Timer.Update(delta);

            if (Timer.IntervalElapsed)
                Map.RemoveEntity(Subject);
        }
    }
}
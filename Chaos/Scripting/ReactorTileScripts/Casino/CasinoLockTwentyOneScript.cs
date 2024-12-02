using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.ReactorTileScripts.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.ReactorTileScripts.Casino;

public class CasinoLockTwentyOneScript : ReactorTileScriptBase
{
    private readonly IIntervalTimer AnimationTimer;
    protected Animation AvailableTile { get; } = new()
    {
        AnimationSpeed = 100,
        TargetAnimation = 96
    };

    /// <inheritdoc />
    public CasinoLockTwentyOneScript(ReactorTile subject)
        : base(subject) =>
        AnimationTimer = new IntervalTimer(TimeSpan.FromMilliseconds(2000));

    /// <inheritdoc />
    public override void OnWalkedOn(Creature source)
    {
        if (source is not Aisling aisling || aisling.OnTwentyOneTile)
            return;

        aisling.OnTwentyOneTile = true;
    }

    /// <inheritdoc />
    public override void Update(TimeSpan delta)
    {
        var aislings = Subject.MapInstance.GetEntitiesAtPoints<Aisling>(Subject);
        AnimationTimer.Update(delta);

        if (aislings.Any())
            return;

        if (AnimationTimer.IntervalElapsed)
            Subject.MapInstance.ShowAnimation(AvailableTile.GetPointAnimation(Subject));
    }
}
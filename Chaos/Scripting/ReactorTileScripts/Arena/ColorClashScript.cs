using Chaos.Definitions;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.ReactorTileScripts.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.ReactorTileScripts.Arena;

public class ColorClashScript : ReactorTileScriptBase
{
    public ArenaTeam? CurrentTeam { get; set; }
    
    private Animation BlueAnimation { get; } = new() { AnimationSpeed = 100, TargetAnimation = 214 };
    private Animation GoldAnimation { get; } = new() { AnimationSpeed = 100, TargetAnimation = 214 };
    private Animation GreenAnimation { get; } = new() { AnimationSpeed = 100, TargetAnimation = 214 };
    private Animation RedAnimation { get; } = new() { AnimationSpeed = 100, TargetAnimation = 214 };
    
    private IIntervalTimer UpdateColorTimer { get; }

    public ColorClashScript(ReactorTile subject)
        : base(subject) => UpdateColorTimer = new IntervalTimer(TimeSpan.FromSeconds(1));

    public override void OnWalkedOn(Creature source)
    {
        if (source is Aisling aisling)
        {
            aisling.Trackers.Enums.TryGetValue(out ArenaTeam team);
            CurrentTeam = team; // Set the current team's color
        }
    }

    public override void Update(TimeSpan delta)
    {
        UpdateColorTimer.Update(delta);

        if (!UpdateColorTimer.IntervalElapsed)
            return;
        
        if (CurrentTeam is ArenaTeam.None or null)
            return;
            
        // Determine which animation to use based on the current team's color
        var animation = CurrentTeam switch
        {
            ArenaTeam.Blue  => BlueAnimation,
            ArenaTeam.Gold  => GoldAnimation,
            ArenaTeam.Green => GreenAnimation,
            ArenaTeam.Red   => RedAnimation,
            _               => throw new ArgumentOutOfRangeException()
        };

        Subject.MapInstance.ShowAnimation(animation.GetPointAnimation(Subject));
    }
}
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.ReactorTileScripts.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.ReactorTileScripts.MainStoryQuest;

public class IntelligenceTrialReactorScript : ReactorTileScriptBase
{

    private readonly IIntervalTimer AdminTrapViewInterval;

    private readonly Animation ViewAnimation = new Animation
    {
        TargetAnimation = 96,
        AnimationSpeed = 100
    };
    /// <inheritdoc />
    public IntelligenceTrialReactorScript(ReactorTile subject)
        : base(subject)
    {
        AdminTrapViewInterval = new IntervalTimer(TimeSpan.FromSeconds(1));
    }

    /// <inheritdoc />
    public override void OnWalkedOn(Creature source)
    {
        if (source is not Aisling aisling)
            return;

        if (source.MapInstance.Name != "Trial of Intelligence")
            return;
        aisling.SendOrangeBarMessage("You fall into a trap and restart the maze.");
        aisling.WarpTo(new Point(12, 34));
    }

    public override void Update(TimeSpan delta)
    {
        AdminTrapViewInterval.Update(delta);

        if (AdminTrapViewInterval.IntervalElapsed)
        {
            var nearbyAdmins = Map.GetEntitiesWithinRange<Aisling>(Subject)
                .Where(aisling => aisling.IsAdmin);

            foreach (var nearbyAdmin in nearbyAdmins)
                Subject.Animate(ViewAnimation);
        }
        
        base.Update(delta);
    }
}
using Chaos.Definitions;
using Chaos.Extensions.Geometry;
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
        var lifecounter = source.Trackers.Counters.TryGetValue("trialofintelligencelives", out var count);
        if (!lifecounter)
        {
            // If the counter doesn't exist, initialize it to 5
            count = 5;
            aisling.Trackers.Counters.Set("trialofintelligencelives", count);
        }

        if (count < 1)
        {
            aisling.Trackers.Counters.Set("trialofintelligencelives", 5);
            aisling.SendOrangeBarMessage("You are out of tries, restart the maze.");
            aisling.WarpTo(new Point(9, 27));
        }
        else
        {
            var newcount = count - 1;
            aisling.Trackers.Counters.Set("trialofintelligencelives", newcount);
            aisling.SendOrangeBarMessage($"You have {newcount} tries left.");
            var point = aisling.DirectionalOffset(source.Direction.Reverse());
            aisling.WarpTo(point);
        }
    }

    public override void Update(TimeSpan delta)
    {
        AdminTrapViewInterval.Update(delta);

        if (AdminTrapViewInterval.IntervalElapsed)
        {
            var nearbyAdmins = Map.GetEntitiesWithinRange<Aisling>(Subject)
                .Where(aisling => aisling.Trackers.Enums.HasValue(GodMode.Yes));

            foreach (var nearbyAdmin in nearbyAdmins)
                Subject.Animate(ViewAnimation);
        }
        
        base.Update(delta);
    }
}
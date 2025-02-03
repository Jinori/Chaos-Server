using Chaos.Collections;
using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Extensions.Geometry;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.ReactorTileScripts.Abstractions;
using Chaos.Storage.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.ReactorTileScripts.Secret;

public class cowPortal : ReactorTileScriptBase
{
    private readonly ISimpleCache SimpleCache;
    protected IIntervalTimer AnimationTimer { get; set; }

    protected IIntervalTimer? Timer { get; set; }

    protected Animation PortalAnimation { get; } = new()
    {
        AnimationSpeed = 145,
        TargetAnimation = 729
    };

    /// <inheritdoc />
    public cowPortal(ReactorTile subject, ISimpleCache simpleCache)
        : base(subject)
    {
        SimpleCache = simpleCache;
        Timer = new IntervalTimer(TimeSpan.FromHours(2), false);
        AnimationTimer = new IntervalTimer(TimeSpan.FromSeconds(1), false);
    }

    /// <inheritdoc />
    public override void OnWalkedOn(Creature source)
    {
        if (Subject.Owner is Aisling owner)
        {
            if (source is not Aisling aisling)
                return;

            var targetMap = SimpleCache.Get<MapInstance>("secret_cow_level");

            var rectangle = new Rectangle(
                41,
                65,
                5,
                5);

            var point = rectangle.GetRandomPoint();

            if (aisling.IsGodModeEnabled())
                aisling.TraverseMap(targetMap, point);

            if ((aisling?.Name == owner.Name) && aisling.Trackers.Enums.HasValue(ClassStatBracket.Grandmaster))
            {
                aisling.TraverseMap(targetMap, point);

                return;
            }

            if ((aisling?.Group != null) && !aisling.Group.Contains(owner))
            {
                aisling.SendOrangeBarMessage("This portal is for another group.");

                return;
            }

            if (aisling?.Group == null)
            {
                aisling?.SendOrangeBarMessage("You must be grouped with the player who opened the portal.");

                return;
            }

            if (!source.Trackers.Enums.HasValue(ClassStatBracket.Grandmaster))
            {
                aisling.SendOrangeBarMessage("That portal looks really dangerous. (Grandmaster to enter)");

                return;
            }

            source.TraverseMap(targetMap, point);
        }
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
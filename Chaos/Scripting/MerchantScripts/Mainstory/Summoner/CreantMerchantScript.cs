using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Models.World;
using Chaos.Scripting.MerchantScripts.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.MerchantScripts.Mainstory.Summoner;

public class CreantMerchantScript : MerchantScriptBase
{
    private readonly IIntervalTimer ActionTimer;
    private readonly IIntervalTimer PortalClose;
    private readonly IIntervalTimer WalkTimer;

    /// <inheritdoc />
    public CreantMerchantScript(Merchant subject)
        : base(subject)
    {
        PortalClose = new IntervalTimer(TimeSpan.FromSeconds(20), false);
        WalkTimer = new IntervalTimer(TimeSpan.FromMilliseconds(800), false);
        ActionTimer = new IntervalTimer(TimeSpan.FromMilliseconds(200), false);
    }

    public void HandleMedusaWalk()
    {
        if (!WalkTimer.IntervalElapsed)
            return;

        if (!Subject.MapInstance
                    .GetEntities<Aisling>()
                    .Any(x => x.Trackers.Enums.HasValue(MainStoryEnums.SpawnedCreants)))
            return;

        var point = new Point(15, 6);

        if (Subject.WithinRange(point, 1))
            Subject.MapInstance.RemoveEntity(Subject);
        else
            Subject.Pathfind(point);

        if (PortalClose.IntervalElapsed)
            Subject.MapInstance.RemoveEntity(Subject);
    }

    public void HandlePhoenixWalk()
    {
        if (!WalkTimer.IntervalElapsed)
            return;

        if (!Subject.MapInstance
                    .GetEntities<Aisling>()
                    .Any(x => x.Trackers.Enums.HasValue(MainStoryEnums.SpawnedCreants)))
            return;

        var point = new Point(8, 14);

        if (Subject.WithinRange(point, 1))
            Subject.MapInstance.RemoveEntity(Subject);
        else
            Subject.Pathfind(point);

        if (PortalClose.IntervalElapsed)
            Subject.MapInstance.RemoveEntity(Subject);
    }

    public void HandleShamensythWalk()
    {
        if (!WalkTimer.IntervalElapsed)
            return;

        if (!Subject.MapInstance
                    .GetEntities<Aisling>()
                    .Any(x => x.Trackers.Enums.HasValue(MainStoryEnums.SpawnedCreants)))
            return;

        var point = new Point(23, 14);

        if (Subject.WithinRange(point, 1))
            Subject.MapInstance.RemoveEntity(Subject);
        else
            Subject.Pathfind(point);

        if (PortalClose.IntervalElapsed)
            Subject.MapInstance.RemoveEntity(Subject);
    }

    public void HandleTaurenWalk()
    {
        if (!WalkTimer.IntervalElapsed)
            return;

        if (!Subject.MapInstance
                    .GetEntities<Aisling>()
                    .Any(x => x.Trackers.Enums.HasValue(MainStoryEnums.SpawnedCreants)))
            return;

        var point = new Point(15, 21);

        if (Subject.WithinRange(point, 1))
            Subject.MapInstance.RemoveEntity(Subject);
        else
            Subject.Pathfind(point);

        if (PortalClose.IntervalElapsed)
            Subject.MapInstance.RemoveEntity(Subject);
    }

    public override void Update(TimeSpan delta)
    {
        ActionTimer.Update(delta);
        WalkTimer.Update(delta);
        PortalClose.Update(delta);

        if (!ActionTimer.IntervalElapsed)
            return;

        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "phoenix_merchant":
            {
                HandlePhoenixWalk();

                break;
            }
            case "tauren_merchant":
            {
                HandleTaurenWalk();

                break;
            }
            case "medusa_merchant":
            {
                HandleMedusaWalk();

                break;
            }
            case "shamensyth_merchant":
            {
                HandleShamensythWalk();

                break;
            }
        }
    }
}
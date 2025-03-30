using Chaos.Common.Definitions;
using Chaos.Common.Utilities;
using Chaos.Extensions;
using Chaos.Extensions.Geometry;
using Chaos.Geometry.Abstractions.Definitions;
using Chaos.Models.World;
using Chaos.Pathfinding;
using Chaos.Pathfinding.Abstractions;
using Chaos.Scripting.MerchantScripts.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.MerchantScripts.Casino;

public class ZertaScript : MerchantScriptBase
{
    public readonly Location CasinoDoorPoint = new("rucesion", 8, 21);
    public readonly Location CasinoPoint = new("rucesion_casino", 7, 15);
    private readonly IIntervalTimer DelayTimer;
    public readonly Location HomePoint = new("rucesion", 12, 22);
    public readonly Location InsideCasinoDoorPoint = new("rucesion_casino", 19, 14);
    private readonly IIntervalTimer WalkTimer;
    public bool AnnouncedPutItOnRed;
    public bool AnnouncedWinOrLoss;
    private IPathOptions Options => PathOptions.Default.ForCreatureType(Subject.Type) with
    {
        LimitRadius = null,
        IgnoreBlockingReactors = true
    };

        
    public Aisling? Source;

    /// <inheritdoc />
    public ZertaScript(Merchant subject)
        : base(subject)
    {
        WalkTimer = new IntervalTimer(TimeSpan.FromMilliseconds(1200), false);

        DelayTimer = new RandomizedIntervalTimer(
            TimeSpan.FromSeconds(4),
            30,
            RandomizationType.Positive,
            false);
    }

    /// <inheritdoc />
    public override void Update(TimeSpan delta)
    {
        if ((Subject.ManhattanDistanceFrom(CasinoDoorPoint) > 0) && Subject.OnSameMapAs(CasinoDoorPoint) && !AnnouncedWinOrLoss)
        {
            WalkTimer.Update(delta);

            if (WalkTimer.IntervalElapsed)
            {
                var door = Subject.MapInstance.GetEntitiesAtPoints<Door>(CasinoDoorPoint).First();

                if ((Source != null) && door.Closed)
                    door.OnClicked(Source);

                Subject.Pathfind(CasinoDoorPoint, 0, Options);
            }
        }
        else if ((Subject.ManhattanDistanceFrom(CasinoPoint) > 0) && Subject.OnSameMapAs(CasinoPoint) && !AnnouncedPutItOnRed)
        {
            WalkTimer.Update(delta);

            if (WalkTimer.IntervalElapsed)
            {
                Subject.Pathfind(CasinoPoint, 0, Options);
            }
        }
        else if ((Subject.ManhattanDistanceFrom(CasinoPoint) <= 2) && !AnnouncedPutItOnRed)
        {
            Subject.Say("Put it all on red!");
            AnnouncedPutItOnRed = true;
        }
        else if (AnnouncedPutItOnRed && !AnnouncedWinOrLoss)
        {
            DelayTimer.Update(delta);

            if (DelayTimer.IntervalElapsed)
            {
                Subject.Say(IntegerRandomizer.RollChance(50) ? "Woohoo! I won!" : "Shucks.. I lost.");
                AnnouncedWinOrLoss = true;
            }
        }
        else if (AnnouncedWinOrLoss && AnnouncedPutItOnRed)
        {
            if ((Subject.ManhattanDistanceFrom(InsideCasinoDoorPoint) > 0) && Subject.OnSameMapAs(InsideCasinoDoorPoint))
            {
                WalkTimer.Update(delta);

                if (WalkTimer.IntervalElapsed)
                    Subject.Pathfind(InsideCasinoDoorPoint, 0, Options);
            }
            else if ((Subject.ManhattanDistanceFrom(HomePoint) > 0) && Subject.OnSameMapAs(HomePoint))
            {
                WalkTimer.Update(delta);

                if (WalkTimer.IntervalElapsed)
                    Subject.Pathfind(HomePoint, 0, Options);
            }
            else if ((Subject.ManhattanDistanceFrom(HomePoint) == 0) && Subject.OnSameMapAs(HomePoint))
            {
                Subject.Say("*shrugs*");
                AnnouncedWinOrLoss = false;
                AnnouncedPutItOnRed = false;
                Subject.Turn(Direction.Down);
                Subject.RemoveScript<ZertaScript>();
            }
        }
    }
}
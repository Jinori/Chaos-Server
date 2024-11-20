using Chaos.Collections;
using Chaos.Common.Definitions;
using Chaos.DarkAges.Definitions;
using Chaos.Definitions;
using Chaos.Extensions.Geometry;
using Chaos.Geometry.Abstractions;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.MapScripts.Abstractions;

namespace Chaos.Scripting.MapScripts.Arena.Non_Combat;

public class ArenaUndergroundScript(MapInstance subject) : MapScriptBase(subject)
{
    private readonly Point CenterWarp = new(11, 10);

    public override void OnEntered(Creature creature)
    {
        if (creature is not Aisling aisling)
            return;

        HandleAislingEntry(aisling);
    }

    private void HandleAislingEntry(Aisling aisling)
    {
        ResetAislingStatsAndEffects(aisling);

        if (aisling.Trackers.Enums.TryGetValue(out ArenaTeam team))
        {
            WarpAislingBasedOnTeam(aisling, team);
        }
        else
        {
            WarpAislingToDefaultLocation(aisling);
        }
    }

    private void ResetAislingStatsAndEffects(Aisling aisling)
    {
        if (!aisling.IsAlive)
        {
            aisling.IsDead = false;
        }

        foreach (var effect in aisling.Effects)
            aisling.Effects.Dispel(effect.Name);

        aisling.StatSheet.SetHealthPct(100);
        aisling.StatSheet.SetManaPct(100);
        aisling.Client.SendAttributes(StatUpdateType.Vitality);
    }

    private void WarpAislingBasedOnTeam(Aisling aisling, ArenaTeam team)
    {
        Rectangle targetRectangle;

        switch (team)
        {
            case ArenaTeam.Blue:
                targetRectangle = new Rectangle(new Point(12, 4), 4, 3);
                break;
            case ArenaTeam.Green:
                targetRectangle = new Rectangle(new Point(18, 10), 3, 4);
                break;
            case ArenaTeam.Gold:
                targetRectangle = new Rectangle(new Point(5, 10), 4, 3);
                break;
            case ArenaTeam.Red:
                targetRectangle = new Rectangle(new Point(11, 17), 4, 3);
                break;
            case ArenaTeam.None:
                targetRectangle = new Rectangle(new Point(11, 10), 3, 4);
                break;
            default:
                aisling.Trackers.Enums.Set(ArenaTeam.None);
                targetRectangle = new Rectangle(new Point(11, 10), 3, 4);
                break;
        }

        aisling.WarpTo(GetValidRandomPoint(targetRectangle));
    }

    private Point GetValidRandomPoint(IRectangle rect)
    {
        Point randomPoint;
        do
        {
            randomPoint = rect.GetRandomPoint();
        } 
        while (CenterWarp == randomPoint);

        return randomPoint;
    }

    private void WarpAislingToDefaultLocation(Aisling aisling)
    {
        var defaultRect = new Rectangle(new Point(11, 10), 3, 4);
        aisling.WarpTo(GetValidRandomPoint(defaultRect));
    }
}
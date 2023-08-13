using Chaos.Collections;
using Chaos.Common.Definitions;
using Chaos.Definitions;
using Chaos.Extensions.Geometry;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.MapScripts.Abstractions;

namespace Chaos.Scripting.MapScripts.Arena;

public class ArenaUndergroundScript : MapScriptBase
{
    private readonly Point CenterWarp = new(11, 10);
    private Point CenterWarpPlayer;

    /// <inheritdoc />
    public ArenaUndergroundScript(MapInstance subject)
        : base(subject) { }

    /// <inheritdoc />
    public override void OnEntered(Creature creature)
    {
        if (creature is Aisling aisling)
        {
            if (!aisling.IsAlive)
            {
                aisling.IsDead = false;
                aisling.StatSheet.SetHealthPct(100);
                aisling.StatSheet.SetManaPct(100);
                aisling.Client.SendAttributes(StatUpdateType.Vitality);
            }

            if (aisling.IsAlive)
            {
                foreach (var effect in aisling.Effects)
                    aisling.Effects.Dispel(effect.Name);

                aisling.StatSheet.SetHealthPct(100);
                aisling.StatSheet.SetManaPct(100);
            }

            aisling.Trackers.Enums.TryGetValue(out ArenaTeam team);

            switch (team)
            {
                case ArenaTeam.Blue:
                {
                    var rect = new Rectangle(new Point(12, 4), 4, 3);
                    aisling.WarpTo(rect.GetRandomPoint());

                    break;
                }
                case ArenaTeam.Green:
                {
                    var rect = new Rectangle(new Point(18, 10), 3, 4);
                    aisling.WarpTo(rect.GetRandomPoint());

                    break;
                }
                case ArenaTeam.Gold:
                {
                    var rect = new Rectangle(new Point(5, 10), 4, 3);
                    aisling.WarpTo(rect.GetRandomPoint());

                    break;
                }
                case ArenaTeam.Red:
                {
                    var rect = new Rectangle(new Point(11, 17), 4, 3);
                    aisling.WarpTo(rect.GetRandomPoint());

                    break;
                }
                case ArenaTeam.None:
                {
                    var rect = new Rectangle(new Point(11, 10), 3, 4);

                    do
                        CenterWarpPlayer = rect.GetRandomPoint();
                    while (CenterWarp == CenterWarpPlayer);

                    aisling.WarpTo(CenterWarpPlayer);

                    break;
                }
                default:
                {
                    var rect = new Rectangle(new Point(11, 10), 3, 4);

                    do
                        CenterWarpPlayer = rect.GetRandomPoint();
                    while (CenterWarp == CenterWarpPlayer);

                    aisling.Trackers.Enums.Set(ArenaTeam.None);
                    aisling.WarpTo(CenterWarpPlayer);

                    break;
                }
            }
        }
    }
}
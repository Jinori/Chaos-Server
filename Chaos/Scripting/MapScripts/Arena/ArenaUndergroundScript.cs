using Chaos.Collections;
using Chaos.Common.Definitions;
using Chaos.Definitions;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.MapScripts.Abstractions;

namespace Chaos.Scripting.MapScripts.Arena;

public class ArenaUndergroundScript : MapScriptBase
{
    private readonly Point GreenPoint = new(16, 6);
    private readonly Point RedPoint = new(6, 6);
    private readonly Point BluePoint = new(6, 15);
    private readonly Point GoldPoint = new(15, 15);
    
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
            
            aisling.Trackers.Enums.TryGetValue(out ArenaTeam team);

            switch (team)
            {
                case ArenaTeam.Blue:
                    aisling.WarpTo(BluePoint);
                    break;
                case ArenaTeam.Green:
                    aisling.WarpTo(GreenPoint);
                    break;
                case ArenaTeam.Gold:
                    aisling.WarpTo(GoldPoint);
                    break;
                case ArenaTeam.Red:
                    aisling.WarpTo(RedPoint);
                    break;
            }
        }
    }
}
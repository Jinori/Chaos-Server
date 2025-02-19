using Chaos.Collections;
using Chaos.Extensions.Geometry;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.ReactorTileScripts.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.ReactorTileScripts.GuildHall;

public class GuildHallEntrance(
    ReactorTile subject,
    ISimpleCache simpleCache)
    : ConfigurableReactorTileScriptBase(subject)
{
    /// <inheritdoc />
    public override void OnWalkedOn(Creature source)
    {
        var targetMap = simpleCache.Get<MapInstance>(Destination.Map);
        
        if (source is not Aisling aisling) 
            return;

        if (aisling.Guild is null)
        {
            aisling.SendOrangeBarMessage("You don't belong to a guild.");
            var point = source.DirectionalOffset(source.Direction.Reverse());
            source.WarpTo(point);
            return;
        }
        
        Subject.MapInstance.RemoveEntity(aisling);

        targetMap.AddEntity(aisling, new Point(98, 46));
        aisling.SendOrangeBarMessage($"You've entered {aisling.Guild?.Name}'s Guild Hall.");
    }

    #region ScriptVars
    protected Location Destination { get; init; } = null!;
    public int? MaxLevel { get; set; }
    public int? MaxVitality { get; set; }
    public int? MinLevel { get; set; }
    public int? MinVitality { get; set; }
    #endregion
}
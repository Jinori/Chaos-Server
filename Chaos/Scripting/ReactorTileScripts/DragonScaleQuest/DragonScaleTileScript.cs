using Chaos.Collections;
using Chaos.Definitions;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.ReactorTileScripts.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.ReactorTileScripts.DragonScaleQuest;

public class DragonScaleTileScript(
    ReactorTile subject,
    ISimpleCache simpleCache)
    : ReactorTileScriptBase(subject)
{
    public override void OnItemDroppedOn(Creature source, GroundItem groundItem)
    {
        
        if (source is not Aisling aisling)
            return;

        var hasStage = source.Trackers.Enums.TryGetValue(out DragonScale stage);
        
        if (hasStage && stage != DragonScale.FoundAllClues && stage != DragonScale.SpawnedDragon)
            return;
        
        if (!CanStartBoss(aisling))
            return;
        
        // Check if dropped item is Lion Fish
        if (groundItem.Name != "Lion Fish")
            return;

        // Check if player has effect
        if (!aisling.Effects.Contains("Sweet Buns"))
            return;

        aisling.MapInstance.RemoveEntity(groundItem);
        aisling.Trackers.Enums.Set(DragonScale.DroppedScale);
    }
    
    private bool CanStartBoss(Aisling source)
    {
        var mapInstance = simpleCache.Get<MapInstance>("wilderness");

        if (!MapHasMonsters(mapInstance)) return true;
        source.SendOrangeBarMessage("The dragon is already here.");
        return false;
    }
    
    private static bool MapHasMonsters(MapInstance mapInstance)
    {
        return mapInstance.GetEntities<Creature>().Where(x => x.Name == "Dragon").OfType<Monster>().Any();
    }
}


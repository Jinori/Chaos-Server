using Chaos.Collections;
using Chaos.Common.Utilities;
using Chaos.Extensions;
using Chaos.Extensions.Common;
using Chaos.Extensions.Geometry;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.DialogScripts.Professions;

public class PortalTrinketScript : DialogScriptBase
{
    private readonly List<string> MapsTrinketCanBeUsed =
    [
        "Mythic",
        "Mileth",
        "Abel",
        "Rucesion",
        "Undine",
        "Suomi",
        "Loures Castle",
        "Loures Castle Way",
        "Tagor",
        "Piet",
        "Rucesion Village Way",
        "Mileth Village Way",
        "Abel Port",
        "Suomi Village Way",
        "Undine Village Way",
        "Piet Village Way",
        "Nobis",
        "Weapon Shop",
        "Armor Shop",
        "Rucesion Bank",
        "Mileth Storage",
        "Black Market",
        "Rucesion Jeweler",
        "Rucesion Church",
        "Rucesion Tailor",
        "Skills Master",
        "Rucesion Inn",
        "Shrine of Skandara",
        "Shrine of Miraelis",
        "Shrine of Theselene",
        "Shrine of Serendael",
        "Piet Empty Room",
        "Piet Inn",
        "Mileth Inn",
        "Abel Inn",
        "Suomi Inn",
        "Piet Storage",
        "Mileth Storage",
        "Piet Alchemy Lab",
        "Piet Tavern",
        "Mileth Tavern",
        "Undine Tavern",
        "Mileth Kitchen",
        "Piet Restaurant",
        "Piet Priestess",
        "Piet Magic Shop",
        "Piet Sewer Entrance",
        "Tagor Forge",
        "Tagor Inn",
        "Tagor Messenger",
        "Tagor Pet Store",
        "Tagor Storage",
        "Tagor Restaurant",
        "Tagor Tavern",
        "Tagor Church",
        "Abel Combat Skill Master",
        "Abel Fish Market",
        "Abel Tavern",
        "Abel Restaurant",
        "Abel Empty Room",
        "Abel Storage",
        "Abel Goods Shop",
        "Abel Magic Shop",
        "Abel Weapon Shop",
        "Abel Armor Shop",
        "Special Skills Masters",
        "Mileth Armor Shop",
        "Mileth Weapon Shop",
        "Special Spells Master",
        "Mileth Church",
        "Kitchen",
        "Restaurant",
        "Tavern",
        "Mileth Beauty Shop",
        "Temple of Choosing",
        "Undine Armor Shop",
        "Undine Weapon Shop",
        "Enchanted Haven",
        "Undine Goods Shop",
        "Undine Black Magic Master",
        "Undine Storage",
        "Undine Restaurant",
        "Undine Tavern",
        "Suomi Special Skill Master",
        "Garamonde Theatre",
        "Suomi Armor Shop",
        "Suomi Weapon Shop",
        "Suomi Cherry Farmer",
        "Suomi White Magic Master",
        "Suomi Combat Skill Master",
        "Suomi Black Magic Master",
        "Suomi Grape Farmer",
        "Suomi Restaurant",
        "Suomi Tavern",
        "Nobis Restaurant",
        "Nobis Tavern",
        "Nobis House",
        "Nobis Storage",
        "Nobis Weapon Shop",
        "Mileth Tailor",
        "Cthonic Remains 11",
        "Cthonic Remains 21",
        "East Woodland Crossroads",
        "West Woodlands Entrance",
        "Eingren Manor Entrance",
        "Mehadi",
        "Base Camp",
        "Lynith Beach",
        "Lynith Beach South",
        "Navigation Room",
        "Main Hall",
        "Pirate Galley",
        "Pirate Shop",
        "Officer's Quarters",
        "Pirate Quarters",
        "First Mate's Quarters",
        "Pirate Hall 1",
        "Lynith Beach North",
        "Shinewood Forest Entrance",
        "Mount Giragan Entrance",
        "The Exchange",
        "Loures Castle Way",
        "Loures Castle",
        "Loures 1 Floor Hall",
        "Loures 1 Floor Corridor",
        "Loures 1 Floor Restaurant",
        "Loures 1 Floor Bedroom",
        "Wilderness",
        "Loures 1 Floor Weapon Storage",
        "Loures 2 Floor Corridor",
        "Loures 2 Floor Empty Room",
        "Guild Master's Room",
        "Loures 2 Floor Restaurant",
        "Loures 2 Floor Tavern",
        "Loures 2 Floor Bedroom",
        "Loures 2 Floor Weapon Storage",
        "Loures 2 Floor Hall",
        "Loures Cyril Corridor",
        "Loures 3 Floor Office",
        "Loures 3 Floor Corridor",
        "Loures Library",
        "Loures 3 Floor Magic Room",
        "Dubhaim Castle",
        "Karlopos Island",
        "Piet Sewer Entrance",
        "Loures Sewer Entrance",
        "Astrid Entrance",
        "Undine Field Entrance",
        "Porte Forest Entrance",
        "Road to House Macabre",
        "Nobis Maze Entrance",
        "Limbo"
    ];

    private readonly IReactorTileFactory ReactorTileFactory;
    private readonly ISimpleCache SimpleCache;

    /// <inheritdoc />
    public PortalTrinketScript(Dialog subject, IReactorTileFactory reactorTileFactory, ISimpleCache simpleCache)
        : base(subject)
    {
        ReactorTileFactory = reactorTileFactory;
        SimpleCache = simpleCache;
    }

    /// <inheritdoc />
    public override void OnDisplaying(Aisling source)
    {
        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "portaltrinket_initial":
            {
                if (!source.Legend.ContainsKey("ench") && !source.IsGodModeEnabled())
                {
                    source.Inventory.RemoveQuantityByTemplateKey("portaltrinket", 1);
                    source.SendOrangeBarMessage("The Geata Chagum breaks in your undefined hands.");
                    Subject.Close(source);
                }

                break;
            }

            case "portaltrinket_summongroup":
            {
                if (source.Trackers.TimedEvents.HasActiveEvent("portalTrinket", out var repairTime))
                {
                    Subject.Reply(
                        source,
                        $"The arcane energies need time to replenish. You must wait {repairTime.Remaining.ToReadableString()} before you can call upon your group once more.");

                    return;
                }

                if (!MapsTrinketCanBeUsed.Contains(source.MapInstance.Name) || source.MapInstance.IsShard)
                {
                    Subject.Reply(source, "The mystical energies of this trinket cannot be used in this location.");

                    return;
                }

                if (source.MapInstance.Name == "Suomi")
                {
                    var cherryRectangle = new Rectangle(
                        26,
                        68,
                        20,
                        25);

                    var grapeRectangle = new Rectangle(
                        76,
                        38,
                        20,
                        24);

                    if (cherryRectangle.Contains(source) || grapeRectangle.Contains(source))
                    {
                        Subject.Reply(source, "Mystical energies of this trinket will not work here.");

                        return;
                    }
                }

                if (source.Group is null)
                {
                    Subject.Reply(source, "You stand alone, unbound by the ties of a group.");

                    return;
                }

                source.SummonTrinketMapInstance = source.MapInstance;
                source.SummonTrinketLocation = new Point(source.X, source.Y);

                foreach (var member in source.Group)
                    if (!member.Equals(source))
                    {
                        if (source.MapInstance.MaximumLevel.HasValue)
                            if (member.UserStatSheet.Level > source.MapInstance.MaximumLevel.Value)
                            {
                                Subject.Reply(source, "One member of your party is not within the level range for this location.");

                                source.SendOrangeBarMessage($"{member.Name} cannot be summoned here due to level range.");

                                continue;
                            }

                        if (source.MapInstance.MinimumLevel.HasValue)
                            if (member.UserStatSheet.Level < source.MapInstance.MinimumLevel.Value)
                            {
                                Subject.Reply(source, "One member of your party is not within the level range for this location.");

                                source.SendOrangeBarMessage($"{member.Name} cannot be summoned here due to level range.");

                                continue;
                            }

                        if (source.MapInstance == member.MapInstance)
                        {
                            var rectangle = new Rectangle(
                                member.X - 2,
                                member.Y - 2,
                                4,
                                4);
                            rectangle.TryGetRandomPoint(x => member.MapInstance.IsWalkable(x, collisionType: member.Type), out var point);

                            if (point == null)
                            {
                                member.SendOrangeBarMessage($"{source.Name} tried to summon portal but there's no space.");

                                continue;
                            }

                            var tile = ReactorTileFactory.Create(
                                "summonportal",
                                member.MapInstance,
                                point,
                                null,
                                source);
                            member.MapInstance.SimpleAdd(tile);
                            member.SendActiveMessage($"{source.Name} has created a group portal for you to enter.");
                        } else
                            Task.Run(
                                async () =>
                                {
                                    await using var sync = await ComplexSynchronizationHelper.WaitAsync(
                                        TimeSpan.FromMilliseconds(500),
                                        TimeSpan.FromMilliseconds(300),
                                        source.MapInstance.Sync,
                                        member.MapInstance.Sync);

                                    var rectangle = new Rectangle(
                                        member.X - 2,
                                        member.Y - 2,
                                        4,
                                        4);

                                    rectangle.TryGetRandomPoint(x => member.MapInstance.IsWalkable(x, collisionType: member.Type), out var point);

                                    if (point == null)
                                    {
                                        member.SendOrangeBarMessage($"{source.Name} tried to summon portal but there's no space.");

                                        return;
                                    }

                                    var tile = ReactorTileFactory.Create(
                                        "summonportal",
                                        member.MapInstance,
                                        point,
                                        null,
                                        source);

                                    member.MapInstance.SimpleAdd(tile);
                                    member.SendActiveMessage($"{source.Name} has created a group portal for you to enter.");
                                });
                    }

                if (!source.IsAdmin)
                    source.Trackers.TimedEvents.AddEvent("portalTrinket", TimeSpan.FromHours(2), true);

                break;
            }

            case "portaltrinket_portalhaven":
            {
                var targetMap = SimpleCache.Get<MapInstance>("undine");
                source.TraverseMap(targetMap, new Point(47, 42));
                source.SendActiveMessage("The orb has taken you back to the Enchanter's Haven.");

                break;
            }
        }
    }
}
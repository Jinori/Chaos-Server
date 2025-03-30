using Chaos.Collections;
using Chaos.DarkAges.Definitions;
using Chaos.Extensions.Common;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.DialogScripts.LynithPirateShip;

public class PirateDialogScript : DialogScriptBase
{
    private readonly ISimpleCache SimpleCache;

    public PirateDialogScript(Dialog subject, ISimpleCache simpleCache)
        : base(subject)
        => SimpleCache = simpleCache;

    private bool HasPirateArmor(Aisling source)
    {
        var armor = source.Equipment[EquipmentSlot.Overcoat];
        var helmet = source.Equipment[EquipmentSlot.Helmet];

        var hasPirateArmor = (armor != null)
                             && (armor.DisplayName.EqualsI("M Pirate Shirt")
                                 || armor.DisplayName.EqualsI("F Pirate Blouse")
                                 || armor.DisplayName.EqualsI("M Pirate Garb")
                                 || armor.DisplayName.EqualsI("F Pirate Dress"));

        var hasPirateHelmet = (helmet != null)
                              && (helmet.DisplayName.EqualsI("Red Pirate Bandana")
                                  || helmet.DisplayName.EqualsI("Red Pirate Scarf")
                                  || helmet.DisplayName.EqualsI("Black Pirate Bandana"));

        return hasPirateArmor || hasPirateHelmet;
    }

    public override void OnDisplaying(Aisling source)
    {
        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "saltysam_initial":
            {
                if (source.UserStatSheet.Level < 11)
                {
                    var pointhome = new Point(5, 8);
                    Subject.Reply(source, "Welcome to my galley! Oh, you're small... Who let you in? Let's get you somewhere safe.");
                    source.SendOrangeBarMessage("Salty Sam escorts you back to Mileth Inn.");
                    var mapinstance1 = SimpleCache.Get<MapInstance>("mileth_inn");
                    source.TraverseMap(mapinstance1, pointhome);

                    return;
                }

                if (!HasPirateArmor(source))
                {
                    Subject.Reply(
                        source,
                        "What are you doing here tiny? This is a galley for pirates like me! You can't be snooping around our food! You're up to no good aren't ya? Off to the brig with you!");
                    var point = new Point(69, 9);

                    source.SendOrangeBarMessage("Salty Sam throws you in the brig!");

                    switch (source.UserStatSheet.Level)
                    {
                        case >= 11 and < 25:
                        {
                            var mapinstance1 = SimpleCache.Get<MapInstance>("lynith_pirate_brig1");
                            source.TraverseMap(mapinstance1, point);

                            return;
                        }
                        case >= 25 and < 41:
                        {
                            var mapinstance1 = SimpleCache.Get<MapInstance>("lynith_pirate_brig2");
                            source.TraverseMap(mapinstance1, point);

                            return;
                        }
                        case >= 41 and < 55:
                        {
                            var mapinstance1 = SimpleCache.Get<MapInstance>("lynith_pirate_brig3");
                            source.TraverseMap(mapinstance1, point);

                            return;
                        }
                        case >= 55 and < 71:
                        {
                            var mapinstance1 = SimpleCache.Get<MapInstance>("lynith_pirate_brig4");
                            source.TraverseMap(mapinstance1, point);

                            return;
                        }
                        case >= 71 and < 99:
                        {
                            var mapinstance1 = SimpleCache.Get<MapInstance>("lynith_pirate_brig5");
                            source.TraverseMap(mapinstance1, point);

                            return;
                        }
                        case >= 99:
                        {
                            var mapinstance1 = SimpleCache.Get<MapInstance>("lynith_pirate_brig6");
                            source.TraverseMap(mapinstance1, point);

                            return;
                        }
                    }
                }
            }

                break;

            case "jollyroger_initial":
            {
                if (source.UserStatSheet.Level < 11)
                {
                    var pointhome = new Point(5, 8);
                    Subject.Reply(source, "You're too young to be here... how did you even get on the ship? Let's get you somewhere safe.");
                    source.SendOrangeBarMessage("Jolly Roger escorts you to Mileth Inn.");
                    var mapinstance1 = SimpleCache.Get<MapInstance>("mileth_inn");
                    source.TraverseMap(mapinstance1, pointhome);

                    return;
                }

                if (!HasPirateArmor(source))
                {
                    Subject.Reply(
                        source,
                        "Who let you in here? Get off my ship! This is no place for you to be wandering around! Actually, off to the brig!");
                    source.SendOrangeBarMessage("Jolly Roger throws you in the brig!");
                    var point = new Point(69, 9);

                    switch (source.UserStatSheet.Level)
                    {
                        case >= 11 and < 25:
                        {
                            var mapinstance1 = SimpleCache.Get<MapInstance>("lynith_pirate_brig1");
                            source.TraverseMap(mapinstance1, point);

                            return;
                        }
                        case >= 25 and < 41:
                        {
                            var mapinstance1 = SimpleCache.Get<MapInstance>("lynith_pirate_brig2");
                            source.TraverseMap(mapinstance1, point);

                            return;
                        }
                        case >= 41 and < 55:
                        {
                            var mapinstance1 = SimpleCache.Get<MapInstance>("lynith_pirate_brig3");
                            source.TraverseMap(mapinstance1, point);

                            return;
                        }
                        case >= 55 and < 71:
                        {
                            var mapinstance1 = SimpleCache.Get<MapInstance>("lynith_pirate_brig4");
                            source.TraverseMap(mapinstance1, point);

                            return;
                        }
                        case >= 71 and < 99:
                        {
                            var mapinstance1 = SimpleCache.Get<MapInstance>("lynith_pirate_brig5");
                            source.TraverseMap(mapinstance1, point);

                            return;
                        }
                        case >= 99:
                        {
                            var mapinstance1 = SimpleCache.Get<MapInstance>("lynith_pirate_brig6");
                            source.TraverseMap(mapinstance1, point);

                            return;
                        }
                    }
                }

                break;
            }

            case "savagesable_initial":
            {
                if (source.UserStatSheet.Level < 11)
                {
                    var pointhome = new Point(5, 8);
                    Subject.Reply(source, "You're a small fella, like me! Let's get you somewhere safe.");
                    source.SendOrangeBarMessage("Savage Sable escorts you to Mileth Inn.");
                    var mapinstance1 = SimpleCache.Get<MapInstance>("mileth_inn");
                    source.TraverseMap(mapinstance1, pointhome);

                    return;
                }

                if (!HasPirateArmor(source))
                {
                    Subject.Reply(source, "You don't belong in our quarters! Get! You're going to the brig!");
                    var point = new Point(69, 9);

                    switch (source.UserStatSheet.Level)
                    {
                        case >= 11 and < 25:
                        {
                            var mapinstance1 = SimpleCache.Get<MapInstance>("lynith_pirate_brig1");
                            source.TraverseMap(mapinstance1, point);

                            return;
                        }
                        case >= 25 and < 41:
                        {
                            var mapinstance1 = SimpleCache.Get<MapInstance>("lynith_pirate_brig2");
                            source.TraverseMap(mapinstance1, point);

                            return;
                        }
                        case >= 41 and < 55:
                        {
                            var mapinstance1 = SimpleCache.Get<MapInstance>("lynith_pirate_brig3");
                            source.TraverseMap(mapinstance1, point);

                            return;
                        }
                        case >= 55 and < 71:
                        {
                            var mapinstance1 = SimpleCache.Get<MapInstance>("lynith_pirate_brig4");
                            source.TraverseMap(mapinstance1, point);

                            return;
                        }
                        case >= 71 and < 99:
                        {
                            var mapinstance1 = SimpleCache.Get<MapInstance>("lynith_pirate_brig5");
                            source.TraverseMap(mapinstance1, point);

                            return;
                        }
                        case >= 99:
                        {
                            var mapinstance1 = SimpleCache.Get<MapInstance>("lynith_pirate_brig6");
                            source.TraverseMap(mapinstance1, point);

                            return;
                        }
                    }
                }

                break;
            }

            case "captainwolfgang_disrespectcaptain":
            {
                var point = new Point(69, 9);
                source.SendOrangeBarMessage("Captain Wolfgang throws you into the brig!");

                switch (source.UserStatSheet.Level)
                {
                    case >= 11 and < 25:
                    {
                        var mapinstance1 = SimpleCache.Get<MapInstance>("lynith_pirate_brig1");
                        source.TraverseMap(mapinstance1, point);

                        return;
                    }
                    case >= 25 and < 41:
                    {
                        var mapinstance1 = SimpleCache.Get<MapInstance>("lynith_pirate_brig2");
                        source.TraverseMap(mapinstance1, point);

                        return;
                    }
                    case >= 41 and < 55:
                    {
                        var mapinstance1 = SimpleCache.Get<MapInstance>("lynith_pirate_brig3");
                        source.TraverseMap(mapinstance1, point);

                        return;
                    }
                    case >= 55 and < 71:
                    {
                        var mapinstance1 = SimpleCache.Get<MapInstance>("lynith_pirate_brig4");
                        source.TraverseMap(mapinstance1, point);

                        return;
                    }
                    case >= 71 and < 99:
                    {
                        var mapinstance1 = SimpleCache.Get<MapInstance>("lynith_pirate_brig5");
                        source.TraverseMap(mapinstance1, point);

                        return;
                    }
                    case >= 99:
                    {
                        var mapinstance1 = SimpleCache.Get<MapInstance>("lynith_pirate_brig6");
                        source.TraverseMap(mapinstance1, point);

                        return;
                    }
                    default:
                        var mapinstance = SimpleCache.Get<MapInstance>("Mileth_Inn");
                        var pointinn = new Point(5, 8);
                        source.TraverseMap(mapinstance, pointinn);
                        source.SendOrangeBarMessage("The Captain escorts you somewhere safe.");

                        return;
                }
            }
        }
    }
}
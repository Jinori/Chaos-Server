using Chaos.Collections;
using Chaos.Common.Definitions;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.DialogScripts.Temple_of_Choosing;

public class AoifeTempleWarpScript(Dialog subject, ISimpleCache simpleCache) : DialogScriptBase(subject)
{
    public override void OnDisplayed(Aisling source)
    {
        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "aoife_temple":
            {
                if (source.Trackers.Flags.HasFlag(QuestFlag1.ChosenClass))
                {
                    source.Client.SendServerMessage(ServerMessageType.OrangeBar1,
                        "You have already chosen a class. Luck be with you.");

                    return;
                }

                var mapInstance = simpleCache.Get<MapInstance>("tocinner");
                var point = new Point(22, 13);
                source.TraverseMap(mapInstance, point);

                return;
            }
            case "aoife_tranchamber":
            {
                var mapInstance = simpleCache.Get<MapInstance>("transcendencechamber");
                var point = new Point(8, 6);
                source.TraverseMap(mapInstance, point);

                return;
            }
            case "aoife_whimsypav":
            {
                var mapInstance = simpleCache.Get<MapInstance>("whimsypavilion");
                var point = new Point(9, 7);
                source.TraverseMap(mapInstance, point);

                return;
            }
            case "aoife_lunarsanctum":
            {
                var mapInstance = simpleCache.Get<MapInstance>("lunarsanctum");
                var point = new Point(9, 8);
                source.TraverseMap(mapInstance, point);

                return;
            }
            case "aoife_radianttemple":
            {
                var mapInstance = simpleCache.Get<MapInstance>("radianttemple");
                var point = new Point(9, 7);
                source.TraverseMap(mapInstance, point);

                return;
            }
            case "god_leaverealm2":
            {
                switch (source.Nation)
                {
                    case Nation.Exile:
                        var point = new Point(8, 5);
                        var map = simpleCache.Get<MapInstance>("toc");
                        source.TraverseMap(map, point);

                        break;
                    case Nation.Suomi:
                        var point1 = new Point(9, 5);
                        var map1 = simpleCache.Get<MapInstance>("suomi_inn");
                        source.TraverseMap(map1, point1);

                        break;
                    case Nation.Ellas:
                        var point2 = new Point(9, 2);

                        break;
                    case Nation.Loures:
                        var point3 = new Point(5, 6);
                        var map3 = simpleCache.Get<MapInstance>("loures_2_floor_empty_room_1");
                        source.TraverseMap(map3, point3);

                        break;
                    case Nation.Mileth:
                        var point4 = new Point(4, 8);
                        var map4 = simpleCache.Get<MapInstance>("mileth_inn");
                        source.TraverseMap(map4, point4);

                        break;
                    case Nation.Tagor:
                        var point5 = new Point(4, 8);
                        var map5 = simpleCache.Get<MapInstance>("tagor_inn");
                        source.TraverseMap(map5, point5);

                        break;
                    case Nation.Rucesion:
                        var point6 = new Point(7, 5);
                        var map6 = simpleCache.Get<MapInstance>("rucesion_inn");
                        source.TraverseMap(map6, point6);

                        break;
                    case Nation.Noes:
                        var point7 = new Point(9, 9);

                        break;
                    case Nation.Illuminati:
                        var point8 = new Point(9, 10);

                        break;
                    case Nation.Piet:
                        var point9 = new Point(5, 8);
                        var map9 = simpleCache.Get<MapInstance>("piet_inn");
                        source.TraverseMap(map9, point9);

                        break;
                    case Nation.Atlantis:
                        var point10 = new Point(9, 12);

                        break;
                    case Nation.Abel:
                        var point11 = new Point(9, 5);
                        var map11 = simpleCache.Get<MapInstance>("abel_inn");
                        source.TraverseMap(map11, point11);

                        break;
                    case Nation.Undine:
                        var point12 = new Point(9, 5);
                        var map12 = simpleCache.Get<MapInstance>("undine_tavern");
                        source.TraverseMap(map12, point12);

                        break;
                    case Nation.Void:
                        var point13 = new Point(12, 16);
                        var map13 = simpleCache.Get<MapInstance>("arena_entrance");
                        source.TraverseMap(map13, point13);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                
                break;
            }
        }

    }
}
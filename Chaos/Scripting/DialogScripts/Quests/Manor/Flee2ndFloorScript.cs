using Chaos.Collections;
using Chaos.Extensions.Common;
using Chaos.Extensions.Geometry;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.DialogScripts.Quests.Manor;

public class Flee2NdFloorScript(Dialog subject, ISimpleCache simpleCache) : DialogScriptBase(subject)
{
    private readonly string[] MapKeys =
    [
        "manor_library",
        "manor_study",
        "manor_study_2",
        "manor_kitchen",
        "manor_kitchen_2",
        "manor_commons",
        "manor_storage",
        "manor_depot",
        "manor_bedroom",
        "manor_bedroom_2",
        "manor_bedroom_3",
        "manor_bunks",
        "manor_master_suite"
    ];

    public override void OnDisplaying(Aisling source)
    {
        switch (Subject.Template.TemplateKey.ToLower())
        {
                case "terminus_initial":
                {
                    if (MapKeys.ContainsI(source.MapInstance.InstanceId))
                        return;
                    
                    var option = new DialogOption
                    {
                        DialogKey = "terminus_flee2ndfloor",
                        OptionText = "Flee Second Floor!"
                    };

                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Insert(0, option);

                    break;
                }
            
            case "terminus_flee2ndfloor2":
            {
                foreach (var member in source.MapInstance.GetEntities<Aisling>())
                {
                    var rectangle = new Rectangle(25, 3, 2, 2);
                    var mapInstance = simpleCache.Get<MapInstance>("manor_main_hall");

                    Point newPoint;
                    do
                    {
                        newPoint = rectangle.GetRandomPoint();
                    } while (!mapInstance.IsWalkable(newPoint, collisionType: member.Type));
                    
                    member.TraverseMap(mapInstance, newPoint);
                }
            }

                break;
        }
    }
}
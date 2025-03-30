using Chaos.Collections;
using Chaos.Extensions.Geometry;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.DialogScripts.Quests.Manor;

public class Flee2NdFloorScript : DialogScriptBase
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

    private readonly ISimpleCache SimpleCache;

    public Flee2NdFloorScript(Dialog subject, ISimpleCache simpleCache)
        : base(subject)
        => SimpleCache = simpleCache;

    public override void OnDisplaying(Aisling source)
    {
        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "terminus_initial":
            {
                if ((source.MapInstance.InstanceId != "manor_library")
                    && (source.MapInstance.InstanceId != "manor_study")
                    && (source.MapInstance.InstanceId != "manor_study_2")
                    && (source.MapInstance.InstanceId != "manor_kitchen")
                    && (source.MapInstance.InstanceId != "manor_kitchen_2")
                    && (source.MapInstance.InstanceId != "manor_commons")
                    && (source.MapInstance.InstanceId != "manor_storage")
                    && (source.MapInstance.InstanceId != "manor_depot")
                    && (source.MapInstance.InstanceId != "manor_bedroom")
                    && (source.MapInstance.InstanceId != "manor_bedroom_2")
                    && (source.MapInstance.InstanceId != "manor_bedroom_3")
                    && (source.MapInstance.InstanceId != "manor_bunks")
                    && (source.MapInstance.InstanceId != "manor_master_suite"))
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
                    var rectangle = new Rectangle(
                        25,
                        3,
                        2,
                        2);
                    var mapInstance = SimpleCache.Get<MapInstance>("manor_main_hall");

                    Point newPoint;

                    do
                        newPoint = rectangle.GetRandomPoint();
                    while (!mapInstance.IsWalkable(newPoint, member.Type));

                    member.TraverseMap(mapInstance, newPoint);
                }
            }

                break;
        }
    }
}
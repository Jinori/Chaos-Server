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
                    source.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You have already chosen a class. Luck be with you.");

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
        }

    }
}
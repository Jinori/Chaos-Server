using Chaos.Common.Definitions;
using Chaos.Containers;
using Chaos.Definitions;
using Chaos.Geometry.Abstractions.Definitions;
using Chaos.Objects.Menu;
using Chaos.Objects.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.DialogScripts.Generic;

public class SgriosReviveScript : DialogScriptBase
{
    private readonly ISimpleCache _simpleCache;

    public SgriosReviveScript(Dialog subject, ISimpleCache simpleCache)
        : base(subject) => _simpleCache = simpleCache;

    public override void OnDisplayed(Aisling source)
    {
        switch (source.Gender)
        {
            case Gender.Male:
            {
                if (source.BodySprite is not BodySprite.Male)
                    source.BodySprite = BodySprite.Male;
                break;
            }
            case Gender.Female:
            {
                if (source.BodySprite is not BodySprite.Female)
                    source.BodySprite = BodySprite.Female;
                break;
            }
        }

        source.IsDead = false;
        source.StatSheet.AddHealthPct(20);
        source.StatSheet.AddManaPct(20);
        source.Client.SendAttributes(StatUpdateType.Vitality);
        source.Turn(Direction.Down);
        source.Refresh(true);
        source.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Sgrios mumbles unintelligble gibberish");
        source.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You are revived and sent home.");
        source.Trackers.Enums.TryGetValue(out TutorialQuestStage tutorial);

        if (tutorial != TutorialQuestStage.CompletedTutorial)
        {
            Point point2;
            point2 = new Point(23, 18);
            var mapInstance2 = _simpleCache.Get<MapInstance>("mileth");
            source.TraverseMap(mapInstance2, point2, true);
            source.Trackers.Enums.Set(TutorialQuestStage.CompletedTutorial);

            return;
        }
        Subject.Close(source);
        switch (source.Nation)
        {
            case Nation.Rucesion:
            {
                Point point;
                point = new Point(25, 36);
                var mapInstance = _simpleCache.Get<MapInstance>("rucesion");
                source.TraverseMap(mapInstance, point, true);
                break;
            }
            case Nation.Mileth:
            {
                Point point;
                point = new Point(23, 18);
                var mapInstance = _simpleCache.Get<MapInstance>("mileth");
                source.TraverseMap(mapInstance, point, true);
                break;
            }
            case Nation.Exile:
            {
                Point point;
                point = new Point(3, 13);
                var mapInstance = _simpleCache.Get<MapInstance>("mileth_village_way");
                source.TraverseMap(mapInstance, point, true);
                break;
            }
            case Nation.Suomi:
            {
                Point point;
                point = new Point(16, 8);
                var mapInstance = _simpleCache.Get<MapInstance>("suomi_village_way");
                source.TraverseMap(mapInstance, point, true);
                break;
            }
            case Nation.Loures:
            {
                Point point;
                point = new Point(10, 6);
                var mapInstance = _simpleCache.Get<MapInstance>("mileth_village_way");
                source.TraverseMap(mapInstance, point, true);
                break;
            }
            case Nation.Tagor:
            {
                Point point;
                point = new Point(22, 94);
                var mapInstance = _simpleCache.Get<MapInstance>("tagor");
                source.TraverseMap(mapInstance, point, true);
                break;
            }
            case Nation.Piet:
            {
                Point point;
                point = new Point(16, 8);
                var mapInstance = _simpleCache.Get<MapInstance>("piet_village_way");
                source.TraverseMap(mapInstance, point, true);
                break;
            }
            case Nation.Abel:
            {
                Point point;
                point = new Point(11, 13);
                var mapInstance = _simpleCache.Get<MapInstance>("abel_port_way");
                source.TraverseMap(mapInstance, point, true);
                break;
            }
            case Nation.Undine:
            {
                Point point;
                point = new Point(11, 12);
                var mapInstance = _simpleCache.Get<MapInstance>("undine_village_way");
                source.TraverseMap(mapInstance, point, true);
                break;
            }
        }
    }
}
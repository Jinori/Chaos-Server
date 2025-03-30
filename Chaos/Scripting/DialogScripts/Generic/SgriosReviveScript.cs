using Chaos.Collections;
using Chaos.DarkAges.Definitions;
using Chaos.Definitions;
using Chaos.Geometry.Abstractions.Definitions;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.NLog.Logging.Definitions;
using Chaos.NLog.Logging.Extensions;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.DialogScripts.Generic;

public class SgriosReviveScript : DialogScriptBase
{
    private readonly ILogger<SgriosReviveScript> Logger;
    private readonly ISimpleCache SimpleCache;

    public SgriosReviveScript(Dialog subject, ISimpleCache simpleCache, ILogger<SgriosReviveScript> logger)
        : base(subject)
    {
        SimpleCache = simpleCache;
        Logger = logger;
    }

    public override void OnDisplayed(Aisling source)
    {
        Logger.WithTopics(Topics.Entities.Aisling, Topics.Actions.Death)
              .WithProperty(source)
              .LogInformation("{@AislingName} has been revived by Sgrios", source.Name);

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
        source.Display();
        source.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Sgrios mumbles unintelligble gibberish");
        source.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You are revived and sent home.");
        source.Trackers.Enums.TryGetValue(out TutorialQuestStage tutorial);
        Subject.Close(source);

        if (tutorial != TutorialQuestStage.CompletedTutorial)
        {
            var point2 = new Point(5, 8);
            var mapInstance2 = SimpleCache.Get<MapInstance>("mileth_inn");
            source.TraverseMap(mapInstance2, point2, true);
            source.Trackers.Enums.Set(TutorialQuestStage.CompletedTutorial);

            return;
        }

        switch (source.Nation)
        {
            case Nation.Rucesion:
            {
                var point = new Point(7, 3);
                var mapInstance = SimpleCache.Get<MapInstance>("rucesion_inn");
                source.TraverseMap(mapInstance, point, true);

                break;
            }
            case Nation.Mileth:
            {
                var point = new Point(5, 8);
                var mapInstance = SimpleCache.Get<MapInstance>("mileth_inn");
                source.TraverseMap(mapInstance, point, true);

                break;
            }
            case Nation.Exile:
            {
                var point = new Point(7, 5);
                var mapInstance = SimpleCache.Get<MapInstance>("toc");
                source.TraverseMap(mapInstance, point, true);

                break;
            }
            case Nation.Suomi:
            {
                var point = new Point(5, 8);
                var mapInstance = SimpleCache.Get<MapInstance>("suomi_inn");
                source.TraverseMap(mapInstance, point, true);

                break;
            }
            case Nation.Loures:
            {
                var point = new Point(21, 16);
                var mapInstance = SimpleCache.Get<MapInstance>("loures_castle");
                source.TraverseMap(mapInstance, point, true);

                break;
            }
            case Nation.Tagor:
            {
                var point = new Point(5, 8);
                var mapInstance = SimpleCache.Get<MapInstance>("tagor_inn");
                source.TraverseMap(mapInstance, point, true);

                break;
            }
            case Nation.Piet:
            {
                var point = new Point(5, 8);
                var mapInstance = SimpleCache.Get<MapInstance>("piet_inn");
                source.TraverseMap(mapInstance, point, true);

                break;
            }
            case Nation.Abel:
            {
                var point = new Point(5, 8);
                var mapInstance = SimpleCache.Get<MapInstance>("abel_inn");
                source.TraverseMap(mapInstance, point, true);

                break;
            }
            case Nation.Labyrinth:
            {
                var point = new Point(6, 7);
                var mapInstance = SimpleCache.Get<MapInstance>("arena_entrance");
                source.TraverseMap(mapInstance, point, true);

                break;
            }
            case Nation.Undine:
            {
                var point = new Point(5, 11);
                var mapInstance = SimpleCache.Get<MapInstance>("undine_village_way");
                source.TraverseMap(mapInstance, point, true);

                break;
            }
        }
    }
}
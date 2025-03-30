using Chaos.Collections;
using Chaos.DarkAges.Definitions;
using Chaos.Definitions;
using Chaos.Models.Data;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.DialogScripts.Quests.Nightmare;

public class NightmareRogueScript : DialogScriptBase
{
    private readonly ISimpleCache SimpleCache;

    public NightmareRogueScript(Dialog subject, ISimpleCache simpleCache)
        : base(subject)
        => SimpleCache = simpleCache;

    public override void OnDisplaying(Aisling source)
    {
        var hasStage = source.Trackers.Enums.TryGetValue(out NightmareQuestStage stage);

        var ani1 = new Animation
        {
            AnimationSpeed = 100,
            TargetAnimation = 75
        };

        var ani2 = new Animation
        {
            AnimationSpeed = 100,
            TargetAnimation = 54
        };

        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "varuna_initial":
            {
                if (source.UserStatSheet.BaseClass != BaseClass.Rogue)
                    return;

                if (source.UserStatSheet.Level < 80)
                    return;

                switch (hasStage)
                {
                    case false:
                        Subject.AddOption("Marauder Legend", "nightmarerogue1");

                        return;
                    case true when stage == NightmareQuestStage.Started:
                        Subject.AddOption("Marauder Legend", "nightmarerogue1");

                        return;
                    case true when stage == NightmareQuestStage.CompletedNightmareWin1:
                        Subject.Reply(
                            source,
                            "You protected the totem and completed the challenge before you... You are better than I thought. Take this as a reward.",
                            "varuna_initial");
                        source.Trackers.Enums.Set(NightmareQuestStage.CompletedNightmareWin2);
                        source.TryGiveGamePoints(20);
                        source.SendOrangeBarMessage("You received 20 Game Points.");

                        return;
                    case true when stage == NightmareQuestStage.CompletedNightmareLoss1:
                        Subject.Reply(
                            source,
                            "Even though you met the Legend, you succumbed to the challenge as most do. That is okay...",
                            "varuna_initial");
                        source.Trackers.Enums.Set(NightmareQuestStage.CompletedNightmareLoss2);

                        return;
                }

                break;
            }

            case "nightmarerogue9":
            {
                source.Trackers.Enums.Set(NightmareQuestStage.Started);

                break;
            }

            case "nightmareblackrose1":
            {
                source.Animate(ani1);

                break;
            }

            case "nightmareblackrose2":
            {
                source.Inventory.RemoveQuantityByTemplateKey("blackrose", 1);
                source.Trackers.Enums.Set(NightmareQuestStage.MetRequirementsToEnter1);
                Subject.Close(source);
                Point point2;
                point2 = new Point(12, 13);
                var mapInstance2 = SimpleCache.Get<MapInstance>("wildernessold");
                source.TraverseMap(mapInstance2, point2);
                source.UserStatSheet.SetHealthPct(100);
                source.Client.SendAttributes(StatUpdateType.Vitality);

                break;
            }

            case "nightmaretotem1":
            {
                if (source.MapInstance.Name != "Old Wilderness")
                    Subject.Close(source);

                break;
            }

            case "nightmaretotem4":
            {
                if ((hasStage && (stage == NightmareQuestStage.MetRequirementsToEnter1))
                    || (stage == NightmareQuestStage.EnteredDream)
                    || (stage == NightmareQuestStage.SpawnedNightmare))
                {
                    Subject.Close(source);
                    Point point2;
                    point2 = new Point(source.X, source.Y);
                    var mapInstance2 = SimpleCache.Get<MapInstance>("wildernessroguechallenge");
                    source.TraverseMap(mapInstance2, point2);
                    source.Trackers.Enums.Set(NightmareQuestStage.EnteredDream);
                    source.UserStatSheet.SetHealthPct(100);
                    source.Client.SendAttributes(StatUpdateType.Vitality);
                }

                break;
            }
        }
    }
}
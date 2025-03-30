using Chaos.Collections;
using Chaos.DarkAges.Definitions;
using Chaos.Definitions;
using Chaos.Models.Data;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.DialogScripts.Quests.Nightmare;

public class NightmareMonkScript : DialogScriptBase
{
    private readonly ISimpleCache _simpleCache;

    public NightmareMonkScript(Dialog subject, ISimpleCache simpleCache)
        : base(subject)
        => _simpleCache = simpleCache;

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

        if (source.UserStatSheet.BaseClass != BaseClass.Monk)
            return;

        if (source.UserStatSheet.Level < 80)
            return;

        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "bodil_initial":
            {
                switch (hasStage)
                {
                    case false:
                        Subject.AddOption("Pattern Walker Legend", "nightmaremonk1");

                        return;
                    case true when stage == NightmareQuestStage.Started:
                        Subject.AddOption("Pattern Walker Legend", "nightmaremonk1");

                        return;
                    case true when stage == NightmareQuestStage.CompletedNightmareWin1:
                        Subject.Reply(
                            source,
                            "I see you have protected the slaves. That is good. Here, take this as a reward for your hard work.",
                            "bodil_initial");
                        source.Trackers.Enums.Set(NightmareQuestStage.CompletedNightmareWin2);
                        source.TryGiveGamePoints(20);
                        source.SendOrangeBarMessage("You received 20 Game Points.");

                        return;
                    case true when stage == NightmareQuestStage.CompletedNightmareLoss1:
                        Subject.Reply(
                            source,
                            "One day you will learn to use your abilities. Take it as a lesson learned.",
                            "bodil_initial");
                        source.Trackers.Enums.Set(NightmareQuestStage.CompletedNightmareLoss2);

                        return;
                }

                break;
            }

            case "nightmaremonk4":
            {
                source.Trackers.Enums.Set(NightmareQuestStage.Started);

                break;
            }

            case "tocpattern1":
            {
                source.Animate(ani1);

                break;
            }

            case "tocpattern7":
            {
                source.Animate(ani2);
                source.Trackers.Enums.Set(NightmareQuestStage.MetRequirementsToEnter1);

                break;
            }

            case "tocpattern12":
            {
                if ((hasStage && (stage == NightmareQuestStage.MetRequirementsToEnter1))
                    || (stage == NightmareQuestStage.EnteredDream)
                    || (stage == NightmareQuestStage.SpawnedNightmare))
                {
                    Subject.Close(source);
                    Point point2;
                    point2 = new Point(12, 12);
                    var mapInstance2 = _simpleCache.Get<MapInstance>("monknightmarechallenge");
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
using Chaos.Collections;
using Chaos.Common.Definitions;
using Chaos.Definitions;
using Chaos.Models.Data;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.DialogScripts.Quests.Nightmare;

public class NightmareFWizPriestScript : DialogScriptBase
{
    private readonly ISimpleCache _simpleCache; 
    
    public NightmareFWizPriestScript(Dialog subject, ISimpleCache simpleCache)
        : base(subject) =>
        _simpleCache = simpleCache;

    public override void OnDisplaying(Aisling source)
    {
        var hasStage = source.Trackers.Enums.TryGetValue(out NightmareQuestStage stage);

        var ani1 = new Animation()
        {
            AnimationSpeed = 100,
            TargetAnimation = 8
        };
        var ani2 = new Animation()
        {
            AnimationSpeed = 100,
            TargetAnimation = 21
        };
        var ani3 = new Animation()
        {
            AnimationSpeed = 100,
            TargetAnimation = 257
        };

        if ((source.UserStatSheet.BaseClass != BaseClass.Priest) && (source.UserStatSheet.BaseClass != BaseClass.Wizard))
            return;

        if (source.UserStatSheet.Level < 80)
            return;

        if ((source.Gender & Gender.Female) == 0)
            return;

        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "narve_initial":
            {
                switch (hasStage)
                {
                    case false:
                        Subject.AddOption($"Morrigu Legend", "nightmarefwizpriest1");

                        return;
                    case true when (stage == NightmareQuestStage.Started):
                        Subject.AddOption($"Morrigu Legend", "nightmarefwizpriest1");
                       
                        return;
                    case true when (stage == NightmareQuestStage.CompletedNightmareWin1):
                        Subject.Reply(source, "You made it back, I must say I am impressed. Please, take this, you have done very well.");
                        source.Trackers.Enums.Set(NightmareQuestStage.CompletedNightmareWin2);
                        source.TryGiveGamePoints(20);
                        source.SendOrangeBarMessage("You received 20 Game Points.");

                        return;
                    case true when (stage == NightmareQuestStage.CompletedNightmareLoss1):
                        Subject.Reply(source, "The challenges you face may be tougher than this one, prepare for the next.", "narve_initial");
                        source.Trackers.Enums.Set(NightmareQuestStage.CompletedNightmareLoss2);
                        return;
                }

                break;
            }

            case "nightmarefwizpriest5":
            {
                source.Trackers.Enums.Set(NightmareQuestStage.Started);

                break;
            }

            case "bloodyflowers3":
            {
                source.Animate(ani1);
                break;
            }
            
            case "bloodyflowers8":
            {
                source.Animate(ani2);
                break;
            }
            
            case "bloodyflowers12":
            {
                source.Animate(ani3);
                break;
            }

            case "bloodyflowers13":
            {
                if ((hasStage && (stage == NightmareQuestStage.MetRequirementsToEnter1)) || (stage == NightmareQuestStage.EnteredDream) || (stage == NightmareQuestStage.SpawnedNightmare))
                {
                    Point point2;
                    point2 = new Point(14, 12);
                    
                    if (source.UserStatSheet.BaseClass == BaseClass.Wizard)
                    {
                        var mapInstance2 = _simpleCache.Get<MapInstance>("femalewizardnightmarechallenge");
                        source.TraverseMap(mapInstance2, point2, false);
                    }

                    if (source.UserStatSheet.BaseClass == BaseClass.Priest)
                    {
                        var mapInstance2 = _simpleCache.Get<MapInstance>("femalepriestnightmarechallenge");  
                        source.TraverseMap(mapInstance2, point2, false);
                    }
                    source.Trackers.Enums.Set(NightmareQuestStage.EnteredDream);
                    source.UserStatSheet.SetHealthPct(100);
                    source.Client.SendAttributes(StatUpdateType.Vitality);
                }

                break;
            }
            
        }
    }
}
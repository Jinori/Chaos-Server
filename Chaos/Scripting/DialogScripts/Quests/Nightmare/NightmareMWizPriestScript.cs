using Chaos.Collections;
using Chaos.Common.Definitions;
using Chaos.DarkAges.Definitions;
using Chaos.Definitions;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.DialogScripts.Quests.Nightmare;

public class NightmareMWizPriestScript : DialogScriptBase
{
    private readonly ISimpleCache _simpleCache; 
    
    public NightmareMWizPriestScript(Dialog subject, ISimpleCache simpleCache)
        : base(subject) =>
        _simpleCache = simpleCache;

    public override void OnDisplaying(Aisling source)
    {
        var hasStage = source.Trackers.Enums.TryGetValue(out NightmareQuestStage stage);
        
        if ((source.UserStatSheet.BaseClass != BaseClass.Priest) && (source.UserStatSheet.BaseClass != BaseClass.Wizard))
            return;

        if (source.UserStatSheet.Level < 80)
            return;

        if ((source.Gender & Gender.Male) == 0)
            return;

        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "gregory_initial":
            {
                switch (hasStage)
                {
                    case false:
                        Subject.AddOption($"What do you know about these Nightmares?", "nightmaremwizpriest1");

                        return;
                    case true when (stage == NightmareQuestStage.Started):
                        Subject.AddOption($"Tell me about the Cthonic Disciple again", "nightmaremwizpriest1");
                       
                        return;
                    case true when (stage == NightmareQuestStage.CompletedNightmareWin1):
                        Subject.Reply(source, "Good job on your victory, I knew I told the right Aisling. Please, take this, you have done very well.");
                        source.Trackers.Enums.Set(NightmareQuestStage.CompletedNightmareWin2);
                        source.TryGiveGamePoints(20);
                        source.SendOrangeBarMessage("You received 20 Game Points.");
                        return;
                        
                        case true when (stage == NightmareQuestStage.CompletedNightmareWin2):
                            Subject.Reply(source, "I'll always remember you Aisling.");
                        return;
                        
                    case true when (stage == NightmareQuestStage.CompletedNightmareLoss1):
                        Subject.Reply(source, "You may have lost the battle, but you haven't lost the war. Press on and try to forget about it.");
                        source.Trackers.Enums.Set(NightmareQuestStage.CompletedNightmareLoss2);
                        return;
                }

                break;
            }

            case "nightmaremwizpriest10":
            {
                source.Trackers.Enums.Set(NightmareQuestStage.Started);

                break;
            }

            case "cthonicdisciplecoffin7":
            {
                if ((hasStage && (stage == NightmareQuestStage.MetRequirementsToEnter1)) || (stage == NightmareQuestStage.EnteredDream) || (stage == NightmareQuestStage.SpawnedNightmare))
                {
                    Point point2;
                    point2 = new Point(13, 10);
                    
                    if (source.UserStatSheet.BaseClass == BaseClass.Wizard)
                    {
                        var mapInstance2 = _simpleCache.Get<MapInstance>("malewizardnightmarechallenge");
                        source.TraverseMap(mapInstance2, point2, false);
                    }

                    if (source.UserStatSheet.BaseClass == BaseClass.Priest)
                    {
                        var mapInstance2 = _simpleCache.Get<MapInstance>("malepriestnightmarechallenge");  
                        source.TraverseMap(mapInstance2, point2, false);
                    }
                    source.Trackers.Enums.Set(NightmareQuestStage.EnteredDream);
                    source.Inventory.RemoveQuantity("Essence of Theselene", 1);
                    source.Inventory.RemoveQuantity("Essence of Miraelis", 1);
                    source.Inventory.RemoveQuantity("Essence of Skandara", 1);
                    source.Inventory.RemoveQuantity("Essence of Serendael", 1);
                    source.UserStatSheet.SetHealthPct(100);
                    source.Client.SendAttributes(StatUpdateType.Vitality);
                }

                break;
            }
            
        }
    }
}
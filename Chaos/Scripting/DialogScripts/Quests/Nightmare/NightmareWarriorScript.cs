using Chaos.Collections;
using Chaos.Common.Definitions;
using Chaos.Definitions;
using Chaos.Models.Data;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.DialogScripts.Quests.Nightmare;

public class NightmareWarriorScript : DialogScriptBase
{
    private readonly ISimpleCache _simpleCache; 
    
    public NightmareWarriorScript(Dialog subject, ISimpleCache simpleCache)
        : base(subject)
    {
        _simpleCache = simpleCache;
    }

    public override void OnDisplaying(Aisling source)
    {
        var hasStage = source.Trackers.Enums.TryGetValue(out NightmareQuestStage stage);
        
        if (source.UserStatSheet is not { BaseClass: BaseClass.Warrior, Level: >= 80 })
            return;

        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "baki_initial":
            {
                if (!hasStage)
                {
                    Subject.AddOption($"I am the hunter", "nightmarewarrior1");

                    return;
                }

                if (hasStage && (stage == NightmareQuestStage.CompletedNightmareWin1))
                {
                    Subject.Reply(source, "Impressive, you have returned. I am surprised you are still alive. You have my respect. Please, take this.");
                    source.Trackers.Enums.Set(NightmareQuestStage.CompletedNightmareWin2);
                    source.TryGiveGamePoints(20);
                    source.SendOrangeBarMessage("You received 20 Game Points.");

                    return;
                }
                if (hasStage && (stage == NightmareQuestStage.CompletedNightmareWin2))
                {
                    Subject.Reply(source, "I am still amazed by your abilities Brave Champion. Go forth and show the world your strength.");

                    return;
                }
                
                if (hasStage && (stage == NightmareQuestStage.CompletedNightmareLoss1))
                {
                    Subject.Reply(source, "I knew you couldn't handle it, atleast you are still alive. Get out of my hut.");

                    return;
                }
                
                Subject.Reply(source, "What are you still doing here? Are you afraid?");
                break;
            }

            case "nightmarewarrior4":
            {
                source.Trackers.Enums.Set(NightmareQuestStage.Started);

                break;
            }

            case "carnunglade1":
            {
                if (source.UserStatSheet.CurrentHp != 1)
                {
                    Subject.Reply(source, "You have no respect for the Carnun Champion. Begone.", "Close");

                    return;
                }
                break;
            }

            case "carnunglade2":
            {
                if ((hasStage && (stage == NightmareQuestStage.MetRequirementsToEnter1)) || (stage == NightmareQuestStage.EnteredDream) || (stage == NightmareQuestStage.SpawnedNightmare))
                {
                    Point point2;
                    point2 = new Point(18, 5);
                    var mapInstance2 = _simpleCache.Get<MapInstance>("warriornightmarechallenge");
                    source.TraverseMap(mapInstance2, point2, false);
                    source.Trackers.Enums.Set(NightmareQuestStage.EnteredDream);
                    Subject.Close(source);
                    source.UserStatSheet.SetHealthPct(100);
                    source.Client.SendAttributes(StatUpdateType.Vitality);
                }

                break;
            }

            case "nightmare_priestsupport_heal":
            {
                var ani = new Animation()
                {
                    AnimationSpeed = 100,
                    TargetAnimation = 4
                };
                source.Animate(ani);
                source.UserStatSheet.SetHealthPct(100);
                source.Client.SendAttributes(StatUpdateType.Vitality);
                var priest = source.MapInstance.GetEntities<Merchant>().FirstOrDefault(x => x.Name == "Priest");
                priest!.MapInstance.RemoveEntity(priest);
                break;
            }
            
        }
    }
}
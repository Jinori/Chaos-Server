using Chaos.Definitions;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ExperienceDistribution;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.DialogScripts.Quests.Wilderness;

public class SwampMazeQuestScript : DialogScriptBase
{
    private IExperienceDistributionScript ExperienceDistributionScript { get; }
    private readonly IItemFactory ItemFactory;

    public SwampMazeQuestScript(Dialog subject, IItemFactory itemFactory)
        : base(subject)
    {
        ItemFactory = itemFactory;
        ExperienceDistributionScript = DefaultExperienceDistributionScript.Create();
    }

    public override void OnDisplaying(Aisling source)
    {
        var hasStage = source.Trackers.Enums.TryGetValue(out SwampMazeQuest stage);


        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "koda_initial":

                if (source.UserStatSheet.Level < 11)
                {
                    Subject.Reply(source, "skip", "koda_initiallow");
                    return;
                }

                if (stage == SwampMazeQuest.Start)
                {
                    Subject.Reply(source, "skip", "koda_initial2");
                    return;
                }

                if (stage == SwampMazeQuest.Complete)
                {
                    Subject.Reply(source, "skip", "koda_initial3");
                    return;
                }

                break;
                
            case "joda_initial2":
                
                if (stage == SwampMazeQuest.Start)
                {
                    Subject.Reply(source, "skip", "joda_initial");
                    return;
                }

                if (stage == SwampMazeQuest.Complete)
                {
                    Subject.Reply(source, "skip", "joda_initial3");
                    return;
                }
                

                break;
            
            case "koda_bye":
            {
                source.Trackers.Enums.Set(SwampMazeQuest.Start);
                source.SendOrangeBarMessage("Koda seemed worried about his brother.");
            }
                break;
            
            case "joda_quest5":
            {
                if (stage == SwampMazeQuest.Start)
                {
                    source.Trackers.Enums.Set(SwampMazeQuest.Complete);
                    source.TryGiveItems(ItemFactory.Create("mushroomhat"));
                    ExperienceDistributionScript.GiveExp(source, 50000);
                    source.SendOrangeBarMessage("You receive 50000 exp and a Mushroom Hat!");
                }
            }
                break;
        }
    }
}
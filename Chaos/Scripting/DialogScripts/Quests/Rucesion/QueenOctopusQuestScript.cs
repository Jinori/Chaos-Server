using Chaos.Common.Definitions;
using Chaos.Definitions;
using Chaos.Objects.Legend;
using Chaos.Objects.Menu;
using Chaos.Objects.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ExperienceDistribution;
using Chaos.Services.Factories.Abstractions;
using Chaos.Time;

namespace Chaos.Scripting.DialogScripts.Quests.Rucesion;

public class QueenOctopusQuestScript: DialogScriptBase
{
    private readonly IItemFactory ItemFactory;
    private IExperienceDistributionScript ExperienceDistributionScript { get; }


    /// <inheritdoc />
    public QueenOctopusQuestScript(Dialog subject, IItemFactory itemFactory)
        : base(subject)
    {
        ItemFactory = itemFactory;
        ExperienceDistributionScript = DefaultExperienceDistributionScript.Create();
    }

    public override void OnDisplaying(Aisling source)
    {
        var hasStage = source.Trackers.Enums.TryGetValue(out QueenOctopusQuest stage);


        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "maria_initial":
            {
                if ((!hasStage) || (stage == QueenOctopusQuest.None))
                {
                    if (source.UserStatSheet.Level is <= 41 or >= 72)
                        return;
                    Subject.Reply(source, "skip", "maria_initial_quest");
                }
                
                if (stage == QueenOctopusQuest.Liver)
                {
                    Subject.Reply(source, "skip", "queenoctopus_liver");
                    return;
                }

                if (stage == QueenOctopusQuest.Pendant)
                {
                    Subject.Reply(source, "skip", "queenoctopus_liver2");
                }
            }

                break;

            case "queenoctopus_start2":
            {
                source.Trackers.Enums.Set(QueenOctopusQuest.Liver);
            }
                break;
            case "queenoctopus_liver1":
            {
                if (!source.Inventory.RemoveQuantity("liver", 5))
                {
                    Subject.Reply(source,
                        "Please collect 5 liver for me.");

                    return;
                }

                var redpearl = ItemFactory.Create("redpearl");
                ExperienceDistributionScript.GiveExp(source, 250000);
                source.SendOrangeBarMessage($"You received 250,000 experience!");
                source.Trackers.Enums.Set(QueenOctopusQuest.Pendant);
                source.TryGiveItem(redpearl);

                Subject.Reply(source,
                    "Oh wow! That was fast. A deal is a deal. Here is the Red Pearl. The only other clue I can remember is him mentioning a secret entrance in Karlopos Island North. If you see my brother out there please tell him to come home. I'm worried about him.",
                    "QueenOctopus_liver2");
            }

                break;
        }
    }
}
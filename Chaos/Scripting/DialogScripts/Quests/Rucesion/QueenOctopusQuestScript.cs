using Chaos.Definitions;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ExperienceDistribution;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.DialogScripts.Quests.Rucesion;

public class QueenOctopusQuestScript : DialogScriptBase
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
                if (!hasStage || (stage == QueenOctopusQuest.None))
                {
                    if (source.UserStatSheet.Level is <= 41 or >= 72)
                        return;

                    Subject.Reply(source, "skip", "maria_initial_quest");
                }

                switch (stage)
                {
                    case QueenOctopusQuest.Liver:
                        Subject.Reply(source, "skip", "queenoctopus_liver");

                        return;
                    case QueenOctopusQuest.Pendant:
                        Subject.Reply(source, "skip", "queenoctopus_liver2");

                        return;
                    case QueenOctopusQuest.Pendant3:
                        Subject.Reply(source, "skip", "queenoctopus_Queen");

                        break;
                    case QueenOctopusQuest.Queen:
                    case QueenOctopusQuest.Complete:
                        Subject.Reply(source, "Welcome Back. Please make yourself comfortable.");

                        break;
                }
            }

                break;

            case "bret_initial":
            {
                if (stage == QueenOctopusQuest.Pendant)
                {
                    Subject.Reply(source, "skip", "queenoctopus_pendant1");

                    return;
                }

                if (stage == QueenOctopusQuest.Pendant2)
                    Subject.Reply(source, "Be safe.", "close");
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
                    Subject.Reply(
                        source,
                        "Please collect 5 liver for me.");

                    return;
                }

                var redpearl = ItemFactory.Create("redpearl");
                ExperienceDistributionScript.GiveExp(source, 250000);
                source.SendOrangeBarMessage("You received 250,000 experience!");
                source.Trackers.Enums.Set(QueenOctopusQuest.Pendant);
                source.TryGiveItem(ref redpearl);

                Subject.Reply(
                    source,
                    "Oh wow! That was fast. A deal is a deal. Here is the Red Pearl. The only other clue I can remember is him mentioning a secret entrance in Karlopos Island North. If you see my brother out there please tell him to come home. I'm worried about him.",
                    "QueenOctopus_liver2");
            }

                break;

            case "queenoctopus_pendant6":
            {
                source.Trackers.Enums.Set(QueenOctopusQuest.Pendant2);
            }

                break;
            case "queenoctopus_queen2":
            {
                source.Trackers.Enums.Set(QueenOctopusQuest.Queen);
            }

                break;
        }
    }
}
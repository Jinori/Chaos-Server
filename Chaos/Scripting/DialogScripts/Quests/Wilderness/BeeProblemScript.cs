using Chaos.Definitions;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ExperienceDistribution;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.DialogScripts.Quests;

public class BeeProblemScript : DialogScriptBase
{
    private IExperienceDistributionScript ExperienceDistributionScript { get; }
    private readonly IItemFactory ItemFactory;

    public BeeProblemScript(Dialog subject, IItemFactory itemFactory)
        : base(subject)
    {
        ItemFactory = itemFactory;
        ExperienceDistributionScript = DefaultExperienceDistributionScript.Create();
    }


    public override void OnDisplaying(Aisling source)
    {
        var hasStage = source.Trackers.Enums.TryGetValue(out BeeProblem stage);


        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "talula_initial":
                if ((!hasStage) || (stage == BeeProblem.None))
                {
                    if (source.UserStatSheet.Level is <= 1 or >= 16)
                        return;

                    Subject.Reply(source, "skip", "talula_initial");
                    return;
                }

                if (stage == BeeProblem.Started)
                {
                    Subject.Reply(source, "skip", "talula_initial2");
                    return;
                }

                if (stage == BeeProblem.Completed)
                {
                    Subject.Reply(source, "skip", "talula_initial3");
                }

                if (source.UserStatSheet.Level is < 16)
                {
                    Subject.Reply(source, "skip", "talula_initial3");
                }

                break;

            case "talula_bee2":
            {
                source.Trackers.Enums.Set(BeeProblem.Started);
            }
                break;

            case "talula_bee3":
            {
                if (!source.Trackers.Counters.TryGetValue("wilderness_Bee", out var value) || (value < 5))
                {
                    Subject.Reply(source, "I can still hear them buzzing around.");
                    source.SendOrangeBarMessage("You need to kill 5 bees.");

                    return;
                }

                source.TryGiveGamePoints(5);
                ExperienceDistributionScript.GiveExp(source, 5000);
                source.TryGiveItems(ItemFactory.Create("windbelt"));
                source.SendOrangeBarMessage("5000 Exp and Wind Belt Rewarded!");
                source.Trackers.Counters.Remove("wilderness_bee", out _);
                source.Trackers.Enums.Set(BeeProblem.Completed);
            }
                break;
        }
    }
}
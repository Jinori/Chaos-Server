using Chaos.Common.Definitions;
using Chaos.Definitions;
using Chaos.Objects.Legend;
using Chaos.Objects.Menu;
using Chaos.Objects.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ExperienceDistribution;
using Chaos.Time;

namespace Chaos.Scripting.DialogScripts.Quests.Manor;

public class ManorNecklaceScript : DialogScriptBase
{
    private IExperienceDistributionScript ExperienceDistributionScript { get; }
    
    public ManorNecklaceScript(Dialog subject) : base(subject)
    {
        ExperienceDistributionScript = DefaultExperienceDistributionScript.Create();
    }

    public override void OnDisplaying(Aisling source)
    {
        var hasStage = source.Trackers.Enums.TryGetValue(out ManorNecklaceStage stage);
        
        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "zulera_keephernecklace":
            {
                if (stage == ManorNecklaceStage.ObtainedNecklace)
                    source.Trackers.Enums.Set(ManorNecklaceStage.ReturnedNecklace);
                source.Legend.AddUnique(new LegendMark("Returned Zulera's Heirloom", "manorNecklace", MarkIcon.Victory, MarkColor.LightPurple, 1, GameTime.Now));
                source.Client.SendServerMessage(ServerMessageType.OrangeBar1, $"You receive a legend mark. The young one looks terribly sad.");
                break;
            }
            case "zulera_givenecklaceback":
            {
                if (!source.Inventory.HasCount("Zulera's Heirloom", 1))
                {
                    source.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Looks like my necklace isn't in your inventory..");
                    Subject.Close(source);
                    return;
                }

                source.Inventory.RemoveQuantity("Zulera's Heirloom", 1);
                if (stage == ManorNecklaceStage.ObtainedNecklace)
                    source.Trackers.Enums.Set(ManorNecklaceStage.ReturnedNecklace);
                ExperienceDistributionScript.GiveExp(source, 150000);
                source.TryGiveGamePoints(20);
                source.Legend.AddUnique(new LegendMark("Returned Zulera's Heirloom", "manorNecklace", MarkIcon.Heart, MarkColor.Blue, 1, GameTime.Now));
                source.Client.SendServerMessage(ServerMessageType.OrangeBar1, $"You receive twenty gamepoints, legend mark and 150,000 exp!");
                
                break;
            }
            case "zulera_initial":
            {
                if (stage == ManorNecklaceStage.ReturnedNecklace)
                {
                    source.Client.SendServerMessage(ServerMessageType.OrangeBar1, $"She smiles and nods while clutching the necklace.");
                    Subject.Close(source);
                }
                if (stage == ManorNecklaceStage.KeptNecklace)
                {
                    Subject.Text = "I don't really want to talk to you anymore. You're mean!";
                    Subject.NextDialogKey?.Remove(0);
                }
                if (stage == ManorNecklaceStage.ObtainedNecklace)
                {
                    Subject.Text = "You found it! Would you please hand over my precious necklace?";
                    Subject.Type = MenuOrDialogType.Menu;
                    Subject.NextDialogKey?.Remove(0);
                    
                    var option = new DialogOption
                    {
                        DialogKey = "zulera_giveNecklaceBack",
                        OptionText = "Yes, here you go."
                    };
                    var optionTwo = new DialogOption
                    {
                        DialogKey = "zulera_keepHerNecklace",
                        OptionText = "It's nice. I'm keeping it!"
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);
                    if (!Subject.HasOption(optionTwo))
                        Subject.Options.Add(optionTwo);
                }
                break;
            }
            case "zulera_acceptedquest":
            {
                if (!hasStage || (stage == ManorNecklaceStage.None))
                {
                    source.Trackers.Enums.Set(ManorNecklaceStage.AcceptedQuest);
                }
                source.SendOrangeBarMessage("Go find the girl's lost necklace inside the manor!");
                break;
            }
        }
    }
}
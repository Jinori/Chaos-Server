using Chaos.Definitions;
using Chaos.Formulae;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ExperienceDistribution;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.DialogScripts.Crafting.HobbyQuest;

public class ForagingQuestScript : DialogScriptBase
{

    public ForagingQuestScript(Dialog subject, IItemFactory itemFactory)
        : base(subject)
    {
        ItemFactory = itemFactory;
    }
    public bool HasOne;
    private readonly IItemFactory ItemFactory;
    
    private IExperienceDistributionScript ExperienceDistributionScript { get; } = DefaultExperienceDistributionScript.Create();
    public override void OnDisplaying(Aisling source)
    {
        var sourceforagingmarks = source.Legend.GetCount("forage");
        var tnl = LevelUpFormulae.Default.CalculateTnl(source);
        var tenPercent = Convert.ToInt32(.10 * tnl);
        var fifteenpercent = Convert.ToInt32(.15 * tnl);
        var twentypercent = Convert.ToInt32(.20 * tnl);
        var twentyfivepercent = Convert.ToInt32(.25 * tnl);
        var thirtypercent = Convert.ToInt32(.30 * tnl);
        var thirtyfivepercent = Convert.ToInt32(.35 * tnl);
        var fortypercent = Convert.ToInt32(.40 * tnl);
        var fortyfivepercent = Convert.ToInt32(.45 * tnl);
        var fiftypercent = Convert.ToInt32(.50 * tnl);
        
        switch (Subject.Template.TemplateKey.ToLower())
        {

            case "goran_initial":
                if (source.Trackers.Flags.HasFlag(Hobbies.Foraging) && sourceforagingmarks >= 250 )
                {
                    Subject.Options.Insert(
                        0,
                        new DialogOption
                        {
                            DialogKey = "foragingquest_initial",
                            OptionText = "I am an Experienced Forager now."
                        });
                }

                break;

            case "foragingquest_initial":
            {
                if (source.Trackers.Flags.HasFlag(ForagingQuest.CompletedForaging))
                {
                    Subject.Reply(source, "The greenery fears your presence now. It tells me so... *goran chuckles*", "goran_initial");
                    return;
                }
                
                if (sourceforagingmarks >= 250 && !source.Trackers.Flags.HasFlag(ForagingQuest.Reached250))
                {
                    source.Trackers.Flags.AddFlag(ForagingQuest.Reached250);

                    if (source.StatSheet.Level != 99)
                    {
                        ExperienceDistributionScript.GiveExp(source, tenPercent);
                    }
                    else
                    {
                        ExperienceDistributionScript.GiveExp(source, 1000000);
                    }
                    source.GiveGoldOrSendToBank(10000);
                    source.TryGiveGamePoints(5);
                    HasOne = true;
                }

                if (sourceforagingmarks >= 500 && !source.Trackers.Flags.HasFlag(ForagingQuest.Reached500))
                {
                    source.Trackers.Flags.AddFlag(ForagingQuest.Reached500);
                    if (source.StatSheet.Level != 99)
                    {
                        ExperienceDistributionScript.GiveExp(source, tenPercent);
                    }
                    else
                    {
                        ExperienceDistributionScript.GiveExp(source, 5000000);
                    }
                    source.GiveGoldOrSendToBank(25000);
                    source.TryGiveGamePoints(5);
                    HasOne = true;
                }
                if (sourceforagingmarks >= 800 && !source.Trackers.Flags.HasFlag(ForagingQuest.Reached800))
                {
                    source.Trackers.Flags.AddFlag(ForagingQuest.Reached800);
                    if (source.StatSheet.Level != 99)
                    {
                        ExperienceDistributionScript.GiveExp(source, fifteenpercent);
                    }
                    else
                    {
                        ExperienceDistributionScript.GiveExp(source, 10000000);
                    }
                    source.GiveGoldOrSendToBank(50000);
                    source.TryGiveGamePoints(5);
                    HasOne = true;
                }
                if (sourceforagingmarks >= 1500 && !source.Trackers.Flags.HasFlag(ForagingQuest.Reached1500))
                {
                    source.Trackers.Flags.AddFlag(ForagingQuest.Reached1500);
                    if (source.StatSheet.Level != 99)
                    {
                        ExperienceDistributionScript.GiveExp(source, twentypercent);
                    }
                    else
                    {
                        ExperienceDistributionScript.GiveExp(source, 20000000);
                    }
                    source.GiveGoldOrSendToBank(100000);
                    source.TryGiveGamePoints(5);
                    HasOne = true;
                }
                if (sourceforagingmarks >= 3000 && !source.Trackers.Flags.HasFlag(ForagingQuest.Reached3000))
                {
                    var clothglove1 = ItemFactory.Create("ironglove");
                    source.Trackers.Flags.AddFlag(ForagingQuest.Reached3000);
                    if (source.StatSheet.Level != 99)
                    {
                        ExperienceDistributionScript.GiveExp(source, twentyfivepercent);
                    }
                    else
                    {
                        ExperienceDistributionScript.GiveExp(source, 25000000);
                    }
                    source.GiveGoldOrSendToBank(150000);
                    source.TryGiveGamePoints(10);
                    source.GiveItemOrSendToBank(clothglove1);
                    source.SendOrangeBarMessage("Goran hands you a new cloth glove!");
                    HasOne = true;
                }
                if (sourceforagingmarks >= 5000 && !source.Trackers.Flags.HasFlag(ForagingQuest.Reached5000))
                {
                    source.Trackers.Flags.AddFlag(ForagingQuest.Reached5000);
                    if (source.StatSheet.Level != 99)
                    {
                        ExperienceDistributionScript.GiveExp(source, twentyfivepercent);
                    }
                    else
                    {
                        ExperienceDistributionScript.GiveExp(source, 30000000);
                    }
                    source.GiveGoldOrSendToBank(200000);
                    source.TryGiveGamePoints(10);
                    HasOne = true;
                }
                if (sourceforagingmarks >= 7500 && !source.Trackers.Flags.HasFlag(ForagingQuest.Reached7500))
                {
                    source.Trackers.Flags.AddFlag(ForagingQuest.Reached7500);
                    if (source.StatSheet.Level != 99)
                    {
                        ExperienceDistributionScript.GiveExp(source, thirtypercent);
                    }
                    else
                    {
                        ExperienceDistributionScript.GiveExp(source, 45000000);
                    }
                    source.GiveGoldOrSendToBank(300000);
                    source.TryGiveGamePoints(10);
                    HasOne = true;
                }
                if (sourceforagingmarks >= 10000 && !source.Trackers.Flags.HasFlag(ForagingQuest.Reached10000))
                {
                    source.Trackers.Flags.AddFlag(ForagingQuest.Reached10000);
                    if (source.StatSheet.Level != 99)
                    {
                        ExperienceDistributionScript.GiveExp(source, thirtyfivepercent);
                    }
                    else
                    {
                        ExperienceDistributionScript.GiveExp(source, 50000000);
                    }
                    source.GiveGoldOrSendToBank(500000);
                    source.TryGiveGamePoints(10);
                    HasOne = true;
                }
                if (sourceforagingmarks >= 12500 && !source.Trackers.Flags.HasFlag(ForagingQuest.Reached12500))
                {
                    source.Trackers.Flags.AddFlag(ForagingQuest.Reached12500);
                    if (source.StatSheet.Level != 99)
                    {
                        ExperienceDistributionScript.GiveExp(source, fortypercent);
                    }
                    else
                    {
                        ExperienceDistributionScript.GiveExp(source, 50000000);
                    }
                    source.GiveGoldOrSendToBank(750000);
                    source.TryGiveGamePoints(15);
                    HasOne = true;
                }
                if (sourceforagingmarks >= 15000 && !source.Trackers.Flags.HasFlag(ForagingQuest.Reached15000))
                {
                    var clothglove2 = ItemFactory.Create("mythrilglove");
                    source.Trackers.Flags.AddFlag(ForagingQuest.Reached15000);
                    if (source.StatSheet.Level != 99)
                    {
                        ExperienceDistributionScript.GiveExp(source, fortyfivepercent);
                    }
                    else
                    {
                        ExperienceDistributionScript.GiveExp(source, 75000000);
                    }
                    source.GiveGoldOrSendToBank(1000000);
                    source.TryGiveGamePoints(15);
                    source.SendOrangeBarMessage("Goran hands you a new cloth glove!");
                    source.GiveItemOrSendToBank(clothglove2);
                    HasOne = true;
                }
                if (sourceforagingmarks >= 20000 && !source.Trackers.Flags.HasFlag(ForagingQuest.Reached20000))
                {
                    source.Trackers.Flags.AddFlag(ForagingQuest.Reached20000);
                    if (source.StatSheet.Level != 99)
                    {
                        ExperienceDistributionScript.GiveExp(source, fiftypercent);
                    }
                    else
                    {
                        ExperienceDistributionScript.GiveExp(source, 75000000);
                    }
                    source.GiveGoldOrSendToBank(2000000);
                    source.TryGiveGamePoints(20);
                    HasOne = true;
                }
                if (sourceforagingmarks >= 25000 && !source.Trackers.Flags.HasFlag(ForagingQuest.Reached25000))
                {
                    source.Trackers.Flags.AddFlag(ForagingQuest.Reached25000);
                    if (source.StatSheet.Level != 99)
                    {
                        ExperienceDistributionScript.GiveExp(source, fiftypercent);
                    }
                    else
                    {
                        ExperienceDistributionScript.GiveExp(source, 100000000);
                    }
                    source.GiveGoldOrSendToBank(2500000);
                    source.TryGiveGamePoints(25);
                    HasOne = true;
                }
                
                if (sourceforagingmarks >= 30000 && !source.Trackers.Flags.HasFlag(ForagingQuest.Reached30000))
                {
                    source.Trackers.Flags.AddFlag(ForagingQuest.Reached30000);
                    if (source.StatSheet.Level != 99)
                    {
                        ExperienceDistributionScript.GiveExp(source, fiftypercent);
                    }
                    else
                    {
                        ExperienceDistributionScript.GiveExp(source, 125000000);
                    }
                    source.GiveGoldOrSendToBank(5000000);
                    source.TryGiveGamePoints(50);
                    HasOne = true;
                }
                
                if (sourceforagingmarks >= 40000 && !source.Trackers.Flags.HasFlag(ForagingQuest.Reached40000))
                {
                    source.Trackers.Flags.AddFlag(ForagingQuest.Reached40000);
                    if (source.StatSheet.Level != 99)
                    {
                        ExperienceDistributionScript.GiveExp(source, fiftypercent);
                    }
                    else
                    {
                        ExperienceDistributionScript.GiveExp(source, 150000000);
                    }
                    source.GiveGoldOrSendToBank(7500000);
                    source.TryGiveGamePoints(75);
                    HasOne = true;
                }
                
                if (sourceforagingmarks >= 50000 && !source.Trackers.Flags.HasFlag(ForagingQuest.Reached50000))
                {
                    var clothglove3 = ItemFactory.Create("hybrasylglove");
                    source.Trackers.Flags.AddFlag(ForagingQuest.Reached50000);
                    source.Trackers.Flags.AddFlag(ForagingQuest.CompletedForaging);
                    if (source.StatSheet.Level != 99)
                    {
                        ExperienceDistributionScript.GiveExp(source, fiftypercent);
                    }
                    else
                    {
                        ExperienceDistributionScript.GiveExp(source, 200000000);
                    }
                    source.SendOrangeBarMessage("Goran hands you a new cloth glove!");
                    source.GiveItemOrSendToBank(clothglove3);
                    source.GiveGoldOrSendToBank(10000000);
                    source.TryGiveGamePoints(100);
                    Subject.Reply(source, "You have put so much dedication into Foraging, I am so glad you've enjoyed that time. I have nothing left to give you Aisling.", "goran_initial");
                    return;
                }

                if (HasOne)
                {
                    Subject.Reply(source,
                        "Not many keep foraging like you Aisling, you're getting better with each day. I must reward you a little something.", "goran_initial");
                    return;
                }

                if (!HasOne)
                {
                    Subject.Reply(source, "Your skill hasn't improved since the last time I saw you Aisling, keep on foraging!", "goran_initial");
                    return;
                }

                break;
            }
        }
    }
}
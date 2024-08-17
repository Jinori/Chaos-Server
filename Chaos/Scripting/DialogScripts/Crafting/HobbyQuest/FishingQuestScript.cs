using Chaos.Definitions;
using Chaos.Formulae;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ExperienceDistribution;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.DialogScripts.Crafting.HobbyQuest;

public class FishingQuestScript : DialogScriptBase
{

    public FishingQuestScript(Dialog subject, IItemFactory itemFactory)
        : base(subject)
    {
        ItemFactory = itemFactory;
    }
    public bool HasOne;
    private readonly IItemFactory ItemFactory;
    
    private IExperienceDistributionScript ExperienceDistributionScript { get; } = DefaultExperienceDistributionScript.Create();
    public override void OnDisplaying(Aisling source)
    {
        var sourcefishingmarks = source.Legend.GetCount("fish");
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

            case "kamel_initial":
                if (source.Trackers.Flags.HasFlag(Hobbies.Fishing) && sourcefishingmarks >= 250 )
                {
                    Subject.Options.Insert(
                        0,
                        new DialogOption
                        {
                            DialogKey = "fishingquest_initial",
                            OptionText = "I am an Experienced Fisher now."
                        });
                }

                break;

            case "fishingquest_initial":
            {
                if (source.Trackers.Flags.HasFlag(FishingQuest.CompletedFishing))
                {
                    Subject.Reply(source, "You're more experienced than I am now, fishing is your domain. I am so proud.");
                    return;
                }
                
                if (sourcefishingmarks >= 250 && !source.Trackers.Flags.HasFlag(FishingQuest.Reached250))
                {
                    source.Trackers.Flags.AddFlag(FishingQuest.Reached250);

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

                if (sourcefishingmarks >= 500 && !source.Trackers.Flags.HasFlag(FishingQuest.Reached500))
                {
                    source.Trackers.Flags.AddFlag(FishingQuest.Reached500);
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
                if (sourcefishingmarks >= 800 && !source.Trackers.Flags.HasFlag(FishingQuest.Reached800))
                {
                    source.Trackers.Flags.AddFlag(FishingQuest.Reached800);
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
                if (sourcefishingmarks >= 1500 && !source.Trackers.Flags.HasFlag(FishingQuest.Reached1500))
                {
                    source.Trackers.Flags.AddFlag(FishingQuest.Reached1500);
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
                if (sourcefishingmarks >= 3000 && !source.Trackers.Flags.HasFlag(FishingQuest.Reached3000))
                {
                    var fishingpole1 = ItemFactory.Create("goodfishingpole");
                    source.Trackers.Flags.AddFlag(FishingQuest.Reached3000);
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
                    source.GiveItemOrSendToBank(fishingpole1);
                    source.SendOrangeBarMessage("Kamel hands you a new fishing pole!");
                    HasOne = true;
                }
                if (sourcefishingmarks >= 5000 && !source.Trackers.Flags.HasFlag(FishingQuest.Reached5000))
                {
                    source.Trackers.Flags.AddFlag(FishingQuest.Reached5000);
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
                if (sourcefishingmarks >= 7500 && !source.Trackers.Flags.HasFlag(FishingQuest.Reached7500))
                {
                    source.Trackers.Flags.AddFlag(FishingQuest.Reached7500);
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
                if (sourcefishingmarks >= 10000 && !source.Trackers.Flags.HasFlag(FishingQuest.Reached10000))
                {
                    source.Trackers.Flags.AddFlag(FishingQuest.Reached10000);
                    if (source.StatSheet.Level != 99)
                    {
                        ExperienceDistributionScript.GiveExp(source, thirtypercent);
                    }
                    else
                    {
                        ExperienceDistributionScript.GiveExp(source, 50000000);
                    }
                    source.GiveGoldOrSendToBank(500000);
                    source.TryGiveGamePoints(10);
                    HasOne = true;
                }
                if (sourcefishingmarks >= 12500 && !source.Trackers.Flags.HasFlag(FishingQuest.Reached12500))
                {
                    source.Trackers.Flags.AddFlag(FishingQuest.Reached12500);
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
                if (sourcefishingmarks >= 15000 && !source.Trackers.Flags.HasFlag(FishingQuest.Reached15000))
                {
                    var fishingpole2 = ItemFactory.Create("greatfishingpole");
                    source.Trackers.Flags.AddFlag(FishingQuest.Reached15000);
                    if (source.StatSheet.Level != 99)
                    {
                        ExperienceDistributionScript.GiveExp(source, fiftypercent);
                    }
                    else
                    {
                        ExperienceDistributionScript.GiveExp(source, 75000000);
                    }
                    source.GiveGoldOrSendToBank(1000000);
                    source.TryGiveGamePoints(15);
                    source.SendOrangeBarMessage("Kamel hands you a new fishing pole!");
                    source.GiveItemOrSendToBank(fishingpole2);
                    HasOne = true;
                }
                if (sourcefishingmarks >= 20000 && !source.Trackers.Flags.HasFlag(FishingQuest.Reached20000))
                {
                    source.Trackers.Flags.AddFlag(FishingQuest.Reached20000);
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
                if (sourcefishingmarks >= 25000 && !source.Trackers.Flags.HasFlag(FishingQuest.Reached25000))
                {
                    source.Trackers.Flags.AddFlag(FishingQuest.Reached25000);
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
                
                if (sourcefishingmarks >= 30000 && !source.Trackers.Flags.HasFlag(FishingQuest.Reached30000))
                {
                    source.Trackers.Flags.AddFlag(FishingQuest.Reached30000);
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
                
                if (sourcefishingmarks >= 40000 && !source.Trackers.Flags.HasFlag(FishingQuest.Reached40000))
                {
                    source.Trackers.Flags.AddFlag(FishingQuest.Reached40000);
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
                
                if (sourcefishingmarks >= 50000 && !source.Trackers.Flags.HasFlag(FishingQuest.Reached50000))
                {
                    var fishingpole3 = ItemFactory.Create("grandfishingpole");
                    source.Trackers.Flags.AddFlag(FishingQuest.Reached50000);
                    source.Trackers.Flags.AddFlag(FishingQuest.CompletedFishing);
                    if (source.StatSheet.Level != 99)
                    {
                        ExperienceDistributionScript.GiveExp(source, fiftypercent);
                    }
                    else
                    {
                        ExperienceDistributionScript.GiveExp(source, 200000000);
                    }
                    source.SendOrangeBarMessage("Kamel hands you a new fishing pole!");
                    source.GiveItemOrSendToBank(fishingpole3);
                    source.GiveGoldOrSendToBank(10000000);
                    source.TryGiveGamePoints(100);
                    Subject.Reply(source, "This is the end of the road, I have nothing more to give you. You are a true fishermen.");
                    return;
                }

                if (HasOne)
                {
                    Subject.Reply(source,
                        "You've done excellent getting those fish! I can't say I'm not proud. Here's something for your troubles. Come back anytime!", "kamel_initial");
                    return;
                }

                if (!HasOne)
                {
                    Subject.Reply(source, "You haven't gotten enough experience fishing yet Aisling, keep on fishing!", "kamel_initial");
                    return;
                }

                break;
            }
        }
    }
}
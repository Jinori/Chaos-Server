using Chaos.Common.Definitions;
using Chaos.Definitions;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ExperienceDistribution;
using Chaos.Services.Factories.Abstractions;
using Humanizer;

namespace Chaos.Scripting.DialogScripts.Quests.MainStory;

public class MainStory2Script(
    Dialog subject,
    IItemFactory itemFactory)
    : DialogScriptBase(subject)
{
    private IExperienceDistributionScript ExperienceDistributionScript { get; } =
        DefaultExperienceDistributionScript.Create();

    private readonly IItemFactory ItemFactory = itemFactory;
    

    public override void OnDisplaying(Aisling source)
    {
        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "mainstory_miraelis_initial":
            {
                if (source.Trackers.Enums.HasValue(MainStoryEnums.RetryServant))
                {
                    Subject.Reply(source, "Go back to Eingren Manor third floor with a group and take down the servant, figure out what he knows about where the Summoner went.");
                    return;
                }
                
                if (source.Trackers.Enums.HasValue(MainStoryEnums.CompletedPreMasterMainStory))
                {
                    Subject.Reply(source, "You've done very well this far Aisling. You've by far passed our expectations of you. With the Summoner's minions lurking around Unora, it is not safe. Please be careful until we get together a plan on taking down these creants.");
                    return;
                }
                
                if (source.Trackers.Enums.HasValue(MainStoryEnums.KilledSummoner))
                {
                    Subject.Reply(source, "Skip", "defeatedsummoner1");
                    return;
                }
                
                if (source.Trackers.Enums.HasValue(MainStoryEnums.FoundSummoner2))
                {
                    Subject.Reply(source, "It's great that you've found him again but this time we need to defeat him. Return to the Cthonic Remains and defeat the Summoner.");
                    return;
                }

                if (source.Trackers.Enums.HasValue(MainStoryEnums.CompletedServant))
                {
                    if (source.Trackers.TimedEvents.HasActiveEvent("servantwait", out var cdtime))
                    {
                        Subject.Reply(source, $"We are still gathering information about what the Summoner is doing in the Cthonic Remains, please wait {cdtime.Remaining.Humanize()}.");
                    }
                    
                    if (source.UserStatSheet.Level < 99)
                    {
                        Subject.Reply(source, "We discovered what the summoner is up to, but unfortunately we don't feel comfortable sending you against it just yet. You are still slightly too weak to handle the Summoner.");
                        return;
                    }
                    
                    Subject.Reply(source, "We have no new information for you yet.");
                }
                
                if (source.Trackers.Enums.HasValue(MainStoryEnums.DefeatedServant))
                {
                    Subject.Reply(source, "Skip", "defeatedservant1");
                    return;
                }
                
                if (source.Trackers.Enums.HasValue(MainStoryEnums.Entered3rdFloor))
                {
                    Subject.Reply(source, "Skip", "sawsummoner1");
                    return;
                }
                
                if (source.Trackers.Enums.HasValue(MainStoryEnums.CompletedTrials))
                {
                    if (source.Trackers.TimedEvents.HasActiveEvent("mainstorycd1", out var cdtime))
                    {
                        Subject.Reply(source,$"We are still gathering information regarding the Summoner's whereabouts. We should be finished in {cdtime.Remaining.Humanize()}. Please return then.");
                        return;
                    }

                    if (source.UserStatSheet.Level < 97)
                    {
                        Subject.Reply(source, "We finished gathering the information and the task we must ask of you Aisling may be a bit too strong. Please return to us when you are stronger.");
                        return;
                    }
                    
                    Subject.Reply(source, "Skip", "summoner_initial3");
                    return;
                }

                if (source.Trackers.Enums.HasValue(MainStoryEnums.SearchForSummoner))
                {
                    Subject.Reply(source, "You have all the information we know now, the summoner is somewhere on the third floor of Eingren Manor. Please go search for him, make sure you bring a party, he is not going to be kind to visitors.");
                }
                break;
            }

            case "sawsummoner4":
            {
                source.Trackers.Enums.Set(MainStoryEnums.RetryServant);
                return;
            }
            
            case "summoner_initial5":
            {
                source.Trackers.Enums.Set(MainStoryEnums.SearchForSummoner);
                var trueartifact = ItemFactory.Create("trueelementalartifact");
                source.GiveItemOrSendToBank(trueartifact);
                source.SendOrangeBarMessage("Find the Summoner on the third floor of Eingren Manor.");
                return;
            }

            case "defeatedservant3":
            {
                var pentagearDictionary = new Dictionary<(BaseClass, Gender), string[]>
                {
                    { (BaseClass.Warrior, Gender.Male), ["hybrasylarmor", "hybrasylhelmet"] },
                    { (BaseClass.Warrior, Gender.Female), ["hybrasylplate", "hybrasylhelmet"] },
                    { (BaseClass.Monk, Gender.Male), ["mountaingarb"] },
                    { (BaseClass.Monk, Gender.Female), ["seagarb"] },
                    { (BaseClass.Rogue, Gender.Male), ["bardocle"] },
                    { (BaseClass.Rogue, Gender.Female), ["kagum"] },
                    { (BaseClass.Priest, Gender.Male), ["dalmatica"] },
                    { (BaseClass.Priest, Gender.Female), ["bansagart"] },
                    { (BaseClass.Wizard, Gender.Male), ["duinuasal"] },
                    { (BaseClass.Wizard, Gender.Female), ["clamyth"] }
                };
                
                var gearKey = (source.UserStatSheet.BaseClass, source.Gender);
                if (pentagearDictionary.TryGetValue(gearKey, out var armor))
                {
                    foreach (var gearItemName in armor)
                    {
                        var gearItem = ItemFactory.Create(gearItemName);
                        source.GiveItemOrSendToBank(gearItem);
                    }
                }
                
                source.Trackers.TimedEvents.AddEvent("servantwait", TimeSpan.FromHours(3), true);
                source.Trackers.Enums.Set(MainStoryEnums.CompletedServant);
                ExperienceDistributionScript.GiveExp(source, 750000);
                source.TryGiveGamePoints(10);
                source.TryGiveGold(125000);
                source.SendOrangeBarMessage("Wait for Miraelis to gather intel. ((3 Hours)).");
                return;
            }

            case "defeatedsummoner3":
            {
                source.Trackers.Enums.Set(MainStoryEnums.CompletedPreMasterMainStory);
                source.SendOrangeBarMessage("Wait for Goddess Miraelis to have more information.");
                return;
            }
        }
    }
}
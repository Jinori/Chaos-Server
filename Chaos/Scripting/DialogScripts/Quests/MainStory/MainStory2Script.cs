using Chaos.Definitions;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ExperienceDistribution;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;
using Humanizer;

namespace Chaos.Scripting.DialogScripts.Quests.MainStory;

public class MainStory2Script(
    Dialog subject,
    IItemFactory itemFactory,
    IMerchantFactory merchantFactory,
    ISimpleCache simpleCache,
    IDialogFactory dialogFactory)
    : DialogScriptBase(subject)
{
    private readonly ISimpleCache SimpleCache = simpleCache;

    private IExperienceDistributionScript ExperienceDistributionScript { get; } =
        DefaultExperienceDistributionScript.Create();

    public override void OnDisplaying(Aisling source)
    {
        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "mainstory_miraelis_initial":
            {
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
                
                if (source.Trackers.Enums.HasValue(MainStoryEnums.DefeatedServant))
                {
                    Subject.Reply(source, "Skip", "defeatedservant1");
                    return;
                }
                
                if (source.Trackers.Enums.HasValue(MainStoryEnums.FoundSummoner))
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
                    
                    Subject.Reply(source, "Skip", "summoner_initial3");
                    return;
                }

                if (source.Trackers.Enums.HasValue(MainStoryEnums.SearchForSummoner))
                {
                    Subject.Reply(source, "You have all the information we know now, the summoner is somewhere on the third floor of Eingren Manor. Please go search for him, make sure you bring a party, he is not going to be kind to visitors.");
                    return;
                }
                break;
            }

            case "summoner_initial5":
            {
                source.Trackers.Enums.Set(MainStoryEnums.SearchForSummoner);
                source.SendOrangeBarMessage("Find the Summoner on the third floor of Eingren Manor.");
                return;
            }

            case "defeatedservant3":
            {
                source.Trackers.Enums.Set(MainStoryEnums.SearchForSummoner2);
                source.SendOrangeBarMessage("Find the Summoner in the Cthonic Remains.");
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
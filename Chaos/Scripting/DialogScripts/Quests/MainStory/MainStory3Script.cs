using Chaos.Definitions;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ExperienceDistribution;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.DialogScripts.Quests.MainStory;

public class MainStory3Script(Dialog subject, IItemFactory itemFactory) : DialogScriptBase(subject)
{
    private IExperienceDistributionScript ExperienceDistributionScript { get; } = DefaultExperienceDistributionScript.Create();

    public override void OnDisplaying(Aisling source)
    {
        var hasStage = source.Trackers.Enums.TryGetValue(out MainstoryMasterEnums _);

        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "mainstory_miraelis_masterinitial":
            {
                if (!source.UserStatSheet.Master || !source.Trackers.Enums.HasValue(MainStoryEnums.CompletedPreMasterMainStory))
                {
                    Subject.Reply(source, "I don't know how you got here, tell a GM.");

                    return;
                }

                if (!hasStage)
                {
                    Subject.Reply(source, "Skip", "startDungeon1");

                    return;
                }

                if (source.Trackers.Flags.HasFlag(MainstoryFlags.FinishedCreants))
                    source.Trackers.Enums.Set(MainstoryMasterEnums.CompletedCreants);

                if (source.Trackers.Enums.HasValue(MainstoryMasterEnums.StartedDungeon))
                {
                    // List of boss flags
                    var bossFlags = new Dictionary<string, CthonicDemiseBoss>
                    {
                        {
                            "John", CthonicDemiseBoss.John
                        },
                        {
                            "Jane", CthonicDemiseBoss.Jane
                        },
                        {
                            "Roy", CthonicDemiseBoss.Roy
                        },
                        {
                            "Ray", CthonicDemiseBoss.Ray
                        },
                        {
                            "Mike", CthonicDemiseBoss.Mike
                        },
                        {
                            "Mary", CthonicDemiseBoss.Mary
                        },
                        {
                            "Phil", CthonicDemiseBoss.Phil
                        },
                        {
                            "Pam", CthonicDemiseBoss.Pam
                        },
                        {
                            "William", CthonicDemiseBoss.William
                        },
                        {
                            "Wanda", CthonicDemiseBoss.Wanda
                        }
                    };

                    // Check which bosses have been defeated
                    var defeatedBosses = bossFlags.Where(boss => source.Trackers.Flags.HasFlag(boss.Value))
                                                  .Select(boss => boss.Key)
                                                  .ToList();

                    if (defeatedBosses.Count > 0)
                    {
                        // Create a properly formatted list
                        var defeatedBossList = defeatedBosses.Count switch
                        {
                            1 => defeatedBosses[0],
                            2 => $"{defeatedBosses[0]} and {defeatedBosses[1]}",
                            _ => string.Join(", ", defeatedBosses.Take(defeatedBosses.Count - 1)) + $", and {defeatedBosses.Last()}"
                        };

                        Subject.Reply(
                            source,
                            $"You have already defeated: {defeatedBossList}. At the bottom of the Cthonic Remains, there are still secrets to be found.");
                    } else
                        Subject.Reply(
                            source,
                            "I've already told you, at the bottom of the Cthonic Remains, there are secrets to be found.");

                    return;
                }

                if (source.Trackers.Enums.HasValue(MainstoryMasterEnums.FinishedDungeon))
                {
                    Subject.Reply(source, "Skip", "finishedDungeon1");

                    return;
                }

                if (source.Trackers.Enums.HasValue(MainstoryMasterEnums.CompletedDungeon))
                {
                    var vitality = source.UserStatSheet.MaximumHp + source.UserStatSheet.MaximumMp * 2;

                    if (vitality < 60000)
                    {
                        Subject.Reply(source, "Skip", "mainstory_miraelis_creantvitality");

                        return;
                    }

                    Subject.Reply(source, "Skip", "mainstory_miraelis_creantinitial");

                    return;
                }

                if (source.Trackers.Enums.HasValue(MainstoryMasterEnums.StartedCreants))
                {
                    Subject.Reply(
                        source,
                        "Search the Shinewood Forest, Oren Ruins, Mount Giragan, and Oren Sewers for the creants. They will come once you step to their altar.");

                    return;
                }

                if (source.Trackers.Enums.HasValue(MainstoryMasterEnums.KilledCreants))
                {
                    Subject.Reply(source, "Skip", "finishedCreants1");

                    return;
                }

                if (source.Trackers.Enums.HasValue(MainstoryMasterEnums.CompletedCreants))
                    Subject.Reply(
                        source,
                        "The world of Unora is currently safe, for now... I will call for you again someday brave Aisling.");

                break;
            }

            case "startdungeon4":
            {
                source.Trackers.Enums.Set(MainstoryMasterEnums.StartedDungeon);
                source.SendOrangeBarMessage("Head to the bottom of the Cthonic Remains and find that Army.");

                return;
            }

            case "finisheddungeon2":
            {
                source.Trackers.Enums.Set(MainstoryMasterEnums.CompletedDungeon);
                ExperienceDistributionScript.GiveExp(source, 50000000);
                source.TryGiveGamePoints(20);

                return;
            }

            case "mainstory_miraelis_creantinitial7":
            {
                source.Trackers.Enums.Set(MainstoryMasterEnums.StartedCreants);
                source.SendOrangeBarMessage("To survive the Creants, you'll need atleast 25,000 Health.");

                return;
            }

            case "finishedcreants4":
            {
                source.Trackers.Enums.Set(MainstoryMasterEnums.CompletedCreants);
                source.Trackers.Flags.AddFlag(MainstoryFlags.FinishedCreants);
                source.Trackers.Flags.AddFlag(MainstoryFlags.ReceivedRewards);
                source.Trackers.Enums.Set(ClassStatBracket.Grandmaster);
                ExperienceDistributionScript.GiveExp(source, 100000000);
                source.TryGiveGamePoints(25);
                var godsstar = itemFactory.Create("godsstar");
                source.GiveItemOrSendToBank(godsstar);
                source.SendOrangeBarMessage("Miraelis hands you a Star from above.");

                return;
            }
        }
    }
}
using Chaos.Definitions;
using Chaos.Extensions.Common;
using Chaos.Formulae;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.NLog.Logging.Definitions;
using Chaos.NLog.Logging.Extensions;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ExperienceDistribution;
using Humanizer;


namespace Chaos.Scripting.DialogScripts.Quests.Astrid;

public class TheSacrificeQuestScript : DialogScriptBase
{
    private readonly ILogger<TheSacrificeQuestScript> Logger;
    private IExperienceDistributionScript ExperienceDistributionScript { get; }

    public TheSacrificeQuestScript(Dialog subject, ILogger<TheSacrificeQuestScript> logger) 
        : base(subject)
    {
        Logger = logger;
        ExperienceDistributionScript = DefaultExperienceDistributionScript.Create();
    }

    public override void OnDisplaying(Aisling source)
    {
        var hasStage = source.Trackers.Enums.TryGetValue(out TheSacrificeQuestStage stage);
        var tnl = LevelUpFormulae.Default.CalculateTnl(source);
        var fivePercent = Convert.ToInt32(.5 * tnl);
        var fiftyPercent = Convert.ToInt32(.5 * tnl);

        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "chloe_initial":
            {
                if (source.UserStatSheet.Level is >= 11 and < 71)
                {
                    var option = new DialogOption
                    {
                        DialogKey = "thesacrifice_initial",
                        OptionText = "The Sacrifice"
                    };
                    
                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Insert(0, option);
                }

                break;
            }

            case "thesacrifice_initial":
            {
                if (source.Trackers.TimedEvents.HasActiveEvent("thesacrificecd", out var cdtime))
                {
                    Subject.Reply(source, $"Thank you again for your help Aisling. I may need your help again soon. (({cdtime.Remaining.ToReadableString()}))", "chloe_initial");
                    return;
                }

                if (hasStage && stage == TheSacrificeQuestStage.Reconaissance)
                {
                    var numFlags = 0;

                    foreach (var value in Enum.GetValues<ReconPoints>())
                        if (source.Trackers.Flags.HasFlag(value))
                            numFlags++;

                    
                    if (numFlags >= 3)
                    {
                        var experience = (numFlags * fivePercent);
                        
                        Logger.WithTopics(
                                Topics.Entities.Aisling,
                                Topics.Entities.Experience,
                                Topics.Entities.Dialog,
                                Topics.Entities.Quest)
                            .WithProperty(source)
                            .WithProperty(Subject)
                            .LogInformation("{@AislingName} has received {@ExpAmount} exp from a quest", source.Name, experience);

                        
                        Subject.Reply(source, $"Wow! You scouted out {numFlags.ToWords()} spots! That's impressive you were able to avoid the goblins and kobolds. Thank you! This will help us find the children.", "chloe_initial");
                        source.TryGiveGamePoints(5);
                        ExperienceDistributionScript.GiveExp(source, experience);
                        source.Trackers.Flags.RemoveFlag(ReconPoints.Reconpoint1);
                        source.Trackers.Flags.RemoveFlag(ReconPoints.Reconpoint2);
                        source.Trackers.Flags.RemoveFlag(ReconPoints.Reconpoint3);
                        source.Trackers.Flags.RemoveFlag(ReconPoints.Reconpoint4);
                        source.Trackers.Flags.RemoveFlag(ReconPoints.Reconpoint5);
                        source.Trackers.Flags.RemoveFlag(ReconPoints.Reconpoint6);
                        source.Trackers.Flags.RemoveFlag(ReconPoints.Reconpoint7);
                        source.Trackers.Enums.Set(TheSacrificeQuestStage.None);
                        source.Trackers.TimedEvents.AddEvent("thesacrificecd", TimeSpan.FromHours(8), true);
                        return;
                    }
                    
                    Subject.Reply(source, "This isn't enough information. Go scout more spots and return to me with something I can really use! Remember to look at the points on the star.", "chloe_initial");
                    return;
                }

                if (hasStage && stage == TheSacrificeQuestStage.AttackCaptors)
                {
                    source.Trackers.Counters.TryGetValue("captorkills1", out int captorkills1);
                    source.Trackers.Counters.TryGetValue("captorkills2", out int captorkills2);

                    var killamount = captorkills1 + captorkills2;

                    if (killamount >= 10)
                    {
                        captorkills1 = Math.Min(captorkills1, 25);
                        captorkills2 = Math.Min(captorkills2, 25);

                        var experience = captorkills1 * 1000 + captorkills2 * 2000 + fivePercent;

                        Subject.Reply(source, $"You killed {killamount.ToWords()} captors! That'll hurt them for sure! They will think twice about taking our kids again! Thank you for your heroic actions.", "chloe_initial");

                        Logger.WithTopics(
                                Topics.Entities.Aisling,
                                Topics.Entities.Experience,
                                Topics.Entities.Dialog,
                                Topics.Entities.Quest)
                            .WithProperty(source)
                            .WithProperty(Subject)
                            .LogInformation("{@AislingName} has received {@ExpAmount} exp from a quest", source.Name, experience);

                        source.TryGiveGamePoints(5);
                        ExperienceDistributionScript.GiveExp(source, experience);

                        source.Trackers.Counters.Set("captorkills1", 0);
                        source.Trackers.Counters.Set("captorkills2", 0);
                        source.Trackers.Enums.Set(TheSacrificeQuestStage.None);
                        source.Trackers.TimedEvents.AddEvent("thesacrificecd", TimeSpan.FromHours(8), true);
                        return;
                    }

                    
                    Subject.Reply(source, "You haven't killed enough to be useful. I don't think that'll get us anywhere. Go slay some more, make them regret taking the children!", "chloe_initial");
                    return;
                }

                if (hasStage && stage == TheSacrificeQuestStage.RescueChildren)
                {
                    if (source.Trackers.Flags.HasFlag(SavedChild.savedchild))
                    {
                        Logger.WithTopics(
                                Topics.Entities.Aisling,
                                Topics.Entities.Experience,
                                Topics.Entities.Dialog,
                                Topics.Entities.Quest)
                            .WithProperty(source)
                            .WithProperty(Subject)
                            .LogInformation("{@AislingName} has received {@ExpAmount} exp from a quest", source.Name, fiftyPercent);

                        Subject.Reply(source, $"Wow! You saved a child! Thank you so much Aisling!", "chloe_initial");
                        source.TryGiveGamePoints(5);
                        ExperienceDistributionScript.GiveExp(source, fiftyPercent);
                        source.Trackers.Flags.RemoveFlag(SavedChild.savedchild);
                        source.Trackers.Enums.Set(TheSacrificeQuestStage.None);
                        source.Trackers.TimedEvents.AddEvent("thesacrificecd", TimeSpan.FromHours(8), true);
                        return;
                    }
                    
                    Subject.Reply(source, "Where are the children?! You returned without a child? I thought you were going to find them. Go! Search for the children in Astrid!", "chloe_initial");
                }
                break;
            }

            case "reconaissance_start":
            {
                if (!hasStage || stage == TheSacrificeQuestStage.None)
                {
                    source.Trackers.Enums.Set(TheSacrificeQuestStage.Reconaissance);
                }
                break;
            }
            case "attackcaptors_start":
            {
                if (!hasStage || stage == TheSacrificeQuestStage.None)
                {
                    source.Trackers.Enums.Set(TheSacrificeQuestStage.AttackCaptors);
                }
                
                break;
            }
            case "rescuechildren_start":
            {
                if (!hasStage || stage == TheSacrificeQuestStage.None)
                {
                    source.Trackers.Enums.Set(TheSacrificeQuestStage.RescueChildren);
                }
                break;
            }
        }
    }
}
using AutoMapper.Execution;
using Chaos.Collections;
using Chaos.Common.Definitions;
using Chaos.DarkAges.Definitions;
using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Extensions.Geometry;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.NLog.Logging.Definitions;
using Chaos.NLog.Logging.Extensions;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Scripting.DialogScripts.Quests.PFQuest;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ExperienceDistribution;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.DialogScripts.Quests.LynithPirateShip;

public class HelpDoltooScript : DialogScriptBase
{
    private readonly ILogger<PFQuestScript> Logger;
    private readonly ISimpleCache SimpleCache;
    private readonly IItemFactory ItemFactory;
    private IExperienceDistributionScript ExperienceDistributionScript { get; }

    /// <inheritdoc />
    public HelpDoltooScript(
        Dialog subject,
        ISimpleCache simpleCache,
        ILogger<PFQuestScript> logger, IItemFactory itemFactory)
        : base(subject)
    {
        SimpleCache = simpleCache;
        Logger = logger;
        ItemFactory = itemFactory;
        ExperienceDistributionScript = DefaultExperienceDistributionScript.Create();
    }

    /// <inheritdoc />
    public override void OnDisplaying(Aisling source)
    {
        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "doltoo_initial":
            {
                if (!source.UserStatSheet.Master)
                    return;
                
                if (source.Trackers.Flags.HasFlag(ShipAttackFlags.FinishedDoltoo))
                {
                    Subject.Reply(source, "Thanks again for showing me the way out! I just love it down here, free meals and a cot.");
                    return;
                }
                
                if (source.Trackers.Enums.HasValue(HelpSable.CompletedEscort))
                {
                    Subject.Reply(source, "Skip", "helpdoltoo_finished");
                    return;
                }
                
                if (source.Trackers.Enums.HasValue(HelpSable.StartedDoltoo) 
                    || source.Trackers.Enums.HasValue(HelpSable.EscortingDoltooStart))
                {
                    Subject.Reply(source, "Skip", "helpdoltoo_return");
                }

                if (!source.Trackers.Enums.HasValue(HelpSable.FinishedCaptain) 
                    && !source.Trackers.Enums.HasValue(HelpSable.StartedDoltoo) 
                    && !source.Trackers.Enums.HasValue(HelpSable.EscortingDoltooStart) 
                    && !source.Trackers.Enums.HasValue(HelpSable.EscortingDoltooFailed)
                    && !source.Trackers.Enums.HasValue(HelpSable.CompletedEscort))
                    return;
                
                var hasStage = source.Trackers.Enums.TryGetValue(out HelpSable stage);

                if (!hasStage)
                    return;

                if (source.Trackers.Enums.HasValue(HelpSable.FinishedCaptain)
                    || source.Trackers.Enums.HasValue(HelpSable.StartedDoltoo) 
                    || source.Trackers.Enums.HasValue(HelpSable.EscortingDoltooStart))
                {
                    var option1 = new DialogOption
                    {
                        DialogKey = "helpdoltoo_initial",
                        OptionText = "Sorry I left you."
                    };

                    if (!Subject.HasOption(option1.OptionText))
                        Subject.Options.Insert(0, option1);
                    
                    return;
                }

                var option = new DialogOption
                {
                    DialogKey = "helpdoltoo_initial",
                    OptionText = "How are you doing Doltoo?"
                };

                if (!Subject.HasOption(option.OptionText))
                    Subject.Options.Insert(0, option);
            }
                break;

            case "helpdoltoo_initial":
            {
                if (source.Trackers.Enums.HasValue(HelpSable.FinishedCaptain))
                {
                    Subject.Reply(source, "Skip", "helpdoltoo_initial2");
                    return;
                }
            }
                break;

            case "helpdoltoo_initial4":
            {
                var group = GetNearbyGroupMembers(source);

                if (group == null)
                {
                    TeleportPlayerToBrigQuest(source);
                    return;
                }
                
                if (!IsGroupWithinLevelRange(source, group))
                {
                    SendLevelRangeInvalidMessage(source);
                    return;
                }

                if (!IsGroupEligible(source))
                {
                    SendGroupEligibleMessage(source);
                    return;
                }

                TeleportGroupToBrigQuest(source, group);
                break;
            }

            case "helpdoltoo_return3":
            { 
                var group = GetNearbyGroupMembers(source);

                if (group == null)
                {
                    TeleportPlayerToBrigQuest(source);
                    return;
                }
                
                if (!IsGroupWithinLevelRange(source, group))
                {
                    SendLevelRangeInvalidMessage(source);
                    return;
                }

                if (!IsGroupEligible(source))
                {
                    SendGroupEligibleMessage(source);
                    return;
                }
                
                if (!IsGroupValid(source))
                {
                    Subject.Reply(source, "Not all of your group members are here.");
                    source.SendOrangeBarMessage("Not all of your group members are here.");
                    return;
                }

                TeleportGroupToBrigQuest(source, group);
                break;
            }

            case "helpdoltoo_finished2":
            {
                source.Trackers.Enums.Set(HelpSable.FinishedDoltoo);
                source.Trackers.Flags.AddFlag(ShipAttackFlags.FinishedDoltoo);
                source.TryGiveGamePoints(15);
                ExperienceDistributionScript.GiveExp(source, 30000000);
                var item = ItemFactory.Create("Shinguards");
                source.GiveItemOrSendToBank(item);
                source.SendOrangeBarMessage("Doltoo hands you some Shinguards.");
                
                Logger.WithTopics(
                        [Topics.Entities.Aisling,
                        Topics.Entities.Experience,
                        Topics.Entities.Item,
                        Topics.Entities.Dialog,
                        Topics.Entities.Quest])
                    .WithProperty(source)
                    .WithProperty(Subject)
                    .LogInformation(
                        "{@AislingName} has received {@ExpAmount} exp from Helping Doltoo.",
                        source.Name,
                        20000000);
                break;
            }
        }
    }
    
    private bool IsGroupEligible(Aisling source) =>
        source.Group != null && source.Group.All(x =>
            x.Trackers.Enums.HasValue(HelpSable.FinishedCaptain)
            || x.Trackers.Enums.HasValue(HelpSable.EscortingDoltooFailed)
            || x.Trackers.Enums.HasValue(HelpSable.StartedDoltoo)
            || x.Trackers.Enums.HasValue(HelpSable.EscortingDoltooStart)
            || x.Trackers.Flags.HasFlag(ShipAttackFlags.FinishedDoltoo));
    private bool IsGroupValid(Aisling source) =>
        source.Group != null && !source.Group.Any(x => !x.OnSameMapAs(source) || !x.WithinRange(source));

    private List<Aisling>? GetNearbyGroupMembers(Aisling source) =>
        source.Group?.Where(x => x.WithinRange(new Point(source.X, source.Y))).ToList();

    private bool IsGroupWithinLevelRange(Aisling source, List<Aisling> group) =>
        group.All(member => member.WithinLevelRange(source) && member.UserStatSheet.Master);
    private void SendLevelRangeInvalidMessage(Aisling source)
    {
        source.Client.SendServerMessage(ServerMessageType.OrangeBar1, "All of your group members must be within level range.");
        Subject.Reply(source, "Some of your companions are not within your level range.");
    }

    private void SendGroupEligibleMessage(Aisling source)
    {
        source.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Not all group members are on this quest.");
        Subject.Reply(source, "A member of your group isn't on this part of the quest.");
    }
    
    private void TeleportGroupToBrigQuest(Aisling source, List<Aisling> group)
    {
        var mapInstance = SimpleCache.Get<MapInstance>("lynith_pirate_brigquest");

        foreach (var member in group)
        {
            
            Point point;
            do
            {
                point = new Point(member.X, member.Y);
            }
            while (!mapInstance.IsWalkable(point, member.Type));
            
            var dialog = member.ActiveDialog.Get();
            dialog?.Close(member);
            Subject.Close(source);

            if (!member.Trackers.Flags.HasFlag(ShipAttackFlags.FinishedDoltoo))
            {
                member.Trackers.Enums.Set(HelpSable.StartedDoltoo); 
            }
            member.TraverseMap(mapInstance, point);
        }
    }

    private void TeleportPlayerToBrigQuest(Aisling source)
    {
        var mapInstance = SimpleCache.Get<MapInstance>("lynith_pirate_brigquest");

        var point = new Point(source.X, source.Y);
        
        source.Trackers.Enums.Set(HelpSable.StartedDoltoo);
        source.TraverseMap(mapInstance, point);
    }
}
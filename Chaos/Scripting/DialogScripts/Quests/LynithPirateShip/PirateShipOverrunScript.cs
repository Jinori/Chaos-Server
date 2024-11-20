using Chaos.Collections;
using Chaos.Common.Definitions;
using Chaos.DarkAges.Definitions;
using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Extensions.Geometry;
using Chaos.Models.Legend;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.NLog.Logging.Definitions;
using Chaos.NLog.Logging.Extensions;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Scripting.DialogScripts.Quests.PFQuest;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ExperienceDistribution;
using Chaos.Storage.Abstractions;
using Chaos.Time;
using Humanizer;

namespace Chaos.Scripting.DialogScripts.Quests.LynithPirateShip;

public class PirateShipOverrunScript : DialogScriptBase
{
    private readonly ILogger<PFQuestScript> Logger;
    private readonly ISimpleCache SimpleCache;
    private IExperienceDistributionScript ExperienceDistributionScript { get; }

    /// <inheritdoc />
    public PirateShipOverrunScript(
        Dialog subject,
        ISimpleCache simpleCache,
        ILogger<PFQuestScript> logger
    )
        : base(subject)
    {
        SimpleCache = simpleCache;
        Logger = logger;
        ExperienceDistributionScript = DefaultExperienceDistributionScript.Create();
    }

    /// <inheritdoc />
    public override void OnDisplaying(Aisling source)
    {
        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "captainwolfgang_initial":
            {
                if (source.StatSheet.Level < 71)
                    return;
                
                var option = new DialogOption
                {
                    DialogKey = "shipattack_initial",
                    OptionText = "What's wrong Captain?"
                };

                if (!Subject.HasOption(option.OptionText))
                    Subject.Options.Insert(0, option);
            }
                break;
                
                case "shipattack_initial":
            {
                var hasStage = source.Trackers.Enums.TryGetValue(out PirateShip _);
                
                if (source.Trackers.Enums.HasValue(PirateShip.CompletedShipAttack) || source.Trackers.Flags.HasFlag(ShipAttackFlags.CompletedShipAttack))
                {
                    Subject.Reply(source, "Thank you again Aisling for defending my ship, we probably would've sunk without you.");
                    return;
                }

                if (source.Trackers.TimedEvents.HasActiveEvent("lynithpirateshipcd", out var cdtime))
                {
                    Subject.Reply(source, $"We just built our ship back up after the last attack. It'll be awhile before they return. (({cdtime.Remaining.Humanize()}))");
                    return;
                }

                if (source.Trackers.Enums.HasValue(PirateShip.None) || !hasStage)
                {
                    Subject.Reply(source, "Skip", "shipattack_initial2");
                    return;
                }
                
                if (source.Trackers.Enums.HasValue(PirateShip.StartedShipAttack)
                    || source.Trackers.Enums.HasValue(PirateShip.FinishedWave1)
                    || source.Trackers.Enums.HasValue(PirateShip.FinishedWave2)
                    || source.Trackers.Enums.HasValue(PirateShip.FinishedWave3)
                    || source.Trackers.Enums.HasValue(PirateShip.FinishedWave4)
                    || source.Trackers.Enums.HasValue(PirateShip.FinishedWave5)
                    || source.Trackers.Enums.HasValue(PirateShip.FinishedWave6)
                    || source.Trackers.Enums.HasValue(PirateShip.FinishedWave7)
                    || source.Trackers.Enums.HasValue(PirateShip.FinishedWave8)
                    || source.Trackers.Enums.HasValue(PirateShip.FinishedWave9)
                    || source.Trackers.Enums.HasValue(PirateShip.FinishedWave10))
                {
                    Subject.Reply(source, "Skip", "shipattack_returnloss");
                    return;
                }

                if (source.Trackers.Enums.HasValue(PirateShip.FinishedShipAttack))
                {
                    Subject.Reply(source, "Skip", "shipattack_returnwin");
                }
            }
                break;

            case "shipattack_initial5":
            {
                var group = source.Group?.Where(x => x.WithinRange(new Point(source.X, source.Y))).ToList();
                var groupnearby = source.Group != null && !source.Group.Any(x => !x.OnSameMapAs(source) || !x.WithinRange(source));

                if ((group == null) || !groupnearby)
                {
                    source.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You must have a group to defend the ship.");
                    Subject.Reply(source, "You must have a group with you to help defend the ship.");

                    return;
                }

                if (group.All(member => member.Trackers.TimedEvents.HasActiveEvent("lynithpirateshipcd", out _)))
                {
                    Subject.Reply(source, "One of your members have attempted this quest too recently.");
                    source.Client.SendServerMessage(ServerMessageType.OrangeBar1, "One of your members have attempted this quest too recently.");
                    return;
                }

                if (!group.All(member => member.WithinLevelRange(source)))
                {
                    source.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Your group members are not in level range.");
                    Subject.Reply(source, "Some of your companions are not within your level range.");
                    return;
                }

                if (!group.All(member => member.UserStatSheet.Level > 70))
                {
                    source.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Someone in your group is under level 71.");
                    Subject.Reply(source, "I can't have you babysitting while you're defending my deck! Please make sure your group members are above level 71!");
                    return;
                }
                
                foreach (var member in group)
                {
                    var dialog = member.ActiveDialog.Get();
                    dialog?.Close(member);
                    member.Trackers.Enums.Set(PirateShip.StartedShipAttack);
                    member.SendOrangeBarMessage("Defend the Ship Deck against the Sea Monsters.");
                    var rect = new Rectangle(11, 11, 15, 6);
                    var mapinstance = SimpleCache.Get<MapInstance>("lynith_pirate_ship_deckquest");
                    Point point;
                    do
                    {
                        point = rect.GetRandomPoint();
                    }
                    while (!mapinstance.IsWalkable(point, member.Type));
                    member.TraverseMap(mapinstance, point);
                }
                break;
            }

            case "shipattack_returnloss":
                    source.Trackers.Enums.Set(PirateShip.None);

                break;

            case "shipattack_returnwin3":

                if (source.Trackers.Flags.HasFlag(ShipAttackFlags.CompletedShipAttack))
                {
                    source.SendOrangeBarMessage("Thank you for helping others.");
                    source.TryGiveGamePoints(25);
                    return;
                }
                source.Trackers.Enums.Set(PirateShip.CompletedShipAttack);
                source.Trackers.Flags.AddFlag(ShipAttackFlags.CompletedShipAttack);
                ExperienceDistributionScript.GiveExp(source, 750000);
                source.Trackers.Flags.AddFlag(AvailableCloaks.Blue);
                source.SendOrangeBarMessage("You unlocked the Blue Cloak for mounts!");
                
                source.Legend.AddOrAccumulate(
                    new LegendMark(
                        "Protected Lynith Pirate Ship from Sea Monsters",
                        "pirateshipoverrun",
                        MarkIcon.Victory,
                        MarkColor.Blue,
                        1,
                        GameTime.Now));

                    
                Logger.WithTopics(
                        [Topics.Entities.Aisling,
                        Topics.Entities.Experience,
                        Topics.Entities.Item,
                        Topics.Entities.Dialog,
                        Topics.Entities.Quest])
                    .WithProperty(source)
                    .WithProperty(Subject)
                    .LogInformation(
                        "{@AislingName} has received {@ExpAmount} exp and Blue Cloak from Lynith Ship Attack",
                        source.Name,
                        750000);
                break;
        }
    }
}
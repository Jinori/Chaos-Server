using Chaos.Collections;
using Chaos.Common.Definitions;
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

public class PirateShipOverrunScript : DialogScriptBase
{
    private readonly IItemFactory ItemFactory;
    private readonly ILogger<PFQuestScript> Logger;
    private readonly ISimpleCache SimpleCache;
    private IExperienceDistributionScript ExperienceDistributionScript { get; }

    /// <inheritdoc />
    public PirateShipOverrunScript(
        Dialog subject,
        IItemFactory itemFactory,
        ISimpleCache simpleCache,
        ILogger<PFQuestScript> logger
    )
        : base(subject)
    {
        ItemFactory = itemFactory;
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
                var hasStage = source.Trackers.Enums.TryGetValue(out ShipAttack stage);
                
                if (source.Trackers.Enums.HasValue(ShipAttack.CompletedShipAttack) || source.Trackers.Flags.HasFlag(ShipAttackFlags.CompletedShipAttack))
                {
                    Subject.Reply(source, "Thank you again Aisling for defending my ship, we probably would've sunk without you.");
                    return;
                }

                if (source.Trackers.Enums.HasValue(ShipAttack.None) || !hasStage)
                {
                    Subject.Reply(source, "Skip", "shipattack_initial2");
                    return;
                }
                
                if (source.Trackers.Enums.HasValue(ShipAttack.StartedShipAttack)
                    || source.Trackers.Enums.HasValue(ShipAttack.FinishedWave1)
                    || source.Trackers.Enums.HasValue(ShipAttack.FinishedWave2)
                    || source.Trackers.Enums.HasValue(ShipAttack.FinishedWave3)
                    || source.Trackers.Enums.HasValue(ShipAttack.FinishedWave4)
                    || source.Trackers.Enums.HasValue(ShipAttack.FinishedWave5)
                    || source.Trackers.Enums.HasValue(ShipAttack.FinishedWave6)
                    || source.Trackers.Enums.HasValue(ShipAttack.FinishedWave7)
                    || source.Trackers.Enums.HasValue(ShipAttack.FinishedWave8)
                    || source.Trackers.Enums.HasValue(ShipAttack.FinishedWave9)
                    || source.Trackers.Enums.HasValue(ShipAttack.FinishedWave10))
                {
                    Subject.Reply(source, "Skip", "shipattack_returnloss");
                    return;
                }

                if (source.Trackers.Enums.HasValue(ShipAttack.FinishedShipAttack))
                {
                    Subject.Reply(source, "Skip", "shipattack_returnwin");
                    return;
                }
            }
                break;

            case "shipattack_initial5":
            {
                var group = source.Group?.Where(x => x.WithinRange(new Point(source.X, source.Y))).ToList();

                if ((group == null) || !group.Any())
                {
                    source.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You must have a group to defend the ship.");
                    Subject.Reply(source, "You must have a group with you to help defend the ship.");

                    return;
                }

                if (!group.All(member => member.WithinLevelRange(source)))
                {
                    source.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Your group members are not in level range.");
                    Subject.Reply(source, "Some of your companions are not within your level range.");
                    return;
                }

                if (!group.All(member => member.UserStatSheet.Level < 71))
                {
                    source.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Someone in your group is under level 71.");
                    Subject.Reply(source, "I can't have you babysitting while you're defending my deck! Please make sure your group members are above level 71!");
                    return;
                }
                
                foreach (var member in group)
                {
                    var dialog = member.ActiveDialog.Get();
                    dialog?.Close(member);
                    member.Trackers.Enums.Set(ShipAttack.StartedShipAttack);
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

            case "shipattack_returnloss3":
                    source.Trackers.Enums.Set(ShipAttack.None);

                break;

            case "shipattack_returnwin3":

                if (source.Trackers.Flags.HasFlag(ShipAttackFlags.CompletedShipAttack))
                {
                    source.SendOrangeBarMessage("Thank you for helping others.");
                    source.TryGiveGamePoints(25);
                    return;
                }
                source.Trackers.Enums.Set(ShipAttack.CompletedShipAttack);
                source.Trackers.Flags.AddFlag(ShipAttackFlags.CompletedShipAttack);
                ExperienceDistributionScript.GiveExp(source, 750000);
                source.Trackers.Flags.AddFlag(AvailableCloaks.Blue);
                source.SendOrangeBarMessage("You unlocked the Blue Cloak for mounts!");
                    
                Logger.WithTopics(
                        Topics.Entities.Aisling,
                        Topics.Entities.Experience,
                        Topics.Entities.Item,
                        Topics.Entities.Dialog,
                        Topics.Entities.Quest)
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
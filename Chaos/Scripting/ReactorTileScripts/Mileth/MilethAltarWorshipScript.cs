using Chaos.Common.Utilities;
using Chaos.DarkAges.Definitions;
using Chaos.Definitions;
using Chaos.Extensions.Common;
using Chaos.Extensions.Geometry;
using Chaos.Models.Legend;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Networking.Abstractions;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ExperienceDistribution;
using Chaos.Scripting.ReactorTileScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Time;

namespace Chaos.Scripting.ReactorTileScripts.Mileth;

public class MilethAltarWorshipScript(ReactorTile subject, IItemFactory itemFactory, IReactorTileFactory reactorTileFactory,
    IClientRegistry<IChaosWorldClient> clientRegistry)
    : ReactorTileScriptBase(subject)
{
    private readonly IReactorTileFactory ReactorTileFactory = reactorTileFactory;
    private IExperienceDistributionScript ExperienceDistributionScript { get; } = DefaultExperienceDistributionScript.Create();

    private readonly IClientRegistry<IChaosWorldClient> ClientRegistry = clientRegistry;
    
    public override void OnItemDroppedOn(Creature source, GroundItem groundItem)
    {
        if (source is not Aisling aisling)
            return;
        
        if (groundItem.Name == "Wirt's Leg")
        {
            if (!source.Trackers.Enums.HasValue(ClassStatBracket.Grandmaster))
            {
                var item = itemFactory.Create(groundItem.Item.Template.TemplateKey);
                aisling.GiveItemOrSendToBank(item);
                aisling.MapInstance.RemoveEntity(groundItem);
                aisling.SendOrangeBarMessage("You must be a Grand Master to do what you were about to do.");

                return;
            }
            
            if (source.MapInstance
                      .GetEntities<ReactorTile>()
                      .Any(x => x.ScriptKeys.ContainsI("cowPortal")))
            {
                var item = itemFactory.Create(groundItem.Item.Template.TemplateKey);
                aisling.GiveItemOrSendToBank(item);
                aisling.MapInstance.RemoveEntity(groundItem);
                aisling.SendOrangeBarMessage("A rift in the realm here has already been made.");
                
                return;
            }
            
            var rectangle = new Rectangle(
                source.X,
                source.Y,
                2,
                2);
            
            rectangle.TryGetRandomPoint(x => aisling.MapInstance.IsWalkable(x, collisionType: source.Type), out var portalpoint);

            if (portalpoint == null)
            {
                aisling.SendOrangeBarMessage("There is no space nearby, please move and try again.");
                var item = itemFactory.Create(groundItem.Item.Template.TemplateKey);
                aisling.GiveItemOrSendToBank(item);
                aisling.MapInstance.RemoveEntity(groundItem);

                return;
            }

            var portal = ReactorTileFactory.Create("cowportal", aisling.MapInstance, portalpoint, null, source);
            aisling.MapInstance.SimpleAdd(portal);
            aisling.MapInstance.RemoveEntity(groundItem);
            
            foreach (var allaisling in ClientRegistry)
                allaisling.SendServerMessage(ServerMessageType.OrangeBar2, "The ground shakes, a rift in space opens in Mileth.");

            return;
        }

        aisling.MapInstance.RemoveEntity(groundItem);

        if (groundItem.Item.Count > 1)
            ExperienceDistributionScript.GiveExp(aisling, groundItem.Item.Template is { BuyCost: < 1000, SellValue: < 1000 } ? 25 * groundItem.Item.Count : 200 * groundItem.Item.Count);
        else
            ExperienceDistributionScript.GiveExp(aisling, groundItem.Item.Template is { BuyCost: < 1000, SellValue: < 1000 } ? 25 : 200);

        if (IntegerRandomizer.RollChance(10))
        {
            var randomMessages = new List<string>
            {
                "The gods are pleased with your sacrifice.",
                "The item glows before dissolving into the altar.",
                "Good fortune the gods will grant."
            };

            var random = new Random();
            var index = random.Next(randomMessages.Count);
            aisling.Client.SendServerMessage(ServerMessageType.OrangeBar1, randomMessages[index]);
        }

        if (!IntegerRandomizer.RollChance(1))
            return;

        if (aisling.UserStatSheet.BaseClass.Equals(BaseClass.Priest))
        {
            aisling.Legend.AddOrAccumulate(
                new LegendMark(
                    "Mileth Altar Worshipper",
                    "milethWorship",
                    MarkIcon.Yay,
                    MarkColor.Pink,
                    1,
                    GameTime.Now));

            aisling.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You received a unique legend mark!");
            ExperienceDistributionScript.GiveExp(aisling, 20000);
            aisling.GiveItemOrSendToBank(itemFactory.Create("amethystring"));
        } else
        {
            ExperienceDistributionScript.GiveExp(aisling, 5000);
            aisling.GiveItemOrSendToBank(itemFactory.Create("emeraldring"));
        }
    }
}
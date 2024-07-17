using Chaos.Definitions;
using Chaos.Extensions.Common;
using Chaos.Models.World;
using Chaos.Scripting.MonsterScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.MonsterScripts;

public class GhostDeathScript : MonsterScriptBase
{
    private readonly IItemFactory ItemFactory;

    /// <inheritdoc />
    public GhostDeathScript(Monster subject, IItemFactory itemFactory)
        : base(subject) =>
        ItemFactory = itemFactory;

    /// <inheritdoc />
    public override void OnDeath()
    {
        var ghosts = Subject.MapInstance.GetEntities<Monster>().Where(x => x.Template.TemplateKey.EndsWithI("phasedghost"));

        if (ghosts.Any())
            return;

        var aislingsToReward = Subject.MapInstance.GetEntities<Aisling>()
                                      .Where(
                                          x => x.Trackers.Enums.TryGetValue(out ManorNecklaceStage _));

        foreach (var aisling in aislingsToReward)
        {
            if (aisling.Trackers.Enums.HasValue(ManorNecklaceStage.SawNecklace))
            {
                var item = ItemFactory.Create("zulerasCursedNecklace");
                aisling.Inventory.RemoveByTemplateKey("clue1");
                aisling.Inventory.RemoveByTemplateKey("clue2");
                aisling.Inventory.RemoveByTemplateKey("clue3");
                aisling.Inventory.RemoveByTemplateKey("clue4");

                if (aisling.Trackers.Enums.TryGetValue(out ManorNecklaceStage stage) && (stage != ManorNecklaceStage.SawNecklace))
                    return;
            
                aisling.GiveItemOrSendToBank(item);
                aisling.Trackers.Enums.Set(ManorNecklaceStage.ReturningNecklace);
                aisling.SendActiveMessage("You received Zulera's Cursed Necklace! Take it back to her.");
                
            }

            if (aisling.Trackers.Enums.HasValue(ManorNecklaceStage.ReturnedNecklace)
                || aisling.Trackers.Enums.HasValue(ManorNecklaceStage.KeptNecklace))
            {
                aisling.TryGiveGamePoints(15);
                aisling.GiveGoldOrSendToBank(25000);
                aisling.SendActiveMessage("Thank you for helping others.");
            }
        }
    }
}
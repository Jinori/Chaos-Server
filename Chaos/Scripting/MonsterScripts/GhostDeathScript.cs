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
                                          x => x.Trackers.Enums.TryGetValue(out ManorNecklaceStage value)
                                               && Equals(value, ManorNecklaceStage.SawNecklace));

        foreach (var aisling in aislingsToReward)
        {
            var item = ItemFactory.Create("zulerasCursedNecklace");

            aisling.Inventory.RemoveByTemplateKey("clue1");
            aisling.Inventory.RemoveByTemplateKey("clue2");
            aisling.Inventory.RemoveByTemplateKey("clue3");
            aisling.Inventory.RemoveByTemplateKey("clue4");

            if (aisling.Trackers.Enums.TryGetValue(out ManorNecklaceStage stage) && (stage != ManorNecklaceStage.SawNecklace))
                return;

            if (!aisling.TryGiveItem(ref item))
            {
                aisling.SendActiveMessage("You don't have any inventory space.");
                aisling.SendActiveMessage("You received Zulera's Cursed Necklace! It was sent to your bank.");
                aisling.Bank.Deposit(item);
            }

            aisling.Trackers.Enums.Set(ManorNecklaceStage.ReturningNecklace);

            aisling.SendActiveMessage("You received Zulera's Cursed Necklace! Take it back to her.");
        }
    }
}
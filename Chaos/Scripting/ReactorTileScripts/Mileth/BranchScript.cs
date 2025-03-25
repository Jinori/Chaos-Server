using Chaos.Common.Definitions;
using Chaos.Common.Utilities;
using Chaos.DarkAges.Definitions;
using Chaos.Definitions;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.ReactorTileScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.ReactorTileScripts.Mileth;

public class BranchScript : ReactorTileScriptBase
{
    private readonly IItemFactory ItemFactory;

    public BranchScript(ReactorTile subject, IItemFactory itemFactory)
        : base(subject) =>
        ItemFactory = itemFactory;

    public override void OnWalkedOn(Creature source)
    {
        if (source is not Aisling aisling)
            return;

        if (aisling.Trackers.Enums.TryGetValue(out RionaTutorialQuestStage stage) && stage == RionaTutorialQuestStage.StartedSpareAStick && IntegerRandomizer.RollChance(18))
        {
            var branch = ItemFactory.Create("branch");
            aisling.GiveItemOrSendToBank(branch);
            var branchcount = aisling.Inventory.CountOf("branch");

            aisling.Client.SendServerMessage(
                ServerMessageType.OrangeBar1,
                $"Oh, you've found a sturdy branch! That makes {branchcount} branches!");
            aisling.Animate(new Animation
            {
                AnimationSpeed = 100,
                TargetAnimation = 10
            });
        }
    }
}
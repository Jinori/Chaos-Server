using Chaos.Common.Definitions;
using Chaos.Common.Utilities;
using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripts.ReactorTileScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripts.ReactorTileScripts
{
    public class BranchScript : ReactorTileScriptBase
    {
        private readonly IItemFactory ItemFactory;
        public BranchScript(ReactorTile subject, IItemFactory itemFactory)
            : base(subject) =>
            ItemFactory = itemFactory;

        public override void OnWalkedOn(Creature source)
        {
            if (source is not Aisling)
                return;
            var aisling = source as Aisling;
            if (aisling!.Flags.HasFlag(QuestFlag1.GatheringSticks))
            {
                if (Randomizer.RollChance(18))
                {
                    var branch = ItemFactory.Create("branch");
                    aisling.TryGiveItem(branch);
                    var branchcount = aisling.Inventory.CountOf("branch");
                    aisling.Client.SendServerMessage(ServerMessageType.OrangeBar1, $"Oh, you've found a sturdy branch! That makes {branchcount} branches!");
                }
            }
        }
    }
}

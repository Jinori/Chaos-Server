using Chaos.Common.Definitions;
using Chaos.Containers;
using Chaos.Extensions;
using Chaos.Extensions.Geometry;
using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripts.ReactorTileScripts.Abstractions;
using Chaos.Services.Factories;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripts.ReactorTileScripts
{
    public class TerrorOfCryptEntranceScript : ReactorTileScriptBase
    {
        private readonly IMerchantFactory MerchantFactory;
        private readonly IDialogFactory DialogFactory;

        public TerrorOfCryptEntranceScript(ReactorTile subject, IMerchantFactory merchantFactory, IDialogFactory dialogFactory)
         : base(subject)
        {
            MerchantFactory = merchantFactory;
            DialogFactory = dialogFactory;
        }

        public override void OnWalkedOn(Creature source)
        {
            var aisling = source as Aisling;

            var currentPoint = new Point(aisling!.X, aisling!.Y);
            var group = aisling.Group?.Where(x => x.WithinRange(currentPoint)).ToList();

            if (group is null || group.Count <= 1)
            {
                aisling?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Make sure you are grouped or your group is near you.");
                var point = source.DirectionalOffset(source.Direction.Reverse());
                source.WarpTo(point);
                return;
            }

            int groupCount = 0;
            foreach (var member in group)
            {
                if (member.WithinLevelRange(source))
                    ++groupCount;
            }
            if (groupCount.Equals(group.Count()))
            {
                var npcpoint = new Point(aisling.X, aisling.Y);
                var merchant = MerchantFactory.Create("teague", aisling.MapInstance, npcpoint);
                var dialog = DialogFactory.Create("teague_enterTerror", merchant);
                dialog.Display(aisling);
            }
            else
            {
                aisling?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Make sure your companions are within level range.");
                var point = source.DirectionalOffset(source.Direction.Reverse());
                source.WarpTo(point);
                return;
            }
        }
    }
}

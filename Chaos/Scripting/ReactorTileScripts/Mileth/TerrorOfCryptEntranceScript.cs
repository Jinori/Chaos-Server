using Chaos.Common.Definitions;
using Chaos.Extensions;
using Chaos.Extensions.Geometry;
using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripting.ReactorTileScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.ReactorTileScripts.Mileth;

public class TerrorOfCryptEntranceScript : ReactorTileScriptBase
{
    private readonly IDialogFactory DialogFactory;
    private readonly IMerchantFactory MerchantFactory;

    public TerrorOfCryptEntranceScript(ReactorTile subject, IMerchantFactory merchantFactory, IDialogFactory dialogFactory)
        : base(subject)
    {
        MerchantFactory = merchantFactory;
        DialogFactory = dialogFactory;
    }

    public override void OnWalkedOn(Creature source)
    {
        if (source is not Aisling aisling)
            return;

        var currentPoint = new Point(aisling.X, aisling.Y);
        var group = aisling.Group?.Where(x => x.WithinRange(currentPoint)).ToList();

        if (group is null || (group.Count <= 1))
        {
            aisling.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Make sure you are grouped or your group is near you.");
            var point = source.DirectionalOffset(source.Direction.Reverse());
            source.WarpTo(point);

            return;
        }

        var groupCount = 0;

        foreach (var member in group)
        {
            if (member.WithinLevelRange(source) && member.Trackers.Flags.HasFlag(QuestFlag1.TerrorOfCryptHunt))
                ++groupCount;
        }

        if (groupCount.Equals(group.Count))
        {
            var npcpoint = new Point(aisling.X, aisling.Y);
            var merchant = MerchantFactory.Create("teague", aisling.MapInstance, npcpoint);
            var dialog = DialogFactory.Create("teague_enterTerror", merchant);
            dialog.Display(aisling);
        } 
        else
        {
            aisling.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Make sure everyone is within level range and has quest.");
            var point = source.DirectionalOffset(source.Direction.Reverse());
            source.WarpTo(point);
        }
    }
}
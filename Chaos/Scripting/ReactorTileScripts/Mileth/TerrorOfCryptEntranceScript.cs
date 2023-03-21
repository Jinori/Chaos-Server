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
    private readonly IDialogFactory _dialogFactory;
    private readonly IMerchantFactory _merchantFactory;

    public TerrorOfCryptEntranceScript(ReactorTile subject, IMerchantFactory merchantFactory, IDialogFactory dialogFactory)
        : base(subject)
    {
        _merchantFactory = merchantFactory;
        _dialogFactory = dialogFactory;
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

        // Check if all members of the group have the quest flag and are within level range
        var allMembersHaveQuestFlag = group.All(member => member.Trackers.Flags.HasFlag(QuestFlag1.TerrorOfCryptHunt) && member.WithinLevelRange(source));
        
        // Check if all conditions are met
        if (allMembersHaveQuestFlag)
        {
            var npcpoint = new Point(aisling.X, aisling.Y);
            var merchant = _merchantFactory.Create("teague", aisling.MapInstance, npcpoint);
            var dialog = _dialogFactory.Create("teague_enterTerror", merchant);
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

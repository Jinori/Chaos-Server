using Chaos.Collections;
using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Extensions.Geometry;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.ReactorTileScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.ReactorTileScripts.UndineFields;

public class UndineFieldsEntranceScript : ReactorTileScriptBase
{
    private readonly IDialogFactory DialogFactory;
    private readonly ISimpleCache SimpleCache;
    private readonly IItemFactory ItemFactory;
    private readonly IMerchantFactory MerchantFactory;

    #region ScriptVars
    protected Location Destination { get; init; } = null!;
    #endregion

    /// <inheritdoc />
    public UndineFieldsEntranceScript(ReactorTile subject, IItemFactory itemFactory, IDialogFactory dialogFactory, ISimpleCache simpleCache, IMerchantFactory merchantFactory)
        : base(subject)
    {
        DialogFactory = dialogFactory;
        ItemFactory = itemFactory;
        SimpleCache = simpleCache;
        MerchantFactory = merchantFactory;
    }

    /// <inheritdoc />
    public override void OnWalkedOn(Creature source)
    {
        var currentMap = SimpleCache.Get<MapInstance>(source.MapInstance.InstanceId);
        
        if (source is not Aisling aisling)
            return;
        
        if (source.Trackers.Enums.TryGetValue(out UndineFieldDungeon stage) &&
            stage != UndineFieldDungeon.StartedDungeon && source.Trackers.Flags.TryGetFlag(out UndineFieldDungeonFlag flag) && flag != UndineFieldDungeonFlag.CompletedUF)
        {
            aisling.SendOrangeBarMessage("You must start the quest to enter the fields.");
            var point = source.DirectionalOffset(source.Direction.Reverse());
            source.WarpTo(point);
            return;
        }
        
        if (aisling.Group is null || aisling.Group.Any(x => !x.OnSameMapAs(aisling) || !x.WithinRange(aisling)))
        {
            // Send a message to the Aisling
            aisling.SendOrangeBarMessage("You must have a group nearby to enter the fields.");
            // Warp the source back
            var point = source.DirectionalOffset(source.Direction.Reverse());
            source.WarpTo(point);

            return;
        }
        
        var allMembersHaveQuestEnum = aisling.Group.All(member =>
            member.Trackers.Enums.TryGetValue(out UndineFieldDungeon stage) &&
            stage == UndineFieldDungeon.StartedDungeon || member.Trackers.Flags.TryGetFlag(out UndineFieldDungeonFlag flag) && flag == UndineFieldDungeonFlag.CompletedUF);
        
        if (allMembersHaveQuestEnum)
        {
            var npcpoint = new Point(aisling.X, aisling.Y);
            var merchant = MerchantFactory.Create("uf_entrance_merchant", aisling.MapInstance, npcpoint);
            var dialog = DialogFactory.Create("uf_entrancesouth", merchant);
            dialog.Display(aisling);
        }
        else
        {
            // Send a message to the Aisling
            aisling.SendOrangeBarMessage("Not all group members have started the quest.");
            // Warp the source back
            var point = source.DirectionalOffset(source.Direction.Reverse());
            source.WarpTo(point);
        }
    }
}
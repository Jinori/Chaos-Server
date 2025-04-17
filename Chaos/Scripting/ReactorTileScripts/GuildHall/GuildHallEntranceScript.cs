using Chaos.Collections;
using Chaos.Extensions.Geometry;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.ReactorTileScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.ReactorTileScripts.GuildHall;

public class GuildHallEntrance : ConfigurableReactorTileScriptBase
{
    private readonly IDialogFactory DialogFactory;

    private readonly IStorage<GuildHouseState> GuildHouseStateStorage;
    private readonly IMerchantFactory MerchantFactory;
    private readonly ISimpleCache SimpleCache;

    public GuildHallEntrance(
        ReactorTile subject,
        ISimpleCache simpleCache,
        IStorage<GuildHouseState> guildHouseStateStorage,
        IDialogFactory dialogFactory,
        IMerchantFactory merchantFactory)
        : base(subject)
    {
        MerchantFactory = merchantFactory;
        GuildHouseStateStorage = guildHouseStateStorage;
        SimpleCache = simpleCache;
        DialogFactory = dialogFactory;
    }

    /// <inheritdoc />
    public override void OnWalkedOn(Creature source)
    {
        if (source is not Aisling aisling)
            return;

        if (aisling.Guild is null)
        {
            aisling.SendOrangeBarMessage("You don't belong to a guild.");
            var point = source.DirectionalOffset(aisling.Direction.Reverse());
            aisling.WarpTo(point);

            return;
        }

        var guildHouseState = GuildHouseStateStorage.Value;
        guildHouseState.SetStorage(GuildHouseStateStorage);

        if (!guildHouseState.HasProperty(aisling.Guild.Name, "deed"))
        {
            var point = source.DirectionalOffset(aisling.Direction.Reverse());
            aisling.WarpTo(point);
            var merchant = MerchantFactory.Create("tibbs", aisling.MapInstance, aisling);
            var dialog = DialogFactory.Create("tibbs_buyhouse", merchant);
            dialog.Display(aisling);

            return;
        }
        
        var targetMap = SimpleCache.Get<MapInstance>(Destination.Map);
        aisling.TraverseMap(targetMap, new Point(98, 46));
        aisling.SendOrangeBarMessage($"You've entered {aisling.Guild?.Name}'s Guild Hall.");
    }

    #region ScriptVars
    protected Location Destination { get; init; } = null!;
    public int? MaxLevel { get; set; }
    public int? MaxVitality { get; set; }
    public int? MinLevel { get; set; }
    public int? MinVitality { get; set; }
    #endregion
}
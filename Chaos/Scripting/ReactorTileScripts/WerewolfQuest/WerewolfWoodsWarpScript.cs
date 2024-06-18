using Chaos.Collections;
using Chaos.Definitions;
using Chaos.Extensions.Geometry;
using Chaos.Geometry.Abstractions;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.ReactorTileScripts.Abstractions;
using Chaos.Services.Factories;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.ReactorTileScripts.WerewolfQuest;

public class WerewolfWoodsWarpScript : ConfigurableReactorTileScriptBase
{
    private readonly ISimpleCache SimpleCache;

    #region ScriptVars
    protected Location Destination { get; init; } = null!;
    #endregion

    private readonly IDialogFactory DialogFactory;
    private readonly IMerchantFactory MerchantFactory;
    /// <inheritdoc />
    public WerewolfWoodsWarpScript(ReactorTile subject, ISimpleCache simpleCache, IDialogFactory dialogFactory, IMerchantFactory merchantFactory)
        : base(subject)
    {
        SimpleCache = simpleCache;
        DialogFactory = dialogFactory;
        MerchantFactory = merchantFactory;
    }

    /// <inheritdoc />
    public override void OnWalkedOn(Creature source)
    {
        if (source is not Aisling aisling)
            return;

        if (!source.Trackers.Enums.HasValue(WerewolfOfPiet.SpokeToWizard)
            && !source.Trackers.Enums.HasValue(WerewolfOfPiet.KilledWerewolf)
            && !source.Trackers.Enums.HasValue(WerewolfOfPiet.CollectedBlueFlower)
            && !source.Trackers.Enums.HasValue(WerewolfOfPiet.ReceivedCure))
        {
            aisling.SendOrangeBarMessage("The sight ahead scares you, you decide to turn around.");
            return;
        }

        var targetMap = SimpleCache.Get<MapInstance>(Destination.Map);
        
        if (source.Trackers.Enums.HasValue(WerewolfOfPiet.SpokeToWizard))
        {
            var point = new Point(aisling.X, aisling.Y);
            var merchant = MerchantFactory.Create("blank_merchant", Subject.MapInstance, point);
            var dialog = DialogFactory.Create("werewolfwarp1", merchant);
            dialog.Display(aisling);
            return;
        }
        
        if (aisling.StatSheet.Level < (targetMap.MinimumLevel ?? 0))
        {
            aisling?.SendOrangeBarMessage($"You must be at least level {targetMap.MinimumLevel} to enter this area.");
            var point = source.DirectionalOffset(aisling.Direction.Reverse());
            aisling.WarpTo(source.Trackers.LastPosition as IPoint ?? point);

            return;
        }

        if (aisling.StatSheet.Level > (targetMap.MaximumLevel ?? int.MaxValue))
        {
            aisling?.SendOrangeBarMessage($"You must be at most level {targetMap.MaximumLevel} to enter this area.");

            var point = source.DirectionalOffset(aisling.Direction.Reverse());
            aisling.WarpTo(source.Trackers.LastPosition as IPoint ?? point);

            return;
        }

        aisling.TraverseMap(targetMap, Destination);
    }
}
using Chaos.Collections;
using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.ReactorTileScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.ReactorTileScripts.MainStoryQuest;

public class StartCreantReactorScript : ReactorTileScriptBase
{
    private readonly IDialogFactory DialogFactory;
    private readonly IMerchantFactory MerchantFactory;
    private readonly ISimpleCache SimpleCache;

    /// <inheritdoc />
    public StartCreantReactorScript(
        ReactorTile subject,
        IDialogFactory dialogFactory,
        IMerchantFactory merchantFactory,
        ISimpleCache simpleCache)
        : base(subject)
    {
        DialogFactory = dialogFactory;
        MerchantFactory = merchantFactory;
        SimpleCache = simpleCache;
    }

    /// <inheritdoc />
    public override void OnWalkedOn(Creature source)
    {
        if (source is not Aisling aisling)
            return;

        if (aisling.Trackers.Flags.HasFlag(CreantEnums.KilledMedusa) && (aisling.MapInstance.Template.TemplateKey == "6599"))
        {
            var mapInstance = SimpleCache.Get<MapInstance>("medusabossroom");
            var placePoint = new Point(11, 10);
            aisling.TraverseMap(mapInstance, placePoint);
            aisling.SendOrangeBarMessage("The altar knocks you back.");
        }

        if (aisling.Trackers.Flags.HasFlag(CreantEnums.KilledSham) && (aisling.MapInstance.Template.TemplateKey == "31010"))
        {
            var mapInstance = SimpleCache.Get<MapInstance>("shamensythbossroom");
            var placePoint = new Point(11, 14);
            aisling.TraverseMap(mapInstance, placePoint);
            aisling.SendOrangeBarMessage("The altar knocks you back.");
        }

        if (aisling.Trackers.Flags.HasFlag(CreantEnums.KilledTauren) && (aisling.MapInstance.Template.TemplateKey == "19522"))
        {
            var mapInstance = SimpleCache.Get<MapInstance>("taurenbossroom");
            var placePoint = new Point(3, 11);
            aisling.TraverseMap(mapInstance, placePoint);
            aisling.SendOrangeBarMessage("The altar knocks you back.");
        }

        if (aisling.Trackers.Flags.HasFlag(CreantEnums.KilledPhoenix) && (aisling.MapInstance.Template.TemplateKey == "989"))
        {
            var mapInstance = SimpleCache.Get<MapInstance>("phoenixbossroom");
            var placePoint = new Point(36, 39);
            aisling.TraverseMap(mapInstance, placePoint);
            aisling.SendOrangeBarMessage("The altar knocks you back.");
        }

        if (aisling.Trackers.Enums.HasValue(MainstoryMasterEnums.CompletedCreants))
        {
            aisling.SendOrangeBarMessage("The Creant is sealed away, the altar does nothing.");

            return;
        }

        if (aisling.Trackers.Enums.HasValue(MainstoryMasterEnums.KilledCreants))
        {
            aisling.SendOrangeBarMessage("All of the Creants have been sealed. Speak to Goddess Miraelis.");

            return;
        }

        if (!aisling.Trackers.Enums.HasValue(MainstoryMasterEnums.StartedCreants))
        {
            aisling.SendOrangeBarMessage("You must speak to Goddess Miraelis before attempting this.");

            return;
        }

        var point = new Point(source.X, source.Y);
        var blankmerchant = MerchantFactory.Create("blank_merchant", Subject.MapInstance, point);
        var dialog = DialogFactory.Create("creant_entrance", blankmerchant);
        dialog.Display(aisling);
    }
}
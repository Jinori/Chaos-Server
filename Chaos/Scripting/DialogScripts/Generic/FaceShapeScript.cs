using Chaos.Common.Definitions;
using Chaos.DarkAges.Definitions;
using Chaos.Extensions;
using Chaos.Extensions.Common;
using Chaos.Models.Data;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.DialogScripts.Generic;

public class FaceShapeScript : DialogScriptBase
{
    private static readonly string[] FaceStyles =
    {
        "defaultfaceshape", "blushfaceshape", "derpyfaceshape", "bugfaceshape", "suprisedfaceshape", "squintfaceshape", "fishfaceshape", "oldmanfaceshape",
        "meanfaceshape", "beautyfaceshape", "soullessfaceshape", "possessedfaceshape", "deepeyesfaceshape", "closedeyesfaceshape", "sleepyeyesfaceshape", "fakesmilefaceshape",  "steppedonalegofaceshape","restingbitchfaceshape",
    };

    private readonly IItemFactory _itemFactory;

    public FaceShapeScript(Dialog subject, IItemFactory itemFactory)
        : base(subject) => _itemFactory = itemFactory;

    public override void OnDisplaying(Aisling source)
    {
        foreach (var s in FaceStyles)
        {
            var item = _itemFactory.CreateFaux(s);
            Subject.Items.Add(ItemDetails.BuyWithGold(item));
        }
    }

    public override void OnNext(Aisling source, byte? optionIndex = null)
    {
        if (!Subject.MenuArgs.TryGet<string>(0, out var hairStyleName))
        {
            Subject.ReplyToUnknownInput(source);

            return;
        }

        var itemDetails = Subject.Items.FirstOrDefault(x => x.Item.DisplayName.EqualsI(hairStyleName));
        var item = itemDetails?.Item;

        if (item == null)

        {
            Subject.ReplyToUnknownInput(source);

            return;
        }

        if (!source.TryTakeGold(itemDetails!.Price))
        {
            Subject.Close(source);

            return;
        }

        source.FaceSprite = item.Template.ItemSprite.DisplaySprite;
        source.Refresh(true);
        source.Display();
    }
}
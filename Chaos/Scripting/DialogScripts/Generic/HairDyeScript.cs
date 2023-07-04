using Chaos.Common.Definitions;
using Chaos.Extensions.Common;
using Chaos.Models.Data;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.DialogScripts.Generic;

public class HairDyeScript : DialogScriptBase
{
    private readonly IItemFactory ItemFactory;

    public HairDyeScript(Dialog subject, IItemFactory itemFactory)
        : base(subject) => ItemFactory = itemFactory;

    public override void OnDisplaying(Aisling source)
    {
        var colors = Enum.GetValues<DisplayColor>();

        foreach (var color in colors)
        {
            var item = ItemFactory.CreateFaux("hairDyeContainer");
            item.DisplayName = $"{color} Hair Dye";
            item.Color = color;
            Subject.Items.Add(ItemDetails.BuyWithGold(item));
        }
    }

    public override void OnNext(Aisling source, byte? optionIndex = null)
    {
        if (!Subject.MenuArgs.TryGet<string>(0, out var dye))
        {
            Subject.ReplyToUnknownInput(source);

            return;
        }

        var itemDetails = Subject.Items.FirstOrDefault(x => x.Item.DisplayName.EqualsI(dye));
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

        source.HairColor = item.Color;
        source.Refresh(true);
    }
}
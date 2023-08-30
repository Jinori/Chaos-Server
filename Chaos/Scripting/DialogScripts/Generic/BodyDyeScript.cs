using Chaos.Common.Definitions;
using Chaos.Extensions;
using Chaos.Extensions.Common;
using Chaos.Models.Data;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.DialogScripts.Generic;

public class BodyDyeScript : DialogScriptBase
{
    private readonly IItemFactory ItemFactory;

    public BodyDyeScript(Dialog subject, IItemFactory itemFactory)
        : base(subject) => ItemFactory = itemFactory;

    public override void OnDisplaying(Aisling source)
    {
        foreach (var color in Enum.GetValues<BodyColor>())
        {
            var item = ItemFactory.CreateFaux("hairDyeContainer");
            item.CustomDisplayName = $"{color} Body Dye";
            item.Color = color.ConvertToDisplayColor();
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

        if (itemDetails == null)
        {
            Subject.ReplyToUnknownInput(source);

            return;
        }

        var item = itemDetails.Item;

        if (!source.TryTakeGold(itemDetails.Price))
        {
            Subject.Close(source);

            return;
        }

        source.BodyColor = item.Color.ConvertToBodyColor();
        source.Refresh(true);
    }
}
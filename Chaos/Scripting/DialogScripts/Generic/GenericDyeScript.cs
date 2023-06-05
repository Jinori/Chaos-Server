using Chaos.Common.Definitions;
using Chaos.Extensions.Common;
using Chaos.Models.Data;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.DialogScripts.Generic;

public class GenericDyeScript : DialogScriptBase
{
    private readonly IItemFactory ItemFactory;
    private const int DyeCost = 5000;

    public GenericDyeScript(Dialog subject, IItemFactory itemFactory)
        : base(subject)
    {
        ItemFactory = itemFactory;
    }

    public override void OnDisplaying(Aisling source)
    {
        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "generic_dyeiteminitial":
            {
                OnDisplayingInitial(source);
                break;
            }
            case "generic_dyeitemcolorselection":
            {
                OnDisplayingColorSelect(source);
                break;
            }
            case "generic_dyeitemconfirmation":
            {
                OnDisplayingConfirmation(source);
                break;
            }
            case "generic_dyeitemfinish":
            {
                OnDisplayingAccepted(source);
                break;
            }   
        }
    }

    private void OnDisplayingAccepted(Aisling source)
    {
        if (!TryFetchArgs<byte, string>(out var slot, out var dyedItemName) || !source.Inventory.TryGetObject(slot, out var item) || !item.Template.IsDyeable)
        {
            Subject.ReplyToUnknownInput(source);
            return;
        }
        var colorName = dyedItemName.ReplaceI(item.Template.Name, string.Empty).Trim();
        if (!Enum.TryParse(colorName, out DisplayColor color))
        {
            Subject.ReplyToUnknownInput(source);
            return;
        }

        if (!source.TryTakeGold(DyeCost))
        {
            Subject.Reply(source, $"You do not have enough gold to dye your {item.DisplayName}. You need {DyeCost} gold.");
            return;
        }

        source.Inventory.Update(slot, item1 =>
        {
            item.DisplayName = dyedItemName;
            item.Color = color;
        });
        
        Subject.InjectTextParameters(item.DisplayName, colorName, DyeCost);
    }

    private void OnDisplayingConfirmation(Aisling source)
    {
        if (!TryFetchArgs<byte, string>(out var slot, out var dyedItemName) || !source.Inventory.TryGetObject(slot, out var item) || !item.Template.IsDyeable)
        {
            Subject.ReplyToUnknownInput(source);
            return;
        }
        var colorName = dyedItemName.ReplaceI(item.Template.Name, string.Empty).Trim();
        if (!Enum.TryParse(colorName, out DisplayColor color))
        {
            Subject.ReplyToUnknownInput(source);
            return;
        }
        Subject.InjectTextParameters(item.DisplayName, colorName, DyeCost);
    }

    private void OnDisplayingColorSelect(Aisling source)
    {
        if (!TryFetchArgs<byte>(out var slot) || !source.Inventory.TryGetObject(slot, out var item) || !item.Template.IsDyeable)
        {
            Subject.ReplyToUnknownInput(source);
            return;
        }
        var colors = Enum.GetValues<DisplayColor>();
        foreach (var color in colors)
        {
            var fauxItem = ItemFactory.CreateFaux(item.Template.TemplateKey);
            fauxItem.DisplayName = $"{color} {item.Template.Name}";
            fauxItem.Color = color;
            Subject.Items.Add(new ItemDetails
            {
                Item = fauxItem,
                Price = DyeCost
            });
        }
        Subject.InjectTextParameters(item.DisplayName);
    }
    
    private void OnDisplayingInitial(Aisling source) => Subject.Slots = source.Inventory.Where(x => x.Template.IsDyeable).Select(x => x.Slot).ToList();
}
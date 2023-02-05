using System.Text;
using Chaos.Common.Definitions;
using Chaos.Extensions.Common;
using Chaos.Objects.Menu;
using Chaos.Objects.Panel;
using Chaos.Objects.World;
using Chaos.Scripts.DialogScripts.Abstractions;
using Chaos.Utilities;

namespace Chaos.Scripts.DialogScripts.Generic;

public class DyeShopScript : DialogScriptBase
{
    private readonly InputCollector InputCollector;
    private DisplayColor? DisplayColor;
    private Item? Item;

    public DyeShopScript(Dialog subject)
        : base(subject) =>
        InputCollector = new InputCollectorBuilder()
                         .RequestTextInput(DialogString.From(RequestColorInput))
                         .HandleInput(HandleColorInput)
                         .RequestOptionSelection(
                             DialogString.From(() => $"{Item!.DisplayName} will be dyed {DisplayColor}. Is this correct?"),
                             DialogString.Yes,
                             DialogString.No)
                         .HandleInput(HandleConfirmation)
                         .Build();

    public bool HandleColorInput(Aisling aisling, Dialog dialog, int? option = null)
    {
        if (!dialog.MenuArgs.TryGet<DisplayColor>(1, out var color))
        {
            dialog.Reply(aisling, DialogString.UnknownInput.Value);

            return false;
        }

        DisplayColor = color;

        return true;
    }

    public bool HandleConfirmation(Aisling aisling, Dialog dialog, int? option = null)
    {
        if (option is not 1)
            return false;

        aisling.Inventory.Update(
            Item!.Slot,
            i =>
            {
                i.Color = DisplayColor!.Value;
            });

        dialog.Close(aisling);

        return true;
    }

    public override void OnNext(Aisling source, byte? optionIndex = null)
    {
        if (Item == null)
        {
            if (!Subject.MenuArgs.TryGet<string>(0, out var itemname))
            {
                Subject.Reply(source, DialogString.UnknownInput.Value);

                return;
            }

            Item = source.Inventory.FirstOrDefault(i => i.DisplayName.EqualsI(itemname));

            if (Item == null)
            {
                Subject.Reply(source, DialogString.UnknownInput.Value);

                return;
            }
        }

        InputCollector.Collect(source, Subject, optionIndex);
    }

    public string RequestColorInput()
    {
        var builder = new StringBuilder();
        builder.AppendLine("What color would you like this item to be?");
        builder.AppendLine("These are the available colors. ");
        var s = Enum.GetNames<DisplayColor>();

        foreach (var t in s)
        {
            builder.Append("[");
            builder.Append(t);
            builder.Append("], ");
        }

        return builder.ToString();
    }
}
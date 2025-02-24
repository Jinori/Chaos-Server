using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Scripting.ItemScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.ItemScripts.Enchantments;

public class EnchantScrollItemScript : ItemScriptBase
{
    private readonly IDialogFactory DialogFactory;

    public EnchantScrollItemScript(Item subject, IDialogFactory dialogFactory)
        : base(subject)
        => DialogFactory = dialogFactory;

    public override void OnUse(Aisling source)
    {
        // 1) Confirm the scroll item is still in this slot
        if (!source.Inventory.TryGetObject(Subject.Slot, out var scrollItem))
        {
            source.SendOrangeBarMessage("Could not find that scroll in your inventory.");

            return;
        }

        // 2) Create the dialog. The second parameter is the "dialog source entity."
        //    Typically, we pass the item (or Subject) here.
        var dialog = DialogFactory.Create("enchantingscroll_selectitem", scrollItem);

        // 3) Assign the scroll item as the dialog's context
        dialog.Context = Subject.Slot;

        // 4) Finally, show the dialog to the player
        dialog.Display(source);
    }
}
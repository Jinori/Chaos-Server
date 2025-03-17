using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Extensions.Common;
using Chaos.Models.Data;
using Chaos.Models.Menu;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.DialogScripts.Crafting;

public class EnchantingScrollScript : DialogScriptBase
{
    private readonly IDialogFactory DialogFactory;
    private readonly IItemFactory ItemFactory;

    // For checking if the target item already has an enchant/prefix
    private readonly string[] Prefix =
    {
        "Airy",
        "Ancient",
        "Blazing",
        "Breezy",
        "Bright",
        "Brilliant",
        "Crippling",
        "Cursed",
        "Darkened",
        "Eternal",
        "Focused",
        "Fury",
        "Gleaming",
        "Hale",
        "Hasty",
        "Hazy",
        "Howling",
        "Infernal",
        "Lucky",
        "Magic",
        "Meager",
        "Mighty",
        "Minor",
        "Modest",
        "Mystical",
        "Nimble",
        "Persisting",
        "Potent",
        "Powerful",
        "Precision",
        "Primal",
        "Pristine",
        "Resilient",
        "Ruthless",
        "Savage",
        "Serene",
        "Shaded",
        "Shrouded",
        "Sinister",
        "Skillful",
        "Soft",
        "Soothing",
        "Spirited",
        "Sturdy",
        "Swift",
        "Thick",
        "Tight",
        "Tiny",
        "Tough",
        "Valiant",
        "Whirling",
        "Wise"
    };

    public EnchantingScrollScript(Dialog subject, IDialogFactory dialogFactory, IItemFactory itemFactory)
        : base(subject)
    {
        DialogFactory = dialogFactory;
        ItemFactory = itemFactory;
    }

    public override void OnDisplaying(Aisling source)
    {
        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "enchantingscroll_selectitem":
            {
                OnDisplayingShowPlayerItems(source);

                break;
            }
            case "enchantingscroll_confirmation":
            {
                OnDisplayingConfirmation(source);

                break;
            }
            case "enchantingscroll_accepted":
            {
                OnDisplayingAccepted(source);

                break;
            }
            case "enchantingscroll_confirmdisenchant":
            {
                OnDisplayingDisenchant(source);

                break;
            }
        }
    }

    /// <summary>
    ///     3) Actually applies the enchant to the selected item. We do so by looking up the scroll’s prefix or script
    ///     property, then matching that to a recipe in your dictionary, and finally applying the modifications to the item.
    /// </summary>
    private void OnDisplayingAccepted(Aisling source)
    {
        // We expect the slot of the item to enchant
        if (!TryFetchArg<byte>(0, out var slot) || !source.Inventory.TryGetObject(slot, out var itemToEnchant))
        {
            Subject.ReplyToUnknownInput(source);

            return;
        }

        // We expect the scroll in the context
        if (Subject.Context is not byte scrollSlot || !source.Inventory.TryGetObject(scrollSlot, out var enchantScroll))
        {
            Subject.Reply(source, "Something went wrong with the enchantment scroll. Notify a GM.");

            return;
        }

        // For example, let’s say the scroll’s “Prefix” is how we identify which enchant we apply.
        var scrollPrefix = enchantScroll.Prefix?.ToLower();

        if (string.IsNullOrEmpty(scrollPrefix))
        {
            Subject.Reply(source, "This scroll does not contain a valid enchantment.");

            return;
        }

        //  Look up the corresponding recipe from your dictionary that matches the scrollPrefix
        var recipe = CraftingRequirements.EnchantingRequirements.Values.FirstOrDefault(r => r.Prefix.EqualsI(scrollPrefix));

        if (recipe == null)
        {
            Subject.Reply(source, "This scroll doesn't match any known enchantment recipe.");

            return;
        }

        //  Apply the recipe’s modification to the item
        recipe.Modification?.Invoke(itemToEnchant);

        //  Update the inventory & inform the player
        source.Inventory.Update(itemToEnchant.Slot);
        Subject.InjectTextParameters(recipe.Prefix, itemToEnchant.Template.Name);

        source.Inventory.RemoveQuantity(enchantScroll.Slot, 1);
    }

    /// <summary>
    ///     2) Confirms the user’s choice. If the item has an existing prefix from our list, prompt them to disenchant first.
    /// </summary>
    private void OnDisplayingConfirmation(Aisling source)
    {
        // The user has chosen a slot from the previous step
        if (!TryFetchArg<byte>(0, out var slot) || !source.Inventory.TryGetObject(slot, out var item))
        {
            Subject.ReplyToUnknownInput(source);

            return;
        }

        if (Subject.Context is not byte scrollSlot || !source.Inventory.TryGetObject(scrollSlot, out var enchantScroll))
        {
            Subject.Reply(source, "Couldn't find the enchant scroll in your inventory anymore.");

            return;
        }

        // If the target item already has an existing prefix, we ask them to disenchant first
        if (Prefix.Any(prefix => item.DisplayName.StartsWith(prefix + " ", StringComparison.OrdinalIgnoreCase)))
        {
            // Jump to a "disenchant" step if we want them to remove the old prefix first
            Subject.Close(source);

            var dialog = DialogFactory.Create("enchantingscroll_disenchant", Subject.DialogSource);
            dialog.MenuArgs = Subject.MenuArgs;
            dialog.Context = scrollSlot; // keep the same scroll context
            dialog.InjectTextParameters(item.DisplayName);
            dialog.Display(source);

            return;
        }

        // Otherwise, we simply show them a confirmation
        Subject.InjectTextParameters(item.DisplayName, enchantScroll.DisplayName);
    }

    /// <summary>
    ///     4) If the item already had a prefix, this step clears it. Then user can start over.
    /// </summary>
    private void OnDisplayingDisenchant(Aisling source)
    {
        if (!TryFetchArg<byte>(0, out var slot) || !source.Inventory.TryGetObject(slot, out var item))
        {
            Subject.ReplyToUnknownInput(source);

            return;
        }

        // We still expect the scroll in context, though we might not need it here
        if (Subject.Context is not byte scrollSlot || !source.Inventory.TryGetObject(scrollSlot, out _))
        {
            Subject.Reply(source, "Something went wrong with the enchantment scroll. Notify a GM.");

            return;
        }

        // Clear prefix & scripts from the item
        if (!source.Inventory.TryGetObject(item.Slot, out var itemToDisenchant))
        {
            Subject.Reply(source, "Something went wrong.");
            return;
        }

        itemToDisenchant.Prefix = string.Empty;
        itemToDisenchant.ScriptKeys.Clear();

        // Restore default modifiers
        if (itemToDisenchant.Template.Modifiers != null)
            itemToDisenchant.Modifiers = itemToDisenchant.Template.Modifiers;
        else
            itemToDisenchant.Modifiers = new Attributes();

        source.Inventory.Update(itemToDisenchant.Slot);
        
        // Show them a message
        Subject.InjectTextParameters(item.DisplayName);
    }

    private void OnDisplayingShowPlayerItems(Aisling source)
    {
        if (Subject.Context is not byte scrollSlot)
        {
            Subject.Reply(source, "No valid scroll context was found. Notify a GM.");

            return;
        }

        if (!source.Inventory.TryGetObject(scrollSlot, out var scrollItem))
        {
            Subject.Reply(source, "The scroll is no longer in your inventory.");

            return;
        }

        var scrollPrefix = scrollItem.Prefix?.ToLower();

        if (string.IsNullOrEmpty(scrollPrefix))
        {
            Subject.Reply(source, "This scroll does not contain a valid enchantment.");

            return;
        }

        //  Look up the corresponding recipe from your dictionary that matches the scrollPrefix
        var recipe = CraftingRequirements.EnchantingRequirements.Values.FirstOrDefault(r => r.Prefix.EqualsI(scrollPrefix));

        if (recipe == null)
        {
            Subject.Reply(source, "This scroll doesn't match any known enchantment recipe.");

            return;
        }

        var recipeItem = ItemFactory.Create(recipe.TemplateKey);

        var modifiableItems = source.Inventory
                                    .Where(x => x.Template.IsModifiable && (x.Template.Level >= recipeItem.Level))
                                    .ToList();

        foreach (var item in modifiableItems.ToList())
            if (!item.Template.RequiresMaster && recipeItem.Template.RequiresMaster)
                modifiableItems.Remove(item);

        if (modifiableItems.Count == 0)
        {
            Subject.Reply(source, "You don’t have any items that can be enchanted.", "Close");

            return;
        }

        Subject.Slots = modifiableItems.Select(x => x.Slot)
                                       .ToList();
        Subject.InjectTextParameters(scrollItem.DisplayName);
    }
}
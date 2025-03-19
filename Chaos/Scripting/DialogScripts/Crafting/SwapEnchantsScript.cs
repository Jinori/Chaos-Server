using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Extensions.Common;
using Chaos.Models.Menu;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Scripting.ItemScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.DialogScripts.Crafting;

public class SwapEnchantsScript : DialogScriptBase
{
    private readonly IDialogFactory DialogFactory;
    private readonly IItemFactory ItemFactory;

    // 1) Define a dictionary of which prefixes can swap to which. 
    //    This is just an example; fill it with the actual swaps you want.
    private static readonly Dictionary<string, List<string>> PrefixSwapOptions
        = new(StringComparer.OrdinalIgnoreCase)
    {
        //beginner
        { "Skillful",    new() { "Swift", "Meager", "Mystical", "Shrouded" } },
        { "Swift",    new() { "Skillful", "Meager", "Shrouded", "Mystical" } },
        { "Meager",   new() { "Skillful", "Swift", "Shrouded", "Mystical" } },
        { "Mystical",  new() { "Skillful", "Meager", "Shrouded", "Swift" } },
        { "Shrouded",  new() { "Skillful", "Meager", "Swift", "Mystical" } },
        //basic
        { "Lucky", new() { "Mighty", "Breezy", "Minor", "Modest", "Tiny", "Darkened", "Serene" } },
        { "Mighty", new() { "Breezy", "Minor", "Modest", "Tiny", "Darkened", "Serene", "Lucky" } },
        { "Breezy", new() { "Mighty", "Minor", "Modest", "Tiny", "Darkened", "Serene", "Lucky" } },
        { "Minor", new() { "Mighty", "Breezy", "Modest", "Tiny", "Darkened", "Serene", "Lucky" } },
        { "Modest", new() { "Mighty", "Breezy", "Minor", "Tiny", "Darkened", "Serene", "Lucky" } },
        { "Tiny", new() { "Mighty", "Breezy", "Minor", "Modest", "Darkened", "Serene", "Lucky" } },
        { "Darkened", new() { "Mighty", "Breezy", "Minor", "Modest", "Tiny", "Serene", "Lucky" } },
        { "Serene", new() { "Mighty", "Breezy", "Minor", "Modest", "Tiny", "Darkened", "Lucky" } },
        //apprentice
        { "Airy", new() { "Valiant", "Soft", "Hasty", "Wise", "Hale", "Brilliant", "Tough", "Nimble", "Focused", "Hazy", "Crippling", "Powerful", "Bright"} },
        { "Valiant", new() { "Soft", "Hasty", "Wise", "Hale", "Brilliant", "Tough", "Nimble", "Focused", "Hazy", "Crippling", "Powerful", "Bright", "Airy" } },
        { "Soft", new() { "Valiant", "Hasty", "Wise", "Hale", "Brilliant", "Tough", "Nimble", "Focused", "Hazy", "Crippling", "Powerful", "Bright", "Airy" } },
        { "Hasty", new() { "Valiant", "Soft", "Wise", "Hale", "Brilliant", "Tough", "Nimble", "Focused", "Hazy", "Crippling", "Powerful", "Bright", "Airy" } },
        { "Wise", new() { "Valiant", "Soft", "Hasty", "Hale", "Brilliant", "Tough", "Nimble", "Focused", "Hazy", "Crippling", "Powerful", "Bright", "Airy" } },
        { "Hale", new() { "Valiant", "Soft", "Hasty", "Wise", "Brilliant", "Tough", "Nimble", "Focused", "Hazy", "Crippling", "Powerful", "Bright", "Airy" } },
        { "Brilliant", new() { "Valiant", "Soft", "Hasty", "Wise", "Hale", "Tough", "Nimble", "Focused", "Hazy", "Crippling", "Powerful", "Bright", "Airy" } },
        { "Tough", new() { "Valiant", "Soft", "Hasty", "Wise", "Hale", "Brilliant", "Nimble", "Focused", "Hazy", "Crippling", "Powerful", "Bright", "Airy" } },
        { "Nimble", new() { "Valiant", "Soft", "Hasty", "Wise", "Hale", "Brilliant", "Tough", "Focused", "Hazy", "Crippling", "Powerful", "Bright", "Airy" } },
        { "Focused", new() { "Valiant", "Soft", "Hasty", "Wise", "Hale", "Brilliant", "Tough", "Nimble", "Hazy", "Crippling", "Powerful", "Bright", "Airy" } },
        { "Hazy", new() { "Valiant", "Soft", "Hasty", "Wise", "Hale", "Brilliant", "Tough", "Nimble", "Focused", "Crippling", "Powerful", "Bright", "Airy" } },
        { "Crippling", new() { "Valiant", "Soft", "Hasty", "Wise", "Hale", "Brilliant", "Tough", "Nimble", "Focused", "Hazy", "Powerful", "Bright", "Airy" } },
        { "Powerful", new() { "Valiant", "Soft", "Hasty", "Wise", "Hale", "Brilliant", "Tough", "Nimble", "Focused", "Hazy", "Crippling", "Bright", "Airy" } },
        { "Bright", new() { "Valiant", "Soft", "Hasty", "Wise", "Hale", "Brilliant", "Tough", "Nimble", "Focused", "Hazy", "Crippling", "Powerful", "Airy" } },
        //artisan
        { "Potent", new() { "Precision", "Tight", "Savage", "Whirling", "Ruthless", "Eternal", "Ancient"} },
        { "Precision", new() { "Tight", "Savage", "Whirling", "Ruthless", "Eternal", "Ancient", "Potent" } },
        { "Tight", new() { "Precision", "Savage", "Whirling", "Ruthless", "Eternal", "Ancient", "Potent" } },
        { "Savage", new() { "Precision", "Tight", "Whirling", "Ruthless", "Eternal", "Ancient", "Potent" } },
        { "Whirling", new() { "Precision", "Tight", "Savage", "Ruthless", "Eternal", "Ancient", "Potent" } },
        { "Ruthless", new() { "Precision", "Tight", "Savage", "Whirling", "Eternal", "Ancient", "Potent" } },
        { "Eternal", new() { "Precision", "Tight", "Savage", "Whirling", "Ruthless", "Ancient", "Potent" } },
        { "Ancient", new() { "Precision", "Tight", "Savage", "Whirling", "Ruthless", "Eternal", "Potent" } },
        //adept
        { "Soothing", new() { "Persisting", "Cursed", "Blazing", "Howling" } },
        { "Persisting", new() { "Cursed", "Blazing", "Howling", "Soothing" } },
        { "Cursed", new() { "Persisting", "Blazing", "Howling", "Soothing" } },
        { "Blazing", new() { "Persisting", "Cursed", "Howling", "Soothing" } },
        { "Howling", new() { "Persisting", "Cursed", "Blazing", "Soothing" } },
        //advanced
        { "Fury", new() { "Sturdy", "Resilient", "Spirited", "Pristine", "Shaded" } },
        { "Sturdy", new() { "Resilient", "Spirited", "Pristine", "Shaded", "Fury" } },
        { "Resilient", new() { "Sturdy", "Spirited", "Pristine", "Shaded", "Fury" } },
        { "Spirited", new() { "Sturdy", "Resilient", "Pristine", "Shaded", "Fury" } },
        { "Pristine", new() { "Sturdy", "Resilient", "Spirited", "Shaded", "Fury" } },
        { "Shaded", new() { "Sturdy", "Resilient", "Spirited", "Pristine", "Fury" } },
        
        //expert
        { "Sinister", new() { "Gleaming", "Infernal", "Primal", "Thick" } },
        { "Gleaming", new() { "Infernal", "Primal", "Thick", "Sinister" } },
        { "Infernal", new() { "Gleaming", "Primal", "Thick", "Sinister" } },
        { "Primal", new() { "Gleaming", "Infernal", "Thick", "Sinister" } },
        { "Thick", new() { "Gleaming", "Infernal", "Primal", "Sinister" } }
    };

    public SwapEnchantsScript(Dialog subject, IDialogFactory dialogFactory, IItemFactory itemFactory)
        : base(subject)
    {
        DialogFactory = dialogFactory;
        ItemFactory = itemFactory;
    }

    public override void OnDisplaying(Aisling source)
    {
        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "enchant_swap_initial":
                // Step 1: Ask which item they want to swap
                OnDisplayingChooseItem(source);
                break;

            case "enchant_swap_choose":
                // Step 2: Ask which new enchant they'd like
                OnDisplayingChooseEnchant(source);
                break;

            case "enchant_swap_confirm":
                // Step 3: Confirm swap
                OnDisplayingConfirmSwap(source);
                break;
        }
    }

    // ─────────────────────────────────────────────────────────────────
    // STEP 1: Player Chooses an Enchanted Item
    // ─────────────────────────────────────────────────────────────────
    private void OnDisplayingChooseItem(Aisling source)
    {
        // Find items that already have an enchant (prefix) we can swap
        var enchantedItems = source.Inventory
            .Where(HasSwappableEnchant)
            .Where(i => !i.DisplayName.Contains("Scroll", StringComparison.OrdinalIgnoreCase))
            .ToList();

        if (!enchantedItems.Any())
        {
            Subject.Reply(source,
                "You have no enchanted items to swap!",
                "Close");
            return;
        }

        // Provide the valid item slots
        Subject.Slots = enchantedItems.Select(i => i.Slot).ToList();
    }

    private bool HasSwappableEnchant(Item item)
    {
        // We'll consider an item swappable if it has a known prefix in our dictionary
        if (string.IsNullOrWhiteSpace(item.Prefix) || item.Prefix.Equals("None", StringComparison.OrdinalIgnoreCase))
            return false;

        // Must also be in the dictionary
        return PrefixSwapOptions.ContainsKey(item.Prefix);
    }
    private void OnDisplayingChooseEnchant(Aisling source)
    {
        // We expect the chosen item slot
        if (!TryFetchArg<byte>(0, out var chosenSlot)
            || !source.Inventory.TryGetObject(chosenSlot, out var chosenItem))
        {
            Subject.ReplyToUnknownInput(source);
            return;
        }

        var currentPrefix = chosenItem.Prefix;
        if (currentPrefix == null 
            || !PrefixSwapOptions.TryGetValue(currentPrefix, out var possibleEnchants)
            || possibleEnchants.Count == 0)
        {
            Subject.Reply(source,
                "We couldn't find any valid swap options for this enchant.",
                "Close");
            return;
        }

        // Provide each possible new prefix as a dialog option
        Subject.Options.Clear();
        for (int i = 0; i < possibleEnchants.Count; i++)
        {
            var newPrefix = possibleEnchants[i];
            Subject.Options.Add(new DialogOption
            {
                OptionText = newPrefix,
                DialogKey = "enchant_swap_confirm"
            });
        }
    }
    private void OnDisplayingConfirmSwap(Aisling source)
    {
        // We expect 2 args: the item slot, and the new enchant index
        if (!TryFetchArg<byte>(0, out var chosenSlot)
            || Subject.Context is not byte optionIndex)
        {
            Subject.ReplyToUnknownInput(source);
            return;
        }

        if (!source.Inventory.TryGetObject(chosenSlot, out var chosenItem))
        {
            Subject.Reply(source, "Couldn't find that item in your inventory.");
            return;
        }

        var currentPrefix = chosenItem.Prefix;
        if ((currentPrefix == null)
            || !PrefixSwapOptions.TryGetValue(currentPrefix, out var possibleEnchants)
            || (optionIndex >= possibleEnchants.Count))
        {
            Subject.Reply(source,
                "Invalid enchant selection. Please try again.",
                "Close");
            return;
        }

        var newPrefix = possibleEnchants[optionIndex];
        
        Subject.InjectTextParameters(currentPrefix, newPrefix);
    }
    private void OnDisplayingFinalizeSwap(Aisling source)
    {
        // Same 2 args as Step 3
        if (!TryFetchArg<byte>(0, out var chosenSlot)
            || Subject.Context is not byte optionIndex)
        {
            Subject.ReplyToUnknownInput(source);
            return;
        }

        if (!source.Inventory.TryGetObject(chosenSlot, out var chosenItem))
        {
            Subject.Reply(source, "Couldn't find that item in your inventory.");
            return;
        }

        // Identify new enchant
        var currentPrefix = chosenItem.Prefix;
        if ((currentPrefix == null) 
            || !PrefixSwapOptions.TryGetValue(currentPrefix, out var possibleEnchants)
            || (optionIndex >= possibleEnchants.Count))
        {
            Subject.Reply(source,
                "Invalid enchant selection. Please try again.",
                "Close");
            return;
        }

        var newPrefix = possibleEnchants[optionIndex];

        // 1) Remove the current prefix (and any relevant scripts/stats)
        RemoveEnchant(source, chosenItem);

        // 2) Apply the new prefix
        ApplyEnchant(source, chosenItem, newPrefix);

        // 3) Update inventory
        source.Inventory.Update(chosenItem.Slot);

        // 4) Inform the player
        Subject.Reply(source,
            $"Your {chosenItem.Template.Name} is now enchanted with **{newPrefix}**!",
            "Close");
    }

    // ─────────────────────────────────────────────────────────────────
    // HELPERS: Remove & Apply Enchant
    // ─────────────────────────────────────────────────────────────────
    private void RemoveEnchant(Aisling source, Item item)
    {
        if (string.IsNullOrEmpty(item.Prefix) 
            || item.Prefix.Equals("None", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }
        
        var prefix = item.Prefix;
        var scriptType = IPrefixEnchantmentScript.PrefixEnchantmentScripts[prefix!];
        var removeEnchant = typeof(IPrefixEnchantmentScript).GetMethod(nameof(IPrefixEnchantmentScript.RemovePrefix))!
                                                            .MakeGenericMethod(scriptType);
        
        removeEnchant.Invoke(null, [item]);
        item.RemoveScript(scriptType);
        
        source.Inventory.Update(item.Slot);
        
        // Show them a message
        Subject.InjectTextParameters(item.DisplayName);

        // Example: item.RemoveScript(...)
        item.Prefix = null;
        // Possibly remove stats or other enchant effects
    }

    private void ApplyEnchant(Aisling source, Item item, string newEnchant)
    {
        var recipe = CraftingRequirements.EnchantingRequirements.Values.FirstOrDefault(r => r.Prefix.EqualsI(newEnchant));

        if (recipe == null)
        {
            Subject.Reply(source, "This scroll doesn't match any known enchantment recipe.");

            return;
        }

        //  Apply the recipe’s modification to the item
        recipe.Modification?.Invoke(item);
        source.Inventory.Update(item.Slot);
        Subject.InjectTextParameters(recipe.Prefix, item.Template.Name);
    }

    public override void OnNext(Aisling source, byte? optionIndex = null)
    {
        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "enchant_swap_choose":
            {
                Subject.Context = (byte)(optionIndex!.Value - 1);
                break;
            }

            case "enchant_swap_confirm":
            {
                if (optionIndex == 1)
                    OnDisplayingFinalizeSwap(source);

                break;
            }
        }
    }
}
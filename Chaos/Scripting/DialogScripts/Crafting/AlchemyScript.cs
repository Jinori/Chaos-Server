using Chaos.Definitions;
using Chaos.Models.Data;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Utilities;

namespace Chaos.Scripting.DialogScripts.Crafting;

public class AlchemyScript : DialogScriptBase
{
    private readonly IItemFactory ItemFactory;

    /// <inheritdoc />
    public AlchemyScript(Dialog subject, IItemFactory itemFactory)
        : base(subject) =>
        ItemFactory = itemFactory;

    /// <inheritdoc />
    public override void OnDisplaying(Aisling source)
    {
        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "alf_craftalchemyinitial":
            {
                OnDisplayingInitial(source);
                break;
            }
            case "alf_craftalchemyconfirmation":
            {
                OnDisplayingConfirmation(source);
                break;
            }
            case "alf_craftalchemyaccepted":
            {
                OnDisplayingAccepted(source);
                break;
            }
        }
    }

    private void OnDisplayingInitial(Aisling source)
    {
        if (source.Trackers.Flags.TryGetFlag(out AlchemyRecipes recipes))
        {
            foreach (var recipe in CraftingRequirements.AlchemyRequirements)
            {
                if ((recipes & recipe.Key) == recipe.Key)
                {
                    var item = ItemFactory.CreateFaux(recipe.Key.ToString());
                    Subject.Items.Add(ItemDetails.DisplayRecipe(item));
                }
            }
        }
    }

    private void OnDisplayingConfirmation(Aisling source)
    {
        if (!Subject.MenuArgs.TryGet<string>(0, out var itemName) || string.IsNullOrWhiteSpace(itemName))
        {
            Subject.Reply(source, DialogString.UnknownInput.Value);

            return;
        }

        foreach (var recipe in CraftingRequirements.AlchemyRequirements)
        {
            var itemNameFix = itemName.Replace(" ", "");
            if (itemNameFix == recipe.Key.ToString())
            {
                if (CraftingRequirements.AlchemyRequirements.TryGetValue(recipe.Key, out var requirement))
                {
                    var ingredientList = new List<string>();

                    foreach (var regeant in requirement)
                    {
                        ingredientList.Add($"({regeant.Amount}) {regeant.DisplayName}");
                    }

                    var ingredients = string.Join(" and ", ingredientList);
                    Subject.InjectTextParameters(itemName, ingredients);
                }
            }
        }
    }

    private void OnDisplayingAccepted(Aisling source)
    {
        if (!Subject.MenuArgs.TryGet<string>(0, out var itemName) || string.IsNullOrWhiteSpace(itemName))
        {
            Subject.Reply(source, DialogString.UnknownInput.Value);
            return;
        }

        foreach (var recipe in CraftingRequirements.AlchemyRequirements)
        {
            var itemNameFix = itemName.Replace(" ", "");
            if (itemNameFix == recipe.Key.ToString())
            {
                if (CraftingRequirements.AlchemyRequirements.TryGetValue(recipe.Key, out var requirement))
                {
                    foreach (var reagant in requirement)
                    {
                        if ((reagant.DisplayName != null) && !source.Inventory.HasCount(reagant.DisplayName, reagant.Amount))
                        {
                            Subject.Close(source);
                            source.SendOrangeBarMessage($"You do not have the required amount of ({reagant.Amount}) of {reagant.DisplayName}.");
                            return;
                        }
                    }
                    
                    var newCraft = ItemFactory.Create(recipe.Key.ToString());
                    foreach (var removeRegant in requirement)
                    {
                        if (removeRegant.DisplayName != null)
                            source.Inventory.RemoveQuantity(removeRegant.DisplayName, removeRegant.Amount);
                    }
                    source.Inventory.TryAddToNextSlot(newCraft);
                    Subject.InjectTextParameters(recipe.Key.ToString());   
                }
            }
        }
    }
}
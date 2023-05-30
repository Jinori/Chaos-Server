using System.Globalization;
using Chaos.Common.Definitions;
using Chaos.Common.Utilities;
using Chaos.Definitions;
using Chaos.Models.Data;
using Chaos.Models.Legend;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Time;
using Chaos.Utilities;

namespace Chaos.Scripting.DialogScripts.Crafting;

public class AlchemyScript : DialogScriptBase
{
    private readonly IItemFactory ItemFactory;
    private readonly IDialogFactory DialogFactory;

    /// <inheritdoc />
    public AlchemyScript(Dialog subject, IItemFactory itemFactory, IDialogFactory dialogFactory)
        : base(subject)
    {
        ItemFactory = itemFactory;
        DialogFactory = dialogFactory;
    }

    private const int BASE_SUCCESS_RATE = 20;
    private const double SUCCESS_RATE_MULTIPLIER = 0.01;

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
        
        itemName = itemName.Replace(" ", "");
        if (Enum.TryParse(itemName, out AlchemyRecipes selectedRecipe))
        {
            if (CraftingRequirements.AlchemyRequirements.TryGetValue(selectedRecipe, out var recipeIngredients))
            {
                foreach (var reagant in recipeIngredients)
                {
                    if ((reagant.DisplayName != null) && !source.Inventory.HasCount(reagant.DisplayName, reagant.Amount))
                    {
                        Subject.Close(source);
        
                        source.SendOrangeBarMessage(
                            $"You do not have the required amount of ({reagant.Amount}) of {reagant.DisplayName}.");
                        
                        source.Say($"I'll need {reagant.DisplayName} to craft a {itemName}.");
                        return;
                    }
                }
                
                var legendMark = source.Legend.GetCount("alch");
                var timesCraftedSuccessfully =
                    source.Trackers.Counters.TryGetValue(itemName, out var value) ? value : 0;
                var successRate = Math.Min(
                    BASE_SUCCESS_RATE + (legendMark * (timesCraftedSuccessfully * SUCCESS_RATE_MULTIPLIER)),
                    100);
                
                source.Say(successRate.ToString(CultureInfo.InvariantCulture));
                
                var newCraft = ItemFactory.Create(itemName);
                foreach (var removeRegant in recipeIngredients)
                {
                    if (removeRegant.DisplayName != null)
                        source.Inventory.RemoveQuantity(removeRegant.DisplayName, removeRegant.Amount);
                }
        
                if (!Randomizer.RollChance((int)successRate))
                {
                    Subject.Close(source);
                    var dialog = DialogFactory.Create("alf_craftAlchemyFailed", Subject.DialogSource);
                    dialog.MenuArgs = Subject.MenuArgs;
                    dialog.InjectTextParameters(itemName);
                    dialog.Display(source);
                    return;
                }
        
                source.Inventory.TryAddToNextSlot(newCraft);
                source.Trackers.Counters.AddOrIncrement(itemName);
        
                source.Legend.AddOrAccumulate(
                    new LegendMark(
                        "Alchemy",
                        "alch",
                        MarkIcon.Yay,
                        MarkColor.White,
                        1,
                        GameTime.Now));
        
                Subject.InjectTextParameters(newCraft.DisplayName);
            } 
            else
            {
                Subject.Reply(source, "Recipe not found or something went wrong.");
            }
        } 
        else
        {
            Subject.Reply(source, "Invalid recipe selection.");
        }
    }
}
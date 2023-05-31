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
    private const string ITEM_COUNTER_PREFIX = "[Alchemy]";
    private readonly double BaseSuccessRate = 30;
    private Animation FailAnimation { get; } = new()
    {
        AnimationSpeed = 100,
        TargetAnimation = 59
    };
    private Animation SuccessAnimation { get; } = new()
    {
        AnimationSpeed = 100,
        TargetAnimation = 127
    };

    private double GetMultiplier(int totalTimesCrafted)
    {
        switch (totalTimesCrafted)
        {
            case <= 100:
                return 1.0; // Novice
            case <= 500:
                return 1.05; // Apprentice
            case <= 1000:
                return 1.1; // Journeyman
            case <= 1500:
                return 1.15; // Craftsman
            case <= 2200:
                return 1.2; // Artisan
            case <= 2900:
                return 1.25; // Expert Artisan
            default:
                return 1.3; // Master Artisan
        }
    }

    private double CalculateSuccessRate(int totalTimesCrafted, int timesCraftedThisItem, double baseSuccessRate)
    {
        double noviceBoost = totalTimesCrafted <= 100 ? 5 : 0; // Provide a little boost to novices
        var multiplier = GetMultiplier(totalTimesCrafted);
        var successRate = (baseSuccessRate + noviceBoost + timesCraftedThisItem / 10.0) * multiplier;

        return Math.Min(successRate, 95); // Cap the success rate at 95
    }

    private void UpdateLegendmark(Aisling source, int legendMarkCount)
    {
        var unused = source.Legend.TryGetValue("alch", out var existingMark);
        if (existingMark is not null)
        {
            switch (legendMarkCount)
            {
                case > 2900 when !existingMark.Text.Contains("Master Alchemist"):
                    existingMark.Text = "Master Alchemist";

                    break;
                case > 2200 when !existingMark.Text.Contains("Expert Alchemist"):
                    existingMark.Text = "Expert Alchemist";

                    break;
                case > 1500 when !existingMark.Text.Contains("Artisan Alchemist"):
                    existingMark.Text = "Artisan Alchemist";

                    break;
                case > 1000 when !existingMark.Text.Contains("Craftsman Alchemist"):
                    existingMark.Text = "Craftsman Alchemist";

                    break;
                case > 500 when !existingMark.Text.Contains("Journeyman Alchemist"):
                    existingMark.Text = "Journeyman Alchemist";

                    break;
                case > 100 when !existingMark.Text.Contains("Apprentice Alchemist"):
                    existingMark.Text = "Apprentice Alchemist";

                    break;
                case >= 0 when !existingMark.Text.Contains("Novice Alchemist"):
                    source.Legend.AddOrAccumulate(
                        new LegendMark(
                            "Novice Alchemist",
                            "alch",
                            MarkIcon.Yay,
                            MarkColor.White,
                            1,
                            GameTime.Now));

                    break;
            }
        }
        if (existingMark is null)
        {
            source.Legend.AddOrAccumulate(
                new LegendMark(
                    "Novice Alchemist",
                    "alch",
                    MarkIcon.Yay,
                    MarkColor.White,
                    1,
                    GameTime.Now));
        }
    }
    
    /// <inheritdoc />
    public AlchemyScript(Dialog subject, IItemFactory itemFactory, IDialogFactory dialogFactory)
        : base(subject)
    {
        ItemFactory = itemFactory;
        DialogFactory = dialogFactory;
    }

    /// <inheritdoc />
    public override void OnDisplaying(Aisling source)
    {
        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "alchemy_initial":
            {
                OnDisplayingInitial(source);
                break;
            }
            case "alchemy_confirmation":
            {
                OnDisplayingConfirmation(source);
                break;
            }
            case "alchemy_accepted":
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
                            
                            $"You do not have the required amount ({reagant.Amount}) of {reagant.DisplayName}.");
                        return;
                    }
                }
                
                var legendMarkCount = source.Legend.GetCount("alch");
                var timesCraftedThisItem =
                    source.Trackers.Counters.TryGetValue(ITEM_COUNTER_PREFIX + itemName, out var value) ? value : 0;


                source.Say(CalculateSuccessRate(legendMarkCount, timesCraftedThisItem, BaseSuccessRate).ToString(CultureInfo.InvariantCulture));
                
                var newCraft = ItemFactory.Create(itemName);
                foreach (var removeRegant in recipeIngredients)
                {
                    if (removeRegant.DisplayName != null)
                        source.Inventory.RemoveQuantity(removeRegant.DisplayName, removeRegant.Amount);
                }
        
                if (!Randomizer.RollChance((int)CalculateSuccessRate(legendMarkCount, timesCraftedThisItem, BaseSuccessRate)))
                {
                    Subject.Close(source);
                    var dialog = DialogFactory.Create("alchemy_Failed", Subject.DialogSource);
                    dialog.MenuArgs = Subject.MenuArgs;
                    dialog.InjectTextParameters(itemName);
                    dialog.Display(source);
                    source.Animate(FailAnimation);
                    return;
                }
        
                source.Inventory.TryAddToNextSlot(newCraft);
                source.Trackers.Counters.AddOrIncrement(ITEM_COUNTER_PREFIX + itemName);
                
                if (timesCraftedThisItem <= 40) 
                    UpdateLegendmark(source, legendMarkCount);

                Subject.InjectTextParameters(newCraft.DisplayName);
                source.Animate(SuccessAnimation);
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
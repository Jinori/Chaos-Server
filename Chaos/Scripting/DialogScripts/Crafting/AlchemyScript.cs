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
    
    //Constants to change based on crafting profession
    private const string ITEM_COUNTER_PREFIX = "[Alchemy]";
    private const double BASE_SUCCESS_RATE = 30;
    private const double NOVICEBOOST = 5;
    private const double SUCCESSRATEMAX = 95;
    //Set this to true if doing armorsmithing type crafting
    private readonly bool CRAFTGOODGREATGRAND = false;
    
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

    // Converts the rank string to an integer value
    private int GetRankAsInt(string rank)
    {
        var rankMappings = new Dictionary<string, int>
        {
            { "Master Alchemist", 7 },
            { "Expert Alchemist", 6 },
            { "Artisan Alchemist", 5 },
            { "Craftsman Alchemist", 4 },
            { "Journeyman Alchemist", 3 },
            { "Apprentice Alchemist", 2 },
            { "Novice Alchemist", 1 }
        };

        if (rankMappings.TryGetValue(rank, out var i))
            return i;

        return -1;
    }
    
    // Converts the status string to an integer value
    private int GetStatusAsInt(string status)
    {
        var statusMappings = new Dictionary<string, int>
        {
            { "Basic", 1 },
            { "Apprentice", 2 },
            { "Journeyman", 3 },
            { "Adept", 4 },
            { "Advanced", 5 },
            { "Expert", 6 },
            { "Master", 7 }
        };

        if (statusMappings.TryGetValue(status, out var i))
            return i;

        return 0;
    }
    
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
    
    // Calculates the success rate of crafting an item
    private double CalculateSuccessRate(int totalTimesCrafted, int timesCraftedThisItem, double baseSuccessRate)
    {
        // Provide a little boost to novices
        var noviceBoost = totalTimesCrafted <= 100 ? NOVICEBOOST : 0;
        // Get the multiplier based on total times crafted
        var multiplier = GetMultiplier(totalTimesCrafted);
        // Calculate the success rate with all the factors
        var successRate = (baseSuccessRate + noviceBoost + timesCraftedThisItem / 10.0) * multiplier;

        // Ensure the success rate does not exceed the maximum allowed value
        return Math.Min(successRate, SUCCESSRATEMAX);
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
        //Checking if the Alchemy recipe is available or not.
        if (source.Trackers.Flags.TryGetFlag(out AlchemyRecipes recipes))
        {
            //Iterating through the Alchemy recipe requirements.
            foreach (var recipe in CraftingRequirements.AlchemyRequirements)
            {
                //Checking if the recipe is available or not.
                if ((recipes & recipe.Key) == recipe.Key)
                {
                    //Creating a faux item for the recipe.
                    var item = ItemFactory.CreateFaux(recipe.Key.ToString());
                    //Adding the recipe to the subject's dialog window
                    Subject.Items.Add(ItemDetails.DisplayRecipe(item));
                }
            }
        }
    }

private void OnDisplayingConfirmation(Aisling source)
{
    // Try to get the selected menu item name from the menu arguments
    if (!Subject.MenuArgs.TryGet<string>(0, out var itemName) || string.IsNullOrWhiteSpace(itemName))
    {
        // If the name is not found or is empty, display an error message and return
        Subject.Reply(source, DialogString.UnknownInput.Value);
        return;
    }
    
     // Remove any spaces from the name
    itemName = itemName.Replace(" ", "");
     // Try to parse the selected recipe from the name
    if (Enum.TryParse(itemName, out AlchemyRecipes selectedRecipe))
    {
        // If the recipe is found, check if the player meets the level requirement
        if (CraftingRequirements.AlchemyRequirements.TryGetValue(selectedRecipe, out var requirement))
        {
            if (requirement.Item3 > source.StatSheet.Level)
            {
                // If the player doesn't meet the requirement, close the menu and display an error message
                Subject.Close(source);
                source.SendOrangeBarMessage($"You are not the required Level of {requirement.Item3} for this recipe.");
                return;
            }
             // If the player meets the requirement, create a list of ingredient names and amounts
            var ingredientList = new List<string>();
            foreach (var regeant in requirement.Item1)
            {
                ingredientList.Add($"({regeant.Amount}) {regeant.DisplayName}");
            }
             // Join the ingredient list into a single string and inject it into the confirmation message
            var ingredients = string.Join(" and ", ingredientList);
            Subject.InjectTextParameters(itemName, ingredients);
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
                foreach (var reagant in recipeIngredients.Item1)
                {
                    if ((reagant.DisplayName != null) && !source.Inventory.HasCount(reagant.DisplayName, reagant.Amount))
                    {
                        Subject.Close(source);
                        source.SendOrangeBarMessage($"You do not have the required amount ({reagant.Amount}) of {reagant.DisplayName}.");
                        return;
                    }
                }
                
                var unused = source.Legend.TryGetValue("alch", out var existingMark);
                var legendMarkCount = existingMark?.Count ?? 0;
                
                var timesCraftedThisItem =
                    source.Trackers.Counters.TryGetValue(ITEM_COUNTER_PREFIX + itemName, out var value) ? value : 0;
                
                foreach (var removeRegant in recipeIngredients.Item1)
                {
                    if (removeRegant.DisplayName != null)
                        source.Inventory.RemoveQuantity(removeRegant.DisplayName, removeRegant.Amount);
                }
        
                if (!Randomizer.RollChance((int)CalculateSuccessRate(legendMarkCount, timesCraftedThisItem, BASE_SUCCESS_RATE)))
                {
                    Subject.Close(source);
                    var dialog = DialogFactory.Create("alchemy_Failed", Subject.DialogSource);
                    dialog.MenuArgs = Subject.MenuArgs;
                    dialog.InjectTextParameters(itemName);
                    dialog.Display(source);
                    source.Animate(FailAnimation);
                    return;
                }
                
                source.Trackers.Counters.AddOrIncrement(ITEM_COUNTER_PREFIX + itemName);

                if (existingMark is not null)
                {
                    var recipeStatus = GetStatusAsInt(recipeIngredients.Item2);
                    var playerRank = GetRankAsInt(existingMark.Text);

                    if (playerRank > recipeStatus)
                    {
                        source.SendOrangeBarMessage("You can no longer gain experience from this recipe.");
                    }
                    if (playerRank <= recipeStatus)
                    {
                        UpdateLegendmark(source, legendMarkCount);
                        existingMark.Count++;
                    }
                }
                if (existingMark is null)
                    UpdateLegendmark(source, 0);

                
                if (CRAFTGOODGREATGRAND)
                {
                    var roll = Randomizer.RollSingle(100);

                    var newCraft = roll switch
                    {
                        < 5  => ItemFactory.Create("grand" + itemName),
                        < 10 => ItemFactory.Create("great" + itemName),
                        < 15 => ItemFactory.Create("good" + itemName),
                        _    => ItemFactory.Create(itemName)
                    };

                    if (!source.Inventory.TryAddToNextSlot(newCraft))
                    {
                        source.Bank.Deposit(newCraft);
                        source.SendOrangeBarMessage("You have no space. It was sent to your bank.");
                    }

                    Subject.InjectTextParameters(newCraft.DisplayName);
                }
                else
                {
                    var newCraft = ItemFactory.Create(itemName);

                    if (!source.Inventory.TryAddToNextSlot(newCraft))
                    {
                        source.Bank.Deposit(newCraft);
                        source.SendOrangeBarMessage("You have no space. It was sent to your bank.");
                    }

                    Subject.InjectTextParameters(newCraft.DisplayName);
                }
                
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
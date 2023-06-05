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

namespace Chaos.Scripting.DialogScripts.Crafting;

public class EnchantingScript : DialogScriptBase
{
    private readonly IItemFactory ItemFactory;
    private readonly IDialogFactory DialogFactory;

    //Constants to change based on crafting profession
    private const string ITEM_COUNTER_PREFIX = "[Enchant]";
    private const string LEGENDMARK_KEY = "ench";
    private const double BASE_SUCCESS_RATE = 60;
    private const double SUCCESSRATEMAX = 95;
    //Ranks from lowest to highest
    private const string RANK_ONE_TITLE = "Beginner Enchanter";
    private const string RANK_TWO_TITLE = "Novice Enchanter";
    private const string RANK_THREE_TITLE = "Apprentice Enchanter";
    private const string RANK_FOUR_TITLE = "Journeyman Enchanter";
    private const string RANK_FIVE_TITLE = "Adept Enchanter";
    private const string RANK_SIX_TITLE = "Advanced Enchanter";
    private const string RANK_SEVEN_TITLE = "Expert Enchanter";
    private const string RANK_EIGHT_TITLE = "Master Enchanter";
    private const string RECIPE_ONE_RANK = "Beginner";
    private const string RECIPE_TWO_RANK = "Basic";
    private const string RECIPE_THREE_RANK = "Apprentice";
    private const string RECIPE_FOUR_RANK = "Journeyman";
    private const string RECIPE_FIVE_RANK = "Adept";
    private const string RECIPE_SIX_RANK = "Advanced";
    private const string RECIPE_SEVEN_RANK = "Expert";
    private const string RECIPE_EIGHT_RANK = "Master";

    private readonly string[] Prefix =
    {
        "Swift",
        "Heavy",
        "Sharp",
        "Durable",
        "Light",
        "Ancient",
        "Potent",
        "Sturdy",
        "Might",
    };
    
    private readonly string[] Suffix =
    {
        "of Miraelis",
        "of Skandara",
        "of Theselene",
        "of Serendael",
        "of Ignatar",
        "of Geolith",
        "of Zephyra",
        "of Aquaedon"
    };


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
            { RANK_EIGHT_TITLE, 8 },
            { RANK_SEVEN_TITLE, 7 },
            { RANK_SIX_TITLE, 6 },
            { RANK_FIVE_TITLE, 5 },
            { RANK_FOUR_TITLE, 4 },
            { RANK_THREE_TITLE, 3 },
            { RANK_TWO_TITLE, 2 },
            { RANK_ONE_TITLE, 1 }
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
            { RECIPE_ONE_RANK, 1 },
            { RECIPE_TWO_RANK, 2 },
            { RECIPE_THREE_RANK, 3 },
            { RECIPE_FOUR_RANK, 4 },
            { RECIPE_FIVE_RANK, 5 },
            { RECIPE_SIX_RANK, 6 },
            { RECIPE_SEVEN_RANK, 7 },
            { RECIPE_EIGHT_RANK, 8 }
        };

        if (statusMappings.TryGetValue(status, out var i))
            return i;

        return 0;
    }

    private double GetMultiplier(int totalTimesCrafted) =>
        totalTimesCrafted switch
        {
            <= 25   => 1.0,
            <= 75   => 1.05,
            <= 150  => 1.1,
            <= 300  => 1.15,
            <= 500  => 1.2,
            <= 1000 => 1.25,
            <= 1500 => 1.3,
            _       => 1.35
        };

    // Calculates the success rate of crafting an item
    private double CalculateSuccessRate(
        int totalTimesCrafted,
        int timesCraftedThisItem,
        double baseSuccessRate,
        int recipeRank
    )
    {
        var rankDifficultyReduction = recipeRank switch
        {
            1 => 0,
            2 => 10,
            3 => 15,
            4 => 20,
            5 => 25,
            6 => 30,
            7 => 35,
            8 => 40,
            _ => 0
        };

        // Get the multiplier based on total times crafted
        var multiplier = GetMultiplier(totalTimesCrafted);
        // Calculate the success rate with all the factors
        var successRate = ((baseSuccessRate - rankDifficultyReduction) + timesCraftedThisItem / 10.0) * multiplier;

        // Ensure the success rate does not exceed the maximum allowed value
        return Math.Min(successRate, SUCCESSRATEMAX);
    }

    private void UpdateLegendmark(Aisling source, int legendMarkCount)
    {
        var unused = source.Legend.TryGetValue(LEGENDMARK_KEY, out var existingMark);

        if (existingMark is null)
        {
            source.Legend.AddOrAccumulate(
                new LegendMark(
                    RANK_ONE_TITLE,
                    LEGENDMARK_KEY,
                    MarkIcon.Yay,
                    MarkColor.White,
                    1,
                    GameTime.Now));
        }

        if (existingMark is not null)
        {
            existingMark.Text = legendMarkCount switch
            {
                > 1500 when !existingMark.Text.Contains(RANK_EIGHT_TITLE) => RANK_EIGHT_TITLE,
                > 1000 when !existingMark.Text.Contains(RANK_SEVEN_TITLE) => RANK_SEVEN_TITLE,
                > 500 when !existingMark.Text.Contains(RANK_SIX_TITLE)    => RANK_SIX_TITLE,
                > 300 when !existingMark.Text.Contains(RANK_FIVE_TITLE)   => RANK_FIVE_TITLE,
                > 150 when !existingMark.Text.Contains(RANK_FOUR_TITLE)   => RANK_FOUR_TITLE,
                > 75 when !existingMark.Text.Contains(RANK_THREE_TITLE)   => RANK_THREE_TITLE,
                > 25 when !existingMark.Text.Contains(RANK_TWO_TITLE)     => RANK_TWO_TITLE,
                _                                                         => existingMark.Text
            };

            existingMark.Count++;
        }
    }

    /// <inheritdoc />
    public EnchantingScript(Dialog subject, IItemFactory itemFactory, IDialogFactory dialogFactory)
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
            case "enchanting_initial":
            {
                OnDisplayingShowItems(source);

                break;
            }
            case "enchanting_selectitem":
            {
                OnDisplayingShowPlayerItems(source);

                break;
            }
            case "enchanting_confirmation":
            {
                OnDisplayingConfirmation(source);

                break;
            }
            case "enchanting_accepted":
            {
                OnDisplayingAccepted(source);

                break;
            }
        }
    }
    
    private void OnDisplayingShowPlayerItems(Aisling source)
    {
        if (Subject.Context is not CraftingRequirements.Recipe recipe)
        {
            Subject.Reply(source, "Cannot get recipe.");
            return;
        }
        
        Subject.InjectTextParameters(recipe.Name);
        Subject.Slots = source.Inventory.Where(x => x.Template.IsModifiable && !Prefix.Any(x.DisplayName.Contains) && !Suffix.Any(x.DisplayName.Contains)).Select(x => x.Slot).ToList();   
    }

    //ShowItems in a Shop Window to the player
    private void OnDisplayingShowItems(Aisling source)
    {
        // Checking if the Alchemy recipe is available or not.
        if (source.Trackers.Flags.TryGetFlag(out EnchantingRecipes recipes))
        {
            // Iterating through the Alchemy recipe requirements.
            foreach (var recipe in CraftingRequirements.EnchantingRequirements)
            {
                // Checking if the recipe is available or not.
                if (!recipes.HasFlag(recipe.Key))
                    continue;
                
                var item = ItemFactory.CreateFaux(recipe.Value.TemplateKey);
                item.DisplayName = recipe.Value.Name;
                item.ItemSprite.PanelSprite = recipe.Value.Rank switch
                {
                    RECIPE_ONE_RANK   => 146,
                    RECIPE_TWO_RANK   => 877,
                    RECIPE_THREE_RANK => 879,
                    RECIPE_FOUR_RANK  => 880,
                    RECIPE_FIVE_RANK  => 882,
                    RECIPE_SIX_RANK   => 878,
                    RECIPE_SEVEN_RANK => 883,
                    RECIPE_EIGHT_RANK => 903,
                    _                 => item.ItemSprite.PanelSprite
                };

                Subject.Items.Add(ItemDetails.DisplayRecipe(item));
                Subject.Context = recipe.Value;
            }
        }
        if (Subject.Items.Count == 0)
        {
            Subject.Reply(source, "You do not have any recipes learned.","enchanting_initial");
        }
    }

    //Show the players a dialog asking if they want to use the ingredients, yes or no
    private void OnDisplayingConfirmation(Aisling source)
    {
        if (!TryFetchArg<byte>(1, out var slot) || !source.Inventory.TryGetObject(slot, out var item))
        {
            Subject.ReplyToUnknownInput(source);
            return;
        }
        
        if (Subject.Context is not CraftingRequirements.Recipe recipe)
        {
            Subject.Reply(source, "Something went wrong with the recipe.");
            return;
        }

        if (recipe.Level > source.StatSheet.Level)
        {
            // If the player doesn't meet the requirement, close the menu and display an error message
            Subject.Close(source);
            source.SendOrangeBarMessage($"Level: {recipe.Level} required to craft or upgrade.");
            return;
        }

        var unused = source.Legend.TryGetValue(LEGENDMARK_KEY, out var existingMark);

        if (existingMark is not null)
        {
            var recipeStatus = GetStatusAsInt(recipe.Rank);
            var playerRank = GetRankAsInt(existingMark.Text);

            if (playerRank < recipeStatus)
            {
                Subject.Close(source);
                source.SendOrangeBarMessage($"Crafting rank: {recipe.Rank} required.");

                return;
            }
        }

        // If the player meets the requirement, create a list of ingredient names and amounts
        var ingredientList = new List<string>();

        foreach (var regeant in recipe.Ingredients)
        {
            ingredientList.Add($"({regeant.Amount}) {regeant.DisplayName}");
        }

        // Join the ingredient list into a single string and inject it into the confirmation message
        var ingredients = string.Join(" and ", ingredientList);
        Subject.InjectTextParameters(recipe.Name, item.DisplayName, ingredients);
    }

    private void OnDisplayingAccepted(Aisling source)
    {
        if (!TryFetchArg<byte>(1, out var slot) || !source.Inventory.TryGetObject(slot, out var item))
        {
            Subject.ReplyToUnknownInput(source);
            return;
        }

        if (Subject.Context is not CraftingRequirements.Recipe recipe)
        {
            Subject.Reply(source, "Something went wrong with the recipe.");
            return;
        }


        foreach (var reagant in recipe.Ingredients)
        {
            if (!source.Inventory.HasCount(reagant.DisplayName, reagant.Amount))
            {
                Subject.Close(source);

                source.SendOrangeBarMessage(
                    $"You do not have the required amount ({reagant.Amount}) of {reagant.DisplayName}.");

                return;
            }
        }

        var unused = source.Legend.TryGetValue(LEGENDMARK_KEY, out var existingMark);
        var legendMarkCount = existingMark?.Count ?? 0;

        var timesCraftedThisItem =
            source.Trackers.Counters.TryGetValue(ITEM_COUNTER_PREFIX + recipe.Name, out var value) ? value : 0;

        foreach (var removeRegant in recipe.Ingredients)
        {
            source.Inventory.RemoveQuantity(removeRegant.DisplayName, removeRegant.Amount);
        }

        if (!IntegerRandomizer.RollChance(
                (int)CalculateSuccessRate(
                    legendMarkCount,
                    timesCraftedThisItem,
                    BASE_SUCCESS_RATE,
                    GetStatusAsInt(recipe.Rank))))
        {
            Subject.Close(source);
            var dialog = DialogFactory.Create("enchanting_failed", Subject.DialogSource);
            dialog.MenuArgs = Subject.MenuArgs;
            dialog.Context = Subject.Context;
            dialog.InjectTextParameters(recipe.Name, item.DisplayName);
            dialog.Display(source);
            source.Animate(FailAnimation);
            return;
        }

        source.Trackers.Counters.AddOrIncrement(ITEM_COUNTER_PREFIX + recipe.Name);

        if (existingMark is null)
        {
            UpdateLegendmark(source, legendMarkCount);
        }

        if (existingMark is not null)
        {
            var recipeStatus = GetStatusAsInt(recipe.Rank);
            var playerRank = GetRankAsInt(existingMark.Text);

            if (playerRank >= 2)
            {
                if ((playerRank - 1) > (recipeStatus))
                {
                    source.SendOrangeBarMessage("You can no longer gain experience from this recipe.");
                }
            }

            if (playerRank <= (recipeStatus + 1))
            {
                UpdateLegendmark(source, legendMarkCount);
            }

            if (playerRank == recipeStatus)
            {
                UpdateLegendmark(source, legendMarkCount);
            }
        }
        
        if (recipe.Modification is not null) 
            source.Inventory.Update(item.Slot, recipe.Modification);
        
        Subject.InjectTextParameters(recipe.Name);

        source.Animate(SuccessAnimation);
    }
}
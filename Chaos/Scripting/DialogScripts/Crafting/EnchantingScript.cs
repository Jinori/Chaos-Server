using System.Text.RegularExpressions;
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
using Chaos.TypeMapper.Abstractions;

namespace Chaos.Scripting.DialogScripts.Crafting;

public class EnchantingScript : DialogScriptBase
{
    //Constants to change based on crafting profession
    private const string ITEM_COUNTER_PREFIX = "[Enchant]";
    private const string LEGENDMARK_KEY = "ench";
    private const double BASE_SUCCESS_RATE = 60;
    private const double SUCCESSRATEMAX = 95;
    //Ranks from lowest to highest
    private const string RANK_ONE_TITLE = "Beginner Enchanter";
    private const string RANK_TWO_TITLE = "Novice Enchanter";
    private const string RANK_THREE_TITLE = "Initiate Enchanter";
    private const string RANK_FOUR_TITLE = "Artisan Enchanter";
    private const string RANK_FIVE_TITLE = "Adept Enchanter";
    private const string RANK_SIX_TITLE = "Advanced Enchanter";
    private const string RANK_SEVEN_TITLE = "Expert Enchanter";
    private const string RANK_EIGHT_TITLE = "Master Enchanter";
    private const string RECIPE_ONE_RANK = "Beginner";
    private const string RECIPE_TWO_RANK = "Basic";
    private const string RECIPE_THREE_RANK = "Initiate";
    private const string RECIPE_FOUR_RANK = "Artisan";
    private const string RECIPE_FIVE_RANK = "Adept";
    private const string RECIPE_SIX_RANK = "Advanced";
    private const string RECIPE_SEVEN_RANK = "Expert";
    private const string RECIPE_EIGHT_RANK = "Master";
    private readonly IDialogFactory DialogFactory;
    private readonly IItemFactory ItemFactory;
    private readonly ITypeMapper Mapper;

    private readonly string[] Prefix =
    {
        "Swift",
        "Skillful",
        "Mystical",
        "Shrouded",
        "Meager",
        "Lucky",
        "Mighty",
        "Breezy",
        "Minor",
        "Modest",
        "Tiny",
        "Darkened",
        "Serene",
        "Airy",
        "Valiant",
        "Soft",
        "Hasty",
        "Hale",
        "Brilliant",
        "Nimble",
        "Wise",
        "Focused",
        "Tough",
        "Hazy",
        "Crippling",
        "Powerful",
        "Bright",
        "Tight",
        "Potent",
        "Precision",
        "Savage",
        "Whirling",
        "Ruthless",
        "Eternal",
        "Ancient",
        "Cursed",
        "Soothing",
        "Persisting",
        "Blazing",
        "Howling"
    };

    /// <inheritdoc />
    public EnchantingScript(
        Dialog subject,
        IItemFactory itemFactory,
        IDialogFactory dialogFactory,
        ITypeMapper mapper
    )
        : base(subject)
    {
        ItemFactory = itemFactory;
        DialogFactory = dialogFactory;
        Mapper = mapper;
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
            case "enchanting_confirmdisenchant":
            {
                OnDisplayingDisenchant(source);

                break;
            }
        }
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
            Subject.Reply(source, "Notify a GM that this recipe is missing.");

            return;
        }

        var hasAllIngredients = true;

        foreach (var reagant in recipe.Ingredients)
            if (!source.Inventory.HasCount(reagant.DisplayName, reagant.Amount))
            {
                hasAllIngredients = false;

                source.SendOrangeBarMessage($"You are missing ({reagant.Amount}) of {reagant.DisplayName}.");
            }

        if (!hasAllIngredients)
        {
            Subject.Close(source);

            return;
        }

        var unused = source.Legend.TryGetValue(LEGENDMARK_KEY, out var existingMark);
        var legendMarkCount = existingMark?.Count ?? 0;

        var timesCraftedThisItem =
            source.Trackers.Counters.TryGetValue(ITEM_COUNTER_PREFIX + recipe.Name, out var value) ? value : 0;

        foreach (var removeRegant in recipe.Ingredients)
            source.Inventory.RemoveQuantity(removeRegant.DisplayName, removeRegant.Amount);

        if (!IntegerRandomizer.RollChance(
                (int)CalculateSuccessRate(
                    legendMarkCount,
                    timesCraftedThisItem,
                    BASE_SUCCESS_RATE,
                    GetStatusAsInt(recipe.Rank),
                    recipe.Difficulty)))
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
            UpdateLegendmark(source, legendMarkCount);

        if (existingMark is not null)
        {
            var recipeStatus = GetStatusAsInt(recipe.Rank);
            var playerRank = GetRankAsInt(existingMark.Text);

            if ((playerRank >= 2) && (playerRank - 1 > recipeStatus))
                source.SendOrangeBarMessage("You can no longer gain experience from this recipe.");

            if ((playerRank >= recipeStatus) && (playerRank <= recipeStatus + 1))
            {
                UpdateLegendmark(source, legendMarkCount);

                if (playerRank == recipeStatus)
                    UpdateLegendmark(source, legendMarkCount);
            }
        }

        if (recipe.Modification is not null)
        {
            recipe.Modification(item);
            source.Inventory.Update(item.Slot);
        }

        Subject.InjectTextParameters(recipe.Name);

        source.Animate(SuccessAnimation);
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
            Subject.Reply(source, "Notify a GM that this recipe is missing.");

            return;
        }

        if (Prefix.Any(prefix => item.DisplayName.StartsWith(prefix + " ", StringComparison.OrdinalIgnoreCase)))
        {
            Subject.Close(source);
            var dialog = DialogFactory.Create("enchanting_disenchant", Subject.DialogSource);
            dialog.MenuArgs = Subject.MenuArgs;
            dialog.Context = Subject.Context;
            dialog.InjectTextParameters(item.DisplayName);
            dialog.Display(source);

            return;
        }

        // If the player meets the requirement, create a list of ingredient names and amounts
        var ingredientList = new List<string>();

        foreach (var regeant in recipe.Ingredients)
            ingredientList.Add($"({regeant.Amount}) {regeant.DisplayName}");

        // Join the ingredient list into a single string and inject it into the confirmation message
        var ingredients = string.Join(" and ", ingredientList);
        Subject.InjectTextParameters(recipe.Name, item.DisplayName, ingredients);
    }

    private void OnDisplayingDisenchant(Aisling source)
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
        
        //Create new item based on the old one
        var newItem = ItemFactory.Create(item.Template.TemplateKey);
        //Remove Old Item
        source.Inventory.Remove(item.DisplayName);

        source.Inventory.TryAddDirect(slot, newItem);
        
        Subject.InjectTextParameters(item.DisplayName);
    }

    //ShowItems in a Shop Window to the player
    private void OnDisplayingShowItems(Aisling source)
    {
        if (source.IsAdmin)
            foreach (var recipe in CraftingRequirements.EnchantingRequirements)
            {
                var item = ItemFactory.CreateFaux(recipe.Value.TemplateKey);
                Subject.Items.Add(ItemDetails.DisplayRecipe(item));
            }
        else
        {
            var unused = source.Legend.TryGetValue(LEGENDMARK_KEY, out var existingMark);

            if (existingMark == null)
                UpdateLegendmark(source, 0);

            if (existingMark != null)
            {
                var playerRank = GetRankAsInt(existingMark.Text);

                if (source.Trackers.Flags.TryGetFlag(out EnchantingRecipes recipes))
                    foreach (var recipe in CraftingRequirements.EnchantingRequirements)
                        if (recipes.HasFlag(recipe.Key) && (playerRank >= GetStatusAsInt(recipe.Value.Rank)))
                        {
                            var item = ItemFactory.CreateFaux(recipe.Value.TemplateKey);

                            if (source.UserStatSheet.Level >= item.Level)
                                Subject.Items.Add(ItemDetails.DisplayRecipe(item));
                        }
            }

            if (Subject.Items.Count == 0)
                Subject.Reply(
                    source,
                    "You do not have any recipes to craft. Check your recipe book (F1 Menu) to see your recipes and their requirements.",
                    "enchanting_initial");
        }
    }

    private void OnDisplayingShowPlayerItems(Aisling source)
    {
        if (!TryFetchArg<string>(0, out var selected))
        {
            Subject.ReplyToUnknownInput(source);

            return;
        }

        var correctRecipe = Regex.Replace(selected, @"\s+|'s|'", "");

        if (!Enum.TryParse<EnchantingRecipes>(correctRecipe, out var selectedRecipeEnum))
        {
            Subject.Reply(source, "Recipe could not be found in Enchanting.");

            return;
        }

        var selectedRecipe = CraftingRequirements.EnchantingRequirements
                                                 .FirstOrDefault(x => x.Key == selectedRecipeEnum)
                                                 .Value;

        if (selectedRecipe == null)
        {
            Subject.Reply(source, "Something went wrong with the selected recipe.");

            return;
        }

        Subject.Context = selectedRecipe;
        Subject.InjectTextParameters(selectedRecipe.Name);

        var modifiableItems = source.Inventory.Where(x => x.Template.IsModifiable).ToList();

        if (modifiableItems.Count == 0)
        {
            Subject.Reply(source, "You don't have anything in your inventory to enchant.", "Close");

            return;
        }

        Subject.Slots = modifiableItems.Select(x => x.Slot).ToList();
    }

    #region Methods
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
        int recipeRank,
        int difficulty
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
        var successRate = (baseSuccessRate - rankDifficultyReduction - difficulty + timesCraftedThisItem / 10.0) * multiplier;

        // Ensure the success rate does not exceed the maximum allowed value
        return Math.Min(successRate, SUCCESSRATEMAX);
    }

    private void UpdateLegendmark(Aisling source, int legendMarkCount)
    {
        if (!source.Legend.TryGetValue(LEGENDMARK_KEY, out var existingMark))
            source.Legend.AddOrAccumulate(
                new LegendMark(
                    RANK_ONE_TITLE,
                    LEGENDMARK_KEY,
                    MarkIcon.Yay,
                    MarkColor.White,
                    1,
                    GameTime.Now));
        else
        {
            var rankThresholds = new[]
            {
                25, 75, 150, 300, 500, 1000, 1500
            };

            var rankTitles = new[]
            {
                RANK_TWO_TITLE, RANK_THREE_TITLE, RANK_FOUR_TITLE, RANK_FIVE_TITLE, RANK_SIX_TITLE,
                RANK_SEVEN_TITLE, RANK_EIGHT_TITLE
            };

            var currentRankIndex = Array.IndexOf(rankTitles, existingMark.Text);

            existingMark.Count++;

            for (var i = currentRankIndex + 1; i < rankThresholds.Length; i++)
                if (legendMarkCount >= rankThresholds[i])
                {
                    existingMark.Text = rankTitles[i];
                    source.SendOrangeBarMessage($"You have reached the rank of {rankTitles[i]}");
                    source.Titles.Add(rankTitles[i]);
                    var first = source.Titles.First();

                    if (source.Titles.First() is "")
                    {
                        source.Titles.Remove(first);
                        source.Titles.Add(first);
                        source.Client.SendSelfProfile();
                    }

                    break;
                }
        }
    }
    #endregion Methods
}
using System.Text.RegularExpressions;
using Chaos.Common.Utilities;
using Chaos.DarkAges.Definitions;
using Chaos.Definitions;
using Chaos.Extensions;
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
    // Constants to change based on crafting profession
    private const string ITEM_COUNTER_PREFIX = "[Enchant]";
    private const string LEGENDMARK_KEY = "ench";
    private const double BASE_SUCCESS_RATE = 60;
    private const double SUCCESSRATEMAX = 95;

    // Ranks from lowest to highest
    private const string RANK_ONE_TITLE = "Beginner Enchanter";
    private const string RANK_TWO_TITLE = "Novice Enchanter";
    private const string RANK_THREE_TITLE = "Initiate Enchanter";
    private const string RANK_FOUR_TITLE = "Artisan Enchanter";
    private const string RANK_FIVE_TITLE = "Adept Enchanter";
    private const string RANK_SIX_TITLE = "Advanced Enchanter";
    private const string RANK_SEVEN_TITLE = "Expert Enchanter";
    private const string RANK_EIGHT_TITLE = "Master Enchanter";

    // The rank strings used in CraftingRequirements.Recipe
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

    // ---------- Animations ----------
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

    /// <inheritdoc />
    public EnchantingScript(Dialog subject, IItemFactory itemFactory, IDialogFactory dialogFactory)
        : base(subject)
    {
        ItemFactory = itemFactory;
        DialogFactory = dialogFactory;
    }

    private double CalculateSuccessRate(
        int totalTimesCrafted,
        int timesCraftedThisItem,
        double baseSuccessRate,
        int recipeRank,
        int difficulty)
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

        var multiplier = GetMultiplier(totalTimesCrafted);
        var successRate = (baseSuccessRate - rankDifficultyReduction - difficulty + timesCraftedThisItem / 5.0) * multiplier;

        return Math.Min(successRate, SUCCESSRATEMAX);
    }

    private double GetMultiplier(int totalTimesCrafted)
        => totalTimesCrafted switch
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

    // ---------- Rank & success logic ----------
    public int GetRankAsInt(string rank)
    {
        var rankMappings = new Dictionary<string, int>
        {
            {
                RANK_EIGHT_TITLE, 8
            },
            {
                RANK_SEVEN_TITLE, 7
            },
            {
                RANK_SIX_TITLE, 6
            },
            {
                RANK_FIVE_TITLE, 5
            },
            {
                RANK_FOUR_TITLE, 4
            },
            {
                RANK_THREE_TITLE, 3
            },
            {
                RANK_TWO_TITLE, 2
            },
            {
                RANK_ONE_TITLE, 1
            }
        };

        if (rankMappings.TryGetValue(rank, out var i))
            return i;

        return -1;
    }

    private int GetStatusAsInt(string status)
    {
        var statusMappings = new Dictionary<string, int>
        {
            {
                RECIPE_ONE_RANK, 1
            },
            {
                RECIPE_TWO_RANK, 2
            },
            {
                RECIPE_THREE_RANK, 3
            },
            {
                RECIPE_FOUR_RANK, 4
            },
            {
                RECIPE_FIVE_RANK, 5
            },
            {
                RECIPE_SIX_RANK, 6
            },
            {
                RECIPE_SEVEN_RANK, 7
            },
            {
                RECIPE_EIGHT_RANK, 8
            }
        };

        return statusMappings.TryGetValue(status, out var i) ? i : 0;
    }

    private void GiveRankReward(Aisling source, string title)
    {
        const string ITEM_KEY = "portaltrinket";
        const string REWARD_LEGEND_KEY = "enchantingtrinket";

        if (!source.Legend.ContainsKey(REWARD_LEGEND_KEY))
        {
            var rewardLegendMark = new LegendMark(
                "Obtained the Legendary Geata Chugam",
                REWARD_LEGEND_KEY,
                MarkIcon.Victory,
                MarkColor.Yellow,
                1,
                GameTime.Now);

            source.Legend.AddOrAccumulate(rewardLegendMark);

            var trinket = ItemFactory.Create(ITEM_KEY);
            source.GiveItemOrSendToBank(trinket);
            source.SendOrangeBarMessage($"You have received Geata Chugam for reaching {title}!");
        }
    }

    /// <inheritdoc />
    public override void OnDisplaying(Aisling source)
    {
        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "enchanting_initial":
            {
                // Show all possible enchanting recipes (based on rank/flags).
                OnDisplayingShowRecipes(source);

                break;
            }
            case "enchanting_confirmation":
            {
                // Confirm that the user wants to craft this scroll (consume reagents).
                OnDisplayingConfirmation(source);

                break;
            }
            case "enchanting_accepted":
            {
                // Attempt to craft the enchanted scroll
                OnDisplayingCraftScroll(source);

                break;
            }
        }
    }

    // --------------------------------------------------------------------------
    // 2) CONFIRMATION => user has selected a recipe, show reagents & confirm
    // --------------------------------------------------------------------------
    private void OnDisplayingConfirmation(Aisling source)
    {
        // The selected recipe name or ID is in Arg(0)
        if (!TryFetchArg<string>(0, out var selected))
        {
            Subject.ReplyToUnknownInput(source);

            return;
        }

        // Clean up string => match an enum
        var correctRecipe = Regex.Replace(selected, @"\s+|'s|'", "");

        if (!Enum.TryParse<EnchantingRecipes>(correctRecipe, out var selectedRecipeEnum))
        {
            Subject.Reply(source, "Recipe could not be found in Enchanting.");

            return;
        }

        // Get the actual recipe from CraftingRequirements
        var recipe = CraftingRequirements.EnchantingRequirements.FirstOrDefault(x => x.Key == selectedRecipeEnum)
                                         .Value;

        if (recipe == null)
        {
            Subject.Reply(source, "Something went wrong with the selected recipe.");

            return;
        }

        // Place this recipe in Subject.Context for next step
        Subject.Context = recipe;

        // Build a string of required reagents: e.g. "(2) Essence of Fire and (1) Unicorn Hair"
        var ingredientStrings = recipe.Ingredients
                                      .Select(i => $"({i.Amount}) {i.DisplayName}")
                                      .ToList();

        var ingredients = string.Join(" and ", ingredientStrings);

        // Show the recipe name and the reagent list
        Subject.InjectTextParameters(recipe.Prefix, ingredients);
    }

    // -----------------------------------------------------------------------------
    // 3) CRAFTSCROLL => consumes reagents, calculates success, and creates scroll
    // -----------------------------------------------------------------------------
    private void OnDisplayingCraftScroll(Aisling source)
    {
        // We expect the recipe from Subject.Context
        if (Subject.Context is not CraftingRequirements.Recipe recipe)
        {
            Subject.Reply(source, "No recipe context found. Notify a GM.");

            return;
        }

        // Check if user has all required reagents
        var hasAllIngredients = true;

        foreach (var reagent in recipe.Ingredients)
            if (!source.Inventory.HasCount(reagent.DisplayName, reagent.Amount))
            {
                hasAllIngredients = false;
                source.SendOrangeBarMessage($"You are missing ({reagent.Amount}) of {reagent.DisplayName}.");
            }

        if (!hasAllIngredients)
        {
            Subject.Close(source);

            return;
        }

        // Legend / counters for success chance
        var hasExistingMark = source.Legend.TryGetValue(LEGENDMARK_KEY, out var existingMark);
        var legendMarkCount = existingMark?.Count ?? 0;

        var timesCraftedThisItem = source.Trackers.Counters.TryGetValue(ITEM_COUNTER_PREFIX + recipe.Name, out var value) ? value : 0;

        // Remove reagents
        foreach (var reagent in recipe.Ingredients)
            source.Inventory.RemoveQuantity(reagent.DisplayName, reagent.Amount);

        var adjustedSuccessRate = AdjustSuccessRateForEffects(BASE_SUCCESS_RATE, source);
        
        // Calculate success chance
        var successChance = (int)CalculateSuccessRate(
            legendMarkCount,
            timesCraftedThisItem,
            adjustedSuccessRate,
            GetStatusAsInt(recipe.Rank),
            recipe.Difficulty);

        // Roll
        if (!IntegerRandomizer.RollChance(successChance))
        {
            // Failure
            Subject.Close(source);

            var failDialog = DialogFactory.Create("enchanting_failed", Subject.DialogSource);
            failDialog.MenuArgs = Subject.MenuArgs;
            failDialog.Context = recipe; // keep context if needed
            failDialog.InjectTextParameters(recipe.Prefix);
            failDialog.Display(source);

            source.Animate(FailAnimation);

            // Even if fail, increment craft count
            source.Trackers.Counters.AddOrIncrement(ITEM_COUNTER_PREFIX + recipe.Name);

            return;
        }

        // Success
        source.Trackers.Counters.AddOrIncrement(ITEM_COUNTER_PREFIX + recipe.Name);

        // Rank ups
        if (!hasExistingMark)
            UpdateLegendmark(source);

        if (existingMark is not null)
        {
            var recipeStatus = GetStatusAsInt(recipe.Rank);
            var playerRank = GetRankAsInt(existingMark.Text);

            // If player's rank is higher than recipe, no new xp
            if ((playerRank >= 2) && ((playerRank - 1) > recipeStatus))
                source.SendOrangeBarMessage("You can no longer gain rank experience from this recipe.");

            if ((playerRank >= recipeStatus) && (playerRank <= (recipeStatus + 1)))
            {
                UpdateLegendmark(source);

                if (playerRank == recipeStatus)
                    UpdateLegendmark(source);
            }
        }

        // Create the blank scroll, apply recipe's modification
        var scroll = ItemFactory.Create("enchantingscroll");
        recipe.Modification?.Invoke(scroll);

        // Give item
        source.GiveItemOrSendToBank(scroll);

        if (source.Inventory.Contains(scroll))
            source.Inventory.Update(scroll.Slot);

        // Show success message
        Subject.InjectTextParameters(recipe.Prefix);
        source.Animate(SuccessAnimation);
    }

    // ------------------------------------------------------
    // 1) SHOWRECIPES => similar to old OnDisplayingShowItems
    // ------------------------------------------------------
    private void OnDisplayingShowRecipes(Aisling source)
    {
        var sortedRecipes = new List<ItemDetails>();

        // If GodMode, display all possible recipes sorted by rank
        if (source.IsGodModeEnabled())
        {
            sortedRecipes = CraftingRequirements.EnchantingRequirements
                                                .Select(
                                                    recipe => ItemDetails.DisplayRecipe(ItemFactory.CreateFaux(recipe.Value.TemplateKey)))
                                                .OrderBy(
                                                    recipe => GetStatusAsInt(
                                                        CraftingRequirements.EnchantingRequirements.First(
                                                                                x => x.Value.TemplateKey
                                                                                     == recipe.Item.Template.TemplateKey)
                                                                            .Value.Rank))
                                                .ToList();
        }
        else
        {
            // Get the user's legend mark for enchanting
            var hasLegend = source.Legend.TryGetValue(LEGENDMARK_KEY, out var existingMark);

            // If no mark, set them at rank 1
            if (!hasLegend)
                UpdateLegendmark(source);

            if (existingMark is not null)
            {
                // Determine player's enchanting rank
                var playerRank = GetRankAsInt(existingMark.Text);

                // If the user has any recipe flags, show matching recipes
                if (source.Trackers.Flags.TryGetFlag(out EnchantingRecipes recipes))
                {
                    sortedRecipes = CraftingRequirements.EnchantingRequirements
                                                        .Where(kv => recipes.HasFlag(kv.Key) && playerRank >= GetStatusAsInt(kv.Value.Rank))
                                                        .Select(
                                                            kv => ItemDetails.DisplayRecipe(ItemFactory.CreateFaux(kv.Value.TemplateKey)))
                                                        .Where(item => source.UserStatSheet.Level >= item.Item.Level)
                                                        .OrderBy(
                                                            item => GetStatusAsInt(
                                                                CraftingRequirements.EnchantingRequirements.First(
                                                                                        x => x.Value.TemplateKey
                                                                                            == item.Item.Template.TemplateKey)
                                                                                    .Value.Rank))
                                                        .ToList();
                }
            }
        }

        // Assign sorted items
        Subject.Items = sortedRecipes;

        // If no items were added, notify the user
        if (Subject.Items.Count == 0)
            Subject.Reply(source, "You do not have any recipes to craft. Check your recipe book (F1 Menu) for requirements.", "Close");
    }


    private void UpdateLegendmark(Aisling source)
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
                25,
                75,
                150,
                300,
                500,
                1000,
                1500
            };

            var rankTitles = new[]
            {
                RANK_TWO_TITLE,
                RANK_THREE_TITLE,
                RANK_FOUR_TITLE,
                RANK_FIVE_TITLE,
                RANK_SIX_TITLE,
                RANK_SEVEN_TITLE,
                RANK_EIGHT_TITLE
            };

            var currentRankIndex = Array.IndexOf(rankTitles, existingMark.Text.Trim());

            if (currentRankIndex == -1)
                currentRankIndex = 0;

            existingMark.Count++;
            var newCount = existingMark.Count;

            for (var i = 0; i < rankThresholds.Length; i++)
                if (newCount >= rankThresholds[i])
                {
                    var newTitle = rankTitles[i];

                    if (i == (currentRankIndex + 1))
                        UpdatePlayerTitle(source, existingMark, newTitle);

                    if ((i >= 5) && !source.Legend.ContainsKey("enchantingtrinket"))
                        GiveRankReward(source, newTitle);
                }
        }
    }

    private void UpdatePlayerTitle(Aisling source, LegendMark existingMark, string newTitle)
    {
        if (source.Titles.Contains(existingMark.Text))
            source.Titles.Remove(existingMark.Text);

        source.Titles.Add(newTitle);
        existingMark.Text = newTitle;
        source.SendOrangeBarMessage($"You have reached the rank of {newTitle}");

        // Move the new title to the front of the list
        if (source.Titles.Any())
        {
            source.Titles.Remove(newTitle);
            source.Titles.Insert(0, newTitle);
        }

        source.Client.SendSelfProfile();
    }
    
    private double AdjustSuccessRateForEffects(double baseSuccessRate, Aisling source)
    {
        if (source.Effects.Contains("Miracle"))
            return baseSuccessRate + 15.0;

        return baseSuccessRate;
    }
}
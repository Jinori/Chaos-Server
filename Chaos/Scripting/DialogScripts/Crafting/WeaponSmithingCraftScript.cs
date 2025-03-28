using Chaos.Common.Utilities;
using Chaos.DarkAges.Definitions;
using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Extensions.Common;
using Chaos.Models.Data;
using Chaos.Models.Legend;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Time;

namespace Chaos.Scripting.DialogScripts.Crafting;

public class WeaponSmithingCraftScript : DialogScriptBase
{
    //Constants to change based on crafting profession
    private const string ITEM_COUNTER_PREFIX = "[Weaponsmithing]";
    private const string LEGENDMARK_KEY = "wpnsmth";
    private const double BASE_SUCCESS_RATE = 60;

    private const double SUCCESSRATEMAX = 95;

    //Ranks from lowest to highest
    private const string RANK_ONE_TITLE = "Beginner Weaponsmith";
    private const string RANK_TWO_TITLE = "Novice Weaponsmith";
    private const string RANK_THREE_TITLE = "Initiate Weaponsmith";
    private const string RANK_FOUR_TITLE = "Artisan Weaponsmith";
    private const string RANK_FIVE_TITLE = "Adept Weaponsmith";
    private const string RANK_SIX_TITLE = "Advanced Weaponsmith";
    private const string RANK_SEVEN_TITLE = "Expert Weaponsmith";

    private const string RANK_EIGHT_TITLE = "Master Weaponsmith";

    //Set this to true if doing armorsmithing type crafting
    private readonly bool Craftgoodgreatgrand = false;
    private readonly IDialogFactory DialogFactory;
    private readonly IItemFactory ItemFactory;

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
    public WeaponSmithingCraftScript(Dialog subject, IItemFactory itemFactory, IDialogFactory dialogFactory)
        : base(subject)
    {
        ItemFactory = itemFactory;
        DialogFactory = dialogFactory;
    }

    // Calculates the success rate of crafting an item
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

        // Get the multiplier based on total times crafted
        var multiplier = GetMultiplier(totalTimesCrafted);

        // Calculate the success rate with all the factors
        var successRate = (baseSuccessRate - rankDifficultyReduction - difficulty + timesCraftedThisItem / 5.0) * multiplier;

        // Ensure the success rate does not exceed the maximum allowed value
        return Math.Min(successRate, SUCCESSRATEMAX);
    }

    private CraftingRequirements.Recipe? FindRecipeByName(string recipeName)
    {
        // First, check in WeaponsmithingRequirements
        var recipe = CraftingRequirements.WeaponSmithingCraftRequirements.Values.FirstOrDefault(r => r.Name.EqualsI(recipeName));

        // If not found, check in WeaponsmithingRequirements2
        if (recipe is null)
            recipe = CraftingRequirements.WeaponSmithingCraftRequirements2.Values.FirstOrDefault(r => r.Name.EqualsI(recipeName));

        return recipe;
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

    // Converts the rank string to an integer value
    private int GetRankAsInt(string rank)
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

    // Converts the status string to an integer value
    private int GetStatusAsInt(string status)
    {
        var statusMappings = new Dictionary<string, int>
        {
            {
                "Beginner", 1
            },
            {
                "Basic", 2
            },
            {
                "Initiate", 3
            },
            {
                "Artisan", 4
            },
            {
                "Adept", 5
            },
            {
                "Advanced", 6
            },
            {
                "Expert", 7
            },
            {
                "Master", 8
            }
        };

        if (statusMappings.TryGetValue(status, out var i))
            return i;

        return 0;
    }

    /// <summary>
    ///     Gives the player a reward when reaching Rank 7 or above.
    /// </summary>
    private void GiveRankReward(Aisling source, string title)
    {
        const string ITEM_KEY = "dmgtrinket";
        const string REWARD_LEGEND_KEY = "weaponsmithtrinket";

        if (!source.Legend.ContainsKey(REWARD_LEGEND_KEY))
        {
            var rewardLegendMark = new LegendMark(
                "Obtained the legendary Claíomh Thugann Anam",
                REWARD_LEGEND_KEY,
                MarkIcon.Victory,
                MarkColor.Yellow,
                1,
                GameTime.Now);

            source.Legend.AddOrAccumulate(rewardLegendMark);
            var trinket = ItemFactory.Create(ITEM_KEY);
            source.GiveItemOrSendToBank(trinket);
            source.SendOrangeBarMessage($"You have received Claíomh Thugann Anam for reaching {title}!");
        }
    }

    /// <inheritdoc />
    public override void OnDisplaying(Aisling source)
    {
        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "weaponsmithing_craftinitial":
            {
                OnDisplayingShowItems(source);

                break;
            }
            case "weaponsmithing_craftconfirmation":
            {
                OnDisplayingConfirmation(source);

                break;
            }
            case "weaponsmithing_craftaccepted":
            {
                OnDisplayingAccepted(source);

                break;
            }
        }
    }

    private void OnDisplayingAccepted(Aisling source)
    {
        if (!TryFetchArgs<string>(out var selectedRecipeName))
        {
            Subject.ReplyToUnknownInput(source);

            return;
        }

        var recipe = FindRecipeByName(selectedRecipeName);

        if (recipe is null)
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

        var timesCraftedThisItem = source.Trackers.Counters.TryGetValue(ITEM_COUNTER_PREFIX + recipe.Name, out var value) ? value : 0;

        foreach (var removeRegant in recipe.Ingredients)
            source.Inventory.RemoveQuantity(removeRegant.DisplayName, removeRegant.Amount);

        var adjustedSuccessRate = AdjustSuccessRateForEffects(BASE_SUCCESS_RATE, source);
        
        if (!IntegerRandomizer.RollChance(
                (int)CalculateSuccessRate(
                    legendMarkCount,
                    timesCraftedThisItem,
                    adjustedSuccessRate,
                    GetStatusAsInt(recipe.Rank),
                    recipe.Difficulty)))
        {
            Subject.Close(source);
            var dialog = DialogFactory.Create("weaponsmithing_craftFailed", Subject.DialogSource);
            dialog.MenuArgs = Subject.MenuArgs;

            dialog.InjectTextParameters(recipe.Name);
            dialog.Display(source);
            source.Animate(FailAnimation);

            //Give item counter exp even if failure
            source.Trackers.Counters.AddOrIncrement(ITEM_COUNTER_PREFIX + recipe.Name);

            return;
        }

        source.Trackers.Counters.AddOrIncrement(ITEM_COUNTER_PREFIX + recipe.Name);

        if (existingMark is null)
            UpdateLegendmark(source, legendMarkCount);

        if (existingMark is not null)
        {
            var recipeStatus = GetStatusAsInt(recipe.Rank);
            var playerRank = GetRankAsInt(existingMark.Text);

            if ((playerRank >= 2) && ((playerRank - 1) > recipeStatus))
                source.SendOrangeBarMessage("You can no longer gain rank experience from this recipe.");

            if ((playerRank >= recipeStatus) && (playerRank <= (recipeStatus + 1)))
            {
                UpdateLegendmark(source, legendMarkCount);

                if (playerRank == recipeStatus)
                    UpdateLegendmark(source, legendMarkCount);
            }
        }

        if (Craftgoodgreatgrand)
        {
            var roll = IntegerRandomizer.RollSingle(100);

            var newCraft = roll switch
            {
                < 10  => ItemFactory.Create("grand" + recipe.TemplateKey),
                < 40  => ItemFactory.Create("great" + recipe.TemplateKey),
                < 100 => ItemFactory.Create("good" + recipe.TemplateKey),
                _     => ItemFactory.Create(recipe.TemplateKey)
            };

            source.GiveItemOrSendToBank(newCraft);

            Subject.InjectTextParameters(newCraft.DisplayName);
        } else
        {
            var newCraft = ItemFactory.Create(recipe.TemplateKey);

            source.GiveItemOrSendToBank(newCraft);

            Subject.InjectTextParameters(newCraft.DisplayName);
        }

        source.Animate(SuccessAnimation);
    }

    //Show the players a dialog asking if they want to use the ingredients, yes or no
    private void OnDisplayingConfirmation(Aisling source)
    {
        if (!TryFetchArgs<string>(out var selectedRecipeName))
        {
            Subject.ReplyToUnknownInput(source);

            return;
        }

        var recipe = FindRecipeByName(selectedRecipeName);

        if (recipe is null)
        {
            Subject.Reply(source, "Notify a GM that this recipe is missing.");

            return;
        }

        // If the player meets the requirement, create a list of ingredient names and amounts
        var ingredientList = new List<string>();

        foreach (var regeant in recipe.Ingredients)
            ingredientList.Add($"({regeant.Amount}) {regeant.DisplayName}");

        // Join the ingredient list into a single string and inject it into the confirmation message
        var ingredients = string.Join(" and ", ingredientList);
        Subject.InjectTextParameters(recipe.Name, ingredients);
    }

    //ShowItems in a Shop Window to the player
    private void OnDisplayingShowItems(Aisling source)
    {
        if (source.IsGodModeEnabled())
        {
            foreach (var recipe in CraftingRequirements.WeaponSmithingCraftRequirements)
            {
                var item = ItemFactory.CreateFaux(recipe.Value.TemplateKey);
                Subject.Items.Add(ItemDetails.DisplayRecipe(item));
            }

            foreach (var recipe2 in CraftingRequirements.WeaponSmithingCraftRequirements2)
            {
                var item = ItemFactory.CreateFaux(recipe2.Value.TemplateKey);
                Subject.Items.Add(ItemDetails.DisplayRecipe(item));
            }
        } else
        {
            var unused = source.Legend.TryGetValue(LEGENDMARK_KEY, out var existingMark);

            if (existingMark == null)
                UpdateLegendmark(source, 0);

            if (existingMark != null)
            {
                var playerRank = GetRankAsInt(existingMark.Text);

                if (source.Trackers.Flags.TryGetFlag(out WeaponSmithingRecipes recipes))

                    // Show items from WeaponsmithingCraftRequirements
                    foreach (var recipe in CraftingRequirements.WeaponSmithingCraftRequirements)
                        if (recipes.HasFlag(recipe.Key) && (playerRank >= GetStatusAsInt(recipe.Value.Rank)))
                        {
                            var item = ItemFactory.CreateFaux(recipe.Value.TemplateKey);

                            if (source.UserStatSheet.Level >= item.Level)
                                Subject.Items.Add(ItemDetails.DisplayRecipe(item));
                        }

                if (source.Trackers.Flags.TryGetFlag(out WeaponSmithingRecipes2 recipes2))

                    // Show items from WeaponsmithingCraftRequirements
                    foreach (var recipe2 in CraftingRequirements.WeaponSmithingCraftRequirements2)
                        if (recipes2.HasFlag(recipe2.Key) && (playerRank >= GetStatusAsInt(recipe2.Value.Rank)))
                        {
                            var item = ItemFactory.CreateFaux(recipe2.Value.TemplateKey);

                            if (source.UserStatSheet.Level >= item.Level)
                                Subject.Items.Add(ItemDetails.DisplayRecipe(item));
                        }
            }

            if (Subject.Items.Count == 0)
                Subject.Reply(
                    source,
                    "You do not have any recipes to craft. Check your recipe book (F1 Menu) to see your recipes and their requirements.",
                    "weaponsmithing_initial");
        }
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

            // Ensure existingMark.Text aligns with rankTitles
            var currentRankIndex = Array.IndexOf(rankTitles, existingMark.Text.Trim());

            if (currentRankIndex == -1)
                currentRankIndex = 0; // Default to the first rank if the title is not found

            existingMark.Count++;

            var newLegendMarkCount = existingMark.Count;

            // Check all thresholds, including previously achieved ones
            for (var i = 0; i < rankThresholds.Length; i++)

                // Check if the player meets the threshold for this rank
                if (newLegendMarkCount >= rankThresholds[i])
                {
                    var newTitle = rankTitles[i];

                    // Update title if this is the current rank
                    if (i == (currentRankIndex + 1))
                        UpdatePlayerTitle(source, existingMark, newTitle);

                    // Grant rank reward if it hasn't already been granted
                    if ((i >= 5) && !source.Legend.ContainsKey("weaponsmithtrinket"))
                        GiveRankReward(source, newTitle);
                }
        }
    }

    /// <summary>
    ///     Updates the player's title and sends a profile update.
    /// </summary>
    private void UpdatePlayerTitle(Aisling source, LegendMark existingMark, string newTitle)
    {
        if (source.Titles.Contains(existingMark.Text))
            source.Titles.Remove(existingMark.Text);

        source.Titles.Add(newTitle);
        existingMark.Text = newTitle;

        source.SendOrangeBarMessage($"You have reached the rank of {newTitle}");

        // Move the new title to the front
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
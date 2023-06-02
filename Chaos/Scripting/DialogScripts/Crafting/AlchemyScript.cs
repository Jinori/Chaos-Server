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
    private const string LEGENDMARK_KEY = "alch";
    private const double BASE_SUCCESS_RATE = 30;
    private const double NOVICEBOOST = 5;
    private const double SUCCESSRATEMAX = 95;
    //Ranks from lowest to highest
    private const string RANK_ONE_TITLE = "Novice Alchemist";
    private const string RANK_TWO_TITLE = "Apprentice Alchemist";
    private const string RANK_THREE_TITLE = "Journeyman Alchemist";
    private const string RANK_FOUR_TITLE = "Craftsman Alchemist";
    private const string RANK_FIVE_TITLE = "Artisan Alchemist";
    private const string RANK_SIX_TITLE = "Expert Alchemist";
    private const string RANK_SEVEN_TITLE = "Master Alchemist";
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

    private double GetMultiplier(int totalTimesCrafted) =>
        totalTimesCrafted switch
        {
            <= 100  => 1.0,
            <= 500  => 1.05,
            <= 1000 => 1.1,
            <= 1500 => 1.15,
            <= 2200 => 1.2,
            <= 2900 => 1.25,
            _       => 1.3
        };

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
            switch (legendMarkCount)
            {
                case > 2900 when !existingMark.Text.Contains(RANK_SEVEN_TITLE):
                    existingMark.Text = RANK_SEVEN_TITLE;

                    break;
                case > 2200 when !existingMark.Text.Contains(RANK_SIX_TITLE):
                    existingMark.Text = RANK_SIX_TITLE;

                    break;
                case > 1500 when !existingMark.Text.Contains(RANK_FIVE_TITLE):
                    existingMark.Text = RANK_FIVE_TITLE;

                    break;
                case > 1000 when !existingMark.Text.Contains(RANK_FOUR_TITLE):
                    existingMark.Text = RANK_FOUR_TITLE;

                    break;
                case > 500 when !existingMark.Text.Contains(RANK_THREE_TITLE):
                    existingMark.Text = RANK_THREE_TITLE;

                    break;
                case > 100 when !existingMark.Text.Contains(RANK_TWO_TITLE):
                    existingMark.Text = RANK_TWO_TITLE;

                    break;
            }

            existingMark.Count++;
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
                OnDisplayingShowItems(source);

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

    //ShowItems in a Shop Window to the player
    private void OnDisplayingShowItems(Aisling source)
    {
        // Checking if the Alchemy recipe is available or not.
        if (source.Trackers.Flags.TryGetFlag(out AlchemyRecipes recipes))
        {
            // Iterating through the Alchemy recipe requirements.
            foreach (var recipe in CraftingRequirements.AlchemyRequirements)
            {
                // Checking if the recipe is available or not.
                if ((recipes & (AlchemyRecipes)Enum.Parse(typeof(AlchemyRecipes), recipe.Key)) == (AlchemyRecipes)Enum.Parse(typeof(AlchemyRecipes), recipe.Key))
                {
                    // Creating a faux item for the recipe.
                    var item = ItemFactory.CreateFaux(recipe.Key);
                    // Adding the recipe to the subject's dialog window.
                    Subject.Items.Add(ItemDetails.DisplayRecipe(item));
                }
            }
        }
    }

    //Show the players a dialog asking if they want to use the ingredients, yes or no
    private void OnDisplayingConfirmation(Aisling source)
    {
        if (!Subject.MenuArgs.TryGet<string>(0, out var selectedRecipeName))
        {
            // If the name is not found or is empty, display an error message and return
            Subject.Reply(source, DialogString.UnknownInput.Value);
            return;
        }

        if (!Enum.TryParse(selectedRecipeName.Replace(" ", string.Empty), out AlchemyRecipes selectedRecipe))
        {
            // If the name cannot be parsed to the AlchemyRecipes enum, display an error message and return
            Subject.Reply(source, DialogString.UnknownInput.Value);
            return;
        }

        // If the recipe is found, check if the player meets the level requirement
        if (CraftingRequirements.AlchemyRequirements.TryGetValue(selectedRecipe.ToString(), out var requirement))
        {
            if (requirement.Item3 > source.StatSheet.Level)
            {
                // If the player doesn't meet the requirement, close the menu and display an error message
                Subject.Close(source);

                source.SendOrangeBarMessage($"You are not the required Level of {requirement.Item3} for this recipe.");

                return;
            }

            var unused = source.Legend.TryGetValue(LEGENDMARK_KEY, out var existingMark);

            if (existingMark is not null)
            {
                var recipeStatus = GetStatusAsInt(requirement.Item2);
                var playerRank = GetRankAsInt(existingMark.Text);

                if (playerRank < recipeStatus)
                {
                    Subject.Close(source);

                    source.SendOrangeBarMessage(
                        $"You do not have the crafting rank of {requirement.Item2} required for this recipe.");

                    return;
                }
            }

            // If the player meets the requirement, create a list of ingredient names and amounts
            var ingredientList = new List<string>();

            foreach (var regeant in requirement.Item1)
            {
                ingredientList.Add($"({regeant.Amount}) {regeant.DisplayName}");
            }

            // Join the ingredient list into a single string and inject it into the confirmation message
            var ingredients = string.Join(" and ", ingredientList);
            Subject.InjectTextParameters(selectedRecipe, ingredients);
        }
    }

    private void OnDisplayingAccepted(Aisling source)
    {
        if (!Subject.MenuArgs.TryGet<string>(0, out var selectedRecipeName))
        {
            // If the name is not found or is empty, display an error message and return
            Subject.Reply(source, DialogString.UnknownInput.Value);
            return;
        }

        if (!Enum.TryParse(selectedRecipeName.Replace(" ", string.Empty), out AlchemyRecipes selectedRecipe))
        {
            // If the name cannot be parsed to the AlchemyRecipes enum, display an error message and return
            Subject.Reply(source, DialogString.UnknownInput.Value);
            return;
        }

        if (CraftingRequirements.AlchemyRequirements.TryGetValue(selectedRecipe.ToString(), out var recipeIngredients))
        {
            foreach (var reagant in recipeIngredients.Item1)
            {
                if ((reagant.DisplayName != null) && !source.Inventory.HasCount(reagant.DisplayName, reagant.Amount))
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
                source.Trackers.Counters.TryGetValue(ITEM_COUNTER_PREFIX + selectedRecipe, out var value) ? value : 0;

            foreach (var removeRegant in recipeIngredients.Item1)
            {
                if (removeRegant.DisplayName != null)
                    source.Inventory.RemoveQuantity(removeRegant.DisplayName, removeRegant.Amount);
            }

            if (!Randomizer.RollChance(
                    (int)CalculateSuccessRate(legendMarkCount, timesCraftedThisItem, BASE_SUCCESS_RATE)))
            {
                Subject.Close(source);
                var dialog = DialogFactory.Create("alchemy_Failed", Subject.DialogSource);
                dialog.MenuArgs = Subject.MenuArgs;
                dialog.InjectTextParameters(selectedRecipe);
                dialog.Display(source);
                source.Animate(FailAnimation);

                return;
            }

            source.Trackers.Counters.AddOrIncrement(ITEM_COUNTER_PREFIX + selectedRecipe);

            if (existingMark is null)
            {
                UpdateLegendmark(source, legendMarkCount);
            }
            
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
                }
            }

            if (CRAFTGOODGREATGRAND)
            {
                var roll = Randomizer.RollSingle(100);

                var newCraft = roll switch
                {
                    < 5  => ItemFactory.Create("grand" + selectedRecipe),
                    < 10 => ItemFactory.Create("great" + selectedRecipe),
                    < 15 => ItemFactory.Create("good" + selectedRecipe),
                    _    => ItemFactory.Create(selectedRecipe.ToString())
                };

                if (!source.Inventory.TryAddToNextSlot(newCraft))
                {
                    source.Bank.Deposit(newCraft);
                    source.SendOrangeBarMessage("You have no space. It was sent to your bank.");
                }

                Subject.InjectTextParameters(newCraft.DisplayName);
            } else
            {
                var newCraft = ItemFactory.Create(selectedRecipe.ToString());

                if (!source.Inventory.TryAddToNextSlot(newCraft))
                {
                    source.Bank.Deposit(newCraft);
                    source.SendOrangeBarMessage("You have no space. It was sent to your bank.");
                }

                Subject.InjectTextParameters(newCraft.DisplayName);
            }

            source.Animate(SuccessAnimation);
        } else
        {
            Subject.Reply(source, "Recipe not found or something went wrong.");
        }
    }
}
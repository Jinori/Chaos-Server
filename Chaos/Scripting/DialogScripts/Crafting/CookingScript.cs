using Chaos.Definitions;
using Chaos.Extensions.Common;
using Chaos.Models.Data;
using Chaos.Models.Menu;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.TypeMapper.Abstractions;
using Chaos.Utilities;

namespace Chaos.Scripting.DialogScripts.Crafting;

public class CookingScript : ConfigurableDialogScriptBase
{
    private readonly ICloningService<Item> ItemCloner;
    private readonly IItemFactory ItemFactory;
    private ItemDetails? ItemDetails;
    protected HashSet<string>? ItemTemplateKeys { get; init; }
    private Item? FauxItem => ItemDetails?.Item;

    public CookingScript(
        Dialog subject,
        ICloningService<Item> itemCloner,
        IItemFactory itemFactory
    )
        : base(subject)
    {
        ItemCloner = itemCloner;
        ItemFactory = itemFactory;
    }

    public override void OnDisplaying(Aisling source)
    {

        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "cooking_initial":
            {
                source.Trackers.Enums.Remove(typeof(CookFoodStage));
                source.Trackers.Enums.Remove(typeof(MeatsStage));
                source.Trackers.Enums.Remove(typeof(FruitsStage));
                source.Trackers.Enums.Remove(typeof(FruitsStage2));
                source.Trackers.Enums.Remove(typeof(FruitsStage3));
                source.Trackers.Enums.Remove(typeof(VegetableStage));
                source.Trackers.Enums.Remove(typeof(VegetableStage2));
                source.Trackers.Enums.Remove(typeof(VegetableStage3));
                source.Trackers.Enums.Remove(typeof(ExtraIngredientsStage));
                source.Trackers.Enums.Remove(typeof(ExtraIngredientsStage2));
                source.Trackers.Enums.Remove(typeof(ExtraIngredientsStage3));
                source.Trackers.Flags.RemoveFlag(CookFoodProgression.NotenoughIngredients);

                #region FlagCheck
                if (source.Trackers.Flags.HasFlag(CookingRecipes.dinnerplate))
                {
                    var item = ItemFactory.CreateFaux("dinnerplate");
                    Subject.Items.Add(ItemDetails.BuyWithGold(item));
                }
                if (source.Trackers.Flags.HasFlag(CookingRecipes.sweetbuns))
                {
                    var item = ItemFactory.CreateFaux("sweetbuns");
                    Subject.Items.Add(ItemDetails.BuyWithGold(item));
                }
                if (source.Trackers.Flags.HasFlag(CookingRecipes.fruitbasket))
                {
                    var item = ItemFactory.CreateFaux("fruitbasket");
                    Subject.Items.Add(ItemDetails.BuyWithGold(item));
                }
                if (source.Trackers.Flags.HasFlag(CookingRecipes.lobsterdinner))
                {
                    var item = ItemFactory.CreateFaux("lobsterdinner");
                    Subject.Items.Add(ItemDetails.BuyWithGold(item));
                }
                if (source.Trackers.Flags.HasFlag(CookingRecipes.sandwich))
                {
                    var item = ItemFactory.CreateFaux("sandwich");
                    Subject.Items.Add(ItemDetails.BuyWithGold(item));
                }
                if (source.Trackers.Flags.HasFlag(CookingRecipes.soup))
                {
                    var item = ItemFactory.CreateFaux("soup");
                    Subject.Items.Add(ItemDetails.BuyWithGold(item));
                }
                if (source.Trackers.Flags.HasFlag(CookingRecipes.pie))
                {
                    var item = ItemFactory.CreateFaux("pie");
                    Subject.Items.Add(ItemDetails.BuyWithGold(item));
                }
                if (source.Trackers.Flags.HasFlag(CookingRecipes.salad))
                {
                    var item = ItemFactory.CreateFaux("salad");
                    Subject.Items.Add(ItemDetails.BuyWithGold(item));
                }
                if (source.Trackers.Flags.HasFlag(CookingRecipes.steakmeal))
                {
                    var item = ItemFactory.CreateFaux("steakmeal");
                    Subject.Items.Add(ItemDetails.BuyWithGold(item));
                }
                #endregion

                if (!CraftingRequirements.FoodRequirements.TryGetValue("dinnerplate", out var requirements))
                {
                    Subject.Reply(source, $"{DialogString.UnknownInput.Value}");

                    return;
                }

                var requirementParams = requirements
                                        .SelectMany(
                                            requirement =>
                                            {
                                                var fauxItem = ItemFactory.CreateFaux(requirement.TemplateKey);
                                                return new object[] { requirement.Amount, fauxItem.DisplayName };
                                            })
                                        .ToArray();

                Subject.Text.Inject(requirementParams);
            }

                break;
        }

    }

    public override void OnNext(Aisling source, byte? optionIndex = null)
    {
        var foodSelected = source.Trackers.Enums.TryGetValue(out CookFoodStage foodStage);
        var meatStage = source.Trackers.Enums.TryGetValue(out MeatsStage mStage);
        var meatStage2 = source.Trackers.Enums.TryGetValue(out MeatsStage2 mStage2);
        var meatStage3 = source.Trackers.Enums.TryGetValue(out MeatsStage3 mStage3);
        var fruitsStage = source.Trackers.Enums.TryGetValue(out FruitsStage fstage);
        var fruitsStage2 = source.Trackers.Enums.TryGetValue(out FruitsStage2 fstage2);
        var fruitsStage3 = source.Trackers.Enums.TryGetValue(out FruitsStage3 fstage3);
        var vegetableStage = source.Trackers.Enums.TryGetValue(out VegetableStage vStage);
        var vegetableStage2 = source.Trackers.Enums.TryGetValue(out VegetableStage2 vStage2);
        var vegetableStage3 = source.Trackers.Enums.TryGetValue(out VegetableStage3 vStage3);
        var extraIngredientsStage = source.Trackers.Enums.TryGetValue(out ExtraIngredientsStage eStage);
        var extraIngredientsStage2 = source.Trackers.Enums.TryGetValue(out ExtraIngredientsStage2 eStage2);
        var extraIngredientsStage3 = source.Trackers.Enums.TryGetValue(out ExtraIngredientsStage3 eStage3);
        switch (Subject.Template.TemplateKey.ToLower())
        {
            #region recipeselection
            case "cooking_initial":
            {
                if (ItemDetails == null)
                {
                    if (!Subject.MenuArgs.TryGet<string>(0, out var itemName))
                    {
                        Subject.Reply(source, DialogString.UnknownInput.Value);

                        return;
                    }

                    ItemDetails = Subject.Items.FirstOrDefault(details => details.Item.DisplayName.EqualsI(itemName));

                    if (ItemDetails == null)
                    {
                        Subject.Reply(source, DialogString.UnknownInput.Value);

                        return;
                    }
                }

                switch (FauxItem?.Template.TemplateKey.ToLower())
                {
                    case "dinnerplate":
                    {
                        source.Trackers.Enums.Set(CookFoodStage.dinnerplate);
                        Subject.Reply(source, "Skip", "cooking_directory");

                        return;
                    }
                    case "sweetbuns":
                    {
                        source.Trackers.Enums.Set(CookFoodStage.sweetbuns);
                        Subject.Reply(source, "Skip", "cooking_directory");

                        return;
                    }
                    case "fruitbasket":
                    {
                        source.Trackers.Enums.Set(CookFoodStage.fruitbasket);
                        Subject.Reply(source, "Skip", "cooking_directory");

                        return;
                    }
                    case "lobsterdinner":
                    {
                        source.Trackers.Enums.Set(CookFoodStage.lobsterdinner);
                        Subject.Reply(source, "Skip", "cooking_directory");

                        return;
                    }
                    case "pie":
                    {
                        source.Trackers.Enums.Set(CookFoodStage.pie);
                        Subject.Reply(source, "Skip", "cooking_directory");

                        return;
                    }
                    case "salad":
                    {
                        source.Trackers.Enums.Set(CookFoodStage.salad);
                        Subject.Reply(source, "Skip", "cooking_directory");

                        return;
                    }
                    case "sandwich":
                    {
                        source.Trackers.Enums.Set(CookFoodStage.sandwich);
                        Subject.Reply(source, "Skip", "cooking_directory");

                        return;
                    }
                    case "soup":
                    {
                        source.Trackers.Enums.Set(CookFoodStage.soup);
                        Subject.Reply(source, "Skip", "cooking_directory");

                        return;
                    }
                    case "steakmeal":
                    {
                        source.Trackers.Enums.Set(CookFoodStage.steakmeal);
                        Subject.Reply(source, "Skip", "cooking_directory");

                        return;
                    }
                }

                break;
            }
            #endregion
            #region meatselection
            case "meat_initial":
            {
                if (ItemDetails == null)
                {
                    if (!Subject.MenuArgs.TryGet<string>(0, out var itemName))
                    {
                        Subject.Reply(source, DialogString.UnknownInput.Value);

                        return;
                    }

                    ItemDetails = Subject.Items.FirstOrDefault(details => details.Item.DisplayName.EqualsI(itemName));

                    if (ItemDetails == null)
                    {
                        Subject.Reply(source, DialogString.UnknownInput.Value);

                        return;
                    }
                }

                switch (FauxItem?.Template.TemplateKey.ToLower())
                {
                    case "beef":
                    {
                        source.Trackers.Enums.Set(MeatsStage.beef);
                        Subject.Reply(source, "Skip", "cooking_directory");

                        return;
                    }
                    case "chicken":
                    {
                        source.Trackers.Enums.Set(MeatsStage.chicken);
                        Subject.Reply(source, "Skip", "cooking_directory");

                        return;
                    }
                    case "beefslices":
                    {
                        source.Trackers.Enums.Set(MeatsStage.beefslices);
                        Subject.Reply(source, "Skip", "cooking_directory");

                        return;
                    }
                    case "clam":
                    {
                        source.Trackers.Enums.Set(MeatsStage.clam);
                        Subject.Reply(source, "Skip", "cooking_directory");

                        return;
                    }
                    case "egg":
                    {
                        source.Trackers.Enums.Set(MeatsStage.egg);
                        Subject.Reply(source, "Skip", "cooking_directory");

                        return;
                    }
                    case "liver":
                    {
                        source.Trackers.Enums.Set(MeatsStage.liver);
                        Subject.Reply(source, "Skip", "cooking_directory");

                        return;
                    }
                    case "lobstertail":
                    {
                        source.Trackers.Enums.Set(MeatsStage.lobstertail);
                        Subject.Reply(source, "Skip", "cooking_directory");

                        return;
                    }
                    case "rawmeat":
                    {
                        source.Trackers.Enums.Set(MeatsStage.rawmeat);
                        Subject.Reply(source, "Skip", "cooking_directory");

                        return;
                    }

                        break;
                }

                break;
            }
            #endregion
            #region fruitselection
            case "fruit_initial":
            {
                if (ItemDetails == null)
                {
                    if (!Subject.MenuArgs.TryGet<string>(0, out var itemName))
                    {
                        Subject.Reply(source, DialogString.UnknownInput.Value);

                        return;
                    }

                    ItemDetails = Subject.Items.FirstOrDefault(details => details.Item.DisplayName.EqualsI(itemName));

                    if (ItemDetails == null)
                    {
                        Subject.Reply(source, DialogString.UnknownInput.Value);

                        return;
                    }
                }

                switch (FauxItem?.Template.TemplateKey.ToLower())
                {
                    case "cherry":
                    {
                        if (!fruitsStage)
                        {
                            source.Trackers.Enums.Set(FruitsStage.cherry);
                            Subject.Reply(source, "Skip", "cooking_directory");
                            return;
                        }
                        if (!fruitsStage2)
                        {
                            source.Trackers.Enums.Set(FruitsStage2.cherry);
                            Subject.Reply(source, "Skip", "cooking_directory");
                            return;
                        }
                        if (!fruitsStage3)
                        {
                            source.Trackers.Enums.Set(FruitsStage3.cherry);
                            Subject.Reply(source, "Skip", "cooking_directory");
                            return;
                        }

                        return;
                    }
                    case "acorn":
                    {
                        if (!fruitsStage)
                        {
                            source.Trackers.Enums.Set(FruitsStage.acorn);
                            Subject.Reply(source, "Skip", "cooking_directory");
                            return;
                        }
                        if (!fruitsStage2)
                        {
                            source.Trackers.Enums.Set(FruitsStage2.acorn);
                            Subject.Reply(source, "Skip", "cooking_directory");
                            return;
                        }
                        if (!fruitsStage3)
                        {
                            source.Trackers.Enums.Set(FruitsStage3.acorn);
                            Subject.Reply(source, "Skip", "cooking_directory");
                            return;
                        }

                        return;
                    }
                    case "apple":
                    {
                        if (!fruitsStage)
                        {
                            source.Trackers.Enums.Set(FruitsStage.apple);
                            Subject.Reply(source, "Skip", "cooking_directory");
                            return;
                        }
                        if (!fruitsStage2)
                        {
                            source.Trackers.Enums.Set(FruitsStage2.apple);
                            Subject.Reply(source, "Skip", "cooking_directory");
                            return;
                        }
                        if (!fruitsStage3)
                        {
                            source.Trackers.Enums.Set(FruitsStage3.apple);
                            Subject.Reply(source, "Skip", "cooking_directory");
                            return;
                        }

                        return;

                        return;
                    }
                    case "grape":
                    {
                        if (!fruitsStage)
                        {
                            source.Trackers.Enums.Set(FruitsStage.grape);
                            Subject.Reply(source, "Skip", "cooking_directory");
                            return;
                        }
                        if (!fruitsStage2)
                        {
                            source.Trackers.Enums.Set(FruitsStage2.grape);
                            Subject.Reply(source, "Skip", "cooking_directory");
                            return;
                        }
                        if (!fruitsStage3)
                        {
                            source.Trackers.Enums.Set(FruitsStage3.grape);
                            Subject.Reply(source, "Skip", "cooking_directory");
                            return;
                        }

                        return;

                        return;
                    }
                    case "greengrapes":
                    {
                        if (!fruitsStage)
                        {
                            source.Trackers.Enums.Set(FruitsStage.greengrapes);
                            Subject.Reply(source, "Skip", "cooking_directory");
                            return;
                        }
                        if (!fruitsStage2)
                        {
                            source.Trackers.Enums.Set(FruitsStage2.greengrapes);
                            Subject.Reply(source, "Skip", "cooking_directory");
                            return;
                        }
                        if (!fruitsStage3)
                        {
                            source.Trackers.Enums.Set(FruitsStage3.greengrapes);
                            Subject.Reply(source, "Skip", "cooking_directory");
                            return;
                        }

                        return;

                        return;
                    }
                    case "strawberry":
                    {
                        if (!fruitsStage)
                        {
                            source.Trackers.Enums.Set(FruitsStage.strawberry);
                            Subject.Reply(source, "Skip", "cooking_directory");
                            return;
                        }
                        if (!fruitsStage2)
                        {
                            source.Trackers.Enums.Set(FruitsStage2.strawberry);
                            Subject.Reply(source, "Skip", "cooking_directory");
                            return;
                        }
                        if (!fruitsStage3)
                        {
                            source.Trackers.Enums.Set(FruitsStage3.strawberry);
                            Subject.Reply(source, "Skip", "cooking_directory");
                            return;
                        }

                        return;

                        return;
                    }
                    case "tangerines":
                    {
                        if (!fruitsStage)
                        {
                            source.Trackers.Enums.Set(FruitsStage.tangerines);
                            Subject.Reply(source, "Skip", "cooking_directory");
                            return;
                        }
                        if (!fruitsStage2)
                        {
                            source.Trackers.Enums.Set(FruitsStage2.tangerines);
                            Subject.Reply(source, "Skip", "cooking_directory");
                            return;
                        }
                        if (!fruitsStage3)
                        {
                            source.Trackers.Enums.Set(FruitsStage3.tangerines);
                            Subject.Reply(source, "Skip", "cooking_directory");
                            return;
                        }

                        return;
                    }
                        

                        break;
                }

                break;
            }
            #endregion
            #region vegetableselection
            case "vegetable_initial":
            {
                if (ItemDetails == null)
                {
                    if (!Subject.MenuArgs.TryGet<string>(0, out var itemName))
                    {
                        Subject.Reply(source, DialogString.UnknownInput.Value);

                        return;
                    }

                    ItemDetails = Subject.Items.FirstOrDefault(details => details.Item.DisplayName.EqualsI(itemName));

                    if (ItemDetails == null)
                    {
                        Subject.Reply(source, DialogString.UnknownInput.Value);

                        return;
                    }
                }

                switch (FauxItem?.Template.TemplateKey.ToLower())
                {
                    case "vegetable":
                    {
                        if (!vegetableStage)
                        {
                            source.Trackers.Enums.Set(VegetableStage.vegetable);
                            Subject.Reply(source, "Skip", "cooking_directory");
                            return;
                        }
                        if (!vegetableStage2)
                        {
                            source.Trackers.Enums.Set(VegetableStage2.vegetable);
                            Subject.Reply(source, "Skip", "cooking_directory");
                            return;
                        }
                        if (!vegetableStage3)
                        {
                            source.Trackers.Enums.Set(VegetableStage3.vegetable);
                            Subject.Reply(source, "Skip", "cooking_directory");
                            return;
                        }

                        break;
                    }
                    case "carrot":
                    {
                        if (!vegetableStage)
                        {
                            source.Trackers.Enums.Set(VegetableStage.carrot);
                            Subject.Reply(source, "Skip", "cooking_directory");
                            return;
                        }
                        if (!vegetableStage2)
                        {
                            source.Trackers.Enums.Set(VegetableStage2.carrot);
                            Subject.Reply(source, "Skip", "cooking_directory");
                            return;
                        }
                        if (!vegetableStage3)
                        {
                            source.Trackers.Enums.Set(VegetableStage3.carrot);
                            Subject.Reply(source, "Skip", "cooking_directory");
                            return;
                        }
                        return;
                    }
                    case "rambutan":
                    {
                        if (!vegetableStage)
                        {
                            source.Trackers.Enums.Set(VegetableStage.rambutan);
                            Subject.Reply(source, "Skip", "cooking_directory");
                            return;
                        }
                        if (!vegetableStage2)
                        {
                            source.Trackers.Enums.Set(VegetableStage2.rambutan);
                            Subject.Reply(source, "Skip", "cooking_directory");
                            return;
                        }
                        if (!vegetableStage3)
                        {
                            source.Trackers.Enums.Set(VegetableStage3.rambutan);
                            Subject.Reply(source, "Skip", "cooking_directory");
                            return;
                        }

                        break;
                    }
                    case "tomato":
                    {
                        if (!vegetableStage)
                        {
                            source.Trackers.Enums.Set(VegetableStage.tomato);
                            Subject.Reply(source, "Skip", "cooking_directory");
                            return;
                        }
                        if (!vegetableStage2)
                        {
                            source.Trackers.Enums.Set(VegetableStage2.tomato);
                            Subject.Reply(source, "Skip", "cooking_directory");
                            return;
                        }
                        if (!vegetableStage3)
                        {
                            source.Trackers.Enums.Set(VegetableStage3.tomato);
                            Subject.Reply(source, "Skip", "cooking_directory");
                            return;
                        }
                        return;
                    }

                        break;
                }

                break;
            }
            #endregion
            #region vegetableselection
            case "extraIngredients_initial":
            {
                if (ItemDetails == null)
                {
                    if (!Subject.MenuArgs.TryGet<string>(0, out var itemName))
                    {
                        Subject.Reply(source, DialogString.UnknownInput.Value);

                        return;
                    }

                    ItemDetails = Subject.Items.FirstOrDefault(details => details.Item.DisplayName.EqualsI(itemName));

                    if (ItemDetails == null)
                    {
                        Subject.Reply(source, DialogString.UnknownInput.Value);

                        return;
                    }
                }

                switch (FauxItem?.Template.TemplateKey.ToLower())
                {
                    case "bread":
                    {
                        if (!extraIngredientsStage)
                        {
                            source.Trackers.Enums.Set(ExtraIngredientsStage.bread);
                            Subject.Reply(source, "Skip", "cooking_directory");
                            return;
                        }
                        if (!extraIngredientsStage2)
                        {
                            source.Trackers.Enums.Set(ExtraIngredientsStage2.bread);
                            Subject.Reply(source, "Skip", "cooking_directory");
                            return;
                        }
                        if (!extraIngredientsStage3)
                        {
                            source.Trackers.Enums.Set(ExtraIngredientsStage3.bread);
                            Subject.Reply(source, "Skip", "cooking_directory");
                            return;
                        }

                        break;
                    }
                    case "cheese":
                    {
                        if (!extraIngredientsStage)
                        {
                            source.Trackers.Enums.Set(ExtraIngredientsStage.cheese);
                            Subject.Reply(source, "Skip", "cooking_directory");
                            return;
                        }
                        if (!extraIngredientsStage2)
                        {
                            source.Trackers.Enums.Set(ExtraIngredientsStage2.cheese);
                            Subject.Reply(source, "Skip", "cooking_directory");
                            return;
                        }
                        if (!extraIngredientsStage3)
                        {
                            source.Trackers.Enums.Set(ExtraIngredientsStage3.cheese);
                            Subject.Reply(source, "Skip", "cooking_directory");
                            return;
                        }
                        return;
                    }
                    case "flour":
                    {
                        if (!extraIngredientsStage)
                        {
                            source.Trackers.Enums.Set(ExtraIngredientsStage.flour);
                            Subject.Reply(source, "Skip", "cooking_directory");
                            return;
                        }
                        if (!extraIngredientsStage2)
                        {
                            source.Trackers.Enums.Set(ExtraIngredientsStage2.flour);
                            Subject.Reply(source, "Skip", "cooking_directory");
                            return;
                        }
                        if (!extraIngredientsStage3)
                        {
                            source.Trackers.Enums.Set(ExtraIngredientsStage3.flour);
                            Subject.Reply(source, "Skip", "cooking_directory");
                            return;
                        }

                        break;
                    }
                    case "marinade":
                    {
                        if (!extraIngredientsStage)
                        {
                            source.Trackers.Enums.Set(ExtraIngredientsStage.marinade);
                            Subject.Reply(source, "Skip", "cooking_directory");
                            return;
                        }
                        if (!extraIngredientsStage2)
                        {
                            source.Trackers.Enums.Set(ExtraIngredientsStage2.marinade);
                            Subject.Reply(source, "Skip", "cooking_directory");
                            return;
                        }
                        if (!extraIngredientsStage3)
                        {
                            source.Trackers.Enums.Set(ExtraIngredientsStage3.marinade);
                            Subject.Reply(source, "Skip", "cooking_directory");
                            return;
                        }
                        return;
                    }
                    case "salt":
                    {
                        if (!extraIngredientsStage)
                        {
                            source.Trackers.Enums.Set(ExtraIngredientsStage.salt);
                            Subject.Reply(source, "Skip", "cooking_directory");
                            return;
                        }
                        if (!extraIngredientsStage2)
                        {
                            source.Trackers.Enums.Set(ExtraIngredientsStage2.salt);
                            Subject.Reply(source, "Skip", "cooking_directory");
                            return;
                        }
                        if (!extraIngredientsStage3)
                        {
                            source.Trackers.Enums.Set(ExtraIngredientsStage3.salt);
                            Subject.Reply(source, "Skip", "cooking_directory");
                            return;
                        }
                        return;
                    }

                        break;
                }

                break;
            }
            #endregion
        }
    }
}
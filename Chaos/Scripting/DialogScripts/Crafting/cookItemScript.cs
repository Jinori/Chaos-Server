using Chaos.Common.Utilities;
using Chaos.Definitions;
using Chaos.Models.Data;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.DialogScripts.Crafting;

public class cookItemScript : DialogScriptBase
{
    private readonly IItemFactory ItemFactory;
    protected HashSet<string>? ItemTemplateKeys { get; init; }

    public cookItemScript(
        Dialog subject,
        IItemFactory itemFactory
    )
        : base(subject) =>
        ItemFactory = itemFactory;

    public override void OnDisplaying(Aisling source)
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
        var extraIngredients = source.Trackers.Enums.TryGetValue(out ExtraIngredientsStage eStage);
        var extraIngredients2 = source.Trackers.Enums.TryGetValue(out ExtraIngredientsStage2 eStage2);
        var extraIngredients3 = source.Trackers.Enums.TryGetValue(out ExtraIngredientsStage3 eStage3);

        if (foodSelected && (foodStage == CookFoodStage.dinnerplate))
            switch (Subject.Template.TemplateKey.ToLower())
            {
                #region meatadd
                case "meat_initial":
                {
                    if (source.Inventory.HasCount("beef", 1))
                    {
                        var item = ItemFactory.CreateFaux("beef");
                        Subject.Items.Add(ItemDetails.BuyWithGold(item));
                    }

                    if (source.Inventory.HasCount("chicken", 1))
                    {
                        var item = ItemFactory.CreateFaux("chicken");
                        Subject.Items.Add(ItemDetails.BuyWithGold(item));
                    }

                    if (source.Inventory.HasCount("beefslices", 1))
                    {
                        var item = ItemFactory.CreateFaux("beefslices");
                        Subject.Items.Add(ItemDetails.BuyWithGold(item));
                    }

                    if (source.Inventory.HasCount("clam", 1))
                    {
                        var item = ItemFactory.CreateFaux("clam");
                        Subject.Items.Add(ItemDetails.BuyWithGold(item));
                    }

                    if (source.Inventory.HasCount("egg", 1))
                    {
                        var item = ItemFactory.CreateFaux("egg");
                        Subject.Items.Add(ItemDetails.BuyWithGold(item));
                    }

                    if (source.Inventory.HasCount("liver", 1))
                    {
                        var item = ItemFactory.CreateFaux("liver");
                        Subject.Items.Add(ItemDetails.BuyWithGold(item));
                    }

                    if (source.Inventory.HasCount("lobstertail", 1))
                    {
                        var item = ItemFactory.CreateFaux("lobstertail");
                        Subject.Items.Add(ItemDetails.BuyWithGold(item));
                    }

                    if (source.Inventory.HasCount("rawmeat", 1))
                    {
                        var item = ItemFactory.CreateFaux("rawmeat");
                        Subject.Items.Add(ItemDetails.BuyWithGold(item));
                    }

                    if (Subject.Items.Count == 0)
                    {
                        Subject.Reply(source, "You do not have any meats to use.","cooking_initial");

                        return;
                    }
                }

                    break;
                #endregion

                #region fruitadd
                case "fruit_initial":
                {
                    if (source.Inventory.HasCount("cherry", 10))
                    {
                        var item = ItemFactory.CreateFaux("cherry");
                        Subject.Items.Add(ItemDetails.BuyWithGold(item));
                    }

                    if (source.Inventory.HasCount("acorn", 10))
                    {
                        var item = ItemFactory.CreateFaux("acorn");
                        Subject.Items.Add(ItemDetails.BuyWithGold(item));
                    }

                    if (source.Inventory.HasCount("apple", 10))
                    {
                        var item = ItemFactory.CreateFaux("apple");
                        Subject.Items.Add(ItemDetails.BuyWithGold(item));
                    }

                    if (source.Inventory.HasCount("greengrapes", 10))
                    {
                        var item = ItemFactory.CreateFaux("greengrapes");
                        Subject.Items.Add(ItemDetails.BuyWithGold(item));
                    }
                    if (source.Inventory.HasCount("grape", 10))
                    {
                        var item = ItemFactory.CreateFaux("grape");
                        Subject.Items.Add(ItemDetails.BuyWithGold(item));
                    }

                    if (source.Inventory.HasCount("strawberry", 10))
                    {
                        var item = ItemFactory.CreateFaux("strawberry");
                        Subject.Items.Add(ItemDetails.BuyWithGold(item));
                    }

                    if (source.Inventory.HasCount("tangerines", 10))
                    {
                        var item = ItemFactory.CreateFaux("tangerines");
                        Subject.Items.Add(ItemDetails.BuyWithGold(item));
                    }

                    if (Subject.Items.Count == 0)
                    {
                        Subject.Reply(source, "You do not have enough fruits to use.", "cooking_initial");

                        return;
                    }
                }

                    break;
                #endregion

                #region vegetableadd
                case "vegetable_initial":
                {
                    if (source.Inventory.HasCount("vegetable", 5))
                    {
                        var item = ItemFactory.CreateFaux("vegetable");
                        Subject.Items.Add(ItemDetails.BuyWithGold(item));
                    }

                    if (source.Inventory.HasCount("carrot", 5))
                    {
                        var item = ItemFactory.CreateFaux("carrot");
                        Subject.Items.Add(ItemDetails.BuyWithGold(item));
                    }

                    if (source.Inventory.HasCount("rambutan", 5))
                    {
                        var item = ItemFactory.CreateFaux("rambutan");
                        Subject.Items.Add(ItemDetails.BuyWithGold(item));
                    }

                    if (source.Inventory.HasCount("tomato", 5))
                    {
                        var item = ItemFactory.CreateFaux("tomato");
                        Subject.Items.Add(ItemDetails.BuyWithGold(item));
                    }

                    if (Subject.Items.Count == 0)
                    {
                        Subject.Reply(source, "You do not have enough vegetables to use.","cooking_initial");

                        return;
                    }
                }

                    break;
                    #endregion

                #region addextraingredients
                case "extraingredients_initial":
                {
                    if (source.Inventory.HasCount("flour", 1))
                    {
                        var item = ItemFactory.CreateFaux("flour");
                        Subject.Items.Add(ItemDetails.BuyWithGold(item));
                    }

                    if (source.Inventory.HasCount("bread", 1))
                    {
                        var item = ItemFactory.CreateFaux("bread");
                        Subject.Items.Add(ItemDetails.BuyWithGold(item));
                    }

                    if (source.Inventory.HasCount("cheese", 1))
                    {
                        var item = ItemFactory.CreateFaux("cheese");
                        Subject.Items.Add(ItemDetails.BuyWithGold(item));
                    }

                    if (source.Inventory.HasCount("marinade", 1))
                    {
                        var item = ItemFactory.CreateFaux("marinade");
                        Subject.Items.Add(ItemDetails.BuyWithGold(item));
                    }

                    if (source.Inventory.HasCount("salt", 1))
                    {
                        var item = ItemFactory.CreateFaux("salt");
                        Subject.Items.Add(ItemDetails.BuyWithGold(item));
                    }

                    if (Subject.Items.Count == 0)
                    {
                        Subject.Reply(source, "You do not have enough extra ingredients to use.","cooking_initial");

                        return;
                    }
                }

                    break;
                #endregion

                #region cookdinnerplate
                case "cook_item":
                {
                    #region CheckItems
                    if (meatStage && (mStage == MeatsStage.beef))
                        if (!source.Inventory.HasCount("beef", 1))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (meatStage && (mStage == MeatsStage.chicken))
                        if (!source.Inventory.HasCount("chicken", 1))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (meatStage && (mStage == MeatsStage.beefslices))
                        if (!source.Inventory.HasCount("beefslices", 1))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (meatStage && (mStage == MeatsStage.clam))
                        if (!source.Inventory.HasCount("clam", 1)) 
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (meatStage && (mStage == MeatsStage.egg))
                        if (!source.Inventory.HasCount("egg", 1)) 
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (meatStage && (mStage == MeatsStage.liver))
                        if (!source.Inventory.HasCount("liver", 1)) 
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (meatStage && (mStage == MeatsStage.lobstertail))
                        if (!source.Inventory.HasCount("lobstertail", 1)) 
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (meatStage && (mStage == MeatsStage.rawmeat))
                        if (!source.Inventory.HasCount("rawmeat", 1)) 
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (meatStage2 && (mStage2 == MeatsStage2.beef))
                        if (!source.Inventory.HasCount("beef", 1)) 
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (meatStage2 && (mStage2 == MeatsStage2.chicken))
                        if (!source.Inventory.HasCount("chicken", 1)) 
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (meatStage2 && (mStage2 == MeatsStage2.beefslices))
                        if (!source.Inventory.HasCount("beefslices", 1)) 
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);
                    
                    if (meatStage2 && (mStage2 == MeatsStage2.clam))
                        if (!source.Inventory.HasCount("clam", 1)) 
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (meatStage2 && (mStage2 == MeatsStage2.egg))
                        if (!source.Inventory.HasCount("egg", 1)) 
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (meatStage2 && (mStage2 == MeatsStage2.liver))
                        if (!source.Inventory.HasCount("liver", 1)) 
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (meatStage2 && (mStage2 == MeatsStage2.lobstertail))
                        if (!source.Inventory.HasCount("lobstertail", 1)) 
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (meatStage2 && (mStage2 == MeatsStage2.rawmeat))
                        if (!source.Inventory.HasCount("rawmeat", 1)) 
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (meatStage3 && (mStage3 == MeatsStage3.beef))
                        if (!source.Inventory.HasCount("beef", 1)) 
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (meatStage3 && (mStage3 == MeatsStage3.chicken))
                        if (!source.Inventory.HasCount("chicken", 1)) 
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (meatStage3 && (mStage3 == MeatsStage3.beefslices))
                        if (!source.Inventory.HasCount("beefslices", 1)) 
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (meatStage3 && (mStage3 == MeatsStage3.clam))
                        if (!source.Inventory.HasCount("clam", 1)) 
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (meatStage3 && (mStage3 == MeatsStage3.egg))
                        if (!source.Inventory.HasCount("egg", 1)) 
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (meatStage3 && (mStage3 == MeatsStage3.liver))
                        if (!source.Inventory.HasCount("liver", 1)) 
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (meatStage3 && (mStage3 == MeatsStage3.lobstertail))
                        if (!source.Inventory.HasCount("lobstertail", 1)) 
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (meatStage3 && (mStage3 == MeatsStage3.rawmeat))
                        if (!source.Inventory.HasCount("rawmeat", 1)) 
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (fruitsStage && (fstage == FruitsStage.cherry))
                        if (!source.Inventory.HasCount("cherry", 10)) 
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (fruitsStage && (fstage == FruitsStage.acorn))
                        if (!source.Inventory.HasCount("acorn", 10)) 
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (fruitsStage && (fstage == FruitsStage.apple))
                        if (!source.Inventory.HasCount("apple", 10)) 
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (fruitsStage && (fstage == FruitsStage.greengrapes))
                        if (!source.Inventory.HasCount("greengrapes", 10)) 
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (fruitsStage && (fstage == FruitsStage.strawberry))
                        if (!source.Inventory.HasCount("strawberry", 10))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (fruitsStage && (fstage == FruitsStage.tangerines))
                        if (!source.Inventory.HasCount("tangerines", 10)) 
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (fruitsStage2 && (fstage2 == FruitsStage2.cherry))
                        if (!source.Inventory.HasCount("cherry", 10))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (fruitsStage2 && (fstage2 == FruitsStage2.acorn))
                        if (!source.Inventory.HasCount("acorn", 10))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (fruitsStage2 && (fstage2 == FruitsStage2.apple))
                        if (!source.Inventory.HasCount("apple", 10))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (fruitsStage2 && (fstage2 == FruitsStage2.greengrapes))
                        if (!source.Inventory.HasCount("greengrapes", 10)) 
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (fruitsStage2 && (fstage2 == FruitsStage2.strawberry))
                        if (!source.Inventory.HasCount("strawberry", 10)) 
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (fruitsStage2 && (fstage2 == FruitsStage2.tangerines))
                        if (!source.Inventory.HasCount("tangerines", 10)) 
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (fruitsStage3 && (fstage3 == FruitsStage3.cherry))
                        if (!source.Inventory.HasCount("cherry", 10)) 
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (fruitsStage3 && (fstage3 == FruitsStage3.acorn))
                        if (!source.Inventory.HasCount("acorn", 10))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (fruitsStage3 && (fstage3 == FruitsStage3.apple))
                        if (!source.Inventory.HasCount("apple", 10)) 
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (fruitsStage3 && (fstage3 == FruitsStage3.greengrapes))
                        if (!source.Inventory.HasCount("greengrapes", 10))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (fruitsStage3 && (fstage3 == FruitsStage3.strawberry))
                        if (!source.Inventory.HasCount("strawberry", 10)) 
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (fruitsStage3 && (fstage3 == FruitsStage3.tangerines))
                        if (!source.Inventory.HasCount("tangerines", 10)) 
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (vegetableStage && (vStage == VegetableStage.vegetable))
                        if (!source.Inventory.HasCount("vegetable", 5)) 
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (vegetableStage && (vStage == VegetableStage.carrot))
                        if (!source.Inventory.HasCount("carrot", 5)) 
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (vegetableStage && (vStage == VegetableStage.rambutan))
                        if (!source.Inventory.HasCount("rambutan", 5)) 
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (vegetableStage && (vStage == VegetableStage.tomato))
                        if (!source.Inventory.HasCount("tomato", 5)) 
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (vegetableStage2 && (vStage2 == VegetableStage2.vegetable))
                        if (!source.Inventory.HasCount("vegetable", 5)) 
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (vegetableStage2 && (vStage2 == VegetableStage2.carrot))
                        if (!source.Inventory.HasCount("carrot", 5)) 
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (vegetableStage2 && (vStage2 == VegetableStage2.rambutan))
                        if (!source.Inventory.HasCount("rambutan", 5)) 
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (vegetableStage2 && (vStage2 == VegetableStage2.tomato))
                        if (!source.Inventory.HasCount("tomato", 5)) 
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (vegetableStage3 && (vStage3 == VegetableStage3.vegetable))
                        if (!source.Inventory.HasCount("vegetable", 5)) 
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (vegetableStage3 && (vStage3 == VegetableStage3.carrot))
                        if (!source.Inventory.HasCount("carrot", 5))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (vegetableStage3 && (vStage3 == VegetableStage3.rambutan))
                        if (!source.Inventory.HasCount("rambutan", 5))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (vegetableStage3 && (vStage3 == VegetableStage3.tomato))
                        if (!source.Inventory.HasCount("tomato", 5))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (extraIngredients && (eStage == ExtraIngredientsStage.bread))
                        if (!source.Inventory.HasCount("bread", 1))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (extraIngredients && (eStage == ExtraIngredientsStage.cheese))
                        if (!source.Inventory.HasCount("cheese", 1))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (extraIngredients && (eStage == ExtraIngredientsStage.flour))
                        if (!source.Inventory.HasCount("flour", 1))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (extraIngredients && (eStage == ExtraIngredientsStage.marinade))
                        if (!source.Inventory.HasCount("marinade", 1))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (extraIngredients && (eStage == ExtraIngredientsStage.salt))
                        if (!source.Inventory.HasCount("salt", 1))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (extraIngredients2 && (eStage2 == ExtraIngredientsStage2.bread))
                        if (!source.Inventory.HasCount("bread", 1))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (extraIngredients2 && (eStage2 == ExtraIngredientsStage2.cheese))
                        if (!source.Inventory.HasCount("cheese", 1))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (extraIngredients2 && (eStage2 == ExtraIngredientsStage2.flour))
                        if (!source.Inventory.HasCount("flour", 1))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (extraIngredients2 && (eStage2 == ExtraIngredientsStage2.marinade))
                        if (!source.Inventory.HasCount("marinade", 1))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (extraIngredients2 && (eStage2 == ExtraIngredientsStage2.salt))
                        if (!source.Inventory.HasCount("salt", 1))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (extraIngredients3 && (eStage3 == ExtraIngredientsStage3.bread))
                        if (!source.Inventory.HasCount("bread", 1))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (extraIngredients3 && (eStage3 == ExtraIngredientsStage3.cheese))
                        if (!source.Inventory.HasCount("cheese", 1))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (extraIngredients3 && (eStage3 == ExtraIngredientsStage3.flour))
                        if (!source.Inventory.HasCount("flour", 1))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (extraIngredients3 && (eStage3 == ExtraIngredientsStage3.marinade))
                        if (!source.Inventory.HasCount("marinade", 1))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (extraIngredients3 && (eStage3 == ExtraIngredientsStage3.salt))
                        if (!source.Inventory.HasCount("salt", 1))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (source.Trackers.Flags.HasFlag(CookFoodProgression.NotenoughIngredients))
                    {
                        Subject.Reply(source, "You don't have enough ingredients to cook this again.", "cooking_initial");
                        source.Trackers.Flags.RemoveFlag(CookFoodProgression.NotenoughIngredients);

                        return;
                    }
                    #endregion
                    #region RemoveItems
                    if (meatStage && (mStage == MeatsStage.beef))
                        source.Inventory.RemoveQuantity("beef", 1);

                    if (meatStage && (mStage == MeatsStage.chicken))
                        source.Inventory.RemoveQuantity("chicken", 1);

                    if (meatStage && (mStage == MeatsStage.beefslices))
                        source.Inventory.RemoveQuantity("beefslices", 1);

                    if (meatStage && (mStage == MeatsStage.clam))
                        source.Inventory.RemoveQuantity("clam", 1);

                    if (meatStage && (mStage == MeatsStage.egg))
                        source.Inventory.RemoveQuantity("egg", 1);

                    if (meatStage && (mStage == MeatsStage.liver))
                        source.Inventory.RemoveQuantity("liver", 1);

                    if (meatStage && (mStage == MeatsStage.lobstertail))
                        source.Inventory.RemoveQuantity("lobstertail", 1);

                    if (meatStage && (mStage == MeatsStage.rawmeat))
                        source.Inventory.RemoveQuantity("rawmeat", 1);

                    if (meatStage2 && (mStage2 == MeatsStage2.beef))
                        source.Inventory.RemoveQuantity("beef", 1);

                    if (meatStage2 && (mStage2 == MeatsStage2.chicken))
                        source.Inventory.RemoveQuantity("chicken", 1);

                    if (meatStage2 && (mStage2 == MeatsStage2.beefslices))
                        source.Inventory.RemoveQuantity("beefslices", 1);

                    if (meatStage2 && (mStage2 == MeatsStage2.clam))
                        source.Inventory.RemoveQuantity("clam", 1);

                    if (meatStage2 && (mStage2 == MeatsStage2.egg))
                        source.Inventory.RemoveQuantity("egg", 1);

                    if (meatStage2 && (mStage2 == MeatsStage2.liver))
                        source.Inventory.RemoveQuantity("liver", 1);

                    if (meatStage2 && (mStage2 == MeatsStage2.lobstertail))
                        source.Inventory.RemoveQuantity("lobstertail", 1);

                    if (meatStage2 && (mStage2 == MeatsStage2.rawmeat))
                        source.Inventory.RemoveQuantity("rawmeat", 1);

                    if (meatStage3 && (mStage3 == MeatsStage3.beef))
                        source.Inventory.RemoveQuantity("beef", 1);

                    if (meatStage3 && (mStage3 == MeatsStage3.chicken))
                        source.Inventory.RemoveQuantity("chicken", 1);

                    if (meatStage3 && (mStage3 == MeatsStage3.beefslices))
                        source.Inventory.RemoveQuantity("beefslices", 1);

                    if (meatStage3 && (mStage3 == MeatsStage3.clam))
                        source.Inventory.RemoveQuantity("clam", 1);

                    if (meatStage3 && (mStage3 == MeatsStage3.egg))
                        source.Inventory.RemoveQuantity("egg", 1);

                    if (meatStage3 && (mStage3 == MeatsStage3.liver))
                        source.Inventory.RemoveQuantity("liver", 1);

                    if (meatStage3 && (mStage3 == MeatsStage3.lobstertail))
                        source.Inventory.RemoveQuantity("lobstertail", 1);

                    if (meatStage3 && (mStage3 == MeatsStage3.rawmeat))
                        source.Inventory.RemoveQuantity("rawmeat", 1);

                    if (fruitsStage && (fstage == FruitsStage.cherry))
                        source.Inventory.RemoveQuantity("cherry", 10);

                    if (fruitsStage && (fstage == FruitsStage.acorn))
                        source.Inventory.RemoveQuantity("acorn", 10);

                    if (fruitsStage && (fstage == FruitsStage.apple))
                        source.Inventory.RemoveQuantity("apple", 10);

                    if (fruitsStage && (fstage == FruitsStage.greengrapes))
                        source.Inventory.RemoveQuantity("greengrapes", 10);

                    if (fruitsStage && (fstage == FruitsStage.strawberry))
                        source.Inventory.RemoveQuantity("strawberry", 10);

                    if (fruitsStage && (fstage == FruitsStage.tangerines))
                        source.Inventory.RemoveQuantity("tangerines", 10);

                    if (fruitsStage2 && (fstage2 == FruitsStage2.cherry))
                        source.Inventory.RemoveQuantity("cherry", 10);

                    if (fruitsStage2 && (fstage2 == FruitsStage2.acorn))
                        source.Inventory.RemoveQuantity("acorn", 10);

                    if (fruitsStage2 && (fstage2 == FruitsStage2.apple))
                        source.Inventory.RemoveQuantity("apple", 10);

                    if (fruitsStage2 && (fstage2 == FruitsStage2.greengrapes))
                        source.Inventory.RemoveQuantity("greengrapes", 10);

                    if (fruitsStage2 && (fstage2 == FruitsStage2.strawberry))
                        source.Inventory.RemoveQuantity("strawberry", 10);

                    if (fruitsStage2 && (fstage2 == FruitsStage2.tangerines))
                        source.Inventory.RemoveQuantity("tangerines", 10);

                    if (fruitsStage3 && (fstage3 == FruitsStage3.cherry))
                        source.Inventory.RemoveQuantity("cherry", 10);

                    if (fruitsStage3 && (fstage3 == FruitsStage3.acorn))
                        source.Inventory.RemoveQuantity("acorn", 10);

                    if (fruitsStage3 && (fstage3 == FruitsStage3.apple))
                        source.Inventory.RemoveQuantity("apple", 10);

                    if (fruitsStage3 && (fstage3 == FruitsStage3.greengrapes))
                        source.Inventory.RemoveQuantity("greengrapes", 10);

                    if (fruitsStage3 && (fstage3 == FruitsStage3.strawberry))
                        source.Inventory.RemoveQuantity("strawberry", 10);

                    if (fruitsStage3 && (fstage3 == FruitsStage3.tangerines))
                        source.Inventory.RemoveQuantity("tangerines", 10);

                    if (vegetableStage && (vStage == VegetableStage.vegetable))
                        source.Inventory.RemoveQuantity("vegetable", 5);

                    if (vegetableStage && (vStage == VegetableStage.carrot))
                        source.Inventory.RemoveQuantity("carrot", 5);

                    if (vegetableStage && (vStage == VegetableStage.rambutan))
                        source.Inventory.RemoveQuantity("rambutan", 5);

                    if (vegetableStage && (vStage == VegetableStage.tomato))
                        source.Inventory.RemoveQuantity("tomato", 5);

                    if (vegetableStage2 && (vStage2 == VegetableStage2.vegetable))
                        source.Inventory.RemoveQuantity("vegetable", 5);

                    if (vegetableStage2 && (vStage2 == VegetableStage2.carrot))
                        source.Inventory.RemoveQuantity("carrot", 5);

                    if (vegetableStage2 && (vStage2 == VegetableStage2.rambutan))
                        source.Inventory.RemoveQuantity("rambutan", 5);

                    if (vegetableStage2 && (vStage2 == VegetableStage2.tomato))
                        source.Inventory.RemoveQuantity("tomato", 5);

                    if (vegetableStage3 && (vStage3 == VegetableStage3.vegetable))
                        source.Inventory.RemoveQuantity("vegetable", 5);

                    if (vegetableStage3 && (vStage3 == VegetableStage3.carrot))
                        source.Inventory.RemoveQuantity("carrot", 5);

                    if (vegetableStage3 && (vStage3 == VegetableStage3.rambutan))
                        source.Inventory.RemoveQuantity("rambutan", 5);

                    if (vegetableStage3 && (vStage3 == VegetableStage3.tomato))
                        source.Inventory.RemoveQuantity("tomato", 5);

                    if (extraIngredients && (eStage == ExtraIngredientsStage.bread))
                        source.Inventory.RemoveQuantity("bread", 1);

                    if (extraIngredients && (eStage == ExtraIngredientsStage.cheese))
                        source.Inventory.RemoveQuantity("cheese", 1);

                    if (extraIngredients && (eStage == ExtraIngredientsStage.flour))
                        source.Inventory.RemoveQuantity("flour", 1);

                    if (extraIngredients && (eStage == ExtraIngredientsStage.marinade))
                        source.Inventory.RemoveQuantity("marinade", 1);

                    if (extraIngredients && (eStage == ExtraIngredientsStage.salt))
                        source.Inventory.RemoveQuantity("salt", 1);

                    if (extraIngredients2 && (eStage2 == ExtraIngredientsStage2.bread))
                        source.Inventory.RemoveQuantity("bread", 1);

                    if (extraIngredients2 && (eStage2 == ExtraIngredientsStage2.cheese))
                        source.Inventory.RemoveQuantity("cheese", 1);

                    if (extraIngredients2 && (eStage2 == ExtraIngredientsStage2.flour))
                        source.Inventory.RemoveQuantity("flour", 1);

                    if (extraIngredients2 && (eStage2 == ExtraIngredientsStage2.marinade))
                        source.Inventory.RemoveQuantity("marinade", 1);

                    if (extraIngredients2 && (eStage2 == ExtraIngredientsStage2.salt))
                        source.Inventory.RemoveQuantity("salt", 1);

                    if (extraIngredients3 && (eStage3 == ExtraIngredientsStage3.bread))
                        source.Inventory.RemoveQuantity("bread", 1);

                    if (extraIngredients3 && (eStage3 == ExtraIngredientsStage3.cheese))
                        source.Inventory.RemoveQuantity("cheese", 1);

                    if (extraIngredients3 && (eStage3 == ExtraIngredientsStage3.flour))
                        source.Inventory.RemoveQuantity("flour", 1);

                    if (extraIngredients3 && (eStage3 == ExtraIngredientsStage3.marinade))
                        source.Inventory.RemoveQuantity("marinade", 1);

                    if (extraIngredients3 && (eStage3 == ExtraIngredientsStage3.salt))
                        source.Inventory.RemoveQuantity("salt", 1);
                    
                    #endregion

                    if (Randomizer.RollChance(20))
                    {
                        Subject.Reply(source, "Skip", "cookfailed_itemrepeat");
                        return;
                    }

                    source.TryGiveItems(ItemFactory.Create("dinnerplate"));
                    source.SendOrangeBarMessage("You have cooked a Dinner Plate!");
                    Subject.Reply(source, "Skip", "cook_itemrepeat");
                }

                    break;
                #endregion
            }

        if (foodSelected && (foodStage == CookFoodStage.sweetbuns))
            switch (Subject.Template.TemplateKey.ToLower())
            {
                #region meatadd
                case "meat_initial":
                {
                    if (source.Inventory.HasCount("beef", 1))
                    {
                        var item = ItemFactory.CreateFaux("beef");
                        Subject.Items.Add(ItemDetails.BuyWithGold(item));
                    }

                    if (source.Inventory.HasCount("chicken", 1))
                    {
                        var item = ItemFactory.CreateFaux("chicken");
                        Subject.Items.Add(ItemDetails.BuyWithGold(item));
                    }

                    if (source.Inventory.HasCount("beefslices", 1))
                    {
                        var item = ItemFactory.CreateFaux("beefslices");
                        Subject.Items.Add(ItemDetails.BuyWithGold(item));
                    }

                    if (source.Inventory.HasCount("clam", 1))
                    {
                        var item = ItemFactory.CreateFaux("clam");
                        Subject.Items.Add(ItemDetails.BuyWithGold(item));
                    }

                    if (source.Inventory.HasCount("egg", 1))
                    {
                        var item = ItemFactory.CreateFaux("egg");
                        Subject.Items.Add(ItemDetails.BuyWithGold(item));
                    }

                    if (source.Inventory.HasCount("liver", 1))
                    {
                        var item = ItemFactory.CreateFaux("liver");
                        Subject.Items.Add(ItemDetails.BuyWithGold(item));
                    }

                    if (source.Inventory.HasCount("lobstertail", 1))
                    {
                        var item = ItemFactory.CreateFaux("lobstertail");
                        Subject.Items.Add(ItemDetails.BuyWithGold(item));
                    }

                    if (source.Inventory.HasCount("rawmeat", 1))
                    {
                        var item = ItemFactory.CreateFaux("rawmeat");
                        Subject.Items.Add(ItemDetails.BuyWithGold(item));
                    }

                    if (Subject.Items.Count == 0)
                    {
                        Subject.Reply(source, "You do not have any meats to use.","cooking_initial");

                        return;
                    }
                }

                    break;
                #endregion

                #region fruitadd
                case "fruit_initial":
                {
                    if (source.Inventory.HasCount("cherry", 15))
                    {
                        var item = ItemFactory.CreateFaux("cherry");
                        Subject.Items.Add(ItemDetails.BuyWithGold(item));
                    }

                    if (source.Inventory.HasCount("acorn", 15))
                    {
                        var item = ItemFactory.CreateFaux("acorn");
                        Subject.Items.Add(ItemDetails.BuyWithGold(item));
                    }

                    if (source.Inventory.HasCount("apple", 15))
                    {
                        var item = ItemFactory.CreateFaux("apple");
                        Subject.Items.Add(ItemDetails.BuyWithGold(item));
                    }

                    if (source.Inventory.HasCount("greengrapes", 15))
                    {
                        var item = ItemFactory.CreateFaux("greengrapes");
                        Subject.Items.Add(ItemDetails.BuyWithGold(item));
                    }
                    if (source.Inventory.HasCount("grape", 15))
                    {
                        var item = ItemFactory.CreateFaux("grape");
                        Subject.Items.Add(ItemDetails.BuyWithGold(item));
                    }

                    if (source.Inventory.HasCount("strawberry", 15))
                    {
                        var item = ItemFactory.CreateFaux("strawberry");
                        Subject.Items.Add(ItemDetails.BuyWithGold(item));
                    }

                    if (source.Inventory.HasCount("tangerines", 15))
                    {
                        var item = ItemFactory.CreateFaux("tangerines");
                        Subject.Items.Add(ItemDetails.BuyWithGold(item));
                    }

                    if (Subject.Items.Count == 0)
                    {
                        Subject.Reply(source, "You do not have enough fruits to use.", "cooking_initial");

                        return;
                    }
                }

                    break;
                #endregion

                #region vegetableadd
                case "vegetable_initial":
                {
                    if (source.Inventory.HasCount("vegetable", 5))
                    {
                        var item = ItemFactory.CreateFaux("vegetable");
                        Subject.Items.Add(ItemDetails.BuyWithGold(item));
                    }

                    if (source.Inventory.HasCount("carrot", 5))
                    {
                        var item = ItemFactory.CreateFaux("carrot");
                        Subject.Items.Add(ItemDetails.BuyWithGold(item));
                    }

                    if (source.Inventory.HasCount("rambutan", 5))
                    {
                        var item = ItemFactory.CreateFaux("rambutan");
                        Subject.Items.Add(ItemDetails.BuyWithGold(item));
                    }

                    if (source.Inventory.HasCount("tomato", 5))
                    {
                        var item = ItemFactory.CreateFaux("tomato");
                        Subject.Items.Add(ItemDetails.BuyWithGold(item));
                    }

                    if (Subject.Items.Count == 0)
                    {
                        Subject.Reply(source, "You do not have enough vegetables to use.","cooking_initial");

                        return;
                    }
                }

                    break;
                    #endregion

                #region addextraingredients
                case "extraingredients_initial":
                {
                    if (source.Inventory.HasCount("flour", 2))
                    {
                        var item = ItemFactory.CreateFaux("flour");
                        Subject.Items.Add(ItemDetails.BuyWithGold(item));
                    }

                    if (source.Inventory.HasCount("bread", 1))
                    {
                        var item = ItemFactory.CreateFaux("bread");
                        Subject.Items.Add(ItemDetails.BuyWithGold(item));
                    }

                    if (source.Inventory.HasCount("cheese", 1))
                    {
                        var item = ItemFactory.CreateFaux("cheese");
                        Subject.Items.Add(ItemDetails.BuyWithGold(item));
                    }

                    if (source.Inventory.HasCount("marinade", 1))
                    {
                        var item = ItemFactory.CreateFaux("marinade");
                        Subject.Items.Add(ItemDetails.BuyWithGold(item));
                    }

                    if (source.Inventory.HasCount("salt", 1))
                    {
                        var item = ItemFactory.CreateFaux("salt");
                        Subject.Items.Add(ItemDetails.BuyWithGold(item));
                    }

                    if (Subject.Items.Count == 0)
                    {
                        Subject.Reply(source, "You do not have enough extra ingredients to use.","cooking_initial");

                        return;
                    }
                }

                    break;
                #endregion

                #region cooksweetbuns
                case "cook_item":
                {
                    #region CheckItems
                    if (meatStage && (mStage == MeatsStage.beef))
                        if (!source.Inventory.HasCount("beef", 1))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (meatStage && (mStage == MeatsStage.chicken))
                        if (!source.Inventory.HasCount("chicken", 1))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (meatStage && (mStage == MeatsStage.beefslices))
                        if (!source.Inventory.HasCount("beefslices", 1))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (meatStage && (mStage == MeatsStage.clam))
                        if (!source.Inventory.HasCount("clam", 1)) 
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (meatStage && (mStage == MeatsStage.egg))
                        if (!source.Inventory.HasCount("egg", 1)) 
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (meatStage && (mStage == MeatsStage.liver))
                        if (!source.Inventory.HasCount("liver", 1)) 
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (meatStage && (mStage == MeatsStage.lobstertail))
                        if (!source.Inventory.HasCount("lobstertail", 1)) 
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (meatStage && (mStage == MeatsStage.rawmeat))
                        if (!source.Inventory.HasCount("rawmeat", 1)) 
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (meatStage2 && (mStage2 == MeatsStage2.beef))
                        if (!source.Inventory.HasCount("beef", 1)) 
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (meatStage2 && (mStage2 == MeatsStage2.chicken))
                        if (!source.Inventory.HasCount("chicken", 1)) 
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (meatStage2 && (mStage2 == MeatsStage2.beefslices))
                        if (!source.Inventory.HasCount("beefslices", 1)) 
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);
                    
                    if (meatStage2 && (mStage2 == MeatsStage2.clam))
                        if (!source.Inventory.HasCount("clam", 1)) 
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (meatStage2 && (mStage2 == MeatsStage2.egg))
                        if (!source.Inventory.HasCount("egg", 1)) 
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (meatStage2 && (mStage2 == MeatsStage2.liver))
                        if (!source.Inventory.HasCount("liver", 1)) 
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (meatStage2 && (mStage2 == MeatsStage2.lobstertail))
                        if (!source.Inventory.HasCount("lobstertail", 1)) 
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (meatStage2 && (mStage2 == MeatsStage2.rawmeat))
                        if (!source.Inventory.HasCount("rawmeat", 1)) 
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (meatStage3 && (mStage3 == MeatsStage3.beef))
                        if (!source.Inventory.HasCount("beef", 1)) 
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (meatStage3 && (mStage3 == MeatsStage3.chicken))
                        if (!source.Inventory.HasCount("chicken", 1)) 
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (meatStage3 && (mStage3 == MeatsStage3.beefslices))
                        if (!source.Inventory.HasCount("beefslices", 1)) 
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (meatStage3 && (mStage3 == MeatsStage3.clam))
                        if (!source.Inventory.HasCount("clam", 1)) 
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (meatStage3 && (mStage3 == MeatsStage3.egg))
                        if (!source.Inventory.HasCount("egg", 1)) 
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (meatStage3 && (mStage3 == MeatsStage3.liver))
                        if (!source.Inventory.HasCount("liver", 1)) 
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (meatStage3 && (mStage3 == MeatsStage3.lobstertail))
                        if (!source.Inventory.HasCount("lobstertail", 1)) 
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (meatStage3 && (mStage3 == MeatsStage3.rawmeat))
                        if (!source.Inventory.HasCount("rawmeat", 1)) 
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (fruitsStage && (fstage == FruitsStage.cherry))
                        if (!source.Inventory.HasCount("cherry", 10)) 
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (fruitsStage && (fstage == FruitsStage.acorn))
                        if (!source.Inventory.HasCount("acorn", 10)) 
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (fruitsStage && (fstage == FruitsStage.apple))
                        if (!source.Inventory.HasCount("apple", 10)) 
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (fruitsStage && (fstage == FruitsStage.greengrapes))
                        if (!source.Inventory.HasCount("greengrapes", 10)) 
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (fruitsStage && (fstage == FruitsStage.strawberry))
                        if (!source.Inventory.HasCount("strawberry", 10))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (fruitsStage && (fstage == FruitsStage.tangerines))
                        if (!source.Inventory.HasCount("tangerines", 10)) 
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (fruitsStage2 && (fstage2 == FruitsStage2.cherry))
                        if (!source.Inventory.HasCount("cherry", 10))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (fruitsStage2 && (fstage2 == FruitsStage2.acorn))
                        if (!source.Inventory.HasCount("acorn", 10))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (fruitsStage2 && (fstage2 == FruitsStage2.apple))
                        if (!source.Inventory.HasCount("apple", 10))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (fruitsStage2 && (fstage2 == FruitsStage2.greengrapes))
                        if (!source.Inventory.HasCount("greengrapes", 10)) 
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (fruitsStage2 && (fstage2 == FruitsStage2.strawberry))
                        if (!source.Inventory.HasCount("strawberry", 10)) 
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (fruitsStage2 && (fstage2 == FruitsStage2.tangerines))
                        if (!source.Inventory.HasCount("tangerines", 10)) 
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (fruitsStage3 && (fstage3 == FruitsStage3.cherry))
                        if (!source.Inventory.HasCount("cherry", 10)) 
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (fruitsStage3 && (fstage3 == FruitsStage3.acorn))
                        if (!source.Inventory.HasCount("acorn", 10))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (fruitsStage3 && (fstage3 == FruitsStage3.apple))
                        if (!source.Inventory.HasCount("apple", 10)) 
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (fruitsStage3 && (fstage3 == FruitsStage3.greengrapes))
                        if (!source.Inventory.HasCount("greengrapes", 10))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (fruitsStage3 && (fstage3 == FruitsStage3.strawberry))
                        if (!source.Inventory.HasCount("strawberry", 10)) 
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (fruitsStage3 && (fstage3 == FruitsStage3.tangerines))
                        if (!source.Inventory.HasCount("tangerines", 10)) 
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (vegetableStage && (vStage == VegetableStage.vegetable))
                        if (!source.Inventory.HasCount("vegetable", 5)) 
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (vegetableStage && (vStage == VegetableStage.carrot))
                        if (!source.Inventory.HasCount("carrot", 5)) 
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (vegetableStage && (vStage == VegetableStage.rambutan))
                        if (!source.Inventory.HasCount("rambutan", 5)) 
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (vegetableStage && (vStage == VegetableStage.tomato))
                        if (!source.Inventory.HasCount("tomato", 5)) 
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (vegetableStage2 && (vStage2 == VegetableStage2.vegetable))
                        if (!source.Inventory.HasCount("vegetable", 5)) 
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (vegetableStage2 && (vStage2 == VegetableStage2.carrot))
                        if (!source.Inventory.HasCount("carrot", 5)) 
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (vegetableStage2 && (vStage2 == VegetableStage2.rambutan))
                        if (!source.Inventory.HasCount("rambutan", 5)) 
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (vegetableStage2 && (vStage2 == VegetableStage2.tomato))
                        if (!source.Inventory.HasCount("tomato", 5)) 
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (vegetableStage3 && (vStage3 == VegetableStage3.vegetable))
                        if (!source.Inventory.HasCount("vegetable", 5)) 
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (vegetableStage3 && (vStage3 == VegetableStage3.carrot))
                        if (!source.Inventory.HasCount("carrot", 5))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (vegetableStage3 && (vStage3 == VegetableStage3.rambutan))
                        if (!source.Inventory.HasCount("rambutan", 5))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (vegetableStage3 && (vStage3 == VegetableStage3.tomato))
                        if (!source.Inventory.HasCount("tomato", 5))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (extraIngredients && (eStage == ExtraIngredientsStage.bread))
                        if (!source.Inventory.HasCount("bread", 1))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (extraIngredients && (eStage == ExtraIngredientsStage.cheese))
                        if (!source.Inventory.HasCount("cheese", 1))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (extraIngredients && (eStage == ExtraIngredientsStage.flour))
                        if (!source.Inventory.HasCount("flour", 1))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (extraIngredients && (eStage == ExtraIngredientsStage.marinade))
                        if (!source.Inventory.HasCount("marinade", 1))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (extraIngredients && (eStage == ExtraIngredientsStage.salt))
                        if (!source.Inventory.HasCount("salt", 1))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (extraIngredients2 && (eStage2 == ExtraIngredientsStage2.bread))
                        if (!source.Inventory.HasCount("bread", 1))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (extraIngredients2 && (eStage2 == ExtraIngredientsStage2.cheese))
                        if (!source.Inventory.HasCount("cheese", 1))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (extraIngredients2 && (eStage2 == ExtraIngredientsStage2.flour))
                        if (!source.Inventory.HasCount("flour", 1))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (extraIngredients2 && (eStage2 == ExtraIngredientsStage2.marinade))
                        if (!source.Inventory.HasCount("marinade", 1))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (extraIngredients2 && (eStage2 == ExtraIngredientsStage2.salt))
                        if (!source.Inventory.HasCount("salt", 1))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (extraIngredients3 && (eStage3 == ExtraIngredientsStage3.bread))
                        if (!source.Inventory.HasCount("bread", 1))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (extraIngredients3 && (eStage3 == ExtraIngredientsStage3.cheese))
                        if (!source.Inventory.HasCount("cheese", 1))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (extraIngredients3 && (eStage3 == ExtraIngredientsStage3.flour))
                        if (!source.Inventory.HasCount("flour", 1))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (extraIngredients3 && (eStage3 == ExtraIngredientsStage3.marinade))
                        if (!source.Inventory.HasCount("marinade", 1))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (extraIngredients3 && (eStage3 == ExtraIngredientsStage3.salt))
                        if (!source.Inventory.HasCount("salt", 1))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (source.Trackers.Flags.HasFlag(CookFoodProgression.NotenoughIngredients))
                    {
                        Subject.Reply(source, "You don't have enough ingredients to cook this again.", "cooking_initial");
                        source.Trackers.Flags.RemoveFlag(CookFoodProgression.NotenoughIngredients);

                        return;
                    }
                    #endregion
                    #region RemoveItems
                    if (meatStage && (mStage == MeatsStage.beef))
                        source.Inventory.RemoveQuantity("beef", 1);

                    if (meatStage && (mStage == MeatsStage.chicken))
                        source.Inventory.RemoveQuantity("chicken", 1);

                    if (meatStage && (mStage == MeatsStage.beefslices))
                        source.Inventory.RemoveQuantity("beefslices", 1);

                    if (meatStage && (mStage == MeatsStage.clam))
                        source.Inventory.RemoveQuantity("clam", 1);

                    if (meatStage && (mStage == MeatsStage.egg))
                        source.Inventory.RemoveQuantity("egg", 1);

                    if (meatStage && (mStage == MeatsStage.liver))
                        source.Inventory.RemoveQuantity("liver", 1);

                    if (meatStage && (mStage == MeatsStage.lobstertail))
                        source.Inventory.RemoveQuantity("lobstertail", 1);

                    if (meatStage && (mStage == MeatsStage.rawmeat))
                        source.Inventory.RemoveQuantity("rawmeat", 1);

                    if (meatStage2 && (mStage2 == MeatsStage2.beef))
                        source.Inventory.RemoveQuantity("beef", 1);

                    if (meatStage2 && (mStage2 == MeatsStage2.chicken))
                        source.Inventory.RemoveQuantity("chicken", 1);

                    if (meatStage2 && (mStage2 == MeatsStage2.beefslices))
                        source.Inventory.RemoveQuantity("beefslices", 1);

                    if (meatStage2 && (mStage2 == MeatsStage2.clam))
                        source.Inventory.RemoveQuantity("clam", 1);

                    if (meatStage2 && (mStage2 == MeatsStage2.egg))
                        source.Inventory.RemoveQuantity("egg", 1);

                    if (meatStage2 && (mStage2 == MeatsStage2.liver))
                        source.Inventory.RemoveQuantity("liver", 1);

                    if (meatStage2 && (mStage2 == MeatsStage2.lobstertail))
                        source.Inventory.RemoveQuantity("lobstertail", 1);

                    if (meatStage2 && (mStage2 == MeatsStage2.rawmeat))
                        source.Inventory.RemoveQuantity("rawmeat", 1);

                    if (meatStage3 && (mStage3 == MeatsStage3.beef))
                        source.Inventory.RemoveQuantity("beef", 1);

                    if (meatStage3 && (mStage3 == MeatsStage3.chicken))
                        source.Inventory.RemoveQuantity("chicken", 1);

                    if (meatStage3 && (mStage3 == MeatsStage3.beefslices))
                        source.Inventory.RemoveQuantity("beefslices", 1);

                    if (meatStage3 && (mStage3 == MeatsStage3.clam))
                        source.Inventory.RemoveQuantity("clam", 1);

                    if (meatStage3 && (mStage3 == MeatsStage3.egg))
                        source.Inventory.RemoveQuantity("egg", 1);

                    if (meatStage3 && (mStage3 == MeatsStage3.liver))
                        source.Inventory.RemoveQuantity("liver", 1);

                    if (meatStage3 && (mStage3 == MeatsStage3.lobstertail))
                        source.Inventory.RemoveQuantity("lobstertail", 1);

                    if (meatStage3 && (mStage3 == MeatsStage3.rawmeat))
                        source.Inventory.RemoveQuantity("rawmeat", 1);

                    if (fruitsStage && (fstage == FruitsStage.cherry))
                        source.Inventory.RemoveQuantity("cherry", 15);

                    if (fruitsStage && (fstage == FruitsStage.acorn))
                        source.Inventory.RemoveQuantity("acorn", 15);

                    if (fruitsStage && (fstage == FruitsStage.apple))
                        source.Inventory.RemoveQuantity("apple", 15);

                    if (fruitsStage && (fstage == FruitsStage.greengrapes))
                        source.Inventory.RemoveQuantity("greengrapes", 15);

                    if (fruitsStage && (fstage == FruitsStage.strawberry))
                        source.Inventory.RemoveQuantity("strawberry", 15);

                    if (fruitsStage && (fstage == FruitsStage.tangerines))
                        source.Inventory.RemoveQuantity("tangerines", 15);

                    if (fruitsStage2 && (fstage2 == FruitsStage2.cherry))
                        source.Inventory.RemoveQuantity("cherry", 15);

                    if (fruitsStage2 && (fstage2 == FruitsStage2.acorn))
                        source.Inventory.RemoveQuantity("acorn", 15);

                    if (fruitsStage2 && (fstage2 == FruitsStage2.apple))
                        source.Inventory.RemoveQuantity("apple", 15);

                    if (fruitsStage2 && (fstage2 == FruitsStage2.greengrapes))
                        source.Inventory.RemoveQuantity("greengrapes", 15);

                    if (fruitsStage2 && (fstage2 == FruitsStage2.strawberry))
                        source.Inventory.RemoveQuantity("strawberry", 15);

                    if (fruitsStage2 && (fstage2 == FruitsStage2.tangerines))
                        source.Inventory.RemoveQuantity("tangerines", 15);

                    if (fruitsStage3 && (fstage3 == FruitsStage3.cherry))
                        source.Inventory.RemoveQuantity("cherry", 15);

                    if (fruitsStage3 && (fstage3 == FruitsStage3.acorn))
                        source.Inventory.RemoveQuantity("acorn", 15);

                    if (fruitsStage3 && (fstage3 == FruitsStage3.apple))
                        source.Inventory.RemoveQuantity("apple", 15);

                    if (fruitsStage3 && (fstage3 == FruitsStage3.greengrapes))
                        source.Inventory.RemoveQuantity("greengrapes", 15);

                    if (fruitsStage3 && (fstage3 == FruitsStage3.strawberry))
                        source.Inventory.RemoveQuantity("strawberry", 15);

                    if (fruitsStage3 && (fstage3 == FruitsStage3.tangerines))
                        source.Inventory.RemoveQuantity("tangerines", 15);

                    if (vegetableStage && (vStage == VegetableStage.vegetable))
                        source.Inventory.RemoveQuantity("vegetable", 5);

                    if (vegetableStage && (vStage == VegetableStage.carrot))
                        source.Inventory.RemoveQuantity("carrot", 5);

                    if (vegetableStage && (vStage == VegetableStage.rambutan))
                        source.Inventory.RemoveQuantity("rambutan", 5);

                    if (vegetableStage && (vStage == VegetableStage.tomato))
                        source.Inventory.RemoveQuantity("tomato", 5);

                    if (vegetableStage2 && (vStage2 == VegetableStage2.vegetable))
                        source.Inventory.RemoveQuantity("vegetable", 5);

                    if (vegetableStage2 && (vStage2 == VegetableStage2.carrot))
                        source.Inventory.RemoveQuantity("carrot", 5);

                    if (vegetableStage2 && (vStage2 == VegetableStage2.rambutan))
                        source.Inventory.RemoveQuantity("rambutan", 5);

                    if (vegetableStage2 && (vStage2 == VegetableStage2.tomato))
                        source.Inventory.RemoveQuantity("tomato", 5);

                    if (vegetableStage3 && (vStage3 == VegetableStage3.vegetable))
                        source.Inventory.RemoveQuantity("vegetable", 5);

                    if (vegetableStage3 && (vStage3 == VegetableStage3.carrot))
                        source.Inventory.RemoveQuantity("carrot", 5);

                    if (vegetableStage3 && (vStage3 == VegetableStage3.rambutan))
                        source.Inventory.RemoveQuantity("rambutan", 5);

                    if (vegetableStage3 && (vStage3 == VegetableStage3.tomato))
                        source.Inventory.RemoveQuantity("tomato", 5);

                    if (extraIngredients && (eStage == ExtraIngredientsStage.bread))
                        source.Inventory.RemoveQuantity("bread", 1);

                    if (extraIngredients && (eStage == ExtraIngredientsStage.cheese))
                        source.Inventory.RemoveQuantity("cheese", 1);

                    if (extraIngredients && (eStage == ExtraIngredientsStage.flour))
                        source.Inventory.RemoveQuantity("flour", 1);

                    if (extraIngredients && (eStage == ExtraIngredientsStage.marinade))
                        source.Inventory.RemoveQuantity("marinade", 1);

                    if (extraIngredients && (eStage == ExtraIngredientsStage.salt))
                        source.Inventory.RemoveQuantity("salt", 1);

                    if (extraIngredients2 && (eStage2 == ExtraIngredientsStage2.bread))
                        source.Inventory.RemoveQuantity("bread", 1);

                    if (extraIngredients2 && (eStage2 == ExtraIngredientsStage2.cheese))
                        source.Inventory.RemoveQuantity("cheese", 1);

                    if (extraIngredients2 && (eStage2 == ExtraIngredientsStage2.flour))
                        source.Inventory.RemoveQuantity("flour", 2);

                    if (extraIngredients2 && (eStage2 == ExtraIngredientsStage2.marinade))
                        source.Inventory.RemoveQuantity("marinade", 1);

                    if (extraIngredients2 && (eStage2 == ExtraIngredientsStage2.salt))
                        source.Inventory.RemoveQuantity("salt", 1);

                    if (extraIngredients3 && (eStage3 == ExtraIngredientsStage3.bread))
                        source.Inventory.RemoveQuantity("bread", 1);

                    if (extraIngredients3 && (eStage3 == ExtraIngredientsStage3.cheese))
                        source.Inventory.RemoveQuantity("cheese", 1);

                    if (extraIngredients3 && (eStage3 == ExtraIngredientsStage3.flour))
                        source.Inventory.RemoveQuantity("flour", 1);

                    if (extraIngredients3 && (eStage3 == ExtraIngredientsStage3.marinade))
                        source.Inventory.RemoveQuantity("marinade", 1);

                    if (extraIngredients3 && (eStage3 == ExtraIngredientsStage3.salt))
                        source.Inventory.RemoveQuantity("salt", 1);
                    
                    #endregion

                    source.TryGiveItems(ItemFactory.Create("sweetbuns"));
                    source.SendOrangeBarMessage("You have cooked Sweet Buns!");
                    Subject.Reply(source, "Skip", "cook_itemrepeat");
                }

                    break;
                #endregion
            }

        
        if (foodSelected && (foodStage == CookFoodStage.fruitbasket))
            switch (Subject.Template.TemplateKey.ToLower())
            {
                #region fruitadd
                case "fruit_initial":
                {
                    if (source.Inventory.HasCount("cherry", 10))
                    {
                        var item = ItemFactory.CreateFaux("cherry");
                        Subject.Items.Add(ItemDetails.BuyWithGold(item));
                    }

                    if (source.Inventory.HasCount("acorn", 10))
                    {
                        var item = ItemFactory.CreateFaux("acorn");
                        Subject.Items.Add(ItemDetails.BuyWithGold(item));
                    }

                    if (source.Inventory.HasCount("apple", 10))
                    {
                        var item = ItemFactory.CreateFaux("apple");
                        Subject.Items.Add(ItemDetails.BuyWithGold(item));
                    }

                    if (source.Inventory.HasCount("greengrapes", 10))
                    {
                        var item = ItemFactory.CreateFaux("greengrapes");
                        Subject.Items.Add(ItemDetails.BuyWithGold(item));
                    }
                    if (source.Inventory.HasCount("grape", 10))
                    {
                        var item = ItemFactory.CreateFaux("grape");
                        Subject.Items.Add(ItemDetails.BuyWithGold(item));
                    }

                    if (source.Inventory.HasCount("strawberry", 10))
                    {
                        var item = ItemFactory.CreateFaux("strawberry");
                        Subject.Items.Add(ItemDetails.BuyWithGold(item));
                    }

                    if (source.Inventory.HasCount("tangerines", 10))
                    {
                        var item = ItemFactory.CreateFaux("tangerines");
                        Subject.Items.Add(ItemDetails.BuyWithGold(item));
                    }

                    if (Subject.Items.Count == 0)
                    {
                        Subject.Reply(source, "You do not have enough fruits to use.", "cooking_initial");
                    }
                }

                    break;
                #endregion

                #region cooksweetbuns
                case "cook_item":
                {
                    #region CheckItems
                    if (fruitsStage && (fstage == FruitsStage.cherry))
                        if (!source.Inventory.HasCount("cherry", 10)) 
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (fruitsStage && (fstage == FruitsStage.acorn))
                        if (!source.Inventory.HasCount("acorn", 10)) 
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (fruitsStage && (fstage == FruitsStage.apple))
                        if (!source.Inventory.HasCount("apple", 10)) 
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (fruitsStage && (fstage == FruitsStage.greengrapes))
                        if (!source.Inventory.HasCount("greengrapes", 10)) 
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (fruitsStage && (fstage == FruitsStage.strawberry))
                        if (!source.Inventory.HasCount("strawberry", 10))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (fruitsStage && (fstage == FruitsStage.tangerines))
                        if (!source.Inventory.HasCount("tangerines", 10)) 
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (fruitsStage2 && (fstage2 == FruitsStage2.cherry))
                        if (!source.Inventory.HasCount("cherry", 10))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (fruitsStage2 && (fstage2 == FruitsStage2.acorn))
                        if (!source.Inventory.HasCount("acorn", 10))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (fruitsStage2 && (fstage2 == FruitsStage2.apple))
                        if (!source.Inventory.HasCount("apple", 10))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (fruitsStage2 && (fstage2 == FruitsStage2.greengrapes))
                        if (!source.Inventory.HasCount("greengrapes", 10)) 
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (fruitsStage2 && (fstage2 == FruitsStage2.strawberry))
                        if (!source.Inventory.HasCount("strawberry", 10)) 
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (fruitsStage2 && (fstage2 == FruitsStage2.tangerines))
                        if (!source.Inventory.HasCount("tangerines", 10)) 
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (fruitsStage3 && (fstage3 == FruitsStage3.cherry))
                        if (!source.Inventory.HasCount("cherry", 10)) 
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (fruitsStage3 && (fstage3 == FruitsStage3.acorn))
                        if (!source.Inventory.HasCount("acorn", 10))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (fruitsStage3 && (fstage3 == FruitsStage3.apple))
                        if (!source.Inventory.HasCount("apple", 10)) 
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (fruitsStage3 && (fstage3 == FruitsStage3.greengrapes))
                        if (!source.Inventory.HasCount("greengrapes", 10))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (fruitsStage3 && (fstage3 == FruitsStage3.strawberry))
                        if (!source.Inventory.HasCount("strawberry", 10)) 
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (fruitsStage3 && (fstage3 == FruitsStage3.tangerines))
                        if (!source.Inventory.HasCount("tangerines", 10)) 
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);
                    

                    if (source.Trackers.Flags.HasFlag(CookFoodProgression.NotenoughIngredients))
                    {
                        Subject.Reply(source, "You don't have enough ingredients to cook this again.", "cooking_initial");
                        source.Trackers.Flags.RemoveFlag(CookFoodProgression.NotenoughIngredients);

                        return;
                    }
                    #endregion
                    #region RemoveItems

                    if (fruitsStage && (fstage == FruitsStage.cherry))
                        source.Inventory.RemoveQuantity("cherry", 10);

                    if (fruitsStage && (fstage == FruitsStage.acorn))
                        source.Inventory.RemoveQuantity("acorn", 10);

                    if (fruitsStage && (fstage == FruitsStage.apple))
                        source.Inventory.RemoveQuantity("apple", 10);

                    if (fruitsStage && (fstage == FruitsStage.greengrapes))
                        source.Inventory.RemoveQuantity("greengrapes", 10);

                    if (fruitsStage && (fstage == FruitsStage.strawberry))
                        source.Inventory.RemoveQuantity("strawberry", 10);

                    if (fruitsStage && (fstage == FruitsStage.tangerines))
                        source.Inventory.RemoveQuantity("tangerines", 10);

                    if (fruitsStage2 && (fstage2 == FruitsStage2.cherry))
                        source.Inventory.RemoveQuantity("cherry", 10);

                    if (fruitsStage2 && (fstage2 == FruitsStage2.acorn))
                        source.Inventory.RemoveQuantity("acorn", 10);

                    if (fruitsStage2 && (fstage2 == FruitsStage2.apple))
                        source.Inventory.RemoveQuantity("apple", 10);

                    if (fruitsStage2 && (fstage2 == FruitsStage2.greengrapes))
                        source.Inventory.RemoveQuantity("greengrapes", 10);

                    if (fruitsStage2 && (fstage2 == FruitsStage2.strawberry))
                        source.Inventory.RemoveQuantity("strawberry", 10);

                    if (fruitsStage2 && (fstage2 == FruitsStage2.tangerines))
                        source.Inventory.RemoveQuantity("tangerines", 10);

                    if (fruitsStage3 && (fstage3 == FruitsStage3.cherry))
                        source.Inventory.RemoveQuantity("cherry", 10);

                    if (fruitsStage3 && (fstage3 == FruitsStage3.acorn))
                        source.Inventory.RemoveQuantity("acorn", 10);

                    if (fruitsStage3 && (fstage3 == FruitsStage3.apple))
                        source.Inventory.RemoveQuantity("apple", 10);

                    if (fruitsStage3 && (fstage3 == FruitsStage3.greengrapes))
                        source.Inventory.RemoveQuantity("greengrapes", 10);

                    if (fruitsStage3 && (fstage3 == FruitsStage3.strawberry))
                        source.Inventory.RemoveQuantity("strawberry", 10);

                    if (fruitsStage3 && (fstage3 == FruitsStage3.tangerines))
                        source.Inventory.RemoveQuantity("tangerines", 10);

                    #endregion

                    source.TryGiveItems(ItemFactory.Create("fruitbasket"));
                    source.SendOrangeBarMessage("You have made a Fruit Basket!");
                    Subject.Reply(source, "Skip", "cook_itemrepeat");
                }

                    break;
                #endregion
            }
    }
}
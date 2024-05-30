using Chaos.Common.Utilities;
using Chaos.Definitions;
using Chaos.Models.Data;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.DialogScripts.Crafting;

public class CookItemScript : DialogScriptBase
{
    private readonly IItemFactory ItemFactory;
    public int BuyCost { get; init; }
    protected HashSet<string>? ItemTemplateKeys { get; init; }

    public CookItemScript(
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
                    if (source.Inventory.HasCountByTemplateKey("beefslices", 1))
                    {
                        var item = ItemFactory.CreateFaux("beefslices");
                        Subject.Items.Add(ItemDetails.DisplayRecipe(item));
                    }

                    if (source.Inventory.HasCountByTemplateKey("clam", 1))
                    {
                        var item = ItemFactory.CreateFaux("clam");
                        Subject.Items.Add(ItemDetails.DisplayRecipe(item));
                    }

                    if (source.Inventory.HasCountByTemplateKey("egg", 1))
                    {
                        var item = ItemFactory.CreateFaux("egg");
                        Subject.Items.Add(ItemDetails.DisplayRecipe(item));
                    }

                    if (source.Inventory.HasCountByTemplateKey("liver", 1))
                    {
                        var item = ItemFactory.CreateFaux("liver");
                        Subject.Items.Add(ItemDetails.DisplayRecipe(item));
                    }

                    if (source.Inventory.HasCountByTemplateKey("lobstertail", 1))
                    {
                        var item = ItemFactory.CreateFaux("lobstertail");
                        Subject.Items.Add(ItemDetails.DisplayRecipe(item));
                    }

                    if (source.Inventory.HasCountByTemplateKey("rawmeat", 1))
                    {
                        var item = ItemFactory.CreateFaux("rawmeat");
                        Subject.Items.Add(ItemDetails.DisplayRecipe(item));
                    }

                    if (Subject.Items.Count == 0)
                    {
                        Subject.Reply(source, "You do not have the required meats.", "cooking_initial");

                        return;
                    }
                }

                    break;
                #endregion

                #region fruitadd
                case "fruit_initial":
                {
                    if (source.Inventory.HasCountByTemplateKey("cherry", 10))
                    {
                        var item = ItemFactory.CreateFaux("cherry");
                        Subject.Items.Add(ItemDetails.DisplayRecipe(item));
                    }

                    if (source.Inventory.HasCountByTemplateKey("acorn", 10))
                    {
                        var item = ItemFactory.CreateFaux("acorn");
                        Subject.Items.Add(ItemDetails.DisplayRecipe(item));
                    }

                    if (source.Inventory.HasCountByTemplateKey("apple", 10))
                    {
                        var item = ItemFactory.CreateFaux("apple");
                        Subject.Items.Add(ItemDetails.DisplayRecipe(item));
                    }

                    if (source.Inventory.HasCountByTemplateKey("greengrapes", 10))
                    {
                        var item = ItemFactory.CreateFaux("greengrapes");
                        Subject.Items.Add(ItemDetails.DisplayRecipe(item));
                    }

                    if (source.Inventory.HasCountByTemplateKey("grape", 10))
                    {
                        var item = ItemFactory.CreateFaux("grape");
                        Subject.Items.Add(ItemDetails.DisplayRecipe(item));
                    }

                    if (source.Inventory.HasCountByTemplateKey("strawberry", 10))
                    {
                        var item = ItemFactory.CreateFaux("strawberry");
                        Subject.Items.Add(ItemDetails.DisplayRecipe(item));
                    }

                    if (source.Inventory.HasCountByTemplateKey("tangerines", 10))
                    {
                        var item = ItemFactory.CreateFaux("tangerines");
                        Subject.Items.Add(ItemDetails.DisplayRecipe(item));
                    }

                    if (Subject.Items.Count == 0)
                    {
                        Subject.Reply(source, "You do not have the required fruits.", "cooking_initial");

                        return;
                    }
                }

                    break;
                #endregion

                #region vegetableadd
                case "vegetable_initial":
                {
                    if (source.Inventory.HasCountByTemplateKey("vegetable", 5))
                    {
                        var item = ItemFactory.CreateFaux("vegetable");
                        Subject.Items.Add(ItemDetails.DisplayRecipe(item));
                    }

                    if (source.Inventory.HasCountByTemplateKey("carrot", 5))
                    {
                        var item = ItemFactory.CreateFaux("carrot");
                        Subject.Items.Add(ItemDetails.DisplayRecipe(item));
                    }

                    if (source.Inventory.HasCountByTemplateKey("rambutan", 5))
                    {
                        var item = ItemFactory.CreateFaux("rambutan");
                        Subject.Items.Add(ItemDetails.DisplayRecipe(item));
                    }

                    if (source.Inventory.HasCountByTemplateKey("tomato", 5))
                    {
                        var item = ItemFactory.CreateFaux("tomato");
                        Subject.Items.Add(ItemDetails.DisplayRecipe(item));
                    }

                    if (Subject.Items.Count == 0)
                    {
                        Subject.Reply(source, "You do not have the required vegetables.", "cooking_initial");

                        return;
                    }
                }

                    break;
                #endregion

                #region addextraingredients
                case "extraingredients_initial":
                {
                    if (source.Inventory.HasCountByTemplateKey("flour", 1))
                    {
                        var item = ItemFactory.CreateFaux("flour");
                        Subject.Items.Add(ItemDetails.DisplayRecipe(item));
                    }

                    if (source.Inventory.HasCountByTemplateKey("bread", 1))
                    {
                        var item = ItemFactory.CreateFaux("bread");
                        Subject.Items.Add(ItemDetails.DisplayRecipe(item));
                    }

                    if (source.Inventory.HasCountByTemplateKey("cheese", 1))
                    {
                        var item = ItemFactory.CreateFaux("cheese");
                        Subject.Items.Add(ItemDetails.DisplayRecipe(item));
                    }

                    if (source.Inventory.HasCountByTemplateKey("marinade", 1))
                    {
                        var item = ItemFactory.CreateFaux("marinade");
                        Subject.Items.Add(ItemDetails.DisplayRecipe(item));
                    }

                    if (source.Inventory.HasCountByTemplateKey("salt", 1))
                    {
                        var item = ItemFactory.CreateFaux("salt");
                        Subject.Items.Add(ItemDetails.DisplayRecipe(item));
                    }

                    if (Subject.Items.Count == 0)
                    {
                        Subject.Reply(source, "You do not have the required extra ingredients.", "cooking_initial");

                        return;
                    }
                }

                    break;
                #endregion

                #region cookdinnerplate
                case "cook_item":
                {
                    #region CheckItems
                    if (meatStage && (mStage == MeatsStage.beefslices))
                        if (!source.Inventory.HasCountByTemplateKey("beefslices", 1))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (meatStage && (mStage == MeatsStage.clam))
                        if (!source.Inventory.HasCountByTemplateKey("clam", 1))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (meatStage && (mStage == MeatsStage.egg))
                        if (!source.Inventory.HasCountByTemplateKey("egg", 1))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (meatStage && (mStage == MeatsStage.liver))
                        if (!source.Inventory.HasCountByTemplateKey("liver", 1))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (meatStage && (mStage == MeatsStage.lobstertail))
                        if (!source.Inventory.HasCountByTemplateKey("lobstertail", 1))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (meatStage && (mStage == MeatsStage.rawmeat))
                        if (!source.Inventory.HasCountByTemplateKey("rawmeat", 1))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (meatStage2 && (mStage2 == MeatsStage2.beefslices))
                        if (!source.Inventory.HasCountByTemplateKey("beefslices", 1))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (meatStage2 && (mStage2 == MeatsStage2.clam))
                        if (!source.Inventory.HasCountByTemplateKey("clam", 1))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (meatStage2 && (mStage2 == MeatsStage2.egg))
                        if (!source.Inventory.HasCountByTemplateKey("egg", 1))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (meatStage2 && (mStage2 == MeatsStage2.liver))
                        if (!source.Inventory.HasCountByTemplateKey("liver", 1))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (meatStage2 && (mStage2 == MeatsStage2.lobstertail))
                        if (!source.Inventory.HasCountByTemplateKey("lobstertail", 1))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (meatStage2 && (mStage2 == MeatsStage2.rawmeat))
                        if (!source.Inventory.HasCountByTemplateKey("rawmeat", 1))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (meatStage3 && (mStage3 == MeatsStage3.beefslices))
                        if (!source.Inventory.HasCountByTemplateKey("beefslices", 1))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (meatStage3 && (mStage3 == MeatsStage3.clam))
                        if (!source.Inventory.HasCountByTemplateKey("clam", 1))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (meatStage3 && (mStage3 == MeatsStage3.egg))
                        if (!source.Inventory.HasCountByTemplateKey("egg", 1))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (meatStage3 && (mStage3 == MeatsStage3.liver))
                        if (!source.Inventory.HasCountByTemplateKey("liver", 1))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (meatStage3 && (mStage3 == MeatsStage3.lobstertail))
                        if (!source.Inventory.HasCountByTemplateKey("lobstertail", 1))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (meatStage3 && (mStage3 == MeatsStage3.rawmeat))
                        if (!source.Inventory.HasCountByTemplateKey("rawmeat", 1))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (fruitsStage && (fstage == FruitsStage.cherry))
                        if (!source.Inventory.HasCountByTemplateKey("cherry", 10))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (fruitsStage && (fstage == FruitsStage.grape))
                        if (!source.Inventory.HasCountByTemplateKey("grape", 10))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (fruitsStage && (fstage == FruitsStage.acorn))
                        if (!source.Inventory.HasCountByTemplateKey("acorn", 10))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (fruitsStage && (fstage == FruitsStage.apple))
                        if (!source.Inventory.HasCountByTemplateKey("apple", 10))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (fruitsStage && (fstage == FruitsStage.greengrapes))
                        if (!source.Inventory.HasCountByTemplateKey("greengrapes", 10))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (fruitsStage && (fstage == FruitsStage.strawberry))
                        if (!source.Inventory.HasCountByTemplateKey("strawberry", 10))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (fruitsStage && (fstage == FruitsStage.tangerines))
                        if (!source.Inventory.HasCountByTemplateKey("tangerines", 10))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (fruitsStage2 && (fstage2 == FruitsStage2.cherry))
                        if (!source.Inventory.HasCountByTemplateKey("cherry", 10))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (fruitsStage2 && (fstage2 == FruitsStage2.grape))
                        if (!source.Inventory.HasCountByTemplateKey("grape", 10))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (fruitsStage2 && (fstage2 == FruitsStage2.acorn))
                        if (!source.Inventory.HasCountByTemplateKey("acorn", 10))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (fruitsStage2 && (fstage2 == FruitsStage2.apple))
                        if (!source.Inventory.HasCountByTemplateKey("apple", 10))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (fruitsStage2 && (fstage2 == FruitsStage2.greengrapes))
                        if (!source.Inventory.HasCountByTemplateKey("greengrapes", 10))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (fruitsStage2 && (fstage2 == FruitsStage2.strawberry))
                        if (!source.Inventory.HasCountByTemplateKey("strawberry", 10))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (fruitsStage2 && (fstage2 == FruitsStage2.tangerines))
                        if (!source.Inventory.HasCountByTemplateKey("tangerines", 10))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (fruitsStage3 && (fstage3 == FruitsStage3.cherry))
                        if (!source.Inventory.HasCountByTemplateKey("cherry", 10))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (fruitsStage3 && (fstage3 == FruitsStage3.grape))
                        if (!source.Inventory.HasCountByTemplateKey("grape", 10))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (fruitsStage3 && (fstage3 == FruitsStage3.acorn))
                        if (!source.Inventory.HasCountByTemplateKey("acorn", 10))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (fruitsStage3 && (fstage3 == FruitsStage3.apple))
                        if (!source.Inventory.HasCountByTemplateKey("apple", 10))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (fruitsStage3 && (fstage3 == FruitsStage3.greengrapes))
                        if (!source.Inventory.HasCountByTemplateKey("greengrapes", 10))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (fruitsStage3 && (fstage3 == FruitsStage3.strawberry))
                        if (!source.Inventory.HasCountByTemplateKey("strawberry", 10))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (fruitsStage3 && (fstage3 == FruitsStage3.tangerines))
                        if (!source.Inventory.HasCountByTemplateKey("tangerines", 10))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (vegetableStage && (vStage == VegetableStage.vegetable))
                        if (!source.Inventory.HasCountByTemplateKey("vegetable", 5))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (vegetableStage && (vStage == VegetableStage.carrot))
                        if (!source.Inventory.HasCountByTemplateKey("carrot", 5))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (vegetableStage && (vStage == VegetableStage.rambutan))
                        if (!source.Inventory.HasCountByTemplateKey("rambutan", 5))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (vegetableStage && (vStage == VegetableStage.tomato))
                        if (!source.Inventory.HasCountByTemplateKey("tomato", 5))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (vegetableStage2 && (vStage2 == VegetableStage2.vegetable))
                        if (!source.Inventory.HasCountByTemplateKey("vegetable", 5))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (vegetableStage2 && (vStage2 == VegetableStage2.carrot))
                        if (!source.Inventory.HasCountByTemplateKey("carrot", 5))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (vegetableStage2 && (vStage2 == VegetableStage2.rambutan))
                        if (!source.Inventory.HasCountByTemplateKey("rambutan", 5))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (vegetableStage2 && (vStage2 == VegetableStage2.tomato))
                        if (!source.Inventory.HasCountByTemplateKey("tomato", 5))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (vegetableStage3 && (vStage3 == VegetableStage3.vegetable))
                        if (!source.Inventory.HasCountByTemplateKey("vegetable", 5))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (vegetableStage3 && (vStage3 == VegetableStage3.carrot))
                        if (!source.Inventory.HasCountByTemplateKey("carrot", 5))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (vegetableStage3 && (vStage3 == VegetableStage3.rambutan))
                        if (!source.Inventory.HasCountByTemplateKey("rambutan", 5))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (vegetableStage3 && (vStage3 == VegetableStage3.tomato))
                        if (!source.Inventory.HasCountByTemplateKey("tomato", 5))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (extraIngredients && (eStage == ExtraIngredientsStage.bread))
                        if (!source.Inventory.HasCountByTemplateKey("bread", 1))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (extraIngredients && (eStage == ExtraIngredientsStage.cheese))
                        if (!source.Inventory.HasCountByTemplateKey("cheese", 1))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (extraIngredients && (eStage == ExtraIngredientsStage.flour))
                        if (!source.Inventory.HasCountByTemplateKey("flour", 1))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (extraIngredients && (eStage == ExtraIngredientsStage.marinade))
                        if (!source.Inventory.HasCountByTemplateKey("marinade", 1))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (extraIngredients && (eStage == ExtraIngredientsStage.salt))
                        if (!source.Inventory.HasCountByTemplateKey("salt", 1))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (extraIngredients2 && (eStage2 == ExtraIngredientsStage2.bread))
                        if (!source.Inventory.HasCountByTemplateKey("bread", 1))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (extraIngredients2 && (eStage2 == ExtraIngredientsStage2.cheese))
                        if (!source.Inventory.HasCountByTemplateKey("cheese", 1))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (extraIngredients2 && (eStage2 == ExtraIngredientsStage2.flour))
                        if (!source.Inventory.HasCountByTemplateKey("flour", 1))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (extraIngredients2 && (eStage2 == ExtraIngredientsStage2.marinade))
                        if (!source.Inventory.HasCountByTemplateKey("marinade", 1))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (extraIngredients2 && (eStage2 == ExtraIngredientsStage2.salt))
                        if (!source.Inventory.HasCountByTemplateKey("salt", 1))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (extraIngredients3 && (eStage3 == ExtraIngredientsStage3.bread))
                        if (!source.Inventory.HasCountByTemplateKey("bread", 1))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (extraIngredients3 && (eStage3 == ExtraIngredientsStage3.cheese))
                        if (!source.Inventory.HasCountByTemplateKey("cheese", 1))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (extraIngredients3 && (eStage3 == ExtraIngredientsStage3.flour))
                        if (!source.Inventory.HasCountByTemplateKey("flour", 1))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (extraIngredients3 && (eStage3 == ExtraIngredientsStage3.marinade))
                        if (!source.Inventory.HasCountByTemplateKey("marinade", 1))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (extraIngredients3 && (eStage3 == ExtraIngredientsStage3.salt))
                        if (!source.Inventory.HasCountByTemplateKey("salt", 1))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (source.Trackers.Flags.HasFlag(CookFoodProgression.NotenoughIngredients))
                    {
                        Subject.Reply(source, "You don't have enough ingredients to cook this again.", "cooking_initial");
                        source.Trackers.Flags.RemoveFlag(CookFoodProgression.NotenoughIngredients);

                        return;
                    }
                    #endregion

                    #region RemoveItems
                    if (meatStage && (mStage == MeatsStage.beefslices))
                        source.Inventory.RemoveQuantityByTemplateKey("beefslices", 1);

                    if (meatStage && (mStage == MeatsStage.clam))
                        source.Inventory.RemoveQuantityByTemplateKey("clam", 1);

                    if (meatStage && (mStage == MeatsStage.egg))
                        source.Inventory.RemoveQuantityByTemplateKey("egg", 1);

                    if (meatStage && (mStage == MeatsStage.liver))
                        source.Inventory.RemoveQuantityByTemplateKey("liver", 1);

                    if (meatStage && (mStage == MeatsStage.lobstertail))
                        source.Inventory.RemoveQuantityByTemplateKey("lobstertail", 1);

                    if (meatStage && (mStage == MeatsStage.rawmeat))
                        source.Inventory.RemoveQuantityByTemplateKey("rawmeat", 1);

                    if (meatStage2 && (mStage2 == MeatsStage2.beefslices))
                        source.Inventory.RemoveQuantityByTemplateKey("beefslices", 1);

                    if (meatStage2 && (mStage2 == MeatsStage2.clam))
                        source.Inventory.RemoveQuantityByTemplateKey("clam", 1);

                    if (meatStage2 && (mStage2 == MeatsStage2.egg))
                        source.Inventory.RemoveQuantityByTemplateKey("egg", 1);

                    if (meatStage2 && (mStage2 == MeatsStage2.liver))
                        source.Inventory.RemoveQuantityByTemplateKey("liver", 1);

                    if (meatStage2 && (mStage2 == MeatsStage2.lobstertail))
                        source.Inventory.RemoveQuantityByTemplateKey("lobstertail", 1);

                    if (meatStage2 && (mStage2 == MeatsStage2.rawmeat))
                        source.Inventory.RemoveQuantityByTemplateKey("rawmeat", 1);

                    if (meatStage3 && (mStage3 == MeatsStage3.beefslices))
                        source.Inventory.RemoveQuantityByTemplateKey("beefslices", 1);

                    if (meatStage3 && (mStage3 == MeatsStage3.clam))
                        source.Inventory.RemoveQuantityByTemplateKey("clam", 1);

                    if (meatStage3 && (mStage3 == MeatsStage3.egg))
                        source.Inventory.RemoveQuantityByTemplateKey("egg", 1);

                    if (meatStage3 && (mStage3 == MeatsStage3.liver))
                        source.Inventory.RemoveQuantityByTemplateKey("liver", 1);

                    if (meatStage3 && (mStage3 == MeatsStage3.lobstertail))
                        source.Inventory.RemoveQuantityByTemplateKey("lobstertail", 1);

                    if (meatStage3 && (mStage3 == MeatsStage3.rawmeat))
                        source.Inventory.RemoveQuantityByTemplateKey("rawmeat", 1);

                    if (fruitsStage && (fstage == FruitsStage.cherry))
                        source.Inventory.RemoveQuantityByTemplateKey("cherry", 10);

                    if (fruitsStage && (fstage == FruitsStage.grape))
                        source.Inventory.RemoveQuantityByTemplateKey("grape", 10);

                    if (fruitsStage && (fstage == FruitsStage.acorn))
                        source.Inventory.RemoveQuantityByTemplateKey("acorn", 10);

                    if (fruitsStage && (fstage == FruitsStage.apple))
                        source.Inventory.RemoveQuantityByTemplateKey("apple", 10);

                    if (fruitsStage && (fstage == FruitsStage.greengrapes))
                        source.Inventory.RemoveQuantityByTemplateKey("greengrapes", 10);

                    if (fruitsStage && (fstage == FruitsStage.strawberry))
                        source.Inventory.RemoveQuantityByTemplateKey("strawberry", 10);

                    if (fruitsStage && (fstage == FruitsStage.tangerines))
                        source.Inventory.RemoveQuantityByTemplateKey("tangerines", 10);

                    if (fruitsStage2 && (fstage2 == FruitsStage2.cherry))
                        source.Inventory.RemoveQuantityByTemplateKey("cherry", 10);

                    if (fruitsStage2 && (fstage2 == FruitsStage2.grape))
                        source.Inventory.RemoveQuantityByTemplateKey("grape", 10);

                    if (fruitsStage2 && (fstage2 == FruitsStage2.acorn))
                        source.Inventory.RemoveQuantityByTemplateKey("acorn", 10);

                    if (fruitsStage2 && (fstage2 == FruitsStage2.apple))
                        source.Inventory.RemoveQuantityByTemplateKey("apple", 10);

                    if (fruitsStage2 && (fstage2 == FruitsStage2.greengrapes))
                        source.Inventory.RemoveQuantityByTemplateKey("greengrapes", 10);

                    if (fruitsStage2 && (fstage2 == FruitsStage2.strawberry))
                        source.Inventory.RemoveQuantityByTemplateKey("strawberry", 10);

                    if (fruitsStage2 && (fstage2 == FruitsStage2.tangerines))
                        source.Inventory.RemoveQuantityByTemplateKey("tangerines", 10);

                    if (fruitsStage3 && (fstage3 == FruitsStage3.cherry))
                        source.Inventory.RemoveQuantityByTemplateKey("cherry", 10);

                    if (fruitsStage3 && (fstage3 == FruitsStage3.grape))
                        source.Inventory.RemoveQuantityByTemplateKey("grape", 10);

                    if (fruitsStage3 && (fstage3 == FruitsStage3.acorn))
                        source.Inventory.RemoveQuantityByTemplateKey("acorn", 10);

                    if (fruitsStage3 && (fstage3 == FruitsStage3.apple))
                        source.Inventory.RemoveQuantityByTemplateKey("apple", 10);

                    if (fruitsStage3 && (fstage3 == FruitsStage3.greengrapes))
                        source.Inventory.RemoveQuantityByTemplateKey("greengrapes", 10);

                    if (fruitsStage3 && (fstage3 == FruitsStage3.strawberry))
                        source.Inventory.RemoveQuantityByTemplateKey("strawberry", 10);

                    if (fruitsStage3 && (fstage3 == FruitsStage3.tangerines))
                        source.Inventory.RemoveQuantityByTemplateKey("tangerines", 10);

                    if (vegetableStage && (vStage == VegetableStage.vegetable))
                        source.Inventory.RemoveQuantityByTemplateKey("vegetable", 5);

                    if (vegetableStage && (vStage == VegetableStage.carrot))
                        source.Inventory.RemoveQuantityByTemplateKey("carrot", 5);

                    if (vegetableStage && (vStage == VegetableStage.rambutan))
                        source.Inventory.RemoveQuantityByTemplateKey("rambutan", 5);

                    if (vegetableStage && (vStage == VegetableStage.tomato))
                        source.Inventory.RemoveQuantityByTemplateKey("tomato", 5);

                    if (vegetableStage2 && (vStage2 == VegetableStage2.vegetable))
                        source.Inventory.RemoveQuantityByTemplateKey("vegetable", 5);

                    if (vegetableStage2 && (vStage2 == VegetableStage2.carrot))
                        source.Inventory.RemoveQuantityByTemplateKey("carrot", 5);

                    if (vegetableStage2 && (vStage2 == VegetableStage2.rambutan))
                        source.Inventory.RemoveQuantityByTemplateKey("rambutan", 5);

                    if (vegetableStage2 && (vStage2 == VegetableStage2.tomato))
                        source.Inventory.RemoveQuantityByTemplateKey("tomato", 5);

                    if (vegetableStage3 && (vStage3 == VegetableStage3.vegetable))
                        source.Inventory.RemoveQuantityByTemplateKey("vegetable", 5);

                    if (vegetableStage3 && (vStage3 == VegetableStage3.carrot))
                        source.Inventory.RemoveQuantityByTemplateKey("carrot", 5);

                    if (vegetableStage3 && (vStage3 == VegetableStage3.rambutan))
                        source.Inventory.RemoveQuantityByTemplateKey("rambutan", 5);

                    if (vegetableStage3 && (vStage3 == VegetableStage3.tomato))
                        source.Inventory.RemoveQuantityByTemplateKey("tomato", 5);

                    if (extraIngredients && (eStage == ExtraIngredientsStage.bread))
                        source.Inventory.RemoveQuantityByTemplateKey("bread", 1);

                    if (extraIngredients && (eStage == ExtraIngredientsStage.cheese))
                        source.Inventory.RemoveQuantityByTemplateKey("cheese", 1);

                    if (extraIngredients && (eStage == ExtraIngredientsStage.flour))
                        source.Inventory.RemoveQuantityByTemplateKey("flour", 1);

                    if (extraIngredients && (eStage == ExtraIngredientsStage.marinade))
                        source.Inventory.RemoveQuantityByTemplateKey("marinade", 1);

                    if (extraIngredients && (eStage == ExtraIngredientsStage.salt))
                        source.Inventory.RemoveQuantityByTemplateKey("salt", 1);

                    if (extraIngredients2 && (eStage2 == ExtraIngredientsStage2.bread))
                        source.Inventory.RemoveQuantityByTemplateKey("bread", 1);

                    if (extraIngredients2 && (eStage2 == ExtraIngredientsStage2.cheese))
                        source.Inventory.RemoveQuantityByTemplateKey("cheese", 1);

                    if (extraIngredients2 && (eStage2 == ExtraIngredientsStage2.flour))
                        source.Inventory.RemoveQuantityByTemplateKey("flour", 1);

                    if (extraIngredients2 && (eStage2 == ExtraIngredientsStage2.marinade))
                        source.Inventory.RemoveQuantityByTemplateKey("marinade", 1);

                    if (extraIngredients2 && (eStage2 == ExtraIngredientsStage2.salt))
                        source.Inventory.RemoveQuantityByTemplateKey("salt", 1);

                    if (extraIngredients3 && (eStage3 == ExtraIngredientsStage3.bread))
                        source.Inventory.RemoveQuantityByTemplateKey("bread", 1);

                    if (extraIngredients3 && (eStage3 == ExtraIngredientsStage3.cheese))
                        source.Inventory.RemoveQuantityByTemplateKey("cheese", 1);

                    if (extraIngredients3 && (eStage3 == ExtraIngredientsStage3.flour))
                        source.Inventory.RemoveQuantityByTemplateKey("flour", 1);

                    if (extraIngredients3 && (eStage3 == ExtraIngredientsStage3.marinade))
                        source.Inventory.RemoveQuantityByTemplateKey("marinade", 1);

                    if (extraIngredients3 && (eStage3 == ExtraIngredientsStage3.salt))
                        source.Inventory.RemoveQuantityByTemplateKey("salt", 1);
                    #endregion

                    if (IntegerRandomizer.RollChance(20))
                    {
                        Subject.Reply(source, "Skip", "cookfailed_itemrepeat");
                        source.SendOrangeBarMessage("The ingredients you used were rotten.");
                        source.SendOrangeBarMessage("The ingredients you used were rotten.");

                        return;
                    }

                    source.GiveItemOrSendToBank(ItemFactory.Create("dinnerplate"));
                    source.SendOrangeBarMessage("You have cooked a Dinner Plate!");
                    Subject.Reply(source, "Skip", "cook_itemrepeat");
                }

                    break;
                #endregion
            }

        if (foodSelected && (foodStage == CookFoodStage.sweetbuns))
            switch (Subject.Template.TemplateKey.ToLower())
            {
                #region fruitadd
                case "fruit_initial":
                {
                    if (source.Inventory.HasCountByTemplateKey("cherry", 15))
                    {
                        var item = ItemFactory.CreateFaux("cherry");
                        Subject.Items.Add(ItemDetails.DisplayRecipe(item));
                    }

                    if (source.Inventory.HasCountByTemplateKey("acorn", 15))
                    {
                        var item = ItemFactory.CreateFaux("acorn");
                        Subject.Items.Add(ItemDetails.DisplayRecipe(item));
                    }

                    if (source.Inventory.HasCountByTemplateKey("apple", 15))
                    {
                        var item = ItemFactory.CreateFaux("apple");
                        Subject.Items.Add(ItemDetails.DisplayRecipe(item));
                    }

                    if (source.Inventory.HasCountByTemplateKey("greengrapes", 15))
                    {
                        var item = ItemFactory.CreateFaux("greengrapes");
                        Subject.Items.Add(ItemDetails.DisplayRecipe(item));
                    }

                    if (source.Inventory.HasCountByTemplateKey("grape", 15))
                    {
                        var item = ItemFactory.CreateFaux("grape");
                        Subject.Items.Add(ItemDetails.DisplayRecipe(item));
                    }

                    if (source.Inventory.HasCountByTemplateKey("strawberry", 15))
                    {
                        var item = ItemFactory.CreateFaux("strawberry");
                        Subject.Items.Add(ItemDetails.DisplayRecipe(item));
                    }

                    if (source.Inventory.HasCountByTemplateKey("tangerines", 15))
                    {
                        var item = ItemFactory.CreateFaux("tangerines");
                        Subject.Items.Add(ItemDetails.DisplayRecipe(item));
                    }

                    if (Subject.Items.Count == 0)
                    {
                        Subject.Reply(source, "You do not have the required fruits.", "cooking_initial");

                        return;
                    }
                }

                    break;
                #endregion

                #region vegetableadd
                case "vegetable_initial":
                {
                    if (source.Inventory.HasCountByTemplateKey("vegetable", 5))
                    {
                        var item = ItemFactory.CreateFaux("vegetable");
                        Subject.Items.Add(ItemDetails.DisplayRecipe(item));
                    }

                    if (source.Inventory.HasCountByTemplateKey("carrot", 5))
                    {
                        var item = ItemFactory.CreateFaux("carrot");
                        Subject.Items.Add(ItemDetails.DisplayRecipe(item));
                    }

                    if (source.Inventory.HasCountByTemplateKey("rambutan", 5))
                    {
                        var item = ItemFactory.CreateFaux("rambutan");
                        Subject.Items.Add(ItemDetails.DisplayRecipe(item));
                    }

                    if (source.Inventory.HasCountByTemplateKey("tomato", 5))
                    {
                        var item = ItemFactory.CreateFaux("tomato");
                        Subject.Items.Add(ItemDetails.DisplayRecipe(item));
                    }

                    if (Subject.Items.Count == 0)
                    {
                        Subject.Reply(source, "You do not have the required vegetables.", "cooking_initial");

                        return;
                    }
                }

                    break;
                #endregion

                #region addextraingredients
                case "extraingredients_initial":
                {
                    if (source.Inventory.HasCountByTemplateKey("flour", 1))
                    {
                        var item = ItemFactory.CreateFaux("flour");
                        Subject.Items.Add(ItemDetails.DisplayRecipe(item));
                    }

                    if (Subject.Items.Count == 0)
                    {
                        Subject.Reply(source, "You do not have the required extra ingredients.", "cooking_initial");

                        return;
                    }
                }

                    break;
                #endregion

                #region cooksweetbuns
                case "cook_item":
                {
                    #region CheckItems
                    if (fruitsStage && (fstage == FruitsStage.cherry))
                        if (!source.Inventory.HasCountByTemplateKey("cherry", 10))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (fruitsStage && (fstage == FruitsStage.grape))
                        if (!source.Inventory.HasCountByTemplateKey("grape", 10))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (fruitsStage && (fstage == FruitsStage.acorn))
                        if (!source.Inventory.HasCountByTemplateKey("acorn", 10))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (fruitsStage && (fstage == FruitsStage.apple))
                        if (!source.Inventory.HasCountByTemplateKey("apple", 10))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (fruitsStage && (fstage == FruitsStage.greengrapes))
                        if (!source.Inventory.HasCountByTemplateKey("greengrapes", 10))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (fruitsStage && (fstage == FruitsStage.strawberry))
                        if (!source.Inventory.HasCountByTemplateKey("strawberry", 10))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (fruitsStage && (fstage == FruitsStage.tangerines))
                        if (!source.Inventory.HasCountByTemplateKey("tangerines", 10))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (fruitsStage2 && (fstage2 == FruitsStage2.cherry))
                        if (!source.Inventory.HasCountByTemplateKey("cherry", 10))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (fruitsStage2 && (fstage2 == FruitsStage2.grape))
                        if (!source.Inventory.HasCountByTemplateKey("grape", 10))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (fruitsStage2 && (fstage2 == FruitsStage2.acorn))
                        if (!source.Inventory.HasCountByTemplateKey("acorn", 10))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (fruitsStage2 && (fstage2 == FruitsStage2.apple))
                        if (!source.Inventory.HasCountByTemplateKey("apple", 10))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (fruitsStage2 && (fstage2 == FruitsStage2.greengrapes))
                        if (!source.Inventory.HasCountByTemplateKey("greengrapes", 10))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (fruitsStage2 && (fstage2 == FruitsStage2.strawberry))
                        if (!source.Inventory.HasCountByTemplateKey("strawberry", 10))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (fruitsStage2 && (fstage2 == FruitsStage2.tangerines))
                        if (!source.Inventory.HasCountByTemplateKey("tangerines", 10))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (fruitsStage3 && (fstage3 == FruitsStage3.cherry))
                        if (!source.Inventory.HasCountByTemplateKey("cherry", 10))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (fruitsStage3 && (fstage3 == FruitsStage3.grape))
                        if (!source.Inventory.HasCountByTemplateKey("grape", 10))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (fruitsStage3 && (fstage3 == FruitsStage3.acorn))
                        if (!source.Inventory.HasCountByTemplateKey("acorn", 10))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (fruitsStage3 && (fstage3 == FruitsStage3.apple))
                        if (!source.Inventory.HasCountByTemplateKey("apple", 10))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (fruitsStage3 && (fstage3 == FruitsStage3.greengrapes))
                        if (!source.Inventory.HasCountByTemplateKey("greengrapes", 10))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (fruitsStage3 && (fstage3 == FruitsStage3.strawberry))
                        if (!source.Inventory.HasCountByTemplateKey("strawberry", 10))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (fruitsStage3 && (fstage3 == FruitsStage3.tangerines))
                        if (!source.Inventory.HasCountByTemplateKey("tangerines", 10))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (vegetableStage && (vStage == VegetableStage.vegetable))
                        if (!source.Inventory.HasCountByTemplateKey("vegetable", 5))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (vegetableStage && (vStage == VegetableStage.carrot))
                        if (!source.Inventory.HasCountByTemplateKey("carrot", 5))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (vegetableStage && (vStage == VegetableStage.rambutan))
                        if (!source.Inventory.HasCountByTemplateKey("rambutan", 5))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (vegetableStage && (vStage == VegetableStage.tomato))
                        if (!source.Inventory.HasCountByTemplateKey("tomato", 5))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (vegetableStage2 && (vStage2 == VegetableStage2.vegetable))
                        if (!source.Inventory.HasCountByTemplateKey("vegetable", 5))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (vegetableStage2 && (vStage2 == VegetableStage2.carrot))
                        if (!source.Inventory.HasCountByTemplateKey("carrot", 5))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (vegetableStage2 && (vStage2 == VegetableStage2.rambutan))
                        if (!source.Inventory.HasCountByTemplateKey("rambutan", 5))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (vegetableStage2 && (vStage2 == VegetableStage2.tomato))
                        if (!source.Inventory.HasCountByTemplateKey("tomato", 5))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (vegetableStage3 && (vStage3 == VegetableStage3.vegetable))
                        if (!source.Inventory.HasCountByTemplateKey("vegetable", 5))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (vegetableStage3 && (vStage3 == VegetableStage3.carrot))
                        if (!source.Inventory.HasCountByTemplateKey("carrot", 5))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (vegetableStage3 && (vStage3 == VegetableStage3.rambutan))
                        if (!source.Inventory.HasCountByTemplateKey("rambutan", 5))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (vegetableStage3 && (vStage3 == VegetableStage3.tomato))
                        if (!source.Inventory.HasCountByTemplateKey("tomato", 5))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (extraIngredients && (eStage == ExtraIngredientsStage.bread))
                        if (!source.Inventory.HasCountByTemplateKey("bread", 1))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (extraIngredients && (eStage == ExtraIngredientsStage.cheese))
                        if (!source.Inventory.HasCountByTemplateKey("cheese", 1))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (extraIngredients && (eStage == ExtraIngredientsStage.flour))
                        if (!source.Inventory.HasCountByTemplateKey("flour", 1))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (extraIngredients && (eStage == ExtraIngredientsStage.marinade))
                        if (!source.Inventory.HasCountByTemplateKey("marinade", 1))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (extraIngredients && (eStage == ExtraIngredientsStage.salt))
                        if (!source.Inventory.HasCountByTemplateKey("salt", 1))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (extraIngredients2 && (eStage2 == ExtraIngredientsStage2.bread))
                        if (!source.Inventory.HasCountByTemplateKey("bread", 1))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (extraIngredients2 && (eStage2 == ExtraIngredientsStage2.cheese))
                        if (!source.Inventory.HasCountByTemplateKey("cheese", 1))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (extraIngredients2 && (eStage2 == ExtraIngredientsStage2.flour))
                        if (!source.Inventory.HasCountByTemplateKey("flour", 1))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (extraIngredients2 && (eStage2 == ExtraIngredientsStage2.marinade))
                        if (!source.Inventory.HasCountByTemplateKey("marinade", 1))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (extraIngredients2 && (eStage2 == ExtraIngredientsStage2.salt))
                        if (!source.Inventory.HasCountByTemplateKey("salt", 1))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (extraIngredients3 && (eStage3 == ExtraIngredientsStage3.bread))
                        if (!source.Inventory.HasCountByTemplateKey("bread", 1))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (extraIngredients3 && (eStage3 == ExtraIngredientsStage3.cheese))
                        if (!source.Inventory.HasCountByTemplateKey("cheese", 1))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (extraIngredients3 && (eStage3 == ExtraIngredientsStage3.flour))
                        if (!source.Inventory.HasCountByTemplateKey("flour", 1))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (extraIngredients3 && (eStage3 == ExtraIngredientsStage3.marinade))
                        if (!source.Inventory.HasCountByTemplateKey("marinade", 1))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (extraIngredients3 && (eStage3 == ExtraIngredientsStage3.salt))
                        if (!source.Inventory.HasCountByTemplateKey("salt", 1))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (source.Trackers.Flags.HasFlag(CookFoodProgression.NotenoughIngredients))
                    {
                        Subject.Reply(source, "You don't have enough ingredients to cook this again.", "cooking_initial");
                        source.Trackers.Flags.RemoveFlag(CookFoodProgression.NotenoughIngredients);

                        return;
                    }
                    #endregion

                    #region RemoveItems
                    if (meatStage && (mStage == MeatsStage.beefslices))
                        source.Inventory.RemoveQuantityByTemplateKey("beefslices", 1);

                    if (meatStage && (mStage == MeatsStage.clam))
                        source.Inventory.RemoveQuantityByTemplateKey("clam", 1);

                    if (meatStage && (mStage == MeatsStage.egg))
                        source.Inventory.RemoveQuantityByTemplateKey("egg", 1);

                    if (meatStage && (mStage == MeatsStage.liver))
                        source.Inventory.RemoveQuantityByTemplateKey("liver", 1);

                    if (meatStage && (mStage == MeatsStage.lobstertail))
                        source.Inventory.RemoveQuantityByTemplateKey("lobstertail", 1);

                    if (meatStage && (mStage == MeatsStage.rawmeat))
                        source.Inventory.RemoveQuantityByTemplateKey("rawmeat", 1);

                    if (meatStage2 && (mStage2 == MeatsStage2.beefslices))
                        source.Inventory.RemoveQuantityByTemplateKey("beefslices", 1);

                    if (meatStage2 && (mStage2 == MeatsStage2.clam))
                        source.Inventory.RemoveQuantityByTemplateKey("clam", 1);

                    if (meatStage2 && (mStage2 == MeatsStage2.egg))
                        source.Inventory.RemoveQuantityByTemplateKey("egg", 1);

                    if (meatStage2 && (mStage2 == MeatsStage2.liver))
                        source.Inventory.RemoveQuantityByTemplateKey("liver", 1);

                    if (meatStage2 && (mStage2 == MeatsStage2.lobstertail))
                        source.Inventory.RemoveQuantityByTemplateKey("lobstertail", 1);

                    if (meatStage2 && (mStage2 == MeatsStage2.rawmeat))
                        source.Inventory.RemoveQuantityByTemplateKey("rawmeat", 1);

                    if (meatStage3 && (mStage3 == MeatsStage3.beefslices))
                        source.Inventory.RemoveQuantityByTemplateKey("beefslices", 1);

                    if (meatStage3 && (mStage3 == MeatsStage3.clam))
                        source.Inventory.RemoveQuantityByTemplateKey("clam", 1);

                    if (meatStage3 && (mStage3 == MeatsStage3.egg))
                        source.Inventory.RemoveQuantityByTemplateKey("egg", 1);

                    if (meatStage3 && (mStage3 == MeatsStage3.liver))
                        source.Inventory.RemoveQuantityByTemplateKey("liver", 1);

                    if (meatStage3 && (mStage3 == MeatsStage3.lobstertail))
                        source.Inventory.RemoveQuantityByTemplateKey("lobstertail", 1);

                    if (meatStage3 && (mStage3 == MeatsStage3.rawmeat))
                        source.Inventory.RemoveQuantityByTemplateKey("rawmeat", 1);

                    if (fruitsStage && (fstage == FruitsStage.cherry))
                        source.Inventory.RemoveQuantityByTemplateKey("cherry", 15);

                    if (fruitsStage && (fstage == FruitsStage.grape))
                        source.Inventory.RemoveQuantityByTemplateKey("grape", 15);

                    if (fruitsStage && (fstage == FruitsStage.acorn))
                        source.Inventory.RemoveQuantityByTemplateKey("acorn", 15);

                    if (fruitsStage && (fstage == FruitsStage.apple))
                        source.Inventory.RemoveQuantityByTemplateKey("apple", 15);

                    if (fruitsStage && (fstage == FruitsStage.greengrapes))
                        source.Inventory.RemoveQuantityByTemplateKey("greengrapes", 15);

                    if (fruitsStage && (fstage == FruitsStage.strawberry))
                        source.Inventory.RemoveQuantityByTemplateKey("strawberry", 15);

                    if (fruitsStage && (fstage == FruitsStage.tangerines))
                        source.Inventory.RemoveQuantityByTemplateKey("tangerines", 15);

                    if (fruitsStage2 && (fstage2 == FruitsStage2.cherry))
                        source.Inventory.RemoveQuantityByTemplateKey("cherry", 15);

                    if (fruitsStage2 && (fstage2 == FruitsStage2.grape))
                        source.Inventory.RemoveQuantityByTemplateKey("grape", 15);

                    if (fruitsStage2 && (fstage2 == FruitsStage2.acorn))
                        source.Inventory.RemoveQuantityByTemplateKey("acorn", 15);

                    if (fruitsStage2 && (fstage2 == FruitsStage2.apple))
                        source.Inventory.RemoveQuantityByTemplateKey("apple", 15);

                    if (fruitsStage2 && (fstage2 == FruitsStage2.greengrapes))
                        source.Inventory.RemoveQuantityByTemplateKey("greengrapes", 15);

                    if (fruitsStage2 && (fstage2 == FruitsStage2.strawberry))
                        source.Inventory.RemoveQuantityByTemplateKey("strawberry", 15);

                    if (fruitsStage2 && (fstage2 == FruitsStage2.tangerines))
                        source.Inventory.RemoveQuantityByTemplateKey("tangerines", 15);

                    if (fruitsStage3 && (fstage3 == FruitsStage3.cherry))
                        source.Inventory.RemoveQuantityByTemplateKey("cherry", 15);

                    if (fruitsStage3 && (fstage3 == FruitsStage3.grape))
                        source.Inventory.RemoveQuantityByTemplateKey("grape", 15);

                    if (fruitsStage3 && (fstage3 == FruitsStage3.acorn))
                        source.Inventory.RemoveQuantityByTemplateKey("acorn", 15);

                    if (fruitsStage3 && (fstage3 == FruitsStage3.apple))
                        source.Inventory.RemoveQuantityByTemplateKey("apple", 15);

                    if (fruitsStage3 && (fstage3 == FruitsStage3.greengrapes))
                        source.Inventory.RemoveQuantityByTemplateKey("greengrapes", 15);

                    if (fruitsStage3 && (fstage3 == FruitsStage3.strawberry))
                        source.Inventory.RemoveQuantityByTemplateKey("strawberry", 15);

                    if (fruitsStage3 && (fstage3 == FruitsStage3.tangerines))
                        source.Inventory.RemoveQuantityByTemplateKey("tangerines", 15);

                    if (vegetableStage && (vStage == VegetableStage.vegetable))
                        source.Inventory.RemoveQuantityByTemplateKey("vegetable", 5);

                    if (vegetableStage && (vStage == VegetableStage.carrot))
                        source.Inventory.RemoveQuantityByTemplateKey("carrot", 5);

                    if (vegetableStage && (vStage == VegetableStage.rambutan))
                        source.Inventory.RemoveQuantityByTemplateKey("rambutan", 5);

                    if (vegetableStage && (vStage == VegetableStage.tomato))
                        source.Inventory.RemoveQuantityByTemplateKey("tomato", 5);

                    if (vegetableStage2 && (vStage2 == VegetableStage2.vegetable))
                        source.Inventory.RemoveQuantityByTemplateKey("vegetable", 5);

                    if (vegetableStage2 && (vStage2 == VegetableStage2.carrot))
                        source.Inventory.RemoveQuantityByTemplateKey("carrot", 5);

                    if (vegetableStage2 && (vStage2 == VegetableStage2.rambutan))
                        source.Inventory.RemoveQuantityByTemplateKey("rambutan", 5);

                    if (vegetableStage2 && (vStage2 == VegetableStage2.tomato))
                        source.Inventory.RemoveQuantityByTemplateKey("tomato", 5);

                    if (vegetableStage3 && (vStage3 == VegetableStage3.vegetable))
                        source.Inventory.RemoveQuantityByTemplateKey("vegetable", 5);

                    if (vegetableStage3 && (vStage3 == VegetableStage3.carrot))
                        source.Inventory.RemoveQuantityByTemplateKey("carrot", 5);

                    if (vegetableStage3 && (vStage3 == VegetableStage3.rambutan))
                        source.Inventory.RemoveQuantityByTemplateKey("rambutan", 5);

                    if (vegetableStage3 && (vStage3 == VegetableStage3.tomato))
                        source.Inventory.RemoveQuantityByTemplateKey("tomato", 5);

                    if (extraIngredients && (eStage == ExtraIngredientsStage.bread))
                        source.Inventory.RemoveQuantityByTemplateKey("bread", 1);

                    if (extraIngredients && (eStage == ExtraIngredientsStage.cheese))
                        source.Inventory.RemoveQuantityByTemplateKey("cheese", 1);

                    if (extraIngredients && (eStage == ExtraIngredientsStage.flour))
                        source.Inventory.RemoveQuantityByTemplateKey("flour", 1);

                    if (extraIngredients && (eStage == ExtraIngredientsStage.marinade))
                        source.Inventory.RemoveQuantityByTemplateKey("marinade", 1);

                    if (extraIngredients && (eStage == ExtraIngredientsStage.salt))
                        source.Inventory.RemoveQuantityByTemplateKey("salt", 1);

                    if (extraIngredients2 && (eStage2 == ExtraIngredientsStage2.bread))
                        source.Inventory.RemoveQuantityByTemplateKey("bread", 1);

                    if (extraIngredients2 && (eStage2 == ExtraIngredientsStage2.cheese))
                        source.Inventory.RemoveQuantityByTemplateKey("cheese", 1);

                    if (extraIngredients2 && (eStage2 == ExtraIngredientsStage2.flour))
                        source.Inventory.RemoveQuantityByTemplateKey("flour", 2);

                    if (extraIngredients2 && (eStage2 == ExtraIngredientsStage2.marinade))
                        source.Inventory.RemoveQuantityByTemplateKey("marinade", 1);

                    if (extraIngredients2 && (eStage2 == ExtraIngredientsStage2.salt))
                        source.Inventory.RemoveQuantityByTemplateKey("salt", 1);

                    if (extraIngredients3 && (eStage3 == ExtraIngredientsStage3.bread))
                        source.Inventory.RemoveQuantityByTemplateKey("bread", 1);

                    if (extraIngredients3 && (eStage3 == ExtraIngredientsStage3.cheese))
                        source.Inventory.RemoveQuantityByTemplateKey("cheese", 1);

                    if (extraIngredients3 && (eStage3 == ExtraIngredientsStage3.flour))
                        source.Inventory.RemoveQuantityByTemplateKey("flour", 1);

                    if (extraIngredients3 && (eStage3 == ExtraIngredientsStage3.marinade))
                        source.Inventory.RemoveQuantityByTemplateKey("marinade", 1);

                    if (extraIngredients3 && (eStage3 == ExtraIngredientsStage3.salt))
                        source.Inventory.RemoveQuantityByTemplateKey("salt", 1);
                    #endregion

                    source.GiveItemOrSendToBank(ItemFactory.Create("sweetbuns"));
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
                    if (source.Inventory.HasCountByTemplateKey("cherry", 5))
                    {
                        var item = ItemFactory.CreateFaux("cherry");
                        Subject.Items.Add(ItemDetails.DisplayRecipe(item));
                    }

                    if (source.Inventory.HasCountByTemplateKey("acorn", 5))
                    {
                        var item = ItemFactory.CreateFaux("acorn");
                        Subject.Items.Add(ItemDetails.DisplayRecipe(item));
                    }

                    if (source.Inventory.HasCountByTemplateKey("apple", 5))
                    {
                        var item = ItemFactory.CreateFaux("apple");
                        Subject.Items.Add(ItemDetails.DisplayRecipe(item));
                    }

                    if (source.Inventory.HasCountByTemplateKey("greengrapes", 5))
                    {
                        var item = ItemFactory.CreateFaux("greengrapes");
                        Subject.Items.Add(ItemDetails.DisplayRecipe(item));
                    }

                    if (source.Inventory.HasCountByTemplateKey("grape", 5))
                    {
                        var item = ItemFactory.CreateFaux("grape");
                        Subject.Items.Add(ItemDetails.DisplayRecipe(item));
                    }

                    if (source.Inventory.HasCountByTemplateKey("strawberry", 5))
                    {
                        var item = ItemFactory.CreateFaux("strawberry");
                        Subject.Items.Add(ItemDetails.DisplayRecipe(item));
                    }

                    if (source.Inventory.HasCountByTemplateKey("tangerines", 5))
                    {
                        var item = ItemFactory.CreateFaux("tangerines");
                        Subject.Items.Add(ItemDetails.DisplayRecipe(item));
                    }

                    if (Subject.Items.Count == 0)
                        Subject.Reply(source, "You do not have the required fruits.", "cooking_initial");
                }

                    break;
                #endregion

                #region cookfruitbasket
                case "cook_item":
                {
                    #region CheckItems
                    if (fruitsStage && (fstage == FruitsStage.cherry))
                        if (!source.Inventory.HasCountByTemplateKey("cherry", 15))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (fruitsStage && (fstage == FruitsStage.grape))
                        if (!source.Inventory.HasCountByTemplateKey("grape", 15))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (fruitsStage && (fstage == FruitsStage.acorn))
                        if (!source.Inventory.HasCountByTemplateKey("acorn", 15))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (fruitsStage && (fstage == FruitsStage.apple))
                        if (!source.Inventory.HasCountByTemplateKey("apple", 15))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (fruitsStage && (fstage == FruitsStage.greengrapes))
                        if (!source.Inventory.HasCountByTemplateKey("greengrapes", 15))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (fruitsStage && (fstage == FruitsStage.strawberry))
                        if (!source.Inventory.HasCountByTemplateKey("strawberry", 15))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (fruitsStage && (fstage == FruitsStage.tangerines))
                        if (!source.Inventory.HasCountByTemplateKey("tangerines", 15))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (fruitsStage2 && (fstage2 == FruitsStage2.cherry))
                        if (!source.Inventory.HasCountByTemplateKey("cherry", 10))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (fruitsStage2 && (fstage2 == FruitsStage2.grape))
                        if (!source.Inventory.HasCountByTemplateKey("grape", 10))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (fruitsStage2 && (fstage2 == FruitsStage2.acorn))
                        if (!source.Inventory.HasCountByTemplateKey("acorn", 10))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (fruitsStage2 && (fstage2 == FruitsStage2.apple))
                        if (!source.Inventory.HasCountByTemplateKey("apple", 10))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (fruitsStage2 && (fstage2 == FruitsStage2.greengrapes))
                        if (!source.Inventory.HasCountByTemplateKey("greengrapes", 10))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (fruitsStage2 && (fstage2 == FruitsStage2.strawberry))
                        if (!source.Inventory.HasCountByTemplateKey("strawberry", 10))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (fruitsStage2 && (fstage2 == FruitsStage2.tangerines))
                        if (!source.Inventory.HasCountByTemplateKey("tangerines", 10))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (fruitsStage3 && (fstage3 == FruitsStage3.cherry))
                        if (!source.Inventory.HasCountByTemplateKey("cherry", 5))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (fruitsStage3 && (fstage3 == FruitsStage3.grape))
                        if (!source.Inventory.HasCountByTemplateKey("grape", 5))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (fruitsStage3 && (fstage3 == FruitsStage3.acorn))
                        if (!source.Inventory.HasCountByTemplateKey("acorn", 5))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (fruitsStage3 && (fstage3 == FruitsStage3.apple))
                        if (!source.Inventory.HasCountByTemplateKey("apple", 5))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (fruitsStage3 && (fstage3 == FruitsStage3.greengrapes))
                        if (!source.Inventory.HasCountByTemplateKey("greengrapes", 5))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (fruitsStage3 && (fstage3 == FruitsStage3.strawberry))
                        if (!source.Inventory.HasCountByTemplateKey("strawberry", 5))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (fruitsStage3 && (fstage3 == FruitsStage3.tangerines))
                        if (!source.Inventory.HasCountByTemplateKey("tangerines", 5))
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
                        source.Inventory.RemoveQuantityByTemplateKey("cherry", 15);

                    if (fruitsStage && (fstage == FruitsStage.grape))
                        source.Inventory.RemoveQuantityByTemplateKey("grape", 15);

                    if (fruitsStage && (fstage == FruitsStage.acorn))
                        source.Inventory.RemoveQuantityByTemplateKey("acorn", 15);

                    if (fruitsStage && (fstage == FruitsStage.apple))
                        source.Inventory.RemoveQuantityByTemplateKey("apple", 15);

                    if (fruitsStage && (fstage == FruitsStage.greengrapes))
                        source.Inventory.RemoveQuantityByTemplateKey("greengrapes", 15);

                    if (fruitsStage && (fstage == FruitsStage.strawberry))
                        source.Inventory.RemoveQuantityByTemplateKey("strawberry", 15);

                    if (fruitsStage && (fstage == FruitsStage.tangerines))
                        source.Inventory.RemoveQuantityByTemplateKey("tangerines", 15);

                    if (fruitsStage2 && (fstage2 == FruitsStage2.cherry))
                        source.Inventory.RemoveQuantityByTemplateKey("cherry", 10);

                    if (fruitsStage2 && (fstage2 == FruitsStage2.grape))
                        source.Inventory.RemoveQuantityByTemplateKey("grape", 10);

                    if (fruitsStage2 && (fstage2 == FruitsStage2.acorn))
                        source.Inventory.RemoveQuantityByTemplateKey("acorn", 10);

                    if (fruitsStage2 && (fstage2 == FruitsStage2.apple))
                        source.Inventory.RemoveQuantityByTemplateKey("apple", 10);

                    if (fruitsStage2 && (fstage2 == FruitsStage2.greengrapes))
                        source.Inventory.RemoveQuantityByTemplateKey("greengrapes", 10);

                    if (fruitsStage2 && (fstage2 == FruitsStage2.strawberry))
                        source.Inventory.RemoveQuantityByTemplateKey("strawberry", 10);

                    if (fruitsStage2 && (fstage2 == FruitsStage2.tangerines))
                        source.Inventory.RemoveQuantityByTemplateKey("tangerines", 10);

                    if (fruitsStage3 && (fstage3 == FruitsStage3.cherry))
                        source.Inventory.RemoveQuantityByTemplateKey("cherry", 5);

                    if (fruitsStage3 && (fstage3 == FruitsStage3.grape))
                        source.Inventory.RemoveQuantityByTemplateKey("grape", 5);

                    if (fruitsStage3 && (fstage3 == FruitsStage3.acorn))
                        source.Inventory.RemoveQuantityByTemplateKey("acorn", 5);

                    if (fruitsStage3 && (fstage3 == FruitsStage3.apple))
                        source.Inventory.RemoveQuantityByTemplateKey("apple", 5);

                    if (fruitsStage3 && (fstage3 == FruitsStage3.greengrapes))
                        source.Inventory.RemoveQuantityByTemplateKey("greengrapes", 5);

                    if (fruitsStage3 && (fstage3 == FruitsStage3.strawberry))
                        source.Inventory.RemoveQuantityByTemplateKey("strawberry", 5);

                    if (fruitsStage3 && (fstage3 == FruitsStage3.tangerines))
                        source.Inventory.RemoveQuantityByTemplateKey("tangerines", 5);
                    #endregion

                    source.GiveItemOrSendToBank(ItemFactory.Create("fruitbasket"));
                    source.SendOrangeBarMessage("You have made a Fruit Basket!");
                    Subject.Reply(source, "Skip", "cook_itemrepeat");
                }

                    break;
                #endregion
            }

        if (foodSelected && (foodStage == CookFoodStage.salad))
            switch (Subject.Template.TemplateKey.ToLower())
            {
                #region meatadd
                case "meat_initial":
                {
                    if (source.Inventory.HasCountByTemplateKey("beefslices", 1))
                    {
                        var item = ItemFactory.CreateFaux("beefslices");
                        Subject.Items.Add(ItemDetails.DisplayRecipe(item));
                    }

                    if (source.Inventory.HasCountByTemplateKey("clam", 1))
                    {
                        var item = ItemFactory.CreateFaux("clam");
                        Subject.Items.Add(ItemDetails.DisplayRecipe(item));
                    }

                    if (source.Inventory.HasCountByTemplateKey("egg", 1))
                    {
                        var item = ItemFactory.CreateFaux("egg");
                        Subject.Items.Add(ItemDetails.DisplayRecipe(item));
                    }

                    if (source.Inventory.HasCountByTemplateKey("liver", 1))
                    {
                        var item = ItemFactory.CreateFaux("liver");
                        Subject.Items.Add(ItemDetails.DisplayRecipe(item));
                    }

                    if (source.Inventory.HasCountByTemplateKey("lobstertail", 1))
                    {
                        var item = ItemFactory.CreateFaux("lobstertail");
                        Subject.Items.Add(ItemDetails.DisplayRecipe(item));
                    }

                    if (source.Inventory.HasCountByTemplateKey("rawmeat", 1))
                    {
                        var item = ItemFactory.CreateFaux("rawmeat");
                        Subject.Items.Add(ItemDetails.DisplayRecipe(item));
                    }

                    if (Subject.Items.Count == 0)
                    {
                        Subject.Reply(source, "You do not have the required meats.", "cooking_initial");

                        return;
                    }
                }

                    break;
                #endregion

                #region vegetableadd
                case "vegetable_initial":
                {
                    if (source.Inventory.HasCountByTemplateKey("vegetable", 15))
                    {
                        var item = ItemFactory.CreateFaux("vegetable");
                        Subject.Items.Add(ItemDetails.DisplayRecipe(item));
                    }

                    if (source.Inventory.HasCountByTemplateKey("carrot", 15))
                    {
                        var item = ItemFactory.CreateFaux("carrot");
                        Subject.Items.Add(ItemDetails.DisplayRecipe(item));
                    }

                    if (source.Inventory.HasCountByTemplateKey("rambutan", 15))
                    {
                        var item = ItemFactory.CreateFaux("rambutan");
                        Subject.Items.Add(ItemDetails.DisplayRecipe(item));
                    }

                    if (source.Inventory.HasCountByTemplateKey("tomato", 15))
                    {
                        var item = ItemFactory.CreateFaux("tomato");
                        Subject.Items.Add(ItemDetails.DisplayRecipe(item));
                    }

                    if (Subject.Items.Count == 0)
                    {
                        Subject.Reply(source, "You do not have the required vegetables.", "cooking_initial");

                        return;
                    }
                }

                    break;
                #endregion

                #region addextraingredients
                case "extraingredients_initial":
                {
                    if (source.Inventory.HasCountByTemplateKey("cheese", 1))
                    {
                        var item = ItemFactory.CreateFaux("cheese");
                        Subject.Items.Add(ItemDetails.DisplayRecipe(item));
                    }

                    if (Subject.Items.Count == 0)
                    {
                        Subject.Reply(source, "You do not have the required extra ingredients.", "cooking_initial");

                        return;
                    }
                }

                    break;
                #endregion

                #region cooksalad
                case "cook_item":
                {
                    #region CheckItems
                    if (meatStage && (mStage == MeatsStage.beefslices))
                        if (!source.Inventory.HasCountByTemplateKey("beefslices", 1))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (meatStage && (mStage == MeatsStage.clam))
                        if (!source.Inventory.HasCountByTemplateKey("clam", 1))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (meatStage && (mStage == MeatsStage.egg))
                        if (!source.Inventory.HasCountByTemplateKey("egg", 1))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (meatStage && (mStage == MeatsStage.liver))
                        if (!source.Inventory.HasCountByTemplateKey("liver", 1))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (meatStage && (mStage == MeatsStage.lobstertail))
                        if (!source.Inventory.HasCountByTemplateKey("lobstertail", 1))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (meatStage && (mStage == MeatsStage.rawmeat))
                        if (!source.Inventory.HasCountByTemplateKey("rawmeat", 1))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (vegetableStage && (vStage == VegetableStage.vegetable))
                        if (!source.Inventory.HasCountByTemplateKey("vegetable", 15))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (vegetableStage && (vStage == VegetableStage.carrot))
                        if (!source.Inventory.HasCountByTemplateKey("carrot", 15))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (vegetableStage && (vStage == VegetableStage.rambutan))
                        if (!source.Inventory.HasCountByTemplateKey("rambutan", 15))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (vegetableStage && (vStage == VegetableStage.tomato))
                        if (!source.Inventory.HasCountByTemplateKey("tomato", 15))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (vegetableStage2 && (vStage2 == VegetableStage2.vegetable))
                        if (!source.Inventory.HasCountByTemplateKey("vegetable", 15))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (vegetableStage2 && (vStage2 == VegetableStage2.carrot))
                        if (!source.Inventory.HasCountByTemplateKey("carrot", 15))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (vegetableStage2 && (vStage2 == VegetableStage2.rambutan))
                        if (!source.Inventory.HasCountByTemplateKey("rambutan", 15))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (vegetableStage2 && (vStage2 == VegetableStage2.tomato))
                        if (!source.Inventory.HasCountByTemplateKey("tomato", 15))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (extraIngredients && (eStage == ExtraIngredientsStage.cheese))
                        if (!source.Inventory.HasCountByTemplateKey("cheese", 1))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (source.Trackers.Flags.HasFlag(CookFoodProgression.NotenoughIngredients))
                    {
                        Subject.Reply(source, "You don't have enough ingredients to cook this again.", "cooking_initial");
                        source.Trackers.Flags.RemoveFlag(CookFoodProgression.NotenoughIngredients);

                        return;
                    }
                    #endregion

                    #region RemoveItems
                    if (meatStage && (mStage == MeatsStage.beefslices))
                        source.Inventory.RemoveQuantityByTemplateKey("beefslices", 1);

                    if (meatStage && (mStage == MeatsStage.clam))
                        source.Inventory.RemoveQuantityByTemplateKey("clam", 1);

                    if (meatStage && (mStage == MeatsStage.egg))
                        source.Inventory.RemoveQuantityByTemplateKey("egg", 1);

                    if (meatStage && (mStage == MeatsStage.liver))
                        source.Inventory.RemoveQuantityByTemplateKey("liver", 1);

                    if (meatStage && (mStage == MeatsStage.lobstertail))
                        source.Inventory.RemoveQuantityByTemplateKey("lobstertail", 1);

                    if (meatStage && (mStage == MeatsStage.rawmeat))
                        source.Inventory.RemoveQuantityByTemplateKey("rawmeat", 1);

                    if (vegetableStage && (vStage == VegetableStage.vegetable))
                        source.Inventory.RemoveQuantityByTemplateKey("vegetable", 15);

                    if (vegetableStage && (vStage == VegetableStage.carrot))
                        source.Inventory.RemoveQuantityByTemplateKey("carrot", 15);

                    if (vegetableStage && (vStage == VegetableStage.rambutan))
                        source.Inventory.RemoveQuantityByTemplateKey("rambutan", 15);

                    if (vegetableStage && (vStage == VegetableStage.tomato))
                        source.Inventory.RemoveQuantityByTemplateKey("tomato", 15);

                    if (vegetableStage2 && (vStage2 == VegetableStage2.vegetable))
                        source.Inventory.RemoveQuantityByTemplateKey("vegetable", 15);

                    if (vegetableStage2 && (vStage2 == VegetableStage2.carrot))
                        source.Inventory.RemoveQuantityByTemplateKey("carrot", 15);

                    if (vegetableStage2 && (vStage2 == VegetableStage2.rambutan))
                        source.Inventory.RemoveQuantityByTemplateKey("rambutan", 15);

                    if (vegetableStage2 && (vStage2 == VegetableStage2.tomato))
                        source.Inventory.RemoveQuantityByTemplateKey("tomato", 15);

                    if (extraIngredients && (eStage == ExtraIngredientsStage.cheese))
                        source.Inventory.RemoveQuantityByTemplateKey("cheese", 1);
                    #endregion

                    if (IntegerRandomizer.RollChance(20))
                    {
                        Subject.Reply(source, "Skip", "cookfailed_itemrepeat");
                        source.SendOrangeBarMessage("The ingredients you used were rotten.");

                        return;
                    }

                    source.GiveItemOrSendToBank(ItemFactory.Create("salad"));
                    source.SendOrangeBarMessage("You have made a Salad!");
                    Subject.Reply(source, "Skip", "cook_itemrepeat");
                }

                    break;
                #endregion
            }

        if (foodSelected && (foodStage == CookFoodStage.lobsterdinner))
            switch (Subject.Template.TemplateKey.ToLower())
            {
                #region meatadd
                case "meat_initial":
                {
                    if (source.Inventory.HasCountByTemplateKey("lobstertail", 1))
                    {
                        var item = ItemFactory.CreateFaux("lobstertail");
                        Subject.Items.Add(ItemDetails.DisplayRecipe(item));
                    }

                    if (Subject.Items.Count == 0)
                    {
                        Subject.Reply(source, "You do not have the required meats.", "cooking_initial");

                        return;
                    }
                }

                    break;
                #endregion

                #region fruitadd
                case "fruit_initial":
                {
                    if (source.Inventory.HasCountByTemplateKey("cherry", 25))
                    {
                        var item = ItemFactory.CreateFaux("cherry");
                        Subject.Items.Add(ItemDetails.DisplayRecipe(item));
                    }

                    if (Subject.Items.Count == 0)
                    {
                        Subject.Reply(source, "You do not have the required fruits.", "cooking_initial");

                        return;
                    }
                }

                    break;
                #endregion

                #region vegetableadd
                case "vegetable_initial":
                {
                    if (source.Inventory.HasCountByTemplateKey("vegetable", 5))
                    {
                        var item = ItemFactory.CreateFaux("vegetable");
                        Subject.Items.Add(ItemDetails.DisplayRecipe(item));
                    }

                    if (source.Inventory.HasCountByTemplateKey("tomato", 5))
                    {
                        var item = ItemFactory.CreateFaux("tomato");
                        Subject.Items.Add(ItemDetails.DisplayRecipe(item));
                    }

                    if (Subject.Items.Count == 0)
                    {
                        Subject.Reply(source, "You do not have the required vegetables.", "cooking_initial");

                        return;
                    }
                }

                    break;
                #endregion

                #region addextraingredients
                case "extraingredients_initial":
                {
                    if (source.Inventory.HasCountByTemplateKey("salt", 1))
                    {
                        var item = ItemFactory.CreateFaux("salt");
                        Subject.Items.Add(ItemDetails.DisplayRecipe(item));
                    }

                    if (Subject.Items.Count == 0)
                    {
                        Subject.Reply(source, "You do not have the required extra ingredients.", "cooking_initial");

                        return;
                    }
                }

                    break;
                #endregion

                #region cooklobsterdinner
                case "cook_item":
                {
                    #region CheckItems
                    if (meatStage && (mStage == MeatsStage.lobstertail))
                        if (!source.Inventory.HasCountByTemplateKey("lobstertail", 1))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (fruitsStage && (fstage == FruitsStage.cherry))
                        if (!source.Inventory.HasCountByTemplateKey("cherry", 25))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (vegetableStage && (vStage == VegetableStage.vegetable))
                        if (!source.Inventory.HasCountByTemplateKey("vegetable", 5))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (vegetableStage && (vStage == VegetableStage.tomato))
                        if (!source.Inventory.HasCountByTemplateKey("tomato", 5))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (vegetableStage2 && (vStage2 == VegetableStage2.vegetable))
                        if (!source.Inventory.HasCountByTemplateKey("vegetable", 5))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (vegetableStage2 && (vStage2 == VegetableStage2.tomato))
                        if (!source.Inventory.HasCountByTemplateKey("tomato", 5))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (extraIngredients && (eStage == ExtraIngredientsStage.salt))
                        if (!source.Inventory.HasCountByTemplateKey("salt", 1))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (source.Trackers.Flags.HasFlag(CookFoodProgression.NotenoughIngredients))
                    {
                        Subject.Reply(source, "You don't have enough ingredients to cook this again.", "cooking_initial");
                        source.Trackers.Flags.RemoveFlag(CookFoodProgression.NotenoughIngredients);

                        return;
                    }
                    #endregion

                    #region RemoveItems
                    if (meatStage && (mStage == MeatsStage.lobstertail))
                        source.Inventory.RemoveQuantityByTemplateKey("lobstertail", 1);

                    if (fruitsStage && (fstage == FruitsStage.cherry))
                        source.Inventory.RemoveQuantityByTemplateKey("cherry", 25);

                    if (vegetableStage && (vStage == VegetableStage.vegetable))
                        source.Inventory.RemoveQuantityByTemplateKey("vegetable", 5);

                    if (vegetableStage && (vStage == VegetableStage.tomato))
                        source.Inventory.RemoveQuantityByTemplateKey("tomato", 5);

                    if (vegetableStage2 && (vStage2 == VegetableStage2.vegetable))
                        source.Inventory.RemoveQuantityByTemplateKey("vegetable", 5);

                    if (vegetableStage2 && (vStage2 == VegetableStage2.tomato))
                        source.Inventory.RemoveQuantityByTemplateKey("tomato", 5);

                    if (extraIngredients && (eStage == ExtraIngredientsStage.salt))
                        source.Inventory.RemoveQuantityByTemplateKey("salt", 1);
                    #endregion

                    if (IntegerRandomizer.RollChance(20))
                    {
                        Subject.Reply(source, "Skip", "cookfailed_itemrepeat");
                        source.SendOrangeBarMessage("The ingredients you used were rotten.");

                        return;
                    }

                    source.GiveItemOrSendToBank(ItemFactory.Create("lobsterdinner"));
                    source.SendOrangeBarMessage("You have cooked a Lobster Dinner!");
                    Subject.Reply(source, "Skip", "cook_itemrepeat");
                }

                    break;
                #endregion
            }

        if (foodSelected && (foodStage == CookFoodStage.pie))
            switch (Subject.Template.TemplateKey.ToLower())
            {
                #region fruitadd
                case "fruit_initial":
                {
                    if (source.Inventory.HasCountByTemplateKey("cherry", 50))
                    {
                        var item = ItemFactory.CreateFaux("cherry");
                        Subject.Items.Add(ItemDetails.DisplayRecipe(item));
                    }

                    if (source.Inventory.HasCountByTemplateKey("acorn", 50))
                    {
                        var item = ItemFactory.CreateFaux("acorn");
                        Subject.Items.Add(ItemDetails.DisplayRecipe(item));
                    }

                    if (source.Inventory.HasCountByTemplateKey("apple", 50))
                    {
                        var item = ItemFactory.CreateFaux("apple");
                        Subject.Items.Add(ItemDetails.DisplayRecipe(item));
                    }

                    if (source.Inventory.HasCountByTemplateKey("greengrapes", 50))
                    {
                        var item = ItemFactory.CreateFaux("greengrapes");
                        Subject.Items.Add(ItemDetails.DisplayRecipe(item));
                    }

                    if (source.Inventory.HasCountByTemplateKey("grape", 50))
                    {
                        var item = ItemFactory.CreateFaux("grape");
                        Subject.Items.Add(ItemDetails.DisplayRecipe(item));
                    }

                    if (source.Inventory.HasCountByTemplateKey("strawberry", 50))
                    {
                        var item = ItemFactory.CreateFaux("strawberry");
                        Subject.Items.Add(ItemDetails.DisplayRecipe(item));
                    }

                    if (source.Inventory.HasCountByTemplateKey("tangerines", 50))
                    {
                        var item = ItemFactory.CreateFaux("tangerines");
                        Subject.Items.Add(ItemDetails.DisplayRecipe(item));
                    }

                    if (Subject.Items.Count == 0)
                    {
                        Subject.Reply(source, "You do not have the required fruits.", "cooking_initial");

                        return;
                    }
                }

                    break;
                #endregion

                #region addextraingredients
                case "extraingredients_initial":
                {
                    if (source.Inventory.HasCountByTemplateKey("flour", 1))
                    {
                        var item = ItemFactory.CreateFaux("flour");
                        Subject.Items.Add(ItemDetails.DisplayRecipe(item));
                    }

                    if (Subject.Items.Count == 0)
                    {
                        Subject.Reply(source, "You do not have the required extra ingredients.", "cooking_initial");

                        return;
                    }
                }

                    break;
                #endregion

                #region cookpie
                case "cook_item":
                {
                    #region CheckItems
                    if (fruitsStage && (fstage == FruitsStage.cherry))
                        if (!source.Inventory.HasCountByTemplateKey("cherry", 50))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (fruitsStage && (fstage == FruitsStage.grape))
                        if (!source.Inventory.HasCountByTemplateKey("grape", 50))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (fruitsStage && (fstage == FruitsStage.acorn))
                        if (!source.Inventory.HasCountByTemplateKey("acorn", 50))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (fruitsStage && (fstage == FruitsStage.apple))
                        if (!source.Inventory.HasCountByTemplateKey("apple", 50))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (fruitsStage && (fstage == FruitsStage.greengrapes))
                        if (!source.Inventory.HasCountByTemplateKey("greengrapes", 50))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (fruitsStage && (fstage == FruitsStage.strawberry))
                        if (!source.Inventory.HasCountByTemplateKey("strawberry", 50))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (fruitsStage && (fstage == FruitsStage.tangerines))
                        if (!source.Inventory.HasCountByTemplateKey("tangerines", 50))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (extraIngredients && (eStage == ExtraIngredientsStage.flour))
                        if (!source.Inventory.HasCountByTemplateKey("flour", 1))
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
                        source.Inventory.RemoveQuantityByTemplateKey("cherry", 50);

                    if (fruitsStage && (fstage == FruitsStage.grape))
                        source.Inventory.RemoveQuantityByTemplateKey("grape", 50);

                    if (fruitsStage && (fstage == FruitsStage.acorn))
                        source.Inventory.RemoveQuantityByTemplateKey("acorn", 50);

                    if (fruitsStage && (fstage == FruitsStage.apple))
                        source.Inventory.RemoveQuantityByTemplateKey("apple", 50);

                    if (fruitsStage && (fstage == FruitsStage.greengrapes))
                        source.Inventory.RemoveQuantityByTemplateKey("greengrapes", 50);

                    if (fruitsStage && (fstage == FruitsStage.strawberry))
                        source.Inventory.RemoveQuantityByTemplateKey("strawberry", 50);

                    if (fruitsStage && (fstage == FruitsStage.tangerines))
                        source.Inventory.RemoveQuantityByTemplateKey("tangerines", 50);

                    if (extraIngredients && (eStage == ExtraIngredientsStage.flour))
                        source.Inventory.RemoveQuantityByTemplateKey("flour", 1);
                    #endregion

                    if (IntegerRandomizer.RollChance(20))
                    {
                        Subject.Reply(source, "Skip", "cookfailed_itemrepeat");
                        source.SendOrangeBarMessage("The ingredients you used were rotten.");

                        return;
                    }

                    if (fruitsStage && (fstage == FruitsStage.cherry))
                        source.GiveItemOrSendToBank(ItemFactory.Create("pie_cherry"));

                    if (fruitsStage && (fstage == FruitsStage.grape))
                        source.GiveItemOrSendToBank(ItemFactory.Create("pie_grape"));

                    if (fruitsStage && (fstage == FruitsStage.acorn))
                        source.GiveItemOrSendToBank(ItemFactory.Create("pie_acorn"));

                    if (fruitsStage && (fstage == FruitsStage.apple))
                        source.GiveItemOrSendToBank(ItemFactory.Create("pie_apple"));

                    if (fruitsStage && (fstage == FruitsStage.greengrapes))
                        source.GiveItemOrSendToBank(ItemFactory.Create("pie_greengrapes"));

                    if (fruitsStage && (fstage == FruitsStage.strawberry))
                        source.GiveItemOrSendToBank(ItemFactory.Create("pie_strawberry"));

                    if (fruitsStage && (fstage == FruitsStage.tangerines))
                        source.GiveItemOrSendToBank(ItemFactory.Create("pie_tangerines"));

                    source.SendOrangeBarMessage("You have cooked a Pie!");
                    Subject.Reply(source, "Skip", "cook_itemrepeat");
                }

                    break;
                #endregion
            }

        if (foodSelected && (foodStage == CookFoodStage.sandwich))
            switch (Subject.Template.TemplateKey.ToLower())
            {
                #region meatadd
                case "meat_initial":
                {
                    if (source.Inventory.HasCountByTemplateKey("beefslices", 1))
                    {
                        var item = ItemFactory.CreateFaux("beefslices");
                        Subject.Items.Add(ItemDetails.DisplayRecipe(item));
                    }

                    if (source.Inventory.HasCountByTemplateKey("clam", 1))
                    {
                        var item = ItemFactory.CreateFaux("clam");
                        Subject.Items.Add(ItemDetails.DisplayRecipe(item));
                    }

                    if (source.Inventory.HasCountByTemplateKey("egg", 1))
                    {
                        var item = ItemFactory.CreateFaux("egg");
                        Subject.Items.Add(ItemDetails.DisplayRecipe(item));
                    }

                    if (source.Inventory.HasCountByTemplateKey("liver", 1))
                    {
                        var item = ItemFactory.CreateFaux("liver");
                        Subject.Items.Add(ItemDetails.DisplayRecipe(item));
                    }

                    if (source.Inventory.HasCountByTemplateKey("lobstertail", 1))
                    {
                        var item = ItemFactory.CreateFaux("lobstertail");
                        Subject.Items.Add(ItemDetails.DisplayRecipe(item));
                    }

                    if (source.Inventory.HasCountByTemplateKey("rawmeat", 1))
                    {
                        var item = ItemFactory.CreateFaux("rawmeat");
                        Subject.Items.Add(ItemDetails.DisplayRecipe(item));
                    }

                    if (Subject.Items.Count == 0)
                        Subject.Reply(source, "You do not have the required meats.", "cooking_initial");
                }

                    break;
                #endregion

                #region vegetableadd
                case "vegetable_initial":
                {
                    if (source.Inventory.HasCountByTemplateKey("vegetable", 1))
                    {
                        var item = ItemFactory.CreateFaux("vegetable");
                        Subject.Items.Add(ItemDetails.DisplayRecipe(item));
                    }

                    if (source.Inventory.HasCountByTemplateKey("tomato", 1))
                    {
                        var item = ItemFactory.CreateFaux("tomato");
                        Subject.Items.Add(ItemDetails.DisplayRecipe(item));
                    }

                    if (Subject.Items.Count == 0)
                        Subject.Reply(source, "You do not have the required vegetables.", "cooking_initial");
                }

                    break;
                #endregion

                #region addextraingredients
                case "extraingredients_initial":
                {
                    if (source.Inventory.HasCountByTemplateKey("bread", 2))
                    {
                        var item = ItemFactory.CreateFaux("bread");
                        Subject.Items.Add(ItemDetails.DisplayRecipe(item));
                    }

                    if (source.Inventory.HasCountByTemplateKey("cheese", 1))
                    {
                        var item = ItemFactory.CreateFaux("cheese");
                        Subject.Items.Add(ItemDetails.DisplayRecipe(item));
                    }

                    if (Subject.Items.Count == 0)
                        Subject.Reply(source, "You do not have the required extra ingredients.", "cooking_initial");
                }

                    break;
                #endregion

                #region cooksandwich
                case "cook_item":
                {
                    #region CheckItems
                    if (meatStage && (mStage == MeatsStage.beefslices))
                        if (!source.Inventory.HasCountByTemplateKey("beefslices", 1))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (meatStage && (mStage == MeatsStage.clam))
                        if (!source.Inventory.HasCountByTemplateKey("clam", 1))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (meatStage && (mStage == MeatsStage.egg))
                        if (!source.Inventory.HasCountByTemplateKey("egg", 1))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (meatStage && (mStage == MeatsStage.liver))
                        if (!source.Inventory.HasCountByTemplateKey("liver", 1))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (meatStage && (mStage == MeatsStage.lobstertail))
                        if (!source.Inventory.HasCountByTemplateKey("lobstertail", 1))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (meatStage && (mStage == MeatsStage.rawmeat))
                        if (!source.Inventory.HasCountByTemplateKey("rawmeat", 1))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (vegetableStage && (vStage == VegetableStage.vegetable))
                        if (!source.Inventory.HasCountByTemplateKey("vegetable", 5))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (vegetableStage && (vStage == VegetableStage.tomato))
                        if (!source.Inventory.HasCountByTemplateKey("tomato", 5))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (vegetableStage2 && (vStage2 == VegetableStage2.vegetable))
                        if (!source.Inventory.HasCountByTemplateKey("vegetable", 5))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (vegetableStage2 && (vStage2 == VegetableStage2.tomato))
                        if (!source.Inventory.HasCountByTemplateKey("tomato", 5))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (extraIngredients && (eStage == ExtraIngredientsStage.bread))
                        if (!source.Inventory.HasCountByTemplateKey("bread", 2))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (extraIngredients && (eStage == ExtraIngredientsStage.cheese))
                        if (!source.Inventory.HasCountByTemplateKey("cheese", 1))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (extraIngredients2 && (eStage2 == ExtraIngredientsStage2.bread))
                        if (!source.Inventory.HasCountByTemplateKey("bread", 2))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (extraIngredients2 && (eStage2 == ExtraIngredientsStage2.cheese))
                        if (!source.Inventory.HasCountByTemplateKey("cheese", 1))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (source.Trackers.Flags.HasFlag(CookFoodProgression.NotenoughIngredients))
                    {
                        Subject.Reply(source, "You don't have enough ingredients to cook this again.", "cooking_initial");
                        source.Trackers.Flags.RemoveFlag(CookFoodProgression.NotenoughIngredients);

                        return;
                    }
                    #endregion

                    #region RemoveItems
                    if (meatStage && (mStage == MeatsStage.beefslices))
                        source.Inventory.RemoveQuantityByTemplateKey("beefslices", 1);

                    if (meatStage && (mStage == MeatsStage.clam))
                        source.Inventory.RemoveQuantityByTemplateKey("clam", 1);

                    if (meatStage && (mStage == MeatsStage.egg))
                        source.Inventory.RemoveQuantityByTemplateKey("egg", 1);

                    if (meatStage && (mStage == MeatsStage.liver))
                        source.Inventory.RemoveQuantityByTemplateKey("liver", 1);

                    if (meatStage && (mStage == MeatsStage.lobstertail))
                        source.Inventory.RemoveQuantityByTemplateKey("lobstertail", 1);

                    if (meatStage && (mStage == MeatsStage.rawmeat))
                        source.Inventory.RemoveQuantityByTemplateKey("rawmeat", 1);

                    if (meatStage2 && (mStage2 == MeatsStage2.beefslices))
                        source.Inventory.RemoveQuantityByTemplateKey("beefslices", 1);

                    if (meatStage2 && (mStage2 == MeatsStage2.clam))
                        source.Inventory.RemoveQuantityByTemplateKey("clam", 1);

                    if (meatStage2 && (mStage2 == MeatsStage2.egg))
                        source.Inventory.RemoveQuantityByTemplateKey("egg", 1);

                    if (meatStage2 && (mStage2 == MeatsStage2.liver))
                        source.Inventory.RemoveQuantityByTemplateKey("liver", 1);

                    if (meatStage2 && (mStage2 == MeatsStage2.lobstertail))
                        source.Inventory.RemoveQuantityByTemplateKey("lobstertail", 1);

                    if (meatStage2 && (mStage2 == MeatsStage2.rawmeat))
                        source.Inventory.RemoveQuantityByTemplateKey("rawmeat", 1);

                    if (vegetableStage && (vStage == VegetableStage.vegetable))
                        source.Inventory.RemoveQuantityByTemplateKey("vegetable", 5);

                    if (vegetableStage && (vStage == VegetableStage.tomato))
                        source.Inventory.RemoveQuantityByTemplateKey("tomato", 5);

                    if (vegetableStage2 && (vStage2 == VegetableStage2.vegetable))
                        source.Inventory.RemoveQuantityByTemplateKey("vegetable", 5);

                    if (vegetableStage2 && (vStage2 == VegetableStage2.tomato))
                        source.Inventory.RemoveQuantityByTemplateKey("tomato", 5);

                    if (extraIngredients && (eStage == ExtraIngredientsStage.bread))
                        source.Inventory.RemoveQuantityByTemplateKey("bread", 2);

                    if (extraIngredients && (eStage == ExtraIngredientsStage.cheese))
                        source.Inventory.RemoveQuantityByTemplateKey("cheese", 1);

                    if (extraIngredients2 && (eStage2 == ExtraIngredientsStage2.bread))
                        source.Inventory.RemoveQuantityByTemplateKey("bread", 2);

                    if (extraIngredients2 && (eStage2 == ExtraIngredientsStage2.cheese))
                        source.Inventory.RemoveQuantityByTemplateKey("cheese", 1);
                    #endregion

                    if (IntegerRandomizer.RollChance(20))
                    {
                        Subject.Reply(source, "Skip", "cookfailed_itemrepeat");
                        source.SendOrangeBarMessage("The ingredients you used were rotten.");

                        return;
                    }

                    source.GiveItemOrSendToBank(ItemFactory.Create("sandwich"));
                    source.SendOrangeBarMessage("You made a Sandwich!");
                    Subject.Reply(source, "Skip", "cook_itemrepeat");
                }

                    break;
                #endregion
            }

        if (foodSelected && (foodStage == CookFoodStage.soup))
            switch (Subject.Template.TemplateKey.ToLower())
            {
                #region meatadd
                case "meat_initial":
                {
                    if (source.Inventory.HasCountByTemplateKey("beefslices", 1))
                    {
                        var item = ItemFactory.CreateFaux("beefslices");
                        Subject.Items.Add(ItemDetails.DisplayRecipe(item));
                    }

                    if (source.Inventory.HasCountByTemplateKey("clam", 1))
                    {
                        var item = ItemFactory.CreateFaux("clam");
                        Subject.Items.Add(ItemDetails.DisplayRecipe(item));
                    }

                    if (source.Inventory.HasCountByTemplateKey("egg", 1))
                    {
                        var item = ItemFactory.CreateFaux("egg");
                        Subject.Items.Add(ItemDetails.DisplayRecipe(item));
                    }

                    if (source.Inventory.HasCountByTemplateKey("liver", 1))
                    {
                        var item = ItemFactory.CreateFaux("liver");
                        Subject.Items.Add(ItemDetails.DisplayRecipe(item));
                    }

                    if (source.Inventory.HasCountByTemplateKey("lobstertail", 1))
                    {
                        var item = ItemFactory.CreateFaux("lobstertail");
                        Subject.Items.Add(ItemDetails.DisplayRecipe(item));
                    }

                    if (source.Inventory.HasCountByTemplateKey("rawmeat", 1))
                    {
                        var item = ItemFactory.CreateFaux("rawmeat");
                        Subject.Items.Add(ItemDetails.DisplayRecipe(item));
                    }

                    if (Subject.Items.Count == 0)
                        Subject.Reply(source, "You do not have the required meats.", "cooking_initial");
                }

                    break;
                #endregion

                #region vegetableadd
                case "vegetable_initial":
                {
                    if (source.Inventory.HasCountByTemplateKey("vegetable", 5))
                    {
                        var item = ItemFactory.CreateFaux("vegetable");
                        Subject.Items.Add(ItemDetails.DisplayRecipe(item));
                    }

                    if (source.Inventory.HasCountByTemplateKey("carrot", 5))
                    {
                        var item = ItemFactory.CreateFaux("carrot");
                        Subject.Items.Add(ItemDetails.DisplayRecipe(item));
                    }

                    if (source.Inventory.HasCountByTemplateKey("rambutan", 5))
                    {
                        var item = ItemFactory.CreateFaux("rambutan");
                        Subject.Items.Add(ItemDetails.DisplayRecipe(item));
                    }

                    if (source.Inventory.HasCountByTemplateKey("tomato", 5))
                    {
                        var item = ItemFactory.CreateFaux("tomato");
                        Subject.Items.Add(ItemDetails.DisplayRecipe(item));
                    }

                    if (Subject.Items.Count == 0)
                        Subject.Reply(source, "You do not have the required vegetables.", "cooking_initial");
                }

                    break;
                #endregion

                #region addextraingredients
                case "extraingredients_initial":
                {
                    if (source.Inventory.HasCountByTemplateKey("flour", 1))
                    {
                        var item = ItemFactory.CreateFaux("flour");
                        Subject.Items.Add(ItemDetails.DisplayRecipe(item));
                    }

                    if (source.Inventory.HasCountByTemplateKey("salt", 1))
                    {
                        var item = ItemFactory.CreateFaux("salt");
                        Subject.Items.Add(ItemDetails.DisplayRecipe(item));
                    }

                    if (Subject.Items.Count == 0)
                        Subject.Reply(source, "You do not have the required extra ingredients.", "cooking_initial");
                }

                    break;
                #endregion

                #region cooksoup
                case "cook_item":
                {
                    #region CheckItems
                    if (meatStage && (mStage == MeatsStage.beefslices))
                        if (!source.Inventory.HasCountByTemplateKey("beefslices", 1))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (meatStage && (mStage == MeatsStage.clam))
                        if (!source.Inventory.HasCountByTemplateKey("clam", 1))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (meatStage && (mStage == MeatsStage.egg))
                        if (!source.Inventory.HasCountByTemplateKey("egg", 1))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (meatStage && (mStage == MeatsStage.liver))
                        if (!source.Inventory.HasCountByTemplateKey("liver", 1))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (meatStage && (mStage == MeatsStage.lobstertail))
                        if (!source.Inventory.HasCountByTemplateKey("lobstertail", 1))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (meatStage && (mStage == MeatsStage.rawmeat))
                        if (!source.Inventory.HasCountByTemplateKey("rawmeat", 1))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (meatStage2 && (mStage2 == MeatsStage2.beefslices))
                        if (!source.Inventory.HasCountByTemplateKey("beefslices", 1))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (meatStage2 && (mStage2 == MeatsStage2.clam))
                        if (!source.Inventory.HasCountByTemplateKey("clam", 1))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (meatStage2 && (mStage2 == MeatsStage2.egg))
                        if (!source.Inventory.HasCountByTemplateKey("egg", 1))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (meatStage2 && (mStage2 == MeatsStage2.liver))
                        if (!source.Inventory.HasCountByTemplateKey("liver", 1))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (meatStage2 && (mStage2 == MeatsStage2.lobstertail))
                        if (!source.Inventory.HasCountByTemplateKey("lobstertail", 1))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (meatStage2 && (mStage2 == MeatsStage2.rawmeat))
                        if (!source.Inventory.HasCountByTemplateKey("rawmeat", 1))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (meatStage3 && (mStage3 == MeatsStage3.beefslices))
                        if (!source.Inventory.HasCountByTemplateKey("beefslices", 1))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (meatStage3 && (mStage3 == MeatsStage3.clam))
                        if (!source.Inventory.HasCountByTemplateKey("clam", 1))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (meatStage3 && (mStage3 == MeatsStage3.egg))
                        if (!source.Inventory.HasCountByTemplateKey("egg", 1))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (meatStage3 && (mStage3 == MeatsStage3.liver))
                        if (!source.Inventory.HasCountByTemplateKey("liver", 1))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (meatStage3 && (mStage3 == MeatsStage3.lobstertail))
                        if (!source.Inventory.HasCountByTemplateKey("lobstertail", 1))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (meatStage3 && (mStage3 == MeatsStage3.rawmeat))
                        if (!source.Inventory.HasCountByTemplateKey("rawmeat", 1))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (vegetableStage && (vStage == VegetableStage.vegetable))
                        if (!source.Inventory.HasCountByTemplateKey("vegetable", 5))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (vegetableStage && (vStage == VegetableStage.carrot))
                        if (!source.Inventory.HasCountByTemplateKey("carrot", 5))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (vegetableStage && (vStage == VegetableStage.rambutan))
                        if (!source.Inventory.HasCountByTemplateKey("rambutan", 5))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (vegetableStage && (vStage == VegetableStage.tomato))
                        if (!source.Inventory.HasCountByTemplateKey("tomato", 5))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (extraIngredients && (eStage == ExtraIngredientsStage.flour))
                        if (!source.Inventory.HasCountByTemplateKey("flour", 1))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (extraIngredients && (eStage == ExtraIngredientsStage.salt))
                        if (!source.Inventory.HasCountByTemplateKey("salt", 1))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (extraIngredients2 && (eStage2 == ExtraIngredientsStage2.flour))
                        if (!source.Inventory.HasCountByTemplateKey("flour", 1))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (extraIngredients2 && (eStage2 == ExtraIngredientsStage2.salt))
                        if (!source.Inventory.HasCountByTemplateKey("salt", 1))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (source.Trackers.Flags.HasFlag(CookFoodProgression.NotenoughIngredients))
                    {
                        Subject.Reply(source, "You don't have enough ingredients to cook this again.", "cooking_initial");
                        source.Trackers.Flags.RemoveFlag(CookFoodProgression.NotenoughIngredients);

                        return;
                    }
                    #endregion

                    #region RemoveItems
                    if (meatStage && (mStage == MeatsStage.beefslices))
                        source.Inventory.RemoveQuantityByTemplateKey("beefslices", 1);

                    if (meatStage && (mStage == MeatsStage.clam))
                        source.Inventory.RemoveQuantityByTemplateKey("clam", 1);

                    if (meatStage && (mStage == MeatsStage.egg))
                        source.Inventory.RemoveQuantityByTemplateKey("egg", 1);

                    if (meatStage && (mStage == MeatsStage.liver))
                        source.Inventory.RemoveQuantityByTemplateKey("liver", 1);

                    if (meatStage && (mStage == MeatsStage.lobstertail))
                        source.Inventory.RemoveQuantityByTemplateKey("lobstertail", 1);

                    if (meatStage && (mStage == MeatsStage.rawmeat))
                        source.Inventory.RemoveQuantityByTemplateKey("rawmeat", 1);

                    if (vegetableStage && (vStage == VegetableStage.vegetable))
                        source.Inventory.RemoveQuantityByTemplateKey("vegetable", 5);

                    if (vegetableStage && (vStage == VegetableStage.carrot))
                        source.Inventory.RemoveQuantityByTemplateKey("carrot", 5);

                    if (vegetableStage && (vStage == VegetableStage.rambutan))
                        source.Inventory.RemoveQuantityByTemplateKey("rambutan", 5);

                    if (vegetableStage && (vStage == VegetableStage.tomato))
                        source.Inventory.RemoveQuantityByTemplateKey("tomato", 5);

                    if (extraIngredients && (eStage == ExtraIngredientsStage.flour))
                        source.Inventory.RemoveQuantityByTemplateKey("flour", 1);

                    if (extraIngredients && (eStage == ExtraIngredientsStage.salt))
                        source.Inventory.RemoveQuantityByTemplateKey("salt", 1);

                    if (extraIngredients2 && (eStage2 == ExtraIngredientsStage2.flour))
                        source.Inventory.RemoveQuantityByTemplateKey("flour", 1);

                    if (extraIngredients2 && (eStage2 == ExtraIngredientsStage2.salt))
                        source.Inventory.RemoveQuantityByTemplateKey("salt", 1);
                    #endregion

                    if (IntegerRandomizer.RollChance(20))
                    {
                        Subject.Reply(source, "Skip", "cookfailed_itemrepeat");
                        source.SendOrangeBarMessage("The ingredients you used were rotten.");

                        return;
                    }

                    source.GiveItemOrSendToBank(ItemFactory.Create("soup"));
                    source.SendOrangeBarMessage("You have prepared some Soup!");
                    Subject.Reply(source, "Skip", "cook_itemrepeat");
                }

                    break;
                #endregion
            }

        if (foodSelected && (foodStage == CookFoodStage.steakmeal))
            switch (Subject.Template.TemplateKey.ToLower())
            {
                #region meatadd
                case "meat_initial":
                {
                    if (source.Inventory.HasCountByTemplateKey("rawmeat", 1))
                    {
                        var item = ItemFactory.CreateFaux("rawmeat");
                        Subject.Items.Add(ItemDetails.DisplayRecipe(item));
                    }

                    if (Subject.Items.Count == 0)
                        Subject.Reply(source, "You do not have the required meats.", "cooking_initial");
                }

                    break;
                #endregion

                #region fruitadd
                case "fruit_initial":
                {
                    if (source.Inventory.HasCountByTemplateKey("cherry", 10))
                    {
                        var item = ItemFactory.CreateFaux("cherry");
                        Subject.Items.Add(ItemDetails.DisplayRecipe(item));
                    }

                    if (source.Inventory.HasCountByTemplateKey("acorn", 10))
                    {
                        var item = ItemFactory.CreateFaux("acorn");
                        Subject.Items.Add(ItemDetails.DisplayRecipe(item));
                    }

                    if (source.Inventory.HasCountByTemplateKey("apple", 10))
                    {
                        var item = ItemFactory.CreateFaux("apple");
                        Subject.Items.Add(ItemDetails.DisplayRecipe(item));
                    }

                    if (source.Inventory.HasCountByTemplateKey("greengrapes", 10))
                    {
                        var item = ItemFactory.CreateFaux("greengrapes");
                        Subject.Items.Add(ItemDetails.DisplayRecipe(item));
                    }

                    if (source.Inventory.HasCountByTemplateKey("grape", 10))
                    {
                        var item = ItemFactory.CreateFaux("grape");
                        Subject.Items.Add(ItemDetails.DisplayRecipe(item));
                    }

                    if (source.Inventory.HasCountByTemplateKey("strawberry", 10))
                    {
                        var item = ItemFactory.CreateFaux("strawberry");
                        Subject.Items.Add(ItemDetails.DisplayRecipe(item));
                    }

                    if (source.Inventory.HasCountByTemplateKey("tangerines", 10))
                    {
                        var item = ItemFactory.CreateFaux("tangerines");
                        Subject.Items.Add(ItemDetails.DisplayRecipe(item));
                    }

                    if (Subject.Items.Count == 0)
                        Subject.Reply(source, "You do not have the required fruits.", "cooking_initial");
                }

                    break;
                #endregion

                #region vegetableadd
                case "vegetable_initial":
                {
                    if (source.Inventory.HasCountByTemplateKey("vegetable", 5))
                    {
                        var item = ItemFactory.CreateFaux("vegetable");
                        Subject.Items.Add(ItemDetails.DisplayRecipe(item));
                    }

                    if (source.Inventory.HasCountByTemplateKey("carrot", 5))
                    {
                        var item = ItemFactory.CreateFaux("carrot");
                        Subject.Items.Add(ItemDetails.DisplayRecipe(item));
                    }

                    if (source.Inventory.HasCountByTemplateKey("rambutan", 5))
                    {
                        var item = ItemFactory.CreateFaux("rambutan");
                        Subject.Items.Add(ItemDetails.DisplayRecipe(item));
                    }

                    if (source.Inventory.HasCountByTemplateKey("tomato", 5))
                    {
                        var item = ItemFactory.CreateFaux("tomato");
                        Subject.Items.Add(ItemDetails.DisplayRecipe(item));
                    }

                    if (Subject.Items.Count == 0)
                        Subject.Reply(source, "You do not have the required vegetables.", "cooking_initial");
                }

                    break;
                #endregion

                #region addextraingredients
                case "extraingredients_initial":
                {
                    if (source.Inventory.HasCountByTemplateKey("marinade", 1))
                    {
                        var item = ItemFactory.CreateFaux("marinade");
                        Subject.Items.Add(ItemDetails.DisplayRecipe(item));
                    }

                    if (Subject.Items.Count == 0)
                        Subject.Reply(source, "You do not have the required extra ingredients.", "cooking_initial");
                }

                    break;
                #endregion

                #region cooksteakmeal
                case "cook_item":
                {
                    #region CheckItems
                    if (meatStage && (mStage == MeatsStage.rawmeat))
                        if (!source.Inventory.HasCountByTemplateKey("rawmeat", 1))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (vegetableStage && (vStage == VegetableStage.vegetable))
                        if (!source.Inventory.HasCountByTemplateKey("vegetable", 5))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (vegetableStage && (vStage == VegetableStage.carrot))
                        if (!source.Inventory.HasCountByTemplateKey("carrot", 5))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (vegetableStage && (vStage == VegetableStage.rambutan))
                        if (!source.Inventory.HasCountByTemplateKey("rambutan", 5))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (vegetableStage && (vStage == VegetableStage.tomato))
                        if (!source.Inventory.HasCountByTemplateKey("tomato", 5))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (extraIngredients && (eStage == ExtraIngredientsStage.flour))
                        if (!source.Inventory.HasCountByTemplateKey("marinade", 1))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (source.Trackers.Flags.HasFlag(CookFoodProgression.NotenoughIngredients))
                    {
                        Subject.Reply(source, "You don't have enough ingredients to cook this again.", "cooking_initial");
                        source.Trackers.Flags.RemoveFlag(CookFoodProgression.NotenoughIngredients);

                        return;
                    }
                    #endregion

                    #region RemoveItems
                    if (meatStage && (mStage == MeatsStage.rawmeat))
                        source.Inventory.RemoveQuantityByTemplateKey("rawmeat", 1);

                    if (fruitsStage && (fstage == FruitsStage.cherry))
                        if (!source.Inventory.HasCountByTemplateKey("cherry", 10))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (fruitsStage && (fstage == FruitsStage.grape))
                        if (!source.Inventory.HasCountByTemplateKey("grape", 10))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (fruitsStage && (fstage == FruitsStage.acorn))
                        if (!source.Inventory.HasCountByTemplateKey("acorn", 10))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (fruitsStage && (fstage == FruitsStage.apple))
                        if (!source.Inventory.HasCountByTemplateKey("apple", 10))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (fruitsStage && (fstage == FruitsStage.greengrapes))
                        if (!source.Inventory.HasCountByTemplateKey("greengrapes", 10))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (fruitsStage && (fstage == FruitsStage.strawberry))
                        if (!source.Inventory.HasCountByTemplateKey("strawberry", 10))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (fruitsStage && (fstage == FruitsStage.tangerines))
                        if (!source.Inventory.HasCountByTemplateKey("tangerines", 10))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (vegetableStage && (vStage == VegetableStage.vegetable))
                        source.Inventory.RemoveQuantityByTemplateKey("vegetable", 5);

                    if (vegetableStage && (vStage == VegetableStage.carrot))
                        source.Inventory.RemoveQuantityByTemplateKey("carrot", 5);

                    if (vegetableStage && (vStage == VegetableStage.rambutan))
                        source.Inventory.RemoveQuantityByTemplateKey("rambutan", 5);

                    if (vegetableStage && (vStage == VegetableStage.tomato))
                        source.Inventory.RemoveQuantityByTemplateKey("tomato", 5);

                    if (extraIngredients && (eStage == ExtraIngredientsStage.salt))
                        source.Inventory.RemoveQuantityByTemplateKey("marinade", 1);
                    #endregion

                    if (IntegerRandomizer.RollChance(20))
                    {
                        Subject.Reply(source, "Skip", "cookfailed_itemrepeat");
                        source.SendOrangeBarMessage("The ingredients you used were rotten.");

                        return;
                    }

                    source.GiveItemOrSendToBank(ItemFactory.Create("steakmeal"));
                    source.SendOrangeBarMessage("You have cooked a Steak Meal!");
                    Subject.Reply(source, "Skip", "cook_itemrepeat");
                }

                    break;
                #endregion
            }

        if (foodSelected && (foodStage == CookFoodStage.popsicle))
            switch (Subject.Template.TemplateKey.ToLower())
            {
                #region fruitadd
                case "fruit_initial":
                {
                    if (source.Inventory.HasCountByTemplateKey("greengrapes", 100))
                    {
                        var item = ItemFactory.CreateFaux("greengrapes");
                        Subject.Items.Add(ItemDetails.DisplayRecipe(item));
                    }

                    if (source.Inventory.HasCountByTemplateKey("strawberry", 100))
                    {
                        var item = ItemFactory.CreateFaux("strawberry");
                        Subject.Items.Add(ItemDetails.DisplayRecipe(item));
                    }

                    if (source.Inventory.HasCountByTemplateKey("tangerines", 100))
                    {
                        var item = ItemFactory.CreateFaux("tangerines");
                        Subject.Items.Add(ItemDetails.DisplayRecipe(item));
                    }

                    if (Subject.Items.Count == 0)
                        Subject.Reply(source, "You do not have the required fruits.", "cooking_initial");
                }

                    break;
                #endregion

                #region addextraingredients
                case "extraingredients_initial":
                {
                    if (source.Inventory.HasCountByTemplateKey("ice", 10))
                    {
                        var item = ItemFactory.CreateFaux("ice");
                        Subject.Items.Add(ItemDetails.DisplayRecipe(item));
                    }

                    if (source.Inventory.HasCountByTemplateKey("sugar", 3))
                    {
                        var item = ItemFactory.CreateFaux("sugar");
                        Subject.Items.Add(ItemDetails.DisplayRecipe(item));
                    }

                    if (Subject.Items.Count == 0)
                        Subject.Reply(source, "You do not have the required extra ingredients.", "cooking_initial");
                }

                    break;
                #endregion

                #region cookpopsicle
                case "cook_item":
                {
                    #region CheckItem
                    if (fruitsStage && (fstage == FruitsStage.greengrapes))
                        if (!source.Inventory.HasCountByTemplateKey("greengrapes", 100))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (fruitsStage && (fstage == FruitsStage.strawberry))
                        if (!source.Inventory.HasCountByTemplateKey("strawberry", 100))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (fruitsStage && (fstage == FruitsStage.tangerines))
                        if (!source.Inventory.HasCountByTemplateKey("tangerines", 100))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (extraIngredients && (eStage == ExtraIngredientsStage.ice))
                        if (!source.Inventory.HasCountByTemplateKey("ice", 10))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (extraIngredients && (eStage == ExtraIngredientsStage.sugar))
                        if (!source.Inventory.HasCountByTemplateKey("sugar", 3))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (extraIngredients2 && (eStage2 == ExtraIngredientsStage2.ice))
                        if (!source.Inventory.HasCountByTemplateKey("ice", 10))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (extraIngredients2 && (eStage2 == ExtraIngredientsStage2.sugar))
                        if (!source.Inventory.HasCountByTemplateKey("sugar", 3))
                            source.Trackers.Flags.AddFlag(CookFoodProgression.NotenoughIngredients);

                    if (source.Trackers.Flags.HasFlag(CookFoodProgression.NotenoughIngredients))
                    {
                        Subject.Reply(source, "You don't have enough ingredients to cook this again.", "cooking_initial");
                        source.Trackers.Flags.RemoveFlag(CookFoodProgression.NotenoughIngredients);

                        return;
                    }
                    #endregion

                    #region RemoveItems
                    if (fruitsStage && (fstage == FruitsStage.greengrapes))
                        source.Inventory.RemoveQuantityByTemplateKey("greengrapes", 100);

                    if (fruitsStage && (fstage == FruitsStage.strawberry))
                        source.Inventory.RemoveQuantityByTemplateKey("strawberry", 100);

                    if (fruitsStage && (fstage == FruitsStage.tangerines))
                        source.Inventory.RemoveQuantityByTemplateKey("tangerines", 100);

                    if (extraIngredients && (eStage == ExtraIngredientsStage.ice))
                        source.Inventory.RemoveQuantityByTemplateKey("ice", 10);

                    if (extraIngredients && (eStage == ExtraIngredientsStage.sugar))
                        source.Inventory.RemoveQuantityByTemplateKey("sugar", 3);

                    if (extraIngredients2 && (eStage2 == ExtraIngredientsStage2.ice))
                        source.Inventory.RemoveQuantityByTemplateKey("ice", 10);

                    if (extraIngredients2 && (eStage2 == ExtraIngredientsStage2.sugar))
                        source.Inventory.RemoveQuantityByTemplateKey("sugar", 3);
                    #endregion

                    if (fruitsStage && (fstage == FruitsStage.greengrapes))
                        source.GiveItemOrSendToBank(ItemFactory.Create("limepopsicle"));

                    if (fruitsStage && (fstage == FruitsStage.strawberry))
                        source.GiveItemOrSendToBank(ItemFactory.Create("watermelonpopsicle"));

                    if (fruitsStage && (fstage == FruitsStage.tangerines))
                        source.GiveItemOrSendToBank(ItemFactory.Create("orangepopsicle"));

                    source.SendOrangeBarMessage("You have made a Popsicle!");
                    Subject.Reply(source, "Skip", "cook_itemrepeat");
                }

                    break;
                #endregion
            }
    }
}
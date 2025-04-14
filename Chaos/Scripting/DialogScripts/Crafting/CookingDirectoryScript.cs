using Chaos.Definitions;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;

namespace Chaos.Scripting.DialogScripts.Crafting;

public class CookingDirectoryScript : DialogScriptBase
{
    public CookingDirectoryScript(Dialog subject)
        : base(subject) { }

    public override void OnDisplaying(Aisling source)
    {
        var foodSelected = source.Trackers.Enums.TryGetValue(out CookFoodStage foodStage);
        var meatStage = source.Trackers.Enums.TryGetValue(out MeatsStage mStage);
        var fruitsStage = source.Trackers.Enums.TryGetValue(out FruitsStage fstage);
        var fruitsStage2 = source.Trackers.Enums.TryGetValue(out FruitsStage2 fstage2);
        var fruitsStage3 = source.Trackers.Enums.TryGetValue(out FruitsStage3 fstage3);
        var vegetableStage = source.Trackers.Enums.TryGetValue(out VegetableStage vStage);
        var vegetableStage2 = source.Trackers.Enums.TryGetValue(out VegetableStage2 vStage2);
        var vegetableStage3 = source.Trackers.Enums.TryGetValue(out VegetableStage3 vStage3);
        var extraIngredients = source.Trackers.Enums.TryGetValue(out ExtraIngredientsStage eStage);
        var extraIngredients2 = source.Trackers.Enums.TryGetValue(out ExtraIngredientsStage2 eStage2);
        var extraIngredients3 = source.Trackers.Enums.TryGetValue(out ExtraIngredientsStage3 eStage3);

        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "cooking_directory":
                if (foodSelected && (foodStage == CookFoodStage.dinnerplate))
                {
                    switch (foodSelected)
                    {
                        case true when !meatStage:
                            Subject.Reply(source, "Skip", "meat_initial");

                            return;
                        case true when meatStage && !fruitsStage:
                            Subject.Reply(source, "Skip", "fruit_initial");

                            return;
                        case true when meatStage && fruitsStage && !vegetableStage:
                            Subject.Reply(source, "Skip", "vegetable_initial");

                            return;
                        case true when meatStage && fruitsStage && vegetableStage:
                            Subject.Reply(source, "Skip", "cook_item");

                            return;
                    }

                    return;
                }

                if (foodSelected && (foodStage == CookFoodStage.sweetbuns))
                {
                    switch (foodSelected)
                    {
                        case true when !fruitsStage:
                            Subject.Reply(source, "Skip", "fruit_initial");

                            return;
                        case true when fruitsStage && !vegetableStage:
                            Subject.Reply(source, "Skip", "vegetable_initial");

                            return;
                        case true when fruitsStage && vegetableStage && !extraIngredients:
                            Subject.Reply(source, "Skip", "extraingredients_initial");

                            return;
                        case true when fruitsStage && vegetableStage && extraIngredients:
                            Subject.Reply(source, "Skip", "cook_item");

                            return;
                    }

                    return;
                }

                if (foodSelected && (foodStage == CookFoodStage.fruitbasket))
                    switch (foodSelected)
                    {
                        case true when !fruitsStage:
                            Subject.Reply(source, "Skip", "fruit_initial");

                            return;
                        case true when fruitsStage && !fruitsStage2:
                            Subject.Reply(source, "Skip", "fruit_initial");

                            return;
                        case true when fruitsStage && fruitsStage2 && !fruitsStage3:
                            Subject.Reply(source, "Skip", "fruit_initial");

                            return;
                        case true when fruitsStage && fruitsStage2 && fruitsStage3:
                            Subject.Reply(source, "Skip", "cook_item");

                            return;
                    }

                if (foodSelected && (foodStage == CookFoodStage.salad))
                    switch (foodSelected)
                    {
                        case true when !meatStage:
                            Subject.Reply(source, "Skip", "meat_initial");

                            return;
                        case true when meatStage && !vegetableStage:
                            Subject.Reply(source, "Skip", "vegetable_initial");

                            return;
                        case true when meatStage && vegetableStage && !vegetableStage2:
                            Subject.Reply(source, "Skip", "vegetable_initial");

                            return;
                        case true when meatStage && vegetableStage && vegetableStage2 && !extraIngredients:
                            Subject.Reply(source, "Skip", "extraingredients_initial");

                            return;

                        case true when meatStage && vegetableStage && vegetableStage2 && extraIngredients:
                            Subject.Reply(source, "Skip", "cook_item");

                            return;
                    }

                if (foodSelected && (foodStage == CookFoodStage.lobsterdinner))
                    switch (foodSelected)
                    {
                        case true when !meatStage:
                            Subject.Reply(source, "Skip", "meat_initial");

                            return;
                        case true when meatStage && !fruitsStage:
                            Subject.Reply(source, "Skip", "fruit_initial");

                            return;
                        case true when meatStage && fruitsStage && !vegetableStage:
                            Subject.Reply(source, "Skip", "vegetable_initial");

                            return;
                        case true when meatStage && fruitsStage && vegetableStage && !vegetableStage2:
                            Subject.Reply(source, "Skip", "vegetable_initial");

                            return;

                        case true when meatStage && fruitsStage && vegetableStage && vegetableStage2 && !extraIngredients:
                            Subject.Reply(source, "Skip", "extraingredients_initial");

                            return;
                        case true when meatStage && fruitsStage && vegetableStage && vegetableStage2 && extraIngredients:
                            Subject.Reply(source, "Skip", "cook_item");

                            return;
                    }

                if (foodSelected && (foodStage == CookFoodStage.sandwich))
                    switch (foodSelected)
                    {
                        case true when !meatStage:
                            Subject.Reply(source, "Skip", "meat_initial");

                            return;
                        case true when meatStage && !vegetableStage:
                            Subject.Reply(source, "Skip", "vegetable_initial");

                            return;
                        case true when meatStage && vegetableStage && !vegetableStage2:
                            Subject.Reply(source, "Skip", "vegetable_initial");

                            return;

                        case true when meatStage && vegetableStage && vegetableStage2 && !extraIngredients:
                            Subject.Reply(source, "Skip", "extraingredients_initial");

                            return;
                        case true when meatStage && vegetableStage && vegetableStage2 && extraIngredients && !extraIngredients2:
                            Subject.Reply(source, "Skip", "extraingredients_initial");

                            return;
                        case true when meatStage && vegetableStage && vegetableStage2 && extraIngredients && extraIngredients2:
                            Subject.Reply(source, "Skip", "cook_item");

                            return;
                    }

                if (foodSelected && (foodStage == CookFoodStage.pie))
                    switch (foodSelected)
                    {
                        case true when !fruitsStage:
                            Subject.Reply(source, "Skip", "fruit_initial");

                            return;
                        case true when fruitsStage && !extraIngredients:
                            Subject.Reply(source, "Skip", "extraingredients_initial");

                            return;
                        case true when fruitsStage && extraIngredients:
                            Subject.Reply(source, "Skip", "cook_item");

                            return;
                    }

                if (foodSelected && (foodStage == CookFoodStage.soup))
                    switch (foodSelected)
                    {
                        case true when !meatStage:
                            Subject.Reply(source, "Skip", "meat_initial");

                            return;
                        case true when meatStage && !vegetableStage:
                            Subject.Reply(source, "Skip", "vegetable_initial");

                            return;
                        case true when meatStage && vegetableStage && !extraIngredients:
                            Subject.Reply(source, "Skip", "extraingredients_initial");

                            return;

                        case true when meatStage && vegetableStage && extraIngredients && !extraIngredients2:
                            Subject.Reply(source, "Skip", "extraingredients_initial");

                            return;
                        case true when meatStage && vegetableStage && extraIngredients && extraIngredients2:
                            Subject.Reply(source, "Skip", "cook_item");

                            return;
                    }

                if (foodSelected && (foodStage == CookFoodStage.steakmeal))
                    switch (foodSelected)
                    {
                        case true when !meatStage:
                            Subject.Reply(source, "Skip", "meat_initial");

                            return;
                        case true when meatStage && !fruitsStage:
                            Subject.Reply(source, "Skip", "fruit_initial");

                            return;
                        case true when meatStage && fruitsStage && !vegetableStage:
                            Subject.Reply(source, "Skip", "vegetable_initial");

                            return;

                        case true when meatStage && fruitsStage && vegetableStage && !extraIngredients:
                            Subject.Reply(source, "Skip", "extraingredients_initial");

                            return;
                        case true when meatStage && fruitsStage && vegetableStage && extraIngredients:
                            Subject.Reply(source, "Skip", "cook_item");

                            return;
                    }

                if (foodSelected && (foodStage == CookFoodStage.popsicle))
                    switch (foodSelected)
                    {
                        case true when !fruitsStage:
                            Subject.Reply(source, "Skip", "fruit_initial");

                            return;

                        case true when fruitsStage && !extraIngredients:
                            Subject.Reply(source, "Skip", "extraingredients_initial");

                            return;
                        case true when fruitsStage && extraIngredients && !extraIngredients2:
                            Subject.Reply(source, "Skip", "extraingredients_initial");

                            return;
                        case true when fruitsStage && extraIngredients && extraIngredients2:
                            Subject.Reply(source, "Skip", "cook_item");

                            return;
                    }

                if (foodSelected && (foodStage == CookFoodStage.hotchocolate))
                    switch (foodSelected)
                    {
                        case true when !extraIngredients:
                            Subject.Reply(source, "Skip", "extraingredients_initial");

                            return;
                        case true when extraIngredients && !extraIngredients2:
                            Subject.Reply(source, "Skip", "extraingredients_initial");

                            return;
                        case true when extraIngredients && extraIngredients2:
                            Subject.Reply(source, "Skip", "cook_item");

                            return;
                    }

                break;
        }
    }
}
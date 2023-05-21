using Chaos.Definitions;
using Chaos.Models.Data;
using Chaos.Models.Menu;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Scripting.DialogScripts.Generic;
using Chaos.Services.Factories.Abstractions;
using Chaos.TypeMapper.Abstractions;
using Microsoft.Extensions.Logging;

namespace Chaos.Scripting.DialogScripts.Crafting;

public class DinnerPlate : DialogScriptBase
{
    private readonly IItemFactory ItemFactory;
    protected HashSet<string>? ItemTemplateKeys { get; init; }

    public DinnerPlate(
        Dialog subject,
        IItemFactory itemFactory 
    )
        : base(subject) =>
        ItemFactory = itemFactory;

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
            case "cook_dinnerplate":
            {
                if (foodSelected || (foodStage == CookFoodStage.dinnerplate))
                {
                    if (meatStage || (mStage == MeatsStage.beef))
                    {
                        source.Inventory.RemoveQuantity("beef", 1);
                    }
                    if (fruitsStage || (fstage == FruitsStage.cherry))
                    {
                        source.Inventory.RemoveQuantity("cherry", 5);
                    }
                    if (vegetableStage || (vStage == VegetableStage.vegetable))
                    {
                        source.Inventory.RemoveQuantity("vegetable", 10);
                    }

                    source.TryGiveItems(ItemFactory.Create("dinnerplate"));
                    source.SendOrangeBarMessage("You have cooked a dinnerplate!");
                    Subject.Reply(source, "You cooked a dinner plate!", "anotherdinnerplate_initial");
                    return;
                }
            }

                break;
        }
    }
}
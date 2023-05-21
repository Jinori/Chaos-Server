using Chaos.Definitions;
using Chaos.Extensions.Common;
using Chaos.Models.Data;
using Chaos.Models.Menu;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Scripting.DialogScripts.Generic;
using Chaos.Services.Factories.Abstractions;
using Chaos.TypeMapper.Abstractions;
using Chaos.Utilities;
using Microsoft.Extensions.Logging;

namespace Chaos.Scripting.DialogScripts.Crafting;

public class CookingScript : ConfigurableDialogScriptBase
{
    private readonly ICloningService<Item> ItemCloner;
    private readonly IItemFactory ItemFactory;
    private readonly ILogger<BuyShopScript> Logger;
    private ItemDetails? ItemDetails;
    protected HashSet<string>? ItemTemplateKeys { get; init; }
    private Item? FauxItem => ItemDetails?.Item;

    public CookingScript(
        Dialog subject,
        ICloningService<Item> itemCloner,
        IItemFactory itemFactory,
        ILogger<BuyShopScript> logger
    )
        : base(subject)
    {
        ItemCloner = itemCloner;
        ItemFactory = itemFactory;
        Logger = logger;
    }

    public override void OnDisplaying(Aisling source)
    {
        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "cooking_initial":
            {
                var foodSelected = source.Trackers.Enums.TryGetValue(out CookFoodStage foodStage);
                var meatStage = source.Trackers.Enums.TryGetValue(out MeatsStage mStage);
                var fruitsStage = source.Trackers.Enums.TryGetValue(out FruitsStage fstage);
                var vegetableStage = source.Trackers.Enums.TryGetValue(out VegetableStage vStage);

                if (foodSelected || meatStage || fruitsStage || vegetableStage)
                {
                    Subject.Reply(source, "Skip", "clearcookingenums");
                    return;
                }
                
                if (source.Trackers.Flags.HasFlag(CookingRecipes.dinnerplate))
                {
                    var item = ItemFactory.CreateFaux("dinnerplate");
                    Subject.Items.Add(ItemDetails.BuyWithGold(item));

                    return;
                }

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

            case "meat_initial":
            {
                if (source.Inventory.HasCount("beef", 1))
                {
                    var item = ItemFactory.CreateFaux("beef");
                    Subject.Items.Add(ItemDetails.BuyWithGold(item));
                }
            }

                break;
            case "fruit_initial":
            {
                if (source.Inventory.HasCount("cherry", 1))
                {
                    var item = ItemFactory.CreateFaux("cherry");
                    Subject.Items.Add(ItemDetails.BuyWithGold(item));
                }
            }

                break;
            case "vegetable_initial":
            {
                if (source.Inventory.HasCount("vegetable", 1))
                {
                    var item = ItemFactory.CreateFaux("vegetable");
                    Subject.Items.Add(ItemDetails.BuyWithGold(item));
                }

                break;
            }
            case "clearcookingenums":
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
                
                Subject.Reply(source, "Skip", "cooking_initial");

                return;
            }

                break;

        }

    }

    public override void OnNext(Aisling source, byte? optionIndex = null)
    {

        switch (Subject.Template.TemplateKey.ToLower())
        {
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
                        Subject.Reply(source, "Skip", "meat_initial");

                        return;
                    }
                }

                break;
            }

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
                        Subject.Reply(source, "Skip", "fruit_initial");

                        return;
                    }
                }

                break;
            }
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
                        source.Trackers.Enums.Set(FruitsStage.cherry);
                        Subject.Reply(source, "Skip", "vegetable_initial");

                        return;
                    }
                }

                break;
            }
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
                        source.Trackers.Enums.Set(VegetableStage.vegetable);
                        Subject.Reply(source, "Skip", "cook_dinnerplate");

                        return;
                    }
                }

                break;
            }
        }
    }
}
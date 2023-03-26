using Chaos.Common.Definitions;
using Chaos.Common.Utilities;
using Chaos.Data;
using Chaos.Definitions;
using Chaos.Extensions.Common;
using Chaos.Objects.Menu;
using Chaos.Objects.Panel;
using Chaos.Objects.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.TypeMapper.Abstractions;
using Chaos.Utilities;
using Microsoft.Extensions.Logging;


namespace Chaos.Scripting.DialogScripts.Generic;

public class StartCookingScript : ConfigurableDialogScriptBase
{
    private readonly ICloningService<Item> ItemCloner;
    private readonly IItemFactory ItemFactory;
    private readonly ILogger<BuyShopScript> Logger;
    private ItemDetails? ItemDetails;
    protected HashSet<string>? ItemTemplateKeys { get; init; }
    private Item? FauxItem => ItemDetails?.Item;

    /// <inheritdoc />
    public StartCookingScript(
        Dialog subject,
        IItemFactory itemFactory,
        ILogger<BuyShopScript> logger,
        ICloningService<Item> itemCloner
    )
        : base(subject)
    {
        Logger = logger;
        ItemCloner = itemCloner;
        ItemFactory = itemFactory;


        if (ItemTemplateKeys != null)
            foreach (var itemTemplateKey in ItemTemplateKeys)
            {
                var item = ItemFactory.CreateFaux(itemTemplateKey);
                Subject.Items.Add(ItemDetails.Default(item));
            }
    }

    public override void OnDisplaying(Aisling source)
    {
        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "fruitbasket_requirements":
            {
                if (!CraftingRequirements.FoodRequirements.TryGetValue("fruitbasket", out var requirements))
                {
                    Subject.Text = DialogString.UnknownInput.Value;

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

                Subject.Text = Subject.Text.Inject(requirementParams);
            }

                break;

            case "fruitbasket_requirements2":
            {
                if (!CraftingRequirements.FoodRequirements.TryGetValue("fruitbasket2", out var requirements))
                {
                    Subject.Text = DialogString.UnknownInput.Value;
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

                Subject.Text = Subject.Text.Inject(requirementParams);
            }
                break;
            
            case "fruitbasket_attempt":
            {
                if (!CraftingRequirements.FoodRequirements.TryGetValue("fruitbasket", out var requirements))
                {
                    Subject.Text = DialogString.UnknownInput.Value;
                    return;
                }

                foreach (var requirement in requirements)
                    if (!source.Inventory.HasCount(requirement.TemplateKey, requirement.Amount))
                    {
                        Subject.Type = MenuOrDialogType.Normal;
                        Subject.Text = "Looks like you don't have enough fruits.";
                        Subject.NextDialogKey = "cooking_initial";

                        return;
                    }

                foreach (var requirement in requirements)
                    source.Inventory.RemoveQuantity(requirement.TemplateKey, requirement.Amount);

                if (!Randomizer.RollChance(75))
                {
                    Subject.Type = MenuOrDialogType.Normal;
                    Subject.Text = "Looks as if these fruit are rotten";
                    Subject.NextDialogKey = "fruitbasket_requirements";

                    return;
                }

                var fruitbasket = ItemFactory.Create("fruitbasket");
                fruitbasket.Count = 3;
                source.SendOrangeBarMessage("You put together 3 Fruit Baskets!");

                if (!source.TryGiveItem(fruitbasket))
                {
                    source.Bank.Deposit(fruitbasket);
                    Subject.Type = MenuOrDialogType.Normal;
                    Subject.Text = "You couldn't hold the basket's weight, we sent it to the bank";
                    Subject.NextDialogKey = "cooking_initial";

                    return;
                }

                Subject.Type = MenuOrDialogType.Normal;
                Subject.Text = "You finished the fruit baskets.";
                Subject.NextDialogKey = "fruitbasket_requirements";
                
                break;
            }
            case "fruitbasket_attempt2":
            {
                if (!CraftingRequirements.FoodRequirements.TryGetValue("fruitbasket2", out var requirements))
                {
                    Subject.Text = DialogString.UnknownInput.Value;
                    return;
                }

                foreach (var requirement in requirements)
                    if (!source.Inventory.HasCount(requirement.TemplateKey, requirement.Amount))
                    {
                        Subject.Type = MenuOrDialogType.Normal;
                        Subject.Text = "Looks like you don't have enough fruits.";
                        Subject.NextDialogKey = "cooking_initial";

                        return;
                    }

                foreach (var requirement in requirements)
                    source.Inventory.RemoveQuantity(requirement.TemplateKey, requirement.Amount);

                if (!Randomizer.RollChance(75))
                {
                    Subject.Text = "Looks as if these fruit are rotten.";
                    Subject.Type = MenuOrDialogType.Normal;
                    Subject.NextDialogKey = "fruitbasket_requirements2";

                    return;
                }

                var fruitbasket = ItemFactory.Create("fruitbasket");
                fruitbasket.Count = 3;
                source.SendOrangeBarMessage("You put together 3 Fruit Baskets!");

                if (!source.TryGiveItem(fruitbasket))
                {
                    source.Bank.Deposit(fruitbasket);
                    Subject.Text = "You couldn't hold the basket's weight, we sent it to the bank";

                    return;
                }

                Subject.Type = MenuOrDialogType.Normal;
                Subject.Text = "You have finished making fruit baskets.";
                Subject.NextDialogKey = "fruitbasket_requirements2";
                
                break;
            }
            case "salad_requirements":
            {
                if (!CraftingRequirements.FoodRequirements.TryGetValue("salad", out var requirements))
                {
                    Subject.Text = DialogString.UnknownInput.Value;

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

                Subject.Text = Subject.Text.Inject(requirementParams);
            }

                break;

            case "salad_requirements2":
            {
                if (!CraftingRequirements.FoodRequirements.TryGetValue("salad2", out var requirements))
                {
                    Subject.Text = DialogString.UnknownInput.Value;
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

                Subject.Text = Subject.Text.Inject(requirementParams);
            }
                break;
            
            case "salad_attempt":
            {
                if (!CraftingRequirements.FoodRequirements.TryGetValue("salad", out var requirements))
                {
                    Subject.Text = DialogString.UnknownInput.Value;
                    return;
                }

                foreach (var requirement in requirements)
                    if (!source.Inventory.HasCount(requirement.TemplateKey, requirement.Amount))
                    {
                        Subject.Type = MenuOrDialogType.Normal;
                        Subject.Text = "Looks like you don't have enough vegetables.";
                        Subject.NextDialogKey = "cooking_initial";

                        return;
                    }

                foreach (var requirement in requirements)
                    source.Inventory.RemoveQuantity(requirement.TemplateKey, requirement.Amount);

                if (!Randomizer.RollChance(75))
                {
                    Subject.Type = MenuOrDialogType.Normal;
                    Subject.Text = "Looks as if these vegetables are rotten";
                    Subject.NextDialogKey = "salad_requirements";

                    return;
                }

                var salad = ItemFactory.Create("salad");
                salad.Count = 3;
                source.SendOrangeBarMessage("You put together 3 Salads!");

                if (!source.TryGiveItem(salad))
                {
                    source.Bank.Deposit(salad);
                    Subject.Type = MenuOrDialogType.Normal;
                    Subject.Text = "You couldn't hold the salads, we sent it to the bank";
                    Subject.NextDialogKey = "cooking_initial";

                    return;
                }

                Subject.Type = MenuOrDialogType.Normal;
                Subject.Text = "You finished the Salads.";
                Subject.NextDialogKey = "salad_requirements";
                
                break;
            }
            case "salad_attempt2":
            {
                if (!CraftingRequirements.FoodRequirements.TryGetValue("salad2", out var requirements))
                {
                    Subject.Text = DialogString.UnknownInput.Value;
                    return;
                }

                foreach (var requirement in requirements)
                    if (!source.Inventory.HasCount(requirement.TemplateKey, requirement.Amount))
                    {
                        Subject.Type = MenuOrDialogType.Normal;
                        Subject.Text = "Looks like you don't have enough vegetables.";
                        Subject.NextDialogKey = "cooking_initial";

                        return;
                    }

                foreach (var requirement in requirements)
                    source.Inventory.RemoveQuantity(requirement.TemplateKey, requirement.Amount);

                if (!Randomizer.RollChance(75))
                {
                    Subject.Text = "Looks as if these vegetables are rotten.";
                    Subject.Type = MenuOrDialogType.Normal;
                    Subject.NextDialogKey = "salad_requirements2";

                    return;
                }

                var salad = ItemFactory.Create("salad");
                salad.Count = 3;
                source.SendOrangeBarMessage("You put together 3 Salads!");

                if (!source.TryGiveItem(salad))
                {
                    source.Bank.Deposit(salad);
                    Subject.Text = "You couldn't hold the salads, we sent it to the bank";
                    Subject.NextDialogKey = "cooking_initial";

                    return;
                }

                Subject.Type = MenuOrDialogType.Normal;
                Subject.Text = "You have finished making salads.";
                Subject.NextDialogKey = "salad_requirements2";
                
                break;
            }
        }
    }

    /// <inheritdoc />
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
                    case "fruitbasket":
                    {
                        Subject.NextDialogKey = "fruitbasket_choose";
                        return;
                    }
                    case "salad":
                    {
                        Subject.NextDialogKey = "salad_choose";
                        return;
                    }
                }

                break;
            }
        }
        
    }
}
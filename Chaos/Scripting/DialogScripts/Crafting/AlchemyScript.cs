using Chaos.Definitions;
using Chaos.Extensions.Common;
using Chaos.Models.Data;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Utilities;

namespace Chaos.Scripting.DialogScripts.Crafting;

public class AlchemyScript : DialogScriptBase
{
    private readonly IItemFactory ItemFactory;
    
    /// <inheritdoc />
    public AlchemyScript(Dialog subject, IItemFactory itemFactory)
        : base(subject) =>
        ItemFactory = itemFactory;

    /// <inheritdoc />
    public override void OnDisplaying(Aisling source)
    {
        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "alf_showrecipes":
            {
                foreach (var recipes in CraftingRequirements.AlchemyRequirements)
                {
                    var item = ItemFactory.CreateFaux(recipes.Key);
                    Subject.Items.Add(ItemDetails.DisplayRecipe(item));
                }
                break;
            }
        }
    }

    /// <inheritdoc />
    public override void OnNext(Aisling source, byte? optionIndex = null)
    {
        if (!Subject.MenuArgs.TryGet<string>(0, out var craftable))
        {
            Subject.Reply(source, DialogString.UnknownInput.Value);
            return;
        }
        
        var itemDetails = Subject.Items.FirstOrDefault(x => x.Item.Template.TemplateKey.EqualsI(craftable));
        var item = itemDetails?.Item;

        if (item == null)
        {
            Subject.Reply(source, DialogString.UnknownInput.Value);
            return;
        }

        if (CraftingRequirements.AlchemyRequirements.TryGetValue(craftable, out var requirement))
        {
            var requiredItems = requirement.Count;
            foreach (var regeant in requirement)
            {
                if (!source.Inventory.HasCount(regeant.DisplayName, regeant.Amount))
                {
                    Subject.Reply(source, $"You need ({regeant.Amount}) {regeant.DisplayName} to craft this item. Please have the requirements before trying to craft. I don't want you blowing up my shop if things go bad..");
                    break;
                }

                if (source.Inventory.HasCount(regeant.DisplayName, regeant.Amount))
                    requiredItems++;
            }

            if (requiredItems == requirement.Count)
            {
                
            }
        }
    }
}
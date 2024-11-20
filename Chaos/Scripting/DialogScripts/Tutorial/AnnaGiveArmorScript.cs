using Chaos.Common.Definitions;
using Chaos.DarkAges.Definitions;
using Chaos.Extensions.Common;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.DialogScripts.Tutorial;

public class AnnaGiveArmorScript : DialogScriptBase
{
    private readonly IItemFactory ItemFactory;

    public AnnaGiveArmorScript(Dialog subject, IItemFactory itemFactory)
        : base(subject) => ItemFactory = itemFactory;

    public override void OnDisplayed(Aisling source)
    {
        if (source.Gender == Gender.Female)
        {
            if (source.Inventory.Any(Item => Item.Template.TemplateKey.EqualsI("blouse")))
                return;

            source.GiveItemOrSendToBank(ItemFactory.Create("blouse"));
        }
        else
        {
            if (source.Inventory.Any(Item => Item.Template.TemplateKey.EqualsI("shirt")))
                return;

            source.GiveItemOrSendToBank(ItemFactory.Create("shirt1"));
        }
    }
}
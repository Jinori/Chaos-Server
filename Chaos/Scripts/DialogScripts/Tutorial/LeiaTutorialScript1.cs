using Chaos.Common.Definitions;
using Chaos.Containers;
using Chaos.Data;
using Chaos.Extensions.Common;
using Chaos.Objects.Legend;
using Chaos.Objects.Menu;
using Chaos.Objects.World;
using Chaos.Scripts.DialogScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripts.DialogScripts.Tutorial
{
    public class LeiaTutorialScript1 : DialogScriptBase
    {
        private readonly IItemFactory ItemFactory;
        
        public LeiaTutorialScript1(Dialog subject, IItemFactory itemFactory) : base(subject) => ItemFactory = itemFactory;

        public override void OnDisplayed(Aisling source)
        {
            if (source.Gender == Common.Definitions.Gender.Female)
            {
                if (source.Inventory.Any(Item => Item.Template.TemplateKey.EqualsI("blouse")))
                    return;
                source.TryGiveItems(ItemFactory.Create("blouse"));
            }
            else
            {
                if (source.Inventory.Any(Item => Item.Template.TemplateKey.EqualsI("shirt")))
                    return;
                source.TryGiveItems(ItemFactory.Create("shirt"));
            }

            if (!source.Inventory.Any(Item => Item.Template.TemplateKey.EqualsI("Stick")) || !source.Equipment.Any(Item => Item.Template.TemplateKey.EqualsI("Stick")))
            {
                source.TryGiveItems(ItemFactory.Create(templateKey: "Stick"));
            }
            
            if (!source.Flags.HasFlag(TutorialFlag.LeiaTutorialFlag1))
            {
                source.Flags.AddFlag(TutorialFlag.LeiaTutorialFlag1);
            }
        }
    }
}

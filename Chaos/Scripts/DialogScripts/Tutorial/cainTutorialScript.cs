using Chaos.Common.Definitions;
using Chaos.Containers;
using Chaos.Data;
using Chaos.Extensions.Common;
using Chaos.Objects.Legend;
using Chaos.Objects.Menu;
using Chaos.Objects.World;
using Chaos.Scripts.DialogScripts.Abstractions;
using Chaos.Services.Factories;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripts.DialogScripts.Tutorial
{
    public class cainTutorialScript : DialogScriptBase
    {
        private readonly IItemFactory ItemFactory;
        private readonly DialogFactory DialogFactory;

        public cainTutorialScript(Dialog subject, IItemFactory itemFactory, DialogFactory dialogFactory)
            : base(subject)
        {
            ItemFactory = itemFactory;
            DialogFactory = dialogFactory;
        }

        public override void OnDisplayed(Aisling source)
        {

            if (source.Flags.HasFlag(TutorialFlag.LeiaTutorialFlag2))
            {
                source.Flags.RemoveFlag(TutorialFlag.LeiaTutorialFlag2);
                source.Flags.AddFlag(TutorialFlag.CainTutorialFlag1);
            }

            if (source.Flags.HasFlag(TutorialFlag.CainTutorialFlag1) && source.Inventory.HasCount("carrot", 3))
            {
                source.Flags.RemoveFlag(TutorialFlag.CainTutorialFlag1);
                source.Flags.AddFlag(TutorialFlag.CainTutorialFlag2);
                source.Inventory.RemoveQuantity("carrot", 3);
                source.GiveExp(1000);
                source.TryGiveGold(1000);
            }

            if (source.Flags.HasFlag(TutorialFlag.CainTutorialFlag2) && source.Equipment.Any(Item => Item.Template.TemplateKey.EqualsI("boots")))
            {
                source.Flags.RemoveFlag(TutorialFlag.CainTutorialFlag2);
                var dialog = DialogFactory.Create("cain_quest", Subject);
                dialog.Display(source);
            }
        }
    }
}

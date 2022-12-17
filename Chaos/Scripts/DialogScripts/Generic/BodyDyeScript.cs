using Chaos.Common.Definitions;
using Chaos.Data;
using Chaos.Extensions;
using Chaos.Extensions.Common;
using Chaos.Objects.Menu;
using Chaos.Objects.Panel;
using Chaos.Objects.World;
using Chaos.Scripts.DialogScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Utilities;

namespace Chaos.Scripts.DialogScripts.Generic
{
    public class BodyDyeScript : DialogScriptBase
    {

        private readonly IItemFactory ItemFactory;

        public BodyDyeScript(Dialog subject, IItemFactory itemFactory) : base(subject)
        {
            ItemFactory = itemFactory;
        }

        public override void OnDisplaying(Aisling source)
        {
            foreach (BodyColor color in Enum.GetValues<BodyColor>())
            {
                var item = ItemFactory.CreateFaux("hairDyeContainer");
                item.DisplayName = $"{color} Body Dye";
                item.Color = color.ConvertToDisplayColor();
                Subject.Items.Add(ItemDetails.Default(item));
            }
        }

        public override void OnNext(Aisling source, byte? optionIndex = null)
        {
            if (!Subject.MenuArgs.TryGet<string>(0, out var dye))
            {
                Subject.Reply(source, DialogString.UnknownInput.Value);
                return;
            }
            var ItemDetails = Subject.Items.FirstOrDefault(x => x.Item.DisplayName.EqualsI(dye));
            var Item = ItemDetails?.Item;

            if (Item == null)
            {
                Subject.Reply(source, DialogString.UnknownInput.Value);
                return;
            }
            if (!source.TryTakeGold(ItemDetails!.AmountOrPrice))
            {
                Subject.Close(source);
                return;
            }
            source.BodyColor = Item.Color.ConvertToBodyColor();
            source.Refresh(true);
        }
    }
}

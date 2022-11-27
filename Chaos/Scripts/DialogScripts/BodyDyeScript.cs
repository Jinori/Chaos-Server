using Chaos.Common.Definitions;
using Chaos.Data;
using Chaos.Extensions;
using Chaos.Extensions.Common;
using Chaos.Factories;
using Chaos.Factories.Abstractions;
using Chaos.Objects.Menu;
using Chaos.Objects.Panel;
using Chaos.Objects.World;
using Chaos.Scripts.DialogScripts.Abstractions;
using Chaos.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chaos.Scripts.DialogScripts
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
                item.Color = ColorSwap.ConvertToDisplayColor(color);
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
            source.BodyColor = ColorSwap.ConvertToBodyColor(Item.Color);
            source.Refresh(true);
        }
    }
}

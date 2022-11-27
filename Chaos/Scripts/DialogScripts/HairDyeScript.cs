using Chaos.Common.Definitions;
using Chaos.Data;
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
    public class HairDyeScript : DialogScriptBase
    {

        private readonly IItemFactory ItemFactory;

        public HairDyeScript(Dialog subject, IItemFactory itemFactory) : base(subject)
        {
            ItemFactory = itemFactory;
        }

        public override void OnDisplaying(Aisling source)
        {
            foreach (DisplayColor color in Enum.GetValues<DisplayColor>())
            {
                var item = ItemFactory.CreateFaux("hairDyeContainer");
                item.DisplayName = $"{color} Hair Dye";
                item.Color = color;
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
            source.HairColor = Item.Color;
            source.Refresh(true);
        }
    }
}

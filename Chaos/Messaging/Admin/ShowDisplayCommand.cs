using Chaos.Collections.Common;
using Chaos.DarkAges.Definitions;
using Chaos.Messaging.Abstractions;
using Chaos.Models.World;

namespace Chaos.Messaging.Admin;

[Command("display", helpText: "<displayNumber?0>")]
public class ShowDisplayCommand : ICommand<Aisling>
{
    //Allows you to see display sprites of items
    /// <inheritdoc />
    public ValueTask ExecuteAsync(Aisling source, ArgumentCollection args)
    {
        if (!args.TryGetNext<ushort>(out var formNumber))
        {
            if (source.Equipment.TryGetObject((byte)EquipmentSlot.Accessory1, out var item1))
            {
                item1.Template.ItemSprite.DisplaySprite = 0;
                source.Display();
            }

            return default;
        }

        if (source.Equipment.TryGetObject((byte)EquipmentSlot.Accessory1, out var item))
        {
            item.Template.ItemSprite.DisplaySprite = formNumber;
            source.Display();
        }

        return default;
    }
}
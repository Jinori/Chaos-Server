using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Scripting.ItemScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.ItemScripts.EventBoxes;

public class MagicianHatScript : ItemScriptBase
{
    private static readonly Random Random = new();

    private readonly Dictionary<string, double> ItemChances = new()
    {
        {
            "petbunny", 0.01
        },
        {
            "scarletkimonodress", 0.05
        },
        {
            "indigokimonodress", 0.05
        },
        {
            "pinkkimonodress", 0.05
        },
        {
            "cobaltkimonorobe", 0.05
        },
        {
            "slatekimonorobe", 0.05
        },
        {
            "crimsonkimonorobe", 0.05
        },
        {
            "adeptjewelcraftingbox", 12
        },
        {
            "adeptenchantingbox", 12
        },
        {
            "adeptweaponsmithingbox", 12
        },
        {
            "adeptarmorsmithingbox", 12
        },
        {
            "adeptalchemybox", 12
        }
    };

    private readonly IItemFactory ItemFactory;

    public MagicianHatScript(Item subject, IItemFactory itemFactory)
        : base(subject)
        => ItemFactory = itemFactory;

    public override void OnUse(Aisling source)
    {
        // consume one hat
        source.Inventory.RemoveQuantity(Subject.Slot, 1);

        // roll between 0.0 and 1.0
        var roll = Random.NextDouble();
        double cumulative = 0;

        foreach (var kv in ItemChances)
        {
            cumulative += kv.Value;

            if (roll < cumulative)
            {
                // found an item
                var item = ItemFactory.Create(kv.Key);
                source.GiveItemOrSendToBank(item);
                source.SendOrangeBarMessage($"You pulled a {item.DisplayName} from the Magician's hat!");

                return;
            }
        }

        source.SendOrangeBarMessage("You reached in... but found nothing this time.");
    }
}
using Chaos.Common.Definitions;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Scripting.ItemScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.ItemScripts;

public class MacabreBoxScript : ItemScriptBase
{
    // Define a random number generator
    private static readonly Random Random = new();

    // List of items with their corresponding drop chances
    private readonly Dictionary<string, double> _itemChances = new()
    {
        {
            "macabrehexedhat", 0.01
        },
        {
            "macabrehexeddress", 0.10
        },
        {
            "macabrehexedrobes", 0.10
        },
        {
            "fmacabreshoes", 0.15
        },
        {
            "mmacabreshoes", 0.15
        },
        {
            "swampwitchpet", 0.5
        },
        {
            "undeadhand", 0.05
        },
        {
            "fiendmask", 0.03
        },
        {
            "abominationmask", 0.03
        },
        {
            "spectremask", 0.03
        },
        {
            "mshadowhair", 0.10
        },
        {
            "fshadowhair", 0.10
        },
        {
            "fshadowcloak", 0.10
        },
        {
            "mshadowcloak", 0.10
        },
        {
            "macabreholyhat", 0.10
        },
        {
            "macabrebewitchedhat", 0.10
        },
        {
            "macabredivinerobe", 0.10
        },
        {
            "macabredivinegown", 0.10
        },
        {
            "fpumpkincostume", 0.15
        },
        {
            "mpumpkincostume", 0.15
        },
        {
            "fpumpkincap", 0.15
        },
        {
            "mpumpkincap", 0.15
        },
        {
            "fpumpkinslippers", 0.15
        },
        {
            "mpumpkinslippers", 0.15
        },
        {
            "macabrebattlearmor", 0.10
        },
        {
            "macabrebattlehelm", 0.10
        },
        {
            "virtuehood", 0.10
        },
        {
            "virtuecap", 0.10
        },
        {
            "dyeablecatears", 0.03
        },
        {
            "ghosteffect", 0.05
        },
        {
            "dungcap", 0.08
        },
        {
            "catbowtail", 0.05
        },
        {
            "ghostface", 0.03
        },
        {
            "warlockhat", 0.15
        },
        {
            "witchhat", 0.15
        },
        {
            "demonhorns", 0.06
        },
        {
            "demonmask", 0.06
        },
        {
            "cyclopseye", 0.08
        },
        {
            "demontail", 0.06
        },
        {
            "gasmask", 0.03
        },
        {
            "witchbuddy", 0.04
        },
        {
            "reaperbuddy", 0.04
        },
        {
            "femalebunnyoutfit", 0.01
        }
    };

    private readonly IItemFactory _itemFactory;

    public MacabreBoxScript(Item subject, IItemFactory itemFactory)
        : base(subject)
        => _itemFactory = itemFactory;

    public override void OnUse(Aisling source)
    {
        source.Inventory.RemoveQuantity(Subject.Slot, 1);
        
        // Total sum of all item chances
        var totalChance = _itemChances.Values.Sum();

        // Roll a random number between 0 and totalChance
        var roll = Random.NextDouble() * totalChance;

        // Determine which item the player receives based on the roll
        double cumulativeChance = 0;

        foreach (var entry in _itemChances)
        {
            cumulativeChance += entry.Value;

            if (roll <= cumulativeChance)
            {
                // Give the selected item to the player
                var item = _itemFactory.Create(entry.Key);
                source.GiveItemOrSendToBank(item);
                
                // Send a message to the player
                source.SendOrangeBarMessage($"You received {item.DisplayName} from the box!");
                break;
            }
        }
    }
}
using Chaos.Common.Utilities;
using Chaos.Models.Data;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.DialogScripts.Crafting;

public class FabricRefiningScript : DialogScriptBase
{
    private const string ITEM_COUNTER_PREFIX = "[Refine]";
    private const double BASE_SUCCESS_RATE = 60;
    private const double SUCCESSRATEMAX = 90;
    private readonly IItemFactory ItemFactory;

    private readonly List<string> MiningTemplateKeys = new()
    {
       "linen", "finelinen", "cotton", "finecotton", "wool", "finewool", "silk", "finesilk", "tornlinen", "torncotton", "tornwool", "tornsilk"
    };

    private Animation FailAnimation { get; } = new()
    {
        AnimationSpeed = 100,
        TargetAnimation = 59
    };
    private Animation SuccessAnimation { get; } = new()
    {
        AnimationSpeed = 100,
        TargetAnimation = 127
    };

    /// <inheritdoc />
    public FabricRefiningScript(Dialog subject, IItemFactory itemFactory)
        : base(subject) =>
        ItemFactory = itemFactory;

    public static double CalculateSuccessRate(Aisling source, double baseSuccessRate, int timesCraftedThisItem)
    {
        var successRate = baseSuccessRate + timesCraftedThisItem / 10.0;

        return Math.Min(successRate, SUCCESSRATEMAX);
    }

    private string GetDowngradeKey(string rawItemKey)
    {
        switch (rawItemKey)
        {
            case "linen":
                return "tornLinen";
            case "finelinen":
                return "tornlinen";
            case "cotton":
                return "tornCotton";
            case "finecotton":
                return "torncotton";
            case "wool":
                return "tornWool";
            case "finewool":
                return "tornwool";
            case "silk":
                return "tornSilk";
            case "finesilk":
                return "tornsilk";
            default:
                return rawItemKey;
        }
    }

    private string GetUpgradeKey(string rawItemKey)
    {
        switch (rawItemKey)
        {
            case "linen":
                return "fineLinen";
            case "finelinen":
                return "exquisitelinen";
            case "cotton":
                return "fineCotton";
            case "finecotton":
                return "exquisitecotton";
            case "wool":
                return "fineWool";
            case "finewool":
                return "exquisitecotton";
            case "silk":
                return "fineSilk";
            case "finesilk":
                return "exquisitesilk";
            default:
                return rawItemKey;
        }
    }

    /// <inheritdoc />
    public override void OnDisplaying(Aisling source)
    {
        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "fabric_refining_initial":
            {
                OnDisplayingShowPlayerItems(source);

                break;
            }
            case "fabric_refining_confirmation":
            {
                OnDisplayingConfirmation(source);

                break;
            }
            case "fabric_refining_accepted":
            {
                OnDisplayingAccepted(source);

                break;
            }
        }
    }

    public void OnDisplayingAccepted(Aisling source)
    {
        if (!TryFetchArg<byte>(0, out var slot) || !source.Inventory.TryGetObject(slot, out var item))
        {
            Subject.ReplyToUnknownInput(source);

            return;
        }

        source.Inventory.RemoveQuantityByTemplateKey(item.Template.TemplateKey, 1);

        var timesCraftedThisItem =
            source.Trackers.Counters.TryGetValue(ITEM_COUNTER_PREFIX + item.Template.TemplateKey, out var value) ? value : 0;

        if (!IntegerRandomizer.RollChance((int)CalculateSuccessRate(source, BASE_SUCCESS_RATE, timesCraftedThisItem)))
        {
            Subject.Reply(source, $"Your attempt to refine {item.Template.Name} has failed.", "fabric_refining_initial");
            var downgradeKey = GetDowngradeKey(item.Template.TemplateKey);
            var downgrade = ItemFactory.Create(downgradeKey);
            source.Inventory.TryAddToNextSlot(downgrade);
            source.Animate(FailAnimation);

            return;
        }

        source.Trackers.Counters.AddOrIncrement(ITEM_COUNTER_PREFIX + item.Template.TemplateKey);

        var upgradeKey = GetUpgradeKey(item.Template.TemplateKey);
        var upgrade = ItemFactory.Create(upgradeKey);

        if (!source.CanCarry(upgrade))
        {
            source.Bank.Deposit(upgrade);
            source.SendOrangeBarMessage($"{upgrade.DisplayName} was sent to your bank.");
        }
        else
            source.Inventory.TryAddToNextSlot(upgrade);

        source.Animate(SuccessAnimation);
        Subject.InjectTextParameters(item.DisplayName, upgrade.DisplayName);
    }

    public void OnDisplayingConfirmation(Aisling source)
    {
        if (!TryFetchArg<byte>(0, out var slot) || !source.Inventory.TryGetObject(slot, out var item))
        {
            Subject.Reply(source, "Skip", "fabric_refining_initial");

            return;
        }

        if (!MiningTemplateKeys.Contains(item.Template.TemplateKey))
        {
            Subject.Reply(source, "Item cannot be refined", "fabric_refining_initial");

            return;
        }

        var timesCraftedThisItem =
            source.Trackers.Counters.TryGetValue(ITEM_COUNTER_PREFIX + item.Template.TemplateKey, out var value) ? value : 0;

        var rate = CalculateSuccessRate(source, BASE_SUCCESS_RATE, timesCraftedThisItem);
        Subject.InjectTextParameters(item.DisplayName, rate);
    }

    public void OnDisplayingShowPlayerItems(Aisling source) => Subject.Slots =
        source.Inventory.Where(x => MiningTemplateKeys.Contains(x.Template.TemplateKey)).Select(x => x.Slot).ToList();
}
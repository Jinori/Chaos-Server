using Chaos.Common.Utilities;
using Chaos.Models.Data;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.DialogScripts.Crafting;

public class MetalRefiningScript : DialogScriptBase
{
    private const string ITEM_COUNTER_PREFIX = "[Refine]";
    private const double BASE_SUCCESS_RATE = 60;
    private const double SUCCESSRATEMAX = 90;
    private readonly IItemFactory ItemFactory;
    private readonly IDialogFactory DialogFactory;

    private readonly List<string> MetalTemplateKeys = new()
    {
        "rawBronze", "tarnishedBronzeBar", "rawIron",
        "tarnishedIronBar", "rawMythril", "tarnishedMythrilBar", "rawHybrasyl",
        "tarnishedHybrasylBar"
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
    public MetalRefiningScript(Dialog subject, IItemFactory itemFactory, IDialogFactory dialogFactory)
        : base(subject)
    {
        ItemFactory = itemFactory;
        DialogFactory = dialogFactory;
    }

    public static double CalculateSuccessRate(Aisling source, double baseSuccessRate, int timesCraftedThisItem)
    {
        var successRate = baseSuccessRate + timesCraftedThisItem / 10.0;

        return Math.Min(successRate, SUCCESSRATEMAX);
    }

    private string GetDowngradeKey(string rawItemKey)
    {
        switch (rawItemKey.ToLower())
        {
            case "rawbronze":
                return "ruinedbronze";
            case "tarnishedbronzeBar":
                return "ruinedbronze";
            case "rawiron":
                return "ruinediron";
            case "tarnishedironbar":
                return "ruinediron";
            case "rawmythril":
                return "ruinedmythril";
            case "tarnishedmythrilBar":
                return "ruinedmythril";
            case "rawhybrasyl":
                return "ruinedhybrasyl";
            case "tarnishedhybrasylbar":
                return "ruinedhybrasyl";
            default:
                return rawItemKey;
        }
    }

    private string GetUpgradeKey(string rawItemKey)
    {
        switch (rawItemKey.ToLower())
        {
            case "rawbronze":
                return "tarnishedbronzebar";
            case "tarnishedbronzebar":
                return "polishedbronzebar";
            case "rawiron":
                return "tarnishedironbar";
            case "tarnishedironbar":
                return "polishedironbar";
            case "rawmythril":
                return "tarnishedmythrilbar";
            case "tarnishedmythrilbar":
                return "polishedmythrilbar";
            case "rawhybrasyl":
                return "tarnishedhybrasylbar";
            case "tarnishedhybrasylbar":
                return "polishedhybrasylbar";
            default:
                return rawItemKey;
        }
    }

    /// <inheritdoc />
    public override void OnDisplaying(Aisling source)
    {
        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "metal_refining_initial":
            {
                OnDisplayingShowPlayerItems(source);

                break;
            }
            case "metal_refining_confirmation":
            {
                OnDisplayingConfirmation(source);

                break;
            }
            case "metal_refining_accepted":
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
            Subject.Reply(source, $"You ran out of that metal to refine.", "metal_refining_initial");

            return;
        }

        source.Inventory.RemoveQuantityByTemplateKey(item.Template.TemplateKey, 2);

        var timesCraftedThisItem =
            source.Trackers.Counters.TryGetValue(ITEM_COUNTER_PREFIX + item.Template.TemplateKey, out var value) ? value : 0;

        if (!IntegerRandomizer.RollChance((int)CalculateSuccessRate(source, BASE_SUCCESS_RATE, timesCraftedThisItem)))
        {
            Subject.Close(source);
            var dialog = DialogFactory.Create("metal_refining_failed", Subject.DialogSource);
            dialog.MenuArgs = Subject.MenuArgs;
            dialog.Context = Subject.Context;
            dialog.InjectTextParameters(item.DisplayName);
            dialog.Display(source);
            var downgradeKey = GetDowngradeKey(item.Template.TemplateKey);
            var downgrade = ItemFactory.Create(downgradeKey);
            source.Inventory.TryAddToNextSlot(downgrade);
            source.Animate(FailAnimation);

            return;
        }

        source.Trackers.Counters.AddOrIncrement(ITEM_COUNTER_PREFIX + item.Template.TemplateKey);

        var upgradeKey = GetUpgradeKey(item.Template.TemplateKey);
        var upgrade = ItemFactory.Create(upgradeKey);

        source.GiveItemOrSendToBank(upgrade);

        source.Animate(SuccessAnimation);
        Subject.InjectTextParameters(item.DisplayName, upgrade.DisplayName);
    }

    public void OnDisplayingConfirmation(Aisling source)
    {
        if (!TryFetchArg<byte>(0, out var slot) || !source.Inventory.TryGetObject(slot, out var item))
        {
            Subject.Reply(source, "Skip", "metal_refining_initial");

            return;
        }

        if (!MetalTemplateKeys.Contains(item.Template.TemplateKey))
        {
            Subject.Reply(source, "Item cannot be refined", "metal_refining_initial");

            return;
        }

        var timesCraftedThisItem =
            source.Trackers.Counters.TryGetValue(ITEM_COUNTER_PREFIX + item.Template.TemplateKey, out var value) ? value : 0;

        var rate = CalculateSuccessRate(source, BASE_SUCCESS_RATE, timesCraftedThisItem);
        Subject.InjectTextParameters(item.DisplayName, rate);
    }

    public void OnDisplayingShowPlayerItems(Aisling source) =>
        Subject.Slots = source.Inventory
                              .Where(x => MetalTemplateKeys.Contains(x.Template.TemplateKey) && (x.Count > 1))
                              .Select(x => x.Slot)
                              .ToList();
}
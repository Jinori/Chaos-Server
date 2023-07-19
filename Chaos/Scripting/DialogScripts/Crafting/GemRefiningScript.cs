using Chaos.Common.Utilities;
using Chaos.Models.Data;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.DialogScripts.Crafting;

public class GemRefiningScript : DialogScriptBase
{
    private const string ITEM_COUNTER_PREFIX = "[Refine]";
    private const double BASE_SUCCESS_RATE = 60;
    private const double SUCCESSRATEMAX = 90;
    private readonly IItemFactory ItemFactory;

    private readonly List<string> MiningTemplateKeys = new()
    {
       "rawsapphire", "flawedsapphire", "uncutsapphire", "chippedsapphire", 
        "rawruby", "flawedruby", "uncutruby", "chippedruby", 
        "rawemerald", "flawedemerald", "uncutemerald", "chippedemerald", 
        "rawheartstone", "flawedheartstone", "uncutheartstone", "chippedheartstone"
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
    public GemRefiningScript(Dialog subject, IItemFactory itemFactory)
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
            case "rawsapphire":
                return "chippedsapphire";
            case "flawedsapphire":
                return "chippedsapphire";
            case "uncutsapphire":
                return "chippedsapphire";
            case "rawruby":
                return "chippedruby";
            case "flawedruby":
                return "chippedruby";
            case "uncutruby":
                return "chippedruby";
            case "rawemerald":
                return "chippedemerald";
            case "flawedemerald":
                return "chippedemerald";
            case "uncutemerald":
                return "chippedemerald";
            case "rawheartstone":
                return "chippedheartstone";
            case "flawedheartstone":
                return "chippedheartstone";
            case "uncutheartstone":
                return "chippedheartstone";
            default:
                return rawItemKey;
        }
    }

    private string GetUpgradeKey(string rawItemKey)
    {
        switch (rawItemKey)
        {
            case "rawsapphire":
                return "flawedsapphire";
            case "flawedsapphire":
                return "uncutsapphire";
            case "uncutsapphire":
                return "pristinesapphire";
            case "rawruby":
                return "flawedruby";
            case "flawedruby":
                return "uncutruby";
            case "uncutruby":
                return "pristineruby";
            case "rawemerald":
                return "flawedemerald";
            case "flawedemerald":
                return "uncutemerald";
            case "uncutemerald":
                return "pristineemerald";
            case "rawheartstone":
                return "flawedheartstone";
            case "flawedheartstone":
                return "uncutheartstone";
            case "uncutheartstone":
                return "pristineheartstone";
            default:
                return rawItemKey;
        }
    }

    /// <inheritdoc />
    public override void OnDisplaying(Aisling source)
    {
        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "gem_refining_initial":
            {
                OnDisplayingShowPlayerItems(source);

                break;
            }
            case "gem_refining_confirmation":
            {
                OnDisplayingConfirmation(source);

                break;
            }
            case "gem_refining_accepted":
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
            Subject.Reply(source, $"Your attempt to refine {item.Template.Name} has failed.", "gem_refining_initial");
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
            Subject.Reply(source, "Skip", "gem_refining_initial");

            return;
        }

        if (!MiningTemplateKeys.Contains(item.Template.TemplateKey))
        {
            Subject.Reply(source, "Item cannot be refined", "gem_refining_initial");

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
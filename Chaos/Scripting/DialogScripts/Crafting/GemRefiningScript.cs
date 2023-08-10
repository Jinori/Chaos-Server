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
    private readonly IDialogFactory DialogFactory;

    private readonly List<string> MiningTemplateKeys = new()
    {
        "rawberyl", "flawedberyl", "uncutberyl", 
       "rawsapphire", "flawedsapphire", "uncutsapphire", 
        "rawruby", "flawedruby", "uncutruby", 
        "rawemerald", "flawedemerald", "uncutemerald", 
        "rawheartstone", "flawedheartstone", "uncutheartstone"
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
    public GemRefiningScript(Dialog subject, IItemFactory itemFactory, IDialogFactory dialogFactory)
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

    private string GetDowngradeKey(string rawItemKey) =>
        rawItemKey.ToLower() switch
        {
            "rawberyl"         => "chippedberyl",
            "flawedberyl"      => "chippedberyl",
            "uncutberyl"       => "chippedberyl",
            "rawsapphire"      => "chippedsapphire",
            "flawedsapphire"   => "chippedsapphire",
            "uncutsapphire"    => "chippedsapphire",
            "rawruby"          => "chippedruby",
            "flawedruby"       => "chippedruby",
            "uncutruby"        => "chippedruby",
            "rawemerald"       => "chippedemerald",
            "flawedemerald"    => "chippedemerald",
            "uncutemerald"     => "chippedemerald",
            "rawheartstone"    => "chippedheartstone",
            "flawedheartstone" => "chippedheartstone",
            "uncutheartstone"  => "chippedheartstone",
            _                  => rawItemKey
        };

    private string GetUpgradeKey(string rawItemKey)
    {
        var randomNumber = new Random().Next(1, 101);

        switch (rawItemKey.ToLower())
        {
            case "rawberyl":
                if (randomNumber <= 10) 
                    return "pristineberyl";
                if (randomNumber <= 35) 
                    return "uncutberyl";

                return "flawedberyl";
            
            case "flawedberyl":
                if (randomNumber <= 15)
                    return "pristineberyl"; 
                
                return "uncutberyl";
                
            case "uncutberyl":
                return "pristineberyl";
            
            case "rawsapphire":
                if (randomNumber <= 10) 
                    return "pristinesapphire";
                if (randomNumber <= 35) 
                    return "uncutsapphire";

                return "flawedsapphire";
            

            case "flawedsapphire":
                if (randomNumber <= 15)
                    return "pristinesapphire"; 
                
                return "uncutsapphire";
                
            case "uncutsapphire":
                return "pristinesapphire";
            
            case "rawruby":
                if (randomNumber <= 10) 
                    return "pristineruby";
                if (randomNumber <= 35) 
                    return "uncutruby";

                return "flawedruby";
            
            case "flawedruby":
                if (randomNumber <= 15)
                    return "pristineruby";
 
                return "uncutruby";
            
            case "uncutruby":
                return "pristineruby";
            
            case "rawemerald":
                if (randomNumber <= 10) 
                    return "pristineemerald";
                if (randomNumber <= 35) 
                    return "uncutemerald";

                return "flawedemerald";
            
            case "flawedemerald":
                if (randomNumber <= 15)
                    return "pristineemerald";

                return "uncutemerald";
            
            case "uncutemerald":
                return "pristineemerald";
            
            case "rawheartstone":
                if (randomNumber <= 10) 
                    return "pristineheartstone";
                if (randomNumber <= 35) 
                    return "uncutheartstone";

                return "flawedheartstone";
            
            case "flawedheartstone":
                if (randomNumber <= 15)
                    return "pristineemerald";

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
            Subject.Reply(source, $"You ran out of those gems to refine.", "gem_refining_initial");

            return;
        }

        source.Inventory.RemoveQuantityByTemplateKey(item.Template.TemplateKey, 1);

        var timesCraftedThisItem =
            source.Trackers.Counters.TryGetValue(ITEM_COUNTER_PREFIX + item.Template.TemplateKey, out var value) ? value : 0;

        if (!IntegerRandomizer.RollChance((int)CalculateSuccessRate(source, BASE_SUCCESS_RATE, timesCraftedThisItem)))
        {
            Subject.Close(source);
            var dialog = DialogFactory.Create("gem_refining_failed", Subject.DialogSource);
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

        if (!MiningTemplateKeys.Contains(item.Template.TemplateKey.ToLower()))
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
        source.Inventory.Where(x => MiningTemplateKeys.Contains(x.Template.TemplateKey.ToLower())).Select(x => x.Slot).ToList();
}
using Chaos.Common.Utilities;
using Chaos.Extensions.Common;
using Chaos.Models.Menu;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Crafting.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.DialogScripts.Crafting.Refining;

public class GemRefiningScript : CraftingBaseScript
{    /// <inheritdoc />
    public GemRefiningScript(Dialog subject, IItemFactory itemFactory, IDialogFactory dialogFactory)
        : base(subject, itemFactory, dialogFactory) { }
    
    protected override string LegendMarkKey => "GemRefining";
    protected override string ItemCounterPrefix => "[Refine]";
    protected override double BaseSucessRate => 60;
    protected override double SuccessRateMax => 90;
    protected override Dictionary<string, string> UpgradeMappings => null!;

    private readonly string[] GemTemplateKeys =
        ["rawberyl", "flawedberyl", "uncutberyl",
         "rawsapphire", "flawedsapphire", "uncutsapphire",
         "rawruby", "flawedruby", "uncutruby",
         "rawemerald", "flawedemerald", "uncutemerald",
         "rawheartstone", "flawedheartstone", "uncutheartstone"];

    private string GetRandomUpgrade(string itemKey)
    {
        var randomNumber = new Random().Next(1, 101);
        return itemKey.ToLower() switch
        {
            "rawberyl"         => randomNumber <= 10 ? "pristineberyl" : randomNumber <= 35 ? "uncutberyl" : "flawedberyl",
            "flawedberyl"      => randomNumber <= 15 ? "pristineberyl" : "uncutberyl",
            "uncutberyl"       => "pristineberyl",
            "rawsapphire"      => randomNumber <= 10 ? "pristinesapphire" : randomNumber <= 35 ? "uncutsapphire" : "flawedsapphire",
            "flawedsapphire"   => randomNumber <= 15 ? "pristinesapphire" : "uncutsapphire",
            "uncutsapphire"    => "pristinesapphire",
            "rawruby"          => randomNumber <= 10 ? "pristineruby" : randomNumber <= 35 ? "uncutruby" : "flawedruby",
            "flawedruby"       => randomNumber <= 15 ? "pristineruby" : "uncutruby",
            "uncutruby"        => "pristineruby",
            "rawemerald"       => randomNumber <= 10 ? "pristineemerald" : randomNumber <= 35 ? "uncutemerald" : "flawedemerald",
            "flawedemerald"    => randomNumber <= 15 ? "pristineemerald" : "uncutemerald",
            "uncutemerald"     => "pristineemerald",
            "rawheartstone"    => randomNumber <= 10 ? "pristineheartstone" : randomNumber <= 35 ? "uncutheartstone" : "flawedheartstone",
            "flawedheartstone" => randomNumber <= 15 ? "pristineheartstone" : "uncutheartstone",
            "uncutheartstone"  => "pristineheartstone",
            _                  => itemKey
        };
    }
    
    protected override Dictionary<string, string> DowngradeMappings { get; } = new(StringComparer.OrdinalIgnoreCase)
    {
        {"rawberyl", "chippedberyl"},
        {"flawedberyl", "chippedberyl"},
        {"uncutberyl", "chippedberyl"},
        {"rawsapphire", "chippedsapphire"},
        {"flawedsapphire", "chippedsapphire"},
        {"uncutsapphire", "chippedsapphire"},
        {"rawruby", "chippedruby"},
        {"flawedruby", "chippedruby"},
        {"uncutruby", "chippedruby"},
        {"rawemerald", "chippedemerald"},
        {"flawedemerald", "chippedemerald"},
        {"uncutemerald", "chippedemerald"},
        {"rawheartstone", "chippedheartstone"},
        {"flawedheartstone", "chippedheartstone"},
        {"uncutheartstone", "chippedheartstone"}
    };

    public override void OnDisplaying(Aisling source)
    {
        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "gem_refining_initial":
                OnDisplayingShowPlayerItems(source);
                break;
            case "gem_refining_confirmation":
                OnDisplayingConfirmation(source);
                break;
            case "gem_refining_accepted":
                OnDisplayingAccepted(source);
                break;
        }
    }

    public void OnDisplayingShowPlayerItems(Aisling source) =>
        Subject.Slots = source.Inventory
                              .Where(x => GemTemplateKeys.ContainsI(x.Template.TemplateKey) && (x.Count > 1))
                              .Select(x => x.Slot)
                              .ToList();
    
    private void OnDisplayingConfirmation(Aisling source)
    {
        if (!TryFetchArg<byte>(0, out var slot) || !source.Inventory.TryGetObject(slot, out var item) || (item.Count < RequiredQuantity(item.Template.TemplateKey)))
        {
            Subject.Reply(source, "You don't have enough material.", "gem_refining_initial");
            return;
        }

        if (!GemTemplateKeys.ContainsI(item.Template.TemplateKey))
        {
            Subject.Reply(source, "This item cannot be refined. Please select a valid material.", "gem_refining_initial");
            return;
        }

        var successRate = CalculateRefiningSuccessRate(source, item);
        Subject.InjectTextParameters(item.DisplayName, successRate.ToString("N2"));
    }

    private void OnDisplayingAccepted(Aisling source)
    {
        if (!TryFetchArg<byte>(0, out var slot)
            || !source.Inventory.TryGetObject(slot, out var item)
            || (item.Count < RequiredQuantity(item.Template.TemplateKey)))
        {
            Subject.Reply(source, $"You ran out of gems to refine.", "gem_refining_initial");
            return;
        }

        source.Inventory.RemoveQuantityByTemplateKey(item.Template.TemplateKey, RequiredQuantity(item.Template.TemplateKey));
        var successRate = CalculateRefiningSuccessRate(source, item);

        if (!IntegerRandomizer.RollChance((int)successRate))
        {
            UpdateRefiningCounter(source, item);
            HandleFailure(source, item, GetDowngradeKey(item.Template.TemplateKey), item.DisplayName, "gem_refining_failed");
            return;
        }

        HandleSuccessRandomGem(source, item);
    }

    protected void HandleSuccessRandomGem(Aisling source, Item item)
    {
        UpdateRefiningCounter(source, item);

        var upgradeKey = GetRandomUpgrade(item.Template.TemplateKey);
        var upgrade = ItemFactory.Create(upgradeKey);
        source.GiveItemOrSendToBank(upgrade);

        AnimateSucess(source);
        Subject.InjectTextParameters(item.DisplayName, upgrade.DisplayName);
    }
    
    private int RequiredQuantity(string templateKey) => templateKey.Contains("raw") ? 2 : 1;
}
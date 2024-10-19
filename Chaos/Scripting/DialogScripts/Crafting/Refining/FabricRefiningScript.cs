using Chaos.Common.Utilities;
using Chaos.Extensions.Common;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Crafting.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.DialogScripts.Crafting.Refining;

public class FabricRefiningScript(Dialog subject, IItemFactory itemFactory, IDialogFactory dialogFactory)
    : CraftingBaseScript(subject, itemFactory, dialogFactory)
{
    protected override string LegendMarkKey => "FabricRefining";
    protected override string ItemCounterPrefix => "[Refine]";
    protected override double BaseSucessRate => 60;
    protected override double SuccessRateMax => 90;

    private readonly string[] FabricTemplateKeys =
        ["linen", "finelinen", "cotton", "finecotton", "wool", "finewool", "silk", "finesilk", "hemp", "finehemp", "exquisitehemp"];

    protected override Dictionary<string, string> UpgradeMappings { get; } = new(StringComparer.OrdinalIgnoreCase)
    {
        {"linen", "finelinen"},
        {"finelinen", "exquisitelinen"},
        {"cotton", "finecotton"},
        {"finecotton", "exquisitecotton"},
        {"wool", "finewool"},
        {"finewool", "exquisitewool"},
        {"silk", "finesilk"},
        {"finesilk", "exquisitesilk"},
        { "hemp", "finehemp"},
        { "finehemp", "exquisitehemp"},
    };
    
    protected override Dictionary<string, string> DowngradeMappings { get; } = new(StringComparer.OrdinalIgnoreCase)
    {
        {"linen", "tornlinen"},
        {"finelinen", "tornlinen"},
        {"cotton", "torncotton"},
        {"finecotton", "torncotton"},
        {"wool", "tornwool"},
        {"finewool", "tornwool"},
        {"silk", "tornsilk"},
        {"finesilk", "tornsilk"},
        {"hemp", "tornhemp"},
        {"finehemp", "tornhemp"},
    };

    public override void OnDisplaying(Aisling source)
    {
        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "fabric_refining_initial":
                OnDisplayingShowPlayerItems(source);
                break;
            case "fabric_refining_confirmation":
                OnDisplayingConfirmation(source);
                break;
            case "fabric_refining_accepted":
                OnDisplayingAccepted(source);
                break;
        }
    }

    public void OnDisplayingShowPlayerItems(Aisling source) =>
        Subject.Slots = source.Inventory
                              .Where(x => FabricTemplateKeys.Contains(x.Template.TemplateKey))
                              .Select(x => x.Slot)
                              .ToList();
    
    private void OnDisplayingConfirmation(Aisling source)
    {
        if (!TryFetchArg<byte>(0, out var slot) || !source.Inventory.TryGetObject(slot, out var item) || (item.Count < 1))
        {
            Subject.Reply(source, "You don't have enough material.", "fabric_refining_initial");
            return;
        }
        
        if (!FabricTemplateKeys.ContainsI(item.Template.TemplateKey))
        {
            Subject.Reply(source, "This item cannot be refined. Please select a valid material.", "fabric_refining_initial");
            return;
        }

        var successRate = CalculateRefiningSuccessRate(source, item);
        Subject.InjectTextParameters(item.DisplayName, successRate.ToString("N2"));
    }
    
    private void OnDisplayingAccepted(Aisling source)
    {
        if (!TryFetchArg<byte>(0, out var slot) || !source.Inventory.TryGetObject(slot, out var item))
        {
            Subject.Reply(source, "You ran out of that fabric to refine.", "fabric_refining_initial");
            return;
        }
        
        if (item.Count < 1)
        {
            Subject.Reply(source, $"You ran out of {item.DisplayName} to refine.", "fabric_refining_initial");
            return;
        }

        source.Inventory.RemoveQuantityByTemplateKey(item.Template.TemplateKey, 1);
        var successRate = CalculateRefiningSuccessRate(source, item);

        if (!IntegerRandomizer.RollChance((int)successRate))
        {
            UpdateRefiningCounter(source, item);
            HandleFailure(source, item, GetDowngradeKey(item.Template.TemplateKey), item.DisplayName, "fabric_refining_failed");
            return;
        }

        HandleSuccess(source, item);
    }
}
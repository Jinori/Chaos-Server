using Chaos.Common.Definitions;
using Chaos.Models.Data;
using Chaos.Models.Legend;
using Chaos.Models.Menu;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Time;

namespace Chaos.Scripting.DialogScripts.Crafting.Abstractions;

public abstract class CraftingBaseScript : DialogScriptBase
{
    /// <inheritdoc />
    protected CraftingBaseScript(Dialog subject, IItemFactory itemFactory, IDialogFactory dialogFactory)
        : base(subject)
    {
        ItemFactory = itemFactory;
        DialogFactory = dialogFactory;
    }

    protected readonly IItemFactory ItemFactory;
    protected readonly IDialogFactory DialogFactory;
    protected abstract string LegendMarkKey { get; }
    protected virtual string[] RankTitles => [];
    protected abstract string ItemCounterPrefix { get; }
    protected abstract double BaseSucessRate { get; }
    protected abstract double SuccessRateMax { get; }
    protected abstract Dictionary<string, string> UpgradeMappings { get; }
    
    protected abstract Dictionary<string, string> DowngradeMappings { get; }
    
    private const string RECIPE_ONE_RANK = "Beginner";
    private const string RECIPE_TWO_RANK = "Basic";
    private const string RECIPE_THREE_RANK = "Initiate";
    private const string RECIPE_FOUR_RANK = "Artisan";
    private const string RECIPE_FIVE_RANK = "Adept";
    private const string RECIPE_SIX_RANK = "Advanced";
    private const string RECIPE_SEVEN_RANK = "Expert";
    private const string RECIPE_EIGHT_RANK = "Master";
    
    private readonly List<int> RankThresholds =
    [
        ..new[]
        {
            25, 75, 150, 300, 500, 1000, 1500
        }
    ];

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
    
    public int GetStatusAsInt(string status)
    {
        var statusMappings = new Dictionary<string, int>
        {
            { RECIPE_ONE_RANK, 1 },
            { RECIPE_TWO_RANK, 2 },
            { RECIPE_THREE_RANK, 3 },
            { RECIPE_FOUR_RANK, 4 },
            { RECIPE_FIVE_RANK, 5 },
            { RECIPE_SIX_RANK, 6 },
            { RECIPE_SEVEN_RANK, 7 },
            { RECIPE_EIGHT_RANK, 8 }
        };

        if (statusMappings.TryGetValue(status, out var i))
            return i;

        return 0;
    }
    
    private double GetMultiplier(int totalTimesCrafted) =>
        totalTimesCrafted switch
        {
            <= 25   => 1.0,
            <= 75   => 1.05,
            <= 150  => 1.1,
            <= 300  => 1.15,
            <= 500  => 1.2,
            <= 1000 => 1.25,
            <= 1500 => 1.3,
            _       => 1.35
        };
    
    protected string GetUpgradeKey(string rawItemKey)
    {
        if (UpgradeMappings.TryGetValue(rawItemKey, out var upgradeKey))
            return upgradeKey;
        
        return rawItemKey;
    }

    protected string GetDowngradeKey(string rawItemKey)
    {
        if (DowngradeMappings.TryGetValue(rawItemKey, out var downgradeKey))
            return downgradeKey;
        
        return rawItemKey;
    }
    
    public void AnimateFailure(MapEntity source) => source.Animate(FailAnimation);
    public void AnimateSucess(MapEntity source) => source.Animate(SuccessAnimation);

    protected void HandleFailure(Aisling source, Item item, string downgradeKey, string itemDisplayName, string failureDialogKey)
    {
        Subject.Close(source);
        var dialog = DialogFactory.Create(failureDialogKey, Subject.DialogSource);
        dialog.MenuArgs = Subject.MenuArgs;
        dialog.Context = Subject.Context;
        dialog.InjectTextParameters(itemDisplayName);
        dialog.Display(source);
        
        var downgradeItem = ItemFactory.Create(downgradeKey);
        source.GiveItemOrSendToBank(downgradeItem);
        AnimateFailure(source);
    }
    
    protected void HandleSuccess(Aisling source, Item item)
    {
        UpdateRefiningCounter(source, item);

        var upgrade = ItemFactory.Create(GetUpgradeKey(item.Template.TemplateKey));
        source.GiveItemOrSendToBank(upgrade);

        AnimateSucess(source);
        Subject.InjectTextParameters(item.DisplayName, upgrade.DisplayName);
    }
    
    public double CalculateRefiningSuccessRate(Aisling source, Item item)
    {
        var timesCraftedSingleItem =
            source.Trackers.Counters.TryGetValue(ItemCounterPrefix + item.Template.TemplateKey, out var value) ? value : 0;

        var sucessRate = BaseSucessRate + timesCraftedSingleItem / 10.0;

        return Math.Min(sucessRate, SuccessRateMax);
    }

    public double CalculateCraftingSuccessRate(Aisling source, Item item, string recipeRank, int recipeDifficulty)
    {
        source.Legend.TryGetValue(LegendMarkKey, out var existingMark);
        var legendMarkCount = existingMark?.Count ?? 0;
        
        var timesCraftedSingleItem =
            source.Trackers.Counters.TryGetValue(ItemCounterPrefix + item.Template.TemplateKey, out var value) ? value : 0;
        
        var recipeDif = GetStatusAsInt(recipeRank);
        var multiplier = GetMultiplier(legendMarkCount);
        var successRate = (BaseSucessRate - recipeDif - recipeDifficulty + timesCraftedSingleItem / 5.0) * multiplier;
        return Math.Min(successRate, SuccessRateMax);
    }

    public void UpdateRefiningCounter(Aisling source, Item item) => source.Trackers.Counters.AddOrIncrement(ItemCounterPrefix + item.Template.TemplateKey);
    
    public void UpdateLegendmark(Aisling source, int craftCount)
    {
        if (!source.Legend.TryGetValue(LegendMarkKey, out var existingMark))
            AddInitialLegendMark(source);
        else
            UpdateExistingLegendMark(source, existingMark, craftCount);
    }

    protected virtual void AddInitialLegendMark(Aisling source) =>
        source.Legend.AddOrAccumulate(
            new LegendMark(
                GetRankTitle(0),
                LegendMarkKey,
                MarkIcon.Yay,
                MarkColor.White,
                1,
                GameTime.Now));

    protected virtual void UpdateExistingLegendMark(Aisling source, LegendMark existingMark, int craftCount)
    {
        var currentRankIndex = Array.IndexOf(RankTitles, existingMark.Text);

        existingMark.Count++;
        CheckForRankUpgrade(source, existingMark, currentRankIndex, craftCount);
    }

    protected virtual void CheckForRankUpgrade(Aisling source, LegendMark existingMark, int currentRankIndex, int craftCount)
    {
        for (var i = currentRankIndex + 1; i < RankThresholds.Count; i++)
        {
            if (craftCount >= RankThresholds[i])
            {
                UpgradeRank(source, existingMark, GetRankTitle(i));
                break;
            }
        }
    }

    protected virtual void UpgradeRank(Aisling source, LegendMark existingMark, string newTitle)
    {
        RemoveOldTitle(source, existingMark.Text);
        source.Titles.Add(newTitle);
        existingMark.Text = newTitle;
        source.SendOrangeBarMessage($"You have reached the rank of {newTitle}!");
        PromoteTitleToFront(source, newTitle);
        source.Client.SendSelfProfile();
    }

    protected virtual void RemoveOldTitle(Aisling source, string oldTitle) => source.Titles.Remove(oldTitle);

    protected virtual void PromoteTitleToFront(Aisling source, string newTitle)
    {
        if (source.Titles.Any() && (source.Titles.First() != newTitle))
        {
            source.Titles.Remove(newTitle);
            source.Titles.Insert(0, newTitle);
        }
    }

    protected virtual string GetRankTitle(int index)
    {
        if ((index < 0) || (index >= RankTitles.Length))
            throw new IndexOutOfRangeException("Rank index is out of range.");

        return RankTitles[index];
    }
}
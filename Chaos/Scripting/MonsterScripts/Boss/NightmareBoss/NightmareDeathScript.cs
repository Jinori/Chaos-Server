using Chaos.Collections;
using Chaos.Common.Definitions;
using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Models.Legend;
using Chaos.Models.World;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ExperienceDistribution;
using Chaos.Scripting.MonsterScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Services.Servers.Options;
using Chaos.Services.Storage;
using Chaos.Storage.Abstractions;
using Chaos.Time;

namespace Chaos.Scripting.MonsterScripts.Boss.NightmareBoss;

// ReSharper disable once ClassCanBeSealed.Global
public class NightmareDeathScript : MonsterScriptBase
{
    protected IExperienceDistributionScript ExperienceDistributionScript { get; set; }
    private readonly IItemFactory ItemFactory;
    private readonly ISimpleCache SimpleCache;

    /// <inheritdoc />
    public NightmareDeathScript(Monster subject, IItemFactory itemFactory, ISimpleCache simpleCache)
        : base(subject)
    {
        ItemFactory = itemFactory;
        SimpleCache = simpleCache;
        ExperienceDistributionScript = DefaultExperienceDistributionScript.Create();
    }

    /// <inheritdoc />
    public override void OnDeath()
    {
        if (!Map.RemoveObject(Subject))
            return;

        //this code will set the reward target to the person at the top of the aggro list
        //var rewardTarget = Subject.AggroList
        //                          .OrderByDescending(kvp => kvp.Value)
        //                          .Select(kvp => Map.TryGetEntity<Aisling>(kvp.Key, out var a) ? a : null)
        //                          .FirstOrDefault(a => a is not null);

        //get the highest contributor
        //if there are no contributor, try getting the highest aggro
        var rewardTarget = Subject.Contribution
                                  .OrderByDescending(kvp => kvp.Value)
                                  .Select(kvp => Map.TryGetEntity<Aisling>(kvp.Key, out var a) ? a : null)
                                  .FirstOrDefault(a => a is not null);

        Aisling[]? rewardTargets = null;

        if (rewardTarget != null)
            rewardTargets = (rewardTarget.Group ?? (IEnumerable<Aisling>)new[] { rewardTarget })
                            .ThatAreWithinRange(rewardTarget)
                            .ToArray();

        Subject.Items.AddRange(Subject.LootTable.GenerateLoot());

        var droppedGold = Subject.TryDropGold(Subject, Subject.Gold, out var money);
        var droppedITems = Subject.TryDrop(Subject, Subject.Items, out var groundItems);

        if (rewardTargets is not null)
        {
            if (WorldOptions.Instance.LootDropsLockToRewardTargetSecs.HasValue)
            {
                var lockSecs = WorldOptions.Instance.LootDropsLockToRewardTargetSecs.Value;

                if (droppedGold)
                    money!.LockToAislings(lockSecs, rewardTargets);

                if (droppedITems)
                    foreach (var groundItem in groundItems!)
                        groundItem.LockToAislings(lockSecs, rewardTargets);
            }

            ExperienceDistributionScript.DistributeExperience(Subject, rewardTargets);
            
            var nightmaregearDictionary = new Dictionary<(BaseClass, Gender), string[]>
            {
                { (BaseClass.Warrior, Gender.Male), new string[] { "malecarnunplate", "carnunhelmet" } },
                { (BaseClass.Warrior, Gender.Female), new string[] { "femalecarnunplate", "carnunhelmet" } },
                { (BaseClass.Monk, Gender.Male), new string[] { "maleaosdicpatternwalker" } },
                { (BaseClass.Monk, Gender.Female), new string[] { "femaleaosdicpatternwalker" } },
                { (BaseClass.Rogue, Gender.Male), new string[] { "malemarauderhide", "maraudermask" } },
                { (BaseClass.Rogue, Gender.Female), new string[] { "femalemarauderhide", "maraudermask" } },
                { (BaseClass.Priest, Gender.Male), new string[] { "malecthonicdisciplerobes", "cthonicdisciplecaputium" } },
                { (BaseClass.Priest, Gender.Female), new string[] { "morrigudisciplepellison", "holyhairband" } },
                { (BaseClass.Wizard, Gender.Male), new string[] { "malecthonicmagusrobes", "cthonicmaguscaputium" } },
                { (BaseClass.Wizard, Gender.Female), new string[] {"morrigumaguspellison", "magushairband"} }
            };

            foreach (var target in rewardTargets)
            {
                target.Trackers.Enums.Set(NightmareQuestStage.CompletedNightmareWin1);
                target.SendOrangeBarMessage("You have conquered your Nightmares!");
                target.Legend.AddOrAccumulate(
                    new LegendMark(
                        "Conquered their Nightmares.",
                        "Nightmare",
                        MarkIcon.Victory,
                        MarkColor.White,
                        1,
                        GameTime.Now));
                
                var mapInstance = SimpleCache.Get<MapInstance>("mileth_inn");
                var pointS = new Point(5, 7);
                target.TraverseMap(mapInstance, pointS);

                var gearKey = (target.UserStatSheet.BaseClass, target.Gender); 
                if (nightmaregearDictionary.TryGetValue(gearKey, out var nightmaregear))
                { 
                    var hasGear = nightmaregear.All(gearItemName =>
                        target.Inventory.Contains(gearItemName) || target.Bank.Contains(gearItemName) || target.Equipment.Contains(gearItemName));

                    if (hasGear)
                    {
                        foreach (var gearItemName in nightmaregear)
                        {
                            var gearItem = ItemFactory.Create(gearItemName);
                            target.GiveItemOrSendToBank(gearItem);
                        }
                    }
                }
            }
        }
    }
}
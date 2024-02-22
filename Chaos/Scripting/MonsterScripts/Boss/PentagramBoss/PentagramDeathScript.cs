using Chaos.Common.Definitions;
using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ExperienceDistribution;
using Chaos.Scripting.MonsterScripts.Abstractions;
using Chaos.Services.Factories;
using Chaos.Services.Factories.Abstractions;
using Chaos.Services.Servers.Options;

namespace Chaos.Scripting.MonsterScripts.Boss.PentagramBoss;

// ReSharper disable once ClassCanBeSealed.Global
public class PentagramDeathScript : MonsterScriptBase
{
    protected IExperienceDistributionScript ExperienceDistributionScript { get; set; }
    private readonly IItemFactory ItemFactory;

    /// <inheritdoc />
    public PentagramDeathScript(Monster subject, IItemFactory itemFactory)
        : base(subject)
    {
        ItemFactory = itemFactory;
        ExperienceDistributionScript = DefaultExperienceDistributionScript.Create();
    }

    /// <inheritdoc />
    public override void OnDeath()
    {
        if (!Map.RemoveEntity(Subject))
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

            var pentagearDictionary = new Dictionary<(BaseClass, Gender), string[]>
            {
                { (BaseClass.Warrior, Gender.Male), ["malescarletcarapace", "scarletchitinhelmet"] },
                { (BaseClass.Warrior, Gender.Female), ["femalescarletcarapace", "scarletchitinhelmet"] },
                { (BaseClass.Monk, Gender.Male), ["malenagatiercloak"] },
                { (BaseClass.Monk, Gender.Female), ["femalenagatiercloak"] },
                { (BaseClass.Rogue, Gender.Male), ["malereitermail", "kopfloserhood"] },
                { (BaseClass.Rogue, Gender.Female), ["femalereitermail", "kopfloserhood"] },
                { (BaseClass.Priest, Gender.Male), ["maledarkclericrobes", "darkclericbrim"] },
                { (BaseClass.Priest, Gender.Female), ["femaledarkclericrobes", "darkclericbrim"] },
                { (BaseClass.Wizard, Gender.Male), ["malelichrobe", "lichhood"] },
                { (BaseClass.Wizard, Gender.Female), ["femalelichrobe", "lichhood"] }
            };

            foreach (var target in rewardTargets)
            {
                target.Trackers.Enums.Set(PentagramQuestStage.DefeatedBoss);
                target.SendOrangeBarMessage("The house calms down, and the darkness fades.");
                target.Trackers.Counters.AddOrIncrement("pentabosskills");

                var gearKey = (target.UserStatSheet.BaseClass, target.Gender);
                if (pentagearDictionary.TryGetValue(gearKey, out var pentagear))
                {
                        foreach (var gearItemName in pentagear)
                        {
                            var gearItem = ItemFactory.Create(gearItemName);
                            target.GiveItemOrSendToBank(gearItem);
                        }
                }


                if (target.Trackers.Counters.TryGetValue("pentabosskills", out var kills) && (kills == 10))
                {
                    target.Trackers.Flags.AddFlag(CookingRecipes.LobsterDinner);
                    target.SendOrangeBarMessage("You have learned how to cook Lobster Dinner!");
                }
            }
        }
    }
}
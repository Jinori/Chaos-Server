using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Models.World;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ExperienceDistribution;
using Chaos.Scripting.MapScripts.MainStoryLine;
using Chaos.Scripting.MonsterScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Services.Servers.Options;

namespace Chaos.Scripting.MonsterScripts.Boss.Creants;

// ReSharper disable once ClassCanBeSealed.Global
public class CreantDeathScript : MonsterScriptBase
{
    private readonly IItemFactory ItemFactory;
    protected IExperienceDistributionScript ExperienceDistributionScript { get; set; }

    /// <inheritdoc />
    public CreantDeathScript(Monster subject, IItemFactory itemFactory)
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
            rewardTargets = (rewardTarget.Group
                             ?? (IEnumerable<Aisling>)new[]
                             {
                                 rewardTarget
                             }).ThatAreWithinRange(rewardTarget)
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
            
            var mapScript = Subject.MapInstance.Script.As<CreantBossMapScript>();
            if (mapScript != null)
                mapScript.State = CreantBossMapScript.ScriptState.CreantKilled;

            if (Subject.Template.TemplateKey == "Phoenix")
            {
                var startedPhoenix = Map.GetEntities<Aisling>()
                                        .Where(
                                            x => x.Trackers.Enums.TryGetValue(out MainstoryMasterEnums stage)
                                                 && (stage == MainstoryMasterEnums.StartedCreants)
                                                 && x.Trackers.Flags.HasFlag(CreantEnums.StartedPhoenix))
                                        .ToList();

                foreach (var player in startedPhoenix)
                {
                    player.Trackers.Flags.RemoveFlag(CreantEnums.StartedPhoenix);
                    player.Trackers.Flags.AddFlag(CreantEnums.KilledPhoenix);
                    player.SendOrangeBarMessage("Lady Phoenix is ready to be sealed away, use the altar.");
                }
            }

            if (Subject.Template.TemplateKey == "Medusa")
            {
                var startedMedusa = Map.GetEntities<Aisling>()
                                       .Where(
                                           x => x.Trackers.Enums.TryGetValue(out MainstoryMasterEnums stage)
                                                && (stage == MainstoryMasterEnums.StartedCreants)
                                                && x.Trackers.Flags.HasFlag(CreantEnums.StartedMedusa))
                                       .ToList();

                foreach (var player in startedMedusa)
                {
                    player.Trackers.Flags.RemoveFlag(CreantEnums.StartedMedusa);
                    player.Trackers.Flags.AddFlag(CreantEnums.KilledMedusa);
                    player.SendOrangeBarMessage("Medusa is ready to be sealed away, use the altar.");
                }
            }

            if (Subject.Template.TemplateKey == "Tauren")
            {
                var startedTauren = Map.GetEntities<Aisling>()
                                       .Where(
                                           x => x.Trackers.Enums.TryGetValue(out MainstoryMasterEnums stage)
                                                && (stage == MainstoryMasterEnums.StartedCreants)
                                                && x.Trackers.Flags.HasFlag(CreantEnums.StartedTauren))
                                       .ToList();

                foreach (var player in startedTauren)
                {
                    player.Trackers.Flags.RemoveFlag(CreantEnums.StartedTauren);
                    player.Trackers.Flags.AddFlag(CreantEnums.KilledTauren);
                    player.SendOrangeBarMessage("Tauren is ready to be sealed away, use the altar.");
                }
            }

            if (Subject.Template.TemplateKey == "Shamensyth")
            {
                var startedTauren = Map.GetEntities<Aisling>()
                                       .Where(
                                           x => x.Trackers.Enums.TryGetValue(out MainstoryMasterEnums stage)
                                                && (stage == MainstoryMasterEnums.StartedCreants)
                                                && x.Trackers.Flags.HasFlag(CreantEnums.StartedSham))
                                       .ToList();

                foreach (var player in startedTauren)
                {
                    player.Trackers.Flags.RemoveFlag(CreantEnums.StartedSham);
                    player.Trackers.Flags.AddFlag(CreantEnums.KilledSham);
                    player.SendOrangeBarMessage("Shamensyth is ready to be sealed away, use the altar.");
                }
            }
        }
    }
}
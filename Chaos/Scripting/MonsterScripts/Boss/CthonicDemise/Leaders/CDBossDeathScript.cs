using Chaos.Common.Definitions;
using Chaos.DarkAges.Definitions;
using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Models.World;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ExperienceDistribution;
using Chaos.Scripting.MonsterScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Services.Servers.Options;

namespace Chaos.Scripting.MonsterScripts.Boss.CthonicDemise.Leaders;

public class CdBossDeathScript : MonsterScriptBase
{
    private readonly ISkillFactory SkillFactory;

    private readonly ISpellFactory SpellFactory;

    public CdBossDeathScript(Monster subject, ISkillFactory skillFactory, ISpellFactory spellFactory)
        : base(subject)
    {
        SkillFactory = skillFactory;
        SpellFactory = spellFactory;
        ExperienceDistributionScript = DefaultExperienceDistributionScript.Create();
    }

    protected IExperienceDistributionScript ExperienceDistributionScript { get; set; }

    public override void OnDeath()
    {
        if (!Map.RemoveEntity(Subject))
            return;

        var rewardTarget = GetRewardTarget();
        var rewardTargets = rewardTarget != null ? GetRewardTargets(rewardTarget) : null;

        DropLootAndGold(rewardTargets);

        if (rewardTargets != null)
        {
            ExperienceDistributionScript.DistributeExperience(Subject, rewardTargets);
            HandleMainStoryProgress(rewardTargets);
        }
    }

    private Aisling? GetRewardTarget()
    {
        return Subject.Contribution
            .OrderByDescending(kvp => kvp.Value)
            .Select(kvp => Map.TryGetEntity<Aisling>(kvp.Key, out var aisling) ? aisling : null)
            .FirstOrDefault(a => a != null);
    }

    private Aisling[] GetRewardTargets(Aisling rewardTarget)
    {
        // Create a list to hold the targets
        var targets = new List<Aisling>();

        // Check if the rewardTarget is in a group
        if (rewardTarget.Group != null)
        {
            // Add all group members that are within range to the targets list
            foreach (var member in rewardTarget.Group)
                if (member.WithinRange(rewardTarget))
                    targets.Add(member);
        }
        else
        {
            // If not in a group, just add the rewardTarget itself
            targets.Add(rewardTarget);
        }

        // Return the list as an array
        return targets.ToArray();
    }


    private void DropLootAndGold(Aisling[]? rewardTargets)
    {
        Subject.Items.AddRange(Subject.LootTable.GenerateLoot());

        var droppedGold = Subject.TryDropGold(Subject, Subject.Gold, out var money);
        var droppedItems = Subject.TryDrop(Subject, Subject.Items, out var groundItems);

        if (rewardTargets != null && WorldOptions.Instance.LootDropsLockToRewardTargetSecs.HasValue)
        {
            var lockSecs = WorldOptions.Instance.LootDropsLockToRewardTargetSecs.Value;

            if (droppedGold)
                money!.LockToAislings(lockSecs, rewardTargets);

            if (droppedItems)
                foreach (var groundItem in groundItems!)
                    groundItem.LockToAislings(lockSecs, rewardTargets);
        }
    }

    private void HandleMainStoryProgress(Aisling[] rewardTargets)
    {
        var bossProgressions = new Dictionary<string, CthonicDemiseBoss>
        {
            { "darkmasterjohn", CthonicDemiseBoss.John },
            { "darkmasterjane", CthonicDemiseBoss.Jane },
            { "darkmasterroy", CthonicDemiseBoss.Roy },
            { "darkmasterray", CthonicDemiseBoss.Ray },
            { "darkmastermike", CthonicDemiseBoss.Mike },
            { "darkmastermary", CthonicDemiseBoss.Mary },
            { "darkmasterphil", CthonicDemiseBoss.Phil },
            { "darkmasterpam", CthonicDemiseBoss.Pam },
            { "darkmasterwilliam", CthonicDemiseBoss.William },
            { "darkmasterwanda", CthonicDemiseBoss.Wanda }
        };

        if (bossProgressions.TryGetValue(Subject.Template.TemplateKey, out var bossFlag))
        {
            var nearbyAislingStartedPhoenix = Map
                .GetEntitiesWithinRange<Aisling>(Subject, 13)
                .FirstOrDefault(x => x.Trackers.Enums.TryGetValue(out MainstoryMasterEnums stage) &&
                                     stage == MainstoryMasterEnums.StartedDungeon);

            foreach (var target in rewardTargets)
                if (nearbyAislingStartedPhoenix != null && !target.Trackers.Flags.HasFlag(bossFlag))
                {
                    target.SendOrangeBarMessage($"You've defeated {Subject.Template.Name}.");

                    if (Subject.Template.TemplateKey == "darkmasterjohn")
                        if (target.UserStatSheet.BaseClass == BaseClass.Warrior &&
                            !target.Trackers.Flags.HasFlag(bossFlag))
                        {
                            var skill = SkillFactory.Create("shockwave");
                            target.SkillBook.TryAddToNextSlot(skill);
                            target.SendOrangeBarMessage("You've learned a new ability!");
                        }

                    if (Subject.Template.TemplateKey == "darkmasterjane")
                        if (target.UserStatSheet.BaseClass == BaseClass.Warrior &&
                            !target.Trackers.Flags.HasFlag(bossFlag))
                        {
                            target.SpellBook.Remove("berserk");
                            
                            var spell = SpellFactory.Create("fury");
                            target.SpellBook.TryAddToNextSlot(spell);
                            target.SendOrangeBarMessage("You've learned a new ability that replaced Berserk!");
                        }

                    if (Subject.Template.TemplateKey == "darkmasterroy")
                        if (target.UserStatSheet.BaseClass == BaseClass.Rogue &&
                            !target.Trackers.Flags.HasFlag(bossFlag))
                        {
                            var spell = SpellFactory.Create("vanish");
                            target.SpellBook.TryAddToNextSlot(spell);
                            target.SendOrangeBarMessage("You've learned a new ability!");
                        }

                    if (Subject.Template.TemplateKey == "darkmasterray")
                        if (target.UserStatSheet.BaseClass == BaseClass.Rogue &&
                            !target.Trackers.Flags.HasFlag(bossFlag))
                        {
                            var skill = SkillFactory.Create("surigumblitz");
                            target.SkillBook.TryAddToNextSlot(skill);
                            target.SendOrangeBarMessage("You've learned a new ability.");
                        }

                    if (Subject.Template.TemplateKey == "darkmastermike")
                        if (target.UserStatSheet.BaseClass == BaseClass.Monk &&
                            !target.Trackers.Flags.HasFlag(bossFlag))
                        {
                            var skill = SkillFactory.Create("leveragekick");
                            target.SkillBook.TryAddToNextSlot(skill);
                            target.SendOrangeBarMessage("You've learned a new ability!");
                        }

                    if (Subject.Template.TemplateKey == "darkmastermary")
                        if (target.UserStatSheet.BaseClass == BaseClass.Monk &&
                            !target.Trackers.Flags.HasFlag(bossFlag))
                        {
                            var spell = SpellFactory.Create("cureailments");
                            target.SpellBook.TryAddToNextSlot(spell);
                            target.SendOrangeBarMessage("You've learned a new ability!");
                        }

                    if (Subject.Template.TemplateKey == "darkmasterphil")
                    {
                        if (target.UserStatSheet.BaseClass == BaseClass.Priest &&
                            target.Trackers.Enums.HasValue(MasterPriestPath.Dark) &&
                            !target.Trackers.Flags.HasFlag(bossFlag))
                        {
                            var spell = SpellFactory.Create("miasma");
                            target.SpellBook.TryAddToNextSlot(spell);
                            target.SendOrangeBarMessage("You've learned a new ability!");
                        }

                        if (target.UserStatSheet.BaseClass == BaseClass.Priest &&
                            target.Trackers.Enums.HasValue(MasterPriestPath.Light) &&
                            !target.Trackers.Flags.HasFlag(bossFlag))
                        {
                            var spell = SpellFactory.Create("deolamh");
                            target.SpellBook.TryAddToNextSlot(spell);
                            target.SendOrangeBarMessage("You've learned a new ability!");
                        }
                    }

                    if (Subject.Template.TemplateKey == "darkmasterpam")
                    {
                        if (target.UserStatSheet.BaseClass == BaseClass.Priest &&
                            target.Trackers.Enums.HasValue(MasterPriestPath.Dark) &&
                            !target.Trackers.Flags.HasFlag(bossFlag))
                        {
                            var spell = SpellFactory.Create("bind");
                            target.SpellBook.TryAddToNextSlot(spell);
                            target.SendOrangeBarMessage("You've learned a new ability!");
                        }

                        if (target.UserStatSheet.BaseClass == BaseClass.Priest &&
                            target.Trackers.Enums.HasValue(MasterPriestPath.Light) &&
                            !target.Trackers.Flags.HasFlag(bossFlag))
                        {
                            var spell = SpellFactory.Create("regeneration");
                            target.SpellBook.TryAddToNextSlot(spell);
                            target.SendOrangeBarMessage("You've learned a new ability!");
                        }
                    }

                    if (Subject.Template.TemplateKey == "darkmasterwilliam")
                        if (target.UserStatSheet.BaseClass == BaseClass.Wizard &&
                            !target.Trackers.Flags.HasFlag(bossFlag))
                        {
                            var spell = SpellFactory.Create("chainlightning");
                            target.SpellBook.TryAddToNextSlot(spell);
                        }

                    if (Subject.Template.TemplateKey == "darkmasterwanda")
                        if (target.UserStatSheet.BaseClass == BaseClass.Wizard &&
                            !target.Trackers.Flags.HasFlag(bossFlag))
                        {
                            var spell = SpellFactory.Create("tidalwave");
                            target.SpellBook.TryAddToNextSlot(spell);
                        }

                    target.Trackers.Flags.AddFlag(bossFlag);

                    if (CheckAllBossesDefeated(target))
                    {
                        target.Trackers.Flags.AddFlag(MainstoryFlags.FinishedDungeon);
                        target.Trackers.Enums.Set(MainstoryMasterEnums.FinishedDungeon);
                        target.SendOrangeBarMessage(
                            "You've defeated Summoner Kades's army, visit Goddess Miraelis.");
                    }
                }
        }
    }

    private bool CheckAllBossesDefeated(Aisling target)
    {
        return target.Trackers.Flags.HasFlag(CthonicDemiseBoss.John) &&
               target.Trackers.Flags.HasFlag(CthonicDemiseBoss.Jane) &&
               target.Trackers.Flags.HasFlag(CthonicDemiseBoss.Mike) &&
               target.Trackers.Flags.HasFlag(CthonicDemiseBoss.Mary) &&
               target.Trackers.Flags.HasFlag(CthonicDemiseBoss.Phil) &&
               target.Trackers.Flags.HasFlag(CthonicDemiseBoss.Pam) &&
               target.Trackers.Flags.HasFlag(CthonicDemiseBoss.Roy) &&
               target.Trackers.Flags.HasFlag(CthonicDemiseBoss.Ray) &&
               target.Trackers.Flags.HasFlag(CthonicDemiseBoss.William) &&
               target.Trackers.Flags.HasFlag(CthonicDemiseBoss.Wanda);
    }
}
using Chaos.Collections;
using Chaos.Collections.Abstractions;
using Chaos.Common.Definitions;
using Chaos.Common.Utilities;
using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Extensions.Geometry;
using Chaos.Formulae;
using Chaos.Geometry.Abstractions;
using Chaos.Models.Data;
using Chaos.Models.Legend;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Networking.Abstractions;
using Chaos.NLog.Logging.Definitions;
using Chaos.NLog.Logging.Extensions;
using Chaos.Scripting.Abstractions;
using Chaos.Scripting.AislingScripts.Abstractions;
using Chaos.Scripting.Behaviors;
using Chaos.Services.Servers.Options;
using Chaos.Scripting.Components.AbilityComponents;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ApplyDamage;
using Chaos.Scripting.FunctionalScripts.ApplyHealing;
using Chaos.Scripting.FunctionalScripts.ExperienceDistribution;
using Chaos.Scripting.MonsterScripts.Boss;
using Chaos.Scripting.MonsterScripts.Pet;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;
using FluentAssertions.Execution;

namespace Chaos.Scripting.AislingScripts;

public class DefaultAislingScript : AislingScriptBase, HealAbilityComponent.IHealComponentOptions
{
    private readonly HashSet<string> ArenaKeys = new(StringComparer.OrdinalIgnoreCase) { "arena_battle_ring", "arena_lava", "arena_lavateams", "arena_colorclash", "arena_escort"};
    private readonly IStore<BulletinBoard> BoardStore;
    private readonly IIntervalTimer ClearOrangeBarTimer;
    private readonly IIntervalTimer OneSecondTimer;
    private readonly IIntervalTimer CleanupSkillsSpellsTimer;
    private readonly IClientRegistry<IChaosWorldClient> ClientRegistry;
    private readonly IEffectFactory EffectFactory;
    private readonly ILogger<DefaultAislingScript> Logger;
    private readonly IStore<MailBox> MailStore;
    private readonly List<string> MapsToNotPunishDeathOn =
    [
        "Mr. Hopps's Home",
        "Mythic",
        "Nightmare",
        "Cain's Farm",
        "Arena Battle Ring",
        "Lava Arena",
        "Lava Arena - Teams",
        "Color Clash - Teams",
        "Hidden Havoc",
        "Escort - Teams",
        "Trial of Sacrifice",
        "Trial of Combat",
        "Trial of Luck",
        "Trial of Intelligence",
        "Mileth",
        "Abel",
        "Rucesion",
        "Undine",
        "Suomi",
        "Loures Castle",
        "Loures Castle Way",
        "Tagor",
        "Piet",
        "Rucesion Village Way",
        "Mileth Village Way",
        "Abel Port",
        "Suomi Village Way",
        "Undine Village Way",
        "Piet Village Way",
        "Nobis"
    ];

    private readonly IMerchantFactory MerchantFactory;
    private readonly ISimpleCache SimpleCache;
    private readonly IIntervalTimer SleepAnimationTimer;
    private readonly IItemFactory ItemFactory;
    public IApplyDamageScript ApplyDamageScript { get; init; } 
    /// <inheritdoc />
    public IApplyHealScript ApplyHealScript { get; init; }
    /// <inheritdoc />
    public int? BaseHeal { get; init; }
    /// <inheritdoc />
    public Stat? HealStat { get; init; }
    /// <inheritdoc />
    public decimal? HealStatMultiplier { get; init; }
    /// <inheritdoc />
    public decimal? PctHpHeal { get; init; }
    private SocialStatus PreAfkSocialStatus { get; set; }
    /// <inheritdoc />
    public IScript SourceScript { get; init; }
    private IExperienceDistributionScript ExperienceDistributionScript { get; }

    private Animation LightningStanceStrike { get; } = new()
    {
        AnimationSpeed = 100,
        TargetAnimation = 698
    };
    
    private Animation FlameHit { get; } = new()
    {
        AnimationSpeed = 100,
        TargetAnimation = 870
    };
    
    private Animation MistHeal { get; } = new()
    {
        AnimationSpeed = 100,
        TargetAnimation = 646
    };
    
    private Animation TideHeal { get; } = new()
    {
        AnimationSpeed = 100,
        TargetAnimation = 849
    };
    protected virtual RelationshipBehavior RelationshipBehavior { get; }
    protected virtual RestrictionBehavior RestrictionBehavior { get; }
    protected virtual VisibilityBehavior VisibilityBehavior { get; }

    /// <inheritdoc />
    public DefaultAislingScript(
        Aisling subject,
        IClientRegistry<IChaosWorldClient> clientRegistry,
        IMerchantFactory merchantFactory,
        ISimpleCache simpleCache,
        IEffectFactory effectFactory,
        IStore<MailBox> mailStore,
        IStore<BulletinBoard> boardStore,
        ILogger<DefaultAislingScript> logger,
        IItemFactory itemFactory)
        : base(subject)
    {
        MailStore = mailStore;
        BoardStore = boardStore;
        Logger = logger;
        ItemFactory = itemFactory;
        OneSecondTimer = new IntervalTimer(TimeSpan.FromSeconds(1));
        RestrictionBehavior = new RestrictionBehavior();
        VisibilityBehavior = new VisibilityBehavior();
        RelationshipBehavior = new RelationshipBehavior();
        ExperienceDistributionScript = DefaultExperienceDistributionScript.Create();
        SleepAnimationTimer = new IntervalTimer(TimeSpan.FromSeconds(5), false);
        ClearOrangeBarTimer = new IntervalTimer(TimeSpan.FromSeconds(WorldOptions.Instance.ClearOrangeBarTimerSecs), false);
        CleanupSkillsSpellsTimer =
            new RandomizedIntervalTimer(TimeSpan.FromMinutes(1), 25, RandomizationType.Balanced, false);
        ClientRegistry = clientRegistry;
        EffectFactory = effectFactory;
        MerchantFactory = merchantFactory;
        SimpleCache = simpleCache;
        SourceScript = this;
        ApplyHealScript = ApplyNonAlertingHealScript.Create();
        ApplyDamageScript = ApplyAttackDamageScript.Create();
        ApplyDamageScript.DamageFormula = DamageFormulae.Default;
        ApplyHealScript.HealFormula = HealFormulae.Default;
    }

    /// <inheritdoc />
    public override bool CanMove() => RestrictionBehavior.CanMove(Subject);

    /// <inheritdoc />
    public override bool CanSee(VisibleEntity entity) => VisibilityBehavior.CanSee(Subject, entity);

    /// <inheritdoc />
    public override bool CanTalk() => RestrictionBehavior.CanTalk(Subject);

    /// <inheritdoc />
    public override bool CanTurn() => RestrictionBehavior.CanTurn(Subject);

    /// <inheritdoc />
    public override bool CanUseItem(Item item) => RestrictionBehavior.CanUseItem(Subject, item);

    /// <inheritdoc />
    public override bool CanUseSkill(Skill skill) => RestrictionBehavior.CanUseSkill(Subject, skill);

    /// <inheritdoc />
    public override bool CanUseSpell(Spell spell) => RestrictionBehavior.CanUseSpell(Subject, spell);

    /// <inheritdoc />
    public override IEnumerable<BoardBase> GetBoardList()
    {
        //mailbox board
        yield return MailStore.Load(Subject.Name);

        //change this to whatever naming scheme you want to follow for guild boards
        if (Subject.Guild is not null && BoardStore.Exists(Subject.Guild.Name))
            yield return BoardStore.Load(Subject.Guild.Name);

        //yield return BoardStore.Load("public_test_board");

        //things like... get board based on Nation, Guild, Enums, Flags, whatever
        //e.g.
        //var nationBoard = Subject.Nation switch
        //{
        //    Nation.Exile      => BoardStore.Load("nation_board_exile"),
        //    Nation.Suomi      => BoardStore.Load("nation_board_suomi"),
        //    Nation.Ellas      => BoardStore.Load("nation_board_ellas"),
        //    Nation.Loures     => BoardStore.Load("nation_board_loures"),
        //    Nation.Mileth     => BoardStore.Load("nation_board_mileth"),
        //    Nation.Tagor      => BoardStore.Load("nation_board_tagor"),
        //    Nation.Rucesion   => BoardStore.Load("nation_board_rucesion"),
        //    Nation.Noes       => BoardStore.Load("nation_board_noes"),
        //    Nation.Illuminati => BoardStore.Load("nation_board_illuminati"),
        //    Nation.Piet       => BoardStore.Load("nation_board_piet"),
        //    Nation.Atlantis   => BoardStore.Load("nation_board_atlantis"),
        //    Nation.Abel       => BoardStore.Load("nation_board_abel"),
        //    Nation.Undine     => BoardStore.Load("nation_board_undine"),
        //    Nation.Purgatory  => BoardStore.Load("nation_board_purgatory"),
        //    _                 => throw new ArgumentOutOfRangeException()
        //};
        //
        //yield return nationBoard;
    }

    /// <inheritdoc />
    public override bool IsFriendlyTo(Creature creature) => RelationshipBehavior.IsFriendlyTo(Subject, creature);

    /// <inheritdoc />
    public override bool IsHostileTo(Creature creature) => RelationshipBehavior.IsHostileTo(Subject, creature);

    public override void OnAttacked(Creature source, int damage)
    {
        if (Subject.IsPramhed())
        {
            Subject.Effects.Dispel("pramh");
            Subject.Effects.Dispel("beagpramh");
        }

        if (Subject.Effects.Contains("wolfFangFist"))
            Subject.Effects.Dispel("wolfFangFist");

        if (Subject.Effects.Contains("Crit"))
            Subject.Effects.Dispel("Crit");

        if (Subject.IsLightningStanced())
        {
            var result = damage * 30;

            // Apply additional effect with a 2% chance
            if (IntegerRandomizer.RollChance(2))
            {
                if (!source.Script.Is<ThisIsABossScript>())
                {
                    var effect = EffectFactory.Create("BeagSuain");
                    source.Effects.Apply(Subject, effect);
                }
            }

            // Update aggro list for monsters
            if (source is Monster monster)
                monster.AggroList.AddOrUpdate(Subject.Id, _ => result, (_, currentAggro) => currentAggro + result);

            // Get all non-wall points on the map within 4 spaces of the subject
            var nonWallPoints = Enumerable.Range(0, Subject.MapInstance.Template.Width)
                .SelectMany(x => Enumerable.Range(0, Subject.MapInstance.Template.Height)
                    .Where(y => !Subject.MapInstance.IsWall(new Point(x, y)) &&
                                Math.Sqrt(Math.Pow(x - Subject.X, 2) + Math.Pow(y - Subject.Y, 2)) <= 4)
                    .Select(y => new Point(x, y))).ToList();

            if (nonWallPoints.Count <= 0)
                return;

            // Select a random non-wall point as the top-left corner of the 2x2 area
            var topLeftPoint = nonWallPoints[Random.Shared.Next(nonWallPoints.Count)];

            // Define the 2x2 area by generating points around the top-left corner
            var areaPoints = new List<Point>
            {
                topLeftPoint,
                new Point(topLeftPoint.X + 1, topLeftPoint.Y),
                new Point(topLeftPoint.X, topLeftPoint.Y + 1),
                new Point(topLeftPoint.X + 1, topLeftPoint.Y + 1)
            };

            // Show animation and apply damage to all entities within the 2x2 area
            foreach (var point in areaPoints)
            {
                // Ensure the point is within the map bounds
                if (point.X >= 0 && point.Y >= 0 && point.X < Subject.MapInstance.Template.Width &&
                    point.Y < Subject.MapInstance.Template.Height)
                {
                    Subject.MapInstance.ShowAnimation(LightningStanceStrike.GetPointAnimation(point));

                    // Check if an entity is standing on the point and apply damage
                    var target = Subject.MapInstance
                        .GetEntitiesAtPoint<Creature>(point).FirstOrDefault(x => x.IsAlive && x.IsHostileTo(Subject));

                    if (target != null)
                    {
                        var areaDamage = (int)(target.StatSheet.EffectiveMaximumHp * 0.05); // 5% of max HP
                        ApplyDamageScript.ApplyDamage(Subject, target, this, areaDamage);
                        target.ShowHealth();
                    }
                }
            }
        }


        if (Subject.IsThunderStanced())
        {
            var result = damage * 30;

            // Apply additional effect with a 2% chance
            if (IntegerRandomizer.RollChance(2))
            {
                if (!source.Script.Is<ThisIsABossScript>())
                {
                    var effect = EffectFactory.Create("Suain");
                    source.Effects.Apply(Subject, effect);
                }
            }

            // Update aggro list for monsters
            if (source is Monster monster)
                monster.AggroList.AddOrUpdate(Subject.Id, _ => result, (_, currentAggro) => currentAggro + result);
        }

        if (Subject.IsSmokeStanced() && IntegerRandomizer.RollChance(15))
        {
            if (!source.Script.Is<ThisIsABossScript>())
            {
                var effect = EffectFactory.Create("Blind");
                source.Effects.Apply(Subject, effect);
            }
        }
        
        if (Subject.IsFlameStanced() && IntegerRandomizer.RollChance(15))
        {
            if (!source.Script.Is<ThisIsABossScript>())
            {
                var effect = EffectFactory.Create("Blind");
                source.Effects.Apply(Subject, effect);
            }
            
            var points = AoeShape.AllAround.ResolvePoints(Subject);

            var targets =
                Subject.MapInstance.GetEntitiesAtPoints<Creature>(points.Cast<IPoint>()).WithFilter(Subject, TargetFilter.HostileOnly).ToList();

            var flamedamage = (int)(Subject.StatSheet.EffectiveMaximumHp * .04);
        
            foreach (var target in targets)
            {
                ApplyDamageScript.ApplyDamage(
                    Subject,
                    target,
                    this,
                    flamedamage);
                
                target.ShowHealth();
                target.Animate(FlameHit, Subject.Id);
            }
        }


        if (Subject.IsTideStanced())
        {
            var healAmount = Math.Round(damage * 0.15m);

            if (Subject.Group is not null)
            {
                foreach (var person in Subject.Group)
                {
                    if (person.IsDead)
                        continue;

                    person.Animate(TideHeal, person.Id);

                    ApplyHealScript.ApplyHeal(
                        source,
                        person,
                        this,
                        (int)healAmount);

                    var manaReplenished = Math.Round(damage * 0.08m);
                    person.StatSheet.AddMp((int)manaReplenished);
                }
            }
            else
            {
                if (Subject.IsDead)
                    return;

                Subject.Animate(TideHeal, Subject.Id);

                ApplyHealScript.ApplyHeal(
                    source,
                    Subject,
                    this,
                    (int)healAmount);

                var manaReplenished = Math.Round(damage * 0.08m);
                Subject.StatSheet.AddMp((int)manaReplenished);
            }
        }

        if (Subject.IsMistStanced())
        {
            var result = Math.Round(damage * 0.15m);

            if (Subject.Group is not null)
                foreach (var person in Subject.Group)
                {
                    if (person.IsDead)
                        continue;

                    person.Animate(MistHeal, person.Id);

                    ApplyHealScript.ApplyHeal(
                        source,
                        person,
                        this,
                        (int)result);

                }
            else
            {
                if (Subject.IsDead)
                    return;

                Subject.Animate(MistHeal, Subject.Id);

                ApplyHealScript.ApplyHeal(
                    source,
                    Subject,
                    this,
                    (int)result);

            }
        }

        if (Subject.IsInLastStand())
            if (damage >= Subject.StatSheet.CurrentHp)
            {
                Subject.StatSheet.SetHp(1);
                Subject.Client.SendAttributes(StatUpdateType.Vitality);
            }

        if (Subject.Effects.Contains("mount"))
        {
            Subject.Effects.Dispel("mount");
            Subject.Refresh();
        }

        if (Subject.IsRuminating())
        {
            Subject.Effects.Dispel("rumination");
            Subject.SendOrangeBarMessage("Taking damage ended your rumination.");
        }
    }

    /// <inheritdoc />
    public override void OnDeath()
    {
        var source = Subject.Trackers.LastDamagedBy;

        if (source?.MapInstance.Name.Equals("Cain's Farm") == true)
        {
            var mapInstance = SimpleCache.Get<MapInstance>("tutorial_hut");
            var pointS = new Point(2, 9);

            Subject.IsDead = false;
            Subject.StatSheet.AddHp(1);
            Subject.Client.SendAttributes(StatUpdateType.Vitality);
            Subject.SendOrangeBarMessage("You are knocked out. Be more careful.");
            Subject.TraverseMap(mapInstance, pointS);
            
            Subject.Refresh(true);
            return;
        }

        Subject.IsDead = true;

        //Refresh to show ghost
        Subject.Refresh(true);
        Subject.Display();

        //Remove all effects from the player
        var effects = Subject.Effects.ToList();

        foreach (var effect in effects)
            Subject.Effects.Dispel(effect.Name);

        if (source?.MapInstance.Name.Equals("Mr. Hopps's Home") == true)
        {
            var terminusSpawn = new Rectangle(source, 8, 8);
            var outline = terminusSpawn.GetOutline().ToList();
            var terminus = MerchantFactory.Create("terminus", source.MapInstance, Point.From(source));
            Point point;

            do
                point = outline.PickRandom();
            while (!source.MapInstance.IsWalkable(point, terminus.Type));

            source.MapInstance.AddEntity(terminus, point);
        }

        if (Subject.UserStatSheet.BaseClass is BaseClass.Priest)
        {
            var monsters = Subject.MapInstance.GetEntities<Monster>();

            foreach (var monster in monsters)
                if (monster.Name.Contains(Subject.Name))
                    monster.MapInstance.RemoveEntity(monster);
        }
        
        if (source?.MapInstance.Name.Equals("Nightmare") == true)
        {
            var mapInstance = SimpleCache.Get<MapInstance>("mileth_inn");
            var pointS = new Point(5, 7);
            var nightmaregearDictionary = new Dictionary<(BaseClass, Gender), string[]>
            {
                { (BaseClass.Warrior, Gender.Male), ["malecarnunplate", "carnunhelmet"] },
                { (BaseClass.Warrior, Gender.Female), ["femalecarnunplate", "carnunhelmet"] },
                { (BaseClass.Monk, Gender.Male), ["maleaosdicpatternwalker"] },
                { (BaseClass.Monk, Gender.Female), ["femaleaosdicpatternwalker"] },
                { (BaseClass.Rogue, Gender.Male), ["malemarauderhide", "maraudermask"] },
                { (BaseClass.Rogue, Gender.Female), ["femalemarauderhide", "maraudermask"] },
                { (BaseClass.Priest, Gender.Male), ["malecthonicdisciplerobes", "cthonicdisciplecaputium"] },
                { (BaseClass.Priest, Gender.Female), ["morrigudisciplepellison", "holyhairband"] },
                { (BaseClass.Wizard, Gender.Male), ["cthonicmagusrobes", "cthonicmaguscaputium"] },
                { (BaseClass.Wizard, Gender.Female), ["morrigumaguspellison", "magushairband"] }
            };
            
            Subject.TraverseMap(mapInstance, pointS);
            Subject.IsDead = false;
            Subject.StatSheet.AddHp(1);
            Subject.Trackers.Enums.Set(NightmareQuestStage.CompletedNightmareLoss1);
            Subject.Client.SendAttributes(StatUpdateType.Vitality);
            Subject.SendOrangeBarMessage("You have been defeated by your Nightmares.");
            Subject.Legend.AddOrAccumulate(
                new LegendMark(
                    "Succumbed to their Nightmares",
                    "Nightmare",
                    MarkIcon.Victory,
                    MarkColor.White,
                    1,
                    GameTime.Now));
            
            var gearKey = (Subject.UserStatSheet.BaseClass, Subject.Gender);

            if (nightmaregearDictionary.TryGetValue(gearKey, out var nightmaregear))
            {
                var hasGear = nightmaregear.All(
                    gearItemName =>
                        Subject.Inventory.ContainsByTemplateKey(gearItemName)
                        || Subject.Bank.Contains(gearItemName)
                        || Subject.Equipment.ContainsByTemplateKey(gearItemName));

                if (!hasGear)
                    foreach (var gearItemName in nightmaregear)
                    {
                        var gearItem = ItemFactory.Create(gearItemName);
                        Subject.GiveItemOrSendToBank(gearItem);
                    }
            }
            
            Subject.Refresh(true);

            return;
        }

        if (ArenaKeys.Contains(Subject.MapInstance.LoadedFromInstanceId))
        {
            var aislings = Subject.MapInstance.GetEntities<Aisling>();

            foreach (var aisling in aislings)
                aisling.SendServerMessage(
                    ServerMessageType.OrangeBar1,
                    $"{Subject.Name} was killed by {source?.Name}.");
        }

        if (MapsToNotPunishDeathOn.Contains(Subject.MapInstance.Name))
            return;

        foreach (var client in ClientRegistry)
            client.SendServerMessage(
                ServerMessageType.OrangeBar1,
                $"{Subject.Name} was killed at {Subject.MapInstance.Name} by {source?.Name ?? "The Guardians"}.");

        var itemsToBreak = Subject.Equipment.Where(
            x => !x.Template.AccountBound
                 && (x.Template.EquipmentType != EquipmentType.Accessory)
                 && (x.Template.EquipmentType != EquipmentType.OverArmor)
                 && (x.Template.EquipmentType != EquipmentType.OverHelmet));

        foreach (var item in itemsToBreak)
            if (IntegerRandomizer.RollChance(2))
            {
                Logger.WithTopics(
                          Topics.Entities.Aisling,
                          Topics.Entities.Item,
                          Topics.Actions.Death,
                          Topics.Actions.Penalty)
                      .WithProperty(Subject)
                      .WithProperty(item)
                      .LogInformation("{@AislingName} has lost {@ItemName} to death", Subject.Name, item.DisplayName);

                Subject.Client.SendServerMessage(
                    ServerMessageType.OrangeBar1,
                    $"{item.DisplayName} has been consumed by death.");

                Subject.Equipment.TryGetRemove(item.Slot, out _);
            }

        var tenPercent = MathEx.GetPercentOf<int>((int)Subject.UserStatSheet.TotalExp, 10);

        if (ExperienceDistributionScript.TryTakeExp(Subject, tenPercent))
        {
            Logger.WithTopics(
                      Topics.Entities.Aisling,
                      Topics.Actions.Death,
                      Topics.Actions.Penalty,
                      Topics.Entities.Experience)
                  .WithProperty(Subject)
                  .LogInformation("{@AislingName} has lost {@ExperienceAmount} experience to death", Subject.Name, tenPercent);

            Subject.Client.SendServerMessage(ServerMessageType.OrangeBar1, $"You have lost {tenPercent} experience.");
        }
        
        Subject.Trackers.Counters.AddOrIncrement("deathcounter", 1);
        Subject.Legend.AddOrAccumulate(new LegendMark(
            "Fell in battle",
            "deathkey",
            MarkIcon.Victory,
            MarkColor.White,
            1,
            GameTime.Now));
    }

    /// <inheritdoc />
    public override void OnStatIncrease(Stat stat)
    {
        if (stat == Stat.STR)
            Subject.UserStatSheet.SetMaxWeight(LevelUpFormulae.Default.CalculateMaxWeight(Subject));
    }

    private void HandleWerewolfEffect()
    {
        if (!Subject.Trackers.Enums.HasValue(WerewolfOfPiet.KilledandGotCursed)
            && !Subject.Trackers.Enums.HasValue(WerewolfOfPiet.SpokeToWizard)
            && !Subject.Trackers.Enums.HasValue(WerewolfOfPiet.KilledWerewolf)
            && !Subject.Trackers.Enums.HasValue(WerewolfOfPiet.CollectedBlueFlower))
            return;

        var lightlevel = Subject.MapInstance.CurrentLightLevel;
        
        if (lightlevel == LightLevel.Darkest_A && !Subject.Effects.Contains("werewolf") && Subject.MapInstance.AutoDayNightCycle.Equals(true))
        {
            if (Subject.Effects.Contains("mount"))
            {
                Subject.Effects.Dispel("mount");
                Subject.SendOrangeBarMessage("You jump off your mount due to becoming a Werewolf.");

                return;
            }
            
            var effect = EffectFactory.Create("werewolf");
            Subject.Effects.Apply(Subject, effect);
        } else if (lightlevel != LightLevel.Darkest_A && Subject.Effects.Contains("werewolf"))
            Subject.Effects.Terminate("werewolf");
    }

    private void NotifyPlayer(string keyToRemove, string keyToKeep)
    {
        Subject.SendOrangeBarMessage("Ability " + keyToKeep + " removed old ability " + keyToRemove + ".");
    }
    
    void RemoveAndNotifyIfBothExist(string keyToKeep, string keyToRemove)
    {
        if (Subject.SpellBook.ContainsByTemplateKey(keyToKeep) && Subject.SpellBook.ContainsByTemplateKey(keyToRemove))
        {
            Subject.SpellBook.RemoveByTemplateKey(keyToRemove);
            NotifyPlayer(keyToRemove, keyToKeep);
            Logger.WithTopics(Topics.Entities.Creature, Topics.Entities.Skill, Topics.Actions.Update)
                .WithProperty(Subject)
                .LogInformation(
                    "Aisling {@AislingName}'s ability {keyToKeep} removed an old ability {@keyToRemove}",
                    Subject.Name,
                    keyToKeep,
                    keyToRemove);
        }
        else if (Subject.SkillBook.ContainsByTemplateKey(keyToKeep) && Subject.SkillBook.ContainsByTemplateKey(keyToRemove))
        {
            Subject.SkillBook.RemoveByTemplateKey(keyToRemove);
            NotifyPlayer(keyToRemove, keyToKeep);
            Logger.WithTopics(Topics.Entities.Creature, Topics.Entities.Skill, Topics.Actions.Update)
                .WithProperty(Subject)
                .LogInformation(
                    "Aisling {@AislingName}'s ability {keyToKeep} removed an old ability {@keyToRemove}",
                    Subject.Name,
                    keyToKeep,
                    keyToRemove);
        }
    }
    
    public override void Update(TimeSpan delta)
    {
        SleepAnimationTimer.Update(delta);
        ClearOrangeBarTimer.Update(delta);
        OneSecondTimer.Update(delta);
        CleanupSkillsSpellsTimer.Update(delta);

        if (CleanupSkillsSpellsTimer.IntervalElapsed && !Subject.IsDiacht())
        {
            if (Subject.UserStatSheet.Level < 99)
            {
                // Calculate the target AC based on level
                var targetAc = 100 - (Subject.UserStatSheet.Level / 3);

                // Calculate the difference in AC (amount to subtract)
                var acDifference = Subject.UserStatSheet.Ac - targetAc;

                // Calculate the new attack speed percentage: 1% for every 3 Dex minus the initial 3 Dex
                var newAtkSpeedPct = Math.Max(0, (Subject.UserStatSheet.Dex - 3) / 3);

                // Calculate the attack speed difference (amount to subtract)
                var atkSpeedPctDifference = Subject.UserStatSheet.AtkSpeedPct - newAtkSpeedPct;

                // Create the attributes object for the new values
                var newAttributes = new Attributes()
                {
                    Ac = acDifference,  // Pass the AC difference to subtract the correct value
                    AtkSpeedPct = atkSpeedPctDifference  // Pass the attack speed percentage difference
                };

                // Apply the new stats
                Subject.UserStatSheet.Subtract(newAttributes);
            }
            else if (Subject.UserStatSheet.Level == 99)
            {
                // Set target AC for level 99
                var targetAc = 67;

                // Calculate the difference in AC (amount to subtract)
                var acDifference = Subject.UserStatSheet.Ac - targetAc;

                // Calculate the new attack speed percentage: 1% for every 3 Dex minus the initial 3 Dex
                var newAtkSpeedPct = Math.Max(0, (Subject.UserStatSheet.Dex - 3) / 3);

                // Calculate the attack speed difference (amount to subtract)
                var atkSpeedPctDifference = Subject.UserStatSheet.AtkSpeedPct - newAtkSpeedPct;

                // Create the attributes object for the new values
                var newAttributes = new Attributes()
                {
                    Ac = acDifference,  // Pass the AC difference to subtract the correct value
                    AtkSpeedPct = atkSpeedPctDifference  // Pass the attack speed percentage difference
                };

                // Apply the new stats
                Subject.UserStatSheet.Subtract(newAttributes);

                // Send full attribute update
                Subject.Client.SendAttributes(StatUpdateType.Full);
            }

            
            RemoveAndNotifyIfBothExist("athar", "beagathar");
            RemoveAndNotifyIfBothExist("morathar", "athar");
            RemoveAndNotifyIfBothExist("morathar", "beagathar");
            RemoveAndNotifyIfBothExist("ardathar", "morathar");
            RemoveAndNotifyIfBothExist("ardathar", "athar");
            RemoveAndNotifyIfBothExist("ardathar", "beagathar");
            RemoveAndNotifyIfBothExist("moratharmeall", "atharmeall");
            RemoveAndNotifyIfBothExist("ardatharmeall", "moratharmeall");
            RemoveAndNotifyIfBothExist("ardatharmeall", "atharmeall");
            RemoveAndNotifyIfBothExist("atharlamh", "beagatharlamh");
            RemoveAndNotifyIfBothExist("moratharlamh", "atharlamh");
            RemoveAndNotifyIfBothExist("moratharlamh", "beagatharlamh");
            RemoveAndNotifyIfBothExist("creag", "beagcreag");
            RemoveAndNotifyIfBothExist("morcreag", "creag");
            RemoveAndNotifyIfBothExist("morcreag", "beagcreag");
            RemoveAndNotifyIfBothExist("ardcreag", "morcreag");
            RemoveAndNotifyIfBothExist("ardcreag", "creag");
            RemoveAndNotifyIfBothExist("ardcreag", "beagcreag");
            RemoveAndNotifyIfBothExist("morcreagmeall", "creagmeall");
            RemoveAndNotifyIfBothExist("ardcreagmeall", "creagmeall");
            RemoveAndNotifyIfBothExist("ardcreagmeall", "morcreagmeall");
            RemoveAndNotifyIfBothExist("creaglamh", "beagcreaglamh");
            RemoveAndNotifyIfBothExist("morcreaglamh", "creaglamh");
            RemoveAndNotifyIfBothExist("morcreaglamh", "beagcreaglamh");
            RemoveAndNotifyIfBothExist("sal", "beagsal");
            RemoveAndNotifyIfBothExist("morsal", "sal");
            RemoveAndNotifyIfBothExist("morsal", "beagsal");
            RemoveAndNotifyIfBothExist("ardsal", "morsal");
            RemoveAndNotifyIfBothExist("ardsal", "sal");
            RemoveAndNotifyIfBothExist("ardsal", "beagsal");
            RemoveAndNotifyIfBothExist("morsalmeall", "salmeall");
            RemoveAndNotifyIfBothExist("ardsalmeall", "salmeall");
            RemoveAndNotifyIfBothExist("ardsalmeall", "morsalmeall");
            RemoveAndNotifyIfBothExist("sallamh", "beagsallamh");
            RemoveAndNotifyIfBothExist("morsallamh", "sallamh");
            RemoveAndNotifyIfBothExist("morsallamh", "beagsallamh");
            RemoveAndNotifyIfBothExist("srad", "beagsrad");
            RemoveAndNotifyIfBothExist("morsrad", "srad");
            RemoveAndNotifyIfBothExist("morsrad", "beagsrad");
            RemoveAndNotifyIfBothExist("ardsrad", "morsrad");
            RemoveAndNotifyIfBothExist("ardsrad", "srad");
            RemoveAndNotifyIfBothExist("ardsrad", "beagsrad");
            RemoveAndNotifyIfBothExist("morsradmeall", "sradmeall");
            RemoveAndNotifyIfBothExist("ardsradmeall", "sradmeall");
            RemoveAndNotifyIfBothExist("ardsradmeall", "morsradmeall");
            RemoveAndNotifyIfBothExist("sradlamh", "beagsradlamh");
            RemoveAndNotifyIfBothExist("morsradlamh", "sradlamh");
            RemoveAndNotifyIfBothExist("morsradlamh", "beagsradlamh");
            RemoveAndNotifyIfBothExist("arcanemissile", "arcanebolt");
            RemoveAndNotifyIfBothExist("arcaneblast", "arcanemissile");
            RemoveAndNotifyIfBothExist("arcaneblast", "arcanebolt");
            RemoveAndNotifyIfBothExist("arcaneexplosion", "arcaneblast");
            RemoveAndNotifyIfBothExist("arcaneexplosion", "arcanemissile");
            RemoveAndNotifyIfBothExist("arcaneexplosion", "arcanebolt");
            RemoveAndNotifyIfBothExist("stilettotrap", "needletrap");
            RemoveAndNotifyIfBothExist("bolttrap", "needletrap");
            RemoveAndNotifyIfBothExist("bolttrap", "stilettotrap");
            RemoveAndNotifyIfBothExist("coiledbolttrap", "needletrap");
            RemoveAndNotifyIfBothExist("coiledbolttrap", "stilettotrap");
            RemoveAndNotifyIfBothExist("coiledbolttrap", "bolttrap");
            RemoveAndNotifyIfBothExist("springtrap", "needletrap");
            RemoveAndNotifyIfBothExist("springtrap", "stilettotrap");
            RemoveAndNotifyIfBothExist("springtrap", "bolttrap");
            RemoveAndNotifyIfBothExist("springtrap", "coiledbolttrap");
            RemoveAndNotifyIfBothExist("maidentrap", "needletrap");
            RemoveAndNotifyIfBothExist("maidentrap", "stilettotrap");
            RemoveAndNotifyIfBothExist("maidentrap", "bolttrap");
            RemoveAndNotifyIfBothExist("maidentrap", "coiledbolttrap");
            RemoveAndNotifyIfBothExist("maidentrap", "springtrap");
            RemoveAndNotifyIfBothExist("pitfalltrap", "needletrap");
            RemoveAndNotifyIfBothExist("pitfalltrap", "stilettotrap");
            RemoveAndNotifyIfBothExist("pitfalltrap", "bolttrap");
            RemoveAndNotifyIfBothExist("pitfalltrap", "coiledbolttrap");
            RemoveAndNotifyIfBothExist("pitfalltrap", "springtrap");
            RemoveAndNotifyIfBothExist("pitfalltrap", "maidentrap");
            RemoveAndNotifyIfBothExist("pramh", "beagpramh");
            RemoveAndNotifyIfBothExist("revive", "beothaich");
            RemoveAndNotifyIfBothExist("resurrection", "beothaich");
            RemoveAndNotifyIfBothExist("resurrection", "revive");
            RemoveAndNotifyIfBothExist("warcry", "battlecry");
            RemoveAndNotifyIfBothExist("howl", "goad");
            RemoveAndNotifyIfBothExist("roar", "goad");
            RemoveAndNotifyIfBothExist("roar", "howl");
            RemoveAndNotifyIfBothExist("vortex", "quake");

            // SkillBook removals with the new methods
            RemoveAndNotifyIfBothExist("cleave", "scathe");
            RemoveAndNotifyIfBothExist("clobber", "strike");
            RemoveAndNotifyIfBothExist("wallop", "strike");
            RemoveAndNotifyIfBothExist("wallop", "clobber");
            RemoveAndNotifyIfBothExist("pulverize", "strike");
            RemoveAndNotifyIfBothExist("pulverize", "clobber");
            RemoveAndNotifyIfBothExist("pulverize", "wallop");
            RemoveAndNotifyIfBothExist("thrash", "strike");
            RemoveAndNotifyIfBothExist("thrash", "clobber");
            RemoveAndNotifyIfBothExist("thrash", "wallop");
            RemoveAndNotifyIfBothExist("thrash", "pulverize");
            RemoveAndNotifyIfBothExist("sunder", "slash");
            RemoveAndNotifyIfBothExist("tempestblade", "windblade");
            RemoveAndNotifyIfBothExist("paralyzeforce", "groundstomp");
            RemoveAndNotifyIfBothExist("madsoul", "flurry");
            RemoveAndNotifyIfBothExist("charge", "bullrush");
            RemoveAndNotifyIfBothExist("doublepunch", "punch");
            RemoveAndNotifyIfBothExist("rapidpunch", "punch");
            RemoveAndNotifyIfBothExist("rapidpunch", "doublepunch");
            RemoveAndNotifyIfBothExist("triplekick", "doublepunch");
            RemoveAndNotifyIfBothExist("triplekick", "punch");
            RemoveAndNotifyIfBothExist("triplekick", "rapidpunch");
            RemoveAndNotifyIfBothExist("dragonstrike", "eaglestrike");
            RemoveAndNotifyIfBothExist("dragonstrike", "phoenixstrike");
            RemoveAndNotifyIfBothExist("phoenixstrike", "eaglestrike");
            RemoveAndNotifyIfBothExist("roundhousekick", "kick");
            RemoveAndNotifyIfBothExist("mantiskick", "highkick");
            RemoveAndNotifyIfBothExist("blitz", "assault");
            RemoveAndNotifyIfBothExist("barrage", "assault");
            RemoveAndNotifyIfBothExist("barrage", "blitz");
            RemoveAndNotifyIfBothExist("gut", "stab");
            RemoveAndNotifyIfBothExist("skewer", "pierce");
            RemoveAndNotifyIfBothExist("midnightslash", "barrage");
            RemoveAndNotifyIfBothExist("midnightslash", "blitz");
            RemoveAndNotifyIfBothExist("midnightslash", "assault");
            RemoveAndNotifyIfBothExist("surigumblitz", "throwsurigum");
        }

        if (OneSecondTimer.IntervalElapsed)
            HandleWerewolfEffect();
        
        if (SleepAnimationTimer.IntervalElapsed)
        {
            var lastManualAction = Subject.Trackers.LastManualAction;

            var isAfk = !lastManualAction.HasValue
                        || (DateTime.UtcNow.Subtract(lastManualAction.Value)
                                    .TotalMinutes
                            > WorldOptions.Instance.SleepAnimationTimerMins);
            
            if (isAfk)
            {
                if (Subject.IsAlive)
                    Subject.AnimateBody(BodyAnimation.Snore);

                if (Subject.UserStatSheet.BaseClass is BaseClass.Priest)
                {
                    var pets = Subject.MapInstance.GetEntities<Monster>().Where(x => x.Script.Is<PetScript>() && x.Name.Contains(Subject.Name));
                    foreach (var pet in pets)
                        pet.MapInstance.RemoveEntity(pet);
                }

                if (Subject.Effects.Contains("mount"))
                {
                    Subject.Effects.Dispel("mount");
                }
                
                //set player to daydreaming if they are currently set to awake
                if (Subject.Options.SocialStatus != SocialStatus.DayDreaming)
                {
                    PreAfkSocialStatus = Subject.Options.SocialStatus;
                    Subject.Options.SocialStatus = SocialStatus.DayDreaming;
                }
            }
            else if (Subject.Options.SocialStatus == SocialStatus.DayDreaming)
                Subject.Options.SocialStatus = PreAfkSocialStatus;
        }

        if (ClearOrangeBarTimer.IntervalElapsed)
        {
            var lastOrangeBarMessage = Subject.Trackers.LastOrangeBarMessage;
            var now = DateTime.UtcNow;

            //clear if
            //an orange bar message has ever been sent
            //and the last message was sent after the last clear
            //and the time since the last message is greater than the clear timer
            var shouldClear = lastOrangeBarMessage.HasValue
                              && (lastOrangeBarMessage > (Subject.Trackers.LastOrangeBarMessageClear ?? DateTime.MinValue))
                              && (now.Subtract(lastOrangeBarMessage.Value)
                                     .TotalSeconds
                                  > WorldOptions.Instance.ClearOrangeBarTimerSecs);

            if (shouldClear)
            {
                Subject.SendServerMessage(ServerMessageType.OrangeBar1, string.Empty);
                Subject.Trackers.LastOrangeBarMessage = lastOrangeBarMessage;
                Subject.Trackers.LastOrangeBarMessageClear = now;
            }
        }
    }
}
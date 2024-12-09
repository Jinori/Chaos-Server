using Chaos.Collections;
using Chaos.Collections.Abstractions;
using Chaos.Common.Definitions;
using Chaos.Common.Utilities;
using Chaos.DarkAges.Definitions;
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
using Chaos.Scripting.Components.AbilityComponents;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ApplyDamage;
using Chaos.Scripting.FunctionalScripts.ApplyHealing;
using Chaos.Scripting.FunctionalScripts.ExperienceDistribution;
using Chaos.Scripting.MonsterScripts.Boss;
using Chaos.Scripting.MonsterScripts.Pet;
using Chaos.Scripting.ReactorTileScripts.Jobs;
using Chaos.Services.Factories.Abstractions;
using Chaos.Services.Servers.Options;
using Chaos.Storage.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.AislingScripts;

public class DefaultAislingScript : AislingScriptBase, HealAbilityComponent.IHealComponentOptions
{
    private readonly HashSet<string> ArenaKeys = new(StringComparer.OrdinalIgnoreCase)
    {
        "arena_battle_ring",
        "arena_lava",
        "arena_lavateams",
        "arena_colorclash",
        "arena_escort",
        "arena_hidden_havoc"
    };

    private readonly IStore<BulletinBoard> BoardStore;
    private readonly IIntervalTimer CleanupSkillsSpellsTimer;
    private readonly IIntervalTimer ClearOrangeBarTimer;
    private readonly IClientRegistry<IChaosWorldClient> ClientRegistry;
    private readonly IEffectFactory EffectFactory;
    private readonly IItemFactory ItemFactory;
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
        "Nobis",
        "Macabre Yard",
        "Macabre Mansion",
        "Weapon Shop",
        "Armor Shop",
        "Rucesion Bank",
        "Mileth Storage",
        "Black Market",
        "Rucesion Jeweler",
        "Rucesion Church",
        "Rucesion Casino",
        "Rucesion Tailor",
        "Skills Master",
        "Rucesion Inn",
        "Shrine of Skandara",
        "Shrine of Miraelis",
        "Shrine of Theselene",
        "Shrine of Serendael",
        "Piet Empty Room",
        "Piet Inn",
        "Mileth Inn",
        "Abel Inn",
        "Suomi Inn",
        "Piet Storage",
        "Mileth Storage",
        "Piet Alchemy Lab",
        "Piet Tavern",
        "Mileth Tavern",
        "Undine Tavern",
        "Mileth Kitchen",
        "Piet Restaurant",
        "Piet Priestess",
        "Piet Magic Shop",
        "Piet Sewer Entrance",
        "Tagor Forge",
        "Tagor Inn",
        "Tagor Messenger",
        "Tagor Pet Store",
        "Tagor Storage",
        "Tagor Restaurant",
        "Tagor Tavern",
        "Tagor Church",
        "Abel Combat Skill Master",
        "Abel Fish Market",
        "Abel Tavern",
        "Abel Restaurant",
        "Abel Empty Room",
        "Abel Storage",
        "Abel Goods Shop",
        "Abel Magic Shop",
        "Abel Weapon Shop",
        "Abel Armor Shop",
        "Special Skills Masters",
        "Mileth Armor Shop",
        "Mileth Weapon Shop",
        "Special Spells Master",
        "Mileth Church",
        "Kitchen",
        "Restaurant",
        "Tavern",
        "Mileth Beauty Shop",
        "Temple of Choosing",
        "Undine Armor Shop",
        "Undine Weapon Shop",
        "Enchanted Haven",
        "Undine Goods Shop",
        "Undine Black Magic Master",
        "Undine Storage",
        "Undine Restaurant",
        "Undine Tavern",
        "Suomi Special Skill Master",
        "Garamonde Theatre",
        "Suomi Armor Shop",
        "Suomi Weapon Shop",
        "Suomi Cherry Farmer",
        "Suomi White Magic Master",
        "Suomi Combat Skill Master",
        "Suomi Black Magic Master",
        "Suomi Grape Farmer",
        "Suomi Restaurant",
        "Suomi Tavern",
        "Nobis Restaurant",
        "Nobis Tavern",
        "Nobis House",
        "Nobis Storage",
        "Nobis Weapon Shop",
        "Mileth Tailor",
        "Thanksgiving Challenge"
    ];

    private readonly IMerchantFactory MerchantFactory;
    private readonly IIntervalTimer OneSecondTimer;
    private readonly ISimpleCache SimpleCache;
    private readonly ISkillFactory SkillFactory;
    private readonly IIntervalTimer SleepAnimationTimer;
    private readonly ISpellFactory SpellFactory;
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

    private Animation FlameHit { get; } = new()
    {
        AnimationSpeed = 100,
        TargetAnimation = 870
    };

    private Animation LightningStanceStrike { get; } = new()
    {
        AnimationSpeed = 100,
        TargetAnimation = 698
    };

    private Animation MistHeal { get; } = new()
    {
        AnimationSpeed = 100,
        TargetAnimation = 646
    };

    protected virtual RelationshipBehavior RelationshipBehavior { get; }
    protected virtual RestrictionBehavior RestrictionBehavior { get; }

    private Animation TideHeal { get; } = new()
    {
        AnimationSpeed = 100,
        TargetAnimation = 849
    };

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
        IItemFactory itemFactory,
        ISpellFactory spellFactory,
        ISkillFactory skillFactory)
        : base(subject)
    {
        MailStore = mailStore;
        BoardStore = boardStore;
        Logger = logger;
        ItemFactory = itemFactory;
        SpellFactory = spellFactory;
        SkillFactory = skillFactory;
        OneSecondTimer = new IntervalTimer(TimeSpan.FromSeconds(1));
        RestrictionBehavior = new RestrictionBehavior();
        VisibilityBehavior = new VisibilityBehavior();
        RelationshipBehavior = new RelationshipBehavior();
        ExperienceDistributionScript = DefaultExperienceDistributionScript.Create();
        SleepAnimationTimer = new IntervalTimer(TimeSpan.FromSeconds(5), false);
        ClearOrangeBarTimer = new IntervalTimer(TimeSpan.FromSeconds(WorldOptions.Instance.ClearOrangeBarTimerSecs), false);

        CleanupSkillsSpellsTimer = new RandomizedIntervalTimer(
            TimeSpan.FromMinutes(3),
            25,
            RandomizationType.Balanced,
            false);
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

    private void HandleWerewolfEffect()
    {
        if (!Subject.Trackers.Enums.HasValue(WerewolfOfPiet.KilledandGotCursed)
            && !Subject.Trackers.Enums.HasValue(WerewolfOfPiet.SpokeToWizard)
            && !Subject.Trackers.Enums.HasValue(WerewolfOfPiet.KilledWerewolf)
            && !Subject.Trackers.Enums.HasValue(WerewolfOfPiet.CollectedBlueFlower))
            return;

        var lightlevel = Subject.MapInstance.CurrentLightLevel;

        if ((lightlevel == LightLevel.Darkest_A)
            && !Subject.Effects.Contains("Werewolf")
            && Subject.MapInstance.AutoDayNightCycle.Equals(true))
        {
            if (Subject.Effects.Contains("Mount"))
            {
                Subject.Effects.Dispel("Mount");
                Subject.SendOrangeBarMessage("You jump off your mount due to becoming a Werewolf.");

                return;
            }

            var effect = EffectFactory.Create("Werewolf");
            Subject.Effects.Apply(Subject, effect);
        } else if ((lightlevel != LightLevel.Darkest_A) && Subject.Effects.Contains("Werewolf"))
            Subject.Effects.Terminate("Werewolf");
    }

    /// <inheritdoc />
    public override bool IsFriendlyTo(Creature creature) => RelationshipBehavior.IsFriendlyTo(Subject, creature);

    /// <inheritdoc />
    public override bool IsHostileTo(Creature creature) => RelationshipBehavior.IsHostileTo(Subject, creature);

    private void NotifyPlayerSpells(string keyToRemove, string keyToKeep)
    {
        var nameToKeep = SpellFactory.Create(keyToKeep);
        var nameToRemove = SpellFactory.Create(keyToRemove);
        Subject.SendOrangeBarMessage("Ability " + nameToKeep.Template.Name + " removed old ability " + nameToRemove.Template.Name + ".");
    }
    
    private void NotifyPlayerSkills(string keyToRemove, string keyToKeep)
    {
        var nameToKeep = SkillFactory.Create(keyToKeep);
        var nameToRemove = SkillFactory.Create(keyToRemove);
        Subject.SendOrangeBarMessage("Ability " + nameToKeep.Template.Name + " removed old ability " + nameToRemove.Template.Name + ".");
    }
    

    public override void OnAttacked(Creature source, int damage)
    {
        if (Subject.IsPramhed())
        {
            Subject.Effects.Dispel("Pramh");
            Subject.Effects.Dispel("Beag Pramh");
        }

        if (Subject.Effects.Contains("Wolf Fang Fist"))
            Subject.Effects.Dispel("Wolf Fang Fist");

        if (Subject.Effects.Contains("Crit"))
            Subject.Effects.Dispel("Crit");

        if (Subject.IsLightningStanced())
        {
            var result = damage * 30;

            // Apply additional effect with a 2% chance
            if (IntegerRandomizer.RollChance(2))
                if (!source.Script.Is<ThisIsABossScript>())
                {
                    var effect = EffectFactory.Create("Beag Suain");
                    source.Effects.Apply(Subject, effect);
                }

            // Update aggro list for monsters
            if (source is Monster monster)
                monster.AggroList.AddOrUpdate(Subject.Id, _ => result, (_, currentAggro) => currentAggro + result);

            // Get all non-wall points on the map within 4 spaces of the subject
            var nonWallPoints = Enumerable.Range(0, Subject.MapInstance.Template.Width)
                                          .SelectMany(
                                              x => Enumerable.Range(0, Subject.MapInstance.Template.Height)
                                                             .Where(
                                                                 y => !Subject.MapInstance.IsWall(new Point(x, y))
                                                                      && (Math.Sqrt(Math.Pow(x - Subject.X, 2) + Math.Pow(y - Subject.Y, 2))
                                                                          <= 4))
                                                             .Select(y => new Point(x, y)))
                                          .ToList();

            if (nonWallPoints.Count <= 0)
                return;

            // Select a random non-wall point as the top-left corner of the 2x2 area
            var topLeftPoint = nonWallPoints[Random.Shared.Next(nonWallPoints.Count)];

            // Define the 2x2 area by generating points around the top-left corner
            var areaPoints = new List<Point>
            {
                topLeftPoint,
                new(topLeftPoint.X + 1, topLeftPoint.Y),
                new(topLeftPoint.X, topLeftPoint.Y + 1),
                new(topLeftPoint.X + 1, topLeftPoint.Y + 1)
            };

            // Show animation and apply damage to all entities within the 2x2 area
            foreach (var point in areaPoints)

                // Ensure the point is within the map bounds
                if ((point.X >= 0)
                    && (point.Y >= 0)
                    && (point.X < Subject.MapInstance.Template.Width)
                    && (point.Y < Subject.MapInstance.Template.Height))
                {
                    Subject.MapInstance.ShowAnimation(LightningStanceStrike.GetPointAnimation(point));

                    // Check if an entity is standing on the point and apply damage
                    var target = Subject.MapInstance
                        .GetEntitiesAtPoints<Creature>(point)
                                        .FirstOrDefault(x => x.IsAlive && x.IsHostileTo(Subject));

                    if (target != null)
                    {
                        var areaDamage = (int)(target.StatSheet.EffectiveMaximumHp * 0.05); // 5% of max HP

                        ApplyDamageScript.ApplyDamage(
                            Subject,
                            target,
                            this,
                            areaDamage);
                        target.ShowHealth();
                    }
                }
        }

        if (Subject.IsThunderStanced())
        {
            var result = damage * 30;

            // Apply additional effect with a 2% chance
            if (IntegerRandomizer.RollChance(2))
                if (!source.Script.Is<ThisIsABossScript>())
                {
                    var effect = EffectFactory.Create("Suain");
                    source.Effects.Apply(Subject, effect);
                }

            // Update aggro list for monsters
            if (source is Monster monster)
                monster.AggroList.AddOrUpdate(Subject.Id, _ => result, (_, currentAggro) => currentAggro + result);
        }

        if (Subject.IsSmokeStanced() && IntegerRandomizer.RollChance(15))
            if (!source.Script.Is<ThisIsABossScript>())
            {
                var effect = EffectFactory.Create("Blind");
                source.Effects.Apply(Subject, effect);
            }

        if (Subject.IsFlameStanced() && IntegerRandomizer.RollChance(15))
        {
            if (!source.Script.Is<ThisIsABossScript>())
            {
                var effect = EffectFactory.Create("Blind");
                source.Effects.Apply(Subject, effect);
            }

            var options = new AoeShapeOptions
            {
                Source = new Point(Subject.X, Subject.Y),
                Range = 1
            };

            var points = AoeShape.AllAround.ResolvePoints(options);

            var targets = Subject.MapInstance
                                 .GetEntitiesAtPoints<Creature>(points.Cast<IPoint>())
                                 .WithFilter(Subject, TargetFilter.HostileOnly)
                                 .ToList();

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

                    if (person.MapInstance.InstanceId != Subject.MapInstance.InstanceId)
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

        if (Subject.Effects.Contains("Mount"))
        {
            Subject.Effects.Dispel("Mount");
            Subject.Refresh();
        }

        if (Subject.IsRuminating())
        {
            Subject.Effects.Dispel("Rumination");
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

        if (source?.MapInstance.Name.Equals("Macabre Mansion") == true)
        {
            var mapInstance = SimpleCache.Get<MapInstance>("hm_road");
            var pointS = new Point(10, 11);

            Subject.IsDead = false;
            Subject.StatSheet.AddHp(1);
            Subject.Client.SendAttributes(StatUpdateType.Vitality);
            Subject.SendOrangeBarMessage("Count removes you from his mansion. Be more careful.");
            Subject.TraverseMap(mapInstance, pointS);

            foreach (var effect in Subject.Effects)
                effect.Subject.Effects.Terminate(effect.Name);

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

            var outline = terminusSpawn.GetOutline()
                                       .ToList();
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
                {
                    (BaseClass.Warrior, Gender.Male), [
                                                          "malecarnunplate",
                                                          "carnunhelmet"
                                                      ]
                },
                {
                    (BaseClass.Warrior, Gender.Female), [
                                                            "femalecarnunplate",
                                                            "carnunhelmet"
                                                        ]
                },
                {
                    (BaseClass.Monk, Gender.Male), ["maleaosdicpatternwalker"]
                },
                {
                    (BaseClass.Monk, Gender.Female), ["femaleaosdicpatternwalker"]
                },
                {
                    (BaseClass.Rogue, Gender.Male), [
                                                        "malemarauderhide",
                                                        "maraudermask"
                                                    ]
                },
                {
                    (BaseClass.Rogue, Gender.Female), [
                                                          "femalemarauderhide",
                                                          "maraudermask"
                                                      ]
                },
                {
                    (BaseClass.Priest, Gender.Male), [
                                                         "malecthonicdisciplerobes",
                                                         "cthonicdisciplecaputium"
                                                     ]
                },
                {
                    (BaseClass.Priest, Gender.Female), [
                                                           "morrigudisciplepellison",
                                                           "holyhairband"
                                                       ]
                },
                {
                    (BaseClass.Wizard, Gender.Male), [
                                                         "cthonicmagusrobes",
                                                         "cthonicmaguscaputium"
                                                     ]
                },
                {
                    (BaseClass.Wizard, Gender.Female), [
                                                           "morrigumaguspellison",
                                                           "magushairband"
                                                       ]
                }
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
                    gearItemName => Subject.Inventory.ContainsByTemplateKey(gearItemName)
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
                aisling.SendServerMessage(ServerMessageType.OrangeBar1, $"{Subject.Name} was killed by {source?.Name}.");
        }

        if (MapsToNotPunishDeathOn.Contains(Subject.MapInstance.Name))
            return;

        foreach (var client in ClientRegistry)
            client.SendServerMessage(
                ServerMessageType.OrangeBar1,
                $"{Subject.Name} was killed at {Subject.MapInstance.Name} by {source?.Name ?? "The Guardians"}.");

        var itemsToBreak = Subject.Equipment
                                  .Where(
                                      x => !x.Template.AccountBound
                                           && (x.Template.EquipmentType != EquipmentType.Accessory)
                                           && (x.Template.EquipmentType != EquipmentType.OverArmor)
                                           && (x.Template.EquipmentType != EquipmentType.OverHelmet))
                                  .ToList();

        var mithrilDiceCount = Subject.Inventory.Count(item => item.Template.TemplateKey == "mithrildice");

        foreach (var item in itemsToBreak)

            // Random chance for item to break (50% chance in this case)
            if (IntegerRandomizer.RollChance(2))
            {
                if (mithrilDiceCount > 0)
                {
                    // Notify player that Mithril Dice saved their item
                    Logger.WithTopics(
                              [
                                  Topics.Entities.Aisling,
                                  Topics.Entities.Item,
                                  Topics.Actions.Death,
                                  Topics.Actions.Penalty
                              ])
                          .WithProperty(Subject)
                          .WithProperty(item)
                          .LogInformation("{@AislingName}'s {@ItemName} was saved by Mithril Dice", Subject.Name, item.DisplayName);

                    Subject.Client.SendServerMessage(
                        ServerMessageType.GroupChat,
                        $"Dice has saved your {item.DisplayName} from being consumed.");

                    // Consume one Mithril Dice (remove one from inventory)
                    Subject.Inventory.RemoveQuantityByTemplateKey("mithrildice", 1);

                    // Decrease the available Mithril Dice count
                    mithrilDiceCount--;
                } else
                {
                    // Log and notify the player that they lost an item
                    Logger.WithTopics(
                              [
                                  Topics.Entities.Aisling,
                                  Topics.Entities.Item,
                                  Topics.Actions.Death,
                                  Topics.Actions.Penalty
                              ])
                          .WithProperty(Subject)
                          .WithProperty(item)
                          .LogInformation("{@AislingName} has lost {@ItemName} to death", Subject.Name, item.DisplayName);

                    Subject.Client.SendServerMessage(ServerMessageType.GroupChat, $"{item.DisplayName} has been consumed by death.");

                    // Remove the item from the player's equipment
                    Subject.Equipment.TryGetRemove(item.Slot, out _);
                }
            }

        var tenPercent = MathEx.GetPercentOf<int>((int)Subject.UserStatSheet.TotalExp, 10);

        if (ExperienceDistributionScript.TryTakeExp(Subject, tenPercent))
        {
            Logger.WithTopics(
                      [
                          Topics.Entities.Aisling,
                          Topics.Actions.Death,
                          Topics.Actions.Penalty,
                          Topics.Entities.Experience
                      ])
                  .WithProperty(Subject)
                  .LogInformation("{@AislingName} has lost {@ExperienceAmount} experience to death", Subject.Name, tenPercent);

            Subject.Client.SendServerMessage(ServerMessageType.OrangeBar1, $"You have lost {tenPercent} experience.");
        }

        Subject.Trackers.Counters.AddOrIncrement("deathcounter", 1);

        Subject.Legend.AddOrAccumulate(
            new LegendMark(
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
        switch (stat)
        {
            case Stat.STR:
                Subject.UserStatSheet.Add(
                    new Attributes
                    {
                        MaximumHp = 25
                    });
                Subject.UserStatSheet.SetMaxWeight(LevelUpFormulae.Default.CalculateMaxWeight(Subject));

                Subject.SendServerMessage(
                    ServerMessageType.ActiveMessage,
                    "STR increased by one and maximum health increased by twenty five.");

                break;
            case Stat.DEX:
                Subject.UserStatSheet.Add(
                    new Attributes
                    {
                        AtkSpeedPct = (Subject.StatSheet.Dex % 3) == 0 ? 1 : 0
                    });
                Subject.SendServerMessage(ServerMessageType.ActiveMessage, "DEX increased by one and Attack Speed increased.");

                break;
            case Stat.INT:
                Subject.UserStatSheet.Add(
                    new Attributes
                    {
                        MaximumMp = 20
                    });
                Subject.SendServerMessage(ServerMessageType.ActiveMessage, "INT increased by one and maximum mana increased by twenty.");

                break;
            case Stat.WIS:
                Subject.UserStatSheet.Add(
                    new Attributes
                    {
                        MaximumMp = 40
                    });
                Subject.SendServerMessage(ServerMessageType.ActiveMessage, "WIS increased by one and maximum mana increased by fourty.");

                break;
            case Stat.CON:
                Subject.UserStatSheet.Add(
                    new Attributes
                    {
                        MaximumHp = 50
                    });
                Subject.SendServerMessage(ServerMessageType.ActiveMessage, "CON increased by one and maximum health increased by fifty.");

                break;
        }
    }

    private void RemoveAndNotifyIfBothExist(string keyToKeep, string keyToRemove)
    {
        if (Subject.IsGodModeEnabled())
            return;
        
        if (Subject.SpellBook.ContainsByTemplateKey(keyToKeep) && Subject.SpellBook.ContainsByTemplateKey(keyToRemove))
        {
            Subject.SpellBook.RemoveByTemplateKey(keyToRemove);
            NotifyPlayerSpells(keyToRemove, keyToKeep);

            Logger.WithTopics(
                      [
                          Topics.Entities.Creature,
                          Topics.Entities.Skill,
                          Topics.Actions.Update
                      ])
                  .WithProperty(Subject)
                  .LogInformation(
                      "Aisling {@AislingName}'s ability {keyToKeep} removed an old ability {@keyToRemove}",
                      Subject.Name,
                      keyToKeep,
                      keyToRemove);
        } else if (Subject.SkillBook.ContainsByTemplateKey(keyToKeep) && Subject.SkillBook.ContainsByTemplateKey(keyToRemove))
        {
            Subject.SkillBook.RemoveByTemplateKey(keyToRemove);
            NotifyPlayerSkills(keyToRemove, keyToKeep);

            Logger.WithTopics(
                      [
                          Topics.Entities.Creature,
                          Topics.Entities.Skill,
                          Topics.Actions.Update
                      ])
                  .WithProperty(Subject)
                  .LogInformation(
                      "Aisling {@AislingName}'s ability {keyToKeep} removed an old ability {@keyToRemove}",
                      Subject.Name,
                      keyToKeep,
                      keyToRemove);
        }
    }

    private static void RemoveOldMonkFormSkillsSpells(Aisling aisling)
    {
        if (aisling.UserStatSheet.BaseClass is not BaseClass.Monk)
            return;

        if (!aisling.Trackers.Enums.TryGetValue(out MonkElementForm currentForm))
            return;

        var elementSkillsAndSpells = new Dictionary<MonkElementForm, (List<string> Skills, List<string> Spells)>
        {
            {
                MonkElementForm.Water, ([
                                            "waterpunch",
                                            "tsunamikick",
                                            "hydrosiphon"
                                        ], [
                                               "miststance",
                                               "tidestance"
                                           ])
            },
            {
                MonkElementForm.Earth, ([
                                            "earthpunch",
                                            "seismickick",
                                            "seismicslam"
                                        ], [
                                               "earthenstance",
                                               "rockstance"
                                           ])
            },
            {
                MonkElementForm.Air, ([
                                          "airpunch",
                                          "tempestkick",
                                          "cyclonetwist"
                                      ], [
                                             "thunderstance",
                                             "lightningstance"
                                         ])
            },
            {
                MonkElementForm.Fire, ([
                                           "firepunch",
                                           "dracotailkick",
                                           "moltenstrike"
                                       ], [
                                              "smokestance",
                                              "flamestance"
                                          ])
            }
        };

        foreach (var element in elementSkillsAndSpells.Where(element => element.Key != currentForm))
            RemoveSkillsAndSpells(aisling, element.Value.Skills, element.Value.Spells);
    }

    private void RemovePureOnlySkills(string keyToRemove)
    {
        if (Subject.IsGodModeEnabled())
            return;
        
        if (Subject.SkillBook.ContainsByTemplateKey(keyToRemove) && Subject.Legend.ContainsKey("dedicated"))
        {
            Subject.SkillBook.RemoveByTemplateKey(keyToRemove);
            var skill = SkillFactory.Create(keyToRemove);
            Subject.SendOrangeBarMessage($"{skill.Template.Name} has been removed.");

            Logger.WithTopics(
                      [
                          Topics.Entities.Creature,
                          Topics.Entities.Skill,
                          Topics.Actions.Update
                      ])
                  .WithProperty(Subject)
                  .LogInformation("Aisling {@AislingName}'s ability {keyToKeep} removed.", Subject.Name, keyToRemove);
        }
    }

    private void RemovePureOnlySpells(string keyToRemove)
    {
        if (Subject.IsGodModeEnabled())
            return;
        
        if (Subject.SpellBook.ContainsByTemplateKey(keyToRemove) && Subject.Legend.ContainsKey("dedicated"))
        {
            Subject.SpellBook.RemoveByTemplateKey(keyToRemove);
            var spell = SpellFactory.Create(keyToRemove);
            Subject.SendOrangeBarMessage($"{spell.Template.Name} has been removed.");

            Logger.WithTopics(
                      [
                          Topics.Entities.Creature,
                          Topics.Entities.Skill,
                          Topics.Actions.Update
                      ])
                  .WithProperty(Subject)
                  .LogInformation("Aisling {@AislingName}'s ability {keyToKeep} removed.", Subject.Name, keyToRemove);
        }
    }

    private static void RemoveSkillsAndSpells(Aisling aisling, List<string> skills, List<string> spells)
    {
        foreach (var skill in skills.Where(skill => aisling.SkillBook.ContainsByTemplateKey(skill)))
            aisling.SkillBook.RemoveByTemplateKey(skill);

        foreach (var spell in spells.Where(spell => aisling.SpellBook.ContainsByTemplateKey(spell)))
            aisling.SpellBook.RemoveByTemplateKey(spell);
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
                var targetAc = 100 - Subject.UserStatSheet.Level / 3;

                // Calculate the difference in AC (amount to subtract)
                var acDifference = Subject.UserStatSheet.Ac - targetAc;

                // Calculate the new attack speed percentage: 1% for every 3 Dex minus the initial 3 Dex
                var newAtkSpeedPct = Math.Max(0, (Subject.UserStatSheet.Dex - 3) / 3);

                // Calculate the attack speed difference (amount to subtract)
                var atkSpeedPctDifference = Subject.UserStatSheet.AtkSpeedPct - newAtkSpeedPct;

                // Create the attributes object for the new values
                var newAttributes = new Attributes
                {
                    Ac = acDifference, // Pass the AC difference to subtract the correct value
                    AtkSpeedPct = atkSpeedPctDifference // Pass the attack speed percentage difference
                };

                // Apply the new stats
                Subject.UserStatSheet.Subtract(newAttributes);
            } else if (Subject.UserStatSheet.Level == 99)
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
                var newAttributes = new Attributes
                {
                    Ac = acDifference, // Pass the AC difference to subtract the correct value
                    AtkSpeedPct = atkSpeedPctDifference // Pass the attack speed percentage difference
                };

                // Apply the new stats
                Subject.UserStatSheet.Subtract(newAttributes);

                // Send full attribute update
                Subject.Client.SendAttributes(StatUpdateType.Full);
            }

            RemovePureOnlySpells("magmasurge");
            RemovePureOnlySpells("tidalbreeze");
            RemovePureOnlySpells("sightoffrailty");
            RemovePureOnlySpells("diacradh");
            RemovePureOnlySpells("hidegroup");
            RemovePureOnlySpells("healingaura");
            RemovePureOnlySpells("darkstorm");
            RemovePureOnlySpells("resurrect");
            RemovePureOnlySpells("evasion");
            RemovePureOnlySkills("annihilate");
            RemovePureOnlySkills("dragonstrike");
            RemovePureOnlySkills("chaosfist");
            RemovePureOnlySkills("madsoul");
            RemovePureOnlySkills("onslaught");
            RemovePureOnlySkills("sneakattack");
            RemovePureOnlySkills("shadowfigure");
            RemovePureOnlySkills("multistrike");
            RemovePureOnlySkills("battlefieldsweep");
            RemovePureOnlySkills("paralyzeforce");
            RemovePureOnlySkills("shadowfigure");
            RemoveOldMonkFormSkillsSpells(Subject);
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
            RemoveAndNotifyIfBothExist("rockstance", "earthenstance");
            RemoveAndNotifyIfBothExist("thunderstance", "lightningstance");
            RemoveAndNotifyIfBothExist("miststance", "tidestance");
            RemoveAndNotifyIfBothExist("smokestance", "earthenstance");
            RemoveAndNotifyIfBothExist("whirlwind", "wrath");
            RemoveAndNotifyIfBothExist("inferno", "wrath");
            RemoveAndNotifyIfBothExist("inferno", "whirlwind");
            RemoveAndNotifyIfBothExist("fury", "berserk");

            // SkillBook removals with the new methods
            RemoveAndNotifyIfBothExist("energybolt", "assail");
            RemoveAndNotifyIfBothExist("blessedbolt", "assail");
            RemoveAndNotifyIfBothExist("charge", "bullrush");
            RemoveAndNotifyIfBothExist("cleave", "scathe");
            RemoveAndNotifyIfBothExist("devour", "scathe");
            RemoveAndNotifyIfBothExist("devour", "cleave");
            RemoveAndNotifyIfBothExist("clobber", "strike");
            RemoveAndNotifyIfBothExist("clobber", "assail");
            RemoveAndNotifyIfBothExist("flank", "assail");
            RemoveAndNotifyIfBothExist("flank", "clobber");
            RemoveAndNotifyIfBothExist("flank", "strike");
            RemoveAndNotifyIfBothExist("wallop", "assail");
            RemoveAndNotifyIfBothExist("wallop", "flank");
            RemoveAndNotifyIfBothExist("wallop", "strike");
            RemoveAndNotifyIfBothExist("wallop", "clobber");
            RemoveAndNotifyIfBothExist("pulverize", "assail");
            RemoveAndNotifyIfBothExist("pulverize", "strike");
            RemoveAndNotifyIfBothExist("pulverize", "clobber");
            RemoveAndNotifyIfBothExist("pulverize", "flank");
            RemoveAndNotifyIfBothExist("pulverize", "wallop");
            RemoveAndNotifyIfBothExist("thrash", "assail");
            RemoveAndNotifyIfBothExist("thrash", "strike");
            RemoveAndNotifyIfBothExist("thrash", "clobber");
            RemoveAndNotifyIfBothExist("thrash", "flank");
            RemoveAndNotifyIfBothExist("thrash", "wallop");
            RemoveAndNotifyIfBothExist("thrash", "pulverize");
            RemoveAndNotifyIfBothExist("slaughter", "assail");
            RemoveAndNotifyIfBothExist("slaughter", "strike");
            RemoveAndNotifyIfBothExist("slaughter", "clobber");
            RemoveAndNotifyIfBothExist("slaughter", "flank");
            RemoveAndNotifyIfBothExist("slaughter", "wallop");
            RemoveAndNotifyIfBothExist("slaughter", "pulverize");
            RemoveAndNotifyIfBothExist("slaughter", "thrash");
            RemoveAndNotifyIfBothExist("sunder", "slash");
            RemoveAndNotifyIfBothExist("tempestblade", "windblade");
            RemoveAndNotifyIfBothExist("paralyzeforce", "groundstomp");
            RemoveAndNotifyIfBothExist("madsoul", "flurry");
            RemoveAndNotifyIfBothExist("charge", "bullrush");
            RemoveAndNotifyIfBothExist("doublepunch", "assail");
            RemoveAndNotifyIfBothExist("doublepunch", "punch");
            RemoveAndNotifyIfBothExist("rapidpunch", "punch");
            RemoveAndNotifyIfBothExist("rapidpunch", "assail");
            RemoveAndNotifyIfBothExist("rapidpunch", "doublepunch");
            RemoveAndNotifyIfBothExist("triplekick", "assail");
            RemoveAndNotifyIfBothExist("triplekick", "doublepunch");
            RemoveAndNotifyIfBothExist("triplekick", "punch");
            RemoveAndNotifyIfBothExist("triplekick", "rapidpunch");
            RemoveAndNotifyIfBothExist("dragonstrike", "eaglestrike");
            RemoveAndNotifyIfBothExist("dragonstrike", "phoenixstrike");
            RemoveAndNotifyIfBothExist("phoenixstrike", "eaglestrike");
            RemoveAndNotifyIfBothExist("roundhousekick", "kick");
            RemoveAndNotifyIfBothExist("mantiskick", "highkick");
            RemoveAndNotifyIfBothExist("smokescreen", "throwsmokebomb");
            RemoveAndNotifyIfBothExist("assault", "assail");
            RemoveAndNotifyIfBothExist("throwsurigum", "assault");
            RemoveAndNotifyIfBothExist("throwsurigum", "assail");
            RemoveAndNotifyIfBothExist("blitz", "assail");
            RemoveAndNotifyIfBothExist("blitz", "assault");
            RemoveAndNotifyIfBothExist("blitz", "throwsurigum");
            RemoveAndNotifyIfBothExist("barrage", "assail");
            RemoveAndNotifyIfBothExist("barrage", "assault");
            RemoveAndNotifyIfBothExist("barrage", "blitz");
            RemoveAndNotifyIfBothExist("barrage", "throwsurigum");
            RemoveAndNotifyIfBothExist("gut", "stab");
            RemoveAndNotifyIfBothExist("skewer", "pierce");
            RemoveAndNotifyIfBothExist("midnightslash", "assail");
            RemoveAndNotifyIfBothExist("midnightslash", "barrage");
            RemoveAndNotifyIfBothExist("midnightslash", "throwsurigum");
            RemoveAndNotifyIfBothExist("midnightslash", "blitz");
            RemoveAndNotifyIfBothExist("midnightslash", "assault");
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
                    var pets = Subject.MapInstance
                                      .GetEntities<Monster>()
                                      .Where(x => x.Script.Is<PetScript>() && x.Name.Contains(Subject.Name));

                    foreach (var pet in pets)
                        pet.MapInstance.RemoveEntity(pet);
                }

                if (Subject.Effects.Contains("mount"))
                    Subject.Effects.Dispel("mount");

                var trap = Subject.MapInstance
                                  .GetDistinctReactorsAtPoint(Subject)
                                  .Where(x => x.Script.Is<ForagingSpotScript>() || x.Script.Is<FishingSpotScript>());

                if (trap.Any())
                {
                    if (Subject.Options.SocialStatus != SocialStatus.Gathering)
                    {
                        PreAfkSocialStatus = Subject.Options.SocialStatus;
                        Subject.Options.SocialStatus = SocialStatus.Gathering;
                    }
                } else
                {
                    //set player to daydreaming if they are currently set to awake
                    if (Subject.Options.SocialStatus != SocialStatus.DayDreaming)
                    {
                        PreAfkSocialStatus = Subject.Options.SocialStatus;
                        Subject.Options.SocialStatus = SocialStatus.DayDreaming;
                    }
                }
            } else if (Subject.Options.SocialStatus is SocialStatus.DayDreaming or SocialStatus.Gathering)
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
using Chaos.Collections;
using Chaos.Collections.Abstractions;
using Chaos.Common.Definitions;
using Chaos.Common.Utilities;
using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Extensions.Geometry;
using Chaos.Formulae;
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
using Chaos.Scripting.FunctionalScripts.ApplyHealing;
using Chaos.Scripting.FunctionalScripts.ExperienceDistribution;
using Chaos.Scripting.MonsterScripts.Pet;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.AislingScripts;

public class DefaultAislingScript : AislingScriptBase, HealAbilityComponent.IHealComponentOptions
{
    private readonly HashSet<string> ArenaKeys = new(StringComparer.OrdinalIgnoreCase) { "arena_battle_ring", "arena_lava", "arena_lavateams", "arena_colorclash", "arena_escort"};
    private readonly IStore<BulletinBoard> BoardStore;
    private readonly IIntervalTimer ClearOrangeBarTimer;
    private readonly IClientRegistry<IWorldClient> ClientRegistry;
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
        "Escort - Teams",
        "Trial of Sacrifice",
        "Trial of Combat",
        "Trial of Luck",
        "Trial of Intelligence"
    ];

    private readonly IMerchantFactory MerchantFactory;
    private readonly ISimpleCache SimpleCache;
    private readonly IIntervalTimer SleepAnimationTimer;
    private readonly IItemFactory ItemFactory;

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

    private Animation MistHeal { get; } = new()
    {
        AnimationSpeed = 100,
        TargetAnimation = 646
    };
    protected virtual RelationshipBehavior RelationshipBehavior { get; }
    protected virtual RestrictionBehavior RestrictionBehavior { get; }
    protected virtual VisibilityBehavior VisibilityBehavior { get; }

    /// <inheritdoc />
    public DefaultAislingScript(
        Aisling subject,
        IClientRegistry<IWorldClient> clientRegistry,
        IMerchantFactory merchantFactory,
        ISimpleCache simpleCache,
        IEffectFactory effectFactory,
        IStore<MailBox> mailStore,
        IStore<BulletinBoard> boardStore,
        ILogger<DefaultAislingScript> logger,
        IItemFactory itemFactory
    )
        : base(subject)
    {
        MailStore = mailStore;
        BoardStore = boardStore;
        Logger = logger;
        ItemFactory = itemFactory;
        RestrictionBehavior = new RestrictionBehavior();
        VisibilityBehavior = new VisibilityBehavior();
        RelationshipBehavior = new RelationshipBehavior();
        ExperienceDistributionScript = DefaultExperienceDistributionScript.Create();
        SleepAnimationTimer = new IntervalTimer(TimeSpan.FromSeconds(5), false);
        ClearOrangeBarTimer = new IntervalTimer(TimeSpan.FromSeconds(WorldOptions.Instance.ClearOrangeBarTimerSecs), false);
        ClientRegistry = clientRegistry;
        EffectFactory = effectFactory;
        MerchantFactory = merchantFactory;
        SimpleCache = simpleCache;
        SourceScript = this;
        ApplyHealScript = ApplyNonAlertingHealScript.Create();
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

        if (Subject.IsThunderStanced())
        {
            var result = damage * 30;

            if (IntegerRandomizer.RollChance(2))
            {
                var effect = EffectFactory.Create("Suain");
                source.Effects.Apply(Subject, effect);
            }

            if (source is Monster monster)
                monster.AggroList.AddOrUpdate(Subject.Id, _ => result, (_, currentAggro) => currentAggro + result);
        }

        if (Subject.IsMistStanced())
        {
            var result = Math.Round(damage * 0.15m);

            if (Subject.Group is not null)
                foreach (var person in Subject.Group)
                {
                    person.Animate(MistHeal, person.Id);

                    ApplyHealScript.ApplyHeal(
                        source,
                        person,
                        this,
                        (int)result);

                    person.SendMessage($"Mist heals you for {result}.");
                }
            else
            {
                Subject.Animate(MistHeal, Subject.Id);

                ApplyHealScript.ApplyHeal(
                    source,
                    Subject,
                    this,
                    (int)result);

                Subject.SendMessage($"Mist heals you for {result}.");
            }
        }

        if (Subject.IsInLastStand())
            if (damage >= Subject.StatSheet.CurrentHp)
            {
                Subject.StatSheet.SetHp(1);
                Subject.Client.SendAttributes(StatUpdateType.Vitality);

                return;
            }

        if (Subject.Effects.Contains("mount"))
        {
            Subject.Effects.Dispel("mount");

            return;
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

    /// <inheritdoc />
    public override void Update(TimeSpan delta)
    {
        SleepAnimationTimer.Update(delta);
        ClearOrangeBarTimer.Update(delta);

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
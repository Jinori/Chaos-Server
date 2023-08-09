using Chaos.Collections;
using Chaos.Collections.Abstractions;
using Chaos.Common.Definitions;
using Chaos.Common.Utilities;
using Chaos.Definitions;
using Chaos.Extensions.Geometry;
using Chaos.Formulae;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Networking.Abstractions;
using Chaos.Scripting.Abstractions;
using Chaos.Scripting.AislingScripts.Abstractions;
using Chaos.Scripting.Behaviors;
using Chaos.Scripting.Components;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ApplyHealing;
using Chaos.Scripting.FunctionalScripts.ExperienceDistribution;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.AislingScripts;

public class DefaultAislingScript : AislingScriptBase, HealComponent.IHealComponentOptions
{
    private readonly IStore<BulletinBoard> BoardStore;
    private readonly IClientRegistry<IWorldClient> ClientRegistry;
    private readonly IEffectFactory EffectFactory;
    private readonly IStore<MailBox> MailStore;
    private readonly List<string> MapsToNotPunishDeathOn = new()
    {
        "Mr. Hopps's Home",
        "Cain's Farm",
        "Arena Battle Ring",
        "Lava Arena",
        "Lava Arena 2",
        "Lava Arena 3",
        "Lava Arena 4",
    };

    private readonly List<string> ArenaMaps = new()
    {
        "Arena Battle Ring",
        "Lava Arena",
        "Lava Arena 2",
        "Lava Arena 3",
        "Lava Arena 4",
    };
    
    private readonly IMerchantFactory MerchantFactory;
    private readonly ISimpleCache SimpleCache;
    private readonly IIntervalTimer SleepAnimationTimer;
    private SocialStatus PreAfkSocialStatus { get; set; }
    protected virtual BlindBehavior BlindBehavior { get; }
    protected virtual RelationshipBehavior RelationshipBehavior { get; }

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
    /// <inheritdoc />
    public IScript SourceScript { get; init; }
    private IExperienceDistributionScript ExperienceDistributionScript { get; }

    private Animation MistHeal { get; } = new()
    {
        AnimationSpeed = 100,
        TargetAnimation = 9
    };
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
        IStore<BulletinBoard> boardStore
    )
        : base(subject)
    {
        MailStore = mailStore;
        BoardStore = boardStore;
        RestrictionBehavior = new RestrictionBehavior();
        VisibilityBehavior = new VisibilityBehavior();
        RelationshipBehavior = new RelationshipBehavior();
        BlindBehavior = new BlindBehavior();
        ExperienceDistributionScript = DefaultExperienceDistributionScript.Create();
        SleepAnimationTimer = new IntervalTimer(TimeSpan.FromSeconds(5), false);
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
    public override bool IsBlind() => BlindBehavior.IsBlind(Subject);

    /// <inheritdoc />
    public override bool IsFriendlyTo(Creature creature) => RelationshipBehavior.IsFriendlyTo(Subject, creature);

    /// <inheritdoc />
    public override bool IsHostileTo(Creature creature) => RelationshipBehavior.IsHostileTo(Subject, creature);

    public override void OnAttacked(Creature source, int damage)
    {
        if (Subject.Status.HasFlag(Status.Pramh))
        {
            Subject.Status &= ~Status.Pramh;
            Subject.Effects.Dispel("pramh");
            Subject.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You awake from your slumber.");
        }

        if (Subject.Effects.Contains("wolfFangFist"))
        {
            Subject.Effects.Dispel("wolfFangFist");
            Subject.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You awake from your slumber.");
        }

        if (Subject.Status.HasFlag(Status.ThunderStance))
        {
            var result = damage * 3;

            if (IntegerRandomizer.RollChance(2))
            {
                var effect = EffectFactory.Create("Suain");
                source.Effects.Apply(Subject, effect);
            }

            if (source is Monster monster)
                monster.AggroList.AddOrUpdate(Subject.Id, _ => result, (_, currentAggro) => currentAggro + result);
        }

        if (Subject.Status.HasFlag(Status.MistStance))
        {
            var result = damage * .15m;

            if (Subject.Group is not null)
                foreach (var person in Subject.Group)
                {
                    person.Animate(MistHeal, person.Id);

                    ApplyHealScript.ApplyHeal(
                        source,
                        Subject,
                        this,
                        (int)result);
                }
            else
            {
                Subject.Animate(MistHeal, Subject.Id);

                ApplyHealScript.ApplyHeal(
                    source,
                    Subject,
                    this,
                    (int)result);
            }
        }

        if (Subject.Status.HasFlag(Status.LastStand))
            if (damage >= Subject.StatSheet.CurrentHp)
            {
                Subject.StatSheet.SetHp(1);
                Subject.Client.SendAttributes(StatUpdateType.Vitality);

                return;
            }

        if (Subject.Effects.Contains("mount"))
        {
            Subject.Effects.Terminate("mount");

            return;
        }

        if (Subject.Effects.Contains("rumination"))
        {
            Subject.Effects.Terminate("rumination");
            Subject.SendOrangeBarMessage("Taking damage ended your rumination.");

            return;
        }

        base.OnAttacked(source, damage);
    }

    /// <inheritdoc />
    public override void OnDeath()
    {
        var source = Subject.Trackers.LastDamagedBy;

        if (source?.MapInstance.Name.Equals("Cain's Farm") == true)
        {
            var mapInstance = SimpleCache.Get<MapInstance>("tutorial_hut");
            var pointS = new Point(2, 9);

            Subject.StatSheet.AddHp(1);
            Subject.Client.SendAttributes(StatUpdateType.Vitality);
            Subject.SendOrangeBarMessage("You are knocked out. Be more careful.");
            Subject.TraverseMap(mapInstance, pointS);

            return;
        }

        Subject.IsDead = true;

        //Refresh to show ghost
        Subject.Refresh(true);

        //Remove all effects from the player
        var effects = Subject.Effects.ToList();

        foreach (var condition in Enum.GetValues<Status>())
            Subject.Status &= ~condition;

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

            source.MapInstance.AddObject(terminus, point);
        }

        if (ArenaMaps.Contains(Subject.MapInstance.Name))
        {
            var aislings = Subject.MapInstance.GetEntities<Aisling>();

            foreach (var aisling in aislings)
            {
                aisling.SendServerMessage(
                    ServerMessageType.OrangeBar1,
                    $"{Subject.Name} was killed by {source?.Name}.");
            }
        }
        
        if (MapsToNotPunishDeathOn.Contains(Subject.MapInstance.Name))
            return;

        foreach (var client in ClientRegistry)
            client.SendServerMessage(
                ServerMessageType.OrangeBar1,
                $"{Subject.Name} was killed at {Subject.MapInstance.Name} by {source?.Name ?? "The Guardians"}.");

        //Let's break some items at 2% chance
        var itemsToBreak = Subject.Equipment.Where(x => x.Template.AccountBound is false);

        foreach (var item in itemsToBreak)
            if (IntegerRandomizer.RollChance(2))
            {
                Subject.Client.SendServerMessage(
                    ServerMessageType.OrangeBar1,
                    $"{item.DisplayName} has been consumed by death.");

                Subject.Equipment.TryGetRemove(item.Slot, out _);
            }

        var tenPercent = MathEx.GetPercentOf<int>((int)Subject.UserStatSheet.TotalExp, 10);

        if (ExperienceDistributionScript.TryTakeExp(Subject, tenPercent))
            Subject.Client.SendServerMessage(ServerMessageType.OrangeBar1, $"You have lost {tenPercent} experience.");
    }

    /// <inheritdoc />
    public override void Update(TimeSpan delta)
    {
        SleepAnimationTimer.Update(delta);

        if (SleepAnimationTimer.IntervalElapsed)
        {
            var lastManualAction = Subject.Trackers.LastManualAction;
            var isAfk = !lastManualAction.HasValue || (DateTime.UtcNow.Subtract(lastManualAction.Value).TotalMinutes > 5);

            if (isAfk)
            {
                if (Subject.IsAlive)
                    Subject.AnimateBody(BodyAnimation.Snore);

                //set player to daydreaming if they are currently set to awake
                if (Subject.Options.SocialStatus != SocialStatus.DayDreaming)
                {
                    PreAfkSocialStatus = Subject.Options.SocialStatus;
                    Subject.Options.SocialStatus = SocialStatus.DayDreaming;
                }
            } else if (Subject.Options.SocialStatus == SocialStatus.DayDreaming)
                Subject.Options.SocialStatus = PreAfkSocialStatus;
        }
    }
}
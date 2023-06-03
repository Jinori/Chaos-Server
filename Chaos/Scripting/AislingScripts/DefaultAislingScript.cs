using Chaos.Collections;
using Chaos.Common.Definitions;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Common.Utilities;
using Chaos.Definitions;
using Chaos.Extensions.Geometry;
using Chaos.Formulae;
using Chaos.Models.Data;
using Chaos.Networking.Abstractions;
using Chaos.Scripting.Abstractions;
using Chaos.Scripting.AislingScripts.Abstractions;
using Chaos.Scripting.Behaviors;
using Chaos.Scripting.Components;
using Chaos.Time;
using Chaos.Time.Abstractions;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ApplyHealing;
using Chaos.Scripting.FunctionalScripts.ExperienceDistribution;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.AislingScripts;

public class DefaultAislingScript : AislingScriptBase, HealComponent.IHealComponentOptions
{
    private readonly IIntervalTimer SleepAnimationTimer;
    protected virtual RestrictionBehavior RestrictionBehavior { get; }
    protected virtual VisibilityBehavior VisibilityBehavior { get; }
    private readonly IClientRegistry<IWorldClient> ClientRegistry;
    private readonly IMerchantFactory MerchantFactory;
    private readonly ISimpleCache SimpleCache;
    private readonly IEffectFactory EffectFactory;
    private IExperienceDistributionScript ExperienceDistributionScript { get; }
    private readonly List<string> MapsToNotPunishDeathOn = new()
    {
        "Mr. Hopps's Home",
        "Cain's Farm"
    };

    private Animation MistHeal { get; } = new()
    {
        AnimationSpeed = 100,
        TargetAnimation = 9
    };
    
   

    /// <inheritdoc />
    public DefaultAislingScript(Aisling subject, IClientRegistry<IWorldClient> clientRegistry, IMerchantFactory merchantFactory, ISimpleCache simpleCache, IEffectFactory effectFactory
    )
        : base(subject)
    {
        RestrictionBehavior = new RestrictionBehavior();
        VisibilityBehavior = new VisibilityBehavior();
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

    public override void OnAttacked(Creature source, int damage)
    {
        var equipment = Subject.Equipment.Where(x => x.CurrentDurability.HasValue ).ToList();
        foreach (var item in equipment)
        {
            var percent = (200 * item.CurrentDurability + 1) / (item.Template.MaxDurability * 2);
            if (percent <= 10)
                Subject.SendOrangeBarMessage($"{item.DisplayName} is at {percent}% durability");
        }
        
        if (Subject.Status.HasFlag(Status.Pramh))
        {
            Subject.Status &= ~Status.Pramh;
            Subject.Effects.Dispel("pramh");
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
            {
                foreach (var person in Subject.Group)
                {
                    person.Animate(MistHeal, person.Id);
                    ApplyHealScript.ApplyHeal(source, Subject, this, (int)result);
                }
            }
            else
            {
                Subject.Animate(MistHeal, Subject.Id);
                ApplyHealScript.ApplyHeal(source, Subject, this, (int)result);
            }
        }
        if (Subject.Status.HasFlag(Status.LastStand))
        {
            if (damage >= Subject.StatSheet.CurrentHp)
            {
                Subject.StatSheet.SetHp(1);
                var aisling = Subject as Aisling;
                aisling.Client.SendAttributes(StatUpdateType.Vitality);
                return;
            }
        }

        if (Subject.Effects.Contains("mount"))
        {
            Subject.Effects.Terminate("mount");

            return;
        }
        
        base.OnAttacked(source, damage);
    }

    /// <inheritdoc />
    public override void OnDeath(Creature source)
    {

        if (source.MapInstance.Name.Equals("Cain's Farm"))
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
        {
            Subject.Status &= ~condition;
        }

        foreach (var effect in effects)
            Subject.Effects.Dispel(effect.Name);

        if (source.MapInstance.Name.Equals("Mr. Hopps's Home"))
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

        if (MapsToNotPunishDeathOn.Contains(Subject.MapInstance.Name))
            return;
        

        foreach (var client in ClientRegistry)
            client.SendServerMessage(
                ServerMessageType.OrangeBar1,
                $"{Subject.Name} was killed at {Subject.MapInstance.Name} by {source.Name}.");

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
        {
            Subject.Client.SendServerMessage(ServerMessageType.OrangeBar1, $"You have lost {tenPercent} experience.");   
        }
    }

    /// <inheritdoc />
    public override void Update(TimeSpan delta)
    {
        SleepAnimationTimer.Update(delta);

        if (SleepAnimationTimer.IntervalElapsed)
        {
            var lastManualAction = Subject.Trackers.LastManualAction;

            if (!lastManualAction.HasValue || (DateTime.UtcNow.Subtract(lastManualAction.Value).TotalMinutes > 5))
                Subject.AnimateBody(BodyAnimation.Snore);
        }
    }

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
}
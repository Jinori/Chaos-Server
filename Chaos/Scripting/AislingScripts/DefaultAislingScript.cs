using Chaos.Clients.Abstractions;
using Chaos.Common.Definitions;
using Chaos.Common.Utilities;
using Chaos.Containers;
using Chaos.Definitions;
using Chaos.Extensions.Geometry;
using Chaos.Networking.Abstractions;
using Chaos.Objects.Panel;
using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripting.AislingScripts.Abstractions;
using Chaos.Scripting.Components;
using Chaos.Time;
using Chaos.Time.Abstractions;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ExperienceDistribution;
using Chaos.Services.Factories;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.AislingScripts;

public sealed class DefaultAislingScript : AislingScriptBase
{
    private readonly IIntervalTimer SleepAnimationTimer;
    private readonly IClientRegistry<IWorldClient> _clientRegistry;
    private readonly IMerchantFactory MerchantFactory;
    private readonly ISimpleCache SimpleCache;
    private IExperienceDistributionScript ExperienceDistributionScript { get; }
    private readonly List<string> _mapsToNotPunishDeathOn = new()
    {
        "Mr. Hopps's Home",
        "Cain's Farm"
    };

    private RestrictionComponent RestrictionComponent { get; }

    /// <inheritdoc />
    public DefaultAislingScript(Aisling subject, IClientRegistry<IWorldClient> clientRegistry, IMerchantFactory merchantFactory,
        ISimpleCache simpleCache
    )
        : base(subject)
    {
        ExperienceDistributionScript = DefaultExperienceDistributionScript.Create();
        RestrictionComponent = new RestrictionComponent();
        SleepAnimationTimer = new IntervalTimer(TimeSpan.FromSeconds(5));
        _clientRegistry = clientRegistry;
        MerchantFactory = merchantFactory;
        SimpleCache = simpleCache;
    }


    /// <inheritdoc />
    public override bool CanMove() => RestrictionComponent.CanMove(Subject);

    /// <inheritdoc />
    public override bool CanTalk() => RestrictionComponent.CanTalk(Subject);

    /// <inheritdoc />
    public override bool CanTurn() => RestrictionComponent.CanTurn(Subject);

    /// <inheritdoc />
    public override bool CanUseItem(Item item) => RestrictionComponent.CanUseItem(Subject, item);

    /// <inheritdoc />
    public override bool CanUseSkill(Skill skill) => RestrictionComponent.CanUseSkill(Subject, skill);

    /// <inheritdoc />
    public override bool CanUseSpell(Spell spell) => RestrictionComponent.CanUseSpell(Subject, spell);

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

        if (_mapsToNotPunishDeathOn.Contains(Subject.MapInstance.Name))
            return;
        

        foreach (var client in _clientRegistry)
            client.SendServerMessage(
                ServerMessageType.OrangeBar1,
                $"{Subject.Name} was killed at {Subject.MapInstance.Name} by {source.Name}.");

        //Let's break some items at 2% chance
        var itemsToBreak = Subject.Equipment.Where(x => x.Template.AccountBound is false);

        foreach (var item in itemsToBreak)
            if (Randomizer.RollChance(2))
            {
                Subject.Client.SendServerMessage(ServerMessageType.OrangeBar1, $"{item.DisplayName} has been consumed by the Death God.");
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
}
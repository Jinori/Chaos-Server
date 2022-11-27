using Chaos.Common.Definitions;
using Chaos.Containers;
using Chaos.Containers.Abstractions;
using Chaos.Data;
using Chaos.Extensions;
using Chaos.Extensions.Geometry;
using Chaos.Formulae;
using Chaos.Geometry.Abstractions;
using Chaos.Geometry.Abstractions.Definitions;
using Chaos.Networking.Definitions;
using Chaos.Objects.Panel;
using Chaos.Scripts.EffectScripts.Abstractions;
using Chaos.Utilities;
using Microsoft.Extensions.Logging;

namespace Chaos.Objects.World.Abstractions;

public abstract class Creature : NamedEntity, IAffected
{
    public Direction Direction { get; set; }
    public IEffectsBar Effects { get; set; }
    public int GamePoints { get; set; }
    public int Gold { get; set; }
    public virtual bool IsDead { get; set; }
    public DateTime LastAttack { get; set; }
    public DateTime LastMove { get; set; }
    public Status Status { get; set; }
    protected ConcurrentDictionary<uint, DateTime> LastClicked { get; init; }
    public abstract int AssailIntervalMs { get; }
    public virtual bool IsAlive => StatSheet.CurrentHp > 0;
    public abstract StatSheet StatSheet { get; }
    public abstract CreatureType Type { get; }
    protected abstract ILogger Logger { get; }

    protected Creature(
        string name,
        ushort sprite,
        MapInstance mapInstance,
        IPoint point
    )
        : base(
            name,
            (ushort)(sprite == 0 ? 0 : sprite + NETWORKING_CONSTANTS.CREATURE_SPRITE_OFFSET),
            mapInstance,
            point)
    {
        Direction = Direction.Down;
        Effects = new EffectsBar(this);
        LastClicked = new ConcurrentDictionary<uint, DateTime>();
    }

    /// <inheritdoc />
    public override void Animate(Animation animation, uint? sourceId = null)
    {
        var targetedAnimation = animation.GetTargetedAnimation(Id, sourceId);

        foreach (var obj in MapInstance.GetEntitiesWithinRange<Aisling>(this)
                                       .ThatCanSee(this))
            obj.Client.SendAnimation(targetedAnimation);
    }

    public virtual void AnimateBody(BodyAnimation bodyAnimation, ushort speed = 25, byte? sound = null)
    {
        foreach (var aisling in MapInstance.GetEntitiesWithinRange<Aisling>(this)
                                           .ThatCanSee(this))
            aisling.Client.SendBodyAnimation(
                Id,
                bodyAnimation,
                speed,
                sound);
    }

    public abstract void ApplyDamage(
        Creature source,
        int amount,
        byte? hitSound = 1
    );

    public abstract void ApplyHealing(
    Creature source,
    int amount);

    public abstract void ApplyMana(
        Creature source,
        int amount);

    public virtual bool CanUse(Skill skill, [MaybeNullWhen(false)] out SkillContext skillContext)
    {
        skillContext = null;

        if (!skill.CanUse())
            return false;

        skillContext = new SkillContext(this);

        return skill.Script.CanUse(skillContext);
    }

    public virtual bool CanUse(
        Spell spell,
        Creature target,
        string? prompt,
        [MaybeNullWhen(false)]
        out SpellContext spellContext
    )
    {
        spellContext = null;

        if (!spell.CanUse())
            return false;

        spellContext = new SpellContext(target, this, prompt);

        return spell.Script.CanUse(spellContext);
    }

    public void Chant(string message) => ShowPublicMessage(PublicMessageType.Chant, message);

    public virtual void Drop(IPoint point, IEnumerable<Item> items)
    {
        var groundItems = items.Select(i => new GroundItem(i, MapInstance, point))
                               .ToList();

        if (!groundItems.Any())
            return;

        MapInstance.AddObjects(groundItems);

        var reactors = MapInstance.GetEntitiesAtPoint<ReactorTile>(point)
                                  .ToList();

        foreach (var groundItem in groundItems)
        {
            groundItem.Item.Script.OnDropped(this, MapInstance);
            Logger.LogDebug("{Name} dropped {Item}", Name, groundItem);

            foreach (var reactor in reactors)
                reactor.OnItemDroppedOn(this, groundItem);
        }
    }

    public virtual void Drop(IPoint point, params Item[] items) => Drop(point, items.AsEnumerable());

    public virtual void DropGold(IPoint point, int amount)
    {
        if ((amount <= 0) || (amount > Gold))
            return;

        Gold -= amount;

        var money = new Money(amount, MapInstance, point);

        MapInstance.AddObject(money, point);

        foreach (var reactor in MapInstance.GetEntitiesAtPoint<ReactorTile>(point))
            reactor.OnGoldDroppedOn(this, money);
    }

    public virtual bool IsFriendlyTo(Creature other) => other switch
    {
        Monster  => false,
        Aisling  => this is Aisling or Merchant, //could also check if map is pvp enabled or something
        Merchant => this is not Monster,
        _        => false
    };

    public virtual void OnApproached(Creature creature) { }
    public virtual void OnDeparture(Creature creature) { }

    public abstract void OnGoldDroppedOn(Aisling source, int amount);

    public abstract void OnItemDroppedOn(Aisling source, byte slot, byte count);

    public void Pathfind(IPoint target)
    {
        var nearbyDoors = MapInstance.GetEntitiesWithinRange<Door>(this)
                                     .Where(door => door.Closed);

        var nearbyCreatures = MapInstance.GetEntitiesWithinRange<Creature>(this)
                                         .ThatCollideWith(this);

        var nearbyUnwalkablePoints = nearbyDoors.Concat<IPoint>(nearbyCreatures)
                                                .ToList();

        var direction = MapInstance.Pathfinder.Pathfind(
            MapInstance.InstanceId,
            this,
            target,
            Type == CreatureType.WalkThrough,
            nearbyUnwalkablePoints);

        if (direction == Direction.Invalid)
            return;

        Walk(direction);
    }

    public void Say(string message) => ShowPublicMessage(PublicMessageType.Normal, message);

    public bool ShouldRegisterClick(uint fromId) =>
        !LastClicked.TryGetValue(fromId, out var lastClick) || (DateTime.UtcNow.Subtract(lastClick).TotalMilliseconds > 500);

    public void Shout(string message) => ShowPublicMessage(PublicMessageType.Shout, message);

    public virtual void ShowHealth(byte? sound = null)
    {
        foreach (var aisling in MapInstance.GetEntitiesWithinRange<Aisling>(this)
                                           .ThatCanSee(this))
            aisling.Client.SendHealthBar(this, sound);
    }

    public void ShowPublicMessage(PublicMessageType publicMessageType, string message)
    {
        IEnumerable<Creature>? entitiesWithinRange;
        var sendMessage = message;

        switch (publicMessageType)
        {
            case PublicMessageType.Normal:
                entitiesWithinRange = MapInstance.GetEntitiesWithinRange<Creature>(this);
                sendMessage = $"{Name}: {message}";

                break;
            case PublicMessageType.Shout:
                entitiesWithinRange = MapInstance.GetEntities<Creature>();
                sendMessage = $"{Name}: {message}";

                break;
            case PublicMessageType.Chant:
                entitiesWithinRange = MapInstance.GetEntities<Creature>();

                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(publicMessageType), publicMessageType, null);
        }

        foreach (var creature in entitiesWithinRange)
            switch (creature)
            {
                case Aisling aisling when IsVisibleTo(aisling):
                    aisling.Client.SendPublicMessage(Id, publicMessageType, sendMessage);

                    break;
                case Merchant merchant:
                    merchant.Script.OnPublicMessage(this, message);

                    break;
            }
    }

    public void TraverseMap(MapInstance destinationMap, IPoint destinationPoint)
    {
        var currentMap = MapInstance;

        if (!currentMap.RemoveObject(this))
            return;

        //set the creature's location and point
        //but at this point they are not technically on the map yet
        //this is so that if a player executes a handler, that handler will enter the new map's synchronization instead of the old one
        //and if they do any movement or anything, it will on the new map
        SetLocation(destinationMap, destinationPoint);

        //run a task that will await entrancy into the destination map
        //once synchronized, the creature will be added to the map
        Task.Run(
            async () =>
            {
                await using var sync = await destinationMap.Sync.WaitAsync();
                destinationMap.AddObject(this, destinationPoint);
            });
    }

    public virtual bool TryUseSkill(Skill skill)
    {
        if (!CanUse(skill, out var context))
            return false;

        skill.Use(context);

        return true;
    }

    public virtual bool TryUseSpell(Spell spell, uint? targetId = null, string? prompt = null)
    {
        Creature? target;

        if (!targetId.HasValue)
        {
            if (spell.Template.SpellType == SpellType.Targeted)
                return false;

            target = this;
        } else if (!MapInstance.TryGetObject(targetId.Value, out target))
            return false;

        if (!CanUse(
                spell,
                target!,
                prompt,
                out var context))
            return false;

        spell.Use(context);

        return true;
    }

    public virtual void Turn(Direction direction)
    {
        Direction = direction;

        foreach (var aisling in MapInstance.GetEntitiesWithinRange<Aisling>(this)
                                           .ThatCanSee(this))
            aisling.Client.SendCreatureTurn(Id, direction);
    }

    public override void Update(TimeSpan delta) => Effects.Update(delta);

    public virtual void Walk(Direction direction)
    {
        Direction = direction;
        var startPoint = Point.From(this);
        var endPoint = ((IPoint)this).DirectionalOffset(direction);

        if (!MapInstance.IsWalkable(endPoint, Type))
            return;

        var visibleBefore = MapInstance.GetEntitiesWithinRange<Creature>(this)
                                       .ToList();

        SetLocation(endPoint);

        var visibleAfter = MapInstance.GetEntitiesWithinRange<Creature>(this)
                                      .ToList();

        foreach (var creature in visibleBefore.Except(visibleAfter))
        {
            if (creature is Aisling aisling)
                HideFrom(aisling);

            Helpers.HandleDeparture(creature, this);
        }

        foreach (var creature in visibleAfter.Except(visibleBefore))
        {
            if (creature is Aisling aisling)
                ShowTo(aisling);

            Helpers.HandleApproach(creature, this);
        }

        foreach (var aisling in visibleAfter.Intersect(visibleBefore).OfType<Aisling>())
            aisling.Client.SendCreatureWalk(Id, startPoint, Direction);

        foreach (var reactor in MapInstance.GetEntitiesAtPoint<ReactorTile>(Point.From(this)))
            reactor.OnWalkedOn(this);
    }

    public void Wander()
    {
        var nearbyDoors = MapInstance.GetEntitiesWithinRange<Door>(this, 1)
                                     .Where(door => door.Closed);

        var nearbyCreatures = MapInstance.GetEntitiesWithinRange<Creature>(this, 1)
                                         .ThatCollideWith(this);

        var nearbyUnwalkablePoints = nearbyDoors.Concat<IPoint>(nearbyCreatures)
                                                .ToList();

        var direction = MapInstance.Pathfinder.Wander(
            MapInstance.InstanceId,
            this,
            Type == CreatureType.WalkThrough,
            nearbyUnwalkablePoints);

        if (direction == Direction.Invalid)
            return;

        Walk(direction);
    }

    /// <inheritdoc />
    public override void WarpTo(IPoint destinationPoint)
    {
        Hide();

        var creaturesBefore = MapInstance.GetEntitiesWithinRange<Creature>(this)
                                         .ToList();

        SetLocation(destinationPoint);

        var creaturesAfter = MapInstance.GetEntitiesWithinRange<Creature>(this)
                                        .ToList();

        foreach (var creature in creaturesBefore.Except(creaturesAfter))
            Helpers.HandleDeparture(creature, this);

        foreach (var creature in creaturesAfter.Except(creaturesBefore))
            Helpers.HandleApproach(creature, this);

        Display();
    }

    public virtual bool WillCollideWith(Creature other) => Type.WillCollideWith(other.Type);

    public virtual bool WithinLevelRange(Creature other) =>
        LevelRangeFormulae.Default.WithinLevelRange(StatSheet.Level, other.StatSheet.Level);
}
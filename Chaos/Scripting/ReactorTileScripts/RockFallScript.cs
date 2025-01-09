using Chaos.DarkAges.Definitions;
using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Extensions.Geometry;
using Chaos.Formulae;
using Chaos.Geometry.Abstractions.Definitions;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Components.AbilityComponents;
using Chaos.Scripting.Components.Execution;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ApplyDamage;
using Chaos.Scripting.ReactorTileScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.ReactorTileScripts;

public class RockFallScript : ConfigurableReactorTileScriptBase,
                              GetTargetsAbilityComponent<Creature>.IGetTargetsComponentOptions,
                              SoundAbilityComponent.ISoundComponentOptions,
                              AnimationAbilityComponent.IAnimationComponentOptions,
                              TrapDamageAbilityComponent.ITrapDamageComponentOptions,
                              ManaDrainAbilityComponent.IManaDrainComponentOptions,
                              ApplyEffectAbilityComponent.IApplyEffectComponentOptions
{
    protected IIntervalTimer AnimationTimer { get; set; }

    protected IIntervalTimer DamageTimer { get; set; }
    protected Creature Owner { get; set; }
    protected IIntervalTimer? Timer { get; set; }
    protected int TriggerCount { get; set; }

    /// <inheritdoc />
    public RockFallScript(ReactorTile subject, IEffectFactory effectFactory)
        : base(subject)
    {
        if (Subject.Owner == null)
            throw new Exception(
                $"""{nameof(TrapScript)} script initialized for {Subject} that has no owner. If this reactor was created through json, you must specify the optional parameter "owningMonsterTemplateKey". If this reactor was created through a script, you must specify the owner in the {nameof(IReactorTileFactory)}.{nameof(IReactorTileFactory.Create)}() call.""");

        Owner = subject.Owner!;
        EffectFactory = effectFactory;
        TriggerCount = 0;

        if (DurationSecs.HasValue)
            Timer = new IntervalTimer(TimeSpan.FromSeconds(DurationSecs.Value), false);

        AnimationTimer = new IntervalTimer(TimeSpan.FromSeconds(1), false);
        DamageTimer = new IntervalTimer(TimeSpan.FromSeconds(3), false);
        ApplyDamageScript = ApplyNonAttackDamageScript.Create();
        ApplyDamageScript.DamageFormula = DamageFormulae.PureDamage;
    }

    private void ApplyTrapEffects(Creature target)
    {
        if (!Filter.IsValidTarget(Owner, target))
            return;

        if (target.IsGodModeEnabled())
            return;

        var executed = new ComponentExecutor(Owner, target).WithOptions(this)
                                                           .ExecuteAndCheck<GetTargetsAbilityComponent<Creature>>()
                                                           ?.Execute<SoundAbilityComponent>()
                                                           .Execute<AnimationAbilityComponent>()
                                                           .Execute<TrapDamageAbilityComponent>()
                                                           .Execute<ManaDrainAbilityComponent>()
                                                           .Execute<ApplyEffectAbilityComponent>()
                       != null;

        if (Owner is Aisling && target is Aisling aisling)
            aisling.SendOrangeBarMessage("You've been crushed by a falling ice boulder!");

        if (executed && MaxTriggers.HasValue)
        {
            TriggerCount++;

            if (TriggerCount >= MaxTriggers)
                Map.RemoveEntity(Subject);
        }
    }

    /// <inheritdoc />
    public override void OnWalkedOn(Creature source)
    {
        if (source is not Aisling aisling)
            return;

        var point = source.DirectionalOffset(source.Direction.Reverse());
        source.WarpTo(point);
        aisling.SendOrangeBarMessage("A big ice boulder is in your way.");
    }

    /// <inheritdoc />
    public override void Update(TimeSpan delta)
    {
        base.Update(delta);
        DamageTimer.Update(delta);

        AnimationTimer.Update(delta);

        if (Subject.Owner is not null && AnimationTimer.IntervalElapsed && Subject.MapInstance.Equals(Subject.Owner.MapInstance))
            if (Animation != null)
                Subject.Owner.MapInstance.ShowAnimation(Animation.GetPointAnimation(Subject));

        if (DamageTimer.IntervalElapsed)
            foreach (var creature in Subject.MapInstance.GetEntities<Creature>())
            {
                var creaturepoint = (creature.X, creature.Y);
                var trappoint = (Subject.X, Subject.Y);

                if (creaturepoint == trappoint)
                {
                    if (creature is not Aisling)
                        return;

                    ApplyTrapEffects(creature);

                    // Move the creature to a random direction
                    var randomDirection = (Direction)new Random().Next(0, 4); // Assuming 8 directions
                    var randomOffset = creature.DirectionalOffset(randomDirection);

                    if (Subject.MapInstance.IsWalkable(randomOffset, CreatureType.Normal)) // Ensure the point is walkable
                        creature.WarpTo(randomOffset);
                }
            }

        if (Timer != null)
        {
            Timer.Update(delta);

            if (Timer.IntervalElapsed)
                Map.RemoveEntity(Subject);
        }
    }

    #region ScriptVars
    public TimeSpan? EffectDurationOverride { get; init; }

    public IEffectFactory EffectFactory { get; init; }
    public IApplyDamageScript ApplyDamageScript { get; init; }
    public AoeShape Shape { get; init; }
    public bool SingleTarget { get; init; }
    public BodyAnimation BodyAnimation { get; init; }
    public int Range { get; init; }
    public TargetFilter Filter { get; init; }
    public Animation? Animation { get; init; }
    public byte? Sound { get; init; }
    public bool AnimatePoints { get; init; }
    public bool MustHaveTargets { get; init; } = true;
    public int? ExclusionRange { get; init; }
    public bool StopOnWalls { get; init; }
    public bool StopOnFirstHit { get; init; }

    public int? BaseDamage { get; init; }
    public bool? MoreDmgLowTargetHp { get; init; }
    public Stat? DamageStat { get; init; }
    public decimal? DamageStatMultiplier { get; init; }
    public decimal? PctHpDamage { get; init; }
    public Element? Element { get; init; }
    public int? DurationSecs { get; init; }
    public int? MaxTriggers { get; init; }
    public string? EffectKey { get; init; }
    public int? EffectApplyChance { get; init; }
    public int? ManaDrain { get; init; }
    public decimal PctManaDrain { get; init; }
    #endregion
}
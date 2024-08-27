using Chaos.Collections;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Abstractions;

public abstract class MonsterScriptBase : SubjectiveScriptBase<Monster>, IMonsterScript
{
    protected Creature? Target
    {
        get => Subject.Target;
        set => Subject.Target = value;
    }

    protected virtual ConcurrentDictionary<uint, int> AggroList => Subject.AggroList;
    protected virtual int AggroRange => Subject.AggroRange;
    protected virtual MapInstance Map => Subject.MapInstance;
    protected virtual bool ShouldMove => Subject.MoveTimer.IntervalElapsed;
    protected virtual bool ShouldUseSkill => Subject.SkillTimer.IntervalElapsed;
    protected virtual bool ShouldUseSpell => Subject.SpellTimer.IntervalElapsed;
    protected virtual bool ShouldWander => Subject.WanderTimer.IntervalElapsed;
    protected virtual IList<Skill> Skills => Subject.Skills;
    protected virtual IList<Spell> Spells => Subject.Spells;

    /// <inheritdoc />
    protected MonsterScriptBase(Monster subject)
        : base(subject) { }

    /// <inheritdoc />
    public virtual bool CanDropItemOn(Aisling source, Item item) => !(item.Template.AccountBound || item.Template.NoTrade);

    /// <inheritdoc />
    public virtual bool CanMove() => true;

    /// <inheritdoc />
    public virtual bool CanSee(VisibleEntity entity) => true;

    /// <inheritdoc />
    public virtual bool CanTalk() => true;

    /// <inheritdoc />
    public virtual bool CanTurn() => true;

    /// <inheritdoc />
    public virtual bool CanUseSkill(Skill skill) => true;

    /// <inheritdoc />
    public virtual bool CanUseSpell(Spell spell) => true;

    /// <inheritdoc />
    public virtual bool IsFriendlyTo(Creature creature) => false;

    /// <inheritdoc />
    public virtual bool IsHostileTo(Creature creature) => false;

    /// <inheritdoc />
    public virtual void OnApproached(Creature source) { }

    /// <inheritdoc />
    public virtual void OnAttacked(Creature source, int damage, int? aggroOverride) { }

    public virtual void OnAttacked(Creature source, int damage)
    {
        if (Subject.Effects.Contains("pramh"))
        {
            Subject.Effects.Dispel("pramh");
        }

        if (Subject.Effects.Contains("beagpramh"))
        {
            Subject.Effects.Dispel("beagpramh");
        }

        if (Subject.Effects.Contains("wolfFangFist"))
        {
            Subject.Effects.Dispel("wolfFangFist");
        }
        
        if (Subject.Effects.Contains("Crit"))
            Subject.Effects.Dispel("Crit");

        if (Subject.Effects.Contains("Amnesia"))
            Subject.Effects.Terminate("Amnesia");
            
        OnAttacked(source, damage, null);
    }

    /// <inheritdoc />
    public virtual void OnClicked(Aisling source) { }

    /// <inheritdoc />
    public virtual void OnDeath() { }

    /// <inheritdoc />
    public virtual void OnDeparture(Creature source) { }

    /// <inheritdoc />
    public virtual void OnGoldDroppedOn(Aisling source, int amount) { }

    /// <inheritdoc />
    public virtual void OnHealed(Creature source, int healing) { }

    /// <inheritdoc />
    public virtual void OnItemDroppedOn(Aisling source, Item item) { }

    /// <inheritdoc />
    public virtual void OnPublicMessage(Creature source, string message) { }

    /// <inheritdoc />
    public virtual void OnSpawn() { }

    /// <inheritdoc />
    public virtual void Update(TimeSpan delta) { }
}
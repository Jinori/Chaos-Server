using System.Runtime.InteropServices;
using Chaos.Collections;
using Chaos.Common.Definitions;
using Chaos.Common.Utilities;
using Chaos.Extensions.Common;
using Chaos.Geometry.Abstractions;
using Chaos.Geometry.Abstractions.Definitions;
using Chaos.Geometry.EqualityComparers;
using Chaos.Models.Abstractions;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.Templates;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Abstractions;
using Chaos.Scripting.MonsterScripts.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;
using Microsoft.Extensions.Logging;

namespace Chaos.Models.World;

public sealed class Monster : Creature, IScripted<IMonsterScript>, IDialogSourceEntity
{
    public int AggroRange { get; set; }
    public ICollection<IPoint> BlackList { get; set; }
    public int Experience { get; set; }
    public LootTable? LootTable { get; set; }
    public Creature? Target { get; set; }
    public Aisling? PetOwner { get; set; }
    public ConcurrentDictionary<uint, int> AggroList { get; }
    public ConcurrentDictionary<uint, int> Contribution { get; }
    public List<Item> Items { get; }
    public new int Gold { get; set; }
    public override ILogger<Monster> Logger { get; }
    public IIntervalTimer MoveTimer { get; }

    /// <inheritdoc />
    public override IMonsterScript Script { get; }
    /// <inheritdoc />
    public override ISet<string> ScriptKeys { get; }
    public List<Skill> Skills { get; }
    public IIntervalTimer SkillTimer { get; }
    public List<Spell> Spells { get; }
    public IIntervalTimer SpellTimer { get; }
    public override StatSheet StatSheet { get; }
    public MonsterTemplate Template { get; }
    public override CreatureType Type { get; }
    public IIntervalTimer WanderTimer { get; }
    /// <inheritdoc />
    public override int AssailIntervalMs => Template.AssailIntervalMs;

    /// <inheritdoc />
    DisplayColor IDialogSourceEntity.Color => DisplayColor.Default;

    /// <inheritdoc />
    EntityType IDialogSourceEntity.EntityType => EntityType.Creature;

    public Monster(
        MonsterTemplate template,
        MapInstance mapInstance,
        IPoint point,
        ILogger<Monster> logger,
        IScriptProvider scriptProvider,
        ICollection<string>? extraScriptKeys = null,
        Aisling? owner = null
    )
        : base(
            template.Name,
            template.Sprite,
            mapInstance,
            point)
    {
        extraScriptKeys ??= Array.Empty<string>();

        AggroRange = template.AggroRange;
        Experience = template.ExpReward;
        Gold = Random.Shared.Next(template.MinGoldDrop, template.MaxGoldDrop + 1);
        Items = new List<Item>();
        Skills = new List<Skill>();
        Spells = new List<Spell>();
        Template = template;
        Logger = logger;
        StatSheet = ShallowCopy<StatSheet>.Create(template.StatSheet);
        Items = new List<Item>();
        Gold = new int();
        PetOwner = owner;
        Type = template.Type;
        Direction = (Direction)Random.Shared.Next(4);
        AggroList = new ConcurrentDictionary<uint, int>();
        Contribution = new ConcurrentDictionary<uint, int>();
        WanderTimer = new RandomizedIntervalTimer(TimeSpan.FromMilliseconds(template.WanderIntervalMs), 10, RandomizationType.Positive);
        MoveTimer = new RandomizedIntervalTimer(TimeSpan.FromMilliseconds(template.MoveIntervalMs), 10, RandomizationType.Positive);
        SkillTimer = new RandomizedIntervalTimer(TimeSpan.FromMilliseconds(template.SkillIntervalMs), 50);
        SpellTimer = new RandomizedIntervalTimer(TimeSpan.FromMilliseconds(template.SpellIntervalMs), 50);
        ScriptKeys = new HashSet<string>(template.ScriptKeys, StringComparer.OrdinalIgnoreCase);
        BlackList = new HashSet<IPoint>(PointEqualityComparer.Instance);
        ScriptKeys.AddRange(extraScriptKeys);
        Script = scriptProvider.CreateScript<IMonsterScript, Monster>(ScriptKeys, this);
    }

    /// <inheritdoc />
    void IDialogSourceEntity.Activate(Aisling source) => Script.OnClicked(source);

    public void ResetAggro()
    {
        Target = null;
        AggroList.Clear();

        foreach (var key in ApproachTime.Keys)
            ApproachTime[key] = DateTime.UtcNow;
    }

    public void ResetAggro(uint id)
    {
        if (Target?.Id == id)
            Target = null;

        AggroList.Remove(id, out _);

        if (ApproachTime.TryGetValue(id, out _))
            ApproachTime[id] = DateTime.UtcNow;
    }

    /// <inheritdoc />
    public override void Update(TimeSpan delta)
    {
        foreach (ref var skill in CollectionsMarshal.AsSpan(Skills))
            skill.Update(delta);

        foreach (ref var spell in CollectionsMarshal.AsSpan(Spells))
            spell.Update(delta);

        WanderTimer.Update(delta);
        MoveTimer.Update(delta);
        SkillTimer.Update(delta);
        SpellTimer.Update(delta);

        base.Update(delta);
    }

    /// <inheritdoc />
    public override void Wander(ICollection<IPoint>? unwalkablePoints = null)
    {
        if (unwalkablePoints.IsNullOrEmpty())
            base.Wander(BlackList);
        else if (BlackList.IsNullOrEmpty())
            base.Wander(unwalkablePoints);
        else
            base.Wander(unwalkablePoints!.Concat(BlackList).ToList());
    }
}
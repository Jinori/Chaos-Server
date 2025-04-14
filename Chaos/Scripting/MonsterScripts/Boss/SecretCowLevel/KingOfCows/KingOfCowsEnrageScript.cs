using Chaos.Collections;
using Chaos.Extensions;
using Chaos.Extensions.Geometry;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Scripting.MonsterScripts.Abstractions;
using Chaos.Scripting.ReactorTileScripts.Secret;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Boss.SecretCowLevel.KingOfCows;

public sealed class KingOfCowsEnrageScript : MonsterScriptBase
{
    private readonly IMonsterFactory MonsterFactory;
    private bool Bonus30Applied;
    private bool Bonus50Applied;
    private bool Bonus75Applied;
    private readonly ISimpleCache SimpleCache;
    
    private Animation UpgradeAnimation { get; } = new()
    {
        AnimationSpeed = 100,
        TargetAnimation = 189
    };

    /// <inheritdoc />
    public KingOfCowsEnrageScript(Monster subject, IMonsterFactory monsterFactory, ISimpleCache simpleCache)
        : base(subject)
    {
        MonsterFactory = monsterFactory;
        SimpleCache = simpleCache;
    }

    /// <inheritdoc />
    public override void OnDeath()
    {
        var mileth = SimpleCache.Get<MapInstance>("mileth");
        
        foreach(var reactor in mileth.GetEntities<ReactorTile>().ToList())
            if (reactor.Script.Is<cowPortal>(out var script) && (script.Instance == Subject.MapInstance))
                mileth.RemoveEntity(reactor);
        
        base.OnDeath();
    }

    public override void Update(TimeSpan delta)
    {
        if (!Bonus75Applied && (Subject.StatSheet.HealthPercent <= 75))
        {
            Bonus75Applied = true;

            var attrib = new Attributes
            {
                Dmg = 10,
                MagicResistance = 15,
                SkillDamagePct = 10,
                SpellDamagePct = 10
            };
            var rectangle = new Rectangle(Subject, 5, 5);

            for (var i = 0; i <= 3; i++)
            {
                if (!rectangle.TryGetRandomPoint(x => Subject.MapInstance.IsWalkable(x, collisionType: Subject.Type), out var point))
                    continue;

                var mobs = MonsterFactory.Create("hell_bovine1", Subject.MapInstance, point);
                Subject.MapInstance.AddEntity(mobs, point);
            }

            Subject.StatSheet.AddBonus(attrib);
            Subject.Animate(UpgradeAnimation);
        }

        if (!Bonus50Applied && (Subject.StatSheet.HealthPercent <= 50))
        {
            Bonus50Applied = true;

            //Give Bonuses
            var attrib = new Attributes
            {
                AtkSpeedPct = 15
            };
            Subject.StatSheet.AddBonus(attrib);
            Subject.Animate(UpgradeAnimation);

            //Spawn Monsters
            var rectangle = new Rectangle(Subject, 5, 5);

            for (var i = 0; i <= 5; i++)
            {
                if (!rectangle.TryGetRandomPoint(x => Subject.MapInstance.IsWalkable(x, collisionType: Subject.Type), out var point))
                    continue;

                var mobs = MonsterFactory.Create("hell_bovine1", Subject.MapInstance, point);
                Subject.MapInstance.AddEntity(mobs, point);
            }
        }

        if (!Bonus30Applied && (Subject.StatSheet.HealthPercent <= 30))
        {
            Bonus30Applied = true;

            var attrib = new Attributes
            {
                Str = 5,
                Ac = 25,
                AtkSpeedPct = 15,
                Dmg = 5,
                FlatSkillDamage = 150,
                FlatSpellDamage = 150,
                Hit = 25,
                MagicResistance = 10
            };
            var rectangle = new Rectangle(Subject, 5, 5);

            for (var i = 0; i <= 7; i++)
            {
                if (!rectangle.TryGetRandomPoint(x => Subject.MapInstance.IsWalkable(x, collisionType: Subject.Type), out var point))
                    continue;

                var mobs = MonsterFactory.Create("hell_bovine1", Subject.MapInstance, point);
                Subject.MapInstance.AddEntity(mobs, point);
            }

            Subject.StatSheet.AddBonus(attrib);
            Subject.Animate(UpgradeAnimation);
        }
    }
}
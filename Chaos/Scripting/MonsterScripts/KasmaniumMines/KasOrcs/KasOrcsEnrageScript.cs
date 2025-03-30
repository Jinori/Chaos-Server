using Chaos.Extensions.Geometry;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Scripting.MonsterScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.MonsterScripts.KasmaniumMines.KasOrcs;

public sealed class KasOrcsEnrageScript : MonsterScriptBase
{
    private readonly IMonsterFactory MonsterFactory;
    private bool Bonus50Applied;

    private Animation UpgradeAnimation { get; } = new()
    {
        AnimationSpeed = 100,
        TargetAnimation = 189
    };

    /// <inheritdoc />
    public KasOrcsEnrageScript(Monster subject, IMonsterFactory monsterFactory)
        : base(subject)
        => MonsterFactory = monsterFactory;

    public override void Update(TimeSpan delta)
    {
        if (!Bonus50Applied && (Subject.StatSheet.HealthPercent <= 50))
        {
            Bonus50Applied = true;

            //Give Bonuses
            var attrib = new Attributes
            {
                AtkSpeedPct = 25,
                Dmg = 10
            };
            Subject.StatSheet.AddBonus(attrib);
            Subject.Animate(UpgradeAnimation);

            //Spawn Monsters
            var rectangle = new Rectangle(Subject, 5, 5);

            for (var i = 0; i <= 1; i++)
            {
                if (!rectangle.TryGetRandomPoint(x => Subject.MapInstance.IsWalkable(x, Subject.Type), out var point))
                    continue;

                var mobs = MonsterFactory.Create("km_hobgoblinSpawn", Subject.MapInstance, point);
                Subject.MapInstance.AddEntity(mobs, point);
            }
        }
    }
}
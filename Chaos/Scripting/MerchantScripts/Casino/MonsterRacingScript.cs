using Chaos.Common.Utilities;
using Chaos.Extensions;
using Chaos.Geometry.Abstractions.Definitions;
using Chaos.Models.World;
using Chaos.Scripting.MerchantScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.MerchantScripts.Casino;

public sealed class MonsterRacingScript : MerchantScriptBase
{
    private readonly IIntervalTimer MessageTimer;
    private readonly IMonsterFactory MonsterFactory;
    private bool CreatedMonsters;

    /// <inheritdoc />
    public MonsterRacingScript(Merchant subject, IMonsterFactory monsterFactory)
        : base(subject)
    {
        MonsterFactory = monsterFactory;

        MessageTimer = new PeriodicMessageTimer(
            TimeSpan.FromMinutes(1),
            TimeSpan.FromSeconds(15),
            TimeSpan.FromSeconds(10),
            TimeSpan.FromSeconds(1),
            "Monster Race will start in {Time}",
            subject.Say);

        var aislingsAtStart = Subject.MapInstance
                                     .GetEntities<Aisling>()
                                     .ToList();

        foreach (var aisling in aislingsAtStart)
            aisling.SendActiveMessage("A game of Monster Racing is beginning. Come join!");
    }

    public void CreateMonsters()
    {
        var racingStallMilk = new Point(2, 7);
        var racingStallSky = new Point(2, 6);
        var racingStallSand = new Point(2, 5);
        var racingStallWater = new Point(2, 4);
        var racingStallGrass = new Point(2, 3);
        var racingStallLava = new Point(2, 2);

        var monsterOne = MonsterFactory.Create("amusementMonster", Subject.MapInstance, racingStallMilk);
        monsterOne.Direction = Direction.Right;
        monsterOne.Sprite = (ushort)IntegerRandomizer.RollSingle(965);

        var monsterTwo = MonsterFactory.Create("amusementMonster", Subject.MapInstance, racingStallSky);
        monsterTwo.Direction = Direction.Right;
        monsterTwo.Sprite = (ushort)IntegerRandomizer.RollSingle(965);

        var monsterThree = MonsterFactory.Create("amusementMonster", Subject.MapInstance, racingStallSand);
        monsterThree.Direction = Direction.Right;
        monsterThree.Sprite = (ushort)IntegerRandomizer.RollSingle(965);

        var monsterFour = MonsterFactory.Create("amusementMonster", Subject.MapInstance, racingStallWater);
        monsterFour.Direction = Direction.Right;
        monsterFour.Sprite = (ushort)IntegerRandomizer.RollSingle(965);

        var monsterFive = MonsterFactory.Create("amusementMonster", Subject.MapInstance, racingStallGrass);
        monsterFive.Direction = Direction.Right;
        monsterFive.Sprite = (ushort)IntegerRandomizer.RollSingle(965);

        var monsterSix = MonsterFactory.Create("amusementMonster", Subject.MapInstance, racingStallLava);
        monsterSix.Direction = Direction.Right;
        monsterSix.Sprite = (ushort)IntegerRandomizer.RollSingle(965);

        Subject.MapInstance.AddEntity(monsterOne, racingStallMilk);
        Subject.MapInstance.AddEntity(monsterTwo, racingStallSky);
        Subject.MapInstance.AddEntity(monsterThree, racingStallSand);
        Subject.MapInstance.AddEntity(monsterFour, racingStallWater);
        Subject.MapInstance.AddEntity(monsterFive, racingStallGrass);
        Subject.MapInstance.AddEntity(monsterSix, racingStallLava);

        CreatedMonsters = true;
    }

    /// <inheritdoc />
    public override void Update(TimeSpan delta)
    {
        MessageTimer.Update(delta);

        if (MessageTimer.IntervalElapsed)
        {
            if (!CreatedMonsters)
                CreateMonsters();

            Subject.RemoveScript<MonsterRacingScript>();
        }
    }
}
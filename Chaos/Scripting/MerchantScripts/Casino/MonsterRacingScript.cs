using Chaos.Common.Utilities;
using Chaos.Extensions;
using Chaos.Geometry.Abstractions.Definitions;
using Chaos.Models.World;
using Chaos.Scripting.MerchantScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.MerchantScripts.Casino;

public class MonsterRacingScript : MerchantScriptBase
{
    private readonly IIntervalTimer MessageTimer;
    private readonly IMonsterFactory MonsterFactory;
    private bool CreatedMonsters;
    public string MonsterWon;

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

        var aislingsAtStart = Subject.MapInstance.GetEntities<Aisling>().ToList();

        foreach (var aisling in aislingsAtStart)
            aisling.SendActiveMessage("A game of Monster Racing is beginning. Come join!");
    }

    public void CreateMonsters()
    {
        var racingStallOne = new Point(1, 8);
        var racingStallTwo = new Point(1, 9);
        var racingStallThree = new Point(1, 10);
        var racingStallFour = new Point(1, 11);

        var monsterOne = MonsterFactory.Create("amusementMonster", Subject.MapInstance, racingStallOne);
        monsterOne.Direction = Direction.Right;
        monsterOne.Sprite = (ushort)IntegerRandomizer.RollSingle(965);
        Subject.MapInstance.AddObject(monsterOne, racingStallOne);

        var monsterTwo = MonsterFactory.Create("amusementMonster", Subject.MapInstance, racingStallTwo);
        monsterTwo.Direction = Direction.Right;
        monsterTwo.Sprite = (ushort)IntegerRandomizer.RollSingle(965);
        Subject.MapInstance.AddObject(monsterTwo, racingStallTwo);

        var monsterThree = MonsterFactory.Create("amusementMonster", Subject.MapInstance, racingStallThree);
        monsterThree.Direction = Direction.Right;
        monsterThree.Sprite = (ushort)IntegerRandomizer.RollSingle(965);
        Subject.MapInstance.AddObject(monsterThree, racingStallThree);

        var monsterFour = MonsterFactory.Create("amusementMonster", Subject.MapInstance, racingStallFour);
        monsterFour.Direction = Direction.Right;
        monsterFour.Sprite = (ushort)IntegerRandomizer.RollSingle(965);
        Subject.MapInstance.AddObject(monsterFour, racingStallFour);

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

            Subject.RemoveScript<IMerchantScript, MonsterRacingScript>();
        }
    }
}
using Chaos.Common.Definitions;
using Chaos.Models.World;
using Chaos.Scripting.ReactorTileScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.ReactorTileScripts;

public class TypingArenaTileScript : ReactorTileScriptBase
{
    private readonly Random Random = new();
    private readonly List<string> Words =
        ["chaos", "power", "battle", "magic", "spell", "defend", "attack", "speed", "fire", "ice"];
    private readonly IMonsterFactory MonsterFactory;
    private readonly IIntervalTimer SpawnTimer;

    /// <inheritdoc />
    public TypingArenaTileScript(ReactorTile subject, IMonsterFactory monsterFactory)
        : base(subject)
    {
        MonsterFactory = monsterFactory;
        SpawnTimer =
            new RandomizedIntervalTimer(TimeSpan.FromMilliseconds(3000), 10, RandomizationType.Balanced, false);
    }

    private string GetRandomWord()
    {
        var index = Random.Next(Words.Count);
        return Words[index];
    }

    public override void Update(TimeSpan delta)
    {
        SpawnTimer.Update(delta);

        if (!SpawnTimer.IntervalElapsed) 
            return;
        
        var randomMonster = MonsterFactory.Create("typingmonster", Subject.MapInstance, Subject);
        var randomWord = GetRandomWord();
        randomMonster.TypingWord = randomWord;
        randomMonster.Chant(randomWord);
        Subject.MapInstance.AddEntity(randomMonster, Subject);
    }
}
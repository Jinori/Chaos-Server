using Chaos.Collections;
using Chaos.Common.Definitions;
using Chaos.Models.World;
using Chaos.Scripting.MapScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.MapScripts.Events;

public class TgAntMapScript : MapScriptBase
{
    private const int SPAWN_INTERVAL_HOURS = 16;
    public const int UPDATE_INTERVAL_MS = 1000; // Increased for realistic update intervals

    private readonly IMonsterFactory MonsterFactory;
    private readonly IIntervalTimer? SpawnTimer;
    private readonly IIntervalTimer? UpdateTimer;
    private ScriptState State;

    public TgAntMapScript(MapInstance subject, IMonsterFactory monsterFactory)
        : base(subject)
    {
        MonsterFactory = monsterFactory;
        UpdateTimer = new IntervalTimer(TimeSpan.FromMilliseconds(UPDATE_INTERVAL_MS));
        SpawnTimer = new RandomizedIntervalTimer(TimeSpan.FromHours(SPAWN_INTERVAL_HOURS), 50, RandomizationType.Positive);
    }

    private void ClearAnts()
    {
        var monsters = Subject.GetEntities<Monster>()
                              .Where(x => !x.Template.Name.Contains("Ant"))
                              .ToList();

        if (monsters.Count > 0)
            NotifyAislings("You defeated the Ant Queen and the other ants escape!");

        foreach (var monster in monsters)
            Subject.RemoveEntity(monster);
    }

    private bool IsBossPresent()
        => Subject.GetEntities<Monster>()
                  .Any(x => x.Name == "Ant Queen");

    private void NotifyAislings(string message)
    {
        foreach (var aisling in Subject.GetEntities<Aisling>())
            aisling.Client.SendServerMessage(ServerMessageType.OrangeBar1, message);
    }

    private void SpawnAntBoss()
    {
        var monster = MonsterFactory.Create("tg_antboss", Subject, new Point(18, 25));
        Subject.AddEntity(monster, monster);
    }

    public override void Update(TimeSpan delta)
    {
        UpdateTimer?.Update(delta);
        SpawnTimer?.Update(delta);

        if (UpdateTimer!.IntervalElapsed)
            switch (State)
            {
                case ScriptState.Dormant:
                {
                    if (SpawnTimer!.IntervalElapsed)
                    {
                        State = ScriptState.Spawning;
                        NotifyAislings("The ants are coming for Thanksgiving!");
                    }
                }

                    break;

                case ScriptState.Spawning:
                {
                    SpawnAntBoss();
                    State = ScriptState.Spawned;
                }

                    break;

                case ScriptState.Spawned:
                {
                    if (!IsBossPresent())
                    {
                        ClearAnts();
                        State = ScriptState.Dormant;
                    }
                }

                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
    }

    private enum ScriptState
    {
        Dormant,
        Spawning,
        Spawned
    }
}
using Chaos.Collections;
using Chaos.Extensions;
using Chaos.Extensions.Common;
using Chaos.Extensions.Geometry;
using Chaos.Models.World;
using Chaos.Scripting.MonsterScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Services.Storage.Abstractions;
using Chaos.Storage.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Boss.Creants.Phoenix;

public class PhoenixPhaseScript : MonsterScriptBase
{
    private enum Phase
    {
        Normal,
        Abduct
    }
    
    private Phase CurrentPhase = Phase.Normal;
    
    #region Timers
    private readonly IIntervalTimer SpawnAddsTimer = new IntervalTimer(TimeSpan.FromMinutes(1));
    private readonly IIntervalTimer AbductMessageTimer = new IntervalTimer(TimeSpan.FromMinutes(1), false);
    private readonly IIntervalTimer AbductGraceTimer = new IntervalTimer(TimeSpan.FromSeconds(3), false);
    private readonly SequentialEventTimer AbductPhaseTimer;
    #endregion
    
    #region Services
    private readonly IMonsterFactory MonsterFactory;
    private readonly ISimpleCache SimpleCache;
    private readonly IShardGenerator ShardGenerator;
    #endregion
    
    #region Points
    private Location SkyLocation = new("phoenix_sky", 7, 7);
    #endregion

    private void SpawnAdds()
    {
        var spawnPoints = Subject.GenerateCardinalPoints()
                                 .WithDirectionBias(Subject.Direction)
                                 .Take(3);

        var spawns = spawnPoints.Select(spawnPoint => MonsterFactory.Create("phoenixWindElemental", Subject.MapInstance, spawnPoint))
                                .ToList();

        Subject.MapInstance.AddEntities(spawns);
    }
    
    /// <inheritdoc />
    public PhoenixPhaseScript(Monster subject, IMonsterFactory monsterFactory, ISimpleCache simpleCache, IShardGenerator shardGenerator)
        : base(subject)
    {
        MonsterFactory = monsterFactory;
        SimpleCache = simpleCache;
        ShardGenerator = shardGenerator;
        AbductPhaseTimer = new SequentialEventTimer(AbductMessageTimer, AbductGraceTimer);
    }

    /// <inheritdoc />
    public override void Update(TimeSpan delta)
    {
        if (!Map.LoadedFromInstanceId.ContainsI("sky"))
        {
            SpawnAddsTimer.Update(delta);

            if (SpawnAddsTimer.IntervalElapsed)
                SpawnAdds();
        }
        
        AbductPhaseTimer.Update(delta);

        if (AbductPhaseTimer.IntervalElapsed)
            if (AbductPhaseTimer.CurrentTimer == AbductMessageTimer)
                Subject.Say("Fly, fool!");
            else if (AbductPhaseTimer.CurrentTimer == AbductGraceTimer)
                CurrentPhase = Phase.Abduct;

        switch (CurrentPhase)
        {
            case Phase.Normal:
                break;
            case Phase.Abduct:
                if (Target is not null && Subject.WithinRange(Target, 1))
                {
                    var targetMap = ShardGenerator.CreateShardOfInstance(SkyLocation.Map);
                    Target.TraverseMap(targetMap, SkyLocation, true);
                    Subject.TraverseMap(targetMap, SkyLocation, true);
                    
                    CurrentPhase = Phase.Normal;
                }

                break;
        }

    }
}
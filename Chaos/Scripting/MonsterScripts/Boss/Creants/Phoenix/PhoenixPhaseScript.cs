using Chaos.Collections;
using Chaos.Common.Definitions;
using Chaos.Common.Utilities;
using Chaos.DarkAges.Definitions;
using Chaos.Extensions;
using Chaos.Extensions.Common;
using Chaos.Extensions.Geometry;
using Chaos.Models.World;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ApplyDamage;
using Chaos.Scripting.MapScripts.MainStoryLine.Creants.Phoenix;
using Chaos.Scripting.MonsterScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Services.Storage.Abstractions;
using Chaos.Storage.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Boss.Creants.Phoenix;

public class PhoenixPhaseScript : MonsterScriptBase
{
    public enum Phase
    {
        Normal,
        Abduct,
        Drop
    }
    
    public Phase CurrentPhase { get; set; } = Phase.Normal;
    private Aisling? AislingToDrop;
    private MapInstance? SkyShard;
    private readonly IApplyDamageScript ApplyDamageScript = ApplyNonAttackDamageScript.Create();
    
    #region Timers
    private readonly IIntervalTimer SpawnAddsTimer = new IntervalTimer(TimeSpan.FromMinutes(1), false);
    private readonly IIntervalTimer AbductMessageTimer = new IntervalTimer(TimeSpan.FromMinutes(1), false);
    private readonly IIntervalTimer AbductGraceTimer = new IntervalTimer(TimeSpan.FromSeconds(3), false);

    private readonly IIntervalTimer DropTimer = new RandomizedIntervalTimer(
        TimeSpan.FromSeconds(5),
        50,
        RandomizationType.Positive,
        false);

    private readonly IIntervalTimer FlyDownTimer = new IntervalTimer(TimeSpan.FromSeconds(2), false);
    private readonly SequentialEventTimer AbductPhaseTimer;
    private readonly SequentialEventTimer DropPhaseTimer;
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
                                 .Where(point => Map.IsWithinMap(point))
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
        DropPhaseTimer = new SequentialEventTimer(DropTimer, FlyDownTimer);
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
            {
                break;
            }
            case Phase.Abduct:
            {
                if (Target is not null && Subject.WithinRange(Target, 1))
                {
                    if (SkyShard is null || !SkyShard.IsRunning)
                        SkyShard = ShardGenerator.CreateShardOfInstance(SkyLocation.Map);
                    
                    //set up the dedicated shard script
                    var script = SkyShard.Script.As<PhoenixSkyDedicatedShardScript>()!;
                    script.Phoenix = Subject;
                    script.FromLocation = Location.From(Subject);
                    script.WhiteList.Add(Target.Name);
                    
                    Target.TraverseMap(SkyShard, SkyLocation, true);
                    Subject.TraverseMap(SkyShard, SkyLocation, true);

                    AislingToDrop = Target as Aisling;
                    CurrentPhase = Phase.Drop;
                }

                break;
            }
            case Phase.Drop:
            {
                DropPhaseTimer.Update(delta);
                
                if(DropPhaseTimer.IntervalElapsed)
                    if (DropPhaseTimer.CurrentTimer == DropTimer)
                    {
                        var script = SkyShard!.Script.As<PhoenixSkyDedicatedShardScript>()!;
                        var originalMap = SimpleCache.Get<MapInstance>(script.FromLocation.Map);
                        originalMap.TryGetRandomWalkablePoint(out var randomPoint, CreatureType.Aisling);
                        
                        AislingToDrop?.TraverseMap(originalMap, randomPoint!, true, onTraverse: () =>
                        {
                            var damage = MathEx.GetPercentOf<int>((int)AislingToDrop!.StatSheet.EffectiveMaximumHp, 75);

                            ApplyDamageScript.ApplyDamage(
                                Subject,
                                AislingToDrop,
                                this,
                                damage);
                            
                            script.WhiteList.Remove(AislingToDrop!.Name);
                            AislingToDrop = null;

                            return Task.CompletedTask;
                        });
                    }
                    else if (DropPhaseTimer.CurrentTimer == FlyDownTimer)
                    {
                        var script = SkyShard!.Script.As<PhoenixSkyDedicatedShardScript>()!;
                        var originalMap = SimpleCache.Get<MapInstance>(script.FromLocation.Map);
                        Subject.TraverseMap(originalMap, script.FromLocation, true);
                        
                        CurrentPhase = Phase.Normal;
                    }
                
                break;
            }

        }

    }
}
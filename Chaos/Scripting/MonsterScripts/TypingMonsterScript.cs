using Chaos.Extensions;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Pathfinding;
using Chaos.Pathfinding.Abstractions;
using Chaos.Scripting.MapScripts.Arena.Arena_Modules;
using Chaos.Scripting.MonsterScripts.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.MonsterScripts;

public class TypingMonsterScript : MonsterScriptBase
{
    /// <inheritdoc />
    public TypingMonsterScript(Monster subject)
        : base(subject)
    {
        
    }

    private readonly Animation TypingDeathAnimation = new()
    {
        AnimationSpeed = 100,
        TargetAnimation = 218,
        Priority = 80,
    };

    private readonly IIntervalTimer ChantTimer = new IntervalTimer(TimeSpan.FromMilliseconds(700), false);
    private IPathOptions Options => PathOptions.Default.ForCreatureType(Subject.Type) with
    {
        LimitRadius = null,
        IgnoreBlockingReactors = true
    };
    public HashSet<uint> UniqueTypers { get; } = [];

    public override void OnPublicMessage(Creature source, string message)
    {
        if ((source.MapInstance.LoadedFromInstanceId == "arena_typing") && source is Aisling aisling && string.Equals(
                message,
                Subject.TypingWord,
                StringComparison.Ordinal))
        {
            if (!UniqueTypers.Contains(aisling.Id))
            {
                UniqueTypers.Add(aisling.Id); 
                var hits = UniqueTypers.Count;

                switch (hits)
                {
                    case 1:
                        Subject.StatSheet.SetHealthPct(66);
                        Subject.ShowHealth();
                        break;
                    
                    case 2:
                        Subject.StatSheet.SetHealthPct(33);
                        Subject.ShowHealth();
                        break;

                    case 3:
                        Subject.MapInstance.ShowAnimation(TypingDeathAnimation.GetPointAnimation(Subject));
                        
                        foreach (var playerId in UniqueTypers)
                        {
                            var player = Subject.MapInstance.GetEntities<Aisling>().FirstOrDefault(x => x.Id == playerId);
                            player?.Trackers.Counters.AddOrIncrement("TypingMonsterKill");
                        }

                        Subject.MapInstance.RemoveEntity(Subject);
                        break;
                }
            }
        }
    }
    
    /// <inheritdoc />
    public override void Update(TimeSpan delta)
    {
        ChantTimer.Update(delta);
        
        if (ChantTimer.IntervalElapsed) 
            Subject.Chant(Subject.TypingWord);
        
        const double SPEED_INCREASE_PER_WAVE = 0.05;

        if (Subject.MapInstance.Script.Is<TypingArenaMapScript>(out var typingMapScript))
            delta *= 1 + typingMapScript.WaveCount * SPEED_INCREASE_PER_WAVE;
            
        
        Subject.MoveTimer.Update(delta);
        
        if (!ShouldMove) 
            return;
        
        Subject.Pathfind(new Point(10, 0), 1, Options);
    }
    
    public override void OnDeath() => Subject.MapInstance.RemoveEntity(Subject);
}
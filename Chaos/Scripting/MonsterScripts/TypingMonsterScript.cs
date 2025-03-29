using Chaos.Extensions.Common;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Pathfinding;
using Chaos.Pathfinding.Abstractions;
using Chaos.Scripting.MonsterScripts.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.MonsterScripts;

public class TypingMonsterScript : MonsterScriptBase
{
    private readonly IIntervalTimer MovementTimer;

    /// <inheritdoc />
    public TypingMonsterScript(Monster subject)
        : base(subject) =>
        MovementTimer = new IntervalTimer(GetMovementInterval());

    private readonly Animation TypingDeathAnimation = new()
    {
        AnimationSpeed = 100,
        TargetAnimation = 218,
        Priority = 80,
    };

    private IPathOptions Options => PathOptions.Default with
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
        MovementTimer.Update(delta);

        if (!MovementTimer.IntervalElapsed) 
            return;
        
        Subject.Pathfind(new Point(10, 0), 1, Options);
        Subject.Chant(Subject.TypingWord);
    }

    private TimeSpan GetMovementInterval() => TimeSpan.FromSeconds(Math.Max(0.5, 2.5 - (Subject.TypingWave * 0.1)));

    public override void OnDeath() => Subject.MapInstance.RemoveEntity(Subject);
}

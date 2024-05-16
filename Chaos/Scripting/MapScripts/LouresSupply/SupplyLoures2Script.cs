using Chaos.Collections;
using Chaos.Common.Definitions;
using Chaos.Definitions;
using Chaos.Extensions.Geometry;
using Chaos.Models.World;
using Chaos.Scripting.MapScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.MapScripts.LouresSupply;

public class SupplyLoures2Script : MapScriptBase
{
    private readonly IMonsterFactory MonsterFactory;
    private readonly TimeSpan StartDelay;
    private readonly TimeSpan SpawnDelay;
    private DateTime? StartTime;
    private ScriptState State;
    private readonly IIntervalTimer? UpdateTimer;
    private readonly ISimpleCache SimpleCache;
    private readonly IMerchantFactory MerchantFactory;
    private readonly IDialogFactory DialogFactory;
    public const int UPDATE_INTERVAL_MS = 1;

    public SupplyLoures2Script(MapInstance subject, IMonsterFactory monsterFactory, ISimpleCache simpleCache, IMerchantFactory merchantFactory, IDialogFactory dialogFactory)
        : base(subject)
    {
        MonsterFactory = monsterFactory;
        SimpleCache = simpleCache;
        MerchantFactory = merchantFactory;
        DialogFactory = dialogFactory;
        StartDelay = TimeSpan.FromSeconds(1);
        SpawnDelay = TimeSpan.FromSeconds(5);
        UpdateTimer = new IntervalTimer(TimeSpan.FromSeconds(UPDATE_INTERVAL_MS));
    }

    public override void Update(TimeSpan delta)
    {
        UpdateTimer?.Update(delta);

        if (UpdateTimer!.IntervalElapsed)
        {
            
            // Switch statement to determine the current state of the script
            switch (State)
            {
                case ScriptState.Dormant:
                {
                    foreach (var aisling in Subject.GetEntities<Aisling>().Where(x =>
                                 x.Trackers.Enums.HasValue(SupplyLouresStage.SawAssassin)))
                    {
                        aisling.Trackers.Enums.Set(SupplyLouresStage.StartedQuest);
                    }
                    
                    if (Subject.GetEntities<Aisling>().Any(x => x.Trackers.Enums.HasValue(SickChildStage.SickChildKilled) && x.Trackers.Enums.HasValue(SupplyLouresStage.StartedQuest) && x.UserStatSheet.Level > 40))
                        State = ScriptState.DelayedStart;
                }

                    break;
                // Delayed start state
                case ScriptState.DelayedStart:
                    // Set the start time if it is not already set
                    StartTime ??= DateTime.UtcNow;

                    // Check if the start delay has been exceeded
                    if (DateTime.UtcNow - StartTime > StartDelay)
                    {
                        // Reset the start time
                        StartTime = null;

                        foreach (var aisling in Subject.GetEntities<Aisling>())
                        {
                            var merchant = MerchantFactory.Create("louresassassin", Subject, aisling);
                            var dialog = DialogFactory.Create("louresassassin_stabyou", merchant);
                            dialog.Display(aisling);
                            aisling.Client.SendServerMessage(
                                ServerMessageType.OrangeBar1,
                                "You notice something's not right...");
                        }
                        // Set the state to spawning
                        State = ScriptState.Spawning;
                    }

                    break;
                // Spawning state
                case ScriptState.Spawning:
                {
                    StartTime ??= DateTime.UtcNow;

                    // Check if the start delay has been exceeded
                    if (DateTime.UtcNow - StartTime > SpawnDelay)
                    {
                        StartTime = null;
                     
                        foreach (var aisling in Subject.GetEntities<Aisling>().Where(x =>
                                     x.Trackers.Enums.HasValue(SickChildStage.SickChildKilled)))
                        {
                            aisling.Trackers.Enums.Set(SupplyLouresStage.SawAssassin);
                        }
                        
                        var player = Subject.GetEntities<Aisling>()
                            .FirstOrDefault(x => x.Trackers.Enums.HasValue(SupplyLouresStage.SawAssassin));
                        if (player != null)
                        {
                            var points = player.GenerateCardinalPoints()
                                .Where(x => Subject.IsWalkable(x, CreatureType.Normal))
                                .ToList();

                            var firstPoint = points.FirstOrDefault();
                            
                            var monster = MonsterFactory.Create("louressupply_assassin", Subject, firstPoint);

                            Subject.AddEntity(monster, monster); 
                        }
                        // Set the state to spawned
                        State = ScriptState.Spawned;
                        // Reset the animation index
                    }
                }

                    break;
                // Spawned state
                case ScriptState.Spawned:
                {
                    if (!Subject.GetEntities<Aisling>().Any(x =>
                            x.Trackers.Enums.HasValue(SupplyLouresStage.SawAssassin) || x.IsDead))
                    {
                        var monster = Subject.GetEntities<Monster>().FirstOrDefault(x => x.Name == "Assassin");

                        if (monster != null)
                        {
                            Subject.RemoveEntity(monster);
                        }
                        else
                        {
                            State = ScriptState.Dormant;
                        }
                    }
                }

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    private enum ScriptState
    {
        Dormant,
        DelayedStart,
        Spawning,
        Spawned
    }
}
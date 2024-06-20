using Chaos.Collections;
using Chaos.Common.Utilities;
using Chaos.Definitions;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.MapScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.MapScripts.MainStoryLine.EingrenManor3
{
    public class EingrenManor3MapScript : MapScriptBase
    {
        private readonly IMerchantFactory MerchantFactory;
        private readonly IReactorTileFactory ReactorTileFactory;
        private readonly IIntervalTimer DelayedStartTimer;
        private ScriptState2 State;
        private readonly IIntervalTimer UpdateTimer;
        public const int UPDATE_INTERVAL_MS = 1000;
        private Point? SummonerMerchantSpawnPoint; // Class-level variable to store the spawn point

        public EingrenManor3MapScript(MapInstance subject, IMerchantFactory merchantFactory, IReactorTileFactory reactorTileFactory)
            : base(subject)
        {
            ReactorTileFactory = reactorTileFactory;
            MerchantFactory = merchantFactory;
            DelayedStartTimer = new IntervalTimer(TimeSpan.FromSeconds(14), false);
            UpdateTimer = new IntervalTimer(TimeSpan.FromMilliseconds(UPDATE_INTERVAL_MS), false);
            SummonerMerchantSpawnPoint = null; // Initialize the spawn point variable
        }

        public override void OnEntered(Creature creature)
        {
            if (creature is not Aisling aisling)
                return;

            // Check if the player has a specific quest flag and the trial is not already in progress
            if (aisling.Trackers.Enums.HasValue(MainStoryEnums.SearchForSummoner) && State == ScriptState2.Dormant)
            {
                if (Subject.GetEntities<Merchant>().Any(x => x.Name == "mainstory_summonermerchant"))
                    return;

                var spawnPoints = new[]
                {
                    new Point(187, 154),
                    new Point(28, 184),
                    new Point(189, 29)
                };

                // Select a random spawn point and store it in the class-level variable
                var selectedPoint = spawnPoints.PickRandom();
                SummonerMerchantSpawnPoint = selectedPoint;
                
                var masterwerewolf = MerchantFactory.Create("mainstory_summonermerchant", Subject, selectedPoint);
                Subject.AddEntity(masterwerewolf, selectedPoint);
                StartTimers();
                State = ScriptState2.DelayedStart;
            }
        }

        public override void Update(TimeSpan delta)
        {
            UpdateTimer.Update(delta);
            DelayedStartTimer.Update(delta);

            if (UpdateTimer.IntervalElapsed)
            {
                if (!Subject.GetEntities<Aisling>().Any())
                    return;
                
                HandleScriptState2();
            }
        }

        private void HandleScriptState2()
        {
            switch (State)
            {
                case ScriptState2.Dormant:
                    if (Subject.GetEntities<Aisling>()
                        .Any(a => a.Trackers.Enums.HasValue(MainStoryEnums.SearchForSummoner)))
                    {
                        State = ScriptState2.DelayedStart;
                    }

                    break;

                case ScriptState2.DelayedStart:
                    if (DelayedStartTimer.IntervalElapsed)
                        State = ScriptState2.SpawningServant;

                    break;

                case ScriptState2.SpawningServant:
                    if (Subject.GetEntities<Monster>().Any(x => x.Name == "summoner_servant"))
                        State = ScriptState2.SpawnedServant;
                    
                    break;

                case ScriptState2.SpawnedServant:
                    if (Subject.GetEntities<Aisling>()
                        .Any(x => x.Trackers.Enums.HasValue(MainStoryEnums.DefeatedServant))) 
                        State = ScriptState2.CompletedServant;

                    break;

                case ScriptState2.CompletedServant:
                    HandleCompletedServant();
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void HandleCompletedServant()
        {
            if (!Subject.GetEntities<Aisling>().Any())
            {
                ClearMonsters();
                State = ScriptState2.Dormant;
            }
            else if (Subject.GetEntities<Aisling>().Any(x => x.Trackers.Enums.HasValue(MainStoryEnums.DefeatedServant)))
            {
                if (SummonerMerchantSpawnPoint.HasValue)
                {
                    var escapePoint = SummonerMerchantSpawnPoint.Value;
                    var reactortile = ReactorTileFactory.Create("eingrenescapePortal", Subject, escapePoint);
                    Subject.SimpleAdd(reactortile);

                    var aislings = Subject.GetEntities<Aisling>().Where(x => x.Trackers.Enums.HasValue(MainStoryEnums.DefeatedServant)).ToList();

                    foreach (var aisling in aislings)
                    {
                        aisling.SendOrangeBarMessage("A portal opens nearby that will take you to the first floor.");
                    }

                    State = ScriptState2.Dormant;
                }
            }
        }

        private void ClearMonsters()
        {
            foreach (var monster in Subject.GetEntities<Monster>())
            {
                Subject.RemoveEntity(monster);
            }
        }

        public void StartTimers()
        {
            State = ScriptState2.DelayedStart;
            UpdateTimer.Reset(); // Reset the update timer
        }
    }

    public enum ScriptState2
    {
        Dormant,
        DelayedStart,
        SpawningServant,
        SpawnedServant,
        CompletedServant
    }
}

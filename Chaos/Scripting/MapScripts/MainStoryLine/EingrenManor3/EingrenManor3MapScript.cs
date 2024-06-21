using Chaos.Collections;
using Chaos.Common.Utilities;
using Chaos.Definitions;
using Chaos.Extensions.Geometry;
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
        private readonly IReactorTileFactory ReactorTileFactory;
        private readonly IMerchantFactory MerchantFactory;
        private readonly IIntervalTimer DelayedStartTimer;
        private ScriptState State;
        private readonly IIntervalTimer UpdateTimer;
        public const int UPDATE_INTERVAL_MS = 1000;

        public EingrenManor3MapScript(MapInstance subject, IMerchantFactory merchantFactory, IReactorTileFactory reactorTileFactory)
            : base(subject)
        {
            MerchantFactory = merchantFactory;
            ReactorTileFactory = reactorTileFactory;
            DelayedStartTimer = new IntervalTimer(TimeSpan.FromSeconds(3), false);
            UpdateTimer = new IntervalTimer(TimeSpan.FromMilliseconds(UPDATE_INTERVAL_MS), false);
        }

        public override void OnEntered(Creature creature)
        {
            if (creature is not Aisling aisling)
                return;

            // Check if the player has a specific quest flag
            if (aisling.Trackers.Enums.HasValue(MainStoryEnums.Entered3rdFloor))
            {
                if (Subject.GetEntities<Merchant>().Any(x => x.Name == "Summoner Kades") || Subject.GetEntities<Monster>().Any(x => x.Name == "Morroc"))
                    return;

                var spawnPoints = new[]
                {
                    new Point(187, 154),
                    new Point(28, 184),
                    new Point(189, 29)
                };

                // Select a random spawn point and store it in the class-level variable
                var selectedPoint = spawnPoints.PickRandom();
                
                var summoner = MerchantFactory.Create("mainstory_summonermerchant", Subject, selectedPoint);
                Subject.AddEntity(summoner, selectedPoint);
                State = ScriptState.Dormant;
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
                case ScriptState.Dormant:
                    if (Subject.GetEntities<Aisling>()
                        .Any(a => a.Trackers.Enums.HasValue(MainStoryEnums.Entered3rdFloor)))
                    {
                        State = ScriptState.SpawningServant;
                    }

                    break;

                case ScriptState.SpawningServant:
                    if (Subject.GetEntities<Monster>().Any(x => x.Name == "Morroc"))
                        State = ScriptState.SpawnedServant;
                    
                    break;

                case ScriptState.SpawnedServant:
                    if (Subject.GetEntities<Aisling>()
                        .Any(x => x.Trackers.Enums.HasValue(MainStoryEnums.DefeatedServant))) 
                        State = ScriptState.CompletedServant;

                    break;

                case ScriptState.CompletedServant:
                    HandleCompletedServant();
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void HandleCompletedServant()
        {
            var aislingportal = Subject.GetEntities<Aisling>()
                .FirstOrDefault(x => x.Trackers.Enums.HasValue(MainStoryEnums.DefeatedServant));

            if (aislingportal == null) return;
            
                var portalSpawn = new Rectangle(aislingportal, 8, 8);
                var outline = portalSpawn.GetOutline().ToList();
                Point point;

                do
                    point = outline.PickRandom();
                while (!Subject.IsWalkable(point, aislingportal.Type));
                
                var reactortile = ReactorTileFactory.Create("eingrenescapePortal", Subject, Point.From(point));
                
                Subject.SimpleAdd(reactortile);

                var aislings = Subject.GetEntities<Aisling>().Where(x => x.Trackers.Enums.HasValue(MainStoryEnums.DefeatedServant)).ToList();

                foreach (var aisling in aislings)
                {
                    aisling.SendOrangeBarMessage("A portal opens nearby that will take you to the first floor.");
                }

                State = ScriptState.Dormant;
        }
    }

    public enum ScriptState
    {
        Dormant,
        SpawningServant,
        SpawnedServant,
        CompletedServant
    }
}

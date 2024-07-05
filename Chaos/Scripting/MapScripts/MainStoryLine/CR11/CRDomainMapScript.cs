using Chaos.Collections;
using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Extensions.Geometry;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.MapScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.MapScripts.MainStoryLine.CR11
{
    public class CRDomainMapScript : MapScriptBase
    {
        private readonly ISimpleCache SimpleCache;
        private readonly IMerchantFactory MerchantFactory;
        private readonly IIntervalTimer DelayedStartTimer;
        private ScriptState State;
        private readonly IIntervalTimer UpdateTimer;
        public const int UPDATE_INTERVAL_MS = 500;

        public CRDomainMapScript(MapInstance subject, IMerchantFactory merchantFactory, ISimpleCache simpleCache)
            : base(subject)
        {
            MerchantFactory = merchantFactory;
            SimpleCache = simpleCache;
            DelayedStartTimer = new IntervalTimer(TimeSpan.FromSeconds(3), false);
            UpdateTimer = new IntervalTimer(TimeSpan.FromMilliseconds(UPDATE_INTERVAL_MS), false);
        }

        public override void OnEntered(Creature creature)
        {
            if (creature is not Aisling aisling)
                return;

            if (aisling.IsGodModeEnabled())
                return;
            
            // Check if the player has a specific quest flag
            if (aisling.Trackers.Enums.HasValue(MainStoryEnums.SearchForSummoner2))
            {
                if (Subject.GetEntities<Merchant>().Any(x => x.Name == "Summoner Kades"))
                    return;

                var spawnPoints = new Point(15, 14);
                var summoner = MerchantFactory.Create("mainstory_summonermerchant2", Subject, spawnPoints);
                Subject.AddEntity(summoner, spawnPoints);
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
                        .Any(a => a.Trackers.Enums.HasValue(MainStoryEnums.SearchForSummoner2)))
                    {
                        State = ScriptState.PortAislings;
                    }

                    break;
                
                case ScriptState.PortAislings:
                {
                    foreach (var aisling in Subject.GetEntities<Aisling>().Where(x => x.IsAlive && !x.IsGodModeEnabled()).ToList())
                    {
                        var rect = new Rectangle(5, 5, 5, 5);
                        var point = rect.GetRandomPoint();
                        if (!rect.Contains(aisling))
                        {
                            aisling.WarpTo(point);
                            aisling.SendOrangeBarMessage("Summoner Kades powerful magic keeps you caged.");
                        }
                    }

                    if (Subject.GetEntities<Aisling>()
                        .Any(x => x.Trackers.Enums.HasValue(MainStoryEnums.StartedSummonerFight)))
                        State = ScriptState.SpawningSummoner;

                    break;
                }

                case ScriptState.SpawningSummoner:
                    if (Subject.GetEntities<Monster>().Any(x => x.Name == "Summoner Kades"))
                        State = ScriptState.SpawnedSummoner;
                    
                    break;

                case ScriptState.SpawnedSummoner:
                    if (Subject.GetEntities<Monster>().Any(x => x.Name != "Summoner Kades")) 
                        State = ScriptState.CompletedSummoner;

                    break;

                case ScriptState.CompletedSummoner:
                    HandleCompletedSummoner();
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void HandleCompletedSummoner()
        {
                var aislings = Subject.GetEntities<Aisling>().Where(x => x.Trackers.Enums.HasValue(MainStoryEnums.SpawnedCreants)).ToList();

                foreach (var aisling in aislings)
                {
                    var rect = new Rectangle(2, 36, 5, 5);
                    var point = rect.GetRandomPoint();
                    var map = SimpleCache.Get<MapInstance>("cr11");
                    aisling.Trackers.Enums.Set(MainStoryEnums.KilledSummoner);
                    aisling.SendOrangeBarMessage("Summoner Kades vanishes, the walls begin to crumble and you escape.");
                    aisling.TraverseMap(map, point);
                }

                State = ScriptState.Dormant;
        }
    }

    public enum ScriptState
    {
        Dormant,
        SpawningSummoner,
        SpawnedSummoner,
        PortAislings,
        CompletedSummoner
    }
}

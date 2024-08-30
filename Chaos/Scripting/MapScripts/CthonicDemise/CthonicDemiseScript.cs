using Chaos.Collections;
using Chaos.Extensions.Geometry;
using Chaos.Models.World;
using Chaos.Scripting.MapScripts.Abstractions;
using Chaos.Storage.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.MapScripts.CthonicDemise
{
    public class CthonicDemiseScript : MapScriptBase
    {
        private readonly IIntervalTimer? UpdateTimer;
        public const int UPDATE_INTERVAL_MS = 1;
        public bool BossSpawned;
        private int BossKillCount;

        private readonly ISimpleCache SimpleCache;

        public CthonicDemiseScript(MapInstance subject, ISimpleCache simpleCache)
            : base(subject)
        {
            SimpleCache = simpleCache;
            UpdateTimer = new IntervalTimer(TimeSpan.FromSeconds(UPDATE_INTERVAL_MS));
            BossKillCount = 0;
        }

        public override void Update(TimeSpan delta)
        {
            UpdateTimer?.Update(delta);

            if (UpdateTimer!.IntervalElapsed)
            {
                if (Subject.GetEntities<Monster>().Any(x => x.Template.TemplateKey.Contains("darkmaster")))
                {
                    BossSpawned = true;
                }

                if (BossSpawned)
                {
                    CheckBossKills();
                }
            }
        }

        private void CheckBossKills()
        {
            var bosses = Subject.GetEntities<Monster>().Where(x => x.Template.TemplateKey.Contains("darkmaster")).ToList();

            if (BossKillCount < 2)
            {
                // If a boss dies, increase the kill count
                foreach (var boss in bosses)
                {
                    if (!boss.IsAlive)
                    {
                        BossKillCount++;
                        Subject.RemoveEntity(boss);

                        if (BossKillCount >= 2)
                        {
                            TeleportPlayersAndReset();
                            break;
                        }
                    }
                }
            }
        }

        private void TeleportPlayersAndReset()
        {
            var players = Subject.GetEntities<Aisling>().ToList();
            var mapInstance = SimpleCache.Get<MapInstance>("cr11");
            var rect = new Rectangle(20, 20, 4, 4);
            var point = rect.GetRandomPoint();

            foreach (var player in players)
            {
                if (player.Inventory.Contains("Cthonic Bell"))
                    player.Inventory.RemoveByTemplateKey("cdbell");
                
                while (!mapInstance.IsWalkable(point, player.Type))
                    player.TraverseMap(mapInstance, point);
            }
        }
    }
}

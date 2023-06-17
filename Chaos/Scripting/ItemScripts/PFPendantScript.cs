using Chaos.Collections;
using Chaos.Common.Utilities;
using Chaos.Definitions;
using Chaos.Extensions.Common;
using Chaos.Extensions.Geometry;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.ItemScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.ItemScripts
{
    public class PFPendantScript : ItemScriptBase
    {
        private readonly IMonsterFactory _monsterFactory;
        private readonly ISimpleCache _simpleCache;

        public PFPendantScript(Item subject, ISimpleCache simpleCache, IMonsterFactory monsterFactory)
            : base(subject)
        {
            _simpleCache = simpleCache;
            _monsterFactory = monsterFactory;
        }

        private Animation GetPlayerAnimation() => new Animation { AnimationSpeed = 100, TargetAnimation = 160 };

        private Animation GetMonsterAnimation() => new Animation { AnimationSpeed = 100, TargetAnimation = 97 };
        
        public override void OnUse(Aisling source)
        {
            if (!CanSpawnMantis(source, out var mapInstance))
                return;

            SpawnMantis(source, mapInstance);
        }

        private bool CanSpawnMantis(Aisling source, out MapInstance mapInstance)
        {
            mapInstance = _simpleCache.Get<MapInstance>("PF_peak");
            source.Trackers.Enums.TryGetValue(out PFQuestStage stage);

            if (!source.IsAlive || (stage != PFQuestStage.FoundPendant) || !source.MapInstance.Name.EqualsI(mapInstance.Name))
                return false;

            var mantisIsSpawned = mapInstance.GetEntities<Monster>().Any(x => x.Template.TemplateKey.EqualsI("PF_giant_mantis"));

            if (mantisIsSpawned)
            {
                source.SendOrangeBarMessage("The Giant Mantis is already on the map.");
                return false;
            }

            return true;
        }

        private void SpawnMantis(Aisling source, MapInstance mapInstance)
        {
            var mantis = CreateMantis(mapInstance, source);
            var point = GetSpawnPoint(mantis, source);

            mantis.AggroRange = 12;
            mantis.Experience = 1000;
            mantis.SetLocation(point);

            source.MapInstance.AddObject(mantis, point);
            mantis.Animate(GetMonsterAnimation());

            foreach (var player in mapInstance.GetEntities<Aisling>())
            {
                player.SendOrangeBarMessage("You feel the ground shake, a quick shadow overcasts you");
                player.Animate(GetPlayerAnimation());
            }
        }

        private Monster CreateMantis(MapInstance mapInstance, Aisling source) =>
            _monsterFactory.Create("pf_giant_mantis", mapInstance, Point.From(source));

        private Point GetSpawnPoint(Creature mantis, MapEntity source)
        {
            var monsterSpawn = new Rectangle(source, 10, 10);
            var outline = monsterSpawn.GetOutline().ToList();
            Point point;

            do
                point = outline.PickRandom();
            while (!source.MapInstance.IsWalkable(point, mantis.Type));

            return point;
        }
    }
}
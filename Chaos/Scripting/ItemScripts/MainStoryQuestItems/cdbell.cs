using Chaos.Collections;
using Chaos.Common.Definitions;
using Chaos.Definitions;
using Chaos.Extensions.Common;
using Chaos.Extensions.Geometry;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Scripting.ItemScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.ItemScripts.MainStoryQuestItems
{
    public class Cdbell : ItemScriptBase
    {
        private readonly ISimpleCache SimpleCache;
        private readonly IMonsterFactory MonsterFactory;

        private bool JohnSpawned;
        private bool JaneSpawned;
        private bool RoySpawned;
        private bool RaySpawned;
        private bool MikeSpawned;
        private bool MarySpawned;
        private bool PhilSpawned;
        private bool PamSpawned;
        private bool WilliamSpawned;
        private bool WandaSpawned;

        public Cdbell(Item subject, ISimpleCache simpleCache,
            IMonsterFactory monsterFactory)
            : base(subject)
        {
            SimpleCache = simpleCache;
            MonsterFactory = monsterFactory;
        }

        private bool CanUseBell(Aisling source)
        {
            var mapInstance = SimpleCache.Get<MapInstance>("cthonic_demise");
            source.Trackers.Enums.TryGetValue(out MainstoryMasterEnums stage);

            if (!source.MapInstance.Name.EqualsI(mapInstance.Name))
            {
                source.Inventory.Remove(Subject.DisplayName);
                source.SendOrangeBarMessage("The bell disintegrates.");
                return false;
            }

            if (stage is not (MainstoryMasterEnums.StartedDungeon))
                return false;

            if (source.MapInstance.GetEntities<Monster>().Any(x => x.Template.TemplateKey.Contains("darkmaster")))
            {
                source.SendOrangeBarMessage("A leader is already summoned here.");
                return false;
            }

            if (!source.IsAlive)
                return false;

            return true;
        }

        private void Bell(Aisling source)
        {
            // Define two rectangles
            var rectangle1 =
                new Rectangle(13, 9, 14,
                    15); // Rectangle with top-left corner at (10,10) and bottom-right corner at (20,20)
            var rectangle2 =
                new Rectangle(151, 148, 21,
                    30); // Rectangle with top-left corner at (30,30) and bottom-right corner at (40,40)

            var rectangle3 =
                new Rectangle(174, 76, 29,
                    22);
            
            var rectangle4 =
                new Rectangle(165, 12, 18,
                    23);
            
            var rectangle5 =
                new Rectangle(6, 40, 19,
                    14);
            var rectangle6 =
                new Rectangle(112, 191, 21,
                    25);
            var rectangle7 =
                new Rectangle(3, 175, 20,
                    26);
            var rectangle8 =
                new Rectangle(5, 106, 30,
                    16);
            var rectangle9 =
                new Rectangle(54, 8, 21,
                    26);
            var rectangle10 =
                new Rectangle(30, 30, 40,
                    40);
            // Check which rectangle the player is in and spawn the corresponding monster
            if (Enumerable.Contains(rectangle1, source))
            {
                if (RaySpawned)
                {
                    source.SendOrangeBarMessage("There are no leaders here anymore.");
                    return;
                }
                RaySpawned = true;
                
                source.SendOrangeBarMessage("You ring the bell loudly.");
                var monsterSpawn = new Rectangle(source, 5, 5);
                var point1 = monsterSpawn.GetRandomPoint();
                RaySpawned = true;
                var monster1 = MonsterFactory.Create("darkmasterray", source.MapInstance, point1);

                while (!source.MapInstance.IsWalkable(point1, CreatureType.Normal))
                    source.MapInstance.AddEntity(monster1, point1);
            }
            else if (Enumerable.Contains(rectangle2, source))
            {
                if (RoySpawned)
                {
                    source.SendOrangeBarMessage("There are no leaders here anymore.");
                    return;
                }
                RoySpawned = true;
                source.SendOrangeBarMessage("You ring the bell loudly.");
                var monsterSpawn = new Rectangle(source, 5, 5);
                var point1 = monsterSpawn.GetRandomPoint();
                var monster1 = MonsterFactory.Create("darkmasterroy", source.MapInstance, point1);

                while (!source.MapInstance.IsWalkable(point1, CreatureType.Normal))
                    source.MapInstance.AddEntity(monster1, point1);
            }
            else if (Enumerable.Contains(rectangle3, source))
            {
                if (JohnSpawned)
                {
                    source.SendOrangeBarMessage("There are no leaders here anymore.");
                    return;
                }
                JohnSpawned = true;
                
                source.SendOrangeBarMessage("You ring the bell loudly.");
                var monsterSpawn = new Rectangle(source, 5, 5);
                var point1 = monsterSpawn.GetRandomPoint();
                var monster1 = MonsterFactory.Create("darkmasterjohn", source.MapInstance, point1);

                while (!source.MapInstance.IsWalkable(point1, CreatureType.Normal))
                    source.MapInstance.AddEntity(monster1, point1);
            }
            else if (Enumerable.Contains(rectangle4, source))
            {
                if (JaneSpawned)
                {
                    source.SendOrangeBarMessage("There are no leaders here anymore.");
                    return;
                }
                JaneSpawned = true;
                source.SendOrangeBarMessage("You ring the bell loudly.");
                var monsterSpawn = new Rectangle(source, 5, 5);
                var point1 = monsterSpawn.GetRandomPoint();
                var monster1 = MonsterFactory.Create("darkmasterjane", source.MapInstance, point1);

                while (!source.MapInstance.IsWalkable(point1, CreatureType.Normal))
                    source.MapInstance.AddEntity(monster1, point1);
            }
            else if (Enumerable.Contains(rectangle5, source))
            {
                if (MikeSpawned)
                {
                    source.SendOrangeBarMessage("There are no leaders here anymore.");
                    return;
                }
                MikeSpawned = true;
                source.SendOrangeBarMessage("You ring the bell loudly.");
                var monsterSpawn = new Rectangle(source, 5, 5);
                var point1 = monsterSpawn.GetRandomPoint();
                var monster1 = MonsterFactory.Create("darkmastermike", source.MapInstance, point1);

                while (!source.MapInstance.IsWalkable(point1, CreatureType.Normal))
                    source.MapInstance.AddEntity(monster1, point1);
            }
            else if (Enumerable.Contains(rectangle6, source))
            {
                if (MarySpawned)
                {
                    source.SendOrangeBarMessage("There are no leaders here anymore.");
                    return;
                }
                MarySpawned = true;
                source.SendOrangeBarMessage("You ring the bell loudly.");
                var monsterSpawn = new Rectangle(source, 5, 5);
                var point1 = monsterSpawn.GetRandomPoint();
                var monster1 = MonsterFactory.Create("darkmastermary", source.MapInstance, point1);

                while (!source.MapInstance.IsWalkable(point1, CreatureType.Normal))
                    source.MapInstance.AddEntity(monster1, point1);
            }
            else if (Enumerable.Contains(rectangle7, source))
            {
                if (PhilSpawned)
                {
                    source.SendOrangeBarMessage("There are no leaders here anymore.");
                    return;
                }
                PhilSpawned = true;
                source.SendOrangeBarMessage("You ring the bell loudly.");
                var monsterSpawn = new Rectangle(source, 5, 5);
                var point1 = monsterSpawn.GetRandomPoint();
                var monster1 = MonsterFactory.Create("darkmasterphil", source.MapInstance, point1);

                while (!source.MapInstance.IsWalkable(point1, CreatureType.Normal))
                    source.MapInstance.AddEntity(monster1, point1);
            }
            else if (Enumerable.Contains(rectangle8, source))
            {
                if (PamSpawned)
                {
                    source.SendOrangeBarMessage("There are no leaders here anymore.");
                    return;
                }
                PamSpawned = true;
                source.SendOrangeBarMessage("You ring the bell loudly.");
                var monsterSpawn = new Rectangle(source, 5, 5);
                var point1 = monsterSpawn.GetRandomPoint();
                var monster1 = MonsterFactory.Create("darkmasterpam", source.MapInstance, point1);

                while (!source.MapInstance.IsWalkable(point1, CreatureType.Normal))
                    source.MapInstance.AddEntity(monster1, point1);
            }
            else if (Enumerable.Contains(rectangle9, source))
            {
                if (WilliamSpawned)
                {
                    source.SendOrangeBarMessage("There are no leaders here anymore.");
                    return;
                }
                WilliamSpawned = true;
                source.SendOrangeBarMessage("You ring the bell loudly.");
                var monsterSpawn = new Rectangle(source, 5, 5);
                var point1 = monsterSpawn.GetRandomPoint();
                var monster1 = MonsterFactory.Create("darkmasterwilliam", source.MapInstance, point1);

                while (!source.MapInstance.IsWalkable(point1, CreatureType.Normal))
                    source.MapInstance.AddEntity(monster1, point1);
            }
            else if (Enumerable.Contains(rectangle10, source))
            {
                if (WandaSpawned)
                {
                    source.SendOrangeBarMessage("There are no leaders here anymore.");
                    return;
                }
                WandaSpawned = true;
                source.SendOrangeBarMessage("You ring the bell loudly.");
                var monsterSpawn = new Rectangle(source, 5, 5);
                var point1 = monsterSpawn.GetRandomPoint();
                var monster1 = MonsterFactory.Create("darkmasterwanda", source.MapInstance, point1);

                while (!source.MapInstance.IsWalkable(point1, CreatureType.Normal))
                    source.MapInstance.AddEntity(monster1, point1);
            }
            else
            {
                source.SendOrangeBarMessage("The bell echoes, but nothing happens.");
            }
        }

        public override void OnUse(Aisling source)
        {
            if (CanUseBell(source))
            {
                Bell(source);
            }
        }
    }
}
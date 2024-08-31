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

        private readonly Rectangle RayRectangle = new Rectangle(13, 9, 14, 15);
        private readonly Rectangle RoyRectangle = new Rectangle(151, 148, 21, 30);
        private readonly Rectangle JohnRectangle = new Rectangle(174, 76, 29, 22);
        private readonly Rectangle JaneRectangle = new Rectangle(165, 12, 18, 23);
        private readonly Rectangle MikeRectangle = new Rectangle(6, 40, 19, 14);
        private readonly Rectangle MaryRectangle = new Rectangle(112, 91, 21, 25);
        private readonly Rectangle PhilRectangle = new Rectangle(3, 175, 20, 26);
        private readonly Rectangle PamRectangle = new Rectangle(5, 106, 30, 16);
        private readonly Rectangle WilliamRectangle = new Rectangle(54, 8, 21, 26);
        private readonly Rectangle WandaRectangle = new Rectangle(71, 72, 25, 20);

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
                source.Inventory.RemoveQuantity(Subject.DisplayName, 1);
                source.SendOrangeBarMessage("The bell disintegrates.");
                return false;
            }

            if (stage is not (MainstoryMasterEnums.StartedDungeon))
            {
                source.SendOrangeBarMessage("Only an Aisling who is on the quest can ring the bell.");
                return false;
            }

            if (source.MapInstance.GetEntities<Monster>().Any(x => x.Template.Name.Contains("Dark Master")))
            {
                source.SendOrangeBarMessage("A leader is already somewhere in the map.");
                return false;
            }

            if (!source.IsAlive)
                return false;

            return true;
        }

        private void Bell(Aisling source)
        {
            var sourcePoint = new Point(source.X, source.Y);
            var allPlayersOnMap = GetAllPlayersOnMap(source);
            // Check which rectangle the player is in and spawn the corresponding monster
            if (RayRectangle.Contains(sourcePoint))
            {
                if (source.Trackers.Flags.HasFlag(CdDungeonBoss.Ray))
                {
                    source.SendOrangeBarMessage("The leader isn't here anymore.");
                    return;
                }

                foreach (var member in allPlayersOnMap)
                {
                    member.Trackers.Flags.AddFlag(CdDungeonBoss.Ray);
                }

                source.SendOrangeBarMessage("You ring the bell loudly then it breaks.");
                var monsterSpawn = new Rectangle(source, 5, 5);
                var point1 = monsterSpawn.GetRandomPoint();
                source.Inventory.RemoveQuantity(Subject.DisplayName, 1);
                var monster1 = MonsterFactory.Create("darkmasterray", source.MapInstance, point1);

                do
                {
                    point1 = monsterSpawn.GetRandomPoint();
                } while (!source.MapInstance.IsWalkable(point1, CreatureType.Normal));

                source.MapInstance.AddEntity(monster1, point1);
            }
            else if (RoyRectangle.Contains(sourcePoint))
            {
                if (source.Trackers.Flags.HasFlag(CdDungeonBoss.Roy))
                {
                    source.SendOrangeBarMessage("The leader isn't here anymore.");
                    return;
                }

                foreach (var member in allPlayersOnMap)
                {
                    member.Trackers.Flags.AddFlag(CdDungeonBoss.Roy);
                }

                source.Inventory.RemoveQuantity(Subject.DisplayName, 1);
                source.SendOrangeBarMessage("You ring the bell loudly then it breaks.");
                var monsterSpawn = new Rectangle(source, 5, 5);
                var point1 = monsterSpawn.GetRandomPoint();
                var monster1 = MonsterFactory.Create("darkmasterroy", source.MapInstance, point1);

                do
                {
                    point1 = monsterSpawn.GetRandomPoint();
                } while (!source.MapInstance.IsWalkable(point1, CreatureType.Normal));

                source.MapInstance.AddEntity(monster1, point1);
            }
            else if (JohnRectangle.Contains(sourcePoint))
            {
                if (source.Trackers.Flags.HasFlag(CdDungeonBoss.John))
                {
                    source.SendOrangeBarMessage("The leader isn't here anymore.");
                    return;
                }


                foreach (var member in allPlayersOnMap)
                {
                    member.Trackers.Flags.AddFlag(CdDungeonBoss.John);
                }

                source.SendOrangeBarMessage("You ring the bell loudly and then breaks.");
                source.Inventory.RemoveQuantity(Subject.DisplayName, 1);
                var monsterSpawn = new Rectangle(source, 5, 5);
                var point1 = monsterSpawn.GetRandomPoint();
                var monster1 = MonsterFactory.Create("darkmasterjohn", source.MapInstance, point1);

                do
                {
                    point1 = monsterSpawn.GetRandomPoint();
                } while (!source.MapInstance.IsWalkable(point1, CreatureType.Normal));

                source.MapInstance.AddEntity(monster1, point1);
            }
            else if (JaneRectangle.Contains(sourcePoint))
            {
                if (source.Trackers.Flags.HasFlag(CdDungeonBoss.Jane))
                {
                    source.SendOrangeBarMessage("The leader isn't here anymore.");
                    return;
                }


                foreach (var member in allPlayersOnMap)
                {
                    member.Trackers.Flags.AddFlag(CdDungeonBoss.Jane);
                }

                source.SendOrangeBarMessage("You ring the bell loudly then it breaks.");
                source.Inventory.RemoveQuantity(Subject.DisplayName, 1);
                var monsterSpawn = new Rectangle(source, 5, 5);
                var point1 = monsterSpawn.GetRandomPoint();
                var monster1 = MonsterFactory.Create("darkmasterjane", source.MapInstance, point1);

                do
                {
                    point1 = monsterSpawn.GetRandomPoint();
                } while (!source.MapInstance.IsWalkable(point1, CreatureType.Normal));

                source.MapInstance.AddEntity(monster1, point1);
            }
            else if (MikeRectangle.Contains(sourcePoint))
            {
                if (source.Trackers.Flags.HasFlag(CdDungeonBoss.Mike))
                {
                    source.SendOrangeBarMessage("The leader isn't here anymore.");
                    return;
                }


                foreach (var member in allPlayersOnMap)
                {
                    member.Trackers.Flags.AddFlag(CdDungeonBoss.Mike);
                }

                source.SendOrangeBarMessage("You ring the bell loudly then it breaks.");
                source.Inventory.RemoveQuantity(Subject.DisplayName, 1);
                var monsterSpawn = new Rectangle(source, 5, 5);
                var point1 = monsterSpawn.GetRandomPoint();
                var monster1 = MonsterFactory.Create("darkmastermike", source.MapInstance, point1);
                do
                {
                    point1 = monsterSpawn.GetRandomPoint();
                } while (!source.MapInstance.IsWalkable(point1, CreatureType.Normal));

                source.MapInstance.AddEntity(monster1, point1);
            }
            else if (MaryRectangle.Contains(sourcePoint))
            {
                if (source.Trackers.Flags.HasFlag(CdDungeonBoss.Mary))
                {
                    source.SendOrangeBarMessage("The leader isn't here anymore.");
                    return;
                }


                foreach (var member in allPlayersOnMap)
                {
                    member.Trackers.Flags.AddFlag(CdDungeonBoss.Mary);
                }

                source.SendOrangeBarMessage("You ring the bell loudly then it breaks.");
                source.Inventory.RemoveQuantity(Subject.DisplayName, 1);
                var monsterSpawn = new Rectangle(source, 5, 5);
                var point1 = monsterSpawn.GetRandomPoint();
                var monster1 = MonsterFactory.Create("darkmastermary", source.MapInstance, point1);
                do
                {
                    point1 = monsterSpawn.GetRandomPoint();
                } while (!source.MapInstance.IsWalkable(point1, CreatureType.Normal));

                source.MapInstance.AddEntity(monster1, point1);
            }
            else if (PhilRectangle.Contains(sourcePoint))
            {
                if (source.Trackers.Flags.HasFlag(CdDungeonBoss.Phil))
                {
                    source.SendOrangeBarMessage("The leader isn't here anymore.");
                    return;
                }


                foreach (var member in allPlayersOnMap)
                {
                    member.Trackers.Flags.AddFlag(CdDungeonBoss.Phil);
                }

                source.SendOrangeBarMessage("You ring the bell loudly then it breaks.");
                source.Inventory.RemoveQuantity(Subject.DisplayName, 1);
                var monsterSpawn = new Rectangle(source, 5, 5);
                var point1 = monsterSpawn.GetRandomPoint();
                var monster1 = MonsterFactory.Create("darkmasterphil", source.MapInstance, point1);
                do
                {
                    point1 = monsterSpawn.GetRandomPoint();
                } while (!source.MapInstance.IsWalkable(point1, CreatureType.Normal));

                source.MapInstance.AddEntity(monster1, point1);
            }
            else if (PamRectangle.Contains(sourcePoint))
            {
                if (source.Trackers.Flags.HasFlag(CdDungeonBoss.Pam))
                {
                    source.SendOrangeBarMessage("The leader isn't here anymore.");
                    return;
                }


                foreach (var member in allPlayersOnMap)
                {
                    member.Trackers.Flags.AddFlag(CdDungeonBoss.Pam);
                }

                source.SendOrangeBarMessage("You ring the bell loudly then it breaks.");
                source.Inventory.RemoveQuantity(Subject.DisplayName, 1);
                var monsterSpawn = new Rectangle(source, 5, 5);
                var point1 = monsterSpawn.GetRandomPoint();
                var monster1 = MonsterFactory.Create("darkmasterpam", source.MapInstance, point1);
                do
                {
                    point1 = monsterSpawn.GetRandomPoint();
                } while (!source.MapInstance.IsWalkable(point1, CreatureType.Normal));

                source.MapInstance.AddEntity(monster1, point1);
            }
            else if (WilliamRectangle.Contains(sourcePoint))
            {
                if (source.Trackers.Flags.HasFlag(CdDungeonBoss.William))
                {
                    source.SendOrangeBarMessage("The leader isn't here anymore.");
                    return;
                }


                foreach (var member in allPlayersOnMap)
                {
                    member.Trackers.Flags.AddFlag(CdDungeonBoss.William);
                }

                source.SendOrangeBarMessage("You ring the bell loudly then it breaks.");
                source.Inventory.RemoveQuantity(Subject.DisplayName, 1);
                var monsterSpawn = new Rectangle(source, 5, 5);
                var point1 = monsterSpawn.GetRandomPoint();
                var monster1 = MonsterFactory.Create("darkmasterwilliam", source.MapInstance, point1);
                do
                {
                    point1 = monsterSpawn.GetRandomPoint();
                } while (!source.MapInstance.IsWalkable(point1, CreatureType.Normal));

                source.MapInstance.AddEntity(monster1, point1);
            }
            else if (WandaRectangle.Contains(sourcePoint))
            {
                if (source.Trackers.Flags.HasFlag(CdDungeonBoss.Wanda))
                {
                    source.SendOrangeBarMessage("The leader isn't here anymore.");
                    return;
                }


                foreach (var member in allPlayersOnMap)
                {
                    member.Trackers.Flags.AddFlag(CdDungeonBoss.Wanda);
                }

                source.SendOrangeBarMessage("You ring the bell loudly then it breaks.");
                source.Inventory.RemoveQuantity(Subject.DisplayName, 1);
                var monsterSpawn = new Rectangle(source, 5, 5);
                var point1 = monsterSpawn.GetRandomPoint();
                var monster1 = MonsterFactory.Create("darkmasterwanda", source.MapInstance, point1);
                do
                {
                    point1 = monsterSpawn.GetRandomPoint();
                } while (!source.MapInstance.IsWalkable(point1, CreatureType.Normal));

                source.MapInstance.AddEntity(monster1, point1);
            }
            else
            {
                source.SendOrangeBarMessage("The bell echoes, but nothing happens.");
            }
        }

        private List<Aisling> GetAllPlayersOnMap(Aisling source) =>
            source.MapInstance.GetEntities<Aisling>().ToList();

        public override void OnUse(Aisling source)
        {
            if (CanUseBell(source))
            {
                Bell(source);
            }
        }
    }
}
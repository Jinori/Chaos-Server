using Chaos.Collections;
using Chaos.Geometry.Abstractions;
using Chaos.Scripting.MapScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using System;
using System.Collections.Generic;
using Chaos.Common.Definitions;
using Chaos.Definitions;
using Chaos.Models.World;
using Chaos.Networking.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.MapScripts
{
    public class EventMapScript : MapScriptBase
    {
        private readonly IReactorTileFactory ReactorTileFactory;
        private readonly IMonsterFactory MonsterFactory;
        private readonly List<string> MonsterTemplates;
        private readonly IIntervalTimer UpdateTimer;
        private readonly IClientRegistry<IChaosWorldClient> ClientRegistry;

        public EventMapScript(MapInstance subject, IReactorTileFactory reactorTileFactory, IMonsterFactory monsterFactory, IClientRegistry<IChaosWorldClient> clientRegistry)
            : base(subject)
        {
            ReactorTileFactory = reactorTileFactory;
            MonsterFactory = monsterFactory;
            ClientRegistry = clientRegistry;
            UpdateTimer = new IntervalTimer(TimeSpan.FromMilliseconds(500), false);

            // Initialize the list with monster template keys
            MonsterTemplates = new List<string>
            {
                "gargoyle_event",
                "MonsterTemplateKey2",
                "MonsterTemplateKey3"
                // Add more monster template keys as needed
            };
        }

        private void CreateRandomMonster(Point point)
        {
            var random = new Random();
            // Get a random monster template key from the list
            var randomMonsterTemplateKey = MonsterTemplates[random.Next(MonsterTemplates.Count)];

            // Use the MonsterFactory to create a monster at the specified point
            var monster = MonsterFactory.Create(randomMonsterTemplateKey, Subject, point);
            Subject.AddEntity(monster, point);
        }

        private Point GenerateRandomPointWithinRectangle(IRectangle rectangle)
        {
            var random = new Random();

            Point randomPoint;
            bool isValidPoint;

            do
            {
                // Generate random X and Y coordinates within the rectangle
                var randomX = random.Next(rectangle.Left, rectangle.Right);
                var randomY = random.Next(rectangle.Top, rectangle.Bottom);

                randomPoint = new Point(randomX, randomY);

                // Check if the random point is not on a wall or blocking reactor
                isValidPoint = !Subject.IsWall(randomPoint) && !Subject.IsBlockingReactor(randomPoint);

            } while (!isValidPoint);

            return randomPoint;
        }

        public override void Update(TimeSpan delta)
        {
            UpdateTimer.Update(delta);

            if (!UpdateTimer.IntervalElapsed)
                return;

            var gmaisling = Subject.GetEntities<Aisling>().FirstOrDefault(x =>
                x.Trackers.Enums.HasValue(GodMode.Yes) && x.Trackers.Enums.HasValue(StartEvent.StartEvent));

            if (gmaisling != null)
            {
                // Get the boundary rectangle of the map
                var mapBoundary = Subject.Template.Bounds;

                // Generate a random point within the map's boundary
                var randomPoint = GenerateRandomPointWithinRectangle(mapBoundary);
                // Spawn a random monster at the generated random point
                CreateRandomMonster(randomPoint);
                
                foreach (var client in ClientRegistry)
                    client.SendServerMessage(ServerMessageType.OrangeBar1, "You feel the ground beginning to shake...");
                
                gmaisling.Trackers.Enums.Set(StartEvent.Off);
            }
        }
    }
}

﻿using Chaos.Collections;
using Chaos.Common.Utilities;
using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Extensions.Geometry;
using Chaos.Models.World;
using Chaos.Scripting.MapScripts.Abstractions;
using Chaos.Scripting.ReactorTileScripts.MainStoryQuest;
using Chaos.Services.Factories.Abstractions;
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
        
        private readonly IReactorTileFactory ReactorTileFactory;

        public CthonicDemiseScript(MapInstance subject,
            IReactorTileFactory reactorTileFactory)
            : base(subject)
        {
            ReactorTileFactory = reactorTileFactory;
            UpdateTimer = new IntervalTimer(TimeSpan.FromSeconds(UPDATE_INTERVAL_MS));
            BossKillCount = 0;
        }

        public override void Update(TimeSpan delta)
        {
            UpdateTimer?.Update(delta);

            if (UpdateTimer!.IntervalElapsed)
            {
                // Check if any bosses are on the map
                var bosses = Subject.GetEntities<Monster>().Where(x => x.Template.TemplateKey.Contains("darkmaster"))
                    .ToList();

                if (bosses.Any())
                {
                    BossSpawned = true;
                }
                else if (BossSpawned)
                {
                    // No bosses are present and a boss was previously spawned, so increment the kill count
                    BossSpawned = false;
                    BossKillCount++;

                    if (BossKillCount >= 2)
                    {
                        OpenEscapePortal();
                    }
                }
            }
        }

        private void OpenEscapePortal()
        {
            var aislingportal = Subject.GetEntities<Aisling>()
                .FirstOrDefault(x =>
                    x.Trackers.Enums.HasValue(MainstoryMasterEnums.StartedDungeon) ||
                    x.Trackers.Flags.HasFlag(MainstoryFlags.FinishedDungeon));

            if (aislingportal == null) return;

            // Check if there's already an escape portal open
            if (Subject.GetEntities<ReactorTile>().Any(x => x.Script.Is<CthonicDemiseEscapeScript>()))
                return;

            var portalSpawn = new Rectangle(aislingportal, 6, 6);
            var outline = portalSpawn.GetOutline().ToList();
            Point point;

            do
                point = outline.PickRandom();
            while (!Subject.IsWalkable(point, aislingportal.Type));

            var reactortile = ReactorTileFactory.Create("cdescapeportal", Subject, Point.From(point));

            Subject.SimpleAdd(reactortile);

            var aislings = Subject.GetEntities<Aisling>().Where(x =>
                x.Trackers.Enums.HasValue(MainstoryMasterEnums.StartedDungeon) ||
                x.Trackers.Flags.HasFlag(MainstoryFlags.FinishedDungeon)).ToList();

            foreach (var aisling in aislings)
            {
                aisling.SendOrangeBarMessage("A portal opens nearby to Cthonic Remains 11 to rest.");
            }
        }
    }
}
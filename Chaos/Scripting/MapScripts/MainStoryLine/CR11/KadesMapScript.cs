using Chaos.Collections;
using Chaos.Common.Utilities;
using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.MapScripts.Abstractions;
using Chaos.Scripting.MonsterScripts.Pet;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.MapScripts.MainStoryLine.CR11;

public class KadesMapScript : MapScriptBase
{
    private readonly IMonsterFactory MonsterFactory;
    private readonly ISimpleCache SimpleCache;
    private readonly IIntervalTimer SpawnTimer;
    private readonly IIntervalTimer UpdateTimer;
    private bool LastFight;

    public KadesMapScript(MapInstance subject, IMonsterFactory monsterFactory, ISimpleCache simpleCache)
        : base(subject)
    {
        MonsterFactory = monsterFactory;
        SimpleCache = simpleCache;
        SpawnTimer = new IntervalTimer(TimeSpan.FromSeconds(45), false);
        UpdateTimer = new IntervalTimer(TimeSpan.FromMilliseconds(500), false);
    }

    public override void OnEntered(Creature creature)
    {
        if (creature is not Aisling aisling)
            return;

        if (aisling.IsGodModeEnabled())
            return;

        if (Subject.GetEntities<Monster>()
                   .Any(x => !x.Script.Is<PetScript>()))
            return;

        var spawnPoints = new Point(5, 5);
        var summoner = MonsterFactory.Create("summoner_boss", Subject, spawnPoints);

        switch (Subject.Template.MapId)
        {
            case 31002:
                summoner.StatSheet.SetHealthPct(80);
                summoner.StatSheet.SetManaPct(80);
                spawnPoints = new Point(8, 6);
                Subject.AddEntity(summoner, spawnPoints);

                break;
            case 31004:
                summoner.StatSheet.SetHealthPct(60);
                summoner.StatSheet.SetManaPct(60);
                spawnPoints = new Point(8, 6);
                Subject.AddEntity(summoner, spawnPoints);

                break;
            case 31003:
                summoner.StatSheet.SetHealthPct(40);
                summoner.StatSheet.SetManaPct(40);
                spawnPoints = new Point(12, 9);
                Subject.AddEntity(summoner, spawnPoints);

                break;
            case 31001:
                summoner.StatSheet.SetHealthPct(20);
                summoner.StatSheet.SetManaPct(20);
                spawnPoints = new Point(6, 8);
                Subject.AddEntity(summoner, spawnPoints);

                break;
            case 22100:
                summoner.StatSheet.SetHealthPct(15);
                summoner.StatSheet.SetManaPct(15);
                spawnPoints = new Point(17, 10);
                Subject.AddEntity(summoner, spawnPoints);
                LastFight = true;

                break;
        }
    }

    public override void Update(TimeSpan delta)
    {
        UpdateTimer.Update(delta);
        SpawnTimer.Update(delta);

        if (!UpdateTimer.IntervalElapsed)
            return;

        if (!LastFight)
            return;

        if (!Subject.GetEntities<Monster>()
                    .Any(x => x.Name == "Summoner Kades"))
        {
            foreach (var monster in Subject.GetEntities<Monster>())
                Subject.RemoveEntity(monster);

            foreach (var player in Subject.GetEntities<Aisling>()
                                          .Where(x => !x.IsGodModeEnabled()))
            {
                if (player.Trackers.Enums.HasValue(MainStoryEnums.StartedSummonerFight))
                    player.Trackers.Enums.Set(MainStoryEnums.KilledSummoner);

                player.SendOrangeBarMessage("Summoner Kades vanishes...");
                player.Trackers.Enums.Remove<SummonerBossFight>();

                var point = new Point(player.X, player.Y);
                var map = SimpleCache.Get<MapInstance>("cthonic_domain");
                player.TraverseMap(map, point);
            }
        }

        var summoner = Subject.GetEntities<Monster>()
                              .FirstOrDefault(x => x.Name == "Summoner Kades");

        if (summoner == null)
            return;

        if (SpawnTimer.IntervalElapsed)
        {
            var point = new Point(summoner.X + 2, summoner.Y + 2);
            var roll = IntegerRandomizer.RollSingle(12);

            if (roll <= 3)
            {
                var monstersummon = MonsterFactory.Create("gale_guardian", Subject, point);
                Subject.AddEntity(monstersummon, point);

                return;
            }

            if (roll <= 6)
            {
                var monstersummon = MonsterFactory.Create("terra_guardian", Subject, point);
                Subject.AddEntity(monstersummon, point);

                return;
            }

            if (roll <= 9)
            {
                var monstersummon = MonsterFactory.Create("tide_guardian", Subject, point);
                Subject.AddEntity(monstersummon, point);

                return;
            }

            if (roll <= 12)
            {
                var monstersummon = MonsterFactory.Create("ignis_guardian", Subject, point);
                Subject.AddEntity(monstersummon, point);

                return;
            }
        }

        var monsters = Subject.GetEntities<Monster>()
                              .Where(
                                  x => x.Template.TemplateKey is "gale_guardian" or "ignis_guardian" or "tide_guardian" or "terra_guardian")
                              .ToList();

        if (monsters.Count == 0)
            return;

        if (summoner.StatSheet.HealthPercent >= 3)
            return;

        summoner.Say("Give me your life summon!");
        Subject.RemoveEntity(monsters.First());
        summoner.StatSheet.SetHealthPct(15);
    }
}
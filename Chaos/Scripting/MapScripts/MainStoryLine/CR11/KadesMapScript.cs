using Chaos.Collections;
using Chaos.Common.Definitions;
using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Extensions.Geometry;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.EffectScripts.Priest;
using Chaos.Scripting.MapScripts.Abstractions;
using Chaos.Scripting.MerchantScripts.Mainstory.Summoner;
using Chaos.Scripting.MonsterScripts.Pet;
using Chaos.Services.Factories;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.MapScripts.MainStoryLine.CR11
{
    public class KadesMapScript : MapScriptBase
    {
        private readonly IMonsterFactory MonsterFactory;

        public KadesMapScript(MapInstance subject,
            IMonsterFactory monsterFactory)
            : base(subject)
        {
            MonsterFactory = monsterFactory;
        }

        public override void OnEntered(Creature creature)
        {
            if (creature is not Aisling aisling)
                return;

            if (aisling.IsGodModeEnabled())
                return;

            if (Subject.GetEntities<Monster>().Any(x => !x.Script.Is<PetScript>()))
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
            }
        }
    }
}

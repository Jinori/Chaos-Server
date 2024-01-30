using Chaos.Collections;
using Chaos.Common.Definitions;
using Chaos.Definitions;
using Chaos.Models.Legend;
using Chaos.Models.World;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ExperienceDistribution;
using Chaos.Scripting.MonsterScripts.Abstractions;
using Chaos.Storage.Abstractions;
using Chaos.Time;

namespace Chaos.Scripting.MonsterScripts;

// ReSharper disable once ClassCanBeSealed.Global
public class SlaveDeathScript : MonsterScriptBase
{
    private readonly ISimpleCache SimpleCache;

    /// <inheritdoc />
    public SlaveDeathScript(Monster subject, ISimpleCache simpleCache)
        : base(subject)
    {
        ExperienceDistributionScript = DefaultExperienceDistributionScript.Create();
        SimpleCache = simpleCache;
    }

    protected IExperienceDistributionScript ExperienceDistributionScript { get; set; }

    /// <inheritdoc />
    public override void OnDeath()
    {
        if (!Map.RemoveEntity(Subject))
            return;

        var players = Map.GetEntities<Aisling>().Where(x => x.IsAlive);

        foreach (var player in players)
        {
            var mapInstance = SimpleCache.Get<MapInstance>("mileth_inn");
            var pointS = new Point(5, 7);

            player.TraverseMap(mapInstance, pointS);
            player.StatSheet.AddHp(1);
            player.Trackers.Enums.Set(NightmareQuestStage.CompletedNightmareLoss1);
            player.Client.SendAttributes(StatUpdateType.Vitality);
            player.SendOrangeBarMessage("You have been defeated by your Nightmares");
            player.Legend.AddOrAccumulate(
                new LegendMark(
                    "Succumbed to their Nightmares.",
                    "Nightmare",
                    MarkIcon.Victory,
                    MarkColor.White,
                    1,
                    GameTime.Now));

            player.Refresh(true);
        }
    }
}
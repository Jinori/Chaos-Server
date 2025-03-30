using Chaos.Collections;
using Chaos.DarkAges.Definitions;
using Chaos.Definitions;
using Chaos.Extensions.Geometry;
using Chaos.Models.World;
using Chaos.Scripting.MapScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.MapScripts.PietWerewolf;

public class PietWerewolfMapScript : MapScriptBase
{
    private readonly IMerchantFactory MerchantFactory;
    private readonly IIntervalTimer UpdateTimer;

    public PietWerewolfMapScript(MapInstance subject, IMerchantFactory merchantFactory)
        : base(subject)
    {
        MerchantFactory = merchantFactory;
        UpdateTimer = new IntervalTimer(TimeSpan.FromSeconds(1), false);
    }

    private Point GenerateSpawnPoint(MapInstance selectedMap)
    {
        Point point;

        do
            point = selectedMap.Template.Bounds.GetRandomPoint();
        while (selectedMap.IsWall(point) || selectedMap.IsBlockingReactor(point));

        return point;
    }

    public override void Update(TimeSpan delta)
    {
        UpdateTimer.Update(delta);

        if (UpdateTimer.IntervalElapsed)
        {
            var werewolf = Subject.GetEntities<Merchant>()
                                  .Any(x => x.Template.TemplateKey == "werewolfToby");

            if (werewolf)
                return;

            if (!Subject.GetEntities<Aisling>()
                        .Any(x => x.Trackers.Enums.HasValue(WerewolfOfPiet.StartedQuest)))
                return;

            if (Subject.CurrentLightLevel != LightLevel.Darkest_A)
                return;

            var merchant = MerchantFactory.Create("werewolfToby", Subject, new Point(5, 10));
            var points = GenerateSpawnPoint(Subject);

            Subject.AddEntity(merchant, points);
        }
    }
}
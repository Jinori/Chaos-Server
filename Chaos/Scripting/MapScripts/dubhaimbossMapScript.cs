using Chaos.Collections;
using Chaos.Models.World;
using Chaos.Scripting.MapScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.MapScripts;

public class DubhaimbossMapScript(MapInstance subject, IMonsterFactory monsterFactory) : MapScriptBase(subject)
{
    private readonly IIntervalTimer? UpdateTimer = new IntervalTimer(TimeSpan.FromSeconds(UPDATE_INTERVAL_MS));
    private readonly IIntervalTimer? BossTimer = new IntervalTimer(TimeSpan.FromMinutes(BOSS_TIMER));
    public const int UPDATE_INTERVAL_MS = 1;
    public const int BOSS_TIMER = 30;

    public override void Update(TimeSpan delta)
    {
        UpdateTimer?.Update(delta);
        BossTimer?.Update(delta);

        if (!UpdateTimer!.IntervalElapsed) return;
        if (!BossTimer!.IntervalElapsed) return;

        var aislingCount = Subject.GetEntities<Aisling>().Count();

        if (!Subject.TryGetRandomWalkablePoint(out var point1))
            BossTimer.Reset();

        if (Subject.GetEntities<Monster>().Any(x => x.Name == "Lord Gargoyle"))
            return;
        
        // Check if there are 4 or more aislings
        if (aislingCount < 3) return;

        if (point1 != null)
        {
            var dubhaimBoss = monsterFactory.Create("dc_basement_gargoyleboss", Subject, point1);
            
            Subject.AddEntity(dubhaimBoss, point1);
        }
    }
}
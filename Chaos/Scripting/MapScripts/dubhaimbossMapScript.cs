using Chaos.Collections;
using Chaos.Models.World;
using Chaos.Scripting.MapScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.MapScripts;

public class dubhaimbossMapScript : MapScriptBase
{
    private readonly IMonsterFactory MonsterFactory;
    private readonly IIntervalTimer? UpdateTimer;
    private readonly IIntervalTimer? BossTimer;
    public const int UPDATE_INTERVAL_MS = 1;
    public const int BOSS_TIMER = 1;

    public dubhaimbossMapScript(MapInstance subject, IMonsterFactory monsterFactory)
        : base(subject)
    {
        MonsterFactory = monsterFactory;
        UpdateTimer = new IntervalTimer(TimeSpan.FromSeconds(UPDATE_INTERVAL_MS));
        BossTimer = new IntervalTimer(TimeSpan.FromHours(BOSS_TIMER));
    }

    public override void Update(TimeSpan delta)
    {
        UpdateTimer?.Update(delta);
        BossTimer?.Update(delta);

        if (!UpdateTimer!.IntervalElapsed) return;
        if (!BossTimer!.IntervalElapsed) return;
        var aislingCount = Subject.GetEntities<Aisling>().Count();
        var point = Subject.GetRandomWalkablePoint();

        if (Subject.GetEntities<Monster>().Any(x => x.Name == "Lord Gargoyle"))
            return;
        
        // Check if there are 4 or more aislings
        if (aislingCount < 4) return;
        
        var dubhaimBoss = MonsterFactory.Create("dc_basement_gargoyleboss", Subject, point);
            
        Subject.AddEntity(dubhaimBoss, point);
    }
}
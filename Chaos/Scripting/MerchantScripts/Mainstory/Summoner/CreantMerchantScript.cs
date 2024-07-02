using Chaos.Common.Definitions;
using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Models.Data;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.MerchantScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.MerchantScripts.Mainstory.Summoner;

public class CreantMerchantScript : MerchantScriptBase
{
    private readonly IIntervalTimer ActionTimer;
    private readonly IIntervalTimer DialogueTimer;
    private readonly IIntervalTimer WalkTimer;
    private readonly IIntervalTimer PortalClose;
    private readonly IMonsterFactory MonsterFactory;
    private readonly IMerchantFactory MerchantFactory;
    private readonly IReactorTileFactory ReactorTileFactory;

    /// <inheritdoc />
    public CreantMerchantScript(Merchant subject, IMonsterFactory monsterFactory, IReactorTileFactory reactorTileFactory, IMerchantFactory merchantFactory)
        : base(subject)
    {
        MonsterFactory = monsterFactory;
        ReactorTileFactory = reactorTileFactory;
        MerchantFactory = merchantFactory;
        PortalClose = new IntervalTimer(TimeSpan.FromSeconds(2), false);
        DialogueTimer = new RandomizedIntervalTimer(
            TimeSpan.FromSeconds(3),
            20,
            RandomizationType.Positive,
            false);
        WalkTimer = new IntervalTimer(TimeSpan.FromMilliseconds(800), false);
        ActionTimer = new IntervalTimer(TimeSpan.FromMilliseconds(200), false);
    }

    public override void Update(TimeSpan delta)
    {
        ActionTimer.Update(delta);

        if (!ActionTimer.IntervalElapsed)
            return;
        
        switch (Subject.SummonerState2)
        {
        }
    }
}
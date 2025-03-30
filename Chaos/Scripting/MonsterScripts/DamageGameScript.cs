using Chaos.Collections;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.MonsterScripts.Abstractions;
using Chaos.Storage.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.MonsterScripts;

public class DamageGameScript : MonsterScriptBase
{
    private readonly IIntervalTimer CountDownTimer;
    public readonly IStorage<DamageGameObj> DamageLeaderboard;
    private readonly ISimpleCache SimpleCache;

    private int DamageDone;
    private DateTime DPSTime;
    private bool GameStarted;

    public DamageGameScript(Monster subject, ISimpleCache simpleCache, IStorage<DamageGameObj> damageLeaderboard)
        : base(subject)
    {
        SimpleCache = simpleCache;
        DamageLeaderboard = damageLeaderboard;

        CountDownTimer = new PeriodicMessageTimer(
            TimeSpan.FromMinutes(1),
            TimeSpan.FromSeconds(15),
            TimeSpan.FromSeconds(10),
            TimeSpan.FromSeconds(1),
            "{Time}",
            subject.Say);
    }

    private int CalculateDPS()
    {
        var fightDuration = DateTime.UtcNow - DPSTime;
        var elapsedSeconds = fightDuration.TotalSeconds;

        return (int)(DamageDone / elapsedSeconds);
    }

    public override void OnAttacked(Creature source, int damage)
    {
        if (!GameStarted)
        {
            GameStarted = true;
            DPSTime = DateTime.UtcNow;
        }

        DamageDone += damage;
    }

    public override void Update(TimeSpan delta)
    {
        if (GameStarted)
        {
            CountDownTimer.Update(delta);

            if (CountDownTimer.IntervalElapsed)
            {
                var player = Subject.MapInstance
                                    .GetEntities<Aisling>()
                                    .FirstOrDefault();

                if (player != null)
                {
                    DamageLeaderboard.Value.AddOrUpdateEntry(
                        player,
                        DamageDone,
                        CalculateDPS(),
                        player.UserStatSheet.BaseClass,
                        player.UserStatSheet.BaseClass);
                    DamageLeaderboard.Save();
                    var mapInstance = SimpleCache.Get<MapInstance>("hm_road");
                    player.TraverseMap(mapInstance, new Point(4, 6));
                    GameStarted = false;
                    Subject.MapInstance.RemoveEntity(Subject);
                }
            }
        }
    }
}
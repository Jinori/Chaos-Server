using Chaos.Collections;
using Chaos.Common.Definitions;
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
    private readonly ISimpleCache SimpleCache;
    private int DamageDone;
    private bool GameStart;

    /// <inheritdoc />
    public DamageGameScript(Monster subject, ISimpleCache simpleCache)
        : base(subject)
    {
        CountDownTimer = new PeriodicMessageTimer(
            TimeSpan.FromMinutes(1),
            TimeSpan.FromSeconds(15),
            TimeSpan.FromSeconds(10),
            TimeSpan.FromSeconds(1),
            "{Time}",
            subject.Say);

        SimpleCache = simpleCache;
    }

    /// <inheritdoc />
    public override void OnAttacked(Creature source, int damage)
    {
        if (!GameStart)
            GameStart = true;

        DamageDone += damage;
    }

    /// <inheritdoc />
    public override void Update(TimeSpan delta)
    {
        if (GameStart)
        {
            CountDownTimer.Update(delta);

            if (CountDownTimer.IntervalElapsed)
            {
                var player = Subject.MapInstance.GetEntities<Aisling>().First();
                Subject.MapInstance.RemoveObject(Subject);
                player.SendServerMessage(ServerMessageType.Whisper, $"{DamageDone} damage in a minute!");
                var mapInstance = SimpleCache.Get<MapInstance>("hm_road");
                player.TraverseMap(mapInstance, new Point(4, 6));
            }
        }
    }
}
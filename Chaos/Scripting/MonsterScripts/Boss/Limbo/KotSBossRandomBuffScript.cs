using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Scripting.MonsterScripts.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Boss.Limbo;

public sealed class KotSBossRandomBuffScript : MonsterScriptBase
{
    private const float BUFF_INTERVAL = 20f; // Gets a new buff every 20 seconds
    private const float BUFF_DURATION = 10f; // Buff lasts for 10 seconds
    private readonly Random Random = new();
    private Attributes? CurrentBuff;
    private bool IsBuffActive;
    private IIntervalTimer BuffDurationTimer { get; }
    private IIntervalTimer BuffTimer { get; }

    public KotSBossRandomBuffScript(Monster subject)
        : base(subject)
    {
        BuffTimer = new IntervalTimer(TimeSpan.FromSeconds(BUFF_INTERVAL));
        BuffDurationTimer = new IntervalTimer(TimeSpan.FromSeconds(BUFF_DURATION));
    }

    private void ActivateBuff()
    {
        IsBuffActive = true;
        BuffDurationTimer.Reset();
        var buffType = Random.Next(1, 4); // Randomly pick a buff type

        switch (buffType)
        {
            case 1:
                CurrentBuff = new Attributes
                {
                    Dmg = 30
                };
                Subject.Say("I feel stronger!");

                break;
            case 2:
                CurrentBuff = new Attributes
                {
                    AtkSpeedPct = 40
                };
                Subject.Say("I am faster now!");

                break;
            case 3:
                CurrentBuff = new Attributes
                {
                    MagicResistance = 50
                };
                Subject.Say("My magic resist grows!");

                break;
            case 4:
                CurrentBuff = new Attributes
                {
                    Ac = -40
                };
                Subject.Say("My armor class increased!");

                break;
        }

        if (CurrentBuff != null)
            Subject.StatSheet.AddBonus(CurrentBuff);

        // Additional visual or sound effects for buff activation
    }

    private void DeactivateBuff()
    {
        IsBuffActive = false;
        BuffTimer.Reset();

        if (CurrentBuff != null)
        {
            Subject.StatSheet.SubtractBonus(CurrentBuff);
            CurrentBuff = null;
        }

        Subject.Say("My buff has worn off!");

        // Additional visual or sound effects for buff deactivation
    }

    public override void Update(TimeSpan delta)
    {
        base.Update(delta);
        BuffTimer.Update(delta);

        if (IsBuffActive)
        {
            BuffDurationTimer.Update(delta);

            if (BuffDurationTimer.IntervalElapsed)
                DeactivateBuff();
        } else if (BuffTimer.IntervalElapsed)
            ActivateBuff();
    }
}
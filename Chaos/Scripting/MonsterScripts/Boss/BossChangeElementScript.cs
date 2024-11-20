using Chaos.Common.Definitions;
using Chaos.DarkAges.Definitions;
using Chaos.Models.World;
using Chaos.Scripting.MonsterScripts.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Boss;

public sealed class BossElementalCycleScript : MonsterScriptBase
{
    private IIntervalTimer ElementCycleTimer { get; }
    private Element CurrentElement;
    private const float CYCLE_DURATION = 15f; // Change element every 15 seconds

    public BossElementalCycleScript(Monster subject)
        : base(subject)
    {
        ElementCycleTimer = new IntervalTimer(TimeSpan.FromSeconds(CYCLE_DURATION));
        CurrentElement = Element.Fire; // Start with fire element
    }

    public override void Update(TimeSpan delta)
    {
        base.Update(delta);
        ElementCycleTimer.Update(delta);

        if (ElementCycleTimer.IntervalElapsed)
            CycleElement();
    }

    private void CycleElement()
    {
        CurrentElement = CurrentElement switch
        {
            Element.Fire => Element.Water,
            Element.Water => Element.Earth,
            Element.Earth => Element.Wind,
            Element.Wind => Element.Fire,
            _ => Element.Fire,
        };

        Subject.Say($"I am now imbued with the power of {CurrentElement}!");
        // Additional visual effects for element change

        UpdateElementalResistance();
    }

    private void UpdateElementalResistance()
    {
        // Reset resistances
        Subject.StatSheet.SetDefenseElement(CurrentElement);
        Subject.StatSheet.SetOffenseElement(CurrentElement);
    }
}

using Chaos.DarkAges.Definitions;
using Chaos.Models.World;
using Chaos.Scripting.MonsterScripts.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Boss;

public sealed class BossElementalCycleScript : MonsterScriptBase
{
    private const float CYCLE_DURATION = 15f; // Change element every 15 seconds
    private Element CurrentElement;
    private IIntervalTimer ElementCycleTimer { get; }

    public BossElementalCycleScript(Monster subject)
        : base(subject)
    {
        ElementCycleTimer = new IntervalTimer(TimeSpan.FromSeconds(CYCLE_DURATION));
        CurrentElement = Element.Fire; // Start with fire element
    }

    private void CycleElement()
    {
        CurrentElement = CurrentElement switch
        {
            Element.Fire  => Element.Water,
            Element.Water => Element.Earth,
            Element.Earth => Element.Wind,
            Element.Wind  => Element.Fire,
            _             => Element.Fire
        };

        Subject.Say($"I am now imbued with the power of {CurrentElement}!");

        // Additional visual effects for element change

        UpdateElementalResistance();
    }

    public override void Update(TimeSpan delta)
    {
        base.Update(delta);
        ElementCycleTimer.Update(delta);

        if (ElementCycleTimer.IntervalElapsed)
            CycleElement();
    }

    private void UpdateElementalResistance()
    {
        // Reset resistances
        Subject.StatSheet.SetDefenseElement(CurrentElement);
        Subject.StatSheet.SetOffenseElement(CurrentElement);
    }
}
using Chaos.Common.Definitions;
using Chaos.DarkAges.Definitions;
using Chaos.Models.World;
using Chaos.Scripting.MonsterScripts.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Boss.Limbo;

public sealed class KotSBossElementalCycleScript : MonsterScriptBase
{
    private const float CYCLE_DURATION = 15f; // Change element every 15 seconds
    private Element CurrentElement;
    private IIntervalTimer ElementCycleTimer { get; }

    public KotSBossElementalCycleScript(Monster subject)
        : base(subject)
    {
        ElementCycleTimer = new IntervalTimer(TimeSpan.FromSeconds(CYCLE_DURATION));
        CurrentElement = Element.Darkness; // Start with dark element
    }

    private void CycleElement()
    {
        CurrentElement = CurrentElement switch
        {
            Element.Darkness => Element.Holy,
            Element.Holy     => Element.Darkness,
            _                => Element.Holy
        };

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
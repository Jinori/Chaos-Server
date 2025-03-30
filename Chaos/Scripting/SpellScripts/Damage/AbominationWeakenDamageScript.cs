using Chaos.DarkAges.Definitions;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Scripting.SpellScripts.Abstractions;

namespace Chaos.Scripting.SpellScripts.Damage;

public class AbominationWeakenDamageScript : ConfigurableSpellScriptBase
{
    protected Animation Animation { get; } = new()
    {
        AnimationSpeed = 75,
        TargetAnimation = 377
    };

    /// <inheritdoc />
    public AbominationWeakenDamageScript(Spell subject)
        : base(subject) { }

    /// <inheritdoc />
    public override void OnUse(SpellContext context)
    {
        foreach (var aisling in context.Source.MapInstance.GetEntities<Aisling>())
        {
            if (aisling.Inventory.Contains("Charm"))
                return;

            aisling.StatSheet.SubtractHealthPct(50);
            aisling.Client.SendAttributes(StatUpdateType.Vitality);
            aisling.SendOrangeBarMessage("The intense cold weakens your body.");
            aisling.Animate(Animation, aisling.Id);
        }
    }
}
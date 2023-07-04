using Chaos.Common.Definitions;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Scripting.SpellScripts.Abstractions;

namespace Chaos.Scripting.SpellScripts.Damage;

public class WeakenDamageScript : ConfigurableSpellScriptBase
{
    protected Animation Animation { get; } = new()
    {
        AnimationSpeed = 75,
        TargetAnimation = 2
    };

    /// <inheritdoc />
    public WeakenDamageScript(Spell subject)
        : base(subject) { }

    /// <inheritdoc />
    public override void OnUse(SpellContext context)
    {
        foreach (var aisling in context.Source.MapInstance.GetEntities<Aisling>())
        {
            if (aisling.Inventory.Contains("Silver Wolf Leather"))
                return;

            aisling.StatSheet.SetHp(1);
            aisling.Client.SendAttributes(StatUpdateType.Vitality);
            aisling.SendOrangeBarMessage("Mantis weakens your body.");
            aisling.Animate(Animation, aisling.Id);
        }
    }
}
using Chaos.Common.Definitions;
using Chaos.Data;
using Chaos.Objects.Panel;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripting.Components;
using Chaos.Scripting.SpellScripts.Abstractions;

namespace Chaos.Scripting.SpellScripts.Healing;

public class AoCradhScript : BasicSpellScriptBase, ManaCostComponent.IManaCostComponentOptions
{
    protected ManaCostComponent ManaCostComponent { get; }

    public AoCradhScript(Spell subject)
        : base(subject) => ManaCostComponent = new ManaCostComponent();

    public override void OnUse(SpellContext context)
    {
        if (CradhNameToRemove is not null)
        {
            if (!ManaCostComponent.TryApplyManaCost(context, this))
                return;

            var targets = AbilityComponent.Activate<Creature>(context, this);

            foreach (var target in targets.TargetEntities)
            {
                target.Effects.Dispel(CradhNameToRemove);
                context.TargetAisling?.Client.SendAttributes(StatUpdateType.Full);
            }
        }
    }

    #region ScriptVars
    protected string? CradhNameToRemove { get; init; }
    public int? ManaCost { get; init; }
    public decimal PctManaCost { get; init; }
    #endregion
}
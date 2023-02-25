using Chaos.Data;
using Chaos.Objects.Panel;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripting.SpellScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.SpellScripts;

public class LayReactorTileScript : BasicSpellScriptBase, ManaCostComponent.IManaCostComponentOptions
{
    protected IReactorTileFactory ReactorTileFactory { get; set; }
    protected ManaCostComponent ManaCostComponent { get; }

    /// <inheritdoc />
    public LayReactorTileScript(Spell subject, IReactorTileFactory reactorTileFactory)
        : base(subject)
    {
        ReactorTileFactory = reactorTileFactory;
        ManaCostComponent = new ManaCostComponent();
    }

    /// <inheritdoc />
    public override void OnUse(SpellContext context)
    {
        if (!ManaCostComponent.TryApplyManaCost(context, this))
            return;

        var targets = AbilityComponent.Activate<Creature>(context, this);

        foreach (var point in targets.TargetPoints)
        {
            var trap = ReactorTileFactory.Create(
                ReactorTileTemplateKey,
                context.Map,
                point,
                owner: context.Target);

            context.Map.SimpleAdd(trap);
        }
    }

    #region ScriptVars
    protected string ReactorTileTemplateKey { get; init; } = null!;
    public int? ManaCost { get; init; }
    public decimal PctManaCost { get; init; }
    #endregion
}
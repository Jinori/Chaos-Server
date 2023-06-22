using Chaos.Common.Definitions;
using Chaos.Definitions;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Components;
using Chaos.Scripting.Components.Utilities;
using Chaos.Scripting.ItemScripts.Abstractions;
using Chaos.Services.Factories;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.ItemScripts;

public class AmnesiaBrewScript : ConfigurableItemScriptBase,
                                 ConsumableComponent.IConsumableComponentOptions,
                                 AbilityComponent<Creature>.IAbilityComponentOptions,
                                 DropAggroComponent.IDropAggroComponentOptions
{

    public AmnesiaBrewScript(Item subject)
        : base(subject) { }

    public override void OnUse(Aisling source) =>
        new ComponentExecutor(source, source)
            .WithOptions(this)
            .ExecuteAndCheck<AbilityComponent<Creature>>()
            ?
            .Execute<ConsumableComponent>()
            .Execute<DropAggroComponent>();

    public bool ExcludeSourcePoint { get; init; }
    public TargetFilter Filter { get; init; }
    public bool MustHaveTargets { get; init; }
    public int Range { get; init; }
    public AoeShape Shape { get; init; }
    public byte? Sound { get; init; }
    public BodyAnimation BodyAnimation { get; init; }
    public bool AnimatePoints { get; init; }
    public Animation? Animation { get; init; }
    public int? ManaCost { get; init; }
    public decimal PctManaCost { get; init; }
    public bool ShouldNotBreakHide { get; init; }
    public bool AllAggro { get; set; }
    public string ItemName { get; init; } = null!;
}
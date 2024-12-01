using Chaos.DarkAges.Definitions;
using Chaos.Models.Data;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Components.EffectComponents;
using Chaos.Scripting.Components.Execution;
using Chaos.Scripting.EffectScripts.Abstractions;

namespace Chaos.Scripting.EffectScripts.Items.CookingMeals;

public class PieAppleEffect : EffectBase, NonOverwritableEffectComponent.INonOverwritableEffectComponentOptions
{
    public List<string> ConflictingEffectNames { get; init; } =
        [
            "Dinner Plate",
            "Sweet Buns",
            "Fruit Basket",
            "Lobster Dinner",
            "Pie Acorn",
            "Pie Apple",
            "Pie Cherry",
            "Pie Grape",
            "Pie Greengrapes",
            "Pie Strawberry",
            "Pie Tangerines",
            "Salad",
            "Sandwich",
            "Soup",
            "Steak Meal"
        ];

    protected override TimeSpan Duration { get; set; } = TimeSpan.FromMinutes(15);

    protected Animation? Animation { get; } = new()
    {
        TargetAnimation = 127,
        AnimationSpeed = 100
    };

    public override byte Icon => 72;
    public override string Name => "Pie Apple";
    protected byte? Sound => 115;

    public override void OnApplied()
    {
        base.OnApplied();

        var attributes = new Attributes
        {
            MaximumHp = 500
        };

        Subject.StatSheet.AddBonus(attributes);
        AislingSubject?.Client.SendAttributes(StatUpdateType.Full);

        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You feel the strength filling your body.");
    }

    public override void OnDispelled() => OnTerminated();

    public override void OnTerminated()
    {
        var attributes = new Attributes
        {
            MaximumHp = 500
        };

        Subject.StatSheet.SubtractBonus(attributes);
        AislingSubject?.Client.SendAttributes(StatUpdateType.Full);
        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You feel the comforting bolstering of your health recede.");
    }

    public override bool ShouldApply(Creature source, Creature target)
    {
        var execution = new ComponentExecutor(source, target).WithOptions(this)
                                                             .ExecuteAndCheck<NonOverwritableEffectComponent>();

        return execution is not null;
    }
}
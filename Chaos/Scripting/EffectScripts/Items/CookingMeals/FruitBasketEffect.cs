using Chaos.Common.Definitions;
using Chaos.Models.Data;
using Chaos.Scripting.EffectScripts.Abstractions;

namespace Chaos.Scripting.EffectScripts.Items.CookingMeals;

public class FruitBasketEffect : NonOverwritableEffectBase
{
    protected override TimeSpan Duration { get; set; } = TimeSpan.FromMinutes(15);
    protected override Animation? Animation { get; } = new()
    {
        TargetAnimation = 127,
        AnimationSpeed = 100
    };
    protected override IReadOnlyCollection<string> ConflictingEffectNames { get; } = new[]
    {
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
    };
    public override byte Icon => 72;
    public override string Name => "Fruit Basket";
    protected override byte? Sound => 115;

    public override void OnApplied()
    {
        base.OnApplied();

        var attributes = new Attributes
        {
            Hit = 8
        };

        Subject.StatSheet.AddBonus(attributes);
        AislingSubject?.Client.SendAttributes(StatUpdateType.Full);
        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Your mind experiences clarity.");
    }

    public override void OnDispelled() => OnTerminated();

    public override void OnTerminated()
    {
        var attributes = new Attributes
        {
            Hit = 8
        };

        Subject.StatSheet.SubtractBonus(attributes);
        AislingSubject?.Client.SendAttributes(StatUpdateType.Full);
        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Your mind returns to normal.");
    }
}
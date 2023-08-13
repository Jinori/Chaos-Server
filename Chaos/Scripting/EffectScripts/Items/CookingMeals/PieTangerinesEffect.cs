using Chaos.Common.Definitions;
using Chaos.Models.Data;
using Chaos.Scripting.EffectScripts.Abstractions;

namespace Chaos.Scripting.EffectScripts.Items.CookingMeals;

public class PieTangerinesEffect : NonOverwritableEffectBase
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
        "Acorn Pie",
        "Apple Pie",
        "Cherry Pie",
        "Grape Pie",
        "Greengrapes Pie",
        "Strawberry Pie",
        "Tangerines Pie",
        "Salad",
        "Sandwich",
        "Soup",
        "Steak Meal"
    };
    public override byte Icon => 72;
    public override string Name => "Tangerines Pie";
    protected override byte? Sound => 115;

    public override void OnApplied()
    {
        base.OnApplied();

        var attributes = new Attributes
        {
            MagicResistance = 20
        };

        Subject.StatSheet.AddBonus(attributes);
        AislingSubject?.Client.SendAttributes(StatUpdateType.Full);

        AislingSubject?.Client.SendServerMessage(
            ServerMessageType.OrangeBar1,
            "You gain a special sense of magical abilities.");
    }

    public override void OnDispelled() => OnTerminated();

    public override void OnTerminated()
    {
        var attributes = new Attributes
        {
            MagicResistance = 20
        };

        Subject.StatSheet.SubtractBonus(attributes);
        AislingSubject?.Client.SendAttributes(StatUpdateType.Full);
        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You've lost your special sense of magical abilities.");
    }
}
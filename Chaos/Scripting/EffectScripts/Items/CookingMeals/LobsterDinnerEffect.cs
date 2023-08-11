using Chaos.Common.Definitions;
using Chaos.Models.Data;
using Chaos.Scripting.EffectScripts.Abstractions;

namespace Chaos.Scripting.EffectScripts.Items.CookingMeals;

public class LobsterDinnerEffect : NonOverwritableEffectBase
{
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
        "Steak Meal",
    };

    protected override TimeSpan Duration { get; set; } = TimeSpan.FromMinutes(15);
    public override byte Icon => 72;
    public override string Name => "Lobster Dinner";
    protected override byte? Sound => 115;

    public override void OnApplied()
    {
        base.OnApplied();

        var attributes = new Attributes
        {
            MaximumHp = 1000,
            MaximumMp = 1000,
            Ac = -6
        };

        Subject.StatSheet.AddBonus(attributes);
        AislingSubject?.Client.SendAttributes(StatUpdateType.Full);
        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You feel much stronger now.");
    }

    public override void OnDispelled() => OnTerminated();

    public override void OnTerminated()
    {
        var attributes = new Attributes
        {
            MaximumHp = 1000,
            MaximumMp = 1000,
            Ac = -6
        };

        Subject.StatSheet.SubtractBonus(attributes);
        AislingSubject?.Client.SendAttributes(StatUpdateType.Full);
        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Your body returns to normal.");
    }
}
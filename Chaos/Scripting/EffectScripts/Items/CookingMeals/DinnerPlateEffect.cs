using Chaos.DarkAges.Definitions;
using Chaos.Models.Data;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Components.EffectComponents;
using Chaos.Scripting.Components.Execution;
using Chaos.Scripting.EffectScripts.Abstractions;

namespace Chaos.Scripting.EffectScripts.Items.CookingMeals;

public class DinnerPlateEffect : EffectBase, NonOverwritableEffectComponent.INonOverwritableEffectComponentOptions
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
            "PieGreengrapes",
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
    public override string Name => "Dinner Plate";
    protected byte? Sound => 115;

    public override void OnApplied()
    {
        base.OnApplied();

        var attributes = new Attributes
        {
            MaximumHp = 200,
            MaximumMp = 200,
            Dmg = 2,
            Hit = 2,
            Ac = -1
        };

        Subject.StatSheet.AddBonus(attributes);
        AislingSubject?.Client.SendAttributes(StatUpdateType.Full);
        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You feel full and strong.");
    }

    public override void OnDispelled() => OnTerminated();

    public override void OnTerminated()
    {
        var attributes = new Attributes
        {
            MaximumHp = 200,
            MaximumMp = 200,
            Dmg = 2,
            Hit = 2,
            Ac = -1
        };

        Subject.StatSheet.SubtractBonus(attributes);
        AislingSubject?.Client.SendAttributes(StatUpdateType.Full);
        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You're no longer full.");
    }

    public override bool ShouldApply(Creature source, Creature target)
    {
        var execution = new ComponentExecutor(source, target).WithOptions(this)
                                                             .ExecuteAndCheck<NonOverwritableEffectComponent>();

        return execution is not null;
    }
}
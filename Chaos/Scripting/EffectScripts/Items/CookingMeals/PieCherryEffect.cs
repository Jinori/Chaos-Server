using Chaos.Common.Definitions;
using Chaos.Models.Data;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Components.EffectComponents;
using Chaos.Scripting.Components.Execution;
using Chaos.Scripting.EffectScripts.Abstractions;

namespace Chaos.Scripting.EffectScripts.Items.CookingMeals;

public class PieCherryEffect : EffectBase, NonOverwritableEffectComponent.INonOverwritableEffectComponentOptions
{
    protected int MaxManaSaved;
    protected override TimeSpan Duration { get; set; } = TimeSpan.FromMinutes(15);
    protected Animation? Animation { get; } = new()
    {
        TargetAnimation = 127,
        AnimationSpeed = 100
    };
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
    public override byte Icon => 72;
    public override string Name => "Cherry Pie";
    protected byte? Sound => 115;

    public override void OnApplied()
    {
        base.OnApplied();

        var maxMana = Subject.StatSheet.MaximumMp;
        
        var attributes = new Attributes
        {
            MaximumMp = Subject.StatSheet.MaximumMp * 20/100
        };

        MaxManaSaved = maxMana;
        Subject.StatSheet.AddBonus(attributes);
        AislingSubject?.Client.SendAttributes(StatUpdateType.Full);

        AislingSubject?.Client.SendServerMessage(
            ServerMessageType.OrangeBar1,
            "Your ability to cast spells has increased.");
    }

    public override void OnDispelled() => OnTerminated();

    public override void OnTerminated()
    {
        var attributes = new Attributes
        {
            MaximumMp = MaxManaSaved * 20 / 100
        };

        Subject.StatSheet.SubtractBonus(attributes);
        AislingSubject?.Client.SendAttributes(StatUpdateType.Full);
        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Your ability to cast spells has returned to normal.");
    }
    public override bool ShouldApply(Creature source, Creature target)
    {
        var execution = new ComponentExecutor(source, target).WithOptions(this)
                                                             .ExecuteAndCheck<NonOverwritableEffectComponent>();

        return execution is not null;
    }
}
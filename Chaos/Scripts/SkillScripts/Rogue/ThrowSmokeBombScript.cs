using Chaos.Data;
using Chaos.Objects.Panel;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripts.SkillScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripts.SkillScripts.Rogue;

public class ThrowSmokeBombScript : BasicSkillScriptBase
{
    private readonly IEffectFactory EffectFactory;
    private string EffectKey { get; init; } = null!;

    protected new Animation Animation { get; } = new()
    {
        AnimationSpeed = 100,
        TargetAnimation = 18
    };
    
    /// <inheritdoc />
    public ThrowSmokeBombScript(Skill subject, IEffectFactory effectFactory)
        : base(subject) =>
        EffectFactory = effectFactory;

    public override void OnUse(ActivationContext context)
    {
        var targets = AbilityComponent.Activate<Creature>(context, AbilityComponentOptions);

        var screen = context.Source.MapInstance.GetEntitiesWithinRange<Creature>(context.SourcePoint)
                            .Where(x => x.IsHostileTo(context.Source));

        if (!screen.Any())
        {
            context.SourceAisling?.SendOrangeBarMessage("There are no hostile mobs to blind.");
            return;
        }

        if (context.SourceAisling?.Inventory.CountOf("smokebomb") < 1)
        {
            context.SourceAisling?.SendOrangeBarMessage("You don't have any smokebombs to throw!");
            return;
        }

        context.Source.Animate(Animation, context.Source.Id);
        context.SourceAisling?.Inventory.RemoveQuantity("smokebomb", 1);
        context.SourceAisling?.SendOrangeBarMessage("You throw a smokebomb at your feet and it explodes!");
        
        foreach (var monster in screen)
        {
            var effect = EffectFactory.Create(EffectKey);
            monster.Effects.Apply(context.Source, effect);
            monster.Animate(Animation, monster.Id);
        }
    }
}
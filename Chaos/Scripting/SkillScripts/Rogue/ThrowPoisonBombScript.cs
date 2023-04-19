using Chaos.Data;
using Chaos.Objects.Panel;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripting.SkillScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.SkillScripts.Rogue;

public class ThrowPoisonBombScript : BasicSkillScriptBase
{
    private readonly IEffectFactory EffectFactory;
    public string EffectKey { get; set; } = null!;
    protected new Animation Animation { get; } = new()
    {
        AnimationSpeed = 100,
        TargetAnimation = 196
    };

    /// <inheritdoc />
    public ThrowPoisonBombScript(Skill subject, IEffectFactory effectFactory)
        : base(subject) =>
        EffectFactory = effectFactory;

    public override void OnUse(ActivationContext context)
    {
        var targets = AbilityComponent.Activate<Creature>(context, this);
        var screen = context.Source.MapInstance.GetEntitiesWithinRange<Creature>(context.SourcePoint, 3)
                            .Where(x => x.IsHostileTo(context.Source));

        if (!screen.Any())
        {
            context.SourceAisling?.SendOrangeBarMessage("There are no hostile mobs to poison near you.");

            return;
        }
        
        context.Source.Animate(Animation, context.Source.Id);
        context.SourceAisling?.SendOrangeBarMessage("A mist of poison spreads to your enemies.");

        foreach (var monster in screen)
        {
            var effect = EffectFactory.Create(EffectKey);
            monster.Effects.Apply(context.Source, effect);
            monster.Animate(Animation, monster.Id);
        }
    }
}
using Chaos.Geometry.Abstractions;
using Chaos.Objects;
using Chaos.Objects.Panel;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripts.SkillScripts.Abstractions;
using Chaos.Scripts.SpellScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripts.SkillScripts;

public class ApplyEffectScript : BasicSkillScriptBase
{
    private readonly IEffectFactory EffectFactory;
    protected string EffectKey { get; init; } = null!;

    /// <inheritdoc />
    public ApplyEffectScript(Skill subject, IEffectFactory effectFactory)
        : base(subject) =>
        EffectFactory = effectFactory;

    public override void OnUse(SkillContext context)
    {
        ShowBodyAnimation(context);

        var affectedPoints = GetAffectedPoints(context).Cast<IPoint>().ToList();
        var affectedEntities = GetAffectedEntities<Creature>(context, affectedPoints);

        ShowAnimation(context, affectedPoints);
        PlaySound(context, affectedPoints);

        var effect = EffectFactory.Create(EffectKey);
        context.Source.Effects.Apply(context.Source, effect);
    }
}
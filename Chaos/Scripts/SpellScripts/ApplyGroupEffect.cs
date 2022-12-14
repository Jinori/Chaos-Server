using Chaos.Extensions;
using Chaos.Geometry.Abstractions;
using Chaos.Objects;
using Chaos.Objects.Panel;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripts.SpellScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using NLog.Targets;

namespace Chaos.Scripts.SpellScripts;

[SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global")]
public class ApplyGroupEffectScript : BasicSpellScriptBase
{
    private readonly IEffectFactory EffectFactory;
    protected string EffectKey { get; init; } = null!;

    /// <inheritdoc />
    public ApplyGroupEffectScript(Spell subject, IEffectFactory effectFactory)
        : base(subject) =>
        EffectFactory = effectFactory;

    /// <inheritdoc />
    public override void OnUse(SpellContext context)
    {
        ShowBodyAnimation(context);
        var group = context.SourceAisling?.Group?.Where(x => x.WithinRange(context.SourcePoint));


        var affectedPoints = GetAffectedPoints(context).Cast<IPoint>().ToList();
        var affectedEntities = GetAffectedEntities<Creature>(context, affectedPoints);

        PlaySound(context, affectedPoints);


        if (group is null)
        {
            ShowAnimation(context, affectedPoints);
            var effect = EffectFactory.Create(EffectKey);
            context.Source.Effects.Apply(context.Source, effect);
        }
        else
        {
            ShowAnimation(context, group!);
            foreach (var entity in group)
            {
                var effect = EffectFactory.Create(EffectKey);
                entity.Effects.Apply(context.Source, effect);
            }
        }
    }
}
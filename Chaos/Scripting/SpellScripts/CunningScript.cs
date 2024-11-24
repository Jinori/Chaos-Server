using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Scripting.SpellScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.SpellScripts;

public class CunningScript : SpellScriptBase
{
    private readonly IEffectFactory EffectFactory;

    /// <inheritdoc />
    public CunningScript(Spell subject, IEffectFactory effectFactory)
        : base(subject)
        => EffectFactory = effectFactory;

    /// <inheritdoc />
    public override void OnUse(SpellContext context)
    {
        var effect1 = EffectFactory.Create("Cunning1");
        var effect2 = EffectFactory.Create("Cunning2");
        var effect3 = EffectFactory.Create("Cunning3");
        var effect4 = EffectFactory.Create("Cunning4");
        var effect5 = EffectFactory.Create("Cunning5");
        var effect6 = EffectFactory.Create("Cunning6");

        if (!context.Source.Effects.Contains("Cunning1")
            && !context.Source.Effects.Contains("Cunning2")
            && !context.Source.Effects.Contains("Cunning3")
            && !context.Source.Effects.Contains("Cunning4")
            && !context.Source.Effects.Contains("Cunning5")
            && !context.Source.Effects.Contains("Cunning6"))
        {
            context.Source.Effects.Apply(context.Source, effect1);

            return;
        }

        if (context.Source.Effects.Contains("Cunning1"))
        {
            context.Source.Effects.Apply(context.Source, effect2);

            return;
        }

        if (context.Source.Effects.Contains("Cunning2"))
        {
            context.Source.Effects.Apply(context.Source, effect3);

            return;
        }

        if (context.Source.Effects.Contains("Cunning3"))
        {
            context.Source.Effects.Apply(context.Source, effect4);

            return;
        }

        if (context.Source.Effects.Contains("Cunning4"))
        {
            context.Source.Effects.Apply(context.Source, effect5);

            return;
        }

        if (context.Source.Effects.Contains("Cunning5"))
            context.Source.Effects.Apply(context.Source, effect6);

        if (context.Source.Effects.Contains("Cunning6"))
            context.Source.Effects.Apply(context.Source, effect6);
    }
}
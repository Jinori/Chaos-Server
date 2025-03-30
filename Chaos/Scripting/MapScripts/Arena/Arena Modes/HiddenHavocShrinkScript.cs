using Chaos.Collections;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Abstractions;
using Chaos.Scripting.MapScripts.Abstractions;
using Chaos.Scripting.MapScripts.Arena.Arena_Modules;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.MapScripts.Arena.Arena_Modes;

public sealed class HiddenHavocShrinkScript : CompositeMapScript
{
    private static readonly ICollection<string> ScriptKeys = new[]
    {
        GetScriptKey(typeof(AislingDeathTouchScript)),
        GetScriptKey(typeof(MapShrinkScript)),
        GetScriptKey(typeof(DeclareWinnerScript))
    };

    private readonly IEffectFactory EffectFactory;

    /// <inheritdoc />
    public HiddenHavocShrinkScript(IScriptProvider scriptProvider, MapInstance subject, IEffectFactory effectFactory)
    {
        if (scriptProvider.CreateScript<IMapScript, MapInstance>(ScriptKeys, subject) is not CompositeMapScript script)
            throw new InvalidOperationException("Unable to create componentized script");

        foreach (var component in script)
            Add(component);

        EffectFactory = effectFactory;
    }

    /// <inheritdoc />
    public override void OnEntered(Creature creature)
    {
        var hide = EffectFactory.Create("HiddenHavocHide");
        creature.Effects.Apply(creature, hide);
    }
}
using Chaos.CommandInterceptor;
using Chaos.CommandInterceptor.Abstractions;
using Chaos.Common.Collections;
using Chaos.Objects.World;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Commands;
[Command("giveEffect")]

public class GiveEffectCommand : ICommand<Aisling>
{
    private readonly IEffectFactory _effectFactory;

    public GiveEffectCommand(IEffectFactory effectFactory)
    {
        _effectFactory = effectFactory;
    }
    
    public ValueTask ExecuteAsync(Aisling source, ArgumentCollection args)
    {
        if (!args.TryGetNext<string>(out var effect)) 
            return default;
        
        var effectSelf = _effectFactory.Create(effect);
        source.Effects.Apply(source, effectSelf);
        return default;
    }
}
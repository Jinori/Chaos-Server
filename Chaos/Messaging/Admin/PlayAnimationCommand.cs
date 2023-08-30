using Chaos.Collections.Common;
using Chaos.Messaging.Abstractions;
using Chaos.Models.Data;
using Chaos.Models.World;

namespace Chaos.Messaging.Admin;

[Command("playanimation", helpText:"<animation number>")]

public class PlayAnimationCommand : ICommand<Aisling>
{
    /// <inheritdoc />
    public ValueTask ExecuteAsync(Aisling source, ArgumentCollection args)
    {
        if (!args.TryGetNext(out ushort? animation))
            return default;

        var play = new Animation
        {
            TargetAnimation = (ushort)animation!,
            AnimationSpeed = 100
        };
        
        source.Animate(play);

        return default;
    }
}
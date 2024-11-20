using Chaos.Collections.Common;
using Chaos.Common.Definitions;
using Chaos.DarkAges.Definitions;
using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Extensions.Common;
using Chaos.Messaging.Abstractions;
using Chaos.Models.World;
using Chaos.Networking.Abstractions;
using Chaos.Scripting.EffectScripts.GMEffects;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Messaging.Admin;

[Command("follow", helpText: "<targetName>")]
public class FollowPlayerCommand(IClientRegistry<IChaosWorldClient> clientRegistry, IEffectFactory effectFactory) : ICommand<Aisling>
{
    /// <inheritdoc />
    public ValueTask ExecuteAsync(Aisling adminSource, ArgumentCollection args)
    {
        if (!args.TryGetNext<string>(out var playerName))
            return default;

        // Find the target player to be followed
        var targetPlayer = clientRegistry.Select(c => c.Aisling)
            .FirstOrDefault(a => a.Name.EqualsI(playerName));

        if (targetPlayer == null)
        {
            adminSource.SendOrangeBarMessage($"{playerName} is not online");
            adminSource.Effects.Terminate("follow");
            return default;
        }

        adminSource.StatSheet.SetHealthPct(100);
        adminSource.Client.SendAttributes(StatUpdateType.Vitality);
        adminSource.SendOrangeBarMessage("Godmode enabled.");
        adminSource.Trackers.Enums.Set(GodMode.Yes);
        // Cast the follow effect on the admin (source), which will follow the target player
        CastFollowEffect(adminSource, targetPlayer);

        return default;
    }

    private void CastFollowEffect(Aisling adminSource, Aisling targetPlayer)
    {
        // Create a new instance of the FollowEffect, applying it to the admin
        var followEffect = effectFactory.Create("Follow");

        // Apply the effect to the adminSource, so they follow the targetPlayer
        adminSource.Effects.Apply(adminSource, followEffect);

        // Set the target player in the effect (if not already set in the FollowEffect itself)
        if (followEffect is FollowEffect followEffectInstance)
        {
            followEffectInstance.TargetPlayer = targetPlayer;  // The player to be followed
        }

        // Notify the admin
        adminSource.SendOrangeBarMessage($"You are now following {targetPlayer.Name}.");
    }
}

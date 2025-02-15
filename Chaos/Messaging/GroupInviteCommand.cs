
#region
using System.Text.RegularExpressions;
using Chaos.Collections.Common;
using Chaos.Extensions;
using Chaos.Extensions.Common;
using Chaos.Messaging.Abstractions;
using Chaos.Models.World;
using Chaos.Networking.Abstractions;
using Chaos.Services.Other.Abstractions;
#endregion

namespace Chaos.Messaging;

[Command("invite", false, "<targetName>")]
public class GroupInviteCommand(IClientRegistry<IChaosWorldClient> clientRegistry, IGroupService groupService) : ICommand<Aisling>
{
    private readonly IClientRegistry<IChaosWorldClient> ClientRegistry = clientRegistry;
    private readonly IGroupService GroupService = groupService;

    /// <inheritdoc />
    public ValueTask ExecuteAsync(Aisling source, ArgumentCollection args)
    {
        if (!args.TryGetNext<string>(out var targetName))
            return default;

        targetName = targetName.ReplaceI("_", " ");

        var targetClient = ClientRegistry.FirstOrDefault(c => c.Aisling.Name.EqualsI(targetName));

        if (targetClient == null)
        {
            source.SendOrangeBarMessage($"{targetName} cannot be found.");
            return default;
        }

        var targetAisling = targetClient.Aisling;

        // Admin in god mode cannot be invited
        if (!source.IsAdmin && targetAisling.IsGodModeEnabled())
        {
            source.SendOrangeBarMessage($"{targetName} cannot be found.");
            return default;
        }

        // If the inviter is an admin, they can group without asking for confirmation
        if (source.IsAdmin)
        {
            GroupService.Invite(targetAisling, source);
            return default;
        }

        // Otherwise, invite normally
        GroupService.Invite(source, targetAisling);

        return default;
    }
}
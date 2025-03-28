#region
using Chaos.Common.Utilities;
using Chaos.DarkAges.Definitions;
using Chaos.Extensions;
using Chaos.Extensions.Common;
using Chaos.Messaging.Abstractions;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Services.Servers.Options;
using Chaos.Utilities;
#endregion

namespace Chaos.Collections;

/// <summary>
///     Represents a group of players
/// </summary>
public sealed class Group : IEnumerable<Aisling>, IDedicatedChannel
{
    public enum GroupLootOption
    {
        Default,
        Random = 1,
        NeedBeforeGreed = 2,
        MasterLooter = 3
    }

    private readonly IChannelService ChannelService;
    public readonly List<Aisling> Members;
    private readonly Lock Sync;

    public GroupLootOption LootOption { get; set; } = GroupLootOption.Default;
    public string ChannelName { get; }

    /// <summary>
    ///     The leader of the group
    /// </summary>
    public Aisling Leader
    {
        get
        {
            using var @lock = Sync.EnterScope();

            return Members[0];
        }
    }

    /// <summary>
    ///     The number of members in the group
    /// </summary>
    public int Count => Members.Count;

    /// <summary>
    ///     Initializes a new instance of the <see cref="Group" /> class.
    /// </summary>
    /// <param name="sender">
    ///     The aisling who started the group
    /// </param>
    /// <param name="receiver">
    ///     The first aisling to join the group (besides the sender/leader)
    /// </param>
    /// <param name="channelService">
    ///     A channel service for the group to communicate through
    /// </param>
    public Group(Aisling sender, Aisling receiver, IChannelService channelService)
    {
        ChannelName = $"!group-{Guid.NewGuid()}";
        ChannelService = channelService;

        Members =
        [
            sender,
            receiver
        ];

        //create a group chat channel and add both members to it
        ChannelService.RegisterChannel(
            null,
            ChannelName,
            WorldOptions.Instance.GroupMessageColor,
            (sub, msg) =>
            {
                var aisling = (Aisling)sub;
                aisling.SendServerMessage(ServerMessageType.GroupChat, msg);

                var chunks = Helpers.ChunkMessage(msg);

                if (chunks.Count > 1)
                {
                    var orangeBarChunk = Helpers.ChunkMessage(msg)[0];
                    orangeBarChunk = $"{orangeBarChunk[..^3]}...";
                    aisling.SendServerMessage(ServerMessageType.OrangeBar1, orangeBarChunk);
                }
            },
            true,
            "!group");

        JoinChannel(sender);
        JoinChannel(receiver);

        sender.SendActiveMessage($"You form a group with {receiver.Name}");
        receiver.SendActiveMessage($"You form a group with {sender.Name}");

        Sync = new Lock();
    }

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc />
    public IEnumerator<Aisling> GetEnumerator()
    {
        List<Aisling> snapshot;

        using (Sync.EnterScope())
            snapshot = Members.ToList();

        return snapshot.GetEnumerator();
    }

    /// <inheritdoc />
    public void JoinChannel(IChannelSubscriber subscriber) => ChannelService.JoinChannel(subscriber, ChannelName, true);

    /// <inheritdoc />
    public void LeaveChannel(IChannelSubscriber subscriber) => ChannelService.LeaveChannel(subscriber, ChannelName);

    /// <inheritdoc />
    public void SendMessage(IChannelSubscriber from, string message) => ChannelService.SendMessage(from, ChannelName, message);

    /// <summary>
    ///     Adds an aisling to the group
    /// </summary>
    /// <param name="aisling">
    ///     The aisling to add to the group
    /// </param>
    public void Add(Aisling aisling)
    {
        using var @lock = Sync.EnterScope();

        foreach (var member in this)
        {
            member.SendActiveMessage($"{aisling.Name} has joined the group");
            member.Client.SendSelfProfile();
        }

        Members.Add(aisling);

        JoinChannel(aisling);
        aisling.SendActiveMessage($"You have joined {Leader.Name}'s group");
        aisling.Group = this;
        aisling.Client.SendSelfProfile();
    }

    /// <summary>
    ///     Determines whether the group contains the specified aisling
    /// </summary>
    /// <param name="aisling">
    ///     The aisling to check for
    /// </param>
    /// <returns>
    ///     <c>
    ///         true
    ///     </c>
    ///     if the group contains the specified aisling, otherwise
    ///     <c>
    ///         false
    ///     </c>
    /// </returns>
    public bool Contains(Aisling aisling)
    {
        using var @lock = Sync.EnterScope();

        return Members.Contains(aisling, WorldEntity.IdComparer);
    }

    private void Disband()
    {
        using var @lock = Sync.EnterScope();

        foreach (var member in this)
        {
            member.Group = null;
            LeaveChannel(member);
            member.SendActiveMessage("The group has been disbanded");
            member.Client.SendSelfProfile();
        }

        ChannelService.UnregisterChannel(ChannelName);
        Members.Clear();
    }

    public void DistributeEvenGold(int gold, Monster monster, bool boosted)
    {
        if ((Members.Count == 0) || (gold <= 1))
            return;

        // Filter group members who are on the same map as the monster
        var eligibleMembers = Members.Where(member => member.MapInstance == monster.MapInstance)
                                     .ToList();

        // If there are no eligible members, skip the gold distribution
        if (eligibleMembers.Count == 0)
            return;

        // Calculate gold per eligible member
        var amountPerMember = gold / eligibleMembers.Count;

        // Distribute gold evenly among eligible members
        foreach (var member in eligibleMembers)
        {
            member.GiveGoldOrSendToBank(amountPerMember);

            member.SendServerMessage(
                ServerMessageType.ActiveMessage,
                boosted
                    ? $"Your split of the group gold is {amountPerMember}, boosted by Serendael!"
                    : $"Your split of the group gold is {amountPerMember}.");
        }
    }

    public void DistributeRandomized(IEnumerable<Item> lootItems, Monster monster)
    {
        if (Members.Count == 0)
            return;

        // Iterate through each loot item
        foreach (var item in lootItems)
        {
            // Filter group members to ensure they are on the same map as the monster
            var eligibleMembers = Members.Where(member => member.MapInstance == monster.MapInstance)
                                         .ToList();

            // If no eligible members, skip the loot distribution
            if (eligibleMembers.Count == 0)
                continue;

            // Pick a random eligible member to receive the loot
            var randomMember = eligibleMembers.PickRandom();

            var adminOnMap = randomMember.MapInstance
                                         .GetEntities<Aisling>()
                                         .Where(x => x.IsGodModeEnabled())
                                         .ToList();

            // Give the item to the random member or send it to their bank
            randomMember.GiveItemOrSendToBank(item);

            // Notify all group members about the loot distribution
            foreach (var member in Members)
                member.SendServerMessage(ServerMessageType.ActiveMessage, $"{randomMember.Name} received {item.DisplayName} from loot.");

            foreach (var admin in adminOnMap)
                admin.SendServerMessage(ServerMessageType.ActiveMessage, $"{randomMember.Name} received {item.DisplayName}.");
        }
    }

    public void Kick(Aisling aisling)
    {
        using var @lock = Sync.EnterScope();

        if (Members.Count <= 2)
        {
            Disband();

            return;
        }

        //leader is kicking himself
        if (aisling.Equals(Leader))
        {
            Leave(aisling);

            return;
        }

        if (!Remove(aisling))
            return;

        aisling.SendActiveMessage($"You have been kicked from the group by {Leader.Name}");

        foreach (var member in Members)
            member.SendActiveMessage($"{aisling.Name} has been kicked from the group");
    }

    /// <summary>
    ///     Leaves the group
    /// </summary>
    /// <param name="aisling">
    ///     The aisling leaving the group
    /// </param>
    /// <remarks>
    ///     This handles if the group has only 2 members (disbands), or if the aisling leaving is the leader. The new leader
    ///     will be the member who has been in the group longest
    /// </remarks>
    public void Leave(Aisling aisling)
    {
        using var @lock = Sync.EnterScope();

        if (Members.Count <= 2)
        {
            Disband();

            return;
        }

        var oldLeader = Leader;

        if (!Remove(aisling))
            return;

        aisling.SendActiveMessage("You have left the group");

        foreach (var member in Members)
        {
            member.SendActiveMessage($"{aisling.Name} has left the group");
            member.Client.SendSelfProfile();
        }

        if (oldLeader.Equals(aisling))
        {
            var newLeader = Leader;

            foreach (var member in Members)
                member.SendActiveMessage($"{newLeader.Name} is now the group leader");
        }
    }

    public void Promote(Aisling aisling)
    {
        using var @lock = Sync.EnterScope();

        if (aisling.Equals(Leader))
        {
            aisling.SendActiveMessage("You dumbly point to yourself");

            return;
        }

        Members.Remove(aisling);
        Members.Insert(0, aisling);

        foreach (var member in Members)
        {
            member.SendActiveMessage($"{aisling.Name} has been promoted to group leader");
            member.Client.SendSelfProfile();
        }
    }

    private bool Remove(Aisling aisling)
    {
        using var @lock = Sync.EnterScope();

        if (!Members.Remove(aisling))
            return false;

        aisling.Group = null;
        aisling.Client.SendSelfProfile();
        LeaveChannel(aisling);

        return true;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        using var @lock = Sync.EnterScope();

        var groupString = "Group members";

        foreach (var user in Members)
        {
            var name = user.Name.ReplaceI(" ", "_");

            if (user.Equals(Leader))
                groupString += '\n' + $"* {name}";
            else
                groupString += '\n' + $"  {name}";
        }

        groupString += '\n' + $"Total {Count}";

        return groupString;
    }
}
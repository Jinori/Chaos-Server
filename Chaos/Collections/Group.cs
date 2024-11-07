using Chaos.Common.Definitions;
using Chaos.Common.Synchronization;
using Chaos.Common.Utilities;
using Chaos.Extensions;
using Chaos.Messaging.Abstractions;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Services.Servers.Options;

namespace Chaos.Collections;

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
    private readonly AutoReleasingMonitor Sync;

    public GroupLootOption LootOption { get; set; } = GroupLootOption.Default;
    public string ChannelName { get; }

    public Aisling Leader
    {
        get
        {
            using var @lock = Sync.Enter();

            return Members[0];
        }
    }

    public int Count => Members.Count;

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
            },
            true,
            "!group");

        JoinChannel(sender);
        JoinChannel(receiver);

        sender.SendActiveMessage($"You form a group with {receiver.Name}");
        receiver.SendActiveMessage($"You form a group with {sender.Name}");

        Sync = new AutoReleasingMonitor();
    }

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc />
    public IEnumerator<Aisling> GetEnumerator()
    {
        List<Aisling> snapshot;

        using (Sync.Enter())
            snapshot = Members.ToList();

        return snapshot.GetEnumerator();
    }

    /// <inheritdoc />
    public void JoinChannel(IChannelSubscriber subscriber) => ChannelService.JoinChannel(subscriber, ChannelName, true);

    /// <inheritdoc />
    public void LeaveChannel(IChannelSubscriber subscriber) => ChannelService.LeaveChannel(subscriber, ChannelName);

    public void SendMessage(IChannelSubscriber from, string message) => ChannelService.SendMessage(from, ChannelName, message);

    public void Add(Aisling aisling)
    {
        using var @lock = Sync.Enter();

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

    public bool Contains(Aisling aisling)
    {
        using var @lock = Sync.Enter();

        return Members.Contains(aisling, WorldEntity.IdComparer);
    }

    private void Disband()
    {
        using var @lock = Sync.Enter();

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

    public void DistributeEvenGold(int gold, Monster monster)
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
            member.SendOrangeBarMessage($"Your split of the group gold is {amountPerMember}.");
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
                member.SendServerMessage(ServerMessageType.GroupChat, $"{randomMember.Name} received {item.DisplayName} from loot.");

            foreach (var admin in adminOnMap)
                admin.SendOrangeBarMessage($"{randomMember.Name} received {item.DisplayName}.");
        }
    }

    public void Kick(Aisling aisling)
    {
        using var @lock = Sync.Enter();

        if (Members.Count <= 2)
        {
            Disband();

            return;
        }

        if (!Remove(aisling))
            return;

        aisling.SendActiveMessage($"You have been kicked from the group by {Leader.Name}");

        foreach (var member in Members)
            member.SendActiveMessage($"{aisling.Name} has been kicked from the group");
    }

    public void Leave(Aisling aisling)
    {
        using var @lock = Sync.Enter();

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

    private bool Remove(Aisling aisling)
    {
        using var @lock = Sync.Enter();

        if (!Members.Remove(aisling))
            return false;

        aisling.Group = null;
        aisling.Client.SendSelfProfile();
        LeaveChannel(aisling);

        return true;
    }

    public override string ToString()
    {
        using var @lock = Sync.Enter();

        var groupString = "Group members";

        foreach (var user in Members)
            if (user.Equals(Leader))
                groupString += '\n' + $"* {user.Name}";
            else
                groupString += '\n' + $"  {user.Name}";

        groupString += '\n' + $"Total {Count}";

        return groupString;
    }
}
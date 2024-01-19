using Chaos.Networking.Abstractions;
using Chaos.Packets;

namespace Chaos.Servers.Abstractions;

public interface IWorldServer : IServer<IWorldClient>
{
    ValueTask OnBeginChant(IWorldClient client, in Packet clientPacket);
    ValueTask OnBoardRequest(IWorldClient client, in Packet clientPacket);
    ValueTask OnChant(IWorldClient client, in Packet clientPacket);
    ValueTask OnClick(IWorldClient client, in Packet clientPacket);
    ValueTask OnClientRedirected(IWorldClient client, in Packet clientPacket);
    ValueTask OnClientWalk(IWorldClient client, in Packet clientPacket);
    ValueTask OnDialogResponse(IWorldClient client, in Packet clientPacket);
    ValueTask OnEmote(IWorldClient client, in Packet clientPacket);
    ValueTask OnExchange(IWorldClient client, in Packet clientPacket);
    ValueTask OnExitRequest(IWorldClient client, in Packet clientPacket);
    ValueTask OnGoldDropped(IWorldClient client, in Packet clientPacket);
    ValueTask OnGoldDroppedOnCreature(IWorldClient client, in Packet clientPacket);
    ValueTask OnGroupRequest(IWorldClient client, in Packet clientPacket);
    ValueTask OnIgnore(IWorldClient client, in Packet clientPacket);
    ValueTask OnItemDropped(IWorldClient client, in Packet clientPacket);
    ValueTask OnItemDroppedOnCreature(IWorldClient client, in Packet clientPacket);
    ValueTask OnMapDataRequest(IWorldClient client, in Packet clientPacket);
    ValueTask OnMetafileRequest(IWorldClient client, in Packet clientPacket);
    ValueTask OnPickup(IWorldClient client, in Packet clientPacket);
    ValueTask OnProfile(IWorldClient client, in Packet clientPacket);
    ValueTask OnProfileRequest(IWorldClient client, in Packet clientPacket);
    ValueTask OnPublicMessage(IWorldClient client, in Packet clientPacket);
    ValueTask OnPursuitRequest(IWorldClient client, in Packet clientPacket);
    ValueTask OnRaiseStat(IWorldClient client, in Packet clientPacket);
    ValueTask OnRefreshRequest(IWorldClient client, in Packet clientPacket);
    ValueTask OnSocialStatus(IWorldClient client, in Packet clientPacket);
    ValueTask OnSpacebar(IWorldClient client, in Packet clientPacket);
    ValueTask OnSwapSlot(IWorldClient client, in Packet clientPacket);
    ValueTask OnToggleGroup(IWorldClient client, in Packet clientPacket);
    ValueTask OnTurn(IWorldClient client, in Packet clientPacket);
    ValueTask OnUnequip(IWorldClient client, in Packet clientPacket);
    ValueTask OnUseItem(IWorldClient client, in Packet clientPacket);
    ValueTask OnUserOptionToggle(IWorldClient client, in Packet clientPacket);
    ValueTask OnUseSkill(IWorldClient client, in Packet clientPacket);
    ValueTask OnUseSpell(IWorldClient client, in Packet clientPacket);
    ValueTask OnWhisper(IWorldClient client, in Packet clientPacket);
    ValueTask OnWorldListRequest(IWorldClient client, in Packet clientPacket);
    ValueTask OnWorldMapClick(IWorldClient client, in Packet clientPacket);
}
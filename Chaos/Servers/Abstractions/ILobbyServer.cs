using Chaos.Networking.Abstractions;
using Chaos.Packets;

namespace Chaos.Servers.Abstractions;

public interface ILobbyServer : IServer<ILobbyClient>
{
    ValueTask OnConnectionInfoRequest(ILobbyClient client, in Packet packet);
    ValueTask OnServerTableRequest(ILobbyClient client, in Packet packet);
}
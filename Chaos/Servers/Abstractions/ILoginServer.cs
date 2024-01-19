using Chaos.Networking.Abstractions;
using Chaos.Packets;

namespace Chaos.Servers.Abstractions;

public interface ILoginServer : IServer<ILoginClient>
{
    ValueTask OnClientRedirected(ILoginClient client, in Packet packet);
    ValueTask OnCreateCharFinalize(ILoginClient client, in Packet packet);
    ValueTask OnCreateCharRequest(ILoginClient client, in Packet packet);
    ValueTask OnHomepageRequest(ILoginClient client, in Packet packet);
    ValueTask OnLogin(ILoginClient client, in Packet packet);
    ValueTask OnMetafileRequest(ILoginClient client, in Packet packet);
    ValueTask OnNoticeRequest(ILoginClient client, in Packet packet);
    ValueTask OnPasswordChange(ILoginClient client, in Packet packet);
}
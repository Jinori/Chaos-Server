using Chaos.Core.Definitions;
using Chaos.Packets.Interfaces;

namespace Chaos.Networking.Model.Server;

public record PublicMessageArgs : ISendArgs
{
    public string Message { get; set; } = null!;
    public PublicMessageType PublicMessageType { get; set; }
    public uint SourceId { get; set; }
}
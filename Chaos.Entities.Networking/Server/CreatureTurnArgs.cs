using Chaos.Geometry.Definitions;
using Chaos.Packets.Interfaces;

namespace Chaos.Entities.Networking.Server;

public record CreatureTurnArgs : ISendArgs
{
    public Direction Direction { get; set; }
    public uint SourceId { get; set; }
}
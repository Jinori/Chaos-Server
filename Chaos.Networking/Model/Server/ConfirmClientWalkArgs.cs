using Chaos.Core.Definitions;
using Chaos.Core.Geometry;
using Chaos.Packets.Interfaces;

namespace Chaos.Networking.Model.Server;

public record ConfirmClientWalkArgs : ISendArgs
{
    public Direction Direction { get; set; }
    public Point OldPoint { get; set; }
}
using Chaos.Packets.Interfaces;

namespace Chaos.Entities.Networking.Server;

public record SoundArgs : ISendArgs
{
    public bool IsMusic { get; set; }
    public byte Sound { get; set; }
}
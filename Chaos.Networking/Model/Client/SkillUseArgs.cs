using Chaos.Packets.Interfaces;

namespace Chaos.Networking.Model.Client;

public record SkillUseArgs(byte SourceSlot) : IReceiveArgs;
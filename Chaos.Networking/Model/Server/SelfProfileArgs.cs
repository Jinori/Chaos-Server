using Chaos.Core.Definitions;
using Chaos.Packets.Interfaces;

namespace Chaos.Networking.Model.Server;

public record SelfProfileArgs : ISendArgs
{
    public AdvClass? AdvClass { get; set; }
    public BaseClass BaseClass { get; set; }
    public IDictionary<EquipmentSlot, ItemArg> Equipment { get; set; } = new Dictionary<EquipmentSlot, ItemArg>();
    public bool GroupOpen { get; set; }
    public string? GroupString { get; set; }
    public string? GuildName { get; set; }
    public string? GuildTitle { get; set; }
    public bool IsMaster { get; set; }
    public ICollection<LegendMarkArg> LegendMarks { get; set; } = new List<LegendMarkArg>();
    public string Name { get; set; } = null!;
    public Nation Nation { get; set; }
    public byte[] Portrait { get; set; } = Array.Empty<byte>();
    public SocialStatus SocialStatus { get; set; }
    public string? SpouseName { get; set; }
    public string ProfileText { get; set; } = null!;
    public ICollection<string> Titles { get; set; } = new List<string>();
}
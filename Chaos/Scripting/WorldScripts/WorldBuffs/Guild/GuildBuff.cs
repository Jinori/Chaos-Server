using System.Text.Json.Serialization;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.WorldScripts.WorldBuffs.Guild;

public class GuildBuff : IDeltaUpdatable
{
    public string GuildName { get; set; } = null!;
    public string BuffName { get; set; } = null!;
    public TimeSpan Duration { get; set; }
    public TimeSpan Elapsed { get; set; }

    [JsonIgnore]
    public bool Expired => Elapsed >= Duration;

    /// <inheritdoc />
    public void Update(TimeSpan delta) => Elapsed += delta;
}
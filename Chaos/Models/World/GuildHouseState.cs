using Chaos.Storage.Abstractions;

namespace Chaos.Models.World;

public sealed class GuildHouseState
{
    private IStorage<GuildHouseState>? Storage;

    public Dictionary<string, Dictionary<string, bool>> GuildProperties { get; set; } = new(StringComparer.OrdinalIgnoreCase);

    public GuildHouseState() { }

    public GuildHouseState(IStorage<GuildHouseState> storage)
    {
        Storage = storage ?? throw new ArgumentNullException(nameof(storage));
    }

    public void SetStorage(IStorage<GuildHouseState> storage)
    {
        Storage = storage;
    }

    public bool HasProperty(string guildName, string propertyName)
    {
        return GuildProperties.TryGetValue(guildName, out var properties) &&
               properties.TryGetValue(propertyName, out var isEnabled) && isEnabled;
    }
    
    public void EnableProperty(string guildName, string propertyName)
    {
        if (!GuildProperties.TryGetValue(guildName, out Dictionary<string, bool>? value))
        {
            value = new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);
            GuildProperties[guildName] = value;
        }

        value[propertyName] = true;

        if (Storage == null)
        {
            throw new InvalidOperationException("Storage is not set. Call SetStorage() before using.");
        }

        Storage.Save();
    }
}
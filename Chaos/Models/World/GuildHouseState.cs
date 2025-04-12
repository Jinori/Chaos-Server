using Chaos.Storage.Abstractions;

namespace Chaos.Models.World;

public sealed class GuildHouseState
{
    private IStorage<GuildHouseState>? Storage;

    public GuildHousePropertiesObj GuildProperties { get; set; } = new();

    public GuildHouseState() { }

    public GuildHouseState(IStorage<GuildHouseState> storage)
    {
        Storage = storage ?? throw new ArgumentNullException(nameof(storage));
    }

    public void SetStorage(IStorage<GuildHouseState> storage)
    {
        Storage = storage ?? throw new ArgumentNullException(nameof(storage));
    }

    public bool HasProperty(string guildName, string propertyName)
    {
        return GuildProperties.Entries.TryGetValue(guildName, out var properties) &&
               GetPropertyValue(properties, propertyName);
    }

    public void EnableProperty(string guildName, string propertyName)
    {
        if (!GuildProperties.Entries.TryGetValue(guildName, out var houseProperties))
        {
            houseProperties = new GuildHousePropertiesObj.HouseProperties();
            GuildProperties.Entries[guildName] = houseProperties;
        }

        SetPropertyValue(houseProperties, propertyName, true);

        SaveChanges();
    }

    private static bool GetPropertyValue(GuildHousePropertiesObj.HouseProperties properties, string propertyName) =>
        propertyName.ToLowerInvariant() switch
        {
            "deed"       => properties.Deed,
            "armory"     => properties.Armory,
            "bank"       => properties.Bank,
            "tailor"     => properties.Tailor,
            "combatroom" => properties.Combatroom,
            _            => throw new ArgumentException($"Unknown property: {propertyName}", nameof(propertyName))
        };

    private static void SetPropertyValue(GuildHousePropertiesObj.HouseProperties properties, string propertyName, bool value)
    {
        switch (propertyName.ToLowerInvariant())
        {
            case "deed":
                properties.Deed = value;
                break;
            case "armory":
                properties.Armory = value;
                break;
            case "bank":
                properties.Bank = value;
                break;
            case "tailor":
                properties.Tailor = value;
                break;
            case "combatroom":
                properties.Combatroom = value;
                break;
            default:
                throw new ArgumentException($"Unknown property: {propertyName}", nameof(propertyName));
        }
    }

    private void SaveChanges()
    {
        if (Storage == null)
        {
            throw new InvalidOperationException("Storage is not set. Call SetStorage() before using.");
        }

        Storage.Save();
    }

    public IReadOnlyList<ItemTransactionLog> GetItemTransactions(string guildName)
    {
        if (GuildProperties.Entries.TryGetValue(guildName, out var houseProperties))
        {
            return houseProperties.ItemLogs;
        }

        return Array.Empty<ItemTransactionLog>();
    }
    
    public void LogItemTransaction(string guildName, string member, string itemName, TransactionType action)
    {
        if (!GuildProperties.Entries.TryGetValue(guildName, out var houseProperties))
        {
            houseProperties = new GuildHousePropertiesObj.HouseProperties();
            GuildProperties.Entries[guildName] = houseProperties;
        }

        houseProperties.ItemLogs.Add(new ItemTransactionLog
        {
            Member = member,
            ItemName = itemName,
            Action = action,
            Timestamp = DateTime.UtcNow
        });

        houseProperties.ItemLogs = action switch
        {
            // Prune logs based on action type
            TransactionType.Withdrawal => houseProperties.ItemLogs.OrderByDescending(log => log.Timestamp)
                                                         .Where(log => log.Action == TransactionType.Withdrawal)
                                                         .Take(30)
                                                         .Concat(
                                                             houseProperties.ItemLogs.Where(
                                                                 log => log.Action != TransactionType.Withdrawal))
                                                         .ToList(),
            TransactionType.Deposit => houseProperties.ItemLogs.OrderByDescending(log => log.Timestamp)
                                                      .Where(log => log.Action == TransactionType.Deposit)
                                                      .Take(30)
                                                      .Concat(houseProperties.ItemLogs.Where(log => log.Action != TransactionType.Deposit))
                                                      .ToList(),
            _ => houseProperties.ItemLogs
        };

        SaveChanges();
    }

    
    public sealed class GuildHousePropertiesObj
    {
        public Dictionary<string, HouseProperties> Entries { get; set; } = new(StringComparer.OrdinalIgnoreCase);

        public class HouseProperties
        {
            public bool Deed { get; set; }
            public bool Armory { get; set; }
            public bool Bank { get; set; }
            public bool Tailor { get; set; }
            public bool Combatroom { get; set; }
            
            public List<ItemTransactionLog> ItemLogs { get; set; } = [];
        }
    }
    
    public sealed class ItemTransactionLog
    {
        public string Member { get; set; } = string.Empty;
        public string ItemName { get; set; } = string.Empty;
        public TransactionType Action { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }

    public enum TransactionType
    {
        Deposit,
        Withdrawal,
        GoldDeposit,
        GoldWithdrawal,
    }
}

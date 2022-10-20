using Chaos.Common.Definitions;
using Chaos.Networking.Entities.Server;
using Chaos.Objects.Panel;
using Chaos.Schemas.Aisling;
using Chaos.Scripting.Abstractions;
using Chaos.Storage.Abstractions;
using Chaos.Templates;
using Chaos.TypeMapper.Abstractions;
using Microsoft.Extensions.Logging;

namespace Chaos.MapperProfiles;

public sealed class ItemMapperProfile : IMapperProfile<Item, ItemSchema>,
                                        IMapperProfile<Item, ItemInfo>
{
    private readonly ILogger<ItemMapperProfile> Logger;
    private readonly IScriptProvider ScriptProvider;
    private readonly ISimpleCache SimpleCache;

    public ItemMapperProfile(
        ISimpleCache simpleCache,
        IScriptProvider scriptProvider,
        ILogger<ItemMapperProfile> logger
    )
    {
        SimpleCache = simpleCache;
        ScriptProvider = scriptProvider;
        Logger = logger;
    }

    public Item Map(ItemSchema obj)
    {
        var template = SimpleCache.Get<ItemTemplate>(obj.TemplateKey);

        var item = new Item(
            template,
            ScriptProvider,
            obj.ScriptKeys,
            obj.UniqueId,
            obj.ElapsedMs)
        {
            Color = obj.Color,
            Count = obj.Count,
            CurrentDurability = obj.CurrentDurability,
            Slot = obj.Slot ?? 0
        };

        Logger.LogTrace("Deserialized item - Name: {ItemName}, UniqueId: {UniqueId}", item.Template.Name, item.UniqueId);

        return item;
    }

    public Item Map(ItemInfo obj) => throw new NotImplementedException();

    ItemInfo IMapperProfile<Item, ItemInfo>.Map(Item obj) => new()
    {
        Color = obj.Color,
        Cost = obj.Template.Value,
        Count = obj.Count < 0
            ? throw new InvalidOperationException($"Item \"{obj.DisplayName}\" has negative count of {obj.Count}")
            : Convert.ToUInt32(obj.Count),
        CurrentDurability = obj.CurrentDurability ?? 0,
        EntityType = EntityType.Item,
        MaxDurability = obj.Template.MaxDurability ?? 0,
        Name = obj.DisplayName,
        Slot = obj.Slot,
        Sprite = obj.Template.ItemSprite.OffsetPanelSprite,
        Stackable = obj.Template.Stackable
    };

    public ItemSchema Map(Item obj)
    {
        var ret = new ItemSchema
        {
            UniqueId = obj.UniqueId,
            ElapsedMs = obj.Elapsed.HasValue ? Convert.ToInt32(obj.Elapsed.Value.TotalMilliseconds) : null,
            ScriptKeys = obj.ScriptKeys.Except(obj.Template.ScriptKeys).ToHashSet(StringComparer.OrdinalIgnoreCase),
            TemplateKey = obj.Template.TemplateKey,
            Color = obj.Color,
            Count = obj.Count,
            CurrentDurability = obj.CurrentDurability,
            Slot = obj.Slot
        };

        Logger.LogTrace("Serialized item - TemplateKey: {TemplateKey}, UniqueId: {UniqueId}", ret.TemplateKey, ret.UniqueId);

        return ret;
    }
}
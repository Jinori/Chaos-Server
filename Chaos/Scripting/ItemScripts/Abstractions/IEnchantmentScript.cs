using Chaos.Extensions.Common;
using Chaos.MetaData.ItemMetaData;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.Templates;
using Chaos.Scripting.ItemScripts.Enchantments;

namespace Chaos.Scripting.ItemScripts.Abstractions;

public interface IEnchantmentScript : IItemScript
{
    static abstract IEnumerable<ItemMetaNode> Mutate(ItemMetaNode node, ItemTemplate template);
}

public interface IPrefixEnchantmentScript : IEnchantmentScript
{
    static abstract Attributes Modifiers { get; }
    static abstract string PrefixStr { get; }
    
    static void ApplyPrefix<T>(Item item) where T: IPrefixEnchantmentScript
    {
        item.Prefix = T.PrefixStr;
        item.Modifiers.Add(T.Modifiers);
    }
    
    static void RemovePrefix<T>(Item item) where T: IPrefixEnchantmentScript
    {
        item.Prefix = null;
        item.Modifiers.Subtract(T.Modifiers);
    }
    
    static IEnumerable<ItemMetaNode> Mutate<T>(ItemMetaNode node, ItemTemplate template) where T: IPrefixEnchantmentScript
    {
        if (!node.Name.StartsWithI(T.PrefixStr))
            yield return node with
            {
                Name = $"{T.PrefixStr} {node.Name}"
            };
    }

    static Dictionary<string, Type> PrefixEnchantmentScripts { get; } = typeof(IPrefixEnchantmentScript).LoadImplementations()
        .ToDictionary(
            type =>
            {
                var prefixProperty = type.GetProperty(nameof(IPrefixEnchantmentScript.PrefixStr));

                return (string)prefixProperty!.GetValue(null)!;
            });
}
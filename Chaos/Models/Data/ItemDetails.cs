using Chaos.Models.Panel;

namespace Chaos.Models.Data;

public sealed class ItemDetails
{
    public required Item Item { get; init; }
    public required int Price { get; init; }

    public static ItemDetails BuyWithGold(Item item)
        => new()
        {
            Item = item,
            Price = item.Template.BuyCost
        };

    public static ItemDetails DisplayRecipe(Item item) => new()
    {
        Item = item,
        Price = 0
    };
    
    public static ItemDetails BuyWithGp(Item item) => new()
    {
        Item = item,
        Price = item.Template.BuyCost
    };
    
    public static ItemDetails BuyWithBp(Item item, int price) => new()
    {
        Item = item,
        Price = price
    };

    
    public static ItemDetails BuyWithTokens(Item item) => new()
    {
        Item = item,
        Price = item.Template.BuyCost
    };
    
    public static ItemDetails SellForTokens(Item item) => new()
    {
        Item = item,
        Price = item.Template.SellValue
    };

    public static ItemDetails WithdrawItem(Item item) => new()
    {
        Item = item,
        Price = item.Count
    };

    public static ItemDetails RecipeCount(Item item) => new()
    {
        Item = item,
        Price = item.Count
    };
}
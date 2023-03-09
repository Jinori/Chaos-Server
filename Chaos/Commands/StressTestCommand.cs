using Chaos.Collections.Common;
using System.Diagnostics;
using Chaos.CommandInterceptor;
using Chaos.CommandInterceptor.Abstractions;
using Chaos.Extensions.Geometry;
using Chaos.Objects.World;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Commands;

[Command("stressTest")]
public class StressTestCommand : ICommand<Aisling>
{
    private readonly IItemFactory ItemFactory;
    private readonly IMerchantFactory MerchantFactory;

    public StressTestCommand(IItemFactory itemFactory, IMerchantFactory merchantFactory)
    {
        ItemFactory = itemFactory;
        MerchantFactory = merchantFactory;
    }

    /// <inheritdoc />
    public ValueTask ExecuteAsync(Aisling source, ArgumentCollection args)
    {
        if (!args.TryGetNext<string>(out var type))
            return default;

        switch (type.ToLower())
        {
            case "grounditems":
                if (!args.TryGetNext<int>(out var amount))
                    return default;

                var sw = new Stopwatch();
                sw.Start();
                
                var items = new List<GroundItem>();
                var map = source.MapInstance;

                for (var i = 0; i < amount; i++)
                {
                    var item = ItemFactory.Create("stick");
                    var point = map.Template.Bounds.RandomPoint();
                    items.Add(new GroundItem(item, map, point));
                }

                map.AddObjects(items);
                sw.Stop();
                source.SendOrangeBarMessage($"{amount} stick(s) spawned on the ground in {sw.Elapsed.TotalSeconds}");
                break;
            
            case "merchants":
                if (!args.TryGetNext<int>(out var amount1))
                    return default;
                
                var sw1 = new Stopwatch();
                sw1.Start();
                
                var map1 = source.MapInstance;
                var merch = new List<Merchant>();

                for (var i = 0; i < amount1; i++)
                {                                  
                    var point1 = map1.Template.Bounds.RandomPoint();
                    var merchant = MerchantFactory.Create("aingeal", map1, point1);
                    merch.Add(merchant);
                }

                map1.AddObjects(merch);
                sw1.Stop();
                source.SendOrangeBarMessage($"{amount1} merchants spawned on the ground in {sw1.Elapsed.TotalSeconds}");
                break;
        }

        return default;
    }
}
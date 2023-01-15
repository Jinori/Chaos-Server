using Chaos.CommandInterceptor;
using Chaos.CommandInterceptor.Abstractions;
using Chaos.Common.Collections;
using Chaos.Extensions.Geometry;
using Chaos.Objects.World;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Commands;

[Command("stressTest")]
public class StressTest : ICommand<Aisling>
{
    private readonly IItemFactory ItemFactory;

    public StressTest(IItemFactory itemFactory) => ItemFactory = itemFactory;

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

                var items = new List<GroundItem>();
                var map = source.MapInstance;

                for (var i = 0; i < amount; i++)
                {
                    var item = ItemFactory.Create("stick");
                    var point = map.Template.Bounds.RandomPoint();
                    items.Add(new GroundItem(item, map, point));
                }

                map.AddObjects(items);

                source.SendOrangeBarMessage($"{amount} stick(s) spawned on the ground");

                break;
        }

        return default;
    }
}
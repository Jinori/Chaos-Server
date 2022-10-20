using Chaos.CommandInterceptor;
using Chaos.CommandInterceptor.Abstractions;
using Chaos.Common.Collections;
using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;

namespace Chaos.Commands;

[Command("clear")]
public class ClearCommand : ICommand<Aisling>
{
    /// <inheritdoc />
    public void Execute(Aisling source, ArgumentCollection args)
    {
        var map = source.MapInstance;

        if (!args.TryGet<string>(0, out var clearType))
            clearType = "all";


        switch (clearType.ToLower())
        {
            case "coins":
            case "money":
                foreach (var money in map.GetEntities<Money>())
                    map.RemoveObject(money);

                break;
            case "groundItems":
            case "items":
                foreach (var groundItem in map.GetEntities<GroundItem>())
                    map.RemoveObject(groundItem);

                break;
            case "monsters":
                foreach (var monster in map.GetEntities<Monster>())
                    map.RemoveObject(monster);

                break;
            case "all":
                foreach (var groundEntity in map.GetEntities<GroundEntity>())
                    map.RemoveObject(groundEntity);

                foreach (var monster in map.GetEntities<Monster>())
                    map.RemoveObject(monster);

                break;
        }
    }
}
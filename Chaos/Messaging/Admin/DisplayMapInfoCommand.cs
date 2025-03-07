using Chaos.Collections;
using Chaos.Collections.Common;
using Chaos.Common.Definitions;
using Chaos.DarkAges.Definitions;
using Chaos.Messaging.Abstractions;
using Chaos.Models.World;
using Chaos.Storage.Abstractions;

namespace Chaos.Messaging.Admin;
[Command("displayMapInfo", true)]

public class DisplayMapInfoCommand : ICommand<Aisling>
{
    
    private readonly ISimpleCache SimpleCache;

    public DisplayMapInfoCommand(ISimpleCache simpleCache) => SimpleCache = simpleCache;

    public ValueTask ExecuteAsync(Aisling source, ArgumentCollection args)
    {
        var mapInstance = SimpleCache.Get<MapInstance>(source.MapInstance.InstanceId);

        source.Client.SendServerMessage(ServerMessageType.ScrollWindow,
            "Name: "
            +mapInstance.Name
            +"\nTemplateKey: "
            +mapInstance.Template.TemplateKey
            +"\nMusic: "
            +mapInstance.Music
            +"\nInstanceId: "
            +mapInstance.InstanceId
            +"\nMax Level: "
            +mapInstance.MaximumLevel
            +"\nMin Level: "
            +mapInstance.MinimumLevel
            +"\nShard: "
            +mapInstance.IsShard
            +"\nX: "
            +mapInstance.Template.Height
            +"\nY: "
            +mapInstance.Template.Width);

        return default;
    }
}
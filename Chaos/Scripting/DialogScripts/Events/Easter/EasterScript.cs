using Chaos.Extensions;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Scripting.ReactorTileScripts.Events;
using Chaos.Scripting.ReactorTileScripts.Events.Easter;
using Chaos.Services.Storage.Abstractions;

namespace Chaos.Scripting.DialogScripts.Events.Easter;

public class EasterScript : DialogScriptBase
{
    /// <inheritdoc />
    public EasterScript(Dialog subject, IShardGenerator shardGenerator)
        : base(subject) =>
        ShardGenerator = shardGenerator;

    private readonly IShardGenerator ShardGenerator;
    
    /// <inheritdoc />
    public override void OnDisplaying(Aisling source)
    {
        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "cadburry_accept":
            {
                var shard = ShardGenerator.CreateShardOfInstance("hopmaze");
                shard.Shards.TryAdd(shard.InstanceId, shard);
                var script = shard.Script.As<HopocalypseScript>();

                if (script == null)
                    shard.AddScript<HopocalypseScript>();

                if (source.Inventory.ContainsByTemplateKey("undinechickenegg"))
                    source.Inventory.RemoveByTemplateKey("undinechickenegg");

                
                if (source.Inventory.ContainsByTemplateKey("undinegoldenchickenegg"))
                    source.Inventory.RemoveByTemplateKey("undinegoldenchickenegg");
                
                source.TraverseMap(shard, new Point(10,14));
                Subject.Close(source);
                break;
            }
        }
    }
}
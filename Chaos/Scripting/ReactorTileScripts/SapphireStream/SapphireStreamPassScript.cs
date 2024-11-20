using Chaos.Collections;
using Chaos.Common.Definitions;
using Chaos.DarkAges.Definitions;
using Chaos.Extensions.Geometry;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.ReactorTileScripts.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.ReactorTileScripts.SapphireStream;

public class SapphireStreamPassScript : ReactorTileScriptBase
{
    private readonly ISimpleCache SimpleCache;

    /// <inheritdoc />
    public SapphireStreamPassScript(ReactorTile subject, ISimpleCache simpleCache)
        : base(subject) =>
        SimpleCache = simpleCache;

    /// <inheritdoc />
    public override void OnWalkedOn(Creature source)
    {
        if (source is Aisling aisling && !aisling.UserStatSheet.BaseClass.Equals(BaseClass.Monk))
        {
            aisling.SendOrangeBarMessage("You must be of the Monk class to access this area.");

            var point = source.DirectionalOffset(source.Direction.Reverse());
            source.WarpTo(point);

            return;
        }
        
        var targetMap = SimpleCache.Get<MapInstance>("sapphire_stream");
        source.TraverseMap(targetMap, new Point(13, 11));
    }
}
using Chaos.Common.Definitions;
using Chaos.Containers;
using Chaos.Extensions.Geometry;
using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;
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
        var targetMap = SimpleCache.Get<MapInstance>("sapphire_stream");

        if (source is Aisling aisling && !aisling.UserStatSheet.BaseClass.Equals(BaseClass.Monk))
        {
            aisling.SendOrangeBarMessage("You must be of the Monk class to access this area.");

            var point = source.DirectionalOffset(source.Direction.Reverse());
            source.WarpTo(point);

            return;
        }

        source.TraverseMap(targetMap, new Point(13, 11));
    }

    #region ScriptVars
    #endregion
}
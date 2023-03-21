using Chaos.Common.Definitions;
using Chaos.Containers;
using Chaos.Extensions.Geometry;
using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripting.ReactorTileScripts.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.ReactorTileScripts.SapphireStream;

public class MinLevelCheckScript : ConfigurableReactorTileScriptBase
{
    private readonly ISimpleCache SimpleCache;

    /// <inheritdoc />
    public MinLevelCheckScript(ReactorTile subject, ISimpleCache simpleCache)
        : base(subject) =>
        SimpleCache = simpleCache;

    /// <inheritdoc />
    public override void OnWalkedOn(Creature source)
    {
        if (source is not Aisling aisling)
            return;

        if (MinimumLevel.HasValue && (MinimumLevel.Value > source.StatSheet.Level))
        {
            aisling.SendOrangeBarMessage($"You must be level {MinimumLevel} to enter");

            var point = source.DirectionalOffset(source.Direction.Reverse());
            source.WarpTo(point);
        }
    }

    #region ScriptVars
    public int? MinimumLevel { get; set; }
    #endregion
}
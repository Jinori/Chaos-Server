using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.ReactorTileScripts.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.ReactorTileScripts
{
    public class SameMapWarpScript : ConfigurableReactorTileScriptBase
    {
        #region ScriptVars
        public Point Points { get; init; }
        #endregion

        /// <inheritdoc />
        public SameMapWarpScript(ReactorTile subject, ISimpleCache simpleCache)
            : base(subject) { }

        /// <inheritdoc />
        public override void OnWalkedOn(Creature source)
        {
            source.WarpTo(Points);
        }
    }
}
using Chaos.Definitions;
using Chaos.Models.World;
using Chaos.Scripting.ReactorTileScripts.Abstractions;

namespace Chaos.Scripting.ReactorTileScripts.BookScripts;

public sealed class CookBookScript : ReactorTileScriptBase
{
    /// <inheritdoc />
    public CookBookScript(ReactorTile subject)
        : base(subject) { }

    /// <inheritdoc />
    public override void OnClicked(Aisling source)
    {
        if (!source.Trackers.Flags.HasFlag(CookingRecipes.Pie))
        {
            source.Trackers.Flags.AddFlag(CookingRecipes.Pie);
            source.SendOrangeBarMessage("You found a cook book! You can now make pie!");

            return;
        }

        source.SendOrangeBarMessage("You found a cook book! But you already know how to make pie.");
    }
}
using Chaos.Common.Definitions;
using Chaos.Definitions;
using Chaos.Models.World;
using Chaos.Scripting.ReactorTileScripts.Abstractions;

namespace Chaos.Scripting.ReactorTileScripts.BookScripts;

public sealed class AlchemyBookScript : ReactorTileScriptBase
{
    /// <inheritdoc />
    public AlchemyBookScript(ReactorTile subject)
        : base(subject) {}

    /// <inheritdoc />
    public override void OnClicked(Aisling source)
    {
        if (!source.Trackers.Flags.HasFlag(AlchemyRecipes.AntidotePotion))
        {
            source.Trackers.Flags.AddFlag(AlchemyRecipes.AntidotePotion);
            source.SendOrangeBarMessage("You found an Alchemy Book! You can now make an Antidote Potion!");

            return;
        }
        source.SendOrangeBarMessage("You found an Alchemy Book, but you already know Antidote Potion.");
        return;
    }
}
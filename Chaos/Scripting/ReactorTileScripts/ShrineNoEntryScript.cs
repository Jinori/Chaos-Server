using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.ReactorTileScripts.Abstractions;

namespace Chaos.Scripting.ReactorTileScripts;

public class ShrineNoEntryScript : ReactorTileScriptBase
{
    /// <inheritdoc />
    public ShrineNoEntryScript(ReactorTile subject)
        : base(subject) { }

    /// <inheritdoc />
    public override void OnWalkedOn(Creature source)
    {
        if (source is not Aisling aisling)
            return;

        if (new Point(8, 19).Equals(Subject) || new Point(8, 18).Equals(Subject))
        {
            aisling.SendOrangeBarMessage("This shrine belonged to Aquaedon, best not to enter.");

            return;
        }

        if (new Point(8, 5).Equals(Subject) || new Point(8, 4).Equals(Subject))
        {
            aisling.SendOrangeBarMessage("This shrine belonged to Geolith, best not to enter.");

            return;
        }

        if (new Point(7, 5).Equals(Subject) || new Point(7, 6).Equals(Subject) || new Point(7, 7).Equals(Subject))
            aisling.SendOrangeBarMessage("This shrine belonged to Zephyra, best not to enter.");
    }
}
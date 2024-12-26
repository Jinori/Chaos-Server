using Chaos.Definitions;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Scripting.ItemScripts.Abstractions;

namespace Chaos.Scripting.ItemScripts;

public class DunanMountItemScript : ItemScriptBase
{
    public DunanMountItemScript(Item subject)
        : base(subject) { }

    public override bool CanUse(Aisling source)
    {
        if (source.Trackers.Flags.HasFlag(AvailableMounts.Dunan))
        {
            source.SendOrangeBarMessage("You already have this mount.");

            return false;
        }

        return true;
    }

    public override void OnUse(Aisling source)
    {
        source.Trackers.Flags.AddFlag(AvailableMounts.Dunan);
        source.SendOrangeBarMessage("You now have the Dunan mount!");
        source.Inventory.RemoveQuantity(Subject.DisplayName, 1);
    }
}
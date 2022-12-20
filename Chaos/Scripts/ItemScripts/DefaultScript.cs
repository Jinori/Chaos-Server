using Chaos.Extensions;
using Chaos.Objects.Panel;
using Chaos.Objects.World;
using Chaos.Scripts.ItemScripts.Abstractions;

namespace Chaos.Scripts.ItemScripts;

public class DefaultScript : ItemScriptBase
{
    public DefaultScript(Item subject)
        : base(subject) { }

    public override void OnUse(Aisling source) => source.SendOrangeBarMessage("You can't use that");
    public override void OnPickup(Aisling aisling)
    {
        if (aisling.Group!.Count > 1)
        {
            var point = new Point(aisling.X, aisling.Y);
            var group = aisling.Group?.Where(x => x.WithinRange(point));
            foreach (var member in group!)
            {
                member.SendOrangeBarMessage($"{aisling.Name} has picked up {Subject.DisplayName}.");
            }
        }
        if (aisling.Group is null)
        {
            if (Subject.Count.Equals(1))
                aisling.SendOrangeBarMessage($"You picked up {Subject.DisplayName}.");

            if (Subject.Count > 1)
                aisling.SendOrangeBarMessage($"You picked up {Subject.Count} of {Subject.DisplayName}.");
        }
    }
}
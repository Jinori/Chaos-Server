using Chaos.Extensions;
using Chaos.Objects.Panel;
using Chaos.Objects.World;
using Chaos.Scripts.ItemScripts.Abstractions;

namespace Chaos.Scripts.ItemScripts
{
    public class PickupNotifierScript : ItemScriptBase
    {
        public PickupNotifierScript(Item subject) : base(subject)
        {

        }
        public override void OnPickup(Aisling aisling)
        {

            if (aisling.Group is null)
            {
                if (Subject.Count.Equals(1))
                    aisling.SendOrangeBarMessage($"You picked up {Subject.DisplayName}.");

                if (Subject.Count > 1)
                    aisling.SendOrangeBarMessage($"You picked up {Subject.Count} of {Subject.DisplayName}.");

                return;
            }

            if (aisling.Group!.Count > 1)
            {
                var point = new Point(aisling.X, aisling.Y);
                var group = aisling.Group?.Where(x => x.WithinRange(point) && x.MapInstance.Equals(aisling.MapInstance));
                foreach (var member in group!)
                {
                    member.SendOrangeBarMessage($"{aisling.Name} has picked up {Subject.DisplayName}.");
                }
            }
        }
    }
}

using Chaos.Extensions;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.ItemScripts.Abstractions;

namespace Chaos.Scripting.ItemScripts
{
    public class PickupNotifierScript : ItemScriptBase
    {
        public PickupNotifierScript(Item subject)
            : base(subject) { }

        public override void OnPickup(Aisling aisling)
        {
            NotifyPlayer(aisling);
            NotifyNearbyPlayers(aisling);
            NotifyGroupMembers(aisling);
        }
        
        private void NotifyPlayer(Aisling aisling)
        {
            var message = Subject.Count.Equals(1)
                ? $"You picked up {Subject.DisplayName}."
                : $"You picked up {Subject.Count} of {Subject.DisplayName}.";

            aisling.SendOrangeBarMessage(message);
        }

        private void NotifyNearbyPlayers(NamedEntity aisling)
        {
            var nearbyPlayers = aisling.MapInstance
                                       .GetEntitiesWithinRange<Aisling>(aisling, 8)
                                       .Where(x => aisling.Id != x.Id).ToList();
            
            if (!nearbyPlayers.Any())
                return;
            
            foreach (var player in nearbyPlayers)
            {
                player.SendOrangeBarMessage($"{aisling.Name} has picked up {Subject.DisplayName}.");
            }
        }

        private void NotifyGroupMembers(Aisling aisling)
        {
            if (aisling.Group is { Count: > 1 })
            {
                var point = new Point(aisling.X, aisling.Y);
                var groupMembersInRange = aisling.Group
                                                 .Where(x => x.WithinRange(point) && x.MapInstance.Equals(aisling.MapInstance));

                foreach (var member in groupMembersInRange)
                {
                    member.SendOrangeBarMessage($"{aisling.Name} has picked up {Subject.DisplayName}.");
                }
            }
        }
    }
}
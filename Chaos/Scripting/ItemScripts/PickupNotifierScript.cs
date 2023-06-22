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

        public override void OnPickup(Aisling aisling, Item originalItem, int originalCount)
        {
            NotifyPlayer(aisling, originalItem, originalCount);
            NotifyNearbyPlayers(aisling, originalItem, originalCount);
            NotifyGroupMembers(aisling, originalItem, originalCount);
        }
        
        private void NotifyPlayer(Aisling aisling, Item originalItem, int originalCount)
        {
            var message = Subject.Count.Equals(1)
                ? $"You picked up {Subject.DisplayName}."
                : $"You picked up {originalCount} of {Subject.DisplayName}.";

            aisling.SendOrangeBarMessage(message);
        }

        private void NotifyNearbyPlayers(NamedEntity aisling, Item originalItem, int originalCount)
        {
            var nearbyPlayers = aisling.MapInstance
                                       .GetEntitiesWithinRange<Aisling>(aisling, 8)
                                       .Where(x => aisling.Id != x.Id).ToList();
            
            if (!nearbyPlayers.Any())
                return;
            
            foreach (var player in nearbyPlayers)
            {
                player.SendOrangeBarMessage(
                    originalCount > 1
                        ? $"{aisling.Name} has picked up {originalCount} {Subject.DisplayName}."
                        : $"{aisling.Name} has picked up {Subject.DisplayName}.");
            }
        }

        private void NotifyGroupMembers(Aisling aisling, Item originalItem, int originalCount)
        {
            if (aisling.Group is { Count: > 1 })
            {
                var point = new Point(aisling.X, aisling.Y);
                var groupMembersInRange = aisling.Group
                                                 .Where(x => x.WithinRange(point) && x.MapInstance.Equals(aisling.MapInstance));

                foreach (var member in groupMembersInRange)
                {
                    member.SendOrangeBarMessage(
                        originalCount > 1
                            ? $"{aisling.Name} has picked up {originalCount} {Subject.DisplayName}."
                            : $"{aisling.Name} has picked up {Subject.DisplayName}.");
                }
            }
        }
    }
}
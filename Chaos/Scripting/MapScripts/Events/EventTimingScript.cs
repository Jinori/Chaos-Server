using Chaos.Collections;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.MapScripts.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.MapScripts.Events
{
    public class EventTimingScript(MapInstance subject, ISimpleCache simpleCache) : MapScriptBase(subject)
    {
        public override void OnEntered(Creature creature)
        {
            if (creature is not Aisling aisling)
                return;

            // Check if the current map has any active events
            var isEventActive = EventPeriod.IsEventActive(DateTime.UtcNow, Subject.InstanceId);

            // Send the player home if no events are active for the current map and the player is not an admin
            if (!isEventActive && !aisling.IsAdmin)
            {
                SendToTemple(aisling);
            }
        }

        /// <summary>
        /// Sends the specified aisling home.
        /// </summary>
        /// <param name="aisling">The aisling to send home.</param>
        private void SendToTemple(Aisling aisling)
        {
            aisling.SendOrangeBarMessage("This area is only accessible during specific event periods.");
            var defaultMapInstance = simpleCache.Get<MapInstance>("toc");
            aisling.TraverseMap(defaultMapInstance, new Point(8, 5));
        }
    }
}
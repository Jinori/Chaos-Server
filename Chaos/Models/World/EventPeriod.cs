
namespace Chaos.Models.World
{
    /// <summary>
    /// Represents an event with a start and end date and associated maps.
    /// </summary>
    public class EventPeriod(DateTime startDate, DateTime endDate, List<string> associatedMaps)
    {
        private DateTime StartDate { get; } = startDate;
        private DateTime EndDate { get; } = endDate;
        private List<string> AssociatedMaps { get; } = associatedMaps ?? throw new ArgumentNullException(nameof(associatedMaps));

        /// <summary>
        /// Checks if the current date falls within the event period.
        /// </summary>
        /// <param name="currentDate">The current date to check.</param>
        /// <returns>True if the current date is within the event period, otherwise false.</returns>
        private bool IsActive(DateTime currentDate)
        {
            return currentDate >= StartDate && currentDate <= EndDate;
        }

        /// <summary>
        /// Initializes a centralized list of all events with their date ranges and associated maps.
        /// </summary>
        /// <returns>A list of event periods.</returns>
        private static List<EventPeriod> GetAllEvents()
        {
            return
            [
                //Christmas Events and associated map instance id's
                new EventPeriod(
                    new DateTime(DateTime.UtcNow.Year, 12, 15),
                    new DateTime(DateTime.UtcNow.Year + 1, 1, 2),
                    [
                        "mtmerry_northpole", "elf_room", "toy_shop", "santas_room", "reindeer_pen",
                        "lift_room", "mtmerry_battleground_98", "mtmerry_battleground_gmmaster",
                        "mtmerry_battleground_master", "mtmerry_frostychallenge", "mtmerry1-1",
                        "mtmerry1-2", "mtmerry1-3", "mtmerry2-1", "mtmerry2-2", "mtmerry2-3",
                        "mtmerry3-1", "mtmerry3-2", "mtmerry3-3", "mtmerry4-1", "mtmerry4-2",
                        "mtmerry4-3", "mtmerry5-1", "mtmerry5-2", "mtmerry5-3"
                    ]
                )
            ];
        }

        /// <summary>
        /// Checks if the current date and map instance fall within any event period.
        /// </summary>
        /// <param name="currentDate">The current date.</param>
        /// <param name="currentMapInstanceId">The map instance ID to check.</param>
        /// <returns>True if any event is active for the given map and date; otherwise, false.</returns>
        public static bool IsEventActive(DateTime currentDate, string currentMapInstanceId)
        {
            return GetAllEvents().Any(eventPeriod =>
                eventPeriod.IsActive(currentDate) &&
                eventPeriod.AssociatedMaps.Contains(currentMapInstanceId));
        }
    }
}

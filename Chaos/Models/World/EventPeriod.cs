using Cronos;

namespace Chaos.Models.World
{
    public class EventPeriod
    {
        private DateTime StartDate { get; }
        private DateTime EndDate { get; }
        private List<string> AssociatedMaps { get; }

        private EventPeriod(string startCronExpression, string endCronExpression, List<string> associatedMaps)
        {
            var currentYear = DateTime.UtcNow.Year;

            StartDate = CronExpression.Parse(startCronExpression)?
                            .GetNextOccurrence(new DateTime(currentYear, 1, 1, 0, 0, 0, DateTimeKind.Utc)) 
                        ?? throw new InvalidOperationException($"Failed to parse start cron expression: {startCronExpression}");

            EndDate = CronExpression.Parse(endCronExpression)?
                          .GetNextOccurrence(new DateTime(currentYear, 1, 1, 0, 0, 0, DateTimeKind.Utc)) 
                      ?? throw new InvalidOperationException($"Failed to parse end cron expression: {endCronExpression}");

            AssociatedMaps = associatedMaps ?? throw new ArgumentNullException(nameof(associatedMaps));
        }


        /// <summary>
        /// Checks if the current date falls within the event period.
        /// </summary>
        private bool IsActive(DateTime currentDate)
        {
            return currentDate >= StartDate && currentDate <= EndDate;
        }

        /// <summary>
        /// Retrieves all events with their cron expressions.
        /// </summary>
        private static List<EventPeriod> GetAllEvents()
        {
            return
            [
                //Mt Merry Events
                new EventPeriod(
                    "0 17 * 12 5#2", // Second Friday of December at 5 PM
                    "0 5 * 1 1#1", // First Monday of January at 5 AM
                    [
                        "mtmerry_northpole", "elf_room", "toy_shop", "santas_room", "reindeer_pen",
                        "lift_room", "mtmerry_battleground_98", "mtmerry_battleground_gmmaster",
                        "mtmerry_battleground_master", "mtmerry_frostychallenge", "mtmerry1-1",
                        "mtmerry1-2", "mtmerry1-3", "mtmerry2-1", "mtmerry2-2", "mtmerry2-3",
                        "mtmerry3-1", "mtmerry3-2", "mtmerry3-3", "mtmerry4-1", "mtmerry4-2",
                        "mtmerry4-3", "mtmerry5-1", "mtmerry5-2", "mtmerry5-3"
                    ]
                ),
                //Valentines
                new EventPeriod(
                    "0 0 * 2 3#2", // Second Wednesday of February at 12 AM
                    "0 6 * 2 4#3", // Thursday after the third Wednesday of February at 6 AM
                    ["loures_castle_way"]
                )
            ];
        }

        /// <summary>
        /// Checks if an event is active for a specific map instance.
        /// </summary>
        public static bool IsEventActive(DateTime currentDate, string currentMapInstanceId)
        {
            return GetAllEvents()
                .Where(eventPeriod => eventPeriod.AssociatedMaps.Contains(currentMapInstanceId))
                .Any(eventPeriod => eventPeriod.IsActive(currentDate));
        }

    }
}

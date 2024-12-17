using NCrontab;

namespace Chaos.Models.World
{
    public class EventPeriod(
        string startCronExpression,
        string endCronExpression,
        int year,
        List<string> associatedMaps)
    {
        private DateTime StartDate { get; } = ParsePreprocessedCronExpression(startCronExpression, year);
        private DateTime EndDate { get; } = ParsePreprocessedCronExpression(endCronExpression, year + 1);
        public List<string> AssociatedMaps { get; } = associatedMaps;

        public bool IsActive(DateTime currentDate)
        {
            return currentDate >= StartDate && currentDate <= EndDate;
        }

        /// <summary>
        /// Checks if an event is active for the current date and map instance.
        /// </summary>
        /// <param name="currentDate">The current UTC date.</param>
        /// <param name="currentMapInstanceId">The current map instance ID.</param>
        /// <returns>True if the event is active; otherwise false.</returns>
        public static bool IsEventActive(DateTime currentDate, string currentMapInstanceId)
        {
            var allEvents = GetAllEvents();

            // Check if the current date is within any active event
            return allEvents.Any(eventPeriod =>
                eventPeriod.IsActive(currentDate) &&
                eventPeriod.AssociatedMaps.Contains(currentMapInstanceId));
        }


        private static List<EventPeriod> GetAllEvents()
        {
            var currentYear = DateTime.UtcNow.Year;

            return
            [
                new EventPeriod(
                    startCronExpression: "0 17 * 12 5#2", // Second Friday of December at 5 PM
                    endCronExpression: "0 5 * 1 1#1", // First Monday of January at 5 AM
                    year: currentYear,
                    associatedMaps:
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
        /// Parses a cron expression after preprocessing unsupported patterns like "5#2".
        /// </summary>
        private static DateTime ParsePreprocessedCronExpression(string cronExpression, int year)
        {
            var preprocessedCron = PreprocessCronExpression(cronExpression, year);
            var baseDate = new DateTime(year, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var schedule = CrontabSchedule.Parse(preprocessedCron);
            return schedule.GetNextOccurrence(baseDate);
        }

        /// <summary>
        /// Converts unsupported cron patterns (e.g., 5#2) into exact dates.
        /// </summary>
        private static string PreprocessCronExpression(string cronExpression, int year)
        {
            // Look for patterns like "5#2" in the cron expression
            var parts = cronExpression.Split(' ');
            if (parts.Length == 5 && parts[4].Contains("#"))
            {
                var nthOccurrenceParts = parts[4].Split('#');
                var dayOfWeek = int.Parse(nthOccurrenceParts[0]);
                var occurrence = int.Parse(nthOccurrenceParts[1]);

                // Calculate the exact date of the nth occurrence
                var month = int.Parse(parts[3]);
                var day = GetNthDayOfWeek(year, month, (DayOfWeek)dayOfWeek, occurrence).Day;

                // Replace the cron expression with the exact day
                parts[2] = day.ToString(); // Day of the month
                parts[4] = "*";            // Reset day of the week

                return string.Join(' ', parts);
            }

            return cronExpression; // Return as-is if no preprocessing is needed
        }

        /// <summary>
        /// Calculates the nth occurrence of a specific day of the week in a given month and year.
        /// </summary>
        private static DateTime GetNthDayOfWeek(int year, int month, DayOfWeek dayOfWeek, int occurrence)
        {
            var date = new DateTime(year, month, 1);

            while (date.DayOfWeek != dayOfWeek)
                date = date.AddDays(1);

            date = date.AddDays((occurrence - 1) * 7);
            return date;
        }
    }
}

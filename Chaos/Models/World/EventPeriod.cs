using Chaos.Extensions.Common;
using Cronos;

namespace Chaos.Models.World;

public class EventPeriod
{
    private List<string> AssociatedMaps { get; }
    private DateTime EndDate { get; }
    private DateTime StartDate { get; }

    private EventPeriod(string startCronExpression, string endCronExpression, List<string> associatedMaps)
    {
        var currentYear = DateTime.UtcNow.Year;

        // Try to get the start date for the current year and the previous year, select the latest occurrence that's not in the future
        var startDateThisYear = CronExpression.Parse(startCronExpression)
                                              ?.GetNextOccurrence(
                                                  new DateTime(
                                                      currentYear,
                                                      1,
                                                      1,
                                                      0,
                                                      0,
                                                      0,
                                                      DateTimeKind.Utc));

        var startDateLastYear = CronExpression.Parse(startCronExpression)
                                              ?.GetNextOccurrence(
                                                  new DateTime(
                                                      currentYear - 1,
                                                      1,
                                                      1,
                                                      0,
                                                      0,
                                                      0,
                                                      DateTimeKind.Utc));

        StartDate = startDateThisYear.HasValue && (startDateThisYear.Value <= DateTime.UtcNow)
            ? startDateThisYear.Value
            : startDateLastYear.HasValue
                ? startDateLastYear.Value
                : throw new InvalidOperationException($"Failed to parse start cron expression: {startCronExpression}");

        // Calculate end date based on the start date
        var potentialEndDate = CronExpression.Parse(endCronExpression)
                                             ?.GetNextOccurrence(StartDate)
                               ?? throw new InvalidOperationException($"Failed to parse end cron expression: {endCronExpression}");

        EndDate = potentialEndDate < StartDate ? potentialEndDate.AddYears(1) : potentialEndDate;

        AssociatedMaps = associatedMaps ?? throw new ArgumentNullException(nameof(associatedMaps));
    }

    /// <summary>
    ///     Retrieves all events with their cron expressions.
    /// </summary>
    private static List<EventPeriod> GetAllEvents()
        =>
        [
            new(
                "0 17 * 12 5#2", // Second Friday of December at 5 PM
                "0 5 * 1 1#1", // First Monday of January at 5 AM
                [
                    "mtmerry_northpole",
                    "elf_room",
                    "toy_shop",
                    "santas_room",
                    "reindeer_pen",
                    "lift_room",
                    "mtmerry_battleground_98",
                    "mtmerry_battleground_gmmaster",
                    "mtmerry_battleground_master",
                    "mtmerry_frostychallenge",
                    "mtmerry1-1",
                    "mtmerry1-2",
                    "mtmerry1-3",
                    "mtmerry2-1",
                    "mtmerry2-2",
                    "mtmerry2-3",
                    "mtmerry3-1",
                    "mtmerry3-2",
                    "mtmerry3-3",
                    "mtmerry4-1",
                    "mtmerry4-2",
                    "mtmerry4-3",
                    "mtmerry5-1",
                    "mtmerry5-2",
                    "mtmerry5-3",
                    "santachallenge"
                ]),

            //New Years
            new(
                "0 12 31 12 *", // December 31st at 12:00 PM
                "0 12 2 1 *", // January 2st at 12:00 AM
                ["rucesion"] // Replace with your actual map identifiers
            ),

            // Valentines

            new(
                "0 6 * 2 1#2", // Second Monday of February at 6 AM
                "0 6 * 2 1#3", // Third Monday of February at 6 AM
                ["loures_castle_way", "shinewood_forest_entrance", "loures_1_floor_restaurant", "loures_2_floor_restaurant", "loures_3_floor_office", "loures_3_floor_magic_room"]
            ),
            
            
            // St. Patrick’s Event (March 14 - March 19)
            new(
                "0 6 14 3 *", // March 14th at 6 AM
                "0 6 19 3 *", // March 19th at 6 AM
                [
                    "enchanted_garden", "crossroads", "astrid_center", "astrid_north", "astrid_north_east", "astrid_north_west",
                    "astrid_south", "astrid_south_east", "astrid_south_west", "mount_giragan1", "mount_giragan2", "mount_giragan3", "mount_giragan4",
                    "piet_sewer_floor_1", "piet_sewer_floor_2", "piet_sewer_floor_3", "piet_sewer_floor_4", "crypt1", "crypt2", "crypt3", "crypt4", 
                    "manor_floor_1", "manor_floor_2", "manor_floor_3", "nobis", "lynith_beach_south","lynith_beach_north1", "lynith_beach_north2", "mileth_village_way"
                ]
            ),
            
            // Easter Event (April 14 - April 28)
            // Easter Event (Testing: March 28 - March 31)
            new(
                "0 6 28 3 *", // March 28th at 6 AM
                "0 6 31 3 *", // March 31st at 6 AM
                [
                    "undine", "hopmaze"
                ]
            ),
        ];

    /// <summary>
    ///     Checks if the current date falls within the event period.
    /// </summary>
    private bool IsActive(DateTime currentDate) => (currentDate >= StartDate) && (currentDate <= EndDate);

    /// <summary>
    ///     Checks if an event is active for a specific map instance.
    /// </summary>
    public static bool IsEventActive(DateTime currentDate, string currentMapInstanceId)
    {
        var events = GetAllEvents();

        foreach (var eventPeriod in events.Where(eventPeriod => eventPeriod.AssociatedMaps.ContainsI(currentMapInstanceId)))
        {
            if (!eventPeriod.IsActive(currentDate))
                continue;

            return true;
        }

        return false;
    }
}
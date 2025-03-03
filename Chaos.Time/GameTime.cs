using Chaos.DarkAges.Definitions;

namespace Chaos.Time;

/// <summary>
///     A <see cref="System.DateTime" /> replacement that runs at 24x speed, to be used for in-game time measurement.
/// </summary>
public readonly struct GameTime : IComparable, IComparable<GameTime>, IEquatable<GameTime>
{
    private readonly DateTime DateTime;

    /// <summary>
    ///     Gets the day component of the game time.
    /// </summary>
    public int Day => DateTime.Day;

    /// <summary>
    ///     Gets the month component of the game time.
    /// </summary>
    public int Month => DateTime.Month;

    /// <summary>
    ///     Gets the year component of the game time.
    /// </summary>
    public int Year => DateTime.Year;

    /// <summary>
    ///     Gets the hour component of the game time.
    /// </summary>
    public int Hour => DateTime.Hour;

    /// <summary>
    ///     Gets the minute component of the game time.
    /// </summary>
    public int Minute => DateTime.Minute;

    /// <summary>
    ///     Gets the current in-game time.
    /// </summary>
    public static GameTime Now => FromDateTime(DateTime.UtcNow);

    /// <summary>
    ///     Returns the appropriate level of light for the time of day.
    /// </summary>
    public LightLevel TimeOfDay =>
        Hour switch
        {
            >= 10 and <= 17 => LightLevel.Lightest_A,
            >= 9 and <= 18  => LightLevel.Lighter_A,
            >= 8 and <= 19  => LightLevel.Light_A,
            >= 7 and <= 20  => LightLevel.Dark_A,
            >= 6 and <= 21  => LightLevel.Darker_A,
            _               => LightLevel.Darkest_A
        };

    /// <summary>
    ///     Returns the in-game time formatted to match the LEGEND log style.
    /// </summary>
    public string ToLegendFormat()
    {
        var season = GetCurrentSeason();
        return $"Termina {Year}, {season}";
    }

    /// <summary>
    ///     Generates a detailed time string including season, moon phase, and formatted date.
    /// </summary>
    public string GetDetailedTimeInfo()
    {
        var season = GetCurrentSeason();
        var moonPhase = GetMoonPhase();
    
        var hour12 = Hour % 12 == 0 ? 12 : Hour % 12;
        var amPm = Hour >= 12 ? "PM" : "AM";
        var fantasyMonth = GetFantasyMonthName(Month);

        return $"Termina {Year}, {Day}{GetDaySuffix} of {fantasyMonth}, " +
               $"{hour12:00}:{Minute:00} {amPm} ({season}) - {moonPhase}";
    }

    /// <summary>
    ///     Converts a numeric month into a fantasy month name.
    /// </summary>
    /// <param name="month">The numeric representation of the month (1-12).</param>
    /// <returns>The corresponding fantasy month name.</returns>
    private string GetFantasyMonthName(int month) =>
        month switch
        {
            1  => "Embris",
            2  => "Ironveil",
            3  => "Galesyn",
            4  => "Tidesol",
            5  => "Crimarc",
            6  => "Halcyra",
            7  => "Veilren",
            8  => "Whimris",
            9  => "Withren",
            10 => "Duskrun",
            11 => "Noctis",
            12 => "Aurelia",
            _  => "Unknown"
        };

    /// <summary>
    ///     Determines the current season based on the in-game month.
    /// </summary>
    /// <returns>The name of the current season.</returns>
    private string GetCurrentSeason() =>
        Month switch
        {
            1 or 2 or 3 => "Spring",
            4 or 5 or 6 => "Summer",
            7 or 8 or 9 => "Autumn",
            _           => "Winter"
        };

    /// <summary>
    ///     Determines the current moon phase based on the in-game day.
    /// </summary>
    /// <returns>The name of the current moon phase.</returns>
    private string GetMoonPhase()
    {
        var moonCycle = (Now.ToDateTime() - Origin).TotalDays % 28; // 28-day moon cycle
        return moonCycle switch
        {
            < 7  => "Veiled Moon",
            < 14 => "Crescent Ascent",
            < 21 => "High Moon",
            _    => "Fading Crescent"
        };
    }

    /// <summary>
    ///     Returns the proper suffix for a day.
    /// </summary>
    public string GetDaySuffix =>
        (((Day % 10) == 1) && (Day != 11)) ? "st" :
        (((Day % 10) == 2) && (Day != 12)) ? "nd" :
        (((Day % 10) == 3) && (Day != 13)) ? "rd" : "th";

    /// <summary>
    ///     Converts a GameTime object to a real-world DateTime.
    /// </summary>
    /// <returns>A DateTime representation of the GameTime.</returns>
    public DateTime ToDateTime() => new(DateTime.Ticks / 24 + Origin.Ticks, DateTimeKind.Utc);

    /// <summary>
    ///     Converts a DateTime object to GameTime.
    /// </summary>
    /// <param name="dTime">The DateTime object to be converted.</param>
    /// <returns>A GameTime instance representing the same point in time.</returns>
    public static GameTime FromDateTime(DateTime dTime)
        => new(dTime.Subtract(Origin).Ticks * 24);

    /// <summary>
    ///     The starting date of the game world.
    /// </summary>
    private static DateTime Origin { get; } = new(2024, 4, 26, 23, 0, 0, DateTimeKind.Utc);
    
    /// <summary>
    ///     Initializes a new instance of the <see cref="GameTime"/> struct using ticks.
    /// </summary>
    /// <param name="ticks">The number of ticks representing the time.</param>
    public GameTime(long ticks) : this(new DateTime(ticks)) { }

    /// <summary>
    ///     Initializes a new instance of the <see cref="GameTime"/> struct using a DateTime object.
    /// </summary>
    /// <param name="time">The DateTime object representing the game time.</param>
    public GameTime(DateTime time) => DateTime = time;

    /// <inheritdoc />
    public int CompareTo(GameTime other) => DateTime.CompareTo(other.DateTime);

    /// <inheritdoc />
    public bool Equals(GameTime other) => DateTime.Equals(other.DateTime);

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is GameTime time && Equals(time);

    /// <inheritdoc />
    public override int GetHashCode() => DateTime.GetHashCode();

    /// <inheritdoc />
    public int CompareTo(object? obj)
    {
        if (obj is GameTime gameTime)
        {
            return CompareTo(gameTime);
        }
        throw new ArgumentException("Object is not a GameTime instance.");
    }
}

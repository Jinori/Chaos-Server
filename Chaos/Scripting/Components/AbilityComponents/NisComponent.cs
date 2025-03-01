using Chaos.DarkAges.Definitions;
using Chaos.Models.Data;
using Chaos.Scripting.Components.Abstractions;
using Chaos.Scripting.Components.Execution;
using Chaos.Time;

namespace Chaos.Scripting.Components.AbilityComponents;

public class NisComponent : IComponent
{
    public void Execute(ActivationContext context, ComponentVars vars)
    {
        var options = vars.GetOptions<INisComponentOptions>();
        
        if (options.OutputType != null)
        {
            var gameTime = GameTime.Now;
            var formattedTime = GetDetailedTimeInfo(gameTime);
            context.SourceAisling?.SendServerMessage(options.OutputType.Value, formattedTime);
        }
    }

    /// <summary>
    ///     Generates a detailed time string including season, moon phase, and formatted date.
    /// </summary>
    private string GetDetailedTimeInfo(GameTime gameTime)
    {
        var season = GetCurrentSeason(gameTime);
        var moonPhase = GetMoonPhase(gameTime);
    
        var hour12 = gameTime.Hour % 12 == 0 ? 12 : gameTime.Hour % 12;
        var amPm = gameTime.Hour >= 12 ? "PM" : "AM";

        var fantasyMonth = GetFantasyMonthName(gameTime.Month);

        return $"Year {gameTime.Year}, {gameTime.Day}{gameTime.GetDaySuffix} of {fantasyMonth}, " +
               $"{hour12:00}:{gameTime.Minute:00} {amPm} ({season}) - {moonPhase}";
    }

    /// <summary>
    ///     Converts a numeric month into a fantasy month name.
    /// </summary>
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
            _  => "Unknown" // Fallback in case of invalid input
        };

    /// <summary>
    ///     Determines the current season based on the in-game day.
    /// </summary>
    private string GetCurrentSeason(GameTime gameTime)
    {
        var month = gameTime.Month;
        return month switch
        {
            1 or 2 or 3 => "Spring",
            4 or 5 or 6 => "Summer",
            7 or 8 or 9 => "Autumn",
            _           => "Winter"
        };
    }

    /// <summary>
    ///     Determines the current moon phase based on the in-game day.
    /// </summary>
    private string GetMoonPhase(GameTime gameTime)
    {
        var moonCycle = gameTime.Day % 28; // 28-day moon cycle
        return moonCycle switch
        {
            < 7  => "Veiled Moon",
            < 14 => "Crescent Ascent",
            < 21 => "High Moon",
            _    => "Fading Crescent"
        };
    }

    /// <summary>
    ///     Returns the proper suffix for a month, based on its number.
    /// </summary>
    private string GetMonthSuffix(int month) =>
        month switch
        {
            1 => "st",
            2 => "nd",
            3 => "rd",
            _ => "th"
        };
    

    public interface INisComponentOptions
    {
        ServerMessageType? OutputType { get; init; }
    }
}

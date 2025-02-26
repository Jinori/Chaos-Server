using Chaos.DarkAges.Definitions;
using Chaos.Models.World;

namespace Chaos.Scripting;

public class GameProperties
{
    public int Damage { get; set; }
    public int DamagePerSecond { get; set; }
    public BaseClass BaseClass { get; set; }
    public BaseClass SubbedClass { get; set; }
}


public sealed class DamageGameObj
{
    private static readonly Dictionary<string, string> ClassMapping = new()
    {
        { "priestClass", "priest" },
        { "wizardClass", "wizard" },
        { "monkClass", "monk" },
        { "warriorClass", "warrior" },
        { "rogueClass", "rogue" }
    };

    public Dictionary<string, GameProperties> Entries { get; } = new(StringComparer.OrdinalIgnoreCase);


    public void AddOrUpdateEntry(
        Aisling aisling,
        int damage,
        int damagePerSecond,
        BaseClass baseClass,
        BaseClass subbedClass
    )
    {
        if (Entries.TryGetValue(aisling.Name.ToLower(), out var stats))
        {
            // Update damage if it's higher
            if (damage > stats.Damage)
                stats.Damage = damage;

            // Update DPS if it's higher
            if (damagePerSecond > stats.DamagePerSecond)
                stats.DamagePerSecond = damagePerSecond;

            stats.BaseClass = baseClass;
        }
        else
        {
            // Get the 'Dedicated to' class from the legend
            aisling.Legend.TryGetValue("dedicated", out var existingMark);
            var className = existingMark?.Text.Replace("Dedicated to ", "");

            // Find the first matching class from the ClassMapping dictionary
            var foundClassString = ClassMapping
                                   .Where(entry => aisling.Legend.ContainsKey(entry.Key))
                                   .Select(entry => entry.Value)
                                   .FirstOrDefault();

            // Convert 'Dedicated to' class into BaseClass
            if (!Enum.TryParse<BaseClass>(className, true, out var sub))
                sub = BaseClass.Peasant; // Default if parsing fails

            // Convert foundClassString into BaseClass
            if (!Enum.TryParse<BaseClass>(foundClassString, true, out var foundClass))
                foundClass = BaseClass.Peasant; // Default if parsing fails

            // Add new entry
            Entries[aisling.Name.ToLower()] = new GameProperties
            {
                Damage = damage,
                DamagePerSecond = damagePerSecond,
                BaseClass = sub,
                SubbedClass = foundClass
            };
        }
    }
}

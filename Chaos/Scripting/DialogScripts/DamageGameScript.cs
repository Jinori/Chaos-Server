using System.Text;
using Chaos.Collections;
using Chaos.DarkAges.Definitions;
using Chaos.DarkAges.Extensions;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Storage.Abstractions;
using Humanizer;

namespace Chaos.Scripting.DialogScripts;

public class DamageGameScript : DialogScriptBase
{
    public readonly IStorage<DamageGameObj> DamageLeaderboard;
    private readonly ISimpleCache SimpleCache;

    /// <inheritdoc />
    public DamageGameScript(Dialog subject, ISimpleCache simpleCache, IStorage<DamageGameObj> damageGameStorage)
        : base(subject)
    {
        SimpleCache = simpleCache;
        DamageLeaderboard = damageGameStorage;
    }

    public void DamageGame(Aisling source)
    {
        // Retrieve leaderboard entries
        var leaderboard = DamageLeaderboard.Value.Entries;

        // Build the leaderboard display
        var builder = new StringBuilder();
        builder.AppendLineFColored(MessageColor.Silver, "Damage Game Leaderboard:");
        builder.AppendLine();
        builder.AppendLineFColored(MessageColor.Orange, $"{"Name",-15} {"Damage",-10} {"DPS",-10} {"Class",-10}");
        builder.AppendLineFColored(MessageColor.Gainsboro, new string('-', 45));

        // Order by victories (descending), then losses (ascending)
        var isSilver = true;

        foreach (var entry in leaderboard.OrderByDescending(kvp => kvp.Value.Damage)
                                         .ThenBy(kvp => kvp.Value.DamagePerSecond))
        {
            var stats = entry.Value;
            var newClass = "";

            if (stats.SubbedClass != BaseClass.Peasant)
            {
                //Priest Sub
                if (stats.SubbedClass is BaseClass.Priest && stats.BaseClass is BaseClass.Warrior)
                    newClass = "Faithblade";

                if (stats.SubbedClass is BaseClass.Priest && stats.BaseClass is BaseClass.Monk)
                    newClass = "Druid";

                if (stats.SubbedClass is BaseClass.Priest && stats.BaseClass is BaseClass.Wizard)
                    newClass = "Prizard";

                if (stats.SubbedClass is BaseClass.Priest && stats.BaseClass is BaseClass.Rogue)
                    newClass = "Shadowmender";

                //Warrior Sub
                if (stats.SubbedClass is BaseClass.Warrior && stats.BaseClass is BaseClass.Rogue)
                    newClass = "Shadowblade";

                if (stats.SubbedClass is BaseClass.Warrior && stats.BaseClass is BaseClass.Wizard)
                    newClass = "Spellblade";

                if (stats.SubbedClass is BaseClass.Warrior && stats.BaseClass is BaseClass.Monk)
                    newClass = "Iron Fist";

                if (stats.SubbedClass is BaseClass.Warrior && stats.BaseClass is BaseClass.Priest)
                    newClass = "Paladin";

                //Monk Sub
                if (stats.SubbedClass is BaseClass.Monk && stats.BaseClass is BaseClass.Wizard)
                    newClass = "Spellfist";

                if (stats.SubbedClass is BaseClass.Monk && stats.BaseClass is BaseClass.Priest)
                    newClass = "Spiritwalker";

                if (stats.SubbedClass is BaseClass.Monk && stats.BaseClass is BaseClass.Rogue)
                    newClass = "Shadowhand";

                if (stats.SubbedClass is BaseClass.Monk && stats.BaseClass is BaseClass.Warrior)
                    newClass = "Titan";

                //Wizard Sub
                if (stats.SubbedClass is BaseClass.Wizard && stats.BaseClass is BaseClass.Warrior)
                    newClass = "Arcane Knight";

                if (stats.SubbedClass is BaseClass.Wizard && stats.BaseClass is BaseClass.Monk)
                    newClass = "Arcane Striker";

                if (stats.SubbedClass is BaseClass.Wizard && stats.BaseClass is BaseClass.Priest)
                    newClass = "Arcane Mystic";

                if (stats.SubbedClass is BaseClass.Wizard && stats.BaseClass is BaseClass.Rogue)
                    newClass = "Arcane Stalker";

                if (stats.SubbedClass is BaseClass.Wizard && stats.BaseClass is BaseClass.Rogue)
                    newClass = "Arcane Stalker";

                //Rogue Sub
                if (stats.SubbedClass is BaseClass.Rogue && stats.BaseClass is BaseClass.Warrior)
                    newClass = "Blade Dancer";

                if (stats.SubbedClass is BaseClass.Rogue && stats.BaseClass is BaseClass.Monk)
                    newClass = "Silent Fist";

                if (stats.SubbedClass is BaseClass.Rogue && stats.BaseClass is BaseClass.Wizard)
                    newClass = "Spellthief";

                if (stats.SubbedClass is BaseClass.Rogue && stats.BaseClass is BaseClass.Priest)
                    newClass = "Veilwalker";
            } else
                newClass = stats.BaseClass.ToString();

            // Alternate colors
            var currentColor = isSilver ? MessageColor.Silver : MessageColor.Gainsboro;

            builder.AppendLineFColored(
                currentColor,
                $"{entry.Key.Humanize(),-15} {stats.Damage,-10} {stats.DamagePerSecond,-10} {newClass,-10}");

            // Toggle the color for the next entry
            isSilver = !isSilver;
        }

        // Send leaderboard to the player
        source.SendServerMessage(ServerMessageType.ScrollWindow, builder.ToString());
    }

    /// <inheritdoc />
    public override void OnDisplaying(Aisling source)
    {
        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "hazel_acceptedquest":
            {
                var mapInstance = SimpleCache.Get<MapInstance>("hm_damagegame");
                source.TraverseMap(mapInstance, new Point(4, 2));

                break;
            }
            case "hazel_topscores":
            {
                DamageGame(source);

                break;
            }
        }
    }
}
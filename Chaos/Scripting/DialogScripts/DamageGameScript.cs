using System.Text;
using System.Text.Json;
using Chaos.Collections;
using Chaos.Common.Definitions;
using Chaos.DarkAges.Definitions;
using Chaos.DarkAges.Extensions;
using Chaos.Extensions.Common;
using Chaos.IO.FileSystem;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.DialogScripts;

public class DamageGameScript : DialogScriptBase
{
    private readonly IConfiguration Configuration;
    private readonly ISimpleCache SimpleCache;

    /// <inheritdoc />
    public DamageGameScript(Dialog subject, ISimpleCache simpleCache)
        : base(subject)
    {
        SimpleCache = simpleCache;
        Configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json", true, true).Build();
    }

    public static void DamageGame(Aisling player, IConfiguration configuration)
    {
        var stagingDirectory = configuration.GetSection("Options:ChaosOptions:StagingDirectory").Value;
        var aislingDirectory = configuration.GetSection("Options:DamageGameOptions:Directory").Value;

        var directory = stagingDirectory + aislingDirectory;
        var filePath = Path.Combine(stagingDirectory + aislingDirectory, "soloDamageGame.json");

        directory.SafeExecute(
            _ =>
            {
                Save(filePath, player);
            });
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
                DamageGame(source, Configuration);

                break;
            }
        }
    }

    public static void Save(string filePath, Aisling player)
    {
        var existingJson = File.ReadAllText(filePath);
        var damageData = JsonSerializer.Deserialize<Dictionary<string, int>>(existingJson);

        if (damageData != null)
        {
            // Sort the scores by damage and send them to the player
            var sortedScores = damageData.OrderByDescending(kv => kv.Value).ToList();
            var sb = new StringBuilder();
            sb.AppendLineFColored(MessageColor.White, "Damage Game Leaderboard:\n");

            foreach (var score in sortedScores)
                sb.AppendLineFColored(MessageColor.Orange, $"{score.Key}: {score.Value}");

            player.Client.SendServerMessage(ServerMessageType.ScrollWindow, sb.ToString());
        }
    }
}
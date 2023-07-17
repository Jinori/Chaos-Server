using System.Text;
using Chaos.Collections;
using Chaos.Common.Definitions;
using Chaos.Extensions.Common;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Storage.Abstractions;
using Microsoft.Extensions.Configuration;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Chaos.Scripting.DialogScripts;

public class DamageGameScript : DialogScriptBase
{
    private readonly ISimpleCache SimpleCache;
    private static readonly object LockObject = new();
    private readonly IConfiguration Configuration;

    /// <inheritdoc />
    public DamageGameScript(Dialog subject, ISimpleCache simpleCache)
        : base(subject)
    {
        SimpleCache = simpleCache;
        Configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json", true, true).Build();
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

    public static void DamageGame(Aisling player, IConfiguration configuration)
    {
        lock (LockObject)
        {
            var stagingDirectory = configuration.GetSection("Options:ChaosOptions:StagingDirectory").Value;
            var aislingDirectory = configuration.GetSection("Options:DamageGameOptions:Directory").Value;

            var filePath = Path.Combine(stagingDirectory + aislingDirectory, "soloDamageGame.json");
            Dictionary<string, int>? damageData;

            if (File.Exists(filePath))
            {
                var existingJson = File.ReadAllText(filePath);
                damageData = JsonSerializer.Deserialize<Dictionary<string, int>>(existingJson);
            }
            else
            {
                return;
            }

            if (damageData != null)
            {
                // Sort the scores by damage and send them to the player
                var sortedScores = damageData.OrderByDescending(kv => kv.Value).ToList();
                var sb = new StringBuilder();
                sb.AppendLineFColored(MessageColor.White, "Damage game scores:");

                foreach (var score in sortedScores)
                {
                    sb.AppendLineFColored(MessageColor.Orange, $"{score.Key}: {score.Value}");
                }

                player.Client.SendServerMessage(ServerMessageType.ScrollWindow, sb.ToString());
            }
        }
    }
}
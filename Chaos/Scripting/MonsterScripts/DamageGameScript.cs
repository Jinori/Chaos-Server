using System.Text.Json;
using Chaos.Collections;
using Chaos.Common.Definitions;
using Chaos.Common.Utilities;
using Chaos.IO.FileSystem;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.MonsterScripts.Abstractions;
using Chaos.Storage.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.MonsterScripts;

public class DamageGameScript(Monster subject, ISimpleCache simpleCache) : MonsterScriptBase(subject)
{
    private readonly IConfiguration Configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json", true, true).Build();
    private readonly IIntervalTimer CountDownTimer = new PeriodicMessageTimer(
        TimeSpan.FromMinutes(1),
        TimeSpan.FromSeconds(15),
        TimeSpan.FromSeconds(10),
        TimeSpan.FromSeconds(1),
        "{Time}",
        subject.Say);
    private int DamageDone;
    private bool GameStarted;

    public static void DamageGame(Aisling player, int damage, IConfiguration configuration)
    {
        var stagingDirectory = configuration.GetSection("Options:ChaosOptions:StagingDirectory").Value;
        var aislingDirectory = configuration.GetSection("Options:DamageGameOptions:Directory").Value;

        var directory = stagingDirectory + aislingDirectory;
        var filePath = Path.Combine(stagingDirectory + aislingDirectory, "soloDamageGame.json");

        directory.SafeExecute(
            _ =>
            {
                Save(filePath, damage, player);
            });
    }

    public override void OnAttacked(Creature source, int damage)
    {
        if (!GameStarted)
            GameStarted = true;

        DamageDone += damage;
    }

    public static void Save(string filePath, int damage, Aisling player)
    {
        Dictionary<string, int>? damageData;

        if (File.Exists(filePath))
        {
            var existingJson = File.ReadAllText(filePath);
            damageData = JsonSerializer.Deserialize<Dictionary<string, int>>(existingJson);
        }
        else
            damageData = new Dictionary<string, int>();

        if (damageData != null)
        {
            // Update the damageDone for the player if it's higher than the previous value
            var playerName = player.Name;

            if (damageData.TryGetValue(playerName, out var previousDamageDone))
            {
                if (damage > previousDamageDone)
                {
                    player.SendServerMessage(
                        ServerMessageType.Whisper,
                        $"You beat your old score of {previousDamageDone}. New score: {damage}!");

                    damageData[playerName] = damage;
                }

                if (damage < previousDamageDone)
                    player.SendServerMessage(ServerMessageType.Whisper, $"No new record. You only did {damage}.");
            }
            else
            {
                player.SendServerMessage(ServerMessageType.Whisper, $"New score of {damage} recorded!");
                damageData.Add(playerName, damage);
            }

            // Serialize the updated dictionary and write it to the JSON file
            var updatedJson = JsonSerializer.Serialize(damageData);
            FileEx.SafeWriteAllText(filePath, updatedJson);
        }
    }

    public override void Update(TimeSpan delta)
    {
        if (GameStarted)
        {
            CountDownTimer.Update(delta);

            if (CountDownTimer.IntervalElapsed)
            {
                var player = Subject.MapInstance.GetEntities<Aisling>().FirstOrDefault();

                if (player != null)
                {
                    DamageGame(player, DamageDone, Configuration);
                    var mapInstance = simpleCache.Get<MapInstance>("hm_road");
                    player.TraverseMap(mapInstance, new Point(4, 6));
                    GameStarted = false;
                    Subject.MapInstance.RemoveEntity(Subject);
                }
            }
        }
    }
}
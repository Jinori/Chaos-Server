using Chaos.Collections;
using Microsoft.Extensions.Configuration;
using Chaos.Common.Definitions;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.MonsterScripts.Abstractions;
using Chaos.Storage.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Chaos.Scripting.MonsterScripts
{
    public class DamageGameScript : MonsterScriptBase
    {
        private readonly IIntervalTimer CountDownTimer;
        private readonly ISimpleCache SimpleCache;
        private int DamageDone;
        private bool GameStarted;
        private readonly IConfiguration Configuration;
        private static readonly object LockObject = new();

        public DamageGameScript(Monster subject, ISimpleCache simpleCache)
            : base(subject)
        {
            CountDownTimer = new PeriodicMessageTimer(
                TimeSpan.FromMinutes(1),
                TimeSpan.FromSeconds(15),
                TimeSpan.FromSeconds(10),
                TimeSpan.FromSeconds(1),
                "{Time}",
                subject.Say);

            Configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json", true, true).Build();
            SimpleCache = simpleCache;
        }

        public override void OnAttacked(Creature source, int damage)
        {
            if (!GameStarted)
            {
                GameStarted = true;
            }

            DamageDone += damage;
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
                        Subject.MapInstance.RemoveObject(Subject);
                        DamageGame(player, DamageDone, Configuration);

                        var mapInstance = SimpleCache.Get<MapInstance>("hm_road");
                        player.TraverseMap(mapInstance, new Point(4, 6));
                    }
                }
            }
        }

        public static void DamageGame(Aisling player, int damage, IConfiguration configuration)
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
                    damageData = new Dictionary<string, int>();
                }

                if (damageData != null)
                {
                    // Update the damageDone for the player if it's higher than the previous value
                    var playerName = player.Name;
                    if (damageData.TryGetValue(playerName, out var previousDamageDone))
                    {
                        if (damage > previousDamageDone)
                        {
                            player.SendServerMessage(ServerMessageType.Whisper, $"You beat your old score of {previousDamageDone}. New score: {damage}!");
                            damageData[playerName] = damage;
                        }

                        if (damage < previousDamageDone)
                        {
                            player.SendServerMessage(ServerMessageType.Whisper, $"No new record. You only did {damage}.");
                        }
                    }
                    else
                    {
                        player.SendServerMessage(ServerMessageType.Whisper, $"New score of {damage} recorded!");
                        damageData.Add(playerName, damage);
                    }
                
                    // Serialize the updated dictionary and write it to the JSON file
                    var updatedJson = JsonSerializer.Serialize(damageData);
                    File.WriteAllText(filePath, updatedJson);
                }
            }
        }
    }
}
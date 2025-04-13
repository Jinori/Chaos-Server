using Chaos.NLog.Logging.Definitions;
using Chaos.NLog.Logging.Extensions;
using Chaos.Scripting.WorldScripts.Abstractions;
using Chaos.Storage.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.WorldScripts.WorldBuffs.Guild;

public class WorldBuffScript : IWorldScript
{
    private readonly IIntervalTimer SaveInterval = new IntervalTimer(TimeSpan.FromMinutes(1), false);
    private readonly IStorage<GuildBuffs> GuildBuffStorage;
    private readonly IStore<Collections.Guild> GuildStore;
    private readonly ILogger<WorldBuffScript> Logger;

    public WorldBuffScript(IStorage<GuildBuffs> guildBuffStorage, IStore<Collections.Guild> guildStore, ILogger<WorldBuffScript> logger)
    {
        GuildBuffStorage = guildBuffStorage;
        GuildStore = guildStore;
        Logger = logger;
    }

    /// <inheritdoc />
    public void Update(TimeSpan delta)
    {
        SaveInterval.Update(delta);

        var buffs = GuildBuffStorage.Value.ActiveBuffs.ToList();

        foreach (var buff in buffs)
        {
            buff.Update(delta);

            if (buff.Expired)
            {
                Collections.Guild? guild = null;
                
                try
                {
                    guild = GuildStore.Load(buff.GuildName);

                    foreach (var member in guild.GetOnlineMembers())
                        member.SendActiveMessage($"{buff.BuffName} has expired");
                    
                } catch
                {
                    //ignored
                }

                GuildBuffStorage.Value.ActiveBuffs.Remove(buff);

                var logEvent = Logger.WithTopics(Topics.Qualifiers.Expired);

                if (guild is not null)
                    logEvent = logEvent.WithProperty(guild);

                logEvent.LogInformation("Buff {BuffName} for guild {GuildName} has expired", buff.BuffName, buff.GuildName);
            }
        }

        if (SaveInterval.IntervalElapsed)
            GuildBuffStorage.Save();
    }

    /// <inheritdoc />
    public bool Enabled => true;
}
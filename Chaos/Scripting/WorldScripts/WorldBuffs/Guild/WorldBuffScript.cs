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
    
    public WorldBuffScript(IStorage<GuildBuffs> guildBuffStorage, IStore<Collections.Guild> guildStore)
    {
        GuildBuffStorage = guildBuffStorage;
        GuildStore = guildStore;
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
                try
                {
                    var guild = GuildStore.Load(buff.GuildName);

                    foreach (var member in guild.GetOnlineMembers())
                        member.SendActiveMessage($"{buff.BuffName} has expired");
                    
                } catch
                {
                    //ignored
                }

                GuildBuffStorage.Value.ActiveBuffs.Remove(buff);
            }
        }

        if (SaveInterval.IntervalElapsed)
            GuildBuffStorage.Save();
    }

    /// <inheritdoc />
    public bool Enabled => true;
}
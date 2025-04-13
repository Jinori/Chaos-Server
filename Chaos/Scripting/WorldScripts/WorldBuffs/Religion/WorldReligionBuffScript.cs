using Chaos.Networking.Abstractions;
using Chaos.Scripting.WorldScripts.Abstractions;
using Chaos.Storage.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.WorldScripts.WorldBuffs.Religion;

public class WorldReligionBuffScript : IWorldScript
{
    private readonly IIntervalTimer SaveInterval = new IntervalTimer(TimeSpan.FromMinutes(1), false);
    private readonly IStorage<ReligionBuffs> ReligionBuffStorage;
    private readonly IClientRegistry<IChaosWorldClient> ClientRegistry;
    
    public WorldReligionBuffScript(IStorage<ReligionBuffs> religionBuffStorage, IClientRegistry<IChaosWorldClient> clientRegistry)
    {
        ReligionBuffStorage = religionBuffStorage;
        ClientRegistry = clientRegistry;
    }

    /// <inheritdoc />
    public void Update(TimeSpan delta)
    {
        SaveInterval.Update(delta);

        var buffs = ReligionBuffStorage.Value.ActiveBuffs.ToList();

        foreach (var buff in buffs)
        {
            buff.Update(delta);

            if (buff.Expired)
            {
                try
                {
                    foreach (var member in ClientRegistry.ToList())
                        member.Aisling.SendActiveMessage($"{buff.BuffName} has expired");
                    
                } catch
                {
                    //ignored
                }

                ReligionBuffStorage.Value.ActiveBuffs.Remove(buff);
            }
        }

        if (SaveInterval.IntervalElapsed)
            ReligionBuffStorage.Save();
    }

    /// <inheritdoc />
    public bool Enabled => true;
}
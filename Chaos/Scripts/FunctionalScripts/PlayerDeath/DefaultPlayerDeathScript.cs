using Chaos.Clients.Abstractions;
using Chaos.Common.Definitions;
using Chaos.Common.Utilities;
using Chaos.Extensions.Common;
using Chaos.Formulae;
using Chaos.Networking.Abstractions;
using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripting.Abstractions;
using Chaos.Scripts.FunctionalScripts.Abstractions;

namespace Chaos.Scripts.FunctionalScripts.PlayerDeath;

public class DefaultPlayerDeathScript : ScriptBase, IPlayerDeathScript
{
    public DefaultPlayerDeathScript(IClientRegistry<IWorldClient> clientRegistry)
    {
        ClientRegistry = clientRegistry;
    }

    /// <inheritdoc />
    public static string Key { get; } = GetScriptKey(typeof(DefaultPlayerDeathScript));
    public IClientRegistry<IWorldClient> ClientRegistry { get; set; }

    /// <inheritdoc />
    public static IPlayerDeathScript Create()
    {
        return FunctionalScriptRegistry.Instance.Get<IPlayerDeathScript>(Key);
    }

    private List<string> mapsToNotPunishDeathOn = new List<string> { "tutorial_bossroom", "tutorial_farm" };
    
    /// <inheritdoc />
    public virtual void OnDeath(Aisling aisling, Creature killedBy)
    {
        aisling.IsDead = true;

        //Refresh to show ghost
        aisling.Refresh(true);

        //Remove all effects from the player
        var effects = aisling.Effects.ToList();
        foreach (var effect in effects)
            aisling.Effects.Dispel(effect.Name);

        if (mapsToNotPunishDeathOn.Contains(aisling.MapInstance.InstanceId))
            return;

        foreach (var client in ClientRegistry)
        {
            client.SendServerMessage(ServerMessageType.OrangeBar1,
                $"{aisling.Name} was killed at {aisling.MapInstance.Name} by {killedBy.Name}.");
        }

        //Let's break some items at 2% chance
        var itemsToBreak = aisling.Equipment.Where((x => x.Template.AccountBound is false));
        foreach (var item in itemsToBreak)
        {
            if (Randomizer.RollChance(2))
            {
                aisling.Client.SendServerMessage(ServerMessageType.OrangeBar1, $"{item.DisplayName} has been consumed by the Death God.");
                aisling.Equipment.TryGetRemove(item.Slot, out _);
            }
        }
        
        //Percent of Exp to take away (Sichi is implementing TakeExp)
        var TNL = LevelUpFormulae.Default.CalculateTnl(aisling);
        var tenPercent = Convert.ToInt32(0.1 * TNL);
        //aisling.TakeExp(tenPercent);
        aisling.Client.SendServerMessage(ServerMessageType.OrangeBar1, $"You have lost {tenPercent} experience.");
    }
}
using Chaos.Common.Definitions;
using Chaos.Common.Utilities;
using Chaos.Extensions.Common;
using Chaos.Formulae;
using Chaos.Objects.World;
using Chaos.Scripts.RuntimeScripts.Abstractions;

namespace Chaos.Scripts.RuntimeScripts.PlayerDeath;

public class DefaultPlayerDeathScript : IPlayerDeathScript
{
    /// <inheritdoc />
    public void OnDeath(Aisling aisling)
    {
        aisling.IsDead = true;

        //Refresh to show ghost
        aisling.Refresh(true);

        //Remove all effects from the player
        var effects = aisling.Effects.ToList();
        foreach (var effect in effects)
            aisling.Effects.Dispel(effect.Name);
        
        //Will worldshout soon
        aisling.Client.SendServerMessage(ServerMessageType.OrangeBar1, $"{aisling.Name} was killed at {aisling.MapInstance.Name} by {aisling.KilledBy.Name}.");
        
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
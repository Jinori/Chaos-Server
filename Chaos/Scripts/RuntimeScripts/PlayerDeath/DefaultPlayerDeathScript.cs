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

        aisling.Client.SendServerMessage(Common.Definitions.ServerMessageType.OrangeBar1, "You died! Wait for a resurrect or press (F1).");
    }
}
using Chaos.Collections.Common;
using Chaos.Common.Definitions;
using Chaos.DarkAges.Definitions;
using Chaos.Extensions.Common;
using Chaos.Messaging.Abstractions;
using Chaos.Models.World;
using Chaos.Networking.Abstractions;

namespace Chaos.Messaging.Admin;

[Command("displaychar", helpText: "<targetName>")]
public class DisplayCharInfoCommand : ICommand<Aisling>
{
    private readonly IClientRegistry<IChaosWorldClient> ClientRegistry;

    public DisplayCharInfoCommand(IClientRegistry<IChaosWorldClient> clientRegistry) => ClientRegistry = clientRegistry;

    public ValueTask ExecuteAsync(Aisling source, ArgumentCollection args)
    {
        if (args.TryGetNext<string>(out var targetName))
        {
            var target = ClientRegistry.Select(client => client.Aisling)
                                       .FirstOrDefault(aisling => aisling.Name.EqualsI(targetName));

            if (target == null)
            {
                source.SendOrangeBarMessage($"{targetName} is not online.");

                return default;
            }

            source.Client.SendServerMessage(
                ServerMessageType.ScrollWindow,
                "Name: "
                + target.Name
                + "\nLevel: "
                + target.UserStatSheet.Level
                + "\nTNL: "
                + target.UserStatSheet.ToNextLevel
                + "\nTotal XP Boxed: "
                + target.UserStatSheet.TotalExp
                + "\n -Base Stats-\n"
                + "\nBase Hp: "
                + target.UserStatSheet.MaximumHp
                + "\nBase Mp: "
                + target.UserStatSheet.MaximumMp
                + "\nBase Stats: "
                + target.UserStatSheet.Str
                + "/"
                + target.UserStatSheet.Int
                + "/"
                + target.UserStatSheet.Wis
                + "/"
                + target.UserStatSheet.Con
                + "/"
                + target.UserStatSheet.Dex
                + "\nBase Hit: "
                + target.UserStatSheet.Hit
                + "\nBase Dmg: "
                + target.UserStatSheet.Dmg
                + "\nBase MR: "
                + target.UserStatSheet.MagicResistance
                + "\nAC: "
                + target.UserStatSheet.Ac
                + "\nGold: "
                + target.Gold
                + "\nGold Banked: "
                + target.Bank.Gold
                + "\n -Geared Stats-\n"
                + "\nGeared Hp: "
                + target.UserStatSheet.EffectiveMaximumHp
                + "\nGeared Mp: "
                + target.UserStatSheet.EffectiveMaximumMp
                + "\nGeared Stats: "
                + target.UserStatSheet.EffectiveStr
                + "/"
                + target.UserStatSheet.EffectiveInt
                + "/"
                + target.UserStatSheet.EffectiveWis
                + "/"
                + target.UserStatSheet.EffectiveCon
                + "/"
                + target.UserStatSheet.EffectiveDex
                + "\nGeared Hit: "
                + target.UserStatSheet.EffectiveHit
                + "\nGeared Dmg: "
                + target.UserStatSheet.EffectiveDmg
                + "\nGeared MR: "
                + target.UserStatSheet.EffectiveMagicResistance
                + "\nEffective AC: "
                + target.UserStatSheet.EffectiveAc
                + "\nEffective Atk Speed: "
                + target.UserStatSheet.EffectiveAttackSpeedPct);

            return default;
        }

        return default;
    }
}
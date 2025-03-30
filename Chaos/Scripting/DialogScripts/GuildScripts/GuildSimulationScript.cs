using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;

namespace Chaos.Scripting.DialogScripts.GuildScripts;

public class GuildSimulationScript(Dialog subject) : DialogScriptBase(subject)
{
    private static void ClearMonsters(Aisling source)
    {
        foreach (var monster in source.MapInstance
                                      .GetEntities<Monster>()
                                      .ToList())
            source.MapInstance.RemoveEntity(monster);
    }

    public override void OnDisplaying(Aisling source)
    {
        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "simm_endsimulation":
            {
                ClearMonsters(source);

                foreach (var guildMember in source.MapInstance
                                                  .GetEntities<Aisling>()
                                                  .Where(x => x.IsDead || !x.IsAlive))
                {
                    RevivePlayer(guildMember);
                    guildMember.SendPersistentMessage($"Simulation has been ended by {source.Name}.");
                }

                break;
            }

            case "simm_revive":
            {
                RevivePlayer(source);

                break;
            }
        }
    }

    private static void RevivePlayer(Aisling player)
    {
        player.IsDead = false;
        player.StatSheet.SetHealthPct(100);
        player.StatSheet.SetManaPct(100);
        player.Refresh(true);
        player.Display();
    }
}
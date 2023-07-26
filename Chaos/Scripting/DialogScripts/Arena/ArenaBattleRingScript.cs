using Chaos.Common.Definitions;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;

namespace Chaos.Scripting.DialogScripts.Arena;

public class ArenaBattleRingScript : DialogScriptBase
{
    /// <inheritdoc />
    public ArenaBattleRingScript(Dialog subject)
        : base(subject) { }

    /// <inheritdoc />
    public override void OnDisplaying(Aisling source)
    {
        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "alex_revive":
            {
                Subject.Close(source);
                if (!source.IsAlive)
                {
                    source.IsDead = false;
                    source.StatSheet.SetHealthPct(25);
                    source.StatSheet.SetManaPct(25);
                    source.Client.SendAttributes(StatUpdateType.Vitality);
                    source.SendActiveMessage("Alex has revived you.");
                    source.Refresh();
                }
                break;
            }
            case "alex_north":
            {
                Subject.Close(source);
                source.WarpTo(new Point(5, 5));
                break;
            }
            case "alex_east":
            {
                
                Subject.Close(source);
                source.WarpTo(new Point(20, 5));
                break;
            }
            case "alex_south":
            {
                Subject.Close(source);
                source.WarpTo(new Point(20, 20));
                break;
            }
            case "alex_west":
            {
                Subject.Close(source);
                source.WarpTo(new Point(5, 20));
                break;
            }
        }
    }
}
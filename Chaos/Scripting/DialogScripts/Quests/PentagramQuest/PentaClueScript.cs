using Chaos.Common.Definitions;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;

namespace Chaos.Scripting.DialogScripts.Quests.PentagramQuest;

public class PentaClueScript(Dialog subject) : DialogScriptBase(subject)
{
    public override void OnDisplaying(Aisling source)
    {
        switch (source.UserStatSheet.BaseClass)
        {
            case BaseClass.Warrior:
            {
                Subject.AddOption("Listen closely", "pentapriestclue");
            }

                break;

            case BaseClass.Rogue:
            {
                Subject.AddOption("Listen closely", "pentawizardclue");
            }

                break;

            case BaseClass.Wizard:
            {
                Subject.AddOption("Listen closely","pentawarriorclue");
            }

                break;

            case BaseClass.Priest:
            {
                Subject.AddOption("Listen closely", "pentamonkclue");
            }

                break;

            case BaseClass.Monk:
            {
                Subject.AddOption("Listen closely", "pentarogueclue");
            }

                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
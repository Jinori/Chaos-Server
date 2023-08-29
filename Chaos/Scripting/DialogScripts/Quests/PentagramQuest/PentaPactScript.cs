using Chaos.Common.Definitions;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;

namespace Chaos.Scripting.DialogScripts.Quests.PentagramQuest;

public class PentaPactScript : DialogScriptBase
{
    public PentaPactScript(Dialog subject)
        : base(subject) { }

    public override void OnDisplaying(Aisling source)
    {
        switch (source.UserStatSheet.BaseClass)
        {
            case BaseClass.Warrior:
            {
                Subject.AddOption($"Open the book", "pentawarriorpact1");
            }

                break;

            case BaseClass.Rogue:
            {
                Subject.AddOption($"Open the book", "pentaroguepact1");
            }

                break;

            case BaseClass.Wizard:
            {
                Subject.AddOption($"Open the book", "pentawizardpact1");
            }

                break;

            case BaseClass.Priest:
            {
                Subject.AddOption($"Open the book", "pentapriestpact1");
            }

                break;

            case BaseClass.Monk:
            {
                Subject.AddOption($"Open the book", "pentamonkpact1");
            }

                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
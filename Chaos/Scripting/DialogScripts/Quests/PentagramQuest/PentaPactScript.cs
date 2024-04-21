using Chaos.Common.Definitions;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;

namespace Chaos.Scripting.DialogScripts.Quests.PentagramQuest;

public class PentaPactScript(Dialog subject) : DialogScriptBase(subject)
{
    private readonly Dictionary<BaseClass, string> ClassToOptionMap = new()
    {
        { BaseClass.Warrior, "pentawarriorpact1" },
        { BaseClass.Rogue, "pentaroguepact1" },
        { BaseClass.Wizard, "pentawizardpact1" },
        { BaseClass.Priest, "pentapriestpact1" },
        { BaseClass.Monk, "pentamonkpact1" }
    };

    public override void OnDisplaying(Aisling source)
    {
        if (!ClassToOptionMap.TryGetValue(source.UserStatSheet.BaseClass, out var option))
            return;
        
        Subject.AddOption("Open the book", option);
    }
}
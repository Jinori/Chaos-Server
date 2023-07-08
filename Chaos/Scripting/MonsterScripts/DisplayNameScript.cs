using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.MonsterScripts.Abstractions;

namespace Chaos.Scripting.MonsterScripts;

public class DisplayNameScript : MonsterScriptBase
{
    public DisplayNameScript(Monster subject) : base(subject)
    {
    }

    public override void OnClicked(Aisling source)
    {
        source.SendOrangeBarMessage(Subject.Name);
    }
}
using Chaos.Objects.Menu;
using Chaos.Objects.World;
using Chaos.Scripting.DialogScripts.Abstractions;

namespace Chaos.Scripting.DialogScripts.Generic;

public class TerminusTitleToggleScript : DialogScriptBase
{
    public TerminusTitleToggleScript(Dialog subject) : base(subject)
    {
    }
    
    
    public override void OnDisplaying(Aisling source)
    {
        if (source.Titles.Count < 2) 
            return;
        
        var first = source.Titles.First();
        source.Titles.Remove(first);
        source.Titles.Add(first);
        source.Client.SendSelfProfile();
    }
}
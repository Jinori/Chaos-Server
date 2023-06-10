using Chaos.Definitions;
using Chaos.Models.Legend;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Networking.Abstractions;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Scripting.DialogScripts.Religion.Abstractions;

namespace Chaos.Scripting.DialogScripts.Religion;

public class MiraelisTempleScript : ReligionScriptBase
{
    public MiraelisTempleScript(Dialog subject, IClientRegistry<IWorldClient> clientRegistry)
        : base(subject, clientRegistry) { }
    
    /// <inheritdoc />
    public override void OnDisplaying(Aisling source)
    {
        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "miraelis_temple_pray":
                PrayToMiraelis(source);
                break;
        }
    }

    private void PrayToMiraelis(Aisling source)
    {
        Pray(source, "Miraelis");
        var s = source.Trackers.Enums.TryGetValue(typeof(ReligionPrayer), out var value);
        Subject.InjectTextParameters(value);
    }
}
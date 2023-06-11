using Chaos.Common.Utilities;
using Chaos.Definitions;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Networking.Abstractions;
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
            case "miraelis_temple_initial":
                TempleInitial(source);
                break;
            case "miraelis_temple_pray":
                PrayToMiraelis(source);
                break;
            case "miraelis_temple_joinquest":
                SendOnJoinQuest(source);
                break;
            case "miraelis_temple_completejoinquest":
                CheckJoinQuestCompletion(source, "Miraelis");
                break;
        }
    }
    
    public void TempleInitial(Aisling source) => HideDialogOptions(source, "Miraelis", Subject);

    private void SendOnJoinQuest(Aisling source) => SendOnJoinQuest(source, "Miraelis");

    private void PrayToMiraelis(Aisling source)
    {
        Pray(source, "Miraelis");
        Subject.InjectTextParameters(DeityPrayers["Miraelis"].PickRandom());
    }
}
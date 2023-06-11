using Chaos.Common.Utilities;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Networking.Abstractions;
using Chaos.Scripting.DialogScripts.Religion.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.DialogScripts.Religion;

public class MiraelisTempleScript : ReligionScriptBase
{
    /// <inheritdoc />
    public MiraelisTempleScript(Dialog subject, IClientRegistry<IWorldClient> clientRegistry, IItemFactory itemFactory)
        : base(subject, clientRegistry, itemFactory) { }
    
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
                SendOnJoinQuest(source, "Miraelis");
                break;
            case "miraelis_temple_completejoinquest":
                CheckJoinQuestCompletion(source, "Miraelis");
                break;
            case "miraelis_temple_createscroll":
                CreateTempleScroll(source, "Miraelis");
                break;
            case "miraelis_temple_transferfaithaccepted":
                TransferFaith(source, "Miraelis");
                break;
        }
    }
    
    public void TempleInitial(Aisling source) => HideDialogOptions(source, "Miraelis", Subject);
    
    private void PrayToMiraelis(Aisling source)
    {
        Pray(source, "Miraelis");
        Subject.InjectTextParameters(DeityPrayers["Miraelis"].PickRandom());
    }
}
using Chaos.Common.Utilities;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Networking.Abstractions;
using Chaos.Scripting.DialogScripts.Religion.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.DialogScripts.Religion;

public class SerendaelTempleScript : ReligionScriptBase
{
    /// <inheritdoc />
    public SerendaelTempleScript(Dialog subject, IClientRegistry<IWorldClient> clientRegistry, IItemFactory itemFactory)
        : base(subject, clientRegistry, itemFactory) { }

    private const string GODNAME = "Serendael";
    
    /// <inheritdoc />
    public override void OnDisplaying(Aisling source)
    {
        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "serendael_temple_initial":
                TempleInitial(source);
                break;
            case "serendael_temple_pray":
                PrayToMiraelis(source);
                break;
            case "serendael_temple_joinquest":
                SendOnJoinQuest(source, GODNAME);
                break;
            case "serendael_temple_completejoinquest":
                CheckJoinQuestCompletion(source, GODNAME);
                break;
            case "serendael_temple_createscroll":
                CreateTempleScroll(source, GODNAME);
                break;
            case "serendael_temple_transferfaithaccepted":
                TransferFaith(source, GODNAME);
                break;
            case "serendael_temple_holdmassself5minute":
                AnnounceMassStart(source, GODNAME, true);
                break;
            case "serendael_temple_holdmassself1minute":
                AnnounceOneMinuteWarning(source, GODNAME, true);
                break;
            case "serendael_temple_holdmassselfendmass":
                AwardAttendees(source, GODNAME, null!, Subject.DialogSource as Merchant, true);
                break;
            case "serendael_temple_holdmassmiraelis":
                var _ = GoddessHoldMass(source, GODNAME, Subject.DialogSource as Merchant);
                break;
            case "serendael_temple_leavefaith":
                LeaveDeity(source, GODNAME);
                break;
        }
    }
    
    public void TempleInitial(Aisling source) => HideDialogOptions(source, GODNAME, Subject);
    
    private void PrayToMiraelis(Aisling source)
    {
        Pray(source, "Serendael");
        Subject.InjectTextParameters(DeityPrayers["Miraelis"].PickRandom());
    }
}
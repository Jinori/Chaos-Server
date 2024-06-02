using Chaos.Common.Utilities;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Networking.Abstractions;
using Chaos.Scripting.DialogScripts.Religion.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.DialogScripts.Religion;

public class MiraelisTempleScript : ReligionScriptBase
{
    private const string GODNAME = "Miraelis";

    /// <inheritdoc />
    public MiraelisTempleScript(Dialog subject, IClientRegistry<IChaosWorldClient> clientRegistry, IItemFactory itemFactory)
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
                SendOnJoinQuest(source, GODNAME);

                break;
            case "miraelis_temple_completejoinquest":
                CheckJoinQuestCompletion(source, GODNAME);

                break;
            case "miraelis_temple_createscroll":
                CreateTempleScroll(source, GODNAME);

                break;
            case "miraelis_temple_transferfaithaccepted":
                TransferFaith(source, GODNAME);

                break;
            case "miraelis_temple_holdmassself5minute":
                AnnounceMassStart(source, GODNAME, true);

                break;
            case "miraelis_temple_holdmassself1minute":
                AnnounceOneMinuteWarning(source, GODNAME, true);

                break;
            case "miraelis_temple_holdmassselfendmass":
                AwardAttendees(
                    source,
                    GODNAME,
                    null!,
                    Subject.DialogSource as Merchant,
                    true);

                break;
            case "miraelis_temple_holdmassmiraelis":
                GoddessHoldMass(source, GODNAME, Subject.DialogSource as Merchant);

                break;
            case "miraelis_temple_leavefaith":
                LeaveDeity(source, GODNAME);

                break;
        }
    }

    private void PrayToMiraelis(Aisling source)
    {
        Pray(source, "Miraelis");
        Subject.InjectTextParameters(DeityPrayers["Miraelis"].PickRandom());
    }

    public void TempleInitial(Aisling source) => HideDialogOptions(source, GODNAME, Subject);
}
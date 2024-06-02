using Chaos.Common.Utilities;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Networking.Abstractions;
using Chaos.Scripting.DialogScripts.Religion.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.DialogScripts.Religion;

public class TheseleneTempleScript : ReligionScriptBase
{
    private const string GODNAME = "Theselene";

    /// <inheritdoc />
    public TheseleneTempleScript(Dialog subject, IClientRegistry<IChaosWorldClient> clientRegistry, IItemFactory itemFactory)
        : base(subject, clientRegistry, itemFactory) { }

    /// <inheritdoc />
    public override void OnDisplaying(Aisling source)
    {
        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "theselene_temple_initial":
                TempleInitial(source);

                break;
            case "theselene_temple_pray":
                PrayToTheselene(source);

                break;
            case "theselene_temple_joinquest":
                SendOnJoinQuest(source, GODNAME);

                break;
            case "theselene_temple_completejoinquest":
                CheckJoinQuestCompletion(source, GODNAME);

                break;
            case "theselene_temple_createscroll":
                CreateTempleScroll(source, GODNAME);

                break;
            case "theselene_temple_transferfaithaccepted":
                TransferFaith(source, GODNAME);

                break;
            case "theselene_temple_holdmassself5minute":
                AnnounceMassStart(source, GODNAME, true);

                break;
            case "theselene_temple_holdmassself1minute":
                AnnounceOneMinuteWarning(source, GODNAME, true);

                break;
            case "theselene_temple_holdmassselfendmass":
                AwardAttendees(
                    source,
                    GODNAME,
                    null!,
                    Subject.DialogSource as Merchant,
                    true);

                break;
            case "theselene_temple_holdmassmiraelis":
                GoddessHoldMass(source, GODNAME, Subject.DialogSource as Merchant);

                break;
            case "theselene_temple_leavefaith":
                LeaveDeity(source, GODNAME);

                break;
        }
    }

    private void PrayToTheselene(Aisling source)
    {
        Pray(source, "Theselene");
        Subject.InjectTextParameters(DeityPrayers["Theselene"].PickRandom());
    }

    public void TempleInitial(Aisling source) => HideDialogOptions(source, GODNAME, Subject);
}
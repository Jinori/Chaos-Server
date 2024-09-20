using Chaos.Common.Utilities;
using Chaos.Extensions.Common;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Networking.Abstractions;
using Chaos.Scripting.DialogScripts.Religion.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.DialogScripts.Religion;

public class SkandaraTempleScript : ReligionScriptBase
{
    private const string GODNAME = "Skandara";

    /// <inheritdoc />
    public SkandaraTempleScript(Dialog subject, IClientRegistry<IChaosWorldClient> clientRegistry, IItemFactory itemFactory)
        : base(subject, clientRegistry, itemFactory) { }

    /// <inheritdoc />
    public override void OnDisplaying(Aisling source)
    {
        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "skandara_temple_initial":
                TempleInitial(source);

                break;
            case "skandara_temple_pray":
                PrayToSkandara(source);

                break;
            case "skandara_temple_joinquest":
                SendOnJoinQuest(source, GODNAME);

                break;
            case "skandara_temple_completejoinquest":
                CheckJoinQuestCompletion(source, GODNAME);

                break;
            case "skandara_temple_createscroll":
                CreateTempleScroll(source, GODNAME);
                
                break;
            
            case "skandara_temple_holdmass":
                if (source.Trackers.TimedEvents.HasActiveEvent("Mass", out var timedEvent))
                    Subject.Reply(
                        source,
                        $"You cannot hold Mass at this time. You've already hosted it recently. \nTry again in {
                            timedEvent.Remaining.ToReadableString()}.");

                break;
            
            case "skandara_temple_holdmassself5minute":
                AnnounceMassStart(source, GODNAME, true);

                break;
            case "skandara_temple_holdmassself1minute":
                AnnounceOneMinuteWarning(source, GODNAME, true);

                break;
            case "skandara_temple_holdmassselfendmass":
                AwardAttendees(
                    source,
                    GODNAME,
                    null!,
                    Subject.DialogSource as Merchant,
                    true);

                break;
            case "skandara_temple_holdmassskandara":
                GoddessHoldMass(source, GODNAME, Subject.DialogSource as Merchant);

                break;
            case "skandara_temple_leavefaith":
                LeaveDeity(source, GODNAME);

                break;
        }
    }

    private void PrayToSkandara(Aisling source)
    {
        Pray(source, "Skandara");
        Subject.InjectTextParameters(DeityPrayers["Skandara"].PickRandom());
    }

    public void TempleInitial(Aisling source) => HideDialogOptions(source, GODNAME, Subject);
}
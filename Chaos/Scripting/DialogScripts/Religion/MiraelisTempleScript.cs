using Chaos.Common.Utilities;
using Chaos.Extensions.Common;
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
    public MiraelisTempleScript(Dialog subject, IClientRegistry<IChaosWorldClient> clientRegistry, IItemFactory itemFactory, IEffectFactory effectFactory)
        : base(subject, clientRegistry, itemFactory, effectFactory) { }

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

            case "miraelis_temple_divineblessing25":
                BuffGroup(source, GODNAME, 25);
                
                break;
            
            case "miraelis_temple_divineblessing50":
                BuffGroup(source, GODNAME, 50);
                
                break;
            
            case "miraelis_temple_divineblessing75":
                BuffGroup(source, GODNAME, 75);
                
                break;
            
            case "miraelis_temple_divineblessing100":
                BuffGroup(source, GODNAME, 100);
                
                break;
            
            case "miraelis_temple_divineblessing300":
                BuffGroup(source, GODNAME, 300);
                
                break;
            
            case "miraelis_temple_holdmass":
                if (source.Trackers.TimedEvents.HasActiveEvent("Mass", out var timedEvent))
                    Subject.Reply(
                        source,
                        $"You cannot hold Mass at this time. You've already hosted it recently. \nTry again in {
                            timedEvent.Remaining.ToReadableString()}.");

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
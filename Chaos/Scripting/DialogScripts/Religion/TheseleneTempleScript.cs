using Chaos.Common.Utilities;
using Chaos.Extensions.Common;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Networking.Abstractions;
using Chaos.Scripting.DialogScripts.Religion.Abstractions;
using Chaos.Scripting.WorldScripts.WorldBuffs.Religion;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.DialogScripts.Religion;

public class TheseleneTempleScript : ReligionScriptBase
{
    private const string GODNAME = "Theselene";

    /// <inheritdoc />
    public TheseleneTempleScript(
        Dialog subject,
        IClientRegistry<IChaosWorldClient> clientRegistry,
        IItemFactory itemFactory,
        IEffectFactory effectFactory, IStorage<ReligionBuffs> religionBuffStorage)
        : base(
            subject,
            clientRegistry,
            itemFactory,
            effectFactory, religionBuffStorage) { }

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

            case "theselene_temple_divineblessing25":
                BuffGroup(source, GODNAME, 25);

                break;

            case "theselene_temple_divineblessing50":
                BuffGroup(source, GODNAME, 50);

                break;

            case "theselene_temple_divineblessing75":
                BuffGroup(source, GODNAME, 75);

                break;

            case "theselene_temple_divineblessing100":
                BuffGroup(source, GODNAME, 100);

                break;

            case "theselene_temple_divineblessing300":
                BuffGroup(source, GODNAME, 300);

                break;

            case "theselene_temple_holdmass":
                if (source.Trackers.TimedEvents.HasActiveEvent("Mass", out var timedEvent))
                    Subject.Reply(
                        source,
                        $"You cannot hold Mass at this time. You've already hosted it recently. \nTry again in {
                            timedEvent.Remaining.ToReadableString()}.");

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
            case "theselene_temple_holdmasstheselene":
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

        Subject.InjectTextParameters(
            DeityPrayers["Theselene"]
                .PickRandom());
    }

    public void TempleInitial(Aisling source) => HideDialogOptions(source, GODNAME, Subject);
}
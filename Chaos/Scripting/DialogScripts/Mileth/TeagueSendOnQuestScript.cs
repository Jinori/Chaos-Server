using Chaos.Common.Definitions;
using Chaos.Data;
using Chaos.Extensions;
using Chaos.Objects.Menu;
using Chaos.Objects.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.DialogScripts.Mileth;

public class TeagueSendOnQuestScript : DialogScriptBase
{
    private readonly IDialogFactory DialogFactory;
    private readonly IMerchantFactory MerchantFactory;

    public TeagueSendOnQuestScript(Dialog subject, IDialogFactory factory, IMerchantFactory merchant)
        : base(subject)
    {
        DialogFactory = factory;
        MerchantFactory = merchant;
    }

    public override void OnDisplaying(Aisling source)
    {
        var ani = new Animation
        {
            AnimationSpeed = 100,
            TargetAnimation = 78
        };

        var point = new Point(source.X, source.Y);
        var group = source.Group?.Where(x => x.WithinRange(point));

        if (group is null)
        {
            source.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Make sure you group your companions for this quest!");
            Subject.Text = "What? You don't have any friends with you.. who are you talking to?";
        }

        if (group is not null)
        {
            var groupCount = 0;

            foreach (var member in group)
                if (member.WithinLevelRange(source))
                    ++groupCount;

            if (groupCount.Equals(group.Count()))
                foreach (var member in group)
                {
                    var npcpoint = new Point(member.X, member.Y);
                    var merchant = MerchantFactory.Create("teague", member.MapInstance, point);
                    var dialog = DialogFactory.Create("teague_groupDialog", merchant);
                    dialog.Display(member);

                    member.Client.SendServerMessage(
                        ServerMessageType.OrangeBar1,
                        "Head to the crypts to end the horrific nightmares of the Old Man");

                    member.Trackers.Flags.AddFlag(QuestFlag1.TerrorOfCryptHunt);
                    member.Animate(ani);
                }
            else
            {
                source.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Make sure your companions are within level range.");
                Subject.Text = "Some of your companions are not within your level range.";
            }
        }
    }
}
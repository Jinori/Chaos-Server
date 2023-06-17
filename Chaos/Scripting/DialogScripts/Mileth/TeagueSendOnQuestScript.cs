using Chaos.Common.Definitions;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Extensions;
using Chaos.Models.Data;

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
        var group = source.Group?.Where(x => x.WithinRange(new Point(source.X, source.Y))).ToList();

        if ((group == null) || !group.Any())
        {
            source.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Make sure you group your companions for this quest!");
            Subject.Reply(source, "What? You don't have any friends with you.. who are you talking to?");
            return;
        }

        if (!group.All(member => member.WithinLevelRange(source)))
        {
            source.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Make sure your companions are within level range.");
            Subject.Reply(source, "Some of your companions are not within your level range.");
            return;
        }

        foreach (var member in group)
        {
            var merchant = MerchantFactory.Create("teague", member.MapInstance, new Point(member.X, member.Y));
            var dialog = DialogFactory.Create("teague_groupDialog", merchant);
            dialog.Display(member);

            member.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Head to the crypts to end the horrific nightmares of the Old Man");
            member.Trackers.Flags.AddFlag(QuestFlag1.TerrorOfCryptHunt);
            member.Animate(new Animation { AnimationSpeed = 100, TargetAnimation = 78 });
        }
    }
}
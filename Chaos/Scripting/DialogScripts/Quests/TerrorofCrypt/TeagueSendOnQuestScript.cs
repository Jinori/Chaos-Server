using Chaos.Common.Definitions;
using Chaos.DarkAges.Definitions;
using Chaos.Extensions;
using Chaos.Extensions.Geometry;
using Chaos.Models.Data;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.DialogScripts.Quests.TerrorofCrypt;

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
        if (source.Group is null || source.Group.Any(x => !x.OnSameMapAs(source) || !x.WithinRange(source)))
        {
            source.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Make sure your friends are close and grouped!");
            Subject.Reply(source, "What? You don't have any friends with you.. who are you talking to?");

            return;
        }

        if (!source.Group.All(member => member.WithinLevelRange(source)))
        {
            source.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Make sure your companions are within level range.");
            Subject.Reply(source, "Some of your companions are not within your level range.");

            return;
        }

        foreach (var member in source.Group)
        {
            var merchant = MerchantFactory.Create("teague", member.MapInstance, new Point(member.X, member.Y));
            var dialog = DialogFactory.Create("teague_groupDialog", merchant);
            dialog.Display(member);

            member.Client.SendServerMessage(
                ServerMessageType.OrangeBar1,
                "Head to the crypts to end the horrific nightmares of the Old Man");

            member.Trackers.Flags.AddFlag(QuestFlag1.TerrorOfCryptHunt);

            member.Animate(
                new Animation
                {
                    AnimationSpeed = 100,
                    TargetAnimation = 78
                });
        }
    }
}
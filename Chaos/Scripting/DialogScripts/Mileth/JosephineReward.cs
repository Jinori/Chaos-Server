using Chaos.Common.Definitions;
using Chaos.Objects.Menu;
using Chaos.Objects.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ExperienceDistribution;

namespace Chaos.Scripting.DialogScripts.Mileth;

public class JosephineRewardScript : DialogScriptBase
{
    private IExperienceDistributionScript ExperienceDistributionScript { get; }

    public JosephineRewardScript(Dialog subject)
        : base(subject) => ExperienceDistributionScript = DefaultExperienceDistributionScript.Create();

    public override void OnDisplaying(Aisling source)
    {
        if (source.Trackers.Flags.HasFlag(QuestFlag1.HeadedToBeautyShop))
        {
            Subject.Reply(source, "Riona sent you? I do have her dye, I'll let her know! Are you interested in a hair style?");
            source.Trackers.Flags.RemoveFlag(QuestFlag1.HeadedToBeautyShop);
            source.Trackers.Flags.AddFlag(QuestFlag1.TalkedToJosephine);
            ExperienceDistributionScript.GiveExp(source, 1000);
            source.TryGiveGold(1000);
            source.TryGiveGamePoints(5);
            source.Client.SendServerMessage(ServerMessageType.OrangeBar1, $"You've received 1000g, 1000exp and 5 game points!");
        }
    }
}
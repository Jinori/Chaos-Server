using Chaos.Common.Definitions;
using Chaos.Objects.Menu;
using Chaos.Objects.World;
using Chaos.Scripts.DialogScripts.Abstractions;
using Chaos.Scripts.FunctionalScripts.Abstractions;
using Chaos.Scripts.FunctionalScripts.ExperienceDistribution;

namespace Chaos.Scripts.DialogScripts.Mileth;

public class JosephineRewardScript : DialogScriptBase
{
    private IExperienceDistributionScript ExperienceDistributionScript { get; }

    public JosephineRewardScript(Dialog subject)
        : base(subject) => ExperienceDistributionScript = DefaultExperienceDistributionScript.Create();

    public override void OnDisplaying(Aisling source)
    {
        if (source.Flags.HasFlag(QuestFlag1.HeadedToBeautyShop))
        {
            Subject.Text = "Riona sent you? I do have her dye, I'll let her know! Are you interested in a hair style?";
            source.Flags.RemoveFlag(QuestFlag1.HeadedToBeautyShop);
            source.Flags.AddFlag(QuestFlag1.TalkedToJosephine);
            ExperienceDistributionScript.GiveExp(source, 1000);
            source.TryGiveGold(1000);
            source.TryGiveGamePoints(5);
            source.Client.SendServerMessage(ServerMessageType.OrangeBar1, $"You've received 1000g, 1000exp and 5 game points!");
        }
    }
}
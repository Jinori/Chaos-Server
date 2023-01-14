using Chaos.Common.Definitions;
using Chaos.Objects.Menu;
using Chaos.Objects.World;
using Chaos.Scripts.DialogScripts.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chaos.Scripts.FunctionalScripts.Abstractions;
using Chaos.Scripts.FunctionalScripts.ExperienceDistribution;

namespace Chaos.Scripts.DialogScripts
{
    public class IsabelleRewardScript : DialogScriptBase
    {
        private IExperienceDistributionScript ExperienceDistributionScript{ get; set; }
        public IsabelleRewardScript(Dialog subject) : base(subject)
        {
            ExperienceDistributionScript = DefaultExperienceDistributionScript.Create();
        }

        public override void OnDisplaying(Aisling source)
        {
            if (source.Flags.HasFlag(QuestFlag1.IsabelleMantisDead))
            {
                source.Flags.RemoveFlag(QuestFlag1.IsabelleMantisDead);
                source.Flags.AddFlag(QuestFlag1.IsabelleComplete);
                ExperienceDistributionScript.GiveExp(source, 150000);
                source.TryGiveGold(25000);
            }
            else
            {
                Subject.Text = "I can still see it from here! Please take care of it.";
                Subject.Options.Clear();
            }
        }
    }
}

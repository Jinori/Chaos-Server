using Chaos.Common.Definitions;
using Chaos.Objects.Menu;
using Chaos.Objects.World;
using Chaos.Scripts.DialogScripts.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chaos.Scripts.DialogScripts
{
    public class HideCainOptionsScript : DialogScriptBase
    {
        public HideCainOptionsScript(Dialog subject) : base(subject)
        {
        }

        public override void OnDisplaying(Aisling source)
        {
            switch (source.Flags)
            {
                case var _ when source.Flags.HasFlag(TutorialFlag.CainTutorialFlag1):
                    Subject.Text = "Did you get the carrots back?";
                    Subject.Options.Clear();

                    Subject.Options.Add(new DialogOption
                    {
                        DialogKey = "cain_yes",
                        OptionText = "Yes I did, those floppies hurt"
                    });
                    Subject.Options.Add(new DialogOption
                    {
                        DialogKey = "cain_no",
                        OptionText = "Still working on it."
                    });

                    break;

                case var _ when source.Flags.HasFlag(TutorialFlag.CainTutorialFlag2):
                    Subject.Text = "Thanks for helping me feed my family, go inside to Abel and buy yourself some equipment.";
                    Subject.Options.Clear();
                    break;
                default:
                    break;
            }
        }
    }
}

using Chaos.Common.Definitions;
using Chaos.Extensions.Common;
using Chaos.Objects.Menu;
using Chaos.Objects.World;
using Chaos.Scripts.DialogScripts.Abstractions;
using Chaos.Templates.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chaos.Scripts.DialogScripts
{
    public class brilannPeasantScript : DialogScriptBase
    {
        public brilannPeasantScript(Dialog subject) : base(subject)
        {
        }

        public override void OnDisplaying(Aisling source)
        {
            if (source.UserStatSheet.BaseClass.Equals(BaseClass.Peasant) && source.Flags.HasFlag(QuestFlag1.ChosenClass))
            {
                Subject.Text = "Hello, young traveler. What do you need help with?";
                Subject.Type = MenuOrDialogType.Menu;
                var BrilannOptions = new List<DialogOption>
                {
                    new()
                    {
                DialogKey = "brilann_learnSpells",
                OptionText = "Learn Spells"
                    },
                    new()
                    {
                DialogKey = "brilann_learnSkills",
                OptionText = "Learn Skills"
                    }
                };
                Subject.Options.AddRange(BrilannOptions);
            }
        }
    }
}

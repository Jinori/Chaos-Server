using Chaos.Common.Definitions;
using Chaos.Objects.Menu;
using Chaos.Objects.World;
using Chaos.Scripts.DialogScripts.Abstractions;

namespace Chaos.Scripts.DialogScripts
{
    public class HideIsabelleOptionsScript : DialogScriptBase
    {
        public HideIsabelleOptionsScript(Dialog subject) : base(subject)
        {
        }

        public override void OnDisplaying(Aisling source)
        {
            switch (source.Flags)
            {
                case var _ when source.Flags.HasFlag(QuestFlag1.IsabelleQuest) || source.Flags.HasFlag(QuestFlag1.IsabelleMantisDead):
                    Subject.Text = "Oh you're back! Is it dead?";
                    Subject.Options.Clear();

                    Subject.Options.Add(new DialogOption
                    {
                        DialogKey = "isabelle_yes",
                        OptionText = "Yeah, it's dead. Squashed like the bug it is."
                    });
                    Subject.Options.Add(new DialogOption
                    {
                        DialogKey = "isabelle_no",
                        OptionText = "Not yet, I'll be back."
                    });

                    break;

                case var _ when source.Flags.HasFlag(QuestFlag1.IsabelleComplete):
                    Subject.Text = "Thank you so much for taking care of that Giant Mantis! We can enjoy the peak again.";
                    Subject.Options.Clear();
                    break;
                default:
                    break;
            }
        }
    }
}

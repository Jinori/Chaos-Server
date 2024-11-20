using Chaos.Common.Definitions;
using Chaos.DarkAges.Definitions;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;

namespace Chaos.Scripting.DialogScripts.Quests.PentagramQuest
{
    public class PentaPactScript : DialogScriptBase
    {
        private readonly Dictionary<BaseClass, string> ClassToOptionMap = new()
        {
            { BaseClass.Warrior, "pentawarriorpact1"},
            { BaseClass.Rogue, "pentaroguepact1" },
            { BaseClass.Wizard, "pentawizardpact1" },
            { BaseClass.Priest, "pentapriestpact1" },
            { BaseClass.Monk, "pentamonkpact1" }
        };

        public PentaPactScript(Dialog subject) : base(subject)
        {
        }

        public override void OnDisplaying(Aisling source)
        {
            if (!ClassToOptionMap.TryGetValue(source.UserStatSheet.BaseClass, out var option))
                return;

            // Check if the full group is on the same map and within level range.
            var group = source.Group;
            if (group == null || !IsGroupOnSameMap(source) || !IsWithinLevelRange(source))
                return;

            Subject.AddOption("Open the book", option);
        }

        private bool IsGroupOnSameMap(Aisling source)
        {
            var currentMap = source.MapInstance.Template.TemplateKey;
            return source.Group != null && source.Group.All(member => member.MapInstance.Template.TemplateKey == currentMap);
        }

        private bool IsWithinLevelRange(Aisling source)
        {
            return source.Group != null && source.Group.All(member => member.WithinLevelRange(source));
        }
    }
}
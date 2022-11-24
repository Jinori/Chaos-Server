using Chaos.Common.Definitions;
using Chaos.Data;
using Chaos.Objects.Legend;
using Chaos.Objects.Menu;
using Chaos.Objects.World;
using Chaos.Scripts.DialogScripts.Abstractions;

namespace Chaos.Scripts.DialogScripts
{
    public class PriestDedicateScript : DialogScriptBase
    {
        public PriestDedicateScript(Dialog subject) : base(subject)
        {
        }

        public override void OnDisplayed(Aisling source)
        {
            if (source.Legend.TryGetValue("base", out var legendMark))
                return;

            var ani = new Animation
            {
                AnimationSpeed = 100,
                TargetAnimation = 78,
            };
            source.UserStatSheet.SetBaseClass(BaseClass.Priest);
            source.Animate(ani, source.Id);
            source.Legend.AddOrAccumulate(new LegendMark("Priest Class Devotion", "base", MarkIcon.Priest, MarkColor.Blue, 1, Time.GameTime.Now));
        }
    }
}

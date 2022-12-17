using Chaos.Objects;
using Chaos.Objects.Panel;
using Chaos.Scripts.SkillScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripts.SkillScripts.Rogue
{
    public class PeasantTentScript : SkillScriptBase
    {
        private readonly IDialogFactory DialogFactory;
        private readonly IMerchantFactory MerchantFactory;

        public PeasantTentScript(Skill subject, IDialogFactory factory, IMerchantFactory merchant) : base(subject)
        {
            DialogFactory = factory;
            MerchantFactory = merchant;
        }

        public override void OnUse(SkillContext context)
        {
            if (context.Source.IsAlive)
            {
                var point = new Point(context.Source.X, context.Source.Y);

                var merchant = MerchantFactory.Create("tent", context.Source.MapInstance, point);
                var dialog = DialogFactory.Create("tent_initial", merchant);
                dialog.Display(context.SourceAisling!);
            }
        }
    }
}

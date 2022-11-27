using Chaos.Factories;
using Chaos.Factories.Abstractions;
using Chaos.Geometry.Abstractions;
using Chaos.Geometry.Abstractions.Definitions;
using Chaos.Objects;
using Chaos.Objects.Panel;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripts.SkillScripts.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chaos.Scripts.SkillScripts
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

using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Geometry.Abstractions;
using Chaos.Objects;
using Chaos.Objects.Panel;
using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripts.SkillScripts.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chaos.Scripts.SkillScripts
{
    public class StudyCreatureElementScript : BasicSkillScriptBase
    {
        public StudyCreatureElementScript(Skill subject) : base(subject)
        {
        }



        public override void OnUse(SkillContext context)
        {
            ShowBodyAnimation(context);

            var affectedPoints = GetAffectedPoints(context).Cast<IPoint>().ToList();
            var affectedEntities = GetAffectedEntities<Creature>(context, affectedPoints);

            ShowAnimation(context, affectedPoints);
            PlaySound(context, affectedPoints);

            var mob = affectedEntities.FirstOrDefault();
            mob?.Chant(mob.StatSheet.DefenseElement.ToString());

            var group = context.SourceAisling?.Group?.Where(x => x.WithinRange(context.SourcePoint));
            if (group is not null)
            {
                foreach (var entity in group)
                {
                    var showMobEle = entity.MapInstance.GetEntities<Creature>().Where(x => x.Equals(mob)).FirstOrDefault();
                    showMobEle?.Chant(showMobEle.StatSheet.DefenseElement.ToString());
                }
            }
        }
    }
}

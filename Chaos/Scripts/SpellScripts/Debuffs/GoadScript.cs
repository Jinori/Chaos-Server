using Chaos.Common.Definitions;
using Chaos.Common.Utilities;
using Chaos.Data;
using Chaos.Geometry.Abstractions;
using Chaos.Objects;
using Chaos.Objects.Panel;
using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripts.SpellScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chaos.Scripts.SpellScripts.Debuffs
{
    public class GoadScript : BasicSpellScriptBase
    {
        protected int? manaSpent { get; init; }
        private readonly IEffectFactory EffectFactory;

        public GoadScript(Spell subject, IEffectFactory effectFactory) : base(subject)
        {
            EffectFactory = effectFactory;
        }

        public override void OnUse(SpellContext context)
        {
            if (manaSpent.HasValue)
            {
                if (context.Source.StatSheet.CurrentMp < manaSpent.Value)
                {
                    context.SourceAisling?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You do not have enough mana for this cast.");
                    return;
                }
                context.Source.StatSheet.SubtractMp(manaSpent.Value);
                context.SourceAisling?.Client.SendAttributes(StatUpdateType.Vitality);
            }

            ShowBodyAnimation(context);


            var affectedPoints = GetAffectedPoints(context).Cast<IPoint>().ToList();
            var affectedEntities = GetAffectedEntities<Creature>(context, affectedPoints);

            ShowAnimation(context, affectedPoints);
            PlaySound(context, affectedPoints);
            ApplyTaunt(context, affectedEntities);
        }

        protected virtual void ApplyTaunt(SpellContext context, IEnumerable<Creature> targetEntities)
        {
            foreach (var target in targetEntities)
            {
                var s = target as Monster;
                s?.AggroList.AddOrUpdate(context.Source.Id, _ => 1000, (_, currentAggro) => currentAggro + 1000);
            }
        }
    }
}

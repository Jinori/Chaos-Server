using Chaos.Common.Definitions;
using Chaos.Common.Utilities;
using Chaos.Objects.World;
using Chaos.Scripts.MonsterScripts.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chaos.Scripts.MonsterScripts
{
    public class RandomDefenseElementScript : ConfigurableMonsterScriptBase
    {
        protected Element[] Elements { get; init; } = Array.Empty<Element>();

        /// <inheritdoc />
        public RandomDefenseElementScript(Monster subject)
            : base(subject)
        {
            var element = Elements.PickRandom();

            Subject.StatSheet.SetDefenseElement(element);
        }
    }
}

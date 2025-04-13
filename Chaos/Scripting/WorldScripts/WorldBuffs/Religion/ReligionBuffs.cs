using Chaos.Collections.Synchronized;

namespace Chaos.Scripting.WorldScripts.WorldBuffs.Religion;

public class ReligionBuffs
{
    public SynchronizedList<ReligionBuff> ActiveBuffs { get; set; } = [];
}
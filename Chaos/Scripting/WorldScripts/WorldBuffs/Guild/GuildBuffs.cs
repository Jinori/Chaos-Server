using Chaos.Collections.Synchronized;

namespace Chaos.Scripting.WorldScripts.WorldBuffs.Guild;

public class GuildBuffs
{
    public SynchronizedList<GuildBuff> ActiveBuffs { get; set; } = [];
}
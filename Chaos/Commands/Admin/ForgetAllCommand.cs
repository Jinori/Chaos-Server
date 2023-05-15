using Chaos.Collections.Common;
using Chaos.Messaging;
using Chaos.Messaging.Abstractions;
using Chaos.Objects.World;

namespace Chaos.Commands.Admin;
[Command("forgetall")]

public class ForgetAllCommand : ICommand<Aisling>
{
    public ValueTask ExecuteAsync(Aisling source, ArgumentCollection args)
    {
        foreach (var skill in source.SkillBook)
            source.SkillBook.Remove(skill.Slot);

        foreach (var spell in source.SpellBook)
            source.SpellBook.Remove(spell.Slot);
        
        return default;
    }
}
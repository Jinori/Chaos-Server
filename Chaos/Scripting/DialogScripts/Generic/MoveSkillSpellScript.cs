using Chaos.Common.Definitions;
using Chaos.Models.Menu;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;

namespace Chaos.Scripting.DialogScripts.Generic;

public class MoveSkillSpellScript : DialogScriptBase
{
    /// <inheritdoc />
    public MoveSkillSpellScript(Dialog subject)
        : base(subject) { }

    /// <inheritdoc />
    public override void OnNext(Aisling source, byte? optionIndex = null)
    {
        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "verity_orgspellsbookone":
            {
                if (!TryFetchArgs<byte>(out var slot) || !source.SpellBook.TryGetObject(slot, out var spell))
                {
                    Subject.ReplyToUnknownInput(source);

                    return;
                }

                if (!source.SpellBook.Contains(spell))
                {
                    Subject.Reply(source, "You don't seem to have that spell.");
                    return;
                }

                if (!source.SpellBook.TryAddToNextSlot(PageType.Page2, spell))
                {
                    Subject.Reply(source, "I tried writing that in your second book but something failed.");
                    return;
                }
                
                source.SpellBook.Remove(slot);
                Subject.Reply(source, $"Your spell {spell.Template.Name} was moved from book one to book two.");
                break;
            }
            case "verity_orgspellsbooktwo":
            {
                if (!TryFetchArgs<byte>(out var slot) || !source.SpellBook.TryGetObject(slot, out var spell))
                {
                    Subject.ReplyToUnknownInput(source);

                    return;
                }

                if (!source.SpellBook.Contains(spell))
                {
                    Subject.Reply(source, "You don't seem to have that spell.");
                    return;
                }

                if (!source.SpellBook.TryAddToNextSlot(PageType.Page1, spell))
                {
                    Subject.Reply(source, "I tried writing that in your first book but something failed.");
                    return;
                }
                
                source.SpellBook.Remove(slot);
                Subject.Reply(source, $"Your spell {spell.Template.Name} was moved from book two to book one.");
                break;
            }
            case "verity_orgskillsbookone":
            {
                if (!TryFetchArgs<byte>(out var slot) || !source.SkillBook.TryGetObject(slot, out var skill))
                {
                    Subject.ReplyToUnknownInput(source);

                    return;
                }

                if (!source.SkillBook.Contains(skill))
                {
                    Subject.Reply(source, "You don't seem to have that skill.");
                    return;
                }

                if (!source.SkillBook.TryAddToNextSlot(PageType.Page2, skill))
                {
                    Subject.Reply(source, "I tried writing that in your second book but something failed.");
                    return;
                }
                
                source.SkillBook.Remove(slot);
                Subject.Reply(source, $"Your skill {skill.Template.Name} was moved from book one to book two.");
                break;
            }
            case "verity_orgskillsbooktwo":
            {
                if (!TryFetchArgs<byte>(out var slot) || !source.SkillBook.TryGetObject(slot, out var skill))
                {
                    Subject.ReplyToUnknownInput(source);

                    return;
                }

                if (!source.SkillBook.Contains(skill))
                {
                    Subject.Reply(source, "You don't seem to have that skill.");
                    return;
                }

                if (!source.SkillBook.TryAddToNextSlot(PageType.Page1, skill))
                {
                    Subject.Reply(source, "I tried writing that in your first book but something failed.");
                    return;
                }
                
                source.SkillBook.Remove(slot);
                Subject.Reply(source, $"Your skill {skill.Template.Name} was moved from book two to book one.");
                break;
            }
        }
    }
}
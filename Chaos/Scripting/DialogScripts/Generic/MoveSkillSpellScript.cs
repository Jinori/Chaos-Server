using Chaos.Common.Definitions;
using Chaos.Models.Menu;
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
                MoveSpell(source, PageType.Page2, "book one", "book two");
                break;
            case "verity_orgspellsbooktwo":
                MoveSpell(source, PageType.Page1, "book two", "book one");
                break;
            case "verity_orgskillsbookone":
                MoveSkill(source, PageType.Page2, "book one", "book two");
                break;
            case "verity_orgskillsbooktwo":
                MoveSkill(source, PageType.Page1, "book two", "book one");
                break;
            default:
                Subject.ReplyToUnknownInput(source);
                break;
        }
    }

    private void MoveSpell(Aisling source, PageType toPage, string fromBook, string toBook)
    {
        if (!TryFetchArgs<byte>(out var slot) || !source.SpellBook.TryGetObject(slot, out var spell))
        {
            Subject.ReplyToUnknownInput(source);
            return;
        }

        if (!spell.CanUse())
        {
            Subject.Reply(source, "You must wait for the cooldown to expire.");
            return;
        }
        
        if (!source.SpellBook.Contains(spell))
        {
            Subject.Reply(source, "You don't seem to have that spell.");
            return;
        }

        if (!source.SpellBook.TryAddToNextSlot(toPage, spell))
        {
            Subject.Reply(source, $"I tried writing that in your {toBook} but something failed.");
            return;
        }

        source.SpellBook.Remove(slot);
        Subject.Reply(source, $"Your spell {spell.Template.Name} was moved from {fromBook} to {toBook}.");
    }

    private void MoveSkill(Aisling source, PageType toPage, string fromBook, string toBook)
    {
        if (!TryFetchArgs<byte>(out var slot) || !source.SkillBook.TryGetObject(slot, out var skill))
        {
            Subject.ReplyToUnknownInput(source);
            return;
        }

        if (!skill.CanUse())
        {
            Subject.Reply(source, "You must wait for the cooldown to expire.");
            return;
        }
        
        if (!source.SkillBook.Contains(skill))
        {
            Subject.Reply(source, "You don't seem to have that skill.");
            return;
        }

        if (!source.SkillBook.TryAddToNextSlot(toPage, skill))
        {
            Subject.Reply(source, $"I tried writing that in your {toBook} but something failed.");
            return;
        }

        source.SkillBook.Remove(slot);
        Subject.Reply(source, $"Your skill {skill.Template.Name} was moved from {fromBook} to {toBook}.");
    }
}

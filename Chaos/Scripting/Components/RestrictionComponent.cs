using Chaos.Definitions;
using Chaos.Extensions.Common;
using Chaos.Objects.Panel;
using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;

namespace Chaos.Scripting.Components;

public class RestrictionComponent
{
    public virtual bool CanMove(Creature creature)
    {
        var aisling = creature as Aisling;
        if (creature.Status.HasFlag(Status.Suain) || creature.Status.HasFlag(Status.Pramh) ||
            (creature.Status.HasFlag(Status.Blind) && creature is not Aisling))
        {
            aisling?.SendOrangeBarMessage("You cannot move.");
            return false;
        }

        return creature.MapInstance.Name.EqualsI("The Afterlife") || creature.IsAlive;
    }

    public virtual bool CanTalk(Creature creature)
    {
        return true;
    }

    public virtual bool CanTurn(Creature creature)
    {
        var aisling = creature as Aisling;
        if (creature.Status.HasFlag(Status.Suain) || creature.Status.HasFlag(Status.Pramh))
        {
            aisling?.SendOrangeBarMessage("You cannot turn.");
            return false;
        }
        return true;
    }

    public virtual bool CanUseItem(Aisling aisling, Item item)
    {
        if (aisling.IsAlive && !aisling.Status.HasFlag(Status.Suain) && !aisling.Status.HasFlag(Status.Pramh)) 
            return aisling.IsAlive;
        
        aisling.SendOrangeBarMessage("You can't do that");
        return false;
    }

    public virtual bool CanUseSkill(Creature creature, Skill skill)
    {
        var aisling = creature as Aisling;
        if (creature.Status.HasFlag(Status.Suain) || creature.Status.HasFlag(Status.Pramh))
        {
            aisling?.SendOrangeBarMessage("You cannot use skills.");
            return false;
        }
        return creature.IsAlive;
    }

    public virtual bool CanUseSpell(Creature creature, Spell spell)
    {
        var aisling = creature as Aisling;
        if (creature.Status.HasFlag(Status.Suain) || creature.Status.HasFlag(Status.Pramh) ||
            (creature.Status.HasFlag(Status.Blind) && creature is not Aisling))
        {
            aisling?.SendOrangeBarMessage("You cannot use spells.");
            return false;
        }

        if (creature.IsDead && spell.Template.Name == "Self Revive")
            return true;
        return creature.IsAlive;
    }
}

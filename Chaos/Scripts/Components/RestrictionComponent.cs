using Chaos.Definitions;
using Chaos.Objects.Panel;
using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;

namespace Chaos.Scripts.Components;

public class RestrictionComponent
{
    public virtual bool CanMove(Creature creature)
    {
        if (creature.Status.HasFlag(Status.Suain))
            return false;

        if (creature.Status.HasFlag(Status.Pramh))
            return false;

        if (creature.Status.HasFlag(Status.BeagSuain))
            return false;

        if (creature.Status.HasFlag(Status.Blind))
            return false;

        return creature.IsAlive;
    }

    public virtual bool CanTalk(Creature creature) => creature.IsAlive;

    public virtual bool CanTurn(Creature creature)
    {
        if (creature.Status.HasFlag(Status.Suain))
            return false;

        if (creature.Status.HasFlag(Status.Pramh))
            return false;

        if (creature.Status.HasFlag(Status.BeagSuain))
            return false;

        return creature.IsAlive;
    }

    public virtual bool CanUseItem(Aisling aisling, Item item)
    {
        if (aisling.Status.HasFlag(Status.Suain))
            return false;

        if (aisling.Status.HasFlag(Status.Pramh))
            return false;

        return aisling.IsAlive;
    }

    public virtual bool CanUseSkill(Creature creature, Skill skill)
    {
        if (creature.Status.HasFlag(Status.Suain))
            return false;

        if (creature.Status.HasFlag(Status.Pramh))
            return false;

        if (creature.Status.HasFlag(Status.BeagSuain))
            return false;

        return creature.IsAlive;
    }

    public virtual bool CanUseSpell(Creature creature, Spell spell)
    {
        if (creature.Status.HasFlag(Status.Suain))
            return false;

        if (creature.Status.HasFlag(Status.Pramh))
            return false;

        return creature.IsAlive;
    }
}
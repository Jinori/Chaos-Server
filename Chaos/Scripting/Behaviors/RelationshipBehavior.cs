using Chaos.DarkAges.Definitions;
using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.MonsterScripts.Boss.MainStory.TrialOfSacrifice.zoe;
using Chaos.Scripting.MonsterScripts.Limbo;
using Chaos.Scripting.MonsterScripts.Nightmare.MonkNightmare;
using Chaos.Scripting.MonsterScripts.Nightmare.PriestNightmare;
using Chaos.Scripting.MonsterScripts.Nightmare.RogueNightmare;
using Chaos.Scripting.MonsterScripts.Pet;

namespace Chaos.Scripting.Behaviors;

public class RelationshipBehavior
{
    public virtual bool IsFriendlyTo(Creature source, Creature target)
        => source switch
        {
            Aisling aisling => target switch
            {
                Aisling other  => IsFriendlyTo(aisling, other),
                Merchant other => IsFriendlyTo(aisling, other),
                Monster other  => IsFriendlyTo(aisling, other),
                _              => throw new ArgumentOutOfRangeException(nameof(target))
            },
            Merchant merchant => target switch
            {
                Aisling other  => IsFriendlyTo(merchant, other),
                Merchant other => IsFriendlyTo(merchant, other),
                Monster other  => IsFriendlyTo(merchant, other),
                _              => throw new ArgumentOutOfRangeException(nameof(target))
            },
            Monster monster => target switch
            {
                Aisling other  => IsFriendlyTo(monster, other),
                Merchant other => IsFriendlyTo(monster, other),
                Monster other  => IsFriendlyTo(monster, other),
                _              => throw new ArgumentOutOfRangeException(nameof(target))
            },
            _ => throw new ArgumentOutOfRangeException(nameof(source))
        };

    protected virtual bool IsFriendlyTo(Aisling source, Merchant target) => true;

    protected virtual bool IsFriendlyTo(Merchant source, Merchant target) => source.Equals(target);

    protected virtual bool IsFriendlyTo(Monster source, Merchant target) => false;

    protected virtual bool IsFriendlyTo(Aisling source, Aisling target)
    {
        if (source.Equals(target))
            return true;

        if (source.Group?.Contains(target) == true)
            return true;

        if (source.IsOnPvEArenaMap())
            return true;

        if (source.IsOnPvPArenaMap())
            return false;

        return false;
    }

    protected virtual bool IsFriendlyTo(Aisling source, Monster target)
    {
        if (target.Script.Is<PetScript>() && target.PetOwner is not null)
            return true;

        return false;
    }

    protected virtual bool IsFriendlyTo(Merchant source, Aisling target) => true;

    protected virtual bool IsFriendlyTo(Merchant source, Monster target) => false;

    protected virtual bool IsFriendlyTo(Monster source, Aisling target)
    {
        var isSourceOrTargetPet = source.Script.Is<PetScript>() || target.Script.Is<PetScript>();

        if (isSourceOrTargetPet)
        {
            var isGroupMember = source.PetOwner?.Group?.Contains(target) == true;

            var isAisling = source.MapInstance
                                  .GetEntities<Aisling>()
                                  .Any();
            var isOwner = target.Equals(source.PetOwner);

            return (isSourceOrTargetPet && isGroupMember) || isOwner || isAisling;
        }

        var isTotem = source.Script.Is<NightmareTotemScript>() || target.Script.Is<NightmareTotemScript>();

        if (isTotem)
        {
            var isGroupMember = source.PetOwner?.Group?.Contains(target) == true;
            var isOwner = target.Equals(source.PetOwner);

            return (isSourceOrTargetPet && isGroupMember) || isOwner;
        }

        var isZoe = source.Script.Is<SacrificeZoe>() || target.Script.Is<SacrificeZoe>();

        if (isZoe)
        {
            var isGroupMember = source.PetOwner?.Group?.Contains(target) == true;
            var isOwner = target.Equals(source.PetOwner);

            return (isSourceOrTargetPet && isGroupMember) || isOwner;
        }

        var isSlave = source.Script.Is<NightmareSlaveScript>() || target.Script.Is<NightmareSlaveScript>();

        if (isSlave)
        {
            var isGroupMember = source.PetOwner?.Group?.Contains(target) == true;
            var isOwner = target.Equals(source.PetOwner);

            return (isSourceOrTargetPet && isGroupMember) || isOwner;
        }

        var isTeammate = source.Script.Is<NightmareTeammateScript>() || target.Script.Is<NightmareTeammateScript>();

        if (isTeammate)
        {
            var isGroupMember = source.PetOwner?.Group?.Contains(target) == true;
            var isOwner = target.Equals(source.PetOwner);

            return (isSourceOrTargetPet && isGroupMember) || isOwner;
        }

        return false;
    }

    protected virtual bool IsFriendlyTo(Monster source, Monster target)
    {
        if (source.Equals(target))
            return true;

        if (source.Script.Is<PetScript>() || target.Script.Is<PetScript>())
            return false;

        if (source.Script.Is<NightmareTeammateScript>() && target.Script.Is<NightmareTeammateScript>())
            return false;

        if (source.Script.Is<NightmareTotemScript>() || target.Script.Is<NightmareTotemScript>())
            return false;

        if (source.Script.Is<SacrificeZoe>() || target.Script.Is<SacrificeZoe>())
            return false;

        if (source.Script.Is<NightmareSlaveScript>() || target.Script.Is<NightmareSlaveScript>())
            return false;

        if (source.Script.Is<LimboFriendlyScript>() && target.Script.Is<LimboFriendlyScript>())
            return true;

        return false;
    }

    public virtual bool IsHostileTo(Creature source, Creature target)
        => source switch
        {
            Aisling aisling => target switch
            {
                Aisling other  => IsHostileTo(aisling, other),
                Merchant other => IsHostileTo(aisling, other),
                Monster other  => IsHostileTo(aisling, other),
                _              => throw new ArgumentOutOfRangeException(nameof(target))
            },
            Merchant merchant => target switch
            {
                Aisling other  => IsHostileTo(merchant, other),
                Merchant other => IsHostileTo(merchant, other),
                Monster other  => IsHostileTo(merchant, other),
                _              => throw new ArgumentOutOfRangeException(nameof(target))
            },
            Monster monster => target switch
            {
                Aisling other  => IsHostileTo(monster, other),
                Merchant other => IsHostileTo(monster, other),
                Monster other  => IsHostileTo(monster, other),
                _              => throw new ArgumentOutOfRangeException(nameof(target))
            },
            _ => throw new ArgumentOutOfRangeException(nameof(source))
        };

    protected virtual bool IsHostileTo(Aisling source, Aisling target)
    {
        if (source.Equals(target))
            return false;

        // Non-arena maps: not hostile
        if (!source.IsOnArenaMap())
            return false;

        // PvE arena maps (lobbies, etc.): not hostile
        if (source.IsOnPvEArenaMap())
            return false;

        // Group members: not hostile
        if (source.Group?.Contains(target) == true)
            return false;

        // Same team: not hostile
        if (source.IsOnSameArenaTeam(target))
            return false;

        // On PvP arena, not grouped, not same team: hostile
        return true;
    }

    protected virtual bool IsHostileTo(Aisling source, Merchant target) => false;

    protected virtual bool IsHostileTo(Aisling source, Monster target)
    {
        if (target.Type is CreatureType.WhiteSquare)
            return false;

        var isPet = target.Script.Is<PetScript>() || source.Script.Is<PetScript>();

        if (isPet)
            return false;

        var isTotem = target.Script.Is<NightmareTotemScript>() || source.Script.Is<NightmareTotemScript>();

        if (isTotem)
            return false;

        var isZoe = target.Script.Is<SacrificeZoe>() || source.Script.Is<SacrificeZoe>();

        if (isZoe)
            return false;

        var isSlave = target.Script.Is<NightmareSlaveScript>() || source.Script.Is<NightmareSlaveScript>();

        if (isSlave)
            return false;

        var isTeammate = target.Script.Is<NightmareTeammateScript>() || source.Script.Is<NightmareTeammateScript>();

        if (isTeammate)
            return false;

        return true;
    }

    protected virtual bool IsHostileTo(Merchant source, Aisling target) => false;

    protected virtual bool IsHostileTo(Merchant source, Merchant target) => false;

    protected virtual bool IsHostileTo(Merchant source, Monster target)
    {
        if (target.Type is CreatureType.WhiteSquare)
            return false;

        return true;
    }

    protected virtual bool IsHostileTo(Monster source, Aisling target)
    {
        if (source.Type is CreatureType.WhiteSquare)
            return false;

        if (source.Script.Is<PetScript>() || target.Script.Is<PetScript>())
        {
            var isGroupMember = source.PetOwner?.Group?.Contains(target) == true;
            var isOwner = target.Equals(source.PetOwner);

            var isAisling = source.MapInstance
                                  .GetEntities<Aisling>()
                                  .Any();

            return !(isGroupMember || isOwner || isAisling);
        }

        if (source.Script.Is<SacrificeZoe>() || target.Script.Is<SacrificeZoe>())
        {
            var isGroupMember = source.PetOwner?.Group?.Contains(target) == true;
            var isOwner = target.Equals(source.PetOwner);

            return !(isGroupMember || isOwner);
        }

        if (source.Script.Is<NightmareSlaveScript>() || target.Script.Is<NightmareSlaveScript>())
        {
            var isGroupMember = source.PetOwner?.Group?.Contains(target) == true;
            var isOwner = target.Equals(source.PetOwner);

            return !(isGroupMember || isOwner);
        }

        if (source.Script.Is<NightmareTeammateScript>() || target.Script.Is<NightmareTeammateScript>())
        {
            var isGroupMember = source.PetOwner?.Group?.Contains(target) == true;
            var isOwner = target.Equals(source.PetOwner);

            return !(isGroupMember || isOwner);
        }

        return true;
    }

    protected virtual bool IsHostileTo(Monster source, Merchant target)
    {
        if (source.Type is CreatureType.WhiteSquare)
            return false;

        return true;
    }

    protected virtual bool IsHostileTo(Monster source, Monster target)
    {
        if (source.Type is CreatureType.WhiteSquare || target.Type is CreatureType.WhiteSquare)
            return false;

        if (source.Script.Is<PetScript>() ^ target.Script.Is<PetScript>())
            return true;

        if (source.Script.Is<NightmareTotemScript>() ^ target.Script.Is<NightmareTotemScript>())
            return true;

        if (source.Script.Is<SacrificeZoe>() ^ target.Script.Is<SacrificeZoe>())
            return true;

        if (source.Script.Is<NightmareSlaveScript>() ^ target.Script.Is<NightmareSlaveScript>())
            return true;

        if (source.Script.Is<NightmareTeammateScript>() ^ target.Script.Is<NightmareTeammateScript>())
            return true;

        return false;
    }
}
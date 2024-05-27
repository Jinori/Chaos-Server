using Chaos.Common.Utilities;
using Chaos.Definitions;
using Chaos.Extensions.Geometry;
using Chaos.Models.World;
using Chaos.Scripting.MonsterScripts.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Pet;

// ReSharper disable once ClassCanBeSealed.Global
public class PetAttackingScript : MonsterScriptBase
{
    /// <inheritdoc />
    public PetAttackingScript(Monster subject)
        : base(subject) { }

    public readonly Dictionary<SummonChosenPet, List<string>?> PetMessages = new()
    {
        { SummonChosenPet.Bunny, ["Hop!", "Nibble", "Sniff!", "Thump!", "Bounce", "Twitch", "Flop!", "Nuzzle", "Squeak", "Leap!"] },
        { SummonChosenPet.Faerie, ["Twirl!", "Giggle", "Spark!", "Glint", "Flutter", "Shine!", "Whirl", "Tinkle", "Glow!", "Dance!"] },
        { SummonChosenPet.Dog, ["Woof!", "Growl!", "Bark!", "Sniff", "Pant!", "Fetch!", "Chase", "Wag!", "Howl!", "Lick!"] },
        { SummonChosenPet.Ducklings, ["Quack!", "Peep!", "Splash", "Waddle", "Paddle", "Dabble", "Flap!", "Shake", "Preen", "Nestle"] },
        { SummonChosenPet.Cat, ["Purr!", "Meow!", "Hiss!", "Prowl", "Swipe", "Slink", "Mew!", "Groom", "Nap!", "Flick!"] },
        { SummonChosenPet.Penguin, ["Waddle", "Squawk", "Flap!", "Slide", "Dive!", "Peck!", "Huddle", "Preen", "Nuzzle", "Chirp!"] },
        { SummonChosenPet.Gloop, ["Gloop!", "Bubble", "Slosh", "Blurp", "Glurp", "Squelch", "Glop!", "Drip", "Slime!", "Ooze!"] },
        { SummonChosenPet.Smoldy, ["Smolder", "Flicker", "Flame!", "Burn", "Glow!", "Ashen", "Sear", "Blaze!", "Char", "Ignite!"] }
    };
    
    public string GetRandomChant(SummonChosenPet petType)
    {
        if (PetMessages.TryGetValue(petType, out var messages))
            if (messages != null)
                return messages.PickRandom();

        return "";
    }
    
    public override void Update(TimeSpan delta)
    {
        if (Subject.PetOwner?.DistanceFrom(Subject) >= 8)
        {
            Target = null;
            return;
        }
        
        if (Target is not { IsAlive: true } || (Subject.DistanceFrom(Target) != 1))
            return;

        var direction = Target.DirectionalRelationTo(Subject);

        if (Subject.Direction != direction)
            return;

        var lastWalk = Subject.Trackers.LastWalk ?? DateTime.MinValue;

        if (DateTime.UtcNow.Subtract(lastWalk)
                    .TotalMilliseconds
            < Subject.EffectiveAssailIntervalMs)
            return;

        var attacked = false;

        if (Subject.DistanceFrom(Target) >= 1)
            if (ShouldUseSpell)
                foreach (var spell in Spells)
                    if (IntegerRandomizer.RollChance(10))
                        Subject.TryUseSpell(spell, Target.Id);
        
        foreach (var skill in Skills)
            if (skill.Template.IsAssail)
                attacked |= Subject.TryUseSkill(skill);

        if (ShouldUseSkill)
        {
            var skill = Skills.Where(skill => Subject.CanUse(skill, out _))
                              .PickRandomWeightedSingleOrDefault(7);

            if (skill is not null)
            {            
                var petKey = SummonChosenPet.None;
                var found = Subject.PetOwner?.Trackers.Enums.TryGetValue(out petKey) ?? false;

                if (found)
                {
                    var chant = GetRandomChant(petKey);
                    Subject.Chant(chant);
                }
                attacked |= Subject.TryUseSkill(skill);
            }
        }

        if (attacked)
        {
            Subject.WanderTimer.Reset();
            Subject.MoveTimer.Reset();
        }
    }
}
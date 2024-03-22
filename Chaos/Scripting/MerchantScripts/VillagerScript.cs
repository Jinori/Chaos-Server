using Chaos.Common.Utilities;
using Chaos.Models.World;
using Chaos.Scripting.MerchantScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.MerchantScripts;

public class VillagerScript : MerchantScriptBase
{
    public VillagerState CurrentState;
    private readonly IIntervalTimer ActionTimer;
    private readonly ISpellFactory SpellFactory;
    
    /// <inheritdoc />
    public VillagerScript(Merchant subject, ISpellFactory spellFactory)
        : base(subject)
    {
        SpellFactory = spellFactory;
        ActionTimer = new IntervalTimer(TimeSpan.FromSeconds(10), false);
        CurrentState = VillagerState.Idle;
    }

    public enum VillagerState
    {
        Idle,
        Wandering,
        EnteringBuilding,
        TalkingToAnotherMerchant,
        TalkingToPlayer
    }

    private readonly List<string> VillagerMessages =
    [
        "Lovely weather we're having!",
        "Have you visited the blacksmith?",
        "Beware of wolves in the forest.",
        "The harvest is bountiful this year.",
        "Stay clear of the Dark Woods.",
        "Have you seen the new merchant?",
        "Legends speak of hidden treasures.",
        "The old well is said to be cursed.",
        "Travelers bring interesting tales.",
        "Mind the slippery cobblestones!",
        "Wish I could see the capital city.",
        "Keep your wits about you at night.",
        "Ever heard the howl of the banshee?",
        "Stay a while and listen.",
        "The river seems high this season.",
        "Look out for falling branches!",
        "A mysterious fog covered us last night.",
        "I miss the songs of the minstrels.",
        "Do you believe in forest spirits?",
        "I'd avoid the ancient ruins.",
        "I've heard tales of a secret cave.",
        "Our mayor is a wise and just leader.",
        "My garden is my greatest treasure.",
        "Best ale in town is at the tavern.",
        "The old tower? Avoid it at all costs!",
        "Strange lights were seen near the hill.",
        "A traveling bard visited yesterday.",
        "The wind speaks, if you listen...",
        "Ever tasted Mileth's famous stew?",
        "The mill's been quiet these days.",
        "Those mountains hold dark secrets.",
        "The full moon brings strange events.",
        "My family's lived here for generations.",
        "I once saw a ghost, I swear it!",
        "Sometimes, I hear whispers in the fog.",
        "I wouldn't stray far from the village.",
        "The forest isn't safe for travelers.",
        "Did you come to see the festival?",
        "Our village is small, but proud.",
        "Beware of the witch in the woods.",
        "Some say the lake is enchanted.",
        "I've never seen a dragon, thankfully.",
        "The ancient tree holds ancient power.",
        "Keep an eye on your belongings.",
        "Nothing beats our local honey.",
        "There's a chill in the air today.",
        "The old bookshop holds rare tomes.",
        "Watch the skies, stranger.",
        "Don't trust the goblins, ever.",
        "The stars have been bright lately."
    ];

    
    /// <inheritdoc />
    public override void Update(TimeSpan delta)
    {
        ActionTimer.Update(delta);
        
        switch (CurrentState)
        {
            case VillagerState.Idle:
                if (ActionTimer.IntervalElapsed)
                    HandleIdleState();
                break;
            case VillagerState.Wandering:
                if (Subject.WanderTimer.IntervalElapsed)
                    HandleWanderingState();
                break;
            case VillagerState.EnteringBuilding:
                break;
            case VillagerState.TalkingToAnotherMerchant:
                break;
            case VillagerState.TalkingToPlayer:
                break;
        }
    }

    private void HandleWanderingState()
    {
        Subject.Wander();
        
        if (Subject.WanderTimer.IntervalElapsed && IntegerRandomizer.RollChance(10))
        {
            CurrentState = VillagerState.Idle;
            ActionTimer.Reset();
        }
        
        Subject.WanderTimer.Reset();
    }

    private void HandleIdleState()
    {
        //Inspect players legend for unique legend marks and call them out
        
        //Inspect players gear for unique gear and talk about it
        
        
        if (IntegerRandomizer.RollChance(10)) 
            SayRandomMessage();
        
        if (IntegerRandomizer.RollChance(3))
            CastBuff();
        
        if (IntegerRandomizer.RollChance(35))
            CurrentState = VillagerState.Wandering;
        
        ActionTimer.Reset();
    }

    
    private void SayRandomMessage()
    {
        var aislings = Subject.MapInstance.GetEntitiesWithinRange<Aisling>(Subject, 8);

        if (aislings.Any())
            Subject.Say(GetRandomMessage());
    }

    private void CastBuff()
    {
        var aislings = Subject.MapInstance.GetEntitiesWithinRange<Aisling>(Subject, 8).Where(x => x.Effects.Contains("ardaite")).ToList();

        if (aislings.Count > 0)
        {
            var spell = SpellFactory.Create("ardnaomhaite");

            foreach (var player in aislings)
            {
                Subject.TryUseSpell(spell, player.Id);     
            }
        }
        Subject.Say("That should help a bit.");
    }
    
    private string GetRandomMessage()
    {
        var randomIndex = new Random().Next(VillagerMessages.Count);
        return VillagerMessages[randomIndex];
    }
}
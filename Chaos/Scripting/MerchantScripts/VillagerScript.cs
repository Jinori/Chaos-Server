using Chaos.Common.Definitions;
using Chaos.Common.Utilities;
using Chaos.Extensions.Geometry;
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
    public readonly Location MilethArmoryDoorPoint = new("mileth", 10, 31);
    public readonly Location MilethArmoryPoint = new("mileth_armor_shop", 4, 8);
    public readonly Location MilethArmoryDoorInsidePoint = new("mileth_armor_shop", 11, 8);
    public bool HasHadConversation;
    public bool HasSaidGreeting;
    public bool HasAskedForItem;
    public bool HasReceivedItem;
    private readonly IIntervalTimer WalkTimer;
    private readonly IIntervalTimer DialogueTimer;
    private readonly Point Spawnpoint;
    
    /// <inheritdoc />
    public VillagerScript(Merchant subject, ISpellFactory spellFactory)
        : base(subject)
    {
        SpellFactory = spellFactory;
        WalkTimer = new IntervalTimer(TimeSpan.FromMilliseconds(600), false);
        ActionTimer = new IntervalTimer(TimeSpan.FromSeconds(10), false);
        CurrentState = VillagerState.Idle;
        DialogueTimer = new RandomizedIntervalTimer(
            TimeSpan.FromSeconds(1),
            50,
            RandomizationType.Positive,
            false);
        
        Spawnpoint = new Point(Subject.X, Subject.Y);
    }

    public enum VillagerState
    {
        Idle,
        Wandering,
        WalkingToArmory,
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
            case VillagerState.WalkingToArmory:
                    HandleWalkingToArmory(delta);
                break;
            case VillagerState.TalkingToAnotherMerchant:
                break;
            case VillagerState.TalkingToPlayer:
                break;
        }
    }

    private void HandleWalkingToArmory(TimeSpan delta)
    {
        if ((Subject.DistanceFrom(MilethArmoryDoorPoint) > 0) && Subject.OnSameMapAs(MilethArmoryDoorPoint) && !HasHadConversation)
        {
            WalkTimer.Update(delta);

            if (WalkTimer.IntervalElapsed)
            {
                var door = Subject.MapInstance.GetEntitiesAtPoint<Door>(MilethArmoryDoorPoint).First();

                if (door.Closed)
                    door.OnClicked(Subject);

                Subject.Pathfind(MilethArmoryDoorPoint, 0);
            }
        }
        else if ((Subject.DistanceFrom(MilethArmoryPoint) > 0) && Subject.OnSameMapAs(MilethArmoryPoint) && !HasHadConversation)
        {
            WalkTimer.Update(delta);

            if (WalkTimer.IntervalElapsed)
                Subject.Pathfind(MilethArmoryPoint, 0);
        }
        else if ((Subject.DistanceFrom(MilethArmoryPoint) <= 2) && !HasHadConversation)
        {
            DialogueTimer.Update(delta);

            if (DialogueTimer.IntervalElapsed)
            {
                var merchant = Subject.MapInstance.GetEntitiesWithinRange<Merchant>(Subject)
                                      .FirstOrDefault(x => (x.Id != Subject.Id));
                
                if (!HasSaidGreeting)
                {
                    Subject.Say($"Hello {merchant?.Name}, how are you today?");
                    HasSaidGreeting = true;
                    DialogueTimer.Reset();
                }
                else if (!HasAskedForItem)
                {
                    var randomitem = merchant?.ItemsForSale.PickRandom();
                    Subject.Say($"I'd like to buy a {randomitem?.DisplayName} please.");
                    HasAskedForItem = true;
                    DialogueTimer.Reset();
                }
                else if (!HasReceivedItem)
                {
                    merchant?.Say($"Aahh, here you are {Subject.Name}.");
                    HasReceivedItem = true;
                    HasHadConversation = true;
                }
            }
        }
        else switch (HasHadConversation)
        {
            case true 
                 when (Subject.DistanceFrom(MilethArmoryDoorInsidePoint) > 0)
                      && Subject.OnSameMapAs(MilethArmoryDoorInsidePoint):
            {
                WalkTimer.Update(delta);

                if (WalkTimer.IntervalElapsed)
                    Subject.Pathfind(MilethArmoryDoorInsidePoint, 0);

                break;
            }
            case true when Subject.OnSameMapAs(MilethArmoryDoorPoint) && (Subject.DistanceFrom(Spawnpoint) > 0):
            {
                WalkTimer.Update(delta);

                if (WalkTimer.IntervalElapsed)
                    Subject.Pathfind(Spawnpoint, 0);

                break;
            }
            case true when Subject.OnSameMapAs(MilethArmoryDoorPoint) && (Subject.DistanceFrom(Spawnpoint) <= 1):
                HasHadConversation = false;
                HasReceivedItem = false;
                HasSaidGreeting = false;
                HasAskedForItem = false;
                CurrentState = VillagerState.Wandering;
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
        
        
        if (IntegerRandomizer.RollChance(5))
            CurrentState = VillagerState.WalkingToArmory;
        
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
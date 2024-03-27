using Chaos.Common.Definitions;
using Chaos.Common.Utilities;
using Chaos.Extensions.Geometry;
using Chaos.Geometry.Abstractions;
using Chaos.Models.World;
using Chaos.Scripting.MerchantScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.MerchantScripts;

public class VillagerScript : MerchantScriptBase
{
    public enum VillagerState
    {
        Idle,
        Wandering,
        WalkingToArmory,
        TalkingToAnotherMerchant,
        TalkingToPlayer
    }
    
    #region Messages
    private static readonly List<string> ItemReceivedResponses =
    [
        "Thanks a lot!",
        "Just what I needed, thank you!",
        "Perfect, appreciate it!",
        "Exactly what I wanted!",
        "You read my mind!",
        "Couldn't be happier!",
        "This is fantastic, thanks!",
        "Spot on, thank you!",
        "You're the best!",
        "Marvelous choice!",
        "Superb, just superb!",
        "Top-notch, as always!",
        "You never disappoint!",
        "Great, just great!",
        "This will do nicely!",
        "Well done, thank you!",
        "You've outdone yourself!",
        "Exceptional, truly!",
        "Remarkable service!",
        "A splendid choice!",
        "Excellent, very pleased!",
        "Oh, this is great!",
        "That's really nice!",
        "Amazing, thank you!",
        "Right on the mark!",
        "So pleased with this!",
        "Quality as expected!",
        "You've nailed it!",
        "Such a fine selection!",
        "A masterful choice!"
    ];

    private static readonly List<string> PurchaseRequests =
    [
        "I'd like to buy a {0} please.",
        "Could I get one {0}?",
        "I'm interested in a {0}.",
        "Can I purchase a {0}?",
        "One {0}, if you don't mind.",
        "How about a {0} for me?",
        "May I have a {0}?",
        "I'll take a {0}, please.",
        "Looking to buy a {0}.",
        "Can you spare a {0}?",
        "I need a {0}, please.",
        "A {0} would be great.",
        "I'd love a {0}.",
        "Please, a {0}?",
        "Would you sell a {0}?",
        "I'm in need of a {0}.",
        "A {0}, if possible.",
        "Let's go with a {0}.",
        "Is the {0} available?",
        "Hand me a {0}, please.",
        "I choose the {0}.",
        "I'd prefer a {0}.",
        "I think a {0} suits me.",
        "Any {0} left for sale?",
        "How's the {0} priced?",
        "May I secure a {0}?",
        "I vote for a {0}.",
        "Let's try the {0}.",
        "How much for a {0}?",
        "Got a {0} in stock?"
    ];

    private static readonly List<string> Greetings =
    [
        "Hello {0}, how are you today?",
        "Good day, {0}! How's everything?",
        "Hi {0}, lovely to see you!",
        "Hey {0}, great to meet you!",
        "Greetings, {0}!",
        "Howdy {0}, what's new?",
        "Hello there, {0}!",
        "Ah, {0}! Good to see you.",
        "Hiya {0}! How are you?",
        "Welcome, {0}! How goes it?",
        "Well met, {0}!",
        "Heya {0}, how's life?",
        "Hello {0}, what's up?",
        "Good to see you, {0}!",
        "Pleased to meet you, {0}.",
        "How's it going, {0}?",
        "Nice to see you, {0}.",
        "What's happening, {0}?",
        "Ahoy {0}!",
        "Salutations, {0}!",
        "Hey there, {0}!",
        "Hello {0}, nice to meet you.",
        "Greetings and welcome, {0}!",
        "Hi {0}, what brings you here?",
        "Ello, {0}!",
        "Ola, {0}! How's your day?",
        "How's everything, {0}?",
        "Hi {0}, all well?",
        "Greetings, {0}, how do you do?",
        "Hey {0}, any news?"
    ];

    private static readonly List<string> MerchantResponses =
    [
        "Certainly! One {0} coming right up.",
        "You've got it. I'll get a {0} for you.",
        "Absolutely! A {0} is an excellent choice.",
        "A favorite of mine. I'll fetch one for you.",
        "Brilliant! The {0} is one of our best!",
        "A {0}? Fantastic! Let me grab that.",
        "Oh, a {0}? An interesting selection!",
        "The {0}? That's an unusual choice, but great!",
        "The {0}, huh? It has a story behind it.",
        "Thank you for choosing the {0}.",
        "A {0}, right away!",
        "Good eye! The {0} is perfect.",
        "A {0}? Excellent taste!",
        "Right then, a {0} for you!",
        "You'll love the {0}.",
        "One moment, getting your {0}.",
        "Sure thing, a {0} coming up.",
        "A wise choice, the {0} it is!",
        "Ah, the {0}! Coming up.",
        "Happy to provide a {0}.",
        "One {0}, with pleasure!",
        "The {0}? A superb choice!",
        "I recommend the {0}.",
        "Your {0} is right here.",
        "Preparing the {0} now.",
        "A {0}? You won't regret it!",
        "The {0}, always a good pick.",
        "One {0}, as requested.",
        "Let's get you that {0}.",
        "You'll be pleased with the {0}."
    ];
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
    #endregion Messages
    private readonly IIntervalTimer ActionTimer;
    private readonly IIntervalTimer DialogueTimer;
    private readonly IIntervalTimer WalkTimer;
    public readonly Location MilethArmoryDoorInsidePoint = new("mileth_armor_shop", 11, 8);
    public readonly Location MilethArmoryDoorPoint = new("mileth", 10, 31);
    public readonly Location MilethArmoryPoint = new("mileth_armor_shop", 4, 8);
    private readonly Location Spawnpoint;
    private readonly ISpellFactory SpellFactory;
    public VillagerState CurrentState;
    public bool HasAskedForItem;
    public bool HasHadConversation;
    public bool HasMerchantResponded;
    public bool HasReceivedItem;
    public bool HasSaidGreeting;
    private string? ItemRequested;

    /// <inheritdoc />
    public VillagerScript(Merchant subject, ISpellFactory spellFactory)
        : base(subject)
    {
        SpellFactory = spellFactory;
        WalkTimer = new IntervalTimer(TimeSpan.FromMilliseconds(600), false);
        ActionTimer = new IntervalTimer(TimeSpan.FromSeconds(10), false);
        CurrentState = VillagerState.Idle;

        DialogueTimer = new RandomizedIntervalTimer(
            TimeSpan.FromSeconds(1.5),
            50,
            RandomizationType.Positive,
            false);

        Spawnpoint = new Location(Subject.MapInstance.InstanceId, Subject.X, Subject.Y);
    }

    private void AttemptToOpenDoor(IPoint doorPoint)
    {
        var door = Subject.MapInstance.GetEntitiesAtPoint<Door>(doorPoint).FirstOrDefault();

        if (door is { Closed: true })
            door.OnClicked(Subject);
    }

    private void CastBuff()
    {
        var aislings = Subject.MapInstance.GetEntitiesWithinRange<Aisling>(Subject, 8).Where(x => x.Effects.Contains("ardaite")).ToList();

        if (aislings.Count > 0)
        {
            var spell = SpellFactory.Create("ardnaomhaite");

            foreach (var player in aislings)
                Subject.TryUseSpell(spell, player.Id);
        }

        Subject.Say("That should help a bit.");
    }

    private void ConductDialogueWithMerchant()
    {
        var merchant = Subject.MapInstance.GetEntitiesWithinRange<Merchant>(Subject)
                              .FirstOrDefault(x => x.Id != Subject.Id);

        if (!HasSaidGreeting)
        {
            var greeting = string.Format(PickRandom(Greetings), merchant?.Name);
            Subject.Say(greeting);
            HasSaidGreeting = true;
            DialogueTimer.Reset();
        }
        else if (!HasAskedForItem)
        {
            var randomItem = merchant?.ItemsForSale.PickRandom();
            var purchaseRequest = string.Format(PickRandom(PurchaseRequests), randomItem?.DisplayName);
            ItemRequested = randomItem?.DisplayName;
            Subject.Say(purchaseRequest);
            HasAskedForItem = true;
            DialogueTimer.Reset();
        }
        else if (!HasMerchantResponded)
        {
            var purchaseRequest = string.Format(PickRandom(MerchantResponses), ItemRequested);
            merchant?.Say(purchaseRequest);
            HasMerchantResponded = true;
            DialogueTimer.Reset();
        }
        else if (!HasReceivedItem)
        {
            var response = PickRandom(ItemReceivedResponses);
            Subject.Say(response);
            HasReceivedItem = true;
            HasHadConversation = true;
        }
    }

    private string GetRandomMessage()
    {
        var randomIndex = IntegerRandomizer.RollSingle(VillagerMessages.Count);

        return VillagerMessages[randomIndex];
    }

    private void HandleApproachToArmory(TimeSpan delta)
    {
        if (ShouldWalkTo(MilethArmoryDoorPoint))
            WalkTowards(MilethArmoryDoorPoint, delta);
        else if (ShouldWalkTo(MilethArmoryPoint))
            WalkTowards(MilethArmoryPoint, delta);
    }

    private void HandleConversationWithMerchant(TimeSpan delta)
    {
        if (IsCloseTo(MilethArmoryPoint, 2))
            UpdateDialogue(delta);
    }

    private void HandleIdleState()
    {
        //Inspect players legend for unique legend marks and call them out

        //Inspect players gear for unique gear and talk about it

        if (IntegerRandomizer.RollChance(80))
            CurrentState = VillagerState.WalkingToArmory;

        if (IntegerRandomizer.RollChance(10))
            SayRandomMessage();

        if (IntegerRandomizer.RollChance(3))
            CastBuff();

        if (IntegerRandomizer.RollChance(35))
            CurrentState = VillagerState.Wandering;

        ActionTimer.Reset();
    }

    private void HandlePostConversationActions(TimeSpan delta)
    {
        if (ShouldWalkTo(MilethArmoryDoorInsidePoint))
            WalkTowards(MilethArmoryDoorInsidePoint, delta);
        else if (ShouldWalkToSpawnPoint())
            WalkTowards(Spawnpoint, delta);
        else
            ResetConversationState();
    }

    private void HandleWalkingToArmory(TimeSpan delta)
    {
        if (!HasHadConversation)
        {
            HandleApproachToArmory(delta);
            HandleConversationWithMerchant(delta);
        }
        else
            HandlePostConversationActions(delta);
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

    private bool IsCloseTo(Location point, int distance) => Subject.DistanceFrom(point) <= distance;

    private static string PickRandom(IReadOnlyList<string> phrases)
    {
        var index = IntegerRandomizer.RollSingle(phrases.Count);
        return phrases[index];
    }

    private void ResetConversationState()
    {
        HasHadConversation = false;
        HasReceivedItem = false;
        HasSaidGreeting = false;
        HasAskedForItem = false;
        HasMerchantResponded = false;
        ItemRequested = null;
        CurrentState = VillagerState.Wandering;
    }

    private void SayRandomMessage()
    {
        var aislings = Subject.MapInstance.GetEntitiesWithinRange<Aisling>(Subject, 8);

        if (aislings.Any())
            Subject.Say(GetRandomMessage());
    }

    private bool ShouldWalkTo(Location destination) => (Subject.DistanceFrom(destination) > 0) && Subject.OnSameMapAs(destination);

    private bool ShouldWalkToSpawnPoint() => Subject.OnSameMapAs(MilethArmoryDoorPoint) && (Subject.DistanceFrom(Spawnpoint) > 0);

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

    private void UpdateDialogue(TimeSpan delta)
    {
        DialogueTimer.Update(delta);

        if (DialogueTimer.IntervalElapsed)
            ConductDialogueWithMerchant();
    }

    private void UpdateWalkTimer(TimeSpan delta) => WalkTimer.Update(delta);

    private void WalkTowards(Location destination, TimeSpan delta)
    {
        UpdateWalkTimer(delta);

        if (WalkTimer.IntervalElapsed)
        {
            if (destination == MilethArmoryDoorPoint)
                AttemptToOpenDoor(MilethArmoryDoorPoint);

            Subject.Pathfind(destination, 0);
        }
    }
}
using Chaos.Collections;
using Chaos.DarkAges.Definitions;
using Chaos.Extensions;
using Chaos.Extensions.Geometry;
using Chaos.Models.Legend;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Scripting.MonsterScripts.Pet;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;
using Chaos.Time;

namespace Chaos.Scripting.DialogScripts.Events.Christmas;

public class TeleportToFrostyChallenge : DialogScriptBase
{
    private readonly IItemFactory ItemFactory;
    private readonly ISimpleCache SimpleCache;
    private readonly ISpellFactory SpellFactory;

    /// <inheritdoc />
    public TeleportToFrostyChallenge(
        Dialog subject,
        ISimpleCache simpleCache,
        IItemFactory itemFactory,
        ISpellFactory spellFactory)
        : base(subject)
    {
        SimpleCache = simpleCache;
        ItemFactory = itemFactory;
        SpellFactory = spellFactory;
    }

    public override void OnDisplaying(Aisling source)
    {
        switch (Subject.Template.TemplateKey.ToLower()) // Switch based on TemplateKey
        {
            case "frostychallenge_leave":
                TeleportPlayerToNorthPole(source);

                break;

            case "frostychallenge_join":
                TeleportPlayerToChallenge(source);

                break;
            case "frostychallenge_checkprogress":
                RewardPlayer(source);

                break;

            case "elf4_frostychallenge":
                TeleportPlayerToFrostyChallengeMap(source);

                break;

            // Add more cases as needed
            default:
                source.SendOrangeBarMessage("Invalid option. Please try again.");

                break;
        }
    }

    private void RewardPlayer(Aisling source)
    {
        source.Trackers.Counters.TryGetValue("frostysurvived2minutes", out var survivor);

        if ((survivor >= 20) && !source.Legend.ContainsKey("frostysurvivor"))
        {
            var uniqueReward = ItemFactory.Create("reindeerbuddy");

            source.GiveItemOrSendToBank(uniqueReward);

            source.Legend.AddUnique(
                new LegendMark(
                    "Survivor of Frosty's Challenge",
                    "frostysurvivor",
                    MarkIcon.Victory,
                    MarkColor.Pink,
                    1,
                    GameTime.Now));

            source.SendOrangeBarMessage("You received a Reindeer Buddy and a unique Legend Mark!");

            Subject.Reply(
                source,
                "You are a true survivor! You received a new legend mark and a unique reward item! Good job out there, I've watched every single one of your matches, you have skills! Maybe you'll come be an elf one day.");

            return;
        }

        if (source.Legend.ContainsKey("frostysurvivor"))
        {
            Subject.Reply(source, "You have really proven yourself Aisling. You are a true survivor!");

            return;
        }

        Subject.Reply(source, "You're not a true survivor yet, you'll need to complete your training!");
    }

    private void TeleportPlayerToChallenge(Aisling source)
    {
        if (source.MapInstance
                  .GetEntities<Monster>()
                  .Any(x => x.Name == "Smiley Blob Bomb"))
        {
            Subject.Reply(source, "You cannot join the challenge until it is over.");

            return;
        }

        var rectangle = new Rectangle(
            8,
            8,
            17,
            13);
        Subject.Close(source);

        source.Trackers.Counters.Remove("frostychallenge", out _);

        var point = rectangle.GetRandomPoint();

        if (source.Effects.Contains("Mount"))
            source.Effects.Dispel("Mount");

        source.WarpTo(point);
    }

    private void TeleportPlayerToFrostyChallengeMap(Aisling source)
    {
        var rectangle = new Rectangle(
            2,
            7,
            3,
            3);
        Subject.Close(source);

        if (source.Effects.Any())
        {
            var npc = source.MapInstance
                            .GetEntities<Merchant>()
                            .FirstOrDefault(x => x.Name.Contains("Elf"));
            var aosith = SpellFactory.Create("aosith");
            npc?.TryUseSpell(aosith, source.Id);
        }

        var mapInstance = SimpleCache.Get<MapInstance>("mtmerry_frostychallenge");
        var point = rectangle.GetRandomPoint();

        var pets = source.MapInstance
                         .GetEntities<Monster>()
                         .Where(x => x.Script.Is<PetScript>() && x.Name.Contains(source.Name));

        foreach (var pet in pets)
            pet.MapInstance.RemoveEntity(pet);

        source.TraverseMap(mapInstance, point);
    }

    private void TeleportPlayerToNorthPole(Aisling source)
    {
        var rectangle = new Rectangle(
            22,
            12,
            5,
            5);
        var mapInstance = SimpleCache.Get<MapInstance>("mtmerry_northpole");

        Subject.Close(source);

        Point point;

        do
            point = rectangle.GetRandomPoint();
        while (!mapInstance.IsWalkable(point, source.Type));

        source.TraverseMap(mapInstance, point);
    }
}
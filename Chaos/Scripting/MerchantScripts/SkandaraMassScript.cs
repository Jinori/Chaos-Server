using Chaos.Common.Utilities;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Networking.Abstractions;
using Chaos.Scripting.MerchantScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Humanizer;

namespace Chaos.Scripting.MerchantScripts;

public class SkandaraMassScript(
    Merchant subject,
    IClientRegistry<IWorldClient> clientRegistry,
    IItemFactory itemFactory
)
    : MerchantScriptBase(subject)
{
    private const int FAITH_REWARD = 25;
    private const int LATE_FAITH_REWARD = 15;
    private const int ESSENCE_CHANCE = 10;
    private const int SERMON_DELAY_SECONDS = 5;
    private const int MASS_SERMON_COUNT = 10;

    #region MassMessages
    private readonly List<string> SkandaraMassMessages =
    [
        "Gather as warriors, united in strength.",
        "Ignite the fire of courage within your hearts.",
        "Embrace the fury of battle, let it fuel your spirit.",
        "Stand firm, unwavering, like a mighty fortress.",
        "May the might of Skandara guide our every step.",
        "Unleash your inner warrior, fierce and determined.",
        "Let the strength of Skandara surge through your veins.",
        "Embrace the power within, for you are mighty.",
        "Forge your path with the resilience of stone.",
        "May Skandara's blessings grant us victory.",
        "Channel your inner strength, unstoppable and resolute.",
        "Unleash your battle cry, let it echo in the winds.",
        "Embrace the unity of warriors, bound by honor.",
        "May Skandara's might be our shield and sword.",
        "Harness the power of Skandara, unwavering and fierce.",
        "Embody the indomitable spirit of the warrior.",
        "Let Skandara's strength guide our every strike.",
        "Stand tall, unyielding in the face of adversity.",
        "May the fires of Skandara burn bright within us.",
        "Rise as warriors, ready to conquer any challenge.",
        "Embrace the courage within, for you are unstoppable.",
        "Ignite the flames of determination in your hearts.",
        "May Skandara's presence inspire courage in us all.",
        "Unleash your inner power, strong as the earth.",
        "Forge your destiny with the strength of Skandara.",
        "Stand together, a force that cannot be shaken.",
        "Embrace the path of the warrior, honor as your guide.",
        "May Skandara's blessings grant us victory and glory.",
        "Unleash your passion, let it fuel your every stride.",
        "Harness the might of Skandara, a force to be reckoned with.",
        "Embody the spirit of Skandara, unyielding and relentless.",
        "Let the valor of warriors fill the air, resolute and fierce.",
        "May Skandara's presence fortify our hearts and minds.",
        "Stand firm, united in the strength of Skandara.",
        "Embrace the warrior within, a force to be reckoned with.",
        "Unleash your inner hero, brave and unstoppable.",
        "Forge your legacy with the power of Skandara.",
        "May Skandara's blessings embolden our every action.",
        "Ignite the flame of determination, burn bright and fierce.",
        "Embody the resilience of Skandara, unyielding and steadfast.",
        "Let the battlefield echo with the might of warriors.",
        "Stand together, unbreakable, in Skandara's name.",
        "Embrace the warrior's code, honor and strength.",
        "May Skandara's presence inspire greatness within us.",
        "Unleash your battle prowess, let it shine on the battlefield.",
        "Harness the raw power of Skandara, an unstoppable force.",
        "Embody the spirit of Skandara, undaunted and resolute.",
        "Let your actions speak of valor and unwavering resolve.",
        "Stand tall, warriors, defenders of Skandara's realm.",
        "Embrace the path of the warrior, fierce and noble.",
        "Unleash your inner warrior, fearless and relentless.",
        "Forge your destiny with the fire of Skandara's might.",
        "May Skandara's blessings guide our blades to victory.",
        "Ignite the flames of passion, let them consume your foes.",
        "Embody the strength of Skandara, unbreakable and true.",
        "Let the battlefield witness your unyielding courage.",
        "Stand united, warriors, in Skandara's name.",
        "Embrace the warrior's spirit, fearless and honorable.",
        "Unleash your warrior heart, let it roar in battle.",
        "Forge your legacy with Skandara's unmatched might.",
        "May Skandara's presence fill us with unwavering resolve.",
        "Harness the power of Skandara, fierce and formidable.",
        "Embody the indomitable spirit of the warrior.",
        "Let Skandara's strength flow through you, unstoppable.",
        "Stand firm, unshakable in the face of adversity.",
        "May the fires of Skandara light our path to glory.",
        "Rise as warriors, ready to conquer any challenge.",
        "Embrace the courage within, for you are warriors."
    ];
    #endregion MassMessages

    private IEnumerable<Aisling>? AislingsAtStart { get; set; }

    private bool AnnouncedMassBegin { get; set; }

    private bool AnnouncedMassFiveMinutes { get; set; }
    private bool AnnouncedMassOneMinute { get; set; }
    private DateTime? LastSermonTime { get; set; }
    private DateTime? MassAnnouncementTime { get; set; }
    private bool MassCompleted { get; set; }
    private int SermonCount { get; set; }
    protected IClientRegistry<IWorldClient> ClientRegistry { get; } = clientRegistry;

    protected IItemFactory ItemFactory { get; } = itemFactory;

    protected Animation PrayerSuccess { get; } = new()
    {
        AnimationSpeed = 60,
        TargetAnimation = 5
    };
    private HashSet<string> SpokenMessages { get; } = new();

    private void AnnounceMassBeginning()
    {
        foreach (var client in ClientRegistry)
            client.Aisling.SendActiveMessage("Mass held by Skandara at the temple is starting now.");

        AislingsAtStart = Subject.MapInstance.GetEntities<Aisling>().ToList();

        Subject.Say($"{AislingsAtStart.Count().ToWords().Humanize(LetterCasing.Title)} aislings bless me with their presence.");
    }

    public void AnnounceMassFiveMinuteStart()
    {
        foreach (var client in ClientRegistry)
            client.Aisling.SendActiveMessage("Skandara will be holding mass at her temple in five minutes.");
    }

    public void AnnounceMassOneMinuteStart()
    {
        foreach (var client in ClientRegistry)
            client.Aisling.SendActiveMessage("Skandara will be holding mass at her temple in one minute.");
    }

    public static string? CheckDeity(Aisling source)
    {
        if (source.Legend.ContainsKey("Skandara"))
            return "Skandara";

        return null;
    }

    private void ConductMass()
    {
        if (SermonCount >= MASS_SERMON_COUNT)
        {
            MassCompleted = true; // Set flag to true when mass is finished

            return;
        }

        // Check if it's time to say the next sermon
        if (LastSermonTime.HasValue && (DateTime.UtcNow.Subtract(LastSermonTime.Value).TotalSeconds < SERMON_DELAY_SECONDS))
            return;

        // Check if we have said all sermons
        if (SermonCount >= MASS_SERMON_COUNT)
            return;

        var random = new Random();
        int index;

        do
            index = random.Next(SkandaraMassMessages.Count);
        while (SpokenMessages.Contains(SkandaraMassMessages[index]));

        var message = SkandaraMassMessages[index];
        Subject.Say(message);
        SpokenMessages.Add(message);
        LastSermonTime = DateTime.UtcNow;
        SermonCount++;
    }

    private void PostMassActions()
    {
        Subject.Say("Mass is complete!");
        var aislingsAtEnd = Subject.MapInstance.GetEntities<Aisling>().ToList();
        var aislingsStillHere = AislingsAtStart!.Intersect(aislingsAtEnd).ToList();

        foreach (var player in aislingsStillHere)
        {
            player.Animate(PrayerSuccess);

            if (IntegerRandomizer.RollChance(ESSENCE_CHANCE))
            {
                var item = ItemFactory.Create("essenceofSkandara");
                player.Inventory.TryAddToNextSlot(item);
                player.SendActiveMessage("You received an Essence of Skandara");
            }

            TryAddFaith(player, FAITH_REWARD);
        }

        foreach (var latePlayers in aislingsAtEnd.Except(aislingsStillHere))
        {
            latePlayers.SendActiveMessage("You must be present from start to finish to receive full benefits.");
            TryAddFaith(latePlayers, LATE_FAITH_REWARD);
        }

        Subject.CurrentlyHostingMass = false;
    }

    public static bool TryAddFaith(Aisling source, int amount)
    {
        var key = CheckDeity(source);

        if ((key != null) && source.Legend.TryGetValue(key, out var faith))
        {
            faith.Count += amount;
            source.SendActiveMessage($"You received {amount} faith!");

            return true;
        }

        return false;
    }

    public override void Update(TimeSpan delta)
    {
        if (Subject.CurrentlyHostingMass)
        {
            if (!AnnouncedMassFiveMinutes)
            {
                AnnounceMassFiveMinuteStart();
                MassAnnouncementTime = DateTime.UtcNow;
                AnnouncedMassFiveMinutes = true;
            }
            else if (MassAnnouncementTime.HasValue
                     && (DateTime.UtcNow.Subtract(MassAnnouncementTime.Value).TotalMinutes >= 4)
                     && !AnnouncedMassOneMinute)
            {
                AnnounceMassOneMinuteStart();
                AnnouncedMassOneMinute = true;
            }
            else if (MassAnnouncementTime.HasValue
                     && (DateTime.UtcNow.Subtract(MassAnnouncementTime.Value).TotalMinutes >= 5)
                     && !AnnouncedMassBegin)
            {
                AnnounceMassBeginning();
                AnnouncedMassBegin = true;
                MassAnnouncementTime = null; // Resetting the timer
            }
            else if (AnnouncedMassBegin && !MassCompleted)
                ConductMass();
            else if (MassCompleted)
                PostMassActions();
        }
    }
}
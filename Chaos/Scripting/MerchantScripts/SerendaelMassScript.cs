using Chaos.Common.Utilities;
using Chaos.Formulae;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Networking.Abstractions;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ExperienceDistribution;
using Chaos.Scripting.MerchantScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Humanizer;

namespace Chaos.Scripting.MerchantScripts;

public class SerendaelMassScript : MerchantScriptBase
{
    private const int FAITH_REWARD = 25;
    private const int LATE_FAITH_REWARD = 15;
    private const int ESSENCE_CHANCE = 10;
    private const int SERMON_DELAY_SECONDS = 5;
    private const int MASS_SERMON_COUNT = 10;

    #region MassMessages
    private readonly List<string> SerendaelMassMessages =
    [
        "Embrace the ever-changing nature of fortune.",
        "Let destiny's path unfold before us.",
        "Find silver linings in unexpected circumstances.",
        "Navigate the twists and turns of life's journey.",
        "Embrace the transformative potential of luck.",
        "May fortune smile upon us in all endeavors.",
        "Celebrate the mysterious dance of destiny.",
        "Trust in the unseen forces guiding our lives.",
        "Embrace the ebb and flow of fortune's tide.",
        "Let serendipity illuminate our path forward.",
        "Acknowledge the role of luck in shaping our lives.",
        "Find beauty in the serendipitous moments.",
        "Embrace the unexpected with open arms.",
        "May the winds of fortune be ever in our favor.",
        "Seek opportunities in unexpected places.",
        "Embrace the magic of serendipity.",
        "Allow destiny's hand to guide us forward.",
        "Navigate the twists of fate with grace.",
        "Find hope in the serendipitous turns of life.",
        "Embrace the uncertainty and embrace destiny.",
        "Trust in the unseen forces shaping our lives.",
        "Let the whispers of fortune guide our steps.",
        "Celebrate the dance of chance and serendipity.",
        "Believe in the power of luck and destiny.",
        "Embrace the unpredictable nature of fate.",
        "May luck accompany us on our journey.",
        "Find hidden opportunities in unexpected moments.",
        "Embrace the unexpected as a gateway to new possibilities.",
        "Trust in the intricate tapestry of destiny.",
        "Embrace the mysteries of life's unexpected turns.",
        "Allow fortune's hand to shape our lives.",
        "Navigate the intricate dance of chance.",
        "Find joy in the surprises life has in store.",
        "Embrace the whims of fate with open hearts.",
        "Believe in the power of serendipity.",
        "May fortune guide our path with grace.",
        "Find beauty in the random synchronicities of life.",
        "Embrace the magic of unexpected encounters.",
        "Allow destiny's web to weave our lives together.",
        "Celebrate the symphony of serendipity.",
        "Trust in the cosmic forces shaping our existence.",
        "Embrace the surprises that color our journey.",
        "May luck shine upon us in every venture.",
        "Find treasures in the unexpected twists of life.",
        "Embrace the mysteries woven into our path.",
        "Navigate the intricate web of destiny with wonder.",
        "Embrace the delightful surprises of fate.",
        "Allow serendipity to guide our steps.",
        "Believe in the serendipitous encounters that shape us.",
        "May fortune smile upon us in all our endeavors.",
        "Find joy in the dance of chance and fortune.",
        "Embrace the unexpected as an opportunity for growth."
    ];

    public SerendaelMassScript(Merchant subject,
        IClientRegistry<IChaosWorldClient> clientRegistry,
        IItemFactory itemFactory) : base(subject)
    {
        ClientRegistry = clientRegistry;
        ItemFactory = itemFactory;
        ExperienceDistributionScript = DefaultExperienceDistributionScript.Create();
    }

    #endregion MassMessages
    private IExperienceDistributionScript ExperienceDistributionScript { get; }
    private IEnumerable<Aisling>? AislingsAtStart { get; set; }

    private bool AnnouncedMassBegin { get; set; }

    private bool AnnouncedMassFiveMinutes { get; set; }
    private bool AnnouncedMassOneMinute { get; set; }
    private DateTime? LastSermonTime { get; set; }
    private DateTime? MassAnnouncementTime { get; set; }
    private bool MassCompleted { get; set; }
    private int SermonCount { get; set; }
    protected IClientRegistry<IChaosWorldClient> ClientRegistry { get; }

    protected IItemFactory ItemFactory { get; }

    protected Animation PrayerSuccess { get; } = new()
    {
        AnimationSpeed = 60,
        TargetAnimation = 5
    };
    private HashSet<string> SpokenMessages { get; } = new();

    private void AnnounceMassBeginning()
    {
        foreach (var client in ClientRegistry)
            client.Aisling.SendActiveMessage("Mass held by Serendael at the temple is starting now.");

        AislingsAtStart = Subject.MapInstance.GetEntities<Aisling>().ToList();

        Subject.Say($"{AislingsAtStart.Count().ToWords().Humanize(LetterCasing.Title)} aislings bless me with their presence.");
    }

    public void AnnounceMassFiveMinuteStart()
    {
        foreach (var client in ClientRegistry)
            client.Aisling.SendActiveMessage("Serendael will be holding mass at her temple in five minutes.");
    }

    public void AnnounceMassOneMinuteStart()
    {
        foreach (var client in ClientRegistry)
            client.Aisling.SendActiveMessage("Serendael will be holding mass at her temple in one minute.");
    }

    public static string? CheckDeity(Aisling source)
    {
        if (source.Legend.ContainsKey("Serendael"))
            return "Serendael";

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
            index = random.Next(SerendaelMassMessages.Count);
        while (SpokenMessages.Contains(SerendaelMassMessages[index]));

        var message = SerendaelMassMessages[index];
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
                var item = ItemFactory.Create("essenceofSerendael");
                player.Inventory.TryAddToNextSlot(item);
                player.SendActiveMessage("You received an Essence of Serendael");
            }

            TryAddFaith(player, FAITH_REWARD);
            var tnl = LevelUpFormulae.Default.CalculateTnl(player);
            var twentyFivePercent = Convert.ToInt32(.25 * tnl);
                
            ExperienceDistributionScript.GiveExp(player, twentyFivePercent);
        }

        foreach (var latePlayers in aislingsAtEnd.Except(aislingsStillHere))
        {
            latePlayers.SendActiveMessage("You must be present from start to finish to receive full benefits.");
            TryAddFaith(latePlayers, LATE_FAITH_REWARD);
            var tnl = LevelUpFormulae.Default.CalculateTnl(latePlayers);
            var twentyFivePercent = Convert.ToInt32(.20 * tnl);
                
            ExperienceDistributionScript.GiveExp(latePlayers, twentyFivePercent);
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
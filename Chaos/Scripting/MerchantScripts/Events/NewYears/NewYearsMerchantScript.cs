using Chaos.Common.Utilities;
using Chaos.DarkAges.Definitions;
using Chaos.Extensions.Geometry;
using Chaos.Formulae;
using Chaos.Models.Data;
using Chaos.Models.Legend;
using Chaos.Models.World;
using Chaos.Networking.Abstractions;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ExperienceDistribution;
using Chaos.Scripting.MerchantScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.MerchantScripts.Events.NewYears;

public class NewYearsMerchantScript : MerchantScriptBase
{
    #region MassMessages
    private readonly List<string> NewYearMessage =
    [
        "The New Year is upon you Aislings, I would like to",
        " take a moment to recap our experience here in Unora!",
        "The Game Masters (GMs) would like to extend a special thanks",
        "to all of our beta testers for their dedication to Unora.",
        "Bivins (Dose), Jeff (Legend), and Nate (Whug) are grateful",
        "for all of the issues you've found within Unora.",
        "The year 2024 was the first time we have opened a game for",
        "beta testing and we have learned a lot from this.",
        "2024 was full of interesting bugs, from our grand mass",
        "fiasco to our release of untimely valentines day bugs.",
        "The endless times we've unintentionally killed players",
        "or power leveling up low levels by spawning monsters in towns.",
        "We are glad to celebrate the New Year with you all.",
        "The Firework show will begin momentarily, Aislings who",
        "are here at the beginning and end will receive a",
        "Unique Legend Mark, Experience, and 50 Game points.",
        "The Firework show will begin every hour for 24 hours,",
        "you may only gain rewards for it once.",
        "Again, thank you for everything Beta Testers! You are why we",
        "are here, you are why we continue to improve!",
        "Let the Fireworks Begin!"
    ];

    public NewYearsMerchantScript(Merchant subject,
        IClientRegistry<IChaosWorldClient> clientRegistry,
        IItemFactory itemFactory) : base(subject)
    {
        ClientRegistry = clientRegistry;
        ItemFactory = itemFactory;
        ExperienceDistributionScript = DefaultExperienceDistributionScript.Create();
        SayTimer = new IntervalTimer(TimeSpan.FromSeconds(3));
        FireworkDelay = new IntervalTimer(TimeSpan.FromMilliseconds(100));
        FireworkDelay2 = new IntervalTimer(TimeSpan.FromSeconds(3));
        FireworkTimer = new IntervalTimer(TimeSpan.FromMinutes(1), false);
    }

    #endregion MassMessages

    private IEnumerable<Aisling>? AislingsAtStart { get; set; }
    private IExperienceDistributionScript ExperienceDistributionScript { get; }

    private readonly IIntervalTimer SayTimer;
    
    private readonly IIntervalTimer FireworkTimer;
    
    private readonly IIntervalTimer FireworkDelay;
    
    private readonly IIntervalTimer FireworkDelay2;
    private bool AnnouncedFireworksBegin { get; set; }
    
    private int CurrentMessageIndex { get; set; }

    private bool AnnouncedFireWorksFiveMinutes { get; set; }
    private bool AnnouncedFireworksOneMinute { get; set; }
    
    private bool GMThankYouComplete { get; set; }
    private DateTime? FireworkAnnouncementTime { get; set; }
    private bool FireworksCompleted { get; set; }
    protected IClientRegistry<IChaosWorldClient> ClientRegistry { get; }

    protected IItemFactory ItemFactory { get; }

    protected Animation Firework1 { get; } = new()
    {
        AnimationSpeed = 200,
        TargetAnimation = 289
    };
    
    protected Animation Firework2 { get; } = new()
    {
        AnimationSpeed = 200,
        TargetAnimation = 294
    };
    
    protected Animation Firework3 { get; } = new()
    {
        AnimationSpeed = 200,
        TargetAnimation = 304
    };
    
    protected Animation Firework4 { get; } = new()
         {
        AnimationSpeed = 100,
        TargetAnimation = 359
    };
    
    protected Animation Ani { get; } = new()
    {
        AnimationSpeed = 200,
        TargetAnimation = 359
    };

    private void AnnounceFireworkBeginning()
    {
        foreach (var client in ClientRegistry)
            client.Aisling.SendActiveMessage("Fireworks show will begin in five minutes in Rucesion.");

        AislingsAtStart = Subject.MapInstance.GetEntities<Aisling>().Where(x => !x.Legend.ContainsKey("2025")).ToList();
    }

    private void ConductGMThankYou(TimeSpan delta)
    {
        SayTimer.Update(delta);
        
        if (SayTimer.IntervalElapsed && (CurrentMessageIndex < NewYearMessage.Count))
        {
            Subject.Say(NewYearMessage[CurrentMessageIndex]);
            CurrentMessageIndex++;

            if (CurrentMessageIndex >= NewYearMessage.Count)
            {
                GMThankYouComplete = true;
            }
        }
    }

    public void AnnounceFireworkFiveMinuteStart()
    {
        foreach (var client in ClientRegistry)
            client.Aisling.SendActiveMessage("Firework show will begin in five minutes in Rucesion.");
    }

    public void AnnounceFireworkOneMinuteStart()
    {
        foreach (var client in ClientRegistry)
            client.Aisling.SendActiveMessage("Firework show will begin in one minute in Rucesion.");
    }

    private void ConductFireworks(TimeSpan delta)
    {
        FireworkTimer.Update(delta);
        FireworkDelay.Update(delta);
        FireworkDelay2.Update(delta);
        
        if (FireworkTimer.IntervalElapsed)
        {
            FireworksCompleted = true;
        }

        if (FireworkDelay.IntervalElapsed)
        {
            var roll = IntegerRandomizer.RollSingle(3);

            var rectangle = new Rectangle(
                Subject.X - 12,
                Subject.Y - 12,
                13,
                13);

            var randomPoint = rectangle.GetRandomPoint();
            
            if (FireworkDelay2.IntervalElapsed)
            {
                Subject.MapInstance.ShowAnimation(Firework4.GetPointAnimation(randomPoint));
            }

            switch (roll)
            {
                case 1:
                    Subject.MapInstance.ShowAnimation(Firework1.GetPointAnimation(randomPoint));

                    break;
                case 2:
                    Subject.MapInstance.ShowAnimation(Firework2.GetPointAnimation(randomPoint));

                    break;
                case 3:
                    Subject.MapInstance.ShowAnimation(Firework3.GetPointAnimation(randomPoint));

                    break;
            }
        }
    }

    private void PostFireworkReward()
    {
        Subject.Say("Firework show is over!");
        var aislingsAtEnd = Subject.MapInstance.GetEntities<Aisling>().ToList();
        var aislingsStillHere = AislingsAtStart!.Intersect(aislingsAtEnd).Where(x => !x.Legend.ContainsKey("2025")).ToList();

        foreach (var player in aislingsStillHere)
        {
            player.Animate(Ani);

            if (player.UserStatSheet.Level < 99)
            {
                var tnl = LevelUpFormulae.Default.CalculateTnl(player);
                ExperienceDistributionScript.GiveExp(player, tnl);
            } else
                ExperienceDistributionScript.GiveExp(player, 100000000);
            
            player.Legend.AddUnique(
                new LegendMark(
                    "Celebrated the New Year (2025)",
                    "2025",
                    MarkIcon.Heart,
                    MarkColor.Yellow,
                    1,
                    GameTime.Now));
        }
        Subject.CurrentlyShootingFireworks = false;
    }

    private void ResetScript()
    {
        Subject.CurrentlyShootingFireworks = true;
        AnnouncedFireworksBegin = false;
        CurrentMessageIndex = 0;
        AnnouncedFireWorksFiveMinutes = false;
        AnnouncedFireworksOneMinute = false;
        GMThankYouComplete = false;
        FireworksCompleted = false;
    }

    public override void Update(TimeSpan delta)
    {
        var isEventActive = EventPeriod.IsEventActive(DateTime.UtcNow, Subject.MapInstance.InstanceId);

        if (!isEventActive)
        {
            Subject.MapInstance.RemoveEntity(Subject);

            return;
        }

        if ((DateTime.UtcNow.Minute == 55) && !Subject.CurrentlyShootingFireworks) 
            ResetScript();

        if (Subject.CurrentlyShootingFireworks)
        {
            if (!AnnouncedFireWorksFiveMinutes)
            {
                AnnounceFireworkFiveMinuteStart();
                FireworkAnnouncementTime = DateTime.UtcNow;
                AnnouncedFireWorksFiveMinutes = true;
            }
            else if (FireworkAnnouncementTime.HasValue
                     && (DateTime.UtcNow.Subtract(FireworkAnnouncementTime.Value).TotalMinutes >= 4)
                     && !AnnouncedFireworksOneMinute)
            {
                AnnounceFireworkOneMinuteStart();
                AnnouncedFireworksOneMinute = true;
            }
            else if (AnnouncedFireworksOneMinute && !GMThankYouComplete)
            {
                ConductGMThankYou(delta);
            }
            else if (FireworkAnnouncementTime.HasValue
                     && (DateTime.UtcNow.Subtract(FireworkAnnouncementTime.Value).TotalMinutes >= 5)
                     && !AnnouncedFireworksBegin
                     && GMThankYouComplete)
            {
                AnnounceFireworkBeginning();
                AnnouncedFireworksBegin = true;
                FireworkAnnouncementTime = null; // Resetting the timer
            }
            else if (AnnouncedFireworksBegin && !FireworksCompleted)
                ConductFireworks(delta);
            else if (FireworksCompleted)
            {
                PostFireworkReward();
            }
            
        }
    }
}
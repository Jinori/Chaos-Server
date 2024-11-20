using Chaos.Common.Definitions;
using Chaos.DarkAges.Definitions;
using Chaos.Models.World;
using Chaos.Networking.Abstractions;
using Chaos.NLog.Logging.Definitions;
using Chaos.NLog.Logging.Extensions;
using Chaos.Scripting.MerchantScripts.Abstractions;
using Humanizer;

namespace Chaos.Scripting.MerchantScripts.Casino;

public class TwentyOneScript(Merchant subject, IClientRegistry<IChaosWorldClient> clientRegistry, ILogger<TwentyOneScript> logger)
    : MerchantScriptBase(subject)
{
    private readonly List<Aisling> AislingsThatDidNotBust = [];

    private IEnumerable<Aisling>? AislingsAtCompletion;
    private IEnumerable<Aisling>? AislingsAtStart;

    private bool AnnouncedOneMinuteTimer;
    private DateTime? AnnouncedStart;
    private bool AnnouncedThirtySecondTimer;
    private bool CountdownStarted;
    private int CountdownStep;
    private DateTime LastCountdownTime;

    public override void Update(TimeSpan delta)
    {
        if (Subject.CurrentlyHosting21Game)
        {
            StartGameCountdown();

            if (GameShouldComplete())
            {
                ProcessGameCompletion();
                ResetGame();
            }

            HandleThirtySecondAnnouncement();

            HandleFinalCountdown();
        }
    }

    private void StartGameCountdown()
    {
        if (!AnnouncedOneMinuteTimer)
        {
            AislingsAtStart = Subject.MapInstance.GetEntities<Aisling>().ToList();
            AnnouncedStart = DateTime.UtcNow;
            AnnouncedOneMinuteTimer = true;

            foreach (var aisling in AislingsAtStart)
                aisling.SendActiveMessage("A game of twenty one has started! Come join!");

            Subject.Say("Let's begin!");
        }
    }

    private bool GameShouldComplete() => AnnouncedStart.HasValue && (DateTime.UtcNow.Subtract(AnnouncedStart.Value).TotalMinutes >= 1) && AnnouncedOneMinuteTimer;

    private void HandleThirtySecondAnnouncement()
    {
        if (AnnouncedStart.HasValue && (DateTime.UtcNow.Subtract(AnnouncedStart.Value).TotalSeconds >= 30) && !AnnouncedThirtySecondTimer)
        {
            Subject.Say("Game starting in thirty seconds!");
            AnnouncedThirtySecondTimer = true;
        }
    }

    private void HandleFinalCountdown()
    {
        if (AnnouncedStart.HasValue && (DateTime.UtcNow.Subtract(AnnouncedStart.Value).TotalSeconds >= 50) && !CountdownStarted)
        {
            Subject.Say("Game starting in ten seconds!");
            CountdownStarted = true;
            CountdownStep = 9;
            LastCountdownTime = DateTime.UtcNow;
        }
        else if (CountdownStarted && (CountdownStep >= 0))
            if (DateTime.UtcNow.Subtract(LastCountdownTime).TotalSeconds >= 1)
            {
                Subject.Say($"{CountdownStep.ToWords().Titleize()}...");
                LastCountdownTime = DateTime.UtcNow;
                CountdownStep--;
            }
    }

    private void ProcessGameCompletion()
    {
        GatherAislingsAtCompletion();
        FindAislingsThatDidNotBust();

        if (AislingsThatDidNotBust.Any())
            ProcessWinners();
        else
            Subject.Say("Everyone bust! There are no winners.");
    }

    private void GatherAislingsAtCompletion() => AislingsAtCompletion = Subject.MapInstance.GetEntities<Aisling>().Where(x => x.BetGoldOnTwentyOne).ToList();

    private void FindAislingsThatDidNotBust()
    {
        if (AislingsAtCompletion != null)
            foreach (var aisling in AislingsAtCompletion)
                if (aisling is { CurrentDiceScore: <= 21 })
                    AislingsThatDidNotBust.Add(aisling);
    }

    private void ProcessWinners()
    {
        var highestScore = AislingsThatDidNotBust.Max(a => a.CurrentDiceScore);
        var winners = AislingsThatDidNotBust.Where(a => a.CurrentDiceScore == highestScore).ToArray();

        if (winners.Length == 1)
            ProcessSingleWinner(winners[0], highestScore);
        else
            ProcessMultipleWinners(winners, highestScore);
    }

    private void ProcessSingleWinner(Aisling winner, int highestScore)
    {
        Subject.Say($"{winner.Name} wins with a score of {highestScore}!");
        DistributeWinnings(new[] { winner });
    }

    private void ProcessMultipleWinners(IReadOnlyCollection<Aisling> winners, int highestScore)
    {
        var winnerNames = string.Join(", ", winners.Select(w => w.Name));
        Subject.Say($"It's a tie between {winnerNames} with a score of {highestScore}!");
        DistributeWinnings(winners);
    }

    private void DistributeWinnings(IReadOnlyCollection<Aisling> winners)
    {
        foreach (var winner in winners)
        {
            if (AislingsAtCompletion != null)
            {
                var winnings = (AislingsAtCompletion.Count() / winners.Count) * 25000;
                var eightPercent = (int)(winnings * 0.08m);
                var winningsMinusEight = winnings - eightPercent;

                logger.WithTopics([Topics.Entities.Aisling, Topics.Entities.Gold])
                      .WithProperty(winner)
                      .WithProperty(Subject)
                      .LogInformation(
                          "{@AislingName} has received {@GoldAmount} gold from a casino win, Casino took {@CasinoAmount} in taxes",
                          winner.Name,
                          winningsMinusEight,
                          eightPercent);

                winner.SendServerMessage(ServerMessageType.Whisper, $"The casino took their cut of {eightPercent.ToWords()} gold!");

                winner.SendServerMessage(
                    ServerMessageType.Whisper,
                    $"You {(winners.Count > 1 ? "tied and receive" : "won the game and receive")} {winningsMinusEight.ToWords()} gold!");

                winner.TryGiveGold(winningsMinusEight);
            }
        }
    }
        
    private void ResetGame()
    {
        Subject.CurrentlyHosting21Game = false;
        AislingsThatDidNotBust.Clear();
        AnnouncedOneMinuteTimer = false;
        AnnouncedStart = null;
        CountdownStarted = false;
            
        ClearPlayersState();

        AislingsAtStart = null;
        AislingsAtCompletion = null;
    }

    private void ClearPlayersState()
    {
        var clearPlayers = clientRegistry.Where(
            x => (AislingsAtCompletion != null)
                 && (AislingsAtStart != null)
                 && (AislingsAtStart.Contains(x.Aisling) || AislingsAtCompletion.Contains(x.Aisling)));

        foreach (var player in clearPlayers)
        {
            player.Aisling.CurrentDiceScore = 0;
            player.Aisling.TwentyOneBust = false;
            player.Aisling.BetGoldOnTwentyOne = false;
            player.Aisling.TwentyOneStayOption = false;
            player.Aisling.OnTwentyOneTile = false;
        }
    }

}
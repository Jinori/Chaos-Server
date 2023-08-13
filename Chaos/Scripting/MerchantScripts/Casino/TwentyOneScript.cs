using Chaos.Common.Definitions;
using Chaos.Models.World;
using Chaos.Networking.Abstractions;
using Chaos.NLog.Logging.Definitions;
using Chaos.NLog.Logging.Extensions;
using Chaos.Scripting.MerchantScripts.Abstractions;
using Humanizer;
using Microsoft.Extensions.Logging;

namespace Chaos.Scripting.MerchantScripts.Casino;

public class TwentyOneScript : MerchantScriptBase
{
    private readonly List<Aisling> AislingsThatDidNotBust = new();
    private readonly IClientRegistry<IWorldClient> ClientRegistry;
    private readonly ILogger<TwentyOneScript> Logger;
    private IEnumerable<Aisling>? AislingsAtCompletion;
    private IEnumerable<Aisling>? AislingsAtStart;
    private bool AnnouncedOneMinuteTimer;
    private DateTime? AnnouncedStart;
    private bool AnnouncedThirtySecondTimer;
    private bool CountdownStarted;
    private int CountdownStep;
    private DateTime LastCountdownTime;

    public TwentyOneScript(Merchant subject, IClientRegistry<IWorldClient> clientRegistry, ILogger<TwentyOneScript> logger)
        : base(subject)
    {
        ClientRegistry = clientRegistry;
        Logger = logger;
    }

    private void ProcessGameCompletion()
    {
        AislingsAtCompletion = Subject.MapInstance.GetEntities<Aisling>().Where(x => x.BetGoldOnTwentyOne).ToList();

        foreach (var aisling in AislingsAtCompletion)
            if (aisling is { CurrentDiceScore: <= 21 })
                AislingsThatDidNotBust.Add(aisling);

        if (AislingsThatDidNotBust.Any())
        {
            var highestScore = AislingsThatDidNotBust.Max(a => a.CurrentDiceScore);
            var winners = AislingsThatDidNotBust.Where(a => a.CurrentDiceScore == highestScore);

            var enumerable = winners as Aisling[] ?? winners.ToArray();

            if (enumerable.Length == 1)
            {
                var winner = enumerable.First();
                Subject.Say($"{winner.Name} wins with a score of {highestScore}!");
                var winnings = AislingsAtCompletion.Count() * 25000;
                var eightPercent = (int)(winnings * 0.08m);
                var winningsMinusEight = winnings - eightPercent;

                Logger.WithTopics(Topics.Entities.Aisling, Topics.Entities.Gold)
                      .WithProperty(winner)
                      .WithProperty(Subject)
                      .LogInformation(
                          "{@AislingName} has received {@GoldAmount} gold from a casino win, Casino took {@CasinoAmount} in taxes",
                          winner.Name,
                          winningsMinusEight,
                          eightPercent);

                winner.TryGiveGold(winningsMinusEight);
                winner.SendServerMessage(ServerMessageType.Whisper, $"The casino took their cut of {eightPercent.ToWords()} gold!");
                winner.SendServerMessage(ServerMessageType.Whisper, $"You won the game and receive {winningsMinusEight.ToWords()} gold!");
            }
            else
            {
                var winnerNames = string.Join(", ", enumerable.Select(w => w.Name));
                Subject.Say($"It's a tie between {winnerNames} with a score of {highestScore}!");

                foreach (var winner in enumerable)
                {
                    var winnings = AislingsAtCompletion.Count() / enumerable.Length * 25000;
                    var eightPercent = (int)(winnings * 0.08m);
                    var winningsMinusEight = winnings - eightPercent;

                    Logger.WithTopics(Topics.Entities.Aisling, Topics.Entities.Gold)
                          .WithProperty(winner)
                          .WithProperty(Subject)
                          .LogInformation(
                              "{@AislingName} has received {@GoldAmount} gold from a casino win, Casino took {@CasinoAmount} in taxes",
                              winner.Name,
                              winningsMinusEight,
                              eightPercent);

                    winner.SendServerMessage(ServerMessageType.Whisper, $"The casino took their cut of {eightPercent.ToWords()} gold!");
                    winner.SendServerMessage(ServerMessageType.Whisper, $"You tied and receive {winningsMinusEight.ToWords()} gold!");
                    winner.TryGiveGold(winningsMinusEight);
                }
            }
        }
        else
            Subject.Say("Everyone bust! There are no winners.");
    }

    private void ResetGame()
    {
        Subject.CurrentlyHosting21Game = false;
        AislingsThatDidNotBust.Clear();
        AnnouncedOneMinuteTimer = false;
        AnnouncedStart = null;
        CountdownStarted = false;

        var clearPlayers = ClientRegistry.Where(
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

        AislingsAtStart = null;
        AislingsAtCompletion = null;
    }

    public override void Update(TimeSpan delta)
    {
        if (Subject.CurrentlyHosting21Game)
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
            else if (AnnouncedStart.HasValue
                     && (DateTime.UtcNow.Subtract(AnnouncedStart.Value).TotalMinutes >= 1)
                     && AnnouncedOneMinuteTimer)
            {
                ProcessGameCompletion();
                ResetGame();
            }
            else if (AnnouncedStart.HasValue
                     && (DateTime.UtcNow.Subtract(AnnouncedStart.Value).TotalSeconds >= 30)
                     && !AnnouncedThirtySecondTimer)
            {
                Subject.Say("Game starting in thirty seconds!");
                AnnouncedThirtySecondTimer = true;
            }
            else if (AnnouncedStart.HasValue && (DateTime.UtcNow.Subtract(AnnouncedStart.Value).TotalSeconds >= 50) && !CountdownStarted)
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
    }
}
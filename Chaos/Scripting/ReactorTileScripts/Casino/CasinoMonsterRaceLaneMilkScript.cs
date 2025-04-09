using Chaos.Extensions;
using Chaos.Extensions.Geometry;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.NLog.Logging.Definitions;
using Chaos.NLog.Logging.Extensions;
using Chaos.Scripting.ReactorTileScripts.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;
using Humanizer;

namespace Chaos.Scripting.ReactorTileScripts.Casino;

public class CasinoMonsterRaceLaneMilkScript : ReactorTileScriptBase
{
    private readonly IIntervalTimer AfterWinnerDelay;
    private readonly List<Aisling> AislingsThatWon = new();
    private readonly ILogger<CasinoMonsterRaceLaneMilkScript> Logger;
    private IEnumerable<Aisling>? AislingsAtCompletion;
    private bool MonsterHasWon;
    public bool GameOver { get; set; }

    protected Animation Winner { get; } = new()
    {
        AnimationSpeed = 180,
        TargetAnimation = 123
    };

    /// <inheritdoc />
    public CasinoMonsterRaceLaneMilkScript(ReactorTile subject, ILogger<CasinoMonsterRaceLaneMilkScript> logger)
        : base(subject)
    {
        Logger = logger;
        AfterWinnerDelay = new IntervalTimer(TimeSpan.FromSeconds(6), false);
    }

    /// <inheritdoc />
    public override void OnWalkedOn(Creature source)
    {
        if (source is not Monster)
            return;

        foreach (var tile in Subject.MapInstance.GetEntities<ReactorTile>())
        {
            if (tile.Script.Is<CasinoMonsterRaceLaneGrassScript>(out var grass))
                grass.GameOver = true;

            if (tile.Script.Is<CasinoMonsterRaceLaneLavaScript>(out var lava))
                lava.GameOver = true;

            if (tile.Script.Is<CasinoMonsterRaceLaneMilkScript>(out var milk))
                milk.GameOver = true;

            if (tile.Script.Is<CasinoMonsterRaceLaneSandScript>(out var sand))
                sand.GameOver = true;

            if (tile.Script.Is<CasinoMonsterRaceLaneSkyScript>(out var sky))
                sky.GameOver = true;

            if (tile.Script.Is<CasinoMonsterRaceLaneWaterScript>(out var water))
                water.GameOver = true;
        }

        var monsters = source.MapInstance
                             .GetEntities<Monster>()
                             .Where(x => x.Template.TemplateKey == "amusementMonster");

        foreach (var monster in monsters)
            monster.MapInstance.RemoveEntity(monster);

        MonsterHasWon = true;

        AislingsAtCompletion = Subject.MapInstance
                                      .GetEntities<Aisling>()
                                      .Where(x => x.BetOnMonsterRaceOption)
                                      .ToList();

        foreach (var aisling in AislingsAtCompletion)
            if (aisling.MonsterRacingLane is "Milk")
                AislingsThatWon.Add(aisling);

        switch (AislingsThatWon.Count)
        {
            case 1:
            {
                var winner = AislingsThatWon.First();

                foreach (var aisling in AislingsAtCompletion)
                    aisling.SendActiveMessage($"{winner.Name} wins on lane Grass!");

                var winnings = AislingsAtCompletion.Count() * 25000;

                Logger.WithTopics(Topics.Entities.Aisling, Topics.Entities.Gold)
                      .WithProperty(winner)
                      .WithProperty(Subject)
                      .LogInformation("{@AislingName} has received {@GoldAmount} gold from a casino win", winner.Name, winnings);

                winner.TryGiveGold(winnings);
                winner.SendActiveMessage($"You won the game and receive {winnings.ToWords()} gold!");

                break;
            }
            case > 1:
            {
                var winnerNames = string.Join(", ", AislingsThatWon.Select(w => w.Name));

                foreach (var aisling in AislingsAtCompletion)
                    aisling.SendActiveMessage($"It's a tie between {winnerNames}!");

                foreach (var winner in AislingsThatWon)
                {
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
                    winner.SendActiveMessage($"You tied and receive {winnings.ToWords()} gold!");
                }

                break;
            }
            default:
            {
                foreach (var aisling in AislingsAtCompletion)
                    aisling.SendActiveMessage("Lane Milk won! Nobody picked Lane Milk!");

                break;
            }
        }

        source.MapInstance.ShowAnimation(Winner.GetPointAnimation(Subject));
        source.MapInstance.PlaySound(165, Subject);
    }

    /// <inheritdoc />
    public override void Update(TimeSpan delta)
    {
        if (MonsterHasWon)
            AfterWinnerDelay.Update(delta);

        if (AfterWinnerDelay.IntervalElapsed && MonsterHasWon)
        {
            if (AislingsAtCompletion != null)
                foreach (var aislings in AislingsAtCompletion)
                {
                    var rect = new Rectangle(new Point(11, 16), 7, 3);
                    aislings.MonsterRacingLane = "";
                    aislings.BetOnMonsterRaceOption = false;
                    aislings.WarpTo(rect.GetRandomPoint());
                }

            MonsterHasWon = false;
        }
    }
}
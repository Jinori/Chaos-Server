using Chaos.Extensions.Geometry;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.ReactorTileScripts.Abstractions;
using Humanizer;

namespace Chaos.Scripting.ReactorTileScripts.Casino;

public class CasinoMonsterRaceLaneTwoScript : ReactorTileScriptBase
{
    private readonly List<Aisling> AislingsThatWon = new();
    private IEnumerable<Aisling>? AislingsAtCompletion;
    protected Animation Winner { get; } = new()
    {
        AnimationSpeed = 180,
        TargetAnimation = 123
    };

    /// <inheritdoc />
    public CasinoMonsterRaceLaneTwoScript(ReactorTile subject)
        : base(subject) { }

    /// <inheritdoc />
    public override void OnWalkedOn(Creature source)
    {
        if (source is not Monster)
            return;

        if (Subject.DistanceFrom(source) <= 0)
        {
            source.MapInstance.ShowAnimation(Winner.GetPointAnimation(Subject));
            source.MapInstance.PlaySound(165, Subject);

            AislingsAtCompletion = Subject.MapInstance.GetEntities<Aisling>().Where(x => x.BetOnMonsterRaceOption).ToList();

            foreach (var aisling in AislingsAtCompletion)
                if (aisling.MonsterRacingLane is 2)
                    AislingsThatWon.Add(aisling);

            if (AislingsThatWon.Count == 1)
            {
                var winner = AislingsThatWon.First();

                foreach (var aisling in AislingsAtCompletion)
                    aisling.SendActiveMessage($"{winner.Name} wins on lane two!");

                var winnings = AislingsAtCompletion.Count() * 25000;
                winner.TryGiveGold(winnings);
                winner.SendActiveMessage($"You won the game and receive {winnings.ToWords()} gold!");
            }
            else if (AislingsThatWon.Count > 1)
            {
                var winnerNames = string.Join(", ", AislingsThatWon.Select(w => w.Name));

                foreach (var aisling in AislingsAtCompletion)
                    aisling.SendActiveMessage($"It's a tie between {winnerNames}!");

                foreach (var winner in AislingsThatWon)
                {
                    var winnings = AislingsAtCompletion.Count() / AislingsThatWon.Count * 25000;
                    winner.SendActiveMessage($"You tied and receive {winnings.ToWords()} gold!");
                    winner.TryGiveGold(winnings);
                }
            }
            else
                foreach (var aisling in AislingsAtCompletion)
                    aisling.SendActiveMessage("Nobody guessed the right lane!");

            var monsters = source.MapInstance.GetEntities<Monster>().Where(x => x.Template.TemplateKey == "amusementMonster");

            foreach (var monster in monsters)
                monster.MapInstance.RemoveObject(monster);

            foreach (var aislings in AislingsAtCompletion)
            {
                aislings.MonsterRacingLane = 0;
                aislings.OnMonsterRacingTile = false;
                aislings.BetOnMonsterRaceOption = false;
            }
        }
    }
}
using Chaos.Common.Utilities;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;

namespace Chaos.Scripting.DialogScripts;

public class TwentyOneScript : DialogScriptBase
{
    /// <inheritdoc />
    public TwentyOneScript(Dialog subject)
        : base(subject) { }

    public override void OnDisplaying(Aisling source)
    {
        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "ladyluck_initial":
            {
                OnDisplayingInitial(source);

                break;
            }
            case "ladyluck_rolldice":
            {
                OnDisplayingDiceRoll(source);

                break;
            }
            case "ladyluck_stay":
            {
                OnDisplayingStay(source);

                break;
            }
        }
    }

    private void OnDisplayingDiceRoll(Aisling source)
    {
        if (source.TryTakeGold(25000))
        {
            source.BetGoldOnTwentyOne = true;

            if (Subject.DialogSource is Merchant merchant)
            {
                merchant.CurrentlyHosting21Game = true;
                var roll = IntegerRandomizer.RollDouble(6);
                source.CurrentDiceScore += roll;

                if (source.CurrentDiceScore <= 21)
                    Subject.InjectTextParameters(source.CurrentDiceScore);
                else
                {
                    source.TwentyOneBust = true;
                    merchant.Say($"{source.Name} has bust!");
                    Subject.Reply(source, $"You've bust! Your total is {source.CurrentDiceScore}. Please wait while others finish.");
                }
            }
        }
    }

    private void OnDisplayingInitial(Aisling source)
    {
        if (source.Gold < 25000)
        {
            Subject.Reply(source, "Looks like your luck has ran out, sweetie. Come back with more gold.");

            return;
        }

        if (source.TwentyOneStayOption || source.TwentyOneBust)
            Subject.Reply(source, $"Please wait while everyone has finished. Your score was {source.CurrentDiceScore}.");
    }

    private void OnDisplayingStay(Aisling source)
    {
        source.TwentyOneStayOption = true;
        var merchant = Subject.DialogSource as Merchant;
        merchant?.Say($"{source.Name} has locked in!");
        Subject.InjectTextParameters(source.CurrentDiceScore);
    }
}
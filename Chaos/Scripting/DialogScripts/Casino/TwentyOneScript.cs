using Chaos.Common.Definitions;
using Chaos.Common.Utilities;
using Chaos.DarkAges.Definitions;
using Chaos.Extensions.Geometry;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;

namespace Chaos.Scripting.DialogScripts.Casino;

public class TwentyOneScript : DialogScriptBase
{
    private bool HasPaid;

    /// <inheritdoc />
    public TwentyOneScript(Dialog subject)
        : base(subject) { }

    public override void OnDisplaying(Aisling source)
    {
        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "twentyonetable_initial":
            {
                OnDisplayingInitial(source);

                break;
            }
            case "twentyonetable_rolldice":
            {
                OnDisplayingDiceRoll(source);

                break;
            }
            case "twentyonetable_stay":
            {
                OnDisplayingStay(source);

                break;
            }
            case "twentyonetable_leavetable":
            {
                OnDisplayingLeaveTable(source);

                break;
            }
        }
    }

    private void OnDisplayingDiceRoll(Aisling source)
    {
        if (Subject.DialogSource is Merchant merchant)
        {
            if ((source.Gold >= 25000) && !HasPaid && (source.CurrentDiceScore == 0))
            {
                source.TryTakeGold(25000);
                source.BetGoldOnTwentyOne = true;
                HasPaid = true;
            }
            
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

    private void OnDisplayingInitial(Aisling source)
    {
        if (!source.OnTwentyOneTile)
        {
            Subject.Reply(source, "Please step up if you wish to play.");

            return;
        }

        if ((source.Gold < 25000) && !HasPaid)
        {
            source.OnTwentyOneTile = false;
            Subject.Reply(source, "Looks like your luck has ran out. Come back with more gold.");
            var point = source.DirectionalOffset(source.Direction.Reverse());
            source.WarpTo(point);

            return;
        }
        
        if (source.TwentyOneStayOption || source.TwentyOneBust)
            Subject.Reply(source, $"Please wait while everyone has finished. Your score was {source.CurrentDiceScore}.");
    }

    private void OnDisplayingLeaveTable(Aisling source)
    {
        var point = source.DirectionalOffset(source.Direction.Reverse());
        source.WarpTo(point);
        source.OnTwentyOneTile = false;

        switch (source.Gender)
        {
            case Gender.Female:
                Subject.InjectTextParameters("sugar");

                break;
            case Gender.Male:
                Subject.InjectTextParameters("cowboy");

                break;
        }
    }

    private void OnDisplayingStay(Aisling source)
    {
        source.TwentyOneStayOption = true;
        var merchant = Subject.DialogSource as Merchant;
        merchant?.Say($"{source.Name} has locked in!");
        Subject.InjectTextParameters(source.CurrentDiceScore);
    }
}
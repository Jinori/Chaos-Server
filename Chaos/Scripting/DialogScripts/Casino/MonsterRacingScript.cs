using Chaos.Common.Definitions;
using Chaos.DarkAges.Definitions;
using Chaos.Extensions;
using Chaos.Extensions.Geometry;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;

namespace Chaos.Scripting.DialogScripts.Casino;

public class MonsterRacingScript : DialogScriptBase
{
    private bool HasPaid;

    /// <inheritdoc />
    public MonsterRacingScript(Dialog subject)
        : base(subject)
    {
    }

    public override void OnDisplaying(Aisling source)
    {
        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "ladychance_initial":
            {
                OnDisplayingInitial(source);

                break;
            }
            case "ladychance_setlane":
            {
                OnDisplayingSetLane(source);

                break;
            }
            case "ladychance_leavetable":
            {
                OnDisplayingLeaveTable(source);

                break;
            }
        }
    }

    private void OnDisplayingInitial(Aisling source)
    {
        if ((source.Gold < 25000) && !HasPaid)
        {
            Subject.Reply(source, "Looks like your luck has ran out, sweetie. Come back with more gold.");
            var rect = new Rectangle(new Point(11, 16), 7, 3);
            source.WarpTo(rect.GetRandomPoint());

            return;
        }

        if (source.BetOnMonsterRaceOption)
            Subject.Reply(
                source,
                $"Please wait while everyone has finished. You chose lane {source.MonsterRacingLane}.",
                "ladychance_leaveGame");
    }

    private void OnDisplayingLeaveTable(Aisling source)
    {
        var rect = new Rectangle(new Point(11, 16), 7, 3);
        source.WarpTo(rect.GetRandomPoint());
        source.MonsterRacingLane = "";
        source.BetOnMonsterRaceOption = false;

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

    private void OnDisplayingSetLane(Aisling source)
    {
        source.BetOnMonsterRaceOption = true;

        if (Subject.DialogSource is Merchant merchant)
        {
            var script = merchant.Script.As<MerchantScripts.Casino.MonsterRacingScript>();

            if (script == null)
                merchant.AddScript<MerchantScripts.Casino.MonsterRacingScript>();

            merchant.Say($"{source.Name} has bet on lane {source.MonsterRacingLane}!");
            Subject.InjectTextParameters(source.MonsterRacingLane);
        }
    }

    /// <inheritdoc />
    public override void OnNext(Aisling source, byte? optionIndex = null)
    {
        if (Subject.Template.TemplateKey.ToLower() == "ladychance_enterrace")
        {
            source.MonsterRacingLane = optionIndex switch
            {
                1 => "Milk",
                2 => "Sky",
                3 => "Sand",
                4 => "Water",
                5 => "Grass",
                6 => "Lava",
                _ => source.MonsterRacingLane
            };

            if (!HasPaid)
            {
                source.TryTakeGold(25000);
                source.SendActiveMessage("Your 25,000 gold is on the books! Good luck!");
                source.BetOnMonsterRaceOption = true;
                HasPaid = true;
            }

            var rect = new Rectangle(new Point(11, 10), 11, 3);
            source.WarpTo(rect.GetRandomPoint());
        }
    }
}
using Chaos.Common.Definitions;
using Chaos.Extensions;
using Chaos.Extensions.Geometry;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.Abstractions;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Scripting.MerchantScripts.Abstractions;
using Humanizer;

namespace Chaos.Scripting.DialogScripts.Casino;

public class MonsterRacingScript : DialogScriptBase
{
    private readonly IScriptFactory<IMerchantScript, Merchant> ScriptFactory;

    private bool HasPaid;

    /// <inheritdoc />
    public MonsterRacingScript(Dialog subject, IScriptFactory<IMerchantScript, Merchant> scriptFactory)
        : base(subject) =>
        ScriptFactory = scriptFactory;

    public override void OnDisplaying(Aisling source)
    {
        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "ladychance_initial":
            {
                OnDisplayingInitial(source);

                break;
            }
            case "ladychance_enterrace":
            {
                OnDisplayingEnterRace(source);

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

    private void OnDisplayingEnterRace(Aisling source)
    {
        if (!HasPaid)
        {
            source.TryTakeGold(25000);
            source.BetOnMonsterRaceOption = true;
            HasPaid = true;
        }
    }

    private void OnDisplayingInitial(Aisling source)
    {
        if (!source.OnMonsterRacingTile)
        {
            Subject.Reply(source, "Please step up to the line if you wish to play, dear.");

            return;
        }

        if ((source.Gold < 25000) && !HasPaid)
        {
            source.OnMonsterRacingTile = false;
            Subject.Reply(source, "Looks like your luck has ran out, sweetie. Come back with more gold.");
            var point = source.DirectionalOffset(source.Direction.Reverse());
            source.WarpTo(point);

            return;
        }

        if (source.BetOnMonsterRaceOption)
            Subject.Reply(source, $"Please wait while everyone has finished. You chose lane {source.MonsterRacingLane.ToWords()}.");
    }

    private void OnDisplayingLeaveTable(Aisling source)
    {
        var point = source.DirectionalOffset(source.Direction.Reverse());
        source.WarpTo(point);
        source.OnMonsterRacingTile = false;

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
                merchant.AddScript(typeof(MerchantScripts.Casino.MonsterRacingScript), ScriptFactory);

            merchant.Say($"{source.Name} has bet on lane {source.MonsterRacingLane.ToWords()}!");
            Subject.InjectTextParameters(source.MonsterRacingLane.ToWords());
        }
    }

    /// <inheritdoc />
    public override void OnNext(Aisling source, byte? optionIndex = null)
    {
        if (Subject.Template.TemplateKey.ToLower() == "ladychance_enterrace")
        {
            if (optionIndex is 1)
                source.MonsterRacingLane = 1;

            if (optionIndex is 2)
                source.MonsterRacingLane = 2;

            if (optionIndex is 3)
                source.MonsterRacingLane = 3;

            if (optionIndex is 4)
                source.MonsterRacingLane = 4;
        }
    }
}
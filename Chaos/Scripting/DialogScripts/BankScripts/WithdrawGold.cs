﻿#region
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.NLog.Logging.Definitions;
using Chaos.NLog.Logging.Extensions;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Utilities;
#endregion

namespace Chaos.Scripting.DialogScripts.BankScripts;

public class WithdrawGoldScript : DialogScriptBase
{
    private readonly ILogger<WithdrawGoldScript> Logger;

    /// <inheritdoc />
    public WithdrawGoldScript(Dialog subject, ILogger<WithdrawGoldScript> logger)
        : base(subject)
        => Logger = logger;

    /// <inheritdoc />
    public override void OnDisplaying(Aisling source) => Subject.InjectTextParameters(source.Bank.Gold);

    /// <inheritdoc />
    public override void OnNext(Aisling source, byte? optionIndex = null)
    {
        if (!TryFetchArgs<int>(out var amount) || (amount <= 0))
        {
            Subject.ReplyToUnknownInput(source);

            return;
        }

        var withdrawResult = ComplexActionHelper.WithdrawGold(source, amount);

        switch (withdrawResult)
        {
            case ComplexActionHelper.WithdrawGoldResult.Success:
                Logger.WithTopics(Topics.Entities.Aisling, Topics.Entities.Gold, Topics.Actions.Withdraw)
                      .WithProperty(Subject)
                      .WithProperty(Subject.DialogSource)
                      .WithProperty(source)
                      .LogInformation("Aisling {@AislingName} withdrew {Amount} gold from the bank", source, amount);

                break;
            case ComplexActionHelper.WithdrawGoldResult.TooMuchGold:
                Subject.Reply(source, "You are carrying too much gold.", "generic_withdrawgold_initial");

                break;
            case ComplexActionHelper.WithdrawGoldResult.DontHaveThatMany:
                Subject.Reply(source, "You don't have that much.", "generic_withdrawgold_initial");

                break;
            case ComplexActionHelper.WithdrawGoldResult.BadInput:
                Subject.ReplyToUnknownInput(source);

                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
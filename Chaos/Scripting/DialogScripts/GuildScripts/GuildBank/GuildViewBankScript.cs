#region
using Chaos.Models.Data;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Scripting.DialogScripts.BankScripts;
#endregion

namespace Chaos.Scripting.DialogScripts.GuildScripts.GuildBank;

public class GuildViewBankScript : DialogScriptBase
{
    /// <inheritdoc />
    public GuildViewBankScript(Dialog subject)
        : base(subject) {}

    /// <inheritdoc />
    public override void OnDisplaying(Aisling source)
    {
        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "generic_viewbank_initial":
            {
                OnDisplayingInitial(source);

                break;
            }
        }
    }

    private void OnDisplayingInitial(Aisling source)
    {
        Subject.Items.AddRange(
                    source.Guild!.Bank
                          .Select(ItemDetails.WithdrawItem)
                          .OrderBy(x => x.Item.Template.Category));
        
        var goldAmount = source.Guild.Bank.Gold;
        Subject.InjectTextParameters(goldAmount);
    }
}
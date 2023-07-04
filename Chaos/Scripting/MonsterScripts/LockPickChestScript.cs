using Chaos.Models.World;
using Chaos.Scripting.MonsterScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.MonsterScripts;

public class LockPickChestScript : MonsterScriptBase
{
    private IMerchantFactory MerchantFactory { get; set; }
    private IDialogFactory DialogFactory { get; }

    public LockPickChestScript(Monster subject, IDialogFactory dialogFactory, IMerchantFactory merchantFactory)
        : base(subject)
    {
        DialogFactory = dialogFactory;
        MerchantFactory = merchantFactory;
    }

    public override void OnClicked(Aisling source)
    {
        var dialogNew = DialogFactory.Create("generic_lockPickChest", Subject);
        dialogNew.Display(source);
    }
}
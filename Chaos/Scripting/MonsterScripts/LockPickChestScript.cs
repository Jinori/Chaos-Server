using Chaos.Objects.World;
using Chaos.Scripting.MonsterScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.MonsterScripts;

public class LockPickChestScript : MonsterScriptBase
{
    private IDialogFactory DialogFactory { get; }
    private IMerchantFactory MerchantFactory { get; }

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
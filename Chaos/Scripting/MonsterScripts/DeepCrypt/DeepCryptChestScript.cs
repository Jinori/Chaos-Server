using Chaos.Models.World;
using Chaos.Scripting.MonsterScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.MonsterScripts.DeepCrypt;

public class DeepCryptChestScript : MonsterScriptBase
{
    private IMerchantFactory MerchantFactory { get; set; }
    private IDialogFactory DialogFactory { get; }

    public DeepCryptChestScript(Monster subject, IDialogFactory dialogFactory, IMerchantFactory merchantFactory)
        : base(subject)
    {
        DialogFactory = dialogFactory;
        MerchantFactory = merchantFactory;
    }

    public override void OnClicked(Aisling source)
    {
        var dialogNew = DialogFactory.Create("deepCrypt_lockedChest", Subject);
        dialogNew.Display(source);
    }
}
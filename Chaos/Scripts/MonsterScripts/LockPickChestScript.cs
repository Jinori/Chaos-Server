﻿using Chaos.Objects.World;
using Chaos.Scripts.MonsterScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripts.MonsterScripts;

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
        var npcpoint = new Point(source.X, source.Y);
        var merchant = MerchantFactory.Create("lockPickChest", source.MapInstance, npcpoint);
        var dialogNew = DialogFactory.Create("generic_lockPickChest", merchant);
        dialogNew.Display(source);
    }
}
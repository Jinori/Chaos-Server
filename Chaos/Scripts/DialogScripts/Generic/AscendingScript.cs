using Chaos.Common.Definitions;
using Chaos.Data;
using Chaos.Objects.Menu;
using Chaos.Objects.World;
using Chaos.Scripts.DialogScripts.Abstractions;

namespace Chaos.Scripts.DialogScripts.Generic;

public class AscendingScript : DialogScriptBase
{
    public AscendingScript(Dialog subject)
        : base(subject) { }

    public override void OnDisplaying(Aisling source)
    {
        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "generic_buyhealthonce":
            {
                var formula = source.StatSheet.MaximumHp * 500;

                if (source.UserStatSheet.TotalExp >= formula)
                {
                    var hp = new Attributes
                    {
                        MaximumHp = 50
                    };

                    source.StatSheet.Add(hp);
                    source.Client.SendAttributes(StatUpdateType.Vitality);
                    source.SendOrangeBarMessage("Health raised by fifty points!");
                }
            }

                break;

            case "generic_buymanaonce":
            {
                var formula = source.StatSheet.MaximumMp * 500;

                if (source.UserStatSheet.TotalExp >= formula)
                {
                    var mp = new Attributes
                    {
                        MaximumMp = 50
                    };

                    source.StatSheet.Add(mp);
                    source.Client.SendAttributes(StatUpdateType.Vitality);
                    source.SendOrangeBarMessage("Mana raised by fifty points!");
                }
            }
        }
    }
}
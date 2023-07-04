using Chaos.Extensions.Geometry;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Religion.Abstractions;
using Chaos.Scripting.MerchantScripts.Abstractions;

namespace Chaos.Scripting.MerchantScripts;

public class ShrineScript : MerchantScriptBase
{
    /// <inheritdoc />
    public ShrineScript(Merchant subject)
        : base(subject) { }

    public override void OnClicked(Aisling source)
    {
        if (source.DistanceFrom(Subject) <= 1)
        {
            if (ReligionScriptBase.CheckDeity(source) != null)
            {
                switch (ReligionScriptBase.CheckDeity(source))
                {
                    case "Miraelis":
                    {
                        source.SendActiveMessage("You pray at the shrine to Miraelis. You received faith.");

                        break;
                    }
                    case "Serendael":
                    {
                        source.SendActiveMessage("You pray at the shrine to Serendael. You received faith.");

                        break;
                    }
                    case "Theselene":
                    {
                        source.SendActiveMessage("You pray at the shrine to Theselene. You received faith.");

                        break;
                    }
                    case "Skandara":
                    {
                        source.SendActiveMessage("You pray at the shrine to Skandara. You received faith.");

                        break;
                    }
                }

                ReligionScriptBase.TryAddFaith(source, 5);
                Subject.MapInstance.RemoveObject(Subject);
            } else
                source.SendActiveMessage("You attempt to pray at the shrine but no god will listen..");
        } else
            source.SendActiveMessage("The shrine seems to beckon you closer...");
    }
}
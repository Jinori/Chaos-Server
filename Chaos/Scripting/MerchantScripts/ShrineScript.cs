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
        if (source.ManhattanDistanceFrom(Subject) <= 1)
        {
            var worshipedDeity = ReligionScriptBase.CheckDeity(source);

            // Check if the player worships the deity of the shrine
            if ((worshipedDeity == null) || !Subject.Template.Name.Contains(worshipedDeity))
            {
                source.SendActiveMessage("You attempt to pray at the shrine but the god won't listen.");

                return; // Exit the method if the player doesn't worship the shrine's deity
            }

            // Player worships the deity of the shrine, allow them to receive faith
            switch (worshipedDeity)
            {
                case "Miraelis":
                    source.SendActiveMessage("You pray at the shrine to Miraelis. You received faith.");

                    break;
                case "Serendael":
                    source.SendActiveMessage("You pray at the shrine to Serendael. You received faith.");

                    break;
                case "Theselene":
                    source.SendActiveMessage("You pray at the shrine to Theselene. You received faith.");

                    break;
                case "Skandara":
                    source.SendActiveMessage("You pray at the shrine to Skandara. You received faith.");

                    break;
            }

            // Add faith to the player
            ReligionScriptBase.TryAddFaith(source, 5);

            // Remove the shrine from the map
            Subject.MapInstance.RemoveEntity(Subject);
        } else
            source.SendActiveMessage("The shrine seems to beckon you closer...");
    }
}
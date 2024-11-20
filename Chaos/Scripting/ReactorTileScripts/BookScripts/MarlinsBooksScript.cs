using Chaos.Common.Definitions;
using Chaos.DarkAges.Definitions;
using Chaos.Models.World;
using Chaos.Scripting.ReactorTileScripts.Abstractions;

namespace Chaos.Scripting.ReactorTileScripts.BookScripts;

public sealed class MarlinsBooksScript : ReactorTileScriptBase
{
    /// <inheritdoc />
    public MarlinsBooksScript(ReactorTile subject)
        : base(subject) {}

    /// <inheritdoc />
    public override void OnClicked(Aisling source)
    {
        source.SendOrangeBarMessage("You pull a news paper from a book on the shelf.");
        
        source.Client.SendServerMessage(
            ServerMessageType.ScrollWindow,
            "{=qLOURES NEWS: Marlin Exonerated as Real Culprit Found{=g\n\nIn a dramatic turn of events, the enigmatic Professor Marlin of Loures Castle has been exonerated of all charges related to the tragic death of Bella. The saga began with a love triangle involving Marlin and a young woman named Bella, who was also admired by many others within the castle walls.\n\nTensions rose when Bella mysteriously disappeared, and suspicion fell upon Marlin. Those who harbored jealousy towards him seized the opportunity to frame him for Bella's murder. However, the plot took a darker turn when it was revealed that Bella had not been killed initially but was held captive in a dungeon.\n\nTragically, Bella met her end while imprisoned, a fate orchestrated by Jean, who masterminded the plot against Marlin. Jean's deceit unraveled when the divine intervention of Goddess Miraelis exposed him sneaking into the dungeon. Despite his claims of ignorance about Bella's whereabouts and his attempts to pin her disappearance on Marlin, Jean was caught red-handed.\n\nNow, Jean languishes in Loures Jail, while Marlin has returned to his duties at Loures Castle. The shadow of Bella's untimely death continues to loom over both men, fueling an ongoing feud. Bella, originally from Piet, had come to the castle to assist Blaise in organizing the library. She was a beloved daughter to her father, Stanley, and her loss is deeply felt.\n\nBella's final resting place is in House Macabre, a somber reminder of the tragic events that transpired within the castle. Her memory lingers, a poignant symbol of love and loss, as Marlin and Jean grapple with the consequences of their actions and the void left by Bella's absence.");
    }
}
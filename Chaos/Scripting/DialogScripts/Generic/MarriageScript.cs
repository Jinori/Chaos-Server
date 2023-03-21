using Chaos.Clients.Abstractions;
using Chaos.Common.Definitions;
using Chaos.Extensions.Common;
using Chaos.Extensions.Geometry;
using Chaos.Networking.Abstractions;
using Chaos.Objects.Legend;
using Chaos.Objects.Menu;
using Chaos.Objects.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Time;
using Chaos.Utilities;
using Microsoft.Extensions.Logging;


namespace Chaos.Scripting.DialogScripts.Generic;


public class MarriageScript : DialogScriptBase
{
    private readonly IClientRegistry<IWorldClient> ClientRegistry;
    private readonly IDialogFactory DialogFactory;

    public MarriageScript(Dialog subject, IClientRegistry<IWorldClient> clientRegistry, IDialogFactory dialogFactory)
        : base(subject) {
        ClientRegistry = clientRegistry;
        DialogFactory = dialogFactory;
    }

    public override void OnDisplaying(Aisling source)
    {
        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "generic_marriageinitial":
                {
                    if (source.Legend.ContainsKey("marriage"))
                    {
                        Subject.Reply(source, "You are already married.");
                        return;
                    }
                    break;
                }

            default:
                break;
        }
        
    }

    public override void OnNext(Aisling source, byte? optionIndex = null)
    {
        switch (Subject.Template.TemplateKey.ToLower())
        {

            case "generic_marriageinitial":
                {
                    if (!Subject.MenuArgs.TryGet<string>(0, out var name))
                    {
                        Subject.Reply(source, DialogString.UnknownInput.Value);
                        return;
                    }

                    var partner = ClientRegistry.FirstOrDefault(cli => cli.Aisling.Name.EqualsI(name));
                    if (partner == null || !partner.Aisling.OnSameMapAs(source))
                    {
                        Subject.Reply(source, "It does not look like they are here.");
                        return;
                    }

                    if (partner.Aisling.Equals(source))
                    {
                        Subject.Reply(source, "You cannot marry yourself.");
                        return;
                    }

                    if (partner.Aisling.Legend.ContainsKey("marriage"))
                    {
                        Subject.Reply(source, "They are already married.");
                        return;
                    }

                    var dialog = DialogFactory.Create("generic_marriagesecondary", Subject.SourceEntity);
                    dialog.MenuArgs = Subject.MenuArgs;
                    dialog.Text = dialog.Text.Inject(source.Name);
                    dialog.Display(partner.Aisling);
                    Subject.Close(source);
                    break;
                }

            case "generic_marriagesecondary":
                {
                    if (!Subject.MenuArgs.TryGetNext<string>(out var nameOne))
                    {
                        Subject.Reply(source, DialogString.UnknownInput.Value);
                        return;
                    }

                    var partnerOne = ClientRegistry.FirstOrDefault(cli => cli.Aisling.Name.EqualsI(nameOne));
                    if (partnerOne == null)
                    {
                        Subject.Reply(source, "That player does not exist");
                        return;
                    }


                    //Second person
                    if (!Subject.MenuArgs.TryGetNext<string>(out var nameTwo))
                    {
                        Subject.Reply(source, DialogString.UnknownInput.Value);
                        return;
                    }

                    var partnerTwo = ClientRegistry.FirstOrDefault(cli => cli.Aisling.Name.EqualsI(nameTwo));
                    if (partnerTwo == null)
                    {
                        Subject.Reply(source, "That player does not exist");
                        return;
                    }

                    if (!Subject.Text.StartsWithI(partnerTwo.Aisling.Name))
                    {
                        Subject.Reply(source, "That person did not propose to you.");
                        return;
                    }

                    var markOne = new LegendMark($"Married {partnerTwo.Aisling.Name}", "marriage", MarkIcon.Heart, MarkColor.Pink, 1, GameTime.Now);
                    var markTwo = new LegendMark($"Married {partnerOne.Aisling.Name}", "marriage", MarkIcon.Heart, MarkColor.Pink, 1, GameTime.Now);

                    partnerOne.Aisling.Legend.AddOrAccumulate(markOne);
                    partnerTwo.Aisling.Legend.AddOrAccumulate(markTwo);

                    Subject.Close(source);
                    break;
                }

            default:
                break;
        }
    }
}
using Chaos.Clients.Abstractions;
using Chaos.Common.Definitions;
using Chaos.Containers;
using Chaos.Data;
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
                    if (!source.HasClass(BaseClass.Priest))
                    {
                        Subject.Reply(source, "You are not a priest.");
                        return;
                    }
                    //if (source.Legend.ContainsKey("marriage"))
                    //{
                    //    Subject.Reply(source, "You are already married.");
                    //    return;
                    //}
                    break;
                }

            case "generic_divorceinitial":
                {
                    if (!source.Legend.ContainsKey("marriage"))
                    {
                        Subject.Reply(source, "You are not married.");
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

                    //Priest entered first partner
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
                    if (!Subject.MenuArgs.TryGet<string>(1, out var nameTwo))
                    {
                        Subject.Reply(source, DialogString.UnknownInput.Value);
                        return;
                    }

                    //First partner entered second partner
                    var partnerTwo = ClientRegistry.FirstOrDefault(cli => cli.Aisling.Name.EqualsI(nameTwo));
                    if (partnerTwo == null || !partnerTwo.Aisling.OnSameMapAs(source))
                    {
                        Subject.Reply(source, "It does not look like they are here.");
                        return;
                    }

                    if (partnerTwo.Aisling.Name.Equals(source.Name))
                    {
                        Subject.Reply(source, "You cannot marry yourself.");
                        return;
                    }

                    var dialog = DialogFactory.Create("generic_marriagefinal", Subject.SourceEntity);
                    dialog.MenuArgs = Subject.MenuArgs;
                    dialog.Text = dialog.Text.Inject(source.Name);
                    dialog.Display(partnerTwo.Aisling);
                    Subject.Close(source);

                    break;
                }

            case "generic_marriagefinal":
                {
                    if (!Subject.MenuArgs.TryGet<string>(0, out var nameOne))
                    {
                        //Subject.Reply(source, DialogString.UnknownInput.Value);
                        source.SendOrangeBarMessage(DialogString.UnknownInput.Value);
                        Subject.Close(source);
                        return;
                    }

                    var partnerOne = ClientRegistry.FirstOrDefault(cli => cli.Aisling.Name.EqualsI(nameOne));
                    if (partnerOne == null || !partnerOne.Aisling.OnSameMapAs(source))
                    {
                        //Subject.Reply(source, "It does not look like they are here.");
                        source.SendOrangeBarMessage("It does not look like they are here.");
                        Subject.Close(source);
                        return;
                    }

                    if (!Subject.MenuArgs.TryGet<string>(1, out var nameTwo))
                    {
                        //Subject.Reply(source, DialogString.UnknownInput.Value);
                        source.SendOrangeBarMessage(DialogString.UnknownInput.Value);
                        Subject.Close(source);
                        return;
                    }

                    var partnerTwo = ClientRegistry.FirstOrDefault(cli => cli.Aisling.Name.EqualsI(nameTwo));
                    if (partnerTwo == null || !partnerTwo.Aisling.OnSameMapAs(source))
                    {
                        //Subject.Reply(source, "It does not look like they are here.");
                        source.SendOrangeBarMessage("It does not look like they are here.");
                        Subject.Close(source);
                        return;
                    }

                    if (optionIndex is 2)
                    {
                        partnerOne.Aisling.SendOrangeBarMessage("They said no..");
                        partnerTwo.Aisling.SendOrangeBarMessage($"You have denied {partnerOne.Aisling.Name}s proposal.");
                    }

                    if (optionIndex is 1)
                    {
                        partnerOne.Aisling.SendOrangeBarMessage($"Congratulations! You are now married to {partnerTwo.Aisling.Name}");
                        partnerTwo.Aisling.SendOrangeBarMessage($"Congratulations! You are now married to {partnerOne.Aisling.Name}");
                        var markOne = new LegendMark($"Married {partnerTwo.Aisling.Name}", "marriage", MarkIcon.Heart, MarkColor.Pink, 1, GameTime.Now);
                        var markTwo = new LegendMark($"Married {partnerOne.Aisling.Name}", "marriage", MarkIcon.Heart, MarkColor.Pink, 1, GameTime.Now);

                        var ani = new Animation
                        {
                            AnimationSpeed = 150,
                            TargetAnimation = 98
                        };

                        partnerOne.Aisling.Animate(ani);
                        partnerTwo.Aisling.Animate(ani);

                        partnerOne.Aisling.MapInstance.PlaySound(50, partnerOne.Aisling);
                        

                        partnerOne.Aisling.Legend.AddOrAccumulate(markOne);
                        partnerTwo.Aisling.Legend.AddOrAccumulate(markTwo);
                    }

                    Subject.Close(source);
                    break;
                }

            //Divorce
            case "generic_divorceinitial":
                {
                    source.Legend.TryGetValue("marriage", out var marriageMark);
                    var markSplit = marriageMark.Text.Split();

                    //Find partner to remove from legend
                    var partner = ClientRegistry.FirstOrDefault(cli => cli.Aisling.Name.EqualsI(markSplit[1]));
                    if (partner == null)
                    {
                        Subject.Reply(source, "It does not look like they currently in Unora.");
                        return;
                    }


                    if (optionIndex is 1)
                    {
                        source.Legend.Remove("marriage", out var marriageMarkOne);
                        partner.Aisling.Legend.Remove("marriage", out var marriageMarkTwo);

                        source.SendOrangeBarMessage($"You have divorced {partner.Aisling.Name}.");
                        partner.Aisling.SendOrangeBarMessage($"{source.Name} has divorced you.");

                        var markOne = new LegendMark($"Divorced {partner.Aisling.Name}", "divorce", MarkIcon.Yay, MarkColor.Brown, 1, GameTime.Now);
                        var markTwo = new LegendMark($"Divorced {source.Name}", "divorce", MarkIcon.Yay, MarkColor.Brown, 1, GameTime.Now);

                        source.Legend.AddOrAccumulate(markOne);
                        partner.Aisling.Legend.AddOrAccumulate(markTwo);
                    }

                    Subject.Close(source);
                    break;
                }

            default:
                break;
        }
    }
}
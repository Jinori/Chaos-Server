using Chaos.Common.Definitions;
using Chaos.Extensions.Common;
using Chaos.Extensions.Geometry;
using Chaos.Models.Data;
using Chaos.Models.Legend;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Networking.Abstractions;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Time;
using Chaos.Utilities;


namespace Chaos.Scripting.DialogScripts.Generic;


public class MarriageScript : DialogScriptBase
{
    private readonly IClientRegistry<IWorldClient> ClientRegistry;
    private readonly IDialogFactory DialogFactory;
    private readonly IEffectFactory _effectFactory;

    public MarriageScript(Dialog subject, IClientRegistry<IWorldClient> clientRegistry, IDialogFactory dialogFactory, IEffectFactory effectFactory)
        : base(subject) {
        ClientRegistry = clientRegistry;
        DialogFactory = dialogFactory;
        _effectFactory = effectFactory;
    }

    public override void OnDisplaying(Aisling source)
    {
        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "generic_marriageinitial":
                {
                    if (!source.HasClass(BaseClass.Priest))
                    {
                        Subject.Reply(source, "You must be a priest to perform a marriage ceremony.");
                    }
                    break;
                }

            case "generic_divorceinitial":
                {
                    if (!source.Legend.ContainsKey("marriage"))
                    {
                        Subject.Reply(source, "You are not married, I cannot assist you with a divorce.");
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
                        Subject.ReplyToUnknownInput(source);
                        return;
                    }

                    //Priest entered first partners name
                    var partner = ClientRegistry.FirstOrDefault(cli => cli.Aisling.Name.EqualsI(name));
                    if ((partner == null) || !partner.Aisling.OnSameMapAs(source))
                    {
                        Subject.Reply(source, "It does not look like they are here, I cannot proceed without your partner being here with you.");
                        return;
                    }

                    if (partner.Aisling.Equals(source))
                    {
                        Subject.Reply(source, "Unfortunately you are not able to marry yourself.");
                        return;
                    }

                    if (partner.Aisling.Legend.ContainsKey("marriage"))
                    {
                        Subject.Reply(source, "That Aisling is already married.");
                        return;
                    }

                    var dialog = DialogFactory.Create("generic_marriagesecondary", Subject.DialogSource);
                    dialog.MenuArgs = Subject.MenuArgs;
                    dialog.InjectTextParameters(source.Name);
                    dialog.Display(partner.Aisling);
                    Subject.Close(source);
                    break;
                }

            case "generic_marriagesecondary":
                {
                    if (!Subject.MenuArgs.TryGet<string>(1, out var nameTwo))
                    {
                        Subject.ReplyToUnknownInput(source);
                        return;
                    }

                    //First partner entered second partners name
                    var partnerTwo = ClientRegistry.FirstOrDefault(cli => cli.Aisling.Name.EqualsI(nameTwo));
                    if ((partnerTwo == null) || !partnerTwo.Aisling.OnSameMapAs(source))
                    {
                        Subject.Reply(source, "It does not look like they are here, I cannot proceed without your partner being here with you.");
                        return;
                    }

                    if (partnerTwo.Aisling.Name.Equals(source.Name))
                    {
                        Subject.Reply(source, "Unfortunately you are not able to marry yourself.");
                        return;
                    }

                    var dialog = DialogFactory.Create("generic_marriagefinal", Subject.DialogSource);
                    dialog.MenuArgs = Subject.MenuArgs;
                    dialog.InjectTextParameters(source.Name);
                    dialog.Display(partnerTwo.Aisling);
                    Subject.Close(source);

                    break;
                }

            case "generic_marriagefinal":
                {
                    if (!Subject.MenuArgs.TryGet<string>(0, out var nameOne))
                    {
                        source.SendOrangeBarMessage(DialogString.UnknownInput);
                        Subject.Close(source);
                        return;
                    }

                    var partnerOne = ClientRegistry.FirstOrDefault(cli => cli.Aisling.Name.EqualsI(nameOne));
                    if ((partnerOne == null) || !partnerOne.Aisling.OnSameMapAs(source))
                    {
                        source.SendOrangeBarMessage("It does not look like they are here.");
                        Subject.Close(source);
                        return;
                    }

                    if (!Subject.MenuArgs.TryGet<string>(1, out var nameTwo))
                    {
                        source.SendOrangeBarMessage(DialogString.UnknownInput);
                        Subject.Close(source);
                        return;
                    }

                    var partnerTwo = ClientRegistry.FirstOrDefault(cli => cli.Aisling.Name.EqualsI(nameTwo));
                    if ((partnerTwo == null) || !partnerTwo.Aisling.OnSameMapAs(source))
                    {
                        source.SendOrangeBarMessage("It does not look like they are here.");
                        Subject.Close(source);
                        return;
                    }

                    if (optionIndex is 2)
                    {
                        partnerOne.Aisling.SendOrangeBarMessage("It looks like they have rejected your proposal.");
                        partnerTwo.Aisling.SendOrangeBarMessage($"You have denied {partnerOne.Aisling.Name}'s proposal.");
                    }

                    if (optionIndex is 1)
                    {
                        partnerOne.Aisling.SendOrangeBarMessage($"Congratulations! You are now married to {partnerTwo.Aisling.Name}!");
                        partnerTwo.Aisling.SendOrangeBarMessage($"Congratulations! You are now married to {partnerOne.Aisling.Name}!");
                        var markOne = new LegendMark($"Married {partnerTwo.Aisling.Name}", "marriage", MarkIcon.Heart, MarkColor.Pink, 1, GameTime.Now);
                        var markTwo = new LegendMark($"Married {partnerOne.Aisling.Name}", "marriage", MarkIcon.Heart, MarkColor.Pink, 1, GameTime.Now);

                        partnerOne.Aisling.Legend.AddOrAccumulate(markOne);
                        partnerTwo.Aisling.Legend.AddOrAccumulate(markTwo);

                        var ani = new Animation
                        {
                            AnimationSpeed = 150,
                            TargetAnimation = 36
                        };
                        var ani2 = new Animation
                        {
                            AnimationSpeed = 150,
                            TargetAnimation = 36
                        };

                        var effect = _effectFactory.Create("marriage");
                        var effect2 = _effectFactory.Create("marriage");
                        partnerOne.Aisling.Effects.Apply(partnerOne.Aisling, effect);
                        partnerTwo.Aisling.Effects.Apply(partnerTwo.Aisling, effect2);
                    }

                    Subject.Close(source);
                    break;
                }

            //Divorce
            case "generic_divorceinitial":
                {
                    source.Legend.TryGetValue("marriage", out var marriageMark);

                    if (marriageMark is null)
                    {
                        break;
                    }

                    var markSplit = marriageMark.Text.Split();

                    if (optionIndex is 1)
                    {
                        source.Legend.Remove("marriage", out var marriageMarkOne);
                        source.SendOrangeBarMessage($"You have divorced {markSplit[1]}.");

                        var markOne = new LegendMark($"Divorced {markSplit[1]}", "divorce", MarkIcon.Yay, MarkColor.Brown, 1, GameTime.Now);

                        source.Legend.AddOrAccumulate(markOne);
                    }

                    Subject.Close(source);
                    break;
                }

            default:
                break;
        }
    }
}
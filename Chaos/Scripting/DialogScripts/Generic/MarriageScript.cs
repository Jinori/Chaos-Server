using Chaos.DarkAges.Definitions;
using Chaos.Extensions.Common;
using Chaos.Extensions.Geometry;
using Chaos.Geometry.Abstractions;
using Chaos.Models.Data;
using Chaos.Models.Legend;
using Chaos.Models.Menu;
using Chaos.Models.Templates;
using Chaos.Models.World;
using Chaos.Networking.Abstractions;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Time;
using Chaos.Utilities;

namespace Chaos.Scripting.DialogScripts.Generic;

public class MarriageScript : DialogScriptBase
{
    private readonly Animation Animation = new()
    {
        AnimationSpeed = 300,
        TargetAnimation = 36
    };

    private readonly IClientRegistry<IChaosWorldClient> ClientRegistry;
    private readonly IDialogFactory DialogFactory;
    private readonly IEffectFactory EffectFactory;

    public MarriageScript(
        Dialog subject,
        IClientRegistry<IChaosWorldClient> clientRegistry,
        IDialogFactory dialogFactory,
        IEffectFactory effectFactory)
        : base(subject)
    {
        ClientRegistry = clientRegistry;
        DialogFactory = dialogFactory;
        EffectFactory = effectFactory;
    }

    private List<IPoint> GenerateValidSpawnPoints(MapTemplate selectedMap)
    {
        var validPoints = new List<IPoint>();

        for (var x = 0; x < selectedMap.Width; x++)
        {
            for (var y = 0; y < selectedMap.Height; y++)
            {
                IPoint point = new Point(x, y);

                if (selectedMap.IsWall(point))
                    continue;

                validPoints.Add(point);
            }
        }

        return validPoints;
    }

    private void HandleDivorce(Aisling source, byte? optionIndex)
    {
        source.Legend.TryGetValue("marriage", out var marriageMark);

        if (marriageMark is null)
            return;

        var markSplit = marriageMark.Text.Split();

        if (optionIndex is 1)
        {
            source.Legend.Remove("marriage", out _);
            source.SendOrangeBarMessage($"You are no longer married to {markSplit[1]}.");
            var partner = ClientRegistry.FirstOrDefault(cli => cli.Aisling.Name.EqualsI(markSplit[1]));
            partner?.Aisling.Legend.Remove("marriage", out _);
            partner?.Aisling.SendOrangeBarMessage($"{source.Name} has divorced you.");
            Subject.Close(source);

            return;
        }

        source.SendOrangeBarMessage("You have decided not to get a divorce.");
        Subject.Close(source);
    }

    private void HandleDivorceInitialization(Aisling source)
    {
        if (!source.Legend.ContainsKey("marriage"))
            Subject.Reply(source, "You are not married, I cannot assist you with a divorce.");
    }

    private void HandleMarriage(Aisling source)
    {
        //Priest entered first partners name
        if (!Subject.MenuArgs.TryGet<string>(0, out var name))
        {
            Subject.ReplyToUnknownInput(source);

            return;
        }

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
    }

    private void HandleMarriageFinal(Aisling source, byte? optionIndex)
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

            var markOne = new LegendMark(
                $"Married {partnerTwo.Aisling.Name}",
                "marriage",
                MarkIcon.Heart,
                MarkColor.Pink,
                1,
                GameTime.Now);

            var markTwo = new LegendMark(
                $"Married {partnerOne.Aisling.Name}",
                "marriage",
                MarkIcon.Heart,
                MarkColor.Pink,
                1,
                GameTime.Now);

            partnerOne.Aisling.Legend.AddOrAccumulate(markOne);
            partnerTwo.Aisling.Legend.AddOrAccumulate(markTwo);

            var effect = EffectFactory.Create("marriage");
            var effect2 = EffectFactory.Create("marriage");
            partnerOne.Aisling.Effects.Apply(partnerOne.Aisling, effect);
            partnerTwo.Aisling.Effects.Apply(partnerTwo.Aisling, effect2);
            var heartList = GenerateValidSpawnPoints(source.MapInstance.Template);

            foreach (var point in heartList)
                source.MapInstance.ShowAnimation(Animation.GetPointAnimation(point));

            foreach (var person in ClientRegistry)
                person.Aisling.SendActiveMessage($"Congratulations newlyweds {partnerOne.Aisling.Name} and {partnerTwo.Aisling.Name}!");
        }

        Subject.Close(source);
    }

    private void HandleMarriageInitialization(Aisling source)
    {
        if (!source.HasClass(BaseClass.Priest))
            Subject.Reply(source, "You must be a priest to perform a marriage ceremony.");
    }

    private void HandleMarriageSecondary(Aisling source)
    {
        //First partner entered second partners name
        if (!Subject.MenuArgs.TryGet<string>(1, out var nameTwo))
        {
            Subject.ReplyToUnknownInput(source);

            return;
        }

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
    }

    public override void OnDisplaying(Aisling source)
    {
        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "generic_marriageinitial":
                HandleMarriageInitialization(source);

                break;

            case "generic_divorceinitial":
                HandleDivorceInitialization(source);

                break;
        }
    }

    public override void OnNext(Aisling source, byte? optionIndex = null)
    {
        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "generic_marriageinitial":
                HandleMarriage(source);

                break;

            case "generic_marriagesecondary":
                HandleMarriageSecondary(source);

                break;

            case "generic_marriagefinal":
                HandleMarriageFinal(source, optionIndex);

                break;

            case "generic_divorceinitial":
                HandleDivorce(source, optionIndex);

                break;
        }
    }
}
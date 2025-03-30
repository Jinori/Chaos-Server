using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.ReactorTileScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.ReactorTileScripts.GuildHall;

public class GuildHallCombatBooksScript : ReactorTileScriptBase
{
    private IIntervalTimer AnimationTimer { get; }
    private IDialogFactory DialogFactory { get; }

    private Animation HuntCombatAnimation { get; } = new()
    {
        AnimationSpeed = 140,
        TargetAnimation = 409
    };

    private Animation MagicCombatAnimation { get; } = new()
    {
        AnimationSpeed = 140,
        TargetAnimation = 406
    };

    private IMerchantFactory MerchantFactory { get; }

    private Animation SkillCombatAnimation { get; } = new()
    {
        AnimationSpeed = 140,
        TargetAnimation = 407
    };

    public GuildHallCombatBooksScript(ReactorTile subject, IDialogFactory dialogFactory, IMerchantFactory merchantFactory)
        : base(subject)
    {
        DialogFactory = dialogFactory;
        MerchantFactory = merchantFactory;
        AnimationTimer = new IntervalTimer(TimeSpan.FromMilliseconds(800), false);
    }

    public override void OnWalkedOn(Creature source)
    {
        if (source is not Aisling aisling)
            return;

        switch (new Point(Subject.X, Subject.Y))
        {
            case var p when p == new Point(54, 30):
                var point = new Point(source.X, source.Y);
                var blankMerchant = MerchantFactory.Create("magiccombat", Subject.MapInstance, point);
                var dialog = DialogFactory.Create("magiccombat_initial", blankMerchant);
                dialog.Display(aisling);

                break;

            case var p when p == new Point(54, 22):
                var point1 = new Point(source.X, source.Y);
                var blankMerchant1 = MerchantFactory.Create("huntcombat", Subject.MapInstance, point1);
                var dialog1 = DialogFactory.Create("huntcombat_initial", blankMerchant1);
                dialog1.Display(aisling);

                break;

            case var p when p == new Point(54, 37):
                var point2 = new Point(source.X, source.Y);
                var blankMerchant2 = MerchantFactory.Create("skillcombat", Subject.MapInstance, point2);
                var dialog2 = DialogFactory.Create("skillcombat_initial", blankMerchant2);
                dialog2.Display(aisling);

                break;
        }
    }

    public override void Update(TimeSpan delta)
    {
        AnimationTimer.Update(delta);

        if (!AnimationTimer.IntervalElapsed)
            return;

        var aislings = Subject.MapInstance.GetEntitiesWithinRange<Aisling>(Subject, 12);

        switch (new Point(Subject.X, Subject.Y))
        {
            case var p when p == new Point(54, 30):
                foreach (var aisling in aislings)
                    aisling.MapInstance.ShowAnimation(MagicCombatAnimation.GetPointAnimation(new Point(Subject.X, Subject.Y)));

                break;

            case var p when p == new Point(54, 22):
                foreach (var aisling in aislings)
                    aisling.MapInstance.ShowAnimation(HuntCombatAnimation.GetPointAnimation(new Point(Subject.X, Subject.Y)));

                break;

            case var p when p == new Point(54, 37):
                foreach (var aisling in aislings)
                    aisling.MapInstance.ShowAnimation(SkillCombatAnimation.GetPointAnimation(new Point(Subject.X, Subject.Y)));

                break;
        }
    }
}
using Chaos.Common.Utilities;
using Chaos.Extensions.Common;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Crafting.Abstractions;
using Chaos.Scripting.WorldScripts.WorldBuffs.Religion;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.DialogScripts.Crafting.Refining;

public class MetalRefiningScript : CraftingBaseScript
{
    private readonly string[] MetalTemplateKeys =
    [
        "rawbronze",
        "tarnishedbronzebar",
        "rawiron",
        "tarnishedironbar",
        "rawmythril",
        "tarnishedmythrilbar",
        "rawhybrasyl",
        "tarnishedhybrasylbar",
        "rawcrimsonite",
        "rawazurium",
        "tarnishedcrimsonitebar",
        "tarnishedazuriumbar"
    ];

    protected override Dictionary<string, string> DowngradeMappings { get; } = new(StringComparer.OrdinalIgnoreCase)
    {
        {
            "rawbronze", "ruinedbronze"
        },
        {
            "tarnishedbronzebar", "ruinedbronze"
        },
        {
            "rawiron", "ruinediron"
        },
        {
            "tarnishedironbar", "ruinediron"
        },
        {
            "rawmythril", "ruinedmythril"
        },
        {
            "tarnishedmythrilbar", "ruinedmythril"
        },
        {
            "rawhybrasyl", "ruinedhybrasyl"
        },
        {
            "tarnishedhybrasylbar", "ruinedhybrasyl"
        },
        {
            "rawcrimsonite", "ruinedcrimsonite"
        },
        {
            "tarnishedcrimsonitebar", "ruinedcrimsonite"
        },
        {
            "rawazurium", "ruinedazurium"
        },
        {
            "tarnishedazuriumbar", "ruinedazurium"
        }
    };

    protected override Dictionary<string, string> UpgradeMappings { get; } = new(StringComparer.OrdinalIgnoreCase)
    {
        {
            "rawbronze", "tarnishedbronzebar"
        },
        {
            "tarnishedbronzebar", "polishedbronzebar"
        },
        {
            "rawiron", "tarnishedironbar"
        },
        {
            "tarnishedironbar", "polishedironbar"
        },
        {
            "rawmythril", "tarnishedmythrilbar"
        },
        {
            "tarnishedmythrilbar", "polishedmythrilbar"
        },
        {
            "rawhybrasyl", "tarnishedhybrasylbar"
        },
        {
            "tarnishedhybrasylbar", "polishedhybrasylbar"
        },
        {
            "rawcrimsonite", "tarnishedcrimsonitebar"
        },
        {
            "rawazurium", "tarnishedazuriumbar"
        },
        {
            "tarnishedazuriumbar", "polishedazuriumbar"
        },
        {
            "tarnishedcrimsonitebar", "polishedcrimsonitebar"
        }
    };

    protected override double BaseSucessRate => 60;
    protected override string ItemCounterPrefix => "[Refine]";

    protected override string LegendMarkKey => "MetalRefining";
    protected override double SuccessRateMax => 90;

    /// <inheritdoc />
    public MetalRefiningScript(Dialog subject, IItemFactory itemFactory, IDialogFactory dialogFactory, IStorage<ReligionBuffs> religionBuffStorage)
        : base(subject, itemFactory, dialogFactory, religionBuffStorage) { }

    public override void OnDisplaying(Aisling source)
    {
        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "metal_refining_initial":
                OnDisplayingShowPlayerItems(source);

                break;
            case "metal_refining_confirmation":
                OnDisplayingConfirmation(source);

                break;
            case "metal_refining_accepted":
                OnDisplayingAccepted(source);

                break;
        }
    }

    private void OnDisplayingAccepted(Aisling source)
    {
        if (!TryFetchArg<byte>(0, out var slot) || !source.Inventory.TryGetObject(slot, out var item))
        {
            Subject.Reply(source, "You ran out of that metal to refine.", "metal_refining_initial");

            return;
        }

        if (item.DisplayName.ContainsI("ruined"))
        {
            Subject.Reply(source, "This metal is ruined already.", "metal_refining_initial");

            return;
        }

        if (item.DisplayName.ContainsI("polished"))
        {
            Subject.Reply(source, "This metal is polished already.", "metal_refining_initial");

            return;
        }

        var requiredCount = item.Template.TemplateKey.Contains("raw") ? 2 : 1;

        if (item.Count < requiredCount)
        {
            Subject.Reply(source, $"You ran out of {item.DisplayName} to refine.", "metal_refining_initial");

            return;
        }

        source.Inventory.RemoveQuantityByTemplateKey(item.Template.TemplateKey, requiredCount);
        var successRate = CalculateRefiningSuccessRate(source, item);

        if (!IntegerRandomizer.RollChance((int)successRate))
        {
            UpdateRefiningCounter(source, item);

            HandleFailure(
                source,
                item,
                GetDowngradeKey(item.Template.TemplateKey),
                item.DisplayName,
                "metal_refining_failed");

            return;
        }

        HandleSuccess(source, item);
    }

    private void OnDisplayingConfirmation(Aisling source)
    {
        if (!TryFetchArg<byte>(0, out var slot) || !source.Inventory.TryGetObject(slot, out var item))
        {
            Subject.Reply(source, "You don't have enough material.", "metal_refining_initial");

            return;
        }

        var requiredCount = MetalTemplateKeys.ContainsI(item.Template.TemplateKey) && item.Template.TemplateKey.ContainsI("raw") ? 2 : 1;

        if (item.Count < requiredCount)
        {
            Subject.Reply(source, $"You need at least {requiredCount} units of the material to proceed.", "metal_refining_initial");

            return;
        }

        if (item.DisplayName.ContainsI("ruined"))
        {
            Subject.Reply(source, "This metal is ruined already.", "metal_refining_initial");

            return;
        }

        if (item.DisplayName.ContainsI("polished"))
        {
            Subject.Reply(source, "This metal is polished already.", "metal_refining_initial");

            return;
        }

        if (!MetalTemplateKeys.ContainsI(item.Template.TemplateKey))
        {
            Subject.Reply(source, "This item cannot be refined. Please select a valid material.", "metal_refining_initial");

            return;
        }

        var successRate = CalculateRefiningSuccessRate(source, item);
        Subject.InjectTextParameters(item.DisplayName, successRate.ToString("N2"));
    }

    public void OnDisplayingShowPlayerItems(Aisling source)
        => Subject.Slots = source.Inventory
                                 .Where(x => MetalTemplateKeys.ContainsI(x.Template.TemplateKey) && (x.Count >= 1))
                                 .Select(x => x.Slot)
                                 .ToList();
}
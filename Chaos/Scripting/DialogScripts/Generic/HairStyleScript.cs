using Chaos.Common.Definitions;
using Chaos.Extensions.Common;
using Chaos.Models.Data;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.DialogScripts.Generic;

public class HairstyleScript : DialogScriptBase
{
    private static readonly string[] FemaleHairstyles =
    {
        "female_hairstyle_1", "female_hairstyle_2", "female_hairstyle_3", "female_hairstyle_4", "female_hairstyle_5", "female_hairstyle_6",
        "female_hairstyle_7", "female_hairstyle_8", "female_hairstyle_9", "female_hairstyle_10", "female_hairstyle_11",
        "female_hairstyle_12", "female_hairstyle_13", "female_hairstyle_14", "female_hairstyle_15", "female_hairstyle_16",
        "female_hairstyle_17",
        "female_hairstyle_19", "female_hairstyle_20", "female_hairstyle_21", "female_hairstyle_22", "female_hairstyle_23",
        "female_hairstyle_24", "female_hairstyle_25",
        "female_hairstyle_26", "female_hairstyle_27", "female_hairstyle_28", "female_hairstyle_29", "female_hairstyle_30",
        "female_hairstyle_31", "female_hairstyle_32",
        "female_hairstyle_33", "female_hairstyle_34", "female_hairstyle_35", "female_hairstyle_36", "female_hairstyle_37",
        "female_hairstyle_38", "female_hairstyle_39",
        "female_hairstyle_40", "female_hairstyle_41", "female_hairstyle_42", "female_hairstyle_43", "female_hairstyle_44",
        "female_hairstyle_45", "female_hairstyle_46",
        "female_hairstyle_47", "female_hairstyle_48", "female_hairstyle_49", "female_hairstyle_50", "female_hairstyle_51",
        "female_hairstyle_52", "female_hairstyle_53",
        "female_hairstyle_54", "female_hairstyle_55", "female_hairstyle_56", "female_hairstyle_57", "female_hairstyle_58",
        "female_hairstyle_59", "female_hairstyle_60",
        "female_hairstyle_64", "female_hairstyle_65", "female_hairstyle_66", "female_hairstyle_67", "female_hairstyle_68",
        "female_hairstyle_69", "female_hairstyle_70", "female_hairstyle_71", "female_hairstyle_72", "female_hairstyle_73",
        "female_hairstyle_74", "female_hairstyle_75", "female_hairstyle_76", "female_hairstyle_77", "female_hairstyle_78",
        "female_hairstyle_79", "female_hairstyle_80", "female_hairstyle_81", "female_hairstyle_82", "female_hairstyle_83",
        "female_hairstyle_84", "female_hairstyle_85", "female_hairstyle_86", "female_hairstyle_87", "female_hairstyle_88",
        "female_hairstyle_89", "female_hairstyle_90", "female_hairstyle_91", "female_hairstyle_92", "female_hairstyle_93",
        "female_hairstyle_94", "female_hairstyle_95", "female_hairstyle_97", "female_hairstyle_98"
    };
    private static readonly string[] MaleHairstyles =
    {
        "male_hairstyle_1", "male_hairstyle_2", "male_hairstyle_3", "male_hairstyle_4", "male_hairstyle_5", "male_hairstyle_6",
        "male_hairstyle_7", "male_hairstyle_8", "male_hairstyle_9", "male_hairstyle_10", "male_hairstyle_11",
        "male_hairstyle_12", "male_hairstyle_13", "male_hairstyle_14", "male_hairstyle_15", "male_hairstyle_16", "male_hairstyle_17",
        "male_hairstyle_19", "male_hairstyle_20", "male_hairstyle_21", "male_hairstyle_22", "male_hairstyle_23", "male_hairstyle_24",
        "male_hairstyle_25",
        "male_hairstyle_26", "male_hairstyle_27", "male_hairstyle_28", "male_hairstyle_29", "male_hairstyle_30", "male_hairstyle_31",
        "male_hairstyle_32",
        "male_hairstyle_33", "male_hairstyle_34", "male_hairstyle_35", "male_hairstyle_36", "male_hairstyle_37", "male_hairstyle_38",
        "male_hairstyle_39",
        "male_hairstyle_40", "male_hairstyle_41", "male_hairstyle_42", "male_hairstyle_43", "male_hairstyle_44", "male_hairstyle_45",
        "male_hairstyle_46",
        "male_hairstyle_47", "male_hairstyle_48", "male_hairstyle_49", "male_hairstyle_50", "male_hairstyle_51", "male_hairstyle_52",
        "male_hairstyle_53",
        "male_hairstyle_54", "male_hairstyle_55", "male_hairstyle_56", "male_hairstyle_57", "male_hairstyle_58", "male_hairstyle_59",
        "male_hairstyle_60",
        "male_hairstyle_64", "male_hairstyle_65", "male_hairstyle_66", "male_hairstyle_67", "male_hairstyle_68",
        "male_hairstyle_69", "male_hairstyle_70", "male_hairstyle_71", "male_hairstyle_72", "male_hairstyle_73",
        "male_hairstyle_74", "male_hairstyle_75", "male_hairstyle_76", "male_hairstyle_77", "male_hairstyle_78",
        "male_hairstyle_79", "male_hairstyle_80", "male_hairstyle_81", "male_hairstyle_82", "male_hairstyle_83",
        "male_hairstyle_84", "male_hairstyle_85", "male_hairstyle_86", "male_hairstyle_87", "male_hairstyle_88",
        "male_hairstyle_89", "male_hairstyle_90", "male_hairstyle_91", "male_hairstyle_92", "male_hairstyle_93",
        "male_hairstyle_94", "male_hairstyle_95", "male_hairstyle_97", "male_hairstyle_98"
    };

    private readonly IItemFactory _itemFactory;

    public HairstyleScript(Dialog subject, IItemFactory itemFactory)
        : base(subject) => _itemFactory = itemFactory;

    public override void OnDisplaying(Aisling source)
    {
        switch (source.Gender)
        {
            case Gender.Male:
            {
                foreach (var s in MaleHairstyles)
                {
                    var item = _itemFactory.CreateFaux(s);
                    item.Color = source.HairColor;
                    Subject.Items.Add(ItemDetails.BuyWithGold(item));
                }

                break;
            }
            case Gender.Female:
            {
                foreach (var s in FemaleHairstyles)
                {
                    var item = _itemFactory.CreateFaux(s);
                    item.Color = source.HairColor;
                    Subject.Items.Add(ItemDetails.BuyWithGold(item));
                }

                break;
            }
        }
    }

    public override void OnNext(Aisling source, byte? optionIndex = null)
    {
        if (!Subject.MenuArgs.TryGet<string>(0, out var hairStyleName))
        {
            Subject.ReplyToUnknownInput(source);

            return;
        }

        var itemDetails = Subject.Items.FirstOrDefault(x => x.Item.DisplayName.EqualsI(hairStyleName));
        var item = itemDetails?.Item;

        if (item == null)

        {
            Subject.ReplyToUnknownInput(source);

            return;
        }

        if (!source.TryTakeGold(itemDetails!.Price))
        {
            Subject.Close(source);

            return;
        }

        source.HairStyle = item.Template.ItemSprite.DisplaySprite;
        source.Refresh(true);
        source.Display();
    }
}
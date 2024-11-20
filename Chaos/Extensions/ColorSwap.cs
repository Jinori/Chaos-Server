using Chaos.Common.Definitions;
using Chaos.DarkAges.Definitions;

namespace Chaos.Extensions;

public static class ColorSwap
{
    public static DisplayColor ConvertToDisplayColor(this BodyColor value)
    {
        switch (value)
        {
            case BodyColor.White:
                return DisplayColor.White;

            case BodyColor.Pale:
                return DisplayColor.Mustard;

            case BodyColor.Brown:
                return DisplayColor.Brown;

            case BodyColor.Green:
                return DisplayColor.Green;

            case BodyColor.Yellow:
                return DisplayColor.Yellow;

            case BodyColor.Tan:
                return DisplayColor.Tan;

            case BodyColor.Grey:
                return DisplayColor.Gray;

            case BodyColor.LightBlue:
                return DisplayColor.LightBlue;

            case BodyColor.Orange:
                return DisplayColor.Orange;

            case BodyColor.Purple:
                return DisplayColor.Default;

            default:
                return DisplayColor.Mustard;
        }
    }

    public static BodyColor ConvertToBodyColor(this DisplayColor value)
    {
        switch (value)
        {
            case DisplayColor.White:
                return BodyColor.White;

            case DisplayColor.Mustard:
                return BodyColor.Pale;

            case DisplayColor.Brown:
                return BodyColor.Brown;

            case DisplayColor.Green:
                return BodyColor.Green;

            case DisplayColor.Yellow:
                return BodyColor.Yellow;

            case DisplayColor.Tan:
                return BodyColor.Tan;

            case DisplayColor.Gray:
                return BodyColor.Grey;

            case DisplayColor.LightBlue:
                return BodyColor.LightBlue;

            case DisplayColor.Orange:
                return BodyColor.Orange;

            case DisplayColor.Default:
                return BodyColor.Purple;

            default:
                return BodyColor.Pale;
        }
    }
}
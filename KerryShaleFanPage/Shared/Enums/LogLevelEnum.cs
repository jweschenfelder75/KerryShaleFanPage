using KerryShaleFanPage.Shared.Attributes;
using System;
using System.ComponentModel;

namespace KerryShaleFanPage.Shared.Enums
{
    [Serializable]
    public enum LogLevelEnum
    {
        [Description("Critical"), BackColorName("DarkRed"), FrontColorName("White")]
        Critical = 5,

        [Description("Error"), BackColorName("Transparent"), FrontColorName("Red")]
        Error = 4,

        [Description("Warn"), BackColorName("Transparent"), FrontColorName("DarkOrange")]
        Warn = 3,

        [Description("Debug"), BackColorName("Transparent"), FrontColorName("DarkGreen")]
        Debug = 2,

        [Description("Info"), BackColorName("Transparent"), FrontColorName("CornflowerBlue")]
        Info = 1,

        [Description("None"), BackColorName("Transparent"), FrontColorName("Black")]
        None = 0
    }
}

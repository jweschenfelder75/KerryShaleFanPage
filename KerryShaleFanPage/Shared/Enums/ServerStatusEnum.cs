using System;
using System.ComponentModel;
using KerryShaleFanPage.Shared.Attributes;

namespace KerryShaleFanPage.Shared.Enums
{
    [Serializable]
    public enum ServerStatusEnum
    {
        [Description("Critical"), BackColorName("Transparent"), FrontColorName("DarkRed")]
        Critical = 4,

        [Description("Error"), BackColorName("Transparent"), FrontColorName("Red")]
        Error = 3,

        [Description("Warning"), BackColorName("Transparent"), FrontColorName("DarkOrange")]
        Warning = 2,

        [Description("Ok"), BackColorName("Transparent"), FrontColorName("GhostWhite")]
        Ok = 1,

        [Description("None"), BackColorName("Transparent"), FrontColorName("GhostWhite")]
        None = 0
    }
}

using System.ComponentModel;

namespace Fred.Net.Types.Enums
{
    public enum SeriesObservationFrequency
    {
        None,

        [Description("d")]
        Daily,

        [Description("w")]
        Weekly,

        [Description("bw")]
        Biweekly,

        [Description("m")]
        Monthly,

        [Description("q")]
        Quarterly,

        [Description("sa")]
        Semiannual,

        [Description("a")]
        Annual,

        [Description("wef")]
        WeeklyEndingFriday,

        [Description("weth")]
        WeeklyEndingThursday,

        [Description("wew")]
        WeeklyEndingWednesday,

        [Description("wetu")]
        WeeklyEndingTuesday,

        [Description("wem")]
        WeeklyEndingMonday,

        [Description("wesu")]
        WeeklyEndingSunday,

        [Description("wesa")]
        WeeklyEndingSaturday,

        [Description("bwew")]
        BiweeklyEndingWednesday,

        [Description("bwem")]
        BiweeklyEndingMonday,
    }
}
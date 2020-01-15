using System.ComponentModel;

namespace Fred.Net.Enums
{
    public enum SeriesObservationUnit
    {
        [Description("lin")]
        Levels,

        [Description("chg")]
        Change,

        [Description("ch1")]
        ChangeFromYearAgo,

        [Description("pch")]
        PercentChange,

        [Description("pc1")]
        PercentChangeFromYearAgo,

        [Description("pca")]
        CompoundedAnnualRateOfChange,

        [Description("cch")]
        ContinuouslyCompoundedRateOfChange,

        [Description("cca")]
        ContinuouslyCompoundedAnnualRateOfChange,

        [Description("log")]
        NaturalLog,
    }
}
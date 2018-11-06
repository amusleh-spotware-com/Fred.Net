using System.ComponentModel;

namespace Fred.Net.Types.Enums
{
    public enum SeriesFilterVariable
    {
        None,

        [Description("units")]
        Units,

        [Description("frequency")]
        Frequency,

        [Description("seasonal_adjustment")]
        SeasonalAdjustment,
    }
}
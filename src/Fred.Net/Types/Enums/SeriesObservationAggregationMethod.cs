using System.ComponentModel;

namespace Fred.Net.Types.Enums
{
    public enum SeriesObservationAggregationMethod
    {
        [Description("avg")]
        Average,

        [Description("sum")]
        Sum,

        [Description("eop")]
        EndOfPeriod,
    }
}
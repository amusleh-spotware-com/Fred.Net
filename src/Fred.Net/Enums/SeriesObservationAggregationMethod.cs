using System.ComponentModel;

namespace Fred.Net.Enums
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
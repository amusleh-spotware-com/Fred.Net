using System.ComponentModel;

namespace Fred.Net.Enums
{
    public enum SeriesObservationOutputType
    {
        [Description("1")]
        RealTimePeriod,

        [Description("2")]
        VintageDateAllObservations,

        [Description("3")]
        VintageDateNewAndRevisedObservationsOnly,

        [Description("4")]
        InitialReleaseOnly,
    }
}
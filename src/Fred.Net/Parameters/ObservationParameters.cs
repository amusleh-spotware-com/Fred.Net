using System;
using System.Collections.Generic;
using System.Text;
using Fred.Net.Enums;
using Fred.Net.Parameters.Abstractions;

namespace Fred.Net.Parameters
{
    public class ObservationParameters: RealTimeParameters
    {
        public string Id { get; set; }

        public int Limit { get; set; } = 1000;

        public int Offset { get; set; } = 0;

        public SortOrder SortOrder { get; set; } = SortOrder.Ascending;

        public DateTime? Start { get; set; }

        public DateTime? End { get; set; }

        public SeriesObservationUnit Unit { get; set; } = SeriesObservationUnit.Levels;

        public SeriesObservationFrequency Frequency { get; set; } = SeriesObservationFrequency.None;

        public SeriesObservationAggregationMethod AggregationMethod { get; set; } = SeriesObservationAggregationMethod.Average;

        public SeriesObservationOutputType OutputType { get; set; } = SeriesObservationOutputType.RealTimePeriod;

        public List<DateTime> VintageDates { get; set; }
    }
}

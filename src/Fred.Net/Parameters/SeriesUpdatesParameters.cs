using System;
using System.Collections.Generic;
using System.Text;
using Fred.Net.Enums;
using Fred.Net.Parameters.Abstractions;

namespace Fred.Net.Parameters
{
    public class SeriesUpdatesParameters: RealTimeParameters
    {

        public int Limit { get; set; } = 1000;

        public int Offset { get; set; } = 0;

        public SeriesFilterValue FilterValue { get; set; } = SeriesFilterValue.All;

        public DateTime? Start { get; set; }

        public DateTime? End { get; set; }
    }
}

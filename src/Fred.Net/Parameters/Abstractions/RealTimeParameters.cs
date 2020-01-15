using System;

namespace Fred.Net.Parameters.Abstractions
{
    public abstract class RealTimeParameters
    {
        public DateTime? RealTimeStart { get; set; }

        public DateTime? RealTimeEnd { get; set; }
    }
}
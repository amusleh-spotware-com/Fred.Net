using System;
using System.Xml.Serialization;

namespace Fred.Net.Types
{
    [XmlType("observation")]
    public class Observation
    {
        [XmlAttribute("realtime_start")]
        public DateTime RealTimeStart { get; set; }

        [XmlAttribute("realtime_end")]
        public DateTime RealTimeEnd { get; set; }

        [XmlAttribute("date")]
        public DateTime Date { get; set; }

        [XmlAttribute("value")]
        public double Value { get; set; }
    }
}
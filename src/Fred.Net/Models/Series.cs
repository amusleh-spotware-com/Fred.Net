using System;
using System.Globalization;
using System.Xml.Serialization;

namespace Fred.Net.Models
{
    [XmlType("series")]
    public class Series
    {
        [XmlAttribute("id")]
        public string Id { get; set; }

        [XmlAttribute("realtime_start")]
        public DateTime RealtimeStart { get; set; }

        [XmlAttribute("realtime_end")]
        public DateTime RealtimeEnd { get; set; }

        [XmlAttribute("title")]
        public string Title { get; set; }

        [XmlAttribute("observation_start")]
        public DateTime ObservationStart { get; set; }

        [XmlAttribute("observation_end")]
        public DateTime ObservationEnd { get; set; }

        [XmlAttribute("frequency")]
        public string Frequency { get; set; }

        [XmlAttribute("frequency_short")]
        public string FrequencyShort { get; set; }

        [XmlAttribute("units")]
        public string Units { get; set; }

        [XmlAttribute("units_short")]
        public string UnitsShort { get; set; }

        [XmlAttribute("seasonal_adjustment")]
        public string SeasonalAdjustment { get; set; }

        [XmlAttribute("seasonal_adjustment_short")]
        public string SeasonalAdjustmentShort { get; set; }

        [XmlAttribute("last_updated")]
        public string LastUpdatedInString { get; set; }

        [XmlIgnore]
        public DateTimeOffset LastUpdated => DateTimeOffset.ParseExact(LastUpdatedInString, "yyyy-MM-dd hh:mm:ssz", CultureInfo.InvariantCulture);

        [XmlAttribute("popularity")]
        public int Popularity { get; set; }

        [XmlAttribute("group_popularity")]
        public int GroupPopularity { get; set; }

        [XmlAttribute("notes")]
        public string Notes { get; set; }
    }
}
using Fred.Net.Enums;
using System;
using System.Globalization;
using System.Xml.Serialization;
using Fred.Net.Utils;

namespace Fred.Net.Models
{
    [XmlType("tag")]
    public class Tag
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("group_id")]
        public string GroupIdInString { get; set; }

        [XmlIgnore]
        public TagGroupId GroupId => EnumDescription.GetValueFromDescription<TagGroupId>(GroupIdInString);

        [XmlAttribute("notes")]
        public string Notes { get; set; }

        [XmlAttribute("created")]
        public string CreatedInString { get; set; }

        [XmlIgnore]
        public DateTimeOffset Created => DateTimeOffset.ParseExact(CreatedInString, "yyyy-MM-dd hh:mm:ssz", CultureInfo.InvariantCulture);

        [XmlAttribute("popularity")]
        public int Popularity { get; set; }

        [XmlAttribute("series_count")]
        public int SeriesCount { get; set; }
    }
}
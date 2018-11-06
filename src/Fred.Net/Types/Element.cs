using System.Collections.Generic;
using System.Xml.Serialization;

namespace Fred.Net.Types
{
    [XmlType("element")]
    public class Element
    {
        [XmlElement("name")]
        public string Name { get; set; }

        [XmlElement("element_id")]
        public int ElementId { get; set; }

        [XmlElement("release_id")]
        public int ReleaseId { get; set; }

        [XmlElement("series_id")]
        public string SeriesId { get; set; }

        [XmlElement("parent_id")]
        public string ParentId { get; set; }

        [XmlElement("line")]
        public string Line { get; set; }

        [XmlElement("type")]
        public string Type { get; set; }

        [XmlElement("level")]
        public int Level { get; set; }

        [XmlArray("children")]
        public List<Element> Children { get; set; }
    }
}
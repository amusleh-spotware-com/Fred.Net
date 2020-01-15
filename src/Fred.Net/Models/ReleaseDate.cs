using System;
using System.Xml.Serialization;

namespace Fred.Net.Models
{
    [XmlType("release_date")]
    public class ReleaseDate
    {
        [XmlAttribute("release_id")]
        public int Id { get; set; }

        [XmlAttribute("release_name")]
        public string Name { get; set; }

        [XmlText]
        public DateTime Date { get; set; }
    }
}
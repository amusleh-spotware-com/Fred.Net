using System;
using System.Xml.Serialization;

namespace Fred.Net.Models
{
    [XmlType("release")]
    public class Release
    {
        [XmlAttribute("id")]
        public int Id { get; set; }

        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("realtime_start")]
        public DateTime RealtimeStart { get; set; }

        [XmlAttribute("realtime_end")]
        public DateTime RealtimeEnd { get; set; }

        [XmlAttribute("press_release")]
        public bool PressRelease { get; set; }

        [XmlAttribute("link")]
        public string Link { get; set; }
    }
}
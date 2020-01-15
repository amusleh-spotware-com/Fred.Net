using System;
using System.Xml.Serialization;

namespace Fred.Net.Models
{
    [XmlType("vintage_date")]
    public class VintageDate
    {
        [XmlText]
        public DateTime Date { get; set; }
    }
}
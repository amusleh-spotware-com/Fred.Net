using System;
using System.Xml.Serialization;

namespace Fred.Net.Types
{
    [XmlType("vintage_date")]
    public class VintageDate
    {
        [XmlText]
        public DateTime Date { get; set; }
    }
}
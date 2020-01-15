using System.Xml.Serialization;

namespace Fred.Net.Models
{
    [XmlType("category")]
    public class Category
    {
        [XmlAttribute("id")]
        public int Id { get; set; }

        [XmlAttribute("parent_id")]
        public int ParentId { get; set; }

        [XmlAttribute("name")]
        public string Name { get; set; }
    }
}
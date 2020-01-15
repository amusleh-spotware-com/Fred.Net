using System;
using System.Collections.Generic;
using System.Text;
using Fred.Net.Enums;

namespace Fred.Net.Parameters
{
    public class RelatedTagsParameters : TagsParameters
    {
        public List<string> ExcludeTags { get; set; }
    }
}

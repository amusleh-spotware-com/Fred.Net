using System;
using System.Collections.Generic;
using System.Text;
using Fred.Net.Enums;

namespace Fred.Net.Parameters
{
    public class RelatedTagParameters: TagParameters
    {
        public List<string> ExcludeTags { get; set; }
    }
}

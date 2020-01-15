using System;
using System.Collections.Generic;
using System.Text;
using Fred.Net.Enums;
using Fred.Net.Parameters.Abstractions;

namespace Fred.Net.Parameters
{
    public class RelatedTagSearchParameters: RealTimeParameters
    {
        public string SeriesSearchText { get; set; }

        public string TagSearchText { get; set; }

        public int Limit { get; set; } = 1000;

        public int Offset { get; set; } = 0;

        public TagOrderBy OrderBy { get; set; } = TagOrderBy.SeriesCount;

        public SortOrder SortOrder { get; set; } = SortOrder.Ascending;

        public TagGroupId GroupId { get; set; } = TagGroupId.None;

        public List<string> Tags { get; set; }

        public List<string> ExcludeTags { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using Fred.Net.Enums;
using Fred.Net.Parameters.Abstractions;

namespace Fred.Net.Parameters
{
    public class SeriesTagsParameters: RealTimeParameters
    {
        public string SeriesId { get; set; }

        public TagOrderBy OrderBy { get; set; } = TagOrderBy.SeriesCount;

        public SortOrder SortOrder { get; set; } = SortOrder.Ascending;
    }
}

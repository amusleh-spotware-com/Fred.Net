using System;
using System.Collections.Generic;
using System.Text;
using Fred.Net.Enums;
using Fred.Net.Parameters.Abstractions;

namespace Fred.Net.Parameters
{
    public class SeriesSearchParameters: RealTimeParameters
    {
        public string SearchText { get; set; }

        public SeriesSearchType SearchType { get; set; } = SeriesSearchType.FullText;

        public int Limit { get; set; } = 1000;

        public int Offset { get; set; } = 0;

        public SeriesSearchOrderBy OrderBy { get; set; } = SeriesSearchOrderBy.None;

        public SortOrder SortOrder { get; set; } = SortOrder.Ascending;

        public SeriesFilterVariable FilterVariable { get; set; } = SeriesFilterVariable.None;

        public string FilterValue { get; set; }

        public List<string> Tags { get; set; }

        public List<string> ExcludeTags { get; set; }
    }
}

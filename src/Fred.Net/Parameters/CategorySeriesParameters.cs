using System;
using System.Collections.Generic;
using System.Text;
using Fred.Net.Enums;
using Fred.Net.Parameters.Abstractions;

namespace Fred.Net.Parameters
{
    public class CategorySeriesParameters: RealTimeParameters
    {
        public int Id { get; set; }

        public int Limit { get; set; } = 1000;

        public int Offset { get; set; } = 0;

        public SeriesOrderBy OrderBy { get; set; } = SeriesOrderBy.Id;

        public SortOrder SortOrder { get; set; } = SortOrder.Ascending;

        public SeriesFilterVariable FilterVariable { get; set; } = SeriesFilterVariable.None;

        public string FilterValue { get; set; }

        public List<string> Tags { get; set; }

        public List<string> ExcludeTags { get; set; }
    }
}

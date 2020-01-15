using System;
using System.Collections.Generic;
using System.Text;
using Fred.Net.Enums;
using Fred.Net.Parameters.Abstractions;

namespace Fred.Net.Parameters
{
    public class ReleasesParameters: RealTimeParameters
    {
        public int Limit { get; set; } = 1000;

        public int Offset { get; set; } = 0;

        public ReleaseOrderBy OrderBy { get; set; } = ReleaseOrderBy.Id;

        public SortOrder SortOrder { get; set; } = SortOrder.Ascending;
    }
}

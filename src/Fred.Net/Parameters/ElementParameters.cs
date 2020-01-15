using System;
using System.Collections.Generic;
using System.Text;

namespace Fred.Net.Parameters
{
    public class ElementParameters
    {
        public int Id { get; set; }

        public int? ElementId { get; set; }

        public bool IncludeObservationValues { get; set; }

        public DateTime? ObservationDate { get; set; }
    }
}

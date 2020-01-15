using System;
using System.Collections.Generic;
using System.Text;
using Fred.Net.Enums;
using Fred.Net.Parameters.Abstractions;

namespace Fred.Net.Parameters
{
    public class ReleaseDatesParameters : ReleasesDatesParameters
    {
        public int Id { get; set; }
    }
}

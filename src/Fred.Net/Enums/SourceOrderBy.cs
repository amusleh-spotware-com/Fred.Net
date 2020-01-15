using System.ComponentModel;

namespace Fred.Net.Enums
{
    public enum SourceOrderBy
    {
        [Description("source_id")]
        Id,

        [Description("name")]
        Name,

        [Description("realtime_start")]
        RealtimeStart,

        [Description("realtime_end")]
        RealtimeEnd,
    }
}
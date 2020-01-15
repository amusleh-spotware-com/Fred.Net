using System.ComponentModel;

namespace Fred.Net.Enums
{
    public enum ReleaseOrderBy
    {
        [Description("release_id")]
        Id,

        [Description("name")]
        Name,

        [Description("press_release")]
        PressRelease,

        [Description("realtime_start")]
        RealtimeStart,

        [Description("realtime_end")]
        RealtimeEnd,
    }
}
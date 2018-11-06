using System.ComponentModel;

namespace Fred.Net.Types.Enums
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
        RealTimeStart,

        [Description("realtime_end")]
        RealTimeEnd,
    }
}
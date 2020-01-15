using System.ComponentModel;

namespace Fred.Net.Enums
{
    public enum ReleaseDateOrderBy
    {
        [Description("release_id")]
        Id,

        [Description("release_name")]
        Name,

        [Description("release_date")]
        Date,
    }
}
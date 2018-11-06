using System.ComponentModel;

namespace Fred.Net.Types.Enums
{
    public enum SeriesSearchType
    {
        [Description("full_text")]
        FullText,

        [Description("series_id")]
        SeriesId,
    }
}
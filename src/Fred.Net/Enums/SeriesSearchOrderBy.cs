using System.ComponentModel;

namespace Fred.Net.Enums
{
    public enum SeriesSearchOrderBy
    {
        None,

        [Description("search_rank")]
        SearchRank,

        [Description("series_id")]
        Id,

        [Description("title")]
        Title,

        [Description("units")]
        Units,

        [Description("frequency")]
        Frequency,

        [Description("seasonal_adjustment")]
        SeasonalAdjustment,

        [Description("realtime_start")]
        RealtimeStart,

        [Description("realtime_end")]
        RealtimeEnd,

        [Description("last_updated")]
        LastUpdated,

        [Description("observation_start")]
        ObservationStart,

        [Description("observation_end")]
        ObservationEnd,

        [Description("popularity")]
        Popularity,

        [Description("group_popularity")]
        GroupPopularity
    }
}
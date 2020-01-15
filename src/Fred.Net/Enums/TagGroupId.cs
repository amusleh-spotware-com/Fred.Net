using System.ComponentModel;

namespace Fred.Net.Enums
{
    public enum TagGroupId
    {
        None,

        [Description("freq")]
        Frequency,

        [Description("gen")]
        General,

        [Description("geo")]
        Geography,

        [Description("geot")]
        GeographyType,

        [Description("rls")]
        Release,

        [Description("seas")]
        SeasonalAdjustment,

        [Description("src")]
        Source
    }
}
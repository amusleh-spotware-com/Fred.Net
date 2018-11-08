using Fred.Net.Types;
using Fred.Net.Types.Enums;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Xml;

namespace Fred.Net
{
    public class Client : IDisposable
    {
        #region Fields

        /// <summary>
        /// It's the base URL for all requests
        /// </summary>
        private const string _baseUrl = "https://api.stlouisfed.org/fred/";

        /// <summary>
        /// The provided API key storage field
        /// </summary>
        private readonly string _apiKey;

        /// <summary>
        /// This web client object will be used during lift time of client object
        /// </summary>
        private readonly WebClient _webClient;

        #endregion Fields

        /// <summary>
        /// Creates an instance of Client class and initializes a web client which is responsible for sending web requests
        /// </summary>
        /// <param name="apiKey">Your Fred API key</param>
        public Client(string apiKey)
        {
            _apiKey = apiKey;

            _webClient = new WebClient();

            _webClient.BaseAddress = _baseUrl;
        }

        #region Properties

        /// <summary>
        /// Returns your provided API key during initialization
        /// </summary>
        public string ApiKey => _apiKey;

        #endregion Properties

        #region Categories

        /// <summary>
        /// Get a category.
        /// https://research.stlouisfed.org/docs/api/fred/category.html
        /// </summary>
        /// <param name="id">The id for a category, default: 0 (root category)</param>
        /// <returns>Category</returns>
        public async Task<Category> GetCategory(int id)
        {
            string url = $"category";

            NameValueCollection query = new NameValueCollection
            {
                { "category_id", id.ToString() },
            };

            XmlDocument xmlDocument = await Request(url, query);

            Category result = Utility.Deserialize<Category>(xmlDocument.DocumentElement.InnerXml);

            return result;
        }

        /// <summary>
        /// Get the child categories for a specified parent category.
        /// https://research.stlouisfed.org/docs/api/fred/category_children.html
        /// </summary>
        /// <param name="id">The id for a category, default: 0 (root category)</param>
        /// <param name="realtimeStart">The start of the real-time period, optional, default: today's date</param>
        /// <param name="realtimeEnd">The end of the real-time period, optional, default: today's date</param>
        /// <returns>List<Category></returns>
        public async Task<List<Category>> GetCategoryChildren(int id, DateTime? realtimeStart = null, DateTime? realtimeEnd = null)
        {
            string url = $"category/children";

            NameValueCollection query = new NameValueCollection
            {
                { "category_id", id.ToString() },
            };

            if (realtimeStart.HasValue)
            {
                query.Add("realtime_start", Utility.FormatDate(realtimeStart.Value));
            }

            if (realtimeEnd.HasValue)
            {
                query.Add("realtime_end", Utility.FormatDate(realtimeEnd.Value));
            }

            XmlDocument xmlDocument = await Request(url, query);

            List<Category> result = new List<Category>();

            foreach (XmlNode xmlNode in xmlDocument.DocumentElement.ChildNodes)
            {
                Category category = Utility.Deserialize<Category>(xmlNode.OuterXml);

                result.Add(category);
            }

            return result;
        }

        /// <summary>
        /// Get the related categories for a category.
        /// A related category is a one-way relation between 2 categories that is not part of a parent-child category hierarchy.
        /// Most categories do not have related categories.
        /// https://research.stlouisfed.org/docs/api/fred/category_related.html
        /// </summary>
        /// <param name="id">The id for a category.</param>
        /// <param name="realtimeStart">The start of the real-time period, optional, default: today's date</param>
        /// <param name="realtimeEnd">The end of the real-time period, optional, default: today's date</param>
        /// <returns>List<Category></returns>
        public async Task<List<Category>> GetCategoryRelated(int id, DateTime? realtimeStart = null, DateTime? realtimeEnd = null)
        {
            string url = $"category/related";

            NameValueCollection query = new NameValueCollection
            {
                { "category_id", id.ToString() },
            };

            if (realtimeStart.HasValue)
            {
                query.Add("realtime_start", Utility.FormatDate(realtimeStart.Value));
            }

            if (realtimeEnd.HasValue)
            {
                query.Add("realtime_end", Utility.FormatDate(realtimeEnd.Value));
            }

            XmlDocument xmlDocument = await Request(url, query);

            List<Category> result = new List<Category>();

            foreach (XmlNode xmlNode in xmlDocument.DocumentElement.ChildNodes)
            {
                Category category = Utility.Deserialize<Category>(xmlNode.OuterXml);

                result.Add(category);
            }

            return result;
        }

        /// <summary>
        /// Get the series in a category.
        /// https://research.stlouisfed.org/docs/api/fred/category_series.html
        /// </summary>
        /// <param name="id">The id for a category.</param>
        /// <param name="realtimeStart">The start of the real-time period, optional, default: today's date</param>
        /// <param name="realtimeEnd">The end of the real-time period, optional, default: today's date</param>
        /// <param name="limit">The maximum number of results to return, between 1 and 1000, optional, default: 1000</param>
        /// <param name="offset">non-negative integer, optional, default: 0</param>
        /// <param name="orderBy">Order results by values of the specified attribute, optional, default: Id</param>
        /// <param name="sortOrder">Sort results is ascending or descending order, optional, default: Ascending</param>
        /// <param name="filterVariable">The attribute to filter results by, optional, no filter by default</param>
        /// <param name="filterValue">The value of the filter_variable attribute to filter results by, optional, no filter by default</param>
        /// <param name="tags">A list of tag names that series match all of, optional, no filtering by tags by default</param>
        /// <param name="excludeTags">A list of tag names that series match none of, it requires that parameter tags also be set to limit the number of matching series, optional, no filtering by tags by default.</param>
        /// <returns>List<Series></returns>
        public async Task<List<Series>> GetCategorySeries(int id, DateTime? realtimeStart = null, DateTime? realtimeEnd = null, int limit = 1000,
            int offset = 0, SeriesOrderBy orderBy = SeriesOrderBy.Id, SortOrder sortOrder = SortOrder.Ascending,
            SeriesFilterVariable filterVariable = SeriesFilterVariable.None, string filterValue = null, List<string> tags = null,
            List<string> excludeTags = null)
        {
            string url = $"category/series";

            NameValueCollection query = new NameValueCollection
            {
                {"category_id", id.ToString() },
                {"limit", limit.ToString() },
                {"offset", offset.ToString() },
                {"order_by", Utility.GetDescription(orderBy) },
                {"sort_order", Utility.GetDescription(sortOrder) }
            };

            if (realtimeStart.HasValue)
            {
                query.Add("realtime_start", Utility.FormatDate(realtimeStart.Value));
            }

            if (realtimeEnd.HasValue)
            {
                query.Add("realtime_end", Utility.FormatDate(realtimeEnd.Value));
            }

            if (filterVariable != SeriesFilterVariable.None)
            {
                query.Add("filter_variable", Utility.GetDescription(filterVariable));
                query.Add("filter_value", filterValue);
            }

            if (tags != null && tags.Any())
            {
                query.Add("tag_names", Utility.GetStringSeparatedBySemicolon(tags));
            }

            if (excludeTags != null && excludeTags.Any())
            {
                query.Add("exclude_tag_names", Utility.GetStringSeparatedBySemicolon(excludeTags));
            }

            XmlDocument xmlDocument = await Request(url, query);

            List<Series> result = new List<Series>();

            foreach (XmlNode xmlNode in xmlDocument.DocumentElement.ChildNodes)
            {
                Series series = Utility.Deserialize<Series>(xmlNode.OuterXml);

                result.Add(series);
            }

            return result;
        }

        /// <summary>
        /// Get the FRED tags for a category.
        /// Optionally, filter results by tag name, tag group, or search.
        /// Series are assigned tags and categories.
        /// Indirectly through series, it is possible to get the tags for a category.
        /// No tags exist for a category that does not have series.
        /// https://research.stlouisfed.org/docs/api/fred/category_tags.html
        /// </summary>
        /// <param name="id">The id for a category.</param>
        /// <param name="realtimeStart">The start of the real-time period, optional, default: today's date</param>
        /// <param name="realtimeEnd">The end of the real-time period, optional, default: today's date</param>
        /// <param name="limit">The maximum number of results to return, between 1 and 1000, optional, default: 1000</param>
        /// <param name="offset">non-negative integer, optional, default: 0</param>
        /// <param name="orderBy">Order results by values of the specified attribute, optional, default: Id</param>
        /// <param name="sortOrder">Sort results is ascending or descending order, optional, default: Ascending</param>
        /// <param name="searchText">The words to find matching tags with, optional, no filtering by search words by default.</param>
        /// <param name="tagGroupId">A tag group id to filter tags by type, optional, no filtering by tag group by default.</param>
        /// <param name="tags">A list of tag names to only include in the response, optional, no filtering by tags by default</param>
        /// <returns>List<Tag></returns>
        public async Task<List<Tag>> GetCategoryTags(int id, DateTime? realtimeStart = null, DateTime? realtimeEnd = null, int limit = 1000,
            int offset = 0, TagOrderBy orderBy = TagOrderBy.SeriesCount, SortOrder sortOrder = SortOrder.Ascending, string searchText = null,
            TagGroupId tagGroupId = TagGroupId.None, List<string> tags = null)
        {
            string url = $"category/tags";

            NameValueCollection query = new NameValueCollection
            {
                {"category_id", id.ToString() },
                {"limit", limit.ToString() },
                {"offset", offset.ToString() },
                {"order_by", Utility.GetDescription(orderBy) },
                {"sort_order", Utility.GetDescription(sortOrder) }
            };

            if (realtimeStart.HasValue)
            {
                query.Add("realtime_start", Utility.FormatDate(realtimeStart.Value));
            }

            if (realtimeEnd.HasValue)
            {
                query.Add("realtime_end", Utility.FormatDate(realtimeEnd.Value));
            }

            if (!string.IsNullOrEmpty(searchText))
            {
                query.Add("search_text", searchText);
            }

            if (tagGroupId != TagGroupId.None)
            {
                query.Add("tag_group_id", Utility.GetDescription(tagGroupId));
            }

            if (tags != null && tags.Any())
            {
                query.Add("tag_names", Utility.GetStringSeparatedBySemicolon(tags));
            }

            XmlDocument xmlDocument = await Request(url, query);

            List<Tag> result = new List<Tag>();

            foreach (XmlNode xmlNode in xmlDocument.DocumentElement.ChildNodes)
            {
                Tag tag = Utility.Deserialize<Tag>(xmlNode.OuterXml);

                result.Add(tag);
            }

            return result;
        }

        /// <summary>
        /// Get the related FRED tags for one or more FRED tags within a category. Optionally, filter results by tag group or search.
        /// https://research.stlouisfed.org/docs/api/fred/category_related_tags.html
        /// </summary>
        /// <param name="id">The id for a category.</param>
        /// <param name="tags">A list of tag names that series match all of, optional, no filtering by tags by default</param>
        /// <param name="realtimeStart">The start of the real-time period, optional, default: today's date</param>
        /// <param name="realtimeEnd">The end of the real-time period, optional, default: today's date</param>
        /// <param name="limit">The maximum number of results to return, between 1 and 1000, optional, default: 1000</param>
        /// <param name="offset">non-negative integer, optional, default: 0</param>
        /// <param name="orderBy">Order results by values of the specified attribute, optional, default: Id</param>
        /// <param name="sortOrder">Sort results is ascending or descending order, optional, default: Ascending</param>
        /// <param name="searchText">The words to find matching tags with, optional, no filtering by search words by default.</param>
        /// <param name="tagGroupId">A tag group id to filter tags by type, optional, no filtering by tag group by default.</param>
        /// <param name="excludeTags">A list of tag names that series match none of, optional, no default value.</param>
        /// <returns>List<Tag></returns>
        public async Task<List<Tag>> GetCategoryRelatedTags(int id, List<string> tags, DateTime? realtimeStart = null,
            DateTime? realtimeEnd = null, int limit = 1000, int offset = 0, TagOrderBy orderBy = TagOrderBy.SeriesCount,
            SortOrder sortOrder = SortOrder.Ascending, string searchText = null, TagGroupId tagGroupId = TagGroupId.None,
            List<string> excludeTags = null)
        {
            string url = $"category/related_tags";

            NameValueCollection query = new NameValueCollection
            {
                {"category_id", id.ToString() },
                {"tag_names", Utility.GetStringSeparatedBySemicolon(tags)},
                {"limit", limit.ToString() },
                {"offset", offset.ToString() },
                {"order_by", Utility.GetDescription(orderBy) },
                {"sort_order", Utility.GetDescription(sortOrder) }
            };

            if (realtimeStart.HasValue)
            {
                query.Add("realtime_start", Utility.FormatDate(realtimeStart.Value));
            }

            if (realtimeEnd.HasValue)
            {
                query.Add("realtime_end", Utility.FormatDate(realtimeEnd.Value));
            }

            if (!string.IsNullOrEmpty(searchText))
            {
                query.Add("search_text", searchText);
            }

            if (tagGroupId != TagGroupId.None)
            {
                query.Add("tag_group_id", Utility.GetDescription(tagGroupId));
            }

            if (excludeTags != null && excludeTags.Any())
            {
                query.Add("exclude_tag_names", Utility.GetStringSeparatedBySemicolon(excludeTags));
            }

            XmlDocument xmlDocument = await Request(url, query);

            List<Tag> result = new List<Tag>();

            foreach (XmlNode xmlNode in xmlDocument.DocumentElement.ChildNodes)
            {
                Tag tag = Utility.Deserialize<Tag>(xmlNode.OuterXml);

                result.Add(tag);
            }

            return result;
        }

        #endregion Categories

        #region Releases

        /// <summary>
        /// Get all releases of economic data.
        /// https://research.stlouisfed.org/docs/api/fred/releases.html
        /// </summary>
        /// <param name="realtimeStart">The start of the real-time period, optional, default: today's date</param>
        /// <param name="realtimeEnd">The end of the real-time period, optional, default: today's date</param>
        /// <param name="limit">The maximum number of results to return, between 1 and 1000, optional, default: 1000</param>
        /// <param name="offset">non-negative integer, optional, default: 0</param>
        /// <param name="orderBy">Order results by values of the specified attribute, optional, default: Id</param>
        /// <param name="sortOrder">Sort results is ascending or descending order, optional, default: asc</param>
        /// <returns>List<Release></returns>
        public async Task<List<Release>> GetReleases(DateTime? realtimeStart = null, DateTime? realtimeEnd = null, int limit = 1000,
            int offset = 0, ReleaseOrderBy orderBy = ReleaseOrderBy.Id, SortOrder sortOrder = SortOrder.Ascending)
        {
            string url = $"releases";

            NameValueCollection query = new NameValueCollection
            {
                {"limit", limit.ToString() },
                {"offset", offset.ToString() },
                {"order_by", Utility.GetDescription(orderBy) },
                {"sort_order", Utility.GetDescription(sortOrder) }
            };

            if (realtimeStart.HasValue)
            {
                query.Add("realtime_start", Utility.FormatDate(realtimeStart.Value));
            }

            if (realtimeEnd.HasValue)
            {
                query.Add("realtime_end", Utility.FormatDate(realtimeEnd.Value));
            }

            XmlDocument xmlDocument = await Request(url, query);

            List<Release> result = new List<Release>();

            foreach (XmlNode xmlNode in xmlDocument.DocumentElement.ChildNodes)
            {
                Release release = Utility.Deserialize<Release>(xmlNode.OuterXml);

                result.Add(release);
            }

            return result;
        }

        /// <summary>
        /// Get release dates for all releases of economic data.
        /// https://research.stlouisfed.org/docs/api/fred/releases_dates.html
        /// </summary>
        /// <param name="realtimeStart">The start of the real-time period, optional, default: First day of the current year</param>
        /// <param name="realtimeEnd">The end of the real-time period, optional, default: Latest available</param>
        /// <param name="limit">The maximum number of results to return, between 1 and 1000, optional, default: 1000</param>
        /// <param name="offset">non-negative integer, optional, default: 0</param>
        /// <param name="orderBy">Order results by values of the specified attribute, optional, default: Date</param>
        /// <param name="sortOrder">Sort results is ascending or descending order, optional, default: asc</param>
        /// <param name="includeReleaseDatesWithNoData">Determines whether release dates with no data available are returned, optional, default: false</param>
        /// <returns>List<Release></returns>
        public async Task<List<ReleaseDate>> GetReleasesDates(DateTime? realtimeStart = null, DateTime? realtimeEnd = null, int limit = 1000,
            int offset = 0, ReleaseDateOrderBy orderBy = ReleaseDateOrderBy.Date, SortOrder sortOrder = SortOrder.Ascending,
            bool includeReleaseDatesWithNoData = false)
        {
            string url = $"releases/dates";

            NameValueCollection query = new NameValueCollection
            {
                {"limit", limit.ToString() },
                {"offset", offset.ToString() },
                {"order_by", Utility.GetDescription(orderBy) },
                {"sort_order", Utility.GetDescription(sortOrder) },
                {"include_release_dates_with_no_data", includeReleaseDatesWithNoData.ToString().ToLowerInvariant() }
            };

            if (realtimeStart.HasValue)
            {
                query.Add("realtime_start", Utility.FormatDate(realtimeStart.Value));
            }

            if (realtimeEnd.HasValue)
            {
                query.Add("realtime_end", Utility.FormatDate(realtimeEnd.Value));
            }

            XmlDocument xmlDocument = await Request(url, query);

            List<ReleaseDate> result = new List<ReleaseDate>();

            foreach (XmlNode xmlNode in xmlDocument.DocumentElement.ChildNodes)
            {
                ReleaseDate releaseDate = Utility.Deserialize<ReleaseDate>(xmlNode.OuterXml);

                result.Add(releaseDate);
            }

            return result;
        }

        /// <summary>
        /// Get a release of economic data.
        /// https://research.stlouisfed.org/docs/api/fred/release.html
        /// </summary>
        /// <param name="id">The id for a release.</param>
        /// <param name="realtimeStart">The start of the real-time period, optional, default: today's date</param>
        /// <param name="realtimeEnd">The end of the real-time period, optional, default: today's date</param>
        /// <returns>Release</returns>
        public async Task<Release> GetRelease(int id, DateTime? realtimeStart = null, DateTime? realtimeEnd = null)
        {
            string url = $"release";

            NameValueCollection query = new NameValueCollection
            {
                {"release_id", id.ToString() }
            };

            if (realtimeStart.HasValue)
            {
                query.Add("realtime_start", Utility.FormatDate(realtimeStart.Value));
            }

            if (realtimeEnd.HasValue)
            {
                query.Add("realtime_end", Utility.FormatDate(realtimeEnd.Value));
            }

            XmlDocument xmlDocument = await Request(url, query);

            Release result = Utility.Deserialize<Release>(xmlDocument.DocumentElement.InnerXml);

            return result;
        }

        /// <summary>
        /// Get release dates for a release of economic data.
        /// https://research.stlouisfed.org/docs/api/fred/release_dates.html
        /// </summary>
        /// <param name="id">The id for a release.</param>
        /// <param name="realtimeStart">The start of the real-time period, optional, default: Earliest available</param>
        /// <param name="realtimeEnd">The end of the real-time period, optional, default: Latest available</param>
        /// <param name="limit">The maximum number of results to return, between 1 and 1000, optional, default: 1000</param>
        /// <param name="offset">non-negative integer, optional, default: 0</param>
        /// <param name="orderBy">Order results by values of the specified attribute, optional, default: Date</param>
        /// <param name="sortOrder">Sort results is ascending or descending order, optional, default: asc</param>
        /// <param name="includeReleaseDatesWithNoData">Determines whether release dates with no data available are returned, optional, default: false</param>
        /// <returns>List<Release></returns>
        public async Task<List<ReleaseDate>> GetReleaseDates(int id, DateTime? realtimeStart = null, DateTime? realtimeEnd = null,
            int limit = 10000, int offset = 0, ReleaseDateOrderBy orderBy = ReleaseDateOrderBy.Date, SortOrder sortOrder = SortOrder.Ascending,
            bool includeReleaseDatesWithNoData = false)
        {
            string url = $"release/dates";

            NameValueCollection query = new NameValueCollection
            {
                {"release_id", id.ToString() },
                {"limit", limit.ToString() },
                {"offset", offset.ToString() },
                {"order_by", Utility.GetDescription(orderBy) },
                {"sort_order", Utility.GetDescription(sortOrder) },
                {"include_release_dates_with_no_data", includeReleaseDatesWithNoData.ToString().ToLowerInvariant() }
            };

            if (realtimeStart.HasValue)
            {
                query.Add("realtime_start", Utility.FormatDate(realtimeStart.Value));
            }

            if (realtimeEnd.HasValue)
            {
                query.Add("realtime_end", Utility.FormatDate(realtimeEnd.Value));
            }

            XmlDocument xmlDocument = await Request(url, query);

            List<ReleaseDate> result = new List<ReleaseDate>();

            foreach (XmlNode xmlNode in xmlDocument.DocumentElement.ChildNodes)
            {
                ReleaseDate releaseDate = Utility.Deserialize<ReleaseDate>(xmlNode.OuterXml);

                result.Add(releaseDate);
            }

            return result;
        }

        /// <summary>
        /// Get a release of economic data.
        /// https://research.stlouisfed.org/docs/api/fred/release_series.html
        /// </summary>
        /// <param name="id">The id for a release.</param>
        /// <param name="realtimeStart">The start of the real-time period, optional, default: today's date</param>
        /// <param name="realtimeEnd">The end of the real-time period, optional, default: today's date</param>
        /// <param name="limit">The maximum number of results to return, between 1 and 1000, optional, default: 1000</param>
        /// <param name="offset">non-negative integer, optional, default: 0</param>
        /// <param name="orderBy">Order results by values of the specified attribute, optional, default: Id</param>
        /// <param name="sortOrder">Sort results is ascending or descending order, optional, default: asc</param>
        /// <param name="filterVariable">The attribute to filter results by, optional, no filter by default</param>
        /// <param name="filterValue">The value of the filter_variable attribute to filter results by, optional, no filter by default</param>
        /// <param name="tags">A list of tag names that series match all of, optional, no filtering by tags by default</param>
        /// <param name="excludeTags">A list of tag names that series match none of, it requires that parameter tags also be set to limit the number of matching series, optional, no filtering by tags by default.</param>
        /// <returns>Release</returns>
        public async Task<List<Series>> GetReleaseSeries(int id, DateTime? realtimeStart = null, DateTime? realtimeEnd = null, int limit = 1000,
            int offset = 0, SeriesOrderBy orderBy = SeriesOrderBy.Id, SortOrder sortOrder = SortOrder.Ascending,
            SeriesFilterVariable filterVariable = SeriesFilterVariable.None, string filterValue = null, List<string> tags = null,
            List<string> excludeTags = null)
        {
            string url = $"release/series";

            NameValueCollection query = new NameValueCollection
            {
                {"release_id", id.ToString() },
                {"limit", limit.ToString() },
                {"offset", offset.ToString() },
                {"order_by", Utility.GetDescription(orderBy) },
                {"sort_order", Utility.GetDescription(sortOrder) }
            };

            if (realtimeStart.HasValue)
            {
                query.Add("realtime_start", Utility.FormatDate(realtimeStart.Value));
            }

            if (realtimeEnd.HasValue)
            {
                query.Add("realtime_end", Utility.FormatDate(realtimeEnd.Value));
            }

            if (filterVariable != SeriesFilterVariable.None)
            {
                query.Add("filter_variable", Utility.GetDescription(filterVariable));
                query.Add("filter_value", filterValue);
            }

            if (tags != null && tags.Any())
            {
                query.Add("tag_names", Utility.GetStringSeparatedBySemicolon(tags));
            }

            if (excludeTags != null && excludeTags.Any())
            {
                query.Add("exclude_tag_names", Utility.GetStringSeparatedBySemicolon(excludeTags));
            }

            XmlDocument xmlDocument = await Request(url, query);

            List<Series> result = new List<Series>();

            foreach (XmlNode xmlNode in xmlDocument.DocumentElement.ChildNodes)
            {
                Series series = Utility.Deserialize<Series>(xmlNode.OuterXml);

                result.Add(series);
            }

            return result;
        }

        /// <summary>
        /// Get the sources for a release of economic data.
        /// https://research.stlouisfed.org/docs/api/fred/release_sources.html
        /// </summary>
        /// <param name="id">The id for a release.</param>
        /// <param name="realtimeStart">The start of the real-time period, optional, default: today's date</param>
        /// <param name="realtimeEnd">The end of the real-time period, optional, default: today's date</param>
        /// <returns>Release</returns>
        public async Task<List<Source>> GetReleaseSources(int id, DateTime? realtimeStart = null, DateTime? realtimeEnd = null)
        {
            string url = $"release/sources";

            NameValueCollection query = new NameValueCollection
            {
                {"release_id", id.ToString() }
            };

            if (realtimeStart.HasValue)
            {
                query.Add("realtime_start", Utility.FormatDate(realtimeStart.Value));
            }

            if (realtimeEnd.HasValue)
            {
                query.Add("realtime_end", Utility.FormatDate(realtimeEnd.Value));
            }

            XmlDocument xmlDocument = await Request(url, query);

            List<Source> result = new List<Source>();

            foreach (XmlNode xmlNode in xmlDocument.DocumentElement.ChildNodes)
            {
                Source source = Utility.Deserialize<Source>(xmlNode.OuterXml);

                result.Add(source);
            }

            return result;
        }

        /// <summary>
        /// Get the FRED tags for a release. Optionally, filter results by tag name, tag group, or search. Series are assigned tags and releases.
        /// https://research.stlouisfed.org/docs/api/fred/release_tags.html
        /// </summary>
        /// <param name="id">The id for a release.</param>
        /// <param name="realtimeStart">The start of the real-time period, optional, default: today's date</param>
        /// <param name="realtimeEnd">The end of the real-time period, optional, default: today's date</param>
        /// <param name="limit">The maximum number of results to return, between 1 and 1000, optional, default: 1000</param>
        /// <param name="offset">non-negative integer, optional, default: 0</param>
        /// <param name="orderBy">Order results by values of the specified attribute, optional, default: Id</param>
        /// <param name="sortOrder">Sort results is ascending or descending order, optional, default: Ascending</param>
        /// <param name="searchText">The words to find matching tags with, optional, no filtering by search words by default.</param>
        /// <param name="tagGroupId">A tag group id to filter tags by type, optional, no filtering by tag group by default.</param>
        /// <param name="tags">A list of tag names to only include in the response, optional, no filtering by tags by default</param>
        /// <returns>List<Tag></returns>
        public async Task<List<Tag>> GetReleaseTags(int id, DateTime? realtimeStart = null, DateTime? realtimeEnd = null, int limit = 1000,
            int offset = 0, TagOrderBy orderBy = TagOrderBy.SeriesCount, SortOrder sortOrder = SortOrder.Ascending, string searchText = null,
            TagGroupId tagGroupId = TagGroupId.None, List<string> tags = null)
        {
            string url = $"release/tags";

            NameValueCollection query = new NameValueCollection
            {
                {"release_id", id.ToString() },
                {"limit", limit.ToString() },
                {"offset", offset.ToString() },
                {"order_by", Utility.GetDescription(orderBy) },
                {"sort_order", Utility.GetDescription(sortOrder) }
            };

            if (realtimeStart.HasValue)
            {
                query.Add("realtime_start", Utility.FormatDate(realtimeStart.Value));
            }

            if (realtimeEnd.HasValue)
            {
                query.Add("realtime_end", Utility.FormatDate(realtimeEnd.Value));
            }

            if (!string.IsNullOrEmpty(searchText))
            {
                query.Add("search_text", searchText);
            }

            if (tagGroupId != TagGroupId.None)
            {
                query.Add("tag_group_id", Utility.GetDescription(tagGroupId));
            }

            if (tags != null && tags.Any())
            {
                query.Add("tag_names", Utility.GetStringSeparatedBySemicolon(tags));
            }

            XmlDocument xmlDocument = await Request(url, query);

            List<Tag> result = new List<Tag>();

            foreach (XmlNode xmlNode in xmlDocument.DocumentElement.ChildNodes)
            {
                Tag tag = Utility.Deserialize<Tag>(xmlNode.OuterXml);

                result.Add(tag);
            }

            return result;
        }

        /// <summary>
        /// Get the related FRED tags for one or more FRED tags within a release. Optionally, filter results by tag group or search.
        /// https://research.stlouisfed.org/docs/api/fred/release_related_tags.html
        /// </summary>
        /// <param name="id">The id for a release.</param>
        /// <param name="tags">A list of tag names that series match all of, optional, no filtering by tags by default</param>
        /// <param name="realtimeStart">The start of the real-time period, optional, default: today's date</param>
        /// <param name="realtimeEnd">The end of the real-time period, optional, default: today's date</param>
        /// <param name="limit">The maximum number of results to return, between 1 and 1000, optional, default: 1000</param>
        /// <param name="offset">non-negative integer, optional, default: 0</param>
        /// <param name="orderBy">Order results by values of the specified attribute, optional, default: Id</param>
        /// <param name="sortOrder">Sort results is ascending or descending order, optional, default: Ascending</param>
        /// <param name="searchText">The words to find matching tags with, optional, no filtering by search words by default.</param>
        /// <param name="tagGroupId">A tag group id to filter tags by type, optional, no filtering by tag group by default.</param>
        /// <param name="excludeTags">A list of tag names that series match none of, optional, no default value.</param>
        /// <returns>List<Tag></returns>
        public async Task<List<Tag>> GetReleaseRelatedTags(int id, List<string> tags, DateTime? realtimeStart = null,
            DateTime? realtimeEnd = null, int limit = 1000, int offset = 0, TagOrderBy orderBy = TagOrderBy.SeriesCount,
            SortOrder sortOrder = SortOrder.Ascending, string searchText = null, TagGroupId tagGroupId = TagGroupId.None,
            List<string> excludeTags = null)
        {
            string url = $"release/related_tags";

            NameValueCollection query = new NameValueCollection
            {
                {"release_id", id.ToString() },
                {"tag_names", Utility.GetStringSeparatedBySemicolon(tags)},
                {"limit", limit.ToString() },
                {"offset", offset.ToString() },
                {"order_by", Utility.GetDescription(orderBy) },
                {"sort_order", Utility.GetDescription(sortOrder) }
            };

            if (realtimeStart.HasValue)
            {
                query.Add("realtime_start", Utility.FormatDate(realtimeStart.Value));
            }

            if (realtimeEnd.HasValue)
            {
                query.Add("realtime_end", Utility.FormatDate(realtimeEnd.Value));
            }

            if (!string.IsNullOrEmpty(searchText))
            {
                query.Add("search_text", searchText);
            }

            if (tagGroupId != TagGroupId.None)
            {
                query.Add("tag_group_id", Utility.GetDescription(tagGroupId));
            }

            if (excludeTags != null && excludeTags.Any())
            {
                query.Add("exclude_tag_names", Utility.GetStringSeparatedBySemicolon(excludeTags));
            }

            XmlDocument xmlDocument = await Request(url, query);

            List<Tag> result = new List<Tag>();

            foreach (XmlNode xmlNode in xmlDocument.DocumentElement.ChildNodes)
            {
                Tag tag = Utility.Deserialize<Tag>(xmlNode.OuterXml);

                result.Add(tag);
            }

            return result;
        }

        /// <summary>
        /// Get release table trees for a given release.
        /// https://research.stlouisfed.org/docs/api/fred/release_tables.html
        /// </summary>
        /// <param name="id">The id for a release.</param>
        /// <param name="elementId">The release table element id you would like to retrieve, when the parameter is not passed, the root(top most) element for the release is given.</param>
        /// <param name="includeObservationValues">A flag to indicate that observations need to be returned.</param>
        /// <param name="observation_date">The observation date to be included with the returned release table, optional, default: Latest available</param>
        /// <returns>List<Element></returns>
        public async Task<List<Element>> GetReleaseTables(int id, int? elementId = null, bool includeObservationValues = false,
            DateTime? observationDate = null)
        {
            string url = $"release/tables";

            NameValueCollection query = new NameValueCollection
            {
                {"release_id", id.ToString() },
                {"include_observation_values", includeObservationValues.ToString() }
            };

            if (elementId.HasValue)
            {
                query.Add("element_id", elementId.ToString());
            }

            if (observationDate.HasValue)
            {
                query.Add("observation_date", Utility.FormatDate(observationDate.Value));
            }

            XmlDocument xmlDocument = await Request(url, query);

            List<Element> result = new List<Element>();

            foreach (XmlNode xmlNode in xmlDocument.DocumentElement.ChildNodes)
            {
                if (!xmlNode.Name.Equals("element", StringComparison.InvariantCultureIgnoreCase))
                {
                    continue;
                }

                Element element = Utility.Deserialize<Element>(xmlNode.OuterXml);

                result.Add(element);
            }

            return result;
        }

        #endregion Releases

        #region Series

        /// <summary>
        /// Get an economic data series.
        /// https://research.stlouisfed.org/docs/api/fred/series.html
        /// </summary>
        /// <param name="id">The id for a series.</param>
        /// <param name="realtimeStart">The start of the real-time period, optional, default: today's date</param>
        /// <param name="realtimeEnd">The end of the real-time period, optional, default: today's date</param>
        /// <returns>Series</returns>
        public async Task<Series> GetSeries(string id, DateTime? realtimeStart = null, DateTime? realtimeEnd = null)
        {
            string url = $"series";

            NameValueCollection query = new NameValueCollection
            {
                {"series_id", id }
            };

            if (realtimeStart.HasValue)
            {
                query.Add("realtime_start", Utility.FormatDate(realtimeStart.Value));
            }

            if (realtimeEnd.HasValue)
            {
                query.Add("realtime_end", Utility.FormatDate(realtimeEnd.Value));
            }

            XmlDocument xmlDocument = await Request(url, query);

            Series result = Utility.Deserialize<Series>(xmlDocument.DocumentElement.InnerXml);

            return result;
        }

        /// <summary>
        /// Get the categories for an economic data series.
        /// https://research.stlouisfed.org/docs/api/fred/series_categories.html
        /// </summary>
        /// <param name="id">The id for a series.</param>
        /// <param name="realtimeStart">The start of the real-time period, optional, default: today's date</param>
        /// <param name="realtimeEnd">The end of the real-time period, optional, default: today's date</param>
        /// <returns>List<Category></returns>
        public async Task<List<Category>> GetSeriesCategories(string id, DateTime? realtimeStart = null, DateTime? realtimeEnd = null)
        {
            string url = $"series/categories";

            NameValueCollection query = new NameValueCollection
            {
                { "series_id", id },
            };

            if (realtimeStart.HasValue)
            {
                query.Add("realtime_start", Utility.FormatDate(realtimeStart.Value));
            }

            if (realtimeEnd.HasValue)
            {
                query.Add("realtime_end", Utility.FormatDate(realtimeEnd.Value));
            }

            XmlDocument xmlDocument = await Request(url, query);

            List<Category> result = new List<Category>();

            foreach (XmlNode xmlNode in xmlDocument.DocumentElement.ChildNodes)
            {
                Category category = Utility.Deserialize<Category>(xmlNode.OuterXml);

                result.Add(category);
            }

            return result;
        }

        /// <summary>
        /// Get the observations or data values for an economic data series.
        /// https://research.stlouisfed.org/docs/api/fred/series_observations.html
        /// </summary>
        /// <param name="id">The id for a series.</param>
        /// <param name="realtimeStart">The start of the real-time period, optional, default: today's date</param>
        /// <param name="realtimeEnd">The end of the real-time period, optional, default: today's date</param>
        /// <param name="limit">The maximum number of results to return, between 1 and 100000, optional, default: 100000</param>
        /// <param name="offset">non-negative integer, optional, default: 0</param>
        /// <param name="sortOrder">Sort results is ascending or descending observation date order, default: Ascending</param>
        /// <param name="observationStart">The start of the observation period, optional, default: 1776-07-04 (earliest available)</param>
        /// <param name="observationEnd">The end of the observation period, optional, default: 9999-12-31 (latest available)</param>
        /// <param name="unit">A key that indicates a data value transformation, optional, default: Level (No transformation)</param>
        /// <param name="frequency">An optional parameter that indicates a lower frequency to aggregate values to, optional, default: None for no frequency aggregation</param>
        /// <param name="aggregationMethod">A key that indicates the aggregation method used for frequency aggregation, optional, default: Average</param>
        /// <param name="outputType">An integer that indicates an output type, optional, default: RealTimePeriod</param>
        /// <param name="VintageDates">A list of dates in history (e.g. 2000-01-01,2005-02-24)</param>
        /// <returns></returns>
        public async Task<List<Observation>> GetSeriesObservations(string id, DateTime? realtimeStart = null, DateTime? realtimeEnd = null,
            int limit = 100000, int offset = 0, SortOrder sortOrder = SortOrder.Ascending, DateTime? observationStart = null,
            DateTime? observationEnd = null, SeriesObservationUnit unit = SeriesObservationUnit.Levels,
            SeriesObservationFrequency frequency = SeriesObservationFrequency.None,
            SeriesObservationAggregationMethod aggregationMethod = SeriesObservationAggregationMethod.Average,
            SeriesObservationOutputType outputType = SeriesObservationOutputType.RealTimePeriod, List<DateTime> vintageDates = null)
        {
            string url = $"series/observations";

            NameValueCollection query = new NameValueCollection
            {
                {"series_id", id },
                {"limit", limit.ToString() },
                {"offset", offset.ToString() },
                {"sort_order", Utility.GetDescription(sortOrder) },
                {"units", Utility.GetDescription(unit)},
                {"aggregation_method", Utility.GetDescription(aggregationMethod) },
                {"output_type", Utility.GetDescription(outputType)}
            };

            if (realtimeStart.HasValue)
            {
                query.Add("realtime_start", Utility.FormatDate(realtimeStart.Value));
            }

            if (realtimeEnd.HasValue)
            {
                query.Add("realtime_end", Utility.FormatDate(realtimeEnd.Value));
            }

            if (observationStart.HasValue)
            {
                query.Add("observation_start", Utility.FormatDate(observationStart.Value));
            }

            if (observationEnd.HasValue)
            {
                query.Add("observation_end", Utility.FormatDate(observationEnd.Value));
            }

            if (frequency != SeriesObservationFrequency.None)
            {
                query.Add("frequency", Utility.GetDescription(frequency));
            }

            if (vintageDates != null && vintageDates.Any())
            {
                query.Add("vintage_dates", Utility.GetDatesSeparatedByComma(vintageDates));
            }

            XmlDocument xmlDocument = await Request(url, query);

            List<Observation> result = new List<Observation>();

            foreach (XmlNode xmlNode in xmlDocument.DocumentElement.ChildNodes)
            {
                Observation observation = Utility.Deserialize<Observation>(xmlNode.OuterXml);

                result.Add(observation);
            }

            return result;
        }

        /// <summary>
        /// Get the release for an economic data series.
        /// https://research.stlouisfed.org/docs/api/fred/series_release.html
        /// </summary>
        /// <param name="id">The id for a series.</param>
        /// <param name="realtimeStart">The start of the real-time period, optional, default: today's date</param>
        /// <param name="realtimeEnd">The end of the real-time period, optional, default: today's date</param>
        /// <returns>Release</returns>
        public async Task<Release> GetSeriesRelease(string id, DateTime? realtimeStart = null, DateTime? realtimeEnd = null)
        {
            string url = $"series/release";

            NameValueCollection query = new NameValueCollection
            {
                {"series_id", id }
            };

            if (realtimeStart.HasValue)
            {
                query.Add("realtime_start", Utility.FormatDate(realtimeStart.Value));
            }

            if (realtimeEnd.HasValue)
            {
                query.Add("realtime_end", Utility.FormatDate(realtimeEnd.Value));
            }

            XmlDocument xmlDocument = await Request(url, query);

            Release result = Utility.Deserialize<Release>(xmlDocument.DocumentElement.InnerXml);

            return result;
        }

        /// <summary>
        /// Get economic data series that match search text.
        /// https://research.stlouisfed.org/docs/api/fred/series_search.html
        /// </summary>
        /// <param name="searchText">The words to match against economic data series.</param>
        /// <param name="searchType">Determines the type of search to perform, optional, default: full_text.</param>
        /// <param name="realtimeStart">The start of the real-time period, optional, default: today's date</param>
        /// <param name="realtimeEnd">The end of the real-time period, optional, default: today's date</param>
        /// <param name="limit">The maximum number of results to return, between 1 and 1000, optional, default: 1000</param>
        /// <param name="offset">non-negative integer, optional, default: 0</param>
        /// <param name="orderBy">Order results by values of the specified attribute, optional, default: None (API default ordering will be applied)</param>
        /// <param name="sortOrder">Sort results is ascending or descending order, optional, default: Ascending</param>
        /// <param name="filterVariable">The attribute to filter results by, optional, no filter by default</param>
        /// <param name="filterValue">The value of the filter_variable attribute to filter results by, optional, no filter by default</param>
        /// <param name="tags">A list of tag names that series match all of, optional, no filtering by tags by default</param>
        /// <param name="excludeTags">A list of tag names that series match none of, it requires that parameter tags also be set to limit the number of matching series, optional, no filtering by tags by default.</param>
        /// <returns>List<Series></returns>
        public async Task<List<Series>> SearchSeries(string searchText, SeriesSearchType searchType = SeriesSearchType.FullText,
            DateTime? realtimeStart = null, DateTime? realtimeEnd = null, int limit = 1000, int offset = 0,
            SeriesSearchOrderBy orderBy = SeriesSearchOrderBy.None, SortOrder sortOrder = SortOrder.Ascending,
            SeriesFilterVariable filterVariable = SeriesFilterVariable.None, string filterValue = null, List<string> tags = null,
            List<string> excludeTags = null)
        {
            string url = $"series/search";

            NameValueCollection query = new NameValueCollection
            {
                {"search_text", searchText },
                {"search_type", Utility.GetDescription(searchType) },
                {"limit", limit.ToString() },
                {"offset", offset.ToString() },
                {"sort_order", Utility.GetDescription(sortOrder) }
            };

            if (orderBy != SeriesSearchOrderBy.None)
            {
                query.Add("order_by", Utility.GetDescription(orderBy));
            }

            if (realtimeStart.HasValue)
            {
                query.Add("realtime_start", Utility.FormatDate(realtimeStart.Value));
            }

            if (realtimeEnd.HasValue)
            {
                query.Add("realtime_end", Utility.FormatDate(realtimeEnd.Value));
            }

            if (filterVariable != SeriesFilterVariable.None)
            {
                query.Add("filter_variable", Utility.GetDescription(filterVariable));
                query.Add("filter_value", filterValue);
            }

            if (tags != null && tags.Any())
            {
                query.Add("tag_names", Utility.GetStringSeparatedBySemicolon(tags));
            }

            if (excludeTags != null && excludeTags.Any())
            {
                query.Add("exclude_tag_names", Utility.GetStringSeparatedBySemicolon(excludeTags));
            }

            XmlDocument xmlDocument = await Request(url, query);

            List<Series> result = new List<Series>();

            foreach (XmlNode xmlNode in xmlDocument.DocumentElement.ChildNodes)
            {
                Series series = Utility.Deserialize<Series>(xmlNode.OuterXml);

                result.Add(series);
            }

            return result;
        }

        /// <summary>
        /// Get the FRED tags for a series search. Optionally, filter results by tag name, tag group, or tag search.
        /// https://research.stlouisfed.org/docs/api/fred/series_search_tags.html
        /// </summary>
        /// <param name="seriesSearchText">The words to match against economic data series.</param>
        /// <param name="realtimeStart">The start of the real-time period, optional, default: today's date</param>
        /// <param name="realtimeEnd">The end of the real-time period, optional, default: today's date</param>
        /// <param name="limit">The maximum number of results to return, between 1 and 1000, optional, default: 1000</param>
        /// <param name="offset">non-negative integer, optional, default: 0</param>
        /// <param name="orderBy">Order results by values of the specified attribute, optional, default: Id</param>
        /// <param name="sortOrder">Sort results is ascending or descending order, optional, default: Ascending</param>
        /// <param name="tagSearchText">The words to find matching tags with, optional, no filtering by search words by default.</param>
        /// <param name="tagGroupId">A tag group id to filter tags by type, optional, no filtering by tag group by default.</param>
        /// <param name="tags">A list of tag names to only include in the response, optional, no filtering by tags by default</param>
        /// <returns>List<Tag></returns>
        public async Task<List<Tag>> SearchSeriesTags(string seriesSearchText, DateTime? realtimeStart = null, DateTime? realtimeEnd = null,
            int limit = 1000, int offset = 0, TagOrderBy orderBy = TagOrderBy.SeriesCount, SortOrder sortOrder = SortOrder.Ascending,
            string tagSearchText = null, TagGroupId tagGroupId = TagGroupId.None, List<string> tags = null)
        {
            string url = $"series/search/tags";

            NameValueCollection query = new NameValueCollection
            {
                {"series_search_text", seriesSearchText },
                {"limit", limit.ToString() },
                {"offset", offset.ToString() },
                {"order_by", Utility.GetDescription(orderBy) },
                {"sort_order", Utility.GetDescription(sortOrder) }
            };

            if (realtimeStart.HasValue)
            {
                query.Add("realtime_start", Utility.FormatDate(realtimeStart.Value));
            }

            if (realtimeEnd.HasValue)
            {
                query.Add("realtime_end", Utility.FormatDate(realtimeEnd.Value));
            }

            if (!string.IsNullOrEmpty(tagSearchText))
            {
                query.Add("tag_search_text", tagSearchText);
            }

            if (tagGroupId != TagGroupId.None)
            {
                query.Add("tag_group_id", Utility.GetDescription(tagGroupId));
            }

            if (tags != null && tags.Any())
            {
                query.Add("tag_names", Utility.GetStringSeparatedBySemicolon(tags));
            }

            XmlDocument xmlDocument = await Request(url, query);

            List<Tag> result = new List<Tag>();

            foreach (XmlNode xmlNode in xmlDocument.DocumentElement.ChildNodes)
            {
                Tag tag = Utility.Deserialize<Tag>(xmlNode.OuterXml);

                result.Add(tag);
            }

            return result;
        }

        /// <summary>
        /// Get the related FRED tags for one or more FRED tags matching a series search. Optionally, filter results by tag group or tag search.
        /// https://research.stlouisfed.org/docs/api/fred/series_search_related_tags.html
        /// </summary>
        /// <param name="seriesSearchText">The words to match against economic data series.</param>
        /// <param name="tags">A list of tag names that series match all of</param>
        /// <param name="realtimeStart">The start of the real-time period, optional, default: today's date</param>
        /// <param name="realtimeEnd">The end of the real-time period, optional, default: today's date</param>
        /// <param name="limit">The maximum number of results to return, between 1 and 1000, optional, default: 1000</param>
        /// <param name="offset">non-negative integer, optional, default: 0</param>
        /// <param name="orderBy">Order results by values of the specified attribute, optional, default: Id</param>
        /// <param name="sortOrder">Sort results is ascending or descending order, optional, default: Ascending</param>
        /// <param name="tagSearchText">The words to find matching tags with, optional, no filtering by search words by default.</param>
        /// <param name="tagGroupId">A tag group id to filter tags by type, optional, no filtering by tag group by default.</param>
        /// <param name="excludeTags">A list of tag names that series match none of, optional, no default value</param>
        /// <returns>List<Tag></returns>
        public async Task<List<Tag>> SearchSeriesRelatedTags(string seriesSearchText, List<string> tags, DateTime? realtimeStart = null,
            DateTime? realtimeEnd = null, int limit = 1000, int offset = 0, TagOrderBy orderBy = TagOrderBy.SeriesCount,
            SortOrder sortOrder = SortOrder.Ascending, string tagSearchText = null, TagGroupId tagGroupId = TagGroupId.None,
            List<string> excludeTags = null)
        {
            string url = $"series/search/related_tags";

            NameValueCollection query = new NameValueCollection
            {
                {"series_search_text", seriesSearchText },
                {"tag_names", Utility.GetStringSeparatedBySemicolon(tags) },
                {"limit", limit.ToString() },
                {"offset", offset.ToString() },
                {"order_by", Utility.GetDescription(orderBy) },
                {"sort_order", Utility.GetDescription(sortOrder) }
            };

            if (realtimeStart.HasValue)
            {
                query.Add("realtime_start", Utility.FormatDate(realtimeStart.Value));
            }

            if (realtimeEnd.HasValue)
            {
                query.Add("realtime_end", Utility.FormatDate(realtimeEnd.Value));
            }

            if (!string.IsNullOrEmpty(tagSearchText))
            {
                query.Add("tag_search_text", tagSearchText);
            }

            if (tagGroupId != TagGroupId.None)
            {
                query.Add("tag_group_id", Utility.GetDescription(tagGroupId));
            }

            if (excludeTags != null && excludeTags.Any())
            {
                query.Add("exclude_tag_names", Utility.GetStringSeparatedBySemicolon(excludeTags));
            }

            XmlDocument xmlDocument = await Request(url, query);

            List<Tag> result = new List<Tag>();

            foreach (XmlNode xmlNode in xmlDocument.DocumentElement.ChildNodes)
            {
                Tag tag = Utility.Deserialize<Tag>(xmlNode.OuterXml);

                result.Add(tag);
            }

            return result;
        }

        /// <summary>
        /// Get the FRED tags for a series.
        /// https://research.stlouisfed.org/docs/api/fred/series_tags.html
        /// </summary>
        /// <param name="seriesId">The id for a series.</param>
        /// <param name="realtimeStart">The start of the real-time period, optional, default: today's date</param>
        /// <param name="realtimeEnd">The end of the real-time period, optional, default: today's date</param>
        /// <param name="orderBy">Order results by values of the specified attribute, optional, default: Id</param>
        /// <param name="sortOrder">Sort results is ascending or descending order, optional, default: Ascending</param>
        /// <returns>List<Tag></returns>
        public async Task<List<Tag>> GetSeriesTags(string seriesId, DateTime? realtimeStart = null, DateTime? realtimeEnd = null,
            TagOrderBy orderBy = TagOrderBy.SeriesCount, SortOrder sortOrder = SortOrder.Ascending)
        {
            string url = $"series/tags";

            NameValueCollection query = new NameValueCollection
            {
                {"series_id", seriesId },
                {"order_by", Utility.GetDescription(orderBy) },
                {"sort_order", Utility.GetDescription(sortOrder) }
            };

            if (realtimeStart.HasValue)
            {
                query.Add("realtime_start", Utility.FormatDate(realtimeStart.Value));
            }

            if (realtimeEnd.HasValue)
            {
                query.Add("realtime_end", Utility.FormatDate(realtimeEnd.Value));
            }

            XmlDocument xmlDocument = await Request(url, query);

            List<Tag> result = new List<Tag>();

            foreach (XmlNode xmlNode in xmlDocument.DocumentElement.ChildNodes)
            {
                Tag tag = Utility.Deserialize<Tag>(xmlNode.OuterXml);

                result.Add(tag);
            }

            return result;
        }

        /// <summary>
        /// Get economic data series sorted by when observations were updated on the FRED® server (attribute last_updated).
        /// https://research.stlouisfed.org/docs/api/fred/series_updates.html
        /// </summary>
        /// <param name="realtimeStart">The start of the real-time period, optional, default: today's date</param>
        /// <param name="realtimeEnd">The end of the real-time period, optional, default: today's date</param>
        /// <param name="limit">The maximum number of results to return, between 1 and 1000, optional, default: 1000</param>
        /// <param name="offset">non-negative integer, optional, default: 0</param>
        /// <param name="filterValue">Limit results by geographic type of economic data series, optional, default: 'All' meaning no filter.</param>
        /// <param name="startTime">Start time for limiting results for a time range, can filter down to minutes</param>
        /// <param name="endTime">End time for limiting results for a time range, can filter down to minutes</param>
        /// <returns>List<Series></returns>
        public async Task<List<Series>> GetSeriesUpdates(DateTime? realtimeStart = null, DateTime? realtimeEnd = null,
            int limit = 1000, int offset = 0, SeriesFilterValue filterValue = SeriesFilterValue.All, DateTime? startTime = null,
            DateTime? endTime = null)
        {
            string url = $"series/updates";

            NameValueCollection query = new NameValueCollection
            {
                {"limit", limit.ToString() },
                {"offset", offset.ToString() },
                {"filter_value", Utility.GetDescription(filterValue) }
            };

            if (realtimeStart.HasValue)
            {
                query.Add("realtime_start", Utility.FormatDate(realtimeStart.Value));
            }

            if (realtimeEnd.HasValue)
            {
                query.Add("realtime_end", Utility.FormatDate(realtimeEnd.Value));
            }

            if (startTime.HasValue)
            {
                query.Add("start_time", Utility.FormatTime(startTime.Value));
            }

            if (endTime.HasValue)
            {
                query.Add("end_time", Utility.FormatTime(endTime.Value));
            }

            XmlDocument xmlDocument = await Request(url, query);

            List<Series> result = new List<Series>();

            foreach (XmlNode xmlNode in xmlDocument.DocumentElement.ChildNodes)
            {
                Series series = Utility.Deserialize<Series>(xmlNode.OuterXml);

                result.Add(series);
            }

            return result;
        }

        /// <summary>
        /// Get the dates in history when a series' data values were revised or new data values were released.
        /// Vintage dates are the release dates for a series excluding release dates when the data for the series did not change.
        /// https://research.stlouisfed.org/docs/api/fred/series_vintagedates.html
        /// </summary>
        /// <param name="seriesId">The id for a series.</param>
        /// <param name="realtimeStart">The start of the real-time period, optional, default: today's date</param>
        /// <param name="realtimeEnd">The end of the real-time period, optional, default: today's date</param>
        /// <param name="limit">The maximum number of results to return, between 1 and 1000, optional, default: 1000</param>
        /// <param name="offset">non-negative integer, optional, default: 0</param>
        /// <param name="sortOrder">Sort results is ascending or descending vintage_date order, optional, default: asc</param>
        /// <returns>List<VintageDate></returns>
        public async Task<List<VintageDate>> GetSeriesVintageDates(string seriesId, DateTime? realtimeStart = null, DateTime? realtimeEnd = null,
            int limit = 1000, int offset = 0, SortOrder sortOrder = SortOrder.Ascending)
        {
            string url = $"series/vintagedates";

            NameValueCollection query = new NameValueCollection
            {
                {"series_id", seriesId },
                {"limit", limit.ToString() },
                {"offset", offset.ToString() },
                {"sort_order", Utility.GetDescription(sortOrder) }
            };

            if (realtimeStart.HasValue)
            {
                query.Add("realtime_start", Utility.FormatDate(realtimeStart.Value));
            }

            if (realtimeEnd.HasValue)
            {
                query.Add("realtime_end", Utility.FormatDate(realtimeEnd.Value));
            }

            XmlDocument xmlDocument = await Request(url, query);

            List<VintageDate> result = new List<VintageDate>();

            foreach (XmlNode xmlNode in xmlDocument.DocumentElement.ChildNodes)
            {
                VintageDate vintageDate = Utility.Deserialize<VintageDate>(xmlNode.OuterXml);

                result.Add(vintageDate);
            }

            return result;
        }

        #endregion Series

        #region Sources

        /// <summary>
        /// Get all sources of economic data.
        /// https://research.stlouisfed.org/docs/api/fred/sources.html
        /// </summary>
        /// <param name="realtimeStart">The start of the real-time period, optional, default: today's date</param>
        /// <param name="realtimeEnd">The end of the real-time period, optional, default: today's date</param>
        /// <param name="limit">The maximum number of results to return, between 1 and 1000, optional, default: 1000</param>
        /// <param name="offset">non-negative integer, optional, default: 0</param>
        /// <param name="orderBy">Order results by values of the specified attribute, optional, default: Id</param>
        /// <param name="sortOrder">Sort results is ascending or descending vintage_date order, optional, default: Ascending</param>
        /// <returns>List<Source></returns>
        public async Task<List<Source>> GetSources(DateTime? realtimeStart = null, DateTime? realtimeEnd = null, int limit = 1000,
            int offset = 0, SourceOrderBy orderBy = SourceOrderBy.Id, SortOrder sortOrder = SortOrder.Ascending)
        {
            string url = $"sources";

            NameValueCollection query = new NameValueCollection
            {
                {"limit", limit.ToString() },
                {"offset", offset.ToString() },
                {"order_by", Utility.GetDescription(orderBy) },
                {"sort_order", Utility.GetDescription(sortOrder) }
            };

            if (realtimeStart.HasValue)
            {
                query.Add("realtime_start", Utility.FormatDate(realtimeStart.Value));
            }

            if (realtimeEnd.HasValue)
            {
                query.Add("realtime_end", Utility.FormatDate(realtimeEnd.Value));
            }

            XmlDocument xmlDocument = await Request(url, query);

            List<Source> result = new List<Source>();

            foreach (XmlNode xmlNode in xmlDocument.DocumentElement.ChildNodes)
            {
                Source source = Utility.Deserialize<Source>(xmlNode.OuterXml);

                result.Add(source);
            }

            return result;
        }

        /// <summary>
        /// Get a source of economic data.
        /// https://research.stlouisfed.org/docs/api/fred/source.html
        /// </summary>
        /// <param name="id">The id for a source.</param>
        /// <param name="realtimeStart">The start of the real-time period, optional, default: today's date</param>
        /// <param name="realtimeEnd">The end of the real-time period, optional, default: today's date</param>
        /// <returns>Source</returns>
        public async Task<Source> GetSource(int id, DateTime? realtimeStart = null, DateTime? realtimeEnd = null)
        {
            string url = $"source";

            NameValueCollection query = new NameValueCollection
            {
                {"source_id", id.ToString() },
            };

            if (realtimeStart.HasValue)
            {
                query.Add("realtime_start", Utility.FormatDate(realtimeStart.Value));
            }

            if (realtimeEnd.HasValue)
            {
                query.Add("realtime_end", Utility.FormatDate(realtimeEnd.Value));
            }

            XmlDocument xmlDocument = await Request(url, query);

            Source result = Utility.Deserialize<Source>(xmlDocument.DocumentElement.InnerXml);

            return result;
        }

        /// <summary>
        /// Get the releases for a source.
        /// https://research.stlouisfed.org/docs/api/fred/source_releases.html
        /// </summary>
        /// <param name="id">The id for a source.</param>
        /// <param name="realtimeStart">The start of the real-time period, optional, default: today's date</param>
        /// <param name="realtimeEnd">The end of the real-time period, optional, default: today's date</param>
        /// <param name="limit">The maximum number of results to return, between 1 and 1000, optional, default: 1000</param>
        /// <param name="offset">non-negative integer, optional, default: 0</param>
        /// <param name="orderBy">Order results by values of the specified attribute, optional, default: Id</param>
        /// <param name="sortOrder">Sort results is ascending or descending order, optional, default: asc</param>
        /// <returns>List<Release></returns>
        public async Task<List<Release>> GetSourceReleases(int id, DateTime? realtimeStart = null, DateTime? realtimeEnd = null, int limit = 1000,
            int offset = 0, ReleaseOrderBy orderBy = ReleaseOrderBy.Id, SortOrder sortOrder = SortOrder.Ascending)
        {
            string url = $"source/releases";

            NameValueCollection query = new NameValueCollection
            {
                {"source_id", id.ToString() },
                {"limit", limit.ToString() },
                {"offset", offset.ToString() },
                {"order_by", Utility.GetDescription(orderBy) },
                {"sort_order", Utility.GetDescription(sortOrder) }
            };

            if (realtimeStart.HasValue)
            {
                query.Add("realtime_start", Utility.FormatDate(realtimeStart.Value));
            }

            if (realtimeEnd.HasValue)
            {
                query.Add("realtime_end", Utility.FormatDate(realtimeEnd.Value));
            }

            XmlDocument xmlDocument = await Request(url, query);

            List<Release> result = new List<Release>();

            foreach (XmlNode xmlNode in xmlDocument.DocumentElement.ChildNodes)
            {
                Release release = Utility.Deserialize<Release>(xmlNode.OuterXml);

                result.Add(release);
            }

            return result;
        }

        #endregion Sources

        #region Tags

        /// <summary>
        /// Get FRED tags. Optionally, filter results by tag name, tag group, or search. FRED tags are attributes assigned to series.
        /// https://research.stlouisfed.org/docs/api/fred/tags.html
        /// </summary>
        /// <param name="realtimeStart">The start of the real-time period, optional, default: today's date</param>
        /// <param name="realtimeEnd">The end of the real-time period, optional, default: today's date</param>
        /// <param name="limit">The maximum number of results to return, between 1 and 1000, optional, default: 1000</param>
        /// <param name="offset">non-negative integer, optional, default: 0</param>
        /// <param name="orderBy">Order results by values of the specified attribute, optional, default: SeriesCount</param>
        /// <param name="sortOrder">Sort results is ascending or descending order, optional, default: Ascending</param>
        /// <param name="searchText">The words to find matching tags with, optional, no filtering by search words by default.</param>
        /// <param name="tagGroupId">A tag group id to filter tags by type, optional, no filtering by tag group by default.</param>
        /// <param name="tags">A list of tag names to only include in the response, optional, no filtering by tags by default</param>
        /// <returns>List<Tag></returns>
        public async Task<List<Tag>> GetTags(DateTime? realtimeStart = null, DateTime? realtimeEnd = null, int limit = 1000,
            int offset = 0, TagOrderBy orderBy = TagOrderBy.SeriesCount, SortOrder sortOrder = SortOrder.Ascending, string searchText = null,
            TagGroupId tagGroupId = TagGroupId.None, List<string> tags = null)
        {
            string url = $"tags";

            NameValueCollection query = new NameValueCollection
            {
                {"limit", limit.ToString() },
                {"offset", offset.ToString() },
                {"order_by", Utility.GetDescription(orderBy) },
                {"sort_order", Utility.GetDescription(sortOrder) }
            };

            if (realtimeStart.HasValue)
            {
                query.Add("realtime_start", Utility.FormatDate(realtimeStart.Value));
            }

            if (realtimeEnd.HasValue)
            {
                query.Add("realtime_end", Utility.FormatDate(realtimeEnd.Value));
            }

            if (!string.IsNullOrEmpty(searchText))
            {
                query.Add("search_text", searchText);
            }

            if (tagGroupId != TagGroupId.None)
            {
                query.Add("tag_group_id", Utility.GetDescription(tagGroupId));
            }

            if (tags != null && tags.Any())
            {
                query.Add("tag_names", Utility.GetStringSeparatedBySemicolon(tags));
            }

            XmlDocument xmlDocument = await Request(url, query);

            List<Tag> result = new List<Tag>();

            foreach (XmlNode xmlNode in xmlDocument.DocumentElement.ChildNodes)
            {
                Tag tag = Utility.Deserialize<Tag>(xmlNode.OuterXml);

                result.Add(tag);
            }

            return result;
        }

        /// <summary>
        /// Get the related FRED tags for one or more FRED tags. Optionally, filter results by tag group or search.
        /// https://research.stlouisfed.org/docs/api/fred/related_tags.html
        /// </summary>
        /// <param name="tags">A list of tag names that series match all of, optional, no filtering by tags by default</param>
        /// <param name="realtimeStart">The start of the real-time period, optional, default: today's date</param>
        /// <param name="realtimeEnd">The end of the real-time period, optional, default: today's date</param>
        /// <param name="limit">The maximum number of results to return, between 1 and 1000, optional, default: 1000</param>
        /// <param name="offset">non-negative integer, optional, default: 0</param>
        /// <param name="orderBy">Order results by values of the specified attribute, optional, default: SeriesCount</param>
        /// <param name="sortOrder">Sort results is ascending or descending order, optional, default: Ascending</param>
        /// <param name="searchText">The words to find matching tags with, optional, no filtering by search words by default.</param>
        /// <param name="tagGroupId">A tag group id to filter tags by type, optional, no filtering by tag group by default.</param>
        /// <param name="excludeTags">A list of tag names that series match none of, optional, no default value.</param>
        /// <returns>List<Tag></returns>
        public async Task<List<Tag>> GetRelatedTags(List<string> tags, DateTime? realtimeStart = null, DateTime? realtimeEnd = null,
            int limit = 1000, int offset = 0, TagOrderBy orderBy = TagOrderBy.SeriesCount, SortOrder sortOrder = SortOrder.Ascending,
            string searchText = null, TagGroupId tagGroupId = TagGroupId.None, List<string> excludeTags = null)
        {
            string url = $"related_tags";

            NameValueCollection query = new NameValueCollection
            {
                {"tag_names", Utility.GetStringSeparatedBySemicolon(tags)},
                {"limit", limit.ToString() },
                {"offset", offset.ToString() },
                {"order_by", Utility.GetDescription(orderBy) },
                {"sort_order", Utility.GetDescription(sortOrder) }
            };

            if (realtimeStart.HasValue)
            {
                query.Add("realtime_start", Utility.FormatDate(realtimeStart.Value));
            }

            if (realtimeEnd.HasValue)
            {
                query.Add("realtime_end", Utility.FormatDate(realtimeEnd.Value));
            }

            if (!string.IsNullOrEmpty(searchText))
            {
                query.Add("search_text", searchText);
            }

            if (tagGroupId != TagGroupId.None)
            {
                query.Add("tag_group_id", Utility.GetDescription(tagGroupId));
            }

            if (excludeTags != null && excludeTags.Any())
            {
                query.Add("exclude_tag_names", Utility.GetStringSeparatedBySemicolon(excludeTags));
            }

            XmlDocument xmlDocument = await Request(url, query);

            List<Tag> result = new List<Tag>();

            foreach (XmlNode xmlNode in xmlDocument.DocumentElement.ChildNodes)
            {
                Tag tag = Utility.Deserialize<Tag>(xmlNode.OuterXml);

                result.Add(tag);
            }

            return result;
        }

        /// <summary>
        /// Get the series matching all tags in the tag_names parameter and no tags in the exclude_tag_names parameter.
        /// https://research.stlouisfed.org/docs/api/fred/tags_series.html
        /// </summary>
        /// <param name="tags">A list of tag names that series match all of, optional, no filtering by tags by default</param>
        /// <param name="realtimeStart">The start of the real-time period, optional, default: today's date</param>
        /// <param name="realtimeEnd">The end of the real-time period, optional, default: today's date</param>
        /// <param name="limit">The maximum number of results to return, between 1 and 1000, optional, default: 1000</param>
        /// <param name="offset">non-negative integer, optional, default: 0</param>
        /// <param name="orderBy">Order results by values of the specified attribute, optional, default: Id</param>
        /// <param name="sortOrder">Sort results is ascending or descending order, optional, default: Ascending</param>
        /// <param name="excludeTags">A list of tag names that series match none of, optional, no default value.</param>
        /// <returns>List<Series></returns>
        public async Task<List<Series>> GetTagsSeries(List<string> tags, DateTime? realtimeStart = null, DateTime? realtimeEnd = null,
            int limit = 1000, int offset = 0, SeriesOrderBy orderBy = SeriesOrderBy.Id, SortOrder sortOrder = SortOrder.Ascending
            , List<string> excludeTags = null)
        {
            string url = $"tags/series";

            NameValueCollection query = new NameValueCollection
            {
                {"tag_names", Utility.GetStringSeparatedBySemicolon(tags)},
                {"limit", limit.ToString() },
                {"offset", offset.ToString() },
                {"order_by", Utility.GetDescription(orderBy) },
                {"sort_order", Utility.GetDescription(sortOrder) }
            };

            if (realtimeStart.HasValue)
            {
                query.Add("realtime_start", Utility.FormatDate(realtimeStart.Value));
            }

            if (realtimeEnd.HasValue)
            {
                query.Add("realtime_end", Utility.FormatDate(realtimeEnd.Value));
            }

            if (excludeTags != null && excludeTags.Any())
            {
                query.Add("exclude_tag_names", Utility.GetStringSeparatedBySemicolon(excludeTags));
            }

            XmlDocument xmlDocument = await Request(url, query);

            List<Series> result = new List<Series>();

            foreach (XmlNode xmlNode in xmlDocument.DocumentElement.ChildNodes)
            {
                Series series = Utility.Deserialize<Series>(xmlNode.OuterXml);

                result.Add(series);
            }

            return result;
        }

        #endregion Tags

        #region Others

        /// <summary>
        /// All requests sends through this method
        /// </summary>
        /// <param name="url">The relative URL of a request</param>
        /// <param name="query">The URL query of a request</param>
        /// <returns></returns>
        private async Task<XmlDocument> Request(string url, NameValueCollection query)
        {
            _webClient.QueryString = query;

            _webClient.QueryString.Add("api_key", _apiKey);

            string response = await _webClient.DownloadStringTaskAsync(url);

            XmlDocument xmlDocument = new XmlDocument();

            xmlDocument.LoadXml(response);

            return xmlDocument;
        }

        /// <summary>
        /// Disposes the web client object
        /// </summary>
        public void Dispose()
        {
            _webClient.Dispose();
        }

        #endregion Others
    }
}
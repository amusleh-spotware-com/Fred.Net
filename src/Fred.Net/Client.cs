using Fred.Net.Enums;
using Fred.Net.Models;
using Fred.Net.Parameters;
using Fred.Net.Parameters.Abstractions;
using Fred.Net.Utils;
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
        /// This web client object will be used during lift time of client object
        /// </summary>
        private readonly WebClient _webClient;

        #endregion Fields

        /// <summary>
        /// Creates an instance of Client class and initializes a web client which is responsible for sending web requests
        /// </summary>
        /// <param name="apiKey">Your Fred API key</param>
        /// <param name="baseUrl">The API base URL</param>
        public Client(string apiKey, string baseUrl = "https://api.stlouisfed.org/fred/")
        {
            ApiKey = apiKey;

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

            _webClient = new WebClient
            {
                BaseAddress = baseUrl
            };
        }

        #region Properties

        /// <summary>
        /// Returns your provided API key during initialization
        /// </summary>
        public string ApiKey { get; }

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

            XmlDocument xmlDocument = await Request(url, query).ConfigureAwait(false);

            Category result = Deserializer.Deserialize<Category>(xmlDocument.DocumentElement.InnerXml);

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
        public async Task<List<Category>> GetCategoryChildren(CategoryParameters parameters)
        {
            string url = $"category/children";

            NameValueCollection query = new NameValueCollection
            {
                { "category_id", parameters.Id.ToString() },
            };

            SetRealTimeParameters(parameters, query);

            XmlDocument xmlDocument = await Request(url, query).ConfigureAwait(false);

            List<Category> result = new List<Category>();

            foreach (XmlNode xmlNode in xmlDocument.DocumentElement.ChildNodes)
            {
                Category category = Deserializer.Deserialize<Category>(xmlNode.OuterXml);

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
        public async Task<List<Category>> GetCategoryRelated(CategoryParameters parameters)
        {
            string url = $"category/related";

            NameValueCollection query = new NameValueCollection
            {
                { "category_id", parameters.Id.ToString() },
            };

            SetRealTimeParameters(parameters, query);

            XmlDocument xmlDocument = await Request(url, query).ConfigureAwait(false);

            List<Category> result = new List<Category>();

            foreach (XmlNode xmlNode in xmlDocument.DocumentElement.ChildNodes)
            {
                Category category = Deserializer.Deserialize<Category>(xmlNode.OuterXml);

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
        public async Task<List<Series>> GetCategorySeries(CategorySeriesParameters parameters)
        {
            string url = $"category/series";

            NameValueCollection query = new NameValueCollection
            {
                {"category_id", parameters.Id.ToString() },
                {"limit", parameters.Limit.ToString() },
                {"offset", parameters.Offset.ToString() },
                {"order_by", EnumDescription.GetDescription(parameters.OrderBy) },
                {"sort_order", EnumDescription.GetDescription(parameters.SortOrder) }
            };

            SetRealTimeParameters(parameters, query);

            if (parameters.FilterVariable != SeriesFilterVariable.None)
            {
                query.Add("filter_variable", EnumDescription.GetDescription(parameters.FilterVariable));
                query.Add("filter_value", parameters.FilterValue);
            }

            if (parameters.Tags != null && parameters.Tags.Any())
            {
                query.Add("tag_names", Separator.GetStringSeparatedBySemicolon(parameters.Tags));
            }

            if (parameters.ExcludeTags != null && parameters.ExcludeTags.Any())
            {
                query.Add("exclude_tag_names", Separator.GetStringSeparatedBySemicolon(parameters.ExcludeTags));
            }

            XmlDocument xmlDocument = await Request(url, query).ConfigureAwait(false);

            List<Series> result = new List<Series>();

            foreach (XmlNode xmlNode in xmlDocument.DocumentElement.ChildNodes)
            {
                Series series = Deserializer.Deserialize<Series>(xmlNode.OuterXml);

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
        public async Task<List<Tag>> GetCategoryTags(TagParameters parameters)
        {
            string url = $"category/tags";

            NameValueCollection query = new NameValueCollection
            {
                {"category_id", parameters.Id.ToString() },
                {"limit", parameters.Limit.ToString() },
                {"offset", parameters.Offset.ToString() },
                {"order_by", EnumDescription.GetDescription(parameters.OrderBy) },
                {"sort_order", EnumDescription.GetDescription(parameters.SortOrder) }
            };

            SetRealTimeParameters(parameters, query);

            if (!string.IsNullOrEmpty(parameters.SearchText))
            {
                query.Add("search_text", parameters.SearchText);
            }

            if (parameters.GroupId != TagGroupId.None)
            {
                query.Add("tag_group_id", EnumDescription.GetDescription(parameters.GroupId));
            }

            if (parameters.Tags != null && parameters.Tags.Any())
            {
                query.Add("tag_names", Separator.GetStringSeparatedBySemicolon(parameters.Tags));
            }

            XmlDocument xmlDocument = await Request(url, query).ConfigureAwait(false);

            List<Tag> result = new List<Tag>();

            foreach (XmlNode xmlNode in xmlDocument.DocumentElement.ChildNodes)
            {
                Tag tag = Deserializer.Deserialize<Tag>(xmlNode.OuterXml);

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
        public async Task<List<Tag>> GetCategoryRelatedTags(RelatedTagParameters parameters)
        {
            string url = $"category/related_tags";

            NameValueCollection query = new NameValueCollection
            {
                {"category_id", parameters.Id.ToString() },
                {"tag_names", Separator.GetStringSeparatedBySemicolon(parameters.Tags)},
                {"limit", parameters.Limit.ToString() },
                {"offset", parameters.Offset.ToString() },
                {"order_by", EnumDescription.GetDescription(parameters.OrderBy) },
                {"sort_order", EnumDescription.GetDescription(parameters.SortOrder) }
            };

            SetRealTimeParameters(parameters, query);

            if (!string.IsNullOrEmpty(parameters.SearchText))
            {
                query.Add("search_text", parameters.SearchText);
            }

            if (parameters.GroupId != TagGroupId.None)
            {
                query.Add("tag_group_id", EnumDescription.GetDescription(parameters.GroupId));
            }

            if (parameters.ExcludeTags != null && parameters.ExcludeTags.Any())
            {
                query.Add("exclude_tag_names", Separator.GetStringSeparatedBySemicolon(parameters.ExcludeTags));
            }

            XmlDocument xmlDocument = await Request(url, query).ConfigureAwait(false);

            List<Tag> result = new List<Tag>();

            foreach (XmlNode xmlNode in xmlDocument.DocumentElement.ChildNodes)
            {
                Tag tag = Deserializer.Deserialize<Tag>(xmlNode.OuterXml);

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
        public async Task<List<Release>> GetReleases(ReleasesParameters parameters)
        {
            string url = $"releases";

            NameValueCollection query = new NameValueCollection
            {
                {"limit", parameters.Limit.ToString() },
                {"offset", parameters.Offset.ToString() },
                {"order_by", EnumDescription.GetDescription(parameters.OrderBy) },
                {"sort_order", EnumDescription.GetDescription(parameters.SortOrder) }
            };

            SetRealTimeParameters(parameters, query);

            XmlDocument xmlDocument = await Request(url, query).ConfigureAwait(false);

            List<Release> result = new List<Release>();

            foreach (XmlNode xmlNode in xmlDocument.DocumentElement.ChildNodes)
            {
                Release release = Deserializer.Deserialize<Release>(xmlNode.OuterXml);

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
        public async Task<List<ReleaseDate>> GetReleasesDates(ReleasesDatesParameters parameters)
        {
            string url = $"releases/dates";

            NameValueCollection query = new NameValueCollection
            {
                {"limit", parameters.Limit.ToString() },
                {"offset", parameters.Offset.ToString() },
                {"order_by", EnumDescription.GetDescription(parameters.OrderBy) },
                {"sort_order", EnumDescription.GetDescription(parameters.SortOrder) },
                {"include_release_dates_with_no_data", parameters.IncludeReleaseDatesWithNoData.ToString().ToLowerInvariant() }
            };

            SetRealTimeParameters(parameters, query);

            XmlDocument xmlDocument = await Request(url, query).ConfigureAwait(false);

            List<ReleaseDate> result = new List<ReleaseDate>();

            foreach (XmlNode xmlNode in xmlDocument.DocumentElement.ChildNodes)
            {
                ReleaseDate releaseDate = Deserializer.Deserialize<ReleaseDate>(xmlNode.OuterXml);

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
        public async Task<Release> GetRelease(ReleaseParameters parameters)
        {
            string url = $"release";

            NameValueCollection query = new NameValueCollection
            {
                {"release_id", parameters.Id.ToString() }
            };

            SetRealTimeParameters(parameters, query);

            XmlDocument xmlDocument = await Request(url, query).ConfigureAwait(false);

            Release result = Deserializer.Deserialize<Release>(xmlDocument.DocumentElement.InnerXml);

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
        public async Task<List<ReleaseDate>> GetReleaseDates(ReleaseDatesParameters parameters)
        {
            string url = $"release/dates";

            NameValueCollection query = new NameValueCollection
            {
                {"release_id", parameters.Id.ToString() },
                {"limit", parameters.Limit.ToString() },
                {"offset", parameters.Offset.ToString() },
                {"order_by", EnumDescription.GetDescription(parameters.OrderBy) },
                {"sort_order", EnumDescription.GetDescription(parameters.SortOrder) },
                {"include_release_dates_with_no_data", parameters.IncludeReleaseDatesWithNoData.ToString().ToLowerInvariant() }
            };

            SetRealTimeParameters(parameters, query);

            XmlDocument xmlDocument = await Request(url, query).ConfigureAwait(false);

            List<ReleaseDate> result = new List<ReleaseDate>();

            foreach (XmlNode xmlNode in xmlDocument.DocumentElement.ChildNodes)
            {
                ReleaseDate releaseDate = Deserializer.Deserialize<ReleaseDate>(xmlNode.OuterXml);

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
        public async Task<List<Series>> GetReleaseSeries(ReleaseSeriesParameters parameters)
        {
            string url = $"release/series";

            NameValueCollection query = new NameValueCollection
            {
                {"release_id", parameters.Id.ToString() },
                {"limit", parameters.Limit.ToString() },
                {"offset", parameters.Offset.ToString() },
                {"order_by", EnumDescription.GetDescription(parameters.OrderBy) },
                {"sort_order", EnumDescription.GetDescription(parameters.SortOrder) }
            };

            SetRealTimeParameters(parameters, query);

            if (parameters.FilterVariable != SeriesFilterVariable.None)
            {
                query.Add("filter_variable", EnumDescription.GetDescription(parameters.FilterVariable));
                query.Add("filter_value", parameters.FilterValue);
            }

            if (parameters.Tags != null && parameters.Tags.Any())
            {
                query.Add("tag_names", Separator.GetStringSeparatedBySemicolon(parameters.Tags));
            }

            if (parameters.ExcludeTags != null && parameters.ExcludeTags.Any())
            {
                query.Add("exclude_tag_names", Separator.GetStringSeparatedBySemicolon(parameters.ExcludeTags));
            }

            XmlDocument xmlDocument = await Request(url, query).ConfigureAwait(false);

            List<Series> result = new List<Series>();

            foreach (XmlNode xmlNode in xmlDocument.DocumentElement.ChildNodes)
            {
                Series series = Deserializer.Deserialize<Series>(xmlNode.OuterXml);

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
        public async Task<List<Source>> GetReleaseSources(ReleaseSourcesParameters parameters)
        {
            string url = $"release/sources";

            NameValueCollection query = new NameValueCollection
            {
                {"release_id", parameters.Id.ToString() }
            };

            SetRealTimeParameters(parameters, query);

            XmlDocument xmlDocument = await Request(url, query).ConfigureAwait(false);

            List<Source> result = new List<Source>();

            foreach (XmlNode xmlNode in xmlDocument.DocumentElement.ChildNodes)
            {
                Source source = Deserializer.Deserialize<Source>(xmlNode.OuterXml);

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
        public async Task<List<Tag>> GetReleaseTags(TagParameters parameters)
        {
            string url = $"release/tags";

            NameValueCollection query = new NameValueCollection
            {
                {"release_id", parameters.Id.ToString() },
                {"limit", parameters.Limit.ToString() },
                {"offset", parameters.Offset.ToString() },
                {"order_by", EnumDescription.GetDescription(parameters.OrderBy) },
                {"sort_order", EnumDescription.GetDescription(parameters.SortOrder) }
            };

            SetRealTimeParameters(parameters, query);

            if (!string.IsNullOrEmpty(parameters.SearchText))
            {
                query.Add("search_text", parameters.SearchText);
            }

            if (parameters.GroupId != TagGroupId.None)
            {
                query.Add("tag_group_id", EnumDescription.GetDescription(parameters.GroupId));
            }

            if (parameters.Tags != null && parameters.Tags.Any())
            {
                query.Add("tag_names", Separator.GetStringSeparatedBySemicolon(parameters.Tags));
            }

            XmlDocument xmlDocument = await Request(url, query).ConfigureAwait(false);

            List<Tag> result = new List<Tag>();

            foreach (XmlNode xmlNode in xmlDocument.DocumentElement.ChildNodes)
            {
                Tag tag = Deserializer.Deserialize<Tag>(xmlNode.OuterXml);

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
        public async Task<List<Tag>> GetReleaseRelatedTags(RelatedTagParameters parameters)
        {
            string url = $"release/related_tags";

            NameValueCollection query = new NameValueCollection
            {
                {"release_id", parameters.Id.ToString() },
                {"tag_names", Separator.GetStringSeparatedBySemicolon(parameters.Tags)},
                {"limit", parameters.Limit.ToString() },
                {"offset", parameters.Offset.ToString() },
                {"order_by", EnumDescription.GetDescription(parameters.OrderBy) },
                {"sort_order", EnumDescription.GetDescription(parameters.SortOrder) }
            };

            SetRealTimeParameters(parameters, query);

            if (!string.IsNullOrEmpty(parameters.SearchText))
            {
                query.Add("search_text", parameters.SearchText);
            }

            if (parameters.GroupId != TagGroupId.None)
            {
                query.Add("tag_group_id", EnumDescription.GetDescription(parameters.GroupId));
            }

            if (parameters.ExcludeTags != null && parameters.ExcludeTags.Any())
            {
                query.Add("exclude_tag_names", Separator.GetStringSeparatedBySemicolon(parameters.ExcludeTags));
            }

            XmlDocument xmlDocument = await Request(url, query).ConfigureAwait(false);

            List<Tag> result = new List<Tag>();

            foreach (XmlNode xmlNode in xmlDocument.DocumentElement.ChildNodes)
            {
                Tag tag = Deserializer.Deserialize<Tag>(xmlNode.OuterXml);

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
        public async Task<List<Element>> GetReleaseTables(ElementParameters parameters)
        {
            string url = $"release/tables";

            NameValueCollection query = new NameValueCollection
            {
                {"release_id", parameters.Id.ToString() },
                {"include_observation_values", parameters.IncludeObservationValues.ToString() }
            };

            if (parameters.ElementId.HasValue)
            {
                query.Add("element_id", parameters.ElementId.ToString());
            }

            if (parameters.ObservationDate.HasValue)
            {
                query.Add("observation_date", DateTimeFormat.FormatDate(parameters.ObservationDate.Value));
            }

            XmlDocument xmlDocument = await Request(url, query).ConfigureAwait(false);

            List<Element> result = new List<Element>();

            foreach (XmlNode xmlNode in xmlDocument.DocumentElement.ChildNodes)
            {
                if (!xmlNode.Name.Equals("element", StringComparison.InvariantCultureIgnoreCase))
                {
                    continue;
                }

                Element element = Deserializer.Deserialize<Element>(xmlNode.OuterXml);

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
        public async Task<Series> GetSeries(SeriesParameters parameters)
        {
            string url = $"series";

            NameValueCollection query = new NameValueCollection
            {
                {"series_id", parameters.Id }
            };

            SetRealTimeParameters(parameters, query);

            XmlDocument xmlDocument = await Request(url, query).ConfigureAwait(false);

            Series result = Deserializer.Deserialize<Series>(xmlDocument.DocumentElement.InnerXml);

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
        public async Task<List<Category>> GetSeriesCategories(SeriesCategoriesParameters parameters)
        {
            string url = $"series/categories";

            NameValueCollection query = new NameValueCollection
            {
                { "series_id", parameters.Id },
            };

            SetRealTimeParameters(parameters, query);

            XmlDocument xmlDocument = await Request(url, query).ConfigureAwait(false);

            List<Category> result = new List<Category>();

            foreach (XmlNode xmlNode in xmlDocument.DocumentElement.ChildNodes)
            {
                Category category = Deserializer.Deserialize<Category>(xmlNode.OuterXml);

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
        public async Task<List<Observation>> GetSeriesObservations(ObservationParameters parameters)
        {
            string url = $"series/observations";

            NameValueCollection query = new NameValueCollection
            {
                {"series_id", parameters.Id },
                {"limit", parameters.Limit.ToString() },
                {"offset", parameters.Offset.ToString() },
                {"sort_order", EnumDescription.GetDescription(parameters.SortOrder) },
                {"units", EnumDescription.GetDescription(parameters.Unit)},
                {"aggregation_method", EnumDescription.GetDescription(parameters.AggregationMethod) },
                {"output_type", EnumDescription.GetDescription(parameters.OutputType)}
            };

            SetRealTimeParameters(parameters, query);

            if (parameters.Start.HasValue)
            {
                query.Add("observation_start", DateTimeFormat.FormatDate(parameters.Start.Value));
            }

            if (parameters.End.HasValue)
            {
                query.Add("observation_end", DateTimeFormat.FormatDate(parameters.End.Value));
            }

            if (parameters.Frequency != SeriesObservationFrequency.None)
            {
                query.Add("frequency", EnumDescription.GetDescription(parameters.Frequency));
            }

            if (parameters.VintageDates != null && parameters.VintageDates.Any())
            {
                query.Add("vintage_dates", Separator.GetDatesSeparatedByComma(parameters.VintageDates));
            }

            XmlDocument xmlDocument = await Request(url, query).ConfigureAwait(false);

            List<Observation> result = new List<Observation>();

            foreach (XmlNode xmlNode in xmlDocument.DocumentElement.ChildNodes)
            {
                Observation observation = Deserializer.Deserialize<Observation>(xmlNode.OuterXml);

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
        public async Task<Release> GetSeriesRelease(SeriesReleaseParameters parameters)
        {
            string url = $"series/release";

            NameValueCollection query = new NameValueCollection
            {
                {"series_id", parameters.Id }
            };

            SetRealTimeParameters(parameters, query);

            XmlDocument xmlDocument = await Request(url, query).ConfigureAwait(false);

            Release result = Deserializer.Deserialize<Release>(xmlDocument.DocumentElement.InnerXml);

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
        public async Task<List<Series>> SearchSeries(SeriesSearchParameters parameters)
        {
            string url = $"series/search";

            NameValueCollection query = new NameValueCollection
            {
                {"search_text", parameters.SearchText },
                {"search_type", EnumDescription.GetDescription(parameters.SearchType) },
                {"limit", parameters.Limit.ToString() },
                {"offset", parameters.Offset.ToString() },
                {"sort_order", EnumDescription.GetDescription(parameters.SortOrder) }
            };

            if (parameters.OrderBy != SeriesSearchOrderBy.None)
            {
                query.Add("order_by", EnumDescription.GetDescription(parameters.OrderBy));
            }

            SetRealTimeParameters(parameters, query);

            if (parameters.FilterVariable != SeriesFilterVariable.None)
            {
                query.Add("filter_variable", EnumDescription.GetDescription(parameters.FilterVariable));
                query.Add("filter_value", parameters.FilterValue);
            }

            if (parameters.Tags != null && parameters.Tags.Any())
            {
                query.Add("tag_names", Separator.GetStringSeparatedBySemicolon(parameters.Tags));
            }

            if (parameters.ExcludeTags != null && parameters.ExcludeTags.Any())
            {
                query.Add("exclude_tag_names", Separator.GetStringSeparatedBySemicolon(parameters.ExcludeTags));
            }

            XmlDocument xmlDocument = await Request(url, query).ConfigureAwait(false);

            List<Series> result = new List<Series>();

            foreach (XmlNode xmlNode in xmlDocument.DocumentElement.ChildNodes)
            {
                Series series = Deserializer.Deserialize<Series>(xmlNode.OuterXml);

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
        public async Task<List<Tag>> SearchSeriesTags(TagSearchParameters parameters)
        {
            string url = $"series/search/tags";

            NameValueCollection query = new NameValueCollection
            {
                {"series_search_text", parameters.SeriesSearchText },
                {"limit", parameters.Limit.ToString() },
                {"offset", parameters.Offset.ToString() },
                {"order_by", EnumDescription.GetDescription(parameters.OrderBy) },
                {"sort_order", EnumDescription.GetDescription(parameters.SortOrder) }
            };

            SetRealTimeParameters(parameters, query);

            if (!string.IsNullOrEmpty(parameters.TagSearchText))
            {
                query.Add("tag_search_text", parameters.TagSearchText);
            }

            if (parameters.GroupId != TagGroupId.None)
            {
                query.Add("tag_group_id", EnumDescription.GetDescription(parameters.GroupId));
            }

            if (parameters.Tags != null && parameters.Tags.Any())
            {
                query.Add("tag_names", Separator.GetStringSeparatedBySemicolon(parameters.Tags));
            }

            XmlDocument xmlDocument = await Request(url, query).ConfigureAwait(false);

            List<Tag> result = new List<Tag>();

            foreach (XmlNode xmlNode in xmlDocument.DocumentElement.ChildNodes)
            {
                Tag tag = Deserializer.Deserialize<Tag>(xmlNode.OuterXml);

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
        public async Task<List<Tag>> SearchSeriesRelatedTags(RelatedTagSearchParameters parameters)
        {
            string url = $"series/search/related_tags";

            NameValueCollection query = new NameValueCollection
            {
                {"series_search_text", parameters.SeriesSearchText },
                {"tag_names", Separator.GetStringSeparatedBySemicolon(parameters.Tags) },
                {"limit", parameters.Limit.ToString() },
                {"offset", parameters.Offset.ToString() },
                {"order_by", EnumDescription.GetDescription(parameters.OrderBy) },
                {"sort_order", EnumDescription.GetDescription(parameters.SortOrder) }
            };

            SetRealTimeParameters(parameters, query);

            if (!string.IsNullOrEmpty(parameters.TagSearchText))
            {
                query.Add("tag_search_text", parameters.TagSearchText);
            }

            if (parameters.GroupId != TagGroupId.None)
            {
                query.Add("tag_group_id", EnumDescription.GetDescription(parameters.GroupId));
            }

            if (parameters.ExcludeTags != null && parameters.ExcludeTags.Any())
            {
                query.Add("exclude_tag_names", Separator.GetStringSeparatedBySemicolon(parameters.ExcludeTags));
            }

            XmlDocument xmlDocument = await Request(url, query).ConfigureAwait(false);

            List<Tag> result = new List<Tag>();

            foreach (XmlNode xmlNode in xmlDocument.DocumentElement.ChildNodes)
            {
                Tag tag = Deserializer.Deserialize<Tag>(xmlNode.OuterXml);

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
        public async Task<List<Tag>> GetSeriesTags(SeriesTagsParameters parameters)
        {
            string url = $"series/tags";

            NameValueCollection query = new NameValueCollection
            {
                {"series_id", parameters.SeriesId },
                {"order_by", EnumDescription.GetDescription(parameters.OrderBy) },
                {"sort_order", EnumDescription.GetDescription(parameters.SortOrder) }
            };

            SetRealTimeParameters(parameters, query);

            XmlDocument xmlDocument = await Request(url, query).ConfigureAwait(false);

            List<Tag> result = new List<Tag>();

            foreach (XmlNode xmlNode in xmlDocument.DocumentElement.ChildNodes)
            {
                Tag tag = Deserializer.Deserialize<Tag>(xmlNode.OuterXml);

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
        public async Task<List<Series>> GetSeriesUpdates(SeriesUpdatesParameters parameters)
        {
            string url = $"series/updates";

            NameValueCollection query = new NameValueCollection
            {
                {"limit", parameters.Limit.ToString() },
                {"offset", parameters.Offset.ToString() },
                {"filter_value", EnumDescription.GetDescription(parameters.FilterValue) }
            };

            SetRealTimeParameters(parameters, query);

            if (parameters.Start.HasValue)
            {
                query.Add("start_time", DateTimeFormat.FormatTime(parameters.Start.Value));
            }

            if (parameters.End.HasValue)
            {
                query.Add("end_time", DateTimeFormat.FormatTime(parameters.End.Value));
            }

            XmlDocument xmlDocument = await Request(url, query).ConfigureAwait(false);

            List<Series> result = new List<Series>();

            foreach (XmlNode xmlNode in xmlDocument.DocumentElement.ChildNodes)
            {
                Series series = Deserializer.Deserialize<Series>(xmlNode.OuterXml);

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
        public async Task<List<VintageDate>> GetSeriesVintageDates(VintageDateParameters parameters)
        {
            string url = $"series/vintagedates";

            NameValueCollection query = new NameValueCollection
            {
                {"series_id", parameters.SeriesId },
                {"limit", parameters.Limit.ToString() },
                {"offset", parameters.Offset.ToString() },
                {"sort_order", EnumDescription.GetDescription(parameters.SortOrder) }
            };

            SetRealTimeParameters(parameters, query);

            XmlDocument xmlDocument = await Request(url, query).ConfigureAwait(false);

            List<VintageDate> result = new List<VintageDate>();

            foreach (XmlNode xmlNode in xmlDocument.DocumentElement.ChildNodes)
            {
                VintageDate vintageDate = Deserializer.Deserialize<VintageDate>(xmlNode.OuterXml);

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
        public async Task<List<Source>> GetSources(SourcesParameters parameters)
        {
            string url = $"sources";

            NameValueCollection query = new NameValueCollection
            {
                {"limit", parameters.Limit.ToString() },
                {"offset", parameters.Offset.ToString() },
                {"order_by", EnumDescription.GetDescription(parameters.OrderBy) },
                {"sort_order", EnumDescription.GetDescription(parameters.SortOrder) }
            };

            SetRealTimeParameters(parameters, query);

            XmlDocument xmlDocument = await Request(url, query).ConfigureAwait(false);

            List<Source> result = new List<Source>();

            foreach (XmlNode xmlNode in xmlDocument.DocumentElement.ChildNodes)
            {
                Source source = Deserializer.Deserialize<Source>(xmlNode.OuterXml);

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
        public async Task<Source> GetSource(SourceParameters parameters)
        {
            string url = $"source";

            NameValueCollection query = new NameValueCollection
            {
                {"source_id", parameters.Id.ToString() },
            };

            SetRealTimeParameters(parameters, query);

            XmlDocument xmlDocument = await Request(url, query).ConfigureAwait(false);

            Source result = Deserializer.Deserialize<Source>(xmlDocument.DocumentElement.InnerXml);

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
        public async Task<List<Release>> GetSourceReleases(SourceReleaseParameters parameters)
        {
            string url = $"source/releases";

            NameValueCollection query = new NameValueCollection
            {
                {"source_id", parameters.Id.ToString() },
                {"limit", parameters.Limit.ToString() },
                {"offset", parameters.Offset.ToString() },
                {"order_by", EnumDescription.GetDescription(parameters.OrderBy) },
                {"sort_order", EnumDescription.GetDescription(parameters.SortOrder) }
            };

            SetRealTimeParameters(parameters, query);

            XmlDocument xmlDocument = await Request(url, query).ConfigureAwait(false);

            List<Release> result = new List<Release>();

            foreach (XmlNode xmlNode in xmlDocument.DocumentElement.ChildNodes)
            {
                Release release = Deserializer.Deserialize<Release>(xmlNode.OuterXml);

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
        public async Task<List<Tag>> GetTags(TagsParameters parameters)
        {
            string url = $"tags";

            NameValueCollection query = new NameValueCollection
            {
                {"limit", parameters.Limit.ToString() },
                {"offset", parameters.Offset.ToString() },
                {"order_by", EnumDescription.GetDescription(parameters.OrderBy) },
                {"sort_order", EnumDescription.GetDescription(parameters.SortOrder) }
            };

            SetRealTimeParameters(parameters, query);

            if (!string.IsNullOrEmpty(parameters.SearchText))
            {
                query.Add("search_text", parameters.SearchText);
            }

            if (parameters.GroupId != TagGroupId.None)
            {
                query.Add("tag_group_id", EnumDescription.GetDescription(parameters.GroupId));
            }

            if (parameters.Tags != null && parameters.Tags.Any())
            {
                query.Add("tag_names", Separator.GetStringSeparatedBySemicolon(parameters.Tags));
            }

            XmlDocument xmlDocument = await Request(url, query).ConfigureAwait(false);

            List<Tag> result = new List<Tag>();

            foreach (XmlNode xmlNode in xmlDocument.DocumentElement.ChildNodes)
            {
                Tag tag = Deserializer.Deserialize<Tag>(xmlNode.OuterXml);

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
        public async Task<List<Tag>> GetRelatedTags(RelatedTagsParameters parameters)
        {
            string url = $"related_tags";

            NameValueCollection query = new NameValueCollection
            {
                {"tag_names", Separator.GetStringSeparatedBySemicolon(parameters.Tags)},
                {"limit", parameters.Limit.ToString() },
                {"offset", parameters.Offset.ToString() },
                {"order_by", EnumDescription.GetDescription(parameters.OrderBy) },
                {"sort_order", EnumDescription.GetDescription(parameters.SortOrder) }
            };

            SetRealTimeParameters(parameters, query);

            if (!string.IsNullOrEmpty(parameters.SearchText))
            {
                query.Add("search_text", parameters.SearchText);
            }

            if (parameters.GroupId != TagGroupId.None)
            {
                query.Add("tag_group_id", EnumDescription.GetDescription(parameters.GroupId));
            }

            if (parameters.ExcludeTags != null && parameters.ExcludeTags.Any())
            {
                query.Add("exclude_tag_names", Separator.GetStringSeparatedBySemicolon(parameters.ExcludeTags));
            }

            XmlDocument xmlDocument = await Request(url, query).ConfigureAwait(false);

            List<Tag> result = new List<Tag>();

            foreach (XmlNode xmlNode in xmlDocument.DocumentElement.ChildNodes)
            {
                Tag tag = Deserializer.Deserialize<Tag>(xmlNode.OuterXml);

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
        public async Task<List<Series>> GetTagsSeries(TagsSeriesParameters parameters)
        {
            string url = $"tags/series";

            NameValueCollection query = new NameValueCollection
            {
                {"tag_names", Separator.GetStringSeparatedBySemicolon(parameters.Tags)},
                {"limit", parameters.Limit.ToString() },
                {"offset", parameters.Offset.ToString() },
                {"order_by", EnumDescription.GetDescription(parameters.OrderBy) },
                {"sort_order", EnumDescription.GetDescription(parameters.SortOrder) }
            };

            SetRealTimeParameters(parameters, query);

            if (parameters.ExcludeTags != null && parameters.ExcludeTags.Any())
            {
                query.Add("exclude_tag_names", Separator.GetStringSeparatedBySemicolon(parameters.ExcludeTags));
            }

            XmlDocument xmlDocument = await Request(url, query).ConfigureAwait(false);

            List<Series> result = new List<Series>();

            foreach (XmlNode xmlNode in xmlDocument.DocumentElement.ChildNodes)
            {
                Series series = Deserializer.Deserialize<Series>(xmlNode.OuterXml);

                result.Add(series);
            }

            return result;
        }

        #endregion Tags

        #region Others

        /// <summary>
        /// Disposes the web client object
        /// </summary>
        public void Dispose()
        {
            _webClient.Dispose();
        }

        /// <summary>
        /// All requests sends through this method
        /// </summary>
        /// <param name="url">The relative URL of a request</param>
        /// <param name="query">The URL query of a request</param>
        /// <returns></returns>
        private async Task<XmlDocument> Request(string url, NameValueCollection query)
        {
            _webClient.QueryString = query;

            _webClient.QueryString.Add("api_key", ApiKey);

            string response = await _webClient.DownloadStringTaskAsync(url).ConfigureAwait(false);

            XmlDocument xmlDocument = new XmlDocument();

            xmlDocument.LoadXml(response);

            return xmlDocument;
        }

        private void SetRealTimeParameters(RealTimeParameters parameters, NameValueCollection query)
        {
            if (parameters.RealTimeStart.HasValue)
            {
                query.Add("realtime_start", DateTimeFormat.FormatDate(parameters.RealTimeStart.Value));
            }

            if (parameters.RealTimeEnd.HasValue)
            {
                query.Add("realtime_end", DateTimeFormat.FormatDate(parameters.RealTimeEnd.Value));
            }
        }

        #endregion Others
    }
}
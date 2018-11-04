using Fred.Net.Types;
using Fred.Net.Types.Enums;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Fred.Net
{
    public class Client
    {
        #region Fields

        private const string _baseUrl = "https://api.stlouisfed.org/fred/";

        private readonly string _apiKey;

        private readonly WebClient _webClient;

        #endregion Fields

        public Client(string apiKey)
        {
            _apiKey = apiKey;

            _webClient = new WebClient();

            _webClient.BaseAddress = _baseUrl;
        }

        #region Properties

        public string ApiKey => _apiKey;

        #endregion Properties

        #region Methods

        public async Task<Category> GetCategory(int id)
        {
            string url = $"category";

            NameValueCollection query = new NameValueCollection
            {
                { "category_id", id.ToString() },
            };

            XmlDocument xmlDocument = await Request(url, query);

            Category result = Deserialize<Category>(xmlDocument.DocumentElement.InnerXml);

            return result;
        }

        public async Task<List<Category>> GetCategoryChildren(int id)
        {
            string url = $"category/children";

            NameValueCollection query = new NameValueCollection
            {
                { "category_id", id.ToString() },
            };

            XmlDocument xmlDocument = await Request(url, query);

            List<Category> categories = new List<Category>();

            foreach (XmlNode xmlNode in xmlDocument.DocumentElement.ChildNodes)
            {
                Category category = Deserialize<Category>(xmlNode.OuterXml);

                categories.Add(category);
            }

            return categories;
        }

        public async Task<List<Category>> GetCategoryRelated(int id)
        {
            string url = $"category/related";

            NameValueCollection query = new NameValueCollection
            {
                { "category_id", id.ToString() },
            };

            XmlDocument xmlDocument = await Request(url, query);

            List<Category> categories = new List<Category>();

            foreach (XmlNode xmlNode in xmlDocument.DocumentElement.ChildNodes)
            {
                Category category = Deserialize<Category>(xmlNode.OuterXml);

                categories.Add(category);
            }

            return categories;
        }

        public async Task<List<Series>> GetCategorySeries(int id, DateTime? realTimeStart = null, DateTime? realTimeEnd = null, int limit = 1000,
            int offset = 0, SeriesOrderBy orderBy = SeriesOrderBy.Id, SortOrder sortOrder = SortOrder.Ascending,
            SeriesFilterVariable filterVariable = SeriesFilterVariable.None, string filterValue = null, List<string> tags = null,
            List<string> excludeTags = null)
        {
            string url = $"category/series";

            string dateFormat = "yyyy-MM-dd";

            NameValueCollection query = new NameValueCollection
            {
                {"category_id", id.ToString() },
                {"realtime_start", realTimeStart.HasValue ? realTimeStart.Value.ToString(dateFormat) : DateTime.Now.ToString(dateFormat) },
                {"realtime_end", realTimeEnd.HasValue ? realTimeEnd.Value.ToString(dateFormat) : DateTime.Now.ToString(dateFormat) },
                {"limit", limit.ToString() },
                {"offset", offset.ToString() },
                {"order_by", Utility.GetDescription(orderBy) },
                {"sort_order", Utility.GetDescription(sortOrder) }
            };

            if (filterVariable != SeriesFilterVariable.None)
            {
                query.Add("filter_variable", Utility.GetDescription(filterVariable));
                query.Add("filter_value", filterValue);
            }

            if (tags != null && tags.Any())
            {
                query.Add("tag_names", Utility.GetTagNamesSeparatedBySemicolon(tags));
            }

            if (excludeTags != null && excludeTags.Any())
            {
                query.Add("exclude_tag_names", Utility.GetTagNamesSeparatedBySemicolon(excludeTags));
            }

            XmlDocument xmlDocument = await Request(url, query);

            List<Series> categorySeries = new List<Series>();

            foreach (XmlNode xmlNode in xmlDocument.DocumentElement.ChildNodes)
            {
                Series series = Deserialize<Series>(xmlNode.OuterXml);

                categorySeries.Add(series);
            }

            return categorySeries;
        }

        public async Task<List<Tag>> GetCategoryTags(int id, DateTime? realTimeStart = null, DateTime? realTimeEnd = null, int limit = 1000,
            int offset = 0, TagOrderBy orderBy = TagOrderBy.SeriesCount, SortOrder sortOrder = SortOrder.Ascending, string searchText = null,
            TagGroupId tagGroupId = TagGroupId.None, List<string> tags = null)
        {
            string url = $"category/tags";

            string dateFormat = "yyyy-MM-dd";

            NameValueCollection query = new NameValueCollection
            {
                {"category_id", id.ToString() },
                {"realtime_start", realTimeStart.HasValue ? realTimeStart.Value.ToString(dateFormat) : DateTime.Now.ToString(dateFormat) },
                {"realtime_end", realTimeEnd.HasValue ? realTimeEnd.Value.ToString(dateFormat) : DateTime.Now.ToString(dateFormat) },
                {"limit", limit.ToString() },
                {"offset", offset.ToString() },
                {"order_by", Utility.GetDescription(orderBy) },
                {"sort_order", Utility.GetDescription(sortOrder) }
            };

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
                query.Add("tag_names", Utility.GetTagNamesSeparatedBySemicolon(tags));
            }

            XmlDocument xmlDocument = await Request(url, query);

            List<Tag> categoryTags = new List<Tag>();

            foreach (XmlNode xmlNode in xmlDocument.DocumentElement.ChildNodes)
            {
                Tag tag = Deserialize<Tag>(xmlNode.OuterXml);

                categoryTags.Add(tag);
            }

            return categoryTags;
        }

        public async Task<List<Tag>> GetCategoryRelatedTags(int id, List<string> tags, DateTime? realTimeStart = null,
            DateTime? realTimeEnd = null, int limit = 1000, int offset = 0, TagOrderBy orderBy = TagOrderBy.SeriesCount,
            SortOrder sortOrder = SortOrder.Ascending, string searchText = null, TagGroupId tagGroupId = TagGroupId.None,
            List<string> excludeTags = null)
        {
            string url = $"category/related_tags";

            string dateFormat = "yyyy-MM-dd";

            NameValueCollection query = new NameValueCollection
            {
                {"category_id", id.ToString() },
                {"tag_names", Utility.GetTagNamesSeparatedBySemicolon(tags)},
                {"realtime_start", realTimeStart.HasValue ? realTimeStart.Value.ToString(dateFormat) : DateTime.Now.ToString(dateFormat) },
                {"realtime_end", realTimeEnd.HasValue ? realTimeEnd.Value.ToString(dateFormat) : DateTime.Now.ToString(dateFormat) },
                {"limit", limit.ToString() },
                {"offset", offset.ToString() },
                {"order_by", Utility.GetDescription(orderBy) },
                {"sort_order", Utility.GetDescription(sortOrder) }
            };

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
                query.Add("exclude_tag_names", Utility.GetTagNamesSeparatedBySemicolon(excludeTags));
            }

            XmlDocument xmlDocument = await Request(url, query);

            List<Tag> categoryTags = new List<Tag>();

            foreach (XmlNode xmlNode in xmlDocument.DocumentElement.ChildNodes)
            {
                Tag tag = Deserialize<Tag>(xmlNode.OuterXml);

                categoryTags.Add(tag);
            }

            return categoryTags;
        }

        private async Task<XmlDocument> Request(string url, NameValueCollection query)
        {
            _webClient.QueryString = query;

            _webClient.QueryString.Add("api_key", _apiKey);

            string response = await _webClient.DownloadStringTaskAsync(url);

            XmlDocument xmlDocument = new XmlDocument();

            xmlDocument.LoadXml(response);

            return xmlDocument;
        }

        private T Deserialize<T>(string xml)
        {
            StringReader reader = new StringReader(xml);

            XmlSerializer serializer = new XmlSerializer(typeof(T));

            T result = (T)serializer.Deserialize(reader);

            return result;
        }

        #endregion Methods
    }
}
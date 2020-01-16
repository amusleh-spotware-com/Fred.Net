using Fred.Net.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Threading.Tasks;
using Fred.Net.Parameters;

namespace Fred.Net.Tests
{
    [TestClass()]
    public class ClientTests
    {
        private string _apiKey = "";

        [TestMethod()]
        public void ClientTest()
        {
            Client client = new Client(_apiKey);

            Assert.AreNotEqual(client, null);

            Assert.AreEqual(client.ApiKey, _apiKey);
        }

        [TestMethod()]
        public async Task GetCategoryTest()
        {
            Client client = new Client(_apiKey);

            Category result = await client.GetCategory(125);

            Assert.AreNotEqual(null, result);

            Assert.AreEqual("Trade Balance", result.Name);
        }

        [TestMethod()]
        public async Task GetCategoryChildrenTest()
        {
            Client client = new Client(_apiKey);

            var parameters = new CategoryParameters
            {
                Id = 13
            };

            var result = await client.GetCategoryChildren(parameters);

            Assert.AreNotEqual(null, result);

            Assert.IsTrue(result.Count > 0);
        }

        [TestMethod()]
        public async Task GetCategoryRelatedTest()
        {
            Client client = new Client(_apiKey);

            var parameters = new CategoryParameters
            {
                Id = 32073
            };

            var result = await client.GetCategoryRelated(parameters);

            Assert.AreNotEqual(null, result);

            Assert.IsTrue(result.Count > 0);
        }

        [TestMethod()]
        public async Task GetCategorySeriesTest()
        {
            Client client = new Client(_apiKey);
            
            var parameters = new CategorySeriesParameters
            {
                Id = 125
            };

            var result = await client.GetCategorySeries(parameters);

            Assert.AreNotEqual(null, result);

            Assert.IsTrue(result.Count > 0);
        }

        [TestMethod()]
        public async Task GetCategoryTagsTest()
        {
            Client client = new Client(_apiKey);

            var parameters = new TagParameters
            {
                Id = 125
            };

            var result = await client.GetCategoryTags(parameters);

            Assert.AreNotEqual(null, result);

            Assert.IsTrue(result.Count > 0);
        }

        [TestMethod()]
        public async Task GetCategoryRelatedTagsTest()
        {
            Client client = new Client(_apiKey);

            var parameters = new RelatedTagParameters
            {
                Id = 125,
                Tags = new List<string> { "services", "quarterly" }
            };

            var result = await client.GetCategoryRelatedTags(parameters);

            Assert.AreNotEqual(null, result);

            Assert.IsTrue(result.Count > 0);
        }

        [TestMethod()]
        public async Task GetReleasesTest()
        {
            Client client = new Client(_apiKey);

            var parameters = new ReleasesParameters();

            var result = await client.GetReleases(parameters);

            Assert.AreNotEqual(null, result);

            Assert.IsTrue(result.Count > 0);
        }

        [TestMethod()]
        public async Task GetReleasesDatesTest()
        {
            Client client = new Client(_apiKey);

            var parameters = new ReleasesDatesParameters();

            var result = await client.GetReleasesDates(parameters);

            Assert.AreNotEqual(null, result);

            Assert.IsTrue(result.Count > 0);
        }

        [TestMethod()]
        public async Task GetReleaseTest()
        {
            Client client = new Client(_apiKey);

            var parameters = new ReleaseParameters
            {
                Id = 53
            };

            var result = await client.GetRelease(parameters);

            Assert.AreNotEqual(null, result);

            Assert.AreEqual(result.Name, "Gross Domestic Product");
        }

        [TestMethod()]
        public async Task GetReleaseDatesTest()
        {
            Client client = new Client(_apiKey);

            var parameters = new ReleaseDatesParameters
            {
                Id = 82
            };

            var result = await client.GetReleaseDates(parameters);

            Assert.AreNotEqual(null, result);

            Assert.IsTrue(result.Count > 0);
        }

        [TestMethod()]
        public async Task GetReleaseSeriesTest()
        {
            Client client = new Client(_apiKey);

            var parameters = new ReleaseSeriesParameters
            {
                Id = 51
            };

            var result = await client.GetReleaseSeries(parameters);

            Assert.AreNotEqual(null, result);

            Assert.IsTrue(result.Count > 0);
        }

        [TestMethod()]
        public async Task GetReleaseSourcesTest()
        {
            Client client = new Client(_apiKey);

            var parameters = new ReleaseSourcesParameters
            {
                Id = 51
            };

            var result = await client.GetReleaseSources(parameters);

            Assert.AreNotEqual(null, result);

            Assert.IsTrue(result.Count > 0);
        }

        [TestMethod()]
        public async Task GetReleaseTagsTest()
        {
            Client client = new Client(_apiKey);

            var parameters = new TagParameters
            {
                Id = 86
            };

            var result = await client.GetReleaseTags(parameters);

            Assert.AreNotEqual(null, result);

            Assert.IsTrue(result.Count > 0);
        }

        [TestMethod()]
        public async Task GetReleaseRelatedTagsTest()
        {
            Client client = new Client(_apiKey);

            var parameters = new RelatedTagParameters
            {
                Id = 86,
                Tags = new List<string> { "sa", "foreign" }
            };

            var result = await client.GetReleaseRelatedTags(parameters);

            Assert.AreNotEqual(null, result);

            Assert.IsTrue(result.Count > 0);
        }

        [TestMethod()]
        public async Task GetReleaseTablesTest()
        {
            Client client = new Client(_apiKey);

            var parameters = new ElementParameters
            {
                Id = 53,
            };

            var result = await client.GetReleaseTables(parameters);

            Assert.AreNotEqual(null, result);

            Assert.IsTrue(result.Count > 0);
        }

        [TestMethod()]
        public async Task GetSeriesTest()
        {
            Client client = new Client(_apiKey);

            var parameters = new SeriesParameters
            {
                Id = "GNPCA",
            };

            var result = await client.GetSeries(parameters);

            Assert.AreNotEqual(null, result);

            Assert.AreEqual(result.Title, "Real Gross National Product");
            Assert.AreEqual(result.Id, "GNPCA");
        }

        [TestMethod()]
        public async Task GetSeriesCategoriesTest()
        {
            Client client = new Client(_apiKey);

            var parameters = new SeriesCategoriesParameters
            {
                Id = "EXJPUS",
            };

            var result = await client.GetSeriesCategories(parameters);

            Assert.AreNotEqual(null, result);

            Assert.IsTrue(result.Count > 0);
        }

        [TestMethod()]
        public async Task GetSeriesObservationsTest()
        {
            Client client = new Client(_apiKey);

            var parameters = new ObservationParameters
            {
                Id = "GNPCA",
            };

            var result = await client.GetSeriesObservations(parameters);

            Assert.AreNotEqual(null, result);

            Assert.IsTrue(result.Count > 0);
        }

        [TestMethod()]
        public async Task GetSeriesReleaseTest()
        {
            Client client = new Client(_apiKey);

            var parameters = new SeriesReleaseParameters
            {
                Id = "IRA",
            };

            var result = await client.GetSeriesRelease(parameters);

            Assert.AreNotEqual(null, result);

            Assert.AreEqual(result.Name, "H.6 Money Stock Measures");
        }

        [TestMethod()]
        public async Task SearchSeriesTest()
        {
            Client client = new Client(_apiKey);

            var parameters = new SeriesSearchParameters
            {
                SearchText = "monetary service index",
            };

            var result = await client.SearchSeries(parameters);

            Assert.AreNotEqual(null, result);

            Assert.IsTrue(result.Count > 0);
        }

        [TestMethod()]
        public async Task SearchSeriesTagsTest()
        {
            Client client = new Client(_apiKey);

            var parameters = new TagSearchParameters
            {
                SeriesSearchText = "monetary service index",
            };

            var result = await client.SearchSeriesTags(parameters);

            Assert.AreNotEqual(null, result);

            Assert.IsTrue(result.Count > 0);
        }

        [TestMethod()]
        public async Task SearchSeriesRelatedTagsTest()
        {
            Client client = new Client(_apiKey);

            var parameters = new RelatedTagSearchParameters
            {
                SeriesSearchText = "mortgage rate",
                Tags = new List<string> { "30-year", "frb" }
            };

            var result = await client.SearchSeriesRelatedTags(parameters);

            Assert.AreNotEqual(null, result);

            Assert.IsTrue(result.Count > 0);
        }

        [TestMethod()]
        public async Task GetSeriesTagsTest()
        {
            Client client = new Client(_apiKey);

            var parameters = new SeriesTagsParameters
            {
                SeriesId = "STLFSI",
            };

            var result = await client.GetSeriesTags(parameters);

            Assert.AreNotEqual(null, result);

            Assert.IsTrue(result.Count > 0);
        }

        [TestMethod()]
        public async Task GetSeriesUpdatesTest()
        {
            Client client = new Client(_apiKey);

            var parameters = new SeriesUpdatesParameters
            {
            };

            var result = await client.GetSeriesUpdates(parameters);

            Assert.AreNotEqual(null, result);

            Assert.IsTrue(result.Count > 0);
        }

        [TestMethod()]
        public async Task GetSeriesVintageDatesTest()
        {
            Client client = new Client(_apiKey);

            var parameters = new VintageDateParameters
            {
                SeriesId = "GNPCA"
            };

            var result = await client.GetSeriesVintageDates(parameters);

            Assert.AreNotEqual(null, result);

            Assert.IsTrue(result.Count > 0);
        }

        [TestMethod()]
        public async Task GetSourcesTest()
        {
            Client client = new Client(_apiKey);

            var parameters = new SourcesParameters
            {
            };

            var result = await client.GetSources(parameters);

            Assert.AreNotEqual(null, result);

            Assert.IsTrue(result.Count > 0);
        }

        [TestMethod()]
        public async Task GetSourceTest()
        {
            Client client = new Client(_apiKey);

            var parameters = new SourceParameters
            {
                Id = 1
            };

            var result = await client.GetSource(parameters);

            Assert.AreNotEqual(null, result);

            Assert.AreEqual(result.Name, "Board of Governors of the Federal Reserve System (US)");
        }

        [TestMethod()]
        public async Task GetSourceReleasesTest()
        {
            Client client = new Client(_apiKey);

            var parameters = new SourceReleaseParameters
            {
                Id = 1
            };

            var result = await client.GetSourceReleases(parameters);

            Assert.AreNotEqual(null, result);

            Assert.IsTrue(result.Count > 0);
        }

        [TestMethod()]
        public async Task GetTagsTest()
        {
            Client client = new Client(_apiKey);

            var parameters = new TagsParameters
            {
            };

            var result = await client.GetTags(parameters);

            Assert.AreNotEqual(null, result);

            Assert.IsTrue(result.Count > 0);
        }

        [TestMethod()]
        public async Task GetRelatedTagsTest()
        {
            Client client = new Client(_apiKey);

            var parameters = new RelatedTagsParameters
            {
                Tags = new List<string> { "monetary aggregates", "weekly" }
            };

            var result = await client.GetRelatedTags(parameters);

            Assert.AreNotEqual(null, result);

            Assert.IsTrue(result.Count > 0);
        }

        [TestMethod()]
        public async Task GetTagsSeriesTest()
        {
            Client client = new Client(_apiKey);

            var parameters = new TagsSeriesParameters
            {
                Tags = new List<string> { "slovenia", "food", "oecd" }
            };

            var result = await client.GetTagsSeries(parameters);

            Assert.AreNotEqual(null, result);

            Assert.IsTrue(result.Count > 0);
        }

        [TestMethod()]
        public void DisposeTest()
        {
            Client client = new Client(_apiKey);

            client.Dispose();
        }
    }
}
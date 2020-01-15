using Fred.Net.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Threading.Tasks;

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

            var result = await client.GetCategoryChildren(13);

            Assert.AreNotEqual(null, result);

            Assert.IsTrue(result.Count > 0);
        }

        [TestMethod()]
        public async Task GetCategoryRelatedTest()
        {
            Client client = new Client(_apiKey);

            var result = await client.GetCategoryRelated(32073);

            Assert.AreNotEqual(null, result);

            Assert.IsTrue(result.Count > 0);
        }

        [TestMethod()]
        public async Task GetCategorySeriesTest()
        {
            Client client = new Client(_apiKey);

            var result = await client.GetCategorySeries(125);

            Assert.AreNotEqual(null, result);

            Assert.IsTrue(result.Count > 0);
        }

        [TestMethod()]
        public async Task GetCategoryTagsTest()
        {
            Client client = new Client(_apiKey);

            var result = await client.GetCategoryTags(125);

            Assert.AreNotEqual(null, result);

            Assert.IsTrue(result.Count > 0);
        }

        [TestMethod()]
        public async Task GetCategoryRelatedTagsTest()
        {
            Client client = new Client(_apiKey);

            var result = await client.GetCategoryRelatedTags(125, new List<string> { "services", "quarterly" });

            Assert.AreNotEqual(null, result);

            Assert.IsTrue(result.Count > 0);
        }

        [TestMethod()]
        public async Task GetReleasesTest()
        {
            Client client = new Client(_apiKey);

            var result = await client.GetReleases();

            Assert.AreNotEqual(null, result);

            Assert.IsTrue(result.Count > 0);
        }

        [TestMethod()]
        public async Task GetReleasesDatesTest()
        {
            Client client = new Client(_apiKey);

            var result = await client.GetReleasesDates();

            Assert.AreNotEqual(null, result);

            Assert.IsTrue(result.Count > 0);
        }

        [TestMethod()]
        public async Task GetReleaseTest()
        {
            Client client = new Client(_apiKey);

            var result = await client.GetRelease(53);

            Assert.AreNotEqual(null, result);

            Assert.AreEqual(result.Name, "Gross Domestic Product");
        }

        [TestMethod()]
        public async Task GetReleaseDatesTest()
        {
            Client client = new Client(_apiKey);

            var result = await client.GetReleaseDates(82);

            Assert.AreNotEqual(null, result);

            Assert.IsTrue(result.Count > 0);
        }

        [TestMethod()]
        public async Task GetReleaseSeriesTest()
        {
            Client client = new Client(_apiKey);

            var result = await client.GetReleaseSeries(51);

            Assert.AreNotEqual(null, result);

            Assert.IsTrue(result.Count > 0);
        }

        [TestMethod()]
        public async Task GetReleaseSourcesTest()
        {
            Client client = new Client(_apiKey);

            var result = await client.GetReleaseSources(51);

            Assert.AreNotEqual(null, result);

            Assert.IsTrue(result.Count > 0);
        }

        [TestMethod()]
        public async Task GetReleaseTagsTest()
        {
            Client client = new Client(_apiKey);

            var result = await client.GetReleaseTags(86);

            Assert.AreNotEqual(null, result);

            Assert.IsTrue(result.Count > 0);
        }

        [TestMethod()]
        public async Task GetReleaseRelatedTagsTest()
        {
            Client client = new Client(_apiKey);

            var result = await client.GetReleaseRelatedTags(86, new List<string> { "sa", "foreign" });

            Assert.AreNotEqual(null, result);

            Assert.IsTrue(result.Count > 0);
        }

        [TestMethod()]
        public async Task GetReleaseTablesTest()
        {
            Client client = new Client(_apiKey);

            var result = await client.GetReleaseTables(53);

            Assert.AreNotEqual(null, result);

            Assert.IsTrue(result.Count > 0);
        }

        [TestMethod()]
        public async Task GetSeriesTest()
        {
            Client client = new Client(_apiKey);

            var result = await client.GetSeries("GNPCA");

            Assert.AreNotEqual(null, result);

            Assert.AreEqual(result.Title, "Real Gross National Product");
            Assert.AreEqual(result.Id, "GNPCA");
        }

        [TestMethod()]
        public async Task GetSeriesCategoriesTest()
        {
            Client client = new Client(_apiKey);

            var result = await client.GetSeriesCategories("EXJPUS");

            Assert.AreNotEqual(null, result);

            Assert.IsTrue(result.Count > 0);
        }

        [TestMethod()]
        public async Task GetSeriesObservationsTest()
        {
            Client client = new Client(_apiKey);

            var result = await client.GetSeriesObservations("GNPCA");

            Assert.AreNotEqual(null, result);

            Assert.IsTrue(result.Count > 0);
        }

        [TestMethod()]
        public async Task GetSeriesReleaseTest()
        {
            Client client = new Client(_apiKey);

            var result = await client.GetSeriesRelease("IRA");

            Assert.AreNotEqual(null, result);

            Assert.AreEqual(result.Name, "H.6 Money Stock Measures");
        }

        [TestMethod()]
        public async Task SearchSeriesTest()
        {
            Client client = new Client(_apiKey);

            var result = await client.SearchSeries("monetary service index");

            Assert.AreNotEqual(null, result);

            Assert.IsTrue(result.Count > 0);
        }

        [TestMethod()]
        public async Task SearchSeriesTagsTest()
        {
            Client client = new Client(_apiKey);

            var result = await client.SearchSeriesTags("monetary service index");

            Assert.AreNotEqual(null, result);

            Assert.IsTrue(result.Count > 0);
        }

        [TestMethod()]
        public async Task SearchSeriesRelatedTagsTest()
        {
            Client client = new Client(_apiKey);

            var result = await client.SearchSeriesRelatedTags("mortgage rate", new List<string> { "30-year", "frb" });

            Assert.AreNotEqual(null, result);

            Assert.IsTrue(result.Count > 0);
        }

        [TestMethod()]
        public async Task GetSeriesTagsTest()
        {
            Client client = new Client(_apiKey);

            var result = await client.GetSeriesTags("STLFSI");

            Assert.AreNotEqual(null, result);

            Assert.IsTrue(result.Count > 0);
        }

        [TestMethod()]
        public async Task GetSeriesUpdatesTest()
        {
            Client client = new Client(_apiKey);

            var result = await client.GetSeriesUpdates();

            Assert.AreNotEqual(null, result);

            Assert.IsTrue(result.Count > 0);
        }

        [TestMethod()]
        public async Task GetSeriesVintageDatesTest()
        {
            Client client = new Client(_apiKey);

            var result = await client.GetSeriesVintageDates("GNPCA");

            Assert.AreNotEqual(null, result);

            Assert.IsTrue(result.Count > 0);
        }

        [TestMethod()]
        public async Task GetSourcesTest()
        {
            Client client = new Client(_apiKey);

            var result = await client.GetSources();

            Assert.AreNotEqual(null, result);

            Assert.IsTrue(result.Count > 0);
        }

        [TestMethod()]
        public async Task GetSourceTest()
        {
            Client client = new Client(_apiKey);

            var result = await client.GetSource(1);

            Assert.AreNotEqual(null, result);

            Assert.AreEqual(result.Name, "Board of Governors of the Federal Reserve System (US)");
        }

        [TestMethod()]
        public async Task GetSourceReleasesTest()
        {
            Client client = new Client(_apiKey);

            var result = await client.GetSourceReleases(1);

            Assert.AreNotEqual(null, result);

            Assert.IsTrue(result.Count > 0);
        }

        [TestMethod()]
        public async Task GetTagsTest()
        {
            Client client = new Client(_apiKey);

            var result = await client.GetTags();

            Assert.AreNotEqual(null, result);

            Assert.IsTrue(result.Count > 0);
        }

        [TestMethod()]
        public async Task GetRelatedTagsTest()
        {
            Client client = new Client(_apiKey);

            var result = await client.GetRelatedTags(new List<string> { "monetary aggregates", "weekly" });

            Assert.AreNotEqual(null, result);

            Assert.IsTrue(result.Count > 0);
        }

        [TestMethod()]
        public async Task GetTagsSeriesTest()
        {
            Client client = new Client(_apiKey);

            var result = await client.GetTagsSeries(new List<string> { "slovenia", "food", "oecd" });

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
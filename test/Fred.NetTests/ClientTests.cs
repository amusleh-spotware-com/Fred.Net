using Fred.Net.Types;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Fred.Net.Tests
{
    [TestClass()]
    public class ClientTests
    {
        [TestMethod()]
        public async Task GetCategoryTest()
        {
            Client client = new Client("eaa1d5cae31ccc11b9e5a0e807ffb618");

            Category category = await client.GetCategory(13);

            Assert.AreNotEqual(null, category);

            Assert.AreEqual("Trade Balance", category.Name);
        }
    }
}
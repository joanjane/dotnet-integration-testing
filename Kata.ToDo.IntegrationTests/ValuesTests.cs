using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kata.ToDo.IntegrationTests
{
    [TestClass]
    public class ValuesTests
    {
        [TestMethod]
        public async Task TestGetValues()
        {
            // Arrange
            var factory = new CustomWebApplicationFactory();
            var httpClient = factory.CreateClient();

            // Act
            var response = await httpClient.GetAsync("/api/values");

            // Assert
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var values = JsonConvert.DeserializeObject<IEnumerable<string>>(content);
            Assert.AreEqual(2, values.Count());
        }
    }
}

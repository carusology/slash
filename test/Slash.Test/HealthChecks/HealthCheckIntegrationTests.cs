using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace Slash.HealthChecks {

    public sealed class HealthCheckIntegrationTests : IClassFixture<TestServerFixture> {

        private TestServerFixture fixture { get; }

        public HealthCheckIntegrationTests(TestServerFixture fixture) {
            this.fixture = fixture;
        }

        [Fact]
        public async Task BasicHealthCheck_IsAlwaysHealthy() {

            // Arrange

            var client = this.fixture.CreateClient();

            // Act

            var response = await client.GetAsync("health");
            var content = await response.Content.ReadAsStringAsync();

            // Assert

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("Healthy", content);
        }

    }

}

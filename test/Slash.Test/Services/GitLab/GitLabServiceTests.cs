using System;
using System.Collections.Immutable;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Moq;
using Peddler;
using RichardSzalay.MockHttp;
using Xunit;

namespace Slash.Services.GitLab {

    public sealed class GitLabServiceTests {

        private IGenerator<PingResult> pingResultGenerator { get;}

        public GitLabServiceTests() {
            pingResultGenerator = new PingResultGenerator();
        }

        [Fact]
        public void Constructor_NullHttpClient_ThrowsArgumentNullException() {

            // Arrange

            var configuration = Options.Create(new GitLabConfiguration());

            // Act

            var exception = Record.Exception(
                () => new GitLabService(null, configuration)
            );

            // Assert

            Assert.IsType<ArgumentNullException>(exception);
        }

        [Fact]
        public void Constructor_NullConfiguration_ThrowsArgumentNullException() {

            // Arrange

            var httpClient = new MockHttpMessageHandler().ToHttpClient();

            // Act

            var exception = Record.Exception(
                () => new GitLabService(httpClient, null)
            );

            // Assert

            Assert.IsType<ArgumentNullException>(exception);
        }

        [Theory]
        [InlineData(HttpStatusCode.OK)]
        [InlineData(HttpStatusCode.Accepted)]
        public async Task PingAsync_2XXResultIsConsideredHealthy(HttpStatusCode statusCode) {

            // Arrange

            var configuration = new GitLabConfiguration { BaseUrl = new Uri("https://www.foo.bar.com") };

            var mockClient = new MockHttpMessageHandler();
            var request =
                mockClient
                    .When(HttpMethod.Get, String.Empty)
                    .Respond(statusCode);

            var service = CreateService(mockClient, configuration);

            // Act

            var result = await service.PingAsync();

            // Assert

            Assert.NotNull(result);
            Assert.True(result.IsSuccessful);
        }

        private static IGitLabService CreateService(
            MockHttpMessageHandler mockHandler,
            GitLabConfiguration configuration) {

            if (mockHandler == null) {
                throw new ArgumentNullException(nameof(mockHandler));
            } else if (configuration == null) {
                throw new ArgumentNullException(nameof(configuration));
            }

            return new GitLabService(
                mockHandler.ToHttpClient(),
                Options.Create<GitLabConfiguration>(configuration)
            );
        }

    }

}
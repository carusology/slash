using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Xunit;

namespace Slash.Services.GitLab {

    public sealed class ServiceCollectionExtensionsTests {

        [Fact]
        public void AddGitLabProxy_NoConfigurationLambda_ThrowsOnNullServiceCollection() {

            // Act

            var exception = Record.Exception(
                () => ServiceCollectionExtensions.AddGitLabProxy(null)
            );

            // Assert

            Assert.IsType<ArgumentNullException>(exception);
        }

        [Fact]
        public void AddGitLabProxy_NoConfigurationLambda_UsesDefaultConfiguration() {

            // Arrange

            var services = new ServiceCollection();

            // Act

            services.AddGitLabProxy();

            // Assert

            var provider = services.BuildServiceProvider();
            var options = provider.GetRequiredService<IOptions<GitLabConfiguration>>();

            Assert.Equal(new GitLabConfiguration().BaseUrl, options?.Value?.BaseUrl);
        }

        [Fact]
        public void AddGitLabProxy_WithConfigurationLambda_ThrowsOnNullServiceCollection() {

            // Arrange

            Action<GitLabConfiguration> configure = config => {};

            // Act

            var exception = Record.Exception(
                () => ServiceCollectionExtensions.AddGitLabProxy(null, configure)
            );

            // Assert

            Assert.IsType<ArgumentNullException>(exception);
        }

        [Fact]
        public void AddGitLabProxy_WithConfigurationLambda_ThrowsOnNullConfigureAction() {

            // Arrange

            var services = new ServiceCollection();

            // Act

            var exception = Record.Exception(
                () => ServiceCollectionExtensions.AddGitLabProxy(services, null)
            );

            // Assert

            Assert.IsType<ArgumentNullException>(exception);
        }

        [Fact]
        public void AddGitLabProxy_WithConfigurationLambda_UsesCustomConfiguration() {

            // Arrange

            var expectedBaseUrl = new Uri("https://www.foo-bar-bizz-buzz.com", UriKind.Absolute);
            var services = new ServiceCollection();

            // Act

            services.AddGitLabProxy(config => config.BaseUrl = expectedBaseUrl);

            // Assert

            var provider = services.BuildServiceProvider();
            var options = provider.GetRequiredService<IOptions<GitLabConfiguration>>();

            Assert.Equal(expectedBaseUrl, options?.Value?.BaseUrl);
        }

    }

}
using System;
using Peddler;
using Xunit;

namespace Slash.Services.GitLab {

    public sealed class GitLabConfigurationVerifierTests {

        private static IGenerator<String> nameGenerator { get; }

        static GitLabConfigurationVerifierTests() {
            // We always ignore "name", so generate various name values to ensure that,
            // no matter what we provide, the value of "name" has no bearing on any test.
            nameGenerator = new MaybeDefaultGenerator<String>(new StringGenerator(1, 20));
        }

        [Fact]
        public void PostConfigure_ThrowsArgumentNullException_OnNullConfiguration() {

            // Arrange

            var name = nameGenerator.Next();

            var verifier = new GitLabConfigurationVerifier();

            // Act

            var exception = Record.Exception(
                () => verifier.PostConfigure(name, null)
            );

            // Assert

            Assert.IsType<ArgumentNullException>(exception);
        }

        [Theory]
        [InlineData(null, "<null>")]
        [InlineData("relative/url", "'relative/url'")]
        [InlineData("/some/relative/path", "'/some/relative/path'")]
        [InlineData("", "''")]
        public void PostConfigure_ThrowsArgumentException_OnNullBaseUrlInConfiguration(
            string baseUrl,
            string expectedInvalidValue) {

            // Arrange

            var name = nameGenerator.Next();

            var configuration = new GitLabConfiguration {
                BaseUrl =
                    baseUrl == null
                        ? null
                        : new Uri(baseUrl, UriKind.RelativeOrAbsolute)
            };

            var verifier = new GitLabConfigurationVerifier();

            // Act

            var exception = Record.Exception(
                () => verifier.PostConfigure(name, configuration)
            );

            // Assert

            Assert.IsType<ArgumentException>(exception);
            Assert.Contains(expectedInvalidValue, exception.Message);
        }

        [Fact]
        public void PostConfigure_DefaultValuesAreValid() {

            // Arrange

            var name = nameGenerator.Next();
            var configuration = new GitLabConfiguration();

            var verifier = new GitLabConfigurationVerifier();

            // Act

            var exception = Record.Exception(
                () => verifier.PostConfigure(name, configuration)
            );

            // Assert

            Assert.Null(exception);
        }

    }

}
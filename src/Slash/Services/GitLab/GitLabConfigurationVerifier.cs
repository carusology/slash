using System;
using Microsoft.Extensions.Options;

namespace Slash.Services.GitLab {

    /// <summary>
    ///   Verifies an instance of <see cref="GitLabConfiguration" /> is configured correctly.
    /// </summary>
    public sealed class GitLabConfigurationVerifier : IPostConfigureOptions<GitLabConfiguration> {

        /// <summary>
        ///   Checks the <see cref="GitLabConfiguration" /> and verifies its values
        ///   are valid. If they are not valid it will throw an exception, causing
        ///   the web service to fail to come online.
        /// </summary>
        /// <param name="name">
        ///   Used if a named-instance of <see cref="GitLabConfiguration" /> was registered
        ///   with the configuration system. For this web service it is always ignored.
        /// </param>
        /// <param name="configuration">
        ///   The configured implementation of <see cref="GitLabConfiguration" />.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///   Thrown when <paramref name="configuration" /> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///   Thrown when one of more of the values on <paramref name="configuration" /> is invalid.
        /// </exception>
        public void PostConfigure(string name, GitLabConfiguration configuration) {
            if (configuration == null) {
                throw new ArgumentNullException(nameof(configuration));
            }

            if (configuration.BaseUrl == null || !configuration.BaseUrl.IsAbsoluteUri) {
                var invalidValue =
                    configuration.BaseUrl == null
                        ? "<null>"
                        : $"'{configuration.BaseUrl}'";

                throw new ArgumentException(
                    $"The GitLab '{nameof(GitLabConfiguration.BaseUrl)}' must be a " +
                    $"non-null, absolute URL. However, {invalidValue} was provided",
                    nameof(configuration)
                );
            }
        }

    }

}
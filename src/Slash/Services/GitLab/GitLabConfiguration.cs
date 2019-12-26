using System;

namespace Slash.Services.GitLab {

    /// <summary>
    ///   When the GitLab proxy is enabled, this configures how the slash
    ///   commands from Slack will behave.
    /// </summary>
    public sealed class GitLabConfiguration {

        /// <summary>
        ///   Base URL of where the GitLab instance lives that is being proxied.
        /// </summary>
        /// <remarks>
        ///   <p>If the GitLab proxy is enabled, this setting is REQUIRED.</p>
        ///   <p>The URL must be an absolute URL.</p>
        ///   <p>The default is "https://www.gitlab.com".</p>
        /// </remarks>
        public Uri BaseUrl { get; set; } = new Uri("https://www.gitlab.com", UriKind.Absolute);

    }

}